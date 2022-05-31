using System;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
namespace TouhouMachineLearningSummary.Command
{
    public class SoundEffectCommand
    {
        public static void Init()
        {
            Info.SoundEffectInfo.audioScoure = GameObject.FindGameObjectWithTag("Audio");
            for (int i = 0; i < Enum.GetValues(typeof(SoundEffectType)).Length; i++)
            {
                //音效，场景1
                //Info.SoundEffectInfo.UISoundEfects[(UISoundEffectType)i] = Resources.Load<AudioClip>("Sound/" + ((UISoundEffectType)i).ToString());
                Info.SoundEffectInfo.SoundEfects[(SoundEffectType)i] = AssetBundleCommand.Load<AudioClip>("SoundEfect" , ((SoundEffectType)i).ToString());
            }
        }
        public static async Task PlayAsync(SoundEffectType type)
        {
            var audioClip = Info.SoundEffectInfo.SoundEfects[type];
            AudioSource Source = Info.SoundEffectInfo.audioScoure.AddComponent<AudioSource>();
            Source.clip = audioClip;
            Source.spatialBlend = 1;
            Source.pitch = 1.3f;
            Source.Play();
            await Task.Delay((int)(audioClip.length * 1000));
            GameObject.DestroyImmediate(Source);
        }
    }
}