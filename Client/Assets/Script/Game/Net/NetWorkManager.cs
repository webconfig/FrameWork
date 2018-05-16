using System;

public class NetWorkManager
{
    public int state = 0;
    #region 初始化
    public void Init(string ip, int port)
    {
        App.Instance.InitNetWork(ip, port, 1, 3, NetWorkType.Tcp);
        InitEvent();

        ////====心跳
        //pkgCSHeartBeatReq heart = new pkgCSHeartBeatReq();
        //heart.Ok = 1;
        //var mem = new MemoryStream();
        //heart.WriteTo(mem);
        //mem.Position = 0;
        //byte[] datas = mem.ToArray();
        //mem.Dispose();
        //heart_data = App.Instance.network.GetData(Main.game.player_1.id, 0, (int)ProtocolCmd.CsheartBeatReq, datas);
    }
    /// <summary>
    /// 添加服务器返回事件
    /// </summary>
    private void InitEvent()
    {
        App.Instance.network.AddHandle(1017, JoinRoomRep);
        App.Instance.network.AddHandle(1022, ProgressRate);
        App.Instance.network.AddHandle(1023, GameStart);
        App.Instance.network.AddHandle(1013, GameOver);
        App.Instance.network.AddHandle(1152, GameConfig);
        App.Instance.network.AddHandle(1102, TimeRecv);
        App.Instance.network.AddHandle(1011, Heart);
    }
    #endregion

    #region 连接
    /// <summary>
    /// 连接服务器
    /// </summary>
    public void Conn(uint conv)
    {
        App.Instance.network.ClearConnEvent();
        App.Instance.network.ConnectResultEvent = Session_ConnectResultEvent;
        App.Instance.network.DisConnectEvent = Session_DisConnectEvent;
        App.Instance.network.HeartEvent = SendHeart;
        App.Instance.network.Conn(conv);
    }

    /// <summary>
    /// 连接断开
    /// </summary>
    private void Session_DisConnectEvent()
    {
        if (state > 0)
        {
            //Main.DebugError("网络异常");
            state = -1;
            //net_work = Main.AddUpdate("net_work", Update);
        }
    }
    /// <summary>
    /// 连接成功
    /// </summary>
    /// <param name="t"></param>
    private void Session_ConnectResultEvent(bool t)
    {
        if (t)
        {
            ////加入房间
            //pkgCSJoinRoomReq request = new pkgCSJoinRoomReq();
            //request.Iroomid = Main.game.room_id;
            //request.UserId = (int)Main.game.player_1.id;
            //request.Token = Main.game.Token;

            //var mem = new MemoryStream();
            //request.WriteTo(mem);
            //mem.Position = 0;
            //byte[] datas = mem.ToArray();
            //mem.Dispose();

            //Main.Debug("==发送加入房间==");
            //App.Instance.network.Send(Main.game.player_1.id, 0, (int)ProtocolCmd.CsjoinRoomReq, datas);
            state = 2;
        }
        else
        {
            state = -1;
        }
        //net_work = Main.AddUpdate("net_work", Update);
    }
    #endregion

    public void Update()
    {
    //    if (state == -1)
    //    {
    //        Main.game.ShowNetError();
    //        Main.RemoveUpdate(net_work);
    //        state = -100;
    //    }

    //    if (state == 2)
    //    {//连接成功
    //        state = 3;
    //        Main.game.HideNetError();

    //        Main.RemoveUpdate(net_work);
    //    }
    }

    #region 服务器返回信息
    /// <summary>
    /// 加入房价
    /// </summary>
    /// <param name="client"></param>
    /// <param name="datas"></param>
    /// <param name="player_id"></param>
    private void JoinRoomRep(NetBase client, byte[] datas, Int32 player_id)
    {
        //pkgSCJoinRoomRep data = pkgSCJoinRoomRep.Parser.ParseFrom(datas);
        //Main.DebugWarning("服务器返回-加入房间->" + data.Ok + "," + player_id);
    }
    /// <summary>
    /// 加载进度
    /// </summary>
    /// <param name="client"></param>
    /// <param name="datas"></param>
    private void ProgressRate(NetBase client, byte[] datas, Int32 player_id)
    {
        //pkgSCProgressRateRep data = pkgSCProgressRateRep.Parser.ParseFrom(datas);

        //Main.DebugWarning("服务器返回-加载进度->" + data.Rate + "," + data.Userid);

        //if (data.Userid == Main.game.player_1.id)
        //{
        //    Main.game.player_1.state = data.Rate;
        //}
        //else if (data.Userid == Main.game.player_2.id)
        //{
        //    Main.game.player_2.state = data.Rate;
        //}
    }
    /// <summary>
    /// 游戏配置
    /// </summary>
    /// <param name="client"></param>
    /// <param name="datas"></param>
    private void GameConfig(NetBase client, byte[] datas, Int32 player_id)
    {
        //config_data = pkgSCGameConfigRep.Parser.ParseFrom(datas);
        //Main.DebugWarning("服务器返回->游戏配置：" + config_data.LifeTime);
        //Main.game.state = 6;
    }

    /// <summary>
    /// 游戏开始
    /// </summary>
    /// <param name="client"></param>
    /// <param name="datas"></param>
    private void GameStart(NetBase client, byte[] datas, Int32 player_id)
    {
        //Main.DebugWarning("服务器返回->游戏开始");
        //Main.game.state = 10;//游戏开始
    }

    public int game_result = 0;
    /// <summary>
    /// 结算
    /// </summary>
    /// <param name="client"></param>
    /// <param name="datas"></param>
    private void GameOver(NetBase client, byte[] datas, Int32 player_id)
    {
        //pkgSCARoundOverRep data = pkgSCARoundOverRep.Parser.ParseFrom(datas); //ProtoBuf.Serializer.Deserialize(typeof(pkgSCARoundOverRep), stream) as pkgSCARoundOverRep;
        //Main.DebugWarning("========结算：" + data.Ok);
        //game_result = data.Ok;
        //is_over = true;
        //switch (game_result)
        //{
        //    case 1:
        //        Main.game.state = 50;//你赢了
        //        break;
        //    case 2:
        //        Main.game.state = -50;//你输了
        //        break;
        //    case 3:
        //        Main.game.state = 60;//平局
        //        break;
        //}
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

    #region 功能
    /// <summary>
    /// 发送心跳
    /// </summary>
    private void SendHeart()
    {
        //if (state > 0)
        //{
        //    //Debug.Log("==发送心跳包==");
        //    App.Instance.network.Send(heart_data);
        //}
    }
    /// <summary>
    /// 发送进度
    /// </summary>
    /// <param name="_rate"></param>
    /// <param name="playe_id"></param>
    public void SendProgressRate(int _rate, uint playe_id)
    {
        //if (state > 0)
        //{
        //    rate.Rate = _rate;
        //    Main.Debug("SendProgressRate:" + _rate);
        //    App.Instance.network.Send(playe_id, 0, (int)ProtocolCmd.CsprogressRateReq, rate.ToByteArray());
        //}
    }
    #endregion

    

    public void Clear()
    {

    }
}

