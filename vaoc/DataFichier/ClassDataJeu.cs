using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    class ClassDataJeu
    {
        public int ID_JEU { get; set; }
        public string S_NOM { get; set; }
        public int I_NOMBRE_TOURS { get; set; }
        public int I_NOMBRE_PHASES { get; set; }
        public DateTime DT_INITIALE { get; set; }
        public int I_HEURE_INITIALE { get; set; }
        public int I_LEVER_DU_SOLEIL { get; set; }
        public int I_COUCHER_DU_SOLEIL { get; set; }

        public string description
        {
            get
            {
                return string.Format("{0}({1})", S_NOM, ID_JEU);
            }
        }
    }
}
