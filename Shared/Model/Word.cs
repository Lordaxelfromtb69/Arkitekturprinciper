using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model
{
    // Shared/Models/Word.cs   (allerede har du denne)
    public class Word
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        // … andre kolonner …
    }

    // Shared/Models/Occurrence.cs
    public class Occurrence
    {
        public int WordId { get; set; }
        public int DocumentId { get; set; }
        public int Position { get; set; }   // nul-baseret ord-indeks i dokumentet
    }

    // Shared/Models/Document.cs
    public class Document
    {
        public int Id { get; set; }
        public string FilePath { get; set; } = null!;  // sti til tekst-filen
                                                       // evt. andre kolonner, som f.eks. Title, etc.
    }

}