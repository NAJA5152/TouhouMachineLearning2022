namespace Server
{
    class HoldListManager
    {
        static Dictionary<AgainstModeType, List<HoldInfo>> HoldList { get; set; } = new Dictionary<AgainstModeType, List<HoldInfo>>();

        static List<PlayerInfo> playerInfos = new List<PlayerInfo>();
        public void Add(AgainstModeType againstMode, PlayerInfo playerInfo)
        {
            HoldList[againstMode].Add(new HoldInfo(playerInfo.Rank, 0, 0));
        }
        public class HoldInfo
        {
            public HoldInfo(int rank, float winRate, float collectionRate)
            {
                Rank = rank;
                WinRate = winRate;
                CollectionRate = collectionRate;
            }

            public int Rank { get; set; }
            public float WinRate { get; set; }
            public float CollectionRate { get; set; }
            public DateTime JoinTime { get; set; }
        }
        public static void Match()
        {
            foreach (var HoldList in HoldList.Values)
            {
                var oldestHoldUser = HoldList.OrderBy(x => x.CollectionRate).FirstOrDefault();
                if (oldestHoldUser != null)
                {
                    HoldList.Except(new List<HoldInfo> { oldestHoldUser }).Select(holdInfo => new { score = MatchRule(oldestHoldUser, holdInfo), holdInfo = holdInfo });
                }
               
            }

            //匹配规则
            static float  MatchRule(HoldInfo target, HoldInfo other)
            {
                var value1 = target.Rank - other.Rank;
                var value2 = target.WinRate - other.WinRate;
                return Math.Abs(value1 + value2 * 10);
            }
        }

    }
}