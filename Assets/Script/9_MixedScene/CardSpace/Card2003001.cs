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
    /// ��������:����:���1����裬ѡ������һ���ǽ�������λ
    /// </summary>
    public class Card2003001 : Card
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
                 await GameSystem.FieldSystem.ChangeField(new TriggerInfoModel(this, this).SetTargetField( CardField.Inspire,1));
                 await GameSystem.SelectSystem.SelectUnite(this, AgainstInfo.cardSet[Orientation.My][GameRegion.Battle][CardRank.Silver,CardRank.Copper].CardList, 1);
                 await GameSystem.PointSystem.Cure(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnits));
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}