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
            Info.AgainstInfo.onlineUserInfo.Decks.Add(new Model.CardDeck("新卡组", 2000001, new List<int> { 2001001, 2001002, 2001003, 2001004 }));
            Info.AgainstInfo.onlineUserInfo.UseDeckNum = Info.AgainstInfo.onlineUserInfo.Decks.Count - 1;
            //将牌库设为可编辑模式
            Info.CardCompnentInfo.isEditDeckMode = true;
            Debug.Log("切换到deck" + Info.AgainstInfo.onlineUserInfo.UseDeckNum);
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
            AgainstModeType targetAgainstMode= AgainstModeType.Practice;
            PlayerInfo sampleUserInfo = null;
            PlayerInfo virtualOpponentInfo = null;
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (Command.MenuStateCommand.HasState(MenuState.LevelSelect))//单人关卡选择模式
            {
                targetAgainstMode = AgainstModeType.Story;
                sampleUserInfo = Info.AgainstInfo.onlineUserInfo.GetSampleInfo();
                sampleUserInfo = new PlayerInfo(
                     "NPC", "神秘的妖怪", "yaya", "",
                     new List<CardDeck>
                     {
                                new CardDeck("gezi", 2000001, new List<int>
                                {
                                    2001001,2001002,2001003,2001004,
                                    2002001,2002002,2002003,2002004,2002005,2002006,
                                    2003001,2003002,2003003,2003004,2003005,
                                    2003001,2003002,2003003,2003004,2003005,
                                    2003001,2003002,2003003,2003004,2003005,
                                })
                     });
                virtualOpponentInfo = new PlayerInfo(
                     "NPC", "神秘的妖怪", "yaya", "",
                     new List<CardDeck>
                     {
                                new CardDeck("gezi", 2000001, new List<int>
                                {
                                    2001001,2001002,2001003,2001004,
                                    2002001,2002002,2002003,2002004,2002005,2002006,
                                    2003001,2003002,2003003,2003004,2003005,
                                    2003001,2003002,2003003,2003004,2003005,
                                    2003001,2003002,2003003,2003004,2003005,
                                })
                     });
            }
            if (Command.MenuStateCommand.HasState(MenuState.PracticeConfig))//单人练习模式
            {
                targetAgainstMode = AgainstModeType.Practice;
                sampleUserInfo = Info.AgainstInfo.onlineUserInfo.GetSampleInfo();
                virtualOpponentInfo = new PlayerInfo(
                     "NPC", "神秘的妖怪", "yaya", "",
                     new List<CardDeck>
                     {
                          new CardDeck("gezi", 10001, new List<int>
                          {
                              10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,10002,
                          })
                     });
            }
            if (Command.MenuStateCommand.HasState(MenuState.CasualModeDeckSelect))//多人休闲模式
            {
                targetAgainstMode = AgainstModeType.Practice;
                sampleUserInfo = Info.AgainstInfo.onlineUserInfo.GetSampleInfo();
            }
            if (Command.MenuStateCommand.HasState(MenuState.RankModeDeckSelect))//多人天梯模式
            {
                targetAgainstMode = AgainstModeType.Rank;
                sampleUserInfo = Info.AgainstInfo.onlineUserInfo.GetSampleInfo();
            }
            if (Command.MenuStateCommand.HasState(MenuState.ArenaModeDeckSelect))//多人竞技场模式
            {
                targetAgainstMode = AgainstModeType.Arena;
                sampleUserInfo = Info.AgainstInfo.onlineUserInfo.GetSampleInfo();
            }

            _ = Command.GameUI.NoticeCommand.ShowAsync("少女排队中~", NotifyBoardMode.Cancel, cancelAction: async () =>
            {
                Command.BookCommand.SimulateFilpPage(false);//开始翻书
                await Task.Delay(2000);
                await Manager.CameraViewManager.MoveToViewAsync(1);
                Command.MenuStateCommand.RebackStare();
                await Command.NetCommand.LeaveHoldOnList(targetAgainstMode, sampleUserInfo.Account);
            });
            //配置对战模式
            Manager.AgainstManager.Init();
            Manager.AgainstManager.SetAgainstMode(targetAgainstMode);
            //开始排队
            await Command.NetCommand.JoinHoldOnList(targetAgainstMode, sampleUserInfo, virtualOpponentInfo);
        }
    }
}
