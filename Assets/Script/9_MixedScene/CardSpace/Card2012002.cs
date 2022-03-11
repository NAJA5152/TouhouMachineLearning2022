using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:赤蛮奇
    /// </summary>
    public class Card2012002 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this,this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Increase)
               .AbilityAdd(async (triggerInfo) =>
               {
                   if (!this[CardState.Furor])//如果不处于狂躁状态
                    {
                       await GameSystem.StateSystem.ClearState(new TriggerInfoModel(this, this).SetTargetState(CardState.Docile));
                       await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Furor));

                       await GameSystem.TransSystem.GenerateCard(new TriggerInfoModel(this, targetCard: null).SetTargetCardId(2013006).SetLocation( CurrentOrientation, CurrentRegion,-1));
                   }
               }, Condition.Default)
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Decrease)
               .AbilityAdd(async (triggerInfo) =>
               {
                   if (!this[CardState.Docile])//如果不处于温顺状态
                   {
                       await GameSystem.StateSystem.ClearState(new TriggerInfoModel(this, this).SetTargetState(CardState.Furor));
                       await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Docile));

                       await GameSystem.TransSystem.GenerateCard(new TriggerInfoModel(this, targetCard: null).SetTargetCardId(2013007).SetLocation(CurrentOrientation, CurrentRegion, -1));

                   }
               }, Condition.Default)
                .AbilityAppend();
        }
    }
}