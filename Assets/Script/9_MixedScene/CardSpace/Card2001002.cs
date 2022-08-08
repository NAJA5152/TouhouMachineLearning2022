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
    /// ��������:ɣ��*�ѿ�
    /// ��������:����:�ݻٳ��Ϸǽ��ҵ����������е�λ
    /// </summary>
    public class Card2001002 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
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
                  await GameSystem.PointSystem.Destory(
                      new Event(this, GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle][CardRank.Copper, CardRank.Silver][CardFeature.LargestUnits].CardList)
                      );
              }, Condition.Default)
              .AbilityAppend();
        }
    }
}