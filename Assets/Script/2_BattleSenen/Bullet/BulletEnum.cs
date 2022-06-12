
using UnityEngine;
namespace TouhouMachineLearningSummary.GameEnum
{
    [SerializeField]
    public enum BulletType
    {
        //DreamOfSeal,
        //Hurt,
        BigBall,
        SmallBall,
        MiBall,
        Butterfly,
        Bomb,//从天而降的轰炸
        Heal,
    }
    public enum BulletTrack
    {
        Round,//环绕
        Line,//直射
        Fixed,//卡牌上发
        Down,//从天而降
        Test,
    }
    public enum BulletColor
    {
        Default,
        Red,
        Blue,
        Green,
        White,
        Black,
    }
    public enum BulletBrust
    {
        None,
        Smoke
    }
}