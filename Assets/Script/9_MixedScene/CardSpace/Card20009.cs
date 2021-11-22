using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    public class Card20009 : Card
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
                 for (int i = 0; i < 1 + GameSystem.FieldSystem.GetTwoSideField(this, CardField.Vitality)+1; i++)
                 {
                     await GameSystem.SelectSystem.SelectUnite(this, AgainstInfo.cardSet[GameRegion.Battle].CardList, 1, false);
                     await GameSystem.StateSystem.SealCard(new TriggerInfo(this).SetTargetCard(AgainstInfo.selectUnits));
                 }
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}