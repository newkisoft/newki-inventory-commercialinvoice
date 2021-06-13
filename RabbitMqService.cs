using System.Text;
using System.Text.Json;
using newkilibraries;
using RabbitMQ.Client;

namespace newki_inventory_commercialinvoice.Services
{
    public interface IRabbitMqService
    {
        void Enqueue(string queueName,InventoryMessage inventoryMessage);
    }
    public class RabbitMqService : IRabbitMqService
    {
       private IConnection _connection;

        public RabbitMqService()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = "user";
            factory.Password = "password";
            factory.HostName = "localhost";            

            _connection = factory.CreateConnection();
        }
  
        public void Enqueue(string queueName,InventoryMessage inventoryMessage)
        {
           using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queueName, false, false, false);
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(inventoryMessage));
                channel.BasicPublish(exchange: string.Empty,
                                routingKey: queueName,
                                basicProperties: null,
                                body: body);
            }
        }
    }
}