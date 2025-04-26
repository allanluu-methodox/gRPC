using Grpc.Net.Client;
using gRPC_Server;

var channel = GrpcChannel.ForAddress("http://localhost:7000");
var client = new gRPC_Server.Math.MathClient(channel);

var reply = await client.AddAsync(new AddRequest { A = 10, B = 20 });
Console.WriteLine($"Result from server: {reply.Result}");