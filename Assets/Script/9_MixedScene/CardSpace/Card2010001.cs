using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:鬼人正邪
    /// </summary>
    public class Card2010001 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            UnityEngine.Debug.LogError("政协初始化");
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   UnityEngine.Debug.LogError("选择位置");
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   UnityEngine.Debug.LogError("部署");
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this, this));
               }, Condition.Default)
               .AbilityAppend();
            //AbalityRegister(TriggerTime.Before, TriggerType.Deploy)
            //   .AbilityAdd(async (triggerInfo) =>
            //   {
            //       UnityEngine.Debug.LogError("触发回合前效果");
            //       //await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Silver, CardRank.Copper].CardList, 1);
            //       //await GameSystem.PointSystem.Reversal(new TriggerInfoModel(triggerInfo.triggerCard, GameSystem.InfoSystem.SelectUnits));
            //   }, Condition.Default)
            //   .AbilityAppend();
        }
    }
}