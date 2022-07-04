using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:狂暴之头
    /// 卡牌能力:每回合开始正序移动至最下一排最右侧并对左侧单位造成1点伤害
    /// </summary>
    public class Card2013006 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new TriggerInfoModel(this,this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.TurnEnd)
              .AbilityAdd(async (triggerInfo) =>
              {
                  await GameSystem.TransferSystem.MoveCard(new TriggerInfoModel(this, this).SetLocation(OppositeOrientation, NextBattleRegion, -1));
                  await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, this.LeftCard).SetPoint(1));
              }, Condition.Default)
              .AbilityAppend();
        }
    }
}