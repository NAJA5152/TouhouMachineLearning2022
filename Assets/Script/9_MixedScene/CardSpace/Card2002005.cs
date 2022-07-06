using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:莉莉白
    /// 卡牌能力:部署:增加场上所有非金妖精牌1点鼓舞
    /// </summary>
    public class Card2002005 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (e) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new Event(this, this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
             .AbilityAdd(async (e) =>
             {
                 await GameSystem.FieldSystem.ChangeField(new Event(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][CardField.Inspire].CardList).SetTargetField( CardField.Inspire,1)); 
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}