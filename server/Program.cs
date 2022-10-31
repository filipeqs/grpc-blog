using Blog;
using Grpc.Core;
using server;

const int Port = 50052;
Server server = null;

try
{
	server = new Server
	{
		Services = { BlogService.BindService(new BlogServiceImpl()) },
		Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
	};

	server.Start();
	Console.WriteLine("The server is listening on the port: " + Port);
	Console.ReadLine();
}
catch (IOException ex)
{
	Console.WriteLine("The serve failed to start: " + ex.Message);
	throw;
}
finally
{
	if (server != null)
		server.ShutdownAsync().Wait();
}