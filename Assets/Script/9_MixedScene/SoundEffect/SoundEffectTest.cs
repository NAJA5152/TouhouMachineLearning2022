using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.GameEnum;
using System.Threading.Tasks;

public class SoundEffectTest : MonoBehaviour
{
    [Button]
    public async void TestAudio(SoundEffectType type)
    {
        var audioCLip =SoundEffectInfo.SoundEfects[type];
        AudioSource Source = SoundEffectInfo.audioScoure.GetComponent<AudioSource>();
        Source.clip = audioCLip;
        Source.Play();
        await Task.Delay((int)(audioCLip.length * 1000));
    }
}
