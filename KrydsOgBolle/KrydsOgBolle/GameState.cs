using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrydsOgBolle
{
    public class GameState : TableEntity
    {
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public string BoardRow1 { get; set; }
        public string BoardRow2 { get; set; }
        public string BoardRow3 { get; set; }
    }
}
