using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

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
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine("链接数据库");
            client = new MongoClient("mongodb://106.15.38.165:28020");
            db = client.GetDatabase("Gezi");
            playerInfoCollection = db.GetCollection<PlayerInfo>("PlayerInfo");
            cardConfigCollection = db.GetCollection<CardConfig>("CardConfig");
            summaryCollection = db.GetCollection<AgainstSummary>("AgainstSummary");
        }
        //////////////////////////////////////////////////账号系统///////////////////////////////////////////////////////////////////
        public static int Register(string account, string password)
        {
            var CheckUserExistQuery = Builders<PlayerInfo>.Filter.Where(info => info.Account == account);
            if (playerInfoCollection.Find(CheckUserExistQuery).CountDocuments() > 0)
            {
                return -1;//已存在
            }
            else
            {
                playerInfoCollection.InsertOne(
                    new PlayerInfo().Creat(account, password, "萌新",
                    new List<CardDeck>()
                    {
                        new CardDeck("npc", 20001, new List<int>
                        {
                            20002,20003,20004,20005,
                            20006,20007,20008,20009,20010,20011,
                            20012,20013,20014,20015,20016,
                            20012,20013,20014,20015,20016,
                            20012,20013,20014,20015,20016,
                        })
                    },
                    new Dictionary<string, int>
                    {
                        {  "20001",3  },
                        {  "20002",3  },
                    }));
                return 1;//成功
            }
        }
        public static PlayerInfo? Login(string account, string password)
        {
            var CheckUserExistQuery = Builders<PlayerInfo>.Filter.Where(info => info.Account == account);
            PlayerInfo? userInfo = null;
            if (playerInfoCollection.Find(CheckUserExistQuery).Any())
            {
                userInfo = playerInfoCollection.Find(CheckUserExistQuery).FirstOrDefault();
                if (userInfo.Password == password.GetSaltHash(userInfo.UID))
                {

                }
                else
                {
                    userInfo = null;
                }
            }
            //1正确,-1密码错误,-2无此账号
            //return (UserInfo != null ? 1 : playerInfoCollection.Find(CheckUserExistQuery).CountDocuments() > 0 ? -1 : -2, UserInfo);
            return userInfo;
        }
        public static int GetRegisterPlayerCount() => playerInfoCollection.AsQueryable().Count();
        //////////////////////////////////////////////////用户信息更新///////////////////////////////////////////////////////////////////
        public static bool UpdateInfo<TField>(string account, string password, Expression<Func<PlayerInfo, TField>> setOperator, TField field)
        {
            var CheckUserExistQuery = Builders<PlayerInfo>.Filter.Where(info => info.Account == account && info.Password == password);
            var updateUserState = Builders<PlayerInfo>.Update.Set(setOperator, field);
            IFindFluent<PlayerInfo, PlayerInfo> findFluent = playerInfoCollection.Find(CheckUserExistQuery);
            if (findFluent.CountDocuments() > 0)
            {
                playerInfoCollection.UpdateOne(CheckUserExistQuery, updateUserState);
                return true;//修改成功
            }
            else
            {
                return false;//修改失败
            }
        }
        //////////////////////////////////////////////////对战记录///////////////////////////////////////////////////////////////////
        public static void InsertAgainstSummary(AgainstSummary againstSummary) => summaryCollection.InsertOne(againstSummary);
        public static List<AgainstSummary> QueryAgainstSummary(string playerName, int skipCount, int takeCount)
        {
            return summaryCollection.AsQueryable().Where(summary => playerName == "" ? true : summary.Player1Name == playerName || summary.Player2Name == playerName).Skip(skipCount).Take(takeCount).ToList();
        }
        //////////////////////////////////////////////////卡牌配置///////////////////////////////////////////////////////////////////
        public static string InsertOrUpdateCardConfig(CardConfig cardConfig)
        {
            var CheckConfigExistQuery = Builders<CardConfig>.Filter.Where(config => config.Version == cardConfig.Version);
            IFindFluent<CardConfig, CardConfig> findFluent = cardConfigCollection.Find(CheckConfigExistQuery);
            if (findFluent.CountDocuments() > 0)
            {
                Console.WriteLine("修改卡组配置文件");
                cardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.UpdataTime, cardConfig.UpdataTime));
                cardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.AssemblyFileData, cardConfig.AssemblyFileData));
                cardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.SingleCardFileData, cardConfig.SingleCardFileData));
                cardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.MultiCardFileData, cardConfig.MultiCardFileData));
                return "已修改配置";//修改成功
            }
            else
            {
                Console.WriteLine("增加卡组配置文件");
                cardConfigCollection.InsertOne(cardConfig);
                return "已新增配置";//修改失败
            }
        }
        public static CardConfig GetCardConfig(string version)
        {
            if (version == "")
            {
                version = cardConfigCollection.AsQueryable().Max(x => x.Version);
            }
            var target = cardConfigCollection.Find(x => x.Version == version).FirstOrDefault();
            return target;
        }
        public static string GetLastCardUpdateTime() => cardConfigCollection.AsQueryable().Max(x => x.UpdataTime).ToString();
        public static string GetLastCardUpdateVersion() => cardConfigCollection.AsQueryable().Max(x => x.Version).ToString();
    }
}