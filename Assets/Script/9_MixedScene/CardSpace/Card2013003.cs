using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:亡灵的送行提灯
    /// 卡牌能力:赋予一个单位延命1
    /// </summary>
    public class Card2013003 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectUnit(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Silver][CardRank.Copper][CardType.Unit].CardList, 1);
                   await GameSystem.FieldSystem.ChangeField(new Event(this, GameSystem.InfoSystem.SelectUnits).SetTargetState(CardState.Apothanasia));
                   await GameSystem.TransferSystem.MoveToGrave(this);
               })
               .AbilityAppend();
        }
    }
}