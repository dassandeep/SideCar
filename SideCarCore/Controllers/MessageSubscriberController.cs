using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SideCarCore.DomainEntity;
using SideCarCore.KafkaRepository;
using SideCarCore.MongoLogRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SideCarCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageSubscriberController : ControllerBase
    {
        private readonly IKafkaSettings _kafkaSettings = null;
        public MessageSubscriberController(IKafkaSettings kafkaSettings)
        {
            _kafkaSettings = kafkaSettings;
        }
        [HttpPost]
        public void PaymentProducer([FromBody] PaymentContext paymentContext)
        {
            if(paymentContext!=null)
            {
                if(_kafkaSettings.Topicname.ToString()==paymentContext.Topic.ToString())
                {
                    Publisher.PushMessage(_kafkaSettings.ConnectionString, "paymentContext", paymentContext.Topic.ToString());
                }

            }
        }
    }
}
/// https://nielsberglund.com/2019/06/18/confluent-platform--kafka-for-a-.net-developer-on-windows/
/// https://medium.com/@shesh.soft/confluent-kafka-integration-with-net-core-2a489fa2512
/// //https://medium.com/@marekzyla95/mongo-repository-pattern-700986454a0e
/// https://www.elemarjr.com/en/archive/building-a-simple-paas-with-sidecars-from-designing-distributed-systems-book-in-c/
/// https://engineering.chrobinson.com/dotnet-avro/guides/kafka
/// https://docs.confluent.io/5.5.0/schema-registry/schema-validation.html
