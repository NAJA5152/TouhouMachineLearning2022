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