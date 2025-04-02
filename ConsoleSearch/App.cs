using System;
using System.Threading.Tasks;

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
                Console.WriteLine($"Search results: {result}");
            }
        }
    }
}
