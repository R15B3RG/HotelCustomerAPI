using HotelCustomerAPI.Entities;
using HotelCustomerAPI.Repository;
using MongoDB.Driver;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace HotelCustomerAPI
{
    public class RabbitMqConsumerService : BackgroundService
    {
        private readonly RabbitMqService _rabbitMqService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RabbitMqConsumerService> _logger;

        public RabbitMqConsumerService(RabbitMqService rabbitMqService, IServiceScopeFactory scopeFactory, ILogger<RabbitMqConsumerService> logger)
        {
            _rabbitMqService = rabbitMqService;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ExecuteAsync called}");
            var consumer = new EventingBasicConsumer(_rabbitMqService.Channel);
            consumer.Received += async (model, args) =>
            {
                _logger.LogInformation("Consumer registered to queue: {QueueName}");
                using var scope = _scopeFactory.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();

                try
                {
                    var body = args.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    _logger.LogInformation("Received RabbitMQ message: {Message}", message);

                    var customer = JsonSerializer.Deserialize<Customer>(message);
                    if (customer == null)
                    {
                        throw new InvalidOperationException("Deserialized customer is null or invalid.");
                    }

                    await repository.AddOneAsync(customer);
                    _logger.LogInformation("Customer successfully processed and saved: {CustomerId}", customer.Id);

                    _rabbitMqService.Acknowledge(args.DeliveryTag);
                    _logger.LogInformation("Message acknowledged to RabbitMQ.");
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "JSON deserialization error while processing message: {Message}", Encoding.UTF8.GetString(args.Body.ToArray()));
                    // Optionally: Dead Letter or requeue logic for malformed messages
                }
                catch (MongoException mongoEx)
                {
                    _logger.LogError(mongoEx, "MongoDB operation failed while saving the customer.");
                    // Optionally: Implement retry logic if required
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error processing RabbitMQ message.");
                }
            };


            _rabbitMqService.Consume(consumer);
            return Task.CompletedTask;
        }
    }
}
