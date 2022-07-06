using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// 卡牌名称:大妖精
    /// 卡牌能力:部署:治愈一个非金妖精单位并重新触发其部署效果
    /// </summary>
    public class Card2001001 : Card
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
                   await GameSystem.SelectSystem.SelectUnite(this, AgainstInfo.cardSet[Orientation.My][GameRegion.Battle][CardRank.Copper, CardRank.Silver][GameEnum.CardTag.Fairy].CardList, 1);
                   await GameSystem.PointSystem.Cure
                   (
                       new Event(this, GameSystem.InfoSystem.SelectUnits)
                       .SetBullet(new BulletModel(BulletType.BigBall, BulletColor.Green, BulletTrack.Line))
                   );
                   if (AgainstInfo.SelectUnits.Any())
                   {
                       AgainstInfo.SelectRowRank = AgainstInfo.SelectUnits[0].Location.X;
                       AgainstInfo.SelectRank = AgainstInfo.SelectUnits[0].Location.Y;
                   }
                   await GameSystem.TransferSystem.DeployCard(new Event(this, GameSystem.InfoSystem.SelectUnits));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}