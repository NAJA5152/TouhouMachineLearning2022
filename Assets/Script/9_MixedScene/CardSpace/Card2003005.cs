using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:樱石
    /// 卡牌能力:复活一个铜色妖精单位
    /// </summary>
    public class Card2003005 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectBoardCard(this, AgainstInfo.cardSet[Orientation.My][GameRegion.Grave][CardRank.Copper][GameEnum.CardTag.Fairy][CardType.Unit].CardList);
                   await GameSystem.TransferSystem.MoveToGrave(this);
                   await GameSystem.TransferSystem.ReviveCard(new Event(this, AgainstInfo.SelectActualCards));
               })
               .AbilityAppend();
        }
    }
}