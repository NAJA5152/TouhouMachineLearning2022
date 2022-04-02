using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:急速充能器
    /// </summary>
    public class Card2101004 : Card
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
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, this).SetTargetField(CardField.Timer, 2));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.TurnEnd)
              .AbilityAdd(async (triggerInfo) =>
              {
                  if (this[CardField.Timer] == 1)
                  {
                      await GameSystem.FieldSystem.ChangeField(new TriggerInfoModel(this, this).SetTargetField(CardField.Timer, -1));
                      this.TwoSideCard.ForEach(async card =>
                      {
                          if (card.CardID == 2103001)
                          {
                              await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, card).SetTargetField(CardField.Energy, 3));
                          }
                          if (card.CardID == 2103002)
                          {
                              await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, card).SetTargetField(CardField.Energy, 8));
                          }
                      });
                      await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, this).SetTargetField(CardField.Timer, 2));
                  }
                  else
                  {
                      await GameSystem.FieldSystem.ChangeField(new TriggerInfoModel(this, this).SetTargetField(CardField.Timer, -1));
                  }
              })
              .AbilityAppend();
        }
    }
}