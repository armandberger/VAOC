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
        System.ComponentModel.BackgroundWorker m_travailleur;
        private Donnees.TAB_VIDEODataTable m_tableVideo = null;

        public string Initialisation(string fichierSource, bool avecSauvegarde, int debut, System.ComponentModel.BackgroundWorker worker)
        {
            try
            {
                m_fichierSource = fichierSource;
                m_bAvecSauvegarde = avecSauvegarde;
                m_nombretours = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR + 1;
                m_travailleur = worker;
                m_traitement = (0==debut) ? 0 : debut-1;//-1 pour reprendre les anciennes données, on reexcute donc le précédent tour
                m_tableVideo = new Donnees.TAB_VIDEODataTable();
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
                    return "Erreur dans Dal.ChargerPartie, tour n°" + m_traitement.ToString();
                }

                if (0 == m_tableVideo.Rows.Count)
                {
                    m_tableVideo.Merge(Donnees.m_donnees.TAB_VIDEO);
                }

                MiseAjourVideo(m_tableVideo);

                if (m_bAvecSauvegarde)
                {
                    Donnees.m_donnees.TAB_VIDEO.Clear();
                    Donnees.m_donnees.TAB_VIDEO.Merge(m_tableVideo);
                    Donnees.m_donnees.SauvegarderPartie(nomfichier, m_traitement, 0, false);
                    //Dal.SauvegarderPartie(m_fichierSource, m_traitement, 0, Donnees.m_donnees);
                }

                m_traitement++;
                m_travailleur.ReportProgress(m_traitement * 100 / m_nombretours);
                if (m_traitement == Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)
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
                if (lignePion.estMessager || lignePion.estPatrouille || lignePion.estRenfort || lignePion.estQG)
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
                    lignePion.IsID_BATAILLENull() ? -1 : lignePion.ID_BATAILLE,
                    lignePion.B_DETRUIT,
                    lignePion.B_FUITE_AU_COMBAT,
                    lignePion.I_MATERIEL,
                    lignePion.I_RAVITAILLEMENT,
                    lignePion.B_BLESSES,
                    lignePion.B_PRISONNIERS,
                    lignePion.C_NIVEAU_DEPOT,
                    lignePion.I_VICTOIRE
                );
                if (lignePion.IsID_BATAILLENull()) ligneVideo.SetID_BATAILLENull();
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
                    -1, //lignePion.ID_BATAILLE,
                    false, //lignePion.B_DETRUIT,
                    false, //lignePion.B_FUITE_AU_COMBAT,
                    0, //lignePion.I_MATERIEL,
                    0, //lignePion.I_RAVITAILLEMENT,
                    false, //lignePion.B_BLESSES,
                    false, //lignePion.B_PRISONNIERS,
                    ' ',//lignePion.C_NIVEAU_DEPOT,
                    ligneNomCarte.I_VICTOIRE
                );

            }
        }
    }
}
