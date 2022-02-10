using System.Threading.Tasks;
using TouhouMachineLearningSummary.Thread;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using UnityEngine;
namespace TouhouMachineLearningSummary.Manager
{
    //梦想封印
    partial class BulletTrackManager : MonoBehaviour
    {
        public float maxDinsance = 2;
        public float speed = 1.5f;
        Vector3 startPosition, endPosition;
        public async Task Play(TriggerInfoModel triggerInfo, BulletTrack track)
        {
            this.startPosition = triggerInfo.triggerCard.transform.position;
            this.endPosition = triggerInfo.targetCard.transform.position;
            Vector3 tempPos = Vector3.zero;
            switch (track)
            {
                case BulletTrack.Round://环绕并直行
                    {
                        await CustomThread.TimerAsync(Mathf.PI * 6f / 4, (timer) =>
                        {
                            float x = timer * Mathf.PI * speed;
                            transform.position = startPosition + (Mathf.Min(timer * 0.5f, maxDinsance)) * new Vector3(Mathf.Cos(x / 2), 0, Mathf.Sin(x / 2)) + new Vector3(0, 3, 0);
                            transform.forward = Vector3.Cross(Vector3.up, (new Vector3(Mathf.Cos(x / 2 + 0.5f), 0, Mathf.Sin(x / 2 + 0.5f))));
                            transform.localScale = Mathf.Min(timer * 0.2f, 0.5f) * Vector3.one;
                        });
                        tempPos = transform.position;
                        await CustomThread.TimerAsync(1f, (timer) =>
                        {
                            transform.position = Vector3.Lerp(tempPos, endPosition, timer);
                            transform.forward = endPosition - tempPos;
                        });
                        Destroy(gameObject);
                        _ = CameraManager.manager.VibrationCameraAsync();
                        await Task.Delay(11000 * (int)(Mathf.PI * 6f / 4 + 0.5f));
                    }
                    break;
                case BulletTrack.Line:
                    {
                        await CustomThread.TimerAsync(0.5f, (timer) =>
                         {
                             //float x = timer * Mathf.PI * speed;
                             transform.position = startPosition + Vector3.up * timer;
                             transform.localScale = timer * Vector3.one;
                         });
                        tempPos = transform.position;
                        _ = Command.AudioCommand.PlayAsync(GameEnum.GameAudioType.Laser);
                        await CustomThread.TimerAsync(1f, (timer) =>
                        {
                            transform.position = Vector3.Lerp(tempPos, endPosition, timer);
                        });
                        Destroy(gameObject);
                        _ = CameraManager.manager.VibrationCameraAsync();
                    }
                    break;
                case BulletTrack.Fixed:
                    transform.position = triggerInfo.targetCard.transform.position;
                    Destroy(gameObject, 3);
                    break;
                case BulletTrack.Down:
                    break;
                case BulletTrack.Test:
                    await CustomThread.TimerAsync(0.5f, (timer) =>
                    {
                        transform.position = startPosition + Vector3.up * timer;
                        transform.localScale = timer * Vector3.one;
                    });
                    tempPos = transform.position;
                    _ = Command.AudioCommand.PlayAsync(GameEnum.GameAudioType.Laser);
                    await CustomThread.TimerAsync(1f, (timer) =>
                    {
                        transform.position = Vector3.Lerp(tempPos, endPosition, timer * 2);
                    });
                    Destroy(gameObject);
                    _ = CameraManager.manager.VibrationCameraAsync();
                    break;
                default:
                    break;
            }
        }
    }
}