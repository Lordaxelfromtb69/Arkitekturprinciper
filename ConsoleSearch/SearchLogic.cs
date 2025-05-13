using System;
using System.Collections.Generic;

namespace ConsoleSearch
{
    public class SearchLogic
    {
        public string Query { get; }
        public int Hits { get; }
        public List<DocumentHit> DocumentHits { get; }
        public List<string> Ignored { get; }
        public TimeSpan TimeUsed { get; }
        public string Timestamp { get; }  // Ny egenskab

        public SearchLogic(string query, int hits, List<DocumentHit> documentHits, List<string> ignored, TimeSpan timeUsed, string timestamp)
        {
            Query = query;
            Hits = hits;
            DocumentHits = documentHits;
            Ignored = ignored;
            TimeUsed = timeUsed;
            Timestamp = timestamp;
        }
    }
}
