using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            MongoDbCommand.Init();
            Console.WriteLine("数据库已初始化");
            var server = new WebSocketServer($"ws://0.0.0.0:514");
            //账户
            server.AddWebSocketService<Register>("/Register");
            server.AddWebSocketService<Login>("/Login");
            //房间
            server.AddWebSocketService<Join>("/Join");
            server.AddWebSocketService<AsyncInfo>("/AsyncInfo");
            server.AddWebSocketService<Leave>("/Leave");
            server.AddWebSocketService<UpdateDecks>("/UpdateDecks");
            //卡牌配置信息
            server.AddWebSocketService<GetCardConfigsVersion>("/GetCardConfigsVersion");
            server.AddWebSocketService<UploadCardConfigs>("/UploadCardConfigs");
            server.AddWebSocketService<DownloadCardConfigs>("/DownloadCardConfigs");
            //对战记录
            server.AddWebSocketService<UploadAgentSummary>("/UploadAgentSummary");
            server.AddWebSocketService<DownloadAgentSummary>("/DownloadAgentSummary");
            Console.WriteLine("已载入回应函数");
            server.Start();
            Console.WriteLine("服务端已启动");
            Console.ReadLine();

            foreach (var room in RoomCommand.Rooms)
            {
                //room.P1.Broadcast("hi1");
                //room.P1.Broadcast("hi2");
            }
            Console.ReadLine();
            server.Stop();


        }
        public static WebSocketSessionManager manager;
        /////////////////////////////////////////////////////////////账户状态相关/////////////////////////////////////////////
        class Register : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                Console.WriteLine("有人登录");
                GeneralCommand<string> reciverMsg = e.Data.ToObject<GeneralCommand<string>>();
                string result = MongoDbCommand.RegisterInfo(reciverMsg.datas[0], reciverMsg.datas[1]);
                Console.WriteLine("结果" + result);
                Send(result);
            }
        }
        class Login : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                Console.WriteLine("登陆中");
                GeneralCommand<string> reciverMsg = e.Data.ToObject<GeneralCommand<string>>();
                int ResultNum = MongoDbCommand.LoginInfo(reciverMsg.datas[0], reciverMsg.datas[1], out PlayerInfo userinfo);
                Console.WriteLine("查询结果" + userinfo.ToJson());
                Send(new GeneralCommand(ResultNum.ToJson(), userinfo.ToJson()).ToJson());
            }
        }
        /////////////////////////////////////////////////////////////对战记录相关/////////////////////////////////////////////

        private class UploadAgentSummary : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                Console.WriteLine("更新卡牌配置信息");
                MongoDbCommand.InsertAgainstSummary(e.Data.ToObject<AgainstSummary>());
                Send("success");
                //try
                //{
                //    Console.WriteLine("更新卡牌配置信息");
                //    MongoDbCommand.InsertAgainstSummary(e.Data.ToObject<AgainstSummary>());
                //    Send("success");
                //}
                //catch (Exception ex)
                //{
                //    Send("上传对战信息出错" + ex.Message);
                //}
            }
        }
        private class DownloadAgentSummary : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                var reciverMsg = e.Data.ToObject<GeneralCommand>();
                string playerName = (string)reciverMsg.datas[0];
                int skipNum = (int)reciverMsg.datas[1];
                int takeNum = (int)reciverMsg.datas[2];
                Console.WriteLine("更新卡牌配置信息");
                var summarys = MongoDbCommand.QueryAgainstSummary(playerName, skipNum, takeNum);
                Send(summarys.ToJson());
                //try
                //{
                //    var reciverMsg = e.Data.ToObject<GeneralCommand>();
                //    string playerName = (string)reciverMsg.datas[0];
                //    int skipNum = (int)reciverMsg.datas[1];
                //    int takeNum = (int)reciverMsg.datas[2];
                //    Console.WriteLine("更新卡牌配置信息");
                //    var summarys = MongoDbCommand.QueryAgainstSummary(playerName, skipNum, takeNum);
                //    Send(summarys.ToJson());
                //}
                //catch (Exception ex)
                //{
                //    Send("下载对战信息出错" + ex.Message);
                //}
            }
        }
        /////////////////////////////////////////////////////////////卡牌配置信息相关/////////////////////////////////////////////
        private class GetCardConfigsVersion : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                Console.WriteLine("查询最新版本");
                var date = MongoDbCommand.GetLastCardUpdateTime();
                Send(date);
                //try
                //{
                //    Console.WriteLine("查询最新版本");
                //    var date = MongoDbCommand.GetLastCardUpdateTime();
                //    Send(date);
                //}
                //catch (Exception ex)
                //{
                //    Send("查询最新版本" + ex.Message);
                //}
            }
        }
        private class UploadCardConfigs : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                Console.WriteLine("更新卡牌配置信息");
                MongoDbCommand.InsertOrUpdateCardConfig(e.Data.ToObject<CardConfig>());
                Send("success");
                //try
                //{
                //    Console.WriteLine("更新卡牌配置信息");
                //    MongoDbCommand.InsertOrUpdateCardConfig(e.Data.ToObject<CardConfig>());
                //    Send("success");
                //}
                //catch (Exception ex)
                //{
                //    Send("更新卡牌配置信息出错" + ex.Message);
                //}
            }
        }
        private class DownloadCardConfigs : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                Console.WriteLine("请求卡牌配置信息" + e.Data);
                Send(MongoDbCommand.GetCardConfig(e.Data));
                //try
                //{
                //    Send(MongoDbCommand.GetCardConfig(e.Data));
                //}
                //catch (Exception ex)
                //{
                //    Send("请求卡牌配置信息出错" + ex.Message);
                //}

            }
        }
        /////////////////////////////////////////////////////////////卡组相关/////////////////////////////////////////////
        private class UpdateDecks : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {

                Console.WriteLine("更新牌组信息");
                PlayerInfo reciverMsg = e.Data.ToObject<PlayerInfo>();
                bool updateResult = MongoDbCommand.UpdateDecks(reciverMsg);
                //Send(updateResult);
            }
        }
        /////////////////////////////////////////////////////////////房间相关/////////////////////////////////////////////
        private class Join : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {

                //Console.WriteLine("收到了" + e.Data.ToObject<GeneralCommand<string>>());
                Console.WriteLine("收到了" + e.Data);
                Console.WriteLine("加入房间");
                //Players.Add(Sessions);
                RoomCommand.JoinRoom(Sessions, e.Data);
                //Send("你好呀！");
            }
        }
        private class Leave : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {

                Console.WriteLine("收到了" + e.Data);
                string[] datas = e.Data.ToObject<GeneralCommand<string>>().datas;
                int roomID = int.Parse(datas[0]);
                string uid = datas[1];
                Console.WriteLine($"玩家离开房间{roomID}");
                //Players.Add(Sessions);
                RoomCommand.LeaveRoom(Sessions, uid, roomID);
            }
        }

        private class AsyncInfo : WebSocketBehavior
        {
            int roomid_origin = 0;
            protected override void OnMessage(MessageEventArgs e)
            {
                Console.WriteLine("同步信息中" + e.Data);
                int typeNum = int.Parse(e.Data.ToObject<GeneralCommand>().datas[0].ToString());
                NetAcyncType type = (NetAcyncType)typeNum;
                int roomId = int.Parse(e.Data.ToObject<GeneralCommand>().datas[1].ToString());

                if (type == NetAcyncType.Init)
                {
                    Console.WriteLine("初始化连接");
                    bool isPlayer1 = (bool)e.Data.ToObject<GeneralCommand>().datas[2];
                    if (isPlayer1)
                    {
                        Console.WriteLine("配置玩家1");
                        RoomCommand.GetRoom(roomId).P1 = ID;
                    }
                    else
                    {
                        Console.WriteLine("配置玩家2");
                        RoomCommand.GetRoom(roomId).P2 = ID;
                    }
                    roomid_origin = roomId;
                }
                else
                {
                    Console.WriteLine("id----:" + roomid_origin + ":" + roomId);
                    RoomCommand.GetRoom(roomId).AsyncInfo(Sessions, ID, e.Data);
                }
            }
        }
    }
}