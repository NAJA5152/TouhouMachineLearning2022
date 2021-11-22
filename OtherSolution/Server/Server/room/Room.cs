
using Microsoft.AspNetCore.SignalR;
using System;

namespace Server
{
    class Room
    {
        public int RoomId;
        public bool IsCanEnter => P2 == null;
        public bool IsEmpty => P1 == null && P2 == null;
        //public bool IsContain(WebSocketSessionManager Player) => Player == P1 || Player == P2;
        public IClientProxy P1;
        public IClientProxy P2;
        PlayerInfo player1Info;
        PlayerInfo player2Info;
        public Room(int roomId)
        {
            RoomId = roomId;
        }
        public void Creat(IClientProxy player, PlayerInfo playerInfo)
        {
            Console.WriteLine($"创建一个房间：房主信息{playerInfo}\n\n");
            P1 = player;
            player1Info = playerInfo;
        }
        public void Join(IClientProxy player, PlayerInfo playerInfo)
        {
            Console.WriteLine($"加入一个房间：房客信息{playerInfo}\n\n");
            
            P2 = player;
            player2Info = playerInfo;
        }
        public void Open()
        {
            Console.WriteLine("我开房啦！！！！！！！！！！！！！！///////");
            Console.WriteLine(P1 + "\n");
            Console.WriteLine(P2 + "\n");
            player1Info = player1Info.ShufflePlayerDeck();
            player2Info = player2Info.ShufflePlayerDeck();
            //发送房间号，是否玩家1判定，玩家信息给对方
            P1.SendAsync("StartAgainst",(RoomId, true, player1Info, player2Info));
            P1.SendAsync("StartAgainst",(RoomId, false, player2Info, player1Info));
        }
        internal void Remove(IClientProxy player)
        {
            if (P1== player)
            {
                Console.WriteLine("玩家1"+P1+"取消匹配");
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
