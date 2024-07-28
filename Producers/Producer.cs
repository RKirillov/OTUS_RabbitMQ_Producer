using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace Producer.Producers
{
    public class Producer
    {
        private string _exchangeType;
        private string _exchangeName;
        private string _routingKey;
        private IModel _model;
        public Producer(string exchangeType, string exchangeName, string routingKey, IModel model)
        {
            _exchangeName = exchangeName;
            _exchangeType = exchangeType;
            _routingKey = routingKey;
            _model = model;
            _model.ExchangeDeclare(_exchangeName, _exchangeType);//объ€вл€ю обменник
        }
        
        public void Produce(string messageContent)
        {
            var message = new MessageDto()
            {
                Content = $"{messageContent} (exchange: {_exchangeType})"
            };
            //содержимое должно быть сериализовано
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            //сообщение в реббите - маршрутно адресна€ инфлормаци€
            //exchange: _exchangeName, - наименование обменника, может быть несколько
            _model.BasicPublish(exchange: _exchangeName,
                routingKey: _routingKey,//маршрутный ключ
                basicProperties: null,//хедеры если нужны
                body: body);//содержимое сообщени€

            Console.WriteLine($"Message is sent into Default Exchange: {_exchangeName}");
        }
    }
}