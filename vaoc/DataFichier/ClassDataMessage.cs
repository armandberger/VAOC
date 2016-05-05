using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    class ClassDataMessage
    {
        public int ID_MESSAGE { get; set; }
        public int ID_PARTIE { get; set; }
        public int ID_EMETTEUR { get; set; }
        public int ID_PION_PROPRIETAIRE { get; set; }
        public DateTime DT_DEPART { get; set; }
        public DateTime DT_ARRIVEE { get; set; }
        public string S_MESSAGE { get; set; }
    }
}
