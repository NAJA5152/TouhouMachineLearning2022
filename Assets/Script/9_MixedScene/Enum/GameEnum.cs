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
        /// <summary>
        /// ����
        /// </summary>
        Generate,
        /// <summary>
        /// ��ȡ
        /// </summary>
        Draw,
        /// <summary>
        /// ���
        /// </summary>
        Play,
        /// <summary>
        /// ����
        /// </summary>
        Deploy,
        /// <summary>
        /// ����
        /// </summary>
        Discard,
        /// <summary>
        /// ����
        /// </summary>
        Dead,
        /// <summary>
        /// ����
        /// </summary>
        Recycle,
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
        /// ��ֵ
        /// </summary>
        Set,
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
        /// <summary>
        /// ����
        /// </summary>
        Reset,
        /// <summary>
        /// �ݻ�
        /// </summary>
        Destory,
        /// <summary>
        /// ǿ��
        /// </summary>
        Strengthen,
        /// <summary>
        /// ����
        /// </summary>
        Weak,
        /// <summary>
        /// ��ת
        /// </summary>
        Reverse,
        /// <summary>
        /// �������ӣ�ֻ�гɹ����������仯ʱ�Żᴥ��
        /// </summary>
        Increase,
        /// <summary>
        /// �������٣�ֻ�гɹ����������仯ʱ�Żᴥ��
        /// </summary>
        Decrease,
        ////////////////////////////////////////////////״̬/////////////////////////////////////////
        StateAdd,
        StateClear,
        ////////////////////////////////////////////////�ֶ�/////////////////////////////////////////
        FieldSet,
        FieldChange,
        ////////////////////////////////////////////////Ʒ��/////////////////////////////////////////
        /// <summary>
        /// �ᴿ
        /// </summary>
        /// /// <summary>
        /// ħ��
        /// </summary>
        ////////////////////////////////////////////////�׶�/////////////////////////////////////////
        RoundStart,
        RoundEnd,
        TurnStart,
        TurnEnd
    }
    /// <summary>
    /// ���Ƹ���״̬����   ����������Ҫ�ı�˳�����µ��ٺ���׷�ӣ�����˺ͷ�������Ҫͬ�����£�������
    /// </summary>
    public enum CardState
    {
        None,//Ĭ�Ͽ�״̬
        Seal,//��ӡ
        Invisibility,//����
        Pry,//��̽
        Close,//���
        Fate,//����
        Lurk,//Ǳ���������
        Secret,//���أ����ƣ�
        Furor,//��
        Docile,//��˳
        Poisoning,//�ж�
        Rely,//ƾ��
        Water,//ˮ
        Fire,//��
        Wind,//��
        Soil,//��
        Hold, //פ��
        Congealbounds,//���
        Forbidden,//����
    }
    /// <summary>
    /// ���Ƹ���ֵ����     ����������Ҫ�ı�˳�����µ��ٺ���׷�ӣ�����˺ͷ�������Ҫͬ�����£�������
    /// </summary>
    public enum CardField 
    {
        None,//Ĭ�Ͽ�״̬
        Timer,//��ʱ
        Inspire,//����
        Apothanasia,//����
        Chain,//����
        Energy,//����
        Shield,//����
    }
    /// <summary>
    /// �������
    /// </summary>
    public enum VariationType
    {
        None,
        Reverse,//��ת
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
        Used,
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
        LargestUnites,
        LowestUnites,
        NotZero,
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
        Default,//Ĭ��״̬
        Select,//���ѡ��ģʽ
        ExchangeCard,//���γ鿨ģʽ
        ShowOnly//�޷�����ģʽ
    }
    public enum CardTag
    {
        SpellCard,
        Variation,
        Machine,
        Fairy,
        Object,
        Tool,
        Yokai,
    }
    public enum Orientation
    {
        /// <summary>
        /// �Ե�ǰ�غϷ���Ϊ���ӽǵ��ҷ�����
        /// </summary>
        My,
        /// <summary>
        /// �Ե�ǰ�غϷ���Ϊ���ӽǵĶԷ�����
        /// </summary>
        Op,
        /// <summary>
        /// ˫������
        /// </summary>
        All,
        /// <summary>
        /// �Կͻ����ӽǷ���Ϊ���ӽǵ��Ϸ�����
        /// </summary>
        Up,
        /// <summary>
        /// �Կͻ����ӽǷ���Ϊ���ӽǵ��·�����
        /// </summary>
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
    //��������Ҫͬ������
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