using System.Net.Http.Json;

public class SearchService
{
    private readonly HttpClient _httpClient;

    public SearchService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> Search(string query)
    {
        var response = await _httpClient.GetAsync($"http://localhost:5000/api/search/{query}");
        return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : "Error: No result.";
    }
}
