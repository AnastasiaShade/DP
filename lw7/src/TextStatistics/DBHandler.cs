using StackExchange.Redis;
using System;

namespace TextStatistics
{
    class DBHandler
    {
        public void SendStatisticsToBD(int textNum, int highRankPart, float avgRank)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost, abortConnect=false");
            IDatabase db = redis.GetDatabase();
            string value = textNum + ":" + highRankPart + ":" + avgRank;
            db.StringSet("statistics", value);
            Console.WriteLine("statistics sent to db0");
        }
    }
}
