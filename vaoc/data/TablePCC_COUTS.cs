using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    [Serializable]
    public class TablePCC_COUTS : IEnumerable<LignePCC_COUTS>
    {
        private List <LignePCC_COUTS> liste;
        Dictionary<int, List<LignePCC_COUTS>> m_listeCoutsParIDCase = new Dictionary<int, List<LignePCC_COUTS>>();
        Dictionary<long, List<LignePCC_COUTS>> m_listeCoutParCaseBloc = new Dictionary<long, List<LignePCC_COUTS>>();

        public TablePCC_COUTS()
        {
            liste = new List<LignePCC_COUTS>();
            m_listeCoutsParIDCase.Clear();
            m_listeCoutParCaseBloc.Clear();
        }

        private long Clef(int idcase, int xBloc, int yBloc)
        {
            long lidcase = idcase;
            return (long)(lidcase << 16) + (xBloc << 8) + yBloc;
        }

        /* fait dans le ajouter
        public bool Initialisation()
        {
            m_listeCoutsParIDCase.Clear();
            List<LignePCC_COUTS> listePCCCouts; //= new List<TAB_PCC_COUTSRow>();

            foreach (LignePCC_COUTS lCout in liste)
            {
                if (m_listeCoutsParIDCase.ContainsKey(lCout.ID_CASE_DEBUT))
                {
                    listePCCCouts = m_listeCoutsParIDCase[lCout.ID_CASE_DEBUT];
                }
                else
                {
                    listePCCCouts = new List<LignePCC_COUTS>();
                    m_listeCoutsParIDCase.Add(lCout.ID_CASE_DEBUT, listePCCCouts);
                }
                listePCCCouts.Add(lCout);
            }
            return true;
        }
        */

        public IEnumerator<LignePCC_COUTS> GetEnumerator()
        {
            return liste.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return liste.GetEnumerator();
        }

        public bool Importer(Donnees.TAB_PCC_COUTSDataTable tableXML)
        {
            Vider();
            foreach (Donnees.TAB_PCC_COUTSRow ligneXML in tableXML)
            {
                Ajouter(ligneXML);
            }
            return true;
        }

        public bool Exporter(Donnees.TAB_PCC_COUTSDataTable tableXML)
        {
            tableXML.Clear();
            foreach (LignePCC_COUTS ligne in liste)
            {
                Donnees.TAB_PCC_COUTSRow ligneXML = tableXML.AddTAB_PCC_COUTSRow(
                    ligne.I_BLOCX,
                    ligne.I_BLOCY,
                    ligne.ID_CASE_DEBUT,
                    ligne.ID_CASE_FIN,
                    ligne.I_COUT,
                    ligne.ID_TRAJET,
                    ligne.I_COUT_INITIAL,
                    ligne.B_CREATION,
                    ligne.ID_NATION ?? -1 );
                if (!ligne.ID_NATION.HasValue) { ligneXML.SetID_NATIONNull(); }
            }
            return true;
        }

        public void Ajouter(Donnees.TAB_PCC_COUTSRow ligneRow)
        {
            LignePCC_COUTS ligne = new LignePCC_COUTS(ligneRow);
            Ajouter(ligne);
        }

        public void Ajouter(LignePCC_COUTS ligne)
        {
            List<LignePCC_COUTS> listePCCCouts;

            liste.Add(ligne);
            if (m_listeCoutsParIDCase.ContainsKey(ligne.ID_CASE_DEBUT))
            {
                listePCCCouts = m_listeCoutsParIDCase[ligne.ID_CASE_DEBUT];
            }
            else
            {
                listePCCCouts = new List<LignePCC_COUTS>();
                m_listeCoutsParIDCase.Add(ligne.ID_CASE_DEBUT, listePCCCouts);
            }
            listePCCCouts.Add(ligne);

            long clef = Clef(ligne.ID_CASE_FIN, ligne.I_BLOCX, ligne.I_BLOCY);
            if (m_listeCoutParCaseBloc.ContainsKey(clef))
            {
                listePCCCouts = m_listeCoutParCaseBloc[clef];
            }
            else
            {
                listePCCCouts = new List<LignePCC_COUTS>();
                m_listeCoutParCaseBloc.Add(clef, listePCCCouts);
            }
            listePCCCouts.Add(ligne);            
        }

        public void Supprimer(LignePCC_COUTS ligne)
        {
            List<LignePCC_COUTS> listePCCCouts;

            liste.Remove(ligne);
            listePCCCouts = m_listeCoutsParIDCase[ligne.ID_CASE_DEBUT];
            if (1 == listePCCCouts.Count())
            {
                m_listeCoutsParIDCase.Remove(ligne.ID_CASE_DEBUT);
            }
            else
            {
                listePCCCouts.Remove(ligne);
            }

            long clef = Clef(ligne.ID_CASE_FIN, ligne.I_BLOCX, ligne.I_BLOCY);
            listePCCCouts = m_listeCoutParCaseBloc[clef];
            if (1 == listePCCCouts.Count())
            {
                m_listeCoutParCaseBloc.Remove(clef);
            }
            else
            {
                listePCCCouts.Remove(ligne);
            }
        }

        public void Vider()
        {
            liste.Clear();
            m_listeCoutsParIDCase.Clear();
            m_listeCoutParCaseBloc.Clear();
        }

        public LignePCC_COUTS this[int i]
        {
            get
            {
                return liste[i];
            }
        }

        public IEnumerable<LignePCC_COUTS> ListeLiensPointBloc(int idcase, int xBloc, int yBloc)
        {
            /*
            List<LignePCC_COUTS> requeteLinq =
                (from dataCout in liste
                where ((dataCout.I_BLOCX == xBloc) && (dataCout.I_BLOCY == yBloc) && (dataCout.ID_CASE_FIN == idcase))
                select dataCout).ToList();
            return requeteLinq;
            */
            List<LignePCC_COUTS> listePCCCouts;

            long clef = Clef(idcase, xBloc, yBloc);            
            listePCCCouts = (m_listeCoutParCaseBloc.ContainsKey(clef)) ? m_listeCoutParCaseBloc[clef] : new List<LignePCC_COUTS>();
            return listePCCCouts;
        }

        /// <summary>
        /// renvoie toutes les cases de liaisons voisines d'autres blocs que le sien
        /// </summary>
        /// <param name="source"></param>
        /// <param name="xBloc"></param>
        /// <param name="yBloc"></param>
        /// <returns></returns>
        public List<LignePCC_COUTS> CasesVoisines(LignePCC_COUTS source, bool BlocInterneCompris)
        {
            List<LignePCC_COUTS> resPCCCouts = new List<LignePCC_COUTS>();
            /* ce qui suit fonctionne mais est très lent, sur des blocs de 20, on a une table de 725102 lignes donc les acces ne sont pas rapides !

            var result = from ligneCout in this
                         where (ligneCout.ID_CASE_DEBUT == source.ID_CASE_FIN) && ((ligneCout.I_BLOCX != source.I_BLOCX) || (ligneCout.I_BLOCY != source.I_BLOCY))
                         select ligneCout;

            foreach (TAB_PCC_COUTSRow lCout in result)
            {
                {
                    resPCCCouts.Add(lCout);
                }
            }
             * */
            List<LignePCC_COUTS> listePCCCouts = m_listeCoutsParIDCase[source.ID_CASE_FIN];
            foreach (LignePCC_COUTS lCout in listePCCCouts)
            {
                if (BlocInterneCompris || ((lCout.I_BLOCX != source.I_BLOCX) || (lCout.I_BLOCY != source.I_BLOCY)))
                {
                    resPCCCouts.Add(lCout);
                }
            }
            return resPCCCouts;
        }
    }
}
