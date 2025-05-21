using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Shared.Model;    // PathOptions

var builder = WebApplication.CreateBuilder(args);

// Bind Paths‐sektionen til PathOptions
builder.Services.Configure<PathOptions>(
    builder.Configuration.GetSection("Paths")
);

// Tilføj Swagger/OpenAPI (valgfrit)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Kun Swagger i Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --- Søge‐endpoint: find snippet i alle filer under den angivne sti ---
app.MapGet("/snippets/search/{term}", (
    string term,
    IOptions<PathOptions> opts,
    ILogger<Program> logger) =>
{
    var folder = opts.Value.Folder;

    if (!Directory.Exists(folder))
    {
        logger.LogWarning("Forespørgsel modtaget, men folder eksisterer ikke: {Folder}", folder);
        return Results.NotFound($"Folder not found: {folder}");
    }

    logger.LogInformation("Søgeforespørgsel modtaget: '{Term}' i folder: {Folder}", term, folder);

    var matches = new List<object>();

    foreach (var filePath in Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories))
    {
        var text = File.ReadAllText(filePath);
        var sentences = Regex.Split(text, @"(?<=[\.!\?])\s+");

        foreach (var sentence in sentences)
        {
            if (Regex.IsMatch(sentence, $@"\b{Regex.Escape(term)}\b", RegexOptions.IgnoreCase))
            {
                matches.Add(new
                {
                    Document = Path.GetRelativePath(folder, filePath),
                    SnippetSentence = sentence.Trim()
                });
                break;
            }
        }
    }

    logger.LogInformation("Søgeord '{Term}' gav {Count} resultat(er)", term, matches.Count);

    return Results.Ok(matches);
});

app.Run();
