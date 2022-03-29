using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:ˮ�������
    /// </summary>
    public class Card2103001 : Card
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

            AbalityRegister(TriggerTime.When, TriggerType.TurnEnd)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.FieldSystem.ChangeField(new TriggerInfoModel(this, this).SetTargetField(CardField.Energy, 1));
                   await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this.TwoSideCard).SetTargetState(CardState.Water).SetMeanWhile());
                   if (this[CardField.Energy] > 3)
                   {
                       await GameSystem.UiSystem.ShowTips(this, "����", new Color(1, 0, 0));
                       await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, this.TwoSideCard).SetPoint(2).SetMeanWhile());
                       await GameSystem.PointSystem.Destory(new TriggerInfoModel(this, this));
                   }
               }, Condition.Default, Condition.OnMyTurn)
               .AbilityAppend();
        }
    }
}