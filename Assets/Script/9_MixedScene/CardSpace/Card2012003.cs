using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:今泉影狼
    /// 卡牌能力:暴躁：对对方同排所有目标获得一点伤害 温顺：按正顺序移动至下一排
    /// </summary>
    public class Card2012003 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new TriggerInfoModel(this, this));

               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Increase)
               .AbilityAdd(async (triggerInfo) =>
               {
                   if (!this[CardState.Furor])//如果不处于狂躁状态
                   {
                       await GameSystem.StateSystem.ClearState(new TriggerInfoModel(this, this).SetTargetState(CardState.Docile));
                       await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Furor));

                       await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][CurrentRegion].CardList).SetPoint(1).SetMeanWhile());

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

                       await GameSystem.TransferSystem.MoveCard(new TriggerInfoModel(this, this).SetLocation(Orientation.My, NextBattleRegion, -1));
                   }
               }, Condition.Default)
                .AbilityAppend();
        }
    }
}