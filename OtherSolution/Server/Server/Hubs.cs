
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
    public void AsyncInfo(NetAcyncType netAcyncType, int roomId, bool isPlayer1, object[] data)
    {
        RoomManager.GetRoom(roomId).AsyncInfo(netAcyncType, data, isPlayer1);
    }
    public void AgainstFinish(int roomId, string account) => RoomManager.DisponseRoom(roomId, account);


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
    //////////////////////////////////////////////日志////////////////////////////////////////////////////////////////////
    //更新牌组信息
    public List<AgainstSummary> DownloadAgentSummary(string playerName, int skipNum, int takeNum) => MongoDbCommand.QueryAgainstSummary(playerName, skipNum, takeNum);
    public void UpdateTurnOperation(int roomId, AgainstSummary.TurnOperation turnOperation) => RoomManager.GetRoom(roomId).Summary.AddTurnOperation(turnOperation);
    public void UpdatePlayerOperation(int roomId, AgainstSummary.TurnOperation.PlayerOperation playerOperation) => RoomManager.GetRoom(roomId).Summary.AddPlayerOperation(playerOperation);
    public void UpdateSelectOperation(int roomId, AgainstSummary.TurnOperation.SelectOperation selectOperation) => RoomManager.GetRoom(roomId).Summary.AddSelectOperation(selectOperation);
    public void UploadStartPoint(int roomId, int relativePoint) => RoomManager.GetRoom(roomId).Summary.AddStartPoint(relativePoint);
    public void UploadEndPoint(int roomId, int relativePoint) => RoomManager.GetRoom(roomId).Summary.AddEndPoint(relativePoint);
    public void UploadSurrender(int roomId, int surrenddrState) => RoomManager.GetRoom(roomId).Summary.AddSurrender(surrenddrState);
    //////////////////////////////////////////////卡牌配置////////////////////////////////////////////////////////////////////
    //查询最新版本
    public string GetCardConfigsVersion() => MongoDbCommand.GetLastCardUpdateTime();
    //更新卡牌配置信息
    public string UploadCardConfigs(CardConfig cardConfig) => MongoDbCommand.InsertOrUpdateCardConfig(cardConfig);
    //下载卡牌配置信息
    public CardConfig DownloadCardConfigs(string date) => MongoDbCommand.GetCardConfig(date);
}