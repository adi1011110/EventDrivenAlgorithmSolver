using EDAS.BacktrackingCombinatronics;
using EDAS.Common;
using EDAS.WebApp.Services;
using EDAS.Worker.Handlers.Commands;
using EDAS.Worker.Mapper;
using EDAS.Worker.Services;
using EDAS.Worker.Services.Factory;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(sp =>
    new RabbitMQClientService(RabbitMQConfig.HOSTNAME, 
    RabbitMQConfig.USERNAME, 
    RabbitMQConfig.PASSWORD));

builder.Services.AddHostedService<ConsumerService>();

builder.Services.AddScoped<ICombinationsAlgorithmFactory, CombinationsAlgorithmFactory>();

//builder.Services.AddScoped<ICombinationAlgo, CombinationAlgo>();

var assembly = typeof(Program).Assembly;

//NOTE: MediatR scope is singleton in BackgroundService
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
});

builder.Services.AddScoped(typeof(IRequestHandler<CombinationsInput, CombinationsOutput>), 
    typeof(SolveCombinationsAlgorithmHandler));

builder.Services.AddAutoMapper(typeof(MapperProfile));

var app = builder.Build();

app.Run();
