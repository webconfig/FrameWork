using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
public class NetWorkManager
{
    private Game game;
    private Dictionary<Int32, Action<NetBase, System.Byte[], System.Int32>> handlers = new Dictionary<int, Action<NetBase, System.Byte[], System.Int32>>();

    private List<JoinClient> clients;
    private List<JoinClient> clients_add;
    private object clients_add_obj = new object();
    public void Init(Game _game)
    {
        game = _game;
        InitEvent();
        new Thread(new ThreadStart(NetworkStart)).Start();
    }

    #region 消息处理
    /// <summary>
    /// 添加服务器返回事件
    /// </summary>
    private void InitEvent()
    {
        AddHandle(1017, JoinRoomRep);
        AddHandle(1022, ProgressRate);
        AddHandle(1023, GameStart);
        AddHandle(1013, GameOver);
        AddHandle(1152, GameConfig);
        AddHandle(1102, TimeRecv);
        AddHandle(1011, Heart);
    }
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

    /// <summary>
    /// 加入房间
    /// </summary>
    /// <param name="client"></param>
    /// <param name="datas"></param>
    /// <param name="player_id"></param>
    private void JoinRoom(NetBase client, byte[] datas, Int32 player_id)
    {
        client.state = 2;
        game.JoinClient(1, client);
    }

    /// <summary>
    /// 游戏开始
    /// </summary>
    /// <param name="client"></param>
    /// <param name="datas"></param>
    private void GameStart(NetBase client, byte[] datas, Int32 player_id)
    {

    }

    public int game_result = 0;
    /// <summary>
    /// 结算
    /// </summary>
    /// <param name="client"></param>
    /// <param name="datas"></param>
    private void GameOver(NetBase client, byte[] datas, Int32 player_id)
    {

    }

    /// <summary>
    /// 心跳包
    /// </summary>
    /// <param name="client"></param>
    /// <param name="datas"></param>
    /// <param name="player_id"></param>
    private void Heart(NetBase client, byte[] datas, Int32 player_id)
    {
        //Main.Debug("==服务器返回心跳包==");
    }

    /// <summary>
    /// 接收时间
    /// </summary>
    /// <param name="client"></param>
    /// <param name="datas"></param>
    /// <param name="player_id"></param>
    private void TimeRecv(NetBase client, byte[] datas, Int32 player_id)
    {

    }
    #endregion

    #region 连接
    private TcpListener NetworkListener;

    private void NetworkStart()
    {
        try
        {
            NetworkListener = new TcpListener(new System.Net.IPEndPoint(0, 3333));
            NetworkListener.Start();
            Log.Info(string.Format("开始监听{0}", 3333));
            NetworkListener.BeginAcceptTcpClient(new AsyncCallback(BeginAcceptTcpClient), (object)null);
        }
        catch (Exception ex)
        {
            Log.Error("NetworkStart Error:" + ex);
        }
    }

    private void BeginAcceptTcpClient(IAsyncResult ar)
    {
        try
        {
            System.Net.Sockets.TcpClient tcpClient = NetworkListener.EndAcceptTcpClient(ar);
            Log.Info(string.Format("新链接:{0}", tcpClient.Client.RemoteEndPoint));
            NetBase netBase = new NetBase(1, 3, NetWorkType.Tcp);
            netBase.SetBuffer(100, 10);
            netBase.StartRecv(tcpClient.Client, handlers);

            JoinClient client = new JoinClient();
            client.net = netBase;
            client.time = DateTime.Now.Ticks;
            lock (clients_add_obj)
            {
                clients_add.Add(client);
            }
        }
        catch (Exception ex)
        {
            Log.Error("BeginAcceptTcpClient Error:" + ex);
        }
        NetworkListener.BeginAcceptTcpClient(new AsyncCallback(this.BeginAcceptTcpClient), (object)null);
    }
    #endregion

    private long run_time, check_time = 1000 * 1000;
    public void Update()
    {
        if (clients_add.Count > 0)
        {
            lock (clients_add_obj)
            {
                clients.AddRange(clients_add);
                clients_add.Clear();
            }
        }

        if (clients.Count > 0)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].net.state < 2)
                {//没验证成功,才在这里运行
                    run_time = DateTime.Now.Ticks - clients[i].time;
                    if (run_time >= check_time)
                    {//超过时间除掉

                        clients[i].net.End();
                        clients.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        clients[i].net.Update();
                    }
                }
                else
                {//认证成功，移除
                    clients.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}

public class JoinClient
{
    public NetBase net;
    public long time;
}

