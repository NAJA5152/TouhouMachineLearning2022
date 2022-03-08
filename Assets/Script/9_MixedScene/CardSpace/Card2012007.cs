using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:草根妖怪网络
    /// </summary>
    public class Card2012007 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectBoardCard(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck][GameEnum.CardTag.Yokai][CardRank.Silver, CardRank.Copper].CardList);
                   await GameSystem.TransSystem.PlayCard(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnits));
                   await GameSystem.TransSystem.MoveToGrave(new TriggerInfoModel(this, this));
               })
               .AbilityAppend();
        }
    }
}