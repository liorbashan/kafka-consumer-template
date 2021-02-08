using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using KafkaConsumer.services;
using KafkaConsumer.Configuration;
using System.Threading.Tasks;

namespace KafkaConsumer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables("KC_")
                .Build();

            var serviceCollection = new ServiceCollection()
                .AddSingleton<IKafkaService, KafkaService>()
                .Configure<KafkaSettings>(options => config.GetSection("KafkaSettings").Bind(options))
                .AddTransient<App>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            await serviceProvider.GetService<App>().Run();
        }
    }
}
