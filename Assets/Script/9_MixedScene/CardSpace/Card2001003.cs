using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:露娜
    /// </summary>
    public class Card2001003 : Card
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
                 for (int i = 0; i < 1 + GameSystem.InfoSystem.GetTwoSideField(this, CardField.Inspire); i++)
                 {
                     await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Battle][CardRank.Copper, CardRank.Silver][CardFeature.Largest].CardList, 1, true);
                     await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnits).SetPoint(1));
                 }
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}