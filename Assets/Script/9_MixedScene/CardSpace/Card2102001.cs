using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:手摇发电机
    /// </summary>
    public class Card2102001 : Card
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
            AbalityRegister(TriggerTime.Before, TriggerType.Deploy)
              .AbilityAdd(async (triggerInfo) =>
              {
                  await GameSystem.FieldSystem.ChangeField(new TriggerInfoModel(this, this).SetTargetField(CardField.Energy, 1));
              }, Condition.Default, Condition.OnMyTurn)
              .AbilityAppend();
        }
    }
}