using System.Collections.Generic;
using System;

public class Game
{
    /// <summary>
    /// 玩家集合
    /// </summary>
    public Dictionary<int, NetBase> clients = new Dictionary<int, NetBase>();
    public int state = 0;
    /// <summary>
    /// 网络
    /// </summary>
    public NetWorkManager net;
    /// <summary>
    /// 定时器
    /// </summary>
    public Time time;
    public void Init()
    {
        time = new Time();
        time.update += Update;
        net = new NetWorkManager();
        net.Init(this);
    }

    private void Update()
    {
        net.Update();
        //if (state == 0) { return; }
        //if (state == -100) { Index = 0; state = -101; }
        //if (state == -101) { Index++; if (Index == 5) { server.Close(); state = -200; timer1.Stop(); } }
        //Index++;
        //time = deltaTime * Index;

        ////==处理输入
        //this.ProcessInputs();
        ////==逻辑更新
        //this.EntityUpdate();
        ////== 物理更新
        //world.Step(deltaTime);
        ////==玩法更新
        //if (!rule.Update(this))
        //{//结算了
        //    state = -100;
        //}
        ////==发送数据给客户端
        //this.SendMsg();
        //msg_intput.Clear();
    }

    /// <summary>
    /// 加入房间
    /// </summary>
    /// <param name="id"></param>
    /// <param name="net"></param>
    public void JoinClient(int id,NetBase net)
    {
        if(clients.ContainsKey(id))
        {//顶替
            Debug.LogWarning("顶替：" + net.address + "<-->" + clients[id].address);
            clients[id].End();
            clients[id] = net;
        }
        else
        {
            clients.Add(id, net);
        }
    }


    /// <summary>
    /// 处理输入
    /// </summary>
    public void ProcessInputs()
    {
        //if (clients_add.Count > 0)
        //{//有新的玩家加入
        //    lock (clients_add)
        //    {
        //        ClientJoin cj;
        //        for (int i = 0; i < clients_add.Count; i++)
        //        {
        //            cj = clients_add[i];
        //            if (players.ContainsKey(cj.id))
        //            {//短线重练
        //                Log.Info("===断线重连:" + cj.index);
        //                players[cj.id].SetState(1);

        //                clients.Add(cj.id, cj.client);

        //                if (AllMsgs.Count - cj.index <= 80)
        //                {//可以重练
        //                    SaveMsgsData smd = new SaveMsgsData();
        //                    for (int k = cj.index; k < AllMsgs.Count; k++)
        //                    {
        //                        smd.msgs.Add(AllMsgs[k]);
        //                    }
        //                    byte[] od = GetBytes<SaveMsgsData>(23, smd);
        //                    Log.Info("====断线重连 继续:" + od.Length);
        //                    cj.client.Send(od);
        //                }
        //                else
        //                {//太久远了
        //                    byte[] od = GetBytes<GameData>(24, old_data);
        //                    Log.Info("====断线重连 全兴:" + od.Length);
        //                    cj.client.Send(od);
        //                }
        //            }
        //            else
        //            {//新加入
        //                byte[] od = GetBytes<GameData>(21, old_data);
        //                Log.Info("===新加入:" + od.Length);
        //                cj.client.Send(od);
        //                clients.Add(cj.id, cj.client);
        //            }
        //            clients_add.RemoveAt(i);
        //            i--;
        //        }
        //    }
        //}

        //if (msg_intput_add.Count > 0)
        //{
        //    lock (msg_intput_add)
        //    {
        //        msg_intput.AddRange(msg_intput_add);
        //        msg_intput_add.Clear();
        //    }
        //}
        //Msg item;
        //for (int i = 0; i < msg_intput.Count; i++)
        //{
        //    item = msg_intput[i];
        //    switch (item.type)
        //    {
        //        case 200://玩家加入
        //            NewPlayerInfo data = NewPlayer(item.client_id);
        //            if (data != null)
        //            {
        //                Log.Info("新客户端加入：" + item.client_id);
        //                ScriptData sd = new ScriptData();
        //                sd.id = 199;
        //                sd.datas = GetData<NewPlayerInfo>(data);
        //                item.events.Add(sd);
        //            }
        //            else
        //            {
        //                Log.Info("客户端重新连接服务器：" + item.client_id);
        //                item.type = 202;//是短线后完成重新连接
        //            }
        //            break;
        //        default:
        //            if (players.ContainsKey(item.client_id))
        //            {
        //                players[item.client_id].applyInput(Index, item.type);
        //            }
        //            break;
        //    }
        //}
    }

    /// <summary>
    /// 逻辑更新
    /// </summary>
    private void EntityUpdate()
    {
        //if (entitys_running_add.Count > 0)
        //{
        //    foreach (var item in entitys_running_add)
        //    {
        //        entitys_running.Add(item.Key, item.Value);
        //    }
        //    entitys_running_add.Clear();
        //}
        ////=====remove update====
        //List<int> remove_ids = new List<int>();
        ////Log.Info("=============="+Index+"==========================");
        //foreach (var item in entitys_running)
        //{
        //    if (item.Value.State == -100)
        //    {
        //        remove_ids.Add(item.Key);
        //    }
        //    else
        //    {
        //        //if (item.Key > 1000)
        //        //{
        //        //    Log.Info("K" + item.Key);
        //        //}
        //        item.Value.LockUpdate();
        //    }
        //}
        ////Log.Info("========================================");
        //if (remove_ids.Count > 0)
        //{
        //    Entity entity;
        //    for (int i = 0; i < remove_ids.Count; i++)
        //    {
        //        entity = entitys_running[remove_ids[i]];
        //        entitys_running.Remove(remove_ids[i]);
        //        entitys_pool.Add(entity);
        //    }
        //    remove_ids.Clear();
        //}
    }

}



