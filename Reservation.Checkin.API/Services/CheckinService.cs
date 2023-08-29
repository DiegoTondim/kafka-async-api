using System;
using System.Diagnostics.Metrics;
using Checkin.Common;
using Polly;

namespace Reservation.Checkin.API.Services
{
	public class CheckinService
	{
        private static IList<CheckinResponse> _responses;

		public CheckinService()
		{
            _responses = new List<CheckinResponse>();
		}

        internal async Task<CheckinResponse> Fetch(string id)
        {
            var polly = Policy
               .Handle<Exception>()
               .WaitAndRetryAsync(3, x => TimeSpan.FromSeconds(1), (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            return await polly.ExecuteAsync(async () => await FetchInternal(id));
        }

        internal void StoreResponse(CheckinResponse checkinResponse)
        {
            _responses.Add(checkinResponse);
        }

        private async Task<CheckinResponse> FetchInternal(string id)
        {
            var response = _responses.FirstOrDefault(x => x.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase));

            if (response == null)
            {
                throw new Exception("not found yet");
            }

            return await Task.FromResult(response);
        }
    }
}

