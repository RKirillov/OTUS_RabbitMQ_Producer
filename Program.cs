using Microsoft.Extensions.Configuration;
using Producer.Settings;
using RabbitMQ.Client;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            //связь с докер физический канал связи
            var connection = GetRabbitConnection(configuration);
            //виртуальный канал, вся работа через него. потом закроем.
            var channel = connection.CreateModel();

            var producer = new Producers.Producer("direct", "exchange.direct", "cars.1", channel);
            //var producer = new Producers.Producer("fanout", "exchange.fanout", "cars.1", channel);
            //var producer = new Producers.Producer("topic", "exchange.topic", "cars.1", channel);

            producer.Produce("Message");
            /*
            while (true)
            {
                producer.Produce("Message");
                Thread.Sleep(TimeSpan.FromSeconds(1)); // Имитация долгой обработки
            }
            */
            // в конце все закрываем
            channel.Close();
            connection.Close();
        }
        
        private static IConnection GetRabbitConnection(IConfiguration configuration)
        {
            var rmqSettings = configuration.Get<ApplicationSettings>().RmqSettings;
            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = rmqSettings.Host,
                VirtualHost = rmqSettings.VHost,
                UserName = rmqSettings.Login,
                Password = rmqSettings.Password,
            };

            return factory.CreateConnection();
        }
    }
}
