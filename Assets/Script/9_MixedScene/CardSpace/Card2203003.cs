using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:请神
    /// 卡牌能力:从牌组中打出点数小于自身祈祷值且最接近的铜色单位
    /// </summary>
    public class Card2203003 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   var targetCard = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck][CardRank.Copper]
                   .CardList
                   .Where(card => card.ShowPoint <= this[CardField.Pary])
                   .OrderBy(card => card.ShowPoint)
                   .FirstOrDefault();

                   await GameSystem.TransferSystem.PlayCard(new Event(this, targetCard));
                   await GameSystem.TransferSystem.MoveToGrave(this);

               })
               .AbilityAppend();
        }
    }
}