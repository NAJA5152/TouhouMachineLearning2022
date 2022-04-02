using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:荷城河取
    /// </summary>
    public class Card2100001 : Card
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
                   await GameSystem.SelectSystem.SelectBoardCard(this,GameSystem.InfoSystem.AgainstCardSet[CardTag.Machine][Orientation.My][GameRegion.Grave].CardList,num:2);
                   await GameSystem.TransferSystem.ReviveCard(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectBoardCards));
               },Condition.Default)
               .AbilityAppend();
        }
    }
}

