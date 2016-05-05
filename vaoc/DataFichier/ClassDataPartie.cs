using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    class ClassDataPartie
    {
        public int ID_PARTIE { get; set; }
        public int ID_JEU { get; set; }
        public string S_NOM { get; set; }
        public int I_TOUR { get; set; }
        public DateTime DT_TOUR { get; set; }
        public int I_PHASE { get; set; }
        public DateTime DT_CREATION { get; set; }
        public DateTime DT_MISEAJOUR { get; set; }
        public int H_JOUR { get; set; }
        public int H_NUIT { get; set; }
        public string S_REPERTOIRE { get; set; }
        public bool FL_MISEAJOUR { get; set; }
        public bool FL_DEMARRAGE { get; set; }
        public int I_NB_CARTE_X { get; set; }
        public int I_NB_CARTE_Y { get; set; }
        public int I_NB_CARTE_ZOOM_X { get; set; }
        public int I_NB_CARTE_ZOOM_Y { get; set; }
        public decimal D_MULT_ZOOM_X { get; set; }
        public decimal D_MULT_ZOOM_Y { get; set; }
        public int I_LARGEUR_CARTE_ZOOM { get; set; }
        public int I_HAUTEUR_CARTE_ZOOM { get; set; }
        public int I_ECHELLE { get; set; }

        public string description
        {
            get
            {
                return string.Format("{0}({1})", S_NOM, ID_PARTIE);
            }
        }
    }
}
