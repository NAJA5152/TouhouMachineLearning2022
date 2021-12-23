using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    partial class RoomManager
    {
        public static List<Room> Rooms { get; set; } = new List<Room>();
        public static Room? GetRoom(int RoomId) => Rooms.FirstOrDefault(room => room.RoomId == RoomId);
        public static Room? ContainPlayerRoom(string account) => Rooms.FirstOrDefault(room => room.Player1Info.Account == account || room.Player2Info.Account == account);
        public static void CreatRoom(HoldInfo player1, HoldInfo player2)
        {
            List<int> RoomdIds = Rooms.Select(room => room.RoomId).ToList();
            for (int i = 0; ; i++)
            {
                if (!RoomdIds.Contains(i))
                {
                    Console.WriteLine($"创建房间{i}");
                    Room TargetRoom = new Room(i);
                    Rooms.Add(TargetRoom);
                    TargetRoom.Creat(player1, player2);
                    break;
                }
            }
        }
        public static bool DisponseRoom(int roomID, string account)
        {
            Room? TargetRoom = GetRoom(roomID);
            if (TargetRoom != null && (TargetRoom.Player1Info.Account == account || TargetRoom.Player2Info.Account == account))
            {
                //房间上传数据
                TargetRoom.Summary.UploadAgentSummary();
                Rooms.Remove(TargetRoom);
                return true;
            }
            return false;
        }
        //public static void CreatRoom(IClientProxy playerID, PlayerInfo playerInfo)
        //{
        //    List<int> RoomdIds = Rooms.Select(room => room.RoomId).ToList();
        //    for (int i = 0; ; i++)
        //    {
        //        if (!RoomdIds.Contains(i))
        //        {
        //            Room TargetRoom = new Room(i);
        //            Rooms.Add(TargetRoom);
        //            TargetRoom.Creat(playerID, playerInfo);
        //            break;
        //        }
        //    }
        //}
        //public static void JoinRoom(AgainstModeType modeType, PlayerInfo playerInfo, IClientProxy player)
        //{

        //    Console.WriteLine("房间数" + Rooms.Count);
        //    string Id = playerInfo._id;
        //    if (Rooms.Where(room => room.IsCanEnter).Any())
        //    {
        //        Room TargetRoom = Rooms.First(room => room.IsCanEnter);
        //        //TargetRoom.Join(modeType, player, playerInfo);
        //        //TargetRoom.Open();
        //        Console.WriteLine("加入房间");
        //    }
        //    else
        //    {
        //        CreatRoom(player, playerInfo);
        //        Console.WriteLine("创建房间");
        //    }
        //    Console.WriteLine("房间为是否空：" + Rooms[0].IsCanEnter);
        //}

        //public static bool LeaveRoom(IClientProxy player, int roomID)
        //{
        //    Room TargetRoom = GetRoom(roomID);
        //    if (TargetRoom != null)
        //    {
        //        TargetRoom.Remove(player);
        //        if (TargetRoom.IsEmpty)
        //        {
        //            Rooms.Remove(TargetRoom);
        //        }
        //    }
        //    return true;
        //}

    }
}
