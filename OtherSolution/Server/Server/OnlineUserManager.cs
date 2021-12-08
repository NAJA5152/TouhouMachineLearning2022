using Microsoft.AspNetCore.SignalR;

namespace Server
{
    public class OnlineUserManager
    {
        public static Dictionary<IClientProxy, PlayerInfo> OnlineUserList { get; set; } = new();
        public static void Add(IClientProxy caller, PlayerInfo playerInfo) => OnlineUserList[caller] = playerInfo;
        public static void Remove(IClientProxy caller)
        {
            Console.WriteLine(OnlineUserList.ContainsKey(caller));
            //OnlineUserList.Remove(OnlineUserList.FirstOrDefault(client=>client.Key));
        }

        public static bool hasAgainstRoom(string account)
        {
           var room= RoomManager.Rooms.FirstOrDefault(room => room.Player1Info.Account== account|| room.Player2Info.Account == account);
            return room!=null;
        }
    }
}
