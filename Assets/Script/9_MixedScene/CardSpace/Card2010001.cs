using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:������а
    /// </summary>
    public class Card2010001 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.UiSystem.ShowFigure(this);
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this, this));
               })
               .AbilityAppend();
            AbalityRegister(TriggerTime.Before, TriggerType.Deploy)
               .AbilityAdd(async (triggerInfo) =>
               {
                   if (Info.AgainstInfo.IsMyTurn)
                   {
                       List<Card> targetCards = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Silver, CardRank.Copper].CardList;
                       targetCards.Remove(triggerInfo.triggerCard);
                       await GameSystem.SelectSystem.SelectUnite(this, targetCards, 1);
                       await GameSystem.PointSystem.Reversal(new TriggerInfoModel(triggerInfo.triggerCard, GameSystem.InfoSystem.SelectUnits));
                   };
               }, Condition.Default)
               .AbilityAppend();
            
            //AbalityRegister(TriggerTime.When, TriggerType.Deploy)
            //   .AbilityAdd(async (triggerInfo) =>
            //   {

            //       await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, this).SetPoint(1));
            //   }, Condition.Default)
            //   .AbilityAppend();
            //AbalityRegister(TriggerTime.When, TriggerType.Increase)
            //  .AbilityAdd(async (triggerInfo) =>
            //  {
            //      UnityEngine.Debug.Log("����");
            //      if (!this[CardState.Furor])
            //      {
            //          await GameSystem.StateSystem.ClearState(new TriggerInfoModel(this, this).SetTargetState(CardState.Docile));
            //          await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Furor));

            //          await System.Threading.Tasks.Task.Delay(2200);
            //          await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, this).SetPoint(1));

            //      }
            //  }, Condition.Default)
            //  .AbilityAppend();
            //AbalityRegister(TriggerTime.When, TriggerType.Decrease)
            //   .AbilityAdd(async (triggerInfo) =>
            //   {
            //       UnityEngine.Debug.Log("����");
            //       if (!this[CardState.Docile])
            //       {
            //           await GameSystem.StateSystem.ClearState(new TriggerInfoModel(this, this).SetTargetState(CardState.Furor));
            //           await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, this).SetTargetState(CardState.Docile));


            //           await System.Threading.Tasks.Task.Delay(2200);
            //           await GameSystem.PointSystem.Gain(new TriggerInfoModel(this, this).SetPoint(1));
            //       }
            //   }, Condition.Default)
            //    .AbilityAppend();
        }
    }
}