using StackExchange.Redis;
using System;
using System.Globalization;

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

        public float GetValueById(string id)
        {
            int dbIndex = GetDBIndexByMessageHash(id);
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost, abortConnect=false");
            IDatabase db = redis.GetDatabase(dbIndex);
            string value = db.StringGet(id);
            Console.WriteLine(id + " got from db" + dbIndex);
            return float.Parse(value, CultureInfo.InvariantCulture
            );
        }

        private int GetDBIndexByMessageHash(string msg)
        {
            int hash = 0;
            foreach (Char symbol in msg)
            {
                hash += symbol;
            }
            return hash % 16;
        }
    }
}
