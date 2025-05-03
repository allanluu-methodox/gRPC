using System.Collections.Concurrent;
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
    }
}
