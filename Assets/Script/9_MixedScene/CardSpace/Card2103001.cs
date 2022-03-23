using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:水利发电机
    /// </summary>
    public class Card2103001 : Card
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

            AbalityRegister(TriggerTime.When, TriggerType.TurnEnd)
               .AbilityAdd(async (triggerInfo) =>
               {
                   if (this[CardField.Energy] < 3)
                   {
                       await GameSystem.FieldSystem.ChangeField(new TriggerInfoModel(this, this).SetTargetField(CardField.Energy, 1));
                   }
                   else
                   {
                       await ThisCardManager.ShowTips("超载", new Color(1, 0, 0));
                       await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, this.TwoSideCard).SetPoint(2).SetMeanWhile());
                       await GameSystem.PointSystem.Destory(new TriggerInfoModel(this, this));
                   }
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}