using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    class BulletCommand
    {
        //配置类型
        //配置颜色
        //附加控制器
        //生成空物体
        //配置danmu
        public static async Task InitBulletAsync(TriggerInfoModel triggerInfo)
        {
            //bool isEnd = false;
            Model.BulletModel danmuInfo = triggerInfo.bulletModel;
            if (danmuInfo != null)
            {
                BulletTrackManager trackManager = null;
                GameObject newBullet = GameObject.Instantiate(Resources.Load<GameObject>("Bullet/" + danmuInfo.bulletType.ToString()));
                if (danmuInfo.color != BulletColor.Default)
                {
                    newBullet.transform.GetChild(0).GetComponent<Renderer>().material.SetColor("_Color", danmuInfo.bulletColor);
                }
                trackManager = newBullet.AddComponent<BulletTrackManager>();
                await trackManager.Play(triggerInfo, danmuInfo.track);
            }
        }
    }
}