using Sirenix.OdinInspector;
using UnityEngine;
namespace TouhouMachineLearningSummary.GameEnum
{
    public enum FirstTurn { PlayerFirst, OpponentFirst, Random }
    public enum AgainstModeType
    {
        Story,//故事模式
        Practice,//练习模式

        Casual,//休闲模式
        Rank,//天梯模式
        Arena,//竞技场模式
    }
    public enum NotifyBoardMode
    {
        Ok,
        Ok_Cancel,
        Cancel,
        Input
    }
    public enum PageType
    {
        CardList,
        Image,
        Text
    }
    public enum TriggerTime
    {
        Before,
        When,
        After
    }
    public enum TriggerType
    {
        Draw,
        Play,
        Deploy,
        Discard,
        Dead,

        /// <summary>
        /// 复活
        /// </summary>
        Revive,
        /// <summary>
        /// 移动
        /// </summary>
        Move,
        /// <summary>
        /// 除外
        /// </summary>
        Banish,
        /// <summary>
        /// 召唤
        /// </summary>
        Summon,

        /// <summary>
        /// 增益
        /// </summary>
        Gain,
        /// <summary>
        /// 伤害
        /// </summary>
        Hurt,
        /// <summary>
        /// 治愈
        /// </summary>
        Cure,
        Reset,
        Destory,
        Strengthen,
        Weak,
        /// <summary>
        /// 封印
        /// </summary>
        Seal,
        Close,
        /// <summary>
        /// 侦查
        /// </summary>
        Scout,
        /// <summary>
        /// 揭示
        /// </summary>
        Reveal,

        FieldChange,//当字段值改变

        SelectUnite,

        RoundStart,
        RoundEnd,
        TurnStart,
        TurnEnd
    }
    public enum Camp
    {
        [LabelText("中立")]
        Neutral,
        [InspectorName("道教")]
        Taoism,
        Shintoism,
        Buddhism,
        science
    }
    public enum GameRegion
    {
        Water,
        Fire,
        Wind,
        Soil,
        Battle,
        None,
        Leader,
        Hand,
        Uesd,
        Deck,
        Grave,
    }
    public enum BattleRegion
    {
        Water, Fire, Wind, Soil, All, None
    }
    public enum CardState
    {
        Spy,
        Seal

    }
    public enum CardType
    {
        Unite,
        Special,
    }
    public enum CardFeature
    {
        Largest,
        Lowest
    }
    public enum CardRank
    {
        Leader,
        Gold,
        Silver,
        Copper,
    }
    public enum CardField
    {
        Timer,//计时
        Vitality,//活力
        Point
    }
    public enum CardBoardMode
    {
        None,//默认状态
        Select,//多次选择模式
        ExchangeCard,//单次抽卡模式
        ShowOnly//无法操作模式
    }
    public enum CardTag
    {
        Machine,
        Fairy,
        Object,
        SpellCard
    }
    public enum Orientation
    {
        My,
        Op,
        All,
        Up,
        Down,
    }
    public enum Territory { My, Op, All }
    public enum Language
    {
        Ch,
        Tc,
        En,
        geyu
    }
    public enum CardPointType
    {
        green,
        red,
        white
    }
    public enum NetAcyncType
    {
        Init,
        FocusCard,
        PlayCard,
        SelectRegion,
        SelectUnites,
        SelectLocation,
        SelectProperty,
        SelectBoardCard,
        ExchangeCard,
        RoundStartExchangeOver,
        Pass,
        Surrender
    }
}