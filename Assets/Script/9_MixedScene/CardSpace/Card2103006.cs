using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:河童的天才维修法
    /// </summary>
    public class Card2103006 : Card
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
                   await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Copper][CardTag.Machine].CardList, 1);
                   await GameSystem.PointSystem.Destory(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnit));

                   var targetCard = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck].CardList
                           .Where(card => card.CardID == GameSystem.InfoSystem.SelectUnit.CardID).ToList().FirstOrDefault();
                   await GameSystem.TransferSystem.SummonCard(new TriggerInfoModel(this, targetCard));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}