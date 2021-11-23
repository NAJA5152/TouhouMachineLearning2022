
using Microsoft.AspNetCore.SignalR;
using System;

namespace Server
{
    class Room
    {
        public int RoomId { get; set; }
        public bool IsCanEnter => P2 == null;
        public bool IsEmpty => P1 == null && P2 == null;
        //public bool IsContain(WebSocketSessionManager Player) => Player == P1 || Player == P2;
        public IClientProxy P1 { get; set; }
        public IClientProxy P2 { get; set; }
        PlayerInfo Player1Info { get; set; }
        PlayerInfo Player2Info { get; set; }
        public Room(int roomId)
        {
            RoomId = roomId;
        }

        public void Creat(IClientProxy player, PlayerInfo playerInfo)
        {
            Console.WriteLine($"创建一个房间：房主信息{playerInfo}\n\n");
            P1 = player;
            Player1Info = playerInfo;
        }
        public void Join(IClientProxy player, PlayerInfo playerInfo)
        {
            Console.WriteLine($"加入一个房间：房客信息{playerInfo}\n\n");

            P2 = player;
            Player2Info = playerInfo;
        }
        public void Open()
        {
            Console.WriteLine("我开房啦！！！！！！！！！！！！！！///////");
            Console.WriteLine(P1 + "\n");
            Console.WriteLine(P2 + "\n");
            Player1Info = Player1Info.ShufflePlayerDeck();
            Player2Info = Player2Info.ShufflePlayerDeck();
            //发送房间号，是否玩家1判定，玩家信息给对方
            P1.SendAsync("StartAgainst", (RoomId, true, Player1Info, Player2Info));
            P1.SendAsync("StartAgainst", (RoomId, false, Player2Info, Player1Info));
        }
        internal void Remove(IClientProxy player)
        {
            if (P1 == player)
            {
                Console.WriteLine("玩家1" + P1 + "取消匹配");
                P1 = null;
            }
            if (P2 == player)
            {
                Console.WriteLine("玩家1" + P2 + "取消匹配");
                P2 = null;
            }
        }

        public void AsyncInfo(IClientProxy player, object[] command)
        {
            Console.WriteLine("同步消息");
            (player == P1 ? P2 : P1).SendAsync("Async", command);
        }
    }
}
