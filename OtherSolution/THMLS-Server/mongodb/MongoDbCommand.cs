using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server
{
    class MongoDbCommand
    {
        static MongoClient client;
        static IMongoDatabase db;
        static IMongoCollection<PlayerInfo> playerInfoCollection;
        static IMongoCollection<CardConfig> cardConfigCollection;
        static IMongoCollection<AgainstSummary> summaryCollection;

        public static void Init()
        {
            //client = new MongoClient("mongodb://127.0.0.1:28020");
            Console.WriteLine("链接数据库");
            client = new MongoClient("mongodb://106.15.38.165:28020");
            db = client.GetDatabase("Gezi");
            playerInfoCollection = db.GetCollection<PlayerInfo>("PlayerInfo");
            cardConfigCollection = db.GetCollection<CardConfig>("CardConfig");
            summaryCollection = db.GetCollection<AgainstSummary>("AgainstSummary");
        }
        public static string RegisterInfo(string Name, string Password)
        {
            var CheckUserExistQuery = Builders<PlayerInfo>.Filter.Where(info => info.name == Name);
            if (playerInfoCollection.Find(CheckUserExistQuery).CountDocuments() > 0)
            {
                return "-1";//已存在
            }
            else
            {
                playerInfoCollection.InsertOne(
                    new PlayerInfo(Name, Password,
                    new List<CardDeck>()
                    {
                        new CardDeck("初始卡组", 20001, new List<int>
                        {
                            20002, 20003, 20004, 20005, 20006,20007,20008, 20009, 20010, 10011,  10012, 10013, 10014, 10015, 10016,10012, 10013,10014, 10015, 10016, 10012, 10013, 10014, 10015, 10016
                        })
                    },
                    new Dictionary<string, int>
                    {
                        {  "20001",3  },
                        {  "20002",3  },
                    }));
                return "1";//成功
            }
        }
        public static int LoginInfo(string Name, string Password, out PlayerInfo UserInfo)
        {
            var LoadUserInfoQuery = Builders<PlayerInfo>.Filter.Where(info => info.name == Name && info.password == Password);
            var CheckUserExistQuery = Builders<PlayerInfo>.Filter.Where(info => info.name == Name);
            UserInfo = null;
            if (playerInfoCollection.Find(LoadUserInfoQuery).ToList().Count > 0)
            {
                UserInfo = playerInfoCollection.Find(LoadUserInfoQuery).ToList()[0];
            }
            //1正确,-1密码错误,-2无此账号
            return UserInfo != null ? 1 : playerInfoCollection.Find(CheckUserExistQuery).CountDocuments() > 0 ? -1 : -2;
        }
        internal static bool UpdateDecks(PlayerInfo playinfo)
        {
            //先验证账号有效性
            //然后验证卡组有效性
            //最后修改数据库
            var CheckUserExistQuery = Builders<PlayerInfo>.Filter.Where(info => info.name == playinfo.name);
            var updateDecks = Builders<PlayerInfo>.Update.Set(x => x.decks, playinfo.decks);
            var updateDecksNum = Builders<PlayerInfo>.Update.Set(x => x.useDeckNum, playinfo.useDeckNum);
            IFindFluent<PlayerInfo, PlayerInfo> findFluent = playerInfoCollection.Find(CheckUserExistQuery);

            if (findFluent.CountDocuments() > 0)
            {
                findFluent.FirstOrDefault().decks = playinfo.decks;
                findFluent.FirstOrDefault().useDeckNum = playinfo.useDeckNum;
                playerInfoCollection.UpdateOne(CheckUserExistQuery, updateDecks);
                playerInfoCollection.UpdateOne(CheckUserExistQuery, updateDecksNum);
                return true;//修改成功
            }
            else
            {
                return false;//修改失败
            }
        }
        public static void InsertAgainstSummary(AgainstSummary againstSummary) => summaryCollection.InsertOne(againstSummary);
        public static List<AgainstSummary> QueryAgainstSummary(string playerName, int skipCount, int takeCount)
        {
            return summaryCollection.AsQueryable().Where(summary => playerName == "" ? true : summary.player1Name == playerName || summary.player2Name == playerName).Skip(skipCount).Take(takeCount).ToList();
        }

        public static bool InsertOrUpdateCardConfig(CardConfig cardConfig)
        {
            //先验证账号有效性
            //然后验证卡组有效性
            //最后修改数据库
            var CheckConfigExistQuery = Builders<CardConfig>.Filter.Where(config => config.date == cardConfig.date);
            IFindFluent<CardConfig, CardConfig> findFluent = cardConfigCollection.Find(CheckConfigExistQuery);
            if (findFluent.CountDocuments() > 0)
            {
                Console.WriteLine("修改卡组配置文件");
                cardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.updataTime, cardConfig.updataTime));
                cardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.assemblyFileData, cardConfig.assemblyFileData));
                cardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.singleCardFileData, cardConfig.singleCardFileData));
                cardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.multiCardFileData, cardConfig.multiCardFileData));
                return true;//修改成功
            }
            else
            {
                Console.WriteLine("增加卡组配置文件");
                cardConfigCollection.InsertOne(cardConfig);
                return false;//修改失败
            }
        }
        public static string GetCardConfig(string date)
        {
            if (date == "")
            {
                date = cardConfigCollection.AsQueryable().Max(x => x.date);
            }
            var target = cardConfigCollection.Find(x => x.date == date).FirstOrDefault();
            if (target == null)
            {
                return "";
            }
            return target.ToJson();
        }
        public static string GetLastCardUpdateTime() => cardConfigCollection.AsQueryable().Max(x => x.updataTime).ToString();
    }
}
