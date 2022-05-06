using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
namespace TouhouMachineLearningSummary.Info
{
    public class SoundEffectInfo : MonoBehaviour
    {
        public static GameObject audioScoure;
        public static Dictionary<AgainstSoundEffectType, AudioClip> AgainstSoundEfects { get; set; } = new Dictionary<AgainstSoundEffectType, AudioClip>();
        public static Dictionary<UISoundEffectType, AudioClip> UISoundEfects { get; set; } = new Dictionary<UISoundEffectType, AudioClip>();
        void Awake() => Command.SoundEffectCommand.Init();//初始化音效系统
    }
}
