using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using static TouhouMachineLearningSummary.Info.AgainstInfo;

namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:ս��ת��
    /// </summary>
    public class Card2003002 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[Orientation.My][GameRegion.Battle][CardRank.Copper][CardTag.Fairy].CardList, 1);
                   await GameSystem.TransferSystem.MoveToDeck(GameSystem.InfoSystem.SelectUnit);

                   await GameSystem.TransferSystem.PlayCard(new TriggerInfoModel(this, GameSystem.InfoSystem.AgainstCardSet[CardRank.Copper][CardFeature.Lowest][ CardType.Special].CardList));
                   await GameSystem.TransferSystem.MoveToGrave(this);
               })
               .AbilityAppend();
        }
    }
}