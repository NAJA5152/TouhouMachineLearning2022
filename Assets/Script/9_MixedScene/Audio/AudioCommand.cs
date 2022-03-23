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
            Info.AudioInfo.audioScoure = GameObject.FindGameObjectWithTag("Audio");
            for (int i = 0; i < Enum.GetValues(typeof(GameAudioType)).Length; i++)
            {
                Info.AudioInfo.StaticClips[(GameAudioType)i] = Resources.Load<AudioClip>("Sound/" + ((GameAudioType)i).ToString());
            }
        }
        public static async Task PlayAsync(GameAudioType type)
        {
            var audioCLip = Info.AudioInfo.StaticClips[type];
            AudioSource Source = Info.AudioInfo.audioScoure.AddComponent<AudioSource>();
            Source.clip = audioCLip;
            Source.spatialBlend = 1;
            Source.pitch = 1.3f;
            Source.Play();
            await Task.Delay((int)(audioCLip.length * 1000));
            GameObject.DestroyImmediate(Source);
        }
    }
}