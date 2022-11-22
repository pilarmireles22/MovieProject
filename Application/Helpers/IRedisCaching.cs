using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public interface IRedisCaching
    {
        Task<List<T>?> GetFromRedis<T>(List<T> entity, string key);
        public T GetFromDatabase<T>(T EntityList, string key);
    }
}
