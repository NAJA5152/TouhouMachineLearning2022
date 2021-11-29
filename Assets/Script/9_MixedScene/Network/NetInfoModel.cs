using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;


namespace TouhouMachineLearningSummary.Model
{
    public class SampleCardModel
    {
        public int CardID { get; set; } = 0;
        public int BasePoint { get; set; } = 0;
        public int ChangePoint { get; set; } = 0;
        public List<int> State { get; set; } = new List<int>();
        public SampleCardModel() { }
        public SampleCardModel(Card card)
        {
            CardID = card.cardID;
            BasePoint = card.basePoint;
            ChangePoint = card.changePoint;
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
        public string Date { get; set; }
        public byte[] AssemblyFileData { get; set; }
        public byte[] SingleCardFileData { get; set; }
        public byte[] MultiCardFileData { get; set; }

        public CardConfig() { }

        public CardConfig(string date, FileInfo assemblyFile, FileInfo singleCardFile, FileInfo multiCardFile)
        {
            _id = Guid.NewGuid().ToString();
            this.UpdataTime = DateTime.Now;
            this.Date = date;
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
        public string UUID { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Password { get; set; }
        public int Level { get; set; }
        public int Rank { get; set; }
        public UserState OnlineUserState { get; set; } = new UserState();

        public Dictionary<string, int> Resource { get; set; } = new Dictionary<string, int>();
        //决定游戏进程
        [ShowInInspector]
        public Dictionary<string, int> CardLibrary { get; set; } = new Dictionary<string, int>();
        public int UseDeckNum { get; set; } = 0;
        public List<CardDeck> Decks { get; set; }
        public CardDeck UseDeck
        {
            get => Decks[UseDeckNum];
            set => Decks[UseDeckNum] = value;
        }
        public PlayerInfo() { }
        public PlayerInfo(string name, string title, string password, List<CardDeck> decks)
        {
            this.Name = name;
            Title = title;
            Decks = decks;
            Password = password;
            Level = 0;
            Rank = 0;
            UseDeckNum = 0;
            Resource = new Dictionary<string, int>();
            Resource.Add("faith", 0);
            Resource.Add("recharge", 0);
        }
        public PlayerInfo GetSampleInfo()
        {
            PlayerInfo sampleInfo = new PlayerInfo(Name, Title, "", Decks);
            sampleInfo.Level = Level;
            sampleInfo.Rank = Rank;
            sampleInfo.UseDeckNum = UseDeckNum;
            return sampleInfo;
        }
    }
    public class UserState
    {
        int step;
        int rank;
        public async Task<bool> UpdateAsync() => await Command.Network.NetCommand.UpdateUserState(Info.AgainstInfo.onlineUserInfo);
    }
    /// <summary>
    /// 客户端服务器通讯通用指令模板
    /// </summary>
    [Serializable]
    public class GeneralCommand
    {
        public object[] Datas;
        public GeneralCommand()
        {
        }
        public GeneralCommand(params object[] Datas)
        {
            this.Datas = Datas;
        }
    }
    /// <summary>
    /// 客户端服务器通讯通用指令模板
    /// </summary>
    [Serializable]
    public class GeneralCommand<T>
    {
        public T[] Datas;
        public GeneralCommand()
        {
        }
        public GeneralCommand(params T[] Datas)
        {
            this.Datas = Datas;
        }
    }
}

