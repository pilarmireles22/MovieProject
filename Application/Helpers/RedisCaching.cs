using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class RedisCaching: IRedisCaching
    {
        private readonly IDistributedCache _distributedCache;
        public RedisCaching(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task<List<T>?> GetFromRedis<T>(List<T> entity, string key)
        {
            string serializedMoviesList;
            var EntityList = new List<T>();
            var redisListEntity = await _distributedCache.GetAsync(key);
            if (redisListEntity != null)
            {
                serializedMoviesList = Encoding.UTF8.GetString(redisListEntity);
                EntityList = JsonConvert.DeserializeObject<List<T>>(serializedMoviesList);
                return EntityList;
            }
            return null;
        }
        public T GetFromDatabase<T>(T EntityList, string key)
        {
            string serializedMoviesList;
            var redisListEntity = _distributedCache.Get(key);
            serializedMoviesList = JsonConvert.SerializeObject(EntityList);
            redisListEntity = Encoding.UTF8.GetBytes(serializedMoviesList);

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));
            _distributedCache.Set(key, redisListEntity, options);
            return EntityList;
        }
    }
}
