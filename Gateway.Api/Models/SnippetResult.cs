namespace Gateway.Api.Models
{
    public record SnippetResult(string Document, string SnippetSentence);
    public record AggregatedSnippet(string Source, string Document, string SnippetSentence);
}