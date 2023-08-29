using Checkin.Common.Kafka;
using Reservation.Checkin.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<KafkaClientHandle>();
builder.Services.AddSingleton<CheckinPublisher>();
builder.Services.AddSingleton<KafkaDependentProducer<string, string>>();
builder.Services.AddHostedService<CheckinResponseConsumer>();
builder.Services.AddSingleton<CheckinService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

