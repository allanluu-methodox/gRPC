using Grpc.Core;
using Grpc.Net.Client;
using gRPC_Server;

GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:7000");
gRPC_Server.Math.MathClient mathClient = new gRPC_Server.Math.MathClient(channel);
gRPC_Server.Calculator.CalculatorClient calculatorClient = new gRPC_Server.Calculator.CalculatorClient(channel);

Console.WriteLine($"***********************************************************************");
Console.WriteLine($"Peforming Unary Call: one client request message, one server response");
Console.WriteLine($"***********************************************************************\n");
// Request once
AddReply mathReply = await mathClient.AddAsync(new AddRequest { A = 10, B = 20 });
Console.WriteLine($"Result from server: {mathReply.Result} ({mathReply.RequestCount})");

// Request twice
AddReply reply2 = await mathClient.AddAsync(new AddRequest { A = 15, B = 6 });
Console.WriteLine($"Result from server: {reply2.Result} ({reply2.RequestCount})");

Console.WriteLine($"\n*******************************************************************************************************");
Console.WriteLine($"Peforming Server Streaming Call: one client request message, server stream message until termination");
Console.WriteLine($"*******************************************************************************************************\n");

int primeFactor = 30;
Console.WriteLine($"Gathering prime factors for the number: {primeFactor}");
var response = calculatorClient.PrimeFactors(new NumberRequest { Number = primeFactor });
await foreach (var message in response.ResponseStream.ReadAllAsync())
    Console.WriteLine($"Prime factor: {message.Factor}");

Console.WriteLine($"\n*******************************************************************************************************");
Console.WriteLine($"Peforming Client Streaming Call: client stream message until termination, one response message from server");
Console.WriteLine($"*******************************************************************************************************\n");

Console.WriteLine($"Sending numbers to server to calculate sum: ");
var sumCall = calculatorClient.SumNumbers();
for (int i = 1; i <= 5; i++)
{
    await sumCall.RequestStream.WriteAsync(new NumberRequest { Number = i });
    Console.WriteLine($"Number \"{i}\" has been sent.");
}
await sumCall.RequestStream.CompleteAsync();

var sumResult = await sumCall.ResponseAsync;
Console.WriteLine($"Sum received from server: {sumResult.Factor}\n");

Console.WriteLine($"*****************************************************************************************");
Console.WriteLine($"Peforming Bi-Directional Streaming Call: one client request message, one server response");
Console.WriteLine($"*****************************************************************************************\n");
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

Console.WriteLine("\nPress Enter to exit...");
Console.ReadLine();