using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.Config
{
    /// <summary>
    /// 剧情模式的卡牌配置信息，管理每个关卡的卡组
    /// </summary>
    public class AgainstDeckConfig
    {
        public static PlayerInfo GetPlayerCardDeck(string Stage)
        {
            return Stage switch
            {
                "1-1" => new PlayerInfo(
                     "NPC", "神秘的妖怪", "yaya", "",
                     new List<CardDeck>
                     {
                        new CardDeck("gezi", 2000001, new List<int>
                        {
                            2001001,2001002,2001003,2001004,
                            2002001,2002002,2002003,2002004,2002005,2002006,
                            2003001,2003002,2003003,2003004,2003005,
                            2003001,2003002,2003003,2003004,2003005,
                            2003001,2003002,2003003,2003004,2003005,
                        })
                     }),
                "1-2" => new PlayerInfo(
                    "NPC", "神秘的妖怪", "yaya", "",
                    new List<CardDeck>
                    {
                        new CardDeck("gezi", 2000001, new List<int>
                        {
                            2001001,2001002,2001003,2001004,
                            2002001,2002002,2002003,2002004,2002005,2002006,
                            2003001,2003002,2003003,2003004,2003005,
                            2003001,2003002,2003003,2003004,2003005,
                            2003001,2003002,2003003,2003004,2003005,
                        })
                    }),
                _ => Info.AgainstInfo.onlineUserInfo.GetSampleInfo(),
            };
        }
        public static PlayerInfo GetOpponentCardDeck(string Stage)
        {

            return Stage switch
            {
                "1-1" => new PlayerInfo(
                     "NPC", "神秘的妖怪", "yaya", "",
                     new List<CardDeck>
                     {
                        new CardDeck("gezi", 2000001, new List<int>
                        {
                            2001001,2001002,2001003,2001004,
                            2002001,2002002,2002003,2002004,2002005,2002006,
                            2003001,2003002,2003003,2003004,2003005,
                            2003001,2003002,2003003,2003004,2003005,
                            2003001,2003002,2003003,2003004,2003005,
                        })
                     }),
                "1-2" => new PlayerInfo(
                    "NPC", "神秘的妖怪", "yaya", "",
                    new List<CardDeck>
                    {
                        new CardDeck("gezi", 2000001, new List<int>
                        {
                            2001001,2001002,2001003,2001004,
                            2002001,2002002,2002003,2002004,2002005,2002006,
                            2003001,2003002,2003003,2003004,2003005,
                            2003001,2003002,2003003,2003004,2003005,
                            2003001,2003002,2003003,2003004,2003005,
                        })
                    }),
                _ => Info.AgainstInfo.onlineUserInfo.GetSampleInfo(),
            };
        }
        public static PlayerInfo GetPracticeCardDeck(PracticeLeader practiceLeader)
        {

            return practiceLeader switch
            {
                PracticeLeader.Reimu_Hakurei => new PlayerInfo(
                     "NPC", "神秘的妖怪", "yaya", "",
                     new List<CardDeck>
                     {
                        new CardDeck("gezi", 2000001, new List<int>
                        {
                            2001001,2001002,2001003,2001004,
                            2002001,2002002,2002003,2002004,2002005,2002006,
                            2003001,2003002,2003003,2003004,2003005,
                            2003001,2003002,2003003,2003004,2003005,
                            2003001,2003002,2003003,2003004,2003005,
                        })
                     }),
                

           PracticeLeader.Sanae_Kotiya => new PlayerInfo(
                     "NPC", "东风谷早苗", "奇迹的代言人", "",
                     new List<CardDeck>
                     {
                        new CardDeck("gezi", 2200001, new List<int>
                        {
                            2201001,2201002,2201003,2201004,
                            2202001,2202002,2202003,2202004,2202005,2202006,
                            2203001,2203002,2203003,2203004,2203005,
                            2203001,2203002,2203003,2203004,2203005,
                            2203001,2203002,2203003,2203004,2203005,
                        })
                     }),
                PracticeLeader.Mononobe_no_Futo => throw new NotImplementedException(),
                PracticeLeader.Kaku_Seiga => throw new NotImplementedException(),
                PracticeLeader.Hijiri_Byakuren => throw new NotImplementedException(),
                PracticeLeader.Koishi_Komeiji => throw new NotImplementedException(),
                PracticeLeader.Nitori_Kawasiro => new PlayerInfo(
                     "NPC", "荷城河取", "卡牌系统的设计师", "",
                     new List<CardDeck>
                     {
                        new CardDeck("gezi", 2100001, new List<int>
                        {
                            2101001,2101002,2101003,2101004,
                            2102001,2102002,2102003,2102004,2102005,2102006,
                            2103001,2103002,2103003,2103004,2103005,
                            2103001,2103002,2103003,2103004,2103005,
                            2103001,2103002,2103003,2103004,2103005,
                        })
                     }),
                PracticeLeader.Kaguya_Houraisan => throw new NotImplementedException(),
                PracticeLeader.Cirno => new PlayerInfo(
                     "NPC", "琪露诺", "雾之湖的霸主", "",
                     new List<CardDeck>
                     {
                        new CardDeck("gezi", 2000001, new List<int>
                        {
                            2001001,2001002,2001003,2001004,
                            2002001,2002002,2002003,2002004,2002005,2002006,
                            2003001,2003002,2003003,2003004,2003005,
                            2003001,2003002,2003003,2003004,2003005,
                            2003001,2003002,2003003,2003004,2003005,
                        })
                     }),
                PracticeLeader.Remilia_Scarlet => throw new NotImplementedException(),
                PracticeLeader.Kijin_Seija => new PlayerInfo(
                     "NPC", "鬼人正邪", "逆转胜负的王牌", "",
                     new List<CardDeck>
                     {
                        new CardDeck("gezi", 2010001, new List<int>
                        {
                            2011001,2011002,2011003,2011004,
                            2012001,2012002,2012003,2012004,2012005,2012006,
                            2013001,2013002,2013003,2013004,2013005,
                            2013001,2013002,2013003,2013004,2013005,
                            2013001,2013002,2013003,2013004,2013005,
                        })
                     }),
                _ => Info.AgainstInfo.onlineUserInfo.GetSampleInfo(),
            };
        }
    }
}
