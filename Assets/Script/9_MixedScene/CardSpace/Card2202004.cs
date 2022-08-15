using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:微概率的奇迹
    /// 卡牌能力:打出牌组中两张铜色奇迹牌
    /// </summary>
    public class Card2202004 : Card
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