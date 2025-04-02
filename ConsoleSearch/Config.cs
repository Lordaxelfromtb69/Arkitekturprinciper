using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Model;
namespace ConsoleSearch
{
    public class Config
    {
        public bool CaseSensitive { get; set; } = true;
        public bool ViewTimeStamps { get; set; } = true; // Sørg for, at dette er korrekt stavet
    }
}