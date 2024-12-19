using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace HotelCustomerAPI
{
    public class RabbitMqService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName;

        public IModel Channel => _channel;

        public RabbitMqService(IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(configuration["RabbitMq:Uri"]),
                ClientProvidedName = configuration["RabbitMq:ClientProvidedName"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            var exchangeName = configuration["RabbitMq:ExchangeName"];
            _queueName = configuration["RabbitMq:QueueName"];
            var routingKey = configuration["RabbitMq:RoutingKey"];

            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(_queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(_queueName, exchangeName, routingKey, arguments: null);
            _channel.BasicQos(0, 1, false);
        }

        public void Consume(EventingBasicConsumer consumer)
        {
            _channel.BasicConsume(_queueName, false, consumer);
        }

        public void Acknowledge(ulong deliveryTag)
        {
            _channel.BasicAck(deliveryTag, false);
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
