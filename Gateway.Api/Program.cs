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

// Configure HttpClient for the two snippet services with correct ports
builder.Services.AddHttpClient("sent", c =>
    c.BaseAddress = new Uri("http://localhost:5099/")   // Snippets.Api HTTP endpoint
);
builder.Services.AddHttpClient("deleted", c =>
    c.BaseAddress = new Uri("http://localhost:5021/")   // Deleted.Api HTTP endpoint
);

// Add Swagger/OpenAPI for testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger UI only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Health-check endpoint
app.MapGet("/health", () => Results.Ok("Healthy"));

// Aggregator endpoint: GET /all-snippets/search/{term}
app.MapGet("/all-snippets/search/{term}", async (
    string term,
    IHttpClientFactory http,
    ILogger<Program> log) =>
{
    var aggregated = new List<AggregatedSnippet>();

    foreach (var source in new[] { "sent", "deleted" })
    {
        var client = http.CreateClient(source);
        try
        {
            var partial = await client.GetFromJsonAsync<List<SnippetResult>>(
                $"/snippets/search/{Uri.EscapeDataString(term)}"
            );
            if (partial != null)
            {
                foreach (var sn in partial)
                {
                    // Split sentence into words
                    var words = Regex
                        .Split(sn.SnippetSentence, @"\W+")
                        .Where(w => !string.IsNullOrWhiteSpace(w))
                        .ToArray();
                    // Find index of the searched term
                    var idx = Array.FindIndex(words, w =>
                        string.Equals(w, term, StringComparison.OrdinalIgnoreCase)
                    );
                    string snippetContext;
                    if (idx < 0)
                    {
                        // Fallback to entire sentence if term not found
                        snippetContext = sn.SnippetSentence;
                    }
                    else
                    {
                        // Take 4 words before and after
                        var start = Math.Max(0, idx - 4);
                        var end = Math.Min(words.Length, idx + 5);
                        snippetContext = string.Join(
                            " ", words.Skip(start).Take(end - start)
                        );
                    }

                    aggregated.Add(new AggregatedSnippet(
                        source,
                        sn.Document,
                        snippetContext
                    ));
                }
            }
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
        {
            log.LogWarning(ex, "Call to snippet service '{Service}' failed", source);
        }
    }

    return Results.Ok(aggregated);
});

app.Run();
