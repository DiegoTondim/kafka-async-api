using System;
using Checkin.Common;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace Reservation.Checkin.API.Services
{
    /// <summary>
    ///     A simple example demonstrating how to set up a Kafka consumer as an
    ///     IHostedService.
    /// </summary>
    public class CheckinResponseConsumer : BackgroundService
    {
        private readonly string topic;
        private readonly IConsumer<string, string> kafkaConsumer;
        private readonly CheckinService _checkinService;

        public CheckinResponseConsumer(IConfiguration config, CheckinService service)
        {
            _checkinService = service;
            var consumerConfig = new ConsumerConfig();
            consumerConfig.AllowAutoCreateTopics = true;
            consumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;

            config.GetSection("Kafka:ConsumerSettings").Bind(consumerConfig);
            this.topic = "checkin-response";
            this.kafkaConsumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
        }

        private void StartConsumerLoop(CancellationToken cancellationToken)
        {
            kafkaConsumer.Subscribe(this.topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = this.kafkaConsumer.Consume(cancellationToken);

                    _checkinService.StoreResponse(JsonConvert.DeserializeObject<CheckinResponse>(cr.Message.Value));
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    Console.WriteLine($"Consume error: {e.Error.Reason}");

                    if (e.Error.IsFatal)
                    {
                        // https://github.com/edenhill/librdkafka/blob/master/INTRODUCTION.md#fatal-consumer-errors
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e}");
                    break;
                }
            }
        }

        public override void Dispose()
        {
            this.kafkaConsumer.Close(); // Commit offsets and leave the group cleanly.
            this.kafkaConsumer.Dispose();

            base.Dispose();
        }
    }
}

