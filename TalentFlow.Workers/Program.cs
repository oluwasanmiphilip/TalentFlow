using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using TalentFlow.Application.Common.Interfaces;
using TalentFlow.Infrastructure.Email;
using TalentFlow.Infrastructure.Sms;
using TalentFlow.Workers;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // ============================
        // RABBITMQ (CLOUDAMQP - FIXED)
        // ============================
        services.AddSingleton<IConnection>(sp =>
        {
            var cloudAmqpUrl =
                context.Configuration["CLOUDAMQP_URL"]
                ?? Environment.GetEnvironmentVariable("CLOUDAMQP_URL");

            if (string.IsNullOrWhiteSpace(cloudAmqpUrl))
                throw new Exception("CLOUDAMQP_URL environment variable is missing");

            var factory = new ConnectionFactory
            {
                Uri = new Uri(cloudAmqpUrl)
            };

            return factory.CreateConnectionAsync()
                          .GetAwaiter()
                          .GetResult();
        });

        // ============================
        // EMAIL & SMS SERVICES
        // ============================
        services.Configure<SmtpSettings>(
            context.Configuration.GetSection("SMTP"));

        services.AddTransient<IEmailService, SmtpEmailService>();
        services.AddTransient<ISmsService, SmtpSmsService>();

        // ============================
        // WORKER
        // ============================
        services.AddHostedService<OtpWorker>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build()
    .Run();