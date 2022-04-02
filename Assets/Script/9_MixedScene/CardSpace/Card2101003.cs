using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:尼斯湖水怪
    /// </summary>
    public class Card2101003 : Card
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

                  int energyPoint = TwoSideCard.Sum(card => card[CardField.Energy]);
                  await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, TwoSideCard).SetTargetField(CardField.Energy, 0));
                  await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle][ Orientation.Op][CardFeature.LargestUnites].CardList).SetPoint(energyPoint));
              }, Condition.Default, Condition.OnMyTurn)
              .AbilityAppend();
        }
    }
}