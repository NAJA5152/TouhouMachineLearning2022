public enum AgainstModeType
{
    Story,//故事模式
    Practice,//练习模式

    Casual,//休闲模式
    Rank,//天梯模式
    Arena,//竞技场模式
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
//需要在客户端同步更新
public enum UpdateType
{
    Name,
    Decks,
    UseDeckNum,
    Stage,
    LastLoginTime,
}
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
    Black,//黑
    White,//白
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