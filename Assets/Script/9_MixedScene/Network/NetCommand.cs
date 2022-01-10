using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using WebSocketSharp;

namespace TouhouMachineLearningSummary.Command
{
    public static class NetCommand
    {
        static string ip => Info.AgainstInfo.isHostNetMode ? "localhost:495" : "106.15.38.165:495";
        static HubConnection TohHouHub { get; set; } = new HubConnectionBuilder().WithUrl($"http://{ip}/TouHouHub").Build();
        /// <summary>
        /// 注册响应事件
        /// </summary>
        public static void Init()
        {
            TohHouHub.On<string>("ChatReceive", message =>
            {
                var receive = message.ToObject<(string name, string text, string targetUser)>();
                ChatManager.MainChat.ReceiveMessage(receive.name, receive.text, receive.targetUser);
            });
            TohHouHub.On<string>("test", message =>
            {
                Debug.Log(message);
            });
            TohHouHub.On<object[]>("StartAgainst", ReceiveInfo =>
            {
                Info.AgainstInfo.RoomID = ReceiveInfo[0].ToType<string>();
                PlayerInfo playerInfo = ReceiveInfo[1].ToType<PlayerInfo>();
                PlayerInfo opponentInfo = ReceiveInfo[2].ToType<PlayerInfo>();
                bool isPlayer1 = ReceiveInfo[3].ToType<bool>();

                _ = Command.GameUI.NoticeCommand.CloseAsync();//关闭ui
                Command.BookCommand.SimulateFilpPage(false);//停止翻书
                Command.MenuStateCommand.AddState(MenuState.ScenePage);//增加路由

                Debug.Log("进入对战配置模式");
                AgainstManager.AutoSetRole(isPlayer1);
                AgainstManager.AutoSetPlayerInfo(playerInfo);
                AgainstManager.AutoSetOpponentInfo(opponentInfo);
                //Manager.LoadingManager.manager?.OpenAsync();
                AgainstManager.AutoStart();
            });
            TohHouHub.On<NetAcyncType, object[]>("Async", (type, receiveInfo) =>
            {
                switch (type)
                {
                    case NetAcyncType.FocusCard:
                        {
                            int X = receiveInfo[0].ToType<int>();
                            int Y = receiveInfo[1].ToType<int>();
                            AgainstInfo.opponentFocusCard = Command.RowCommand.GetCard(X, Y);
                            break;
                        }
                    case NetAcyncType.PlayCard:
                        {
                            int X = receiveInfo[0].ToType<int>();
                            int Y = receiveInfo[1].ToType<int>();
                            Card targetCard = Command.RowCommand.GetCard(X, Y);
                            Info.AgainstInfo.playerPlayCard = targetCard;
                            break;
                        }
                    case NetAcyncType.SelectRegion:
                        {
                            Debug.Log("触发区域同步");
                            int X = receiveInfo[0].ToType<int>();
                            AgainstInfo.SelectRegion = Command.RowCommand.GetSingleRowInfoById(X);
                            break;
                        }
                    case NetAcyncType.SelectUnites:
                        {
                            Debug.Log("收到同步单位信息");
                            List<Location> Locations = receiveInfo[0].ToType<List<Location>>();
                            AgainstInfo.selectUnits.AddRange(Locations.Select(location => Command.RowCommand.GetCard(location.X, location.Y)));
                            break;
                        }
                    case NetAcyncType.SelectLocation:
                        {
                            Debug.Log("触发坐标同步");
                            int X = receiveInfo[0].ToType<int>();
                            int Y = receiveInfo[1].ToType<int>();
                            Info.AgainstInfo.SelectRegion = Command.RowCommand.GetSingleRowInfoById(X);
                            Info.AgainstInfo.SelectLocation = Y;
                            Debug.Log($"坐标为：{X}:{Y}");
                            break;
                        }
                    case NetAcyncType.Pass:
                        {
                            Info.AgainstInfo.isPlayerPass = true;
                            //Command GameUI.UiCommand.SetCurrentPass();
                            break;
                        }
                    case NetAcyncType.Surrender:
                        {
                            Debug.Log("收到结束指令");
                            _ = StateCommand.AgainstEnd(true, true);
                            break;
                        }
                    case NetAcyncType.ExchangeCard:
                        {
                            Debug.Log("交换卡牌信息");
                            Location location = receiveInfo[0].ToType<Location>();
                            int washInsertRank = receiveInfo[1].ToType<int>();
                            _ = CardCommand.ExchangeCard(Command.RowCommand.GetCard(location), IsPlayerExchange: false, WashInsertRank: washInsertRank);
                            break;
                        }
                    case NetAcyncType.RoundStartExchangeOver:
                        if (AgainstInfo.IsPlayer1)
                        {
                            AgainstInfo.isPlayer2RoundStartExchangeOver = true;
                        }
                        else
                        {
                            AgainstInfo.isPlayer1RoundStartExchangeOver = true;
                        }
                        break;
                    case NetAcyncType.SelectProperty:
                        {
                            AgainstInfo.SelectProperty = receiveInfo[0].ToType<BattleRegion>();
                            Debug.Log("通过网络同步当前属性为" + Info.AgainstInfo.SelectProperty);
                            break;
                        }
                    case NetAcyncType.SelectBoardCard:
                        {
                            AgainstInfo.selectBoardCardRanks = receiveInfo[0].ToType<List<int>>();
                            AgainstInfo.IsSelectCardOver = receiveInfo[1].ToType<bool>();
                            break;
                        }
                    default:
                        break;
                }
            });
        }
        public static async void Dispose()
        {
            Debug.Log("释放网络资源");
            await TohHouHub.StopAsync();
            //AsyncConnect.Close();
        }
        public static async Task<int> RegisterAsync(string account, string password)
        {
            try
            {
                Debug.Log("注册请求");

                if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
                return await TohHouHub.InvokeAsync<int>("Register", account, password);
            }
            catch (Exception e) { Debug.LogException(e); }
            return -1;
        }
        public static async Task<bool> LoginAsync(string account, string password)
        {
            try
            {
                Debug.Log("登陆请求");
                if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
                Info.AgainstInfo.onlineUserInfo = await TohHouHub.InvokeAsync<PlayerInfo>("Login", account, password);
                Debug.Log(Info.AgainstInfo.onlineUserInfo.ToJson());
            }
            catch (Exception e) { Debug.LogException(e); }
            return Info.AgainstInfo.onlineUserInfo != null;
        }


        ///////////////////////////////////////对战记录///////////////////////////////////////////////////////////////////////
        public static async Task UpdateTurnOperationAsync(AgainstSummaryManager.TurnOperation turnOperation)
        {
            if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
            await TohHouHub.SendAsync("UpdateTurnOperation", Info.AgainstInfo.RoomID, turnOperation);
        }
        public static async Task UpdateTurnPlayerOperationAsync(AgainstSummaryManager.TurnOperation.PlayerOperation playerOperation)
        {
            if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
            await TohHouHub.SendAsync("UpdatePlayerOperation", Info.AgainstInfo.RoomID, playerOperation);
        }
        public static async Task UpdateTurnSelectOperationAsync(AgainstSummaryManager.TurnOperation.SelectOperation selectOperation)
        {
            if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
            await TohHouHub.SendAsync("UpdateSelectOperation", Info.AgainstInfo.RoomID, selectOperation);
        }
        public static async Task UploadStartPointAsync()
        {
            if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
            await TohHouHub.SendAsync("UploadStartPoint", Info.AgainstInfo.RoomID, AgainstInfo.TurnRelativePoint);
        }
        public static async Task UploadEndPointAsync()
        {
            if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
            await TohHouHub.SendAsync("UploadEndPoint", Info.AgainstInfo.RoomID, AgainstInfo.TurnRelativePoint);
        }
        public static async Task UploadSurrenderAsync(int surrenddrState)
        {
            if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
            await TohHouHub.SendAsync("UploadSurrender", Info.AgainstInfo.RoomID, surrenddrState);
        }
        public static async Task<List<AgainstSummaryManager>> DownloadOwnerAgentSummaryAsync(string playerAccount, int skipCount, int takeCount)
        {
            if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
            return await TohHouHub.InvokeAsync<List<AgainstSummaryManager>>("DownloadOwnerAgentSummary", playerAccount, skipCount, takeCount);
        }
        public static async Task<List<AgainstSummaryManager>> DownloadAllAgentSummaryAsync(int skipCount, int takeCount)
        {
            if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
            return await TohHouHub.InvokeAsync<List<AgainstSummaryManager>>("DownloadAllAgentSummary", skipCount, takeCount);
        }
        ///////////////////////////////////////卡牌配置///////////////////////////////////////////////////////////////////////

        internal static async Task<string> GetCardConfigsVersionAsync()
        {
            if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
            return await TohHouHub.InvokeAsync<string>("GetCardConfigsVersion");
        }

        public static async Task UploadCardConfigsAsync(CardConfig cardConfig)
        {
            try
            {
                Debug.Log("上传卡牌配置");
                if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
                string result = await TohHouHub.InvokeAsync<string>("UploadCardConfigs", cardConfig);
                Debug.Log("新卡牌配置上传结果: " + result);
            }
            catch (Exception e) { Debug.LogException(e); }
        }
        /// <summary>
        /// 根据输入日期下载指定版本卡牌数据
        /// </summary>
        /// <param name="date"></param>
        public static async Task<CardConfig> DownloadCardConfigsAsync(string date)
        {
            try
            {
                Debug.Log("下载卡牌配置");
                if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
                return await TohHouHub.InvokeAsync<CardConfig>("DownloadCardConfigs", date);
            }
            catch (Exception e) { Debug.LogException(e); }
            return null;
        }
        ///////////////////////////////////////////////////用户操作////////////////////////////////////////////////////////////////
        public static async Task<bool> UpdateInfoAsync(UpdateType updateType, object updateValue)
        {
            try
            {
                Debug.Log("更新");
                if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
                return await TohHouHub.InvokeAsync<bool>("UpdateInfo", updateType, AgainstInfo.onlineUserInfo.Account, AgainstInfo.onlineUserInfo.Password, updateValue);
            }
            catch (Exception e) { Debug.LogException(e); }
            return false;
        }
        public static async Task ChatAsync(string name, string text, string target = "")
        {
            if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
            await TohHouHub.SendAsync("Chat", name, text, target);
        }
        ///////////////////////////////////////////////////房间操作////////////////////////////////////////////////////////////////
        public static async Task JoinHoldOnList(AgainstModeType modeType, PlayerInfo userInfo, PlayerInfo virtualOpponentInfo)
        {
            if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
            try
            {
                Debug.Log($"发送数据 我方牌组数：{userInfo.UseDeck.CardIds.Count} 敌方牌组数{virtualOpponentInfo.UseDeck.CardIds.Count}");
                await TohHouHub.SendAsync("Join", modeType, userInfo, virtualOpponentInfo);
            }
            catch (Exception ex) { Debug.LogException(ex); }
        }
        public static async Task<bool> LeaveHoldOnList(AgainstModeType modeType, string account)
        {
            if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
            return await TohHouHub.InvokeAsync<bool>("Leave", modeType, account);
        }
        public static async Task<bool> AgainstFinish()
        {
            if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
            return await TohHouHub.InvokeAsync<bool>("AgainstFinish", Info.AgainstInfo.RoomID, AgainstInfo.onlineUserInfo.Account,AgainstInfo.PlayerScore.P1Score, AgainstInfo.PlayerScore.P2Score);
        }
        //判断是否存在正在对战中的房间
        internal static async Task CheckRoomAsync(string text1, string text2)
        {
            if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
            int roomId = await TohHouHub.InvokeAsync<int>("CheckRoom");
            if (true)
            {

            }
        }
        //数据同步类型
        public static async void AsyncInfo(NetAcyncType AcyncType)
        {


            if (Info.AgainstInfo.IsPVP && (Info.AgainstInfo.IsMyTurn || AcyncType == NetAcyncType.FocusCard || AcyncType == NetAcyncType.ExchangeCard || AcyncType == NetAcyncType.RoundStartExchangeOver))
            {
                if (TohHouHub.State == HubConnectionState.Disconnected) { await TohHouHub.StartAsync(); }
                switch (AcyncType)
                {
                    case NetAcyncType.FocusCard:
                        {
                            Location TargetCardLocation = Info.AgainstInfo.playerFocusCard != null ? Info.AgainstInfo.playerFocusCard.Location : new Location(-1, -1);
                            await TohHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, new object[] { TargetCardLocation.X, TargetCardLocation.Y });
                            break;
                        }
                    case NetAcyncType.PlayCard:
                        {
                            Debug.Log("同步打出卡牌");
                            Location TargetCardLocation = Info.AgainstInfo.playerPlayCard.Location;
                            await TohHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, new object[] { TargetCardLocation.X, TargetCardLocation.Y });
                            break;
                        }
                    case NetAcyncType.SelectRegion:
                        {
                            int RowRank = Info.AgainstInfo.SelectRegion.RowRank;
                            Debug.Log("同步焦点区域为" + RowRank);
                            await TohHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, new object[] { RowRank });
                            break;
                        }
                    case NetAcyncType.SelectUnites:
                        {
                            List<Location> Locations = Info.AgainstInfo.selectUnits.SelectList(unite => unite.Location);
                            Debug.LogError("选择单位完成：" + Locations.ToJson());
                            await TohHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, new object[] { Locations });
                            break;
                        }
                    case NetAcyncType.SelectLocation:
                        {
                            int RowRank = Info.AgainstInfo.SelectRegion.RowRank;
                            int LocationRank = Info.AgainstInfo.SelectLocation;
                            Debug.Log("同步焦点坐标给对面：" + RowRank);
                            await TohHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, new object[] { RowRank, LocationRank });
                            break;
                        }
                    case NetAcyncType.ExchangeCard:
                        {
                            Debug.Log("触发交换卡牌信息");
                            Location Locat = Info.AgainstInfo.TargetCard.Location;
                            int RandomRank = Info.AgainstInfo.washInsertRank;
                            await TohHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, new object[] { RandomRank });
                            break;
                        }
                    case NetAcyncType.RoundStartExchangeOver:
                        Debug.Log("触发回合开始换牌完成信息");
                        await TohHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1);
                        break;
                    case NetAcyncType.Pass:
                        {
                            Debug.Log("pass");
                            await TohHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1);
                            break;
                        }
                    case NetAcyncType.Surrender:
                        {
                            Debug.Log("投降");
                            await TohHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1);
                            break;
                        }
                    case NetAcyncType.SelectProperty:
                        {
                            Debug.Log("选择场地属性");
                            await TohHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, AgainstInfo.SelectProperty);
                            break;
                        }
                    case NetAcyncType.SelectBoardCard:
                        Debug.Log("同步面板卡牌数据选择");
                        await TohHouHub.SendAsync("Async", AcyncType, AgainstInfo.RoomID, AgainstInfo.IsPlayer1, new object[] { Info.AgainstInfo.selectBoardCardRanks, Info.AgainstInfo.IsSelectCardOver });
                        break;
                    default:
                        {
                            Debug.Log("异常同步指令");
                            break;
                        }
                }
            }
        }
    }
}