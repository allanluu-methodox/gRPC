using Grpc.Core;
using gRPC_Server; // The generated namespace

namespace gRPC_Server.Services
{
    public class MathService : Math.MathBase // Inherit from the generated base class
    {
        private readonly ILogger<MathService> _logger;
        public MathService(ILogger<MathService> logger)
        {
            _logger = logger;
        }

        public uint RequestCount = 0;

        // Override the Add method defined in the generated MathBase class
        public override Task<AddReply> Add(AddRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Adding {A} + {B}", request.A, request.B);
            RequestCount++;

            return Task.FromResult(new AddReply
            {
                Result = request.A + request.B,
                RequestCount = RequestCount
            });
        }
    }
}
