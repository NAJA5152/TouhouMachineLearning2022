using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:若鹭姬
    /// </summary>
    public class Card2012001 : Card
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

            //替换原有设值效果，添加暴躁温顺判定
            AbalityRegister(TriggerTime.When, TriggerType.Set)
               .AbilityAdd(async (triggerInfo) =>
               {
                   var targetShowPoint = triggerInfo.targetCard.ShowPoint;
                   await Command.CardCommand.Set(triggerInfo);
                   //如果原本点数大于设置点数
                   if (targetShowPoint < triggerInfo.point)
                   {
                       await GameSystem.PointSystem.Increase(triggerInfo);
                   }
                   if (targetShowPoint > triggerInfo.point)
                   {
                       await GameSystem.PointSystem.Decrease(triggerInfo);
                   }
               })
               .AbilityReplace();
            //替换原有增益效果，添加温顺判定
            AbalityRegister(TriggerTime.When, TriggerType.Gain)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await Command.CardCommand.Gain(triggerInfo);
                   if (triggerInfo.point>0)//如果不处于温顺状态
                   {
                       await GameSystem.PointSystem.Increase(triggerInfo);
                   }
               })
               .AbilityReplace();

            //替换原有受伤效果，添加温顺判定
            AbalityRegister(TriggerTime.When, TriggerType.Hurt)
                .AbilityAdd(async (triggerInfo) =>
                {
                    if (this[cardState: CardState.Congealbounds])
                    {
                        await GameSystem.StateSystem.ClearState(new TriggerInfoModel(triggerInfo.triggerCard, this));
                    }
                    else
                    {
                        bool isBreak = triggerInfo.point > this[CardField.Shield];
                        await Command.CardCommand.Hurt(triggerInfo);
                        if (!this[CardState.Docile])//如果不处于温顺状态
                        {
                            await GameSystem.StateSystem.ClearState(new TriggerInfoModel(this, this).SetTargetState(CardState.Furor));
                            await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Docile));
                        }
                    }
                })
                .AbilityReplace();

            AbalityRegister(TriggerTime.When, TriggerType.StateAdd)
                .AbilityAdd(async (triggerInfo) =>
                {
                    if (triggerInfo.targetState == CardState.Furor)
                    {
                        UnityEngine.Debug.Log("狂躁");
                        if (!this[CardState.Docile])//如果不处于温顺状态
                        {
                            await GameSystem.StateSystem.ClearState(new TriggerInfoModel(this, this).SetTargetState(CardState.Furor));
                            await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Docile));
                        }
                        if (!this[CardState.Furor])//如果不处于温顺状态
                        {
                            await GameSystem.StateSystem.ClearState(new TriggerInfoModel(this, this).SetTargetState(CardState.Docile));
                            await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Furor));
                        }
                        //狂躁效果
                    }
                    if (triggerInfo.targetState == CardState.Docile)
                    {
                        //温顺效果
                        UnityEngine.Debug.Log("温顺");

                    }
                }, Condition.Default)
                .AbilityAppend();
        }
    }
}