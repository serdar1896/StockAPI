using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StockConsumer.Consumer.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockConsumer.Consumer
{
    public class StockConsumer : BackgroundService
    {
        private IConnection _connection;
        private readonly IConfiguration _configuration;


        private IModel _stockChannel;

        private readonly StockConsumerService _service;
        public StockConsumer(StockConsumerService service, IConfiguration configuration)
        {
            _configuration = configuration;
            _service = service;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory();

            string host = _configuration["RabbitMQ:Host"];
            string port = _configuration["RabbitMQ:Port"];
            string user = _configuration["RabbitMQ:User"];
            string password = _configuration["RabbitMQ:Password"];

            factory.Uri = new Uri($"amqp://{user}:{password}@{host}/{port}");

            _connection = factory.CreateConnection();

            _stockChannel = _connection.CreateModel();

            _stockChannel.QueueDeclare("StockQueue", true, false, false, null);
            _stockChannel.BasicQos(0, 1, false);            

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var stockConsumer = new EventingBasicConsumer(_stockChannel);
            stockConsumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());

                try
                {
                    StockHandler(content);

                    _stockChannel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception e)
                {
                    _stockChannel.BasicReject(ea.DeliveryTag, true);
                }

            };

            stockConsumer.Shutdown += OnConsumerShutdown;
            stockConsumer.Registered += OnConsumerRegistered;
            stockConsumer.Unregistered += OnConsumerUnregistered;
            stockConsumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _stockChannel.BasicConsume("StockQueue", false, stockConsumer);


            return Task.CompletedTask;
        }   
   
        private  void StockHandler(string message)
        {
             _service.InsertorUpdateStock(message);
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {

            _stockChannel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
