using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    class ClassDataUtilisateur
    {
        public int ID_UTILISATEUR { get; set; }
        public string S_LOGIN { get; set; }
        public string S_COURRIEL { get; set; }
        public DateTime DT_CREATION { get; set; }
        public DateTime DT_DERNIERECONNEXION { get; set; }
        public string S_NOM { get; set; }
        public string S_PRENOM { get; set; }

        /// <summary>
        /// Nombre de tours ou un jour n'a pas émis d'ordre
        /// </summary>
        public int I_ONR { get; set; }
    }
}
