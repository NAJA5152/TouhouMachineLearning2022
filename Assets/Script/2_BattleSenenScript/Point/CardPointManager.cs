using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
using UnityEngine.UI;
namespace TouhouMachineLearningSummary.Manager
{
    //ÕâÉ¶ÍæÒâ£¿£¿Ðü¸¡ÉËº¦Êý×ÖÂð
    public class CardPointManager : MonoBehaviour
    {
        public Canvas pointCanvas;
        static CardPointManager manager;
        private void Awake() => manager = this;
        public static async Task CaretPointAsync(Card card, int point, CardPointType cardPointType)
        {
            var pointCanvas = Instantiate(manager.pointCanvas, card.transform.position, Quaternion.Euler(0, 0, 0));
            pointCanvas.transform.forward = -Camera.main.transform.forward;
            switch (cardPointType)
            {
                case (CardPointType.green):
                    pointCanvas.transform.GetChild(0).GetComponent<Text>().text = "+" + point;
                    pointCanvas.transform.GetChild(1).GetComponent<Text>().text = "+" + point;
                    pointCanvas.transform.GetChild(0).GetComponent<Text>().color = new Color(0, 1, 0, 1);
                    pointCanvas.transform.GetChild(1).GetComponent<Text>().color = new Color(0.1f, 1, 0.1f, 1);
                    break;
                case (CardPointType.red):
                    pointCanvas.transform.GetChild(0).GetComponent<Text>().text = "-" + point;
                    pointCanvas.transform.GetChild(1).GetComponent<Text>().text = "-" + point;
                    pointCanvas.transform.GetChild(0).GetComponent<Text>().color = new Color(1, 0, 0, 1);
                    pointCanvas.transform.GetChild(1).GetComponent<Text>().color = new Color(1, 0.1f, 0.1f, 1);
                    break;
                default:
                    break;
            }
            if (point > 0)
            {


            }
            if (point < 0)
            {

            }
            else
            {

            }

            await CustomThread.TimerAsync(5, process =>
            {
                pointCanvas.transform.position += Vector3.up * 0.02f;
                pointCanvas.transform.GetComponent<CanvasGroup>().alpha = 1 - process;
            });
            Destroy(pointCanvas.gameObject);
        }
    }
}

