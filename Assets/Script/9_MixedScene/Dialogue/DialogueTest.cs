using System.Threading.Tasks;
using TouhouMachineLearningSummary.Command.Dialogue;
using UnityEngine;
using static TouhouMachineLearningSummary.Info.Dialogue.DialgueInfo;

namespace TouhouMachineLearningSummary.Test.DialgueInfo
{
    public class DialogueTest : MonoBehaviour
    {
        public static DialogueTest Instance;
        Chara lmA = Chara.灵梦A;
        Chara lmB = Chara.灵梦B;
        private void Awake()
        {
            Instance = this;
        }
        [Dial(1, 1)]
        public async Task Dia_1_1()
        {
            await DialogueCommand.Say("你好啊,灵梦", lmB, false);
            await DialogueCommand.Say("你好啊", lmA);
            await DialogueCommand.Say("你有钱吗", lmB, false);
            await DialogueCommand.Say("没有呢。。。", lmA, FaceNum: 1);
            await DialogueCommand.Say("...我也是", lmB, false, FaceNum: 1);
        }
        [Dial(1, 2)]
        public async Task Dia_1_2()
        {
            await DialogueCommand.Say("需要我教你规则吗", lmB, false);
            DialogueCommand.voice(2);
            await DialogueCommand.Say("好啊", lmA);
        }
    }
}