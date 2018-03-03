using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Redis_Example
{
    public class RedisLoader
    {
        private ConnectionMultiplexer Redis { get; } 
        private IDatabase DB { get; }

        public RedisLoader(string connection)
        {
            Redis = ConnectionMultiplexer.Connect(connection);
            DB = Redis.GetDatabase();
        }

        public async Task LoadItem(string key, string value)
        {
           await DB.StringSetAsync(key, value);
        
        }

        public async Task LoadItems(IDictionary<RedisKey, RedisValue> dictionary)
        {
            await DB.StringSetAsync(dictionary.ToArray());

        }



    }
}
