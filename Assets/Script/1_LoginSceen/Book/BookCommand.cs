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
            await Manager.CameraViewManager.MoveToViewAsync(1);
        }
        public static async Task InitToOpenStateAsync()
        {
            Info.GameUI.UiInfo.loginCanvas.SetActive(false);
            await SetCoverStateAsync(true);
            ActiveCompment();
            //Command.MenuStateCommand.ChangeToMainPage(MenuState.);
        }
        [Button]
        public static async Task SetCoverStateAsync(bool isBookOpen, bool isImmediately = false) =>
          await CustomThread.TimerAsync(isImmediately ? 0 : 0.3f, runAction: (process) =>
                {
                    Info.BookInfo.instance.coverModel.transform.eulerAngles = Vector3.zero;
                    float length = (Info.BookInfo.instance.coverModel.transform.position - Info.BookInfo.instance.axisModel.transform.position).magnitude;
                    float angle = isBookOpen ? Mathf.Lerp(0, 180, process) : Mathf.Lerp(180, 0, process);
                    Info.BookInfo.instance.coverModel.transform.localPosition = new Vector3(0, 0.08f, 0) + new Vector3(length * Mathf.Cos(Mathf.PI / 180 * angle), length * Mathf.Sin(Mathf.PI / 180 * angle));
                    Info.BookInfo.instance.coverModel.transform.eulerAngles = new Vector3(0, 0, angle);
                });
        public static void ActiveCompment(params BookCompmentType[] types)
        {
            Info.BookInfo.instance.singleModeSelectComponent.SetActive(false);
            Info.BookInfo.instance.multiplayerModeSelectComponent.SetActive(false);
            Info.BookInfo.instance.practiceComponent.SetActive(false);
            Info.BookInfo.instance.cardDetailComponent.SetActive(false);
            Info.BookInfo.instance.cardListComponent.SetActive(false);
            Info.BookInfo.instance.mapComponent.SetActive(false);
            Info.BookInfo.instance.cardDeckListComponent.SetActive(false);
            Info.BookInfo.instance.cardLibraryComponent.SetActive(false);
            Info.BookInfo.instance.campSelectComponent.SetActive(false);
            Info.BookInfo.instance.campSelectComponent.SetActive(false);
            Info.BookInfo.instance.scenePageComponent.SetActive(false);
            types.ToList().ForEach(type =>
            {
                GameObject targetUiComoinent;
                switch (type)
                {
                    case BookCompmentType.Single: targetUiComoinent = Info.BookInfo.instance.singleModeSelectComponent; break;
                    case BookCompmentType.Multiplayer: targetUiComoinent = Info.BookInfo.instance.multiplayerModeSelectComponent; break;
                    case BookCompmentType.Practice: targetUiComoinent = Info.BookInfo.instance.practiceComponent; break;
                    case BookCompmentType.CardDetial: targetUiComoinent = Info.BookInfo.instance.cardDetailComponent; break;
                    case BookCompmentType.CardList: targetUiComoinent = Info.BookInfo.instance.cardListComponent; break;
                    case BookCompmentType.DeckList: targetUiComoinent = Info.BookInfo.instance.cardDeckListComponent; break;
                    case BookCompmentType.CardLibrary: targetUiComoinent = Info.BookInfo.instance.cardLibraryComponent; break;
                    case BookCompmentType.Map: targetUiComoinent = Info.BookInfo.instance.mapComponent; break;
                    case BookCompmentType.CampSelect: targetUiComoinent = Info.BookInfo.instance.campSelectComponent; break;
                    case BookCompmentType.ScenePage: targetUiComoinent = Info.BookInfo.instance.scenePageComponent; break;
                    default: targetUiComoinent = null; break;
                }
                if (targetUiComoinent != null)
                {
                    targetUiComoinent.GetComponent<CanvasGroup>().alpha = 0;
                    Vector3 point = targetUiComoinent.transform.position;
                    targetUiComoinent.SetActive(true);
                    _ = CustomThread.TimerAsync(0.2f, runAction: (process) => //在0.4秒内不断移动并降低透明度
                    {
                        targetUiComoinent.GetComponent<CanvasGroup>().alpha = process;
                        targetUiComoinent.transform.position = point + new Vector3(-1, 0, 1) * (1 - process) * 0.05f;
                    });
                }
            });
        }

        public static async void SimulateFilpPage(bool IsSimulateFilpPage, bool isRightToLeft=true)
        {
            Info.BookInfo.IsSimulateFilpPage = IsSimulateFilpPage;
            if (IsSimulateFilpPage)
            {
                while (Info.BookInfo.IsSimulateFilpPage)
                {
                    await Task.Delay(2000);
                    Manager.TaskLoopManager.Throw();

                    GameObject voidPageModel = Info.BookInfo.instance.voidPageModel;
                    GameObject fakePageModel = Info.BookInfo.instance.fakePageModel;
                    GameObject startPage = Instantiate(voidPageModel, voidPageModel.transform.position, voidPageModel.transform.rotation, voidPageModel.transform.parent);
                    GameObject endPage = Instantiate(voidPageModel, voidPageModel.transform.position, voidPageModel.transform.rotation, voidPageModel.transform.parent);
                    GameObject fakePage1 = Instantiate(fakePageModel, fakePageModel.transform.position, fakePageModel.transform.rotation, fakePageModel.transform.parent);
                    GameObject fakePage2 = Instantiate(fakePageModel, fakePageModel.transform.position, fakePageModel.transform.rotation, fakePageModel.transform.parent);
                    _ = FileSinglePage(isRightToLeft, startPage);
                    await Task.Delay(200);
                    _ = FileSinglePage(isRightToLeft, fakePage1);
                    await Task.Delay(200);
                    _ = FileSinglePage(isRightToLeft, fakePage2);
                    await Task.Delay(200);
                    await FileSinglePage(isRightToLeft, endPage);
                }
            }

            static async Task FileSinglePage(bool isRightToLeft, GameObject page)
            {
                page.SetActive(true);
                await CustomThread.TimerAsync(0.5f, runAction: (process) =>
                {
                    page.transform.eulerAngles = Vector3.zero;
                    float length = (page.transform.position - Info.BookInfo.instance.axisModel.transform.position).magnitude;
                    float angle = isRightToLeft ? Mathf.Lerp(0, 180, process) : Mathf.Lerp(180, 0, process);
                    page.transform.localPosition = new Vector3(0, 0.08f, 0) + new Vector3(length * Mathf.Cos(Mathf.PI / 180 * angle), length * Mathf.Sin(Mathf.PI / 180 * angle));
                    page.transform.eulerAngles = new Vector3(0, 0, angle);
                });
                DestroyImmediate(page);
            }
        }

    }
}

