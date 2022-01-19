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
        static IMongoCollection<PlayerInfo>? PlayerInfoCollection { get; set; }
        static IMongoCollection<CardConfig>? CardConfigCollection { get; set; }
        static IMongoCollection<AgainstSummary>? SummaryCollection { get; set; }
        public static void Init()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            Console.WriteLine("链接数据库");
            client = new MongoClient("mongodb://106.15.38.165:28020");
            client = new MongoClient("mongodb://172.16.15.141:27017");
            db = client.GetDatabase("Gezi");
            PlayerInfoCollection = db.GetCollection<PlayerInfo>("PlayerInfo");
            CardConfigCollection = db.GetCollection<CardConfig>("CardConfig");
            SummaryCollection = db.GetCollection<AgainstSummary>("AgainstSummary");
        }
        //////////////////////////////////////////////////账号系统///////////////////////////////////////////////////////////////////
        public static int Register(string account, string password)
        {
            var CheckUserExistQuery = Builders<PlayerInfo>.Filter.Where(info => info.Account == account);
            if (PlayerInfoCollection.Find(CheckUserExistQuery).CountDocuments() > 0)
            {
                return -1;//已存在
            }
            else
            {
                PlayerInfoCollection.InsertOne(
                    new PlayerInfo().Creat(account, password, "萌新",
                    new List<CardDeck>()
                    {
                         new CardDeck("萌新的第一套卡组", 2000001, new List<int>
                         {
                             2001001,2001002,2001003,2001004,
                             2002001,2002002,2002003,2002004,2002005,2002006,
                             2003001,2003002,2003003,2003004,2003005,
                             2003001,2003002,2003003,2003004,2003005,
                             2003001,2003002,2003003,2003004,2003005,
                         })
                    },
                    new Dictionary<string, int>
                    {
                        {  "2000001",3  },
                        {  "2001001",3  },
                    }));
                return 1;//成功
            }
        }
        public static PlayerInfo? Login(string account, string password)
        {
            var CheckUserExistQuery = Builders<PlayerInfo>.Filter.Where(info => info.Account == account);
            PlayerInfo? userInfo = null;
            if (PlayerInfoCollection.Find(CheckUserExistQuery).Any())
            {
                userInfo = PlayerInfoCollection.Find(CheckUserExistQuery).FirstOrDefault();
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
        public static int GetRegisterPlayerCount() => PlayerInfoCollection.AsQueryable().Count();
        //////////////////////////////////////////////////用户信息更新///////////////////////////////////////////////////////////////////
        public static bool UpdateInfo<TField>(string account, string password, Expression<Func<PlayerInfo, TField>> setOperator, TField field)
        {
            var CheckUserExistQuery = Builders<PlayerInfo>.Filter.Where(info => info.Account == account && info.Password == password);
            var updateUserState = Builders<PlayerInfo>.Update.Set(setOperator, field);
            IFindFluent<PlayerInfo, PlayerInfo> findFluent = PlayerInfoCollection.Find(CheckUserExistQuery);
            if (findFluent.CountDocuments() > 0)
            {
                PlayerInfoCollection.UpdateOne(CheckUserExistQuery, updateUserState);
                return true;//修改成功
            }
            else
            {
                return false;//修改失败
            }
        }
        //////////////////////////////////////////////////对战记录///////////////////////////////////////////////////////////////////
        public static void InsertAgainstSummary(AgainstSummary againstSummary) => SummaryCollection.InsertOne(againstSummary);
        public static List<AgainstSummary> QueryAgainstSummary(int skipCount, int takeCount) => SummaryCollection.AsQueryable().Skip(skipCount).Take(takeCount).ToList();
        public static List<AgainstSummary> QueryAgainstSummary(string playerAccount, int skipCount, int takeCount)
        {
            return SummaryCollection.AsQueryable().Where(summary => summary.Player1Info.Account == playerAccount || summary.Player1Info.Account == playerAccount).Skip(skipCount).Take(takeCount).ToList();
        }

        //////////////////////////////////////////////////卡牌配置///////////////////////////////////////////////////////////////////
        public static string InsertOrUpdateCardConfig(CardConfig cardConfig)
        {
            var CheckConfigExistQuery = Builders<CardConfig>.Filter.Where(config => config.Version == cardConfig.Version);
            IFindFluent<CardConfig, CardConfig> findFluent = CardConfigCollection.Find(CheckConfigExistQuery);
            if (findFluent.CountDocuments() > 0)
            {
                Console.WriteLine("修改卡组配置文件");
                CardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.UpdataTime, cardConfig.UpdataTime));
                CardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.AssemblyFileData, cardConfig.AssemblyFileData));
                CardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.SingleCardFileData, cardConfig.SingleCardFileData));
                CardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.MultiCardFileData, cardConfig.MultiCardFileData));
                return "已修改配置";//修改成功
            }
            else
            {
                Console.WriteLine("增加卡组配置文件");
                CardConfigCollection.InsertOne(cardConfig);
                return "已新增配置";//修改失败
            }
        }
        public static CardConfig GetCardConfig(string version)
        {
            if (version == "")
            {
                version = CardConfigCollection.AsQueryable().Max(x => x.Version);
            }
            var target = CardConfigCollection.Find(x => x.Version == version).FirstOrDefault();
            return target;
        }
        public static string GetLastCardUpdateTime() => CardConfigCollection.AsQueryable().Max(x => x.UpdataTime).ToString();
        public static string GetLastCardUpdateVersion() => CardConfigCollection.AsQueryable().Max(x => x.Version).ToString();
    }
}