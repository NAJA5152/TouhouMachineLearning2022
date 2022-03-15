using AntDesign;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Server
{
    public class DiyCommand
    {
        public static List<DiyCardInfo> GetDiyCardsInfo() => MongoDbCommand.DiyCardCollection.AsQueryable().ToList();
        public static string GetDiyCardsImageUrl(int uid) => GetDiyCardsInfo().FirstOrDefault(x => x.uid == uid)?.imageUrl;
        public static void AddDiyCardInfos(string name, string describe,string imageUrl)
        {
            int uid = GetDiyCardsInfo().Count();
            DiyCardInfo diyCard = new DiyCardInfo()
            {
                uid = uid,
                cardName = name,
                describe = describe,
                imageUrl = imageUrl,
                commits = new List<DiyCardInfo.Commit>
                        {
                            new DiyCardInfo.Commit()
                            {
                                user="gezi",
                                text="好烂"
                            },
                            new DiyCardInfo.Commit()
                            {
                                user="gezi",
                                text="好烂"
                            }
                        }
            };
            MongoDbCommand.DiyCardCollection.InsertOne(diyCard);
        }
    }
}