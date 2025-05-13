using System;
using System.Collections.Generic;

namespace ConsoleSearch
{
    public class SearchResult
    {
        public string[] Query { get; }
        public int Hits { get; }
        public List<DocumentHit> DocumentHits { get; }
        public List<string> Ignored { get; }
        public TimeSpan TimeUsed { get; }

        // Brug denne hvis du allerede har en string[] query
        public SearchResult(string[] query, int hits, List<DocumentHit> documents, List<string> ignored, TimeSpan timeUsed)
        {
            Query = query;
            Hits = hits;
            DocumentHits = documents;
            Ignored = ignored;
            TimeUsed = timeUsed;
        }

        // Brug denne hvis du har query som én samlet streng
        public SearchResult(string query, int hits, List<DocumentHit> documents, List<string> ignored, TimeSpan timeUsed)
        {
            Query = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Hits = hits;
            DocumentHits = documents;
            Ignored = ignored;
            TimeUsed = timeUsed;
        }
    }
}
