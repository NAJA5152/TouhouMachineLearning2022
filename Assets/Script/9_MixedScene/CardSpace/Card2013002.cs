using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:万宝槌（仿制品）
    /// </summary>
    public class Card2013002 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle][CardRank.Silver][CardRank.Copper][CardType.Unite].CardList, 1);
                   if (GameSystem.InfoSystem.SelectUnits.Any())
                   {
                       Card targetCard = GameSystem.InfoSystem.SelectUnit;
                       await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, targetCard).SetPoint(targetCard.ShowPoint - 1));
                   }
                   await GameSystem.TransSystem.MoveToGrave(new TriggerInfoModel(this, this));
               })
               .AbilityAppend();
        }
    }
}