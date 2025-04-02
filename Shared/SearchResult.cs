using ConsoleSearch;
using System.Collections.Generic;
using System;

namespace Shared.Model
{
    public class SearchResult
    {
        public string Query { get; set; }
        public int TotalHits { get; set; }
        public List<DocumentHit> DocumentHits { get; set; }
        public List<string> Ignored { get; set; }
        public TimeSpan TimeUsed { get; set; }
        public string Timestamp { get; set; }

        public SearchResult(string query, int totalHits, List<DocumentHit> documentHits, List<string> ignored, TimeSpan timeUsed, string timestamp)
        {
            Query = query;
            TotalHits = totalHits;
            DocumentHits = documentHits;
            Ignored = ignored;
            TimeUsed = timeUsed;
            Timestamp = timestamp;
        }
    }
}
