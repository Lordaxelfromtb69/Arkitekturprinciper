using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using Shared.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<PathOptions>(
    builder.Configuration.GetSection("Paths")
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

    logger.LogInformation("Inbox.Api modtog søgeord: '{Term}'", term);

    var matches = new List<object>();
    var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories);

    foreach (var file in files)
    {
        var text = File.ReadAllText(file);
        var words = Regex.Split(text, @"\W+").Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();

        for (int i = 0; i < words.Length; i++)
        {
            if (string.Equals(words[i], term, StringComparison.OrdinalIgnoreCase))
            {
                int start = Math.Max(0, i - 4);
                int end = Math.Min(words.Length, i + 5);
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

    logger.LogInformation("Søgeord gav {Count} resultater", matches.Count);

    return Results.Ok(matches);
});

app.Run();
