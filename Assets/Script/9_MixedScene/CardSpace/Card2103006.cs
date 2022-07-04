using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��ͯ�����ά�޷�
    /// ��������:����ѡ�񲢽��ҷ�����һ����е��λֱ������Ĺ�أ�����Ϊ�˺��������������������д����ͬ����λ
    /// </summary>
    public class Card2103006 : Card
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
                   await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Copper][CardTag.Machine].CardList, 1);
                   await GameSystem.PointSystem.Destory(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnit));

                   var targetCard = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck].GetSameIdCard(GameSystem.InfoSystem.SelectUnit.CardID, 1);
                   await GameSystem.TransferSystem.SummonCard(new TriggerInfoModel(this, targetCard));
               }, Condition.Default)
               .AbilityAppend();
        }
    }
}