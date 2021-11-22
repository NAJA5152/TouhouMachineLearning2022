using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Server
{
    public class PlayerInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public int level { get; set; }
        public int rank { get; set; }
        public Dictionary<string, int> resource { get; set; }
        public Dictionary<string, int> cardLibrary { get; set; }
        public int useDeckNum { get; set; }
        public List<CardDeck> decks { get; set; }
        public CardDeck UseDeck => decks[useDeckNum];
        public string ShufflePlayerDeck()
        {
            decks[useDeckNum].CardIds = UseDeck.CardIds.OrderBy(i => new Random(DateTime.Now.GetHashCode()).Next()).ToList();
            return this.ToJson();
        }
        public PlayerInfo(string name, string password, List<CardDeck> deck, Dictionary<string, int> cardLibrary)
        {
            this.name = name;
            this.decks = deck;
            this.password = password;
            this.cardLibrary = cardLibrary;
            level = 0;
            rank = 0;
            useDeckNum = 0;
            resource = new Dictionary<string, int>();
            resource.Add("faith", 0);
            resource.Add("recharge", 0);
        }
    }
    //卡牌不同版本的配置文件类型
    public class CardConfig
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public DateTime updataTime;
        public string date;
        public byte[] assemblyFileData;
        public byte[] singleCardFileData;
        public byte[] multiCardFileData;
        public CardConfig() { }
        public CardConfig(string date, FileInfo assemblyFile, FileInfo singleCardFile, FileInfo multiCardFile)
        {
            this.date = date;
            this.assemblyFileData = File.ReadAllBytes(assemblyFile.FullName);
            this.singleCardFileData = File.ReadAllBytes(singleCardFile.FullName);
            this.multiCardFileData = File.ReadAllBytes(multiCardFile.FullName);
        }
    }
    public class CardDeck
    {
        public string DeckName { get; set; }
        public int LeaderId { get; set; }
        public List<int> CardIds { get; set; }
        public CardDeck(string DeckName, int LeaderId, List<int> CardIds)
        {
            this.DeckName = DeckName;
            this.LeaderId = LeaderId;
            this.CardIds = CardIds;
        }
    }
    public class AgainstSummary
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string assemblyVerision = "";
        public string player1Name = "";
        public string player2Name = "";
        public string winner = "";
        public List<TurnOperation> turnOperations = new List<TurnOperation>();
        //简易的数字卡牌量化模型
        public class SampleCardModel
        {
            public int cardID = 0;
            public int basePoint = 0;
            public int changePoint = 0;
            public List<int> state = new List<int>();
            public SampleCardModel(){}
        }
        public class TurnOperation
        {
            public int roundRank;//当前小局数
            public int turnRank;//当前回合数
            public int totalTurnRank;//当前总回合数
            public bool isOnTheOffensive;//是否先手
            public int absoluteStartPoint;//玩家操作前双方的点数差
            public int absoluteEndPoint;//玩家操作后双方的点数差  
            //0表示不投降，1表示玩家1投降，2表示玩家2投降
            public int isSurrender = 0;
            public List<List<SampleCardModel>> allCardList = new List<List<SampleCardModel>>();
            public PlayerOperation playerOperation;
            public List<SelectOperation> selectOperations = new List<SelectOperation>();
            public TurnOperation(){}
            public class PlayerOperation
            {
                public List<int> operation;
                public List<SampleCardModel> targetcardList;
                public int selectCardID;
                public PlayerOperation(){}
            }
            public class SelectOperation
            {
                //操作类型 选择场地属性/从战场选择多个单位/从卡牌面板中选择多张牌/从战场中选择一个位置/从战场选择多片对战区域
                public List<int> operation;
                public int triggerCardID;

                //选择单位
                public List<SampleCardModel> targetCardList;
                public List<int> selectCardRank;
                public int selectMaxNum;
                //区域
                public int selectRegionRank;
                public int selectLocation;
                //换牌完成,true为玩家1换牌操作，false为玩家2换牌操作
                public bool isPlay1ExchangeOver;
                public SelectOperation(){}
            }
        }
    }
    [Serializable]
    public class GeneralCommand
    {
        public object[] datas { get; set; }
        public GeneralCommand(params object[] datas)
        {
            this.datas = datas;
        }
    }
    [Serializable]
    public class GeneralCommand<T>
    {
        public T[] datas;
        public GeneralCommand(params T[] datas)
        {
            this.datas = datas;
        }
    }
}