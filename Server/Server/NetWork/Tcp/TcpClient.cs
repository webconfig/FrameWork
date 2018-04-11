using System;
using System.Net.Sockets;
using System.Threading;
using System.Net;

/// <summary>
/// tcp 客户端
/// </summary>
public class TcpClient
{
    private Socket socket;
    //==
    private Thread recvThraed;
    //==
    private NetBase parent;
    //==数据
    private int BufferSize = 1024;
    private byte[] buffer;
    public Action<bool> ConnectResultEvent;
    public Action DisConnectEvent;

    public TcpClient(NetBase _parent)
    {
        BufferSize = BufferSize * 5;
        this.buffer = new byte[BufferSize];
        parent = _parent;
        head_data = new byte[20]; 
    }

    #region 连接
    public void ConnectAsync(string host, int port)
    {
        CloseNetwork();
        try
        {
            Debug.Log("开始连接:" + host + "," + port);

            IPAddress[] address = Dns.GetHostAddresses(host);
            if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
            {
                //Debug.Log("Connect InterNetworkV6");
                socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            }
            else
            {
                //Debug.Log("Connect InterNetwork");
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            socket.NoDelay = true;
            socket.BeginConnect(host, port, new AsyncCallback(OnConnect), null);
        }
        catch
        {
            if (ConnectResultEvent != null)
            {
                ConnectResultEvent(false);
            }
            return;
        }
    }
    private void OnConnect(IAsyncResult result)
    {
        try
        {
            socket.EndConnect(result);
            parent.state = 10;
            StartRrcv();
            if (ConnectResultEvent != null)
            {
                ConnectResultEvent(true);
            }
        }
        catch (Exception ex)
        {
            parent.state = -1;
            Debug.Log("OnConnect Error:" + ex.ToString());
            if (ConnectResultEvent != null)
            {
                ConnectResultEvent(false);
            }
            return;
        }
    }
    private void CloseNetwork()
    {
        parent.state = -1;
        if (socket != null)
        {
            try
            {
                socket.Close();
            }
            catch { }
            socket = null;
        }
        try
        {
            if (recvThraed != null)
            {
                recvThraed.Abort();
                recvThraed = null;
            }
        }
        catch
        { }
    }
    #endregion

    #region 接收数据
    public void StartRrcv()
    {
        Debug.LogError("开始接受数据");
        recvThraed = new Thread(ProcessReceive);
        recvThraed.Start();
    }
    public void StartRrcv(Socket _socket)
    {
        Debug.LogError("开始接受数据");
        socket = _socket;
        recvThraed = new Thread(ProcessReceive);
        recvThraed.Start();
    }

    Int32 DataSize = 0, MsgSize = 0, bytesRead = 0;
    public byte[] head_data;
    public int head_recv_index = 0;
    public int head_recv_count = 0;
    /// <summary>
    /// 接收数据
    /// </summary>
    /// <param name="e"></param>
    private void ProcessReceive()
    {
        try
        {
            while (parent.state > 0)
            {
                //接受头
                head_recv_index = 0;
                head_recv_count = 0;
                while (head_recv_index < 20)
                {
                    head_recv_count = 20 - head_recv_count;
                    bytesRead = socket.Receive(buffer, head_recv_count, SocketFlags.None);
                    if (bytesRead <= 0)
                    {
                        Debug.Log("==接受头 等于0退出==" + bytesRead);
                        parent.DisConn();
                        return;
                    }
                    Buffer.BlockCopy(buffer, 0, head_data, head_recv_index, bytesRead);
                    head_recv_index += bytesRead;
                }


                //Debug.Log("==网络接收：" + bytesRead);

                MsgData msgData = parent.GetMsg();
                DataSize = BitConverter.ToInt32(head_data, 0);//包长度
                msgData.id = BitConverter.ToInt32(head_data, 4);
                msgData.cmd = BitConverter.ToInt32(head_data, 12);
                MsgSize = DataSize - 20;//消息体长度


                if (MsgSize < 0)
                {
                    Debug.LogError("==============接受数据后解析内容异常=========");
                    parent.DisConn();
                    return;
                }


                msgData.datas = new byte[MsgSize];
                msgData.datas_recv_index = 0;
                while (msgData.datas_recv_index < MsgSize)
                {//读取消息体
                    msgData.datas_recv_count = MsgSize - msgData.datas_recv_index;
                    //接受内容
                    bytesRead = socket.Receive(buffer, 0, msgData.datas_recv_count, SocketFlags.None);
                    if (bytesRead <= 0)
                    {
                        Debug.Log("==接受协议内容 等于0退出==" + bytesRead);
                        parent.DisConn();
                        return;
                    }
                    Buffer.BlockCopy(buffer, 0, msgData.datas, msgData.datas_recv_index, bytesRead);
                    msgData.datas_recv_index += bytesRead;
                }
                parent.HasRecv(msgData);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("==============接受数据异常退出："+ex.ToString());
        }
    }
    #endregion

    #region 发送  
    public void Send(UInt32 id, UInt32 protocol, Int32 cmd, byte[] msg)
    {
        if (parent.state < 0) { return; }
        uint body_key = Convert.ToUInt32(CRC32Cls.GetCRC32(msg));
        UInt32 total_length = (UInt32)(20 + msg.Length);
        byte[] data = new byte[total_length];
        //消息体结构：消息体长度+消息体
        BitConverter.GetBytes(total_length).CopyTo(data, 0);
        BitConverter.GetBytes(id).CopyTo(data, 4);
        BitConverter.GetBytes(protocol).CopyTo(data, 8);
        BitConverter.GetBytes(cmd).CopyTo(data, 12);
        BitConverter.GetBytes(body_key).CopyTo(data, 16);
        msg.CopyTo(data, 20);
        try
        {
            parent.HasSend();
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(send_back), null);

            ////=====测试======
            //string str = "";
            //for (int i = 0; i < data.Length; i++)
            //{
            //    str += Convert.ToString(data[i], 16);
            //}
            //Debug.Log("发送命令：" + cmd);
        }
        catch (Exception ex)
        {
            Debug.LogError("发送数据错误:" + ex.ToString());
            parent.DisConn();
        }

    }
    public void Send(byte[] datas)
    {
        if (parent.state < 0) { return; }
        try
        {
            parent.HasSend();
            socket.BeginSend(datas, 0, datas.Length, SocketFlags.None, new AsyncCallback(send_back), null);
        }
        catch (Exception ex)
        {
            Debug.LogError("发送数据错误:" + ex.ToString());
            parent.DisConn();
        }

    }
    private void send_back(IAsyncResult ar)
    {
        if (parent.state < 0) { return; }
        try
        {
            int bytesSent = socket.EndSend(ar);
        }
        catch
        {

        }
    }
    #endregion

    public void End()
    {
        if (parent != null)
        {
            //==
            head_data = null;
            parent = null;
            buffer = null;
            ConnectResultEvent = null;
            DisConnectEvent = null;
            //==
            if (socket != null)
            {
                try
                {
                    socket.Close();
                }
                catch (Exception)
                {
                }
                socket = null;
            }
            //==
            if (recvThraed != null)
            {
                recvThraed.Abort();
                recvThraed = null;
            }
        }
    }
}

public class BuffItem
{
    public byte[] datas;
    public bool Over;
    public int total_length;
}
public class MsgData
{
    public int id;
    public int cmd;
    public byte[] datas;
    public int datas_recv_index = 0;
    public int datas_recv_count = 0;
}

