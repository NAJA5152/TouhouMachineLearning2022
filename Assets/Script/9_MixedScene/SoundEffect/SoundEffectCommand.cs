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
            for (int i = 0; i < Enum.GetValues(typeof(AgainstSoundEffectType)).Length; i++)
            {
                //音效，场景1
                //Info.SoundEffectInfo.UISoundEfects[(UISoundEffectType)i] = Resources.Load<AudioClip>("Sound/" + ((UISoundEffectType)i).ToString());
                Info.SoundEffectInfo.UISoundEfects[(UISoundEffectType)i] = AssetBundleCommand.Load<AudioClip>("UiSoundEfect" , ((UISoundEffectType)i).ToString());
            }
            for (int i = 0; i < Enum.GetValues(typeof(AgainstSoundEffectType)).Length; i++)
            {
                //音效，场景2
                //Info.SoundEffectInfo.AgainstSoundEfects[(AgainstSoundEffectType)i] = Resources.Load<AudioClip>("Sound/" + ((AgainstSoundEffectType)i).ToString());
                Info.SoundEffectInfo.UISoundEfects[(UISoundEffectType)i] = AssetBundleCommand.Load<AudioClip>("AgainstSoundEfect", ((UISoundEffectType)i).ToString());

            }
        }
        public static async Task PlayAsync(UISoundEffectType type)
        {
            var audioClip = Info.SoundEffectInfo.UISoundEfects[type];
            AudioSource Source = Info.SoundEffectInfo.audioScoure.AddComponent<AudioSource>();
            Source.clip = audioClip;
            Source.spatialBlend = 1;
            Source.pitch = 1.3f;
            Source.Play();
            await Task.Delay((int)(audioClip.length * 1000));
            GameObject.DestroyImmediate(Source);
        }
        public static async Task PlayAsync(AgainstSoundEffectType type)
        {
            var audioCLip = Info.SoundEffectInfo.AgainstSoundEfects[type];
            AudioSource Source = Info.SoundEffectInfo.audioScoure.AddComponent<AudioSource>();
            Source.clip = audioCLip;
            Source.spatialBlend = 1;
            Source.pitch = 1.3f;
            Source.Play();
            await Task.Delay((int)(audioCLip.length * 1000));
            GameObject.DestroyImmediate(Source);
        }
    }
}