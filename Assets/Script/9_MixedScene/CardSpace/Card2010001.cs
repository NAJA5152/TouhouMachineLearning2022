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
            UnityEngine.Debug.LogError("��Э��ʼ��");
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   UnityEngine.Debug.LogError("ѡ��λ��");
                   await GameSystem.SelectSystem.SelectLocation(this, CardDeployTerritory, CardDeployRegion);
                   UnityEngine.Debug.LogError("����");
                   await GameSystem.TransSystem.DeployCard(new TriggerInfoModel(this, this));
               }, Condition.Default)
               .AbilityAppend();
            //AbalityRegister(TriggerTime.Before, TriggerType.Deploy)
            //   .AbilityAdd(async (triggerInfo) =>
            //   {
            //       UnityEngine.Debug.LogError("�����غ�ǰЧ��");
            //       //await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Silver, CardRank.Copper].CardList, 1);
            //       //await GameSystem.PointSystem.Reversal(new TriggerInfoModel(triggerInfo.triggerCard, GameSystem.InfoSystem.SelectUnits));
            //   }, Condition.Default)
            //   .AbilityAppend();
        }
    }
}