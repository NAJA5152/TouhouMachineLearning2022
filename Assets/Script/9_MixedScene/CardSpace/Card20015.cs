using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    public class Card20015 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this,region,territory);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this).SetTargetCard(this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
             .AbilityAdd(async (triggerInfo) =>
             {
                 for (int i = 0; i < GameSystem.FieldSystem.GetTwoSideField(this, CardField.Vitality) + 1; i++)
                 {
                     await GameSystem.SelectSystem.SelectUnite(this, AgainstInfo.cardSet[Orientation.Op][GameRegion.Battle][CardRank.Copper, CardRank.Silver][CardFeature.Largest].CardList, 1, true);
                     await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this).SetTargetCard(AgainstInfo.SelectUnits).SetPoint(1));
                 }
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}