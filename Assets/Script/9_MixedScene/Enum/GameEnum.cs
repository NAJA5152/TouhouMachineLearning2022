using Sirenix.OdinInspector;
using UnityEngine;
namespace TouhouMachineLearningSummary.GameEnum
{
    public enum FirstTurn { PlayerFirst, OpponentFirst, Random }
    public enum AgainstModeType
    {
        Story,//����ģʽ
        Practice,//��ϰģʽ

        Casual,//����ģʽ
        Rank,//����ģʽ
        Arena,//������ģʽ
    }
    public enum NotifyBoardMode
    {
        Ok,
        Ok_Cancel,
        Cancel,
        Input
    }
    public enum PageType
    {
        CardList,
        Image,
        Text
    }
    public enum TriggerTime
    {
        Before,
        When,
        After
    }
    public enum TriggerType
    {
        ////////////////////////////////////////////////�ƶ�/////////////////////////////////////////
        Draw,
        Play,
        Deploy,
        Discard,
        Dead,
        /// <summary>
        /// ����
        /// </summary>
        Revive,
        /// <summary>
        /// λ��
        /// </summary>
        Move,
        /// <summary>
        /// ��϶
        /// </summary>
        Banish,
        /// <summary>
        /// �ٻ�
        /// </summary>
        Summon,
        ////////////////////////////////////////////////����/////////////////////////////////////////
        /// <summary>
        /// ����
        /// </summary>
        Gain,
        /// <summary>
        /// �˺�
        /// </summary>
        Hurt,
        /// <summary>
        /// ����
        /// </summary>
        Cure,
        Reset,
        Destory,
        Strengthen,
        Weak,
        ////////////////////////////////////////////////״̬/////////////////////////////////////////
        /// <summary>
        /// ��ӡ
        /// </summary>
        //Seal,
        //Close,
        ///// <summary>
        ///// ���
        ///// </summary>
        //Scout,
        ///// <summary>
        ///// ��ʾ
        ///// </summary>
        //Reveal,
        StateSet,
        StateClear,
        ////////////////////////////////////////////////�ֶ�/////////////////////////////////////////
        FieldSet,
        FieldChange,
        //FieldIncrease,
        //FieldDecrease,
        ////////////////////////////////////////////////ѡ��/////////////////////////////////////////

        SelectUnite,

        RoundStart,
        RoundEnd,
        TurnStart,
        TurnEnd
    }
    public enum CardState
    {
        Seal,//��ӡ
        Invisibility,//��ӡ
        Pry,//��̽
        Fate,//����
        Lurk,//Ǳ��
        Furor,//��
        Docile,//��˳
        Poisoning,//�ж�
        Rely,//ƾ��
    }
    public enum CardField
    {
        Timer,//��ʱ
        Vitality,//����
        Point
    }
    public enum Camp
    {
        Neutral,
        Taoism,
        Shintoism,
        Buddhism,
        science
    }
    public enum GameRegion
    {
        Water,
        Fire,
        Wind,
        Soil,
        Leader,
        Hand,
        Uesd,
        Deck,
        Grave,
        Battle = 99,
        None = 100,
    }
    public enum BattleRegion
    {
        Water, Fire, Wind, Soil, All = 99, None = 100
    }

    public enum CardType
    {
        Unite,
        Special,
    }
    public enum CardFeature
    {
        Largest,
        Lowest
    }
    public enum CardRank
    {
        Leader,
        Gold,
        Silver,
        Copper,
    }

    public enum CardBoardMode
    {
        None,//Ĭ��״̬
        Select,//���ѡ��ģʽ
        ExchangeCard,//���γ鿨ģʽ
        ShowOnly//�޷�����ģʽ
    }
    public enum CardTag
    {
        Machine,
        Fairy,
        Object,
        SpellCard
    }
    public enum Orientation
    {
        My,
        Op,
        All,
        Up,
        Down,
    }
    public enum Territory { My, Op, All }
    public enum Language
    {
        Ch,
        Tc,
        En,
        geyu
    }
    public enum CardPointType
    {
        green,
        red,
        white
    }
    public enum NetAcyncType
    {
        Init,
        FocusCard,
        PlayCard,
        SelectRegion,
        SelectUnites,
        SelectLocation,
        SelectProperty,
        SelectBoardCard,
        ExchangeCard,
        RoundStartExchangeOver,
        Pass,
        Surrender
    }
}