using Confluent.Kafka;
using Newtonsoft.Json;
using SideCarCore.DomainEntity;
using SolTechnology.Avro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SideCarCore.KafkaRepository
{
    public class Subscriber
    {
      
        public static PaymentContext PushMessage()
        {
            PaymentContext paymentContext = new PaymentContext();
            //if (!string.IsNullOrWhiteSpace(SchemaRegistry(paymentContext, out string schema)))
            if (!string.IsNullOrWhiteSpace(GetSchemaAvro(out string schema)))
            {
                Dictionary<string, object> configValuePairs = GetPairs("localhost:9092", "Payment", schema);
                if (configValuePairs != null)
                {
                    paymentContext = ReadMessage(configValuePairs,schema);
                }
            }
            return paymentContext;
        }

     
        private static PaymentContext  ReadMessage(Dictionary<string, object> config, string schema)
        {
            var consumer = new ConsumerBuilder<string, PaymentContext>(new ConsumerConfig
            {
                BootstrapServers = (string)config[KafkaPropNames.BootstrapServers],
                GroupId = (string)config[KafkaPropNames.GroupId],
                AutoOffsetReset = AutoOffsetReset.Earliest
            }).SetKeyDeserializer(Deserializers.Utf8)
              .SetAvroValueDeserializer(schema)
              .Build();
            var topic = (string)config[KafkaPropNames.Topic];

            consumer.Assign(new List<TopicPartitionOffset>
            {
                new TopicPartitionOffset(topic, (int)config[KafkaPropNames.Partition], (int)config[KafkaPropNames.Offset])
            });
            var handler = new Handler();
            PaymentContext paymentContext = null;
             var kafkaConsumer = new KafkaConsumer<string, PaymentContext>(
                    consumer,
                    (key, value, utcTimestamp) =>
                    {
                        Console.WriteLine($"C#     {key}  ->  ");
                        Console.WriteLine($"   {utcTimestamp}");
                        paymentContext= handler.Handle1(value);
                        Console.WriteLine(value);
                    }, CancellationToken.None)
                .StartConsuming();
            return paymentContext;
        }

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
        static Dictionary<string, object> GetPairs(string connectionString, string topicName, string schema)
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
    }
    public static class ConsumerBuilderExtensions
    {
        public static ConsumerBuilder<TKey, TValue> SetAvroValueDeserializer<TKey, TValue>(this ConsumerBuilder<TKey, TValue> consumerBuilder, string schema)
        {
            var des = new AvroConvertDeserializer<TValue>(schema);
            return consumerBuilder.SetValueDeserializer(des);
        }
    }
    public class AvroConvertDeserializer<T> : IDeserializer<T>
    {
        private readonly string _schema;

        public AvroConvertDeserializer(string schema)
        {
            var confluentSchema = new ConfluentSchema(schema);
            _schema = confluentSchema.SchemaString;
        }

        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            var dataArray = data.ToArray();
            var dataWithoutMagicNumber = dataArray.Skip(5);

            var result = AvroConvert.DeserializeHeadless<T>(dataWithoutMagicNumber.ToArray(), _schema);
            return result;
        }
    }
    public class Handler
    {
        public void Handle(PaymentContext recordModel)
        {
           Console.WriteLine(JsonConvert.SerializeObject(recordModel));
        }
        public PaymentContext Handle1(PaymentContext recordModel)
        {
            return recordModel;
        }
    }
    public class KafkaConsumer<TKey, TValue> : IDisposable
    {
        private readonly IConsumer<TKey, TValue> _consumer;
        private readonly Action<TKey, TValue, DateTime> _handler;
        private readonly CancellationToken _cancellationToken;
        private Task _taskConsumer;


        public KafkaConsumer(IConsumer<TKey, TValue> consumer, Action<TKey, TValue, DateTime> handler, CancellationToken cancellationToken)
        {
            _consumer = consumer;
            _handler = handler;
            _cancellationToken = cancellationToken;
        }

      
        public KafkaConsumer<TKey, TValue> StartConsuming()
        {
            _taskConsumer = StartConsumingInner();
            return this;
        }

     
        private async Task StartConsumingInner()
        {

            while (!_cancellationToken.IsCancellationRequested)
            {
                ConsumeResult<TKey, TValue> consumeResult = await Task.Run(() => _consumer.Consume(_cancellationToken));

                _handler(consumeResult.Message.Key, consumeResult.Message.Value, consumeResult.Message.Timestamp.UtcDateTime);
            }

            _consumer.Close();

        }


        public void Dispose()
        {
            _taskConsumer?.Wait(_cancellationToken);
        }
    }
}
