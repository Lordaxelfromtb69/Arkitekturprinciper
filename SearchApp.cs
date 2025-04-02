using System;
using Shared.Model;

namespace ConsoleSearch
{
    class SearchApp
    {
        static void Main(string[] args)
        {
            new SearchApp().Run(); // Kald Run-metoden fra App-klassen
        }

        private void Run()
        {
            throw new NotImplementedException();
        }
    }

    internal class WorkApp
    {
        public WorkApp()
        {
            // Initialisering af nødvendige komponenter kan gøres her
        }

        internal void Run()
        {
            var searchLogic = new SearchLogic();
            var config = new Config(); // Opretter en instans af Config

            while (true)
            {
                Console.WriteLine("Enter search terms or a command ('/casesensitive=on' or '/casesensitive=off'). Type 'q' to quit.");
                var input = Console.ReadLine();

                // Kommandoer for at slå case-sensitiv søgning til eller fra
                if (input.StartsWith("/casesensitive="))
                {
                    var caseSensitiveValue = input.Split('=')[1];
                    if (caseSensitiveValue == "on")
                    {
                        config.CaseSensitive = true;
                        Console.WriteLine("Case-sensitive search is now ON.");
                    }
                    else if (caseSensitiveValue == "off")
                    {
                        config.CaseSensitive = false;
                        Console.WriteLine("Case-sensitive search is now OFF.");
                    }
                    continue;
                }

                // Kommando for at vælge om tidsstempel skal vises
                if (input.StartsWith("/timestamp="))
                {
                    var timestampValue = input.Split('=')[1];
                    if (timestampValue == "on")
                    {
                        config.ViewTimeStamps = true;
                        Console.WriteLine("Timestamp will be shown in results.");
                    }
                    else if (timestampValue == "off")
                    {
                        config.ViewTimeStamps = false;
                        Console.WriteLine("Timestamp will NOT be shown in results.");
                    }
                    continue;
                }

                if (input == "q")
                    break;

                // Søgning
                var query = input.Split(' ');

                var result = searchLogic.Search(query, config);

                // Udskriv resultaterne
                string output = $"Documents: {result.Hits}. Time: {result.TimeUsed.TotalSeconds}s";

                // Hvis timestamp vises
                if (config.ViewTimeStamps)
                {
                    output += $" (Indexed at: {result.Timestamp})"; // Tilføj timestamp
                }

                Console.WriteLine(output);
            }
        }
    }
}
