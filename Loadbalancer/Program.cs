using System.Net.Http.Json;
using LoadBalancer.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Liste over Inbox instans-URL'er
var inboxInstances = new[]
{
    "http://localhost:6001", // Inbox instance 1
    "http://localhost:6002"  // Inbox instance 2
};

int roundRobinIndex = 0;

app.MapGet("/inbox/search/{term}", async (
    string term,
    IHttpClientFactory http,
    ILogger<Program> log) =>
{
    // Vælg instans baseret på round-robin
    var instance = inboxInstances[roundRobinIndex % inboxInstances.Length];
    roundRobinIndex++;

    log.LogInformation("Forwarding request to {Instance}", instance);

    var client = http.CreateClient();
    try
    {
        var results = await client.GetFromJsonAsync<List<SnippetResult>>(
            $"{instance}/snippets/search/{Uri.EscapeDataString(term)}"
        );

        return results is not null ? Results.Ok(results) : Results.NoContent();
    }
    catch (Exception ex)
    {
        log.LogError(ex, "Failed to contact Inbox API instance: {Instance}", instance);
        return Results.Problem("Inbox instans fejlede.");
    }
});

app.Run();
