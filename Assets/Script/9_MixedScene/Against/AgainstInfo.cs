using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.Info
{
    /// <summary>
    /// 全局对战信息
    /// </summary>
    public static class AgainstInfo
    {
        public static bool isHostNetMode = true;//本地测试模式
        public static bool isTrainMode = true;//训练加速模式，所有等待设为0
        public static bool isReplayMode = false;//回放模式，会加载指定对战记录读取操作
        public static bool isJumpMode = false;//跳转到指定回合模式
        public static bool isShouldUploadSummaryOperation => !isReplayMode && ((IsPVP && IsMyTurn) || IsPVE);//是否处于应该上传对战记录操作状态,回放模式不上传，单人模式客户端双方均上传记录，多人模式由双方在客户端主体方上传记录

        ////对局的卡牌配置信息
        //public static CardConfig downloadCardConfigAssembly;
        /// <summary>
        /// 玩家线上人物信息
        /// </summary>
        [ShowInInspector]
        public static PlayerInfo onlineUserInfo;
        //玩家的用户信息（可能为线上信息或者单机固定卡组信息）
        [ShowInInspector]
        public static PlayerInfo currentUserInfo;
        //对手的用户信息（可能为线上信息或者单机固定卡组信息）
        [ShowInInspector]
        public static PlayerInfo currentOpponentInfo;
        //双方用户信息

        public static string userName => currentUserInfo.Name;
        public static string opponentName => currentOpponentInfo.Name;
        public static CardDeck userDeck => currentUserInfo.UseDeck;
        public static CardDeck opponentDeck => currentOpponentInfo.UseDeck;

        public static Manager.AgainstSummaryManager summary = new Manager.AgainstSummaryManager();

        //网络同步信息
        public static Card TargetCard;
        public static int washInsertRank;

        public static bool IsSelectCardOver;
        public static string RoomID;
        //操作标志位
        public static List<GameObject> ArrowList = new List<GameObject>();

        //public static CardBoardMode CardBoardMode;
        public static List<int> Player1BlackCardList;
        public static List<int> Player2BlackCardList;

        public static Vector3 dragToPoint;
        //玩家注视的卡牌
        public static Card playerFocusCard;
        //对手注视的卡牌
        public static Card opponentFocusCard;
        //玩家准备使用的悬空状态下的牌
        public static Card playerPrePlayCard;
        //玩家打出的牌
        public static Card playerPlayCard;
        //玩家放弃的牌
        public static Card playerDisCard;
        //玩家选择过牌
        public static bool isPlayerPass = false;

        public static int roundRank = 1;
        public static int turnRank = 0;
        public static int totalTurnRank = 0;
        public static bool isOnTheOffensive = true;

        //选择属性
        public static BattleRegion SelectProperty { get; set; }
        public static bool IsWaitForSelectProperty { get; set; }

        //选择的区域
        public static SingleRowInfo PlayerFocusRegion { get; set; }
        public static bool IsWaitForSelectRegion { get; set; }
        public static SingleRowInfo SelectRegion { get; set; }
        //选择的单位
        public static Card ArrowStartCard { get; set; }
        public static Card ArrowEndCard { get; set; }
        public static bool IsWaitForSelectUnits { get; set; }
        public static List<Card> selectUnits = new List<Card>();//玩家选择的单位
        //选择的坐标
        public static Vector3 FocusPoint;
        public static bool IsWaitForSelectLocation;
        public static int SelectLocation = -1;
        //选择的卡牌面板卡片
        public static bool isRoundStartExchange = false;
        public static bool isPlayer1RoundStartExchangeOver = false;
        public static bool isPlayer2RoundStartExchangeOver = false;

        public static CardBoardMode cardBoardMode;
        public static List<int> cardBoardIDList;
        public static List<Card> cardBoardList;

        public static List<int> selectBoardCardRanks;
        public static List<Card> selectActualCards => selectBoardCardRanks.Select(rank => cardBoardList[rank]).ToList();
        public static List<int> selectVirualCardIds => selectBoardCardRanks.Select(rank => cardBoardIDList[rank]).ToList();

        //public static bool IsFinishSelectBoardCard;
        public static int ExChangeableCardNum = 0;
        //判断是否1号玩家
        public static bool IsPlayer1 { get; set; } = false;
        public static bool IsMyTurn { get; set; }
        public static bool IsPVP { get; set; } = false;
        public static bool IsPVE => !IsPVP;
        //判断是用Ai代替玩家操作
        public static bool IsAiAgent { get; set; } = true;
        public static bool IsAIControl => IsPVE && !IsMyTurn || (!IsPVP && IsMyTurn && Timer.isTimeout);

        /// <summary>
        /// 对局中卡牌的集合
        /// </summary>
        public static CardSet cardSet = new CardSet();

        public static List<Card> AllCardList => CardSet.globalCardList.SelectMany(x => x).ToList();
        public static (int P1Score, int P2Score) PlayerScore;
        public static (int MyScore, int OpScore) ShowScore => IsPlayer1 ? (PlayerScore.P1Score, PlayerScore.P2Score) : (PlayerScore.P2Score, PlayerScore.P1Score);
        public static int TotalUpPoint => cardSet[Orientation.Up][GameRegion.Battle].CardList.Sum(card => card.showPoint);
        public static int TotalDownPoint => cardSet[Orientation.Down][GameRegion.Battle].CardList.Sum(card => card.showPoint);
        public static int TotalMyPoint => cardSet[Orientation.My][GameRegion.Battle].CardList.Sum(card => card.showPoint);
        public static int TotalOpPoint => cardSet[Orientation.Op][GameRegion.Battle].CardList.Sum(card => card.showPoint);
        public static int TotalPlayer1Point => IsPlayer1 ? TotalDownPoint : TotalUpPoint;
        public static int TotalPlayer2Point => IsPlayer1 ? TotalUpPoint : TotalDownPoint;
        public static int TurnRelativePoint => TotalMyPoint - TotalOpPoint;

        public static bool isUpPass = false;
        public static bool isDownPass = false;

        public static bool isCurrectPass => IsMyTurn ? isDownPass : isUpPass;

        public static bool isBoothPass => isUpPass && isDownPass;
    }
}