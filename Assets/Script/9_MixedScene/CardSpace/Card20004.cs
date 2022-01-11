using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    public class Card20004 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, region, territory);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this).SetTargetCard(this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
             .AbilityAdd(async (triggerInfo) =>
             {
                 await GameSystem.SelectSystem.SelectRegion(this, GameRegion.Battle, Territory.Op);
                 List<Card> targetCardList = GameSystem.InfoSystem.AgainstCardSet[GameSystem.InfoSystem.SelectRowRank];
                 int hurtMaxValue = GameSystem.FieldSystem.GetTwoSideField(this, CardField.Vitality) + 1;
                 for (int i = 0; i < Math.Min(targetCardList.Count, hurtMaxValue); i++)
                 {
                     await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this).SetTargetCard(targetCardList[i]).SetPoint(hurtMaxValue - i));
                 }
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}