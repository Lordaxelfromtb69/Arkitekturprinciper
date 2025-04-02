using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace LoadBalancer.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class LoadBalancerController : ControllerBase
    {
        private static int _currentIndex = 0;
        private static readonly List<string> _searchApiInstances = new()
        {
            "http://localhost:5167/api/search", // Instans 1
            "http://localhost:5168/api/search"  // Instans 2
        };
        private static readonly object _lock = new();

        private static string GetNextInstance()
        {
            lock (_lock)
            {
                var instance = _searchApiInstances[_currentIndex];
                _currentIndex = (_currentIndex + 1) % _searchApiInstances.Count;
                return instance;
            }
        }

        [HttpGet("{query}")]
        public async Task<IActionResult> Search(string query)
        {
            var apiUrl = $"{GetNextInstance()}?query={query}";
            using var client = new HttpClient();
            var response = await client.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "SearchAPI instance unavailable");

            var result = await response.Content.ReadAsStringAsync();
            return Ok(result);
        }
    }
}
