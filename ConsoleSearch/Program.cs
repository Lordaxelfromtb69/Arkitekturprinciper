using System;
using System.Threading.Tasks;

namespace ConsoleSearch
{
    class Program
    {
        static async Task Main(string[] args) // Ændret til async
        {
            await new WorkApp().Run(); // Kør asynkront
        }
    }
}
