using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:四季映姬
    /// 卡牌能力:部署：选择两个单位，依次赋予黑、白状态 回合开始，赋予左侧单位黑状态，赋予右侧单位白状态
    /// </summary>
    public class Card2081003 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new TriggerInfoModel(this,this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectUnite(this,GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.NoGold].CardList,1);
                   await GameSystem.StateSystem.SetState(new TriggerInfoModel(this,GameSystem.InfoSystem.SelectUnit).SetTargetState( CardState.Black));
                   await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.NoGold].CardList, 1);
                   await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnit).SetTargetState(CardState.White));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.TurnStart)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, LeftCard).SetTargetState(CardState.Black));
                   await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, RightCard).SetTargetState(CardState.White));
               })
               .AbilityAppend();
        }
    }
}