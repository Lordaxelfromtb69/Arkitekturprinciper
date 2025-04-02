using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model
{
    public class Config
    {
        public bool CaseSensitive { get; set; } = true;
        public bool ViewTimeStamps { get; set; } = true; // Denne property bestemmer om tidsstempel skal vises
    }
}


