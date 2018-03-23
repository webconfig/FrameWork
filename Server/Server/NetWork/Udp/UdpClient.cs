using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;

/// <summary>
/// kcp客户端
/// </summary>
public class UdpClient
{
    private Socket socket;
    //==
    private Thread recvThraed;
    private NetBase parent;
    //==数据
    public int state = 0;
    private IPEndPoint srvIpEnd;
    private EndPoint srvEnd;

    const int DEF_SIZE = 1024;
    private byte[] recvBuf;
    int result = 0; //接收的字节数  


    public UdpClient(NetBase _parent)
    {
        parent = _parent;
    }

    /// <summary>
    /// 连接
    /// </summary>
    public void Connect(string host, int port)
    {
        IPAddress[] address = Dns.GetHostAddresses(host);
        if (address[0].AddressFamily == AddressFamily.InterNetworkV6)
        {
            socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
        }
        else
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);
        srvIpEnd = new IPEndPoint(IPAddress.Parse(host), port); //服务器地址 
        srvEnd = (EndPoint)srvIpEnd; //必须要经过这个类型转换,EndPoint是一个抽象类
        Debug.LogWarning("==udp完成连接："+ host+","+ port);
    }
    private void CloseNetwork()
    {
        state = -1;
        try
        {
            if (socket != null)
            {
                try
                {
                    socket.Close();
                }
                catch { }
                socket = null;
            }
        }
        catch { }
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
    #region 接受数据
    public void StartRrcv()
    {
        index = 0;
        recvThraed = new Thread(OnRecievedData);
        recvThraed.Start();
    }
    Int32 DataSize = 0,  MsgSize = 0;
    private int index = 0;
    void OnRecievedData()
    {
        while (state >= 0)
        {
            try
            {
                if (index == 0)
                {
                    recvBuf = new byte[DEF_SIZE]; //接收缓冲区大小  
                    index = 2;
                }
                else if (index == 1)
                {
                    result = socket.ReceiveFrom(recvBuf, ref srvEnd);
                    if (result > 0)
                    {
                        if (result <20)
                        {
                            Debug.LogError("奇怪的数据：" + result);
                        }
                        else
                        {
                            MsgData msgData = parent.GetMsg();
                            DataSize = BitConverter.ToInt32(recvBuf, 0);//包长度
                            msgData.id = BitConverter.ToInt32(recvBuf, 4);
                            msgData.cmd = BitConverter.ToInt32(recvBuf, 12);
                            MsgSize = DataSize - 20;//消息体长度
                            msgData.datas = new byte[MsgSize];
                            Buffer.BlockCopy(recvBuf, 20, msgData.datas, 0, MsgSize);
                            parent.HasRecv(msgData);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch { }
        }
    }
    #endregion

    #region 发送数据
    public void Send(UInt32 id, UInt32 protocol, Int32 cmd, byte[] msg)
    {
        uint body_key = Convert.ToUInt32(CRC32Cls.GetCRC32(msg));
        byte[] id_bytes = BitConverter.GetBytes(id);//id
        byte[] protocol_bytes = BitConverter.GetBytes(protocol);//protocol
        byte[] cmd_bytes = BitConverter.GetBytes(cmd);//cmd
        byte[] body_key_bytes = BitConverter.GetBytes(body_key);//cmd

        UInt32 total_length = (UInt32)(id_bytes.Length + protocol_bytes.Length + cmd_bytes.Length + body_key_bytes.Length + msg.Length + 4);
        byte[] data = new byte[total_length];
        BitConverter.GetBytes(total_length).CopyTo(data, 0);
        id_bytes.CopyTo(data, 4);
        protocol_bytes.CopyTo(data, 8);
        cmd_bytes.CopyTo(data, 12);
        body_key_bytes.CopyTo(data, 16);
        msg.CopyTo(data, 20);
        try
        {
            socket.SendTo(data, 0, data.Length, SocketFlags.None, srvIpEnd);
            if (index == 2)
            {
                index = 1;
            }
            Debug.LogWarning("==发送Udp验证数据==");
        }
        catch
        {
        }
    }
    public int Send(byte[] datas)
    {
        try
        {
            //UnityEngine.Debug.Log("==发送Udp:"+ datas.Length);
           return  socket.SendTo(datas, 0, datas.Length, SocketFlags.None, srvIpEnd);
        }
        catch(Exception ex)
        {
            Debug.Log("发送错误：" + ex.ToString());
        }
        return 0;
    }
    #endregion

    /// <summary>
    /// 外包关闭
    /// </summary>
    public void End()
    {
        //Main.Debug("==End==");
        if (socket != null)
        {
            socket.Close();
            socket = null;

            if (recvThraed != null)
            {
                recvThraed.Abort();
                recvThraed = null;
            }
        }
    }
}
