
using System;
using WebSocketSharp.Server;

namespace Server
{
    class Room
    {
        public int RoomId;
        public bool IsCanEnter => P2 == null;
        public bool IsEmpty => P1 == null && P2 == null;
        //public bool IsContain(WebSocketSessionManager Player) => Player == P1 || Player == P2;
        public string P1;
        public string P2;
        string Player1Info;
        string Player2Info;
        public Room(int roomId)
        {
            RoomId = roomId;
        }
        public void Creat(string playerID, string playerInfo)
        {
            Console.WriteLine($"创建一个房间：房主信息{playerInfo}\n\n");
            P1 = playerID;
            Player1Info = playerInfo;
        }
        public void Join(string playerID, string playerInfo)
        {
            Console.WriteLine($"加入一个房间：房客信息{playerInfo}\n\n");
            
            P2 = playerID;
            Player2Info = playerInfo;
        }
        public void Open(WebSocketSessionManager sessions)
        {
            Console.WriteLine("我开房啦！！！！！！！！！！！！！！///////");
            Console.WriteLine(P1 + "\n");
            Console.WriteLine(P2 + "\n");
            Player1Info = Player1Info.ToObject<PlayerInfo>().ShufflePlayerDeck();
            Player2Info = Player2Info.ToObject<PlayerInfo>().ShufflePlayerDeck();
            //发送房间号，是否玩家1判定，玩家信息给对方
            sessions.SendTo(new GeneralCommand(RoomId, true, Player1Info, Player2Info).ToJson(), P1);
            sessions.SendTo(new GeneralCommand(RoomId, false, Player2Info, Player1Info).ToJson(), P2);
            Console.WriteLine("发送完毕");
        }
        internal void Remove(string playerID)
        {
            if (P1== playerID)
            {
                Console.WriteLine("玩家1"+P1+"取消匹配");
                P1 = null;
            }
            if (P2 == playerID)
            {
                Console.WriteLine("玩家1" + P2 + "取消匹配");
                P2 = null;
            }
        }


        public void AsyncInfo(WebSocketSessionManager sessions, string playerID, string Data)
        {
            Console.WriteLine("同步消息");
            sessions.SendTo(Data, playerID == P1 ? P2 : P1);
        }

       
    }
}
