using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Shared.Model; // PathOptions

var builder = WebApplication.CreateBuilder(args);

// Bind stier fra appsettings.json
builder.Services.Configure<PathOptions>(
    builder.Configuration.GetSection("Paths")
);

// Swagger til udvikling
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// === Endpoint: søg efter ord i dokumenter 1-500 ===
app.MapGet("/snippets/search/{term}", (
    string term,
    IOptions<PathOptions> opts,
    ILogger<Program> logger) =>
{
    var folder = opts.Value.Folder;

    if (!Directory.Exists(folder))
    {
        logger.LogWarning("Folder not found: {Folder}", folder);
        return Results.NotFound("Folder not found.");
    }

    logger.LogInformation("Søger efter '{Term}' i filer 1-500 i folder {Folder}", term, folder);

    // Filtrer filer med navne som tal fra 1-500
    var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories)
        .Where(path =>
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            return int.TryParse(fileName, out int id) && id >= 1 && id <= 500;
        });

    return Results.Ok(SnippetSearch(term, files, folder));
});

app.Run();

// === Snippet-logik ===
static List<object> SnippetSearch(string term, IEnumerable<string> files, string folder)
{
    var matches = new List<object>();

    foreach (var file in files)
    {
        var text = File.ReadAllText(file);
        var words = Regex.Split(text, @"\W+")
                         .Where(w => !string.IsNullOrWhiteSpace(w))
                         .ToArray();

        for (int i = 0; i < words.Length; i++)
        {
            if (string.Equals(words[i], term, StringComparison.OrdinalIgnoreCase))
            {
                int start = Math.Max(0, i - 4);
                int end = Math.Min(words.Length, i + 5);
                string snippet = string.Join(" ", words.Skip(start).Take(end - start));

                matches.Add(new
                {
                    Document = Path.GetRelativePath(folder, file),
                    SnippetSentence = snippet
                });

                break; // kun ét match per dokument
            }
        }
    }

    return matches;
}
