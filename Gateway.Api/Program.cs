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

// Registrer HttpClient-navne internt
builder.Services.AddHttpClient("sent", c =>
    c.BaseAddress = new Uri("http://localhost:5099/"));  // Snippets.Api (1–500)

builder.Services.AddHttpClient("sentapi", c =>
    c.BaseAddress = new Uri("http://localhost:5217/"));  // Sent.Api (501+)

builder.Services.AddHttpClient("deleted", c =>
    c.BaseAddress = new Uri("http://localhost:5021/"));  // Deleted.Api

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --- Aggregator endpoint ---
// GET /all-snippets/search/{term}?source=sent1-500|sent501+|deleted
app.MapGet("/all-snippets/search/{term}", async (
    string term,
    string? source,  // optional query parameter
    IHttpClientFactory http,
    ILogger<Program> log) =>
{
    log.LogInformation("Gateway modtog søgning: '{Term}', source: {Source}", term, source ?? "all");

    var aggregated = new List<AggregatedSnippet>();

    // Brugervenlige navne → interne HttpClient-navne
    var sourceMap = new Dictionary<string, string>
    {
        { "sent1-500", "sent" },
        { "sent501+", "sentapi" },
        { "deleted", "deleted" }
    };

    string[] selectedSources = source?.ToLower() switch
    {
        "sent1-500" => new[] { "sent1-500" },
        "sent501+" => new[] { "sent501+" },
        "deleted" => new[] { "deleted" },
        _ => new[] { "sent1-500", "sent501+", "deleted" } // default: alle
    };

    foreach (var userSource in selectedSources)
    {
        var internalClientName = sourceMap[userSource];
        var client = http.CreateClient(internalClientName);

        try
        {
            log.LogInformation("Sender forespørgsel til {Source}.Api", userSource);

            var partial = await client.GetFromJsonAsync<List<SnippetResult>>(
                $"/snippets/search/{Uri.EscapeDataString(term)}");

            if (partial != null)
            {
                log.LogInformation("Modtog {Count} resultater fra {Source}.Api", partial.Count, userSource);

                foreach (var sn in partial)
                {
                    var words = Regex
                        .Split(sn.SnippetSentence, @"\W+")
                        .Where(w => !string.IsNullOrWhiteSpace(w))
                        .ToArray();

                    var idx = Array.FindIndex(words, w =>
                        string.Equals(w, term, StringComparison.OrdinalIgnoreCase));

                    string snippetContext = idx < 0
                        ? sn.SnippetSentence
                        : string.Join(" ", words.Skip(Math.Max(0, idx - 4)).Take(9));

                    aggregated.Add(new AggregatedSnippet(userSource, sn.Document, snippetContext));
                }
            }
            else
            {
                log.LogWarning("Ingen resultater fra {Source}.Api", userSource);
            }
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            log.LogWarning(ex, "Kald til {Source}.Api fejlede", userSource);
        }
    }

    log.LogInformation("Samlet antal resultater: {Total}", aggregated.Count);

    return Results.Ok(aggregated);
});

app.Run();
