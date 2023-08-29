using Checkin.Common;
using Checkin.Common.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

Console.WriteLine("Started");

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var handle = new KafkaClientHandle(configuration);
var producer = new KafkaDependentProducer<string, string>(handle);

var consumerConfig = new ConsumerConfig();
configuration.GetSection("Kafka:ConsumerSettings").Bind(consumerConfig);
consumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
consumerConfig.AllowAutoCreateTopics = true;

var consumerTopic = "checkin-request";
var producerTopic = "checkin-response";

var kafkaConsumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
kafkaConsumer.Subscribe(consumerTopic);

while (true)
{
    var message = kafkaConsumer.Consume(TimeSpan.FromSeconds(1));

    if (message != null)
    {
        var checkin = JsonSerializer.Deserialize<CheckinCommand>(message.Value);
        await producer.ProduceAsync(
            producerTopic,
            new Message<string, string> {
                Key = message.Key,
                Value = new CheckinResponse(checkin.Id.ToString())
                {
                    Success = true
                }.ToString()
            });
    }
}