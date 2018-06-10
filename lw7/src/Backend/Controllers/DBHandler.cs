using System;
using StackExchange.Redis;
using System.Threading;

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
                if (value != null)
                {
                    try
                    {
                        float tmpValue = (float) Convert.ToDouble(value);
                        break;
                    }
                    catch(FormatException)
                    {

                    }
                }

                Thread.Sleep(sleepTime);
                value = db.StringGet(key);
            }

            Console.WriteLine(key + " got from db" + dbIndex);
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
