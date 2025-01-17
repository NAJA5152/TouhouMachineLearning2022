﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
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
            Info.PageCompnentInfo.selectDeckRank = Info.AgainstInfo.onlineUserInfo.UseDeckNum;
            Info.PageCompnentInfo.tempDeck = Info.AgainstInfo.onlineUserInfo.UseDeck.Clone();
            Info.PageCompnentInfo.Instance.deckModel.SetActive(false);

            var decks = Info.AgainstInfo.onlineUserInfo.Decks;
            var deckModel = Info.PageCompnentInfo.Instance.deckModel;
            var deckModels = Info.PageCompnentInfo.Instance.deckModels;
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
            Info.PageCompnentInfo.values.Clear();
            for (int i = 0; i < decks.Count; i++)
            {
                Info.PageCompnentInfo.values.Add(Info.PageCompnentInfo.bias + i * Info.PageCompnentInfo.fre);
            }
        }
        public static async void OnDeckClick(GameObject deck)
        {
            int selectRank = Info.PageCompnentInfo.Instance.deckModels.IndexOf(deck);
            if (Info.PageCompnentInfo.selectDeckRank != selectRank)
            {
                Info.PageCompnentInfo.selectDeckRank = selectRank;
                Info.PageCompnentInfo.isCardClick = true;
                Info.AgainstInfo.onlineUserInfo.UseDeckNum = Info.PageCompnentInfo.selectDeckRank;
                await Info.AgainstInfo.onlineUserInfo.UpdateDecksAsync();
                Command.DeckBoardCommand.Init();
                Command.CardListCommand.Init();
                Debug.LogWarning("点击修改为" + Info.PageCompnentInfo.selectDeckRank);
            }
        }
        public static void UpdateDeckPosition()
        {
            if (Info.PageCompnentInfo.Instance.content.gameObject.activeInHierarchy)
            {
                Info.PageCompnentInfo.show = Info.PageCompnentInfo.Instance.content.GetComponent<RectTransform>().localPosition.x;
                for (int i = 0; i < Info.PageCompnentInfo.values.Count; i++)
                {
                    Info.PageCompnentInfo.values[i] = Info.PageCompnentInfo.bias + i * Info.PageCompnentInfo.fre;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    Info.PageCompnentInfo.isDragMode = true;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (!Info.PageCompnentInfo.isCardClick)
                    {
                        float selectValue = Info.PageCompnentInfo.values.OrderBy(value => Mathf.Abs(value - Info.PageCompnentInfo.Instance.content.GetComponent<RectTransform>().localPosition.x)).First();

                        GameObject deck = Info.PageCompnentInfo.Instance.deckModels[Info.PageCompnentInfo.values.IndexOf(selectValue)];
                        OnDeckClick(deck);
                    }
                    else
                    {
                        Info.PageCompnentInfo.isCardClick = false;
                    }
                    Info.PageCompnentInfo.isDragMode = false;
                }
                if (!Info.PageCompnentInfo.isDragMode)
                {
                    Vector3 end = new Vector3(Info.PageCompnentInfo.values[Info.PageCompnentInfo.selectDeckRank], 120, 0);
                    Info.PageCompnentInfo.Instance.content.GetComponent<RectTransform>().localPosition = Vector3.Lerp(Info.PageCompnentInfo.Instance.content.GetComponent<RectTransform>().localPosition, end, Time.deltaTime * 3);
                }
            }
        }
        public static async void CreatDeck(int LeaderId)
        {
            Info.AgainstInfo.onlineUserInfo.Decks.Add(new Model.CardDeck("新卡组", LeaderId, new List<int> { }));
            Info.AgainstInfo.onlineUserInfo.UseDeckNum = Info.AgainstInfo.onlineUserInfo.Decks.Count - 1;
            //将牌库设为可编辑模式
            Info.PageCompnentInfo.isEditDeckMode = true;
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
            _ = NoticeCommand.ShowAsync("删除卡组", NotifyBoardMode.Ok_Cancel, okAction: async () =>
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
                    await NoticeCommand.ShowAsync("请至少保留一个卡组", NotifyBoardMode.Ok);
                }
            });
        }
        public static void RenameDeck()
        {
            _ = NoticeCommand.ShowAsync("重命名卡牌", NotifyBoardMode.Input, inputAction: async (text) =>
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
            Info.PageCompnentInfo.currentAgainstMode = AgainstModeType.Practice;
            Info.AgainstInfo.FirstMode = 0;
            PlayerInfo sampleUserInfo = null;
            PlayerInfo virtualOpponentInfo = null;
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (Command.MenuStateCommand.HasState(MenuState.LevelSelect))//单人关卡选择模式
            {
                Info.PageCompnentInfo.currentAgainstMode = AgainstModeType.Story;
                string targetStage = Info.PageCompnentInfo.CurrentStage + Info.PageCompnentInfo.CurrentStep;
                sampleUserInfo = DeckConfigCommand.GetPlayerCardDeck(targetStage);
                virtualOpponentInfo = DeckConfigCommand.GetPlayerCardDeck(targetStage);
                Manager.AgainstManager.SetFirstMode(2);
                await DialogueCommand.Play(Info.PageCompnentInfo.CurrentStage, Info.PageCompnentInfo.CurrentStep);
                //播放剧情

            }
            if (Command.MenuStateCommand.HasState(MenuState.PracticeConfig))//单人练习模式
            {
                Info.PageCompnentInfo.currentAgainstMode = AgainstModeType.Practice;
                sampleUserInfo = Info.AgainstInfo.onlineUserInfo.GetSampleInfo();
                virtualOpponentInfo = DeckConfigCommand.GetPracticeCardDeck(Info.PageCompnentInfo.SelectLeader);
                Manager.AgainstManager.SetFirstMode(Info.PageCompnentInfo.SelectFirstHandMode);

            }
            if (Command.MenuStateCommand.HasState(MenuState.CasualModeDeckSelect))//多人休闲模式
            {
                Info.PageCompnentInfo.currentAgainstMode = AgainstModeType.Casual;
                sampleUserInfo = Info.AgainstInfo.onlineUserInfo.GetSampleInfo();
            }
            if (Command.MenuStateCommand.HasState(MenuState.RankModeDeckSelect))//多人天梯模式
            {
                Info.PageCompnentInfo.currentAgainstMode = AgainstModeType.Rank;
                sampleUserInfo = Info.AgainstInfo.onlineUserInfo.GetSampleInfo();
            }
            if (Command.MenuStateCommand.HasState(MenuState.ArenaModeDeckSelect))//多人竞技场模式
            {
                Info.PageCompnentInfo.currentAgainstMode = AgainstModeType.Arena;
                sampleUserInfo = Info.AgainstInfo.onlineUserInfo.GetSampleInfo();
            }

            _ = NoticeCommand.ShowAsync("少女祈祷中~", NotifyBoardMode.Cancel, cancelAction: async () =>
            {
                Command.BookCommand.SimulateFilpPage(false);//开始翻书
                await Task.Delay(2000);
                await Manager.CameraViewManager.MoveToViewAsync(1);
                Command.MenuStateCommand.RebackStare();
                await Command.NetCommand.LeaveHoldOnList(Info.PageCompnentInfo.currentAgainstMode, sampleUserInfo.Account);
            });
            //配置对战模式
            Manager.AgainstManager.Init();
            Manager.AgainstManager.SetAgainstMode(Info.PageCompnentInfo.currentAgainstMode);
            //开始排队
            await Command.NetCommand.JoinHoldOnList(Info.PageCompnentInfo.currentAgainstMode, Info.AgainstInfo.FirstMode, sampleUserInfo, virtualOpponentInfo);
        }
    }
}
