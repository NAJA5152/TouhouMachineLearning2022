using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:鬼人正邪
    /// </summary>
    public class Card2010001 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.UiSystem.ShowFigure(this);
                   await GameSystem.TransferSystem.DeployCard(new TriggerInfoModel(this, this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.Before, TriggerType.Deploy)
               .AbilityAdd(async (triggerInfo) =>
               {
                   if (Info.AgainstInfo.IsMyTurn)
                   {
                       List<Card> targetCards = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Silver, CardRank.Copper].CardList;
                       targetCards.Remove(triggerInfo.triggerCard);
                       await GameSystem.SelectSystem.SelectUnite(this, targetCards, 1);
                       await GameSystem.PointSystem.Reversal(new TriggerInfoModel(triggerInfo.triggerCard, GameSystem.InfoSystem.SelectUnits));
                   };
               }, Condition.Default)
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.TurnEnd)
             .AbilityAdd(async (triggerInfo) =>
             {
                 await GameSystem.TransferSystem.GenerateCard(new TriggerInfoModel(this, targetCard: null).SetTargetCardId(2013006).SetLocation(CurrentOrientation, CurrentRegion, -1));
                 await GameSystem.TransferSystem.GenerateCard(new TriggerInfoModel(this, targetCard: null).SetTargetCardId(2013007).SetLocation(OppositeOrientation, CurrentRegion, -1));
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}