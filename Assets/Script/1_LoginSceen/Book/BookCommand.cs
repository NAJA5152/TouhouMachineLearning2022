using Sirenix.OdinInspector;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
namespace TouhouMachineLearningSummary.Command
{
    public partial class BookCommand : MonoBehaviour
    {
        public static async Task InitAsync()
        {
            Info.GameUI.UiInfo.loginCanvas.SetActive(false);
            await SetCoverStateAsync(true);
            ActiveCompment();
            await Task.Delay(1000);
            Command.MenuStateCommand.ChangeToMainPage(MenuState.Single);
            await Manager.CameraViewManager.MoveToBookViewAsync();
        }
        public static async Task InitToOpenStateAsync()
        {
            Info.GameUI.UiInfo.loginCanvas.SetActive(false);
            await SetCoverStateAsync(true);
            ActiveCompment();
            //Command.MenuStateCommand.ChangeToMainPage(MenuState.);
        }
        [Button]
        public static async Task SetCoverStateAsync(bool isBookOpen,bool isImmediately=false) =>
          await CustomThread.TimerAsync(isImmediately?0:2, runAction: (time) =>
            {
                Info.BookInfo.coverModel.transform.eulerAngles = Vector3.zero;
                float length = (Info.BookInfo.coverModel.transform.position - Info.BookInfo.axisModel.transform.position).magnitude;
                float angle = isBookOpen ? Mathf.Lerp(0, 180, time) : Mathf.Lerp(180, 0, time);
                Info.BookInfo.coverModel.transform.localPosition = new Vector3(0, 0.08f, 0) + new Vector3(length * Mathf.Cos(Mathf.PI / 180 * angle), length * Mathf.Sin(Mathf.PI / 180 * angle));
                Info.BookInfo.coverModel.transform.eulerAngles = new Vector3(0, 0, angle);
            });
        public static void ActiveCompment(params BookCompmentType[] types)
        {
            Info.BookInfo.singleModeSelectComponent.SetActive(false);
            Info.BookInfo.multiplayerModeSelectComponent.SetActive(false);
            Info.BookInfo.practiceComponent.SetActive(false);
            Info.BookInfo.cardDetailComponent.SetActive(false);
            Info.BookInfo.cardListComponent.SetActive(false);
            Info.BookInfo.mapComponent.SetActive(false);
            Info.BookInfo.cardDeckListComponent.SetActive(false);
            Info.BookInfo.cardLibraryComponent.SetActive(false);
            Info.BookInfo.campSelectComponent.SetActive(false);
            types.ToList().ForEach(type =>
            {
                GameObject targetUiComoinent;
                switch (type)
                {
                    case BookCompmentType.Single: targetUiComoinent = Info.BookInfo.singleModeSelectComponent; break;
                    case BookCompmentType.Multiplayer: targetUiComoinent = Info.BookInfo.multiplayerModeSelectComponent; break;
                    case BookCompmentType.Practice: targetUiComoinent = Info.BookInfo.practiceComponent; break;
                    case BookCompmentType.CardDetial: targetUiComoinent = Info.BookInfo.cardDetailComponent; break;
                    case BookCompmentType.CardList: targetUiComoinent = Info.BookInfo.cardListComponent; break;
                    case BookCompmentType.DeckList: targetUiComoinent = Info.BookInfo.cardDeckListComponent; break;
                    case BookCompmentType.CardLibrary: targetUiComoinent = Info.BookInfo.cardLibraryComponent; break;
                    case BookCompmentType.Map: targetUiComoinent = Info.BookInfo.mapComponent; break;
                    case BookCompmentType.CampSelect: targetUiComoinent = Info.BookInfo.campSelectComponent; break;
                    default: targetUiComoinent = null; break;
                }
                if (targetUiComoinent != null)
                {
                    targetUiComoinent.GetComponent<CanvasGroup>().alpha = 0;
                    Vector3 point = targetUiComoinent.transform.position;
                    targetUiComoinent.SetActive(true);
                    float second = 0.4f;
                    _ = CustomThread.TimerAsync(second, runAction: (time) => //在0.4秒内不断移动并降低透明度
                    {
                        targetUiComoinent.GetComponent<CanvasGroup>().alpha = time / second;
                        targetUiComoinent.transform.position = point + new Vector3(-1, 0, 1) * (1 - time / second) * 0.05f;
                    });
                }
            });
        }

    }
}

