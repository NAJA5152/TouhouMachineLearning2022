using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:充能水炮
    /// </summary>
    public class Card2103004 : Card
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
                 int num = LeftCard[CardField.Energy];
                 await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, LeftCard).SetTargetField(CardField.Energy, 0));

                 await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Battle][CardRank.Silver, CardRank.Copper].CardList, 1);
                 await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnits).SetPoint(num));
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}