using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Shared.Model;

namespace SearchWebApp.Services
{
    public class SearchService
    {
        private readonly HttpClient _httpClient;

        public SearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SearchResult?> SearchAsync(string query)
        {
            return await _httpClient.GetFromJsonAsync<SearchResult>($"api/search?query={query}");
        }
    }
}
