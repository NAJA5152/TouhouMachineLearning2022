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
                PracticeLeader.Sanae_Kotiya => throw new NotImplementedException(),
                PracticeLeader.Koishi_Komeiji => throw new NotImplementedException(),
                PracticeLeader.Nitori_Kawasiro => throw new NotImplementedException(),
                PracticeLeader.Cirno => throw new NotImplementedException(),
                PracticeLeader.Remilia_Scarlet => throw new NotImplementedException(),
                PracticeLeader.Kijin_Seija => throw new NotImplementedException(),
                _ => Info.AgainstInfo.onlineUserInfo.GetSampleInfo(),
            };
        }
    }
}
