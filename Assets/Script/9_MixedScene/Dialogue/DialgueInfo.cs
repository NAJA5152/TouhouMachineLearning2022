using System;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Info.Dialogue
{

    public class DialgueInfo : MonoBehaviour
    {
        public static DialgueInfo instance;
        void Awake() => instance = this;
        public GameObject Left;
        public GameObject Right;
        public Text Text;
        public bool IsNext;
        public enum Chara
        {
            灵梦A,
            灵梦B,
            早苗
        }
        public class Dial : Attribute
        {
            public int step;
            public int rank;
            public Dial(int step, int rank)
            {
                this.step = step;
                this.rank = rank;
            }
        }
    }
}
