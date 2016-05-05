using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    [Serializable]
    public class TableCASE : IEnumerable
    {
        private List<LigneCASE> liste;
        private Hashtable indexID;
        private Array m_listeIndex = null; //renvoie l'id cas à partir d'un x,y : optimisation mémoire
        public const int CST_COUTMAX = Int32.MaxValue;
        private int m_largeur;
        private int m_hauteur;

        public bool Importer(Donnees.TAB_CASEDataTable tableXML)
        {
            Vider();
            foreach (Donnees.TAB_CASERow ligneXML in tableXML)
            {
                Ajouter(ligneXML);
            }
            return true;
        }

        public bool Exporter(Donnees.TAB_CASEDataTable tableXML)
        {
            tableXML.Clear();
            foreach (LigneCASE ligne in liste)
            {
                Donnees.TAB_CASERow ligneXML = tableXML.AddTAB_CASERow(ligne.ID_CASE,
                    ligne.ID_MODELE_TERRAIN,
                    ligne.I_X,
                    ligne.I_Y,
                    ligne.ID_PROPRIETAIRE ?? -1,
                    ligne.ID_NOUVEAU_PROPRIETAIRE ?? -1,
                    ligne.ID_MODELE_TERRAIN_SI_OCCUPE ?? -1,
                    ligne.I_COUT);
                if (!ligne.ID_PROPRIETAIRE.HasValue) { ligneXML.SetID_PROPRIETAIRENull(); }
                if (!ligne.ID_NOUVEAU_PROPRIETAIRE.HasValue) { ligneXML.SetID_NOUVEAU_PROPRIETAIRENull(); }
                if (!ligne.ID_MODELE_TERRAIN_SI_OCCUPE.HasValue) { ligneXML.SetID_MODELE_TERRAIN_SI_OCCUPENull(); }
            }
            return true;
        }

        private int IDPourXY(int x, int y)
        {
            // a utiliser pour la prohaine carte seulement
            int largeur = BD.Base.Jeu[0].I_LARGEUR_CARTE - 1;
            return y * largeur + x;
        }

        public TableCASE(int largeur, int hauteur)
        {
            liste = new List<LigneCASE>();
            indexID = new Hashtable();
            m_largeur = largeur;
            m_hauteur = hauteur;
            m_listeIndex = Array.CreateInstance(typeof(int), m_largeur, m_hauteur);
        }

        public void Ajouter(Donnees.TAB_CASERow ligneRow)
        {
            LigneCASE ligne = new LigneCASE(ligneRow);
            Ajouter(ligne);
        }

        public void Ajouter(LigneCASE ligne)
        {
            liste.Add(ligne);
            indexID.Add(ligne.ID_CASE, ligne);
            m_listeIndex.SetValue(ligne.ID_CASE, ligne.I_X, ligne.I_Y);
        }

        public void Supprimer(LigneCASE ligne)
        {
            indexID.Remove(ligne.ID_CASE);
            liste.Remove(ligne);
        }

        public void Vider()
        {
            indexID.Clear();
            liste.Clear();
        }

        public LigneCASE this[int i]
        {
            get
            {
                //return (LigneCASE)(liste.GetByIndex(i));
                return (LigneCASE)indexID[i];
            }
        }

        public LigneCASE TrouveParID_CASE(int id)
        {
            return (LigneCASE)indexID[id];
        }

        public LigneCASE[] CasesVoisines(LigneCASE source)
        {
            LigneCASE[] resCase;
            int maxX = m_largeur - 1;
            int maxY = m_hauteur - 1;

            int[] x = new int[8];
            int[] y = new int[8];
            int nb_points = 0;
            if (source.I_X > 0 && source.I_Y > 0)
            {
                x[nb_points] = source.I_X - 1;
                y[nb_points++] = source.I_Y - 1;
            }
            if (source.I_X > 0 && source.I_Y < maxY)
            {
                x[nb_points] = source.I_X - 1;
                y[nb_points++] = source.I_Y + 1;
            }
            if (source.I_X < maxX && source.I_Y < maxY)
            {
                x[nb_points] = source.I_X + 1;
                y[nb_points++] = source.I_Y + 1;
            }
            if (source.I_X < maxX && source.I_Y > 0)
            {
                x[nb_points] = source.I_X + 1;
                y[nb_points++] = source.I_Y - 1;
            }
            if (source.I_X > 0)
            {
                x[nb_points] = source.I_X - 1;
                y[nb_points++] = source.I_Y;
            }
            if (source.I_X < maxX)
            {
                x[nb_points] = source.I_X + 1;
                y[nb_points++] = source.I_Y;
            }
            if (source.I_Y > 0)
            {
                x[nb_points] = source.I_X;
                y[nb_points++] = source.I_Y - 1;
            }
            if (source.I_Y < maxY)
            {
                x[nb_points] = source.I_X;
                y[nb_points++] = source.I_Y + 1;
            }
            resCase = new LigneCASE[nb_points];
            for (int i = 0; i < nb_points; i++)
            {
                resCase[i] = FindByXY(x[i], y[i]);
            }

            if (0 == resCase.Length)
            {
                string message = string.Format("CasesVoisines ne trouve aucun voisin pour x={0} et y={1}", source.I_X, source.I_Y);
                throw new Exception(message);
            }

            return resCase;
        }

        /// <summary>
        /// renvoie une case d'après ses coordonnées
        /// </summary>
        /// <param name="x">abscisse</param>
        /// <param name="y">ordonnée</param>
        /// <returns>case trouvée, null si la case n'existe pas</returns>
        public LigneCASE FindByXY(int x, int y)
        {
            try
            {
                return this[(int)m_listeIndex.GetValue(x, y)];
            }
            catch
            {
                return null;
            }
        }

        public IList<LigneCASE> CasesVoisinesInBloc(LigneCASE source, int xBloc, int yBloc, int tailleBloc)
        {
            IList<LigneCASE> resCase;
            int maxX = m_largeur - 1;
            int maxY = m_hauteur - 1;
            int xmin = xBloc * tailleBloc;
            int xmax = xmin + tailleBloc;
            int ymin = yBloc * tailleBloc;
            int ymax = ymin + tailleBloc;

            int[] x = new int[8];
            int[] y = new int[8];
            int nb_points = 0;
            if (source.I_X > 0 && source.I_Y > 0)
            {
                x[nb_points] = source.I_X - 1;
                y[nb_points++] = source.I_Y - 1;
            }
            if (source.I_X > 0 && source.I_Y < maxY)
            {
                x[nb_points] = source.I_X - 1;
                y[nb_points++] = source.I_Y + 1;
            }
            if (source.I_X < maxX && source.I_Y < maxY)
            {
                x[nb_points] = source.I_X + 1;
                y[nb_points++] = source.I_Y + 1;
            }
            if (source.I_X < maxX && source.I_Y > 0)
            {
                x[nb_points] = source.I_X + 1;
                y[nb_points++] = source.I_Y - 1;
            }
            if (source.I_X > 0)
            {
                x[nb_points] = source.I_X - 1;
                y[nb_points++] = source.I_Y;
            }
            if (source.I_X < maxX)
            {
                x[nb_points] = source.I_X + 1;
                y[nb_points++] = source.I_Y;
            }
            if (source.I_Y > 0)
            {
                x[nb_points] = source.I_X;
                y[nb_points++] = source.I_Y - 1;
            }
            if (source.I_Y < maxX)
            {
                x[nb_points] = source.I_X;
                y[nb_points++] = source.I_Y + 1;
            }

            resCase = new List<LigneCASE>();
            for (int i = 0; i < nb_points; i++)
            {
                if (x[i] >= xmin && x[i] <= xmax && y[i] >= ymin && y[i] <= ymax)
                //if (x[i] >= xmin && x[i] < xmax && y[i] >= ymin && y[i] < ymax)
                {
                    LigneCASE ligneCase = FindByXY(x[i], y[i]);
                    resCase.Add(ligneCase);
                }
            }

            if (0 == resCase.Count)
            {
                string message = string.Format("CasesVoisinesInBloc ne trouve aucun voisin pour x={0} et y={1}", source.I_X, source.I_Y);
                throw new Exception(message);
            }

            return resCase;
        }

        public IEnumerable<LigneCASE> ListeCaseseParCout()
        {
            //requete = string.Format("I_COUT<{0}", CST_COUTMAX);
            //listeCaseEspace = (LigneCASE[])Donnees.m_donnees.TAB_CASE.Select(requete, "I_COUT");

            IEnumerable<LigneCASE> requeteLinq =
                from dataCase in liste
                where (dataCase.I_COUT < CST_COUTMAX)
                orderby dataCase.I_COUT descending
                select dataCase;
            return requeteLinq;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public CASEEnum GetEnumerator()
        {
            return new CASEEnum(liste);
        }
    }

    public class CASEEnum : IEnumerator
    {
        public List<LigneCASE> _cases;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public CASEEnum(List<LigneCASE> list)
        {
            _cases = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < _cases.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public LigneCASE Current
        {
            get
            {
                try
                {
                    return _cases[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }

}
