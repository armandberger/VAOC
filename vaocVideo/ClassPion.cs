using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using WaocLib;

namespace vaocVideo
{
    partial class Donnees
    {
        partial class TAB_PIONDataTable
        {
            /// <summary>
            /// Methode sans avoir à fournir l'ID_PION
            /// </summary>
            /// <param name="ID_MODELE_PION"></param>
            /// <param name="ID_PION_PROPRIETAIRE"></param>
            /// <param name="S_NOM"></param>
            /// <param name="I_INFANTERIE"></param>
            /// <param name="I_CAVALERIE"></param>
            /// <param name="I_ARTILLERIE"></param>
            /// <param name="I_FATIGUE"></param>
            /// <param name="I_MORAL"></param>
            /// <param name="I_MORAL_MAX"></param>
            /// <param name="I_EXPERIENCE"></param>
            /// <param name="I_TACTIQUE"></param>
            /// <param name="I_STRATEGIQUE"></param>
            /// <param name="C_NIVEAU_HIERARCHIQUE"></param>
            /// <param name="I_DISTANCE_A_PARCOURIR"></param>
            /// <param name="I_NB_PHASES_MARCHE_JOUR"></param>
            /// <param name="I_NB_PHASES_MARCHE_NUIT"></param>
            /// <param name="ID_CASE"></param>
            /// <param name="I_TOUR_SANS_RAVITAILLEMENT"></param>
            /// <param name="ID_BATAILLE"></param>
            /// <param name="I_ZONE_BATAILLE"></param>
            /// <param name="I_TOUR_FUITE_RESTANT"></param>
            /// <param name="B_DETRUIT"></param>
            /// <param name="B_INTERCEPTION"></param>
            /// <returns></returns>
            public TAB_PIONRow AddTAB_PIONRow(
                        int ID_MODELE_PION,
                        int ID_PION_PROPRIETAIRE,
                        int ID_NOUVEAU_PION_PROPRIETAIRE,
                        int ID_ANCIEN_PION_PROPRIETAIRE,
                        string S_NOM,//5
                        int I_INFANTERIE,
                        int I_INFANTERIE_INITIALE,
                        int I_CAVALERIE,
                        int I_CAVALERIE_INITIALE,
                        int I_ARTILLERIE,//10
                        int I_ARTILLERIE_INITIALE,
                        int I_FATIGUE,
                        int I_MORAL,
                        int I_MORAL_MAX,
                        decimal I_EXPERIENCE,//15
                        int I_TACTIQUE,
                        int I_STRATEGIQUE,
                        char C_NIVEAU_HIERARCHIQUE,
                        decimal I_DISTANCE_A_PARCOURIR,
                        int I_NB_PHASES_MARCHE_JOUR,//20
                        int I_NB_PHASES_MARCHE_NUIT,
                        int I_NB_HEURES_COMBAT,
                        int ID_CASE,
                        int I_TOUR_SANS_RAVITAILLEMENT,
                        int ID_BATAILLE,//25
                        int I_ZONE_BATAILLE,
                        int I_TOUR_RETRAITE_RESTANT,
                        int I_TOUR_FUITE_RESTANT,
                        bool B_DETRUIT,
                        bool B_FUITE_AU_COMBAT,//30
                        bool B_INTERCEPTION,
                        bool B_REDITION_RAVITAILLEMENT,
                        bool B_TELEPORTATION,
                        bool B_ENNEMI_OBSERVABLE,
                        int I_MATERIEL,//35
                        int I_RAVITAILLEMENT,
                        bool B_CAVALERIE_DE_LIGNE,
                        bool B_CAVALERIE_LOURDE,
                        bool B_GARDE,
                        bool B_VIEILLE_GARDE,//40
                        int I_TOUR_CONVOI_CREE,
                        int ID_DEPOT_SOURCE,
                        int I_SOLDATS_RAVITAILLES,
                        int I_NB_HEURES_FORTIFICATION,
                        int I_NIVEAU_FORTIFICATION,//45
                        int ID_PION_REMPLACE,
                        int I_DUREE_HORS_COMBAT,
                        int I_TOUR_BLESSURE,
                        bool B_BLESSES,
                        bool B_PRISONNIERS,//50
                        bool B_RENFORT,
                        int ID_LIEU_RATTACHEMENT,
                        char C_NIVEAU_DEPOT,
                        int ID_PION_ESCORTE,
                        int I_INFANTERIE_ESCORTE,//55
                        int I_CAVALERIE_ESCORTE,
                        int I_MATERIEL_ESCORTE,
                        int I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT,
                        int I_VICTOIRE
                )
            {
                //recherche du max de l'ID_PION pour effectuer l'insertion, l'ID_PION ne peut pas
                //être incrementé automatiquement à cause de la bascule des pions de TAB_RENFORT vers TAB_PION
                LogFile.Notifier("AjouterPion:AddTAB_PIONRow");
                string tri = "ID_PION DESC";
                Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                TAB_PIONRow[] resCout = (TAB_PIONRow[])Select(string.Empty, tri);
                if (0 == resCout.Length)
                {
                    Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    return null;
                }
                TAB_PIONRow rowTAB_PIONRow = ((TAB_PIONRow)(this.NewRow()));
                object[] columnValuesArray = new object[] {
                        resCout[0].ID_PION+1,
                        ID_MODELE_PION,
                        ID_PION_PROPRIETAIRE,
                        ID_NOUVEAU_PION_PROPRIETAIRE,
                        ID_ANCIEN_PION_PROPRIETAIRE,
                        S_NOM,
                        I_INFANTERIE,
                        I_INFANTERIE_INITIALE,
                        I_CAVALERIE,
                        I_CAVALERIE_INITIALE,
                        I_ARTILLERIE,
                        I_ARTILLERIE_INITIALE,
                        I_FATIGUE,
                        I_MORAL,
                        I_MORAL_MAX,
                        I_EXPERIENCE,
                        I_TACTIQUE,
                        I_STRATEGIQUE,
                        C_NIVEAU_HIERARCHIQUE,
                        I_DISTANCE_A_PARCOURIR,
                        I_NB_PHASES_MARCHE_JOUR,
                        I_NB_PHASES_MARCHE_NUIT,
                        I_NB_HEURES_COMBAT,
                        ID_CASE,
                        I_TOUR_SANS_RAVITAILLEMENT,
                        ID_BATAILLE,
                        I_ZONE_BATAILLE,
                        I_TOUR_RETRAITE_RESTANT,
                        I_TOUR_FUITE_RESTANT,
                        B_DETRUIT,
                        B_FUITE_AU_COMBAT,
                        B_INTERCEPTION,
                        B_REDITION_RAVITAILLEMENT,
                        B_TELEPORTATION,
                        B_ENNEMI_OBSERVABLE,
                        I_MATERIEL,
                        I_RAVITAILLEMENT,
                        B_CAVALERIE_DE_LIGNE,
                        B_CAVALERIE_LOURDE,
                        B_GARDE,
                        B_VIEILLE_GARDE,
                        I_TOUR_CONVOI_CREE,
                        ID_DEPOT_SOURCE,
                        I_SOLDATS_RAVITAILLES,
                        I_NB_HEURES_FORTIFICATION,
                        I_NIVEAU_FORTIFICATION,
                        ID_PION_REMPLACE,
                        I_DUREE_HORS_COMBAT,
                        I_TOUR_BLESSURE,
                        B_BLESSES,
                        B_PRISONNIERS,
                        B_RENFORT,
                        ID_LIEU_RATTACHEMENT,
                        C_NIVEAU_DEPOT,
                        ID_PION_ESCORTE,
                        I_INFANTERIE_ESCORTE,
                        I_CAVALERIE_ESCORTE,
                        I_MATERIEL_ESCORTE,
                        I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT,
                        I_VICTOIRE
                };
                rowTAB_PIONRow.ItemArray = columnValuesArray;
                this.Rows.Add(rowTAB_PIONRow);
                Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                return rowTAB_PIONRow;
            }
        }

        partial class TAB_PIONRow
        {
            #region proprietes
            /// <summary>
            /// Renvoie le pion qui a remplacé le pion courant
            /// </summary>
            public Donnees.TAB_PIONRow pionRemplacant
            {
                get
                {
                    //on recherche le pion de remplacement
                    string requete = string.Format("ID_PION_REMPLACE = {0}", ID_PION);
                    Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    TAB_PIONRow[] resPions = (TAB_PIONRow[])m_donnees.TAB_PION.Select(requete);
                    Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    if (resPions.Length <= 0)
                    {
                        return null;
                    }
                    return resPions[0];
                }
            }

            /// <summary>
            /// renvoie le pion que le pion courant a remplacé
            /// </summary>
            public Donnees.TAB_PIONRow pionRemplace
            {
                get
                {
                    int IdPionRemplace = ID_PION_REMPLACE;
                    if (Constantes.NULLENTIER == IdPionRemplace)
                    {
                        return null;
                    }
                    return Donnees.m_donnees.TAB_PION.FindByID_PION(IdPionRemplace);
                }
            }

            //true si l'unité n'a fait aucune activité ce jour, false sinon
            public bool reposComplet
            {
                get
                {
                    //if ((I_NB_HEURES_COMBAT > 0) || (I_NB_HEURES_FORTIFICATION > 0) || (I_NB_PHASES_MARCHE_JOUR > 0) || (I_NB_PHASES_MARCHE_NUIT > 0))
                    //I_NB_HEURES_FORTIFICATION n'est jamais renseigné
                    if ((I_NB_HEURES_COMBAT > 0) || (I_NB_PHASES_MARCHE_JOUR > 0) || (I_NB_PHASES_MARCHE_NUIT > 0))
                    {
                        return false;
                    }
                    //si l'unité s'est fortifié durant la journée, elle n'était pas en repos
                    Donnees.TAB_ORDRERow ligneORDRE = Donnees.m_donnees.TAB_ORDRE.SeFortifier(ID_PION);
                    if (null != ligneORDRE)
                    {
                        if (ligneORDRE.IsI_TOUR_FINNull() || ligneORDRE.I_TOUR_FIN +24 > Donnees.m_donnees.TAB_PARTIE.HeureCourante())
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            //renvoie la nation du pion
            public TAB_NATIONRow nation
            {
                get
                {
                    TAB_MODELE_PIONRow ligneModelePion = this.modelePion;
                    if (null == ligneModelePion) { return null; }
                    return m_donnees.TAB_NATION.FindByID_NATION(ligneModelePion.ID_NATION);
                }
            }

            /// <summary>
            /// Nom du pion pour afficher dans une liste (sur FormPrincipale, la combo, et la liste des noms villes)
            /// </summary>
            public string nomListe
            {
                get { return string.Format("{0}:{1}", ID_PION, S_NOM); }
            }

            /// <summary>
            /// Nom du pion avec effectifs pour afficher dans une liste (sur FormNomCarte)
            /// </summary>
            public string nomListeEffectifs
            {
                get { return string.Format("{0}:{1} - {2}i {3}c {4}a", ID_PION, S_NOM, I_INFANTERIE, I_CAVALERIE, I_ARTILLERIE); }
            }

            /// <summary>
            /// effectif total du pion en tenant compte de la fatigue
            /// </summary>
            public int effectifTotal
            {
                get
                {
                    return infanterie + cavalerie + artillerie;
                    //return I_ARTILLERIE + I_CAVALERIE + I_INFANTERIE;
                }
                //set; 
            }

            /// <summary>
            /// effectif total du pion pour un ordre de mouvement
            /// Dans ce cas, on crée un effectif "artificiel" pour les pontonniers, les dépôts et les convois
            /// </summary>
            public int effectifTotalEnMouvement
            {
                get
                {
                    if (estDepot)
                    {
                        return Constantes.CST_ENCOMBREMENT_DEPOT[(int)(C_NIVEAU_DEPOT - 'A')] * this.nation.I_ENCOMBREMENT_INFANTERIE;// / Donnees.m_donnees.TAB_JEU[0].I_ECHELLE; 
                    }
                    if (estConvoiDeRavitaillement)
                    {
                        return Constantes.CST_ENCOMBREMENT_CONVOI_RAVITAILLEMENT * this.nation.I_ENCOMBREMENT_INFANTERIE;// / Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                    }
                    if (estPontonnier)
                    {
                        return Constantes.CST_ENCOMBREMENT_CONVOI_RAVITAILLEMENT * this.nation.I_ENCOMBREMENT_INFANTERIE;// / Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                    }

                    return infanterie + cavalerie + artillerie;
                    //return I_ARTILLERIE + I_CAVALERIE + I_INFANTERIE;
                }
                //set; 
            }

            //renvoie le moral effectif de l'unité en tenant compte de la fatigue
            public int Moral
            {
                get
                {
                    int moralFatigue = 0;
                    if (I_FATIGUE >= 20) { moralFatigue = 1; }
                    if (I_FATIGUE >= 40) { moralFatigue = 2; }
                    if (I_FATIGUE >= 50) { moralFatigue = 3; }
                    if (I_FATIGUE >= 60) { moralFatigue = 5; }
                    if (I_FATIGUE >= 80) { moralFatigue = 15; }
                    return Math.Max(0, I_MORAL - moralFatigue);
                }
            }

            /// <summary>
            /// Renvoie le nombre de fantassins en tenant compte de la fatigue
            /// </summary>
            public int infanterie
            {
                get { return (int)Math.Round((decimal)I_INFANTERIE * (100 - I_FATIGUE) / 100); }
            }

            /// <summary>
            /// Renvoie le nombre de cavaliers en tenant compte de la fatigue
            /// </summary>
            public int cavalerie
            {
                get { return (int)Math.Round((decimal)I_CAVALERIE * (100 - I_FATIGUE) / 100); }
            }

            /// <summary>
            /// Renvoie le nombre de canons en tenant compte de la fatigue
            /// </summary>
            public int artillerie
            {
                get { return (int)Math.Round((decimal)I_ARTILLERIE * (100 - I_FATIGUE) / 100); }
            }

            /// <summary>
            /// renvoi le modèle du pion
            /// </summary>
            /// <returns></returns>
            public TAB_MODELE_PIONRow modelePion
            {
                get
                {
                    return m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION(ID_MODELE_PION);
                }
            }

            /// <summary>
            /// rennvoie le proprietaire du pion
            /// </summary>
            public TAB_PIONRow proprietaire
            {
                get
                {
                    return m_donnees.TAB_PION.FindByID_PION(ID_PION_PROPRIETAIRE);
                }
            }

            /// <summary>
            /// rennvoie l'escorte du pion (qui est donc un convoi de prisonniers)
            /// </summary>
            public TAB_PIONRow escorte
            {
                get
                {
                    int idPionEscorte = ID_PION_ESCORTE;
                    return (Constantes.NULLENTIER == idPionEscorte) ? null : m_donnees.TAB_PION.FindByID_PION(idPionEscorte);
                }
            }

            /// <summary>
            /// renvoie la nation du pion
            /// </summary>
            /// <returns>identifiant de la nation du pion, -1 si non trouvé</returns>
            public int idNation
            {
                get
                {
                    TAB_MODELE_PIONRow ligneModele = modelePion;
                    if (null == ligneModele) { return -1; }
                    return ligneModele.ID_NATION;
                }
            }

            /// <summary>
            /// Indique si un pion est joué par un joueur réel
            /// </summary>
            /// <returns>true si joué directement, false sinon</returns>
            public bool estJoueur
            {
                get
                {
                    //le pion est joué par un joueur s'il y a un role associé
                    string requete = string.Format("ID_PION={0}", this.ID_PION);

                    TAB_ROLERow[] resRole = (TAB_ROLERow[])m_donnees.tableTAB_ROLE.Select(requete);

                    if (0 == resRole.Length)
                    {
                        return false;
                    }
                    return true;
                }
            }

            /// <summary>
            /// Indique si le pion est engagé dans un combat ou pas
            /// </summary>
            /// <returns>true si l'unité est engagée au combat, false sinon</returns>
            public bool estAuCombat
            {
                //une unité est actuellement engagée si l'identifiant de la bataille est renseignée
                //la table TAB_BATAILLE_PIONS indique dans quel combat l'unité pourrait s'engager et si elle y a 
                //participée avec B_ENGAGEMENT
                get
                {
                    if (IsID_BATAILLENull() || ID_BATAILLE <= 0)
                    {
                        return false;
                    }
                    return true;
                }
            }

            /// <summary>
            /// Indique si le pion est en mouvement ou pas
            /// </summary>
            /// <returns>true si l'unité est en mouvement, false sinon</returns>
            public bool estStatique
            {
                get
                {
                    TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(ID_PION);
                    //if (null == ligneOrdre || lignePion.estEngageeAuCombat())
                    if (null == ligneOrdre || estAuCombat || (null != ligneOrdre && !OrdreActif(ligneOrdre)))
                    {
                        return true;
                    }
                    return false;
                }
            }

            /// <summary>
            /// indique si le pion a l'aptitude QG ou pas 
            /// </summary>
            /// <returns>true si QG false sinon</returns>
            public bool estQG
            {
                get
                {
                    return possedeAptitude("QG");
                }
            }

            /// <summary>
            /// indique si le pion a l'aptitude PONTONNIER ou pas 
            /// </summary>
            /// <returns>true si PONTONNIER false sinon</returns>
            public bool estPontonnier
            {
                get
                {
                    return possedeAptitude("PONTONNIER");
                }
            }

            /// <summary>
            /// indique si le pion a l'aptitude CONVOI ou pas 
            /// </summary>
            /// <returns>true si CONVOI false sinon</returns>
            public bool estConvoi
            {
                get
                {
                    return possedeAptitude("CONVOI");
                }
            }

            /// <summary>
            /// indique si le pion a l'aptitude CONVOI + un niveau de dépôt ou pas 
            /// </summary>
            /// <returns>true si CONVOI de ravitaillement false sinon</returns>
            public bool estConvoiDeRavitaillement
            {
                get
                {
                    return estConvoi && (C_NIVEAU_DEPOT == 'A' || C_NIVEAU_DEPOT == 'B' || C_NIVEAU_DEPOT == 'C' || C_NIVEAU_DEPOT == 'D');
                }
            }

            /// <summary>
            /// indique si le pion a l'aptitude CONVOI + B_BLESSES ou pas 
            /// </summary>
            /// <returns>true si CONVOI de ravitaillement false sinon</returns>
            public bool estBlesses
            {
                get
                {
                    //return estConvoi && B_BLESSES;
                    return B_BLESSES;//en fait, les "convois" de blessés sont des divisions ! Ils ne doivent pas avancer à la vitesse d'un convoi
                }
            }

            /// <summary>
            /// indique si le pion a l'aptitude CONVOI + B_PRISONNIERS ou pas 
            /// </summary>
            /// <returns>true si CONVOI de ravitaillement false sinon</returns>
            public bool estPrisonniers
            {
                get
                {
                    //return estConvoi && B_PRISONNIERS;
                    return B_PRISONNIERS;//en fait, les "convois" de prisonniers sont des divisions ! Ils ne doivent pas avancer à la vitesse d'un convoi
                }
            }

            /// <summary>
            /// indique si le pion a l'aptitude messager ou pas
            /// </summary>
            /// <returns>true si Messager false sinon</returns>
            public bool estMessager
            {
                get
                {
                    if (possedeAptitude("MESSAGER") || possedeAptitude("PATROUILLEMESSAGER"))
                    {
                        return true;
                    }
                    return false;
                }
            }

            /// <summary>
            /// indique si le pion est un renfort ou pas
            /// </summary>
            /// <returns>true si renfort false sinon</returns>
            public bool estRenfort
            {
                get
                {
                    return B_RENFORT;
                }
            }

            /// <summary>
            /// indique si le pion a l'aptitude depot ou pas
            /// </summary>
            /// <returns>true si Depot false sinon</returns>
            public bool estDepot
            {
                get
                {
                    if (possedeAptitude("DEPOT"))
                    {
                        return true;
                    }
                    return false;
                }
            }

            /// <summary>
            /// indique si le pion a l'aptitude patrouille ou pas
            /// </summary>
            /// <returns>true si Patrouille false sinon</returns>
            public bool estPatrouille
            {
                get
                {
                    if (possedeAptitude("PATROUILLE") || possedeAptitude("PATROUILLEMESSAGER"))
                    {
                        return true;
                    }
                    return false;
                }
            }

            /// <summary>
            /// indique si le pion doit être ravitaillé ou pas
            /// </summary>
            /// <returns>false si l'unité n'a pas besoin de ravitaillement, true sinon</returns>
            public bool estRavitaillable
            {
                get
                {
                    if (B_DETRUIT)
                    {
                        return false;
                    }

                    if (estQG || estMessager || estPatrouille || estDepot || estConvoi || estPontonnier)
                    {
                        return false;
                    }

                    if (I_CAVALERIE == 0 && I_INFANTERIE == 0)
                    {
                        //on ne ravitaille pas les unités d'artillerie pure
                        return false;
                    }

                    return true;
                }
            }

            /// <summary>
            /// indique si le pion envoie un rapport de position même si ce n'est pas une unité ravitaillable
            /// </summary>
            /// <returns>false si l'unité n'a pas besoin de ravitaillement, true sinon</returns>
            public bool estRapportQuotidienHorsRavitaillement
            {
                get
                {
                    if (B_DETRUIT)
                    {
                        return false;
                    }

                    if (estDepot || estConvoi || estPontonnier)
                    {
                        return true;
                    }

                    if (I_CAVALERIE == 0 && I_INFANTERIE == 0 && I_ARTILLERIE > 0)
                    {
                        return true;
                    }

                    return false;
                }
            }

            #endregion

            //static protected AStar m_star = null;
            //internal AStar etoile
            //{
            //    get
            //    {
            //        if (null == m_star)
            //        {
            //            m_star = new AStar();
            //        }
            //        return m_star;
            //    }
            //}

            public int vision
            {
                get
                {
                    Donnees.TAB_MODELE_PIONRow ligneModelePion = this.modelePion;
                    if (null == ligneModelePion)
                    {
                        return -1;
                    }
                    if (!Donnees.m_donnees.TAB_PARTIE.Nocturne())
                    {
                        return ligneModelePion.I_VISION_JOUR;
                    }
                    else
                    {
                        return ligneModelePion.I_VISION_NUIT;
                    }
                }
            }

            /// <summary>
            /// calcul du cadre de vision du pion
            /// </summary>
            /// <param name="ligneCase">si null, prend la case du pion</param>
            /// <param name="xCaseHautGauche">abscisse supérieur gauche</param>
            /// <param name="yCaseHautGauche">ordonnée supérieur gauche</param>
            /// <param name="xCaseBasDroite">abscisse inférieure droite</param>
            /// <param name="yCaseBasDroite">ordonnée inférieure droite</param>
            public void CadreVision(Donnees.TAB_CASERow ligneCase, out int xCaseHautGauche, out int yCaseHautGauche, out int xCaseBasDroite, out int yCaseBasDroite)
            {
                int visionPixel;
                visionPixel = vision * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;

                //un joueur pouvant toujours voir les unités en bataille, autant continuer à lui donner les informations du cadre
                if (estAuCombat && !estJoueur)
                {
                    Donnees.TAB_BATAILLERow ligneBataille = Donnees.m_donnees.TAB_BATAILLE.FindByID_BATAILLE(this.ID_BATAILLE);
                    xCaseHautGauche = ligneBataille.I_X_CASE_HAUT_GAUCHE;
                    yCaseHautGauche = ligneBataille.I_Y_CASE_HAUT_GAUCHE;
                    xCaseBasDroite = ligneBataille.I_X_CASE_BAS_DROITE;
                    yCaseBasDroite = ligneBataille.I_Y_CASE_BAS_DROITE;
                }
                else
                {
                    Donnees.TAB_CASERow ligneCaseBase = (null == ligneCase) ? Donnees.m_donnees.TAB_CASE.FindByID_CASE(this.ID_CASE) : ligneCase;
                    xCaseHautGauche = Math.Max(0, ligneCaseBase.I_X - visionPixel);
                    yCaseHautGauche = Math.Max(0, ligneCaseBase.I_Y - visionPixel);
                    xCaseBasDroite = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1, ligneCaseBase.I_X + visionPixel);
                    yCaseBasDroite = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1, ligneCaseBase.I_Y + visionPixel);
                }
            }

            /// <summary>
            /// Indique si le pion a un ennemi dans son cadre de vision
            /// </summary>
            /// <param name="ligneCase"></param>
            /// <returns>true si un ennemi est en vu, false sinon</returns>
            public bool EnnemiObservable(Donnees.TAB_CASERow ligneCase)
            {
                bool bEnnemiObservable = false;
                //string requete;
                int xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite;

                CadreVision(ligneCase, out xCaseHautGauche, out yCaseHautGauche, out xCaseBasDroite, out yCaseBasDroite);

                Donnees.TAB_CASERow[] ligneCaseVues = Donnees.m_donnees.TAB_CASE.CasesCadre(xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite);
                Donnees.TAB_MODELE_PIONRow ligneModelePion = this.modelePion;
                foreach (Donnees.TAB_CASERow ligneCaseVue in ligneCaseVues)
                {
                    if (this.estEnnemi(ligneCaseVue, ligneModelePion, true, true))
                    {
                        return true;
                    }
                }

                return bEnnemiObservable;
            }

            public void CalculerRepartitionEffectif(int effectif, out int iINFANTERIE, out int iCAVALERIE, out int iARTILLERIE)
            {
                int iInfanterie;

                if (estDepot || estConvoiDeRavitaillement || estPontonnier)
                {

                    iInfanterie = (effectif == 0) ? effectifTotalEnMouvement : effectif;
                }
                else
                {
                    iInfanterie = I_INFANTERIE;
                }

                CalculerRepartitionEffectif(iInfanterie, I_CAVALERIE, I_ARTILLERIE, effectif, out iINFANTERIE, out iCAVALERIE, out iARTILLERIE);
            }

            public void CalculerRepartitionEffectif(int iINFANTERIESource, int iCAVALERIESource, int iARTILLERIESource, int effectif, out int iINFANTERIE, out int iCAVALERIE, out int iARTILLERIE)
            {
                //on considère que la cavalerie arrive en premier, l'infanterie en second, l'artillerie au final
                if (effectif > iCAVALERIESource)
                {
                    iCAVALERIE = iCAVALERIESource;
                }
                else
                {
                    iCAVALERIE = effectif;
                }

                if (effectif > iCAVALERIESource + iINFANTERIESource)
                {
                    iINFANTERIE = iINFANTERIESource;
                    iARTILLERIE = effectif - iCAVALERIESource - iINFANTERIESource;
                }
                else
                {
                    iINFANTERIE = effectif - iCAVALERIE;
                    iARTILLERIE = 0;
                }

                string messageErreur;
                string message = string.Format("CalculerRepartitionEffectif iINFANTERIESource={0}, iCAVALERIESource={1}, iARTILLERIESource={2}, effectif={3}, iINFANTERIE={4}, iCAVALERIE={5}, iARTILLERIE={6}",
                    iINFANTERIESource, iCAVALERIESource, iARTILLERIESource, effectif, iINFANTERIE, iCAVALERIE, iARTILLERIE);
                LogFile.Notifier(message, out messageErreur);
            }

            public bool CalculerEffectif(Donnees.TAB_NATIONRow ligneNation, decimal encombrement, bool enMouvement, out int iINFANTERIE, out int iCAVALERIE, out int iARTILLERIE)
            {
                int iInfanterie;

                iInfanterie = (enMouvement && (estDepot || estConvoiDeRavitaillement || estPontonnier)) ? effectifTotalEnMouvement : I_INFANTERIE;

                return CalculerEffectif(ligneNation, iInfanterie, I_CAVALERIE, I_ARTILLERIE, encombrement, enMouvement, out iINFANTERIE, out iCAVALERIE, out iARTILLERIE);
            }

            /// <summary>
            /// Calcule la part de chaque type d'éléments du pion qui constitue l'encombrement
            /// </summary>
            /// <param name="ligneNation">nation à laquelle appartient les éléments</param>
            /// <param name="lignePion">pion auquel appartient les éléments</param>
            /// <param name="iInfanterieInitial">nombre de fantassins</param>
            /// <param name="iCavalerieInitial">nombre de cavaliers</param>
            /// <param name="iArtillerieInitial">nombre d'artillerie</param>
            /// <param name="encombrement">nombre de points (pixels) d'encombrement</param>
            /// <param name="enMouvement">true si en mouvement, false sinon</param>
            /// <param name="iINFANTERIE">infanterie constituant l'encombrement</param>
            /// <param name="iCAVALERIE">cavalerie constituant l'encombrement</param>
            /// <param name="iARTILLERIE">artillerie constituant l'encombrement</param>
            /// <returns>true si OK, false si KO</returns>
            public bool CalculerEffectif(Donnees.TAB_NATIONRow ligneNation,
                int iInfanterieInitial, int iCavalerieInitial, int iArtillerieInitial, decimal encombrement, bool enMouvement, out int iINFANTERIE, out int iCAVALERIE, out int iARTILLERIE)
            {
                int encombrementCavalerie, encombrementInfanterie;
                int effectif;
                int echelle = Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                decimal encombrementReel;
                int rapportMoral = 100;

                iINFANTERIE = iCAVALERIE = iARTILLERIE = 0;

                // on retire les fourgons, on tenant compte, en même temps, du "bonus" moral, pour éviter les problèmes d'arrondis si cela est fait séparement
                if (estDepot || estConvoiDeRavitaillement || estPontonnier)
                {
                    encombrementReel = encombrement;//pas de prise en compte du moral ou des fourgons pour ces unités
                }
                else
                {
                    rapportMoral = (I_MORAL > 0) ? 100 : 50;
                    encombrementReel = (decimal)(encombrement * rapportMoral) / (100 + ligneNation.I_FOURGON);
                }

                if (enMouvement)
                {
                    //encombrement sur route
                    encombrementCavalerie = iCavalerieInitial * (100 - I_FATIGUE) / 100 * echelle / ligneNation.I_ENCOMBREMENT_CAVALERIE;
                    if (encombrementReel < encombrementCavalerie)
                    {
                        //la cavalerie occupe tout l'espace
                        iCAVALERIE = (int)(encombrementReel * ligneNation.I_ENCOMBREMENT_CAVALERIE / echelle);
                        return true;
                    }
                    else
                    {
                        //on inclue les "fatigués" puisqu'ils sont retirés lors du calcul de l'encombrement
                        iCAVALERIE = iCavalerieInitial;
                        encombrementReel -= encombrementCavalerie;
                    }

                    encombrementInfanterie = iInfanterieInitial * (100 - I_FATIGUE) / 100 * echelle / ligneNation.I_ENCOMBREMENT_INFANTERIE;
                    if (encombrementReel < encombrementInfanterie)
                    {
                        //l'infanterie occupe tout l'espace restant
                        iINFANTERIE = (int)(encombrementReel * ligneNation.I_ENCOMBREMENT_INFANTERIE / echelle);
                        return true;
                    }
                    else
                    {
                        //on inclue les "fatigués" puisqu'ils sont retirés lors du calcul de l'encombrement
                        iINFANTERIE = iInfanterieInitial;
                        encombrementReel -= encombrementInfanterie;
                    }
                    //l'artillerie occupe ce qui reste
                    iARTILLERIE = (int)Math.Round(encombrementReel * ligneNation.I_ENCOMBREMENT_ARTILLERIE / echelle);
                }
                else
                {
                    //encombrement en statique
                    effectif = (int)(ligneNation.I_ENCOMBREMENT_ARRET * encombrement / (echelle * echelle));
                    CalculerRepartitionEffectif(effectif, out iINFANTERIE, out iCAVALERIE, out iARTILLERIE);
                }

                return true;
            }

            public string DescriptifOrdreEnCours(int tour, int phase)
            {
                string sOrdreCourant, requete;
                //recherche de l'ordre courant de l'unité au moment du dernier message
                //requete = string.Format("ID_PION={0} AND I_TOUR_DEBUT<={1} AND I_PHASE_DEBUT<={2} AND ((I_TOUR_FIN IS NULL AND I_PHASE_FIN IS NULL) OR (I_TOUR_FIN>{1} AND I_PHASE_FIN>{2}))",
                //    lignePion.ID_PION, ligneMessage.I_TOUR_DEPART, ligneMessage.I_PHASE_DEPART);
                //string requete = string.Format("ID_PION={0} AND I_TOUR_DEBUT<={1} AND ((I_TOUR_FIN IS NULL) OR (I_TOUR_FIN>{1}))", lignePion.ID_PION, tour);
                requete = string.Format("ID_PION={0} AND I_TOUR_DEBUT<={1} AND ((I_TOUR_FIN = {3}) OR (I_TOUR_FIN>{1}) OR (I_TOUR_FIN={1} AND I_PHASE_FIN>{2}))",
                                             this.ID_PION, tour, phase, Constantes.NULLENTIER);
                Donnees.TAB_ORDRERow[] resOrdre = (Donnees.TAB_ORDRERow[])Donnees.m_donnees.TAB_ORDRE.Select(requete, "ID_ORDRE");
                if (0 == resOrdre.Length)
                {
                    sOrdreCourant = "aucun ordre";
                }
                else
                {
                    sOrdreCourant = string.Empty;
                    int o = 0;
                    while (sOrdreCourant == string.Empty && o < resOrdre.Length)
                    {
                        //c'est seulement si l'ordre est dans le même tour que le message que la valeur de la phase intervient
                        if ((resOrdre[o].I_TOUR_DEBUT != tour) ||
                            (resOrdre[o].I_PHASE_DEBUT <= phase && (resOrdre[o].IsI_PHASE_FINNull() || resOrdre[o].I_PHASE_FIN > phase)))
                        {
                            sOrdreCourant = ClassMessager.MessageDecrivantUnOrdre(resOrdre[o], false);
                        }
                        o++;
                    }
                }
                return sOrdreCourant;
            }

            /// <summary>
            /// indique si l'ordre de l'unité est dans le créneau actif en ce moment
            /// </summary>
            /// <param name="ligneOrdre">ordre à tester</param>
            /// <returns>true si actif, false sinon </returns>
            public bool OrdreActif(TAB_ORDRERow ligneOrdre)
            {
                string message;

                if (null == ligneOrdre) { return false; }

                //on vérifie si l'on est bien dans le créneau horaire souhaité
                //s'il n'y a pas d'heure définie c'est que l'unité bouge tout le temps (messager par exemple)
                if (!ligneOrdre.IsI_HEURE_DEBUTNull() && ligneOrdre.I_DUREE > 0)
                {
                    if (ligneOrdre.I_HEURE_DEBUT < (ligneOrdre.I_HEURE_DEBUT + ligneOrdre.I_DUREE) % 24)
                    {
                        if (ligneOrdre.I_HEURE_DEBUT > Donnees.m_donnees.TAB_PARTIE.HeureCourante())
                        {
                            message = string.Format("{0}(ID={1}, l'ordre ID={2} doit être executé à partir de {3}, et il est {4})",
                                S_NOM,
                                ID_PION,
                                ligneOrdre.ID_ORDRE,
                                ligneOrdre.I_HEURE_DEBUT,
                                Donnees.m_donnees.TAB_PARTIE.HeureCourante());
                            LogFile.Notifier(message);
                            return false;
                        }
                        if ((ligneOrdre.I_HEURE_DEBUT + ligneOrdre.I_DUREE - 1) % 24 < Donnees.m_donnees.TAB_PARTIE.HeureCourante())
                        {
                            message = string.Format("{0}(ID={1}, l'ordre ID={2} doit être executé jusqu'à {3}({4}+{5} modulo 24={6}, et il est {7})",
                                S_NOM,
                                ID_PION,
                                ligneOrdre.ID_ORDRE,
                                ligneOrdre.I_HEURE_DEBUT + ligneOrdre.I_DUREE,
                                ligneOrdre.I_HEURE_DEBUT,
                                ligneOrdre.I_DUREE,
                                (ligneOrdre.I_HEURE_DEBUT + ligneOrdre.I_DUREE) % 24,
                                Donnees.m_donnees.TAB_PARTIE.HeureCourante());
                            LogFile.Notifier(message);
                            return false;
                        }
                    }
                    else
                    {
                        //cas d'une de marche de nuit ou d'une durée sur 24 heures (possible pour les QGs)
                        if ((ligneOrdre.I_HEURE_DEBUT + ligneOrdre.I_DUREE - 1) % 24 < Donnees.m_donnees.TAB_PARTIE.HeureCourante()
                            && ligneOrdre.I_HEURE_DEBUT > Donnees.m_donnees.TAB_PARTIE.HeureCourante())
                        {
                            message = string.Format("{0}(ID={1}, cas 2, l'ordre ID={2} doit être executé de {3} jusqu'à ({4}+{5} modulo 24={6}, et il est {7})",
                                S_NOM,
                                ID_PION,
                                ligneOrdre.ID_ORDRE,
                                ligneOrdre.I_HEURE_DEBUT,
                                ligneOrdre.I_HEURE_DEBUT,
                                ligneOrdre.I_DUREE,
                                (ligneOrdre.I_HEURE_DEBUT + ligneOrdre.I_DUREE) % 24,
                                Donnees.m_donnees.TAB_PARTIE.HeureCourante());
                            LogFile.Notifier(message);
                            return false;
                        }
                    }
                }
                return true;
            }

            /// <summary>
            /// indique si la case est occupée par un ennemi
            /// </summary>
            /// <param name="ligneCase">case recherchée</param>
            /// <returns>true si ennemi, false sinon</returns>
            public bool estEnnemi(TAB_CASERow ligneCase)
            {
                return estEnnemi(ligneCase, false);
            }

            /// <summary>
            /// indique si la case est occupée par un ennemi
            /// </summary>
            /// <param name="ligneCase">case recherchée</param>
            /// <param name="bCombattif">true, l'ennemi doit être combattif, false=n'importe quel ennemi</param>
            /// <returns>true si ennemi, false sinon</returns>
            public bool estEnnemi(TAB_CASERow ligneCase, bool bCombattif)
            {
                TAB_MODELE_PIONRow ligneModele;

                ligneModele = this.modelePion;
                if (null == ligneModele)
                {
                    throw new Exception("TAB_PIONRow.estEnnemi impossible de trouver le modèle du pion source");
                }
                return (estEnnemi(ligneCase, ligneModele, bCombattif));
            }

            public bool estEnnemi(TAB_CASERow ligneCase, TAB_MODELE_PIONRow ligneModele, bool bCombattif)
            {
                return estEnnemi(ligneCase, ligneModele, bCombattif, false);
            }

            public bool estEnnemi(TAB_CASERow ligneCase, TAB_MODELE_PIONRow ligneModele, bool bCombattif, bool combattifSansMoral)
            {
                TAB_MODELE_PIONRow ligneModeleAdversaire;
                TAB_PIONRow lignePionAdversaire;

                Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                int IdProprietaire = ligneCase.ID_PROPRIETAIRE;
                if (Constantes.NULLENTIER != IdProprietaire)
                {
                    lignePionAdversaire = m_donnees.tableTAB_PION.FindByID_PION(IdProprietaire);
                    if (null == lignePionAdversaire)
                    {
                        Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                        return false; //cas possible si j'ai détruit une unité manuellement
                    }
                    if (lignePionAdversaire.B_DETRUIT)
                    {
                        Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                        return false;
                    }
                    ligneModeleAdversaire = lignePionAdversaire.modelePion;
                    if (null == ligneModeleAdversaire)
                    {
                        Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                        throw new Exception("TAB_PIONRow.estEnnemi impossible de trouver le modèle du premier pion");
                    }
                    if ((ligneModele.ID_NATION != ligneModeleAdversaire.ID_NATION) && ((bCombattif && lignePionAdversaire.estCombattifQG(false, combattifSansMoral)) || !bCombattif))
                    {
                        if (bCombattif)
                        {
                            Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                            return lignePionAdversaire.estCombattifQG(false, combattifSansMoral);
                        }
                        Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                        return true;
                    }
                }
                int IdNouveauProprietaire = ligneCase.ID_NOUVEAU_PROPRIETAIRE;
                if (Constantes.NULLENTIER != IdNouveauProprietaire)
                {
                    lignePionAdversaire = m_donnees.tableTAB_PION.FindByID_PION(IdNouveauProprietaire);
                    ligneModeleAdversaire = lignePionAdversaire.modelePion;
                    if (null == ligneModeleAdversaire)
                    {
                        Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                        throw new Exception("TAB_PIONRow.estEnnemi impossible de trouver le modèle du premier pion");
                    }
                    if ((ligneModele.ID_NATION != ligneModeleAdversaire.ID_NATION) && ((bCombattif && lignePionAdversaire.estCombattifQG(false, combattifSansMoral)) || !bCombattif))
                    {
                        Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                        return lignePionAdversaire.estCombattifQG(false, combattifSansMoral);
                    }
                }
                Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                return false;
            }

            /// <summary>
            /// indique si deux pions sont amis ou ennemis
            /// </summary>
            /// <param name="ligneAdversaire">pion adversaire potentiel</param>
            /// <returns>true si les deux pions sont ennemis, false sinon</returns>
            public bool estEnnemi(TAB_PIONRow ligneAdversaire)
            {
                TAB_MODELE_PIONRow ligneModele, ligneModeleAdversaire;
                ligneModele = this.modelePion;
                ligneModeleAdversaire = ligneAdversaire.modelePion;
                if (null == ligneModele || null == ligneModeleAdversaire)
                {
                    throw new Exception("TAB_PIONRow.estEnnemi impossible de trouver le modèle d'un des deux pions");
                }
                return ligneModele.ID_NATION != ligneModeleAdversaire.ID_NATION;
            }

            /// <summary>
            /// indique si deux pions sont amis ou ennemis
            /// </summary>
            /// <param name="idPionAdversaire">id du pion adversaire potentiel</param>
            /// <returns>true si les deux pions sont ennemis, false sinon</returns>
            public bool estEnnemi(int idPionAdversaire)
            {
                TAB_PIONRow ligneAdversaire;

                ligneAdversaire = m_donnees.TAB_PION.FindByID_PION(idPionAdversaire);

                if (null == ligneAdversaire)
                {
                    throw new Exception(string.Format("TAB_PIONRow.estEnnemi impossible de trouver la ligne pour ID={0}", idPionAdversaire));
                }
                return estEnnemi(ligneAdversaire);
            }

            /// Si la case est occupée par un ennemi, renvoie le pion de l'ennemi
            /// </summary>
            /// <param name="ligneCase">case recherchée</param>
            /// <returns>true si ennemi, false sinon</returns>
            public TAB_PIONRow rechercheEnnemi(TAB_CASERow ligneCase)
            {
                TAB_MODELE_PIONRow ligneModele, ligneModeleAdversaire;
                TAB_PIONRow lignePionAdversaire;

                ligneModele = this.modelePion;
                if (null == ligneModele)
                {
                    throw new Exception("TAB_PIONRow.rechercheEnnemi impossible de trouver le modèle du pion source");
                }
                int IdProprietaire = ligneCase.ID_PROPRIETAIRE;
                if (Constantes.NULLENTIER != IdProprietaire)
                {
                    lignePionAdversaire = m_donnees.tableTAB_PION.FindByID_PION(IdProprietaire);
                    ligneModeleAdversaire = lignePionAdversaire.modelePion;
                    if (null == ligneModeleAdversaire)
                    {
                        throw new Exception("TAB_PIONRow.rechercheEnnemi impossible de trouver le modèle du premier pion");
                    }
                    if (ligneModele.ID_NATION != ligneModeleAdversaire.ID_NATION)
                    {
                        return lignePionAdversaire;
                    }
                }
                int IdNouveauProprietaire = ligneCase.ID_NOUVEAU_PROPRIETAIRE;
                if (Constantes.NULLENTIER != IdNouveauProprietaire)
                {
                    lignePionAdversaire = m_donnees.tableTAB_PION.FindByID_PION(IdNouveauProprietaire);
                    ligneModeleAdversaire = lignePionAdversaire.modelePion;
                    if (null == ligneModeleAdversaire)
                    {
                        throw new Exception("TAB_PIONRow.rechercheEnnemi impossible de trouver le modèle du premier pion");
                    }
                    if (ligneModele.ID_NATION != ligneModeleAdversaire.ID_NATION)
                    {
                        return lignePionAdversaire;
                    }
                }
                return null;
            }

            /// <summary>
            /// Recherche si un ennemi d'un pion est à proximité d'une case donnée à une distance de CST_DISTANCE_RECHERCHE_ENNEMI
            /// </summary>
            /// <param name="ligneCase">case source de la recherche</param>
            /// <param name="lignePion">pion dont on chercher un ennemi</param>
            /// <returns>true, il y a un ennemi, false, il n'y en a pas</returns>
            private bool RechercheEnnemiAProximite(Donnees.TAB_CASERow ligneCase)
            {
                int xCaseHautGauche = Math.Max(0, ligneCase.I_X - Constantes.CST_DISTANCE_RECHERCHE_ENNEMI * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE);
                int yCaseHautGauche = Math.Max(0, ligneCase.I_Y - Constantes.CST_DISTANCE_RECHERCHE_ENNEMI * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE);
                int xCaseBasDroite = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1, ligneCase.I_X + Constantes.CST_DISTANCE_RECHERCHE_ENNEMI * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE);
                int yCaseBasDroite = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1, ligneCase.I_Y + Constantes.CST_DISTANCE_RECHERCHE_ENNEMI * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE);
                Donnees.TAB_CASERow[] lignesCaseRecherche = Donnees.m_donnees.TAB_CASE.CasesCadre(xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite);

                //on recherche si un ennemi de l'unité se trouve proche de la case indiquée
                foreach (Donnees.TAB_CASERow ligneCaseRecherche in lignesCaseRecherche)
                {
                    if (estEnnemi(ligneCaseRecherche, true))
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// indique si la case est occupée par un ami
            /// </summary>
            /// <param name="ligneCase">case recherchée</param>
            /// <param name="bCombattif">true, l'ami doit être combattif, false=n'importe quel ennemi</param>
            /// <returns>true si ami, false sinon</returns>
            public bool estAmi(TAB_CASERow ligneCase, bool bCombattif)
            {
                TAB_MODELE_PIONRow ligneModele, ligneModeleOccupe;
                TAB_PIONRow lignePionOccupe;

                ligneModele = this.modelePion;
                if (null == ligneModele)
                {
                    throw new Exception("TAB_PIONRow.estAmi impossible de trouver le modèle du pion source");
                }
                int IdProprietaire = ligneCase.ID_PROPRIETAIRE;
                if (Constantes.NULLENTIER != IdProprietaire)
                {
                    lignePionOccupe = m_donnees.tableTAB_PION.FindByID_PION(IdProprietaire);
                    if (null == lignePionOccupe)
                    {
                        return false; //cas possible si j'ai détruit une unité manuellement
                    }
                    ligneModeleOccupe = lignePionOccupe.modelePion;
                    if (null == ligneModeleOccupe)
                    {
                        throw new Exception("TAB_PIONRow.estAmi impossible de trouver le modèle du premier pion");
                    }
                    if (ligneModele.ID_NATION == ligneModeleOccupe.ID_NATION && ((bCombattif && lignePionOccupe.estCombattif) || !bCombattif))
                    {
                        return true;
                    }
                }
                int IdNouveauProprietaire = ligneCase.ID_NOUVEAU_PROPRIETAIRE;
                if (Constantes.NULLENTIER != IdNouveauProprietaire)
                {
                    lignePionOccupe = m_donnees.tableTAB_PION.FindByID_PION(IdNouveauProprietaire);
                    ligneModeleOccupe = lignePionOccupe.modelePion;
                    if (null == ligneModeleOccupe)
                    {
                        throw new Exception("TAB_PIONRow.estAmi impossible de trouver le modèle du premier pion");
                    }
                    if (ligneModele.ID_NATION == ligneModeleOccupe.ID_NATION && ((bCombattif && lignePionOccupe.estCombattif) || !bCombattif))
                    {
                        return true;
                    }
                }
                return false;
            }

            public bool possedeAptitude(string nomAptitude)
            {
                string requete;

                //recherche le modèle du pion
                requete = string.Format("ID_MODELE_PION={0}", this.ID_MODELE_PION);
                Monitor.Enter(m_donnees.TAB_MODELE_PION.Rows.SyncRoot); //select n'est pas thread safe
                TAB_MODELE_PIONRow[] resModelePion = (TAB_MODELE_PIONRow[])m_donnees.TAB_MODELE_PION.Select(requete);
                Monitor.Exit(m_donnees.TAB_MODELE_PION.Rows.SyncRoot);

                //recherche l'aptitude fournie en paramètre
                requete = string.Format("S_NOM='{0}'", nomAptitude);
                Monitor.Enter(m_donnees.TAB_APTITUDES.Rows.SyncRoot);
                TAB_APTITUDESRow[] resAptitude = (TAB_APTITUDESRow[])m_donnees.TAB_APTITUDES.Select(requete);
                Monitor.Exit(m_donnees.TAB_APTITUDES.Rows.SyncRoot);

                //recherche si le modele de pion possède l'aptitude demandée
                if (0 == resModelePion.Count() || 0 == resAptitude.Count())
                {
                    return false;
                }
                requete = string.Format("ID_MODELE_PION={0} AND ID_APTITUDE={1}", resModelePion[0].ID_MODELE_PION, resAptitude[0].ID_APTITUDE);
                Monitor.Enter(m_donnees.TAB_APTITUDES_PION.Rows.SyncRoot);
                TAB_APTITUDES_PIONRow[] resAptitudesPion = (TAB_APTITUDES_PIONRow[])m_donnees.TAB_APTITUDES_PION.Select(requete);
                Monitor.Exit(m_donnees.TAB_APTITUDES_PION.Rows.SyncRoot);
                if (null == resAptitudesPion || 0 == resAptitudesPion.Length)
                {
                    return false;
                }
                return true;
            }

            /// <summary>
            /// indique si le pion a l'aptitude QG ou pas et son niveau hierarchique
            /// </summary>
            /// <param name="niveauHierarchique">niveau hiérarchique de l'unité</param>
            /// <returns>true si QG false sinon</returns>
            public bool estQGHierarchique(out char niveauHierarchique)
            {
                niveauHierarchique = C_NIVEAU_HIERARCHIQUE;
                return possedeAptitude("QG");
            }

            /// <summary>
            /// Indique s'il s'agit uniquement d'une unité d'artillerie
            /// </summary>
            /// <returns>true si pur unité d'artillerie, false sinon</returns>
            public bool estArtillerie
            {
                get
                {
                    if (I_INFANTERIE > 0 || I_CAVALERIE > 0 || I_ARTILLERIE <= 0)
                        return false;
                    return true;
                }
            }

            /// <summary>
            /// indique si le pion est en état de se battre ou pas
            /// </summary>
            /// <returns>true si combattif false sinon</returns>
            public bool estCombattif
            {
                get
                {
                    return estCombattifQG(false, false);
                }
            }

            /// <summary>
            /// indique si le pion est en état de se battre ou pas
            /// </summary>
            /// <param name="QGcombattif">true si on considére le QG comme unité combattante, false sinon</param>
            /// <returns>true si combattif false sinon</returns>
            public bool estCombattifQG(bool QGcombattif, bool combattifSansMoral)
            {
                if (B_DETRUIT)
                {
                    return false;
                }

                if ((I_TOUR_FUITE_RESTANT > 0 || I_TOUR_RETRAITE_RESTANT > 0) && !combattifSansMoral)
                {
                    return false;
                }

                if (estMessager || estPatrouille || estDepot || estConvoi || estPontonnier || estBlesses || estPrisonniers || estRenfort)
                {
                    return false;
                }

                if (estQG)
                {
                    return QGcombattif;
                }

                if (combattifSansMoral)
                {
                    return true;
                }

                if (I_MATERIEL <= 0 || I_RAVITAILLEMENT <= 0)
                {
                    return false;
                }

                //Les unités de pur artillerie sont considérées comme combattives
                if (I_MORAL > 0 || (I_INFANTERIE == 0 && 0 == I_CAVALERIE))
                {
                    return true;
                }

                return false;
            }

            /// <summary>
            /// renvoie la case courante occupée par le pion
            /// </summary>
            /// <returns>la case, null si non trouvée</returns>
            public TAB_CASERow CaseCourante()
            {
                return m_donnees.TAB_CASE.FindByID_CASE(ID_CASE);
            }

            /// <summary>
            /// Calcul de l'encombrement des troupes en pixels
            /// note : les blessés occupent le même encombrement qu'une unité standard
            /// note : les prisonniers ont une escorte égale à 10% du nombre de prisonniers dont il faut tenir compte
            /// </summary>
            /// <param name="ligneNation">Nation à laquelle appartient les effectifs</param>
            /// <param name="effectifInfanterie">effectifs d'infanterie</param>
            /// <param name="effectifCavalerie">effectifs de cavalerie</param>
            /// <param name="effectifArtillerie">effectifs d'artillerie</param>
            /// <param name="moral">moral de l'unité</param>
            /// <param name="enMouvement">true si unité en mouvement, false si à l'arrêt</param>
            /// <returns>valeur d'encombrement en pixels</returns>
            public decimal CalculerEncombrement(Donnees.TAB_NATIONRow ligneNation, int effectifInfanterie, int effectifCavalerie, int effectifArtillerie, bool enMouvement)
            {
                decimal encombrement = 0;

                if (this.estDepot || this.estConvoiDeRavitaillement || this.estPontonnier)
                {
                    //je ne comprends pas ce cas, en plus, cela donne un encombrement nul quand les convois se déplacent et cela crée des bugs !
                    //sauf que si on le met en commentaire, les pontonniers en mouvement n'avancent plus avec une partie des forces arrivées à destination, hum...
                    //en effet, l'encombrement sur route devient 100-100 et donc les forces à destination ne progressent plus
                    if (enMouvement /*|| effectifInfanterie > 0*/)
                    {
                        //return immediat car on ne doit pas tenir compte du moral ou des fourgons pour les depôts et les pontonniers
                        return (int)((decimal)effectifInfanterie * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE / ligneNation.I_ENCOMBREMENT_INFANTERIE);
                    }
                    else
                    {
                        if (this.estDepot) { return Constantes.CST_ENCOMBREMENT_DEPOT[(int)(C_NIVEAU_DEPOT - 'A')] * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE; }
                        if (this.estConvoiDeRavitaillement) { return Constantes.CST_ENCOMBREMENT_CONVOI_RAVITAILLEMENT * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE; }
                        if (this.estPontonnier) { return Constantes.CST_ENCOMBREMENT_CONVOI_RAVITAILLEMENT * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE; }
                    }
                }

                if (enMouvement)
                {
                    //encombrement sur route
                    encombrement = ((decimal)effectifInfanterie * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE / ligneNation.I_ENCOMBREMENT_INFANTERIE
                        + (decimal)effectifCavalerie * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE / ligneNation.I_ENCOMBREMENT_CAVALERIE
                        + (decimal)effectifArtillerie * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE / ligneNation.I_ENCOMBREMENT_ARTILLERIE);
                }
                else
                {
                    //encombrement a l'arrêt
                    encombrement = (decimal)(effectifInfanterie + effectifCavalerie + effectifArtillerie) * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE / ligneNation.I_ENCOMBREMENT_ARRET;
                }

                if (this.estPrisonniers)
                {
                    //il faut ajouter les 10% supplémentaires liés à l'escorte, pas de distinction infanterie/cavalerie car si les prisonniers ont été capturés par une unité de cavalerie, ils vont être escortés 
                    // par des cavaliers et non des fantassins, donc, je fais "en gros"
                    //sinon, je pourrai aussi prendre la taille réelle de l'escorte mais c'est beaucoup de temps pour pas grand chose
                    encombrement += encombrement / 10;
                }

                // on ajoute les fourgons
                encombrement = ((100 + ligneNation.I_FOURGON) * encombrement) / 100;
                //on tient compte du moral
                if (I_MORAL <= 0) encombrement = encombrement * 2;

                //return (int)Math.Round(encombrement);
                return encombrement;
            }

            /// <summary>
            /// Calcul de l'encombrement des troupes en pixels
            /// </summary>
            /// <param name="ligneNation">Nation à laquelle appartient les effectifs</param>
            /// <param name="enMouvement">true si unité en mouvement, false si à l'arrêt</param>
            /// <returns>valeur d'encombrement en pixels</returns>
            public decimal CalculerEncombrement(Donnees.TAB_NATIONRow ligneNation, bool enMouvement)
            {
                return CalculerEncombrement(ligneNation, this.infanterie, this.cavalerie, this.artillerie, enMouvement);
            }

            internal decimal CalculVitesseMouvement()
            {
                decimal vitesse = int.MaxValue;
                string requete;

                requete = string.Format("ID_MODELE_PION={0}", ID_MODELE_PION);
                Monitor.Enter(Donnees.m_donnees.TAB_MODELE_PION.Rows.SyncRoot);
                Donnees.TAB_MODELE_PIONRow[] resModelePion = (Donnees.TAB_MODELE_PIONRow[])Donnees.m_donnees.TAB_MODELE_PION.Select(requete);
                Monitor.Exit(Donnees.m_donnees.TAB_MODELE_PION.Rows.SyncRoot);

                requete = string.Format("ID_MODELE_MOUVEMENT={0}", resModelePion[0].ID_MODELE_MOUVEMENT);
                Monitor.Enter(Donnees.m_donnees.TAB_MODELE_MOUVEMENT.Rows.SyncRoot);
                Donnees.TAB_MODELE_MOUVEMENTRow[] resModeleMouvement = (Donnees.TAB_MODELE_MOUVEMENTRow[])Donnees.m_donnees.TAB_MODELE_MOUVEMENT.Select(requete);
                Monitor.Exit(Donnees.m_donnees.TAB_MODELE_MOUVEMENT.Rows.SyncRoot);
                if (0 == I_INFANTERIE && 0 == I_CAVALERIE && 0 == I_ARTILLERIE)
                {
                    //unités sans effectif
                    vitesse = Math.Min(vitesse, resModeleMouvement[0].I_VITESSE_CAVALERIE);
                }
                else
                {
                    if (I_INFANTERIE > 0)
                    {
                        vitesse = Math.Min(vitesse, resModeleMouvement[0].I_VITESSE_INFANTERIE);
                    }
                    if (I_CAVALERIE > 0)
                    {
                        vitesse = Math.Min(vitesse, resModeleMouvement[0].I_VITESSE_CAVALERIE);
                    }
                    if (I_ARTILLERIE > 0)
                    {
                        vitesse = Math.Min(vitesse, resModeleMouvement[0].I_VITESSE_ARTILLERIE);
                    }
                }
                //les unités en déroute (moral nul) bouge de 1 km/h plus  vite
                return (I_MORAL>0) ? vitesse : vitesse+1;
            }

            internal string ChaineAppartenance()
            {
                string retour = string.Empty;
                Donnees.TAB_PIONRow lignePionProprietaire = this;
                while (!lignePionProprietaire.estJoueur)
                {
                    lignePionProprietaire = lignePionProprietaire.proprietaire;
                    string nomProprietaire = lignePionProprietaire.S_NOM;
                    string formatChaine = (Constantes.DebuteParUneVoyelle(nomProprietaire)) ? " de l'{0}" : " du {0}";
                    retour += string.Format(formatChaine, nomProprietaire);
                }
                return retour;
            }

            internal bool enMouvement
            {
                get
                {
                    Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(ID_PION);
                    return (null != ligneOrdre);
                }
            }

            internal bool IsID_NOUVEAU_PION_PROPRIETAIRENull()
            {
                return this.ID_NOUVEAU_PION_PROPRIETAIRE == Constantes.NULLENTIER;
            }

            internal bool IsID_ANCIEN_PION_PROPRIETAIRENull()
            {
                return this.ID_ANCIEN_PION_PROPRIETAIRE == Constantes.NULLENTIER;
            }

            internal bool IsC_NIVEAU_HIERARCHIQUENull()
            {
                return this.C_NIVEAU_HIERARCHIQUE == Constantes.NULLCHAR;
            }

            internal void SetID_ANCIEN_PION_PROPRIETAIRENull()
            {
                Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.ID_ANCIEN_PION_PROPRIETAIRE = Constantes.NULLENTIER;
                Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }

            internal void SetID_NOUVEAU_PION_PROPRIETAIRENull()
            {
                Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.ID_NOUVEAU_PION_PROPRIETAIRE = Constantes.NULLENTIER;
                Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }

            /******************** Nouveaux *********************/
            internal bool IsID_DEPOT_SOURCENull()
            {
                return this.ID_DEPOT_SOURCE == Constantes.NULLENTIER;
            }

            internal void SetID_DEPOT_SOURCENull()
            {
                Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.ID_DEPOT_SOURCE = Constantes.NULLENTIER;
                Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }

            internal bool IsID_PION_REMPLACENull()
            {
                return this.ID_PION_REMPLACE == Constantes.NULLENTIER;
            }

            internal void SetID_PION_REMPLACENull()
            {
                Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.ID_PION_REMPLACE = Constantes.NULLENTIER;
                Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }

            //todo
            internal bool IsID_BATAILLENull()
            {
                return this.ID_BATAILLE == Constantes.NULLENTIER;
            }

            internal bool IsI_TOUR_BLESSURENull()
            {
                return this.I_TOUR_BLESSURE == Constantes.NULLENTIER;
            }

            internal void SetI_TOUR_BLESSURENull()
            {
                this.I_TOUR_BLESSURE = Constantes.NULLENTIER;
            }

            internal bool IsID_LIEU_RATTACHEMENTNull()
            {
                return this.ID_LIEU_RATTACHEMENT == Constantes.NULLENTIER;
            }

            internal void SetID_LIEU_RATTACHEMENTNull()
            {
                Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.ID_LIEU_RATTACHEMENT = Constantes.NULLENTIER;
                Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }

            internal bool IsID_PION_ESCORTENull()
            {
                return this.ID_PION_ESCORTE == Constantes.NULLENTIER;
            }

            internal void SetID_PION_ESCORTENull()
            {
                Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.ID_PION_ESCORTE = Constantes.NULLENTIER;
                Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }

            internal bool IsI_TOUR_DERNIER_RAVITAILLEMENT_DIRECTNull()
            {
                return this.I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT == Constantes.NULLENTIER;
            }

            internal void SetI_TOUR_DERNIER_RAVITAILLEMENT_DIRECTNull()
            {
                Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT = Constantes.NULLENTIER;
                Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }

            internal bool IsI_VICTOIRENull()
            {
                return this.I_VICTOIRE == Constantes.NULLENTIER;
            }

            internal bool IsI_ZONE_BATAILLENull()
            {
                return this.I_ZONE_BATAILLE == Constantes.NULLENTIER;
            }

            internal bool IsI_TOUR_RETRAITE_RESTANTNull()
            {
                return this.I_TOUR_RETRAITE_RESTANT == Constantes.NULLENTIER;
            }

            internal void SetID_BATAILLENull()
            {
                Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.ID_BATAILLE = Constantes.NULLENTIER;
                Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }

            internal void SetI_ZONE_BATAILLENull()
            {
                Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.I_ZONE_BATAILLE = Constantes.NULLENTIER;
                Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }

            internal string NomDeLaPatrouille()
            {
                return "Patrouille du " + S_NOM;
            }

            //internal void SetI_TOUR_CONVOI_CREENull()
            //{
            //    this.I_TOUR_CONVOI_CREE = Constantes.NULLENTIER;
            //}

        }
    }
}
