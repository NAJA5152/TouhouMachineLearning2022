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

            AbalityRegister(TriggerTime.When, TriggerType.StateAdd)
              .AbilityAdd(async (triggerInfo) => { 
                  if (triggerInfo.targetState== CardState.Furor)
                  {
                      //狂躁效果
                  }
                  if (triggerInfo.targetState == CardState.Docile)
                  {
                      //温顺效果
                  }
              },Condition.Default)
              .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Gain)
               .AbilityAdd(async (triggerInfo) => { await Command.CardCommand.Gain(triggerInfo); })
               .AbilityReplace();
                //替换
                AbalityRegister(TriggerTime.When, TriggerType.Hurt)
                .AbilityAdd(async (triggerInfo) =>
                {
                    if (this[cardState: CardState.Congealbounds])
                    {
                        await GameSystem.StateSystem.ClearState(new TriggerInfoModel(triggerInfo.triggerCard, this));
                    }
                    else
                    {
                        await Command.CardCommand.Hurt(triggerInfo);
                        if (!this[CardState.Docile])//如果不处于温顺状态
                        {
                            await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Docile));
                        }
                    }
                })
                .AbilityReplace();
        }
    }
}