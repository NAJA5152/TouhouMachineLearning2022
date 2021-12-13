using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Command
{
    class DeckBoardCommand
    {
        public static void Init()
        {
            var s = Info.AgainstInfo.onlineUserInfo;
            Info.CardCompnentInfo.seleceDeckRank = Info.AgainstInfo.onlineUserInfo.UseDeckNum;
            Info.CardCompnentInfo.tempDeck = Info.AgainstInfo.onlineUserInfo.UseDeck.Clone();
            Info.CardCompnentInfo.instance.deckModel.SetActive(false);

            var decks = Info.AgainstInfo.onlineUserInfo.Decks;
            var deckModel = Info.CardCompnentInfo.instance.deckModel;
            var deckModels = Info.CardCompnentInfo.instance.deckModels;
            deckModels.ForEach(model => model.SetActive(false));

            Debug.LogWarning(deckModels.Count + "-" + decks.Count);
            if (decks.Count > deckModels.Count - 1)
            {
                int num = decks.Count - (deckModels.Count - 1);
                for (int i = 0; i < num; i++)
                {
                    deckModels.Insert(deckModels.Count, UnityEngine.Object.Instantiate(deckModel, deckModel.transform.parent));
                }
            }
            for (int i = 0; i < decks.Count; i++)
            {
                deckModels[i].SetActive(true);
                //更新卡组信息
                deckModels[i].transform.GetChild(1).GetComponent<Text>().text = decks[i].DeckName;
                var cardInfo = Manager.CardAssemblyManager.GetCurrentCardInfos(decks[i].LeaderId);
                Sprite cardTex = cardInfo.icon.ToSprite();
                deckModels[i].transform.GetComponent<Image>().sprite = cardTex;
            }
            Info.CardCompnentInfo.values.Clear();
            for (int i = 0; i < decks.Count; i++)
            {
                Info.CardCompnentInfo.values.Add(Info.CardCompnentInfo.bias + i * Info.CardCompnentInfo.fre);
            }
        }
        public static async void OnDeckClick(GameObject deck)
        {
            int selectRank = Info.CardCompnentInfo.instance.deckModels.IndexOf(deck);
            if (Info.CardCompnentInfo.seleceDeckRank != selectRank)
            {
                Info.CardCompnentInfo.seleceDeckRank = selectRank;
                Info.CardCompnentInfo.isCardClick = true;
                Info.AgainstInfo.onlineUserInfo.UseDeckNum = Info.CardCompnentInfo.seleceDeckRank;
                //Command.Network.NetCommand.UpdateDecksAsync(Info.AgainstInfo.onlineUserInfo);
                //await Command.Network.NetCommand.UpdateInfoAsync(UpdateType.Decks, Info.AgainstInfo.onlineUserInfo.Decks);
                //await Command.Network.NetCommand.UpdateInfoAsync(UpdateType.UseDeckNum, Info.AgainstInfo.onlineUserInfo.UseDeckNum);
                await Info.AgainstInfo.onlineUserInfo.UpdateDecksAsync();
                Command.DeckBoardCommand.Init();
                Command.CardListCommand.Init();
                Debug.LogWarning("点击修改为" + Info.CardCompnentInfo.seleceDeckRank);
            }
        }
        public static void UpdateDeckPosition()
        {
            if (Info.CardCompnentInfo.instance.content.gameObject.activeInHierarchy)
            {
                Info.CardCompnentInfo.show = Info.CardCompnentInfo.instance.content.GetComponent<RectTransform>().localPosition.x;
                for (int i = 0; i < Info.CardCompnentInfo.values.Count; i++)
                {
                    Info.CardCompnentInfo.values[i] = Info.CardCompnentInfo.bias + i * Info.CardCompnentInfo.fre;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    Info.CardCompnentInfo.isDragMode = true;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (!Info.CardCompnentInfo.isCardClick)
                    {
                        float selectValue = Info.CardCompnentInfo.values.OrderBy(value => Mathf.Abs(value - Info.CardCompnentInfo.instance.content.GetComponent<RectTransform>().localPosition.x)).First();

                        GameObject deck = Info.CardCompnentInfo.instance.deckModels[Info.CardCompnentInfo.values.IndexOf(selectValue)];
                        OnDeckClick(deck);
                    }
                    else
                    {
                        Info.CardCompnentInfo.isCardClick = false;
                    }
                    Info.CardCompnentInfo.isDragMode = false;
                }
                if (!Info.CardCompnentInfo.isDragMode)
                {
                    Vector3 end = new Vector3(Info.CardCompnentInfo.values[Info.CardCompnentInfo.seleceDeckRank], 120, 0);
                    Info.CardCompnentInfo.instance.content.GetComponent<RectTransform>().localPosition = Vector3.Lerp(Info.CardCompnentInfo.instance.content.GetComponent<RectTransform>().localPosition, end, Time.deltaTime * 3);
                }
            }
        }
        public static async void CreatDeck()
        {
            Info.AgainstInfo.onlineUserInfo.Decks.Add(new Model.CardDeck("新卡组", 20002, new List<int> { 20002, 20001, 20001 }));
            Info.AgainstInfo.onlineUserInfo.UseDeckNum = Info.AgainstInfo.onlineUserInfo.Decks.Count - 1;
            //将牌库设为可编辑模式
            Info.CardCompnentInfo.isEditDeckMode = true;
            Debug.Log("切换到deck" + Info.AgainstInfo.onlineUserInfo.UseDeckNum);
            //Command.Network.NetCommand.UpdateDecksAsync(Info.AgainstInfo.onlineUserInfo);
            //await Command.Network.NetCommand.UpdateInfoAsync(UpdateType.Decks, Info.AgainstInfo.onlineUserInfo.Decks);
            //await Command.Network.NetCommand.UpdateInfoAsync(UpdateType.UseDeckNum, Info.AgainstInfo.onlineUserInfo.UseDeckNum);
            await Info.AgainstInfo.onlineUserInfo.UpdateDecksAsync();
            Command.DeckBoardCommand.Init();
            Command.CardListCommand.Init();
            //切换状态至牌库
            Command.MenuStateCommand.RebackStare();
            Command.MenuStateCommand.AddState(MenuState.CardListChange);
        }
        public static void DeleteDeck()
        {
            _ = Command.GameUI.NoticeCommand.ShowAsync("删除卡组", NotifyBoardMode.Ok_Cancel, okAction: async () =>
            {
                if (Info.AgainstInfo.onlineUserInfo.Decks.Count > 1)
                {
                    Debug.Log("删除卡组成功");
                    Info.AgainstInfo.onlineUserInfo.Decks.Remove(Info.AgainstInfo.onlineUserInfo.UseDeck);
                    Info.AgainstInfo.onlineUserInfo.UseDeckNum = Math.Max(0, Info.AgainstInfo.onlineUserInfo.UseDeckNum - 1);
                    //Command.Network.NetCommand.UpdateDecksAsync(Info.AgainstInfo.onlineUserInfo);
                    //await Command.Network.NetCommand.UpdateInfoAsync(UpdateType.Decks, Info.AgainstInfo.onlineUserInfo.Decks);
                    //await Command.Network.NetCommand.UpdateInfoAsync(UpdateType.UseDeckNum, Info.AgainstInfo.onlineUserInfo.UseDeckNum);
                    await Info.AgainstInfo.onlineUserInfo.UpdateDecksAsync();
                    Command.DeckBoardCommand.Init();
                    Command.CardListCommand.Init();
                }
                else
                {
                    await Command.GameUI.NoticeCommand.ShowAsync("请至少保留一个卡组", NotifyBoardMode.Ok);
                }

            });
        }
        public static void RenameDeck()
        {
            _ = Command.GameUI.NoticeCommand.ShowAsync("重命名卡牌", NotifyBoardMode.Input, inputAction: async (text) =>
            {
                Debug.Log("重命名卡组为" + text);
                Info.AgainstInfo.onlineUserInfo.UseDeck.DeckName = text;
                await Task.Delay(100);
                //Command.Network.NetCommand.UpdateDecksAsync(Info.AgainstInfo.onlineUserInfo);
                //await Command.Network.NetCommand.UpdateInfoAsync(UpdateType.Decks, Info.AgainstInfo.onlineUserInfo.Decks);
                //await Command.Network.NetCommand.UpdateInfoAsync(UpdateType.UseDeckNum, Info.AgainstInfo.onlineUserInfo.UseDeckNum);
                await Info.AgainstInfo.onlineUserInfo.UpdateDecksAsync();
                Command.DeckBoardCommand.Init();
                Command.CardListCommand.Init();

            }, inputField: Info.AgainstInfo.onlineUserInfo.UseDeck.DeckName);
        }
        public static async Task StartAgainstAsync()
        {
            await Manager.CameraViewManager.MoveToViewAsync(2);
            Command.MenuStateCommand.AddState(MenuState.WaitForBattle);
            Command.BookCommand.SimulateFilpPage(true);//开始翻书
            if (Command.MenuStateCommand.HasState(MenuState.LevelSelect))//单人关卡选择模式
            {
                _ = Command.GameUI.NoticeCommand.ShowAsync("进入剧情关卡", NotifyBoardMode.Ok_Cancel, okAction: async () =>
                {
                    Command.BookCommand.SimulateFilpPage(false);//停止翻书
                    Command.MenuStateCommand.AddState(MenuState.ScenePage);
                    await Task.Delay(3000);

                    AgainstManager.Init();
                    AgainstManager.SetPvPMode(false);
                    AgainstManager.SetTurnFirst(FirstTurn.PlayerFirst);
                    Debug.Log("进入对战配置模式");
                    AgainstManager.SetPlayerInfo(Info.AgainstInfo.onlineUserInfo.GetSampleInfo());
                    AgainstManager.SetOpponentInfo(
                        new PlayerInfo(
                          "神秘的妖怪", "yaya", "",
                          new List<CardDeck>
                          {
                                new CardDeck("gezi", 20001, new List<int>
                                {
                                    20002,20003,20004,20005,
                                    20006,20007,20008,20009,20010,20011,
                                    20012,20013,20014,20015,20016,
                                    20012,20013,20014,20015,20016,
                                    20012,20013,20014,20015,20016,
                                })
                          }));
                    Debug.Log("打开切换UI");
                    //Manager.LoadingManager.manager?.OpenAsync();
                    Debug.Log("开始对战");
                    AgainstManager.Start();
                });
            }
            if (Command.MenuStateCommand.HasState(MenuState.PracticeConfig))//单人练习模式
            {
                _ = Command.GameUI.NoticeCommand.ShowAsync("生成练习对手", NotifyBoardMode.Ok_Cancel, okAction: async () =>
                {
                    AgainstManager.Init();
                    AgainstManager.SetPvPMode(false);
                    AgainstManager.SetTurnFirst(FirstTurn.PlayerFirst);
                    AgainstManager.SetPlayerInfo(new PlayerInfo(
                    "gezi", "yaya", "",
                    new List<CardDeck>
                    {
                        new CardDeck("gezi", 10001, new List<int>
                        {
                            10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,
                        })
                    }));
                    AgainstManager.SetOpponentInfo(new PlayerInfo(
                    "gezi", "yaya", "",
                    new List<CardDeck>
                    {
                        new CardDeck("gezi", 10001, new List<int>
                        {
                            10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,
                        })
                    }));
                    AgainstManager.Start();
                });
            }
            if (Command.MenuStateCommand.HasState(MenuState.CasualModeDeckSelect))//多人休闲模式
            {
                Debug.LogWarning("开始匹配休闲模式");
                _ = Command.GameUI.NoticeCommand.ShowAsync("休闲模式匹配中", NotifyBoardMode.Cancel, cancelAction: async () =>
                {
                    Command.Network.NetCommand.LeaveRoom();
                });
                await Command.Network.NetCommand.JoinRoomAsync(AgainstModeType.Casual);
            }
            if (Command.MenuStateCommand.HasState(MenuState.RankModeDeckSelect))//多人天梯模式
            {
                _ = Command.GameUI.NoticeCommand.ShowAsync("天梯模式匹配中", NotifyBoardMode.Cancel, cancelAction: async () =>
                {
                    Command.Network.NetCommand.LeaveRoom();
                });
                Command.Network.NetCommand.JoinRoomAsync(AgainstModeType.Rank);
            }
            if (Command.MenuStateCommand.HasState(MenuState.ArenaModeDeckSelect))//多人竞技场模式
            {
                _ = Command.GameUI.NoticeCommand.ShowAsync("地下竞技场模式匹配中", NotifyBoardMode.Cancel, cancelAction: async () =>
                {
                    Command.Network.NetCommand.LeaveRoom();
                });
                Command.Network.NetCommand.JoinRoomAsync(AgainstModeType.Arena);
            }
        }
    }
}
