using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using JobApplication.Jobs;

namespace JobApplication.MQTT
{
    public interface IMqttService
    {
        Task StartAsync();
    }

    public class MqttService : IMqttService
    {
        private readonly ILogger<MqttService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<string, Type> _jobMappings;
        private IMqttClient _mqttClient;

        public MqttService(ILogger<MqttService> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;

            _jobMappings = new Dictionary<string, Type>
        {
            { "mytopic/job1", typeof(IMyJob) },
            { "mytopic/job2", typeof(IAnotherJob) }
        };
        }

        public async Task StartAsync()
        {
            var mqttFactory = new MqttFactory();
            _mqttClient = mqttFactory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithClientId("JobAppClient")
                .WithTcpServer(_configuration["MqttSettings:Broker"])
                .Build();
            _mqttClient.ConnectedAsync += async e =>
            {
                _logger.LogInformation("Connected to MQTT broker.");

                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                    .WithTopic(_configuration["MqttSettings:Topic"])
                    .Build());

                _logger.LogInformation($"Subscribed to topic {_configuration["MqttSettings:Topic"]}");
            };

            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                _logger.LogInformation($"Received message: {e.ApplicationMessage.Topic}");

                if (_jobMappings.TryGetValue(e.ApplicationMessage.Topic, out var jobType))
                {
                    using var scope = _serviceProvider.CreateScope();

                    if (scope.ServiceProvider.GetRequiredService(jobType) is ISchedulerJob job)
                    {
                        await job.ExecuteAsync();
                    }

                }
                else
                {
                    _logger.LogWarning($"No job mapping found for topic: {e.ApplicationMessage.Topic}");
                }
            };

            await _mqttClient.ConnectAsync(options, CancellationToken.None);
        }
    }
}
