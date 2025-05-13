using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ConsoleSearch; // Vigtigt: Sørg for at namespace matcher der hvor SearchResult ligger

public class SearchClient
{
    private readonly HttpClient _httpClient;

    public SearchClient()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000") }; // Juster port hvis nødvendigt
    }

    public async Task<SearchResult> SearchAsync(string query)
    {
        var response = await _httpClient.GetAsync($"/api/search?query={Uri.EscapeDataString(query)}");
        response.EnsureSuccessStatusCode(); // Kaster exception ved fejlstatus

        return await response.Content.ReadFromJsonAsync<SearchResult>();
    }
}
