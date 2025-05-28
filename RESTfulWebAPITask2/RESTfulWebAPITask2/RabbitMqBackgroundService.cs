using System;
using RESTfulWebAPITask2.Services;
using Microsoft.Extensions.DependencyInjection;
using RESTfulWebAPITask2.Model;

namespace RESTfulWebAPITask2
{
    public class RabbitMqBackgroundService : BackgroundService
    {
        private readonly RabbitMqListener _listener;
        public RabbitMqBackgroundService(IServiceProvider serviceProvider)
        {
            _listener = new RabbitMqListener(serviceProvider);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            _listener.StartListening();

            // Background service runs as long as the application is active
            return Task.Delay(Timeout.Infinite, stoppingToken);
            //return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _listener.Dispose();
            base.Dispose();
        }
    }

}
