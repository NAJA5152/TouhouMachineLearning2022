using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:应急充能箱
    /// </summary>
    public class Card2102002 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this,this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardField.Energy].CardList, 2);
                   await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, this).SetTargetField(CardField.Energy, GameSystem.InfoSystem.SelectUnits.Sum(card => card[CardField.Energy])));
                   await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnits).SetTargetField(CardField.Energy, 0));
               })
               .AbilityAppend();
        }
    }
}