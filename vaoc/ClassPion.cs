using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using WaocLib;

namespace vaoc
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
                        int I_VICTOIRE,
                        int I_TRI,
                        string S_ENNEMI_OBSERVABLE,
                        int I_TOUR_ENNEMI_OBSERVABLE
                )
            {
                //recherche du max de l'ID_PION pour effectuer l'insertion, l'ID_PION ne peut pas
                //être incrementé automatiquement à cause de la bascule des pions de TAB_RENFORT vers TAB_PION
                LogFile.Notifier("AjouterPion:AddTAB_PIONRow");
                string tri = "ID_PION DESC";
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                TAB_PIONRow[] resCout = (TAB_PIONRow[])Select(string.Empty, tri);
                if (0 == resCout.Length)
                {
                    Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
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
                        I_VICTOIRE,
                        I_TRI,
                        S_ENNEMI_OBSERVABLE,
                        I_TOUR_ENNEMI_OBSERVABLE
                };
                rowTAB_PIONRow.ItemArray = columnValuesArray;
                this.Rows.Add(rowTAB_PIONRow);
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
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
                    /*
                    string requete = string.Format("ID_PION_REMPLACE = {0}", ID_PION);
                    Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    TAB_PIONRow[] resPions = (TAB_PIONRow[])m_donnees.TAB_PION.Select(requete);
                    Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    if (resPions.Length <= 0)
                    {
                        return null;
                    }
                    return resPions[0];
                    */
                    return Donnees.m_donnees.TAB_PION.FindByID_PION(this.ID_PION_REMPLACE);
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
                        if (ligneORDRE.IsI_TOUR_FINNull() || ligneORDRE.I_TOUR_FIN +24 > Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            /// <summary>
            ///renvoie la nation du pion
            /// </summary>
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
            }

            /// <summary>
            /// effectif total du pion en début de campagne
            /// </summary>
            public int effectifInitial
            {
                get
                {
                    return I_ARTILLERIE_INITIALE + I_CAVALERIE_INITIALE + I_INFANTERIE_INITIALE;
                }
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
                        return Constantes.CST_ENCOMBREMENT_DEPOT[(int)(C_NIVEAU_DEPOT - 'A')] * Constantes.CST_ENCOMBREMENT_INFANTERIE;// / Donnees.m_donnees.TAB_JEU[0].I_ECHELLE; 
                    }
                    if (estConvoiDeRavitaillement)
                    {
                        return Constantes.CST_ENCOMBREMENT_CONVOI_RAVITAILLEMENT * Constantes.CST_ENCOMBREMENT_INFANTERIE;// / Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                    }
                    if (estPontonnier)
                    {
                        return Constantes.CST_ENCOMBREMENT_CONVOI_RAVITAILLEMENT * Constantes.CST_ENCOMBREMENT_INFANTERIE;// / Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
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
                    Donnees.TAB_CASERow ligneCaseBase = ligneCase ?? Donnees.m_donnees.TAB_CASE.FindParID_CASE(this.ID_CASE);
                    xCaseHautGauche = Math.Max(0, ligneCaseBase.I_X - visionPixel);
                    yCaseHautGauche = Math.Max(0, ligneCaseBase.I_Y - visionPixel);
                    xCaseBasDroite = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1, ligneCaseBase.I_X + visionPixel);
                    yCaseBasDroite = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1, ligneCaseBase.I_Y + visionPixel);
                }
            }

            /// <summary>
            /// Indique les pions ennemis dans son cadre de vision
            /// </summary>
            /// <param name="ligneCase"></param>
            /// <returns>ennemis en vu, vide sinon</returns>
            public string EnnemiObservable(Donnees.TAB_CASERow ligneCase)
            {
                string sEnnemiObservable = string.Empty;
                //string requete;
                int xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite;

                CadreVision(ligneCase, out xCaseHautGauche, out yCaseHautGauche, out xCaseBasDroite, out yCaseBasDroite);

                Donnees.TAB_CASERow[] ligneCaseVues = Donnees.m_donnees.TAB_CASE.CasesCadre(xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite);
                Donnees.TAB_MODELE_PIONRow ligneModelePion = this.modelePion;
                foreach (Donnees.TAB_CASERow ligneCaseVue in ligneCaseVues)
                {
                    //je detecte deux cases sur des colonnnes/lignes différentes en espéant ne plus avoir l'effet d'unités en bordure qui apparaissent/disparaissent en emettant un message à chaque fois
                    if (this.estEnnemi(ligneCaseVue, ligneModelePion, true, true))
                    {
                        sEnnemiObservable = ligneCaseVue.ID_NOUVEAU_PROPRIETAIRE.ToString();
                        break;
                    }
                }

                return sEnnemiObservable;
            }

            /// <summary>
            /// Si une unité voit un ennemi ou ne voit plus d'ennemi durant son déplacement, elle envoie un message d'information
            /// </summary>
            /// <param name="ligneCase">Case à partir de laquelle l'unité fait son observation (en tête de son mouvement typiquement), null si à l'arrêt </param>
            public bool MessageEnnemiObserve(Donnees.TAB_CASERow ligneCase)
            {
                try
                {
                    bool bRetour = true;

                    //si un ennemi apparait ou disparait, il faut envoyer un message
                    if (this.estCombattifQG(true, true) && 
                        (this.IsI_TOUR_ENNEMI_OBSERVABLENull() ||
                        this.I_TOUR_ENNEMI_OBSERVABLE != Donnees.m_donnees.TAB_PARTIE[0].I_TOUR))
                    {
                        string sObservable = this.EnnemiObservable(ligneCase);
                        if ((string.Empty == sObservable && string.Empty != this.S_ENNEMI_OBSERVABLE)
                            || (string.Empty != sObservable && string.Empty == this.S_ENNEMI_OBSERVABLE))
                        {
                            if (string.Empty != sObservable)
                            {
                                bRetour = ClassMessager.EnvoyerMessage(this, ligneCase, ClassMessager.MESSAGES.MESSAGE_ENNEMI_OBSERVE);
                            }
                            else
                            {
                                bRetour = ClassMessager.EnvoyerMessage(this, ClassMessager.MESSAGES.MESSAGE_SANS_ENNEMI_OBSERVE);
                            }
                            Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                            this.S_ENNEMI_OBSERVABLE = sObservable;
                            this.I_TOUR_ENNEMI_OBSERVABLE = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                            Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        }
                    }
                    return bRetour;
                }
                catch (Exception ex)
                {
                    string messageEX = string.Format("exception ClassPion.MessageEnnemiObserve {3} : {0} : {1} :{2}",
                           ex.Message, (null == ex.InnerException) ? "sans inner exception" : ex.InnerException.Message,
                           ex.StackTrace, ex.GetType().ToString());
                    LogFile.Notifier(messageEX);
                    throw ex;
                }
            }

            /// <summary>
            /// Remet à vide tous les espaces occupés par le pion
            /// </summary>
            /// <param name="bParcoursCompris">true, on détruit egalement le parcours, false, on laisse le parcours</param>
            public void DetruireEspacePion(bool bParcoursCompris)
            {
                Donnees.m_donnees.TAB_ESPACE.SupprimerEspacePion(ID_PION);
                if (bParcoursCompris)
                {
                    Donnees.m_donnees.TAB_PARCOURS.SupprimerParcoursPion(ID_PION);
                }
                //lignePion.SupprimerTousLesOrdres(); si on fait cela, les ordres ne seront plus visibles sur le web
                //si l'unité a des effectifs sur carte, il faut les supprimer pour avoir un affichage correcte après la bataille
                Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                if (!estQG && !estMessager && !estPatrouille)
                {
                    string requete = string.Format("(ID_PROPRIETAIRE= {0}) OR (ID_NOUVEAU_PROPRIETAIRE={0})", ID_PION);
                    Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                    Donnees.TAB_CASERow[] changeRows = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete);
                    Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                    foreach (Donnees.TAB_CASERow ligneChange in changeRows)
                    {
                        Donnees.TAB_CASERow ligne = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneChange.ID_CASE);
                        ligne.SetID_PROPRIETAIRENull();
                        ligne.SetID_NOUVEAU_PROPRIETAIRENull();
                    }
                }
                else
                {
                    //les autes unités n'occupent, potentiellement, qu'une case
                    Donnees.TAB_CASERow ligne = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ID_CASE);
                    if (null != ligne)
                    {
                        int IdProprietaire = ligne.ID_PROPRIETAIRE;
                        if ((Constantes.NULLENTIER != IdProprietaire) && (IdProprietaire == ID_PION))
                        {
                            ligne.SetID_PROPRIETAIRENull();
                        }
                        int IdNouveauProprietaire = ligne.ID_NOUVEAU_PROPRIETAIRE;
                        if ((Constantes.NULLENTIER != IdNouveauProprietaire) && (IdNouveauProprietaire == ID_PION))
                        {
                            ligne.SetID_NOUVEAU_PROPRIETAIRENull();
                        }
                    }
                }
                Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
            }

            /// <summary>
            /// Remet à vide tous les espaces occupés par le pion
            /// </summary>
            public void DetruireEspacePion()
            {
                DetruireEspacePion(true);
            }

            public void DetruirePion()
            {
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                B_DETRUIT = true;
                I_ARTILLERIE = 0;//pas toujours remis à zéro quand l'unité est surtout composé de cavalerie et d'infanterie
                I_MORAL = 0; //ainsi, les blessés légers d'une bataille sont mis automatiquement en bléssés graves (sinon, l'unité pourrait "renaitre" post bataille)
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                DetruireEspacePion();
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

                string message = string.Format("CalculerRepartitionEffectif iINFANTERIESource={0}, iCAVALERIESource={1}, iARTILLERIESource={2}, effectif={3}, iINFANTERIE={4}, iCAVALERIE={5}, iARTILLERIE={6}",
                    iINFANTERIESource, iCAVALERIESource, iARTILLERIESource, effectif, iINFANTERIE, iCAVALERIE, iARTILLERIE);
                LogFile.Notifier(message);
            }

            public bool CalculerEffectif(decimal encombrement, bool enMouvement, out int iINFANTERIE, out int iCAVALERIE, out int iARTILLERIE)
            {
                int iInfanterie;

                iInfanterie = (enMouvement && (estDepot || estConvoiDeRavitaillement || estPontonnier)) ? effectifTotalEnMouvement : I_INFANTERIE;

                return CalculerEffectif(iInfanterie, I_CAVALERIE, I_ARTILLERIE, encombrement, enMouvement, out iINFANTERIE, out iCAVALERIE, out iARTILLERIE);
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
            public bool CalculerEffectif(int iInfanterieInitial, int iCavalerieInitial, int iArtillerieInitial, decimal encombrement, bool enMouvement, out int iINFANTERIE, out int iCAVALERIE, out int iARTILLERIE)
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
                    encombrementReel = (decimal)(encombrement * rapportMoral) / (100 + this.modelePion.I_FOURGON);
                }

                if (enMouvement)
                {
                    //encombrement sur route
                    encombrementCavalerie = iCavalerieInitial * (100 - I_FATIGUE) / 100 * echelle / Constantes.CST_ENCOMBREMENT_CAVALERIE;
                    if (encombrementReel < encombrementCavalerie)
                    {
                        //la cavalerie occupe tout l'espace
                        iCAVALERIE = (int)(encombrementReel * Constantes.CST_ENCOMBREMENT_CAVALERIE / echelle);
                        return true;
                    }
                    else
                    {
                        //on inclue les "fatigués" puisqu'ils sont retirés lors du calcul de l'encombrement
                        iCAVALERIE = iCavalerieInitial;
                        encombrementReel -= encombrementCavalerie;
                    }

                    encombrementInfanterie = iInfanterieInitial * (100 - I_FATIGUE) / 100 * echelle / Constantes.CST_ENCOMBREMENT_INFANTERIE;
                    if (encombrementReel < encombrementInfanterie)
                    {
                        //l'infanterie occupe tout l'espace restant
                        iINFANTERIE = (int)(encombrementReel * Constantes.CST_ENCOMBREMENT_INFANTERIE / echelle);
                        return true;
                    }
                    else
                    {
                        //on inclue les "fatigués" puisqu'ils sont retirés lors du calcul de l'encombrement
                        iINFANTERIE = iInfanterieInitial;
                        encombrementReel -= encombrementInfanterie;
                    }
                    //l'artillerie occupe ce qui reste
                    iARTILLERIE = (int)Math.Round(encombrementReel * Constantes.CST_ENCOMBREMENT_ARTILLERIE / echelle);
                }
                else
                {
                    //encombrement en statique
                    effectif = (int)(Constantes.CST_ENCOMBREMENT_ARRET * encombrement / (echelle * echelle));
                    CalculerRepartitionEffectif(effectif, out iINFANTERIE, out iCAVALERIE, out iARTILLERIE);
                }

                return true;
            }

            internal void Arret(Donnees.TAB_ORDRERow ligneOrdre)
            {
                //il faut supprimer tous les ordres actuellement en cours
                PlacerPionEnBivouac(ligneOrdre);
                SupprimerTousLesOrdres(ligneOrdre.ID_ORDRE);
                TerminerOrdre(ligneOrdre, false, false);
            }

            /// <summary>
            /// recherche de l'ordre courant de l'unité au moment du dernier message
            /// </summary>
            /// <param name="tour"></param>
            /// <param name="phase"></param>
            /// <returns></returns>
            public Donnees.TAB_ORDRERow OrdreEnCours(int tour, int phase)
            {
                Donnees.TAB_ORDRERow ligneOrdreEnCours = null;
                //requete = string.Format("ID_PION={0} AND I_TOUR_DEBUT<={1} AND I_PHASE_DEBUT<={2} AND ((I_TOUR_FIN IS NULL AND I_PHASE_FIN IS NULL) OR (I_TOUR_FIN>{1} AND I_PHASE_FIN>{2}))",
                //    lignePion.ID_PION, ligneMessage.I_TOUR_DEPART, ligneMessage.I_PHASE_DEPART);
                //string requete = string.Format("ID_PION={0} AND I_TOUR_DEBUT<={1} AND ((I_TOUR_FIN IS NULL) OR (I_TOUR_FIN>{1}))", lignePion.ID_PION, tour);
                string requete = string.Format("ID_PION={0} AND I_TOUR_DEBUT<={1} AND ((I_TOUR_FIN = {3}) OR (I_TOUR_FIN>{1}) OR (I_TOUR_FIN={1} AND I_PHASE_FIN>{2}))",
                                             this.ID_PION, tour, phase, Constantes.NULLENTIER);
                Donnees.TAB_ORDRERow[] resOrdre = (Donnees.TAB_ORDRERow[])Donnees.m_donnees.TAB_ORDRE.Select(requete, "ID_ORDRE");
                if (resOrdre.Length>0)
                {
                    int o = 0;
                    while (null == ligneOrdreEnCours && o < resOrdre.Length)
                    {
                        //c'est seulement si l'ordre est dans le même tour que le message que la valeur de la phase intervient
                        if ((resOrdre[o].I_TOUR_DEBUT != tour) ||
                            (resOrdre[o].I_PHASE_DEBUT <= phase && (resOrdre[o].IsI_PHASE_FINNull() || resOrdre[o].I_PHASE_FIN > phase || resOrdre[o].I_TOUR_FIN > tour)))
                        {
                            ligneOrdreEnCours = resOrdre[o];
                        }
                        o++;
                    }
                }
                return ligneOrdreEnCours;
            }
            public string DescriptifOrdreEnCours(int tour, int phase)
            {
                Donnees.TAB_ORDRERow ligneOrdreEnCours = OrdreEnCours(tour, phase);
                string sOrdreCourant;
                sOrdreCourant = (null == ligneOrdreEnCours) ? "aucun ordre" : ClassMessager.MessageDecrivantUnOrdre(ligneOrdreEnCours, false);
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
                //s'il n'y a pas d'heure définie c'est que l'unité bouge tout le temps (messager par exemple)
                if (ligneOrdre.IsI_HEURE_DEBUTNull() || ligneOrdre.I_DUREE >=24 || 0== ligneOrdre.I_DUREE) { return true; }
                if (ligneOrdre.I_HEURE_DEBUT > (ligneOrdre.I_HEURE_DEBUT + ligneOrdre.I_DUREE)%24)
                {
                    //l'ordre depasse minuit
                    /*
                    if ((ligneOrdre.I_HEURE_DEBUT <= Donnees.m_donnees.TAB_PARTIE.HeureCourante() + 24) &&
                        (ligneOrdre.I_HEURE_DEBUT + ligneOrdre.I_DUREE > Donnees.m_donnees.TAB_PARTIE.HeureCourante() + 24))
                    { return true; }
                    */
                    if ((ligneOrdre.I_HEURE_DEBUT <= Donnees.m_donnees.TAB_PARTIE.HeureCourante()) ||
                            (ligneOrdre.I_HEURE_DEBUT + ligneOrdre.I_DUREE) % 24 >= Donnees.m_donnees.TAB_PARTIE.HeureCourante())
                    { return true; }
                }
                else
                {
                    if (ligneOrdre.I_HEURE_DEBUT <= Donnees.m_donnees.TAB_PARTIE.HeureCourante() &&
                        (ligneOrdre.I_HEURE_DEBUT + ligneOrdre.I_DUREE > Donnees.m_donnees.TAB_PARTIE.HeureCourante()))
                    { return true; }
                }
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
                /*
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
                */
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
                return estEnnemi(ligneCase, ligneModele, bCombattif, true);//BEA le 24/03/2021 true au lieu de false, sinon, la nuit les unités démoralisées peuvent être traversées
            }

            public bool estEnnemi(TAB_CASERow ligneCase, TAB_MODELE_PIONRow ligneModele, bool bCombattif, bool bCombattifSansMoral)
            {
                TAB_PIONRow lignePionAdversaire;

                if (!ligneCase.IsID_PROPRIETAIRENull())
                {
                    Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                    int IdProprietaire = ligneCase.ID_PROPRIETAIRE;
                    Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                    lignePionAdversaire = m_donnees.tableTAB_PION.FindByID_PION(IdProprietaire);
                    return estPionEnnemi(lignePionAdversaire, ligneModele.ID_NATION, bCombattif, bCombattifSansMoral);
                }

                if (!ligneCase.IsID_NOUVEAU_PROPRIETAIRENull())
                {
                    Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                    int IdNouveauProprietaire = ligneCase.ID_NOUVEAU_PROPRIETAIRE;
                    Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                    lignePionAdversaire = m_donnees.tableTAB_PION.FindByID_PION(IdNouveauProprietaire);
                    return estPionEnnemi(lignePionAdversaire, ligneModele.ID_NATION, bCombattif, bCombattifSansMoral);
                }
                return false;
            }

            private bool estPionEnnemi(TAB_PIONRow lignePionAdversaire, int iNationPion, bool bCombattif, bool bCombattifSansMoral)
            {
                if (null == lignePionAdversaire)
                {
                    return false; //cas possible si j'ai détruit une unité manuellement
                }

                if (lignePionAdversaire.B_DETRUIT)
                {
                    return false;
                }

                TAB_MODELE_PIONRow ligneModeleAdversaire = lignePionAdversaire.modelePion;
                if (null == ligneModeleAdversaire)
                {
                    throw new Exception("TAB_PIONRow.estPionEnnemi impossible de trouver le modèle du pion");
                }

                if ((iNationPion != ligneModeleAdversaire.ID_NATION) &&
                    ((bCombattif && lignePionAdversaire.estCombattifQG(false, bCombattifSansMoral)) || !bCombattif))
                {
                    if (bCombattif)
                    {
                        return lignePionAdversaire.estCombattifQG(false, bCombattifSansMoral);
                    }
                    return true;
                }
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

            private bool MessageEnnemiAProximite(int tour)
            {
                //on envoie un message pour prévenir l'officier commandant
                Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageEmis(ID_PION, ClassMessager.MESSAGES.MESSAGE_ORDRE_REPORTE_ENNEMI_PROCHE);
                if (null == ligneMessage ||
                    ligneMessage.I_TOUR_DEPART + ClassMessager.CST_MESSAGE_FREQUENCE_ALERTE < tour)
                {
                    if (!ClassMessager.EnvoyerMessage(this, ClassMessager.MESSAGES.MESSAGE_ORDRE_REPORTE_ENNEMI_PROCHE))
                    {
                        string message = string.Format("{0},ID={1}, erreur sur EnvoyerMessage avec MESSAGE_ORDRE_REPORTE_ENNEMI_PROCHE dans ExecuterDetruirePont ou SeFortifier", S_NOM, ID_PION);
                        LogFile.Notifier(message);
                        return false;
                    }
                }
                return true;
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
            /// Termine l'ordre en cours de l'unité, et envoie un message s'il y a un ordre suivant
            /// </summary>
            /// <param name="ligneOrdre">ordre qui se termine</param>
            /// <param name="bEnvoieMessageSiOrdreSuivant">true si l'on doit envoyer un message s'il y a un ordre suivant, false sinon</param>
            /// <param name="bTerminerLesOrdresSuivant">true si l'on doit terminer tous les ordres suivants (car d'un changement d'ordre), false si l'on doit passer à l'ordre suivant au contraire</param>
            /// <returns>true si ok, false si ko</returns>
            public bool TerminerOrdre(TAB_ORDRERow ligneOrdre, bool bEnvoieMessageSiOrdreSuivant, bool bTerminerLesOrdresSuivant)
            {
                if (null == ligneOrdre)
                {
                    //Il n'y a pas d'ordre à terminer en fait
                    return true;
                }
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.I_DISTANCE_A_PARCOURIR = 0;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                ligneOrdre.I_TOUR_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                ligneOrdre.I_PHASE_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;
                Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                if (!ligneOrdre.IsID_ORDRE_SUIVANTNull() && ligneOrdre.ID_ORDRE_SUIVANT >= 0)
                {
                    //on recherche que le nouvel ordre et on indique que le nouvel ordre vient de demarrer
                    TAB_ORDRERow ligneOrdreSuivant = m_donnees.TAB_ORDRE.FindByID_ORDRE(ligneOrdre.ID_ORDRE_SUIVANT);

                    if (bTerminerLesOrdresSuivant)
                    {
                        //on clos en même temps tous les ordres suivants recursivement et on envoie pas de message, cela n'a pas de sens
                        if (!TerminerOrdre(ligneOrdreSuivant, false, bTerminerLesOrdresSuivant))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                        ligneOrdreSuivant.ID_PION = this.ID_PION;
                        ligneOrdreSuivant.ID_CASE_DEPART = this.ID_CASE;
                        ligneOrdreSuivant.I_TOUR_DEBUT = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                        ligneOrdreSuivant.I_PHASE_DEBUT = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;
                        ligneOrdreSuivant.I_EFFECTIF_DEPART = this.effectifTotalEnMouvement;
                        //dans le cas de l'ordre d'attaque à proximité celui-ci peut devenir actif à nouveau plusieurs fois, d'où la fin à remettre à blanc à chaque fois
                        ligneOrdreSuivant.SetI_TOUR_FINNull();
                        ligneOrdreSuivant.SetI_PHASE_FINNull();
                        Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);

                        if (bEnvoieMessageSiOrdreSuivant)
                        {
                            //on envoie un message si besoin
                            return ClassMessager.EnvoyerMessage(this, ClassMessager.MESSAGES.MESSAGE_ORDRE_SUIVANT);
                        }
                    }
                }
                return true;
            }

            public bool SupprimerTousLesOrdres()
            {
                return SupprimerTousLesOrdres(-1);
            }

            /// <summary>
            /// On supprimer tous les ordres avant l'ordre courant
            /// </summary>
            /// <param name="idordre">ordre courant</param>
            /// <returns></returns>
            public bool SupprimerTousLesOrdres(int idordre)
            {
                string requete;

                //recherche les ordres existants
                requete = (idordre == -1) ? string.Format("ID_PION={0} AND I_TOUR_FIN = {1}", ID_PION, Constantes.NULLENTIER) :
                                          string.Format("ID_PION={0} AND ID_ORDRE< {1} AND I_TOUR_FIN = {2}", ID_PION, idordre, Constantes.NULLENTIER);
                Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                TAB_ORDRERow[] resOrdres = (TAB_ORDRERow[])m_donnees.TAB_ORDRE.Select(requete);
                foreach (TAB_ORDRERow ligneOrdre in resOrdres)
                {
                    ligneOrdre.I_TOUR_FIN = m_donnees.tableTAB_PARTIE[0].I_TOUR;
                    ligneOrdre.I_PHASE_FIN = m_donnees.tableTAB_PARTIE[0].I_PHASE;
                }
                Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                //AcceptChanges only updates your rows in the (in memory) dataset, that is - marks them as "not needed for actual database update".
                //m_donnees.TAB_ORDRE.AcceptChanges();

                //suppression des parcours en mémoire car l'unité devient statiques
                Monitor.Enter(Donnees.m_donnees.TAB_ESPACE.Rows.SyncRoot);
                m_donnees.TAB_ESPACE.SupprimerEspacePion(ID_PION);
                Monitor.Exit(Donnees.m_donnees.TAB_ESPACE.Rows.SyncRoot);
                Monitor.Enter(Donnees.m_donnees.TAB_PARCOURS.Rows.SyncRoot);
                m_donnees.TAB_PARCOURS.SupprimerParcoursPion(ID_PION);
                Monitor.Exit(Donnees.m_donnees.TAB_PARCOURS.Rows.SyncRoot);

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
            /// indique si le pion est celui d'un rôle d'un joueur
            /// </summary>
            public bool estRole
            {
                get 
                {
                    return null != Donnees.m_donnees.TAB_ROLE.TrouvePion(ID_PION);
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

                if (estMessager || estPatrouille || estDepot || estConvoi || estPontonnier || estBlesses || estPrisonniers)// || estRenfort) -> le renfort est une troupe combattive
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

            public bool estRavitaillableDirect(int tour, int phase)
            {
                int tourDebut, phaseDebut;
                int heure = ClassMessager.DateHeure(tour, phase).Hour;
                //on part dela première heure depuis minuit

                if (0==heure)
                {
                    //cas de la phase de ravitaillement quotidienne
                    tourDebut = tour - 23;
                    phaseDebut = 0;
                }
                else
                {
                    tourDebut = tour - heure;
                    phaseDebut = (tour == tourDebut) ? phase : 0;
                }
                string requete = string.Format("ID_PION={0} AND I_TOUR_DEBUT>={1} AND I_PHASE_DEBUT>={2} AND I_ORDRE_TYPE={3}",
                                                this.ID_PION, tourDebut, phaseDebut, Constantes.ORDRES.RAVITAILLEMENT_DIRECT);
                Donnees.TAB_ORDRERow[] resOrdre = (Donnees.TAB_ORDRERow[])Donnees.m_donnees.TAB_ORDRE.Select(requete, "I_TOUR_DEBUT DESC");
                if (resOrdre.Count() > 0)
                {
                    //if (tour - resOrdre[0].I_TOUR_DEBUT<=24) { return true; }//l'ordre n'est valable que 24 heures
                    return true;//la calcul de validité avant minuit est fait dans le calcul du tour précédent
                }
                return false;
            }

            internal bool RavitaillementDirect(TAB_ORDRERow ligneOrdre, int tour, int phase, out string nomDepot)
            {
                string message;
                int effectifsRavitailles;
                int meilleurPourcentageRavitaillement;
                int meilleurPourcentageMateriel;
                nomDepot = string.Empty;
                Donnees.TAB_PIONRow ligneDepot = RechercheMeilleurDepotDirect(out effectifsRavitailles, out meilleurPourcentageRavitaillement, out meilleurPourcentageMateriel);
                if (null == ligneDepot || 0 == effectifsRavitailles)
                {
                    if (!ClassMessager.EnvoyerMessage(this, ClassMessager.MESSAGES.MESSAGE_RAVITAILLEMENT_DIRECT_IMPOSSIBLE))
                    {
                        message = string.Format("{0},ID={1}, erreur sur EnvoyerMessage avec MESSAGE_RAVITAILLEMENT_DIRECT_IMPOSSIBLE dans RavitaillementDirect", S_NOM, ID_PION);
                        LogFile.Notifier(message);
                        return false;
                    }                    
                }
                else
                {
                    nomDepot = ligneDepot.S_NOM;
                    Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    ligneDepot.I_SOLDATS_RAVITAILLES += effectifsRavitailles;
                    ligneDepot.I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                    //on ravitaille l'unité et on informe le joueur
                    if (!ClassMessager.EnvoyerMessage(this, 0,
                        meilleurPourcentageRavitaillement - this.I_RAVITAILLEMENT,
                        meilleurPourcentageMateriel - this.I_MATERIEL,
                        ligneDepot.S_NOM, ClassMessager.MESSAGES.MESSAGE_RAVITAILLEMENT_DIRECT))
                    {
                        message = string.Format("{0},ID={1}, erreur sur EnvoyerMessage avec MESSAGE_RAVITAILLEMENT_DIRECT dans RavitaillementDirect", S_NOM, ID_PION);
                        LogFile.Notifier(message);
                        Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        return false;
                    }
                    this.I_RAVITAILLEMENT = meilleurPourcentageRavitaillement;
                    this.I_MATERIEL = meilleurPourcentageMateriel;

                    int ligneDepotTable = ligneDepot.C_NIVEAU_DEPOT - 'A';
                    if (ligneDepot.I_SOLDATS_RAVITAILLES >= Constantes.tableLimiteRavitaillementDepot[ligneDepotTable])
                    {
                        //Il faut réduire le dépot sauf s'il est de type A auquel cas on ne fait rien 
                        if ('A' != ligneDepot.C_NIVEAU_DEPOT)
                        {
                            if ('D' != ligneDepot.C_NIVEAU_DEPOT)
                            {
                                ligneDepot.C_NIVEAU_DEPOT++;
                                ligneDepot.I_SOLDATS_RAVITAILLES = 0;
                                if (!ClassMessager.EnvoyerMessage(ligneDepot, ClassMessager.MESSAGES.MESSAGE_DEPOT_REDUIT))
                                {
                                    message = string.Format("{0},ID={1}, erreur sur EnvoyerMessage avec MESSAGE_DEPOT_REDUIT dans RavitaillementDirect", ligneDepot.S_NOM, ligneDepot.ID_PION);
                                    LogFile.Notifier(message);
                                    Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                                    return false;
                                }
                            }
                            else
                            {
                                //le depot est carrement détruit
                                if (!ClassMessager.EnvoyerMessage(ligneDepot, ClassMessager.MESSAGES.MESSAGE_DEPOT_DETRUIT))
                                {
                                    message = string.Format("{0},ID={1}, erreur sur EnvoyerMessage avec MESSAGE_DEPOT_DETRUIT dans RavitaillementDirect", ligneDepot.S_NOM, ligneDepot.ID_PION);
                                    LogFile.Notifier(message);
                                    Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                                    return false;
                                }
                                ligneDepot.DetruirePion();
                            }
                        }
                    }
                    Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                }
                //l'ordre est terminé
                TerminerOrdre(ligneOrdre, false, true);
                return true;
            }

            internal bool ExecuterAttaqueProche(TAB_ORDRERow ligneOrdre)
            {
                string message;
                if (null == Donnees.m_donnees.TAB_ORDRE.Mouvement(ID_PION) && !Donnees.m_donnees.TAB_PARTIE.Nocturne())
                {
                    //l'unité ne bouge pas vers l'unité la plus proche, il faut donc la chercher
                    Donnees.TAB_CASERow ligneCasePion = Donnees.m_donnees.TAB_CASE.FindByID_CASE(this.ID_CASE);
                    int xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite;
                    this.CadreVision(ligneCasePion, out xCaseHautGauche, out yCaseHautGauche, out xCaseBasDroite, out yCaseBasDroite);

                    Donnees.TAB_CASERow[] ligneCaseVues = Donnees.m_donnees.TAB_CASE.CasesCadre(xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite);
                    Donnees.TAB_MODELE_PIONRow ligneModelePion = modelePion;
                    double distanceMin = double.MaxValue;
                    Donnees.TAB_CASERow ligneCaseEnnemi = null;
                    foreach (Donnees.TAB_CASERow ligneCaseVue in ligneCaseVues)
                    {
                        if (this.estEnnemi(ligneCaseVue, ligneModelePion, true, true))
                        {
                            double distance = Constantes.Distance(ligneCasePion.I_X, ligneCasePion.I_Y, ligneCaseVue.I_X, ligneCaseVue.I_Y);
                            if (distance < distanceMin)
                            {
                                distanceMin = distance;
                                ligneCaseEnnemi = ligneCaseVue;
                            }
                        }
                    }
                    if (distanceMin == double.MaxValue)
                    {
                        //il n'y a aucun ennemi a proximité, on arrête l'ordre et on prévient le joueur
                        if (!ClassMessager.EnvoyerMessage(this, ClassMessager.MESSAGES.MESSAGE_AUCUN_ENNEMI_PROCHE))
                        {
                            message = string.Format("{0},ID={1}, erreur sur EnvoyerMessage avec MESSAGE_AUCUN_ENNEMI_PROCHE dans ExecuterOrdreHorsMouvement", S_NOM, ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }
                        this.TerminerOrdre(ligneOrdre, true, false);
                    }
                    else
                    {
                        //si on est déjà sur l'unité à attaquer c'est que celle-ci ne peut être engagée en combat car "protégée", à ce moment là , on va vers l'unite de protection
                        if (ligneCaseEnnemi.ID_CASE == ID_CASE)
                        {
                            Donnees.TAB_PIONRow lignePionAmi = ClassMessager.AmiEnvironnant(ligneCaseEnnemi);
                            if (null == lignePionAmi)
                            {
                                message = string.Format("{0},ID={1}, erreur sur la recherche de l'ami de l'ennemi sur ID_CASE {2}:{3},{4}: dans ExecuterOrdreHorsMouvement",
                                    S_NOM, ID_PION, ligneCaseEnnemi.ID_CASE, ligneCaseEnnemi.I_X, ligneCaseEnnemi.I_Y);
                                LogFile.Notifier(message);
                                return false;
                            }
                            ligneCaseEnnemi = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePionAmi.ID_CASE);
                            //Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageEmis(lignePion.ID_PION, ClassMessager.MESSAGES.MESSAGE_ENNEMI_PROCHE_INATTAQUABLE);
                            //if (null == ligneMessage ||
                            //    ligneMessage.I_TOUR_DEPART + ClassMessager.CST_MESSAGE_FREQUENCE_ALERTE < Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)
                            //{
                            //    if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_ENNEMI_PROCHE_INATTAQUABLE))
                            //    {
                            //        return false;
                            //    }
                            //}
                        }

                        //On se déplace vers l'unité à attaquer
                        //et maintenant, un ordre de mouvement
                        Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                        Donnees.TAB_ORDRERow ligneOrdreMouvement = Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                            Constantes.NULLENTIER,//id_ordre_transmis
                            ligneOrdre.ID_ORDRE,//id_ordre_suivant
                            Constantes.NULLENTIER,
                            Constantes.ORDRES.MOUVEMENT,
                            ID_PION,
                            ID_CASE,
                            effectifTotal,//I_EFFECTIF_DEPART
                            ligneCaseEnnemi.ID_CASE,
                            Constantes.NULLENTIER,//id nom destination
                            0,//I_EFFECTIF_DESTINATION
                            Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,//I_TOUR_DEBUT
                            Donnees.m_donnees.TAB_PARTIE[0].I_PHASE,//I_PHASE_DEBUT
                            Constantes.NULLENTIER,//I_TOUR_FIN
                            Constantes.NULLENTIER,//I_PHASE_FIN
                            Constantes.NULLENTIER,//ID_MESSAGE
                            Constantes.NULLENTIER, //lignePionDestinataire.ID_PION,
                            Constantes.NULLENTIER,//ID_CIBLE
                            Constantes.NULLENTIER,//ID_DESTINATAIRE_CIBLE
                            Constantes.NULLENTIER,//ID_BATAILLE
                            Constantes.NULLENTIER,//I_ZONE_BATAILLE
                            Donnees.m_donnees.TAB_JEU[0].I_LEVER_DU_SOLEIL,//I_HEURE_DEBUT
                            Donnees.m_donnees.TAB_JEU[0].I_COUCHER_DU_SOLEIL - Donnees.m_donnees.TAB_JEU[0].I_LEVER_DU_SOLEIL,//I_DUREE
                            Constantes.NULLENTIER);//I_ENGAGEMENT
                                                   //l'ordre d'attaque à proximité est mis en ordre suivant et on annule sa date début sinon, c'est toujours l'ordre actif et non pas l'ordre de mouvement !
                        ligneOrdre.I_TOUR_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                        ligneOrdre.I_PHASE_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;
                        Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                        LogFile.Notifier(string.Format("ExecuterOrdreHorsMouvement ATTAQUE_PROCHE:  ID={0} mouvement vers case ID={1}:{2},{3}",
                            ID_PION, ligneCaseEnnemi.ID_CASE, ligneCaseEnnemi.I_X, ligneCaseEnnemi.I_Y));
                    }
                }
                return true;
            }

            /// <summary>
            /// renvoie la case courante occupée par le pion
            /// </summary>
            /// <returns>la case, null si non trouvée</returns>
            public TAB_CASERow CaseCourante()
            {
                return m_donnees.TAB_CASE.FindParID_CASE(ID_CASE);
            }

            public int nombrePatrouillesEnCours()
            {
                int nbPatrouilles = 0;
                string requete;

                //donne le nombre de patrouilles actuellement utilisées par le pion
                requete = string.Format("ID_PION_PROPRIETAIRE={0}", ID_PION);
                TAB_PIONRow[] resPions = (TAB_PIONRow[])m_donnees.TAB_PION.Select(requete);
                foreach (TAB_PIONRow lignePion in resPions)
                {
                    if (lignePion.estPatrouille && !lignePion.B_DETRUIT && ID_PION != lignePion.ID_PION)
                    {
                        nbPatrouilles++;
                    }
                }

                //recherche les ordres de patrouilles envoyés mais non encore arrivée
                // Il faut ajouter tous les messages en cours de transport vers une unité de cavaliers
                // et qui ne soient pas encore affecté (id_pion = -1)
                //requete = string.Format("I_TYPE={0} AND ID_PION_EMETTEUR={1} AND I_TOUR_ARRIVEE IS NOT NULL", Constantes.ORDRES.PATROUILLE, ID_PION);
                //TAB_MESSAGERow[] resMessages = (TAB_MESSAGERow[])m_donnees.TAB_MESSAGE.Select(requete);
                requete = string.Format("I_ORDRE_TYPE={0} AND ID_DESTINATAIRE={1} AND ID_PION = {2} AND I_TOUR_FIN = {2}", Constantes.ORDRES.PATROUILLE, ID_PION, Constantes.NULLENTIER);
                TAB_ORDRERow[] resMessages = (TAB_ORDRERow[])m_donnees.TAB_ORDRE.Select(requete);
                if (resMessages.Count() > 0)
                {
                    int desbugs = 0;
                    desbugs++;
                }
                nbPatrouilles += resMessages.Count();
                return nbPatrouilles;
            }

            public bool ActionPontTermine(ClassMessager.MESSAGES typeMessage, Donnees.TAB_ORDRERow ligneOrdre, int tour, int phase, Donnees.TAB_CASERow ligneCasePont)
            {
                string message;

                //on indique que le pont est détruit/construit grace à nous (c'est de l'auto promo)
                if (!ClassMessager.EnvoyerMessage(this, ligneCasePont, typeMessage))
                {
                    message = string.Format("{0}(ID={1}, erreur sur EnvoyerMessage avec type id={2} dans ActionPontTermine (déjà détruit) ", S_NOM, ID_PION, typeMessage);
                    LogFile.Notifier(message);
                    return false;
                }

                //l'ordre est terminé
                TerminerOrdre(ligneOrdre, true, false);
                message = string.Format("{0},ID={1}, ActionPontTermine en {2}:{3},{4}",
                    S_NOM, ID_PION, ligneCasePont.ID_CASE, ligneCasePont.I_X, ligneCasePont.I_Y);
                LogFile.Notifier(message);
                return true;
            }

            public void FatigueActionPont(Donnees.TAB_ORDRERow ligneOrdre, int tour)
            {
                if (ligneOrdre.I_TOUR_DEBUT != tour)
                {
                    if (Donnees.m_donnees.TAB_PARTIE.Nocturne())
                    {
                        I_NB_PHASES_MARCHE_NUIT++;
                    }
                    else
                    {
                        I_NB_PHASES_MARCHE_JOUR++;
                    }
                }
            }

            private void AlerteBruitEndommagePont(Donnees.TAB_CASERow ligneCaseDestruction)
            {
                AlerteBruit(ligneCaseDestruction, Constantes.CST_BRUIT_ENDOMMAGE_PONT, ClassMessager.MESSAGES.MESSAGE_BRUIT_ENDOMMAGE_PONT);
            }

            private void AlerteBruitReparePontConstruitPonton(Donnees.TAB_CASERow ligneCasePont)
            {
                AlerteBruit(ligneCasePont, Constantes.CST_BRUIT_REPARER_PONT_PONTON, ClassMessager.MESSAGES.MESSAGE_BRUIT_REPARER_PONT_PONTON);
            }

            private void AlerteBruit(Donnees.TAB_CASERow ligneCaseBruit, int distanceBruit, ClassMessager.MESSAGES typeMessage)
            {
                double dist;
                int i;
                Donnees.TAB_CASERow ligneCasePion;

                i = 0;
                int nbPions = Donnees.m_donnees.TAB_PION.Count();
                while (i < nbPions)
                {
                    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];

                    if (lignePion != this && lignePion.estCombattifQG(true, true))
                    {
                        //Pion à distance correcte
                        Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageEmis(lignePion.ID_PION, typeMessage);
                        if (null == ligneMessage ||
                            ligneMessage.I_TOUR_DEPART + ClassMessager.CST_MESSAGE_FREQUENCE_ALERTE < Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)
                        {
                            ligneCasePion = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePion.ID_CASE);
                            dist = Constantes.Distance(ligneCasePion.I_X, ligneCasePion.I_Y, ligneCaseBruit.I_X, ligneCaseBruit.I_Y);
                            if (dist < distanceBruit * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE)//distance en km où l'on entend le bruit de construction d'un pont ou d'un ponton
                            {
                                ClassMessager.EnvoyerMessage(lignePion, ligneCaseBruit, typeMessage);
                            }
                        }
                    }
                    i++;
                }
            }

            /// <summary>
            /// Calcul d'une durée de construction de pont ou de ponton sachant que les heures de nuit comptent double
            /// </summary>
            /// <param name="longueur">longueur de l'ouvrage</param>
            /// <param name="baseDuree">durée élémentaire pour faire 100m d'ouvrage</param>
            /// <param name="tourInitial">tour auquel commence le travail</param>
            /// <returns></returns>
            private int CalculDureeConstructionGenie(int longueur, int baseDuree, int tourInitial)
            {
                int lg = Math.Min(4, longueur); //maximum longueur de 400m même si c'est plus
                int dureeNocturne = Donnees.m_donnees.TAB_PARTIE.DureeNocturne(tourInitial, lg * baseDuree);
                return (lg * baseDuree) + dureeNocturne;
            }

            private bool InitialisationActionPont(int CSTAction, Donnees.TAB_ORDRERow ligneOrdre, int tour, int phase, out Donnees.TAB_CASERow ligneCasePont)
            {
                string message;
                Donnees.TAB_CASERow ligneCase;
                int longueurPont, longueurPonton;

                ligneCasePont = null;
                // priori, on fait toujours une action sur les ponts par rapport à la case où se trouve le pion quand il doit executer l'ordre
                //if (ligneOrdre.IsID_CASE_DESTINATIONNull())
                //{
                    //on recherche le pont sur lequel on souhaite faire l'action
                    ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ID_CASE);
                //}
                //else
                //{
                //    ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DESTINATION);
                //}
                switch (CSTAction)
                {
                    case Constantes.CST_DUREE_ENDOMMAGE_PONT:
                        //on peut casser un pont ou un ponton avec le même ordre, il faut chercher le plus proche
                        ligneCasePont = ligneCase.RecherchePontouPonton(true, false, out longueurPont);
                        Donnees.TAB_CASERow ligneCasePonton = ligneCase.RecherchePontouPonton(false, false, out longueurPonton);
                        if (null == ligneCasePont && null != ligneCasePonton)
                        {
                            //il y a un ponton et pas de pont, on garde le ponton
                            ligneCasePont = ligneCasePonton;
                        }
                        else
                        {
                            if (null != ligneCasePont && null != ligneCasePonton)
                            {
                                if (Constantes.Distance(ligneCase.I_X, ligneCase.I_Y, ligneCasePonton.I_X, ligneCasePonton.I_Y) < Constantes.Distance(ligneCase.I_X, ligneCase.I_Y, ligneCasePont.I_X, ligneCasePont.I_Y))
                                {
                                    //le ponton est plus proche que le pont
                                    ligneCasePont = ligneCasePonton;
                                    longueurPont = longueurPonton;
                                }
                            }
                        }
                        break;
                    case Constantes.CST_DUREE_REPARER_PONT:
                        ligneCasePont = ligneCase.RecherchePontouPonton(true, true, out longueurPont);
                        break;
                    case Constantes.CST_DUREE_CONSTRUIRE_PONTON:
                        //un guet c'est un ponton cassé
                        ligneCasePont = ligneCase.RecherchePontouPonton(false, true, out longueurPont);
                        break;
                    default:
                        message = string.Format("{0}(ID={1}, erreur InitialisationActionPont action demandé inconnue id={2}", S_NOM, ID_PION, CSTAction);
                        LogFile.Notifier(message);
                        return false;
                }

                if (null == ligneCasePont)
                {
                    //ça va être dur de faire une action sur un pont qui n'existe pas, l'ordre est inapplicable
                    if (!ClassMessager.EnvoyerMessage(this, ClassMessager.MESSAGES.MESSAGE_PONT_INTROUVABLE))
                    {
                        message = string.Format("{0}(ID={1}, erreur sur EnvoyerMessage avec MESSAGE_PONT_INTROUVABLE dans InitialisationActionPont", S_NOM, ID_PION);
                        LogFile.Notifier(message);
                        return false;
                    }
                    //l'ordre est terminé
                    TerminerOrdre(ligneOrdre, true, false);
                    return true;
                }
                else
                {
                    ligneOrdre.ID_CASE_DESTINATION = ligneCasePont.ID_CASE;
                    // la durée est doublée durant la nuit
                    //ligneOrdre.I_DUREE = longueurPont * CST_DUREE_DESTRUCTION_PONT;
                    ligneOrdre.I_DUREE = CalculDureeConstructionGenie(longueurPont, CSTAction, tour);
                }
                
                return true;
            }

            public bool ExecuterEndommagerPont(Donnees.TAB_ORDRERow ligneOrdre, int tour, int phase)
            {
                string message;
                Donnees.TAB_CASERow ligneCasePont;

                if (ligneOrdre.I_PHASE_DEBUT == phase)
                {
                  if (!InitialisationActionPont(Constantes.CST_DUREE_ENDOMMAGE_PONT, ligneOrdre, tour, phase, out ligneCasePont))
                    {
                        LogFile.Notifier("Erreur sur InitialisationActionPont dans ExecuterEndommagerPont");
                        return false;
                    }

                    if (null == ligneCasePont)
                    {
                        //on a pas trouvé de pont
                        //l'ordre est terminé si ce n'est pas déjà le cas
                        if (ligneOrdre.IsI_TOUR_FINNull())
                        {
                            TerminerOrdre(ligneOrdre, true, false);
                            message = string.Format("{0},ID={1}, ExecuterEndommagerPont: endommagement du pont terminée en {2}:{3},{4} car il n'y a pas de pont",
                                S_NOM, ID_PION, ligneCasePont.ID_CASE, ligneCasePont.I_X, ligneCasePont.I_Y);
                            LogFile.Notifier(message);
                        }
                        return true;
                    }

                    //On vérifie que le pont n'est pas déjà détruit (par une autre unité par exemple)
                    Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCasePont.ID_MODELE_TERRAIN);
                    if (ligneModeleTerrain.B_DETRUIT == true)
                    {
                        //on indique que le pont est détruit grace à nous (c'est de l'auto promo)
                        if (!ActionPontTermine(ClassMessager.MESSAGES.MESSAGE_PONT_DETRUIT, ligneOrdre, tour, phase, ligneCasePont))
                        {
                            message = string.Format("{0}(ID={1}, erreur sur ActionPontTermine avec MESSAGE_PONT_DETRUIT dans ExecuterEndommagerPont (déjà détruit) ", S_NOM, ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }
                        return true;
                    }

                    //On vérifie qu'aucune troupe ennemie combattive ne se trouve à proximité du lieu de réparation
                    if (RechercheEnnemiAProximite(ligneCasePont))
                    {
                        //on envoie un message pour prévenir l'officier commandant
                        if (!MessageEnnemiAProximite(tour)) { return false; }

                        //les réparations repartent du début !
                        ligneOrdre.I_TOUR_DEBUT = tour;
                        return true;
                    }

                    //fatigue liée à la destruction
                    FatigueActionPont(ligneOrdre, tour);

                    //bruit du canon liée à l'opération de destruction
                    AlerteBruitEndommagePont(ligneCasePont);

                    if (ligneOrdre.I_TOUR_DEBUT + ligneOrdre.I_DUREE <= tour)
                    {
                        message = string.Format("{0},ID={1}, ExecuterEndommagerPont: début de l'endommagement effectif", S_NOM, ID_PION);
                        LogFile.Notifier(message);

                        if (!ligneCasePont.EndommagerReparerPont())
                        {
                            message = string.Format("{0},ID={1}, ExecuterEndommagerPont: erreur lors de l'appel à Cartographie.EndommagerReparerPont sur la case {2}:{3},{4}",
                                S_NOM, ID_PION, ligneCasePont.ID_CASE, ligneCasePont.I_X, ligneCasePont.I_Y);
                            LogFile.Notifier(message);
                            return false;
                        }
                        //On envoit un message au supérieur pour lui indiquer que le pont est bien détruit.
                        if (!ClassMessager.EnvoyerMessage(this, ligneCasePont, ClassMessager.MESSAGES.MESSAGE_PONT_DETRUIT))
                        {
                            message = string.Format("{0}(ID={1}, erreur sur EnvoyerMessage avec MESSAGE_PONT_DETRUIT dans ExecuterDetruirePont", S_NOM, ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }

                        //l'ordre est terminé
                        TerminerOrdre(ligneOrdre, true, false);
                        message = string.Format("{0},ID={1}, ExecuterEndommagerPont: endommagement du pont terminée en {2}:{3},{4}",
                            S_NOM, ID_PION, ligneCasePont.ID_CASE, ligneCasePont.I_X, ligneCasePont.I_Y);
                        LogFile.Notifier(message);
                        return true;
                    }
                }
                else
                {
                    //rien pour l'instant
                    message = string.Format("{0},ID={1}, ExecuterEndommagerPont: fin le tour={2}, phase={3}, nous sommes à tour={4}, phase={5}",
                        S_NOM, ID_PION, ligneOrdre.I_TOUR_DEBUT + Constantes.CST_DUREE_ENDOMMAGE_PONT, ligneOrdre.I_PHASE_DEBUT, tour, phase);
                    LogFile.Notifier(message);
                }
                return true;
            }

            public bool ExecuterReparerPont(Donnees.TAB_ORDRERow ligneOrdre, int tour, int phase)
            {
                string message;
                Donnees.TAB_CASERow ligneCasePont;

                if (ligneOrdre.I_PHASE_DEBUT == phase)
                {
                    if (!InitialisationActionPont(Constantes.CST_DUREE_REPARER_PONT, ligneOrdre, tour, phase, out ligneCasePont))
                    {
                        LogFile.Notifier("Erreur sur InitialisationActionPont dans ExecuterReparerPont");
                        return false;
                    }

                    if (null == ligneCasePont)
                    {
                        //on a pas trouvé de pont
                        //l'ordre est terminé si ce n'est pas déjà le cas
                        if (ligneOrdre.IsI_TOUR_FINNull())
                        {
                            TerminerOrdre(ligneOrdre, true, false);
                            message = string.Format("{0},ID={1}, ExecuterEndommagerPont: endommagement du pont terminée en {2}:{3},{4} car il n'y a pas de pont",
                                S_NOM, ID_PION, ligneCasePont.ID_CASE, ligneCasePont.I_X, ligneCasePont.I_Y);
                            LogFile.Notifier(message);
                        }
                        return true;
                    }

                    //On vérifie que le pont n'est pas déjà réparé (par une autre unité par exemple)
                    Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCasePont.ID_MODELE_TERRAIN);
                    if (ligneModeleTerrain.B_DETRUIT == false)
                    {
                        //on indique que le pont est réparé grace à nous (c'est de l'auto promo)
                        if (!ActionPontTermine(ClassMessager.MESSAGES.MESSAGE_PONT_REPARE, ligneOrdre, tour, phase, ligneCasePont))
                        {
                            message = string.Format("{0}(ID={1}, erreur sur ActionPontTermine avec MESSAGE_PONT_REPARE dans ExecuterReparerPont (déjà détruit) ", S_NOM, ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }
                        return true;
                    }

                    //On vérifie qu'aucune troupe ennemie combattive ne se trouve à proximité du lieu de réparation
                    if (RechercheEnnemiAProximite(ligneCasePont))
                    {
                        //on envoie un message pour prévenir l'officier commandant
                        if (!MessageEnnemiAProximite(tour)) { return false; }

                        //les réparations repartent du début !
                        ligneOrdre.I_TOUR_DEBUT = tour;
                        return true;
                    }

                    //la réparation est en cours, il faut ajouter la fatigue
                    FatigueActionPont(ligneOrdre, tour);

                    //bruit du liée à l'opération de réparation
                    AlerteBruitReparePontConstruitPonton(ligneCasePont);

                    if (ligneOrdre.I_TOUR_DEBUT + ligneOrdre.I_DUREE <= tour)
                    {
                        message = string.Format("{0},ID={1}, ExecuterReparerPont: début de la réparation effective", S_NOM, ID_PION);
                        LogFile.Notifier(message);

                        if (!ligneCasePont.EndommagerReparerPont())
                        {
                            message = string.Format("{0},ID={1}, ExecuterReparerPont: erreur lors de l'appel à Cartographie.ReparerPont sur la case {2}:{3},{4}",
                                S_NOM, ID_PION, ligneCasePont.ID_CASE, ligneCasePont.I_X, ligneCasePont.I_Y);
                            LogFile.Notifier(message);
                            return false;
                        }
                        //On envoit un message au supérieur pour lui indiquer que le pont est bien réparé.
                        if (!ClassMessager.EnvoyerMessage(this, ClassMessager.MESSAGES.MESSAGE_PONT_REPARE))
                        {
                            message = string.Format("{0}(ID={1}, erreur sur EnvoyerMessage avec MESSAGE_PONT_REPARE dans ExecuterReparerPont", S_NOM, ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }

                        //l'ordre est terminé
                        TerminerOrdre(ligneOrdre, true, false);
                        message = string.Format("{0},ID={1}, ExecuterReparerPont: réparation du pont terminée en {2}:{3},{4}",
                            S_NOM, ID_PION, ligneCasePont.ID_CASE, ligneCasePont.I_X, ligneCasePont.I_Y);
                        LogFile.Notifier(message);
                        return true;
                    }
                }
                else
                {
                    //rien pour l'instant
                    message = string.Format("{0},ID={1}, ExecuterReparerPont: fin le tour={2}, phase={3}, nous sommes à tour={4}, phase={5}",
                        S_NOM, ID_PION, ligneOrdre.I_TOUR_DEBUT + Constantes.CST_DUREE_REPARER_PONT, ligneOrdre.I_PHASE_DEBUT, tour, phase);
                    LogFile.Notifier(message);
                }
                return true;
            }

            public bool ExecuterConstruirePonton(Donnees.TAB_ORDRERow ligneOrdre, int tour, int phase)
            {
                string message;
                Donnees.TAB_CASERow ligneCasePont;

                if (ligneOrdre.I_PHASE_DEBUT == phase)
                {
                    if (!InitialisationActionPont(Constantes.CST_DUREE_CONSTRUIRE_PONTON, ligneOrdre, tour, phase, out ligneCasePont))
                    {
                        LogFile.Notifier("Erreur sur InitialisationActionPont dans ExecuterConstruirePonton");
                        return false;
                    }

                    if (null == ligneCasePont)
                    {
                        //on a pas trouvé de ponton
                        return true;
                    }

                    //On vérifie que le ponton n'est pas déjà construit (par une autre unité par exemple)
                    Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCasePont.ID_MODELE_TERRAIN);
                    if (ligneModeleTerrain.B_PONTON == true)
                    {
                        //on indique que le pont est réparé grace à nous (c'est de l'auto promo)
                        if (!ActionPontTermine(ClassMessager.MESSAGES.MESSAGE_PONTON_CONSTRUIT, ligneOrdre, tour, phase, ligneCasePont))
                        {
                            message = string.Format("{0}(ID={1}, erreur sur ActionPontTermine avec MESSAGE_PONTON_CONSTRUIT dans ExecuterConstruirePonton (déjà construit) ", S_NOM, ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }
                        return true;
                    }

                    //On vérifie qu'aucune troupe ennemie combattive ne se trouve à proximité du lieu de construction
                    if (RechercheEnnemiAProximite(ligneCasePont))
                    {
                        //on envoie un message pour prévenir l'officier commandant
                        if (!MessageEnnemiAProximite(tour)) { return false; }

                        //les réparations repartent du début !
                        ligneOrdre.I_TOUR_DEBUT = tour;
                        return true;
                    }

                    //la réparation est en cours, il faut ajouter la fatigue
                    FatigueActionPont(ligneOrdre, tour);

                    if (ligneOrdre.I_TOUR_DEBUT + ligneOrdre.I_DUREE == tour)
                    {
                        message = string.Format("{0},ID={1}, ExecuterConstruirePonton: début de la construction effective", S_NOM, ID_PION);
                        LogFile.Notifier(message);

                        if (!ligneCasePont.ConstruirePonton())
                        {
                            message = string.Format("{0},ID={1}, ExecuterConstruirePonton: erreur lors de l'appel à Cartographie.ConstruirePonton sur la case {2}:{3},{4}",
                                S_NOM, ID_PION, ligneCasePont.ID_CASE, ligneCasePont.I_X, ligneCasePont.I_Y);
                            LogFile.Notifier(message);
                            return false;
                        }

                        //On envoit un message au supérieur pour lui indiquer que le ponton est construit.
                        if (!ClassMessager.EnvoyerMessage(this, ClassMessager.MESSAGES.MESSAGE_PONTON_CONSTRUIT))
                        {
                            message = string.Format("{0}(ID={1}, erreur sur EnvoyerMessage avec MESSAGE_PONTON_CONSTRUIT dans ExecuterConstruirePonton", S_NOM, ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }

                        //l'ordre est terminé
                        TerminerOrdre(ligneOrdre, true, false);
                        message = string.Format("{0},ID={1}, ExecuterConstruirePonton: construction du ponton terminé en {2}:{3},{4}",
                            S_NOM, ID_PION, ligneCasePont.ID_CASE, ligneCasePont.I_X, ligneCasePont.I_Y);
                        LogFile.Notifier(message);
                        return true;
                    }
                }
                else
                {
                    //rien pour l'instant
                    message = string.Format("{0},ID={1}, ExecuterConstruirePonton: fin le tour={2}, phase={3}, nous sommes à tour={4}, phase={5}",
                        S_NOM, ID_PION, ligneOrdre.I_TOUR_DEBUT + Constantes.CST_DUREE_ENDOMMAGE_PONT, ligneOrdre.I_PHASE_DEBUT, tour, phase);
                    LogFile.Notifier(message);
                }
                return true;
            }

            public bool ExecuterSeFortifier(Donnees.TAB_ORDRERow ligneOrdre, int tour, int phase)
            {
                string message;

                if (ligneOrdre.I_PHASE_DEBUT == phase)
                {
                    //On vérifie qu'aucune troupe ennemie combattive ne se trouve à proximité du lieu de construction
                    if (RechercheEnnemiAProximite(CaseCourante()))
                    {
                        //on envoie un message pour prévenir l'officier commandant
                        if (!MessageEnnemiAProximite(tour)) { return false; }

                        //les constructions repartent du début !
                        ligneOrdre.I_TOUR_DEBUT = tour;
                        return true;
                    }

                    //la construction est en cours, il faut ajouter la fatigue
                    /* Non, cela ne fatigue mais empêche de se reposer, gérer dans le traitement de fatigue de fin de journée
                    if (ligneOrdre.I_TOUR_DEBUT != tour)
                    {
                        //on ne construit pas les fortifications de nuit
                        if (!Donnees.m_donnees.TAB_PARTIE.Nocturne())
                        {
                            I_NB_PHASES_MARCHE_JOUR++;
                        }
                    }*/

                    if (ligneOrdre.I_TOUR_DEBUT + Constantes.CST_DUREE_CONSTRUIRE_FORTIFICATIONS == tour)
                    {
                        message = string.Format("{0},ID={1}, ExecuterSeFortifier ajout d'un niveau de fortification", S_NOM, ID_PION);
                        LogFile.Notifier(message);

                        if (I_NIVEAU_FORTIFICATION < Constantes.CST_FORTIFICATIONS_MAX)
                        {
                            I_NIVEAU_FORTIFICATION++;
                        }

                        //si l'on a atteint le niveau max l'ordre est terminé
                        if (I_NIVEAU_FORTIFICATION < Constantes.CST_FORTIFICATIONS_MAX)
                        {
                            //On envoit un message au supérieur pour lui indiquer que la fortification est construite.
                            if (!ClassMessager.EnvoyerMessage(this, ClassMessager.MESSAGES.MESSAGE_FORTIFICATION_TERMINEE_UN))
                            {
                                message = string.Format("{0}(ID={1}, erreur sur EnvoyerMessage avec MESSAGE_FORTIFICATION_TERMINEE_UN dans ExecuterSeFortifier", S_NOM, ID_PION);
                                LogFile.Notifier(message);
                                return false;
                            }

                            //sinon on continue vers le niveau suivant
                            ligneOrdre.I_TOUR_DEBUT = tour;
                        }
                        else
                        {
                            //On envoit un message au supérieur pour lui indiquer que la fortification est construite.
                            if (!ClassMessager.EnvoyerMessage(this, ClassMessager.MESSAGES.MESSAGE_FORTIFICATION_TERMINEE_MAX))
                            {
                                message = string.Format("{0}(ID={1}, erreur sur EnvoyerMessage avec MESSAGE_FORTIFICATION_TERMINEE_MAX dans ExecuterSeFortifier", S_NOM, ID_PION);
                                LogFile.Notifier(message);
                                return false;
                            }

                            TerminerOrdre(ligneOrdre, true, false);
                            message = string.Format("{0},ID={1}, ExecuterSeFortifier niveau max de fortification atteint",
                                S_NOM, ID_PION);
                            LogFile.Notifier(message);
                        }
                        return true;
                    }
                }
                else
                {
                    //rien pour l'instant
                    message = string.Format("{0},ID={1}, ExecuterSeFortifier: fin le tour={2}, phase={3}, nous sommes à tour={4}, phase={5}",
                        S_NOM, ID_PION, ligneOrdre.I_TOUR_DEBUT + Constantes.CST_DUREE_CONSTRUIRE_FORTIFICATIONS, ligneOrdre.I_PHASE_DEBUT, tour, phase);
                    LogFile.Notifier(message);
                }
                return true;
            }

            /// <summary>
            /// Recherche du meilleur depot assurant un ravitaillement direct
            /// </summary>
            /// <param name="meilleurPourcentageRavitaillement">pourcentage assuré par le meilleur depôt</param>
            /// <returns>Null si ne trouve aucun dépot, le meilleur depôt sinon</returns>
            public Donnees.TAB_PIONRow RechercheMeilleurDepotDirect(out int effectifsRavitailles, out int meilleurPourcentageRavitaillement, out int meilleurPourcentageMateriel)
            {
                string message, messageErreur;
                List<LigneCASE> chemin;
                AstarTerrain[] tableCoutsMouvementsTerrain;
                double cout, coutHorsRoute;
                Donnees.TAB_PIONRow ligneMeilleurDepot = null;
                AStar etoile = new AStar();

                meilleurPourcentageRavitaillement = 0;
                meilleurPourcentageMateriel = 0;
                effectifsRavitailles = 0;
                Donnees.TAB_CASERow ligneCaseDepart = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ID_CASE);
                int besoinEnRavitaillement = this.effectifTotal * (100 - this.I_MATERIEL + 100 - this.I_RAVITAILLEMENT) / 100;
                foreach (Donnees.TAB_PIONRow ligneDepot in Donnees.m_donnees.TAB_PION)
                {
                    if (ligneDepot.B_DETRUIT || !ligneDepot.estDepot || ligneDepot.estEnnemi(this)) { continue; } //on ne ravitaille que ses copains
                    Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneDepot.ID_CASE);

                    if (!etoile.RechercheChemin(Constantes.TYPEPARCOURS.RAVITAILLEMENT, this, ligneCaseDepart, ligneCaseDestination, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                    {
                        message = string.Format("{0}(ID={1}, erreur sur RechercheMeilleurDepotDirect :{2})", S_NOM, ID_PION, messageErreur);
                        LogFile.Notifier(message);
                        return null;
                    }

                    decimal distanceRavitaillement = (null == chemin) ? Constantes.CST_COUTMAX : (decimal)chemin.Count / Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;//distance en km
                    //il faut être proche pour pouvoir assurer un ravitaillement direct
                    if (distanceRavitaillement > Constantes.CST_DISTANCE_DEPOT_RAVITAILLEMENT) { continue; }

                    //Quand une unité est à moins de 1km d’un dépôt et ne bouge pas durant une journée(en repos), 
                    //elle est automatiquement ravitaillée à 100 % en ravitaillement et matériel, 100 % de ravitaillement 
                    //dans l’une ou l’autre d’une catégorie équivaut à un soldat complet ravitaillé. 
                    //Exemple, une unité de 500 soldats se repose à côté d’un dépôt avec 56 % de ravitaillement et 67 % de matériel, 
                    //elle retire 500 * (1 - 0,56 + 1 - 0,67) = 385 points « équivalents » soldats ravitaillés.
                    int pourcentageRavitaillement = 0;
                    int pourcentageMateriel = 0;
                    int ligneDepotTable = ligneDepot.C_NIVEAU_DEPOT - 'A';
                    int capaciteDepot = Constantes.tableLimiteRavitaillementDepot[ligneDepotTable] - ligneDepot.I_SOLDATS_RAVITAILLES;
                    if (0 == capaciteDepot) { continue; } //cas d'un dépôt A saturé
                    if (besoinEnRavitaillement <= capaciteDepot)
                    {
                        pourcentageRavitaillement = 100;
                        pourcentageMateriel = 100;
                        effectifsRavitailles = besoinEnRavitaillement;
                    }
                    else
                    {
                        //Si une unité se présente avec plus de besoin en ravitaillement que la capacité disponible du dépôt, 
                        //celle - ci est alimentée au prorata de ce qui reste. 
                        //L’unité sera ravitaillée à 0,56 + (1 - 0,56)*200 / 385 = 79 % en ravitaillement et à 0,67 + (1 - 0,67)*200 / 385 = 84 % en matériel
                        //(elle pourrait ensuite recevoir, en plus, le ravitaillement standard lié au dépôt).
                        pourcentageRavitaillement = I_RAVITAILLEMENT + (100 - I_RAVITAILLEMENT) * capaciteDepot / besoinEnRavitaillement;
                        pourcentageMateriel = I_MATERIEL + (100 - I_MATERIEL) * capaciteDepot / besoinEnRavitaillement;
                        effectifsRavitailles = capaciteDepot;
                    }

                    if (pourcentageRavitaillement == meilleurPourcentageRavitaillement && null != ligneMeilleurDepot)
                    {
                        //en cas d'égalité on prélève en priorité sur le dépôt le plus petit
                        if (ligneDepot.C_NIVEAU_DEPOT>ligneMeilleurDepot.C_NIVEAU_DEPOT)
                        {
                            ligneMeilleurDepot = ligneDepot;
                        }
                    }
                    else
                    {
                        if (pourcentageRavitaillement > meilleurPourcentageRavitaillement)
                        {
                            meilleurPourcentageRavitaillement = pourcentageRavitaillement;
                            meilleurPourcentageMateriel = pourcentageMateriel;
                            ligneMeilleurDepot = ligneDepot;
                        }
                    }
                }
                return ligneMeilleurDepot;
            }

            /// <summary>
            /// Recherche du meilleur depot, c'est à dire celui qui ravitaille le mieux hors ravitaillement direct
            /// </summary>
            /// <param name="meilleurDistanceRavitaillement">distance du meilleur dépôt</param>
            /// <param name="meilleurPourcentageRavitaillement">pourcentage assuré par le meilleur depôt</param>
            /// <returns>Null si ne trouve aucun dépot, le meilleur depôt sinon</returns>
            public Donnees.TAB_PIONRow RechercheMeilleurDepot(out decimal meilleurDistanceRavitaillement, out int meilleurPourcentageRavitaillement)
            {
                string message, messageErreur;
                List<LigneCASE> chemin;
                AstarTerrain[] tableCoutsMouvementsTerrain;
                double cout, coutHorsRoute;
                Donnees.TAB_PIONRow ligneMeilleurDepot = null;
                AStar etoile = new AStar();

                meilleurDistanceRavitaillement = -1;
                meilleurPourcentageRavitaillement = 0;
                Donnees.TAB_CASERow ligneCaseDepart = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ID_CASE);
                foreach (Donnees.TAB_PIONRow ligneDepot in Donnees.m_donnees.TAB_PION)
                {
                    if (ligneDepot.B_DETRUIT || !ligneDepot.estDepot || ligneDepot.estEnnemi(this)) { continue; } //on ne ravitaille que ses copains
                    Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneDepot.ID_CASE);

                    if (!etoile.RechercheChemin(Constantes.TYPEPARCOURS.RAVITAILLEMENT, this, ligneCaseDepart, ligneCaseDestination, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                    {
                        message = string.Format("{0}(ID={1}, erreur sur RechercheMeilleurDepot :{2})", S_NOM, ID_PION, messageErreur);
                        LogFile.Notifier(message);
                        return null;
                    }

                    decimal distanceRavitaillement = (null == chemin) ? Constantes.CST_COUTMAX : (decimal)chemin.Count / Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;//distance en km
                    int pourcentageRavitaillement = 0;
                    int colonneKilometreTable = -1;
                    if (distanceRavitaillement <= 10) { colonneKilometreTable = 0; }
                    if (distanceRavitaillement > 10 && distanceRavitaillement <= 25) { colonneKilometreTable = 1; }
                    if (distanceRavitaillement > 25 && distanceRavitaillement <= 50) { colonneKilometreTable = 2; }
                    if (distanceRavitaillement > 50 && distanceRavitaillement <= 100) { colonneKilometreTable = 3; }
                    if (distanceRavitaillement > 100 && distanceRavitaillement <= 150) { colonneKilometreTable = 4; }
                    if (distanceRavitaillement > 150)
                    {
                        pourcentageRavitaillement = 0;
                    }
                    else
                    {
                        int ligneDepotTable = ligneDepot.C_NIVEAU_DEPOT - 'A';
                        pourcentageRavitaillement = Constantes.tableRavitaillementDepot[ligneDepotTable, colonneKilometreTable];
                    }
                    if (pourcentageRavitaillement > meilleurPourcentageRavitaillement)
                    {
                        meilleurPourcentageRavitaillement = pourcentageRavitaillement;
                        meilleurDistanceRavitaillement = distanceRavitaillement;
                        ligneMeilleurDepot = ligneDepot;
                    }
                }
                return ligneMeilleurDepot;
            }

            /// <summary>
            /// Effectue le ravitaillement d'une unité
            /// </summary>
            /// <param name="bUniteRavitaillee">true si l'unité à été ravitaillée, false sinon</param>
            /// <returns>true si OK, false si KO</returns>
            public bool RavitaillementUnite(out bool bUniteRavitaillee, out decimal meilleurDistanceRavitaillement, out string depotRavitaillement)
            {
                string message;
                Donnees.TAB_PIONRow ligneMeilleurDepot = null;
                int meilleurPourcentageRavitaillement;
                Donnees.TAB_METEORow ligneMeteo = Donnees.m_donnees.TAB_METEO.FindByID_METEO(Donnees.m_donnees.TAB_PARTIE[0].ID_METEO);
                Donnees.TAB_NATIONRow ligneNation;
                //AStar etoile = new AStar();

                bUniteRavitaillee = false;
                meilleurDistanceRavitaillement = -1;
                depotRavitaillement = string.Empty;

                if (!estRavitaillable) { bUniteRavitaillee = true; return true; } //seules les unités combattantes doivent être ravitaillées
                ligneNation = this.nation;

                #region consommation journaliere
                //quelle que soit l'activité, l'unité perd 10% de son ravitaillement
                I_RAVITAILLEMENT = Math.Max(0, I_RAVITAILLEMENT - 10);
                //ainsi que 5% de son matériel par heure de combat
                I_MATERIEL = Math.Max(0, I_MATERIEL - (5 * I_NB_HEURES_COMBAT));
                #endregion

                #region fourrage : Vivre sur le terrain
                //l'unité reprend du ravitaillement en prenant sur le terrain en tenant compte de la météo
                if (I_RAVITAILLEMENT < 100)
                {
                    I_RAVITAILLEMENT = Math.Min(100, I_RAVITAILLEMENT + (this.modelePion.I_FOURRAGE * ligneMeteo.I_POURCENT_RAVITAILLEMENT / 100));
                }
                #endregion

                //si l'unité n'est pas en repos complet, elle ne peut pas se ravitailler à un dépôt
                if (!reposComplet)
                {
                    message = string.Format("{0}(ID={1}, l'unité a fait des actions, pas de ravitaillement par dépôt)", S_NOM, ID_PION);
                    LogFile.Notifier(message);
                    return true;
                }

                ligneMeilleurDepot = RechercheMeilleurDepot(out meilleurDistanceRavitaillement, out meilleurPourcentageRavitaillement);

                //modification suivant la météo en cours
                meilleurPourcentageRavitaillement = Math.Min(100, (int)((decimal)ligneMeteo.I_POURCENT_RAVITAILLEMENT * meilleurPourcentageRavitaillement / (decimal)100));
                if (null != ligneMeilleurDepot && meilleurPourcentageRavitaillement > 0)
                {
                    bUniteRavitaillee = true;
                    I_RAVITAILLEMENT = Math.Min(100, I_RAVITAILLEMENT + meilleurPourcentageRavitaillement);
                    I_MATERIEL = Math.Min(100, I_MATERIEL + meilleurPourcentageRavitaillement);
                    depotRavitaillement = ligneMeilleurDepot.S_NOM;
                    message = string.Format("{0}(ID={1}, Ravitaillement ok, distance 'effective' du dépôt {3} = {2} km)", S_NOM, ID_PION, meilleurDistanceRavitaillement, ligneMeilleurDepot.S_NOM);
                    LogFile.Notifier(message);
                }

                if (estRavitaillableDirect(Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE))
                {
                    string nomDepot;
                    if (!this.RavitaillementDirect(null, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE, out nomDepot))
                    {
                        return false;
                    }
                    //if (string.Empty != nomDepot) { depotRavitaillement = nomDepot; }, il ne faut pas changer le nom du dépot car le dépôt ayant servi à ravitailler en standard n'est pas toujours celui ayant ravitaillé en direct
                }

                return true;
            }

            /// <summary>
            /// Recherche l'hopital disponible le plus prêt pour le pion
            /// </summary>
            /// <returns>case de l'hopital si disponible, null sinon</returns>
            public Donnees.TAB_NOMS_CARTERow RechercheHopital()
            {
                int idNation = this.nation.ID_NATION;
                //un nom de carte peut être sans propriétaire en début de partie
                var resultComplet = from NomsCarte in Donnees.m_donnees.TAB_NOMS_CARTE
                                    where (!NomsCarte.IsID_NATION_CONTROLENull())
                                    && (NomsCarte.ID_NATION_CONTROLE == idNation)
                                    && (NomsCarte.B_HOPITAL == true)
                                    select NomsCarte;

                double distMini = double.MaxValue;
                Donnees.TAB_NOMS_CARTERow ligneHopitalMini = null;
                foreach (Donnees.TAB_NOMS_CARTERow ligneHopital in resultComplet)
                {
                    //Pion à distance correcte
                    Donnees.TAB_CASERow ligneCaseHopital = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneHopital.ID_CASE);
                    Donnees.TAB_CASERow ligneCasePion = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ID_CASE);
                    double dist = Constantes.Distance(ligneCasePion.I_X, ligneCasePion.I_Y, ligneCaseHopital.I_X, ligneCaseHopital.I_Y);
                    if (dist < distMini)
                    {
                        distMini = dist;
                        ligneHopitalMini = ligneHopital;
                    }
                }
                if (distMini < Constantes.CST_DISTANCE_RECHERCHE_HOPITAL * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE)
                {
                    return ligneHopitalMini;
                }
                return null;
            }

            /// <summary>
            /// Recherche la prison disponible la plus proche pour le pion
            /// </summary>
            /// <returns>case de la prison si disponible, null sinon</returns>
            public Donnees.TAB_NOMS_CARTERow RecherchePrison()
            {
                int idNation = this.nation.ID_NATION;
                var resultComplet = from NomsCarte in Donnees.m_donnees.TAB_NOMS_CARTE
                                    where (!NomsCarte.IsID_NATION_CONTROLENull())
                                    && (NomsCarte.ID_NATION_CONTROLE == idNation)
                                    && (NomsCarte.B_PRISON == true)
                                    select NomsCarte;

                double distMini = double.MaxValue;
                Donnees.TAB_NOMS_CARTERow lignePrisonMini = null;
                foreach (Donnees.TAB_NOMS_CARTERow lignePrison in resultComplet)
                {
                    //Pion à distance correcte
                    Donnees.TAB_CASERow ligneCaseHopital = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePrison.ID_CASE);
                    Donnees.TAB_CASERow ligneCasePion = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ID_CASE);
                    double dist = Constantes.Distance(ligneCasePion.I_X, ligneCasePion.I_Y, ligneCaseHopital.I_X, ligneCaseHopital.I_Y);
                    if (dist < distMini)
                    {
                        distMini = dist;
                        lignePrisonMini = lignePrison;
                    }
                }
                if (distMini < Constantes.CST_DISTANCE_RECHERCHE_PRISON * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE)
                {
                    return lignePrisonMini;
                }
                return null;
            }

            /// <summary>
            /// Calcul du point relatif d'avancement sur le trajet qui est le barycentre de l'avancement du pion
            /// position dans la chaine de mouvement = (encombrement arrivee * lg chaine occupée + somme de toutes  les position occupées dans la chaine )/ encombrement total
            /// </summary>
            /// <param name="lignePion">pion</param>
            /// <param name="ligneOrdre">ordre de mouvement</param>
            /// <param name="ligneNation">nation de l'unité</param>
            /// <param name="chemin">chemin du pion</param>
            /// <returns>position relative dans le parcours</returns>
            public int CalculPionPositionRelativeAvancement(Donnees.TAB_ORDRERow ligneOrdre, Donnees.TAB_NATIONRow ligneNation, out List<LigneCASE> chemin)
            {
                string message, messageErreur;
                decimal encombrementRoute = 0, encombrementArrivee = 0, encombrementTotal;
                int poidsEncombrementRoute = 0, poidsEncombrementArrivee = 0;
                int iCavalerie, iInfanterie, iArtillerie;
                int iCavalerieDestination, iInfanterieDestination, iArtillerieDestination;
                AstarTerrain[] tableCoutsMouvementsTerrain;
                double cout, coutHorsRoute;
                int nouvellePosition = 0;

                chemin = null;
                if (null == ligneOrdre || null == ligneNation)
                {
                    LogFile.Notifier("CalculPionPositionRelativeAvancement, l'un des paramètres est null");
                    return -1;
                }
                Donnees.TAB_CASERow ligneCaseDepart = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DEPART);
                Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DESTINATION);

                encombrementTotal = CalculerEncombrement(this.infanterie, this.cavalerie, this.artillerie, true);
                AStar etoile = new AStar();
                if (!etoile.RechercheChemin(Constantes.TYPEPARCOURS.MOUVEMENT, this, ligneCaseDepart, ligneCaseDestination, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                {
                    message = string.Format("{0}(ID={1}, erreur sur RechercheChemin dans CalculPionPositionRelativeAvancement:{2}", S_NOM, ID_PION, messageErreur);
                    LogFile.Notifier(message);
                    return -1;
                }

                if (ligneOrdre.I_EFFECTIF_DESTINATION > 0)
                {
                    #region Pion à destination
                    //l'unité est arrivée, il faut donc "écouler" les éléments qui ne sont pas encore arrivés s'il y en a
                    message = string.Format("\r\n{0}(CalculPionPositionRelativeAvancement : ID={1}, une partie des troupes à destination ligneOrdre.I_EFFECTIF_DESTINATION={2})",
                        S_NOM, ID_PION, ligneOrdre.I_EFFECTIF_DESTINATION);
                    LogFile.Notifier(message);

                    //effectifs actuellement à l'arrivée
                    CalculerRepartitionEffectif(ligneOrdre.I_EFFECTIF_DESTINATION,
                                                out iInfanterieDestination, out iCavalerieDestination, out iArtillerieDestination);
                    encombrementArrivee = CalculerEncombrement(iInfanterieDestination, iCavalerieDestination, iArtillerieDestination, true);
                    poidsEncombrementArrivee = (int)(encombrementArrivee * chemin.Count());
                    message = string.Format("CalculPionPositionRelativeAvancement :effectif à destination :i={0} c={1} a={2} avec un encombrement de {3} et un poids de {4}",
                                            iInfanterieDestination, iCavalerieDestination, iArtillerieDestination, encombrementArrivee, poidsEncombrementArrivee);
                    LogFile.Notifier(message);

                    if (ligneOrdre.I_EFFECTIF_DESTINATION < this.effectifTotal)
                    {
                        //calcul de l'encombrement des gens encore sur la route
                        if (0 == ligneOrdre.I_EFFECTIF_DEPART)
                        {
                            //la route est partiellement occupée
                            encombrementRoute = encombrementTotal - encombrementArrivee;
                        }
                        else
                        {
                            //la route est complètement occupée
                            encombrementRoute = chemin.Count;
                        }
                        //on part de l'arrivee pour calculer le poids de chaque élément de route
                        // somme Sn = (n+1)/2*(Uo + Un)
                        //poidsEncombrementRoute = encombrementRoute / 2 * (chemin.Count - encombrementRoute + chemin.Count - 1);
                        poidsEncombrementRoute = (int)(encombrementRoute * (chemin.Count - encombrementRoute + chemin.Count - 1) / 2);
                        message = string.Format("CalculPionPositionRelativeAvancement :encombrement sur route={0}, poids ={2}, effectif destination={1}", encombrementRoute, ligneOrdre.I_EFFECTIF_DESTINATION, poidsEncombrementRoute);
                        LogFile.Notifier(message);
                    }
                    #endregion
                }
                else
                {
                    #region Pion non encore arrivé à destination
                    message = string.Format("{0}(ID={1}, CalculPionPositionRelativeAvancement, aucune troupe à destination, ligneOrdre.I_EFFECTIF_DEPART={2})", S_NOM, ID_PION, ligneOrdre.I_EFFECTIF_DEPART);
                    LogFile.Notifier(message);

                    //on calcule de combien de cases le pion a déjà avancé
                    this.CalculerRepartitionEffectif(
                        this.effectifTotalEnMouvement - ligneOrdre.I_EFFECTIF_DEPART,
                        out iInfanterie, out iCavalerie, out iArtillerie);
                    message = string.Format("PlacerPionEnBivouac :effectif sur route: i={0} c={1} a={2}", iInfanterie, iCavalerie, iArtillerie);
                    LogFile.Notifier(message);

                    encombrementRoute = CalculerEncombrement(iInfanterie, iCavalerie, iArtillerie, true);

                    //recherche du plus court chemin
                    if (!etoile.RechercheChemin(Constantes.TYPEPARCOURS.MOUVEMENT, this, ligneCaseDepart, ligneCaseDestination, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                    {
                        message = string.Format("{0}(ID={1}, erreur sur RechercheChemin (cas 2) dans CalculPionPositionRelativeAvancement: {2})", S_NOM, ID_PION, messageErreur);
                        LogFile.Notifier(message);
                        return -1;
                    }

                    //on recherche la position actuelle qui correspond à la queue du mouvement
                    int i = 0;
                    while (i < chemin.Count && chemin[i].ID_CASE != ID_CASE)
                    {
                        i++;
                    }

                    //on part de l'arrivee pour calculer le poids de chaque élément de route
                    // somme Sn = (n+1)/2*(Uo + Un)                
                    poidsEncombrementRoute = (int)(encombrementRoute * (i + i + encombrementRoute - 1) / 2);
                    poidsEncombrementArrivee = 0;
                    message = string.Format("PlacerPionEnBivouac : encombrement sur route={0} poids={1}", encombrementRoute, poidsEncombrementArrivee);
                    LogFile.Notifier(message);
                    #endregion
                }

                //calcul de la case finale sur le bivouac, avec des pbs d'arrondi, il est possible que la nouvelle position dépasse la taille du chemin
                nouvellePosition = (int)Math.Min(chemin.Count - 1, (poidsEncombrementArrivee + poidsEncombrementRoute) / encombrementTotal);
                message = string.Format("{0}(ID={1}) CalculPionPositionRelativeAvancement, la troupe en {2} position en {3}", S_NOM, ID_PION, ID_CASE, nouvellePosition);
                LogFile.Notifier(message);

                return nouvellePosition;
            }

            public bool PlacerPionEnBivouac(Donnees.TAB_ORDRERow ligneOrdre)
            {
                Donnees.TAB_NATIONRow ligneNation = this.nation;// Donnees.m_donnees.TAB_PION.TrouveNation(lignePion);
                return PlacerPionEnBivouac(ligneOrdre, ligneNation);
            }
            /// <summary>
            /// Calcul du point de bivouac qui est le barycentre de l'avancement du pion
            /// position dans la chaine de mouvement = (encombrement arrivee * lg chaine occupée + somme de toutes  les position occupées dans la chaine )/ encombrement total
            /// </summary>
            /// <param name="lignePion">pion en bivouac</param>
            /// <param name="ligneOrdre">ordre de mouvement</param>
            /// <param name="ligneNation">nation de l'unité</param>
            /// <returns></returns>
            public bool PlacerPionEnBivouac(Donnees.TAB_ORDRERow ligneOrdre, Donnees.TAB_NATIONRow ligneNation)
            {
                string message;
                List<LigneCASE> chemin;
                int nouvellePosition = 0;

                if (null == ligneOrdre || null == ligneNation)
                {
                    LogFile.Notifier("PlacerPionEnBivouac, l'un des paramètres est null");
                    return false;
                }

                //les pions sans encombrement ne se place pas en bivouac
                if (0 == CalculerEncombrement(false)) { return true; }

                //les pions qui ne sont pas en cours de mouvement, sont déjà en bivouac !
                if (ligneOrdre.I_ORDRE_TYPE != Constantes.ORDRES.MOUVEMENT) { return true; }

                //calcul de la case finale sur le bivouac, avec des pbs d'arrondi, il est possible que la nouvelle position dépasse la taille du chemin
                nouvellePosition = CalculPionPositionRelativeAvancement(ligneOrdre, ligneNation, out chemin);
                if (nouvellePosition >= 0)
                {
                    message = string.Format("{0}(ID={1}) PlacerPionEnBivouac, la troupe en {2} fait son bivouac en {3}", S_NOM, ID_PION, ID_CASE, chemin[nouvellePosition].ID_CASE);
                    LogFile.Notifier(message);
                }
                else
                {
                    message = string.Format("{0}(ID={1}) PlacerPionEnBivouac, erreur recontrée dans CalculPionPositionRelativeAvancement",
                        S_NOM, ID_PION);
                    LogFile.Notifier(message);
                    return false;
                }

                ID_CASE = chemin[nouvellePosition].ID_CASE;
                ligneOrdre.ID_CASE_DEPART = ID_CASE;
                ligneOrdre.I_EFFECTIF_DEPART = this.effectifTotalEnMouvement;
                ligneOrdre.I_EFFECTIF_DESTINATION = 0;
                return true;
            }

            /// <summary>
            /// Calcul de l'encombrement des troupes en pixels
            /// note : les blessés occupent le même encombrement qu'une unité standard
            /// note : les prisonniers ont une escorte égale à 10% du nombre de prisonniers dont il faut tenir compte
            /// </summary>
            /// <param name="effectifInfanterie">effectifs d'infanterie</param>
            /// <param name="effectifCavalerie">effectifs de cavalerie</param>
            /// <param name="effectifArtillerie">effectifs d'artillerie</param>
            /// <param name="moral">moral de l'unité</param>
            /// <param name="enMouvement">true si unité en mouvement, false si à l'arrêt</param>
            /// <returns>valeur d'encombrement en pixels</returns>
            public decimal CalculerEncombrement(int effectifInfanterie, int effectifCavalerie, int effectifArtillerie, bool enMouvement)
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
                        return (int)((decimal)effectifInfanterie * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE / Constantes.CST_ENCOMBREMENT_INFANTERIE);
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
                    encombrement = ((decimal)effectifInfanterie * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE / Constantes.CST_ENCOMBREMENT_INFANTERIE
                        + (decimal)effectifCavalerie * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE / Constantes.CST_ENCOMBREMENT_CAVALERIE
                        + (decimal)effectifArtillerie * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE / Constantes.CST_ENCOMBREMENT_ARTILLERIE);
                }
                else
                {
                    //encombrement a l'arrêt
                    encombrement = (decimal)(effectifInfanterie + effectifCavalerie + effectifArtillerie) * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE / Constantes.CST_ENCOMBREMENT_ARRET;
                }

                if (this.estPrisonniers)
                {
                    //il faut ajouter les 10% supplémentaires liés à l'escorte, pas de distinction infanterie/cavalerie car si les prisonniers ont été capturés par une unité de cavalerie, ils vont être escortés 
                    // par des cavaliers et non des fantassins, donc, je fais "en gros"
                    //sinon, je pourrai aussi prendre la taille réelle de l'escorte mais c'est beaucoup de temps pour pas grand chose
                    encombrement += encombrement / 10;
                }

                // on ajoute les fourgons
                encombrement = ((100 + this.modelePion.I_FOURGON) * encombrement) / 100;
                //on tient compte du moral
                if (I_MORAL <= 0) encombrement *= 2;

                //return (int)Math.Round(encombrement);
                return encombrement;
            }

            /// <summary>
            /// Calcul de l'encombrement des troupes en pixels
            /// </summary>
            /// <param name="enMouvement">true si unité en mouvement, false si à l'arrêt</param>
            /// <returns>valeur d'encombrement en pixels</returns>
            public decimal CalculerEncombrement(bool enMouvement)
            {
                return CalculerEncombrement(this.infanterie, this.cavalerie, this.artillerie, enMouvement);
            }

            /// <summary>
            /// Gain d'expérience une unité ayant participées à une bataille de 4 heures ou plus
            /// </summary>
            /// <returns>true si ok, false si ko</returns>
            public bool GainExperienceFinDeBataille()
            {
                string message;

                //if (lignePion.B_DETRUIT) { return true; } -> même les unités détruites gagnent de l'expérience car cela compte ensuite pour les blessés et les prisonniers
                if (estArtillerie) { return true; }//pas d'expérience pour les unités d'artillerie
                decimal experienceAvant = I_EXPERIENCE;
                I_EXPERIENCE = Math.Min(2, I_EXPERIENCE + Constantes.CST_GAIN_EXPERIENCE_BATAILLE);// 2 = expérience maximale
                message = string.Format("GainExperienceFinDeBataille {0}:{1} voit son expérience passée de {2} à {3}",
                    ID_PION, S_NOM, experienceAvant, I_EXPERIENCE);
                LogFile.Notifier(message);
                return true;
            }

            /// <summary>
            /// Creer un nouveau chef à partir du pion actuel, en en créant un nouveau
            /// </summary>
            /// <returns>renvoie le nouveau chef</returns>
            [Obsolete("Il faut utiliser CreationRemplacantChefBlesse2")]
            public Donnees.TAB_PIONRow CreationRemplacantChefBlesse()
            {
                int[] des;
                int[] effectifs;
                int[] canons;
                int[] modificateurs;
                int nbUnites012 = 0, nbUnites345 = 0;//pour les bonus stratégiques
                Donnees.TAB_PIONRow[] lignePionsEnBataille012;
                Donnees.TAB_PIONRow[] lignePionsEnBataille345;
                int idLeader;
                string message;
                string nom = string.Empty;
                int tactiquePromu = Constantes.JetDeDes(1) / 3;//valeur tactique du nouveau chef

                //on prend le nom suivant dans la liste
                int i = 0;
                while ((nom == string.Empty) && i < Donnees.m_donnees.TAB_NOMS_PROMUS.Count)
                {
                    Donnees.TAB_NOMS_PROMUSRow ligneNomPromu = Donnees.m_donnees.TAB_NOMS_PROMUS[i++];
                    if ((ligneNomPromu.IsB_NOM_PROMUNull() || !ligneNomPromu.B_NOM_PROMU) && ligneNomPromu.ID_NATION == this.idNation)
                    {
                        nom = ligneNomPromu.S_NOM;
                        ligneNomPromu.B_NOM_PROMU = true;
                    }
                }
                if (nom == string.Empty)
                {
                    LogFile.Notifier("CreationRemplacantChefBlesse : Erreur, il n'y a plus de noms disponibles dans TAB_NOMS_PROMUS");
                    return null;
                }

                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                Donnees.TAB_PIONRow lignePionRemplacant = Donnees.m_donnees.TAB_PION.AddTAB_PIONRow(
                    ID_MODELE_PION,
                    ID_PION_PROPRIETAIRE,
                    -1,//int ID_NOUVEAU_PION_PROPRIETAIRE,
                    -1,//int ID_ANCIEN_PION_PROPRIETAIRE,
                    nom,
                    I_INFANTERIE,
                    I_INFANTERIE,
                    I_CAVALERIE,
                    I_CAVALERIE,
                    I_ARTILLERIE,
                    I_ARTILLERIE,
                    I_FATIGUE,
                    I_MORAL,
                    I_MORAL_MAX,
                    0, //int I_EXPERIENCE,
                    tactiquePromu,//int I_TACTIQUE,
                    Math.Max(2, I_STRATEGIQUE - 1),//int I_STRATEGIQUE,
                    C_NIVEAU_HIERARCHIQUE++,//char C_NIVEAU_HIERARCHIQUE, // 'A' c'est le meilleur, 'D' le pire
                    0,//int I_DISTANCE_A_PARCOURIR,
                    0,//int I_NB_PHASES_MARCHE_JOUR,
                    0,//int I_NB_PHASES_MARCHE_NUIT,
                    0,//int I_NB_HEURES_COMBAT,
                    ID_CASE,
                    0,//I_TOUR_SANS_RAVITAILLEMENT,
                    -1,//int ID_BATAILLE,
                    -1,//int I_ZONE_BATAILLE,
                    I_TOUR_RETRAITE_RESTANT,
                    I_TOUR_FUITE_RESTANT,
                    false,//bool B_DETRUIT,
                    B_FUITE_AU_COMBAT,
                    false,//bool B_INTERCEPTION,
                    B_REDITION_RAVITAILLEMENT,
                    true,//bool B_TELEPORTATION,
                    B_ENNEMI_OBSERVABLE,
                    I_MATERIEL,
                    I_RAVITAILLEMENT,
                    B_CAVALERIE_DE_LIGNE,
                    B_CAVALERIE_LOURDE,
                    B_GARDE,
                    B_VIEILLE_GARDE,
                    0,//I_TOUR_CONVOI_CREE,
                    -1,//ID_DEPOT_SOURCE
                    I_SOLDATS_RAVITAILLES,
                    I_NB_HEURES_FORTIFICATION,
                    I_NIVEAU_FORTIFICATION,
                    -1,//int ID_PION_REMPLACE,
                    0,//int I_DUREE_HORS_COMBAT,
                    -1,//int I_TOUR_BLESSURE,
                    false,//bool B_BLESSES,
                    false,//bool B_PRISONNIERS,
                    false,//bool B_RENFORT,
                    -1,//int ID_LIEU_RATTACHEMENT,
                    'Z',//char C_NIVEAU_DEPOT,
                    -1,//int ID_PION_ESCORTE, 
                    0,//int I_INFANTERIE_ESCORTE, 
                    0,//int I_CAVALERIE_ESCORTE,
                    0,//int I_MATERIEL_ESCORTE
                    0,//I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT
                    0,//I_VICTOIRE
                    -1,//I_TRI
                    string.Empty,
                    -1
                    );

                if (null == lignePionRemplacant)
                {
                    LogFile.Notifier("CreationRemplacantChefBlesse : Erreur à l'appel de AddTAB_PIONRow");
                    Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    return null;
                }

                //le remplaçant est engagé dans la bataille de son prédécesseur mais pas dans une zone
                if (IsID_BATAILLENull()) lignePionRemplacant.SetID_BATAILLENull();
                else
                {
                    lignePionRemplacant.ID_BATAILLE = ID_BATAILLE;
                    Monitor.Enter(Donnees.m_donnees.TAB_BATAILLE_PIONS.Rows.SyncRoot); 
                    if (null == Donnees.m_donnees.TAB_BATAILLE_PIONS.AddTAB_BATAILLE_PIONSRow(
                        lignePionRemplacant.ID_BATAILLE, lignePionRemplacant.ID_PION, lignePionRemplacant.nation.ID_NATION, false, false /*bEnDefense*/,
                        lignePionRemplacant.I_INFANTERIE,
                        -1,//i_INFANTERIE_FIN
                        lignePionRemplacant.I_CAVALERIE,
                        -1,
                        lignePionRemplacant.I_ARTILLERIE,
                        -1,
                        lignePionRemplacant.I_MORAL,
                        -1,
                        lignePionRemplacant.I_FATIGUE,
                        -1,
                        false,//B_RETRAITE
                        false,//B_ENGAGEMENT
                        -1 // I_ZONE_BATAILLE_ENGAGEMENT
                    ))
                    {
                        Monitor.Exit(Donnees.m_donnees.TAB_BATAILLE_PIONS.Rows.SyncRoot);
                        LogFile.Notifier("CreationRemplacantChefBlesse : Erreur à l'appel de AddTAB_BATAILLE_PIONSRow");
                        Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        return null;
                    }
                    Monitor.Exit(Donnees.m_donnees.TAB_BATAILLE_PIONS.Rows.SyncRoot);

                    //le chef blessé n'est plus engagé (considéré en retraite)
                    Donnees.TAB_BATAILLE_PIONSRow ligneBataillePion = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(ID_PION, ID_BATAILLE);
                    ligneBataillePion.B_RETRAITE = true;
                    SetID_BATAILLENull();
                    SetI_ZONE_BATAILLENull();

                    //si le chef blessé dirigeait le combat, il faut déterminer qui est le nouveau commandant sur le champ de bataille
                    Donnees.TAB_BATAILLERow ligneBataille = Donnees.m_donnees.TAB_BATAILLE.FindByID_BATAILLE(lignePionRemplacant.ID_BATAILLE);
                    if (!ligneBataille.RecherchePionsEnBataille(out nbUnites012, out nbUnites345, out des, out modificateurs, out effectifs, out canons, out lignePionsEnBataille012, out lignePionsEnBataille345, null/*bengagement*/, false/*bcombattif*/, true/*QG*/, false /*bArtillerie*/))
                    {
                        message = string.Format("CreationRemplacantChefBlesse : erreur dans RecherchePionsEnBataille QG");
                        LogFile.Notifier(message);
                    }

                    if (ligneBataille.ID_LEADER_012 == ID_PION)
                    {
                        idLeader = ligneBataille.TrouverLeaderBataille(lignePionsEnBataille012, true);
                        if (idLeader >= 0)
                        {
                            ligneBataille.ID_LEADER_012 = idLeader;
                        }
                        else
                        {
                            ligneBataille.SetID_LEADER_012Null();
                        }
                    }
                    if (ligneBataille.ID_LEADER_345 == ID_PION)
                    {
                        idLeader = ligneBataille.TrouverLeaderBataille(lignePionsEnBataille345, false);
                        if (idLeader >= 0)
                        {
                            ligneBataille.ID_LEADER_345 = idLeader;
                        }
                        else
                        {
                            ligneBataille.SetID_LEADER_345Null();
                        }
                    }
                }

                lignePionRemplacant.SetID_NOUVEAU_PION_PROPRIETAIRENull();
                lignePionRemplacant.SetID_ANCIEN_PION_PROPRIETAIRENull();
                lignePionRemplacant.SetI_ZONE_BATAILLENull();
                lignePionRemplacant.SetID_PION_REMPLACENull();
                lignePionRemplacant.SetI_TOUR_BLESSURENull();
                lignePionRemplacant.SetID_LIEU_RATTACHEMENTNull();
                lignePionRemplacant.SetID_PION_ESCORTENull();
                lignePionRemplacant.SetID_DEPOT_SOURCENull();
                lignePionRemplacant.SetI_TRINull();
                lignePionRemplacant.SetI_TOUR_ENNEMI_OBSERVABLENull();
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                return lignePionRemplacant;
            }

            /// <summary>
            /// Creer un nouveau chef à partir du pion actuel, déplace les anciennes valeurs sur le nouveau pion
            /// le pion du rôle, et ses liens, ne sont donc pas modifiés
            /// </summary>
            /// <returns>renvoie l'ancien chef dont les valeur sont "stockées" (pour un eventuel retour)</returns>
            public Donnees.TAB_PIONRow CreationRemplacantChefBlesse2()
            {
                string nom = string.Empty;
                int tactiquePromu = Constantes.JetDeDes(1) / 3;//valeur tactique du nouveau chef

                //on prend le nom suivant dans la liste
                int i = 0;
                while ((nom == string.Empty) && i < Donnees.m_donnees.TAB_NOMS_PROMUS.Count)
                {
                    Donnees.TAB_NOMS_PROMUSRow ligneNomPromu = Donnees.m_donnees.TAB_NOMS_PROMUS[i++];
                    if ((ligneNomPromu.IsB_NOM_PROMUNull() || !ligneNomPromu.B_NOM_PROMU) && ligneNomPromu.ID_NATION == this.idNation)
                    {
                        nom = ligneNomPromu.S_NOM;
                        ligneNomPromu.B_NOM_PROMU = true;
                    }
                }
                if (nom == string.Empty)
                {
                    LogFile.Notifier("CreationRemplacantChefBlesse : Erreur, il n'y a plus de noms disponibles dans TAB_NOMS_PROMUS");
                    return null;
                }

                //copie de l'ancien chef comme nouveau pion, blessé
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot); 
                Donnees.TAB_PIONRow ligneAncienPion = Donnees.m_donnees.TAB_PION.AddTAB_PIONRow(
                    ID_MODELE_PION,
                    ID_PION_PROPRIETAIRE,
                    -1,//int ID_NOUVEAU_PION_PROPRIETAIRE,
                    -1,//int ID_ANCIEN_PION_PROPRIETAIRE,
                    S_NOM,
                    I_INFANTERIE,
                    I_INFANTERIE,
                    I_CAVALERIE,
                    I_CAVALERIE,
                    I_ARTILLERIE,
                    I_ARTILLERIE,
                    I_FATIGUE,
                    I_MORAL,
                    I_MORAL_MAX,
                    I_EXPERIENCE,
                    I_TACTIQUE,
                    I_STRATEGIQUE,
                    C_NIVEAU_HIERARCHIQUE,//char C_NIVEAU_HIERARCHIQUE, // 'A' c'est le meilleur, 'D' le pire
                    0,//int I_DISTANCE_A_PARCOURIR,
                    0,//int I_NB_PHASES_MARCHE_JOUR,
                    0,//int I_NB_PHASES_MARCHE_NUIT,
                    0,//int I_NB_HEURES_COMBAT,
                    ID_CASE,
                    0,//I_TOUR_SANS_RAVITAILLEMENT,
                    -1,//int ID_BATAILLE,
                    -1,//int I_ZONE_BATAILLE,
                    I_TOUR_RETRAITE_RESTANT,
                    I_TOUR_FUITE_RESTANT,
                    false,//bool B_DETRUIT,
                    B_FUITE_AU_COMBAT,
                    false,//bool B_INTERCEPTION,
                    B_REDITION_RAVITAILLEMENT,
                    false,//bool B_TELEPORTATION,
                    B_ENNEMI_OBSERVABLE,
                    I_MATERIEL,
                    I_RAVITAILLEMENT,
                    B_CAVALERIE_DE_LIGNE,
                    B_CAVALERIE_LOURDE,
                    B_GARDE,
                    B_VIEILLE_GARDE,
                    0,//I_TOUR_CONVOI_CREE,
                    -1,//ID_DEPOT_SOURCE
                    I_SOLDATS_RAVITAILLES,
                    I_NB_HEURES_FORTIFICATION,
                    I_NIVEAU_FORTIFICATION,
                    -1,//int ID_PION_REMPLACE,
                    I_DUREE_HORS_COMBAT,
                    -1,//int I_TOUR_BLESSURE,
                    B_BLESSES,//bool B_BLESSES,
                    false,//bool B_PRISONNIERS,
                    false,//bool B_RENFORT,
                    -1,//int ID_LIEU_RATTACHEMENT,
                    'Z',//char C_NIVEAU_DEPOT,
                    -1,//int ID_PION_ESCORTE, 
                    0,//int I_INFANTERIE_ESCORTE, 
                    0,//int I_CAVALERIE_ESCORTE,
                    0,//int I_MATERIEL_ESCORTE
                    0,//I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT
                    0,//I_VICTOIRE
                    I_TRI,//I_TRI
                    S_ENNEMI_OBSERVABLE,
                    I_TOUR_ENNEMI_OBSERVABLE
                    );

                if (null == ligneAncienPion)
                {
                    Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot); 
                    LogFile.Notifier("CreationRemplacantChefBlesse : Erreur à l'appel de AddTAB_PIONRow");
                    return null;
                }

                //on affecte les nouvelles valeurs
                S_NOM = nom;
                I_TACTIQUE = tactiquePromu;
                I_STRATEGIQUE = Math.Max(2, I_STRATEGIQUE - 1);
                C_NIVEAU_HIERARCHIQUE++;//char C_NIVEAU_HIERARCHIQUE, // 'A' c'est le meilleur, 'D' le pire
                I_DUREE_HORS_COMBAT = 0;
                B_BLESSES = false;
                //ID_PION_REMPLACE = ligneAncienPion.ID_PION;
                Donnees.TAB_PIONRow lignePrecedentChefRemplace = pionRemplace;
                if (null != lignePrecedentChefRemplace)
                {
                    //un chef remplaçant blessé est simplement soustrait par son nouveau remplaçant, on ne gère pas son retour
                    ID_PION_REMPLACE = lignePrecedentChefRemplace.ID_PION;
                }
                else
                {
                    ID_PION_REMPLACE = ligneAncienPion.ID_PION;
                }

                //le chef blessé n'est plus engagé (considéré en retraite)
                //ID_BATAILLE = 69;//juste pour avancer
                //ce qui suit ne marche pas, le leader est désengagé, le bataille est donc non renseigné, on recherche donc la dernière bataille ou l'officer a été engagé
                System.Nullable<int> IdBtatailleBlesse =
                    (from BataillePions in Donnees.m_donnees.TAB_BATAILLE_PIONS
                     where BataillePions.ID_PION == ID_PION
                     select BataillePions.ID_BATAILLE).Max();
                Donnees.TAB_BATAILLE_PIONSRow ligneBataillePion = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(ID_PION, (int)IdBtatailleBlesse);
                ligneBataillePion.B_RETRAITE = true;
                SetID_BATAILLENull();
                SetI_ZONE_BATAILLENull();

                ligneAncienPion.SetID_NOUVEAU_PION_PROPRIETAIRENull();
                ligneAncienPion.SetID_ANCIEN_PION_PROPRIETAIRENull();
                ligneAncienPion.SetI_ZONE_BATAILLENull();
                ligneAncienPion.SetID_PION_REMPLACENull();
                ligneAncienPion.SetI_TOUR_BLESSURENull();
                ligneAncienPion.SetID_LIEU_RATTACHEMENTNull();
                ligneAncienPion.SetID_PION_ESCORTENull();
                ligneAncienPion.SetID_DEPOT_SOURCENull();
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot); 

                return ligneAncienPion;
            }

            public bool ArriveADestination(Donnees.TAB_ORDRERow ligneOrdre, Donnees.TAB_NATIONRow ligneNation)
            {
                //les derniers viennent d'arriver
                
                string message = string.Format("{0}(ID={1}, en mouvement, les derniers sont arrivés)", S_NOM, ID_PION);
                LogFile.Notifier(message);
                Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                ligneOrdre.I_EFFECTIF_DEPART = 0;
                ligneOrdre.I_EFFECTIF_DESTINATION = this.effectifTotalEnMouvement;
                Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                ID_CASE = ligneOrdre.ID_CASE_DESTINATION;
                I_DISTANCE_A_PARCOURIR = 0;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                //placer l'unité sur la carte
                //la zone d'arrivée devient la zone de depart pour le placement des troupes
                Donnees.m_donnees.TAB_ESPACE.DeplacerEspacePion(ID_PION, AStar.CST_DESTINATION);

                if (null == ligneNation)
                {
                    ligneNation = this.nation; // Donnees.m_donnees.TAB_PION.TrouveNation(lignePion);
                }
                PlacementPion(ligneNation, true);

                if (!ArriveeSurPrisonOuHopital()) { return false; }

                //si l'unité est arrivée à une prison ou un hopital, un message est envoyée et l'unitée est marquée
                //détruite, inutile donc d'envoyer un nouveau message
                if (!B_DETRUIT)
                {
                    //envoyer un messager pour prévenir de l'arrivée complète
                    if (!ClassMessager.EnvoyerMessage(this, ClassMessager.MESSAGES.MESSAGE_ARRIVE_A_DESTINATION))
                    {
                        message = string.Format("ArriveADestination  : erreur lors de l'envoi d'un message pour indiquer qu'une unité arrive à destination");
                        LogFile.Notifier(message);
                        return false;
                    }
                }

                // l'ordre de mouvement est terminé
                this.TerminerOrdre(ligneOrdre, true, false);

                return true;
            }

            public bool ArriveeSurPrisonOuHopital()
            {
                string message;
                if (this.estPrisonniers)
                {
                    //si le convoi termine son mouvement à coté d'une case de prison, les prisonniers sont mis en prison et l'escorte est disponible comme renfort
                    Donnees.TAB_NOMS_CARTERow lignePrison = this.RecherchePrison();
                    if (null == lignePrison)
                    {
                        if (!ClassMessager.EnvoyerMessage(this, ClassMessager.MESSAGES.MESSAGE_ARRIVE_A_DESTINATION))
                        {
                            message = string.Format("ExecuterMouvementSansEffectif  : erreur lors de l'envoi d'un message pour indiquer qu'un convoi de prisonniers arrive à destination");
                            LogFile.Notifier(message);
                            return false;
                        }
                    }
                    else
                    {
                        Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        ID_LIEU_RATTACHEMENT = lignePrison.ID_NOM;

                        //Maintenant on crée le pion de renfort venant de l'escorte
                        Donnees.TAB_PIONRow lignePionEscorte = this.escorte;
                        Donnees.TAB_PIONRow pionRenfort = lignePionEscorte.CreerConvoi(lignePionEscorte.proprietaire, false /*bBlesses*/, false /*bPrisonniers*/, true /*bRenfort*/);
                        pionRenfort.I_INFANTERIE = I_INFANTERIE_ESCORTE;
                        pionRenfort.I_CAVALERIE = I_CAVALERIE_ESCORTE;
                        pionRenfort.I_ARTILLERIE = 0;
                        pionRenfort.I_EXPERIENCE = lignePionEscorte.I_EXPERIENCE;
                        pionRenfort.I_TACTIQUE = lignePionEscorte.I_TACTIQUE;
                        pionRenfort.I_INFANTERIE_INITIALE = pionRenfort.I_INFANTERIE;
                        pionRenfort.I_CAVALERIE_INITIALE = pionRenfort.I_CAVALERIE;
                        pionRenfort.I_ARTILLERIE_INITIALE = pionRenfort.I_ARTILLERIE;
                        pionRenfort.I_FATIGUE = I_FATIGUE;//même fatigue que l'unité d'origine
                        pionRenfort.I_MATERIEL = I_MATERIEL_ESCORTE;
                        pionRenfort.I_RAVITAILLEMENT = I_RAVITAILLEMENT; // ils sont ravitaillés comme l'unité qu'ils escortaient
                        pionRenfort.ID_CASE = this.ID_CASE;
                        Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);

                        //on supprimer le pion prisonniers d'origine
                        //on fait comme si le pion de prisonniers source était détruit sinon il va rester actif partout !
                        this.DetruirePion();

                        //on envoie le message après ,sinon le pion reste visible (non détruit)
                        if (!ClassMessager.EnvoyerMessage(this, ClassMessager.MESSAGES.MESSAGE_PRISONNIERS_ARRIVE_A_DESTINATION))
                        {
                            message = string.Format("ExecuterMouvementSansEffectif  : erreur lors de l'envoi d'un message pour indiquer qu'un convoi de prisonniers arrive à une prison");
                            LogFile.Notifier(message);
                            return false;
                        }

                        //il faut aussi envoyer un message pour prévenir de la présence de l'escorte
                        if (!ClassMessager.EnvoyerMessageImmediat(pionRenfort, ClassMessager.MESSAGES.MESSAGE_RENFORT))
                        {
                            message = string.Format("ExecuterMouvementSansEffectif  : erreur lors de l'envoi d'un message pour indiquer que l'escorte d'un convoi de prisonniers est disponible");
                            LogFile.Notifier(message);
                            return false;
                        }

                    }
                }
                else
                {
                    if (this.estBlesses)
                    {
                        //si le convoi termine son mouvement à coté d'une case d'hopital, les blessés vont à l'hopital pour être soignés
                        //on recherche s'il existe un hopital ami à proximité
                        Donnees.TAB_NOMS_CARTERow ligneHopital = this.RechercheHopital();
                        if (null == ligneHopital)
                        {
                            if (!ClassMessager.EnvoyerMessage(this, ClassMessager.MESSAGES.MESSAGE_ARRIVE_A_DESTINATION))
                            {
                                message = string.Format("ExecuterMouvementSansEffectif  : erreur lors de l'envoi d'un message pour indiquer qu'un convoi de blessés arrive à destination");
                                LogFile.Notifier(message);
                                return false;
                            }
                        }
                        else
                        {
                            Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                            ID_LIEU_RATTACHEMENT = ligneHopital.ID_NOM;
                            I_TOUR_BLESSURE = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                            Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                            this.DetruirePion();//on fait comme si le pion était détruit sinon il va rester actif partout !
                                                //on envoie le message après ,sinon le pion reste visible (non détruit)
                            if (!ClassMessager.EnvoyerMessage(this, ClassMessager.MESSAGES.MESSAGE_BLESSES_ARRIVE_A_DESTINATION))
                            {
                                message = string.Format("ExecuterMouvementSansEffectif  : erreur lors de l'envoi d'un message pour indiquer qu'un convoi de blessés arrive à un hopital");
                                LogFile.Notifier(message);
                                return false;
                            }
                        }
                    }
                }
                return true;
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
                    if (I_ARTILLERIE > 0 && vitesse == int.MaxValue)
                    {
                        //uniquement pour les unités d'artillerie pure
                        vitesse = Math.Min(vitesse, resModeleMouvement[0].I_VITESSE_ARTILLERIE);
                    }
                }
                //les unités en déroute (moral nul) bouge de 1 km/h plus  vite
                return (I_MORAL>0) ? vitesse : vitesse+1;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="lignePion">Pion recherchant l'espace</param>
            /// <param name="depart"></param>
            /// <param name="ligneCaseDepart">Case de départ</param>
            /// <param name="espace">taille de l'espace recherché</param>
            /// <param name="nombrePixelParCase"></param>
            /// <param name="tableCoutsMouvementsTerrain"></param>
            /// <param name="listeCaseEspace>liste des cases du parcours, ordonnées par I_COUT</param>
            /// <param name="erreur">message d'erreur</param>
            /// <returns>true si OK, false si KO</returns>
            internal bool RechercheEspace(bool depart, Donnees.TAB_CASERow ligneCaseDepart, int espace, int nombrePixelParCase, out AstarTerrain[] tableCoutsMouvementsTerrain, out List<int> listeIDCaseEspace, out string erreur)
            {
                string message;
                //char typeEspace;

                DateTime timeStart;
                TimeSpan perf;
                timeStart = DateTime.Now;
                tableCoutsMouvementsTerrain = null;
                //List <Donnees.TAB_CASERow> listeCaseEspace = null;
                listeIDCaseEspace = null;

                if (null == ligneCaseDepart)
                {
                    erreur = string.Format("RechercheEspace : ligneCaseDepart null");
                    LogFile.Notifier(erreur);
                    return false;
                }

                //typeEspace = (depart) ? AStar.CST_DEPART : AStar.CST_DESTINATION;
                //existe-il déjà un chemin pour le pion sur le trajet demandé ? BEA, vérifier, si cela se trouve c'est devenu aussi rapide de refaire le calcul à chaque fois
                //string requete = string.Format("ID_PION={0} AND C_TYPE='{1}'", ID_PION, typeEspace);
                /* Note : en traitement parallèle, il semblerait que locker/rechercher les données de TAB_ESPACE prennent plus de temps que de refaire le calcul
                Monitor.Enter(Donnees.m_donnees.TAB_ESPACE.Rows.SyncRoot); 
                Donnees.TAB_ESPACERow[] parcoursExistant = (Donnees.TAB_ESPACERow[])Donnees.m_donnees.TAB_ESPACE.Select(requete, "I_COUT");
                Monitor.Exit(Donnees.m_donnees.TAB_ESPACE.Rows.SyncRoot); 

                if (parcoursExistant.Length > 5000)
                {
                    erreur = string.Format("Alerte, l'unité {0}({1}) occupe {2} cases dans TAB_ESPACE", S_NOM, ID_PION, parcoursExistant.Length);
                    LogFile.Notifier(erreur, out messageErreur);
                }

                if ((null != parcoursExistant) &&
                    (0 < parcoursExistant.Length) &&
                    (espace <= parcoursExistant.Length)
                    && (ligneCaseDepart.ID_CASE == parcoursExistant[0].ID_CASE)
                    && parcoursExistant.Length < 5000)
                {
                    //il existe déjà un parcours mémorisé
                    listeIDCaseEspace = new int[parcoursExistant.Length];
                    for (int i = 0; i < parcoursExistant.Length; i++)
                    {
                        listeIDCaseEspace[i] = parcoursExistant[i].ID_CASE;
                    }
                    perf = DateTime.Now - timeStart;
                    message = string.Format("RechercheEspace : existant en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds);
                    LogFile.Notifier(message, out messageErreur);
                    return true;
                }
                //suppression de l'ancien espace
                if ((null != parcoursExistant) && (0 < parcoursExistant.Length))
                {
                    Monitor.Enter(Donnees.m_donnees.TAB_ESPACE.Rows.SyncRoot);
                    for (int i = 0; i < parcoursExistant.Length; i++)
                    {
                        Donnees.m_donnees.TAB_ESPACE.RemoveTAB_ESPACERow(parcoursExistant[i]);
                    }
                    Monitor.Exit(Donnees.m_donnees.TAB_ESPACE.Rows.SyncRoot);
                }
                */
                //calcul des couts, à renvoyer pour connaitre le cout pour avancer d'une case supplémentaire
                AStar etoile = new AStar();
                etoile.CalculModeleMouvementsPion(out tableCoutsMouvementsTerrain);

                //calcul du nouvel espace
                if (!etoile.SearchSpace(ligneCaseDepart, espace, tableCoutsMouvementsTerrain, nombrePixelParCase, nation.ID_NATION, out listeIDCaseEspace, out erreur))
                {
                    erreur = string.Format("{0}(ID={1}, erreur sur SearchPath dans RechercheEspace idDepart={2})",
                        S_NOM, ID_PION, ligneCaseDepart.ID_CASE);
                    LogFile.Notifier(erreur);
                    return false;
                }
                perf = DateTime.Now - timeStart;
                message = string.Format("RechercheEspace : nouveau en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds);
                /*
                int nbCaseEspace = listeCaseEspace.Count;
                listeIDCaseEspace = new int[nbCaseEspace];
                for (int i = 0; i < nbCaseEspace; i++)
                {
                    listeIDCaseEspace[i] = listeCaseEspace[i].ID_CASE;
                }*/
                //stockage de l'espace en table
                /*
                Monitor.Enter(Donnees.m_donnees.TAB_ESPACE.Rows.SyncRoot);
                for (int i = 0; i < nbCaseEspace; i++)
                {
                    Donnees.m_donnees.TAB_ESPACE.AddTAB_ESPACERow(ID_PION,
                                                                            typeEspace,
                                                                            listeCaseEspace[i].ID_CASE,
                                                                            listeCaseEspace[i].I_COUT);
                }
                Monitor.Exit(Donnees.m_donnees.TAB_ESPACE.Rows.SyncRoot);
                 * */
                //AcceptChanges only updates your rows in the (in memory) dataset, that is - marks them as "not needed for actual database update".
                //Donnees.m_donnees.TAB_ESPACE.AcceptChanges();

                perf = DateTime.Now - timeStart;
                message = string.Format("RechercheEspace : nouveau et stockage en {0} minutes, {1} secondes, {2} millisecondes", perf.Minutes, perf.Seconds, perf.Milliseconds);
                LogFile.Notifier(message);
                return true;
            }

            public bool RapportDePatrouille()
            {
                string message;
                ClassMessager.MESSAGES typeMessage;
                string phrase;

                if (estMessager)
                {
                    return true;//si la patrouille est déjà transformée en messager, elle n'envoit pas de deuxième rapport
                }

                #region création du message d'après ce qu'il y a autour de la patrouille
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ID_CASE);
                if (!ligneCase.EstInnocupe() && estEnnemi(ligneCase))
                {
                    //cas particulier, la patrouille trouve un ennemi
                    typeMessage = ClassMessager.MESSAGES.MESSAGE_PATROUILLE_CONTACT_ENNEMI;
                }
                else
                {
                    typeMessage = ClassMessager.MESSAGES.MESSAGE_PATROUILLE_RAPPORT;
                }
                phrase = ClassMessager.GenererPhrase(this,
                    typeMessage, //ClassMessager.MESSAGE_ARRIVE_A_DESTINATION
                    0, //iPertesInfanterie, 
                    0, //iPertesCavalerie, 
                    0,//iPertesArtillerie, 
                    0, //moralPerduOuGagne, 
                    0, //fatiguePerduOuGagne, 
                    null, //ligneBataille
                    null, //lignePionCible
                    null, //ligneCaseDestination
                    null, //ligneModeleDestination
                    -1, //distanceravitaillement
                    0, //ravitaillementGagneOuPerdu,
                    0, //materielGagneOuPerdu
                    string.Empty, //depotravitaillement
                    null//poins en bataille
                    );

                if (string.Empty == phrase)
                {
                    return false;
                }

                //ajout du message dans la table des message
                Donnees.TAB_MESSAGERow ligneMessage =
                    Donnees.m_donnees.TAB_MESSAGE.AjouterMessage(
                        ID_PION,
                        ID_PION,
                        (int)typeMessage,
                        0,
                        0,
                        Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,
                        Donnees.m_donnees.TAB_PARTIE[0].I_PHASE,
                        phrase,
                        0,
                        0,
                        0,
                        0,//I_FATIGUE
                        0,
                        0,
                        Constantes.CST_IDNULL,//ID_BATAILLE
                        Constantes.CST_IDNULL,
                        0,//I_TOUR_FUITE_RESTANT,
                        false,//B_DETRUIT,
                        ID_CASE,
                        ID_CASE,
                        0,//I_NB_PHASES_MARCHE_JOUR
                        0,//I_NB_PHASES_MARCHE_NUIT
                        0,//I_NB_HEURES_COMBAT
                        0,//I_MATERIEL,
                        0,//I_RAVITAILLEMENT,
                        0,//I_SOLDATS_RAVITAILLES,
                        0,//I_NB_HEURES_FORTIFICATION,
                        0,//I_NIVEAU_FORTIFICATION
                        0,//I_DUREE_HORS_COMBAT,
                        Constantes.CST_IDNULL,//I_TOUR_BLESSURE
                        'Z'//C_NIVEAU_DEPOT
                        );

                ligneMessage.SetI_ARTILLERIENull();
                ligneMessage.SetI_CAVALERIENull();
                ligneMessage.SetI_INFANTERIENull();
                ligneMessage.SetI_FATIGUENull();
                ligneMessage.SetI_MORALNull();
                ligneMessage.SetI_TOUR_SANS_RAVITAILLEMENTNull();
                ligneMessage.SetID_BATAILLENull();
                ligneMessage.SetI_ZONE_BATAILLENull();
                ligneMessage.SetI_TOUR_ARRIVEENull();
                ligneMessage.SetI_PHASE_ARRIVEENull();
                ligneMessage.SetI_NB_PHASES_MARCHE_JOURNull();
                ligneMessage.SetI_NB_PHASES_MARCHE_NUITNull();
                ligneMessage.SetI_NB_HEURES_COMBATNull();
                ligneMessage.SetI_TOUR_BLESSURENull();

                #endregion

                #region la patrouille revient à son QG, elle devient donc patrouille-messager
                //recherche du modèle patrouille-messager pour la nation concernée
                int idModelePATROUILLEMESSAGER = Donnees.m_donnees.TAB_MODELE_PION.RechercherModele(modelePion.ID_NATION, "PATROUILLEMESSAGER");
                if (idModelePATROUILLEMESSAGER >= 0)
                {
                    //on a trouvé le modèle, il faut modifier celui du pion associé
                    ID_MODELE_PION = idModelePATROUILLEMESSAGER;
                }
                else
                {
                    message = string.Format("RapportDePatrouille : impossible de trouver le modèle PATROUILLEMESSAGER ID_PION={0}, ID_MODELE={1} ID_NATION:{2}",
                        ID_PION, ID_MODELE_PION, modelePion.ID_NATION);
                    LogFile.Notifier(message);
                    return false;
                }
                //la destination de la patrouille, c'est dorénavant son leader
                Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(ID_PION);
                Donnees.TAB_PIONRow lignePionChef = this.proprietaire;
                ligneOrdre.I_ORDRE_TYPE = Constantes.ORDRES.MESSAGE;//ce n'est plus une patrouille mais un message
                ligneOrdre.ID_CASE_DEPART = ID_CASE;
                ligneOrdre.ID_CASE_DESTINATION = lignePionChef.ID_CASE;
                ligneOrdre.ID_DESTINATAIRE = lignePionChef.ID_PION;
                ligneOrdre.ID_MESSAGE = ligneMessage.ID_MESSAGE;
                ligneOrdre.SetID_NOM_DESTINATIONNull();
                #endregion
                return true;
            }

            internal TAB_PIONRow CreerConvoi(Donnees.TAB_PIONRow lignePionProprietaire, bool bBlesses, bool bPrisonniers, bool bRenfort)
            {
                Donnees.TAB_PIONRow lignePionConvoi = null;

                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot); 
                lignePionConvoi = Donnees.m_donnees.TAB_PION.AddTAB_PIONRow(
                    -1,//idModeleCONVOI,
                    lignePionProprietaire.ID_PION,
                    -1,
                    -1,
                    "",//nomConvoi,
                    0, 0, 0, 0, 0, 0, 0, 
                    10, //le moral doit être à 100, sinon il va ajouter +1km/h à la vitesse pour déroute -> réduit à 10 sinon pourrait potentiellement être un avantage en combat
                    10, //le moral doit être à 100, sinon il va ajouter +1km/h à la vitesse pour déroute
                    0, 0, 0, 'Z', 0, 0, 0, 0,
                    ID_CASE,
                    0, //I_TOUR_SANS_RAVITAILLEMENT
                    0, -1,
                    0,//I_TOUR_RETRAITE_RESTANT
                    0, false, false, false, false, false,
                    false,//B_ENNEMI_OBSERVABLE
                    0,//I_MATERIEL,
                    0,//I_RAVITAILLEMENT,
                    false,//B_CAVALERIE_DE_LIGNE,
                    false,//B_CAVALERIE_LOURDE,
                    false,//B_GARDE,
                    false,//B_VIEILLE_GARDE,
                    Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,//I_TOUR_CONVOI_CREE,
                    ID_PION,//ID_DEPOT_SOURCE
                    0,//I_SOLDATS_RAVITAILLES,
                    0,//I_NB_HEURES_FORTIFICATION,
                    0,//I_NIVEAU_FORTIFICATION,
                    -1,//ID_PION_REMPLACE,
                    0,//I_DUREE_HORS_COMBAT,
                    0,//I_TOUR_BLESSURE,
                    bBlesses,//B_BLESSES,
                    bPrisonniers,//B_PRISONNIERS,
                    bRenfort,//B_RENFORT
                    -1,//ID_LIEU_RATTACHEMENT,
                    'Z',//C_NIVEAU_DEPOT
                    -1,//ID_PION_ESCORTE, 
                    0,//I_INFANTERIE_ESCORTE, 
                    0,//I_CAVALERIE_ESCORTE
                    0,//I_MATERIEL_ESCORTE
                    0,//I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT
                    0,//I_VICTOIRE
                    -1,//I_TRI
                    string.Empty,
                    -1//I_TOUR_ENNEMI_OBSERVABLE
                    );
                lignePionConvoi.SetID_ANCIEN_PION_PROPRIETAIRENull();
                lignePionConvoi.SetID_NOUVEAU_PION_PROPRIETAIRENull();
                lignePionConvoi.SetI_ZONE_BATAILLENull();
                lignePionConvoi.SetID_BATAILLENull();
                lignePionConvoi.SetID_LIEU_RATTACHEMENTNull();
                lignePionConvoi.SetID_PION_ESCORTENull();
                lignePionConvoi.SetI_TRINull();
                lignePionConvoi.SetI_TOUR_ENNEMI_OBSERVABLENull();

                if (bBlesses)
                {
                    lignePionConvoi.S_NOM = "Blessés de la " + S_NOM;
                    lignePionConvoi.ID_MODELE_PION = ID_MODELE_PION;
                    lignePionConvoi.I_MATERIEL = I_MATERIEL;
                    lignePionConvoi.I_RAVITAILLEMENT = I_RAVITAILLEMENT;
                }
                else
                {
                    if (bPrisonniers)
                    {
                        lignePionConvoi.S_NOM = "Prisonniers de la " + S_NOM;
                        lignePionConvoi.ID_MODELE_PION = ID_MODELE_PION;
                    }
                    else
                    {
                        if (bRenfort)
                        {
                            lignePionConvoi.S_NOM = "Renforts pour le " + S_NOM;
                            lignePionConvoi.ID_MODELE_PION = ID_MODELE_PION;
                        }
                        else
                        {
                            //par défaut c'est un convoi de ravitaillement
                            lignePionConvoi.S_NOM = NouveauNomConvoiRavitaillement();
                            lignePionConvoi.ID_MODELE_PION = Donnees.m_donnees.TAB_MODELE_PION.RechercherModele(modelePion.ID_NATION, "CONVOI");
                        }
                    }
                }
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                /* finalement non, certains joueurs préférant envoyer leurs convois/blessés ailleurs que le lieu de création, il faut donc imposer au moins un mouvement avant de rejoindre un hopital ou une prison.
                if (bBlesses || bPrisonniers)
                {
                    //on en profite aussi pour voir si l'unité n'est pas sur un hopital ou une prison
                    lignePionConvoi.ArriveeSurPrisonOuHopital();
                }
                */
                return lignePionConvoi;
            }

            /// <summary>
            /// Renvoi un nom de convoi de ravitaillement unique et l'ajoute dans la table de référence
            /// </summary>
            /// <returns>Nom pour l'unité</returns>
            public string NouveauNomConvoiRavitaillement()
            {
                bool bExistant = true;
                string nom= string.Format("Convoi de ravitaillement du {0}", S_NOM);
                int iNumero=1;

                Monitor.Enter(Donnees.m_donnees.TAB_NOMS_PIONS.Rows.SyncRoot);
                while (bExistant)
                {
                    int i = 0;
                    while (i < m_donnees.TAB_NOMS_PIONS.Count && m_donnees.TAB_NOMS_PIONS[i].S_NOM != nom) i++;
                    if (i < m_donnees.TAB_NOMS_PIONS.Count)
                    {
                        iNumero++;
                        nom = string.Format("Convoi de ravitaillement n°{1} du {0}", S_NOM, iNumero);
                    }
                    else
                    {
                        bExistant = false;
                    }
                }
                m_donnees.TAB_NOMS_PIONS.AddTAB_NOMS_PIONSRow(nom);
                Monitor.Exit(Donnees.m_donnees.TAB_NOMS_PIONS.Rows.SyncRoot);
                return nom;
            }

            internal bool InterceptionMessage(Donnees.TAB_PIONRow lignePionEnnemi)
            {
                string message;
                if (!B_INTERCEPTION)
                {
                    //il n'y a encore jamais eut de tentative d'interception, cette tentative ne peut arriver qu'une seule fois.
                    if (Constantes.JetDeDes(1) == 1)
                    {
                        //une chance sur 6 que le message soit intercepté
                        message = string.Format("ExecuterMouvementSansEffectif  : interception 'un message porté par {0}({1}) par {2}({3})",
                            S_NOM, ID_PION,
                            lignePionEnnemi.S_NOM, lignePionEnnemi.ID_PION);
                        LogFile.Notifier(message);
                        if (!ClassMessager.EnvoyerMessage(lignePionEnnemi, this, ClassMessager.MESSAGES.MESSAGE_INTERCEPTION))
                        {
                            message = string.Format("InterceptionMessage  : erreur lors de l'envoi d'un message pour indiquer que l'on a capturé un message");
                            LogFile.Notifier(message);
                            return false;
                        }
                    }
                    Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    B_INTERCEPTION = true;
                    Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                }
                return true;
            }

            /// <summary>
            /// Tentative de prise d'occupation d'une case par une unité
            /// </summary>
            /// <param name="ligneCase">Case à occuper</param>
            /// <param name="enMouvement">true si la case à requisitionner est fait par des hommes en mouvement</param>
            /// <param name="nbplaces">nombres de places déjà occupées par le pion, incrémentée automatiquement</param>
            /// <returns>true si ok, false si ko</returns>
            public bool RequisitionCase(LigneCASE ligneCase, bool enMouvement, ref int nbplaces)
            {
                return RequisitionCase(m_donnees.TAB_CASE.FindParID_CASE(ligneCase.ID_CASE), enMouvement, ref nbplaces);
            }

            /// <summary>
            /// Tentative de prise d'occupation d'une case par une unité
            /// </summary>
            /// <param name="ligneCase">Case à occuper</param>
            /// <param name="enMouvement">true si la case à requisitionner est fait par des hommes en mouvement</param>
            /// <param name="nbplaces">nombres de places déjà occupées par le pion, incrémentée automatiquement</param>
            /// <returns>true si ok, false si ko</returns>
            public bool RequisitionCase(Donnees.TAB_CASERow ligneCase, bool enMouvement, ref int nbplaces)
            {
                if (!Donnees.m_donnees.TAB_PARTIE.Nocturne())
                {
                    //pour les autres, on vérifie si ce mouvement ne fait pas entrer l'unité dans une zone de bataille, uniquement de jour
                    Monitor.Enter(Donnees.m_donnees.TAB_BATAILLE.Rows.SyncRoot);
                    foreach (Donnees.TAB_BATAILLERow ligneBataille in Donnees.m_donnees.TAB_BATAILLE)
                    {
                        if (!ligneBataille.IsI_TOUR_FINNull()) { continue; } // la bataille est terminée
                        if (ligneCase.I_X <= ligneBataille.I_X_CASE_BAS_DROITE && ligneCase.I_Y <= ligneBataille.I_Y_CASE_BAS_DROITE &&
                            ligneCase.I_X >= ligneBataille.I_X_CASE_HAUT_GAUCHE && ligneCase.I_Y >= ligneBataille.I_Y_CASE_HAUT_GAUCHE)
                        {
                            ligneBataille.AjouterPionDansLaBataille(this, ligneCase);
                        }
                    }
                    Monitor.Exit(Donnees.m_donnees.TAB_BATAILLE.Rows.SyncRoot);
                }
                if (!ligneCase.EstOccupeeOuBloqueParEnnemi(this, enMouvement)/* || lignePion.estMessager || lignePion.estPatrouille || lignePion.estQG || lignePion.estDepot*/)
                {
                    //-> Maintenant c'est l'inverse, le calcul du cout de case étant fait avant la requisition
                    //if (!enMouvement || ligneCase.IsID_NOUVEAU_PROPRIETAIRENull())
                    //{
                    //    //si on ne fait pas ce test, la dernière unité en mvt prend toujours la case
                    //    //et une route n'est donc jamais "surchargée"
                    //    ligneCase.ID_NOUVEAU_PROPRIETAIRE = lignePion.ID_PION;
                    //}
                    if (enMouvement || ligneCase.IsID_NOUVEAU_PROPRIETAIRENull())
                    {
                        //Une unité en mouvement étant la seule à surchargée une route, son occupation est prioritaire par rapport à une unité fixe
                        Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                        ligneCase.ID_NOUVEAU_PROPRIETAIRE = ID_PION;
                        
                        if (null == Donnees.m_donnees.TAB_MAJ_PROPRIO.FindByID_CASE(ligneCase.ID_CASE))
                        {
                            Donnees.m_donnees.TAB_MAJ_PROPRIO.AddTAB_MAJ_PROPRIORow(ligneCase.ID_CASE);
                            //string requete1 = string.Format("ID_NOUVEAU_PROPRIETAIRE<>{0}", Constantes.NULLENTIER);
                            //Donnees.TAB_CASERow[] changeRows1 = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete1);
                            //string requete2 = string.Format("ID_PROPRIETAIRE<>{0}", Constantes.NULLENTIER);
                            //Donnees.TAB_CASERow[] changeRows2 = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete2);
                            //string requete3 = string.Format("ID_PROPRIETAIRE<>ID_NOUVEAU_PROPRIETAIRE", Constantes.NULLENTIER);
                            //Donnees.TAB_CASERow[] changeRows3 = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete3);
                            //LogFile.Notifier(string.Format("requisition cases {0},{1},{2}", changeRows1.Count(), changeRows2.Count(), m_listeNouveauProprietaire.Count));
                        };
                        Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                    }

                    nbplaces++;
                }
                else
                {
                    if (estEnnemi(ligneCase))
                    {
                        Donnees.TAB_PIONRow lignePionEnnemi = rechercheEnnemi(ligneCase);
                        return Rencontre(lignePionEnnemi, ligneCase);
                    }
                }
                return true;
            }

            /// <summary>
            /// Rencontre entre deux unités ennemis.
            /// Note : pour l'instant les QG sont immortels
            /// </summary>
            /// <param name="lignePion">Premier pion</param>
            /// <param name="lignePionEnnemi">Second pion</param>
            /// <param name="ligneCase">Case sur laquelle a lieu la rencontre</param>
            /// <returns>true si ok, false si ko</returns>
            internal bool Rencontre(Donnees.TAB_PIONRow lignePionEnnemi, Donnees.TAB_CASERow ligneCase)
            {
                string message;

                //if (lignePion.effectifTotal > 0 && 0 == lignePion.I_TOUR_FUITE_RESTANT && lignePionEnnemi.estCombattifQG(false, true))
                //si l'un des deux pions est en retraite ou en fuite, il n'y a aucun effet possible
                if (I_TOUR_FUITE_RESTANT > 0 || I_TOUR_RETRAITE_RESTANT > 0 || lignePionEnnemi.I_TOUR_FUITE_RESTANT > 0 || lignePionEnnemi.I_TOUR_RETRAITE_RESTANT > 0)
                {
                    return true;
                }

                #region Une patrouille, elle indique la présence de l'ennemi et se transforme en messager.
                //un poin patrouille ET message est une patrouille sur le retour, elle n'envoit pas deux rapports !
                if (estPatrouille && !estMessager && (lignePionEnnemi.estCombattif || lignePionEnnemi.estPatrouille))
                {
                    //on prévient l'unité du contact de la patrouille si elle est repérée
                     if (Constantes.JetDeDes(2)>=6)
                    {
                        if (!ClassMessager.EnvoyerMessage(lignePionEnnemi, ClassMessager.MESSAGES.MESSAGE_CONTACT_PATROUILLE_ENNEMIE))
                        {
                            LogFile.Notifier("Rencontre : erreur I lors de l'envoi d'un message MESSAGE_CONTACT_PATROUILLE_ENNEMIE");
                            return false;
                        }
                    }
                    return RapportDePatrouille();//si on continue, le message de la patrouille peut être directement intercepté
                }
                if (lignePionEnnemi.estPatrouille && !lignePionEnnemi.estMessager && (estCombattif || estPatrouille))
                {
                    //on prévient l'unité du contact de la patrouille si elle est repérée
                    if (Constantes.JetDeDes(2) >= 6)
                    {
                        if (!ClassMessager.EnvoyerMessage(this, ClassMessager.MESSAGES.MESSAGE_CONTACT_PATROUILLE_ENNEMIE))
                        {
                            LogFile.Notifier("Rencontre : erreur II lors de l'envoi d'un message MESSAGE_CONTACT_PATROUILLE_ENNEMIE");
                            return false;
                        }
                    }
                    return lignePionEnnemi.RapportDePatrouille();//si on continue, le message de la patrouille peut être directement intercepté
                }
                #endregion

                #region un messager rencontre une unité ennemie, il a une chance d'être intercepté et/ou que son message soit donnée à l'ennemi mais le message continue quand même sa route (voir règle p10, les messagers arrivaient toujours)
                if (estMessager && lignePionEnnemi.estCombattif)
                {
                    if (!InterceptionMessage(lignePionEnnemi)) return false;
                }
                if (lignePionEnnemi.estMessager && estCombattif)
                {
                    if (!lignePionEnnemi.InterceptionMessage(this)) return false;
                }
                #endregion

                if (!Donnees.m_donnees.TAB_PARTIE.Nocturne())
                {
                    //la capture n'est possible que si aucun pion ami combattif n'est visible
                    bool bEnDanger;
                    #region un depot rencontre une unité ennemie, une capture ne peut avoir lieu que de jour, ceci afin d'éviter des captures par des mouvements ne déclenchant pas de combat
                    if ((estDepot || estArtillerie) && lignePionEnnemi.estCombattif)
                    {
                        ClassMessager.PionsEnvironnants(this, ClassMessager.MESSAGES.MESSAGE_AUCUN_MESSAGE, ligneCase, false, out _, out bEnDanger);
                        if (bEnDanger)
                        {
                            if (estDepot)
                            {
                                if (!this.CaptureDepot(lignePionEnnemi, ligneCase)) return false;
                            }
                            if (estArtillerie)
                            {
                                if (!this.CapturePion(lignePionEnnemi, this.proprietaire.ID_PION, string.Empty, lignePionEnnemi.nation.ID_NATION, ligneCase)) return false;
                            }                            
                        }
                    }
                    if ((lignePionEnnemi.estDepot || lignePionEnnemi.estArtillerie) && estCombattif)
                    {
                        ClassMessager.PionsEnvironnants(lignePionEnnemi, ClassMessager.MESSAGES.MESSAGE_AUCUN_MESSAGE, ligneCase, false, out _, out bEnDanger);
                        if (bEnDanger)
                        {
                            if (lignePionEnnemi.estDepot)
                            {
                                if (!lignePionEnnemi.CaptureDepot(this, ligneCase)) return false;
                            }
                            if (lignePionEnnemi.estArtillerie)
                            {
                                if (!lignePionEnnemi.CapturePion(this, this.proprietaire.ID_PION, string.Empty, this.nation.ID_NATION, ligneCase)) return false;
                            }
                        }
                    }
                    #endregion

                    #region un convoi rencontre une unité ennemie, une capture ne peut avoir lieu que de jour, ceci afin d'éviter des captures par des mouvements ne déclenchant pas de combat
                    //aucune des deux unités ne doit être en fuite ou en retraite (pris en compte est l'appel à estCombattif)
                    if ((estConvoi || estBlesses || estPrisonniers) && lignePionEnnemi.estCombattif)
                    {
                        ClassMessager.PionsEnvironnants(this, ClassMessager.MESSAGES.MESSAGE_AUCUN_MESSAGE, ligneCase, false, out _, out bEnDanger);
                        if (bEnDanger)
                        {
                            if (!CaptureConvoiBlessesPrisonniers(lignePionEnnemi, ligneCase)) return false;
                        }
                    }
                    if ((lignePionEnnemi.estConvoi || lignePionEnnemi.estBlesses || lignePionEnnemi.estPrisonniers) && estCombattif)
                    {
                        ClassMessager.PionsEnvironnants(lignePionEnnemi, ClassMessager.MESSAGES.MESSAGE_AUCUN_MESSAGE, ligneCase, false, out _, out bEnDanger);
                        if (bEnDanger)
                        {
                            if (!lignePionEnnemi.CaptureConvoiBlessesPrisonniers(this, ligneCase)) return false;
                        }
                    }

                    if (estPontonnier && lignePionEnnemi.estCombattif)
                    {
                        ClassMessager.PionsEnvironnants(this, ClassMessager.MESSAGES.MESSAGE_AUCUN_MESSAGE, ligneCase, false, out _, out bEnDanger);
                        if (bEnDanger)
                        {
                            if (!CapturePion(lignePionEnnemi, lignePionEnnemi.ID_PION_PROPRIETAIRE, "PONTONNIER", lignePionEnnemi.nation.ID_NATION, ligneCase)) return false;
                        }
                    }
                    if (lignePionEnnemi.estPontonnier && estCombattif)
                    {
                        ClassMessager.PionsEnvironnants(lignePionEnnemi, ClassMessager.MESSAGES.MESSAGE_AUCUN_MESSAGE, ligneCase, false, out _, out bEnDanger);
                        if (bEnDanger)
                        {
                            if (!lignePionEnnemi.CapturePion(this, ID_PION_PROPRIETAIRE, "PONTONNIER", nation.ID_NATION, ligneCase)) return false;
                        }
                    }
                    #endregion

                    //Bataille ? Une unité d'artillerie ne peut jamais, a elle seule, déclenchée une bataille
                    //6/4/2019, test remis en fin, sinon, pour toute les unités qui ne sont pas des artilleries, on passe dans ce test et on ne fait pas les tests de capture
                    if (!estArtillerie && !lignePionEnnemi.estArtillerie)
                    {
                        //si l'un des deux unités est déjà engagée en bataille, elle ne peut pas créer une nouvelle bataille
                        if (!estAuCombat && !lignePionEnnemi.estAuCombat &&
                            ((estCombattifQG(false, false) && lignePionEnnemi.estCombattifQG(false, true))
                            || (estCombattifQG(false, true) && lignePionEnnemi.estCombattifQG(false, false))))
                        {
                            //unites combattantes standard, création d'une bataille
                            int IdProprietaire = ligneCase.ID_PROPRIETAIRE;
                            string proprio = (Constantes.NULLENTIER == IdProprietaire) ? "null" : IdProprietaire.ToString();
                            int IdNouveauProprietaire = ligneCase.ID_NOUVEAU_PROPRIETAIRE;
                            string nouveauProprio = (Constantes.NULLENTIER == IdNouveauProprietaire) ? "null" : IdNouveauProprietaire.ToString();

                            message = string.Format("RequisitionCase-NouvelleBataille: entre les poins ID_PION={0} et ID_PION={1} ou ID_PION={2} sur ID_CASE:{3}",
                                ID_PION, proprio, nouveauProprio, ligneCase.ID_CASE);
                            LogFile.Notifier(message);
                            return Cartographie.NouvelleBataille(ligneCase, this);
                        }
                    }

                }
                return true;
            }

            internal bool CapturePion(Donnees.TAB_PIONRow lignePionEnnemi, int idNouveauPionProprietaire, string aptitude, int idNationCaptureur, Donnees.TAB_CASERow ligneCaseContact)
            {
                string message;

                message = string.Format("CapturePion  : de {0}:{1} par {2} avec l'aptitude {3}",
                    this.ID_PION, this.S_NOM, idNouveauPionProprietaire, aptitude);
                LogFile.Notifier(message);
                //on envoit les messages pour indiquer la capture
                if (!ClassMessager.EnvoyerMessage(this, lignePionEnnemi, ClassMessager.MESSAGES.MESSAGE_EST_CAPTURE))
                {
                    message = string.Format("CapturePion  : erreur lors de l'envoi d'un message pour indiquer que l'on a capturé un pion");
                    LogFile.Notifier(message);
                    return false;
                }

                //le pion change de proprietaire
                if (!TransfertPion(idNouveauPionProprietaire))
                {
                    message = string.Format("CapturePion  : erreur lors du transfert de pion");
                    LogFile.Notifier(message);
                    return false;
                }

                if (!ClassMessager.EnvoyerMessage(lignePionEnnemi, this, ClassMessager.MESSAGES.MESSAGE_A_CAPTURE))
                {
                    message = string.Format("CapturePion  : erreur lors de l'envoi d'un message pour indiquer que le pion est capturé");
                    LogFile.Notifier(message);
                    return false;
                }

                #region le pion doit changer de modele de pion, on recherche le modele de pion correspondant de la nouvelle nation
                int idModelePionCaptureur = -1;
                if (string.Empty == aptitude)
                {
                    //Dans ce cas (prisonniers ou blessés ou artillerie) ils ont le même type que l'unité qui fait la capture
                    idModelePionCaptureur = lignePionEnnemi.ID_MODELE_PION;
                }
                else
                {
                    //recherche l'aptitude fournie en paramètre
                    string requete = "S_NOM='" + aptitude + "'";
                    Donnees.TAB_APTITUDESRow[] resAptitude = (Donnees.TAB_APTITUDESRow[])Donnees.m_donnees.TAB_APTITUDES.Select(requete);

                    requete = string.Format("ID_APTITUDE={0}", resAptitude[0].ID_APTITUDE);
                    Donnees.TAB_APTITUDES_PIONRow[] resAptitudesPion = (Donnees.TAB_APTITUDES_PIONRow[])Donnees.m_donnees.TAB_APTITUDES_PION.Select(requete);
                    foreach (Donnees.TAB_APTITUDES_PIONRow aptitudePion in resAptitudesPion)
                    {
                        Donnees.TAB_MODELE_PIONRow ligneModele = Donnees.m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION(aptitudePion.ID_MODELE_PION);

                        if (ligneModele.ID_NATION == idNationCaptureur)
                        {
                            idModelePionCaptureur = ligneModele.ID_MODELE_PION;
                            break;
                        }
                    }
                }
                if (idModelePionCaptureur == -1)
                {
                    message = string.Format("CapturePion: erreur grave, impossible de trouver le nouveau modele de pion capturé {0}({1}) par la nation {2}",
                        S_NOM, ID_PION, idNationCaptureur);
                    LogFile.Notifier(message);
                    return false;
                }
                //le pion change de nationalité.
                message = string.Format("CapturePion : nouveau modèle {0}",idModelePionCaptureur);
                LogFile.Notifier(message);
                ID_MODELE_PION = idModelePionCaptureur;
                //si l'unité faisait quelque chose, il faut qu'elle arrête
                Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(ID_PION);
                if (null != ligneOrdre)
                {
                    //s'il y a un arrêt alors que l'unité est en mouvement il faut qu'elle fasse bivouac sur le point de contact
                    //comme pour une bataille
                    ID_CASE = ligneCaseContact.ID_CASE;
                    SupprimerTousLesOrdres();
                    PlacerStatique();
                }
                else
                {
                    SupprimerTousLesOrdres();
                }
                //l'unité ne peut pas être recapturée durant 2 heures pour éviter un effet, je te capture, je te recapture, etc...
                I_TOUR_RETRAITE_RESTANT = 2; // même chose que I_TOUR_FUITE_RESTANT mais tu peux donner des ordres
                #endregion

                return true;
            }

            public void ReceptionMessageTransfert(Donnees.TAB_MESSAGERow ligneMessage)
            {
                //cas particulier des transferts d'unités ou celui là ne devient effectif qu'à la reception de l'information
                if (ligneMessage.I_TYPE == (int)ClassMessager.MESSAGES.MESSAGE_A_PERDU_TRANSFERT)
                {
                    //Tant qu'id ancien proprietaire est renseigné, l'ancien propriétaire doit continuer à voir l'unité dans son bilan
                    //la valeur est remise à vide quand l'ancien proprietaire reçoit le message/ordre du transfert
                    Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    SetID_ANCIEN_PION_PROPRIETAIRENull();
                    Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                }
                if (ligneMessage.I_TYPE == (int)ClassMessager.MESSAGES.MESSAGE_A_RECU_TRANSFERT)
                {
                    //Tant qu'id nouveau proprietaire est renseigné, le nouveau propriétaire ne doit pas voir l'unité dans son bilan
                    //la valeur est remise à vide quand l'ancien proprietaire reçoit le message/ordre du transfert
                    Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    SetID_NOUVEAU_PION_PROPRIETAIRENull();
                    Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                }
            }

            /// <summary>
            /// Changement de propriétaire pour le pion
            /// </summary>
            /// <param name="lignePion">Pion dont on change le proprietaire</param>
            /// <param name="idNouveauPionProprietaire">identifiant du nouveau proprietaire</param>
            /// <param name="transfertInvolontaire">false si l'unité est transféré volontairement par son chef, true sinon (cas d'une capture) 
            /// --------> Cela n'a pas de sens, même pour un transfert volontaire, comme le transfert n'est pas à l'initiative du chef du pion, il faut le prévenir</param>
            /// <returns>rrue si OK, false si KO</returns>
            internal bool TransfertPion(int idPionNouveauProprietaire)
            {
                string message;

                //Tant qu'id ancien proprietaire est renseigné, l'ancien propriétaire doit continuer à voir l'unité dans son bilan
                //la valeur est remise à vide quand l'ancien proprietaire reçoit le message/ordre du transfert
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                ID_ANCIEN_PION_PROPRIETAIRE = ID_PION_PROPRIETAIRE;
                //Tant qu'id nouveau proprietaire est renseigné, le nouveau propriétaire ne doit pas voir l'unité dans son bilan
                //la valeur est remise à vide quand l'ancien proprietaire reçoit le message/ordre du transfert
                ID_NOUVEAU_PION_PROPRIETAIRE = idPionNouveauProprietaire;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);

                //prévenir l'ancier proprietaire du transfert
                if (!ClassMessager.EnvoyerMessage(this, ClassMessager.MESSAGES.MESSAGE_A_PERDU_TRANSFERT))
                {
                    message = string.Format("TransfertPion  : erreur lors de l'envoi d'un message pour indiquer que l'unité {0}:{1} perd l'unité transférée", ID_PION, S_NOM);
                    LogFile.Notifier(message);
                    return false;
                }

                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                ID_PION_PROPRIETAIRE = idPionNouveauProprietaire;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);

                if (!ClassMessager.EnvoyerMessage(this, ClassMessager.MESSAGES.MESSAGE_A_RECU_TRANSFERT))
                {
                    message = string.Format("TransfertPion  : erreur lors de l'envoi d'un message pour indiquer que l'unité {0}:{1} gagne l'unité transférée", ID_PION, S_NOM);
                    LogFile.Notifier(message);
                    return false;
                }
                return true;
            }

            /// <summary>
            /// Rencontre d'un convoi avec une unité ennemie, les convois sont de quatre types
            /// Ravitaillement : ils sont alors capturés comme un dépôt
            /// Renforts : ils deviennent des prisonniers  -> pas vu dans tab_pion comme on reperait un renfort, doivent-ils se battre ?
            /// Prisonniers : ils deviennent des renforts
            /// Blessés : ils deviennent des prisonniers
            /// </summary>
            /// <param name="lignePion">pion du convoi</param>
            /// <param name="lignePionEnnemi">^pion capturant le convoi</param>
            /// <returns>true si ok, false si ko</returns>
            internal bool CaptureConvoiBlessesPrisonniers(Donnees.TAB_PIONRow lignePionEnnemi, Donnees.TAB_CASERow ligneCaseCapture)
            {
                bool bEnDanger;
                string message;

                //La capture ne peut avoir lieu que si aucune unité amie visible n'est à proximité
                ClassMessager.PionsEnvironnants(this, ClassMessager.MESSAGES.MESSAGE_AUCUN_MESSAGE, ligneCaseCapture, false, out message, out bEnDanger);
                if (!bEnDanger) { return true; }

                //if (lignePion.estDepot) -> pas un bon test, c'est un convoi à ce moment là, pas un dépôt
                if (estConvoiDeRavitaillement)
                {
                    // Un dépôt capturé est réduit d'un niveau (règle avancé) et capturé
                    // s'il était au dernier niveau possible, le dépôt est détruit.
                    if (C_NIVEAU_DEPOT == 'D')
                    {
                        return DestructionAuContact(lignePionEnnemi);
                    }
                    Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    C_NIVEAU_DEPOT++;// 'A' c'est le meilleur, 'D' le pire
                    Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    return CapturePion(lignePionEnnemi, lignePionEnnemi.ID_PION_PROPRIETAIRE, "CONVOI", lignePionEnnemi.nation.ID_NATION, ligneCaseCapture);
                }
                if (estBlesses)
                {
                    //Le convoi devient un convoi de prisonniers
                    Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    B_BLESSES = false;
                    B_PRISONNIERS = true;
                    S_NOM = "Prisonniers de " + S_NOM;
                    Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    CreerEscorte(lignePionEnnemi, this);
                }
                else
                {
                    if (estPrisonniers)
                    {
                        //L'escorte devient un convoi de prisonniers !
                        if (null == CreerConvoiDePrisonniers(lignePionEnnemi))
                        {
                            LogFile.Notifier("Erreur rencontrée dans CreerConvoiDePrisonniers");
                            return false;
                        }

                        //Le convoi devient un convoi de renfort, il n'y a plus d'escorte
                        Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        B_PRISONNIERS = false;
                        B_RENFORT = true;
                        I_INFANTERIE_ESCORTE = 0;
                        I_CAVALERIE_ESCORTE = 0;
                        I_MATERIEL_ESCORTE = 0;
                        SetID_PION_ESCORTENull();
                        S_NOM = "Renfort de " + S_NOM;
                        Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    }
                }
                //le pion change maintenant de proprietaire, celui-ci est affecté au proprietaire du pion de capture
                //pas forcément très logique pour un convoi de ravitaillement mais pas logique non plus si le leader de niveau A est très très loin de l'unité
                //à chaque pour lui de faire un transfert s'il ne veut pas laisser un dépôt à la charge de l'un de ses subordonnés
                return CapturePion(lignePionEnnemi, lignePionEnnemi.ID_PION_PROPRIETAIRE, string.Empty, lignePionEnnemi.nation.ID_NATION, ligneCaseCapture);
            }

            /// <summary>
            /// Le pion est détruit par le contact d'une unité ennemie, cas des depôts 'D' par exemple
            /// </summary>
            /// <param name="lignePionEnnemi"></param>
            /// <returns></returns>
            internal bool DestructionAuContact(Donnees.TAB_PIONRow lignePionEnnemi)
            {
                string message;
                //Destruction du pion par l'ennemi
                //on envoie des messages aux uns et aux autres
                DetruirePion();
                if (!ClassMessager.EnvoyerMessage(lignePionEnnemi, this, ClassMessager.MESSAGES.MESSAGE_EST_DETRUIT_AU_CONTACT))
                {
                    message = string.Format("DestructionAuContact  : erreur lors de l'envoi d'un message pour indiquer que l'on a detruit un pion");
                    LogFile.Notifier(message);
                    return false;
                }
                if (!ClassMessager.EnvoyerMessage(this, lignePionEnnemi, ClassMessager.MESSAGES.MESSAGE_A_DETRUIT_AU_CONTACT))
                {
                    message = string.Format("DestructionAuContact  : erreur lors de l'envoi d'un message pour indiquer que le pion est détruit");
                    LogFile.Notifier(message);
                    return false;
                }
                return true;
            }

            /// <summary>
            /// Un dépôt est capturé par contact avec l'ennemi
            /// </summary>
            /// <param name="lignePionDepot">depôt capturé</param>
            /// <param name="lignePionEnnemi">pion capturant le dépot</param>
            /// <returns>true si ok, false si ko</returns>
            internal bool CaptureDepot(Donnees.TAB_PIONRow lignePionEnnemi, Donnees.TAB_CASERow ligneCaseCapture)
            {
                int idNouveauPionProprietaire;
                int idNationCaptureur;
                bool bEnDanger;

                //La capture ne peut avoir lieu que si aucune unité amie visible n'est à proximité
                ClassMessager.PionsEnvironnants(this, ClassMessager.MESSAGES.MESSAGE_AUCUN_MESSAGE, ligneCaseCapture, false, out _, out bEnDanger);
                if (!bEnDanger) { return true; }

                // Un dépôt capturé est réduit d'un niveau (règle avancé) et capturé
                // s'il était au dernier niveau possible, le dépôt est détruit.
                if (C_NIVEAU_DEPOT == 'D')
                {
                    return DestructionAuContact(lignePionEnnemi);
                }
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                C_NIVEAU_DEPOT++;// 'A' c'est le meilleur, 'D' le pire
                idNationCaptureur = lignePionEnnemi.idNation;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);

                //Tout dépôt capturé est attribué au leader de niveau A de l'unité effectuant la capture
                /* -> plus maintenant, n'importe quel chef peut diriger un dépôt
                Donnees.TAB_PIONRow lignePionEnnemiQG = null;
                Donnees.TAB_RENFORTRow lignePionEnnemiQGRenfort = null;
                string requete;

                requete = "C_NIVEAU_HIERARCHIQUE = 'A'";
                Donnees.TAB_PIONRow[] lignesPion = (Donnees.TAB_PIONRow[])Donnees.m_donnees.TAB_PION.Select(requete);
                Verrou.Derrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);

                //on garde le premier QG de la bonne nationalité
                foreach (Donnees.TAB_PIONRow lignePion in lignesPion)
                {
                    if (!lignePion.B_DETRUIT && lignePion.estQG && (idNationCaptureur == lignePion.idNation))
                    {
                        lignePionEnnemiQG = lignePion;
                    }
                }
                if (null == lignePionEnnemiQG)
                {
                    //le responsable peut arriver plus tardivement en renfort
                    Donnees.TAB_RENFORTRow[] lignesRenfort = (Donnees.TAB_RENFORTRow[])Donnees.m_donnees.TAB_RENFORT.Select(requete);
                    //on garde le premier QG de la bonne nationalité
                    foreach (Donnees.TAB_RENFORTRow ligneRenfort in lignesRenfort)
                    {
                        if (ligneRenfort.estQG && (idNationCaptureur == ligneRenfort.idNation))
                        {
                            lignePionEnnemiQGRenfort = ligneRenfort;
                        }
                    }
                    if (null == lignePionEnnemiQGRenfort)
                    {
                        //ne doit jamais arrivé !
                        message = string.Format("CaptureDepot: erreur grave, impossible de trouver un leader de rang hierarchique A pour l'unité {0}({1})",
                            lignePionEnnemi.S_NOM, lignePionEnnemi.ID_PION);
                        LogFile.Notifier(message);
                        return false;
                    }
                    idNouveauPionProprietaire = lignePionEnnemiQGRenfort.ID_PION;
                }
                else
                {
                    idNouveauPionProprietaire = lignePionEnnemiQG.ID_PION;
                }
                */
                idNouveauPionProprietaire = lignePionEnnemi.proprietaire.ID_PION;

                return CapturePion(lignePionEnnemi, idNouveauPionProprietaire, "DEPOT", idNationCaptureur, ligneCaseCapture);
            }

            internal Donnees.TAB_PIONRow CreerConvoiDePrisonniers(Donnees.TAB_PIONRow lignePionQuiCapture)
            {
                return CreerConvoiDePrisonniers(lignePionQuiCapture, -1, -1, -1);
            }

            internal Donnees.TAB_PIONRow CreerConvoiDePrisonniers(Donnees.TAB_PIONRow lignePionQuiCapture, int infanteriePrisonnier, int cavaleriePrisonnier, int artilleriePrisonnier)
            {
                Donnees.TAB_PIONRow lignePionProprietaire = lignePionQuiCapture.proprietaire;
                Donnees.TAB_PIONRow lignePionPrisonniers = lignePionQuiCapture.CreerConvoi(lignePionProprietaire, false /*bBlesses */ , true /* bPrisonniers */ , false /* bRenfort*/);
                //lignePionPrisonniers.S_NOM = "Prisonniers de " + lignePionQuiCapture.S_NOM;
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                if (estPrisonniers)
                {
                    //l'unité capturée est elle-même un convoi de prisonniers
                    //il faut retrouver l'unité source de l'escorte, pour affecter les caractéristiques des prisonniers
                    Donnees.TAB_PIONRow lignePionEscorte = escorte;
                    lignePionPrisonniers.I_INFANTERIE = I_INFANTERIE_ESCORTE;
                    lignePionPrisonniers.I_CAVALERIE = I_CAVALERIE_ESCORTE;
                    lignePionPrisonniers.I_ARTILLERIE = 0;
                    lignePionPrisonniers.I_EXPERIENCE = lignePionEscorte.I_EXPERIENCE;
                    lignePionPrisonniers.I_TACTIQUE = lignePionEscorte.I_TACTIQUE;
                }
                else
                {
                    lignePionPrisonniers.I_INFANTERIE = (infanteriePrisonnier < 0) ? 0 : infanteriePrisonnier;
                    lignePionPrisonniers.I_CAVALERIE = (cavaleriePrisonnier < 0) ? 0 : cavaleriePrisonnier;
                    lignePionPrisonniers.I_ARTILLERIE = (artilleriePrisonnier < 0) ? 0 : artilleriePrisonnier;
                    lignePionPrisonniers.I_EXPERIENCE = I_EXPERIENCE;
                    lignePionPrisonniers.I_TACTIQUE = I_TACTIQUE;
                }
                lignePionPrisonniers.I_INFANTERIE_INITIALE = lignePionPrisonniers.I_INFANTERIE;
                lignePionPrisonniers.I_CAVALERIE_INITIALE = lignePionPrisonniers.I_CAVALERIE;
                lignePionPrisonniers.I_ARTILLERIE_INITIALE = lignePionPrisonniers.I_ARTILLERIE;
                lignePionPrisonniers.I_FATIGUE = I_FATIGUE;//même fatigue que l'unité d'origine
                lignePionPrisonniers.I_MATERIEL = 0; // on ne laisse pas de matériel aux prisonniers
                lignePionPrisonniers.I_RAVITAILLEMENT = lignePionQuiCapture.I_RAVITAILLEMENT; // ils sont ravitaillés comme l'unité qui les capture
                lignePionPrisonniers.ID_PION_PROPRIETAIRE = lignePionProprietaire.ID_PION;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                CreerEscorte(lignePionQuiCapture, lignePionPrisonniers);
                //note : le message de la capture est fait dans "capturePion" en amont

                //on en profite aussi pour voir si l'unité n'est pas sur un hopital ou une prison
                lignePionPrisonniers.ArriveeSurPrisonOuHopital();
                return lignePionPrisonniers;
            }

            private static void CreerEscorte(TAB_PIONRow lignePionQuiCapture, TAB_PIONRow lignePionPrisonniers)
            {
                lignePionPrisonniers.ID_PION_ESCORTE = lignePionQuiCapture.ID_PION;

                // creation de l'escorte du convoi de prisonniers, c'est à dire réduction des effectifs correspondants de l'unité qui capture
                // on prélève en priorité des fantassins pour escorter des prisonniers, et, en deuxième choix des cavaliers
                int nbEscorte = (lignePionPrisonniers.I_INFANTERIE + lignePionPrisonniers.I_CAVALERIE) / 10;
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                lignePionPrisonniers.I_INFANTERIE_ESCORTE = (lignePionQuiCapture.I_INFANTERIE > nbEscorte) ? nbEscorte : lignePionQuiCapture.I_INFANTERIE;
                nbEscorte -= lignePionPrisonniers.I_INFANTERIE_ESCORTE;
                lignePionPrisonniers.I_CAVALERIE_ESCORTE = (lignePionQuiCapture.I_CAVALERIE > nbEscorte) ? nbEscorte : lignePionQuiCapture.I_CAVALERIE;
                lignePionPrisonniers.I_MATERIEL_ESCORTE = lignePionQuiCapture.I_MATERIEL;
                lignePionQuiCapture.I_INFANTERIE -= lignePionPrisonniers.I_INFANTERIE_ESCORTE;
                lignePionQuiCapture.I_CAVALERIE -= lignePionPrisonniers.I_CAVALERIE_ESCORTE;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                if (lignePionQuiCapture.effectifTotal <= 0)
                {
                    //tout le pion d'origine passe en escorte, cela ne devrait pas arrivée souvent j'espère !
                    lignePionQuiCapture.DetruirePion();
                }
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

            /// <summary>
            /// Placement d'un pion sur la carte qui ne soit ne bouge pas soit est complètement arrivé
            /// </summary>
            /// <param name="lignePion">Pion à placer</param>
            /// <param name="ligneNation">Nation du pion à placer</param>
            /// <param name="depart">true si on place sur le départ, false si on place sur la case de destination</param>
            /// <returns>true si OK, false si KO</returns>
            internal bool PlacementPion(Donnees.TAB_NATIONRow ligneNation, bool depart)
            {
                return PlacementPion(ID_CASE, ligneNation, depart, infanterie, cavalerie, artillerie);
            }

            /// <summary>
            /// Placement du pion avec les effectifs arrivés sur place
            /// </summary>
            /// <param name="IDcase">case où placer les effectifs</param>
            /// <param name="lignePion">Pion à placer</param>
            /// <param name="effectif">effectifs sur zone</param>
            /// <returns>true si OK, false si KO</returns>
            internal bool PlacementPion(int IDcase, Donnees.TAB_NATIONRow ligneNation, bool depart, int effectif)
            {
                int iINFANTERIE, iCAVALERIE, iARTILLERIE;

                if (effectif <= 0 || B_DETRUIT)
                {
                    return true; //il n'y a rien à placer en réalité !
                }

                CalculerRepartitionEffectif(effectif, out iINFANTERIE, out iCAVALERIE, out iARTILLERIE);
                return PlacementPion(IDcase, ligneNation, depart, iINFANTERIE, iCAVALERIE, iARTILLERIE);
            }

            /// <summary>
            /// Placement d'un pion avec les effectifs réels sur la case
            /// </summary>
            /// <param name="IDcase">case où placer les effectifs</param>
            /// <param name="lignePion">Pion a placer</param>
            /// <param name="depart">true si on place sur le départ, false si on place sur la case de destination</param>
            /// <param name="effectifInfanterie">effectifs d'infanterie présents</param>
            /// <param name="effectifCavalerie">effectifs de cavalerie présents</param>
            /// <param name="effectifArtillerie">effectifs d'artillerie présents</param>
            /// <returns>true si OK, false si KO</returns>
            internal bool PlacementPion(int IDcase, Donnees.TAB_NATIONRow ligneNation, bool depart, int effectifInfanterie, int effectifCavalerie, int effectifArtillerie)
            {
                decimal encombrement;
                int i, nbplacesOccupes;
                string message, messageErreur;
                //string requete;
                List<int> listeCaseEspace = null;

                try
                {
                    if (IDcase < 0)
                    {
                        message = string.Format("PlacementPion: {0}(ID={1}, ID_CASE:{2}, erreur demande de placement de pion sur une IDcase incorrect)", S_NOM, ID_PION, IDcase);
                        return LogFile.Notifier(message, out messageErreur);
                    }
                    //calcul de l'encombrement
                    encombrement = CalculerEncombrement(effectifInfanterie, effectifCavalerie, effectifArtillerie, false);

                    message = string.Format("PlacementPion : pion ID={0} iInf={1} iCav={2} iArt={3} encombrement={4} en IDcase={5}",
                        ID_PION, effectifInfanterie, effectifCavalerie, effectifArtillerie, encombrement, IDcase);
                    LogFile.Notifier(message, out messageErreur);

                    //maintenant y'a plus qu'à trouver de la place...
                    Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(IDcase);
                    if (null == ligneCase)
                    {
                        message = string.Format("PlacementPion: {0}(ID={1}, ID_CASE:{2}, impossible de trouver la case de placement en base)", S_NOM, ID_PION, IDcase);
                        return LogFile.Notifier(message, out messageErreur);
                    }

                    if (encombrement <= 1)
                    {
                        //cas des unités sans effectifs ou première case de mouvement
                        nbplacesOccupes = 0;
                        if (!RequisitionCase(ligneCase, false, ref nbplacesOccupes)) { return false; }
                    }
                    else
                    {
                        AstarTerrain[] tableCoutsMouvementsTerrain;
                        //CalculModeleMouvementsPion(lignePion, out tableCoutsMouvementsTerrain);
                        //if (!m_etoile.SearchSpace(ligneCase, encombrement, tableCoutsMouvementsTerrain, DataSetCoutDonnees.m_donnees.TAB_JEU[0].I_ECHELLE, out message))
                        if (!RechercheEspace(depart, ligneCase, (int)encombrement, Donnees.m_donnees.TAB_JEU[0].I_ECHELLE, out tableCoutsMouvementsTerrain, out listeCaseEspace, out message))
                        {
                            LogFile.Notifier(message, out messageErreur);
                            message = string.Format("PlacementPion :{0}(ID={1} encombrement={2}, Impossible de trouver l'espace necessaire au pion)",
                                S_NOM, ID_PION, encombrement);
                            return LogFile.Notifier(message, out messageErreur);
                        }

                        i = 0;
                        nbplacesOccupes = 0;
                        while (i < listeCaseEspace.Count && nbplacesOccupes < encombrement)
                        {
                            Donnees.TAB_CASERow ligneOccupation = Donnees.m_donnees.TAB_CASE.FindParID_CASE(listeCaseEspace[i]);

                            if (!RequisitionCase(ligneOccupation, false, ref nbplacesOccupes)) { return false; }
                            i++;
                        }
                        if (nbplacesOccupes < encombrement)
                        {
                            message = string.Format("ALERTE PlacementPion : impossible de placer les effectifs PION={0}({1}) nbplacesOccupes={2}<encombrement={3}",
                                S_NOM, ID_PION, nbplacesOccupes, encombrement);
                            LogFile.Notifier(message, out messageErreur);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string messageEX = string.Format("exception PlacementPion {3} : {0} : {1} :{2}",
                           ex.Message, (null == ex.InnerException) ? "sans inner exception" : ex.InnerException.Message,
                           ex.StackTrace, ex.GetType().ToString());
                    LogFile.Notifier(messageEX);
                    throw ex;
                }
                return true;
            }

            internal bool PlacerStatique()
            {
                string messageErreur, message;
                Donnees.TAB_NATIONRow ligneNation;

                try
                {
                    if (B_DETRUIT) { return true; }

                    Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ID_CASE);
                    if (!MessageEnnemiObserve(ligneCase)) { return false; }

                    Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(ID_PION);
                    if (null != ligneOrdre)
                    {
                        ligneNation = nation;
                        if (null == ligneNation)
                        {
                            message = string.Format("PlacerStatique, ordre mouvement :{0}(ID={1}, Impossible de trouver la nation affectée à l'unité)", S_NOM, ID_PION);
                            return LogFile.Notifier(message, out messageErreur);
                        }

                        //si l'ordre de mouvement n'est pas actif, le pion ne doit pas bouger
                        if (!OrdreActif(ligneOrdre))
                        {
                            PlacerPionEnRoute(ligneOrdre, ligneNation);
                            return true;
                        }
                        else
                        {
                            message = string.Format("{0}(ID={1}, unité non statique, ID_CASE={2})", S_NOM, ID_PION, ID_CASE);
                            return LogFile.Notifier(message, out messageErreur);
                        }
                    }
                    else
                    {
                        //placer l'unité sur la carte
                        ligneNation = nation;
                        if (null == ligneNation)
                        {
                            message = string.Format("PlacerStatique, sans ordre mouvement :{0}(ID={1}, Impossible de trouver la nation affectée à l'unité)", S_NOM, ID_PION);
                            return LogFile.Notifier(message, out messageErreur);
                        }
                        PlacementPion(ligneNation, true);
                    }                    
                }
                catch (Exception ex)
                {
                    string messageEX = string.Format("exception PlacerStatique {3} : {0} : {1} :{2}",
                           ex.Message, (null == ex.InnerException) ? "sans inner exception" : ex.InnerException.Message,
                           ex.StackTrace, ex.GetType().ToString());
                    LogFile.Notifier(messageEX);//ne marche si une exception est lancé semble-t-il...
                    throw ex;
                }
                return true;
            }

            internal bool PlacerPionEnRoute(Donnees.TAB_ORDRERow ligneOrdre, Donnees.TAB_NATIONRow ligneNation)
            {
                int idCaseD, idCaseF;
                return PlacerPionEnRoute(ligneOrdre, ligneNation, false, out idCaseD, out idCaseF);
            }

            internal bool PlacerPionEnRoute(Donnees.TAB_ORDRERow ligneOrdre, Donnees.TAB_NATIONRow ligneNation, bool rechercheIdCase, out int idCaseDebut, out int idCaseFin)
            {
                string messageErreur, message;
                decimal encombrement, encombrementTotal;
                int iCavalerie, iInfanterie, iArtillerie;
                int iCavalerieDestination, iInfanterieDestination, iArtillerieDestination;
                int iCavalerieRoute, iInfanterieRoute, iArtillerieRoute;
                AstarTerrain[] tableCoutsMouvementsTerrain;
                List<LigneCASE> chemin;
                int i,j;
                int nbplacesOccupes;
                double cout, coutHorsRoute;
                Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DESTINATION);
                Donnees.TAB_CASERow ligneCaseDepart = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DEPART);
                AStar etoile = new AStar();

                idCaseDebut = -1;
                idCaseFin = ID_CASE;//valeur par défaut
                int effectifInfanterie = (estDepot || estConvoiDeRavitaillement || estPontonnier) ? effectifTotalEnMouvement : infanterie;
                encombrementTotal = CalculerEncombrement(effectifInfanterie, cavalerie, artillerie, true);
                //recherche du plus court chemin
                if (!etoile.RechercheChemin(Constantes.TYPEPARCOURS.MOUVEMENT, this, ligneCaseDepart, ligneCaseDestination, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                {
                    message = string.Format("{0},ID={1}, erreur sur RechercheChemin (cas 2) dans PlacerPionEnRoute: {2})", S_NOM, ID_PION, messageErreur);
                    LogFile.Notifier(message);
                    return false;
                }
                message = string.Format("PlacerPionEnRoute : {0},ID={1}, SearchPath longueur={2}, ID_CASE={3}", S_NOM, ID_PION, chemin.Count, ID_CASE);
                LogFile.Notifier(message);

                if (ligneOrdre.I_EFFECTIF_DESTINATION > 0)
                {
                    #region Pion à destination
                    //l'unité est arrivée, il faut donc "écouler" les éléments qui ne sont pas encore arrivés s'il y en a
                    message = string.Format("PlacerPionEnRoute : {0},ID={1} en mouvement, une partie des troupes à destination)", S_NOM, ID_PION);
                    LogFile.Notifier(message);

                    message = string.Format("PlacerPionEnRoute : {0},ID={1}, ligneOrdre.I_EFFECTIF_DESTINATION={2}", S_NOM, ID_PION, ligneOrdre.I_EFFECTIF_DESTINATION);
                    LogFile.Notifier(message);
                    if (ligneOrdre.I_EFFECTIF_DESTINATION <= effectifTotalEnMouvement)
                    {
                        //il faut faire avancer la "queue" de troupes jusqu'à l'arrivée
                        //on avance suivant le modele par défaut, sur route, dont on calcule le cout

                        //effectifs actuellement à l'arrivée
                        CalculerRepartitionEffectif(ligneOrdre.I_EFFECTIF_DESTINATION,
                                                    out iInfanterieDestination, out iCavalerieDestination, out iArtillerieDestination);
                        message = string.Format("PlacerPionEnRoute : {0},ID={1}, effectif à destination :i={2} c={3} a={4}",
                                                S_NOM, ID_PION,
                                                iInfanterieDestination, iCavalerieDestination, iArtillerieDestination);
                        LogFile.Notifier(message);

                        //effectifs maximum sur la route
                        decimal encombrementArrivee = CalculerEncombrement(iInfanterieDestination, iCavalerieDestination, iArtillerieDestination, true);
                        if (!CalculerEffectif(
                                Math.Max(0, effectifInfanterie - iInfanterieDestination),
                                Math.Max(0, cavalerie - iCavalerieDestination),
                                Math.Max(0, artillerie - iArtillerieDestination),
                                encombrementTotal - encombrementArrivee,
                                //chemin.Count, 
                                true,
                                out iInfanterieRoute, out iCavalerieRoute, out iArtillerieRoute))
                        {
                            message = string.Format("PlacerPionEnRoute :{0}, ID={1}, erreur CalculerEffectif sur la route renvoie false)", S_NOM, ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }
                        message = string.Format("PlacerPionEnRoute effectif maximum sur route iINFANTERIESource={0}, iCAVALERIESource={1}, iARTILLERIESource={2}, lg chemin={3}, iINFANTERIERoute={4}, iCAVALERIERoute={5}, iARTILLERIERoute={6}",
                            effectifInfanterie - iInfanterieDestination,
                            cavalerie - iCavalerieDestination,
                            artillerie - iArtillerieDestination,
                            chemin.Count,
                            iInfanterieRoute, iCavalerieRoute, iArtillerieRoute);
                        LogFile.Notifier(message);

                        //calcul de l'encombrement des gens encore sur la route
                        //if (iInfanterieRoute + iCavalerieRoute + iArtillerieRoute >= lignePion.effectifTotal - ligneOrdre.I_EFFECTIF_DESTINATION)
                        if (0 == ligneOrdre.I_EFFECTIF_DEPART)
                        {
                            //la route est partiellement occupée
                            encombrement = CalculerEncombrement(iInfanterieRoute, iCavalerieRoute, iArtillerieRoute, true);
                        }
                        else
                        {
                            //la route est complètement occupée
                            encombrement = chemin.Count;
                            //on recalcule les effectifs reellement sur la route
                            if (!CalculerEffectif(
                                    iInfanterieRoute, iCavalerieRoute, iArtillerieRoute,
                                    encombrement,
                                    true,
                                    out iInfanterieRoute, out iCavalerieRoute, out iArtillerieRoute))
                            {
                                message = string.Format("PlacerPionEnRoute :{0}(ID={1}, erreur CalculerEffectif sur la route totlement occupée)", S_NOM, ID_PION);
                                LogFile.Notifier(message);
                                return false;
                            }
                        }
                        message = string.Format("PlacerPionEnRoute :{0},ID={1},encombrement sur route={2}", S_NOM, ID_PION, encombrement);
                        LogFile.Notifier(message);
                        message = string.Format("PlacerPionEnRoute {0},ID={1},:effectif destination={2}", S_NOM, ID_PION, ligneOrdre.I_EFFECTIF_DESTINATION);
                        LogFile.Notifier(message);

                        if (rechercheIdCase)
                        {
                            idCaseDebut = ligneOrdre.ID_CASE_DESTINATION;
                            idCaseFin = chemin[Math.Max(0, chemin.Count - Math.Max(1, (int)encombrement))].ID_CASE;
                        }
                        else
                        {
                            //placer les effectifs en chemin
                            i = chemin.Count - 1;
                            j = nbplacesOccupes = 0;
                            //while (i >= 0 && nbplacesOccupes < encombrement)
                            while (i >= 0 && j < encombrement)
                                {
                                    //une case de plus est occupée
                                    if (!RequisitionCase(chemin[i], true, ref nbplacesOccupes))
                                {
                                    return false;
                                }
                                i--;
                                j++;
                            }
                            if (nbplacesOccupes < encombrement)
                            {
                                message = string.Format("ALERTE PlacerPionEnRoute : impossible de placer les effectifs encore en route PION={0}, {1}) nbplacesOccupes={2}<encombrement={3}",
                                    S_NOM, ID_PION, nbplacesOccupes, encombrement);
                                LogFile.Notifier(message);
                            }

                            //placement des effectifs à destination sur la carte
                            PlacementPion(ligneOrdre.ID_CASE_DESTINATION, ligneNation, false, ligneOrdre.I_EFFECTIF_DESTINATION);

                            //placer les effectifs encore au point de départ
                            if (ligneOrdre.I_EFFECTIF_DEPART > 0)
                            {
                                message = string.Format("PlacerPionEnRoute : {0},ID={1}, effectif depart={2} i={3}, c={4}, a={5}",
                                    S_NOM, ID_PION,
                                    ligneOrdre.I_EFFECTIF_DEPART,
                                    infanterie - iInfanterieDestination - iInfanterieRoute,
                                    cavalerie - iCavalerieDestination - iCavalerieRoute,
                                    artillerie - iArtillerieDestination - iArtillerieRoute);
                                LogFile.Notifier(message);
                                //out iInfanterie, out iCavalerie, out iArtillerie
                                PlacementPion(ligneOrdre.ID_CASE_DEPART, ligneNation, true,
                                    effectifInfanterie - iInfanterieDestination - iInfanterieRoute,
                                    cavalerie - iCavalerieDestination - iCavalerieRoute,
                                    artillerie - iArtillerieDestination - iArtillerieRoute);
                            }
                        }
                    }
                    else
                    {
                        //l'unité est complètement arrivée, cas possible si la fatigue de fin de journée réduit le nombre de personnes  -> traiter dans fatigue et repose
                        /*
                        idCaseDebut = ligneOrdre.ID_CASE_DESTINATION;
                        idCaseFin = idCaseDebut;
                        PlacementPion(ligneOrdre.ID_CASE_DESTINATION, lignePion, ligneNation, false, lignePion.effectifTotalEnMouvement);
                        */
                        message = string.Format("{0}(ID={1}, PlacerPionEnRoute Erreur fin du mouvement, complètement arrivée : cas normalement impossible)", S_NOM, ID_PION);
                        LogFile.Notifier(message);
                        return false;
                    }
                    #endregion
                }
                else
                {
                    #region Pion non encore arrivé à destination
                    message = string.Format("{0}(ID={1}, en mouvement, aucune troupe à destination)", S_NOM, ID_PION);
                    LogFile.Notifier(message);
                    message = string.Format("PlacerPionEnRoute :ligneOrdre.I_EFFECTIF_DEPART={0}", ligneOrdre.I_EFFECTIF_DEPART);
                    LogFile.Notifier(message);

                    //on calcule de combien de cases le pion a déjà avancé
                    CalculerRepartitionEffectif(Math.Max(0,effectifTotalEnMouvement - ligneOrdre.I_EFFECTIF_DEPART),
                        out iInfanterie, out iCavalerie, out iArtillerie);
                    message = string.Format("PlacerPionEnRoute :effectif: i={0} c={1} a={2}", iInfanterie, iCavalerie, iArtillerie);
                    LogFile.Notifier(message);

                    encombrement = CalculerEncombrement(iInfanterie, iCavalerie, iArtillerie, true);
                    message = string.Format("PlacerPionEnRoute : encombrement={0}", encombrement);
                    LogFile.Notifier(message);

                    i = 0;
                    //on recherche la position de la queue de colonne
                    while (i < chemin.Count && chemin[i].ID_CASE != ID_CASE)
                    {
                        i++;
                    }
                    if (i >= chemin.Count)
                    {
                        message = string.Format("{0}(ID={1}, PlacerPionEnRoute: impossible de trouver la position du pion sur le parcours !)",
                            S_NOM, ID_PION);
                        LogFile.Notifier(message);
                        return false;
                    }

                    if (rechercheIdCase)
                    {
                        idCaseDebut = chemin[Math.Min(chemin.Count - 1, i + (int)encombrement)].ID_CASE;
                        idCaseFin = ID_CASE;
                    }
                    else
                    {
                        j = nbplacesOccupes = 0;
                        //while (i < chemin.Count && nbplacesOccupes < encombrement)
                        while (i < chemin.Count && j < encombrement)//on doit toujours considérer, libre ou non, sinon, on saute les cases occupées par l'ennemi la nuit ou on avance plus vite si c'est des amis !
                        {
                            if (!RequisitionCase(chemin[i++], true, ref nbplacesOccupes)) { return false; }
                            j++;
                        }
                        if (nbplacesOccupes < encombrement)
                        {
                            message = string.Format("ALERTE PlacerPionEnRoute : impossible de placer les effectifs en mouvement PION={0}({1}) nbplacesOccupes={2}<encombrement={3}",
                                S_NOM, ID_PION, nbplacesOccupes, encombrement);
                            LogFile.Notifier(message);
                        }

                        //on place les effectifs encore au départ
                        PlacementPion(ligneOrdre.ID_CASE_DEPART, ligneNation, true, ligneOrdre.I_EFFECTIF_DEPART);
                        message = string.Format("PlacerPionEnRoute :ligneOrdre.I_EFFECTIF_DEPART final={0}", ligneOrdre.I_EFFECTIF_DEPART);
                        LogFile.Notifier(message);
                    }
                    #endregion
                }
                return true;
            }

            /// <summary>
            /// Renvoie la case du début et de fin de la colonne de progression d'une unité
            /// </summary>
            /// <param name="lignePion">Pion dont on recherche la case</param>
            /// <param name="iDCaseDebut">identifiant de la case correspondante</param>
            /// <param name="iDCaseFin">identifiant de la case correspondante</param>
            /// <returns>true si OK, false si KO</returns>
            internal bool CasesDebutFin(out int iDCaseDebut, out int iDCaseFin)
            {
                iDCaseFin = ID_CASE;
                iDCaseDebut = iDCaseFin;
                if (B_DETRUIT || effectifTotal <= 0)
                {
                    //une unité sans effectifs ou détruite n'a jamais de colonne de progression
                    return true;
                }
                Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(ID_PION);
                if (null == ligneOrdre)
                {
                    //une unité qui n'est pas en mouvement n'a pas de colonne de progression
                    return true;
                }
                if (!OrdreActif(ligneOrdre))
                {
                    //une unité qui n'est pas en mouvement n'a pas de colonne de progression
                    return true;
                }
                Donnees.TAB_NATIONRow ligneNation = nation; // Donnees.m_donnees.TAB_PION.TrouveNation(lignePion);
                return PlacerPionEnRoute(ligneOrdre, ligneNation, true, out iDCaseDebut, out iDCaseFin);
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
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.ID_ANCIEN_PION_PROPRIETAIRE = Constantes.NULLENTIER;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }

            internal void SetID_NOUVEAU_PION_PROPRIETAIRENull()
            {
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.ID_NOUVEAU_PION_PROPRIETAIRE = Constantes.NULLENTIER;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }

            /******************** Nouveaux *********************/
            internal bool IsID_DEPOT_SOURCENull()
            {
                return this.ID_DEPOT_SOURCE == Constantes.NULLENTIER;
            }

            internal void SetID_DEPOT_SOURCENull()
            {
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.ID_DEPOT_SOURCE = Constantes.NULLENTIER;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }

            internal bool IsID_PION_REMPLACENull()
            {
                return this.ID_PION_REMPLACE == Constantes.NULLENTIER;
            }

            internal void SetID_PION_REMPLACENull()
            {
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.ID_PION_REMPLACE = Constantes.NULLENTIER;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
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
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.I_TOUR_BLESSURE = Constantes.NULLENTIER;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }

            internal bool IsID_LIEU_RATTACHEMENTNull()
            {
                return this.ID_LIEU_RATTACHEMENT == Constantes.NULLENTIER;
            }

            internal void SetID_LIEU_RATTACHEMENTNull()
            {
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.ID_LIEU_RATTACHEMENT = Constantes.NULLENTIER;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }

            internal bool IsID_PION_ESCORTENull()
            {
                return this.ID_PION_ESCORTE == Constantes.NULLENTIER;
            }

            internal void SetID_PION_ESCORTENull()
            {
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.ID_PION_ESCORTE = Constantes.NULLENTIER;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }

            internal bool IsI_TOUR_DERNIER_RAVITAILLEMENT_DIRECTNull()
            {
                return this.I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT == Constantes.NULLENTIER;
            }

            internal void SetI_TOUR_DERNIER_RAVITAILLEMENT_DIRECTNull()
            {
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT = Constantes.NULLENTIER;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
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
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.ID_BATAILLE = Constantes.NULLENTIER;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }

            internal void SetI_ZONE_BATAILLENull()
            {
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.I_ZONE_BATAILLE = Constantes.NULLENTIER;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }

            internal string NomDeLaPatrouille()
            {
                return "Patrouille du " + S_NOM;
            }

            internal bool IsI_TRINull()
            {
                if (this.IsNull("I_TRI")) return true;
                return this.I_TRI == Constantes.NULLENTIER;
            }

            internal void SetI_TRINull()
            {
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.I_TRI = Constantes.NULLENTIER;
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }
            /*
            internal bool IsI_TOUR_ENNEMI_OBSERVABLENull()
            {
                if (this.IsNull("I_TOUR_ENNEMI_OBSERVABLE")) return true;
                return this.I_TOUR_ENNEMI_OBSERVABLE == Constantes.NULLENTIER;
            }
            internal void SetI_TOUR_ENNEMI_OBSERVABLENull()
            {
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                this.I_TOUR_ENNEMI_OBSERVABLE = Constantes.NULLENTIER;
                Verrou.Derrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }
            */
        }
    }
}
