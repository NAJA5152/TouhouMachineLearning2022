
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using Newtonsoft.Json;
using Server;
using System.Linq.Expressions;


public class TouHouHub : Hub
{
    //int num => Clients.co.
    public override Task OnConnectedAsync()
    {
        Console.WriteLine("一个用户登录了" + Context.ConnectionId);
        Clients.User(Context.ConnectionId).SendAsync("test", "你好呀");
        return base.OnConnectedAsync();
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("一个用户登出了" + Context.ConnectionId);
        OnlineUserManager.Remove(Context.ConnectionId);
        //OnlineUserManager.Remove(Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }
    //////////////////////////////////////////////账户////////////////////////////////////////////////////////////////////
    public int Register(string account, string password) => MongoDbCommand.Register(account, password);
    public PlayerInfo? Login(string account, string password)
    {
        var playInfo = MongoDbCommand.Login(account, password);
        //判断是否已有
        if (playInfo != null)
        {
            OnlineUserManager.Add(Context.ConnectionId, playInfo);
            //var targetRoom = RoomManager.Rooms.FirstOrDefault(room => room.IsContain(playInfo.Account));
        }
        return playInfo;
    }

    //////////////////////////////////////////////房间////////////////////////////////////////////////////////////////////
    public void Join(PlayerInfo playerInfo) => RoomCommand.JoinRoom(Clients.Caller, playerInfo);
    public void Leave(int roomID) => RoomCommand.LeaveRoom(Clients.Caller, roomID);
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
    //////////////////////////////////////////////用户操作////////////////////////////////////////////////////////////////////
    public bool UpdateName(string account, string password, string name) => MongoDbCommand.UpdateName(account, password, name);
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
    public bool UpdateDecks(PlayerInfo playerInfo) => MongoDbCommand.UpdateDecks(playerInfo);
    //public PlayerInfo? UpdateDecks(string account, string password, string stateName) => MongoDbCommand.Login(account, password);
    public static bool UpdateUserState(string account, string password, UserState userState) => MongoDbCommand.UpdateState(account, password, userState);
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

    public bool UpdateTurnOperation(int roomId, AgainstSummary.TurnOperation turnOperation)
    {
        RoomCommand.GetRoom(roomId).Summary.AddTurnOperation(turnOperation);
        return true;
    }
    public bool UpdateTurnPlayerOperationAsync(int roomId, AgainstSummary.TurnOperation.PlayerOperation playerOperation)
    {
        //RoomCommand.GetRoom(roomId).Summary.UpdateTurnOperation
        return true;
    }
    
    //////////////////////////////////////////////卡牌配置////////////////////////////////////////////////////////////////////
    //查询最新版本
    public string GetCardConfigsVersion() => MongoDbCommand.GetLastCardUpdateTime();
    //更新卡牌配置信息
    public string UploadCardConfigs(CardConfig cardConfig) => MongoDbCommand.InsertOrUpdateCardConfig(cardConfig);
    //下载卡牌配置信息
    public CardConfig DownloadCardConfigs(string date) => MongoDbCommand.GetCardConfig(date);
}

