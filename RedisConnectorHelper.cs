using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyDataCronJob
{
    public class RedisConnectorHelper
    {
        private const string SERVER_NAME = "localhost:6379";
        static RedisConnectorHelper()
        {
            RedisConnectorHelper.lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                var options = ConfigurationOptions.Parse(SERVER_NAME);
                options.AllowAdmin = true;
                return ConnectionMultiplexer.Connect(options);
            });
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection;

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }

        }
        /**
        * Clear cache data in the memory. 
        */
        public static void ClearRedisCache()
        {
            Connection.GetServer(SERVER_NAME).FlushDatabase();
        }

        /**
        * Store cache data in the memory of the server provided to the config of running redis server. 
        * param: key - Set key to hold the string value. If key already holds a value, it will be overwritten; Keys are set to expire after 23hours, 59 mins and 59sec
        * param: value -  value stored at key 
        */
        public static void SaveDataInRedisCache(string key, string value)
        {
            var cache = RedisConnectorHelper.Connection.GetDatabase();
            cache.KeyExpire(key, new TimeSpan(23, 59, 59));
            cache.StringSet(key, value);            
        }

    }
}
