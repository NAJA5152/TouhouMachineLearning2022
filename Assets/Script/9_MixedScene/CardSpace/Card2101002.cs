using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:压力测试法
    /// </summary>
    public class Card2101002  : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
                .AbilityAdd(async (triggerInfo) =>
                {
                    await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardTag.Machine].CardList, 1);
                    await GameSystem.TransferSystem.MoveCard(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnit).SetLocation(Orientation.Op, GameSystem.InfoSystem.SelectUnit.CurrentRegion, -1));
                })
               .AbilityAdd(async (triggerInfo) =>
               {
                   if (GameSystem.InfoSystem.SelectUnit != null)
                   {
                       int num = GameSystem.InfoSystem.SelectUnit[CardField.Energy];
                       await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnit).SetTargetField(CardField.Energy, 0));
                       await GameSystem.PointSystem.Hurt(new TriggerInfoModel(GameSystem.InfoSystem.SelectUnit, GameSystem.InfoSystem.SelectUnit.BelongCardList).SetPoint(num).SetMeanWhile());
                   }
               })
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.TransferSystem.MoveToGrave(this);
               })
               .AbilityAppend();
        }
    }
}