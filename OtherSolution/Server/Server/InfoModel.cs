using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Server
{
    public class UserState
    {
        public int Step { get; set; }
        public int Rank { get; set; }
    }
    public class PlayerInfo
    {
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string UID { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Password { get; set; }
        public int Level { get; set; }
        public int Rank { get; set; }
        public float WinRate { get; set; }
        public Dictionary<string, int> Resource { get; set; }
        //决定游戏进程
        public UserState OnlineUserState { get; set; } = new UserState();
        public Dictionary<string, int> CardLibrary { get; set; }
        public int UseDeckNum { get; set; }
        public List<CardDeck> Decks { get; set; }
        public CardDeck UseDeck => Decks[UseDeckNum];
        public PlayerInfo ShufflePlayerDeck()
        {
            Decks[UseDeckNum].CardIds = UseDeck.CardIds.OrderBy(i => new Random(DateTime.Now.GetHashCode()).Next()).ToList();
            return this;
        }
        public PlayerInfo() { }
        public PlayerInfo Creat(string account, string password, string title, List<CardDeck> deck, Dictionary<string, int> cardLibrary)
        {
            _id = Guid.NewGuid().ToString();
            UID = (MongoDbCommand.GetRegisterPlayerCount() + 1000).ToString();
            Account = account;
            Name = "村中人";
            Title = title;
            Decks = deck;
            Password = password.GetSaltHash(UID);
            CardLibrary = cardLibrary;
            Level = 0;
            Rank = 0;
            UseDeckNum = 0;
            Resource = new Dictionary<string, int>();
            OnlineUserState = new UserState();
            Resource.Add("faith", 0);
            Resource.Add("recharge", 0);
            return this;
        }
    }
    //卡牌不同版本的配置文件类型
    public class CardConfig
    {
        [BsonId]
        public string _id { get; set; }
        public DateTime UpdataTime { get; set; }
        public string Date { get; set; }
        public byte[] AssemblyFileData { get; set; }
        public byte[] SingleCardFileData { get; set; }
        public byte[] MultiCardFileData { get; set; }
        public CardConfig() { }
    }
    public class CardDeck
    {
        public string DeckName { get; set; }
        public int LeaderId { get; set; }
        public List<int> CardIds { get; set; }
        public CardDeck() { }
        public CardDeck(string DeckName, int LeaderId, List<int> CardIds)
        {
            this.DeckName = DeckName;
            this.LeaderId = LeaderId;
            this.CardIds = CardIds;
        }
    }
    public class SampleCardModel
    {
        public int CardID { get; set; } = 0;
        public int BasePoint { get; set; } = 0;
        public int ChangePoint { get; set; } = 0;
        public List<int> State { get; set; } = new List<int>();
        public SampleCardModel() { }
    }
    public class AgainstSummary
    {
        [BsonId]
        public string _id { get; set; }
        public string AssemblyVerision { get; set; } = "";
        public string Player1Name { get; set; } = "";
        public string Player2Name { get; set; } = "";
        public string Winner { get; set; } = "";
        public List<TurnOperation> TurnOperations { get; set; } = new List<TurnOperation>();
        //简易的数字卡牌量化模型

        public class TurnOperation
        {
            public int RoundRank { get; set; }//当前小局数
            public int TurnRank { get; set; }//当前回合数
            public int TotalTurnRank { get; set; }//当前总回合数
            public bool IsOnTheOffensive { get; set; }//是否先手
            public int AbsoluteStartPoint { get; set; }//玩家操作前双方的点数差
            public int AbsoluteEndPoint { get; set; }//玩家操作后双方的点数差  
            //0表示不投降，1表示玩家1投降，2表示玩家2投降
            public int isSurrender { get; set; } = 0;
            public List<List<SampleCardModel>> AllCardList { get; set; } = new List<List<SampleCardModel>>();
            public PlayerOperation PlayerTurnOperation { get; set; }
            public List<SelectOperation> SelectTurnOperations { get; set; } = new List<SelectOperation>();
            public TurnOperation() { }
            public class PlayerOperation
            {
                public List<int> Operation { get; set; }
                public List<SampleCardModel> TargetcardList { get; set; }
                public int SelectCardID { get; set; }
                public PlayerOperation() { }
            }
            public class SelectOperation
            {
                //操作类型 选择场地属性/从战场选择多个单位/从卡牌面板中选择多张牌/从战场中选择一个位置/从战场选择多片对战区域
                public List<int> Operation { get; set; }
                public int TriggerCardID { get; set; }

                //选择单位
                public List<SampleCardModel> TargetCardList { get; set; }
                public List<int> SelectCardRank { get; set; }
                public int SelectMaxNum { get; set; }
                //区域
                public int SelectRegionRank { get; set; }
                public int SelectLocation { get; set; }
                //换牌完成,true为玩家1换牌操作，false为玩家2换牌操作
                public bool IsPlay1ExchangeOver { get; set; }
                public SelectOperation() { }
            }
        }
        /// <summary>
        /// 增加一个回合记录
        /// </summary>
        /// <param name="turnOperation"></param>
        public void AddTurnOperation(TurnOperation turnOperation) => TurnOperations.Add(turnOperation);
        /// <summary>
        /// 增加一个回合玩家操作记录
        /// </summary>
        /// <param name="turnOperation"></param>
        public void AddPlayerOperation(PlayerOperationType operation, TurnOperation.PlayerOperation playerOperation) => TurnOperations.Last().PlayerTurnOperation = playerOperation;
        /// <summary>
        /// 增加一个回合玩家选择记录
        /// </summary>
        /// <param name="turnOperation"></param>
        public void AddTurnSelectOperation(TurnOperation.SelectOperation selectOperation) => TurnOperations.Last().SelectTurnOperations.Add(selectOperation);

        public void UploadAgentSummary() => MongoDbCommand.InsertAgainstSummary(this);
    }
}