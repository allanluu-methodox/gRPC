using Grpc.Core;
using Grpc.Net.Client;
using gRPC_Server;

GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:7000");
gRPC_Server.Math.MathClient mathClient = new gRPC_Server.Math.MathClient(channel);
gRPC_Server.Calculator.CalculatorClient calculatorClient = new gRPC_Server.Calculator.CalculatorClient(channel);

//// Request once
//AddReply mathReply = await mathClient.AddAsync(new AddRequest { A = 10, B = 20 });
//Console.WriteLine($"Result from server: {mathReply.Result} ({mathReply.RequestCount})");

//// Request twice
//AddReply reply2 = await client.AddAsync(new AddRequest { A = 10, B = 20 });
//Console.WriteLine($"Result from server: {reply.Result} ({reply.RequestCount})");

using var call = calculatorClient.RunningTotal();

string userId = "allan123";
int[] numbers = { 2, 4, -1, 5 };

var readTask = Task.Run(async () =>
{
    await foreach (var reply in call.ResponseStream.ReadAllAsync())
    {
        Console.WriteLine($"[{reply.UserId}] Running Total: {reply.Total}");
    }
});

foreach (var num in numbers)
{
    await call.RequestStream.WriteAsync(new NumberRequest
    {
        UserId = userId,
        Number = num
    });
    await Task.Delay(200);
}

await call.RequestStream.CompleteAsync();
await readTask;