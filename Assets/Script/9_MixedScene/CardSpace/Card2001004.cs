using System;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    public class Card2001004 : Card
    {
        public override void Init()
        {
            base.Init();

            //初始化通用卡牌效果
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
                 await GameSystem.SelectSystem.SelectRegion(this, Territory.Op, GameRegion.Battle);
                 List<Card> targetCardList = GameSystem.InfoSystem.AgainstCardSet[GameSystem.InfoSystem.SelectRowRank];
                 int hurtMaxValue = GameSystem.InfoSystem.GetTwoSideField(this, CardField.Inspire) + 1;
                 for (int i = 0; i < Math.Min(targetCardList.Count, hurtMaxValue); i++)
                 {
                     await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, targetCardList[i]).SetPoint(hurtMaxValue - i));
                 }
             }, Condition.Default)
             .AbilityAppend();
            
        }
    }
}