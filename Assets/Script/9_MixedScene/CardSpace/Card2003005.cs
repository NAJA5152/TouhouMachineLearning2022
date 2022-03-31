using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:樱石
    /// </summary>
    public class Card2003005 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectBoardCard(this, AgainstInfo.cardSet[Orientation.My][GameRegion.Grave][CardRank.Copper][GameEnum.CardTag.Fairy][CardType.Unite].CardList);
                   await GameSystem.TransferSystem.MoveToGrave(this);
                   await GameSystem.TransferSystem.ReviveCard(new TriggerInfoModel(this, AgainstInfo.SelectActualCards));
               })
               .AbilityAppend();
        }
    }
}