using System;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
namespace TouhouMachineLearningSummary.Test
{
    public class BulletOnGui : MonoBehaviour
    {
        public List<string> toolbarStrings => Init(typeof(BulletType));
        private static List<string> Init(Type type)
        {
            List<string> toolbarStrings = new List<string>();
            var s = Enum.GetValues(type);
            foreach (var item in s)
            {
                toolbarStrings.Add(item.ToString());
            }
            return toolbarStrings;
        }
        int BulletTypeInt;
        int BulletColorInt;
        int BulletTrackInt;
        private void OnGUI()
        {
            BulletTypeInt = GUI.Toolbar(new Rect(25, 25, 1250, 30), BulletTypeInt, Init(typeof(BulletType)).ToArray());
            BulletColorInt = GUI.Toolbar(new Rect(25, 55, 1250, 30), BulletColorInt, Init(typeof(BulletColor)).ToArray());
            BulletTrackInt = GUI.Toolbar(new Rect(25, 85, 1250, 30), BulletTrackInt, Init(typeof(BulletTrack)).ToArray());
            if (GUI.Button(new Rect(25, 150, 250, 30), "´¥·¢Ð§¹û"))
            {
                var tirggerCard = Info.AgainstInfo.cardSet[Orientation.Down][GameRegion.Battle].CardList.FirstOrDefault();
                var targetCards = Info.AgainstInfo.cardSet[Orientation.Up][GameRegion.Battle].CardList.Take(3).ToList();
                _ = GameSystem.PointSystem.Hurt(new TriggerInfoModel(tirggerCard, targetCards)
                    .SetPoint(1)
                    .SetBullet(
                       new Model.BulletModel
                        (
                            (BulletType)BulletTypeInt,
                            (BulletColor)BulletColorInt,
                            (BulletTrack)BulletTrackInt
                        )));

                //switch (bulletType)
                //{
                //    case BulletType.DreamOfSeal:
                //        break;
                //    case BulletType.Heal:
                //        _ = GameSystem.PointSystem.Cure(new TriggerInfo(tirggerCard, targetCards, 1, BulletType.Heal, Color.green));
                //        break;
                //    case BulletType.Hurt:
                //        _ = GameSystem.PointSystem.Hurt(new TriggerInfo(tirggerCard, targetCards, 1, BulletType.Hurt, Color.red));
                //        break;
                //    default:
                //        break;
                //}

            }
            if (GUI.Button(new Rect(25, 180, 250, 30), "ÕÙ»½¿¨Æ¬"))
            {
                var cards1 = Info.AgainstInfo.cardSet[Orientation.Down][GameRegion.Deck].CardList.Take(3);
                _ = GameSystem.TransSystem.SummonCard(new TriggerInfoModel(null, cards1.ToList()));
                var cards2 = Info.AgainstInfo.cardSet[Orientation.Up][GameRegion.Deck].CardList.Take(3);
                _ = GameSystem.TransSystem.SummonCard(new TriggerInfoModel(null, cards2.ToList()));
            }
        }


    }
}

