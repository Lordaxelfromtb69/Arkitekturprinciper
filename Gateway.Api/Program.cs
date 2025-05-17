using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// DTO-record for individual snippets
record SnippetResult(string Document, string SnippetSentence);

// DTO-record for aggregated snippets with source info
record AggregatedSnippet(string Source, string Document, string SnippetSentence);

namespace Gateway.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1) Register HttpClient instances for each back-end service
            builder.Services.AddHttpClient("sent", client =>
                client.BaseAddress = new Uri("http://localhost:5099/") // Snippets.Api HTTP
            );
            builder.Services.AddHttpClient("deleted", client =>
                client.BaseAddress = new Uri("http://localhost:5021/") // Deleted.Api HTTP
            );

            // 2) Add Swagger/OpenAPI for testing
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // 3) Swagger UI only in Development
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // 4) Health-check endpoint
            app.MapGet("/health", () => Results.Ok("Healthy"));

            // 5) Aggregator endpoint: combine sent and deleted snippets
            app.MapGet("/all-snippets/search/{term}", async (string term, IHttpClientFactory http, ILogger<Program> log) =>
            {
                var aggregated = new List<AggregatedSnippet>();

                foreach (var source in new[] { "sent", "deleted" })
                {
                    var client = http.CreateClient(source);
                    try
                    {
                        var partial = await client.GetFromJsonAsync<List<SnippetResult>>("/snippets/search/" + Uri.EscapeDataString(term));
                        if (partial != null)
                        {
                            foreach (var sn in partial)
                            {
                                aggregated.Add(new AggregatedSnippet(source, sn.Document, sn.SnippetSentence));
                            }
                        }
                    }
                    catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
                    {
                        log.LogWarning(ex, "Call to {Source} failed", source);
                    }
                }

                return Results.Ok(aggregated);
            });

            app.Run();
        }
    }
}
