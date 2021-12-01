using MongoDB.Driver;
using System;
using System.Collections.Generic;
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
            //client = new MongoClient("mongodb://127.0.0.1:28020");
            Console.WriteLine("链接数据库");
            client = new MongoClient("mongodb://106.15.38.165:28020");
            db = client.GetDatabase("Gezi");
            playerInfoCollection = db.GetCollection<PlayerInfo>("PlayerInfo");
            cardConfigCollection = db.GetCollection<CardConfig>("CardConfig");
            summaryCollection = db.GetCollection<AgainstSummary>("AgainstSummary");
        }
        public static int GetRegisterPlayerCount() => playerInfoCollection.AsQueryable().Count();
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
            //var LoadUserInfoQuery = Builders<PlayerInfo>.Filter.Where(info => info.Account == account && password.GetSaltHash(info.UUID)==info.Password );
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
        //////////////////////////////////////////////////用户信息更新///////////////////////////////////////////////////////////////////
        internal static bool UpdateName(string account, string password, string name)
        {
            return UpdateInfo(account, password, (x => x.Name), name);
        }

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

        public static bool UpdateDecks(PlayerInfo playinfo)
        {
            //先验证账号有效性
            //然后验证卡组有效性
            //最后修改数据库
            var CheckUserExistQuery = Builders<PlayerInfo>.Filter.Where(info => info.Name == playinfo.Name);
            var updateDecks = Builders<PlayerInfo>.Update.Set(x => x.Decks, playinfo.Decks);
            var updateDecksNum = Builders<PlayerInfo>.Update.Set(x => x.UseDeckNum, playinfo.UseDeckNum);
            IFindFluent<PlayerInfo, PlayerInfo> findFluent = playerInfoCollection.Find(CheckUserExistQuery);

            if (findFluent.CountDocuments() > 0)
            {
                findFluent.FirstOrDefault().Decks = playinfo.Decks;
                findFluent.FirstOrDefault().UseDeckNum = playinfo.UseDeckNum;
                playerInfoCollection.UpdateOne(CheckUserExistQuery, updateDecks);
                playerInfoCollection.UpdateOne(CheckUserExistQuery, updateDecksNum);
                return true;//修改成功
            }
            else
            {
                return false;//修改失败
            }
        }
        public static bool UpdateState(string account, string password, UserState userState)
        {

            var CheckUserExistQuery = Builders<PlayerInfo>.Filter.Where(info => info.Account == account && info.Password == password);
            var updateUserState = Builders<PlayerInfo>.Update.Set(x => x.OnlineUserState, userState);
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
        public static void InsertAgainstSummary(AgainstSummary againstSummary) => summaryCollection.InsertOne(againstSummary);
        public static List<AgainstSummary> QueryAgainstSummary(string playerName, int skipCount, int takeCount)
        {
            return summaryCollection.AsQueryable().Where(summary => playerName == "" ? true : summary.Player1Name == playerName || summary.Player2Name == playerName).Skip(skipCount).Take(takeCount).ToList();
        }
        public static string InsertOrUpdateCardConfig(CardConfig cardConfig)
        {
            var CheckConfigExistQuery = Builders<CardConfig>.Filter.Where(config => config.Date == cardConfig.Date);
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
        public static CardConfig GetCardConfig(string date)
        {
            if (date == "")
            {
                date = cardConfigCollection.AsQueryable().Max(x => x.Date);
            }
            var target = cardConfigCollection.Find(x => x.Date == date).FirstOrDefault();
            return target;
        }
        public static string GetLastCardUpdateTime() => cardConfigCollection.AsQueryable().Max(x => x.UpdataTime).ToString();
    }
}
