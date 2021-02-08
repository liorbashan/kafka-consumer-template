using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaConsumer.services
{
    public interface IKafkaService
    {
        void Subscribe();
        ConsumeResult<string, string> Consume();
        bool ProcessMessage(ConsumeResult<string, string> message);

        Task StartAsync(CancellationToken cancellationToken);
    }
}
