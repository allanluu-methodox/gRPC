using System.Collections.Concurrent;
using System.IO;
using Grpc.Core;

namespace gRPC_Server.Services
{
    public class CalculatorService : Calculator.CalculatorBase
    {
        private readonly ILogger<CalculatorService> _logger;
        public CalculatorService(ILogger<CalculatorService> logger)
        {
            _logger = logger;
        }
        private static readonly ConcurrentDictionary<string, int> _userTotals = new();

        public override async Task RunningTotal(
            IAsyncStreamReader<NumberRequest> requestStream,
            IServerStreamWriter<TotalReply> responseStream,
            ServerCallContext context)
        {
            await foreach (var request in requestStream.ReadAllAsync())
            {
                var userId = request.UserId;

                _userTotals.AddOrUpdate(userId, request.Number, (id, existing) => existing + request.Number);

                var newTotal = _userTotals[userId];

                await responseStream.WriteAsync(new TotalReply
                {
                    UserId = userId,
                    Total = newTotal
                });
            }
        }
        public override async Task PrimeFactors(NumberRequest request, IServerStreamWriter<NumberReply> responseStream, ServerCallContext context)
        {
            int n = request.Number;
            int divisor = 2;

            while (n > 1)
            {
                if (n % divisor == 0)
                {
                    await responseStream.WriteAsync(new NumberReply { Factor = divisor });
                    n /= divisor;
                    await Task.Delay(500); // simulate streaming delay
                }
                else
                    divisor++;
            }
        }
        public override async Task<NumberReply> SumNumbers(IAsyncStreamReader<NumberRequest> requestStream, ServerCallContext context)
        {
            int total = 0;
            await foreach (var request in requestStream.ReadAllAsync())
                total += request.Number;
            return new NumberReply { Factor = total };
        }
    }
}
