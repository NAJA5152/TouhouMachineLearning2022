﻿
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using Server;
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
        return base.OnDisconnectedAsync(exception);
    }
    //////////////////////////////////////////////账户////////////////////////////////////////////////////////////////////
    public int Register(string account, string password) => MongoDbCommand.Register(account, password);
    public PlayerInfo? Login(string account, string password) => MongoDbCommand.Login(account, password);
    //////////////////////////////////////////////房间////////////////////////////////////////////////////////////////////
    public void Join(PlayerInfo playerInfo) => RoomManager.JoinRoom(Clients.Caller, playerInfo);
    public void Leave(int roomID)
    {
        RoomManager.LeaveRoom(Clients.Caller, roomID);
    }
    public void AsyncInfo(NetAcyncType netAcyncType, int roomId, bool isPlayer1, object[] data)
    {

        if (netAcyncType == NetAcyncType.Init)
        {
            Console.WriteLine("初始化连接");
            if (isPlayer1)
            {
                RoomManager.GetRoom(roomId).P1 = Clients.Caller;
            }
            else
            {
                RoomManager.GetRoom(roomId).P2 = Clients.Caller;
            }
        }
        else
        {
            RoomManager.GetRoom(roomId).AsyncInfo(Clients.Caller, data);
        }
    }
    //////////////////////////////////////////////用户操作////////////////////////////////////////////////////////////////////
    public bool UpdateDecks(PlayerInfo playerInfo) => MongoDbCommand.UpdateDecks(playerInfo);
    public PlayerInfo? TriggerUserState(string account, string password,string stateName) => MongoDbCommand.Login(account, password);

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
    public void UploadAgentSummary(AgainstSummary summary) => MongoDbCommand.InsertAgainstSummary(summary);
    public List<AgainstSummary> DownloadAgentSummary(string playerName, int skipNum, int takeNum) => MongoDbCommand.QueryAgainstSummary(playerName, skipNum, takeNum);
    //////////////////////////////////////////////卡牌配置////////////////////////////////////////////////////////////////////
    //查询最新版本
    public string GetCardConfigsVersion() => MongoDbCommand.GetLastCardUpdateTime();
    //更新卡牌配置信息
    public string UploadCardConfigs(CardConfig cardConfig) => MongoDbCommand.InsertOrUpdateCardConfig(cardConfig);
    //下载卡牌配置信息
    public CardConfig DownloadCardConfigs(string date) => MongoDbCommand.GetCardConfig(date);
}

