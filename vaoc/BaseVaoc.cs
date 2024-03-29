﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using WaocLib;

namespace vaoc
{
    partial class Donnees
    {
        #region variables
        public static Donnees m_donnees;
        public static int m_phaseCases = -1;
        public static int m_tourCases = -1;
        /// <summary>
        /// Liste des case où le proprietaire à changer pour ne pas avoir à faire le 
        /// string requete = string.Format("(ID_PROPRIETAIRE<>{0}) OR (ID_NOUVEAU_PROPRIETAIRE<>{0})", Constantes.NULLENTIER);
        /// Donnees.TAB_CASERow[] changeRows = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete);
        /// qui prendre, à lui seul 15% du temps de traitement
        /// </summary>
        //public static HashSet<TAB_CASERow> m_listeNouveauProprietaire = new HashSet<TAB_CASERow>();
        #endregion

        partial class TAB_VIDEORow
        {
            public int EffectifTotal
            {
                get { return this.I_ARTILLERIE + this.I_INFANTERIE + this.I_CAVALERIE; }
            }
        }

        partial class TAB_NOMS_CARTERow
        {
            /// <summary>
            /// Renvoie la nation qui contrôle ce "nom" sur la carte
            /// </summary>
            public TAB_NATIONRow nation
            {
                get
                {
                    return m_donnees.TAB_NATION.FindByID_NATION(this.ID_NATION_CONTROLE);
                }
            }
        }

        partial class TAB_RENFORTRow
        {
            public bool possedeAptitude(string nomAptitude)
            {
                string requete;

                //recherche le modèle du pion
                requete = string.Format("ID_MODELE_PION={0}", this.ID_MODELE_PION);
                Monitor.Enter(m_donnees.TAB_MODELE_PION.Rows.SyncRoot);
                TAB_MODELE_PIONRow[] resModelePion = (TAB_MODELE_PIONRow[])m_donnees.TAB_MODELE_PION.Select(requete);
                Monitor.Exit(m_donnees.TAB_MODELE_PION.Rows.SyncRoot);

                //recherche l'aptitude fournie en paramètre
                requete = string.Format("S_NOM='{0}'", nomAptitude);
                Monitor.Enter(m_donnees.TAB_APTITUDES.Rows.SyncRoot);
                TAB_APTITUDESRow[] resAptitude = (TAB_APTITUDESRow[])m_donnees.TAB_APTITUDES.Select(requete);
                Monitor.Exit(m_donnees.TAB_APTITUDES.Rows.SyncRoot);

                //recherche si le modele de pion possède l'aptitude demandée
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
                    return !estQG;
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
                    TAB_MODELE_PIONRow ligneModele = m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION(ID_MODELE_PION);
                    if (null == ligneModele) { return -1; }
                    return ligneModele.ID_NATION;
                }
            }

            /// <summary>
            /// effectif total du pion
            /// </summary>
            public int effectifTotal
            {
                get
                {
                    return I_ARTILLERIE + I_CAVALERIE + I_INFANTERIE;
                }
                //set; 
            }
        }

        partial class TAB_PCC_COUTSDataTable
        {
            Dictionary<int, List<TAB_PCC_COUTSRow>> m_listeCoutsParIDCase = new Dictionary<int, List<TAB_PCC_COUTSRow>>();

            public bool Initialisation()
            {
                m_listeCoutsParIDCase.Clear();
                List<TAB_PCC_COUTSRow> listePCCCouts; //= new List<TAB_PCC_COUTSRow>();

                foreach (TAB_PCC_COUTSRow lCout in this)
                {
                    if (m_listeCoutsParIDCase.ContainsKey(lCout.ID_CASE_DEBUT))
                    {
                        listePCCCouts = m_listeCoutsParIDCase[lCout.ID_CASE_DEBUT];
                    }
                    else
                    {
                        listePCCCouts = new List<TAB_PCC_COUTSRow>();
                        m_listeCoutsParIDCase.Add(lCout.ID_CASE_DEBUT, listePCCCouts);
                    }
                    listePCCCouts.Add(lCout);
                }
                return true;
            }

            /// <summary>
            /// renvoie toutes les cases de liaisons voisines d'autres blocs que le sien
            /// </summary>
            /// <param name="source"></param>
            /// <param name="xBloc"></param>
            /// <param name="yBloc"></param>
            /// <returns></returns>
            public List<TAB_PCC_COUTSRow> CasesVoisines(TAB_PCC_COUTSRow source, bool BlocInterneCompris)
            {
                List<TAB_PCC_COUTSRow> resPCCCouts = new List<TAB_PCC_COUTSRow>();
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
                List<TAB_PCC_COUTSRow> listePCCCouts = m_listeCoutsParIDCase[source.ID_CASE_FIN];
                foreach (TAB_PCC_COUTSRow lCout in listePCCCouts)
                {
                    if (BlocInterneCompris || ((lCout.I_BLOCX != source.I_BLOCX) || (lCout.I_BLOCY != source.I_BLOCY)))
                    {
                        resPCCCouts.Add(lCout);
                    }
                }
                return resPCCCouts;
            }
        }

        partial class TAB_PCC_CASE_BLOCSDataTable
        {
            public TAB_PCC_CASE_BLOCSRow[] ListeCasesBloc(int xBloc, int yBloc)
            {
                string requete = string.Format("I_BLOCX={0} AND I_BLOCY={1}",
                    xBloc, yBloc);
                Monitor.Enter(this.Rows.SyncRoot);
                TAB_PCC_CASE_BLOCSRow[] resCase = (TAB_PCC_CASE_BLOCSRow[])Select(requete);
                Monitor.Exit(this.Rows.SyncRoot);
                return resCase;
            }
        }

        partial class TAB_PARCOURSDataTable
        {
            public void SupprimerParcoursPion(int id_pion)
            {
                string requete = string.Format("ID_PION={0}", id_pion);

                Monitor.Enter(Donnees.m_donnees.TAB_PARCOURS.Rows.SyncRoot);
                System.Data.DataRow[] lignesASupprimer = Select(requete);
                foreach (System.Data.DataRow ligne in lignesASupprimer)
                {
                    ligne.Delete();
                }
                Monitor.Exit(Donnees.m_donnees.TAB_PARCOURS.Rows.SyncRoot);
            }
        }

        partial class TAB_ESPACEDataTable
        {
            /// <summary>
            /// Deplace un espace pion 
            /// </summary>
            /// <param name="id_pion">identifiant du pion</param>
            /// <param name="typeEspaceSource">AStar.CST_DEPART ou AStar.CST_DESTINATION</param>
            public void DeplacerEspacePion(int id_pion, char typeEspaceSource)
            {
                char typeEspaceDestination = (typeEspaceSource == AStar.CST_DESTINATION) ? AStar.CST_DEPART : AStar.CST_DESTINATION;

                //suppression des lignes à remplacer
                SupprimerEspacePion(id_pion, typeEspaceDestination);

                //remplacement par les nouvelles lignes
                string requete = string.Format("ID_PION={0} AND C_TYPE='{1}'", id_pion, typeEspaceSource);
                Monitor.Enter(Donnees.m_donnees.TAB_ESPACE.Rows.SyncRoot);
                System.Data.DataRow[] lignesARemplacer = Select(requete);
                foreach (TAB_ESPACERow ligne in lignesARemplacer)
                {
                    ligne.C_TYPE = typeEspaceDestination;
                }
                Monitor.Exit(Donnees.m_donnees.TAB_ESPACE.Rows.SyncRoot);
            }

            public void SupprimerEspacePion(int id_pion, char typeEspace)
            {
                string requete = string.Format("ID_PION={0} AND C_TYPE='{1}'", id_pion, typeEspace);

                Monitor.Enter(Donnees.m_donnees.TAB_ESPACE.Rows.SyncRoot);
                System.Data.DataRow[] lignesASupprimer = Select(requete);
                foreach (System.Data.DataRow ligne in lignesASupprimer)
                {
                    ligne.Delete();
                }
                Monitor.Exit(Donnees.m_donnees.TAB_ESPACE.Rows.SyncRoot);
            }

            public void SupprimerEspacePion(int id_pion)
            {
                string requete = string.Format("ID_PION={0}", id_pion);

                Monitor.Enter(Donnees.m_donnees.TAB_ESPACE.Rows.SyncRoot);
                System.Data.DataRow[] lignesASupprimer = Select(requete);
                foreach (System.Data.DataRow ligne in lignesASupprimer)
                {
                    ligne.Delete();
                }
                Monitor.Exit(Donnees.m_donnees.TAB_ESPACE.Rows.SyncRoot);
            }
        }

        partial class TAB_MESSAGEDataTable
        {
            static int id_messageGlobal = -1;//increment des identifiants des messages

            /// <summary>
            /// Recherche le dernier ordre envoyé par une unite
            /// </summary>
            /// <param name="ID_PION">identifiant du pion</param>
            /// <returns>dernier message envoyé, null si aucun</returns>
            internal TAB_MESSAGERow DernierMessageRecu(int id_pion, int id_pion_proprietaire)
            {
                string requete;
                if (id_pion_proprietaire >= 0)
                {
                    requete = string.Format("ID_PION_EMETTEUR={0} AND ID_PION_PROPRIETAIRE={1} AND I_TOUR_ARRIVEE IS NOT NULL", id_pion, id_pion_proprietaire);
                }
                else
                {
                    requete = string.Format("ID_PION_EMETTEUR={0} AND I_TOUR_ARRIVEE IS NOT NULL", id_pion);
                }
                string tri = "I_TOUR_DEPART, I_PHASE_DEPART, ID_MESSAGE";
                TAB_MESSAGERow[] resMessage = (TAB_MESSAGERow[])Select(requete, tri);
                if (0 == resMessage.Length)
                {
                    return null;
                }
                return resMessage[resMessage.Length - 1];
            }

            internal TAB_MESSAGERow DernierMessageRecu(int id_pion, ClassMessager.MESSAGES i_type)
            {
                string requete = string.Format("ID_PION_EMETTEUR={0} AND (I_TOUR_ARRIVEE IS NOT NULL) AND I_TYPE={1}", id_pion, (int)i_type);
                string tri = "I_TOUR_DEPART, I_PHASE_DEPART";
                TAB_MESSAGERow[] resMessage = (TAB_MESSAGERow[])Select(requete, tri);
                if (0 == resMessage.Length)
                {
                    return null;
                }
                return resMessage[0];
            }

            internal TAB_MESSAGERow DernierMessageEmis(int id_pion)
            {
                //s'il est émis, peu importe de vérifier s'il n'est pas arrivé, sinon, ne fonctionne pas pour les messages directs qui arrivent
                //immédiatement
                string requete = string.Format("ID_PION_EMETTEUR={0}", id_pion);
                string tri = "I_TOUR_DEPART DESC, I_PHASE_DEPART DESC";
                TAB_MESSAGERow[] resMessage = (TAB_MESSAGERow[])Select(requete, tri);
                if (0 == resMessage.Length)
                {
                    return null;
                }
                return resMessage[0];
            }

            internal TAB_MESSAGERow DernierMessageEmis(int id_pion, ClassMessager.MESSAGES i_type)
            {
                //string requete = string.Format("ID_PION_EMETTEUR={0} AND (I_TOUR_ARRIVEE IS NULL) AND I_TYPE={1}", id_pion, (int)i_type);
                //s'il est émis, peu importe de vérifier s'il n'est pas arrivé, sinon, ne fonctionne pas pour les messages directs qui arrivent
                //immédiatement
                string requete = string.Format("ID_PION_EMETTEUR={0} AND I_TYPE={1}", id_pion, (int)i_type);
                string tri = "I_TOUR_DEPART DESC, I_PHASE_DEPART DESC";
                Monitor.Enter(Donnees.m_donnees.TAB_MESSAGE.Rows.SyncRoot);
                TAB_MESSAGERow[] resMessage = (TAB_MESSAGERow[])Select(requete, tri);
                Monitor.Exit(Donnees.m_donnees.TAB_MESSAGE.Rows.SyncRoot);
                if (0 == resMessage.Length)
                {
                    return null;
                }
                return resMessage[0];
            }

            public TAB_MESSAGERow AjouterMessage(
                        int ID_PION_EMETTEUR,
                        int ID_PION_PROPRIETAIRE,
                        int I_TYPE,
                        int I_TOUR_ARRIVEE,
                        int I_PHASE_ARRIVEE,
                        int I_TOUR_DEPART,
                        int I_PHASE_DEPART,
                        string S_TEXTE,
                        int I_INFANTERIE,
                        int I_CAVALERIE,
                        int I_ARTILLERIE,
                        int I_FATIGUE,
                        int I_MORAL,
                        int I_TOUR_SANS_RAVITAILLEMENT,
                        int ID_BATAILLE,
                        int I_ZONE_BATAILLE,
                        int I_RETRAITE,
                        bool B_DETRUIT,
                        int ID_CASE_DEBUT,
                        int ID_CASE_FIN,
                        int I_NB_PHASES_MARCHE_JOUR,
                        int I_NB_PHASES_MARCHE_NUIT,
                        int I_NB_HEURES_COMBAT,
                        int I_MATERIEL,
                        int I_RAVITAILLEMENT,
                        int I_SOLDATS_RAVITAILLES,
                        int I_NB_HEURES_FORTIFICATION,
                        int I_NIVEAU_FORTIFICATION,
                        int I_DUREE_HORS_COMBAT,
                        int I_TOUR_BLESSURE,
                        char C_NIVEAU_DEPOT
                )
            {
                Monitor.Enter(Donnees.m_donnees.TAB_MESSAGE.Rows.SyncRoot);
                if (id_messageGlobal < 0)
                {
                    //recherche du plus grand identifiant
                    if (this.Count > 0)
                    {
                        System.Nullable<int> maxIdMessage =
                            (from message in this
                             select message.ID_MESSAGE)
                            .Max();
                        id_messageGlobal = (int)maxIdMessage;
                    }
                    else
                    {
                        id_messageGlobal = 0;
                    }
                }
                id_messageGlobal++;

                LogFile.Notifier("AjouterMessage");
                TAB_MESSAGERow rowTAB_MESSAGERow = ((TAB_MESSAGERow)(this.NewRow()));
                object[] columnValuesArray = new object[] {
                        id_messageGlobal,
                        ID_PION_EMETTEUR,
                        ID_PION_PROPRIETAIRE,
                        I_TYPE,
                        I_TOUR_ARRIVEE,
                        I_PHASE_ARRIVEE,
                        I_TOUR_DEPART,
                        I_PHASE_DEPART,
                        S_TEXTE,
                        I_INFANTERIE,
                        I_CAVALERIE,
                        I_ARTILLERIE,
                        I_FATIGUE,
                        I_MORAL,
                        I_TOUR_SANS_RAVITAILLEMENT,
                        ID_BATAILLE,
                        I_ZONE_BATAILLE,
                        I_RETRAITE,
                        B_DETRUIT,
                        ID_CASE_DEBUT,
                        ID_CASE_DEBUT,
                        ID_CASE_FIN,
                        I_NB_PHASES_MARCHE_JOUR,
                        I_NB_PHASES_MARCHE_NUIT,
                        I_NB_HEURES_COMBAT,
                        I_MATERIEL,
                        I_RAVITAILLEMENT,
                        I_SOLDATS_RAVITAILLES,
                        I_NB_HEURES_FORTIFICATION,
                        I_NIVEAU_FORTIFICATION,
                        I_DUREE_HORS_COMBAT,
                        I_TOUR_BLESSURE,
                        C_NIVEAU_DEPOT
                };
                rowTAB_MESSAGERow.ItemArray = columnValuesArray;
                this.Rows.Add(rowTAB_MESSAGERow);
                Monitor.Exit(Donnees.m_donnees.TAB_MESSAGE.Rows.SyncRoot);
                return rowTAB_MESSAGERow;
            }
        }

        partial class TAB_MESSAGERow
        {
            public TAB_PIONRow emetteur
            {
                get
                {
                    TAB_PIONRow lignePionEmetteur = Donnees.m_donnees.TAB_PION.FindByID_PION(this.ID_PION_EMETTEUR);
                    return lignePionEmetteur;
                }
            }
        }

        partial class TAB_ORDRE_ANCIENDataTable
        {
            /// <summary>
            ///  Renvoi l'ordre correspondant à un identifiant du web
            /// </summary>
            /// <param name="ID_ORDREWEB">identifiant de l'ordre du web</param>
            /// <returns>ordre Web, null si aucun</returns>
            public TAB_ORDRE_ANCIENRow OrdreWeb(int ID_ORDREWEB)
            {
                TAB_ORDRE_ANCIENRow ligneOrdreRetour = null;
                string requete = string.Format("ID_ORDRE_WEB={0}", ID_ORDREWEB);
                Monitor.Enter(Donnees.m_donnees.TAB_ORDRE_ANCIEN.Rows.SyncRoot);
                TAB_ORDRE_ANCIENRow[] resOrdre = (TAB_ORDRE_ANCIENRow[])Select(requete);
                if (0 != resOrdre.Length)
                {
                    ligneOrdreRetour = resOrdre[0];
                }
                Monitor.Exit(Donnees.m_donnees.TAB_ORDRE_ANCIEN.Rows.SyncRoot);
                return ligneOrdreRetour;
            }
        }

        partial class TAB_ORDREDataTable
        {
            //public int ProchainID_ORDRE
            //{
            //    get
            //    {
            //        if (this.Count > 0)
            //        {
            //            System.Nullable<int> maxIdOrdre =
            //                (from ordre in this
            //                 select ordre.ID_ORDRE)
            //                .Max();
            //            return (int)maxIdOrdre+1;
            //        }
            //        return 0;
            //    }
            //}

            /// <summary>
            /// Renvoie tous les ordres d'envoie de patrouilles mais non encore reçues par l'unite finale
            /// </summary>
            /// <param name="ID_PION">Pion proprietaire de la patrouille</param>
            /// <returns>liste des ordres de patrouilles</returns>
            public TAB_ORDRERow[] PatrouillesNonEnvoyees(int ID_PION)
            {
                string tri = "ID_ORDRE";
                //On cherche l'ordre actuellement actif s'il existe, on vérifie qu'il s'agit bien d'un ordre de patrouille
                //note : correctif le 11/02/2018 avec ajout de ID_PION, sinon l'ordre est valide, c'est celui executé par la patrouille en cours
                string requete = string.Format("(ID_PION={0}) AND (ID_DESTINATAIRE={0}) AND (I_TOUR_FIN = {1}) AND (I_PHASE_FIN = {1}) AND I_ORDRE_TYPE = {2}",
                    ID_PION, Constantes.NULLENTIER, Constantes.ORDRES.PATROUILLE);
                Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                TAB_ORDRERow[] resOrdre = (TAB_ORDRERow[])Select(requete, tri);
                Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                return resOrdre;
            }

            /// <summary>
            ///  Renvoi l'ordre correspondant à un identifiant du web
            /// </summary>
            /// <param name="ID_ORDREWEB">identifiant de l'ordre du web</param>
            /// <returns>ordre Web, null si aucun</returns>
            public TAB_ORDRERow OrdreWeb(int ID_ORDREWEB)
            {
                TAB_ORDRERow ligneOrdreRetour = null;
                string requete = string.Format("ID_ORDRE_WEB={0}", ID_ORDREWEB);
                Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                TAB_ORDRERow[] resOrdre = (TAB_ORDRERow[])Select(requete);
                if (0 != resOrdre.Length)
                {
                    ligneOrdreRetour = resOrdre[0];
                }
                Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                return ligneOrdreRetour;
            }


            /// <summary>
            ///  Renvoi l'ordre de mouvement affecté à l'unité, null si aucun
            /// </summary>
            /// <param name="ID_PION">Pion recherchant son mouvement</param>
            /// <returns>ordre de mouvement du pion, null si aucun</returns>
            public TAB_ORDRERow Mouvement(int ID_PION)
            {
                TAB_ORDRERow ligneOrdreRetour = null;
                //string requete = string.Format("ID_PION={0} AND I_ORDRE_TYPE = {1}  AND I_TOUR_FIN IS NOT NULL AND I_PHASE_FIN IS NOT NULL", ID_PION, CST_ORDRE_MOUVEMENT);
                //quand l'ordre est terminé d'execution, I_TOUR_FIN et I_PHASE_FIN sont renseignés
                string tri = "ID_ORDRE";
                /* ancienne version avant les ordres chainées
                string requete = string.Format("(ID_PION={0}) AND (I_ORDRE_TYPE = {1} OR I_ORDRE_TYPE = {2} OR I_ORDRE_TYPE = {3})  AND (I_TOUR_FIN IS NULL) AND (I_PHASE_FIN IS NULL)", 
                    ID_PION, 
                    Constantes.ORDRES.MOUVEMENT, Constantes.ORDRES.MESSAGE, Constantes.ORDRES.PATROUILLE);
                TAB_ORDRERow[] resOrdre = (TAB_ORDRERow[])Select(requete, tri);
                if (0 == resOrdre.Length)
                {
                    return null;
                }
                 */
                //On cherche l'ordre actuellement actif s'il existe, on vérifie qu'il s'agit bien d'un ordre de mouvement
                string requete = string.Format("(ID_PION={0}) AND (I_TOUR_FIN = {1}) AND (I_PHASE_FIN = {1})", ID_PION, Constantes.NULLENTIER);
                Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                TAB_ORDRERow[] resOrdre = (TAB_ORDRERow[])Select(requete, tri);
                if (0 != resOrdre.Length)
                {
                    if ((resOrdre[0].I_ORDRE_TYPE == Constantes.ORDRES.MOUVEMENT) ||
                        (resOrdre[0].I_ORDRE_TYPE == Constantes.ORDRES.MESSAGE) ||
                        (resOrdre[0].I_ORDRE_TYPE == Constantes.ORDRES.SUIVRE_UNITE) ||
                        (resOrdre[0].I_ORDRE_TYPE == Constantes.ORDRES.PATROUILLE))
                    {
                        ligneOrdreRetour = resOrdre[0];
                    }
                }
                Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                return ligneOrdreRetour;
            }

            /// <summary>
            ///  Renvoi le dernier ordre de fortification affecté à l'unité, null si aucun
            /// </summary>
            /// <param name="ID_PION">Pion avec l'ordre</param>
            /// <returns>ordre de mouvement du pion, null si aucun</returns>
            public TAB_ORDRERow SeFortifier(int ID_PION)
            {
                TAB_ORDRERow ligneOrdreRetour = null;
                string tri = "ID_ORDRE";
                string requete = string.Format("(ID_PION={0}) AND (I_ORDRE_TYPE={1})", ID_PION, Constantes.ORDRES.SEFORTIFIER);
                Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                TAB_ORDRERow[] resOrdre = (TAB_ORDRERow[])Select(requete, tri);
                if (0 != resOrdre.Length)
                {
                    ligneOrdreRetour = resOrdre[0];
                }
                Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                return ligneOrdreRetour;
            }

            /// <summary>
            ///  Renvoi l'ordre courant affecté à l'unité, null si aucun
            /// </summary>
            /// <param name="ID_PION">identifiant du pion sur lequel on cherche l'ordre</param>
            /// <returns>ordre courant du pion, null si aucun</returns>
            public TAB_ORDRERow Courant(int ID_PION)
            {
                TAB_ORDRERow ligneOrdreRetour = null;
                string requete = string.Format("ID_PION={0} AND I_TOUR_FIN = {1} AND I_PHASE_FIN = {1}", ID_PION, Constantes.NULLENTIER);
                string tri = "ID_ORDRE";
                Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                TAB_ORDRERow[] resOrdre = (TAB_ORDRERow[])Select(requete, tri);
                if (0 != resOrdre.Length)
                {
                    ligneOrdreRetour = resOrdre[0];
                }
                Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                return ligneOrdreRetour;
            }

            /// <summary>
            ///  Renvoi le premier ordre affecté à l'unité, null si aucun
            /// </summary>
            /// <param name="ID_PION">identifiant du pion sur lequel on cherche l'ordre</param>
            /// <returns>ordre courant du pion, null si aucun</returns>
            public TAB_ORDRERow Premier(int ID_PION)
            {
                TAB_ORDRERow ligneOrdreRetour = null;
                string requete = string.Format("ID_PION={0}", ID_PION);
                string tri = "ID_ORDRE";
                Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                TAB_ORDRERow[] resOrdre = (TAB_ORDRERow[])Select(requete, tri);
                if (0 != resOrdre.Length)
                {
                    ligneOrdreRetour = resOrdre[0];
                }
                Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                return ligneOrdreRetour;
            }
        }

        partial class TAB_ORDRERow
        {

            public bool IsI_TOUR_FINNull() { return (Constantes.NULLENTIER == this.I_TOUR_FIN); }
            public bool IsI_PHASE_FINNull() { return (Constantes.NULLENTIER == this.I_PHASE_FIN); }
            public bool IsI_HEURE_DEBUTNull() { return (Constantes.NULLENTIER == this.I_HEURE_DEBUT); }
            public bool IsI_DUREENull() { return (Constantes.NULLENTIER == this.I_DUREE); }
            public bool IsID_ORDRE_SUIVANTNull() { return (Constantes.NULLENTIER == this.ID_ORDRE_SUIVANT); }
            public bool IsID_CASE_DESTINATIONNull() { return (Constantes.NULLENTIER == this.ID_CASE_DESTINATION); }
            public bool IsID_CIBLENull() { return (Constantes.NULLENTIER == this.ID_CIBLE); }
            public bool IsID_ORDRE_TRANSMISNull() { return (Constantes.NULLENTIER == this.ID_ORDRE_TRANSMIS); }
            public bool IsID_PIONNull() { return (Constantes.NULLENTIER == this.ID_PION); }
            public bool IsID_MESSAGENull() { return (Constantes.NULLENTIER == this.ID_MESSAGE); }
            public bool IsID_DESTINATAIRENull() { return (Constantes.NULLENTIER == this.ID_DESTINATAIRE); }

            /// <summary>
            /// renvoie l'ordre suivant de l'ordre courant
            /// </summary>
            public TAB_ORDRERow ordreSuivant
            {
                get
                {
                    if (this.IsID_ORDRE_SUIVANTNull() || this.ID_ORDRE_SUIVANT < 0) { return null; }
                    return m_donnees.TAB_ORDRE.FindByID_ORDRE(this.ID_ORDRE_SUIVANT);
                }
            }
            internal TAB_ORDRERow EstOrdreSuivant()
            {
                TAB_ORDRERow ligneOrdreRetour = null;
                string requete = string.Format("ID_ORDRE_SUIVANT={0}", ID_ORDRE);
                Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                TAB_ORDRERow[] resOrdre = (TAB_ORDRERow[])Donnees.m_donnees.TAB_ORDRE.Select(requete);
                if (0 != resOrdre.Length)
                {
                    ligneOrdreRetour = resOrdre[0];
                }
                Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                return ligneOrdreRetour;
            }

            public TAB_PIONRow cible
            {
                get
                {
                    if (this.IsID_CIBLENull()) { return null; }
                    return m_donnees.TAB_PION.FindByID_PION(ID_CIBLE);
                }
            }

            /// <summary>
            /// renvoie l'ordre transmis par une unité (de messager)
            /// </summary>
            public TAB_ORDRERow ordreTransmis
            {
                get
                {
                    if (this.IsID_ORDRE_TRANSMISNull()) { return null; }
                    return m_donnees.TAB_ORDRE.FindByID_ORDRE(this.ID_ORDRE_TRANSMIS);
                }
            }

            /// <summary>
            /// renvoie le message transmis par une unité (de messager)
            /// </summary>
            public TAB_MESSAGERow messageTransmis
            {
                get
                {
                    if (this.IsID_MESSAGENull()) { return null; }
                    return m_donnees.TAB_MESSAGE.FindByID_MESSAGE(this.ID_MESSAGE);
                }
            }

            internal void SetID_CIBLENull()
            {
                this.ID_CIBLE = Constantes.NULLENTIER;
            }

            internal void SetID_DESTINATAIRE_CIBLENull()
            {
                this.ID_DESTINATAIRE_CIBLE = Constantes.NULLENTIER; ;
            }

            internal void SetID_NOM_DESTINATIONNull()
            {
                this.ID_NOM_DESTINATION = Constantes.NULLENTIER;
            }

            internal void SetID_ORDRE_TRANSMISNull()
            {
                this.ID_ORDRE_TRANSMIS = Constantes.NULLENTIER;
            }

            internal void SetID_ORDRE_SUIVANTNull()
            {
                this.ID_ORDRE_SUIVANT = Constantes.NULLENTIER;
            }

            internal void SetI_ENGAGEMENTNull()
            {
                this.I_ENGAGEMENT = Constantes.NULLENTIER;
            }

            internal void SetI_TOUR_FINNull()
            {
                this.I_TOUR_FIN = Constantes.NULLENTIER;
            }
            internal void SetI_PHASE_FINNull()
            {
                this.I_PHASE_FIN = Constantes.NULLENTIER;
            }
        }

        partial class TAB_MODELE_PIONDataTable
        {
            /// <summary>
            /// Recherche un modele de pion dans la table associé pour une nation donnée
            /// </summary>
            /// <param name="idNation">Identifiant de la nation du modele</param>
            /// <param name="nomModele">nom du modèle</param>
            /// <returns>-1 si aucun modèle, ID_MODELE_PION sinon </returns>
            public int RechercherModele(int idNationSource, string nomModele)
            {
                //recherche du modele
                var result = from Modele in this
                             join AptitudesPion in Donnees.m_donnees.TAB_APTITUDES_PION
                             on new { Modele.ID_MODELE_PION, idNation = Modele.ID_NATION } equals new { AptitudesPion.ID_MODELE_PION, idNation = idNationSource }
                             join Aptitudes in Donnees.m_donnees.TAB_APTITUDES
                             on new { AptitudesPion.ID_APTITUDE, stipe = nomModele } equals new { Aptitudes.ID_APTITUDE, stipe = Aptitudes.S_NOM }
                             select Modele.ID_MODELE_PION;

                if (0 == result.Count())
                {
                    return -1;
                }

                return result.ElementAt(0);
            }
        }

        partial class TAB_MODELE_PIONRow
        {
            /// <summary>
            /// Renvoie le coût d'une case pour un modèle de terrain particulier pour le modèle de mouvent courant et la météo courante
            /// </summary>
            /// <param name="ID_MODELE_TERRAIN">modèle de terrain</param>
            /// <returns>cout du terrain</returns>
            public int CoutCase(int ID_MODELE_TERRAIN)
            {
                string requete = string.Format("ID_MODELE_MOUVEMENT={0} AND ID_METEO={1} AND ID_MODELE_TERRAIN={2}",
                        this.ID_MODELE_MOUVEMENT,
                        Donnees.m_donnees.TAB_PARTIE[0].ID_METEO,
                        ID_MODELE_TERRAIN);
                Monitor.Enter(Donnees.m_donnees.TAB_MOUVEMENT_COUT.Rows.SyncRoot);
                Donnees.TAB_MOUVEMENT_COUTRow[] resCout = (Donnees.TAB_MOUVEMENT_COUTRow[])Donnees.m_donnees.TAB_MOUVEMENT_COUT.Select(requete);
                Monitor.Exit(Donnees.m_donnees.TAB_MOUVEMENT_COUT.Rows.SyncRoot);
                return resCout[0].I_COUT;
            }

            /// <summary>
            /// Nom du modèle pour afficher dans une liste (combo, dans la liste des noms villes)
            /// </summary>
            public string nomListe
            {
                get { return string.Format("{0}:{1}", ID_MODELE_PION, S_NOM); }
            }
        }

        partial class TAB_MODELE_MOUVEMENTRow
        {
            /// <summary>
            /// Renvoie le coût d'une case pour un modèle de terrain particulier pour le modèle de mouvent courant et la météo courante
            /// </summary>
            /// <param name="ID_MODELE_TERRAIN">modèle de terrain</param>
            /// <returns>cout du terrain</returns>
            public int CoutCase(int ID_MODELE_TERRAIN)
            {
                string requete = string.Format("ID_MODELE_MOUVEMENT={0} AND ID_METEO={1} AND ID_MODELE_TERRAIN={2}",
                        this.ID_MODELE_MOUVEMENT,
                        Donnees.m_donnees.TAB_PARTIE[0].ID_METEO,
                        ID_MODELE_TERRAIN);
                Monitor.Enter(Donnees.m_donnees.TAB_MOUVEMENT_COUT.Rows.SyncRoot);
                Donnees.TAB_MOUVEMENT_COUTRow[] resCout = (Donnees.TAB_MOUVEMENT_COUTRow[])Donnees.m_donnees.TAB_MOUVEMENT_COUT.Select(requete);
                Monitor.Exit(Donnees.m_donnees.TAB_MOUVEMENT_COUT.Rows.SyncRoot);
                return resCout[0].I_COUT;
            }
        }

        partial class TAB_MODELE_MOUVEMENTDataTable
        {
            /// <summary>
            /// calcul de la vitesse minimale de tous les modèles définis
            /// </summary>
            /// <returns>la vitesse minimale définie, -1 si non trouvé</returns>
            public decimal VitesseMinimale()
            {
                decimal vitesse = decimal.MaxValue;
                foreach (TAB_MODELE_MOUVEMENTRow ligneModeleMouvement in Rows)
                {
                    if (ligneModeleMouvement.I_VITESSE_CAVALERIE > 0)
                    {
                        vitesse = Math.Min(vitesse, ligneModeleMouvement.I_VITESSE_CAVALERIE);
                    }
                    if (ligneModeleMouvement.I_VITESSE_INFANTERIE > 0)
                    {
                        vitesse = Math.Min(vitesse, ligneModeleMouvement.I_VITESSE_INFANTERIE);
                    }
                    if (ligneModeleMouvement.I_VITESSE_ARTILLERIE > 0 && vitesse == decimal.MaxValue)
                    {
                        //uniquement pour les unités d'artillerie pure
                        vitesse = Math.Min(vitesse, ligneModeleMouvement.I_VITESSE_ARTILLERIE);
                    }
                }
                if (vitesse == int.MaxValue)
                {
                    vitesse = 1;//vitesse d'un convoi
                }
                return vitesse;
            }

            /// <summary>
            /// calcul de la vitesse maximale de tous les modèles définis
            /// </summary>
            /// <returns>la vitesse maximale définie, -1 si non trouvé</returns>
            public decimal VitesseMaximale()
            {
                decimal vitesse = 1;//vitesse d'un convoi
                foreach (TAB_MODELE_MOUVEMENTRow ligneModeleMouvement in Rows)
                {
                    if (ligneModeleMouvement.I_VITESSE_CAVALERIE > 0)
                    {
                        vitesse = Math.Max(vitesse, ligneModeleMouvement.I_VITESSE_CAVALERIE);
                    }
                    if (ligneModeleMouvement.I_VITESSE_INFANTERIE > 0)
                    {
                        vitesse = Math.Max(vitesse, ligneModeleMouvement.I_VITESSE_INFANTERIE);
                    }
                    if (ligneModeleMouvement.I_VITESSE_ARTILLERIE > 0)
                    {
                        vitesse = Math.Max(vitesse, ligneModeleMouvement.I_VITESSE_ARTILLERIE);
                    }
                }
                return vitesse;
            }
        }

        partial class TAB_MOUVEMENT_COUTDataTable
        {
            public int CalculCout(long idMeteo, long idModeleTerrain, long idModeleMouvement)
            {
                string requete = string.Format("ID_MODELE_MOUVEMENT={0} AND ID_MODELE_TERRAIN={1} AND ID_METEO={2}",
                    idModeleMouvement, idModeleTerrain, idMeteo);
                Monitor.Enter(Donnees.m_donnees.TAB_MOUVEMENT_COUT.Rows.SyncRoot);
                TAB_MOUVEMENT_COUTRow[] resCout = (TAB_MOUVEMENT_COUTRow[])Select(requete);
                Monitor.Exit(Donnees.m_donnees.TAB_MOUVEMENT_COUT.Rows.SyncRoot);
                if (0 == resCout.Length)
                {
                    return -1;
                }
                if (resCout.Length > 1)
                {
                    string message = string.Format("DonneCout trouve {0} valeurs pour ID_MODELE_MOUVEMENT={1} et ID_MODELE_TERRAIN={2} et ID_METEO={3}", resCout.Length, idModeleMouvement, idModeleTerrain, idMeteo);
                    throw new Exception(message);
                }
                return resCout[0].I_COUT;
            }

            /// <summary>
            /// calcul de la case avec le cout minimum
            /// </summary>
            /// <returns>cout minimum, -1 si non trouvé</returns>
            public int CoutMinimum()
            {
                int cout = Constantes.CST_COUTMAX;
                foreach (TAB_MOUVEMENT_COUTRow ligneMouvementCout in Rows)
                {
                    if (ligneMouvementCout.I_COUT > 0)
                    {
                        cout = Math.Min(cout, ligneMouvementCout.I_COUT);
                    }
                }
                if (cout == Constantes.CST_COUTMAX)
                {
                    cout = -1;
                }
                return cout;
            }
        }

        partial class TAB_CASEDataTable
        {
            private Array m_listeIndex = null; //renvoie l'index case à partir d'un x,y : optimisation mémoire
            private List<int> m_listeCasesCoutNonMax = new List<int>(); //renvoie l'index case dont le cout ne vaut pas CST_COUTMAX, optimisation pour AStar.SearchSpace

            public void AjouterCaseCoutNonMax(Donnees.TAB_CASERow ligne)
            {
                m_listeCasesCoutNonMax.Add((int)m_listeIndex.GetValue(ligne.I_X, ligne.I_Y));
            }

            public void InitialisationListeCaseNonCoutMax()
            {
                m_listeCasesCoutNonMax.Clear();
                //foreach (Donnees.TAB_CASERow ligne in this)
                //{
                //    ligne.I_COUT = Constantes.CST_COUTMAX;
                //}

                string requete = string.Format("I_COUT<>{0}", Constantes.CST_COUTMAX);
                Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                Donnees.TAB_CASERow[] changeRows = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete);
                for (int l = 0; l < changeRows.Count(); l++)
                {
                    Donnees.TAB_CASERow ligne = changeRows[l];
                    ligne.I_COUT = Constantes.CST_COUTMAX;
                }
                Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
            }

            public void ReinitialisationListeCasesNonCoutMax()
            {
                foreach (int index in m_listeCasesCoutNonMax)
                {
                    Donnees.TAB_CASERow ligne = this[index];
                    ligne.I_COUT = Constantes.CST_COUTMAX;
                }
                m_listeCasesCoutNonMax.Clear();
            }

            public Donnees.TAB_CASERow[] ListeCasesNonCoutMax()
            {
                Donnees.TAB_CASERow[] listeCases = new TAB_CASERow[m_listeCasesCoutNonMax.Count];
                int i = 0;
                foreach (int index in m_listeCasesCoutNonMax)
                {
                    listeCases[i++] = this[index];
                }
                return listeCases;
            }

            public bool InitialisationListeCase(int maxX, int maxY)
            {
                m_listeIndex = Array.CreateInstance(typeof(int), maxX, maxY);

                for (int x = 0; x < maxX; x++) for (int y = 0; y < maxY; y++) { m_listeIndex.SetValue(-1, x, y); }

                for (int i = 0; i < this.Count; i++)
                {
                    TAB_CASERow ligneCase = this[i];
                    if (ligneCase.I_X < maxX && ligneCase.I_Y < maxY)
                    {
                        m_listeIndex.SetValue(i, ligneCase.I_X, ligneCase.I_Y);
                    }
                }
                return true;
            }

            /// <summary>
            /// renvoie une case d'après son lieu
            /// </summary>
            /// <param name="id_nom">identifiant du nom</param>
            /// <returns>case trouvée, null si la case n'existe pas</returns>
            public TAB_CASERow FindByID_NOM(int id_nom)
            {
                TAB_NOMS_CARTERow ligneNom = m_donnees.TAB_NOMS_CARTE.FindByID_NOM(id_nom);
                if (null == ligneNom) return null;
                return FindParID_CASE(ligneNom.ID_CASE);
            }

            public bool estCaseChargee(int x, int y)
            {
                int index = (int)m_listeIndex.GetValue(x, y);
                if (index < 0)
                {
                    return false;
                }
                return true;
            }

            /// <summary>
            /// renvoie une case d'après ses coordonnées
            /// </summary>
            /// <param name="x">abscisse</param>
            /// <param name="y">ordonnée</param>
            /// <returns>case trouvée, null si la case n'existe pas</returns>
            public TAB_CASERow FindParXY(int x, int y)
            {
                int index;
                try
                {
                    if (null == m_listeIndex)
                    {
                        string requete = string.Format("I_X={0} AND I_Y={1}", x, y);
                        Monitor.Enter(m_donnees.TAB_CASE.Rows.SyncRoot);
                        TAB_CASERow[] resCase = (TAB_CASERow[])Select(requete);
                        Monitor.Exit(m_donnees.TAB_CASE.Rows.SyncRoot);
                        if (0 == resCase.Length)
                        {
                            return null;
                        }
                        if (resCase.Length > 1)
                        {
                            string message = string.Format("TrouveCase trouve {0} valeurs pour x={1} et y={2}", resCase.Length, x, y);
                            throw new Exception(message);
                        }

                        return resCase[0];
                    }
                    else
                    {
                        index = (int)m_listeIndex.GetValue(x, y);
                        if (index < 0)
                        {
                            if (!ChargerCases(x, y, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR_CASES, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE_CASES))
                            {
                                string message = string.Format("TrouveCase m_donnees.ChargerCases ne trouve pas de valeurs pour x={0} et y={1}", x, y);
                                throw new Exception(message);
                            }
                            index = (int)m_listeIndex.GetValue(x, y);
                        }
                        Monitor.Enter(this.Rows.SyncRoot);//l'intêret du lock semble pas évident mais il y a des crashs bizarres sinon, des fois il ne trouve plus la valeur d'une colonne
                        TAB_CASERow retourLigne = this[index];
                        Monitor.Exit(this.Rows.SyncRoot);
                        return retourLigne;
                    }
                }
                catch (Exception ex)
                {
                    string message = string.Format("Erreur sur TABCASEDataTables.FindParXY tombe en exception x={0} et y={1} : {2}, {3}",
                        x, y, ex.Message, ex.StackTrace);
                    MessageBox.Show(message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LogFile.Notifier(message);
                    //throw new Exception(message); //peut visiblement arriver sur certains mouvements de souris, mieux vaut que cela ne crash pas tout
                    return null;//pas sur que ce soit bien géré dérrière...
                }
            }

            private string nomRepertoireCases(int tour, int phase)
            {
                return string.Format("{0}cases\\{1:D4}_{2:D3}\\", Constantes.repertoireDonnees, tour, phase);
            }

            private string NomFichierCases(int x, int y, string repertoire)
            {
                string nomfichier = string.Format("{0}{1:D5}_{2:D5}.cases",
                    repertoire, (x / Constantes.CST_TAILLE_BLOC_CASES) * Constantes.CST_TAILLE_BLOC_CASES,
                                (y / Constantes.CST_TAILLE_BLOC_CASES) * Constantes.CST_TAILLE_BLOC_CASES);
                return nomfichier;
            }

            internal bool SauvegarderCases(Donnees.TAB_CASEDataTable baseSource, int x, int y, int tour, int phase)
            {
                Cursor oldCursor = Cursor.Current;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    string repertoire = nomRepertoireCases(tour, phase);
                    if (!Directory.Exists(repertoire))
                    {
                        Directory.CreateDirectory(repertoire);
                    }
                    string nomfichier = NomFichierCases(x, y, repertoire);

                    if (File.Exists(nomfichier))
                    {
                        File.Delete(nomfichier);
                    }
                    ZipArchive fichierZip = ZipFile.Open(nomfichier, ZipArchiveMode.Create);
                    ZipArchiveEntry fichier = fichierZip.CreateEntry(nomfichier);
                    StreamWriter ecrivain = new StreamWriter(fichier.Open());
                    baseSource.WriteXml(ecrivain);
                    ecrivain.Close();
                    fichierZip.Dispose();
                    Cursor.Current = oldCursor;
                }
                catch (Exception e)
                {
                    Cursor.Current = oldCursor;
                    MessageBox.Show("Erreur sur BaseVaoc.SauvegarderCases :" + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                return true;
            }

            public int XY_Vers_ID_CASE(int x, int y)
            {
                return x * m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE + y;
            }

            public void ID_CASE_Vers_XY(int idcase, out int x, out int y)
            {
                y = idcase % m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE;
                x = (idcase - y) / m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE;
            }

            public TAB_CASERow FindParID_CASE(int ID_CASE)
            {
                TAB_CASERow retour;
                retour = FindByID_CASE(ID_CASE);
                //((TAB_CASERow)(this.Rows.Find(new object[] {ID_CASE})));
                if (null == retour)
                {
                    int x, y;
                    m_donnees.TAB_CASE.ID_CASE_Vers_XY(ID_CASE, out x, out y);
                    ChargerCases(x, y, m_donnees.TAB_PARTIE[0].I_TOUR_CASES, m_donnees.TAB_PARTIE[0].I_PHASE_CASES);
                    //retour = ((TAB_CASERow)(this.Rows.Find(new object[] {ID_CASE})));
                    retour = FindByID_CASE(ID_CASE);
                }
                return retour;
            }

            internal bool ChargerDonnneesCases(ref Donnees.TAB_CASEDataTable donneesSource, int x, int y, int tour, int phase)
            {
                try
                {
                    string repertoire = nomRepertoireCases(tour, phase);
                    string nomfichier = NomFichierCases(x, y, repertoire);

                    //on recherche le dernier fichier sauvegardé sur les cases
                    /*** plus maintenant, le fichier doit toujours être là ! -> hum, je ne vois pas pourquoi j'ai mis ce qui suit en commentaire, en tout cas pour formmajcases, cela ne marche pas ***/
                    int phaserecherche = 0; // phase;
                    int tourrecherche = tour;
                    while (!File.Exists(nomfichier) && tourrecherche >= 0)
                    {
                        tourrecherche--;
                        repertoire = nomRepertoireCases(tourrecherche, phaserecherche);
                        nomfichier = NomFichierCases(x, y, repertoire);
                        Debug.WriteLine("ChargerCases, test sur le fichier " + nomfichier);
                    }

                    if (tourrecherche < 0)
                    {
                        string messageErreur = string.Format("Erreur sur ChargerDonnneesCases : Impossible de trouver un fichiers de cases pour x={0}, y={1}, tour={2}, phase={3}",
                            x, y, tour, phase);
                        LogFile.Notifier(messageErreur);
                        MessageBox.Show(messageErreur
                            , "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    /*********************************/

                    ZipArchive fichierZip = ZipFile.OpenRead(nomfichier);
                    ZipArchiveEntry fichier = fichierZip.Entries[0];
                    donneesSource.ReadXml(fichier.Open());//m_donnees.TAB_CASE.ReadXml(fichier.Open()); ne marche pas mais je ne sais pas pourquoi !
                    fichierZip.Dispose();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Erreur sur TABCASEDataTables.ChargerDonnneesCases :" + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                return true;
            }

            internal bool ChargerCases(int x, int y, int tour, int phase)
            {
                Cursor oldCursor = Cursor.Current;
                Donnees.TAB_CASEDataTable donneesSource = new TAB_CASEDataTable();
                string repertoire, nomfichier;
                int i, debutNouvellesLignes;
                TAB_CASERow ligneCase;

                try
                {
                    LogFile.Notifier(string.Format("ChargerCases x={0}, y={1}", x, y));
                    Cursor.Current = Cursors.WaitCursor;
                    phase = 0; //en attendant mieux !
                    repertoire = nomRepertoireCases(tour, phase);
                    nomfichier = NomFichierCases(x, y, repertoire);

                    //on recherche le dernier fichier sauvegardé sur les cases
                    int phaserecherche = 0; // phase;
                    int tourrecherche = tour;
                    while (!File.Exists(nomfichier) && tourrecherche >= 0)
                    {
                        //phaserecherche -= Constantes.CST_SAUVEGARDE_ECART_PHASES;
                        //if (phaserecherche < 0)
                        //{
                        //    phaserecherche = m_donnees.TAB_JEU[0].I_NOMBRE_PHASES;
                        //    tourrecherche--;
                        //}
                        tourrecherche--;
                        repertoire = nomRepertoireCases(tourrecherche, phaserecherche);
                        nomfichier = NomFichierCases(x, y, repertoire);
                        Debug.WriteLine("ChargerCases, test sur le fichier " + nomfichier);
                    }

                    if (tourrecherche < 0)
                    {
                        Cursor.Current = oldCursor;
                        string messageErreur = string.Format("Erreur sur ChargerCases : Impossible de trouver un fichiers de cases pour x={0}, y={1}, tour={2}, phase={3}",
                            x, y, tour, phase);
                        LogFile.Notifier(messageErreur);
                        MessageBox.Show(messageErreur
                            , "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    ZipArchive fichierZip = ZipFile.OpenRead(nomfichier);
                    ZipArchiveEntry fichier = fichierZip.Entries[0];
                    donneesSource.ReadXml(fichier.Open());
                    fichierZip.Dispose();

                    DateTime timeStart;
                    TimeSpan perf;
                    Monitor.Enter(this.Rows.SyncRoot);
                    //importrow duplicate les cases sans vérifier aucune contrainte.
                    timeStart = DateTime.Now;
                    debutNouvellesLignes = this.Count;
                    //Donnees.m_donnees.TAB_CASE.Merge(donneesSource, false); -> prends beaucoup trop de temps, >50 secondes quand la table est déjà très chargée
                    //Debug.WriteLine(string.Format("ChargerCases x={0}, y={1}", x, y));
                    foreach (Donnees.TAB_CASERow ligneCasePlus in donneesSource)
                    {
                        //si je retrouve la même ligne avant l'intertion,je sors ! C'est possible si deux process differents veulent voir en même temps une case non chargée
                        if ((int)m_listeIndex.GetValue(ligneCasePlus.I_X, ligneCasePlus.I_Y) > 0)
                        {
                            Monitor.Exit(this.Rows.SyncRoot);
                            return true;
                        }
                        this.ImportRow(ligneCasePlus);
                        //Donnees.m_donnees.TAB_CASE.AddTAB_CASERow(
                        //    ligneCasePlus.ID_CASE,
                        //    ligneCasePlus.ID_MODELE_TERRAIN,
                        //    ligneCasePlus.I_X,
                        //    ligneCasePlus.I_Y,
                        //    ligneCasePlus.ID_PROPRIETAIRE,
                        //    ligneCasePlus.ID_NOUVEAU_PROPRIETAIRE,
                        //    ligneCasePlus.ID_MODELE_TERRAIN_SI_OCCUPE,
                        //    ligneCasePlus.I_COUT);
                    }
                    perf = DateTime.Now - timeStart;
                    //Debug.WriteLine(string.Format("Donnees.m_donnees.TAB_CASE.Merge en {0} heures, {1} minutes, {2} secondes, {3} millisecondes :{4},{5}", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds, x, y));
                    //mise à jour de l'index
                    timeStart = DateTime.Now;
                    for (i = debutNouvellesLignes; i < this.Count; i++)
                    {
                        ligneCase = this[i];// si je ne le fais que sur donnees source, je ne peux pas garantir que l'ordre est respecté par le merge, mais plus je charge plus c'est long
                        m_listeIndex.SetValue(i, ligneCase.I_X, ligneCase.I_Y);
                    }
                    Monitor.Exit(this.Rows.SyncRoot);
                    perf = DateTime.Now - timeStart;
                    //Debug.WriteLine(string.Format("mise à jour de l'index en {0} heures, {1} minutes, {2} secondes, {3} millisecondes :{4},{5}", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds, x, y));
                    Cursor.Current = oldCursor;
                }
                catch (Exception ex)
                {
                    Cursor.Current = oldCursor;
                    string messageErreur = "Erreur sur TABCASEDataTables.ChargerCases :" + ex.Message;
                    LogFile.Notifier(messageErreur);
                    MessageBox.Show(messageErreur, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw;//tres grave en fait
                }
                return true;
            }

            /*
            public bool ChargerCases(int x, int y, int tour, int phase)
            {
                Cursor oldCursor = Cursor.Current;
                Don donneesSource;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    if (!Dal.ChargerCases(x, y, tour, phase, out donneesSource))
                    {
                        return false;
                    }
                    // Ne marche pas erreur : <target>.ID_CASE and <source>.ID_CASE have conflicting properties: DataType property mismatch.
                    //Donnees.TAB_CASEDataTable tableCases = ConvertToTypedDataTable<Donnees.TAB_CASEDataTable>(donneesSource.Tables["TAB_CASE"]);
                    m_donnees.TAB_CASE.Merge(((Donnees)donneesSource).TAB_CASE, false);
                    //

                    //mise à jour de l'index
                    for (int i = 0; i < donneesSource.Tables["TAB_CASE"].Rows.Count; i++)
                    {
                        //TAB_CASERow ligneCase = (TAB_CASERow)donneesSource.Tables["TAB_CASE"].Rows[i];
                        DataRow ligneCaseSource = donneesSource.Tables["TAB_CASE"].Rows[i];
                        TAB_CASERow ligneCase = m_donnees.TAB_CASE.AddTAB_CASERow(
                            ligneCaseSource["ID_CASE"], 
                            ligneCaseSource["ID_MODELE_TERRAIN"], 
                            ligneCaseSource["I_X"], 
                            ligneCaseSource["I_Y"],
                            string.Empty == ligneCaseSource["ID_PROPRIETAIRE"] ? -1 : ligneCaseSource["ID_PROPRIETAIRE"],
                            string.Empty == ligneCaseSource["ID_NOUVEAU_PROPRIETAIRE"] ? -1 : ligneCaseSource["ID_NOUVEAU_PROPRIETAIRE"],
                            ligneCaseSource["ID_MODELE_TERRAIN_SI_OCCUPE"],
                            string.Empty == ligneCaseSource["I_COUT"] ? -1 : ligneCaseSource["I_COUT"]);

                        if (string.Empty == ligneCaseSource["ID_PROPRIETAIRE"]) ligneCase.SetID_PROPRIETAIRENull();
                        if (string.Empty == ligneCaseSource["ID_NOUVEAU_PROPRIETAIRE"]) ligneCase.SetID_NOUVEAU_PROPRIETAIRENull();
                        if (string.Empty == ligneCaseSource["I_COUT"]) ligneCase.SetI_COUTNull();
                        m_listeIndex.SetValue(i, ligneCase.I_X, ligneCase.I_Y);
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Cursor.Current = oldCursor;
                    MessageBox.Show("Erreur sur Dal.ChargerCases :" + e.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
*/
            public TAB_CASERow[] CasesCadre(int xCaseHautGauche, int yCaseHautGauche, int xCaseBasDroite, int yCaseBasDroite)
            {
                //note, je ne vérifie pas que les coordonnées du cadre sont bien entre 0 et largeur/hauteur, je considère que cela a été fait dans le calcul de ces valeurs
                int nb = 0;
                TAB_CASERow[] resCase;
                resCase = new TAB_CASERow[(xCaseBasDroite - xCaseHautGauche + 1) * (yCaseBasDroite - yCaseHautGauche + 1)];
                for (int x = xCaseHautGauche; x <= xCaseBasDroite; x++)
                {
                    for (int y = yCaseHautGauche; y <= yCaseBasDroite; y++)
                    {
                        resCase[nb++] = FindParXY(x, y);
                    }
                }

                /* Note : La méthode ci-dessous est clairement plus rapide sur une petite carte (testée sur une carte 1152x648, à vérifier sur une grande carte
                requete = string.Format("I_X>={0} AND I_Y>={1} AND I_X<={2} AND I_Y<={3}", xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite);
                Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                Donnees.TAB_CASERow[] resCase = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete);
                Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                */

                return resCase;
            }

            public TAB_CASERow[] CasesVoisines(TAB_CASERow source)
            {
                int largeur = m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1;
                int hauteur = m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1;
                TAB_CASERow[] resCase;

                //essai d'optimisation mais qui fait que, dans certains cas, on ne trouve pas le résultat, c'est dommage
                //if (Math.Abs(source.I_X - cible.I_X) > CST_ESPACE_DE_RECHERCHE || Math.Abs(source.I_Y - cible.I_Y) > CST_ESPACE_DE_RECHERCHE)
                //{
                //    return new TAB_CASERow[0];//aucun voisin fourni, c'est trop loin
                //}

                //Requête qui fonctionne mais qui met plus d'une seconde en execution 
                /*
                requete = string.Format("I_X-{0}<=1 AND I_Y-{1}<=1 AND I_X-{0}>=-1 AND I_Y-{1}>=-1 AND (I_X<>{0} OR I_Y<>{1})", 
                    source.I_X, source.I_Y);
                resCase = (TAB_CASERow[])Select(requete);
                */
                int[] x = new int[8];
                int[] y = new int[8];
                int nb_points = 0;
                if (source.I_X > 0 && source.I_Y > 0)
                {
                    x[nb_points] = source.I_X - 1;
                    y[nb_points++] = source.I_Y - 1;
                }
                if (source.I_X > 0 && source.I_Y < hauteur)
                {
                    x[nb_points] = source.I_X - 1;
                    y[nb_points++] = source.I_Y + 1;
                }
                if (source.I_X < largeur && source.I_Y < hauteur)
                {
                    x[nb_points] = source.I_X + 1;
                    y[nb_points++] = source.I_Y + 1;
                }
                if (source.I_X < largeur && source.I_Y > 0)
                {
                    x[nb_points] = source.I_X + 1;
                    y[nb_points++] = source.I_Y - 1;
                }
                if (source.I_X > 0)
                {
                    x[nb_points] = source.I_X - 1;
                    y[nb_points++] = source.I_Y;
                }
                if (source.I_X < largeur)
                {
                    x[nb_points] = source.I_X + 1;
                    y[nb_points++] = source.I_Y;
                }
                if (source.I_Y > 0)
                {
                    x[nb_points] = source.I_X;
                    y[nb_points++] = source.I_Y - 1;
                }
                if (source.I_Y < hauteur)
                {
                    x[nb_points] = source.I_X;
                    y[nb_points++] = source.I_Y + 1;
                }
                resCase = new TAB_CASERow[nb_points];
                for (int i = 0; i < nb_points; i++)
                {
                    resCase[i] = FindParXY(x[i], y[i]);
                }

                if (0 == resCase.Length)
                {
                    string message = string.Format("CasesVoisines ne trouve aucun voisin pour x={0} et y={1}", source.I_X, source.I_Y);
                    throw new Exception(message);
                }

                //suppression de la case équivalente à la source
                //TAB_CASEDataTable retourTable=new TAB_CASEDataTable();
                //foreach (TAB_CASERow ligne in resCase)
                //{
                //    retourTable.ImportRow(ligne);
                //}
                //requete = string.Format("I_X<>{0} AND I_Y<>{1}", source.I_X, source.I_Y);
                //resCase = (TAB_CASERow[])retourTable.Select(requete);
                return resCase;
            }

            public Node[] CasesVoisines(Node source)
            {
                try
                {
                    int largeur = m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1;
                    int hauteur = m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1;
                    Node[] resCase;

                    int[] x = new int[8];
                    int[] y = new int[8];
                    int nb_points = 0;
                    if (source.I_X > 0 && source.I_Y > 0)
                    {
                        x[nb_points] = source.I_X - 1;
                        y[nb_points++] = source.I_Y - 1;
                    }
                    if (source.I_X > 0 && source.I_Y < hauteur)
                    {
                        x[nb_points] = source.I_X - 1;
                        y[nb_points++] = source.I_Y + 1;
                    }
                    if (source.I_X < largeur && source.I_Y < hauteur)
                    {
                        x[nb_points] = source.I_X + 1;
                        y[nb_points++] = source.I_Y + 1;
                    }
                    if (source.I_X < largeur && source.I_Y > 0)
                    {
                        x[nb_points] = source.I_X + 1;
                        y[nb_points++] = source.I_Y - 1;
                    }
                    if (source.I_X > 0)
                    {
                        x[nb_points] = source.I_X - 1;
                        y[nb_points++] = source.I_Y;
                    }
                    if (source.I_X < largeur)
                    {
                        x[nb_points] = source.I_X + 1;
                        y[nb_points++] = source.I_Y;
                    }
                    if (source.I_Y > 0)
                    {
                        x[nb_points] = source.I_X;
                        y[nb_points++] = source.I_Y - 1;
                    }
                    if (source.I_Y < hauteur)
                    {
                        x[nb_points] = source.I_X;
                        y[nb_points++] = source.I_Y + 1;
                    }
                    resCase = new Node[nb_points];
                    for (int i = 0; i < nb_points; i++)
                    {
                        Donnees.TAB_CASERow ligneCase = FindParXY(x[i], y[i]);//avec le find en direct, on a un crash d'accès !
                        resCase[i] = new Node(ligneCase);
                    }

                    if (0 == resCase.Length)
                    {
                        string message = string.Format("CasesVoisines ne trouve aucun node pour x={0} et y={1}", source.I_X, source.I_Y);
                        throw new Exception(message);
                    }
                    return resCase;
                }
                catch (Exception ex)
                {
                    string message = string.Format("TAB_CASEDataTable.CasesVoisines {3} : {0} : {1} :{2}",
                           ex.Message, (null == ex.InnerException) ? "sans inner exception" : ex.InnerException.Message,
                           ex.StackTrace, ex.GetType().ToString());
                    LogFile.Notifier(message);
                    throw;
                }
            }

            public IList<Donnees.TAB_CASERow> CasesVoisinesInBloc(TAB_CASERow source, int xBloc, int yBloc, int tailleBloc)
            {
                IList<Donnees.TAB_CASERow> resCase;
                int largeur = m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1;
                int hauteur = m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1;
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
                if (source.I_X > 0 && source.I_Y < hauteur)
                {
                    x[nb_points] = source.I_X - 1;
                    y[nb_points++] = source.I_Y + 1;
                }
                if (source.I_X < largeur && source.I_Y < hauteur)
                {
                    x[nb_points] = source.I_X + 1;
                    y[nb_points++] = source.I_Y + 1;
                }
                if (source.I_X < largeur && source.I_Y > 0)
                {
                    x[nb_points] = source.I_X + 1;
                    y[nb_points++] = source.I_Y - 1;
                }
                if (source.I_X > 0)
                {
                    x[nb_points] = source.I_X - 1;
                    y[nb_points++] = source.I_Y;
                }
                if (source.I_X < largeur)
                {
                    x[nb_points] = source.I_X + 1;
                    y[nb_points++] = source.I_Y;
                }
                if (source.I_Y > 0)
                {
                    x[nb_points] = source.I_X;
                    y[nb_points++] = source.I_Y - 1;
                }
                if (source.I_Y < hauteur)
                {
                    x[nb_points] = source.I_X;
                    y[nb_points++] = source.I_Y + 1;
                }

                resCase = new List<Donnees.TAB_CASERow>();
                for (int i = 0; i < nb_points; i++)
                {
                    if (x[i] >= xmin && x[i] <= xmax && y[i] >= ymin && y[i] <= ymax)
                    //if (x[i] >= xmin && x[i] < xmax && y[i] >= ymin && y[i] < ymax)
                    {
                        Donnees.TAB_CASERow ligneCase = FindParXY(x[i], y[i]);
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

            public IList<Node> CasesVoisinesInBloc(Node source, int xBloc, int yBloc, int tailleBloc)
            {
                IList<Node> resCase;
                int largeur = m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1;
                int hauteur = m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1;
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
                if (source.I_X > 0 && source.I_Y < hauteur)
                {
                    x[nb_points] = source.I_X - 1;
                    y[nb_points++] = source.I_Y + 1;
                }
                if (source.I_X < largeur && source.I_Y < hauteur)
                {
                    x[nb_points] = source.I_X + 1;
                    y[nb_points++] = source.I_Y + 1;
                }
                if (source.I_X < largeur && source.I_Y > 0)
                {
                    x[nb_points] = source.I_X + 1;
                    y[nb_points++] = source.I_Y - 1;
                }
                if (source.I_X > 0)
                {
                    x[nb_points] = source.I_X - 1;
                    y[nb_points++] = source.I_Y;
                }
                if (source.I_X < largeur)
                {
                    x[nb_points] = source.I_X + 1;
                    y[nb_points++] = source.I_Y;
                }
                if (source.I_Y > 0)
                {
                    x[nb_points] = source.I_X;
                    y[nb_points++] = source.I_Y - 1;
                }
                if (source.I_Y < hauteur)
                {
                    x[nb_points] = source.I_X;
                    y[nb_points++] = source.I_Y + 1;
                }

                resCase = new List<Node>();
                for (int i = 0; i < nb_points; i++)
                {
                    if (x[i] >= xmin && x[i] <= xmax && y[i] >= ymin && y[i] <= ymax)
                    //if (x[i] >= xmin && x[i] < xmax && y[i] >= ymin && y[i] < ymax)
                    {
                        Donnees.TAB_CASERow ligneCase = FindParXY(x[i], y[i]);
                        resCase.Add(new Node(ligneCase));
                    }
                }

                if (0 == resCase.Count)
                {
                    string message = string.Format("CasesVoisinesInBloc ne trouve aucun voisin pour x={0} et y={1}", source.I_X, source.I_Y);
                    throw new Exception(message);
                }

                return resCase;
            }

            internal void SetValueIndex(int i, int i_X, int i_Y)
            {
                m_listeIndex.SetValue(i, i_X, i_Y);
            }

            internal void ViderLaTable(bool viderIndex)
            {
                Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                Clear();//le but c'est de ne pas les sauver pour gagner en temps de chargement justement
                //m_listeIndex peut être null quand on crée la partie et que la carte n'est pas encore générée
                if (viderIndex && null != m_listeIndex)
                {
                    for (int x = 0; x < Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE; x++) for (int y = 0; y < Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE; y++) { m_listeIndex.SetValue(-1, x, y); }
                }
                Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
            }
        }

        /*
        partial class TAB_VOISINS_CASEDataTable
        {
            /// <summary>
            /// renvoie les identifiants des voisins d'une case
            /// </summary>
            /// <param name="ligneCase">case dont on souhaite connaitre les voisins</param>
            /// <returns>liste des identifiants des voisins</returns>
            public TAB_VOISINS_CASERow[] TrouveVoisins(TAB_CASERow ligneCase)
            {
                string requete = string.Format("ID_CASE={0}", ligneCase.ID_CASE);
                return (TAB_VOISINS_CASERow[])Select(requete);
            }

            public TAB_CASERow[] TrouveCasesVoisines(TAB_CASERow ligneCase)
            {
                int i;
                TAB_CASERow[] res;
                TAB_VOISINS_CASERow[] resVoisins;
                string requete = string.Format("ID_CASE={0}", ligneCase.ID_CASE);
                resVoisins= (TAB_VOISINS_CASERow[])Select(requete);
                if (resVoisins.Length == 0)
                {
                    return null;
                }
                res = new TAB_CASERow[resVoisins.Length];
                i=0;
                while (i<resVoisins.Length)
                {
                    res[i] = m_donnees.tableTAB_CASE.FindByID_CASE(resVoisins[i].ID_VOISIN);
                    i++;
                }
                return res;
            }
        }
    */
        partial class TAB_PARTIEDataTable
        {
            /// <summary>
            /// indique s'il fait jour ou nuit
            /// </summary>
            /// <returns>true il fait nuit, false, il fait jour</returns>
            public bool Nocturne()
            {
                //return false;//BEA juste pour les tests
                return Nocturne(m_donnees.TAB_PARTIE.HeureCourante());
            }

            /// <summary>
            /// indique s'il fait jour ou nuit pour une heure donnée
            /// </summary>
            /// <returns>true il fait nuit, false, il fait jour</returns>
            public bool Nocturne(int heure)
            {
                if (heure < m_donnees.TAB_JEU[0].I_LEVER_DU_SOLEIL
                    || heure >= m_donnees.TAB_JEU[0].I_COUCHER_DU_SOLEIL)
                {
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Indique s'il fait nuit ou s'il est trop tard pour déclencher une bataille
            /// </summary>
            /// <returns></returns>
            public bool NocturneOuBatailleImpossible()
            {
                int heureCourante = Donnees.m_donnees.TAB_PARTIE.HeureCourante();
                return Donnees.m_donnees.TAB_PARTIE.Nocturne() || Donnees.m_donnees.TAB_PARTIE.Nocturne(heureCourante + 1) || Donnees.m_donnees.TAB_PARTIE.Nocturne(heureCourante + 2);
            }
            /// <summary>
            /// Renvoie l'heure courante sur une base 0-23
            /// </summary>
            /// <returns>heure en cours</returns>
            public int HeureCourante()
            {
                return HeureBase(m_donnees.TAB_PARTIE[0].I_TOUR + m_donnees.TAB_PARTIE[0].I_PHASE / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES);
            }

            /// <summary>
            /// Renvoie l'heure courante sur une base 0-23
            /// </summary>
            /// <returns>heure en cours</returns>
            public int HeureBase(int tour)
            {
                return (m_donnees.TAB_JEU[0].I_HEURE_INITIALE + tour) % 24;
            }

            /// <summary>
            /// renvoie le nombre d'heures effectuées de nuit dans un délai
            /// </summary>
            /// <param name="tourDebut">tour du début du délai</param>
            /// <param name="duree">durée du délai</param>
            /// <returns></returns>
            public int DureeNocturne(int tourDebut, int duree)
            {
                int lNocturne = 0;
                //methode de bourrin !
                for (int i = tourDebut; i < duree; i++)
                {
                    if (Nocturne(HeureBase(i))) { lNocturne++; }
                }
                return lNocturne;
            }
        }

        /*
            partial class TAB_CARTEDataTable
            {
                /// <summary>
                /// renvoie tous les voisins d'une case, par leurs abscissses et leurs ordonnées
                /// </summary>
                /// <param name="ligneCase">case dont on souhaite connaitre les voisins</param>
                public void TrouveVoisins(TAB_CASERow ligneCase, out Collection<int> x_voisins, out Collection<int> y_voisins)
                {
                    x_voisins = new Collection<int>();
                    y_voisins = new Collection<int>();
                    bool blignegauche = false;
                    bool blignedroite = false;
                    bool blignehaute = false;
                    bool blignebasse = false;

                    if (0 == ligneCase.I_X)
                    {
                        blignegauche=true;
                    }
                    if (0 == ligneCase.I_Y)
                    {
                        blignehaute = true;
                    }
                    if (this[0].I_LARGEUR - 1 == ligneCase.I_X)
                    {
                        blignedroite = true;
                    }
                    if (this[0].I_HAUTEUR - 1 == ligneCase.I_Y)
                    {
                        blignebasse = true;
                    }

                    if (!blignehaute)
                    {
                        if (!blignegauche)
                        {
                            x_voisins.Add(ligneCase.I_X - 1);
                            y_voisins.Add(ligneCase.I_Y - 1);
                        }
                        x_voisins.Add(ligneCase.I_X);
                        y_voisins.Add(ligneCase.I_Y - 1);
                        if (!blignedroite)
                        {
                            x_voisins.Add(ligneCase.I_X + 1);
                            y_voisins.Add(ligneCase.I_Y - 1);
                        }
                    }

                    if (!blignedroite)
                    {
                        x_voisins.Add(ligneCase.I_X + 1);
                        y_voisins.Add(ligneCase.I_Y);
                    }

                    if (!blignebasse)
                    {
                        if (!blignedroite)
                        {
                            x_voisins.Add(ligneCase.I_X + 1);
                            y_voisins.Add(ligneCase.I_Y + 1);
                        }
                        x_voisins.Add(ligneCase.I_X);
                        y_voisins.Add(ligneCase.I_Y + 1);
                        if (!blignegauche)
                        {
                            x_voisins.Add(ligneCase.I_X - 1);
                            y_voisins.Add(ligneCase.I_Y + 1);
                        }
                    }
                    if (!blignegauche)
                    {
                        x_voisins.Add(ligneCase.I_X - 1);
                        y_voisins.Add(ligneCase.I_Y);
                    }
                }
            }
            */
        partial class TAB_PHRASEDataTable
        {
            /// <summary>
            /// Donner une phrase au hasard du type demandé
            /// </summary>
            /// <param name="typePhrase">chaine du type de la phrase</param>
            /// <returns>une chaine de phrase, chaine vide sinon</returns>
            public string DonneUnePhrase(ClassMessager.MESSAGES typePhrase)
            {
                //pour tester sur une phrase fixe
                //if (typePhrase == ClassMessager.MESSAGES.MESSAGE_ARRIVE_A_DESTINATION)
                //{
                //    TAB_PHRASERow lignePhrase = this.FindByID_PHRASE(47);
                //    return lignePhrase.S_PHRASE;
                //}
                string phrase = string.Empty;
                Random de = new Random();

                string requete = string.Format("I_TYPE={0}", (int)typePhrase);
                TAB_PHRASERow[] listePhrase = (TAB_PHRASERow[])Select(requete);
                if (null != listePhrase && listePhrase.Length > 0)
                {
                    phrase = listePhrase[de.Next(0, listePhrase.Length)].S_PHRASE;
                }
                return phrase;
            }
        }

        partial class TAB_ROLEDataTable
        {
            public TAB_ROLERow TrouvePion(int idPion)
            {
                string requete = string.Format("ID_PION={0}", idPion);
                TAB_ROLERow[] resRole = (TAB_ROLERow[])Select(requete);
                if (0 == resRole.Length)
                {
                    return null;
                }
                if (resRole.Length > 1)
                {
                    string message = string.Format("TAB_ROLEDataTable::TrouvePion trouve {0} valeurs pour idPion={1}", resRole.Length, idPion);
                    throw new Exception(message);
                }
                return resRole[0];
            }
        }

        /// <summary>
        /// Suppression des tables d'optimisation
        /// </summary>
        internal void NettoyageBase()
        {
            m_donnees.TAB_ESPACE.Clear();
            m_donnees.TAB_PARCOURS.Clear();
        }

        internal bool SauvegarderPartie(string nomFichier, bool bConserverCases)
        {
            //if (0 == TAB_JEU.Count)
            //{
            //    //possible sur une nouvelle partie
            //    return Dal.SauvegarderPartie(nomFichier, 0, 0, Donnees.m_donnees, true);
            //}

            ////Mise à jour de la version du fichier pour de futures mise à jour
            //TAB_JEU[0].I_VERSION = 6;
            return SauvegarderPartie(nomFichier, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, m_donnees.TAB_PARTIE[0].I_PHASE, true, bConserverCases);
        }

        internal bool SauvegarderPartie(string nomFichier, int iTour, int iPhase, bool bSuperieur, bool bConserverCases)
        {
            return SauvegarderPartie(nomFichier, iTour, iPhase, bSuperieur, bConserverCases, true);
        }

        internal bool SauvegarderPartie(string nomFichier, int iTour, int iPhase, bool bSuperieur, bool bConserverCases, bool bSauvegarderCases)
        {
            bool retour;
            //Donnees.TAB_CASEDataTable caseTemp = null;

            if (0 == TAB_JEU.Count)
            {
                //possible sur une nouvelle partie
                Monitor.Enter(Donnees.m_donnees);
                retour = Dal.SauvegarderPartie(nomFichier, 0, 0, Donnees.m_donnees, bSuperieur);
                Monitor.Exit(Donnees.m_donnees);
                return retour;
            }

            //Mise à jour de la version du fichier pour de futures mise à jour
            TAB_JEU[0].I_VERSION = 14;
            //ChargerToutesLesCases();//pour test
            if (!bConserverCases) //on sauvegarde toujours les cases maintenant
            {
                if (0 != iTour && bSauvegarderCases)
                {
                    if (!SauvegarderCases()) { return false; }
                }
                if (0 == iPhase || 0 == this.TAB_PARTIE.Count() || !this.TAB_PARTIE[0].FL_DEMARRAGE)
                {
                    //caseTemp = (Donnees.TAB_CASEDataTable)Donnees.m_donnees.TAB_CASE.Copy();
                    this.TAB_CASE.ViderLaTable(true);
                }
            }

            Monitor.Enter(Donnees.m_donnees);
            retour = Dal.SauvegarderPartie(nomFichier, iTour, iPhase, Donnees.m_donnees, bSuperieur);
            //if (null!= caseTemp)
            //{
            //    Donnees.m_donnees.TAB_CASE.Merge(caseTemp,false);
            //}
            Monitor.Exit(Donnees.m_donnees);
            return retour;

        }

        internal bool ChargerToutesLesCases()
        {
            if (0 == TAB_PARTIE.Count || 0 == TAB_JEU.Count)
            {
                return true;
            }
            Donnees.TAB_CASEDataTable donneesSource = new TAB_CASEDataTable();
            Cursor oldCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            m_donnees.TAB_CASE.ViderLaTable(true);
            for (int x = 0; x < Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE; x += Constantes.CST_TAILLE_BLOC_CASES)
            {
                for (int y = 0; y < Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE; y += Constantes.CST_TAILLE_BLOC_CASES)
                {
                    //on vérifie que le chargement n'a pas déjà été fait ->appelé uniquement en génération de cartes, faux, appeler pour les noms de ponts par exemple
                    string requete = string.Format("I_X={0} AND I_Y={1}", x, y);
                    Donnees.TAB_CASERow[] listeCases = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete);
                    if (listeCases.Count() > 0)
                    {
                        continue; //cases déjà chargées
                    }
                    if (!Donnees.m_donnees.TAB_CASE.ChargerDonnneesCases(ref donneesSource, x, y, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR_CASES, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE_CASES)) { return false; }
                }
            }
            Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
            Donnees.m_donnees.TAB_CASE.Merge(donneesSource, false);
            Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
            //mise à jour de l'index
            for (int i = 0 /*debutNouvellesLignes*/; i < Donnees.m_donnees.TAB_CASE.Count; i++)
            {
                TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE[i];// si je ne le fais que sur donnees source, je ne peux pas garantir que l'ordre est respecté par le merge, mais plus je charge plus c'est long
                Donnees.m_donnees.TAB_CASE.SetValueIndex(i, ligneCase.I_X, ligneCase.I_Y);
            }
            Cursor.Current = oldCursor;
            return true;
        }

        internal bool SauvegarderCases()
        {
            if (0 == TAB_PARTIE.Count || 0 == TAB_JEU.Count)
            {
                return true;
            }
            int iTour, iPhase;
            iTour = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
            iPhase = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;
            Donnees.TAB_CASEDataTable baseCases = new Donnees.TAB_CASEDataTable();

            //if (0 == iTour)
            //{
            //sauvegarde complète
            for (int x = 0; x < Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE; x += Constantes.CST_TAILLE_BLOC_CASES)
            {
                for (int y = 0; y < Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE; y += Constantes.CST_TAILLE_BLOC_CASES)
                {
                    Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                    string requete = string.Format("I_X>={0} AND I_X<{1} AND I_Y>={2} AND I_Y<{3}",
                        x, x + Constantes.CST_TAILLE_BLOC_CASES, y, y + Constantes.CST_TAILLE_BLOC_CASES);
                    Donnees.TAB_CASERow[] listeCases = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete);
                    //Debug.WriteLine(requete + " : " + listeCases.Count());
                    if (0 == listeCases.Count()) { Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot); continue; }
                    for (int i = 0; i < listeCases.Count(); i++)
                    {
                        //if (i==0 || (listeCases[i].ID_CASE != listeCases[i-1].ID_CASE))//test qui, normalement, devrait être inutile sauf que parfois les Select duplique les lignes pour une inexplicable raison lié à endloaddata qui est bugué
                        {
                            baseCases.ImportRow(listeCases[i]);
                        }

                    }
                    if (!this.TAB_CASE.SauvegarderCases(baseCases, x, y, iTour, iPhase)) { Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot); return false; }
                    baseCases.Rows.Clear();
                    Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                }
            }
            m_donnees.TAB_PARTIE[0].I_TOUR_CASES = m_donnees.TAB_PARTIE[0].I_TOUR;
            m_donnees.TAB_PARTIE[0].I_PHASE_CASES = m_donnees.TAB_PARTIE[0].I_PHASE;
            //}
            //else
            //{
            //    //on ne sauvegarde que les blocs avec des cases modifiées
            //    foreach (Donnees.TAB_CASE_MODIFICATIONRow ligneCaseModifiée in Donnees.m_donnees.TAB_CASE_MODIFICATION)
            //    {
            //        Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneCaseModifiée.ID_CASE);
            //        int xBloc, yBloc;
            //        ligneCase.BlocCourant(out xBloc, out yBloc);
            //        int x = xBloc * Constantes.CST_TAILLE_BLOC_CASES;
            //        int y = yBloc * Constantes.CST_TAILLE_BLOC_CASES;
            //        Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
            //        string requete = string.Format("I_X>={0} AND I_X<{1} AND I_Y>={2} AND I_Y<{3}",
            //            x, x + Constantes.CST_TAILLE_BLOC_CASES, y, y + Constantes.CST_TAILLE_BLOC_CASES);
            //        Donnees.TAB_CASERow[] listeCases = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete);
            //        Debug.WriteLine(requete + " : " + listeCases.Count());
            //        if (0 == listeCases.Count()) { Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot); continue; }
            //        for (int i = 0; i < listeCases.Count(); i++)
            //        {
            //            //if (i==0 || (listeCases[i].ID_CASE != listeCases[i-1].ID_CASE))//test qui, normalement, devrait être inutile sauf que parfois les Select duplique les lignes pour une inexplicable raison lié à endloaddata qui est bugué
            //            {
            //                baseCases.ImportRow(listeCases[i]);
            //            }

            //        }
            //        if (!this.TAB_CASE.SauvegarderCases(baseCases, x, y, iTour, iPhase)) { Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot); return false; }
            //        baseCases.Rows.Clear();
            //        Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
            //    }
            //}
            return true;
        }

        internal bool ChargerPartie(string filename)
        {
            if (!WaocLib.Dal.ChargerPartie(filename, this))
            {
                return false;
            }
            if (m_donnees.TAB_PARTIE.Count() > 0)
            {
                m_donnees.TAB_PARTIE[0].I_TOUR_CASES = m_donnees.TAB_PARTIE[0].I_TOUR;
                m_donnees.TAB_PARTIE[0].I_PHASE_CASES = m_donnees.TAB_PARTIE[0].I_PHASE;
            }
            return MiseAJourVersion();
        }

        private bool MiseAJourVersion()
        {
            if (0 == TAB_JEU.Count)
            {
                //possible sur une nouvelle partie
                return true;
            }

            #region version 1
            if (TAB_JEU[0].I_VERSION < 2)
            {
                foreach (TAB_MESSAGERow ligneMessage in TAB_MESSAGE)
                {
                    if (ligneMessage.IsID_CASE_DEBUTNull()) { ligneMessage.ID_CASE_DEBUT = ligneMessage.ID_CASE; }
                    if (ligneMessage.IsID_CASE_FINNull()) { ligneMessage.ID_CASE_FIN = ligneMessage.ID_CASE; }
                }

                //cas devenu impossible avec la suppression des DBNUlls de la table
                //foreach (TAB_PCC_COUTSRow trajet in TAB_PCC_COUTS)
                //{
                //    if (trajet.IsB_CREATIONNull()) { trajet.B_CREATION = false; }
                //    if (trajet.IsI_COUT_INITIALNull()) { trajet.I_COUT_INITIAL = trajet.I_COUT; }
                //}

                #region correctif sur les nations pour les messagers, et les messagers patrouilles */
                foreach (TAB_PIONRow lignePion in TAB_PION)
                {
                    if (lignePion.estMessager || lignePion.estPatrouille)
                    {
                        TAB_MODELE_PIONRow ligneModeleProprietaire = lignePion.modelePion;
                        if (lignePion.possedeAptitude("MESSAGER"))
                        {
                            if (0 == ligneModeleProprietaire.ID_NATION && lignePion.ID_MODELE_PION != 5)
                            {
                                int pb = 0;
                                pb++;
                            }
                            if (0 != ligneModeleProprietaire.ID_NATION && lignePion.ID_MODELE_PION != 6)
                            {
                                int pb = 0;
                                pb++;
                            }
                            lignePion.ID_MODELE_PION = (0 == ligneModeleProprietaire.ID_NATION) ? 5 : 6;
                        }
                        if (lignePion.possedeAptitude("PATROUILLEMESSAGER"))
                        {
                            if (0 == ligneModeleProprietaire.ID_NATION && lignePion.ID_MODELE_PION != 9)
                            {
                                int pb = 0;
                                pb++;
                            }
                            if (0 != ligneModeleProprietaire.ID_NATION && lignePion.ID_MODELE_PION != 10)
                            {
                                int pb = 0;
                                pb++;
                            }
                            lignePion.ID_MODELE_PION = (0 == ligneModeleProprietaire.ID_NATION) ? 9 : 10;
                        }
                        if (lignePion.possedeAptitude("PATROUILLE"))
                        {
                            if (0 == ligneModeleProprietaire.ID_NATION && lignePion.ID_MODELE_PION != 7)
                            {
                                int pb = 0;
                                pb++;
                            }
                            if (0 != ligneModeleProprietaire.ID_NATION && lignePion.ID_MODELE_PION != 8)
                            {
                                int pb = 0;
                                pb++;
                            }
                            lignePion.ID_MODELE_PION = (0 == ligneModeleProprietaire.ID_NATION) ? 7 : 8;
                        }
                    }
                }
                #endregion
            }
            #endregion

            #region version 2
            if (TAB_JEU[0].I_VERSION < 3)
            {
                //nouveaux champs
                foreach (TAB_PIONRow lignePion in TAB_PION)
                {
                    lignePion.I_MATERIEL = 50;
                    lignePion.I_RAVITAILLEMENT = 50;
                    lignePion.B_CAVALERIE_DE_LIGNE = false;
                    lignePion.B_CAVALERIE_LOURDE = false;
                    lignePion.B_GARDE = false;
                    lignePion.B_VIEILLE_GARDE = false;
                    lignePion.I_TOUR_CONVOI_CREE = 0;
                    lignePion.I_SOLDATS_RAVITAILLES = 0;
                    lignePion.I_NB_HEURES_FORTIFICATION = 0;
                    lignePion.I_NIVEAU_FORTIFICATION = 0;
                    lignePion.SetID_PION_REMPLACENull();
                    lignePion.I_DUREE_HORS_COMBAT = 0;
                    lignePion.SetI_TOUR_BLESSURENull();
                    lignePion.B_BLESSES = false;
                    lignePion.B_PRISONNIERS = false;
                    lignePion.B_RENFORT = false;
                    lignePion.SetID_LIEU_RATTACHEMENTNull();
                    lignePion.C_NIVEAU_DEPOT = 'Z';
                }

                foreach (TAB_RENFORTRow ligneRenfort in TAB_RENFORT)
                {
                    ligneRenfort.I_MATERIEL = 50;
                    ligneRenfort.I_RAVITAILLEMENT = 50;
                    ligneRenfort.B_CAVALERIE_DE_LIGNE = false;
                    ligneRenfort.B_CAVALERIE_LOURDE = false;
                    ligneRenfort.B_GARDE = false;
                    ligneRenfort.B_VIEILLE_GARDE = false;
                    ligneRenfort.C_NIVEAU_DEPOT = 'Z';
                }

                foreach (TAB_MESSAGERow ligneMessage in TAB_MESSAGE)
                {
                    ligneMessage.I_MATERIEL = 0;
                    ligneMessage.I_RAVITAILLEMENT = 0;
                    ligneMessage.I_SOLDATS_RAVITAILLES = 0;
                    ligneMessage.I_NB_HEURES_FORTIFICATION = 0;
                    ligneMessage.I_NIVEAU_FORTIFICATION = 0;
                    ligneMessage.I_DUREE_HORS_COMBAT = 0;
                    ligneMessage.I_TOUR_BLESSURE = 0;
                }

                foreach (TAB_BATAILLERow ligneBataille in TAB_BATAILLE)
                {
                    ligneBataille.I_ENGAGEMENT_0 = 1;
                    ligneBataille.I_ENGAGEMENT_1 = 1;
                    ligneBataille.I_ENGAGEMENT_2 = 1;
                    ligneBataille.I_ENGAGEMENT_3 = 1;
                    ligneBataille.I_ENGAGEMENT_4 = 1;
                    ligneBataille.I_ENGAGEMENT_5 = 1;
                }

                foreach (TAB_NOMS_CARTERow ligneNomCarte in TAB_NOMS_CARTE)
                {
                    ligneNomCarte.I_CHANCE_RENFORT = 0;
                    if (TAB_MODELE_PION.Count <= 0)
                    {
                        ligneNomCarte.SetID_MODELE_PION_RENFORTNull();
                    }
                    else
                    {
                        ligneNomCarte.ID_MODELE_PION_RENFORT = TAB_MODELE_PION[0].ID_MODELE_PION;
                    }
                    ligneNomCarte.SetID_PION_PROPRIETAIRE_RENFORTNull();

                    ligneNomCarte.B_HOPITAL = false;
                    ligneNomCarte.B_PRISON = false;
                    ligneNomCarte.I_INFANTERIE_RENFORT = 0;
                    ligneNomCarte.I_CAVALERIE_RENFORT = 0;
                    ligneNomCarte.I_ARTILLERIE_RENFORT = 0;
                    ligneNomCarte.I_MORAL_RENFORT = 0;
                    ligneNomCarte.I_MATERIEL_RENFORT = 50;
                    ligneNomCarte.I_RAVITAILLEMENT_RENFORT = 50;
                }

                //evolutions supprimés en version 12
                //foreach (TAB_NATIONRow ligneNation in TAB_NATION)
                //{
                //ligneNation.I_FOURRAGE = 0;
                //ligneNation.I_LIMITE_FOURRAGE = 100;//limite du niveau de ravitaillement en dessous de laquelle on peut fourrager, 100%=égal, dans tous les cas (pour les français par exemple)
                //ligneNation.I_GUERISON = 0;
                //}
            }
            #endregion 

            #region version 3
            if (TAB_JEU[0].I_VERSION < 4)
            {
                //nouveaux champ
                foreach (TAB_ORDRERow ligneOrdre in TAB_ORDRE)
                {
                    if (ligneOrdre.IsID_ORDRE_SUIVANTNull())
                    {
                        ligneOrdre.SetID_ORDRE_TRANSMISNull();
                    }
                    else
                    {
                        ligneOrdre.ID_ORDRE_TRANSMIS = ligneOrdre.ID_ORDRE_SUIVANT;
                        ligneOrdre.SetID_ORDRE_SUIVANTNull();
                    }
                    ligneOrdre.SetI_ENGAGEMENTNull();
                }

                foreach (TAB_METEORow ligneMeteo in TAB_METEO)
                {
                    ligneMeteo.I_POURCENT_RAVITAILLEMENT = 100;
                    ligneMeteo.I_POURCENT_FATIGUE = 100;
                    ligneMeteo.I_POURCENT_RALLIEMENT = 100;
                }

                foreach (TAB_PIONRow lignePion in TAB_PION)
                {
                    lignePion.SetID_PION_ESCORTENull();
                    lignePion.I_INFANTERIE_ESCORTE = 0;
                    lignePion.I_CAVALERIE_ESCORTE = 0;
                    lignePion.I_MATERIEL_ESCORTE = 0;
                    lignePion.I_TOUR_RETRAITE_RESTANT = 0;
                }
            }
            #endregion

            #region version 4
            if (TAB_JEU[0].I_VERSION < 5)
            {
                foreach (TAB_PIONRow lignePion in TAB_PION)
                {
                    lignePion.B_CAVALERIE_DE_LIGNE = false;
                    lignePion.B_CAVALERIE_LOURDE = false;
                    lignePion.B_GARDE = false;
                    lignePion.B_VIEILLE_GARDE = false;
                    lignePion.B_RENFORT = false;
                    lignePion.SetID_DEPOT_SOURCENull();
                }

                foreach (TAB_RENFORTRow ligneRenfort in TAB_RENFORT)
                {
                    ligneRenfort.B_CAVALERIE_DE_LIGNE = false;
                    ligneRenfort.B_CAVALERIE_LOURDE = false;
                    ligneRenfort.B_GARDE = false;
                    ligneRenfort.B_VIEILLE_GARDE = false;
                }
            }

            #endregion

            #region version 5
            if (TAB_JEU[0].I_VERSION < 6)
            {
                foreach (TAB_METEORow ligneMeteo in TAB_METEO)
                {
                    ligneMeteo.ID_METEO_AGGRAVATION = -1;
                    ligneMeteo.I_NB_TOURS_AGGRAVATION = 0;
                }
                TAB_PARTIE[0].I_NB_METEO_SUCCESSIVE = 0;
            }
            #endregion

            #region version 6
            if (TAB_JEU[0].I_VERSION < 7)
            {
                for (int l = 0; l < Donnees.m_donnees.TAB_NOMS_CARTE.Count; l++)
                {
                    Donnees.TAB_NOMS_CARTERow ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE[l];
                    Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneNomCarte.ID_CASE);
                    if (null != ligneCase)
                    {
                        ligneNomCarte.I_X = ligneCase.I_X;
                        ligneNomCarte.I_Y = ligneCase.I_Y;
                    }
                }
            }
            #endregion

            #region version 8
            if (TAB_JEU[0].I_VERSION < 8)
            {
                Donnees.m_donnees.TAB_CASE.InitialisationListeCase(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE, Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE);
                for (int l = 0; l < Donnees.m_donnees.TAB_NOMS_CARTE.Count; l++)
                {
                    Donnees.TAB_NOMS_CARTERow ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE[l];
                    ligneNomCarte.B_CREATION_DEPOT = false;
                }
            }
            #endregion

            #region version 9
            if (TAB_JEU[0].I_VERSION < 9)
            {
                for (int l = 0; l < Donnees.m_donnees.TAB_BATAILLE.Count; l++)
                {
                    Donnees.TAB_BATAILLERow ligneBataille = Donnees.m_donnees.TAB_BATAILLE[l];
                    ligneBataille.B_ZONE_UNIQUE = false;
                }
                for (int l = 0; l < Donnees.m_donnees.TAB_MODELE_TERRAIN.Count; l++)
                {
                    Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN[l];
                    ligneModeleTerrain.B_BATAILLE_ZONE_UNIQUE = false;
                }
            }
            #endregion

            #region version 10
            if (TAB_JEU[0].I_VERSION < 10)
            {
                foreach (TAB_PIONRow lignePion in TAB_PION)
                {
                    if (lignePion.IsI_TRINull())
                        lignePion.SetI_TRINull();
                }
            }
            #endregion

            #region version 11
            if (TAB_JEU[0].I_VERSION < 11)
            {
                for (int l = 0; l < Donnees.m_donnees.TAB_MODELE_TERRAIN.Count; l++)
                {
                    Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN[l];
                    ligneModeleTerrain.B_SANS_BONUS_CAVALERIE = false;
                }
                for (int l = 0; l < Donnees.m_donnees.TAB_NATION.Count; l++)
                {
                    Donnees.TAB_NATIONRow ligneNation = Donnees.m_donnees.TAB_NATION[l];
                    ligneNation.I_LIMITE_DEPOT_A = 1;
                }
            }
            #endregion

            #region version 12
            if (TAB_JEU[0].I_VERSION < 12)
            {
                for (int l = 0; l < Donnees.m_donnees.TAB_MODELE_PION.Count; l++)
                {
                    Donnees.TAB_MODELE_PIONRow ligneModelePion = Donnees.m_donnees.TAB_MODELE_PION[l];
                    ligneModelePion.I_FOURGON = 0;
                    ligneModelePion.I_GUERISON = 0;
                    ligneModelePion.I_FOURRAGE = 0;
                    ligneModelePion.S_CRI_RALLIEMENT = string.Empty;
                    ligneModelePion.S_NATION = string.Empty;
                }
            }
            #endregion

            #region version 13
            if (TAB_JEU[0].I_VERSION < 13)
            {
                for (int l = 0; l < Donnees.m_donnees.TAB_PION.Count; l++)
                {
                    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[l];
                    lignePion.I_TOUR_ENNEMI_OBSERVABLE = Constantes.NULLENTIER;
                    lignePion.S_ENNEMI_OBSERVABLE = string.Empty;
                }
            }
            #endregion

            #region version 14
            if (TAB_JEU[0].I_VERSION < 14)
            {
                Donnees.m_donnees.TAB_CASE.InitialisationListeCase(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE, Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE);
                for (int l = 0; l < Donnees.m_donnees.TAB_VIDEO.Count; l++)
                {
                    Donnees.TAB_VIDEORow ligneVideo = Donnees.m_donnees.TAB_VIDEO[l];
                    if (ligneVideo.ID_CASE >= 0)
                    {
                        Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneVideo.ID_CASE);
                        ligneVideo.I_X = ligneCase.I_X;
                        ligneVideo.I_Y = ligneCase.I_Y;
                    }
                }
            }
            #endregion
            return true;
        }
    }
}
