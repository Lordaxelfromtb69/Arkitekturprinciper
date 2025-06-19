using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using Gateway.Api.Models;
using Microsoft.Extensions.Caching.StackExchangeRedis;


var builder = WebApplication.CreateBuilder(args);

// Registrer HttpClient-navne internt
builder.Services.AddHttpClient("sent", c =>
    c.BaseAddress = new Uri("http://sent1")); // searchengine-sent1

builder.Services.AddHttpClient("sentapi", c =>
    c.BaseAddress = new Uri("http://sent2")); // searchengine-sent2

builder.Services.AddHttpClient("deleted", c =>
    c.BaseAddress = new Uri("http://deleted")); // searchengine-delete


// Redis caching, for at køre redis skal du have en Redis-server kørende brug denne kommando i terminalen: docker run -d --name redis -p 6379:6379 redis

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "redis:6379"; // korrekt netværksnavn indenfor docker-compose
});


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();


// --- Aggregator endpoint ---
// GET /all-snippets/search/{term}?source=sent1-500|sent501+|deleted
app.MapGet("/all-snippets/search/{term}", async (
    string term,
    string? source,
    IHttpClientFactory http,
    ILogger<Program> log,
    IDistributedCache cache) =>
{
    var cacheKey = $"{term}:{source ?? "all"}";
    var cached = await cache.GetStringAsync(cacheKey);
    if (cached != null)
    {
        log.LogInformation("Cache hit for '{CacheKey}'", cacheKey);
        return Results.Content(cached, "application/json");
    }

    log.LogInformation("Gateway modtog søgning: '{Term}', source: {Source}", term, source ?? "all");
    var aggregated = new List<AggregatedSnippet>();

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
        _ => new[] { "sent1-500", "sent501+", "deleted" }
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

    var json = System.Text.Json.JsonSerializer.Serialize(aggregated);
    await cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
    });

    return Results.Content(json, "application/json");
});

app.Run();
