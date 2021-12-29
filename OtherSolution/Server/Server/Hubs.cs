
using Server;
using MongoDB.Bson;
using Microsoft.AspNetCore.SignalR;
public class TouHouHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Console.WriteLine("一个用户登录了" + Context.ConnectionId);
        return base.OnConnectedAsync();
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("一个用户登出了" + Context.ConnectionId);
        OnlineUserManager.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
    //////////////////////////////////////////////账户////////////////////////////////////////////////////////////////////
    public int Register(string account, string password) => MongoDbCommand.Register(account, password);
    public PlayerInfo? Login(string account, string password) => MongoDbCommand.Login(account, password);
    //////////////////////////////////////////////等候列表////////////////////////////////////////////////////////////////////
    public void Join(AgainstModeType againstMode, PlayerInfo playerInfo, PlayerInfo virtualOpponentInfo) => HoldListManager.Add(againstMode, playerInfo, virtualOpponentInfo, Clients.Caller);
    public void Leave(AgainstModeType againstMode, string account) => HoldListManager.Remove(againstMode, account);
    //////////////////////////////////////////////房间////////////////////////////////////////////////////////////////////
    public void AsyncInfo(NetAcyncType netAcyncType, string roomId, bool isPlayer1, object[] data) => RoomManager.GetRoom(roomId).AsyncInfo(netAcyncType, isPlayer1, data);
    public bool AgainstFinish(string roomId, string account) => RoomManager.DisponseRoom(roomId, account);
    //////////////////////////////////////////////用户信息更新操作////////////////////////////////////////////////////////////////////
    public bool UpdateInfo(UpdateType updateType, string account, string password, object updateValue)
    {
        switch (updateType)
        {

            case UpdateType.Name: return MongoDbCommand.UpdateInfo(account, password, (x => x.Name), updateValue.To<string>());
            case UpdateType.Deck: return MongoDbCommand.UpdateInfo(account, password, (x => x.Decks), updateValue.To<List<CardDeck>>());
            case UpdateType.UseDeckNum: return MongoDbCommand.UpdateInfo(account, password, (x => x.UseDeckNum), updateValue.To<int>());
            case UpdateType.UserState: return MongoDbCommand.UpdateInfo(account, password, (x => x.OnlineUserState), updateValue.To<UserState>());
            default: return false;
        }
    }
    //////////////////////////////////////////////聊天////////////////////////////////////////////////////////////////////
    public void Chat(string name, string message, string target)
    {
        Console.WriteLine("转发聊天记录" + message);
        if (target == "")
        {
            Clients.All.SendAsync("ChatReceive", (name, message).ToJson());
        }
        else
        {
            Clients.Client("").SendAsync("ChatReceive", (name, message).ToJson());
        }
    }
    public void Test(string text)
    {
        Clients.Caller.SendAsync("Test","服务器向你问候"+text);
        Console.WriteLine(text);
    }
    //////////////////////////////////////////////日志////////////////////////////////////////////////////////////////////
    //下载自己的记录
    public List<AgainstSummary> DownloadOwnerAgentSummary(string playerName, int skipNum, int takeNum) => MongoDbCommand.QueryAgainstSummary(playerName, skipNum, takeNum);
    //下载所有的记录
    public List<AgainstSummary> DownloadAllAgentSummary(int skipNum, int takeNum) => MongoDbCommand.QueryAgainstSummary( skipNum, takeNum);
    public void UpdateTurnOperation(string roomId, AgainstSummary.TurnOperation turnOperation) => RoomManager.GetRoom(roomId).Summary.AddTurnOperation(turnOperation);
    public void UpdatePlayerOperation(string roomId, AgainstSummary.TurnOperation.PlayerOperation playerOperation) => RoomManager.GetRoom(roomId).Summary.AddPlayerOperation(playerOperation);
    public void UpdateSelectOperation(string roomId, AgainstSummary.TurnOperation.SelectOperation selectOperation) => RoomManager.GetRoom(roomId).Summary.AddSelectOperation(selectOperation);
    public void UploadStartPoint(string roomId, int relativePoint) => RoomManager.GetRoom(roomId).Summary.AddStartPoint(relativePoint);
    public void UploadEndPoint(string roomId, int relativePoint) => RoomManager.GetRoom(roomId).Summary.AddEndPoint(relativePoint);
    public void UploadSurrender(string roomId, int surrenddrState) => RoomManager.GetRoom(roomId)?.Summary.AddSurrender(surrenddrState);
    //////////////////////////////////////////////卡牌配置////////////////////////////////////////////////////////////////////
    //查询最新版本
    public string GetCardConfigsVersion() => MongoDbCommand.GetLastCardUpdateTime();
    //更新卡牌配置信息
    public string UploadCardConfigs(CardConfig cardConfig) => MongoDbCommand.InsertOrUpdateCardConfig(cardConfig);
    //下载卡牌配置信息
    public CardConfig DownloadCardConfigs(string date) => MongoDbCommand.GetCardConfig(date);
}