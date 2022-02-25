using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockService.Helpers
{
    public class RabbitHelper
    {
        private readonly RabbitMQService _rabbitMQService;
        private readonly IConfiguration _configuration;

        public RabbitHelper(IConfiguration configuration)
        {
            _configuration = configuration;

            _rabbitMQService = new RabbitMQService(_configuration);

        }
        public RabbitHelper(string user, string password, string host, string port) =>
            _rabbitMQService = new RabbitMQService(user, password, host, port);

        public void SetMessage(string queueName, string message)
        {
            using (var connection = _rabbitMQService.GetRabbitMQConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queueName, true, false, false, null);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish("", queueName, null, Encoding.UTF8.GetBytes(message));
                }

            }
        }
        public List<string> GetMessage(string queueName)
        {
            List<string> messages = new List<string>();
            using (var connection = _rabbitMQService.GetRabbitMQConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queueName, true, false, false, null);
                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                    channel.BasicConsume(queueName, false, consumer);
                    consumer.Received += (model, e) =>
                    {
                        var body = e.Body.ToArray();
                        string message = Encoding.UTF8.GetString(body);
                        messages.Add(message);
                        channel.BasicAck(e.DeliveryTag, false);

                    };
                    channel.BasicConsume(queue: queueName,
                                   autoAck: true,
                                   consumer: consumer);
                }

            }
            return messages;

        }
    }
}
