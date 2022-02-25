using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:美国小妖精
    /// </summary>
    public class Card2002006 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this, this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
             .AbilityAdd(async (triggerInfo) =>
             {
                 await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardField.Inspire].CardList, 2, false);
                 GameSystem.InfoSystem.SelectUnits.ForEach(async unite =>
                 {
                     await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, unite).SetTargetField(CardField.Inspire, unite[CardField.Inspire] * 2));
                 });
                 foreach (var unite in GameSystem.InfoSystem.SelectUnits)
                 {
                    
                 }
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}