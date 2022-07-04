using System.Linq;
using System.Collections.Generic;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.GameEnum;
namespace TouhouMachineLearningSummary.CardSpace
{
    /// <summary>
    /// ��������:��ҵ����
    /// ��������:�����ҷ���λ��������ֵ����������
    /// </summary>
    public class Card2203004 : Card
    {
        public override void Init()
        {
            //��ʼ��ͨ�ÿ���Ч��
            base.Init();
            AbalityRegister(TriggerTime.When, TriggerType.Play)
               .AbilityAdd(async (triggerInfo) =>
               {
                   await GameSystem.SelectSystem.SelectUnite(this, GameSystem.InfoSystem.AgainstCardSet[GameRegion.Battle][CardRank.NoGold].CardList, 1);
                   await GameSystem.PointSystem.Gain(new TriggerInfoModel(this, GameSystem.InfoSystem.SelectUnits).SetPoint(this[CardField.Pary]));
               })
               .AbilityAppend();
        }
    }
}