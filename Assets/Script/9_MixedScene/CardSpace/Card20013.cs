using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using static TouhouMachineLearningSummary.Info.AgainstInfo;

namespace TouhouMachineLearningSummary.CardSpace
{
    public class Card20013 : Card
    {
        public override void Init()
        {
            this[CardField.Vitality] = 1;

            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectUnite(this, cardSet[Orientation.My][GameRegion.Battle][CardRank.Copper, CardRank.Silver][CardTag.Fairy].CardList, 1);
                   await GameSystem.TransSystem.MoveToGrave(new TriggerInfo(this).SetTargetCard(this));
                   await GameSystem.TransSystem.PlayCard(new TriggerInfo(this).SetTargetCard(selectUnits));
               })
               .AbilityAppend();
        }
    }
}