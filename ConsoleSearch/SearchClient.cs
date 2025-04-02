using System;
using System.Net.Http;
using System.Threading.Tasks;

public class SearchClient
{
    private readonly HttpClient _httpClient;

    public SearchClient()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000") }; // Juster port hvis nødvendigt
    }

    public async Task<string> SearchAsync(string query)
    {
        var response = await _httpClient.GetAsync($"/api/search?query={Uri.EscapeDataString(query)}");
        if (!response.IsSuccessStatusCode)
        {
            return $"Error: {response.StatusCode}";
        }

        return await response.Content.ReadAsStringAsync();
    }
}
