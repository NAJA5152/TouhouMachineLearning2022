using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��С����
    /// ��������:����:���1����������ӿ�����������һ�ŵ�����͵�ͭɫ������λ
    /// </summary>
    public class Card2003003 : Card
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
                 await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, this).SetTargetField(CardField.Inspire,1));
                 await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Deck][CardRank.Copper][CardTag.Fairy][CardFeature.LowestUnites].CardList, 1, true);
                 await GameSystem.TransferSystem.PlayCard(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnits));
             }, Condition.Default)
             .AbilityAppend();
        }
    }
}