using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:弱小妖精
    /// </summary>
    public class Card2003003 : Card
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

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
             .AbilityAdd(async (triggerInfo) =>
             {
                 await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, this).SetTargetField(CardField.Inspire,1));
                 await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck][CardRank.Copper][CardFeature.Lowest][GameEnum.CardTag.Fairy][CardType.Unite].CardList, 1, true);
                 await GameSystem.TransSystem.PlayCard(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnits));
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}