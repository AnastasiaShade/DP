using StackExchange.Redis;
using System;

namespace VowelConsRater
{
    class DBHandler
    {
        public void SendToDB(string key, float value) 
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
    }
}
