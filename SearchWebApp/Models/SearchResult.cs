namespace SearchWebApp.Models
{
    public class SearchResult
    {
        public List<DocumentHit> Documents { get; set; } = new();
    }

    public class DocumentHit
    {
        public string Title { get; set; }
        public int NoOfHits { get; set; }
    }
}
