using System;
using System.Collections.Generic;

namespace SearchAPI
{
    public class SearchResult
    {
        public SearchResult(String[] query, int hits, List<DocumentHit> documents, List<string> ignored, TimeSpan timeUsed)
        {
            Query = query;
            Hits = hits;
            DocumentHits = documents;
            Ignored = ignored;
            TimeUsed = timeUsed;
        }

        public String[] Query { get; }

        public int Hits { get; }

        public List<DocumentHit> DocumentHits { get; }

        public List<string> Ignored { get; }

        public TimeSpan TimeUsed { get; }
    }
}
