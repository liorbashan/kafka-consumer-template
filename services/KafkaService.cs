using Confluent.Kafka;
using KafkaConsumer.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaConsumer.services
{
    public class KafkaService : IKafkaService
    {
        public KafkaSettings _kafkaConfig { get; set; }
        private IConsumer<string, string> consumer;
        private ConsumerConfig _consumerConfig;
        public KafkaService(IOptions<KafkaSettings> kafkaConfig)
        {
            this._kafkaConfig = kafkaConfig.Value;
            string buildDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string certLocation = Path.Combine(buildDirectory, _kafkaConfig.CaLocation);
            this._consumerConfig = new ConsumerConfig
            {
                BootstrapServers = this._kafkaConfig.Url,
                GroupId = this._kafkaConfig.ConsumerGroup,
                SaslMechanism = this._kafkaConfig.SaslMechanisms == "plain" ? SaslMechanism.Plain :
                    (this._kafkaConfig.SaslMechanisms == "gssapi" ? SaslMechanism.Gssapi : SaslMechanism.Plain),
                SaslUsername = this._kafkaConfig.SaslUsername,
                SaslPassword = this._kafkaConfig.SaslPassword,
                SslCaLocation = certLocation,
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                MaxPollIntervalMs = (int?)TimeSpan.FromMinutes(30).TotalMilliseconds,
                SecurityProtocol = this._kafkaConfig.SecurityProtocol == "SASL_SSL" ? SecurityProtocol.SaslSsl : SecurityProtocol.Plaintext
            };
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.Subscribe();
            if(this.consumer != null && this.consumer.Subscription.Count > 0)
            {
                Consume();
            }
            return Task.CompletedTask;
        }

        public ConsumeResult<string, string> Consume()
        {
            int maxAttemptsNumber = 2;
            int attemptNumber = 1;

            while (true)
            {
                ConsumeResult<string, string> consumerResult = this.consumer.Consume();
                bool sucess = ProcessMessage(consumerResult);
                if(!sucess && attemptNumber <= maxAttemptsNumber)
                {
                    this.consumer.Seek(consumerResult.TopicPartitionOffset);
                    attemptNumber += 1;
                }
                else
                {
                    // this.consumer.Commit(consumerResult);
                    attemptNumber = 1;
                }
            }
        }

        public bool ProcessMessage(ConsumeResult<string, string> message)
        {
            long offset = message.Offset.Value;
            bool result = offset == 2 ? false : true;
            return result;
        }

        public void Subscribe()
        {
            try
            {
                this.consumer = new ConsumerBuilder<string, string>(this._consumerConfig).Build();
                this.consumer.Subscribe(_kafkaConfig.TopicName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
 
    }
}
