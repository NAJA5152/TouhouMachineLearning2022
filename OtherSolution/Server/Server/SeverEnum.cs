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
    FocusRegion,
    FocusLocation,
    SelectUnites,
    ExchangeCard,
    Pass,
    Surrender
}
public enum UpdateType
{
    Name,
    Deck,
    UseDeckNum,
    UserState,
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
    Spy,
    Seal
}
public enum CardField
{
    Timer,//计时
    Vitality,//活力
    Point
}