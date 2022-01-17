using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    public class Card2003005 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectBoardCard(this,AgainstInfo.cardSet[Orientation.My][GameRegion.Grave][CardRank.Copper][CardTag.Fairy][CardType.Unite].CardList);
                   await GameSystem.TransSystem.MoveToGrave(new TriggerInfoModel(this).SetTargetCard(this));
                   await GameSystem.TransSystem.ReviveCard(new TriggerInfoModel(this).SetTargetCard(AgainstInfo.selectActualCards));
               })
               .AbilityAppend();
        }
    }
}