using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:���¾�*��
    /// ��������:����:�����������Ϊ2, ���ٻ����¾�*�գ����¾�*�� ���ٻ�ʱ:�����������Ϊ2
    /// </summary>
    public class Card2002002 : Card
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
            //����Ч��
            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
             .AbilityAdd(async (e) =>
             {
                 await GameSystem.FieldSystem.SetField(new Event(this, this).SetTargetField(CardField.Inspire, 2));
                 await GameSystem.TransferSystem.SummonCard(
                     new Event(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck].CardList
                     .Where(card => card.CardID == 2002001 || card.CardID == 2002003)
                     .ToList()
                     ));
             }, Condition.Default)
             .AbilityAppend();
            //���ٻ�ʱЧ��
            AbalityRegister(TriggerTime.When, TriggerType.Summon)
                .AbilityAdd(async (e) =>
                {
                    await GameSystem.FieldSystem.SetField(new Event(this, this).SetTargetField(CardField.Inspire, 2));
                }, Condition.Default)
                .AbilityAppend();
        }
    }
}