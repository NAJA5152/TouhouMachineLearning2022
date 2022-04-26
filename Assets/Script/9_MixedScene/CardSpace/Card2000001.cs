using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:������¶ŵ
    /// </summary>
    public class Card2000001 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new TriggerInfoModel(this, this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (triggerInfo) =>
               {
                   int targetCount = AgainstInfo.cardSet[Orientation.My][GameRegion.Battle][CardTag.Fairy].CardList.Count-1;
                   Debug.Log("������������Ϊ" + targetCount);
                   for (int i = 0; i < AgainstInfo.cardSet[Orientation.My][GameRegion.Battle][CardTag.Fairy].CardList.Count; i++)
                   {

                       List<Card> cardlist = AgainstInfo.cardSet[Orientation.Op][GameRegion.Battle][CardFeature.LargestUnites].CardList.ToList();
                       if (cardlist.Any())
                       {
                           await GameSystem.SelectSystem.SelectUnite(this, cardlist, 1, isAuto: true);
                           await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, AgainstInfo.SelectUnits).SetPoint(1));
                           if (BasePoint > 1)
                           {
                               await GameSystem.PointSystem.Weak(new TriggerInfoModel(this, this).SetPoint(1));
                           }
                       }
                   }
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}