using System;
using System.Collections.Generic;
using System.Linq;
using WebSocketSharp.Server;

namespace Server
{
    partial class RoomCommand
    {
        public static List<Room> Rooms = new List<Room>();
        static Random rand = new Random();
        public static Room GetRoom(int RoomId) => Rooms.First(room => room.RoomId == RoomId);

        public static void CreatRoom(string playerID, string UserInfo)
        {
            List<int> RoomdIds = Rooms.Select(room => room.RoomId).ToList();
            for (int i = 0; ; i++)
            {
                if (!RoomdIds.Contains(i))
                {
                    Room TargetRoom = new Room(i);
                    Rooms.Add(TargetRoom);
                    TargetRoom.Creat(playerID, UserInfo);
                    break;
                }
            }
        }
        public static void JoinRoom(WebSocketSessionManager player, string playerInfo)
        {

            Console.WriteLine("房间数" + Rooms.Count);
            string Id = playerInfo.ToObject<PlayerInfo>()._id;
            if (Rooms.Where(room => room.IsCanEnter).Any())
            {
                Room TargetRoom = Rooms.First(room => room.IsCanEnter);
                TargetRoom.Join(Id, playerInfo);
                TargetRoom.Open(player);
                Console.WriteLine("加入房间");
            }
            else
            {
                CreatRoom(Id, playerInfo);
                Console.WriteLine("创建房间");
            }
            Console.WriteLine("房间为是否空：" + Rooms[0].IsCanEnter);
        }
        public static void LeaveRoom(WebSocketSessionManager player, string playerID, int RoomID)
        {
            Console.WriteLine("房间移除");
            Room TargetRoom = GetRoom(RoomID);
            if (TargetRoom != null)
            {
                TargetRoom.Remove(playerID);
                if (TargetRoom.IsEmpty)
                {
                    Rooms.Remove(TargetRoom);
                }
                //记录数据
            }
            player.SendTo(new GeneralCommand("success").ToJson(), playerID);

        }
        public static string ShufflePlayerDeck(string playerInfo)
        {
            PlayerInfo targetPlayerInfo = playerInfo.ToObject<PlayerInfo>();
            targetPlayerInfo.UseDeck.CardIds = targetPlayerInfo.UseDeck.CardIds.OrderBy(i => rand.Next()).ToList();
            return targetPlayerInfo.ToJson();
        }
    }
}
