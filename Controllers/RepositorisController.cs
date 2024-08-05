using fnxWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Text.Json;
using System.IO;
//using Microsoft.AspNetCore.Cors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace fnxWebAPI.Controllers
{
    //[EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class RepositorisController : ControllerBase
    {
        private Root root;

        [Authorize]
        [HttpGet]
        public async Task<List<Repository>> GetAsync(string searchword)
        {
            try
            {
                List<Repository> repositoris = new List<Repository>();
                var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("githubRepositorySearch", "1.0"));
                client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/search/repositories?q={searchword}");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                var root = JsonSerializer.Deserialize<Root>(result);                  

                foreach (var obj in root.items)
                {
                    Repository repository = new Repository()
                    {
                        avatarUrl = obj.owner?.avatar_url,
                        name = obj.name,
                        bookmarkUrl = obj.url,
                        bookmark = obj.name
                    };
                    repositoris.Add(repository);
                }
                return repositoris;
            }
            catch (Exception ex)
            {

                // Path to the JSON file. Adjust the path based on where the file is located.
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Split("bin")[0], "imageerrorresult.json");

                // Read the JSON file content
                var result = System.IO.File.ReadAllText(filePath);
                var root = JsonSerializer.Deserialize<Root>(result);

                List<Repository> repositories = new List<Repository>();
                foreach (var obj in root.items)
                {                   
                    Repository repository = new Repository()
                    {
                        avatarUrl = obj.owner?.avatar_url,
                        name = obj.name,
                        bookmarkUrl = obj.url,
                        bookmark = obj.name
                    };
                    repositories.Add(repository);
                }
                return repositories;
            }
        }

    }
}
