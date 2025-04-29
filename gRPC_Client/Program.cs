using Grpc.Net.Client;
using gRPC_Server;

GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:7000");
gRPC_Server.Math.MathClient client = new gRPC_Server.Math.MathClient(channel);

// Request once
AddReply reply = await client.AddAsync(new AddRequest { A = 10, B = 20 });
Console.WriteLine($"Result from server: {reply.Result} ({reply.RequestCount})");

// Request twice
AddReply reply2 = await client.AddAsync(new AddRequest { A = 10, B = 20 });
Console.WriteLine($"Result from server: {reply.Result} ({reply.RequestCount})");