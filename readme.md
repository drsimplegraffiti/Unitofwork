- dotnet new sln -n PocketBook // Create a new solution
- dotnet new classlib -n PocketBook.Domain // holds all the domain models e.g. User, Book, etc.
- dotnet new classlib -n PocketBook.Infrastructure // holds all the infrastructure models e.g. Database, Email, etc.
- dotnet new classlib -n PocketBook.Application // holds all the application models e.g. Services, Repositories, etc.
- dotnet new webapi -n PocketBook.API // holds all the API models e.g. Controllers, etc.



---

##### Simple IHttpFactoryClient
In Controller
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PocketBook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalApiController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public ExternalApiController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://jsonplaceholder.org/posts");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return Ok(await response.Content.ReadAsStringAsync());
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"https://jsonplaceholder.org/posts/{id}");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return Ok(await response.Content.ReadAsStringAsync());
            }
            else
            {
                return BadRequest();
            }
        }


        [HttpGet("github")]
        public async Task<ActionResult> GetGitHub()
        {
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://jsonplaceholder.org");
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            string result = await client.GetStringAsync("/posts");
            return Ok(result);
        }
    }
}
```

Use Named Instances of HttpClientFactory in ASP.NET Core
In Program.cs
```csharp

services.AddHttpClient();
services.AddHttpClient("github", c =>
{
    c.BaseAddress = new Uri("https://api.github.com/");
    c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
    c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
});
    
```



In Controller
```csharp
public class NamedClientController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public NamedClientController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

     [HttpGet]
    public async Task<ActionResult> Get()
    {
        var client = _httpClientFactory.CreateClient("github");
        string result = await client.GetStringAsync("/");
        return Ok(result);
    }
}
```


Use Typed Clients in ASP.NET Core
In ExternalApiClient.cs
```csharp
public class ExternalApiClient
{
    public HttpClient Client { get; private set; }

    public ExternalApiClient(HttpClient client)
    {
        client.BaseAddress = new Uri("https://api.github.com/");
        client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
        client.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
        Client = client;
    }

}
```

In Startup.cs
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpClient<ExternalApiClient>();
}
```

In Controller
```csharp
public class TypedClientController : Controller
{
    private readonly ExternalApiClient _client;

    public TypedClientController(ExternalApiClient client)
    {
        _client = client;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        string result = await _client.Client.GetStringAsync("/");
        return Ok(result);
    }
}
```


Use interfaces to register HttpClientFactory in ASP.NET Core
In IExternalApiClient.cs
```csharp
public interface IExternalApiClient
{
    Task<string> Get();
    Task<string> Get(string path);
    Task<string> Get(string path, string token);
    Task<string> Post(string path, object data);
    Task<string> Post(string path, object data, string token);
    Task<string> Put(string path, object data);
    Task<string> Put(string path, object data, string token);
}
```

In ExternalApiClient.cs
```csharp
public class ExternalApiClient : IExternalApiClient
{
    private readonly HttpClient _client;

    public ExternalApiClient(HttpClient client)
    {
        client.BaseAddress = new Uri("https://api.github.com/");
        client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
        client.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
        _client = client;
    }

    public async Task<string> Get()
    {
        return await _client.GetStringAsync("/");
    }

    public async Task<string> Get(string path)
    {
        return await _client.GetStringAsync(path);
    }

    public async Task<string> Get(string path, string token)
    {
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        return await _client.GetStringAsync(path);
    }

    public async Task<string> Post(string path, object data)
    {
        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(path, content);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> Post(string path, object data, string token)
    {
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(path, content);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> Put(string path, object data)
    {
        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync(path, content);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> Put(string path, object data, string token)
    {
        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync(path, content);
        return await response.Content.ReadAsStringAsync();
    }
}
```

In Startup.cs
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpClient<IExternalApiClient, ExternalApiClient>();
}
```

In Controller
```csharp
public class TypedClientController : Controller
{
    private readonly IExternalApiClient _client;

    public TypedClientController(IExternalApiClient client)
    {
        _client = client;
    }

    [HttpGet]
    public async Task<ActionResult> Get()
    {
        string result = await _client.Get();
        return Ok(result);
    }

    [HttpGet("{path}")]
    public async Task<ActionResult> Get(string path)
    {
        string result = await _client.Get(path);
        return Ok(result);
    }

    [HttpGet("{path}/{token}")]
    public async Task<ActionResult> Get(string path, string token)
    {
        string result = await _client.Get(path, token);
        return Ok(result);
    }

    [HttpPost("{path}")]
    public async Task<ActionResult> Post(string path, [FromBody] object data)
    {
        string result = await _client.Post(path, data);
        return Ok(result);
    }

    [HttpPost("{path}/{token}")]
    public async Task<ActionResult> Post(string path, [FromBody] object data, string token)
    {
        string result = await _client.Post(path, data, token);
        return Ok(result);
    }

    [HttpPut("{path}")]
    public async Task<ActionResult> Put(string path, [FromBody] object data)
    {
        string result = await _client.Put(path, data);
        return Ok(result);
    }

    [HttpPut("{path}/{token}")]
    public async Task<ActionResult> Put(string path, [FromBody] object data, string token)
    {
        string result = await _client.Put(path, data, token);
        return Ok(result);
    }


}
```