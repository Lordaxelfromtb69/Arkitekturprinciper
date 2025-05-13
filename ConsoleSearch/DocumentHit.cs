using System.Collections.Generic;
using Shared.Model;

namespace ConsoleSearch
{
    public class DocumentHit
    {
        public BEDocument Document { get; set; }

        // Antal søgeord som blev fundet i dokumentet
        public int NoOfHits { get; set; }

        // Liste over søgeord som ikke blev fundet i netop dette dokument
        public List<string> Missing { get; set; }

        // NYT: Kort uddrag fra dokumentet med søgeordene fremhævet
        public string Snippet { get; set; }

        public DocumentHit(BEDocument document, int noOfHits, List<string> missing, string snippet)
        {
            Document = document;
            NoOfHits = noOfHits;
            Missing = missing;
            Snippet = snippet;
        }

        public override string ToString()
        {
            return $"{Document.Title} ({NoOfHits} hits)\nSnippet: {Snippet}";
        }
    }
}
