using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using JobApplication.Jobs;
using JobApplication.MQTT;

namespace JobApplication
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var mqttService  = host.Services.GetService<IMqttService>();
            await mqttService.StartAsync();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<MQTTSettings>(context.Configuration.GetSection("MqttSettings"));
                    services.AddTransient<ISchedulerJob, EPAJob>();

                    services.AddSingleton<IMqttService, MqttService>();
                });
        
    }
}
