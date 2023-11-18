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