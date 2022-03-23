using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:压力测试法
    /// </summary>
    public class Card2102001 : Card
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

            //AbalityRegister(TriggerTime.When, TriggerType.Deploy)
            //   .AbilityAdd(async (triggerInfo) =>
            //   {
            //       await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[CardTag.Machine][Orientation.My][GameRegion.Grave].CardList,1);
            //       await GameSystem.TransSystem.MoveCard(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnit).SetLocation());

            //   })
            //   .AbilityAdd(async (triggerInfo) =>
            //   {
            //       await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[CardTag.Machine][Orientation.My][GameRegion.Grave].CardList, 1);
            //       await GameSystem.TransSystem.MoveCard(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnit).SetLocation());

            //   })
            //   .AbilityAppend();
        }
    }
}