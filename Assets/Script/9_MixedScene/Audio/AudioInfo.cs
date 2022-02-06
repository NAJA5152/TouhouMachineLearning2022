using Sirenix.OdinInspector;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
namespace TouhouMachineLearningSummary.Info
{
    public class AudioInfo : MonoBehaviour
    {
        public static AudioInfo Instance;
        void Awake()
        {
            Instance = this;
            Command.AudioCommand.Init();//初始化音效系统
        }
        public static Dictionary<GameAudioType, AudioClip> StaticClips=new Dictionary<GameAudioType, AudioClip>();
    }
}
