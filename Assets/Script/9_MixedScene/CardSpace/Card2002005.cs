using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    public class Card2002005 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this, this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
             .AbilityAdd(async (triggerInfo) =>
             {
                 await GameSystem.FieldSystem.ChangeField(
                     new TriggerInfoModel(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][CardField.Inspire].CardList)
                     .SetTargetField( CardField.Inspire,1)
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