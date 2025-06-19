using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Shared.Model;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:80");

builder.Services.Configure<PathOptions>(
    builder.Configuration.GetSection("Paths")
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

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

    var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories)
                         .Where(f =>
                         {
                             var name = Path.GetFileNameWithoutExtension(f);
                             return int.TryParse(name, out int num) && num >= 501;
                         })
                         .OrderBy(f => f);

    return Results.Ok(SnippetSearch(term, files, folder));
});

app.Run();

static List<object> SnippetSearch(string term, IEnumerable<string> files, string folder)
{
    var matches = new List<object>();

    foreach (var file in files)
    {
        var text = File.ReadAllText(file);
        var words = Regex.Split(text, @"\W+").Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();

        for (int i = 0; i < words.Length; i++)
        {
            if (string.Equals(words[i], term, StringComparison.OrdinalIgnoreCase))
            {
                var start = Math.Max(0, i - 4);
                var end = Math.Min(words.Length, i + 5);
                var snippet = string.Join(" ", words.Skip(start).Take(end - start));

                matches.Add(new
                {
                    Document = Path.GetRelativePath(folder, file),
                    SnippetSentence = snippet
                });
                break;
            }
        }
    }

    return matches;
}
