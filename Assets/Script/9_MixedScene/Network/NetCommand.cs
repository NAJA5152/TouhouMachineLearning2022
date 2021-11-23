using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.Thread;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using WebSocketSharp;
using System.Threading.Tasks;
using static UnityEngine.Networking.UnityWebRequest;
using System;
using Microsoft.AspNetCore.SignalR.Client;

namespace TouhouMachineLearningSummary.Command
{

    namespace Network
    {
        public static class NetCommand
        {
            static string ip => Info.AgainstInfo.isHostNetMode ? "localhost:495" : "106.15.38.165:514";
            //static string ip = "106.15.38.165:514";
            static WebSocket AsyncConnect = new WebSocket($"ws://{ip}/AsyncInfo");

            static HubConnection connentHub = new HubConnectionBuilder().WithUrl($"http://{ip}/TouHouHub").Build();

            public static void Init()
            {
                connentHub.On<string>("ChatReceive", message =>
                {
                    var receive = message.ToObject<(string name, string text, string targetUser)>();
                    ChatManager.MainChat.ReceiveMessage(receive.name, receive.text, receive.targetUser);
                });
            }
            public static void Dispose()
            {
                Debug.Log("释放网络资源");
                AsyncConnect.Close();
            }
            public static async Task RegisterAsync(string name, string password)
            {
                try
                {
                    if (connentHub.State == HubConnectionState.Disconnected) { await connentHub.StartAsync(); }
                    Debug.Log("注册请求");
                    await connentHub.StartAsync();
                    int result = await connentHub.InvokeAsync<int>("Register", name, password);
                    //await accountHub.StopAsync();
                    switch (result)
                    {
                        case (1): await Command.GameUI.NoticeCommand.ShowAsync("注册成功", NotifyBoardMode.Ok); break;
                        case (-1): await Command.GameUI.NoticeCommand.ShowAsync("账号已存在", NotifyBoardMode.Ok); break;
                        default: await Command.GameUI.NoticeCommand.ShowAsync("注册发生异常", NotifyBoardMode.Ok); break;
                    }
                }
                catch (System.Exception e)
                {

                    Debug.LogException(e);
                }

            }
            public static async Task LoginAsync(string name, string password)
            {
                try
                {
                    Debug.Log("登陆请求");
                    if (connentHub.State == HubConnectionState.Disconnected) { await connentHub.StartAsync(); }
                    PlayerInfo playerInfo = await connentHub.InvokeAsync<PlayerInfo>("Login", name, password);
                    Info.AgainstInfo.onlineUserInfo = playerInfo;
                    Debug.Log(Info.AgainstInfo.onlineUserInfo.ToJson());
                    //await accountHub.StopAsync();
                    if (playerInfo != null)
                    {
                        await Command.BookCommand.InitAsync();
                    }
                    else
                    {
                        await Command.GameUI.NoticeCommand.ShowAsync("账号或密码错误，请重试", NotifyBoardMode.Ok);
                    }
                }
                catch (Exception e) { Debug.LogException(e); }

            }
            public static void UploadAgentSummary(AgainstSummaryManager summary)
            {
                var client = new WebSocket($"ws://{ip}/UploadAgentSummary");
                client.Connect();
                client.Send(summary.ToJson());
            }
            public static async Task<List<AgainstSummaryManager>> DownloadAgentSummaryAsync(string playerName, int skipCount, int takeCount)
            {
                bool isReceive = false;
                var client = new WebSocket($"ws://{ip}/DownloadAgentSummary");
                List<AgainstSummaryManager> summarys = new List<AgainstSummaryManager>();
                client.OnMessage += (sender, e) =>
                {
                    var summary = e.Data.ToObject<List<AgainstSummaryManager>>();
                    client.Close();
                    isReceive = true;
                };
                client.Connect();
                client.Send(new GeneralCommand(playerName, skipCount, takeCount).ToJson());
                while (!isReceive)
                {
                    TakeLoopManager.cancel.Token.ThrowIfCancellationRequested();
                    await Task.Delay(10);
                }
                return summarys;
            }

            internal static async Task<string> GetCardConfigsVersionAsync()
            {
                if (connentHub.State == HubConnectionState.Disconnected) { await connentHub.StartAsync(); }
                return await connentHub.InvokeAsync<string>("GetCardConfigsVersion");
            }

            public static async Task UploadCardConfigsAsync(CardConfig cardConfig)
            {
                try
                {
                    Debug.Log("上传卡牌配置");
                    if (connentHub.State == HubConnectionState.Disconnected) { await connentHub.StartAsync(); }
                    string result = await connentHub.InvokeAsync<string>("UploadCardConfigs", cardConfig);
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
                    if (connentHub.State == HubConnectionState.Disconnected) { await connentHub.StartAsync(); }
                    return await connentHub.InvokeAsync<CardConfig>("DownloadCardConfigs", date);
                }
                catch (Exception e) { Debug.LogException(e); }
                return null;
            }
            ///////////////////////////////////////////////////用户操作////////////////////////////////////////////////////////////////
            public static void UpdateDecks(PlayerInfo playerInfo)
            {
                //Debug.Log("更新请求");
                var client = new WebSocket($"ws://{ip}/UpdateDecks");
                client.OnMessage += (sender, e) =>
                {
                    Debug.Log("收到回应: " + e.Data);
                    client.Close();
                };
                client.Connect();
                Debug.Log("连接完成");
                client.Send(playerInfo.ToJson());
            }
            public static async Task ChatAsync(string name, string text,string target="")
            {
                if (connentHub.State == HubConnectionState.Disconnected) { await connentHub.StartAsync(); }
                await connentHub.SendAsync("Chat", name, text, target);
            }
            ///////////////////////////////////////////////////房间操作////////////////////////////////////////////////////////////////
            public static async Task JoinRoomAsync(MultiplayerModeType rank)
            {
                Debug.Log("登录请求");
                bool isReceive = false;
                PlayerInfo userInfo = null;
                PlayerInfo opponentInfo = null;
                var client = new WebSocket($"ws://{ip}/Join");
                client.OnMessage += (sender, e) =>
                {
                    Debug.LogError("收到了来自服务器的初始信息" + e.Data);
                    object[] ReceiveInfo = e.Data.ToObject<GeneralCommand>().Datas;
                    Info.AgainstInfo.RoomID = int.Parse(ReceiveInfo[0].ToString());
                    Debug.Log("房间号为" + Info.AgainstInfo.RoomID);
                    Info.AgainstInfo.isPlayer1 = (bool)ReceiveInfo[1];
                    Debug.Log("是否玩家1？：" + Info.AgainstInfo.isPlayer1);
                    Info.AgainstInfo.isPVP = true;
                    userInfo = ReceiveInfo[2].ToString().ToObject<PlayerInfo>();
                    opponentInfo = ReceiveInfo[3].ToString().ToObject<PlayerInfo>();
                    Debug.Log("收到回应: " + e.Data);
                    isReceive = true;
                };
                client.Connect();
                client.Send(Info.AgainstInfo.currentUserInfo.ToJson());
                while (!isReceive)
                {
                    TakeLoopManager.cancel.Token.ThrowIfCancellationRequested();
                    await Task.Delay(10);
                }
                AgainstManager.Init();
                AgainstManager.SetPlayerInfo(userInfo);
                AgainstManager.SetOpponentInfo(opponentInfo);
                AgainstManager.SetPvPMode(true);
                AgainstManager.SetTurnFirst(FirstTurn.Random);
                await AgainstManager.Start();
                //创建联机连接
                InitAsyncConnection();
                //Debug.Log(Info.AgainstInfo.currentUserInfo.ToJson());
                //Debug.Log("发送完毕");
            }
            public static void LeaveRoom()
            {
                Debug.Log("登录请求");
                var client = new WebSocket($"ws://{ip}/Leave");
                client.OnMessage += (sender, e) =>
                {
                    Debug.LogError("已离开房间" + e.Data);
                };
                client.Connect();
                Debug.Log("连接完成");
                client.Send(new GeneralCommand(Info.AgainstInfo.RoomID, Info.AgainstInfo.currentUserInfo._id).ToJson());
                Debug.Log(Info.AgainstInfo.RoomID.ToJson());
                Debug.Log("发送完毕");
            }
            //初始化接收响应
            private static void InitAsyncConnection()
            {
                AsyncConnect = new WebSocket($"ws://{ip}/AsyncInfo");
                AsyncConnect.Connect();
                AsyncConnect.OnMessage += async (sender, e) =>
                {
                    try
                    {
                        Debug.Log("收到信息" + e.Data);
                        object[] receiveInfo = e.Data.ToObject<GeneralCommand>().Datas;
                        NetAcyncType Type = (NetAcyncType)int.Parse(receiveInfo[0].ToString());
                        switch (Type)
                        {
                            case NetAcyncType.FocusCard:
                                {
                                    int X = int.Parse(receiveInfo[2].ToString());
                                    int Y = int.Parse(receiveInfo[3].ToString());
                                    AgainstInfo.opponentFocusCard = RowsInfo.GetCard(X, Y);
                                    break;
                                }
                            case NetAcyncType.PlayCard:
                                {
                                    //Debug.Log("触发卡牌同步");
                                    int X = int.Parse(receiveInfo[2].ToString());
                                    int Y = int.Parse(receiveInfo[3].ToString());
                                    Card targetCard = RowsInfo.GetCard(X, Y);
                                    Info.AgainstInfo.playerPlayCard = targetCard;
                                    //await GameSystem.TransSystem.PlayCard(new TriggerInfo(targetCard).SetTargetCard(targetCard), false);
                                    //AgainstInfo.IsCardEffectCompleted = true;
                                    break;
                                }
                            case NetAcyncType.SelectRegion:
                                {
                                    //Debug.Log("触发区域同步");
                                    int X = int.Parse(receiveInfo[2].ToString());
                                    AgainstInfo.SelectRegion = Info.RowsInfo.GetSingleRowInfoById(X);
                                    break;
                                }
                            case NetAcyncType.SelectUnites:
                                {
                                    //Debug.Log("收到同步单位信息为" + rawData);
                                    List<Location> Locations = receiveInfo[2].ToString().ToObject<List<Location>>();
                                    AgainstInfo.selectUnits.AddRange(Locations.Select(location => RowsInfo.GetCard(location.X, location.Y)));
                                    break;
                                }
                            case NetAcyncType.SelectLocation:
                                {
                                    Debug.Log("触发坐标同步");
                                    int X = int.Parse(receiveInfo[2].ToString());
                                    int Y = int.Parse(receiveInfo[3].ToString());
                                    //Info.RowsInfo.SingleRowInfos.First(infos => infos.ThisRowCard == Info.RowsInfo.GlobalCardList[X]);
                                    Info.AgainstInfo.SelectRegion = Info.RowsInfo.GetSingleRowInfoById(X);
                                    Info.AgainstInfo.SelectLocation = Y;
                                    Debug.Log($"坐标为：{X}:{Y}");
                                    Debug.Log($"信息为：{"gezi"}:{Info.AgainstInfo.SelectLocation}");
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
                                    await StateCommand.AgainstEnd(true, true);
                                    break;
                                }
                            case NetAcyncType.ExchangeCard:
                                {
                                    Debug.Log("交换卡牌信息");
                                    // Debug.Log("收到信息" + rawData);
                                    Location location = receiveInfo[2].ToString().ToObject<Location>();
                                    int randomRank = int.Parse(receiveInfo[3].ToString());
                                    _ = CardCommand.ExchangeCard(RowsInfo.GetCard(location), IsPlayerExchange: false, RandomRank: randomRank);
                                    break;
                                }
                            case NetAcyncType.RoundStartExchangeOver:
                                if (AgainstInfo.isPlayer1)
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
                                    AgainstInfo.SelectProperty = (BattleRegion)int.Parse(receiveInfo[2].ToString());
                                    Debug.Log("通过网络同步当前属性为" + Info.AgainstInfo.SelectProperty);
                                    break;
                                }
                            case NetAcyncType.SelectBoardCard:
                                {
                                    AgainstInfo.selectBoardCardRanks = receiveInfo[2].ToString().ToObject<List<int>>(); ;
                                    AgainstInfo.IsSelectCardOver = (bool)receiveInfo[3];
                                    break;
                                }
                            case NetAcyncType.Init:
                                break;

                            default:
                                break;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.Log(ex);
                        throw;
                    }

                };
                AsyncConnect.OnError += (sender, e) =>
                {
                    Debug.Log("连接失败" + e.Message);
                    Debug.Log("连接失败" + e.Exception);
                };
                Debug.LogError("初始化数据" + new GeneralCommand(NetAcyncType.Init, Info.AgainstInfo.RoomID, Info.AgainstInfo.isPlayer1).ToJson());
                AsyncConnect.Send(new GeneralCommand(NetAcyncType.Init, Info.AgainstInfo.RoomID, Info.AgainstInfo.isPlayer1).ToJson());

            }
            //数据同步类型
            public static void AsyncInfo(NetAcyncType AcyncType)
            {
                if (Info.AgainstInfo.isPVP && (Info.AgainstInfo.isMyTurn || AcyncType == NetAcyncType.FocusCard || AcyncType == NetAcyncType.ExchangeCard || AcyncType == NetAcyncType.RoundStartExchangeOver))
                {
                    switch (AcyncType)
                    {
                        case NetAcyncType.FocusCard:
                            {
                                Location TargetCardLocation = Info.AgainstInfo.playerFocusCard != null ? Info.AgainstInfo.playerFocusCard.location : new Location(-1, -1);
                                AsyncConnect.Send(new GeneralCommand(AcyncType, Info.AgainstInfo.RoomID, TargetCardLocation.X, TargetCardLocation.Y).ToJson());
                                break;
                            }
                        case NetAcyncType.PlayCard:
                            {
                                Debug.Log("发出同步");
                                Location TargetCardLocation = Info.AgainstInfo.playerPlayCard.location;
                                AsyncConnect.Send(new GeneralCommand(AcyncType, Info.AgainstInfo.RoomID, TargetCardLocation.X, TargetCardLocation.Y).ToJson());

                                break;
                            }
                        case NetAcyncType.SelectRegion:
                            {
                                int RowRank = Info.AgainstInfo.SelectRegion.RowRank;
                                Debug.Log("同步焦点区域为" + RowRank);
                                AsyncConnect.Send(new GeneralCommand(AcyncType, Info.AgainstInfo.RoomID, RowRank).ToJson());

                                break;
                            }
                        case NetAcyncType.SelectUnites:
                            {
                                List<Location> Locations = Info.AgainstInfo.selectUnits.SelectList(unite => unite.location);
                                Debug.LogError("发出的指令为：" + new GeneralCommand(AcyncType, Info.AgainstInfo.RoomID, Locations).ToJson());
                                AsyncConnect.Send(new GeneralCommand(AcyncType, Info.AgainstInfo.RoomID, Locations.ToJson()).ToJson());
                                Debug.LogError("选择单位完成");
                                break;
                            }
                        case NetAcyncType.SelectLocation:
                            {
                                int RowRank = Info.AgainstInfo.SelectRegion.RowRank;
                                int LocationRank = Info.AgainstInfo.SelectLocation;
                                Debug.Log("同步焦点坐标给对面：" + RowRank);
                                AsyncConnect.Send(new GeneralCommand(AcyncType, Info.AgainstInfo.RoomID, RowRank, LocationRank).ToJson());
                                break;
                            }
                        case NetAcyncType.ExchangeCard:
                            {
                                Debug.Log("触发交换卡牌信息");
                                Location Locat = Info.AgainstInfo.TargetCard.location;
                                int RandomRank = Info.AgainstInfo.washInsertRank;
                                AsyncConnect.Send(new GeneralCommand(AcyncType, Info.AgainstInfo.RoomID, Locat.ToJson(), RandomRank).ToJson());
                                break;
                            }
                        case NetAcyncType.RoundStartExchangeOver:
                            Debug.Log("触发回合开始换牌完成信息");
                            AsyncConnect.Send(new GeneralCommand(AcyncType, Info.AgainstInfo.RoomID).ToJson());
                            break;
                        case NetAcyncType.Pass:
                            {
                                Debug.Log("pass");
                                AsyncConnect.Send(new GeneralCommand(AcyncType, Info.AgainstInfo.RoomID).ToJson());
                                break;
                            }
                        case NetAcyncType.Surrender:
                            {
                                Debug.Log("投降");
                                AsyncConnect.Send(new GeneralCommand(AcyncType, Info.AgainstInfo.RoomID).ToJson());
                                break;
                            }
                        case NetAcyncType.SelectProperty:
                            {
                                Debug.Log("选择场地属性");
                                AsyncConnect.Send(new GeneralCommand(AcyncType, Info.AgainstInfo.RoomID, Info.AgainstInfo.SelectProperty).ToJson());
                                break;
                            }

                        case NetAcyncType.Init:
                            break;
                        case NetAcyncType.SelectBoardCard:
                            Debug.Log("同步面板卡牌数据选择");
                            AsyncConnect.Send(new GeneralCommand(AcyncType, Info.AgainstInfo.RoomID, Info.AgainstInfo.selectBoardCardRanks, Info.AgainstInfo.IsSelectCardOver).ToJson());
                            break;
                        default:
                            {
                                Debug.Log("异常同步指令");
                            }
                            break;
                    }
                }
            }
        }
    }
}