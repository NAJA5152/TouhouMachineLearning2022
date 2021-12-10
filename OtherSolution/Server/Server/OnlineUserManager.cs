using Microsoft.AspNetCore.SignalR;

namespace Server
{
    public class OnlineUserManager
    {
        public static Dictionary<string, PlayerInfo> OnlineUserList { get; set; } = new();
        public static void Add(string connectId, PlayerInfo playerInfo) => OnlineUserList[connectId] = playerInfo;
        public static void Remove(string connectId)
        {
            Console.WriteLine(OnlineUserList.ContainsKey(connectId));
            OnlineUserList.Remove(connectId);
        }

        public static bool hasAgainstRoom(string account)
        {
           var room= RoomManager.Rooms.FirstOrDefault(room => room.Player1Info.Account== account|| room.Player2Info.Account == account);
            return room!=null;
        }
    }
}
