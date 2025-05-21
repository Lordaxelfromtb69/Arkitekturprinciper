using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Gateway.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure HttpClient for the two snippet services
builder.Services.AddHttpClient("sent", c =>
    c.BaseAddress = new Uri("http://localhost:5099/")
);
builder.Services.AddHttpClient("deleted", c =>
    c.BaseAddress = new Uri("http://localhost:5021/")
);

// Add Swagger/OpenAPI for testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok("Healthy"));

// --- Aggregator endpoint with optional source ---
app.MapGet("/all-snippets/search/{term}", async (
    string term,
    string? source,  // optional query parameter: sent, deleted, both/null
    IHttpClientFactory http,
    ILogger<Program> log) =>
{
    log.LogInformation("Gateway modtog søgeforespørgsel: '{Term}', source={Source}", term, source ?? "both");

    var aggregated = new List<AggregatedSnippet>();

    // Decide which services to call
    var selectedSources = source?.ToLower() switch
    {
        "sent" => new[] { "sent" },
        "deleted" => new[] { "deleted" },
        _ => new[] { "sent", "deleted" }  // default: both
    };

    foreach (var src in selectedSources)
    {
        var client = http.CreateClient(src);
        try
        {
            log.LogInformation("Sender forespørgsel til {Source}.Api", src);

            var partial = await client.GetFromJsonAsync<List<SnippetResult>>(
                $"/snippets/search/{Uri.EscapeDataString(term)}"
            );

            if (partial != null)
            {
                log.LogInformation("Modtog {Count} resultater fra {Source}.Api", partial.Count, src);

                foreach (var sn in partial)
                {
                    var words = Regex
                        .Split(sn.SnippetSentence, @"\W+")
                        .Where(w => !string.IsNullOrWhiteSpace(w))
                        .ToArray();

                    var idx = Array.FindIndex(words, w =>
                        string.Equals(w, term, StringComparison.OrdinalIgnoreCase)
                    );

                    string snippetContext = idx < 0
                        ? sn.SnippetSentence
                        : string.Join(" ", words.Skip(Math.Max(0, idx - 4)).Take(9));

                    aggregated.Add(new AggregatedSnippet(src, sn.Document, snippetContext));
                }
            }
            else
            {
                log.LogWarning("Ingen data modtaget fra {Source}.Api", src);
            }
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
        {
            log.LogWarning(ex, "Kald til {Source}.Api fejlede", src);
        }
    }

    log.LogInformation("Samlet antal resultater: {Total}", aggregated.Count);

    return Results.Ok(aggregated);
});

app.Run();
