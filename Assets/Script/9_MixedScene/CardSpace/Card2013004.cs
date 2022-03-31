using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:隙间的折叠伞
    /// </summary>
    public class Card2013004 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Silver][CardRank.Copper][CardType.Unite].CardList, 1);
                   if (GameSystem.InfoSystem.SelectUnits.Any())
                   {
                       Card targetCard = GameSystem.InfoSystem.SelectUnits.First();
                       switch (targetCard.CurrentRegion)
                       {
                           case GameRegion.Water: await GameSystem.TransferSystem.MoveCard(new TriggerInfoModel(this, targetCard).SetLocation(targetCard.CurrentOrientation, GameRegion.Fire, -1)); break;
                           case GameRegion.Fire: await GameSystem.TransferSystem.MoveCard(new TriggerInfoModel(this, targetCard).SetLocation(targetCard.CurrentOrientation, GameRegion.Water, 0)); break;
                           case GameRegion.Wind: await GameSystem.TransferSystem.MoveCard(new TriggerInfoModel(this, targetCard).SetLocation(targetCard.CurrentOrientation, GameRegion.Soil, 0)); break;
                           case GameRegion.Soil: await GameSystem.TransferSystem.MoveCard(new TriggerInfoModel(this, targetCard).SetLocation(targetCard.CurrentOrientation, GameRegion.Wind, -1)); break;
                           default: break;
                       }
                   }
                   await GameSystem.TransferSystem.MoveToGrave(this);
               })
               .AbilityAppend();
        }
    }
}