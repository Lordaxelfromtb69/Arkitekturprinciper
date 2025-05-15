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
app.MapGet("/snippets/search/{term}", (string term, IOptions<PathOptions> opts) =>
{
    var folder = opts.Value.Folder;
    if (!Directory.Exists(folder))
        return Results.NotFound($"Folder not found: {folder}");

    var matches = new List<object>();

    // Scan alle filer inklusive undermapper
    foreach (var filePath in Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories))
    {
        var text = File.ReadAllText(filePath);

        // Opdel i sætninger: split på ., ! eller ? efterfulgt af whitespace
        var sentences = Regex.Split(text, @"(?<=[\.!\?])\s+");

        foreach (var sentence in sentences)
        {
            // Tjek om sætningen indeholder term som helt ord (case-insensitive)
            if (Regex.IsMatch(sentence, $@"\b{Regex.Escape(term)}\b", RegexOptions.IgnoreCase))
            {
                matches.Add(new
                {
                    Document = Path.GetRelativePath(folder, filePath),
                    SnippetSentence = sentence.Trim()
                });
                break;  // kun første match per dokument
            }
        }
    }

    return Results.Ok(matches);
});

app.Run();
