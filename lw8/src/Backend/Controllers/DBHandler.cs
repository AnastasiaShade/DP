using System;
using StackExchange.Redis;
using System.Threading;
using System.Globalization;

namespace Backend.Controllers
{
    public class DBHandler
    {
        public string GetFromDB(string key)
        {
            int dbIndex = GetDBIndexByMessageHash(key);
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost, abortConnect=false");
            IDatabase db = redis.GetDatabase(dbIndex);
            string value = db.StringGet(key);

            int tryNumber = 10;
            int sleepTime = 200;

            for (int i = 0; i <= tryNumber; ++i)
            {
                try
                {
                    value = db.StringGet(key);
                    float floatValue = float.Parse(value, CultureInfo.InvariantCulture);
                    Console.WriteLine("Success parsed to float");
                    break;
                }
                catch(Exception ex)
                {
                    Thread.Sleep(sleepTime);
                }
            }

            Console.WriteLine(key + ":" + value + " got from db" + dbIndex);
            return value;
        }

        public void SendToBD(string key, string value)
        {
            int dbIndex = GetDBIndexByMessageHash(key);
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost, abortConnect=false");
            IDatabase db = redis.GetDatabase(dbIndex);
            db.StringSet(key, value);
            Console.WriteLine(key + " sent to db" + dbIndex);
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

        public string GetStatisticsFromDB()
        {                                     
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost, abortConnect=false");
            IDatabase db = redis.GetDatabase();

            string value = db.StringGet("statistics");
            return value;
        }
    }
}
