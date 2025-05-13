using System;
using System.Threading.Tasks;
using Shared.Model;

namespace ConsoleSearch
{
    internal class WorkApp
    {
        private readonly SearchClient _searchClient;

        public WorkApp()
        {
            _searchClient = new SearchClient();
        }

        internal async Task Run()
        {
            while (true)
            {
                Console.WriteLine("Enter search terms or type 'q' to quit:");
                var input = Console.ReadLine();

                if (input == "q") break;

                var result = await _searchClient.SearchAsync(input);

                Console.WriteLine($"Documents: {result.Hits}. Time: {result.TimeUsed.TotalSeconds}s");

                foreach (var hit in result.DocumentHits)
                {
                    Console.WriteLine($"[{hit.NoOfHits} hits] {hit.Document.Name}");
                    Console.WriteLine($"Snippet: {hit.Snippet}");
                    Console.WriteLine("----------");
                }

                if (result.Ignored.Count > 0)
                {
                    Console.WriteLine("Ignored terms: " + string.Join(", ", result.Ignored));
                }
            }
        }
    }
}
