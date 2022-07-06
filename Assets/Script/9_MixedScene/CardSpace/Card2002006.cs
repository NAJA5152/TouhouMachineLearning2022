using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:����С����
    /// ��������:����:ѡ�������ǽ�������λ�����������ֵ
    /// </summary>
    public class Card2002006 : Card
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
                 await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardField.Inspire].CardList, 2, false);
                 GameSystem.InfoSystem.SelectUnits.ForEach(async unite =>
                 {
                     await GameSystem.FieldSystem.SetField(new Event(this, unite).SetTargetField(CardField.Inspire, unite[CardField.Inspire] * 2));
                 });
                 foreach (var unite in GameSystem.InfoSystem.SelectUnits)
                 {
                    
                 }
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}