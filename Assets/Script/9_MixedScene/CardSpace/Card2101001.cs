using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:黄瓜加特林
    /// </summary>
    public class Card2101001 : Card
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
                   var cardList = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardField.Energy].CardList;
                   int num = cardList.Sum(card => card[CardField.Energy]);
                   await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, cardList).SetTargetField(CardField.Energy, 0));
                   for (int i = 0; i < num; i++)
                   {
                       await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Battle][CardRank.Silver][CardRank.Copper][CardFeature.Largest].CardList.FirstOrDefault()));
                   }
               })
               .AbilityAppend();
        }
    }
}