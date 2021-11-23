﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.SceneManagement;
using TouhouMachineLearningSummary.Extension;

namespace TouhouMachineLearningSummary.Command
{
    public static class StateCommand
    {
        //初始化双方状态并判断当前是否第初始回合换牌阶段
        internal static bool AgainstStateInit()
        {
            Info.CardInfo.CreatCardRank = 0;
            //加载目标回合，同步下状态
            AgainstSummaryManager.TurnOperation targetJumpTurn = AgainstInfo.summary.targetJumpTurn;
            AgainstInfo.roundRank = targetJumpTurn.roundRank;
            AgainstInfo.turnRank = targetJumpTurn.turnRank;
            AgainstInfo.totalTurnRank = targetJumpTurn.totalTurnRank;
            //判断该回合是否为预处理换牌阶段
            bool isExchangeTurn = targetJumpTurn.turnRank == 0;
            //AgainstInfo.totalTurnRank AgainstInfo.summary.targetJumpTurn.allCardList
            //根据对战中的回合数据初始化场上卡牌
            AgainstInfo.cardSet = new CardSet();
            foreach (var item in GameObject.FindGameObjectsWithTag("SingleInfo"))
            {
                SingleRowInfo singleRowInfo = item.GetComponent<SingleRowInfo>();
                AgainstInfo.cardSet.singleRowInfos.Add(singleRowInfo);
            }
            //CardSet.globalCardList = AgainstInfo.summary.targetJumpTurn.allCardList
            //    .Select(sampleCardList => sampleCardList.Select(CardCommand.CreateCard).ToList()).ToList();
            CardSet.globalCardList = targetJumpTurn.allCardList.SelectList(sampleCardList => sampleCardList.SelectList(CardCommand.CreateCard));
            AgainstInfo.cardSet[GameRegion.Leader, GameRegion.Battle].CardList.ForEach(card => card.isCanSee = true);
            AgainstInfo.cardSet[GameRegion.Hand][AgainstInfo.isReplayMode ? Orientation.All : Orientation.My].CardList.ForEach(card => card.isCanSee = true);
            AgainstInfo.isJumpMode = false;
            return isExchangeTurn;
        }
        ////////////////////////////////////////////////////对局流程指令////////////////////////////////////////////////////////////////////////////////
        public static async Task AgainstStart()
        {
            Info.CardInfo.CreatCardRank = 0;
            Manager.TakeLoopManager.Init();
            //如果不是通过配置文件启动的场景
            if (AgainstInfo.currentUserInfo == null)
            {
                await Manager.CardAssemblyManager.SetCurrentAssembly("");
                AgainstInfo.currentUserInfo = new PlayerInfo(
                    "gezi", "yaya", "",
                    new List<CardDeck>
                    {
                        new CardDeck("npc", 20001, new List<int>
                                //{
                                //    20002,20003,20004,20005,
                                //    20006,20007,20008,20009,20010,20011,
                                //    20012,20013,20014,20015,20016,
                                //    20012,20013,20014,20015,20016,
                                //    20012,20013,20014,20015,20016,
                                //})
                                {
                                    20003,10002,10002,10002,
                                    10002,10002,10002,10002,10002,10002,
                                    10002,10002,10002,10002,10002,
                                    10002,10002,10002,10002,10002,
                                    10002,10002,10002,10002,10002,
                                })
                    });
                AgainstInfo.currentOpponentInfo = new PlayerInfo(
                    "gezi", "yaya", "",
                    new List<CardDeck>
                    {
                        new CardDeck("npc", 20001, new List<int>
                                {
                                    20002,20003,20004,20005,
                                    20006,20007,20008,20009,20010,20011,
                                    20012,20013,20014,20015,20016,
                                    20012,20013,20014,20015,20016,
                                    20012,20013,20014,20015,20016,
                                })
                    });
            }
            //if (AgainstInfo.isReplayMode)
            //{
            //    AgainstInfo.currentUserInfo = AgainstInfo.isPlayer1 ? AgainstSummaryManager.player1Info : AgainstSummaryManager.player2Info;
            //    AgainstInfo.currentOpponentInfo = AgainstInfo.isPlayer1 ? AgainstSummaryManager.player2Info : AgainstSummaryManager.player1Info;
            //}
            //AgainstInfo.isMyTurn = AgainstInfo.isPlayer1;
            AgainstInfo.cardSet = new CardSet();
            foreach (var item in GameObject.FindGameObjectsWithTag("SingleInfo"))
            {
                SingleRowInfo singleRowInfo = item.GetComponent<SingleRowInfo>();
                AgainstInfo.cardSet.singleRowInfos.Add(singleRowInfo);
            }
            //可以舍去？
            //AgainstInfo.cardSet.CardList = null;

            RowCommand.SetRegionSelectable(GameRegion.None);
            await CustomThread.Delay(1500);
            Manager.LoadingManager.manager?.CloseAsync();
            //Debug.LogError("初始双方信息");
            try
            {
                //await Task.Delay(500);
                //Debug.Log("对战开始".TransUiText());
                await GameUI.UiCommand.NoticeBoardShow("对战开始".Translation());

                //初始化我方领袖卡
                Card MyLeaderCard = CardCommand.CreateCard(Info.AgainstInfo.userDeck.LeaderId);
                AgainstInfo.cardSet[Orientation.Down][GameRegion.Leader].Add(MyLeaderCard);
                MyLeaderCard.SetCardSeeAble(true);
                //初始化敌方领袖卡
                Card OpLeaderCard = CardCommand.CreateCard(Info.AgainstInfo.opponentDeck.LeaderId);
                AgainstInfo.cardSet[Orientation.Up][GameRegion.Leader].Add(OpLeaderCard);
                OpLeaderCard.SetCardSeeAble(true);
                //Debug.LogError("初始双方化牌组");
                //初始双方化牌组
                for (int i = 0; i < Info.AgainstInfo.userDeck.CardIds.Count; i++)
                {
                    Card NewCard = CardCommand.CreateCard(Info.AgainstInfo.userDeck.CardIds[i]);
                    CardSet cardSet = AgainstInfo.cardSet[Orientation.Down][GameRegion.Deck];
                    cardSet.Add(NewCard);
                }
                for (int i = 0; i < Info.AgainstInfo.opponentDeck.CardIds.Count; i++)
                {
                    Card NewCard = CardCommand.CreateCard(Info.AgainstInfo.opponentDeck.CardIds[i]);
                    AgainstInfo.cardSet[Orientation.Up][GameRegion.Deck].Add(NewCard);
                }
                await CustomThread.Delay(000);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
        public static async Task AgainstEnd(bool IsSurrender = false, bool IsWin = true)
        {
            await GameUI.UiCommand.NoticeBoardShow($"对战终止\n{AgainstInfo.ShowScore.MyScore}:{AgainstInfo.ShowScore.OpScore}");
            await Task.Delay(2000);
            //Debug.Log("释放线程资源");
            TakeLoopManager.cancel.Cancel();
            //AgainstInfo.summary.Explort();
            //AgainstInfo.summary.Upload();
            SceneManager.LoadSceneAsync(0);
        }
        public static async Task RoundStart()
        {
            ReSetPassState();
            await GameUI.UiCommand.NoticeBoardShow($"第{AgainstInfo.roundRank}小局开始");
            //await GameSystem.SelectSystem.SelectProperty();
            //await Task.Delay(2000);
            switch (AgainstInfo.roundRank)
            {
                case (1):
                    {
                        Info.AgainstInfo.ExChangeableCardNum = 0;
                        //Info.AgainstInfo.ExChangeableCardNum = 3;
                        Info.GameUI.UiInfo.CardBoardTitle = "剩余抽卡次数为".Translation() + Info.AgainstInfo.ExChangeableCardNum;
                        for (int i = 0; i < 10; i++)
                        {
                            await CardCommand.DrawCard(isPlayerDraw: true, isOrder: false);
                        }
                        for (int i = 0; i < 10; i++)
                        {
                            await CardCommand.DrawCard(isPlayerDraw: false, isOrder: false);
                        }
                        CardCommand.OrderCard();
                        break;
                    }
                case (2):
                    {
                        Info.AgainstInfo.ExChangeableCardNum += 1;
                        Info.GameUI.UiInfo.CardBoardTitle = "剩余抽卡次数为" + Info.AgainstInfo.ExChangeableCardNum;
                        await CardCommand.DrawCard();
                        await CardCommand.DrawCard(false);
                        break;
                    }
                case (3):
                    {
                        Info.AgainstInfo.ExChangeableCardNum += 1;
                        Info.GameUI.UiInfo.CardBoardTitle = "剩余抽卡次数为" + Info.AgainstInfo.ExChangeableCardNum;
                        await CardCommand.DrawCard();
                        await CardCommand.DrawCard(false);
                        break;
                    }
                default:
                    break;
            }
            await CustomThread.Delay(2500);
            //Debug.LogWarning("等待换牌选择");

        }
        public static async Task RoundEnd()
        {
            await GameUI.UiCommand.NoticeBoardShow($"第{AgainstInfo.roundRank}小局结束\n{AgainstInfo.TotalDownPoint}:{AgainstInfo.TotalUpPoint}\n{((AgainstInfo.TotalDownPoint > AgainstInfo.TotalUpPoint) ? "Win" : "Lose")}");
            //await Task.Delay(2000);
            int result = 0;
            if (AgainstInfo.TotalPlayer1Point > AgainstInfo.TotalPlayer2Point)
            {
                result = 1;
            }
            else if (AgainstInfo.TotalPlayer1Point < AgainstInfo.TotalPlayer2Point)
            {
                result = 2;
            }
            AgainstInfo.PlayerScore.P1Score += result == 0 || result == 1 ? 1 : 0;
            AgainstInfo.PlayerScore.P2Score += result == 0 || result == 2 ? 1 : 0;
            await Task.Delay(3500);
            await GameSystem.ProcessSystem.WhenRoundEnd();
        }
        public static async Task TurnStart()
        {
            AgainstInfo.summary.AddTurn();
            await GameUI.UiCommand.NoticeBoardShow((AgainstInfo.isMyTurn ? "我方回合开始" : "对方回合开始").Translation());
            RowCommand.SetPlayCardMoveFree(AgainstInfo.isMyTurn);
            await CustomThread.Delay(1000);

        }
        public static async Task TurnEnd()
        {

            RowCommand.SetPlayCardMoveFree(false);
            await GameUI.UiCommand.NoticeBoardShow((AgainstInfo.isMyTurn ? "我方回合结束" : "对方回合结束").Translation());
            await GameSystem.ProcessSystem.WhenTurnEnd();
            await CustomThread.Delay(1000);
            AgainstInfo.isMyTurn = !AgainstInfo.isMyTurn;
        }
        ////////////////////////////////////////////////////玩家操作指令////////////////////////////////////////////////////////////////////////////////
        public static void SetCurrentPass()
        {
            Info.GameUI.UiInfo.MyPass.SetActive(true);
            switch (Info.AgainstInfo.isMyTurn)
            {
                case true: Info.AgainstInfo.isDownPass = true; break;
                case false: Info.AgainstInfo.isUpPass = true; break;
            }
        }
        public static void ReSetPassState()
        {
            Info.GameUI.UiInfo.MyPass.SetActive(false);
            Info.GameUI.UiInfo.OpPass.SetActive(false);
            Info.AgainstInfo.isUpPass = false;
            Info.AgainstInfo.isDownPass = false;
        }
        public static async Task Surrender()
        {
            Debug.Log("投降");
            Command.Network.NetCommand.AsyncInfo(NetAcyncType.Surrender);
            AgainstInfo.summary.AddSurrender(AgainstInfo.isPlayer1);
            await AgainstEnd(true, false);
        }
        ////////////////////////////////////////////////////等待操作指令////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 等待玩家进行操作
        /// 1：玩家主动
        /// 2：由网络同步的来自对方的操作
        /// 3：Ai自动判断的操作
        /// 4：玩家主动操作超时导致的AI自动操作
        /// 5：由记录复现操作
        /// </summary>
        /// <returns></returns>
        public static async Task WaitForPlayerOperation()
        {
            //先开始计时，然后等待
            Timer.SetIsTimerStart(6000);
            Debug.LogWarning("等待玩家操作");
            //初始化要打出/放弃/过牌操作
            while (true)
            {
                //如果是录播模式，则通过制定的对局数据获取操作记录
                if (AgainstInfo.isReplayMode)
                {
                    var operation = AgainstInfo.summary.GetCurrentPlayerOperation();
                    if (operation.operation.OneHotToEnum<PlayerOperationType>() == PlayerOperationType.PlayCard)
                    {
                        Info.AgainstInfo.playerPlayCard = Info.AgainstInfo.cardSet[Orientation.My][GameRegion.Hand].CardList[operation.selectCardIndex];
                    }
                    else if (operation.operation.OneHotToEnum<PlayerOperationType>() == PlayerOperationType.DisCard)
                    {
                        Info.AgainstInfo.playerPlayCard = Info.AgainstInfo.cardSet[Orientation.My][GameRegion.Hand].CardList[operation.selectCardIndex];
                    }
                    else if (operation.operation.OneHotToEnum<PlayerOperationType>() == PlayerOperationType.Pass)
                    {
                        AgainstInfo.isPlayerPass = true;
                    }
                    else
                    {
                        Debug.LogError("对战记录识别出现严重bug");
                        throw new Exception("");
                    }
                }
                else
                {
                    //若果当前模式满足ai操作，则交由ai处理
                    if (AgainstInfo.isAIControl)
                    {
                        await AiCommand.TempPlayOperation();
                    }
                    else
                    {
                        //如果是我的回合则等待玩家操作，否则等待网络同步对方操作
                        if (AgainstInfo.isMyTurn)
                        {

                        }
                        else
                        {

                        }
                    }
                }
                //如果当前回合出牌
                if (Info.AgainstInfo.playerPlayCard != null)
                {
                    Debug.Log("当前打出了牌");
                    Info.AgainstInfo.summary.AddPlayerOperation(Manager.PlayerOperationType.PlayCard, Info.AgainstInfo.cardSet[Orientation.My][GameRegion.Hand].CardList, Info.AgainstInfo.playerPlayCard);
                    //假如是我的回合，则广播操作给对方，否则只接收操作不广播
                    await GameSystem.TransSystem.PlayCard(new TriggerInfo(null).SetTargetCard(Info.AgainstInfo.playerPlayCard), AgainstInfo.isMyTurn);
                    Debug.Log("打出效果执行完毕");

                    break;
                }
                //如果当前回合弃牌
                if (Info.AgainstInfo.playerDisCard != null)
                {
                    Info.AgainstInfo.summary.AddPlayerOperation(Manager.PlayerOperationType.DisCard, Info.AgainstInfo.cardSet[Orientation.My][GameRegion.Hand].CardList, Info.AgainstInfo.playerDisCard);
                    await GameSystem.TransSystem.DisCard(new TriggerInfo(null).SetTargetCard(Info.AgainstInfo.playerDisCard));
                    break;
                }
                if (AgainstInfo.isCurrectPass)//如果当前pass则结束回合
                {
                    break;
                }
                else
                {
                    if (AgainstInfo.isPlayerPass)
                    {
                        Info.AgainstInfo.summary.AddPlayerOperation(PlayerOperationType.Pass, Info.AgainstInfo.cardSet[Orientation.My][GameRegion.Hand].CardList, null);
                        SetCurrentPass();
                        AgainstInfo.isPlayerPass = false;
                        break;
                    }
                }
                await Task.Delay(10);
                TakeLoopManager.cancel.Token.ThrowIfCancellationRequested();
            }
            Timer.SetIsTimerClose();
        }
        public static async Task WaitForSelectProperty()
        {
            //放大硬币
            CoinManager.ScaleUp();
            await CustomThread.Delay(1000);
            CoinManager.Unfold();
            AgainstInfo.IsWaitForSelectProperty = true;
            AgainstInfo.SelectProperty = BattleRegion.None;
            //暂时设为1秒，之后还原成10秒
            Timer.SetIsTimerStart(1);
            //AgainstInfo.SelectRegion = null;
            // Debug.Log("等待选择属性");
            while (AgainstInfo.SelectProperty == BattleRegion.None)
            {
                TakeLoopManager.cancel.Token.ThrowIfCancellationRequested();
                if (AgainstInfo.isAIControl)
                {
                    //Debug.Log("自动选择属性");
                    int rowRank = AiCommand.GetRandom(0, 4);
                    await CoinManager.ChangePropertyAsync((BattleRegion)rowRank);
                }
                await Task.Delay(1);
            }
            Command.Network.NetCommand.AsyncInfo(NetAcyncType.SelectProperty);
            Timer.SetIsTimerClose();
            AgainstInfo.IsWaitForSelectProperty = false;
            await CustomThread.Delay(1000);
            CoinManager.ScaleDown();
        }
        public static async Task WaitForPlayerExchange()
        {
            AgainstInfo.isRoundStartExchange = true;
            await WaitForSelectBoardCard(null, AgainstInfo.cardSet[Orientation.Down][GameRegion.Hand].CardList, CardBoardMode.ExchangeCard);
            AgainstInfo.isRoundStartExchange = false;
        }
        public static async Task WaitForSelectRegion(Card triggerCard, GameRegion regionTypes, Territory territory)
        {
            AgainstInfo.IsWaitForSelectRegion = true;
            AgainstInfo.SelectRegion = null;
            RowCommand.SetRegionSelectable(regionTypes, territory);
            while (Info.AgainstInfo.SelectRegion == null)
            {
                TakeLoopManager.cancel.Token.ThrowIfCancellationRequested();
                if (AgainstInfo.isReplayMode)
                {
                    var operation = AgainstInfo.summary.GetCurrentSelectOperation();
                    if (operation.operation.OneHotToEnum<SelectOperationType>() == SelectOperationType.SelectRegion)
                    {
                        List<SingleRowInfo> rows = AgainstInfo.cardSet.singleRowInfos.Where(row => row.CanBeSelected).ToList();
                        int rowRank = operation.selectRegionRank;
                        AgainstInfo.SelectRegion = rows[rowRank];
                    }
                    else
                    {
                        Debug.LogError("对战记录识别出现严重bug");
                        throw new Exception("");
                    }
                }
                else if (AgainstInfo.isAIControl)
                {
                    await CustomThread.Delay(1000);
                    List<SingleRowInfo> rows = AgainstInfo.cardSet.singleRowInfos.Where(row => row.CanBeSelected).ToList();
                    int rowRank = AiCommand.GetRandom(0, rows.Count());
                    AgainstInfo.SelectRegion = rows[rowRank];//设置部署区域
                }
                await Task.Delay(1);
            }
            Network.NetCommand.AsyncInfo(NetAcyncType.SelectRegion);
            AgainstInfo.summary.AddSelectOperation(SelectOperationType.SelectRegion, triggerCard);
            RowCommand.SetRegionSelectable(GameRegion.None);
            AgainstInfo.IsWaitForSelectRegion = false;
        }
        public static async Task WaitForSelectLocation(Card triggerCard, BattleRegion region, Territory territory)
        {
            AgainstInfo.IsWaitForSelectLocation = true;
            //设置指定坐标可选
            RowCommand.SetRegionSelectable((GameRegion)region, territory);
            AgainstInfo.SelectLocation = -1;
            while (AgainstInfo.SelectLocation < 0)
            {
                TakeLoopManager.cancel.Token.ThrowIfCancellationRequested();
                if (AgainstInfo.isReplayMode)
                {
                    var operation = AgainstInfo.summary.GetCurrentSelectOperation();
                    if (operation.operation.OneHotToEnum<SelectOperationType>() == SelectOperationType.SelectLocation)
                    {
                        List<SingleRowInfo> rows = AgainstInfo.cardSet.singleRowInfos.Where(row => row.CanBeSelected).ToList();
                        AgainstInfo.SelectRegion = rows[operation.selectRegionRank];
                        AgainstInfo.SelectLocation = operation.selectLocation;
                    }
                    else
                    {
                        Debug.LogError("对战记录识别出现严重bug");
                        throw new Exception("");
                    }
                }
                else if (AgainstInfo.isAIControl)
                {
                    await CustomThread.Delay(1000);
                    List<SingleRowInfo> rows = AgainstInfo.cardSet.singleRowInfos.Where(row => row.CanBeSelected).ToList();
                    int rowRank = AiCommand.GetRandom(0, rows.Count());
                    AgainstInfo.SelectRegion = rows[rowRank];//设置部署区域
                    AgainstInfo.SelectLocation = 0;//设置部署次序
                }
                await Task.Delay(1);
            }
            Network.NetCommand.AsyncInfo(NetAcyncType.SelectLocation);
            AgainstInfo.summary.AddSelectOperation(SelectOperationType.SelectLocation, triggerCard);
            RowCommand.SetRegionSelectable(GameRegion.None);
            AgainstInfo.IsWaitForSelectLocation = false;
        }
        public static async Task WaitForSelecUnit(Card triggerCard, List<Card> filterCards, int num, bool isAuto)
        {
            //可选列表中移除自身
            filterCards.Remove(triggerCard);
            AgainstInfo.ArrowStartCard = triggerCard;
            AgainstInfo.IsWaitForSelectUnits = true;
            AgainstInfo.AllCardList.ForEach(card => card.isGray = true);
            filterCards.ForEach(card => card.isGray = false);
            AgainstInfo.selectUnits.Clear();
            //await Task.Delay(500);
            if (Info.AgainstInfo.isMyTurn && !isAuto)
            {
                GameUI.UiCommand.CreatFreeArrow();
            }
            int selectableNum = Math.Min(filterCards.Count, num);
            while (AgainstInfo.selectUnits.Count < selectableNum)
            {
                TakeLoopManager.cancel.Token.ThrowIfCancellationRequested();
                //AI操作或者我方回合自动选择模式时 ，用自身随机决定，否则等待网络同步
                if (AgainstInfo.isReplayMode)
                {
                    var operation = AgainstInfo.summary.GetCurrentSelectOperation();
                    if (operation.operation.OneHotToEnum<SelectOperationType>() == SelectOperationType.SelectUnite)
                    {
                        AgainstInfo.selectUnits = operation.selectCardRank.SelectList(index => filterCards[index]);
                    }
                    else
                    {
                        Debug.LogError("对战记录识别出现严重bug");
                        throw new Exception("");
                    }
                }
                else if (AgainstInfo.isAIControl || (isAuto && AgainstInfo.isMyTurn))
                {
                    Debug.Log("自动选择场上单位");
                    AgainstInfo.selectUnits = filterCards.OrderBy(x => AiCommand.GetRandom(0, 514)).Take(selectableNum).ToList();
                }
                await Task.Delay(1);
            }
            //Debug.Log("选择单位完毕" + Math.Min(Cards.Count, num));
            Network.NetCommand.AsyncInfo(NetAcyncType.SelectUnites);
            AgainstInfo.summary.AddSelectOperation(SelectOperationType.SelectUnite, triggerCard, filterCards, num);
            GameUI.UiCommand.DestoryAllArrow();
            await CustomThread.Delay(250);
            //Debug.Log("同步选择单位完毕");
            AgainstInfo.AllCardList.ForEach(card => card.isGray = false);
            AgainstInfo.IsWaitForSelectUnits = false;
            //Debug.Log("结束选择单位");
        }
        public static async Task WaitForSelectBoardCard<T>(Card triggerCard, List<T> cardIds, CardBoardMode mode = CardBoardMode.Select, int num = 1)
        {
            AgainstInfo.selectBoardCardRanks = new List<int>();
            AgainstInfo.IsSelectCardOver = false;
            AgainstInfo.cardBoardMode = mode;
            GameUI.UiCommand.SetCardBoardShow();
            if (typeof(T) == typeof(Card))
            {
                GameUI.CardBoardCommand.LoadBoardCardList(cardIds.Cast<Card>().ToList());
            }
            else
            {
                GameUI.CardBoardCommand.LoadBoardCardList(cardIds.Cast<int>().ToList());
            }
            //Debug.Log("进入选择模式");
            switch (mode)
            {
                case CardBoardMode.Select:
                    //while (AgainstInfo.selectBoardCardRanks.Count < Mathf.Min(cardIds.Count, num) && !AgainstInfo.IsFinishSelectBoardCard)
                    while (AgainstInfo.selectBoardCardRanks.Count < Mathf.Min(cardIds.Count, num) && !AgainstInfo.IsSelectCardOver)
                    {
                        await Task.Delay(1);
                    }
                    GameUI.UiCommand.SetCardBoardHide();
                    break;
                case CardBoardMode.ExchangeCard:
                    {
                        AiCommand.RoundStartExchange(false);
                        while (Info.AgainstInfo.ExChangeableCardNum != 0 && !Info.AgainstInfo.IsSelectCardOver)
                        {
                            TakeLoopManager.cancel.Token.ThrowIfCancellationRequested();
                            if (Info.AgainstInfo.selectBoardCardRanks.Count > 0)
                            {
                                //List<Card> CardLists = CardIds.Cast<Card>().ToList();
                                List<Card> CardLists = AgainstInfo.cardSet[Orientation.Down][GameRegion.Hand].CardList;
                                int selectRank = AgainstInfo.selectBoardCardRanks[0];
                                //卡牌记录出现问题？？？明天修
                                AgainstInfo.summary.AddSelectOperation(SelectOperationType.SelectBoardCard, triggerCard, CardLists, 1);
                                await CardCommand.ExchangeCard(CardLists[selectRank], isRoundStartExchange: true);
                                Info.AgainstInfo.ExChangeableCardNum--;
                                Info.AgainstInfo.selectBoardCardRanks.Clear();
                                GameUI.UiCommand.SetCardBoardTitle("剩余抽卡次数为" + Info.AgainstInfo.ExChangeableCardNum);

                            }
                            await Task.Delay(10);
                        }
                        if (AgainstInfo.isPlayer1)
                        {
                            AgainstInfo.isPlayer1RoundStartExchangeOver = true;

                        }
                        else
                        {
                            AgainstInfo.isPlayer2RoundStartExchangeOver = true;
                        }
                        GameUI.UiCommand.SetCardBoardHide();
                        Network.NetCommand.AsyncInfo(NetAcyncType.RoundStartExchangeOver);

                        bool isAlerdlySummaryPlayer1ExchangeOver = false;
                        bool isAlerdlySummaryPlayer2ExchangeOver = false;
                        while (true)
                        {
                            TakeLoopManager.cancel.Token.ThrowIfCancellationRequested();

                            if (isAlerdlySummaryPlayer1ExchangeOver && isAlerdlySummaryPlayer2ExchangeOver)//退出流程
                            {
                                if (AgainstInfo.isPlayer1RoundStartExchangeOver && AgainstInfo.isPlayer2RoundStartExchangeOver)
                                {
                                    AgainstInfo.isPlayer1RoundStartExchangeOver = false;
                                    AgainstInfo.isPlayer2RoundStartExchangeOver = false;
                                    AgainstInfo.IsSelectCardOver = false;
                                    break;
                                }

                            }
                            else
                            {
                                if (AgainstInfo.isPlayer1RoundStartExchangeOver && !isAlerdlySummaryPlayer1ExchangeOver)
                                {
                                    AgainstInfo.summary.AddSelectOperation(SelectOperationType.SelectExchangeOver, isPlayer1ExchangeOver: true);
                                    isAlerdlySummaryPlayer1ExchangeOver = true;
                                }
                                if (AgainstInfo.isPlayer2RoundStartExchangeOver && !isAlerdlySummaryPlayer2ExchangeOver)
                                {
                                    AgainstInfo.summary.AddSelectOperation(SelectOperationType.SelectExchangeOver, isPlayer1ExchangeOver: false);
                                    isAlerdlySummaryPlayer2ExchangeOver = true;
                                }
                            }
                            await Task.Delay(10);
                        }
                        //while (!(AgainstInfo.isPlayer1RoundStartExchangeOver && AgainstInfo.isPlayer2RoundStartExchangeOver))
                        //{
                        //    TakeLoopManager.cancel.Token.ThrowIfCancellationRequested();
                        //    await Task.Delay(10);
                        //}

                        break;
                    }
                case CardBoardMode.ShowOnly:
                    //while (AgainstInfo.selectBoardCardRanks.Count < Mathf.Min(cardIds.Count, num) && !AgainstInfo.IsFinishSelectBoardCard)
                    while (AgainstInfo.selectBoardCardRanks.Count < Mathf.Min(cardIds.Count, num) && !AgainstInfo.IsSelectCardOver)
                    {
                        await Task.Delay(1);
                    }
                    GameUI.UiCommand.SetCardBoardHide();
                    break;
                default:
                    break;

            }
            AgainstInfo.cardBoardMode = CardBoardMode.None;
        }
    }
}