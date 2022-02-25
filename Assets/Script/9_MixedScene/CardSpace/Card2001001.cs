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
    /// ��������:������
    /// </summary>
    public class Card2001001 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   UnityEngine.Debug.Log("��ʼѡ������");
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   UnityEngine.Debug.Log("ѡ��λ���");
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this, this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (triggerInfo) =>
               {
                   UnityEngine.Debug.Log("��ʼѡ��λ");
                   await GameSystem.SelectSystem.SelectUnite(this, AgainstInfo.cardSet[Orientation.My][GameRegion.Battle][CardRank.Copper, CardRank.Silver][CardTag.Fairy].CardList, 1);
                   UnityEngine.Debug.Log("ѡ��λ���");
                   await GameSystem.PointSystem.Cure
                   (
                       new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnits)
                       .SetBullet(new BulletModel(BulletType.BigBall, BulletColor.Green, BulletTrack.Line))
                   );
                   if (AgainstInfo.SelectUnits.Any())
                   {
                       AgainstInfo.SelectRowRank = AgainstInfo.SelectUnits[0].Location.X;
                       AgainstInfo.SelectRank = AgainstInfo.SelectUnits[0].Location.Y;
                   }
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnits));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}