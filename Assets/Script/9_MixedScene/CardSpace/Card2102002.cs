using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:Ӧ��������
    /// </summary>
    public class Card2102002 : Card
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

            AbalityRegister(TriggerTime.When, TriggerType.Deploy)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[CardTag.Machine][Orientation.My][GameRegion.Grave].CardList, 1);

                   var cardList = GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardField.Energy].CardList;
                   int num = cardList.Sum(card => card[CardField.Energy]);
                   await GameSystem.FieldSystem.SetField(new TriggerInfoModel(this, cardList).SetTargetField(CardField.Energy, 0));
                   for (int i = 0; i < num; i++)
                   {
                       await GameSystem.PointSystem.Hurt(new TriggerInfoModel(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.Op][GameRegion.Battle][CardRank.Silver][CardRank.Copper][CardFeature.Largest].CardList.FirstOrDefault()));
                   }
               })
               .AbilityAppend();
        }
    }
}