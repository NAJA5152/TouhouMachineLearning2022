namespace Server
{
    class HoldListManager
    {
        static Dictionary<AgainstModeType, List<HoldInfo>> HoldLists { get; set; } = new Dictionary<AgainstModeType, List<HoldInfo>>();

        static List<PlayerInfo> playerInfos = new List<PlayerInfo>();
        public static void Add(AgainstModeType againstMode, PlayerInfo playerInfo) => HoldLists[againstMode].Add(new HoldInfo(playerInfo.Rank, 0, 0));
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
            for (int i = 0; i < HoldLists.Count; i++)
            {
                var mode = HoldLists.Keys.ToList()[i];
                var targetHoldList = HoldLists.Values.ToList()[i];
                if (mode == AgainstModeType.Story || mode == AgainstModeType.Practice)
                {
                    //单人类型，直接创建房间并移除
                }
                if (mode == AgainstModeType.Casual || mode == AgainstModeType.Rank || mode == AgainstModeType.Arena)
                {
                    //多人类型，从排队最久的用户开始检索，寻找一定范围内等级+胜率差最低的单位 成对创建房间并移除
                    List<HoldInfo> successMatchList = new List<HoldInfo>();
                    //循环匹配列表
                    for (int j = 0; j < targetHoldList.Count; j++)
                    {
                        var currentHoldInfo = targetHoldList[j];
                        //如果当前目标还没有成功匹配
                        if (!successMatchList.Contains(currentHoldInfo))
                        {
                            var tempMathchList = targetHoldList
                                 .Skip(i)//直接跳到当前匹配目标之后开始
                                 .Except(successMatchList)//排除成功匹配到的目标
                                 .Take(10);//先暂定十个，会根据匹配时长等级上升
                            var result = tempMathchList
                                .Select(tempHoldInfo => new { score = MatchRule(tempHoldInfo, currentHoldInfo), holdInfo = tempHoldInfo })
                                .OrderBy(x => x.score)
                                .FirstOrDefault();
                            //先暂定为5，会根据匹配时长等级上升
                            if (result != null && result.score < 5)
                            {
                                successMatchList.Add(currentHoldInfo);
                                successMatchList.Add(result.holdInfo);
                                RoomCommand.CreatRoom(currentHoldInfo, result.holdInfo);
                            }
                        }
                    }
                    //移除已成功匹配的目标
                    successMatchList.ForEach(targetHoldInfo => targetHoldList.Remove(targetHoldInfo));
                }
            }

            //匹配规则
            static float MatchRule(HoldInfo target, HoldInfo other)
            {
                var value1 = target.Rank - other.Rank;
                var value2 = target.WinRate - other.WinRate;
                return Math.Abs(value1 + value2 * 10);
            }
        }

    }
}