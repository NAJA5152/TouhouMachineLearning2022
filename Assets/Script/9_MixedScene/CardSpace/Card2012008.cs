using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:万宝槌
    /// 卡牌能力:对同排所有单位产生2点增益
    /// </summary>
    public class Card2012008 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectRegion(this);
                   await GameSystem.PointSystem.Gain(new Event(this, GameSystem.InfoSystem.SelectRegionCardList).SetPoint(2).SetMeanWhile());
                   await GameSystem.TransferSystem.MoveToGrave(this);
               })
               .AbilityAppend();
        }
    }
}