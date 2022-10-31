using Blog;
using Grpc.Core;

var channel = new Channel("localhost", 50052, ChannelCredentials.Insecure);

await channel.ConnectAsync().ContinueWith((task) =>
{
    if (task.Status == TaskStatus.RanToCompletion)
        Console.WriteLine("The client connected successfully");
});

var client = new BlogService.BlogServiceClient(channel);
//var newBlog = CreateBlog(client);
//UpdateBlog(client, newBlog);
//DeleteBlog(client, newBlog);
await ListBlog(client);

channel.ShutdownAsync().Wait();
Console.ReadKey();

Blog.Blog CreateBlog(BlogService.BlogServiceClient client)
{
    var response = client.CreateBlog(new CreateBlogRequest
    {
        Blog = new Blog.Blog
        {
            AuthorId = "Client",
            Title = "New blog!",
            Content = "Hello world, this is a new blog"
        }
    });

    Console.WriteLine($"The new blog {response.Blog.Id} was create!");

    return response.Blog;
}

void ReadBlog(BlogService.BlogServiceClient client)
{
    try
    {
        var response = client.ReadBlog(new ReadBlogRequest { Id = "" });

        Console.WriteLine(response.Blog.ToString());
    }
    catch (RpcException ex)
    {
        Console.WriteLine(ex.Status.Detail);
    }
}

void UpdateBlog(BlogService.BlogServiceClient client, Blog.Blog blog)
{
    try
    {
        blog.AuthorId = "Updated author";
        blog.Title = "Updated title";
        blog.Content = "Updated content";

        var response = client.UpdateBlog(new UpdateBlogRequest
        {
            Blog = blog
        });

        Console.WriteLine(response.Blog.ToString());
    }
    catch (RpcException ex)
    {
        Console.WriteLine(ex.Status.Detail);
    }
}

void DeleteBlog(BlogService.BlogServiceClient client, Blog.Blog blog)
{
    try
    {
        var response = client.DeleteBlog(new DeleteBlogRequest { Id = blog.Id });
        Console.WriteLine($"The blog with if {response.Id} was deleted");
    }
    catch (RpcException ex)
    {
        Console.WriteLine(ex.Status.Detail);
    }
}

async Task ListBlog(BlogService.BlogServiceClient client)
{
    var response = client.ListBlog(new ListBlogRequest());

    while (await response.ResponseStream.MoveNext())
    {
        Console.WriteLine(response.ResponseStream.Current.Blog.Title());
    }
}