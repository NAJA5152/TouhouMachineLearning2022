using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:八班神奈子
    /// </summary>
    public class Card2201001 : Card
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
            AbalityRegister(TriggerTime.After, TriggerType.Move)
               .AbilityAdd(async (triggerInfo) =>
               {
                   triggerInfo.targetCards.ForEach(async card =>
                   {
                       if (GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle][CardRank.NoGold].CardList.Contains(card))
                       {
                           await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, card).SetPoint(1).SetBullet(new BulletModel()));
                       }
                   });
               }, Condition.Default, Condition.OnMyTurn)
               .AbilityAppend();
        }
    }
}