using EDAS.Worker.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<ConsumerService>();

var app = builder.Build();

app.UseAuthentication();

app.UseAuthorization();

app.Run();
