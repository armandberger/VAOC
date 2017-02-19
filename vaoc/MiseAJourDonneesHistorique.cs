using System;
using System.Collections.Generic;
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

        public string Initialisation(string fichierSource, bool avecSauvegarde, System.ComponentModel.BackgroundWorker worker)
        {
            try
            {
                m_fichierSource = fichierSource;
                m_bAvecSauvegarde = avecSauvegarde;
                m_nombretours = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR + 1;

                m_travailleur = worker;
                m_traitement = 0;
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
                int idCaseDebut, idCaseFin;
                string nomfichier = Dal.NomFichierTourPhase(m_fichierSource, m_traitement, 0, false);
                //if (!Dal.ChargerPartie(Dal.NomFichierTourPhase(m_fichierSource, m_traitement, 0, false), Donnees.m_donnees))
                if (!Donnees.m_donnees.ChargerPartie(nomfichier))
                {
                        return "Erreur dans Dal.ChargerPartie, tour n°" + m_traitement.ToString();
                }

                int i = 0;
                while (i< Donnees.m_donnees.TAB_VIDEO.Count)
                {
                    Donnees.TAB_VIDEORow ligneVideo = Donnees.m_donnees.TAB_VIDEO[i];
                    if (ligneVideo.I_TOUR== m_traitement)
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
                    lignePion.CasesDebutFin(out idCaseDebut, out idCaseFin);
                    Donnees.TAB_VIDEORow ligneVideo = Donnees.m_donnees.TAB_VIDEO.AddTAB_VIDEORow(
                        m_traitement,
                        lignePion.ID_PION,
                        lignePion.ID_MODELE_PION,
                        lignePion.ID_PION_PROPRIETAIRE,
                        lignePion.S_NOM,
                        lignePion.I_INFANTERIE,
                        lignePion.I_CAVALERIE,
                        lignePion.I_ARTILLERIE,
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
                        lignePion.C_NIVEAU_DEPOT
                    );
                    if (lignePion.IsID_BATAILLENull()) ligneVideo.SetID_BATAILLENull();
                }

                if (m_bAvecSauvegarde)
                {
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
    }
}
