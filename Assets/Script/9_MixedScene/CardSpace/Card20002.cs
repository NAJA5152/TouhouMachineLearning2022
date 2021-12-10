using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    public class Card20002 : Card
    {
        public override void Init()
        {
            //初始化通用卡牌效果
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, region, territory);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfo(this).SetTargetCard(this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
                .AbilityAdd(async (triggerInfo) =>
                {
                    await GameSystem.PointSystem.Gain(new TriggerInfo(this).SetTargetCard(this).SetPoint(10));
                    UnityEngine.Debug.Log("增益自身10点");
                }, Condition.Default)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectUnite(this, AgainstInfo.cardSet[Orientation.My][GameRegion.Battle][CardRank.Copper, CardRank.Silver][CardTag.Fairy].CardList, 1);
                   await GameSystem.PointSystem.Cure
                   (
                       new TriggerInfo(this)
                       .SetTargetCard(AgainstInfo.selectUnits)
                       .SetBullet(new BulletModel(BulletType.BigBall, BulletColor.Green, BulletTrack.Line))
                   );
                   if (AgainstInfo.selectUnits.Any())
                   {
                       AgainstInfo.SelectRegion = RowsInfo.GetSingleRowInfoById(AgainstInfo.selectUnits[0].location.X);
                       AgainstInfo.SelectLocation = AgainstInfo.selectUnits[0].location.Y;
                   }
                   await GameSystem.TransSystem.DeployCard(new TriggerInfo(this).SetTargetCard(AgainstInfo.selectUnits));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}