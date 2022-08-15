using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:卡牌生成模板
    /// 卡牌能力:无
    /// </summary>
    public class Card0 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   
                   await GameSystem.TransferSystem.MoveToGrave(this);
               })
               .AbilityAppend();
        }
    }
}