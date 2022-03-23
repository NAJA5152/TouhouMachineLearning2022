using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.GameEnum;
using System.Threading.Tasks;

public class AudioTest : MonoBehaviour
{
    [Button]
    public async void TestAudio(GameAudioType type)
    {
        var audioCLip =AudioInfo.StaticClips[type];
        AudioSource Source = AudioInfo.audioScoure.GetComponent<AudioSource>();
        Source.clip = audioCLip;
        Source.Play();
        await Task.Delay((int)(audioCLip.length * 1000));
    }
}
