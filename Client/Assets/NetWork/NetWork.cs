using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 网络模块
/// </summary>
public class NetBase
{
    public TcpClient tcp;
    public KCPClient kcp = null;
    public UdpClient udp = null;

    public string ip;
    public int port;
    public int udp_port;

    public float DisConnTime = 3, HeartTime = 1;

    public NetWorkType type;
    /// <summary>
    /// 状态 -1：网络断开  1：连接
    /// </summary>
    public int state;

    public Action DisConnectEvent;
    public Action HeartEvent;
    public Action<bool> ConnectResultEvent;

    public object has_send_obj = new object();
    public bool has_send = false, has_recv = false,can_send_heart=false,can_check_dis=false;
    public double heart_time=0, dis_time=0;
    private DateTime last_send, Last_recv;
    private object msg_obj = new object();
    public List<MsgData> msgs = new List<MsgData>();
    public List<MsgData> msgs_all = new List<MsgData>();
    public List<MsgData> msg_pool = new List<MsgData>();
    private object msg_pool_obj = new object();

    public NetBase(string _ip, int _port, float _HeartTime, float _DisConnTime, NetWorkType _type)
    {
        ip = _ip;
        port = _port;
        type = _type;
        DisConnTime = _DisConnTime*1000;
        HeartTime = _HeartTime*1000;
        lock (msg_obj) { }
        switch (type)
        {
            case NetWorkType.Kcp:
                kcp = new KCPClient(this);
                kcp.DisConnectEvent += DisConn;
                kcp.ConnectResultEvent += ConnectBack;
                break;
            case NetWorkType.Tcp:
                tcp = new TcpClient(this);
                tcp.DisConnectEvent += DisConn;
                tcp.ConnectResultEvent += ConnectBack;
                break;
        }
    }

    #region 连接
    public void Conn(uint conv)
    {
        Clear();
        switch (type)
        {
            case NetWorkType.Kcp:
                kcp.Connect(ip, port, conv);
                break;
            case NetWorkType.Tcp:
                tcp.ConnectAsync(ip, port);
                break;
        }
    }
    public void ClearConnEvent()
    {
        DisConnectEvent = null;
        HeartEvent = null;
        ConnectResultEvent = null;
    }
    private void ConnectBack(bool t)
    {
        if (ConnectResultEvent != null)
        {
            ConnectResultEvent(t);
        }
    }
    public void DisConn()
    {
        Debug.LogError("==dis conn==");
        state = -1;
        if (DisConnectEvent != null)
        {
            DisConnectEvent();
        }
    }
    #endregion

    #region udp
    public void RunUdp(int _udp_port)
    {
        udp_port = _udp_port;
        udp = new UdpClient(this);
    }
    public void ConnUdp()
    {
        udp.Connect(ip, udp_port);
    }
    public void StartRecvUdp()
    {
        udp.StartRrcv();
    }
    public void UdpSend(UInt32 id, UInt32 protocol, Int32 cmd, byte[] datas)
    {
        udp.Send(id, protocol, cmd, datas);
    }
    public int UdpSend(byte[] datas)
    {
        return udp.Send(datas);
    }
    #endregion


    private void Clear()
    {
        lock (msg_obj)
        {
            msgs.Clear();
            has_recv = false;
        }
        msgs_all.Clear();
        lock (has_send_obj)
        {
            has_send = false;
        }
        can_send_heart = false;
        can_check_dis = false;
    }

    public void HasSend()
    {
        lock (has_send_obj)
        {
            has_send = true;
        }
    }
    public void HasRecv(MsgData obj)
    {
        lock (msg_obj)
        {
            msgs.Add(obj);
        }
    }
    /// <summary>
    /// 处理数据
    /// </summary>
    private void DealData()
    {
        lock (msg_obj)
        {
            if (msgs.Count > 0)
            {
                msgs_all.AddRange(msgs);
                msgs.Clear();
                has_recv = true;
                //Debug.LogError("有信息");
            }
        }
        if (msgs_all.Count > 0)
        {
            for (int i = 0; i < msgs_all.Count; i++)
            {
                //Debug.LogError("处理信息："+ msgs_all[i].cmd);
                Handle(msgs_all[i].id, msgs_all[i].cmd, msgs_all[i].datas);
            }
            BackMsg(msgs_all);
            msgs_all.Clear();
        }
    }
    public void Update()
    {
        if (state > 0)
        {
            DealData();
            //Debug.Log("断线判断时间：" + dis_time + ",心跳时间：" + heart_time);
            if (has_send) { has_send = false; last_send = DateTime.Now; can_send_heart = true;}
            if (has_recv) { has_recv = false; Last_recv = DateTime.Now; can_check_dis = true; }
            if (can_send_heart)
            {//没间隔1秒发一个心跳包
                heart_time= (DateTime.Now - last_send).TotalMilliseconds;
                if (heart_time >= HeartTime)
                {
                    SendHeart();
                }
            }
            if (can_check_dis)
            {//断线判断
                dis_time = (DateTime.Now - Last_recv).TotalMilliseconds;
                if (dis_time >= DisConnTime)
                {
                    Debug.LogError("=======居然间隔" + dis_time + "秒都没有数据，断线了:" + heart_time);
                    can_check_dis = false;
                    DisConn();
                }
            }
        }
    }

    #region pool
    public MsgData GetMsg()
    {
        lock (msg_pool_obj)
        {
            MsgData msg = null;
            if (msg_pool.Count > 0)
            {
                msg = msg_pool[0];
                msg_pool.RemoveAt(0);
            }
            else
            {
                msg = new MsgData();
            }
            return msg;
        }
    }
    public void BackMsg(List<MsgData> items)
    {
        lock (msg_pool_obj)
        {
            msg_pool.AddRange(items);
        }
    }
    public void ClearMsg()
    {
        lock (msg_pool_obj)
        {
            if (msg_pool != null)
            {
                msg_pool.Clear();
                msg_pool = null;
            }
        }
        lock (msg_obj)
        {
            msgs = null;
            msgs_all = null;
        }
    }
    #endregion

    public void SendHeart()
    {
        if (state < 0) { Debug.Log("断线后，不发送心跳"); }
        if (HeartEvent != null)
        {
            HeartEvent();
        }
    }
    /// <summary>
    /// 发送数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id">玩家id</param>
    /// <param name="protocol">0</param>
    /// <param name="cmd">命令</param>
    /// <param name="t">协议</param>
    public void Send(UInt32 id, UInt32 protocol, Int32 cmd, byte[] datas)
    {
        if (state < 0) { Debug.Log("断线后，不发送数据"); }
        //else { Debug.Log("发送命令：" + cmd); }
        switch (type)
        {
            case NetWorkType.Kcp:
                kcp.Send(id, protocol, cmd, datas);
                break;
            case NetWorkType.Tcp:
                tcp.Send(id, protocol, cmd, datas);
                break;
        }

    }

    public void Send(byte[] datas)
    {
        if (state < 0) { Debug.Log("断线后，不发送心跳"); }
        //else { Debug.Log("发送数据：" + datas.Length); }
        switch (type)
        {
            case NetWorkType.Kcp:
                kcp.Send(datas);
                break;
            case NetWorkType.Tcp:
                tcp.Send(datas);
                break;
        }
    }

    public byte[] GetData(UInt32 id, UInt32 protocol, Int32 cmd, byte[] msg)
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

    #region 处理数据
    public void StartRecv()
    {
        switch (type)
        {
            case NetWorkType.Kcp:
                kcp.StartRrcv();
                break;
            case NetWorkType.Tcp:
                tcp.StartRrcv();
                break;
        }
    }
    //public delegate void PacketHandlerFunc(NetBase client, byte[] datas, Int32 player_id);
    private Dictionary<Int32, Action<NetBase, System.Byte[], System.Int32>> handlers = new Dictionary<int, Action<NetBase, System.Byte[], System.Int32>>();
    public void AddHandle(Int32 cmd, Action<NetBase, System.Byte[], System.Int32> func)
    {
        if (handlers.ContainsKey(cmd))
        {
            handlers[cmd] = func;
        }
        else
        {
            handlers.Add(cmd, func);
        }
    }
    public void RemoveHandle(Int32 cmd)
    {
        if (handlers.ContainsKey(cmd))
        {
            handlers.Remove(cmd);
        }
    }
    public virtual void Handle(Int32 player_id, Int32 command, byte[] datas)
    {
        Action<NetBase, System.Byte[], System.Int32> handler;
        if (!handlers.TryGetValue(command, out handler))
        {
            Debug.Log("未知命令：" + command);
            return;
        }

        try
        {
            //Debug.Log("处理命令：" + command);
            handler(this, datas, player_id);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(
                "处理服务器消息异常: " + ex.Message + Environment.NewLine +
                ex.StackTrace + Environment.NewLine +
                "Packet: " + Environment.NewLine +
                command.ToString()
            );
        }
    }
    #endregion

    public void End()
    {
        Debug.Log("====end=====");
        ClearMsg();
        handlers = null;
        switch (type)
        {
            case NetWorkType.Kcp:
                if (kcp != null)
                {
                    kcp.End();
                    kcp = null;
                }
                break;
            case NetWorkType.Tcp:
                if (tcp != null)
                {
                    tcp.End();
                    tcp = null;
                }
                break;
        }
    }
}
public enum NetWorkType
{
    Tcp,
    Kcp
}

