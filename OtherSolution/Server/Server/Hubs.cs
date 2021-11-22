
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using Server;
public class AccountHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Console.WriteLine("!!!");
        return base.OnConnectedAsync();
    }
    public int Register(string name, string password)
    {
        int v = MongoDbCommand.RegisterInfo(name, password); Console.WriteLine(v);
        return v;
    }

    public PlayerInfo? Login(string name, string password)
    {
        Console.WriteLine("我被调用了");
        return MongoDbCommand.LoginInfo(name, password);
    }
}
public class RoomHub : Hub
{
    public void Join(PlayerInfo playerInfo) => RoomCommand.JoinRoom(Clients.Caller, playerInfo);
    public void Leave(int roomID)
    {
        RoomCommand.LeaveRoom(Clients.Caller, roomID);
    }
    public void AsyncInfo(NetAcyncType netAcyncType, int roomId, bool isPlayer1, object[] data)
    {

        if (netAcyncType == NetAcyncType.Init)
        {
            Console.WriteLine("初始化连接");
            if (isPlayer1)
            {
                RoomCommand.GetRoom(roomId).P1 = Clients.Caller;
            }
            else
            {
                RoomCommand.GetRoom(roomId).P2 = Clients.Caller;
            }
        }
        else
        {
            RoomCommand.GetRoom(roomId).AsyncInfo(Clients.Caller, data);
        }
    }
}
public class AgentSummaryHub : Hub
{
    public void UploadAgentSummary(AgainstSummary summary) => MongoDbCommand.InsertAgainstSummary(summary);
    public List<AgainstSummary> DownloadAgentSummary(string playerName, int skipNum, int takeNum) => MongoDbCommand.QueryAgainstSummary(playerName, skipNum, takeNum);
}
//更新牌组信息
public class UserHub : Hub
{
    public bool UpdateDecks(PlayerInfo playerInfo) => MongoDbCommand.UpdateDecks(playerInfo);
    public void Chat(string name, string message)
    {
        Console.WriteLine("转发聊天记录"+message);
        Clients.All.SendAsync("ChatReceive", (name, message));
    }
}
//查询最新版本
//更新卡牌配置信息
//下载卡牌配置信息
public class CardConfigsHub : Hub
{
    public string GetCardConfigsVersion() => MongoDbCommand.GetLastCardUpdateTime();
    public string UploadCardConfigs(CardConfig cardConfig) => MongoDbCommand.InsertOrUpdateCardConfig(cardConfig);
    public CardConfig DownloadCardConfigs(string date) => MongoDbCommand.GetCardConfig(date);
}
