using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:诹坊子
    /// </summary>
    public class Card2201002 : Card
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
            AbalityRegister(TriggerTime.When, TriggerType.TurnEnd)
               .AbilityAdd(async (triggerInfo) =>
               {
                   if (RightCard != null)
                   {
                       await GameSystem.TransferSystem.MoveCard(new TriggerInfoModel(this, this).SetLocation(RightCard.CurrentOrientation, RightCard.CurrentRegion, RightCard.CurrentIndex));
                       await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, card).SetPoint(1));
                       await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, card).SetPoint(1));
                   }
               })
               .AbilityAppend();
        }
    }
}