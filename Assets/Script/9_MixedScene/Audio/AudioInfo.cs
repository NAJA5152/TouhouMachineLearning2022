using Sirenix.OdinInspector;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
namespace TouhouMachineLearningSummary.Info
{
    public class AudioInfo : MonoBehaviour
    {
        //public AudioSource Source;
       // public Dictionary<GameEnum.AudioType, AudioClip>  clips;
        public static AudioInfo Instance;
        //public static AudioSource audioSource => Instance.Source;
        void Awake()
        {
            Instance = this;
            Command.AudioCommand.Init();//初始化音效系统
        }

        public static Dictionary<GameAudioType, AudioClip> StaticClips=new Dictionary<GameAudioType, AudioClip>();
        [Button()]
        public void Play(GameAudioType i)
        {
            Command.AudioCommand.PlayAsync(i);
        }
    }
}
