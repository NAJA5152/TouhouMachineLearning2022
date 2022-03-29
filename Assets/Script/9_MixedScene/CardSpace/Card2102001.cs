using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:压力测试法
    /// </summary>
    public class Card2102001 : Card
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

                    ////if (GameSystem.InfoSystem.SelectUnit!=null)
                    ////{
                    ////}
                })
               .AbilityAdd(async (triggerInfo) =>
               {
                   if (GameSystem.InfoSystem.SelectUnit != null)
                   {
                       int num = GameSystem.InfoSystem.SelectUnit[CardField.Energy];
                       await GameSystem.PointSystem.Hurt(new TriggerInfoModel(GameSystem.InfoSystem.SelectUnit, GameSystem.InfoSystem.SelectUnit.belongCardList).SetPoint(num).SetMeanWhile());
                   }
               })
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.TransferSystem.MoveToGrave(new TriggerInfoModel(this, this));
               })
               .AbilityAppend();
        }
    }
}