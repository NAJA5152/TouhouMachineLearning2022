using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;

namespace TouhouMachineLearningSummary.Model
{
    public class SampleCardModel
    {
        public int CardID { get; set; } = 0;
        public int BasePoint { get; set; } = 0;
        public int ChangePoint { get; set; } = 0;
        public Dictionary<string, int> CardFields { get; set; }
        public List<int> State { get; set; } = new List<int>();
        public SampleCardModel() { }
        public SampleCardModel(Card card)
        {
            CardID = card.CardID;
            BasePoint = card.BasePoint;
            ChangePoint = card.ChangePoint;
            CardFields = card.cardFields.ToDictionary(field => field.Key.ToString(), field => field.Value);
            State = Enumerable.Range(0, Enum.GetNames(typeof(GameEnum.CardState)).Length).SelectList(index => card[(GameEnum.CardState)index] ? 1 : 0);
        }
    }
    [Serializable]

    /// <summary>
    /// 卡牌坐标模板
    /// </summary>
    public class Location
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Location(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
    //卡牌配置模板
    public class CardConfig
    {
        public string _id { get; set; }
        public DateTime UpdataTime { get; set; }
        public string Version { get; set; }
        public byte[] AssemblyFileData { get; set; }
        public byte[] SingleCardFileData { get; set; }
        public byte[] MultiCardFileData { get; set; }

        public CardConfig() { }

        public CardConfig(string version, FileInfo assemblyFile, FileInfo singleCardFile, FileInfo multiCardFile)
        {
            _id = Guid.NewGuid().ToString();
            this.UpdataTime = DateTime.Now;
            this.Version = version;
            this.AssemblyFileData = File.ReadAllBytes(assemblyFile.FullName);
            this.SingleCardFileData = File.ReadAllBytes(singleCardFile.FullName);
            this.MultiCardFileData = File.ReadAllBytes(multiCardFile.FullName);
        }
    }
    /// <summary>
    /// 玩家信息模板
    /// </summary>
    public class PlayerInfo
    {
        public string _id { get; set; }
        public string UID { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Password { get; set; }
        public int Level { get; set; }
        public int Rank { get; set; }
        public DateTime LastLoginTime { get; set; }

        //public class UserState
        //{
        //    public int Step { get; set; }
        //    public int Rank { get; set; }
        //}
        //public UserState OnlineUserState { get; set; } = new UserState();
        public Dictionary<string, int> Stage { get; set; }=new();
        public int GetStage(string StageName) => !Stage.ContainsKey(StageName)?-1: Stage[StageName];
        public Dictionary<string, int> Resource { get; set; } = new Dictionary<string, int>();
        //决定游戏进程
        [ShowInInspector]
        public Dictionary<string, int> CardLibrary { get; set; } = new Dictionary<string, int>();
        public int UseDeckNum { get; set; } = 0;
        public List<CardDeck> Decks { get; set; }
        [JsonIgnore]
        public CardDeck UseDeck
        {
            get => Decks[UseDeckNum];
            set => Decks[UseDeckNum] = value;
        }
        public PlayerInfo() { }
        public PlayerInfo(string account, string name, string title, string password, List<CardDeck> decks)
        {
            Account = account;
            Name = name;
            Title = title;
            Decks = decks;
            Password = password;
            Level = 0;
            Rank = 0;
            UseDeckNum = 0;
        }
        /// <summary>
        /// 返回脱敏后的简易用户信息
        /// </summary>
        /// <returns></returns>
        public PlayerInfo GetSampleInfo()
        {
            PlayerInfo sampleInfo = new PlayerInfo(Account, Name, Title, "", Decks);
            sampleInfo.Level = Level;
            sampleInfo.Rank = Rank;
            sampleInfo.UseDeckNum = UseDeckNum;
            return sampleInfo;
        }
        public async Task<bool> UpdateName(string name)
        {
            Info.AgainstInfo.onlineUserInfo.Name = name;
            Manager.UserInfoManager.Refresh();
            return await Command.NetCommand.UpdateInfoAsync(UpdateType.Name, name);
        }

        public async Task<bool> UpdateUserStateAsync(int step, int rank)
        {
            OnlineUserState.Step = step;
            OnlineUserState.Rank = rank;
            return await Command.NetCommand.UpdateInfoAsync(UpdateType.Stage, OnlineUserState);
        }
        public async Task<bool> UpdateDecksAsync()
        {
            bool isSuccessUpdateDeck = await Command.NetCommand.UpdateInfoAsync(UpdateType.Decks, Decks);
            bool isSuccessUpdateUseDeckNum = await Command.NetCommand.UpdateInfoAsync(UpdateType.UseDeckNum, UseDeckNum);
            return isSuccessUpdateDeck && isSuccessUpdateUseDeckNum;
        }
    }
}