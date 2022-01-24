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
        ////////////////////////////////////////////////移动/////////////////////////////////////////
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
        /// 位移
        /// </summary>
        Move,
        /// <summary>
        /// 间隙
        /// </summary>
        Banish,
        /// <summary>
        /// 召唤
        /// </summary>
        Summon,
        ////////////////////////////////////////////////点数/////////////////////////////////////////
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
        ////////////////////////////////////////////////状态/////////////////////////////////////////
        StateAdd,
        StateClear,
        ////////////////////////////////////////////////字段/////////////////////////////////////////
        FieldSet,
        FieldChange,
        ////////////////////////////////////////////////选择/////////////////////////////////////////
        SelectUnite,

        RoundStart,
        RoundEnd,
        TurnStart,
        TurnEnd
    }
    public enum CardState
    {
        Seal,//封印
        Invisibility,//隐身
        Pry,//窥探
        Close,//封闭
        Fate,//命运
        Lurk,//潜伏（间谍）
        Secret,//隐秘（盖牌）
        Furor,//狂暴
        Docile,//温顺
        Poisoning,//中毒
        Rely,//凭依
        Water,//水
        Fire,//火
        Wind,//风
        Soil,//土
        Hold, //驻守
    }
    public enum CardField
    {
        Timer,//计时
        Vitality,//活力
        Apothanasia,//延命
        Chain,//连锁
        Energy,//能量
        Shield,//护盾
    }
    public enum Camp
    {
        Neutral,
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
        Leader,
        Hand,
        Uesd,
        Deck,
        Grave,
        Battle = 99,
        None = 100,
    }
    public enum BattleRegion
    {
        Water, Fire, Wind, Soil, All = 99, None = 100
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