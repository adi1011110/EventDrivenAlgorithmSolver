using EDAS.Worker.Extensions;

var builder = WebApplication.CreateBuilder(args);

var rabbitMqConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "rabbitmqconfig.json");
builder.Configuration.AddJsonFile(rabbitMqConfigPath, optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

builder.Services
    .RegisterConfigs(builder)
    .RegisterServices();

builder.Services.AddHostedService<ConsumerService>();

var app = builder.Build();

var workerType = Environment.GetEnvironmentVariable("WorkerType__Type");

app.MapGet("/",
    () => Results.Json(
                new
                {
                    status = "running",
                    message = "Worker is handling RabbitMQ messages.",
                    workerType = workerType,
                    timestamp = DateTime.UtcNow
                }));

app.Run();