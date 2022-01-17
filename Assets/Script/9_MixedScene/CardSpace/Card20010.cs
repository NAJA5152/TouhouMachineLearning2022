using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    public class Card20010 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this).SetTargetCard(this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
             .AbilityAdd(async (triggerInfo) =>
             {
                 await GameSystem.FieldSystem.ChangeField(
                     new TriggerInfoModel(this)
                     .SetTargetCard(GameSystem.InfoSystem.AgainstCardSet[Orientation.My][CardField.Vitality].CardList)
                     .SetTargetField( CardField.Vitality,1)
                     ); 
                 //foreach (var unite in AgainstInfo.cardSet[Orientation.My][CardField.Vitality].CardList)
                 //{
                 //    await GameSystem.FieldSystem.ChangeField(new TriggerInfoModel(this).SetTargetCard(AgainstInfo.SelectUnits).SetPoint(unite[CardField.Vitality] + 1));
                 //}
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}