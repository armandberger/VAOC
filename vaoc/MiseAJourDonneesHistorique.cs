using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaocLib;

namespace vaoc
{
    /// <summary>
    /// Mise à jour de données sur tous les fichiers depuis le début de la campagne
    /// </summary>
    class MiseAJourDonneesHistorique
    {
        private string m_fichierSource { get; set; }
        private bool m_bAvecSauvegarde { get; set; }
        private int m_traitement;//traitement principal
        private int m_nombretours;
        private int m_debut;
        System.ComponentModel.BackgroundWorker m_travailleur;
        private Donnees.TAB_VIDEODataTable m_tableVideo = null;
        private Donnees.TAB_BATAILLE_VIDEODataTable m_tableBatailleVideo = null;
        private Donnees.TAB_BATAILLE_PIONS_VIDEODataTable m_tableBataillePionsVideo = null;
        public string Initialisation(string fichierSource, bool avecSauvegarde, int debut, System.ComponentModel.BackgroundWorker worker)
        {
            try
            {
                m_fichierSource = fichierSource;
                m_bAvecSauvegarde = avecSauvegarde;
                m_nombretours = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR + 1 - debut;
                m_travailleur = worker;
                m_debut = debut;
                m_traitement = (0==debut) ? 0 : debut;
                m_tableVideo = new Donnees.TAB_VIDEODataTable();
                m_tableBatailleVideo = new Donnees.TAB_BATAILLE_VIDEODataTable();
                m_tableBataillePionsVideo = new Donnees.TAB_BATAILLE_PIONS_VIDEODataTable();
                return string.Empty;
            }
            catch (Exception e)
            {
                return "Exception: " + e.ToString() + " pile:"+e.StackTrace;
            }
        }

        public string Terminer()
        {
            try
            {
                m_travailleur.ReportProgress(100);
                return string.Empty;
            }
            catch (Exception e)
            {
                return "Exception: " + e.ToString() + " pile:" + e.StackTrace;
            }
        }

        public string Traitement()
        {
            /// <summary>
            /// Rechargement de tous les fichiers précédents pour mettre à jour TAB_VIDEO
            /// </summary>
            try
            {                
                string nomfichier = Dal.NomFichierTourPhase(m_fichierSource, m_traitement, 0, false);
                //if (!Dal.ChargerPartie(Dal.NomFichierTourPhase(m_fichierSource, m_traitement, 0, false), Donnees.m_donnees))
                if (!Donnees.m_donnees.ChargerPartie(nomfichier))
                {
                    string message = "Erreur dans Dal.ChargerPartie, tour n°" + m_traitement.ToString();
                    LogFile.Notifier(message);
                    // bon, on continue, il manquera un tour voilà tout
                    //return message;
                }
                else
                {
                    Donnees.m_donnees.TAB_CASE.ViderLaTable(true);//sinon on peut avoir des doublons au chargement de fichiers contenant des données de cases
                    if (0 == m_tableVideo.Rows.Count)
                    {
                        m_tableVideo.Merge(Donnees.m_donnees.TAB_VIDEO);
                    }
                    if (0 == m_tableBatailleVideo.Rows.Count)
                    {
                        m_tableBatailleVideo.Merge(Donnees.m_donnees.TAB_BATAILLE_VIDEO);
                    }
                    if (0 == m_tableBataillePionsVideo.Rows.Count)
                    {
                        m_tableBataillePionsVideo.Merge(Donnees.m_donnees.TAB_BATAILLE_PIONS_VIDEO);
                    }

                    MiseAjourVideo(m_tableVideo);
                    MiseAjourBatailleVideo(m_tableBatailleVideo, m_tableBataillePionsVideo);
                    //CalculNombreTotalPointsDeVictoire(); -> pas possible I_VICTOIRE non renseigné

                    if (m_bAvecSauvegarde)
                    {
                        Donnees.m_donnees.TAB_VIDEO.Clear();
                        Donnees.m_donnees.TAB_VIDEO.Merge(m_tableVideo);
                        Donnees.m_donnees.TAB_BATAILLE_VIDEO.Clear();
                        Donnees.m_donnees.TAB_BATAILLE_VIDEO.Merge(m_tableBatailleVideo);
                        Donnees.m_donnees.TAB_BATAILLE_PIONS_VIDEO.Clear();
                        Donnees.m_donnees.TAB_BATAILLE_PIONS_VIDEO.Merge(m_tableBataillePionsVideo);
                        Donnees.m_donnees.SauvegarderPartie(nomfichier, m_traitement, 0, false, false, false);
                        //Dal.SauvegarderPartie(m_fichierSource, m_traitement, 0, Donnees.m_donnees);
                    }
                }

                m_traitement++;
                m_travailleur.ReportProgress((m_traitement-m_debut) * 100 / m_nombretours);
                if (m_traitement - m_debut >= m_nombretours)
                {
                    return "Reprise terminée";
                }
                return string.Empty;
            }
            catch (Exception e)
            {
                return "Exception: " + e.ToString() + " pile:" + e.StackTrace;
            }
        }

        private void MiseAjourBatailleVideo(Donnees.TAB_BATAILLE_VIDEODataTable tableBatailleVideo, Donnees.TAB_BATAILLE_PIONS_VIDEODataTable tableBataillePionsVideo)
        {
            int nbUnites012, nbUnites345;
            bool bAjoutDonnees;

            //Donnees.TAB_BATAILLERow ligneBatailleTest = Donnees.m_donnees.TAB_BATAILLE.FindByID_BATAILLE(114);
            //CalculDeLaVictoire(ligneBatailleTest);

            //si on refait le même tour, on supprimer les anciennes données pour ne pas les avoir en double
            int i = 0;
            while (i < tableBatailleVideo.Count)
            {
                Donnees.TAB_BATAILLE_VIDEORow ligneVideo = tableBatailleVideo[i];
                if (ligneVideo.I_TOUR == m_traitement)
                {
                    ligneVideo.Delete();
                }
                else
                {
                    i++;
                }
            }
            i = 0;
            while (i < tableBataillePionsVideo.Count)
            {
                Donnees.TAB_BATAILLE_PIONS_VIDEORow ligneVideo = tableBataillePionsVideo[i];
                if (ligneVideo.I_TOUR == m_traitement)
                {
                    ligneVideo.Delete();
                }
                else
                {
                    i++;
                }
            }

            foreach (Donnees.TAB_BATAILLERow ligneBataille in Donnees.m_donnees.TAB_BATAILLE)
            {
                bAjoutDonnees = false;
                if (ligneBataille.IsI_TOUR_FINNull() || ligneBataille.I_TOUR_FIN>=m_traitement+1)
                {
                    try
                    {
                        if (ligneBataille.I_TOUR_DEBUT + 1 == m_traitement)
                        {
                            //on ne voit le déclenchement d'une bataille qu'au début du tour suivant
                            if (ligneBataille.IsI_PERTES_0Null())
                            {
                                for (i = 0; i < 6; i++)
                                {
                                    ligneBataille["I_PERTES_" + Convert.ToString(i)] = 0;
                                }
                            }
                            ligneBataille.S_FIN = "";
                            ligneBataille.AjouterDonneesVideo(m_traitement, tableBatailleVideo, tableBataillePionsVideo);
                        }
                        if (!ligneBataille.IsI_TOUR_FINNull() && ligneBataille.I_TOUR_FIN + 1 == m_traitement)
                        {
                            //tour de bilan
                            bAjoutDonnees = true;
                            CalculDeLaVictoire(ligneBataille);
                        }
                        else
                        {
                            ligneBataille.S_FIN = "";
                        }
                        if (0 == (m_traitement - ligneBataille.I_TOUR_DEBUT) % 2) { bAjoutDonnees = true; }
                        if (bAjoutDonnees)
                        {
                            //recalcul des pertes si necessaire 
                          if (ligneBataille.IsI_PERTES_0Null())
                            {
                                for (i = 0; i < 6; i++)
                                {
                                    if (ligneBataille["S_COMBAT_" + Convert.ToString(i)].ToString() == string.Empty)
                                    {
                                        ligneBataille["I_PERTES_" + Convert.ToString(i)] = 0;
                                    }
                                    else
                                    {
                                        string scombat = ligneBataille["S_COMBAT_" + Convert.ToString(i)].ToString();
                                        int score = (scombat.IndexOf('=') < 0 ) ? 0 : Convert.ToInt32(scombat.Substring(scombat.LastIndexOf(' ')));
                                        if (0 == score)
                                        { ligneBataille["I_PERTES_" + Convert.ToString(i)] = 0; }
                                        else
                                        {
                                            int[] des;
                                            int[] effectifs;
                                            int[] canons;
                                            int[] modificateurs;
                                            Donnees.TAB_PIONRow[] tablePionsEngages012;
                                            Donnees.TAB_PIONRow[] tablePionsEngages345;

                                            ligneBataille.RecherchePionsEnBataille(out nbUnites012, out nbUnites345, out des, out modificateurs, out effectifs, out canons, out tablePionsEngages012, out tablePionsEngages345, true/*bengagement*/, false/*bcombattif*/, true/*QG*/, true /*bArtillerie*/);
                                            if (i < 3)
                                            {
                                                ligneBataille["I_PERTES_" + Convert.ToString(i)] = Donnees.TAB_BATAILLERow.EffectifTotalSurZone(i, tablePionsEngages012.ToList(), false /*bCombatif*/) * score / 400;
                                            }
                                            else
                                            {
                                                ligneBataille["I_PERTES_" + Convert.ToString(i)] = Donnees.TAB_BATAILLERow.EffectifTotalSurZone(i, tablePionsEngages345.ToList(), false /*bCombatif*/) * score / 400;
                                            }
                                        }
                                    }
                                }
                            }
                            ligneBataille.AjouterDonneesVideo(m_traitement, tableBatailleVideo, tableBataillePionsVideo);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("MiseAjourBatailleVideo 1 Exception: " + ex.ToString() + " pile:" + ex.StackTrace);
                    }
                }
                else
                {
                    try
                    {
                        //        //il faut recalculer systématiquement la fin de bataille -> sauf que tous les paramètres de la bataille ayant changé, ce n'est plus possible
                        //        if (!ligneBataille.IsI_TOUR_FINNull())
                        //        {
                        //            CalculDeLaVictoire(ligneBataille);
                        //        }
                        string[] tableVictoiresBataille = new string[] { "VICTOIRE012","RETRAITE012","NUIT","RETRAITE345","NUIT","RETRAITE345","VICTOIRE012","RETRAITE012","VICTOIRE345","VICTOIRE012","RETRAITE012","RETRAITE012",
                                "NUIT","VICTOIRE345","RETRAITE012","RETRAITE012","RETRAITE012","RETRAITE345","VICTOIRE012","VICTOIRE345","VICTOIRE012","RETRAITE345","VICTOIRE345","VICTOIRE345","VICTOIRE012","RETRAITE012","RETRAITE345",
                                "VICTOIRE012","RETRAITE345","VICTOIRE012","RETRAITE345","RETRAITE345","VICTOIRE012","VICTOIRE345","NUIT","RETRAITE012","RETRAITE345","NUIT","NUIT","VICTOIRE012","NUIT","VICTOIRE345","VICTOIRE012","VICTOIRE345",
                                "VICTOIRE345","VICTOIRE345","VICTOIRE345","NUIT","VICTOIRE345","VICTOIRE012","VICTOIRE345","VICTOIRE012","VICTOIRE345","NUIT","VICTOIRE012","VICTOIRE345","RETRAITE345","RETRAITE345","RETRAITE345","VICTOIRE345",
                                "VICTOIRE345","VICTOIRE345","VICTOIRE345","RETRAITE012","RETRAITE345","VICTOIRE345","VICTOIRE345","VICTOIRE345","VICTOIRE345","VICTOIRE012","VICTOIRE345","VICTOIRE345","VICTOIRE012","VICTOIRE012","VICTOIRE345",
                                "VICTOIRE345","VICTOIRE345","VICTOIRE012","VICTOIRE345","VICTOIRE012","VICTOIRE345","RETRAITE012","RETRAITE012","VICTOIRE345","VICTOIRE012","VICTOIRE012","VICTOIRE345","VICTOIRE345","VICTOIRE012","VICTOIRE345",
                                "VICTOIRE012","VICTOIRE012","VICTOIRE012","VICTOIRE012","RETRAITE012","VICTOIRE345","VICTOIRE012","RETRAITE012","RETRAITE345","RETRAITE012","VICTOIRE012","RETRAITE012","VICTOIRE345","RETRAITE012","VICTOIRE345",
                                "RETRAITE012","VICTOIRE345","VICTOIRE345","VICTOIRE012","VICTOIRE345","VICTOIRE345","VICTOIRE012","VICTOIRE345","VICTOIRE012","VICTOIRE012","VICTOIRE345","VICTOIRE345","VICTOIRE345","VICTOIRE012","NUIT","VICTOIRE345",
                                "VICTOIRE345","VICTOIRE345","RETRAITE345","NUIT","VICTOIRE012","NUIT","NUIT","RETRAITE012","NUIT","VICTOIRE345","VICTOIRE012","VICTOIRE012","VICTOIRE012","VICTOIRE012","VICTOIRE012","VICTOIRE345","VICTOIRE345"
                                };
                        ligneBataille.S_FIN = tableVictoiresBataille[ligneBataille.ID_BATAILLE];
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("MiseAjourBatailleVideo 2 Exception: " + ex.ToString() + " pile:" + ex.StackTrace);
                    }
                }
            }
        }

        private void CalculDeLaVictoire(Donnees.TAB_BATAILLERow ligneBataille)
        {
            string message;
            Donnees.TAB_PIONRow[] lignePionsCombattifBataille012;
            Donnees.TAB_PIONRow[] lignePionsCombattifBataille345;
            bool bVictoire012 = true, bVictoire345 = true, bRetraite012 = false, bRetraite345 = false;
            int nbUnites012, nbUnites345;
            Donnees.TAB_ORDRE_ANCIENRow[] resOrdreRetraiteAnciens;
            Donnees.TAB_ORDRERow[] resOrdreRetraite;

            if (Donnees.m_donnees.TAB_PARTIE.Nocturne(Donnees.m_donnees.TAB_PARTIE.HeureBase(ligneBataille.I_TOUR_FIN)))
            {
                ligneBataille.S_FIN = "NUIT";
            }
            else
            {
                if (!ligneBataille.RecherchePionsEnBataille(out nbUnites012, out nbUnites345, out int[] des, out int[] modificateurs, out int[] effectifs, out int[] canons,
                    out lignePionsCombattifBataille012, out lignePionsCombattifBataille345, true /*engagement*/, true/*combattif*/, false/*QG*/, false /*bArtillerie*/))
                {
                    message = string.Format("MiseAjourBatailleVideo : erreur dans RecherchePionsEnBataille");
                    LogFile.Notifier(message);
                }

                //Un ordre de retraite a-t-il été donné à ce tour ? ou à un autre ? En fait, il ne peut y avoir qu'une retraite sur une bataille !
                string requete = string.Format("ID_BATAILLE={0} AND  I_ORDRE_TYPE={1}",
                    ligneBataille.ID_BATAILLE, Constantes.ORDRES.RETRAITE);
                resOrdreRetraite = (Donnees.TAB_ORDRERow[])Donnees.m_donnees.TAB_ORDRE.Select(requete);
                if (resOrdreRetraite.Length > 0)
                {
                    foreach (Donnees.TAB_ORDRERow ligneOrdre in resOrdreRetraite)
                    {
                        if (ligneOrdre.I_ZONE_BATAILLE <= 2)
                        {
                            bRetraite012 = true;
                        }
                        else
                        {
                            bRetraite345 = true;
                        }
                    }
                }
                else
                {
                    //on verifie si ce n'est pas dans les ordres archivés
                    resOrdreRetraiteAnciens = (Donnees.TAB_ORDRE_ANCIENRow[])Donnees.m_donnees.TAB_ORDRE_ANCIEN.Select(requete);
                    if (resOrdreRetraite.Length > 0)
                    {
                        foreach (Donnees.TAB_ORDRE_ANCIENRow ligneOrdre in resOrdreRetraiteAnciens)
                        {
                            if (ligneOrdre.I_ZONE_BATAILLE <= 2)
                            {
                                bRetraite012 = true;
                            }
                            else
                            {
                                bRetraite345 = true;
                            }
                        }
                    }
                }
                if ((0 == nbUnites012 || 0 == nbUnites345 || bRetraite012 || bRetraite345)
                    && (nbUnites012 > 0 || nbUnites345 > 0)
                    && (ligneBataille.I_TOUR_FIN - ligneBataille.I_TOUR_DEBUT >= 2))
                {
                    if (nbUnites012 > 0 || bRetraite345)
                    {
                        bVictoire345 = false;
                    }
                    else
                    {
                        bVictoire012 = false;
                    }
                    if (bVictoire012) { ligneBataille.S_FIN = "VICTOIRE012"; }
                    if (bVictoire345) { ligneBataille.S_FIN = "VICTOIRE345"; }
                    if (bRetraite012) { ligneBataille.S_FIN = "RETRAITE012"; }
                    if (bRetraite345) { ligneBataille.S_FIN = "RETRAITE345"; }
                }
            }
        }

        public void MiseAjourVideo(Donnees.TAB_VIDEODataTable tableVideo)
        {
            int idCaseDebut, idCaseFin;
            //si on refait le même tour, on supprimer les anciennes données pour ne pas les avoir en double
            int i = 0;
            while (i < tableVideo.Count)
            {
                Donnees.TAB_VIDEORow ligneVideo = tableVideo[i];
                if (ligneVideo.I_TOUR == m_traitement)
                {
                    ligneVideo.Delete();
                }
                else
                {
                    i++;
                }
            }

            foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            {
                //if (lignePion.estMessager || lignePion.estPatrouille || lignePion.estRenfort || lignePion.estQG)
                if (lignePion.estMessager || lignePion.estPatrouille || lignePion.estRenfort)
                {
                        continue;
                }
                try
                {
                    lignePion.CasesDebutFin(out idCaseDebut, out idCaseFin);
                }
                catch (Exception eCasesDebutFin)
                {
                    //parfois il y a des erreurs de récupération
                    idCaseDebut = lignePion.ID_CASE;
                    Debug.WriteLine("Exception: " + eCasesDebutFin.ToString() + " pile:" + eCasesDebutFin.StackTrace);
                }
                if (idCaseDebut < 0) idCaseDebut = lignePion.ID_CASE;//possible si pas d'ordre en cours
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindParID_CASE(idCaseDebut);
                Donnees.TAB_VIDEORow ligneVideo = tableVideo.AddTAB_VIDEORow(
                    m_traitement,
                    lignePion.nation.ID_NATION,
                    lignePion.ID_PION,
                    lignePion.ID_MODELE_PION,
                    lignePion.ID_PION_PROPRIETAIRE,
                    lignePion.S_NOM,
                    lignePion.I_INFANTERIE,
                    lignePion.I_INFANTERIE_INITIALE,
                    lignePion.I_CAVALERIE,
                    lignePion.I_CAVALERIE_INITIALE,
                    lignePion.I_ARTILLERIE,
                    lignePion.I_ARTILLERIE_INITIALE,
                    lignePion.I_FATIGUE,
                    lignePion.I_MORAL,
                    idCaseDebut,
                    ligneCase.I_X,
                    ligneCase.I_Y,
                    lignePion.IsID_BATAILLENull() ? -1 : lignePion.ID_BATAILLE,
                    lignePion.B_DETRUIT,
                    lignePion.B_FUITE_AU_COMBAT,
                    lignePion.I_MATERIEL,
                    lignePion.I_RAVITAILLEMENT,
                    lignePion.B_BLESSES,
                    lignePion.B_PRISONNIERS,
                    lignePion.C_NIVEAU_DEPOT,
                    lignePion.IsI_VICTOIRENull() ? 0 : lignePion.I_VICTOIRE,
                    -1,//I_TYPE
                    lignePion.estConvoiDeRavitaillement,
                    lignePion.estQG,
                    lignePion.estPontonnier,
                    lignePion.estDepot
                );
                if (lignePion.IsID_BATAILLENull()) ligneVideo.SetID_BATAILLENull();
                ligneVideo.I_TYPE = (int) tipeVideo(ligneVideo);
            }

            foreach( Donnees.TAB_NOMS_CARTERow ligneNomCarte in Donnees.m_donnees.TAB_NOMS_CARTE)
            {
                if (ligneNomCarte.IsID_NATION_CONTROLENull() || ligneNomCarte.ID_NATION_CONTROLE<0)
                { 
                    continue;  
                }
                Donnees.TAB_VIDEORow ligneVideo = tableVideo.AddTAB_VIDEORow(
                    m_traitement,
                    ligneNomCarte.ID_NATION_CONTROLE,
                    -1,//lignePion.ID_PION,
                    -1,//lignePion.ID_MODELE_PION,
                    -1,//lignePion.ID_PION_PROPRIETAIRE,
                    ligneNomCarte.S_NOM,
                    0, //lignePion.I_INFANTERIE,
                    0, //lignePion.I_INFANTERIE_INITIALE,
                    0, //lignePion.I_CAVALERIE,
                    0, //lignePion.I_CAVALERIE_INITIALE,
                    0, //lignePion.I_ARTILLERIE,
                    0, //lignePion.I_ARTILLERIE_INITIALE,
                    0, //lignePion.I_FATIGUE,
                    0, //lignePion.I_MORAL,
                    -1,//idCaseDebut,
                    -1,//I_X
                    -1,//I_Y
                    -1, //lignePion.ID_BATAILLE,
                    false, //lignePion.B_DETRUIT,
                    false, //lignePion.B_FUITE_AU_COMBAT,
                    0, //lignePion.I_MATERIEL,
                    0, //lignePion.I_RAVITAILLEMENT,
                    false, //lignePion.B_BLESSES,
                    false, //lignePion.B_PRISONNIERS,
                    ' ',//lignePion.C_NIVEAU_DEPOT,
                    ligneNomCarte.I_VICTOIRE,
                    Constantes.NULLENTIER,//I_TYPE
                    false,
                    false, //lignePion.B_QG
                    false, //B_PONTONNIER
                    false
                );                
            }
        }

        public TIPEUNITEVIDEO tipeVideo(Donnees.TAB_VIDEORow ligneVideo)
        {
            if (ligneVideo.B_QG)
            {
                return TIPEUNITEVIDEO.QG;
            }
            if (ligneVideo.B_PRISONNIERS)
            {
                return TIPEUNITEVIDEO.PRISONNIER;
            }
            if (ligneVideo.B_BLESSES)
            {
                return TIPEUNITEVIDEO.BLESSE;
            }
            if (ligneVideo.B_CONVOI_RAVITAILLEMENT)
            {
                return TIPEUNITEVIDEO.CONVOI;
            }
            if (ligneVideo.B_DEPOT)
            {
                return TIPEUNITEVIDEO.DEPOT;
            }
            if (ligneVideo.I_INFANTERIE == 0 && ligneVideo.I_CAVALERIE == 0 && ligneVideo.I_ARTILLERIE > 0)
            {
                return TIPEUNITEVIDEO.ARTILLERIE;
            }
            if (ligneVideo.B_PONTONNIER)
            {
                return TIPEUNITEVIDEO.PONTONNIER;
            }
            //les renforts n'ont pas d'infanterie et de calaverie initiale
            if (0 == ligneVideo.I_INFANTERIE_INITIALE && (ligneVideo.I_CAVALERIE_INITIALE > 0 || ligneVideo.I_CAVALERIE>0))
            {
                return TIPEUNITEVIDEO.CAVALERIE;
            }
            if (ligneVideo.I_INFANTERIE_INITIALE > 0 || ligneVideo.I_INFANTERIE>0)
            {
                return TIPEUNITEVIDEO.INFANTERIE;
            }
            //LogFile.Notifier("Tipe video non reconnu pour le pion " + ligneVideo.ID_PION);
            return TIPEUNITEVIDEO.AUTRE;//Ce sont les lieux, hopitaux, villes, etc, donc pas à afficher
        }

        /// <summary>
        /// Calcul du nombre total de pts de victoire possibles sur la partie avec les unités et les noms de lieux
        /// </summary>
        private void CalculNombreTotalPointsDeVictoire()
        {
            System.Nullable<int> victoire =
                (from pion in Donnees.m_donnees.TAB_PION
                 select pion.I_VICTOIRE)
                .Sum();

            victoire +=
                (from nom_carte in Donnees.m_donnees.TAB_NOMS_CARTE
                 select nom_carte.I_VICTOIRE).Sum();

            if (Donnees.m_donnees.TAB_PARTIE[0].IsI_NB_TOTAL_VICTOIRENull())
            {
                Donnees.m_donnees.TAB_PARTIE[0].I_NB_TOTAL_VICTOIRE = victoire.Value;
            }
            else
            {
                Donnees.m_donnees.TAB_PARTIE[0].I_NB_TOTAL_VICTOIRE = Math.Max(victoire.Value, Donnees.m_donnees.TAB_PARTIE[0].I_NB_TOTAL_VICTOIRE);
            }
        }
    }
}
