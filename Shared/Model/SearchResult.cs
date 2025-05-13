using System;
using System.Collections.Generic;

namespace Shared.Model
{
    public class SearchResult
    {
        public string Query { get; set; }
        public int Hits { get; set; }
        public List<DocumentHit> DocumentHits { get; set; }
        public List<string> Ignored { get; set; }
        public TimeSpan TimeUsed { get; set; }
    }
}
