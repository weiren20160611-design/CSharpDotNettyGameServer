using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Core.Db
{
    public class RedisService
    {
        public static RedisService Instance = new RedisService();

        private ConnectionStringSettings redisIp;
        private ConnectionStringSettings redisPort;
        public void Init()
        {
            redisIp = ConfigurationManager.ConnectionStrings["redisIp"];
            redisPort = ConfigurationManager.ConnectionStrings["redisPort"];

        }

        public void SortedSetAdd(string name, string key, int value)
        {
            try
            {
                ConfigurationOptions options = new ConfigurationOptions()
                {
                    EndPoints = { { redisIp.ConnectionString, int.Parse(redisPort.ConnectionString) } }
                };
                ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(options);
                IDatabase db = conn.GetDatabase(3);
                db.SortedSetAdd(name, key, value);
                conn.Close();
                conn.Dispose();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
            }
        }

        public SortedSetEntry[] SortedSetRangeByRankWithScore(string rankName, int num)
        {
            ConfigurationOptions options = new ConfigurationOptions()
            {
                EndPoints = { { redisIp.ConnectionString, int.Parse(redisPort.ConnectionString) } }
            };
            ConnectionMultiplexer conn = ConnectionMultiplexer.Connect(options);
            IDatabase db = conn.GetDatabase(3);            
            SortedSetEntry[] rankValueByScore = db.SortedSetRangeByRankWithScores(rankName, 0, num, Order.Descending);
            conn.Close();
            conn.Dispose();
            return rankValueByScore;
        }
    }
}
