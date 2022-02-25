using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace StockService.Helpers
{
    public class RabbitMQService
    {
        private readonly IConfiguration _configuration;

        private readonly string _user;
        private readonly string _password;
        private readonly string _host;
        private readonly string _port;

        public RabbitMQService(IConfiguration configuration)
        {
            _configuration = configuration;

            _user = _configuration["RabbitMQ:User"];
            _password = _configuration["RabbitMQ:Password"];
            _host = _configuration["RabbitMQ:Host"];
            _port = _configuration["RabbitMQ:Port"];
        }

        public RabbitMQService(string user, string password, string host, string port)
        {
            _user = user;
            _password = password;
            _host = host;
            _port = port;
        }

        public IConnection GetRabbitMQConnection()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();

            connectionFactory.Uri = new Uri($"amqp://{_user}:{_password}@{_host}/{_port}");

            return connectionFactory.CreateConnection();
        }


    }
}
