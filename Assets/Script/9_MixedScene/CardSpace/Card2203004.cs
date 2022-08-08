using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:商业繁荣
    /// 卡牌能力:给与我方单位与自身祈祷值等量的增益
    /// </summary>
    public class Card2203004 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle][CardRank.NoGold].CardList, 1);
                   await GameSystem.PointSystem.Gain(new Event(this, GameSystem.InfoSystem.SelectUnits).SetPoint(this[CardField.Pary]));
               })
               .AbilityAppend();
        }
    }
}