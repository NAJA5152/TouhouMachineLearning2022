
using Microsoft.AspNetCore.SignalR;
using System;

namespace Server
{
    class Room
    {
        public int RoomId { get; set; }
        public bool IsCanEnter => P2 == null;
        public bool IsEmpty => P1 == null && P2 == null;
        public bool IsContain(string Account) => Account == Player1Info.Account || Account == Player2Info.Account;
        public IClientProxy P1 { get; set; }
        public IClientProxy P2 { get; set; }
        public List<IClientProxy> clientProxies { get; set; }
        public PlayerInfo Player1Info { get; set; }
        public PlayerInfo Player2Info { get; set; }
        public AgainstSummary Summary { get; set; } = new AgainstSummary();

        public Room(int roomId) => RoomId = roomId;
        internal void Creat(HoldInfo player1, HoldInfo player2)
        {
            P1 = player1.Client;
            P2 = player2.Client;
            Player1Info = player1.UserInfo;
            Player2Info = player2.UserInfo;
            Console.WriteLine("我开房啦！！！！！！！！！！！！！！///////");

            Player1Info = Player1Info.ShufflePlayerDeck();
            Player2Info = Player2Info.ShufflePlayerDeck();
            //发送房间号，默认玩家1是先手，将玩家牌组信息打乱并发送给对方
            Summary._id = Guid.NewGuid().ToString();
            Summary.Player1Name = Player1Info.Name;
            Summary.Player2Name = Player2Info.Name;
            Summary.AssemblyVerision = MongoDbCommand.GetLastCardUpdateVersion();

            P1?.SendAsync("StartAgainst", new object[] { RoomId, Player1Info, Player2Info, true, true });
            P2?.SendAsync("StartAgainst", new object[] { RoomId, Player2Info, Player1Info, false, false });


        }
        public AgainstSummary ReConnect(IClientProxy player, bool isPlayer1)
        {
            Console.WriteLine($"重新连接房间房间：房客信息{(isPlayer1 ? Player1Info.Name : Player2Info.Name)}\n\n");
            if (isPlayer1)
            {
                P1 = player;
            }
            else
            {
                P2 = player;
            }
            return Summary;
        }
        public void AsyncInfo(NetAcyncType netAcyncType, object[] command, bool isPlayer1)
        {
            Console.WriteLine("同步消息");
            (isPlayer1 ? P2 : P1).SendAsync("Async", netAcyncType,command);
        }
    }
}
