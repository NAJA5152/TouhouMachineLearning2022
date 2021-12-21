using System;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
namespace TouhouMachineLearningSummary.Command
{
    public class AudioCommand
    {
        public static void Init()
        {
            for (int i = 0; i < Enum.GetValues(typeof(GameAudioType)).Length; i++)
            {
                Info.AudioInfo.StaticClips[(GameAudioType)i] = Resources.Load<AudioClip>("Sound/" + ((GameEnum.GameAudioType)i).ToString());
            }
        }
        public static async Task PlayAsync(GameAudioType type)
        {
            var audioCLip = Info.AudioInfo.StaticClips[type];
            AudioSource Source = Info.AudioInfo.Instance.gameObject.AddComponent<AudioSource>();
            Source.clip = audioCLip;
            Source.Play();
            await Task.Delay((int)(audioCLip.length * 1000));
            GameObject.DestroyImmediate(Source);
        }
    }
}
