using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace WaocLib
{
    public class Constantes
    {
        #region constantes
        public const string CST_CLEFDEREGISTRE = "SOFTWARE\\WAOC\\VAOC";
        public const int CST_IDNULL = -1;//utilis� uniquement pour les �changes avec la form principale
        public const string CST_TRAJET_VILLE = "V";
        /// <summary>
        /// Le trajet n'est occup� par aucune nation
        /// </summary>
        //public const int CST_TRAJET_INOCCUPE = -1;//trajet

        /// <summary>
        /// Le trajet poss�de des cases occup�es par deux nations diff�rentes
        /// </summary>
        //public const int CST_TRAJET_CONTESTE = -2;
        
        public const int CST_PERTE_MORAL_FUITE = 10;
        public const decimal CST_GAIN_EXPERIENCE_BATAILLE = 0.5M;
        public const int CST_GAIN_MORAL_MAITRE_TERRAIN = 5;
        public const int CST_GAIN_FATIGUE_REPOS = 20;
        public const int CST_GAIN_MORAL_REPOS = 10;
        public const int CST_BRUIT_DU_CANON = 30;//distance en km o� l'on entend le bruit d'une bataille
        public const int CST_RECHERCHE_ZONE_VICTOIRE = 1;//rayon en km sur laquelle on recherche le contr�le autour d'un point ayant une valeur
        public const int CST_PERTE_MORAL_MAX_SS_RAVITAILLEMENT = 1;//valeur maximale de moral qu'une unit� peut perdre par heure sans ravitaillement
        public const int CST_DISTANCE_MAX_RECHERCHE_DESTINATAIRE = 30; // Distance "effective" en km au dela de laquelle un messager ne d�livre pas son message si le destinataire n'est pas pr�sent
        public const int CST_LIMITE_MORAL_CAVALERIE_POURSUITE = 20;//en dessous de cette valeur, une cavalerie ne peut pas poursuivre
        public const int CST_PAS_DE_PERTES = 100;//pas de base des pertes lors d'une poursuite
        public const int CST_DUREE_ENDOMMAGE_PONT = 2;//dur�e necessaire, en heures, pour endommager 100m de pont (la r�gle, c'est 2 heures pour un point mineur, 4 pour un pont majeur)
        public const int CST_DUREE_REPARER_PONT = 4;//dur�e necessaire, en heures, pour r�parer 100m de pont (la r�gle, c'est 2 heures pour un point mineur, 12 pour un pont majeur, uniquement pour des pontonniers)
        public const int CST_DUREE_CONSTRUIRE_PONTON = 6;//dur�e necessaire, en heures, pour constuire 100m de ponton (la r�gle, c'est 24 heures pour une rivi�re principale, uniquement pour des pontonniers)
        public const int CST_DISTANCE_RECHERCHE_ENNEMI = 3; //distance en kilom�tre o� la pr�sence d'un ennemi emp�che la construction d'un ponton ou la reconstruction d'un pont
        public const int CST_BRUIT_ENDOMMAGE_PONT = 10;//distance en km o� l'on entend le bruit de destruction d'un pont ou la vue de la colonne de fum�e li�e au feu
        public const int CST_BRUIT_REPARER_PONT_PONTON = 5;//distance en km o� l'on entend le bruit de construction d'un pont ou d'un ponton
        public const int CST_DISTANCE_MAX_RAVITAILLEMENT = 200;//distance "effective" en km au dela de laquelle le ravitaillement n'est plus assur�
        public const int CST_DISTANCE_DEPOT_RAVITAILLEMENT = 3; //distance en kilom�tre o� l'on consid�re que l'unit� se ravitaille automatiquement sur le d�p�t
        public const int CST_TAILLE_CHAMP_DE_BATAILLE = 6;//en kilom�tres
        public const int CST_PONDERATION_CASE_COMBAT = 10;//renforce la valeur d'un mod�le de terrain si la case est occup�e
        public const int CST_DISTANCE_RECHERCHE_PONT_GUET = 5;//en kilom�tres
        public const int CST_ENCOMBREMENT_CONVOI_RAVITAILLEMENT = 10;//un convoi de ravitaillement, un convoi prend 10km sur route d'apr�s la r�gle
        public const int CST_ENCOMBREMENT_PONTONNIERS = 4;//disons qu'un groupe de pontonnier fait 1000 hommes + 100% de mat�riel, �a nous fait 4 km
        public const int CST_VICTOIRE_EGALITE = 3;//aucun des deux camps ne l'a emport�
        public const int CST_DISTANCE_RECHERCHE_HOPITAL = 3; //distance en kilom�tre o� l'on consid�re que l'on se trouve � cot� d'un hopital
        public const int CST_DISTANCE_RECHERCHE_PRISON = 3; //distance en kilom�tre o� l'on consid�re que l'on se trouve � cot� d'une prison
        public const int CST_TAILLE_MINIMUM_UNITE = 100; //Effectif minimum de bless�s necessaire, soit pour cr�er le convoi de bless�s, soit faire un convoi de renfort de gu�ris
        public const int CST_DUREE_CONSTRUIRE_FORTIFICATIONS = 24;//dur�e necessaire, en heures, pour construire un niveau de fortifications (2 au max)
        public const int CST_FORTIFICATIONS_MAX = 2;//niveau maximum de fortifications
        public const int CST_DUREE_INDISPONIBLE_SUR_BLESSURE = 2;//temps necessaire � trouver un rempla�ant sur blessure grave.

        public const double SQRT2 = 1.4142135623730950488016887242097;
        public enum VICTOIRECOMBAT { VICTOIRE012, EGALITE, VICTOIRE345 };

        //public const char CST_RESERVE = 'R';
        //public const char CST_FRONT_GAUCHE = 'G';

        public const char CST_BATAILLE_HORIZONTALE = 'O';
        public const char CST_BATAILLE_VERTICALE = 'V';

        //liste des diff�rents objetifs possibles dans une partie
        public static class OBJECTIFS
        {
            public const int CST_OBJECTIF_DEMORALISATION = 1;
        }

        public const int CST_ESPACE_DE_RECHERCHE = 100;

        //liste des diff�rents mod�les de cartes utilis�s en affichage
        public enum MODELESCARTE { HISTORIQUE, ZOOM, GRIS, TOPOGRAPHIQUE };
        //liste des diff�rents recherches de parcours
        public enum TYPEPARCOURS { MOUVEMENT = 0, RAVITAILLEMENT = 1 };

        //liste des diff�rents ordres
        /*
        public enum ORDRES
        {
            MOUVEMENT = 1,
            COMBAT = 2,
            RETRAITE = 3,
            PATROUILLE = 4,
            MESSAGE = 5,
            ENDOMMAGER_PONT = 6,
            REPARER_PONT = 7,
            CONSTRUIRE_PONTON = 8,
            ARRET = 9,
            TRANSFERER = 10,
            GENERERCONVOI = 11,
            RENFORCER = 12,
            /// <summary>
            /// construction de fortification de campagne
            /// </summary>
            SEFORTIFIER = 13,
            /// <summary>
            /// Modification du niveau d'engagement sur un secteur de la bataille
            /// </summary>
            ENGAGEMENT = 14
        }
        */
        public static class ORDRES
        {
            public const int MOUVEMENT = 1;
            public const int COMBAT = 2;
            public const int RETRAITE = 3;
            public const int PATROUILLE = 4;
            public const int MESSAGE = 5;
            public const int ENDOMMAGER_PONT = 6;
            public const int REPARER_PONT = 7;
            public const int CONSTRUIRE_PONTON = 8;
            public const int ARRET = 9;
            public const int TRANSFERER = 10;
            public const int GENERERCONVOI = 11;
            public const int RENFORCER = 12;
            /// <summary>
            /// construction de fortification de campagne
            /// </summary>
            public const int SEFORTIFIER = 13;
            /// <summary>
            /// Modification du niveau d'engagement sur un secteur de la bataille
            /// </summary>
            public const int ENGAGEMENT = 14;
            /// <summary>
            /// Transformer un convoi en d�p�t de niveau 'D'
            /// </summary>
            public const int ETABLIRDEPOT = 15;
            /// <summary>
            /// Faire un retrait d'une zone d'engagement sur un combat
            /// </summary>
            public const int RETRAIT = 16;
        }

        //Heures de lev�e du soleil suivant les mois janvier - d�cembre
        public static short[] tableHeuresLeveeDuSoleil = new short[12] { 8, 7, 6, 5, 4, 3, 3, 4, 5, 6, 7, 8 };

        //Heures de cocuher du soleil suivant les mois janvier - d�cembre
        public static short[] tableHeuresCoucherDuSoleil = new short[12] { 18, 18, 19, 20, 21, 22, 22, 21, 20, 19, 18, 18 };

        //premiere colonne, type de d�p�t, deuxi�me, distance en kilometre, il s'agit du pourcentage de gain en �quipement et mat�riel
        public static int[,] tableRavitaillementDepot = new int[4, 5] 
        {   { 25, 20, 15, 10, 5}, 
            { 15, 10,  7,  5, 3},
            { 10,  7,  5,  3, 1},
            {  5,  3,  1,  0, 0}
         };

        //colonne = valeur de mat�riel, ligne = ravitaillement, cette table ne doit �tre utilis�e qu'avec la m�thode CalculerEfficaciteAuCombat de cette m�me classe
        private static int[,] tableEffaciteAuCombat = new int[5, 5] 
        {   { -2, -2, -1, -1,  0}, 
            { -2, -1, -1,  0,  1},
            { -1, -1,  0,  1,  1},
            { -1,  0,  1,  1,  2},
            {  0,  1,  1,  2,  2}
         };

        public static int[] CST_ENCOMBREMENT_DEPOT = new int[4] { 25, 20, 15, 10 };//un d�p�t, �a prend de la place ! choix personnel, je n'ai pas trouv� l'info dans les r�gles 

        public static int[] tableLimiteRavitaillementDepot = new int[]
        {50000, 40000, 30000, 20000};

        //premi�re colonne jet de d�, deuxi�me moral (le tout est modifi� par le rapport)
        public static int[,] tablePoursuite = new int[10, 4] 
        {   { 0, 2, 4, 6}, 
            { 2, 4, 6, 8},
            { 4, 6, 8,10}, 
            { 6, 8,10,12}, 
            { 8,10,12,14}, 
            {10,12,14,16}, 
            {12,14,16,18}, 
            {14,16,18,20}, 
            {16,18,20,22}, 
            {18,20,22,24} };

        //pts de fatigue par heure de marche
        public static int[] tableFatigueInfanterie = new int[25] 
        { 0, 0, 0, 0, 0, 1, 1, 2, 2, 3, 4, 5, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30 };

        public static int[] tableFatigueCavalerie = new int[25] 
        { 1, 1, 1, 1, 1, 1, 2, 2, 3, 4, 5, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32 };

        //Libell�s pour le moral des unit�s de 0 � 50, de 10 en 10: 0 - 1-10, 11-20, 21-30, 31-40, 41-50
        public static string[] tableLibelleMoral = new string[] { "d�route", "milice", "novice", "ligne", "�lite", "garde"};

        //Libell�s pour l'exp�rience des unit�s de -2 � 2, de 1 en 1
        public static string[] tableLibelleExperience = new string[] { "bleu", "novice", "profesionnel", "�lite", "garde" };

        //Libell�s pour la fatigue des unit�s de 0 � 80, de 20 en 20 : 0 - 1-20, 21-40, 41-60, 61-80, 81-100
        public static string[] tableLibelleFatigue = new string[] { "aucune", "faible", "cons�quante", "�puis�", "ext�nu�", "d�sint�gr�" };

        //Libell�s pour l'�quipement et le ravitaillement des unit�s de 0 � 80, de 20 en 20 : 0 - 1-20, 21-40, 41-60, 61-80, 81-99, 100
        public static string[] tableLibelleMaterielRavitaillement = new string[] { "aucun", "faible", "m�diocre", "bon", "excellent", "complet", "maximum" };

        #endregion

        #region variables
        private static string m_repertoireDonnee;
        #endregion

        #region methodes
        public static string repertoireDonnees
        {
            get
            {
                return m_repertoireDonnee;
            }
            set
            {
                m_repertoireDonnee = value.Substring(0, value.LastIndexOf('\\') + 1);
            }
        }

        public static double Distance(double srcX, double srcY, double destX, double destY)
        {
            return Math.Sqrt(Math.Pow(srcX - destX, 2) + Math.Pow(srcY - destY, 2));
        }

        public static double AngleOfView(double ViewPt_X, double ViewPt_Y,
                                        double Pt1_X, double Pt1_Y,
                                        double Pt2_X, double Pt2_Y)
        {
            double a1, b1, a2, b2, a, b, t, cosinus;

            a1 = Pt1_X - ViewPt_X;
            a2 = Pt1_Y - ViewPt_Y;

            b1 = Pt2_X - ViewPt_X;
            b2 = Pt2_Y - ViewPt_Y;

            a = Math.Sqrt((a1 * a1) + (a2 * a2));
            b = Math.Sqrt((b1 * b1) + (b2 * b2));

            if ((a == 0.0) || (b == 0.0))
                return (0.0);

            cosinus = (a1 * b1 + a2 * b2) / (a * b);

            t = Math.Acos(cosinus);///l'angle r�sultat est donn� en radians

            t = t * 180.0 / Math.PI;

            return (t);
        }
        
        /// <summary>
        /// Jet de d�s � 6 faces
        /// </summary>
        /// <param name="nombre">nombre de d�s � lancer</param>
        /// <returns>total des jets de d�s</returns>
        public static int JetDeDes(int nombre)
        {
            int i, resultat=0;
            Random de = new Random();

            for (i = 0; i < nombre; i++)
            {
                resultat += de.Next(1, 6);
            }
            return resultat;
        }

        /// <summary>
        /// Jet de d�s � 6 faces
        /// </summary>
        /// <param name="nombre">nombre de d�s � lancer</param>
        /// <param name="relance">nombre de d�s que l'on relance apr�s tirage si le score est 1 ou 2</param>
        /// <returns>total des jets de d�s</returns>
        public static int JetDeDes(int nombre, int relance)
        {
            int nb6 = 0;
            return JetDeDes(nombre, relance, out nb6);
        }

        /// <summary>
        /// Jet de d�s � 6 faces
        /// </summary>
        /// <param name="nombre">nombre de d�s � lancer</param>
        /// <param name="relance">nombre de d�s que l'on relance apr�s tirage si le score est 1 ou 2</param>
        /// <param name="nb6">nombre de '6' lanc�s (utile pour d�terminer si un chef est bless�)</param>
        /// <returns>total des jets de d�s</returns>
        public static int JetDeDes(int nombre, int relance, out int nb6)
        {
            int i;
            int resultat = 0;
            nb6 = 0;
            Random de = new Random();

            for (i = 0; i < nombre; i++)
            {
                int score = de.Next(1, 6);
                if ((relance > 0) && (score == 1 || score == 2))
                {
                    score = de.Next(1, 6);
                    relance--;
                }
                resultat += score;
                if (6 == score) { nb6++; }
            }
            return resultat;
        }

        /// <summary>
        /// indique si le premier caract�re de la phrase est une voyelle
        /// </summary>
        /// <param name="phrase">phrase � tester</param>
        /// <returns>true si c'est une voyelle, false sinon</returns>
        public static bool DebuteParUneVoyelle(string phrase)
        {
            char[] VOYELLES = "aeiouyAEIOUY����������������".ToCharArray();

            if (0 == phrase.IndexOfAny(VOYELLES))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// renvoie la libell� texte d'une valeur num�rique d'exp�rience
        /// </summary>
        /// <param name="iExperience">valeur num�rique</param>
        /// <returns>lib�ll�</returns>
        public static string LibelleExperience(int iExperience)
        {
            return tableLibelleExperience[iExperience + 4];
        }

        /// <summary>
        /// renvoie la libell� texte d'une valeur num�rique pour le moral
        /// </summary>
        /// <param name="iMoral">valeur num�rique</param>
        /// <returns>lib�ll�</returns>
        public static string LibelleMoral(int iMoral)
        {
            if (iMoral == 0) { return tableLibelleMoral[0]; }
            return tableLibelleMoral[(int)Math.Ceiling((decimal)iMoral / (decimal)10)];
        }

        /// <summary>
        /// renvoie la libell� texte d'une valeur num�rique de fatigue
        /// </summary>
        /// <param name="iFatigue">fatigue num�rique</param>
        /// <returns>lib�ll�</returns>
        public static string LibelleFatigue(int iFatigue)
        {
            if (iFatigue == 0) { return tableLibelleFatigue[0]; }
            return tableLibelleFatigue[(int)Math.Ceiling((decimal)iFatigue / (decimal)20)];
        }

        /// <summary>
        /// renvoie la libell� texte d'une valeur num�rique d'�quipement ou de ravitaillement
        /// </summary>
        /// <param name="iMaterielRavitaillement">valeur num�rique</param>
        /// <returns>lib�ll�</returns>
        public static string LibelleMaterielRavitaillement(int iMaterielRavitaillement)
        {
            if (iMaterielRavitaillement == 0) { return tableLibelleMaterielRavitaillement[0]; }
            if (iMaterielRavitaillement >= 100) { return tableLibelleMaterielRavitaillement[tableLibelleMaterielRavitaillement.Length-1]; }
            return tableLibelleMaterielRavitaillement[(int)Math.Ceiling((decimal)iMaterielRavitaillement / (decimal)20)];
        }

        /// <summary>
        /// Convertir une chaine pour une insertion en base
        /// </summary>
        /// <param name="source">chaine source</param>
        /// <returns>chaine au format SQL</returns>
        public static string ChaineSQL(string source)
        {
            return source.Replace("'", "''").Replace("\\", "");//remplacement des apostrophes et des antislashs pour l'insertion en base
        }

        public static string ChaineSQL(decimal source)
        {
            return source.ToString().Replace(",", ".");//remplacement de la virgule par un point
        }

        public static string DateHeureSQL(DateTime dateHeure)
        {
            //'1805-06-15 02:04:18'
            return dateHeure.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static int CalculerEfficaciteAuCombat(int materiel, int ravitaillement)
        {
            //colonne = valeur de mat�riel, ligne = ravitaillement
            return tableEffaciteAuCombat[Math.Max(0, (materiel - 1) / 20), Math.Max(0, (ravitaillement - 1) / 20)];
        }

        #endregion
    }
}
