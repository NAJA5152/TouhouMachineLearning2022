using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using static TouhouMachineLearningSummary.Manager.AgainstSummaryManager.TurnOperation;
using Newtonsoft.Json;
using UnityEngine;
using System.Threading.Tasks;

namespace TouhouMachineLearningSummary.Manager
{
    public enum PlayerOperationType
    {
        PlayCard,//出牌
        DisCard,//弃牌
        Pass,//过牌
    }
    public enum SelectOperationType
    {
        SelectProperty,//选择属性
        SelectUnite,//选择单位
        SelectRegion,//选择对战区域
        SelectLocation,//选择位置坐标
        SelectBoardCard,//从面板中选择卡牌
        SelectExchangeOver,//选择换牌完毕
    }
    public class AgainstSummaryManager
    {
        public string AssemblyVerision { get; set; } = "";
        public string player1Name { get; set; } = "";
        public string player2Name { get; set; } = "";
        public static PlayerInfo Player1Info { get; set; }
        public static PlayerInfo Player2Info { get; set; }
        public string Winner { get; set; } = "";
        public DateTime UpdateTime { get; set; }
        //是否按照流程完成对局
        bool IsFinishAgainst { get; set; }

        public List<TurnOperation> TurnOperations { get; set; } = new List<TurnOperation>();
        [JsonIgnore]
        public TurnOperation TargetJumpTurn { get; set; } =null;

        //简易的数字卡牌量化模型

        public class TurnOperation//更新数据模型时须同步更新服务端数据模型
        {
            public int roundRank;//当前小局数
            public int turnRank;//当前回合数
            public int totalTurnRank;//当前总回合数
            public bool isOnTheOffensive;//是否先手
            public bool isPlayer1Turn;//是否处于玩家1的操作回合
            public int absoluteStartPoint;//玩家操作前双方的点数差
            public int absoluteEndPoint;//玩家操作后双方的点数差  
            //0表示不投降，1表示玩家1投降，2表示玩家2投降
            public int isSurrender = 0;
            public List<List<SampleCardModel>> allCardList = new List<List<SampleCardModel>>();
            public PlayerOperation playerOperation;
            public List<SelectOperation> selectOperations = new List<SelectOperation>();
            public TurnOperation() { }
            public TurnOperation Init()
            {
                this.roundRank = AgainstInfo.roundRank;
                this.turnRank = AgainstInfo.turnRank;
                this.totalTurnRank = AgainstInfo.totalTurnRank;
                this.isOnTheOffensive = AgainstInfo.isOnTheOffensive;
                this.isPlayer1Turn = AgainstInfo.isPlayer1 ^ AgainstInfo.isMyTurn;
                this.allCardList = CardSet.globalCardList.SelectList(cardlist => cardlist.SelectList(card => new SampleCardModel(card)));
                return this;
            }
            //回合开始时的基本操作，共三类
            //1从手牌中选择一张打出
            //2从手牌中选择一张打出
            //3pass
            public class PlayerOperation
            {
                public List<int> operation;
                public List<SampleCardModel> targetcardList;
                public int selectCardID;         //打出的目标卡牌id
                public int selectCardIndex;     //打出的目标卡牌索引

                public PlayerOperation() { }
                public PlayerOperation(PlayerOperationType operation, List<Card> targetcardList, Card selectCard = null)
                {
                    this.operation = operation.EnumToOneHot();
                    this.targetcardList = targetcardList.SelectList(card => new SampleCardModel(card));
                    this.selectCardID = selectCard == null ? 0 : selectCard.cardID;
                    this.selectCardIndex = selectCard == null ? -1 : targetcardList.IndexOf(selectCard);
                }
            }
            public class SelectOperation
            {
                //操作类型 选择场地属性/从战场选择多个单位/从卡牌面板中选择多张牌/从战场中选择一个位置/从战场选择多片对战区域
                public List<int> operation;
                public int triggerCardID;
                //选择面板卡牌
                public List<int> selectBoardCardRanks;
                //选择单位
                public List<SampleCardModel> targetCardList;
                public List<int> selectCardRank;
                public int selectMaxNum;
                //区域
                public int selectRegionRank;
                public int selectLocation;
                //换牌完成,true为玩家1换牌操作，false为玩家2换牌操作
                public bool isPlay1ExchangeOver;
                public SelectOperation() { }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="isPlayer1Exchange">玩家1换牌完毕或是玩家2换牌完毕</param>
            public void AddExchangeOver(bool isPlayer1Exchange)
            {
                UnityEngine.Debug.Log($"记录玩家{(isPlayer1Exchange ? "1" : "2")}换牌结束事件");
                SelectOperation operation = new SelectOperation();
                //如果是玩家1主动结束选择或者玩家2收到被动结束选择，这是玩家1选择完毕
                operation.isPlay1ExchangeOver = isPlayer1Exchange;
                operation.operation = SelectOperationType.SelectExchangeOver.EnumToOneHot();
                selectOperations.Add(operation);
            }
            public void AddSelectRegion(Card triggerCard)
            {
                SelectOperation operation = new SelectOperation();
                operation.triggerCardID = triggerCard.cardID;
                operation.selectRegionRank = AgainstInfo.SelectRegion.RowRank;
                operation.operation = SelectOperationType.SelectRegion.EnumToOneHot();
                selectOperations.Add(operation);
            }
            public void AddSelectBoardCard(Card triggerCard)
            {
                SelectOperation operation = new SelectOperation();
                operation.triggerCardID = triggerCard != null ? triggerCard.cardID : 0;
                operation.selectBoardCardRanks = AgainstInfo.selectBoardCardRanks;
                operation.operation = SelectOperationType.SelectBoardCard.EnumToOneHot();
                selectOperations.Add(operation)
;
            }
            public void AddSelectLocation(Card triggerCard)
            {
                SelectOperation operation = new SelectOperation();
                operation.triggerCardID = triggerCard.cardID;
                operation.selectRegionRank = AgainstInfo.SelectRegion.RowRank;
                operation.selectLocation = AgainstInfo.SelectLocation;
                operation.operation = SelectOperationType.SelectLocation.EnumToOneHot();
                selectOperations.Add(operation);
            }

            public void AddSelectUnite(Card triggerCard, List<Card> targetCardList, int selectMaxNum)
            {
                SelectOperation operation = new SelectOperation();
                operation.triggerCardID = triggerCard.cardID;
                operation.selectCardRank = AgainstInfo.selectUnits.SelectList(selectUnite => targetCardList.IndexOf(selectUnite));
                operation.targetCardList = targetCardList.SelectList(card => new SampleCardModel(card));
                operation.selectMaxNum = selectMaxNum;
                operation.operation = SelectOperationType.SelectLocation.EnumToOneHot();
                selectOperations.Add(operation);
            }
        }
        public static AgainstSummaryManager Load(int summaryID) => File.ReadAllText("summary.json").ToObject<AgainstSummaryManager>();
        public void AddRound()
        {
            //AgainstInfo.roundRank++;
            AgainstInfo.turnRank = 0;
            AgainstInfo.isOnTheOffensive = true;
            //添加换牌阶段回合操作，回合0代表换牌操作
            TurnOperations.Add(new TurnOperation().Init());
        }
        public void AddTurn()
        {
            if (AgainstInfo.isReplayMode)
            {
                currentTurnOperationsRank++;
            }
            else
            {

                if (!AgainstInfo.isOnTheOffensive)
                {
                    AgainstInfo.turnRank++;
                    AgainstInfo.totalTurnRank++;
                }
                TurnOperations.Add(new TurnOperation().Init());
                AgainstInfo.isOnTheOffensive = !AgainstInfo.isOnTheOffensive;
            }

        }
        public void AddStartPoint()
        {
            if (!AgainstInfo.isReplayMode)
            {
                TurnOperations.Last().absoluteStartPoint = AgainstInfo.turnChangePoint;
                UnityEngine.Debug.LogWarning("双方起始点数差" + AgainstInfo.turnChangePoint);
            }
        }
        public void AddEndPoint()
        {
            if (!AgainstInfo.isReplayMode)
            {
                TurnOperations.Last().absoluteEndPoint = AgainstInfo.turnChangePoint;
                UnityEngine.Debug.LogWarning("双方结束点数差" + AgainstInfo.turnChangePoint + "" + AgainstInfo.TotalMyPoint + "" + AgainstInfo.TotalOpPoint);
            }
        }
        public void AddPlayerOperation(PlayerOperationType operation, List<Card> targetcardList, Card selectCard)
        {
            if (!AgainstInfo.isReplayMode)
            {
                TurnOperations.Last().playerOperation = new PlayerOperation(operation, targetcardList, selectCard);
            }
        }

        /// <summary>
        /// 记录玩家在回合中的操作
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="triggerCard"></param>
        /// <param name="targetcardList"></param>
        /// <param name="selectMaxNum"></param>
        /// <param name="isPlayer1ExchangeOver"></param>
        public void AddSelectOperation(SelectOperationType operation, Card triggerCard = null, List<Card> targetcardList = null, int selectMaxNum = 0, bool isPlayer1ExchangeOver = false)//是否玩家1操作完成
        {
            if (!AgainstInfo.isReplayMode)
            {
                switch (operation)
                {
                    case SelectOperationType.SelectProperty:
                        break;
                    case SelectOperationType.SelectUnite:
                        TurnOperations.Last().AddSelectUnite(triggerCard, targetcardList, selectMaxNum);
                        break;
                    case SelectOperationType.SelectBoardCard:
                        TurnOperations.Last().AddSelectBoardCard(triggerCard);
                        break;
                    case SelectOperationType.SelectRegion:
                        TurnOperations.Last().AddSelectRegion(triggerCard);
                        break;
                    case SelectOperationType.SelectLocation:
                        TurnOperations.Last().AddSelectLocation(triggerCard);
                        break;
                    case SelectOperationType.SelectExchangeOver:
                        TurnOperations.Last().AddExchangeOver(isPlayer1ExchangeOver);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                currentSelectOperationsRank++;
            }
        }

        public void AddSurrender(bool isPlayer1Surrenddr) => TurnOperations.Last().isSurrender = isPlayer1Surrenddr ? 1 : 2;
        //////////////////////////////////对战指令解析///////////////////////////////// ///////////

        //当前指向的命令编号
        int currentTurnOperationsRank = 0;
        int currentSelectOperationsRank = 0;
        public PlayerOperation GetCurrentPlayerOperation() => TurnOperations[currentTurnOperationsRank].playerOperation;
        public SelectOperation GetCurrentSelectOperation() => TurnOperations[currentTurnOperationsRank].selectOperations[currentSelectOperationsRank];
        //////////////////////////////////对战记录读取////////////////////////////////////////////
        public void Replay(int TotalRank)
        {
            TakeLoopManager.cancel.Cancel();
            //设置回合初始状态
        }
        public async Task JumpToTurnAsync(int totalTurnRank, bool isOnTheOffensive)
        {
            TakeLoopManager.cancel.Cancel();
            //设置回合初始状态
            TargetJumpTurn = TurnOperations.FirstOrDefault(turn => turn.isOnTheOffensive == isOnTheOffensive && turn.totalTurnRank == totalTurnRank);
            if (TargetJumpTurn == null)
            {
                Debug.LogError("回合跳转溢出，重置跳转到最后");
                TargetJumpTurn = TurnOperations.Last();
            }
            //清空所有卡牌
            CardSet.globalCardList.ForEach(cardlist => cardlist.ForEach(card => UnityEngine.Object.Destroy(card.gameObject)));
            CardSet.globalCardList.ForEach(cardlist => cardlist.Clear());
            //await Task.Delay(1000);

            //设置当前为跳转模式
            AgainstInfo.isJumpMode = true;
            await Control.StateControl.CreatAgainstProcess();
            //设置当前指定回合
            //重新读取
        }
        //////////////////////////////////对战记录输出////////////////////////////////////////////
        public void Show() => UnityEngine.Debug.LogWarning(this.ToJson());
        public void Explort() => File.WriteAllText("summary.json", this.ToJson());
        public void Upload()
        {
            player1Name = AgainstInfo.isPlayer1 ? AgainstInfo.userName : AgainstInfo.opponentName;
            player1Name = AgainstInfo.isPlayer1 ? AgainstInfo.opponentName : AgainstInfo.userName;
            UpdateTime = DateTime.Now;
            Command.Network.NetCommand.UploadAgentSummary(this);
        }
    }
}
