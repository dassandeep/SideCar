using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SideCarCore.KafkaRepository
{
    public interface IKafkaSettings
    {
        string ConnectionString { get; set; }
        public List<string> Topicname { get; set; }
    }
}
