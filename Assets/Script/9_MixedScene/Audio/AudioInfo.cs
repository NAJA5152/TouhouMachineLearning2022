using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
namespace TouhouMachineLearningSummary.Info
{
    public class AudioInfo : MonoBehaviour
    {
        public static Dictionary<GameAudioType, AudioClip> StaticClips = new Dictionary<GameAudioType, AudioClip>();
        void Awake() => Command.AudioCommand.Init();//初始化音效系统
    }
}
