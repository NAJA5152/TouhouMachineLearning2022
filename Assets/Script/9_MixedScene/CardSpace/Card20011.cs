using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    public class Card20011 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this,region,territory);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfo(this).SetTargetCard(this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
             .AbilityAdd(async (triggerInfo) =>
             {
                 await GameSystem.SelectSystem.SelectUnite(this, AgainstInfo.cardSet[Orientation.My][GameRegion.Battle][CardField.Vitality].CardList, 2, false);
                 foreach (var unite in AgainstInfo.selectUnits)
                 {
                     await GameSystem.FieldSystem.Change(new TriggerInfo(this).SetTargetCard(unite).SetPoint(unite[CardField.Vitality] * 2));
                 }
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}