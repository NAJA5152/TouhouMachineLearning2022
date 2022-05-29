using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:�ļ�ӳ��
    /// </summary>
    public class Card2081003 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   await GameSystem.TransferSystem.DeployCard(new TriggerInfoModel(this,this));
               })
               .AbilityAppend();

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectUnite(this,GameSystem.InfoSystem.AgainstCardSet[CardRank.NoGold].CardList,1);
                   await GameSystem.StateSystem.SetState(new TriggerInfoModel(this,GameSystem.InfoSystem.SelectUnit).SetTargetState( CardState.Black));
                   await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[CardRank.NoGold].CardList, 1);
                   await GameSystem.StateSystem.SetState(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnit).SetTargetState(CardState.White));
               })
               .AbilityAppend();
        }
    }
}