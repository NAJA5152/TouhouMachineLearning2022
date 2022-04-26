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
        /// <summary>
        /// 生成
        /// </summary>
        Generate,
        /// <summary>
        /// 抽取
        /// </summary>
        Draw,
        /// <summary>
        /// 打出
        /// </summary>
        Play,
        /// <summary>
        /// 部署
        /// </summary>
        Deploy,
        /// <summary>
        /// 丢弃
        /// </summary>
        Discard,
        /// <summary>
        /// 死亡
        /// </summary>
        Dead,
        /// <summary>
        /// 回手
        /// </summary>
        Recycle,
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
        /// 置值
        /// </summary>
        Set,
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
        /// <summary>
        /// 重置
        /// </summary>
        Reset,
        /// <summary>
        /// 摧毁
        /// </summary>
        Destory,
        /// <summary>
        /// 强化
        /// </summary>
        Strengthen,
        /// <summary>
        /// 弱化
        /// </summary>
        Weak,
        /// <summary>
        /// 逆转
        /// </summary>
        Reverse,
        /// <summary>
        /// 点数增加，只有成功触发点数变化时才会触发
        /// </summary>
        Increase,
        /// <summary>
        /// 点数减少，只有成功触发点数变化时才会触发
        /// </summary>
        Decrease,
        ////////////////////////////////////////////////状态/////////////////////////////////////////
        StateAdd,
        StateClear,
        ////////////////////////////////////////////////字段/////////////////////////////////////////
        FieldSet,
        FieldChange,
        ////////////////////////////////////////////////品质/////////////////////////////////////////
        /// <summary>
        /// 提纯
        /// </summary>
        /// /// <summary>
        /// 魔怔
        /// </summary>
        ////////////////////////////////////////////////阶段/////////////////////////////////////////
        RoundStart,
        RoundEnd,
        TurnStart,
        TurnEnd
    }
    /// <summary>
    /// 卡牌附加状态类型   ！！！！不要改变顺序，有新的再后面追加，服务端和翻译表格需要同步更新！！！！
    /// </summary>
    public enum CardState
    {
        None,//默认空状态
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
        Congealbounds,//结界
        Forbidden,//禁足
    }
    /// <summary>
    /// 卡牌附加值类型     ！！！！不要改变顺序，有新的再后面追加，服务端和翻译表格需要同步更新！！！！
    /// </summary>
    public enum CardField 
    {
        None,//默认空状态
        Timer,//计时
        Inspire,//鼓舞
        Apothanasia,//延命
        Chain,//连锁
        Energy,//能量
        Shield,//护盾
    }
    /// <summary>
    /// 异变类型
    /// </summary>
    public enum VariationType
    {
        None,
        Reverse,//逆转
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
        Used,
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
        LargestUnites,
        LowestUnites,
        NotZero,
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
        Default,//默认状态
        Select,//多次选择模式
        ExchangeCard,//单次抽卡模式
        ShowOnly//无法操作模式
    }
    public enum CardTag
    {
        SpellCard,
        Variation,
        Machine,
        Fairy,
        Object,
        Tool,
        Yokai,
    }
    public enum Orientation
    {
        /// <summary>
        /// 以当前回合方作为主视角的我方区域
        /// </summary>
        My,
        /// <summary>
        /// 以当前回合方作为主视角的对方区域
        /// </summary>
        Op,
        /// <summary>
        /// 双方区域
        /// </summary>
        All,
        /// <summary>
        /// 以客户端视角方作为主视角的上方区域
        /// </summary>
        Up,
        /// <summary>
        /// 以客户端视角方作为主视角的下方区域
        /// </summary>
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
    //服务器需要同步更新
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