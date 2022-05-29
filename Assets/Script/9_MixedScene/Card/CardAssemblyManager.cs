using Sirenix.OdinInspector;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.Manager
{

    class CardAssemblyManager : MonoBehaviour
    {
        //是否使用本地卡牌版本，若为是则会卡顿
        static bool isUseLocalAssembly = true;
        /// <summary>
        /// 已下载的历史配置文件
        /// </summary>
        static Dictionary<string, CardConfig> cardConfigs = new Dictionary<string, CardConfig>();
        /// <summary>
        /// 当前使用的配置文件
        /// </summary>
        static CardConfig currentConfig;
        //当前使用的卡牌代码
        static Assembly currentAssembly;
        //当前使用的卡牌信息
        static List<CardModel> currenttSingleCardInfos;
        static List<CardModel> currentMultiCardInfos;
        //获取当前引用卡牌数据的日期
        public static string GetCurrentConfigDate => currentConfig.Version;
        [ShowInInspector]
        public static List<CardModel> GetcurrentSingleCardInfos => currenttSingleCardInfos;
        [ShowInInspector]
        public static List<CardModel> GetcurrentMultiCardInfos => currentMultiCardInfos;
        [ShowInInspector]
        public static List<CardModel> GetLastSingleCardInfos => lastSingleCardInfos;
        [ShowInInspector]
        public static List<CardModel> GetLastMultiCardInfos => lastMultiCardInfos;
        /// <summary>
        /// 最新版本的配置文件
        /// </summary>
        //[ShowInInspector]
        //static CardConfig lastConfig;
        //最新版本的卡牌代码
        [ShowInInspector]
        static Assembly lastAssembly;
        //最新版本的卡牌信息
        [ShowInInspector]
        public static List<CardModel> lastSingleCardInfos;
        [ShowInInspector]
        public static List<CardModel> lastMultiCardInfos;
        //设置
        public static async Task SetCurrentAssembly(string date)
        {
            //识别日期编号
            //假如是空的，则先查询是否存在该配置文件
            //假如存在则加载
            //假如不存在下载最后一个配置文件
            //不然下载指定配置文件

            //如果是请求最新的卡牌配置，则直接更新，确保每次对局都是最新版数据
            //步骤
            //查询最新的版本编号
            //检查是否已存在
            //如果已存在啧直接使用
            //如果不存在则从服务器拉取最新版本
            var data = await GetCardConfigsVersionAsync();
            //为""则获取最新版本的卡牌数据
            if (date == "")
            {
                var targetConfig = cardConfigs.Values.ToList().FirstOrDefault(config => config.UpdataTime.ToString() == data);
                if (targetConfig != null)
                {
                    currentConfig = targetConfig;
                    Debug.Log($"当前已是最新版本{data}，无需更新");
                }
                else
                {
                    await LoadOrDownloadConfig(date);
                }
                lastAssembly = Assembly.Load(currentConfig.AssemblyFileData);
                //加载卡牌信息与卡牌图片
                lastSingleCardInfos = Encoding.UTF8.GetString(currentConfig.SingleCardFileData).ToObject<List<CardModel>>().SelectList(card => card.Init(isSingle: true));
                lastMultiCardInfos = Encoding.UTF8.GetString(currentConfig.MultiCardFileData).ToObject<List<CardModel>>().SelectList(card => card.Init(isSingle: false));
            }
            //否则获取指定日期的卡牌数据
            else
            {
                if (cardConfigs.Keys.Contains(date))
                {
                    currentConfig = cardConfigs[date];
                }
                else
                {
                    await LoadOrDownloadConfig(date);
                }
            }
            currentAssembly = Assembly.Load(currentConfig.AssemblyFileData);
            //加载卡牌信息与卡牌图片
            currenttSingleCardInfos = Encoding.UTF8.GetString(currentConfig.SingleCardFileData).ToObject<List<CardModel>>().SelectList(card => card.Init(true));
            currentMultiCardInfos = Encoding.UTF8.GetString(currentConfig.MultiCardFileData).ToObject<List<CardModel>>().SelectList(card => card.Init(false));
            Debug.Log("下载完成");
            //下载指定版本数据
            static async Task LoadOrDownloadConfig(string date)
            {
                currentConfig = await Command.NetCommand.DownloadCardConfigsAsync(date);
                cardConfigs[currentConfig.Version] = currentConfig;
            }
        }
        public static async Task<string> GetCardConfigsVersionAsync() => await Command.NetCommand.GetCardConfigsVersionAsync();
        /// <summary>
        /// 从加载的dll获得指定id的卡牌脚本
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Type GetCardScript(int id) => currentAssembly.GetType("TouhouMachineLearningSummary.CardSpace.Card" + id);
        /// <summary>
        /// 获取当前加载版本的卡牌信息，用于在对局内回放指定版本的牌库，卡组信息
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        public static CardModel GetCurrentCardInfos(int cardID)
        {
            CardModel cardModelInfo = new List<CardModel>()
                .Union(GetcurrentSingleCardInfos)
                .Union(GetcurrentMultiCardInfos)
                .FirstOrDefault(info => info.cardID == cardID);
            if (cardModelInfo == null)
            {
                Debug.LogError("卡牌" + cardID + "查找失败");
            }
            return cardModelInfo;
        }
        /// <summary>
        /// 获取最新版本的卡牌信息，用于在对局外获取最新的牌库，卡组信息
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        public static CardModel GetLastCardInfo(int cardID)
        {
            CardModel cardModelInfo = new List<CardModel>()
                .Union(GetLastSingleCardInfos)
                .Union(GetLastMultiCardInfos)
                .FirstOrDefault(info => info.cardID == cardID);
            if (cardModelInfo == null)
            {
                Debug.LogError("卡牌" + cardID + "查找失败");
            }
            return cardModelInfo;
        }
    }

}
