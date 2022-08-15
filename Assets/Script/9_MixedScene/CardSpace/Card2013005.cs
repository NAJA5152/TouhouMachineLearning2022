using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:闪避布
    /// 卡牌能力:为一个非金单位添加隐身状态
    /// </summary>
    public class Card2013005 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Silver][CardRank.Copper][CardType.Unit].CardList, 1);
                   await GameSystem.StateSystem.SetState(new Event(this, GameSystem.InfoSystem.SelectUnits).SetTargetState(CardState.Invisibility));
                   await GameSystem.TransferSystem.MoveToGrave(this);
               })
               .AbilityAppend();
        }
    }
}