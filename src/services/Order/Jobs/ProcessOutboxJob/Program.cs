using MassTransit;
using ProcessOutboxJob;
using Quartz;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("OutboxPublishJob");
            q.AddJob<OutboxPublishJob>(opts => opts.WithIdentity(jobKey));

            var triggerKey = new TriggerKey("OutboxPublishTrigger");
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity(triggerKey)
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromSeconds(5))
                    .RepeatForever()));
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.AddMassTransit(configurator =>
        {
            configurator.UsingRabbitMq((context, _configurator) =>
            {
                _configurator.Host(hostContext.Configuration["RabbitMQ:Host"], "/", hostConfigurator =>
                {
                    hostConfigurator.Username(hostContext.Configuration["RabbitMQ:Username"]!);
                    hostConfigurator.Password(hostContext.Configuration["RabbitMQ:Password"]!);
                });
            });
        });
    })
    .Build();

await host.RunAsync();