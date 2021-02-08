using KafkaConsumer.services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaConsumer
{
    public class App
    {
        public IKafkaService _kafkaService { get; set; }

        public App(IKafkaService kafkaService)
        {
            this._kafkaService = kafkaService;
        }
        public Task Run()
        {
            CancellationToken cancellationToken = new CancellationToken();
            return this._kafkaService.StartAsync(cancellationToken);
        }
    }
}
