using System.Diagnostics;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RESTfulWebAPITask2.Services;
using Shared;

namespace RESTfulWebAPITask2
{
    public class RabbitMqListener
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string ExchangeName = "catalog_exchange";
        private const string QueueName = "cart_queue";
        private const string RoutingKey = "catalog.item.updated";

        private readonly IServiceProvider _serviceProvider;

        public RabbitMqListener(IServiceProvider serviceProvider)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare exchange and queue
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable: true);
            _channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: RoutingKey);


            // Configure QoS - fetch one message at a time
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            //initialize service reference
            _serviceProvider = serviceProvider;
        }

        public void StartListening()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                try
                {
                    var catalogEvent = JsonSerializer.Deserialize<ItemUpdatedEvent>(message);
                    if (catalogEvent != null)
                    {
                        // Process the message
                        Console.WriteLine($"Received event: ItemId: {catalogEvent.ItemId}, Name: {catalogEvent.Name}, Price: {catalogEvent.Price}");
                        
                        //Update cart
                        Task.Delay(100);
                        // Logic to update the cart in databse
                        UpdateCart(catalogEvent);

                        // Acknowledge the message
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");

                    // Reject and requeue the message
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);

            Console.WriteLine("Listener started, Waiting for messages...");
        }

        private void UpdateCart(ItemUpdatedEvent itemUpdated)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                // Create a scoped service to resolve ICartItemService
                var _cartItemService = scope.ServiceProvider.GetRequiredService<ICartItemService>();

                //var cartItem = _cartItemService.GetOnlyCartItems().Find(x => x.Name.ToLower() == itemUpdated.Name.ToLower());
                var cartItem = _cartItemService.GetOnlyCartItems().Find(x => x.Id == itemUpdated.ItemId);
                if (cartItem != null)
                {
                    cartItem.Price = itemUpdated.Price;
                    cartItem.Name = itemUpdated.Name;

                    // Cart Item Update
                    _cartItemService.UpdateSpecificCartItem(cartItem);
                    Console.WriteLine($"Cart updated with ItemId = {itemUpdated.ItemId}, Name = {itemUpdated.Name}, Price = {itemUpdated.Price}");
                }
                else
                {
                    // No Item found message
                    Console.WriteLine($"No item found in the cart with ItemId: {itemUpdated.ItemId}");
                }
            }
        }
        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }

    }
}
