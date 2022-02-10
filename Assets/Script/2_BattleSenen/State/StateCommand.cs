using System;
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
            AgainstSummaryManager.TurnOperation targetJumpTurn = AgainstInfo.summary.TargetJumpTurn;
            AgainstInfo.roundRank = targetJumpTurn.RoundRank;
            AgainstInfo.turnRank = targetJumpTurn.TurnRank;
            AgainstInfo.totalTurnRank = targetJumpTurn.TotalTurnRank;
            //判断该回合是否为预处理换牌阶段
            bool isExchangeTurn = targetJumpTurn.TurnRank == 0;
            //AgainstInfo.totalTurnRank AgainstInfo.summary.targetJumpTurn.allCardList
            //根据对战中的回合数据初始化场上卡牌
            AgainstInfo.cardSet = new CardSet();
            foreach (var item in GameObject.FindGameObjectsWithTag("SingleInfo"))
            {
                SingleRowManager singleRowInfo = item.GetComponent<SingleRowManager>();
                AgainstInfo.cardSet.SingleRowInfos.Add(singleRowInfo);
            }
            //CardSet.globalCardList = AgainstInfo.summary.targetJumpTurn.allCardList
            //    .Select(sampleCardList => sampleCardList.Select(CardCommand.CreateCard).ToList()).ToList();
            CardSet.GlobalCardList = targetJumpTurn.AllCardList.SelectList(sampleCardList => sampleCardList.SelectList(CardCommand.CreateCard));
            AgainstInfo.cardSet[GameRegion.Leader, GameRegion.Battle].CardList.ForEach(card => card.IsCanSee = true);
            AgainstInfo.cardSet[GameRegion.Hand][AgainstInfo.isReplayMode ? Orientation.All : Orientation.My].CardList.ForEach(card => card.IsCanSee = true);
            AgainstInfo.isJumpMode = false;
            return isExchangeTurn;
        }
        ////////////////////////////////////////////////////对局流程指令////////////////////////////////////////////////////////////////////////////////
        public static async Task AgainstStart()
        {
            Info.CardInfo.CreatCardRank = 0;
            Manager.TaskLoopManager.Init();
            //如果不是通过配置文件启动的场景
            if (AgainstInfo.currentUserInfo == null)
            {
                //初始化网络系统，用于获取指定版本卡牌信息
                await Command.NetCommand.Init();
                await Manager.CardAssemblyManager.SetCurrentAssembly("");
                AgainstInfo.IsMyTurn = true;
                AgainstInfo.currentUserInfo = new PlayerInfo(
                     "NPC", "gezi", "yaya", "",
                    new List<CardDeck>
                    {
                        new CardDeck("萌新的第一套卡组", 2010001, new List<int>
                         {
                             2011001,2011002,2011003,2011004,
                             2012001,2012002,2002003,2002004,2002005,2002006,
                             2003001,2003002,2003003,2003004,2003005,
                             //2003001,2003002,2003003,2003004,2003005,
                             //2003001,2003002,2003003,2003004,2003005,
                         })
                    });
                AgainstInfo.currentOpponentInfo = new PlayerInfo(
                     "NPC", "gezi", "yaya", "",
                    new List<CardDeck>
                    {
                        new CardDeck("萌新的第一套卡组", 2000001, new List<int>
                         {
                             2001001,2001002,2001003,2001004,
                             2002001,2002002,2002003,2002004,2002005,2002006,
                             2003001,2003002,2003003,2003004,2003005,
                             2003001,2003002,2003003,2003004,2003005,
                             2003001,2003002,2003003,2003004,2003005,
                         })
                    });
            }
            AgainstInfo.cardSet = new CardSet();
            foreach (var item in GameObject.FindGameObjectsWithTag("SingleInfo"))
            {
                SingleRowManager singleRowInfo = item.GetComponent<SingleRowManager>();
                AgainstInfo.cardSet.SingleRowInfos.Add(singleRowInfo);
            }
            RowCommand.SetRegionSelectable(region: GameRegion.None);
            await CustomThread.Delay(1500);
            Manager.LoadingManager.manager?.CloseAsync();
            //Debug.LogError("初始双方信息");
            try
            {
                //await Task.Delay(500);
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
            if (AgainstInfo.isShouldUploadSummaryOperation)
            {
                await Command.NetCommand.AgainstFinish();
            }
            await Task.Delay(2000);
            //Debug.Log("释放线程资源");
            TaskLoopManager.cancel.Cancel();
            SceneManager.LoadScene(0);
            await Manager.CameraViewManager.MoveToViewAsync(2);

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
                        Info.AgainstInfo.ExChangeableCardNum = 3;
                        Command.GameUI.UiCommand.SetCardBoardTitle("剩余抽卡次数为".Translation() + Info.AgainstInfo.ExChangeableCardNum);
                        // Info.GameUI.UiInfo.CardBoardTitle = "剩余抽卡次数为".Translation() + Info.AgainstInfo.ExChangeableCardNum;
                        for (int i = 0; i < 10; i++)
                        {
                            await CardCommand.DrawCard(isPlayerDraw: true, isOrder: false);
                        }
                        for (int i = 0; i < 10; i++)
                        {
                            await CardCommand.DrawCard(isPlayerDraw: false, isOrder: false);
                        }
                        CardCommand.OrderHandCard();
                        break;
                    }
                case (2):
                    {
                        Info.AgainstInfo.ExChangeableCardNum += 1;
                        Command.GameUI.UiCommand.SetCardBoardTitle("剩余抽卡次数为" + Info.AgainstInfo.ExChangeableCardNum);
                        //Info.GameUI.UiInfo.CardBoardTitle ="剩余抽卡次数为" + Info.AgainstInfo.ExChangeableCardNum ;
                        await CardCommand.DrawCard();
                        await CardCommand.DrawCard(false);
                        break;
                    }
                case (3):
                    {
                        Info.AgainstInfo.ExChangeableCardNum += 1;
                        Command.GameUI.UiCommand.SetCardBoardTitle("剩余抽卡次数为" + Info.AgainstInfo.ExChangeableCardNum);
                        //Info.GameUI.UiInfo.CardBoardTitle ="剩余抽卡次数为" + Info.AgainstInfo.ExChangeableCardNum ;
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
            Manager.AgainstSummaryManager.UploadTurn();
            await GameUI.UiCommand.NoticeBoardShow((AgainstInfo.IsMyTurn ? "我方回合开始" : "对方回合开始").Translation());
            RowCommand.SetPlayCardMoveFree(AgainstInfo.IsMyTurn);
            await CustomThread.Delay(1000);

        }
        public static async Task TurnEnd()
        {
            RowCommand.SetPlayCardMoveFree(false);
            await GameUI.UiCommand.NoticeBoardShow((AgainstInfo.IsMyTurn ? "我方回合结束" : "对方回合结束").Translation());
            await GameSystem.ProcessSystem.WhenTurnEnd();
            await CustomThread.Delay(1000);
            AgainstInfo.IsMyTurn = !AgainstInfo.IsMyTurn;
        }
        ////////////////////////////////////////////////////玩家操作指令////////////////////////////////////////////////////////////////////////////////
        public static void SetCurrentPass()
        {
            Info.GameUI.UiInfo.MyPass.SetActive(true);
            switch (Info.AgainstInfo.IsMyTurn)
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
            Command.NetCommand.AsyncInfo(NetAcyncType.Surrender);
            Manager.AgainstSummaryManager.UploadSurrender(AgainstInfo.IsPlayer1);
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
                //如果是录播模式，则通过指定的对局数据获取操作记录
                if (AgainstInfo.isReplayMode)
                {
                    var operation = AgainstInfo.summary.GetCurrentPlayerOperation();
                    if (operation != null)//如果拥有指令则执行，否则为pass跳过
                    {
                        if (operation.Operation.OneHotToEnum<PlayerOperationType>() == PlayerOperationType.PlayCard)
                        {
                            Info.AgainstInfo.playerPlayCard = Info.AgainstInfo.cardSet[Orientation.My][GameRegion.Leader, GameRegion.Hand].CardList[operation.SelectCardIndex];
                            Debug.LogWarning("打出卡牌" + Info.AgainstInfo.playerPlayCard.CardID);

                        }
                        else if (operation.Operation.OneHotToEnum<PlayerOperationType>() == PlayerOperationType.DisCard)
                        {
                            Info.AgainstInfo.playerDisCard = Info.AgainstInfo.cardSet[Orientation.My][GameRegion.Leader, GameRegion.Hand].CardList[operation.SelectCardIndex];
                        }
                        else if (operation.Operation.OneHotToEnum<PlayerOperationType>() == PlayerOperationType.Pass)
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

                    }
                }
                else
                {
                    //若果当前模式满足ai操作，则交由ai处理
                    if (AgainstInfo.IsAIControl)
                    {
                        await AiCommand.TempPlayOperation();
                    }
                    else
                    {
                        //如果是我的回合则等待玩家操作，否则等待网络同步对方操作
                        if (AgainstInfo.IsMyTurn)
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
                    //Debug.Log("当前打出了牌");
                    await AgainstSummaryManager.UploadPlayerOperationAsync(PlayerOperationType.PlayCard, AgainstInfo.cardSet[Orientation.My][GameRegion.Leader, GameRegion.Hand].CardList, AgainstInfo.playerPlayCard);
                    //假如是我的回合，则广播操作给对方，否则只接收操作不广播
                    await GameSystem.TransSystem.PlayCard(new TriggerInfoModel(null, AgainstInfo.playerPlayCard), AgainstInfo.IsMyTurn);
                    //Debug.Log("打出效果执行完毕");

                    break;
                }
                //如果当前回合弃牌
                if (Info.AgainstInfo.playerDisCard != null)
                {
                    await AgainstSummaryManager.UploadPlayerOperationAsync(PlayerOperationType.DisCard, AgainstInfo.cardSet[Orientation.My][GameRegion.Leader, GameRegion.Hand].CardList, AgainstInfo.playerDisCard);
                    await GameSystem.TransSystem.DisCard(new TriggerInfoModel(null, AgainstInfo.playerDisCard));
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
                        await AgainstSummaryManager.UploadPlayerOperationAsync(PlayerOperationType.Pass, AgainstInfo.cardSet[Orientation.My][GameRegion.Leader, GameRegion.Hand].CardList, null);
                        SetCurrentPass();
                        AgainstInfo.isPlayerPass = false;
                        break;
                    }
                }
                await Task.Delay(10);
                TaskLoopManager.Throw();
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
            // Debug.Log("等待选择属性");
            while (AgainstInfo.SelectProperty == BattleRegion.None)
            {
                TaskLoopManager.Throw();
                if (AgainstInfo.IsAIControl)
                {
                    //Debug.Log("自动选择属性");
                    int rowRank = AiCommand.GetRandom(0, 4);
                    await CoinManager.ChangePropertyAsync((BattleRegion)rowRank);
                }
                await Task.Delay(1);
            }
            Command.NetCommand.AsyncInfo(NetAcyncType.SelectProperty);
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
        public static async Task WaitForSelectRegion(Card triggerCard, Territory territory, GameRegion regionTypes)
        {
            AgainstInfo.IsWaitForSelectRegion = true;
            AgainstInfo.SelectRowRank = -1;
            RowCommand.SetRegionSelectable(territory, regionTypes);
            while (AgainstInfo.SelectRowRank == -1)
            {
                TaskLoopManager.Throw();
                if (AgainstInfo.isReplayMode)
                {
                    var operation = AgainstInfo.summary.GetCurrentSelectOperation();
                    if (operation.Operation.OneHotToEnum<SelectOperationType>() == SelectOperationType.SelectRegion)
                    {
                        AgainstInfo.SelectRowRank = operation.SelectRegionRank;
                    }
                    else
                    {
                        Debug.LogError("对战记录识别出现严重bug");
                        throw new Exception("");
                    }
                }
                else if (AgainstInfo.IsAIControl)
                {
                    await CustomThread.Delay(1000);
                    AgainstInfo.SelectRowRank = AgainstInfo.cardSet.SingleRowInfos.Where(row => row.CanBeSelected).OrderBy(x => AiCommand.GetRandom()).FirstOrDefault().RowRank;
                }
                await Task.Delay(1);
            }
            NetCommand.AsyncInfo(NetAcyncType.SelectRegion);
            AgainstSummaryManager.UploadSelectOperation(SelectOperationType.SelectRegion, triggerCard);
            RowCommand.SetRegionSelectable(region: GameRegion.None);
            AgainstInfo.IsWaitForSelectRegion = false;
        }
        public static async Task WaitForSelectLocation(Card triggerCard, Territory territory, BattleRegion region)
        {
            AgainstInfo.IsWaitForSelectLocation = true;
            //设置指定坐标可选
            RowCommand.SetRegionSelectable(territory, (GameRegion)region);
            AgainstInfo.SelectRank = -1;
            while (AgainstInfo.SelectRank < 0)
            {
                TaskLoopManager.Throw();
                if (AgainstInfo.isReplayMode)
                {
                    var operation = AgainstInfo.summary.GetCurrentSelectOperation();
                    if (operation.Operation.OneHotToEnum<SelectOperationType>() == SelectOperationType.SelectLocation)
                    {
                        AgainstInfo.SelectRowRank = operation.SelectRegionRank;
                        AgainstInfo.SelectRank = operation.SelectLocation;
                    }
                    else
                    {
                        Debug.LogError("对战记录识别出现严重bug");
                        throw new Exception("");
                    }
                }
                else if (AgainstInfo.IsAIControl)
                {
                    await CustomThread.Delay(1000);
                    AgainstInfo.SelectRowRank = AgainstInfo.cardSet.SingleRowInfos.Where(row => row.CanBeSelected).OrderBy(x => AiCommand.GetRandom()).FirstOrDefault().RowRank;
                    AgainstInfo.SelectRank = 0;//设置部署次序
                }
                await Task.Delay(10);
            }
            NetCommand.AsyncInfo(NetAcyncType.SelectLocation);
            AgainstSummaryManager.UploadSelectOperation(SelectOperationType.SelectLocation, triggerCard);
            RowCommand.SetRegionSelectable(region: GameRegion.None);
            AgainstInfo.IsWaitForSelectLocation = false;
        }
        public static async Task WaitForSelecUnit(Card triggerCard, List<Card> filterCards, int num, bool isAuto)
        {
            //可选列表中移除自身
            filterCards.Remove(triggerCard);
            AgainstInfo.ArrowStartCard = triggerCard;
            AgainstInfo.IsWaitForSelectUnits = true;
            AgainstInfo.AllCardList.ForEach(card => card.IsGray = true);
            filterCards.ForEach(card => card.IsGray = false);
            AgainstInfo.SelectUnits.Clear();
            //await Task.Delay(500);
            //若为回放模式
            if (AgainstInfo.isReplayMode)
            {
                var operation = AgainstInfo.summary.GetCurrentSelectOperation();
                if (operation.Operation.OneHotToEnum<SelectOperationType>() == SelectOperationType.SelectUnite)
                {
                    AgainstInfo.SelectUnits = operation.SelectCardRank.SelectList(index => filterCards[index]);
                }
                else
                {
                    Debug.LogError("对战记录识别出现严重bug");
                    throw new Exception("");
                }
            }
            else
            {
                if (Info.AgainstInfo.IsMyTurn && !isAuto)
                {
                    GameUI.UiCommand.CreatFreeArrow();
                }
                int selectableNum = Math.Min(filterCards.Count, num);
                while (AgainstInfo.SelectUnits.Count < selectableNum)
                {
                    TaskLoopManager.Throw();
                    //AI操作或者我方回合自动选择模式时 ，用自身随机决定，否则等待网络同步

                    if (AgainstInfo.IsAIControl || (isAuto && AgainstInfo.IsMyTurn))
                    {
                        Debug.Log("自动选择场上单位");
                        IEnumerable<Card> autoSelectTarget = filterCards.OrderBy(x => AiCommand.GetRandom(0, 514)).Take(selectableNum);
                        AgainstInfo.SelectUnits = autoSelectTarget.ToList();
                    }
                    await Task.Delay(10);
                }
                //Debug.Log("选择单位完毕" + Math.Min(Cards.Count, num));

            }
            NetCommand.AsyncInfo(NetAcyncType.SelectUnites);
            AgainstSummaryManager.UploadSelectOperation(SelectOperationType.SelectUnite, triggerCard, filterCards, num);
            GameUI.UiCommand.DestoryAllArrow();
            await CustomThread.Delay(250);
            //Debug.Log("同步选择单位完毕");
            AgainstInfo.AllCardList.ForEach(card => card.IsGray = false);
            AgainstInfo.IsWaitForSelectUnits = false;
            //Debug.Log("结束选择单位");
        }
        public static async Task WaitForSelectBoardCard<T>(Card triggerCard, List<T> cardIds, CardBoardMode mode = CardBoardMode.Select, int num = 1)
        {
            AgainstInfo.selectBoardCardRanks = new List<int>();
            AgainstInfo.IsSelectCardOver = false;
            AgainstInfo.cardBoardMode = mode;
            GameUI.UiCommand.SetCardBoardShow();
            //加载真实或虚拟的卡牌列表
            if (typeof(T) == typeof(Card))
            {
                CardBoardCommand.LoadBoardCardList(cardIds.Cast<Card>().ToList());
            }
            else
            {
                CardBoardCommand.LoadBoardCardList(cardIds.Cast<int>().ToList());
            }
            //Debug.Log("进入选择模式");
            switch (mode)
            {
                case CardBoardMode.Select:
                    while (AgainstInfo.selectBoardCardRanks.Count < Mathf.Min(cardIds.Count, num) && !AgainstInfo.IsSelectCardOver)
                    {
                        await Task.Delay(10);
                    }
                    GameUI.UiCommand.SetCardBoardHide();
                    break;
                case CardBoardMode.ExchangeCard:
                    {
                        if (AgainstInfo.isReplayMode)
                        {
                            Debug.LogWarning("以记录方式读取操作");
                            List<AgainstSummaryManager.TurnOperation.SelectOperation> selectOperations = AgainstInfo.summary.GetCurrentSelectOperations();
                            for (int i = 0; i < selectOperations.Count; i++)
                            {
                                var operation = selectOperations[i];
                                if (operation.Operation.OneHotToEnum<SelectOperationType>() == SelectOperationType.SelectBoardCard)
                                {
                                    //如果是我方换牌，则记录，如果是对方换牌，直接生效
                                    Info.AgainstInfo.selectBoardCardRanks = operation.SelectBoardCardRanks;
                                    Info.AgainstInfo.washInsertRank = operation.WashInsertRank;
                                    bool isPlayer1Select = operation.IsPlayer1Select;
                                    List<Card> CardLists = AgainstInfo.cardSet[Orientation.Down][GameRegion.Hand].CardList;
                                    int selectRank = AgainstInfo.selectBoardCardRanks[0];
                                    //交换对象需要重新考究
                                    await CardCommand.ExchangeCard(CardLists[selectRank], IsPlayerExchange: !(isPlayer1Select ^ Info.AgainstInfo.IsPlayer1), isRoundStartExchange: true, WashInsertRank: AgainstInfo.washInsertRank);
                                    Info.AgainstInfo.ExChangeableCardNum--;
                                    Info.AgainstInfo.selectBoardCardRanks.Clear();
                                    GameUI.UiCommand.SetCardBoardTitle("剩余抽卡次数为" + Info.AgainstInfo.ExChangeableCardNum);
                                }
                                else if (operation.Operation.OneHotToEnum<SelectOperationType>() == SelectOperationType.SelectExchangeOver)
                                {

                                    if (operation.IsPlay1ExchangeOver)
                                    {
                                        AgainstInfo.isPlayer1RoundStartExchangeOver = true;
                                    }
                                    else
                                    {
                                        AgainstInfo.isPlayer2RoundStartExchangeOver = true;
                                    }
                                }
                                else
                                {
                                    Debug.LogError("对战记录识别出现严重bug");
                                    throw new Exception("");
                                }
                            }
                        }
                        else
                        {
                            //我方超时或者打ai时对方自动结束换牌
                            AiCommand.RoundStartExchange(false);
                            //如果满足换牌条件则持续换牌状态
                            while (Info.AgainstInfo.ExChangeableCardNum != 0 && !Info.AgainstInfo.IsSelectCardOver)
                            {
                                TaskLoopManager.Throw();
                                //通过对战记录换牌

                                //有牌要被换
                                if (Info.AgainstInfo.selectBoardCardRanks.Count > 0)
                                {
                                    List<Card> CardLists = AgainstInfo.cardSet[Orientation.Down][GameRegion.Hand].CardList;
                                    int selectRank = AgainstInfo.selectBoardCardRanks[0];
                                    AgainstSummaryManager.UploadSelectOperation(SelectOperationType.SelectBoardCard, triggerCard, CardLists, 1, isPlayer1Select: Info.AgainstInfo.IsPlayer1);
                                    await CardCommand.ExchangeCard(CardLists[selectRank], IsPlayerExchange: true, isRoundStartExchange: true, WashInsertRank: AgainstInfo.washInsertRank);
                                    Info.AgainstInfo.ExChangeableCardNum--;
                                    Info.AgainstInfo.selectBoardCardRanks.Clear();
                                    GameUI.UiCommand.SetCardBoardTitle("剩余抽卡次数为" + Info.AgainstInfo.ExChangeableCardNum);
                                }
                                await Task.Delay(10);
                            }
                            //跳出换牌循环，并告知对面
                            if (AgainstInfo.IsPlayer1)
                            {
                                AgainstInfo.isPlayer1RoundStartExchangeOver = true;
                            }
                            else
                            {
                                AgainstInfo.isPlayer2RoundStartExchangeOver = true;
                            }
                            NetCommand.AsyncInfo(NetAcyncType.RoundStartExchangeOver);
                        }

                        GameUI.UiCommand.SetCardBoardHide();
                        //等待双方退出
                        await CustomThread.UnitllAcync(
                            () => AgainstInfo.isPlayer1RoundStartExchangeOver,
                            () => AgainstSummaryManager.UploadSelectOperation(SelectOperationType.SelectExchangeOver, isPlayer1ExchangeOver: true)
                        );
                        await CustomThread.UnitllAcync(
                            () => AgainstInfo.isPlayer2RoundStartExchangeOver,
                            () => AgainstSummaryManager.UploadSelectOperation(SelectOperationType.SelectExchangeOver, isPlayer1ExchangeOver: false)
                        );

                        AgainstInfo.isPlayer1RoundStartExchangeOver = false;
                        AgainstInfo.isPlayer2RoundStartExchangeOver = false;
                        AgainstInfo.IsSelectCardOver = false;
                        break;
                    }
                case CardBoardMode.ShowOnly:
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