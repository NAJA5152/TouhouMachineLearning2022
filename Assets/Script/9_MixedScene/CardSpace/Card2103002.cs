using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:���ܵ�����
    /// </summary>
    public class Card2103002 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this,this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.TurnEnd)
              .AbilityAdd(async (triggerInfo) =>
              {
                  if (this[CardField.Energy] < 8)
                  {
                      await GameSystem.FieldSystem.ChangeField(new TriggerInfoModel(this, this).SetTargetField(CardField.Energy, 1));
                  }
                  else
                  {
                      await GameSystem.UiSystem.ShowTips(this, "����", new Color(1, 0, 0));
                      await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this,GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle].CardList).SetPoint(1).SetMeanWhile());
                      await GameSystem.PointSystem.Destory(new TriggerInfoModel(this, this));
                  }
              }, Condition.Default, Condition.OnMyTurn)
              .AbilityAppend();
        }
    }
}