using StackExchange.Redis;
using System;

namespace VowelConsCounter
{
    class DBHandler
    {
        public string GetValueById(string key) 
        {
            int dbIndex = GetDBIndexByMessageHash(key);
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost, abortConnect=false");
            IDatabase db = redis.GetDatabase(dbIndex);
            string value = db.StringGet(key);
            Console.WriteLine(key + " got from db" + dbIndex);

            return value;
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
