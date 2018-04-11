using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
public class NetWorkManager
{
    private Game game;
    private Dictionary<Int32, Action<NetBase, System.Byte[], System.Int32>> handlers = new Dictionary<int, Action<NetBase, System.Byte[], System.Int32>>();
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
    /// 加入房价
    /// </summary>
    /// <param name="client"></param>
    /// <param name="datas"></param>
    /// <param name="player_id"></param>
    private void JoinRoomRep(NetBase client, byte[] datas, Int32 player_id)
    {
        
    }
    /// <summary>
    /// 加载进度
    /// </summary>
    /// <param name="client"></param>
    /// <param name="datas"></param>
    private void ProgressRate(NetBase client, byte[] datas, Int32 player_id)
    {
       
    }
    /// <summary>
    /// 游戏配置
    /// </summary>
    /// <param name="client"></param>
    /// <param name="datas"></param>
    private void GameConfig(NetBase client, byte[] datas, Int32 player_id)
    {

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
            netBase.StartRecv(tcpClient.Client, handlers);
        }
        catch (Exception ex)
        {
            Log.Error("BeginAcceptTcpClient Error:" + ex);
        }
        NetworkListener.BeginAcceptTcpClient(new AsyncCallback(this.BeginAcceptTcpClient), (object)null);
    }
    #endregion
}

