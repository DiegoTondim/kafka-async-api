using System;
using Checkin.Common;
using Checkin.Common.Kafka;
using Confluent.Kafka;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Reservation.Checkin.API.Services
{
	public class CheckinPublisher
	{
        private readonly KafkaDependentProducer<string, string> _producer;

        public CheckinPublisher(KafkaDependentProducer<string, string> producer)
        {
            _producer = producer;
        }

        public async Task<string> PublishAsync(CheckinCommand command)
        {
            command.GenerateId();
            await _producer.ProduceAsync(
                "checkin-request",
                new Message<string, string>
                    {
                        Key = command.Pnr,
                        Value = command.ToString()
                    });

            return command.Id;
        }
    }
}

