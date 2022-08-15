using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:祈求命运的赌徒
    /// 卡牌能力:展现牌库中最上方的铜卡，选择是否打出，若选择了跳过或牌组中不存在铜卡，则直接打出牌库最上方的银卡
    /// </summary>
    public class Card2202005 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {

                   await GameSystem.SelectSystem.SelectBoardCard(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck][CardTag.Miracle].CardList, num: 2);
                   await GameSystem.TransferSystem.PlayCard(new Event(this, GameSystem.InfoSystem.SelectUnits));
                   await GameSystem.TransferSystem.MoveToGrave(this);
               })
               .AbilityAppend();
        }
    }
}