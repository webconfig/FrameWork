using System;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using Google.Protobuf;

/// <summary>
/// kcp客户端
/// </summary>
public class KCPClient
{
    private static readonly DateTime utc_time = new DateTime(1970, 1, 1);
    //===
    private Socket socket;
    //==
    private Thread recvThraed;
    public KCP m_Kcp;
    private NetBase parent;
    //==数据
    public int state = 0;
    private bool m_NeedUpdateFlag;
    private UInt32 m_NextUpdateTime;

    private SwitchQueue<byte[]> m_RecvQueue;

    private IPAddress srvAddr;
    private IPEndPoint srvIpEnd;
    private EndPoint srvEnd;

    const int DEF_SIZE = 1024 * 10;
    private byte[] recvBuf;
    int result = 0; //接收的字节数  

    /// <summary>
    /// 最后收到数据的时刻。
    /// </summary>
    internal float m_LastRecvTimestamp;
    /// <summary>
    /// 最后发送数据的时刻。
    /// </summary>
    internal float m_LastSendTimestamp = 0;

    private IPEndPoint server_addr;

    public event Action<bool> ConnectResultEvent;
    public event Action DisConnectEvent;

    private byte[] heart_data;



    private List<byte[]> RecvBuffer_Add;
    private List<byte> RecvBuffer;
    private object buff_obj = new object();


    public KCPClient(NetBase _parent)
    {
        RecvBuffer_Add = new List<byte[]>();
        RecvBuffer = new List<byte>();

        parent = _parent;

        //pkgCSHeartBeatReq heart = new pkgCSHeartBeatReq();
        //heart.Ok = 1;
        //var mem = new MemoryStream();
        //heart.WriteTo(mem);
        //mem.Position = 0;
        //byte[] datas = mem.ToArray();
        //mem.Dispose();
        //heart_data = KCPClient.GetData(Main.game.player_1.id, 0, (int)ProtocolCmd.CsheartBeatReq, datas);
    }

    /// <summary>
    /// 连接
    /// </summary>
    public void Connect(string host, int port, uint index)
    {
        m_RecvQueue = new SwitchQueue<byte[]>(128);

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); //创建socket  
        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);
        srvAddr = IPAddress.Parse(host); //服务器IP地址  
        srvIpEnd = new IPEndPoint(srvAddr, port); //服务器地址 
        srvEnd = (EndPoint)srvIpEnd; //必须要经过这个类型转换,EndPoint是一个抽象类

        state = 1;
        init_kcp(index);
        RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
        if (ConnectResultEvent != null)
        {
            ConnectResultEvent(true);
        }
    }

    /// <summary>
    /// 初始化kcp
    /// </summary>
    /// <param name="conv"></param>
    void init_kcp(UInt32 conv)
    {
        m_Kcp = new KCP(conv, null);
        m_Kcp.SetOutput((byte[] buf, int size, object user) =>
        {
            //UnityEngine.Main.Debug("send:" + size);
            try
            {
                //UnityEngine.Debug.Log("kcp send:" + size);
                socket.SendTo(buf, 0, size, SocketFlags.None, srvIpEnd);
                if (index == 2)
                {
                    index = 1;
                }
            }
            catch (Exception ex)
            {
                //Main.Debug("send errror:" + ex.ToString());
                NetError();
            }
        });

        m_Kcp.NoDelay(1, 10, 2, 1);
        m_Kcp.WndSize(128, 128);
    }

    #region 接受数据
    public void StartRrcv()
    {
        //Main.Debug("StartRrcv");
        index = 0;
        recvThraed = new Thread(OnRecievedData);
        recvThraed.Start();
    }
    private IPEndPoint RemoteIpEndPoint;
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
                        if (result > recvBuf.Length)
                        {
                            //Main.Debug("接受到的数据大于缓冲区了：");
                        }
                        else
                        {
                            byte[] receiveBytes = new byte[result];
                            //UnityEngine.Main.Debug("OnRecievedData-->"+ result);
                            Buffer.BlockCopy(recvBuf, 0, receiveBytes, 0, result);
                            //UnityEngine.Main.Debug("OnRecievedData-->2");
                            m_RecvQueue.Push(receiveBytes);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch (SocketException ee)
            {
#if DEBUG
                if (ee.NativeErrorCode == 10054)
                {
                    UnityEngine.Debug.Log("无法连接到远程服务器");
                }
                else
                {
                    UnityEngine.Debug.Log("OnRecievedData-->" + ee.ToString());
                }
#endif
                NetError();
                return;
            }
        }
    }
    private void Recv(byte[] buf)
    {
        //UInt32 DataSize = BitConverter.ToUInt32(buf, 0);//包长度
        //UInt32 id = BitConverter.ToUInt32(buf, 4);//id
        //UInt32 protocol = BitConverter.ToUInt32(buf, 8);//id
        //Int32 cmd = BitConverter.ToInt32(buf, 12);//id
        //UInt32 body_key = BitConverter.ToUInt32(buf, 16);//id
        //UnityEngine.Main.Debug("1111DataSize:" + DataSize + ",id:" + id + ",protocol:" + protocol + ",cmd:" + cmd + ",body_key:" + body_key + ",MsgSize:" + MsgSize);
        lock (buff_obj)
        {
            RecvBuffer_Add.Add(buf);
        }
    }

    Int32 DataSize = 0, id = 0, protocol = 0, body_key = 0, MsgSize = 0, cmd = 0;
    byte[] int_data = new byte[4];
    /// <summary>
    /// 处理数据
    /// </summary>
    private void DealData()
    {
        if (RecvBuffer_Add.Count > 0)
        {
            lock (buff_obj)
            {
                for (int i = 0; i < RecvBuffer_Add.Count; i++)
                {
                    for (int j = 0; j < RecvBuffer_Add[i].Length; j++)
                    {
                        RecvBuffer.Add(RecvBuffer_Add[i][j]);
                    }
                }
                RecvBuffer_Add.Clear();
            }
        }
        if (RecvBuffer.Count >= 20)
        {
            for (int i = 0; i < 10; i++)
            {
                if (RecvBuffer.Count >= 20)
                {
                    DataSize = 0; id = 0; protocol = 0; body_key = 0; MsgSize = 0; cmd = 0;
                    BytesToInt(RecvBuffer, 0, ref DataSize);//包长度
                    if (DataSize <= RecvBuffer.Count)//包长度小于于接受数据长度
                    {
                        BytesToInt(RecvBuffer, 4, ref id);
                        BytesToInt(RecvBuffer, 8, ref protocol);
                        BytesToInt(RecvBuffer, 12, ref cmd);//命令
                        BytesToInt(RecvBuffer, 16, ref body_key);//

                        MsgSize = DataSize - 20;//消息体长度
                                                //UnityEngine.Main.Debug("2222DataSize:" + DataSize + ",id:" + id + ",protocol:" + protocol + ",cmd:" + cmd + ",body_key:" + body_key + ",MsgSize:" + MsgSize);
                        if (MsgSize > 0)
                        {
                            byte[] msg_datas = new byte[MsgSize];
                            RecvBuffer.CopyTo(20, msg_datas, 0, (int)MsgSize);
                            parent.Handle(id, cmd, msg_datas);
                        }
                        else
                        {
                            //Main.Debug("2222DataSize:" + DataSize + ",id:" + id + ",protocol:" + protocol + ",cmd:" + cmd + ",body_key:" + body_key + ",MsgSize:" + MsgSize);
                            //Main.DebugError("数据错误：" + DataSize + "--" + RecvBuffer.Count);
                        }
                        RecvBuffer.RemoveRange(0, DataSize);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void BytesToInt(List<byte> data, int offset, ref Int32 num)
    {
        for (int i = 0; i < 4; i++)
        {
            int_data[i] = data[offset + i];
        }
        num = BitConverter.ToInt32(int_data, 0);
    }

    void process_recv_queue()
    {
        m_RecvQueue.Switch();
        while (!m_RecvQueue.Empty())
        {
            if (m_Kcp == null)
            {//退出
                return;
            }
            var buf = m_RecvQueue.Pop();
            m_Kcp.Input(buf);
            m_NeedUpdateFlag = true;
            int size = m_Kcp.PeekSize();
            //UnityEngine.Main.Debug("size:" + size);
            for (; size > 0; size = m_Kcp.PeekSize())
            {
                var buffer = new byte[size];
                if (m_Kcp.Recv(buffer) > 0)
                {
                    Recv(buffer);
                }
            }
        }
    }
    #endregion

    UInt32 current;
    public void Update()
    {
        current = (UInt32)(Convert.ToInt64(DateTime.UtcNow.Subtract(utc_time).TotalMilliseconds) & 0xffffffff);
        if (_send)
        {
            m_LastSendTimestamp = UnityEngine.Time.time;
            _send = false;
        }
        if (state > 0)
        {
            //UnityEngine.Main.Debug("m_LastSendTimestamp:" + m_LastSendTimestamp+"---"+(UnityEngine.Time.time - m_LastSendTimestamp));
            if (m_LastSendTimestamp > 0 && UnityEngine.Time.time - m_LastSendTimestamp > parent.HeartTime)
            {//超过1秒没发生数据，发送心跳包
                m_LastSendTimestamp = UnityEngine.Time.time;
                parent.SendHeart();
            }
            //if (UnityEngine.Time.time - m_LastRecvTimestamp > 5)
            //{//超过5秒没收到数据，短线
            //    Main.Debug("超过5秒没收到数据，短线");
            //    NetError();
            //}
        }
        //Main.Debug("222222222222222");
        process_recv_queue();
        if (m_Kcp != null)
        {
            if (m_NeedUpdateFlag || current >= m_NextUpdateTime)
            {
                m_Kcp.Update(current);
                m_NextUpdateTime = m_Kcp.Check(current);
                m_NeedUpdateFlag = false;
            }
        }
        DealData();
    }

    #region 发送数据
    private bool _send = false;
    public void Send(UInt32 id, UInt32 protocol, Int32 cmd, byte[] msg)
    {
        //UnityEngine.Main.Debug("msg:" + msg.Length);
        uint body_key = Convert.ToUInt32(CRC32Cls.GetCRC32(msg));

        byte[] id_bytes = BitConverter.GetBytes(id);//id
        byte[] protocol_bytes = BitConverter.GetBytes(protocol);//protocol
        byte[] cmd_bytes = BitConverter.GetBytes(cmd);//cmd
        byte[] body_key_bytes = BitConverter.GetBytes(body_key);//cmd

        UInt32 total_length = (UInt32)(id_bytes.Length + protocol_bytes.Length + cmd_bytes.Length + body_key_bytes.Length + msg.Length + 4);
        //UnityEngine.Main.Debug("send3333");

        //消息体结构：消息体长度+消息体
        byte[] data = new byte[total_length];
        BitConverter.GetBytes(total_length).CopyTo(data, 0);
        id_bytes.CopyTo(data, 4);
        protocol_bytes.CopyTo(data, 8);
        cmd_bytes.CopyTo(data, 12);
        body_key_bytes.CopyTo(data, 16);
        msg.CopyTo(data, 20);
        //UnityEngine.Main.Debug("send44444");
        //socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(send_back), null);
        string str = "";
        for (int i = 0; i < data.Length; i++)
        {
            str += data[i].ToString();
        }
        //UnityEngine.Main.Debug("发送数据:" + str);
        //UnityEngine.Main.Debug("数据长度:" + data.Length);

        m_Kcp.Send(data);
        _send = true;
        m_NeedUpdateFlag = true;

    }
    public void Send(byte[] datas)
    {
        //UnityEngine.Main.Debug("发送心跳包");
        m_Kcp.Send(datas);
        _send = true;
        m_NeedUpdateFlag = true;

    }
    public static byte[] GetData(UInt32 id, UInt32 protocol, Int32 cmd, byte[] msg)
    {

        uint body_key = Convert.ToUInt32(CRC32Cls.GetCRC32(msg));

        byte[] id_bytes = BitConverter.GetBytes(id);//id
        byte[] protocol_bytes = BitConverter.GetBytes(protocol);//protocol
        byte[] cmd_bytes = BitConverter.GetBytes(cmd);//cmd
        byte[] body_key_bytes = BitConverter.GetBytes(body_key);//cmd

        UInt32 total_length = (UInt32)(id_bytes.Length + protocol_bytes.Length + cmd_bytes.Length + body_key_bytes.Length + msg.Length + 4);

        //消息体结构：消息体长度+消息体
        byte[] data = new byte[total_length];
        BitConverter.GetBytes(total_length).CopyTo(data, 0);
        id_bytes.CopyTo(data, 4);
        protocol_bytes.CopyTo(data, 8);
        cmd_bytes.CopyTo(data, 12);
        body_key_bytes.CopyTo(data, 16);
        msg.CopyTo(data, 20);

        return data;

    }
    #endregion

    /// <summary>
    /// 网络异常关闭
    /// </summary>
    public void NetError()
    {
        if (DisConnectEvent != null)
        {
            DisConnectEvent();
            DisConnectEvent = null;
        }
    }

    /// <summary>
    /// 外包关闭
    /// </summary>
    public void End()
    {
        //Main.Debug("==End==");
        if (state != -100)
        {
            state = -100;
            if (socket != null)
            {
                socket.Close();
                socket = null;
            }
            if (recvThraed != null)
            {
                recvThraed.Abort();
                recvThraed = null;
            }
            if (m_Kcp != null)
            {
                m_Kcp.Release();
                m_Kcp = null;
            }
            m_NeedUpdateFlag = false;
            recvBuf = null;
            ConnectResultEvent = null;
            DisConnectEvent = null;
            server_addr = null;
        }
    }
}
