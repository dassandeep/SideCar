using Confluent.Kafka;
using SideCarCore.DomainEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SolTechnology.Avro;
using Chr.Avro.Abstract;
using Confluent.SchemaRegistry;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using System.IO;

namespace SideCarCore.KafkaRepository
{
    public class Publisher
    {
        static readonly AutoResetEvent _closing = new AutoResetEvent(false);

        static string GetSchemaAvro(out string schema)
        {
            string summaryQuery = string.Empty;
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\PaymentContext.txt";
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                schema = streamReader.ReadToEnd();
            }
            return schema;
        }

        static string SchemaRegistry(PaymentContext paymentContext, out string paymentContextschema)
        {
            var builder = new SchemaBuilder();
            var schema = builder.BuildSchema<PaymentContext>();

            //paymentContextschema = AvroConvert.GenerateSchema(typeof(PaymentContext));
            paymentContextschema = AvroConvert.GenerateSchema(schema.GetType());
            return paymentContextschema;
        }
        static Dictionary<string, object> GetPairs(string connectionString, string topicName,string schema)
        {
            return new Dictionary<string, object>
            {
                { KafkaPropNames.BootstrapServers, connectionString},
                { KafkaPropNames.SchemaRegistryUrl, schema},
                { KafkaPropNames.Topic, topicName },
                { KafkaPropNames.GroupId, "aa-group" },
                { KafkaPropNames.Partition, 0 },
                { KafkaPropNames.Offset, 0 },
            };
           
        }
        static void CreateProducer(Dictionary<string, object> configValuePairs, PaymentContext paymentContext,string schema)
        {
            var producer = new ProducerBuilder<string, PaymentContext>(
                 new ProducerConfig { BootstrapServers = (string)configValuePairs[KafkaPropNames.BootstrapServers] })
                 .SetKeySerializer(Serializers.Utf8)
                 .SetValueSerializer(new AvroConvertSerializer<PaymentContext>(schema))
                 .Build();
            producer.Produce((string)configValuePairs[KafkaPropNames.Topic], new Message<string, PaymentContext>
            {
                Key = paymentContext.GetType().ToString(),
                Value = paymentContext
            });

        }
       
    
        public static void PushMessage(string connectionString, PaymentContext paymentContext,string topicName)
        {
            //if (!string.IsNullOrWhiteSpace(SchemaRegistry(paymentContext, out string schema)))
            if (!string.IsNullOrWhiteSpace(GetSchemaAvro(out string schema)))
            {
                Dictionary<string, object> configValuePairs = GetPairs(connectionString, topicName, schema);
                if (configValuePairs != null)
                {
                    CreateProducer(configValuePairs, paymentContext, schema);
                }
            }
            
        }
  

    }
}
