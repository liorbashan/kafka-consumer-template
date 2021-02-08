using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaConsumer.Configuration
{
    public class KafkaSettings
    {
        public string TopicName { get; set; }
        public string ConsumerGroup { get; set; }
        public string Url { get; set; }
        public string SaslMechanisms { get; set; }
        public string SecurityProtocol { get; set; }
        public string SaslUsername { get; set; }
        public string SaslPassword { get; set; }
        public string CaLocation { get; set; }
    }
}
