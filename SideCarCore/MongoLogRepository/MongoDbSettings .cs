using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SideCarCore.MongoLogRepository
{
    public class MongoDbSettings:IMongoDbSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
    }
}
