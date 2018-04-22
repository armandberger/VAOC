using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

using WaocLib;
using System.Threading.Tasks;
using System.Threading;

namespace vaoc
{
    class rechercheQG
    {
        public int idPion;
        public bool bEngagee;
        public int izoneEngagement;
    }

    class ClassTraitementHeure
    {
        protected InterfaceVaocWeb m_iWeb;

        /// <summary>
        /// true si une bataille s'est teminée dans le tour, false sinon, sert à savoir si l'on fait un tour de plus
        /// </summary>
        protected bool m_bFinDeBataille;

        public ClassTraitementHeure()
        {
        }

        public bool TraitementHeure(string fichierCourant, System.ComponentModel.BackgroundWorker travailleur, out string messageErreur)
        {
            string message;
            int nbPhases;
            int i;
            bool bFinDePartie;
            bool bTourSuivant;//tant que cela vaut true, on continue l'execution
            int nbTourExecutes;//nombre de tours executés successivement
            bool bRenfort = false;
            bool bDebutDeNuit = false;

            Debug.WriteLine("TraitementHeure");
            messageErreur = "";

            LogFile.CreationLogFile(fichierCourant, "tour", Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, -1);
            m_iWeb = ClassVaocWebFactory.CreerVaocWeb(fichierCourant, false);

            //TestCreation();
            AmeliorationsPerformances();

            //IEnumerable<Donnees.TAB_PIONRow> test = Donnees.m_donnees.TAB_NATION.CommandantEnChef(0);
            //test = Donnees.m_donnees.TAB_NATION.CommandantEnChef(1);
            //test = Donnees.m_donnees.TAB_NATION.CommandantEnChef(0);
            //test = Donnees.m_donnees.TAB_NATION.CommandantEnChef(1);
            //test = Donnees.m_donnees.TAB_NATION.CommandantEnChef(0);
            //test = Donnees.m_donnees.TAB_NATION.CommandantEnChef(1);

            //On determine l'heure de levée et de coucher du soleil d'après le mois en cours
            int moisEnCours = ClassMessager.DateHeure().Month;
            Donnees.m_donnees.TAB_JEU[0].I_LEVER_DU_SOLEIL = Constantes.tableHeuresLeveeDuSoleil[moisEnCours];
            Donnees.m_donnees.TAB_JEU[0].I_COUCHER_DU_SOLEIL = Constantes.tableHeuresCoucherDuSoleil[moisEnCours];

            //On note la dernière fois que l'on a envoyé des messages aux joueurs
            //Donnees.m_donnees.TAB_PARTIE[0].I_TOUR_NOTIFICATION = (Donnees.m_donnees.TAB_PARTIE[0].IsI_TOUR_NOTIFICATIONNull()) ? 1 : Donnees.m_donnees.TAB_PARTIE[0].I_TOUR; -> on ne peut pas le faire là au cas où le tour serait pris en cours de route, déplacé dans la dialogue de notification

            //on va rechercher les nouveaux ordres ou les nouveaux messages (quand la partie n'est pas commence)
            if (!NouveauxMessages()) { LogFile.Notifier("Erreur rencontrée dans NouveauxMessages()"); return false; }
            if (Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE)
            {
                if (!NouveauxOrdres(out nbTourExecutes)) { LogFile.Notifier("Erreur rencontrée dans NouveauxOrdres()"); return false; }
            }
            else
            {
                nbTourExecutes = 0;
            }

            //pour la toute première phase on envoit un message d'initialisation des positions pour les pions des joueurs
            if (0 == Donnees.m_donnees.TAB_MESSAGE.Count)
            //if (0 == DataSetCoutDonnees.m_donnees.TAB_PARTIE[0].I_PHASE && 0 == DataSetCoutDonnees.m_donnees.TAB_PARTIE[0].I_TOUR)
            {
                //on initialise également la météo
                Donnees.m_donnees.TAB_PARTIE[0].ID_METEO = Donnees.m_donnees.TAB_PARTIE[0].ID_METEO_INITIALE;
                //on envoit un message avec effet immédiat pour prévenir le joueur de l'arrivée de la troupe
                for(int l=0; l< Donnees.m_donnees.TAB_PION.Count;l++)
                {
                    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[l];
                    if (lignePion.B_DETRUIT) { continue; }//plutot curieux une unité détruite en début de partie mais...
                    lignePion.B_TELEPORTATION = true;//le pion arrive sur la carte
                }
            }

            #region Teleportation, pions nouvellement arrivés ou modification de la position par l'arbitre (pour le placement initial par exemple)
            for (int l = 0; l < Donnees.m_donnees.TAB_PION.Count; l++)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[l];
                if (lignePion.B_DETRUIT || !lignePion.B_TELEPORTATION) { continue; }
                //s'il y a un ordre courant, il faut changer la case de depart
                Donnees.TAB_ORDRERow ligneOrdreCourant = Donnees.m_donnees.TAB_ORDRE.Courant(lignePion.ID_PION);
                if (null != ligneOrdreCourant) 
                {
                    ligneOrdreCourant.ID_CASE_DEPART = lignePion.ID_CASE;
                    //je suis obligé de réaffecter les effectifs, sinon, si l'unité est téléporté plus proche de son lieu d'arrivée, je peux avoir un crash pour ne pas être capable de placer toute
                    //l'unité sur le "chemin" du parcours
                    ligneOrdreCourant.I_EFFECTIF_DEPART = lignePion.effectifTotalEnMouvement;
                    ligneOrdreCourant.I_EFFECTIF_DESTINATION = 0;
                }
                if (lignePion.estMessager || lignePion.estPatrouille)
                {
                    lignePion.B_TELEPORTATION = false;//pas de message pour ce type de pion
                    continue;
                }
                if (!ClassMessager.EnvoyerMessageImmediat(lignePion, ClassMessager.MESSAGES.MESSAGE_RENFORT))
                {
                    throw new NotImplementedException("Il FAUT paramétrer un message de type RENFORT de manière à avoir un message de position pour toutes les unités dés le départ.");
                }
                lignePion.DetruireEspacePion();//force, par la suite, le recalcul de tous les espaces, parcours, etc.
                lignePion.B_TELEPORTATION = false;
            }
            #endregion

            #region transfert correctif
            /*
            Donnees.TAB_PIONRow lignePionAncien = Donnees.m_donnees.TAB_PION.FindByID_PION(2016);
            Donnees.TAB_PIONRow lignePionNouveau = Donnees.m_donnees.TAB_PION.FindByID_PION(6);
            TransfertsDesliens(lignePionAncien, lignePionNouveau);
            Donnees.TAB_ROLERow ligneRole = Donnees.m_donnees.TAB_ROLE.FindByID_ROLE(8);
            ligneRole.ID_PION = 6;
            lignePionAncien.DetruirePion();
            */
            #endregion

            CalculNombreTotalPointsDeVictoire();

            //pour chaque phase
            nbPhases = (Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE) ? Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES : Donnees.m_donnees.TAB_PARTIE[0].I_PHASE + 1;

            //le nombre de tours déjà executé est égal au tour courant moins le fichier de tour du fichier XML
            //InterfaceVaocWeb iWeb;
            //iWeb = ClassVaocWebFactory.CreerVaocWeb(fichierCourant, false);
            //List<ClassDataPartie> liste = iWeb.ListeParties(Donnees.m_donnees.TAB_PARTIE[0].ID_JEU, Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);
            //nbTourExecutes = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR - liste[0].I_TOUR;
            //Donnees.m_donnees.ChargerToutesLesCases();//uniquement pour test, BEA -> provoque des crashs si on se déplace sur la carte en même temps, à éviter
            bTourSuivant = true;
            bDebutDeNuit = Donnees.m_donnees.TAB_PARTIE.Nocturne();
            while (bTourSuivant)
            {
                m_bFinDeBataille = false;
                if (0 == Donnees.m_donnees.TAB_PARTIE.HeureCourante())
                {
                    //première heure de la journée
                    if (!NouveauJour())
                    {
                        return false;
                    }
                }

                //Donnees.m_donnees.TAB_PARTIE[0].I_PHASE = 95;//BEA, permet de tester une fin de bataille
                while (Donnees.m_donnees.TAB_PARTIE[0].I_PHASE < nbPhases)
                {
                    //Initialisation de la phase
                    LogFile.CreationLogFile(fichierCourant, "tour", Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE);
                    message = string.Format("TraitementHeure : début phase={0}", Donnees.m_donnees.TAB_PARTIE[0].I_PHASE);
                    if (!LogFile.Notifier(message, out messageErreur))
                    {
                        return false;
                    }

                    //construction de la carte pour les images liés aux messages
                    if (!Cartographie.ChargerLesFichiers()) { return false; }

                    //on reconstruit la carte graphique à chaque étape pour voir la progression à l'écran
                    //ca ne marche pas, alors autant gagner du temps
                    //Cartographie.ConstructionCarte();
                    //Cartographie.AfficherUnites(Cartographie.modeleCarte.HISTORIQUE);
                    //Cartographie.AfficherUnites(Cartographie.modeleCarte.ZOOM);

                    //Traitement de la phase
                    if (0 == Donnees.m_donnees.TAB_PARTIE[0].I_PHASE ||
                        false == Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE)
                    {
                        if (!NouvelleHeure(Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE,out bRenfort)) { return false; }
                    }

                    //on place toutes les unités statiques
                    message = string.Format("**** Placement des unités statiques ****");
                    if (!LogFile.Notifier(message, out messageErreur)) { return false; }

                    DateTime timeStart;
                    TimeSpan perf;
                    timeStart = DateTime.Now;
                    if (!Cartographie.PlacerLesUnitesStatiques())
                    {
                        messageErreur = "Erreur durant le traitement PlacerLesUnitesStatiques";
                        return false;
                    }
                    perf = DateTime.Now - timeStart;
                    Debug.WriteLine(string.Format("PlacerLesUnitesStatiques en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));

                    //timeStart = DateTime.Now;
                    //if (!Cartographie.PlacerLesUnitesStatiquesParallele())
                    //{
                    //    messageErreur = "Erreur durant le traitement PlacerLesUnitesStatiquesParallele";
                    //    return false;
                    //}
                    //perf = DateTime.Now - timeStart;
                    //Debug.WriteLine(string.Format("PlacerLesUnitesStatiquesParallele en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));


                    //on execute les mouvements
                    #region Mouvement de toutes les unités avec effectifs (les unités de combat en fait)
                    message = string.Format("**** Mouvements : toutes les unités AVEC effectifs ****");
                    if (!LogFile.Notifier(message, out messageErreur))
                    {
                        return false;
                    }
                    /* methode standard
                    timeStart = DateTime.Now;
                    i = 0;
                    while (i < Donnees.m_donnees.TAB_PION.Count())
                    {
                        Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                        if (lignePion.B_DETRUIT) { ++i; continue; }
                        if (lignePion.effectifTotal > 0)
                        {
                            if (!ExecuterMouvement(lignePion, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE))
                            {
                                messageErreur = "Erreur durant le traitement ExecuterMouvement";
                                return false;
                            }
                        }
                        ++i;
                    }
                    perf = DateTime.Now - timeStart;
                    Debug.WriteLine(string.Format("ExecuterMouvement en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
                     * */
                    timeStart = DateTime.Now;
                    if (!ExecuterMouvementAvecEffectifsEnParallele())
                    {
                        messageErreur = "Erreur durant le traitement ExecuterMouvementAvecEffectifsEnParallele";
                        LogFile.Notifier(messageErreur);
                        return false;
                    }
                    perf = DateTime.Now - timeStart;
                    Debug.WriteLine(string.Format("ExecuterMouvementAvecEffectifsEnParallele en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
                    #endregion

                    #region Mouvement de toutes les unités sans effectif (QG, messagers)
                    message = string.Format("**** Mouvements : toutes les unités SANS effectif ****");
                    if (!LogFile.Notifier(message, out messageErreur))
                    {
                        return false;
                    }

                    /* Version Standard
                    i = 0;
                    while (i < Donnees.m_donnees.TAB_PION.Count())
                    {
                        Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                        if (lignePion.B_DETRUIT) { i++; continue; }
                        if (lignePion.effectifTotal == 0)
                        {
                            if (!ExecuterMouvement(lignePion, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE))
                            {
                                messageErreur = "Erreur durant le traitement ExecuterMouvement";
                                return false;
                            }
                        }
                        i++;
                    }
                    */
                    timeStart = DateTime.Now;
                    if (!ExecuterMouvementSansEffectifsEnParallele())
                    {
                        messageErreur = "Erreur durant le traitement ExecuterMouvementSansEffectifsEnParallele";
                        LogFile.Notifier(messageErreur);
                        return false;
                    }
                    perf = DateTime.Now - timeStart;
                    Debug.WriteLine(string.Format("ExecuterMouvementSansEffectifsEnParallele en {0} heures, {1} minutes, {2} secondes, {3} millisecondes", perf.Hours, perf.Minutes, perf.Seconds, perf.Milliseconds));
                    #endregion

                    #region Toutes les actions qui ne sont pas des mouvements
                    /* Version Standard
                    i = 0;
                    while (i < Donnees.m_donnees.TAB_PION.Count())
                    {
                        Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                        if (lignePion.B_DETRUIT) { i++; continue; }
                        if (!ExecuterOrdreHorsMouvement(lignePion, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE))
                        {
                            messageErreur = "Erreur durant le traitement ExecuterMouvement";
                            return false;
                        }
                        i++;
                    }
                    */

                    if (!ExecuterActionHorsMouvementEnParallele())
                    {
                        LogFile.Notifier("Erreur : ExecuterActionHorsMouvementEnParallele renvoie false");
                        return false;
                    }
                    #endregion

                    /************* rendre actif l'ordre suivant
                 * L'idée était de vérifier si un ordre avait été terminé durant cette phase, si oui et qu'il avait un ID_ORDRE_SUIVANT le rend actif
                 * mais qu'un ce qu'un ordre actif si ce n'est le premier ordre non terminé ? Donc, a priori, il n'y a rien à faire à tester quand même :-)
                 * --> et il faut quand même envoyer un message pour prévenir du changement d'ordre
                 * --> et en plus c'est pas vrai, il faut indiquer i_tour_debut/i_phase_debut dans l'ordre, tout est géré dans TerminerOrdreCourant (sur PIONRow)
                 *********************************/
                    LogFile.Notifier("Mise à jour des propriétaires des cases");
                    MiseAJourProprietaires();
                    if (0 == Donnees.m_donnees.TAB_PARTIE[0].I_PHASE % Constantes.CST_SAUVEGARDE_ECART_PHASES)
                    {
                        if ((0 != Donnees.m_donnees.TAB_PARTIE[0].I_PHASE))
                        {
                            //au cas où il y aurait un chargement de case par la souris, la collection va changée, provoquant un crash
                            Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                            //Donnees.m_donnees.SauvegarderPartie(fichierCourant, true); //-> prend quand même près de dix minutes !
                            Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                        }

                        ClassTraitementWeb webRoles = new ClassTraitementWeb(fichierCourant);
                        if (!webRoles.GenerationWebFichiersRoles(true))
                        {
                            LogFile.Notifier("Erreur durant la génération des fichiers roles Web. Consultez le fichier de log");
                            return false;
                        }
                    }

                    if (99 == Donnees.m_donnees.TAB_PARTIE[0].I_PHASE/* || 60 == Donnees.m_donnees.TAB_PARTIE[0].I_PHASE*/)
                    {
                        //au cas où il y aurait un chargement de case par la souris, la collection va changée, provoquant un crash
                        Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                        Donnees.m_donnees.SauvegarderPartie(fichierCourant, true); //-> prend quand même près de dix minutes !
                        Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                    }

                    Donnees.m_donnees.TAB_PARTIE[0].I_PHASE++;
                    travailleur.ReportProgress(0);//toujours zero durant le traitement, car affichage basé differement d'un pourcentage
                    LogFile.Notifier("Fin de la phase");
                }

                /************ A la fin de l'heure ****************/
                //on regarde toutes les unités qui participent à un combat
                for (int l=0; l < Donnees.m_donnees.TAB_BATAILLE.Count; l++)
                {
                    Donnees.TAB_BATAILLERow ligneBataille = Donnees.m_donnees.TAB_BATAILLE[l];
                    if (!ligneBataille.EffectuerBataille(out m_bFinDeBataille))
                    {
                        messageErreur = "Erreur durant le traitement EffectuerBataille";
                        LogFile.Notifier(messageErreur);
                        return false;
                    }
                }

                // Une unité envoie un message régulièrement, ça rassure le joueur s'il n'est pas proche de ses troupes !
                i = 0;
                while (i < Donnees.m_donnees.TAB_PION.Count())
                {
                    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                    if (!lignePion.B_DETRUIT && !lignePion.B_TELEPORTATION)
                    {
                        if (!lignePion.estMessager && !lignePion.estPatrouille && !lignePion.estDepot)
                        {
                            Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageEmis(lignePion.ID_PION);
                            if (null == ligneMessage ||
                                ligneMessage.I_TOUR_DEPART + ClassMessager.CST_MESSAGE_FREQUENCE_ALERTE < Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)
                            {
                                if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_POSITION))
                                {
                                    return false;
                                }
                            }
                            //on en profite aussi pour voir si l'unité n'a pas rejoint un hopital ou une prison
                            //if (!lignePion.ArriveeSurPrisonOuHopital()) { return false; } -> ne pas le faire là, car sinon, un convoi de prisonniers s'arrête dès qu'il rencontre une prison, déplacée à la création de colonne de prisonniers ou de blessés
                        }
                    }
                    i++;
                }

                // On regarde si toutes les unités sont bien ravitaillées
                //pas à faire toutes les heures, mais seulement en fin de journée et avec une liaison sur les dépôts que si l'unité n'a pas bougée
                //cela est donc effectué dans FinDuJour
                //if (!Ravitaillement())
                //{
                //    messageErreur = "Erreur durant le traitement Ravitaillement";
                //    return false;
                //}

                //on met à jour les données pour le film vidéo de fin de partie
                MiseAJourDonneesHistorique majVideo = new MiseAJourDonneesHistorique();
                majVideo.Initialisation(string.Empty, false, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, null);
                majVideo.MiseAjourVideo(Donnees.m_donnees.TAB_VIDEO);

                //reconstruction de la carte suite aux batailles éventuelles, pour l'affichage générale
                message = string.Format("**** Reconstruction carte ****");
                if (!LogFile.Notifier(message, out messageErreur)) { return false; }
                if (!Cartographie.ConstructionCarte())
                {
                    messageErreur = "Erreur durant le traitement ConstructionCarte";
                    return false;
                }

                //a la fin de la journée, on met à jour le moral, la fatigue et on envoit un rapport

                if (23 == Donnees.m_donnees.TAB_PARTIE.HeureCourante())
                {
                    if (!FinDuJour())
                    {
                        messageErreur = "Erreur durant le traitement FinDuJour";
                        return false;
                    }
                }

                //Cartographie.MiseAJourProprietaires(); -> BEA surtout ne pas faire car déjà fait en fin de phase, sinon nouveau->proprio (fin de phase) puis proprio ->vide (ce cout là)

                message = string.Format("TraitementHeure : fin des traitements ************************************");
                if (!LogFile.Notifier(message, out messageErreur))
                {
                    return false;
                }

                if (Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE)
                {
                    Donnees.m_donnees.TAB_PARTIE[0].I_TOUR++;
                    Donnees.m_donnees.TAB_PARTIE[0].I_PHASE = 0;
                }
                else
                {
                    //avant le demmarage, le temps avance d'une phase à chaque échange
                    //ce qui est traité dans la boucle while initiale
                }

                //On vérifie si la partie est terminée et, si oui, on determine le gagnant
                if (!VictoireDefaite(out bFinDePartie))
                {
                    messageErreur = "Erreur durant le traitement VictoireDefaite";
                    return false;
                }

                //suppression des tables d'optimisation
                //Cartographie.NettoyageBase();, bof, finalement, ça sert aussi pour générer les fichiers web ensuite alors autant les garder

                //génération des fichiers web et sql
                //on vient de terminer un tour, on lance la génération des fichiers web et sql
                ClassTraitementWeb web = new ClassTraitementWeb(fichierCourant);
                if (!web.GenerationWeb())
                {
                    LogFile.Notifier("Erreur durant la génération des fichiers Web. Consultez le fichier de log");
                    return false;
                }

                if (!miseÀJourInternet(fichierCourant, out messageErreur))
                {
                    LogFile.Notifier("Erreur durant la génération des fichiers SQL :" + messageErreur);
                    return false;
                }

                nbTourExecutes++;
                #region maintenant on regarde si l'on fait un tour de plus ou pas
                if (bFinDePartie 
                    || !Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE
                    || 1==Donnees.m_donnees.TAB_PARTIE[0].I_TOUR //premier lancement, on ne continue pas pour que les joueurs donnent leurs ordres
                    || (nbTourExecutes >= 8) 
                    || (Donnees.m_donnees.TAB_PARTIE.HeureCourante() == Donnees.m_donnees.TAB_JEU[0].I_LEVER_DU_SOLEIL) 
                    || (bRenfort)
                    || m_bFinDeBataille //si une bataille vient de se terminer,on ne continue pas pour laisser au vaincu le temps de s'enfuir
                    )
                {
                    //pas plus de 8 tours successifs et l'on s'arrête toujours au lever du soleil ou s'il y a des renforts ou si la partie est terminée
                    LogFile.Notifier("pas plus de 8 tours successifs et l'on s'arrête toujours au lever du soleil ou a une heure du matin (renforts potentiels) ou si la partie est terminée");
                    if (!Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE)
                    {
                        LogFile.Notifier("Fin de tour :Partie pas demarrée");
                    }
                    if(1 == Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)
                    {
                        LogFile.Notifier("Fin de tour :premier lancement, on ne continue pas pour que les joueurs donnent leurs ordres");
                    }
                    if (nbTourExecutes >= 8 && (!bDebutDeNuit || !Donnees.m_donnees.TAB_PARTIE.Nocturne()))
                    {
                        LogFile.Notifier("Fin de tour :Pas plus de 8 tours en journée");
                    }
                    if (Donnees.m_donnees.TAB_PARTIE.HeureCourante() == Donnees.m_donnees.TAB_JEU[0].I_LEVER_DU_SOLEIL)
                    {
                        LogFile.Notifier("Fin de tour :Levée du jour");
                    }
                    if (bRenfort)
                    {
                        LogFile.Notifier("Fin de tour :Arrivée de renforts");
                    }
                    if (m_bFinDeBataille)
                    {
                        LogFile.Notifier("Fin de tour :si une bataille vient de se terminer, avec un leader dirigeant la bataille,on ne continue pas pour laisser au vaincu le temps de s'enfuir et d'être poursuivi");
                    }
                    bTourSuivant = false;
                }
                else
                {
                    //Si une bataille est en cours et qu'il fait jour on ne fait jamais plus de deux heures de suite
                    if (!Donnees.m_donnees.TAB_PARTIE.Nocturne() && nbTourExecutes >= 2)
                    {
                        for (int l=0; l< Donnees.m_donnees.TAB_BATAILLE.Count; l++)
                        {
                            Donnees.TAB_BATAILLERow ligneBataille = Donnees.m_donnees.TAB_BATAILLE[l];
                            if (ligneBataille.IsI_TOUR_FINNull())
                            {
                                bTourSuivant = false;
                            }
                        }
                    }

                    if (bTourSuivant)
                    {
                        double distanceMin, distanceMinEnMouvement;
                        Donnees.TAB_PIONRow lignePionMin1, lignePionMin2, lignePionMax1, lignePionMax2;
                        DistanceMinimaleEntreEnnemis(out distanceMin, out distanceMinEnMouvement, 
                                                    out lignePionMin1, out lignePionMin2,
                                                    out lignePionMax1, out lignePionMax2);
                        LogFile.Notifier(string.Format("Test de tours contigus : nb tours consécutifs={0} Distance entre deux unités ennemies : {1} ({3}:{4} <-> {5}:{6}) Distance entre deux unités ennemies en mouvement : {2} ({7}:{8} <-> {9}:{10})",
                           nbTourExecutes, 
                           distanceMin / Donnees.m_donnees.TAB_JEU[0].I_ECHELLE, 
                           distanceMinEnMouvement / Donnees.m_donnees.TAB_JEU[0].I_ECHELLE,
                           lignePionMin1.ID_PION,
                           lignePionMin1.S_NOM,
                           lignePionMin2.ID_PION,//5
                           lignePionMin2.S_NOM, 
                           lignePionMax1.ID_PION,
                           lignePionMax1.S_NOM, 
                           lignePionMax2.ID_PION,
                           lignePionMax2.S_NOM));

                        if (distanceMin < 30 * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE)
                        {
                            //A moins de 30 kilomètres, on ne fait pas plus de deux heures de suite le jour, 4 tours la nuit
                            if (nbTourExecutes >= 4 && (!bDebutDeNuit || !Donnees.m_donnees.TAB_PARTIE.Nocturne()))
                            {
                                bTourSuivant = false;
                                LogFile.Notifier("Fin de tour :A moins de 30 kilomètres, on ne fait pas plus de 4 tours la nuit sauf s'il y a déjà eu un tour la nuit");
                            }
                            else
                            {
                                if (!Donnees.m_donnees.TAB_PARTIE.Nocturne() && nbTourExecutes >= 2)
                                {
                                    bTourSuivant = false;
                                    LogFile.Notifier("Fin de tour :A moins de 30 kilomètres, on ne fait pas plus de deux heures de suite le jour");
                                }
                            }
                            // sauf si le prochain tour est le lever du soleil ou la nuit ou la première heure, dans ce cas, on fait encore une heure de plus
                            if ((Donnees.m_donnees.TAB_PARTIE.HeureCourante() + 1 == Donnees.m_donnees.TAB_JEU[0].I_LEVER_DU_SOLEIL) ||
                                (Donnees.m_donnees.TAB_PARTIE.HeureCourante() + 1 == Donnees.m_donnees.TAB_JEU[0].I_COUCHER_DU_SOLEIL) ||
                                (Donnees.m_donnees.TAB_PARTIE.HeureCourante() + 1 == 1))
                            {
                                bTourSuivant = true;
                                LogFile.Notifier("Fin de tour :+1 tour pour finir au lever du soleil ou à la première heure du jour");
                            }
                        }
                        else
                        {
                            //A moins de 50 kilomètres, on ne fait pas plus de quatre heures de suite et pas plus d'un tour la nuit
                            if ((distanceMin < 50 * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE) && (nbTourExecutes >= 4) && (!bDebutDeNuit || !Donnees.m_donnees.TAB_PARTIE.Nocturne()))
                            {
                                bTourSuivant = false;
                                LogFile.Notifier("Fin de tour :A moins de 50 kilomètres, on ne fait pas plus de quatre heures de suite");
                            }
                        }
                    }
                }
                #endregion

                //dernière sauvegarde pour demarrer au tour suivant
                //au cas où il y aurait un chargement de case par la souris, la collection va changée, provoquant un crash
                Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                Donnees.m_donnees.SauvegarderPartie(fichierCourant, bTourSuivant);//remet toutes cases à vides, donc ne marche plus ensuite pour la génération de cartes
                Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
                travailleur.ReportProgress(100);//c'est la fin de l'heure courante 

                nbPhases = Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES;//le nombre de phases peut juste différer au premier tour, pas aux suivants
            }
            return true;
        }

        private void AmeliorationsPerformances()
        {
            #region Backup des messages, ordres et pions détruits ou passés
            /* Retrait de l'optimisation sur les messages, en effets, certains pouvant être crées sur le web d'autres par l'interface, cela crée des bugs d'identifiants.
            int i = 0;
            while (i < Donnees.m_donnees.TAB_MESSAGE.Count())
            {
                Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE[i];
                if (!ligneMessage.IsI_TOUR_ARRIVEENull())
                {
                    Donnees.TAB_MESSAGE_ANCIENRow ligneMessageAncien = Donnees.m_donnees.TAB_MESSAGE_ANCIEN.AddTAB_MESSAGE_ANCIENRow(
                        ligneMessage.ID_MESSAGE,
                        ligneMessage.ID_PION_EMETTEUR,
                        ligneMessage.ID_PION_PROPRIETAIRE,
                        ligneMessage.I_TYPE,
                        ligneMessage.IsI_TOUR_ARRIVEENull() ? -1 : ligneMessage.I_TOUR_ARRIVEE,
                        ligneMessage.IsI_PHASE_ARRIVEENull() ? -1 : ligneMessage.I_PHASE_ARRIVEE,
                        ligneMessage.IsI_TOUR_DEPARTNull() ? -1 : ligneMessage.I_TOUR_DEPART,
                        ligneMessage.IsI_PHASE_DEPARTNull() ? -1 : ligneMessage.I_PHASE_DEPART,
                        ligneMessage.IsS_TEXTENull() ? "" : ligneMessage.S_TEXTE,
                        ligneMessage.IsI_INFANTERIENull() ? -1 : ligneMessage.I_INFANTERIE,
                        ligneMessage.IsI_CAVALERIENull() ? -1 : ligneMessage.I_CAVALERIE,
                        ligneMessage.IsI_ARTILLERIENull() ? -1 : ligneMessage.I_ARTILLERIE,
                        ligneMessage.IsI_FATIGUENull() ? -1 : ligneMessage.I_FATIGUE,
                        ligneMessage.IsI_MORALNull() ? -1 : ligneMessage.I_MORAL,
                        ligneMessage.IsI_TOUR_SANS_RAVITAILLEMENTNull() ? -1 : ligneMessage.I_TOUR_SANS_RAVITAILLEMENT,
                        ligneMessage.IsID_BATAILLENull() ? -1 : ligneMessage.ID_BATAILLE,
                        ligneMessage.IsI_ZONE_BATAILLENull() ? -1 : ligneMessage.I_ZONE_BATAILLE,
                        ligneMessage.IsI_RETRAITENull() ? -1 : ligneMessage.I_RETRAITE,
                        ligneMessage.B_DETRUIT,
                        ligneMessage.ID_CASE,
                        ligneMessage.IsID_CASE_DEBUTNull() ? -1 : ligneMessage.ID_CASE_DEBUT,
                        ligneMessage.IsID_CASE_FINNull() ? -1 : ligneMessage.ID_CASE_FIN,
                        ligneMessage.IsI_NB_PHASES_MARCHE_JOURNull() ? -1 : ligneMessage.I_NB_PHASES_MARCHE_JOUR,
                        ligneMessage.IsI_NB_PHASES_MARCHE_NUITNull() ? -1 : ligneMessage.I_NB_PHASES_MARCHE_NUIT,
                        ligneMessage.IsI_NB_HEURES_COMBATNull() ? -1 : ligneMessage.I_NB_HEURES_COMBAT,
                        ligneMessage.IsI_MATERIELNull() ? -1 : ligneMessage.I_MATERIEL,
                        ligneMessage.IsI_RAVITAILLEMENTNull() ? -1 :ligneMessage.I_RAVITAILLEMENT,
                        ligneMessage.IsI_SOLDATS_RAVITAILLESNull() ? -1 :ligneMessage.I_SOLDATS_RAVITAILLES,
                        ligneMessage.IsI_NB_HEURES_FORTIFICATIONNull() ? -1 : ligneMessage.I_NB_HEURES_FORTIFICATION,
                        ligneMessage.IsI_NIVEAU_FORTIFICATIONNull() ? -1 : ligneMessage.I_NIVEAU_FORTIFICATION,
                        ligneMessage.IsI_DUREE_HORS_COMBATNull() ? -1 : ligneMessage.I_DUREE_HORS_COMBAT,
                        ligneMessage.IsI_TOUR_BLESSURENull() ? -1 : ligneMessage.I_TOUR_BLESSURE,
                        ligneMessage.IsC_NIVEAU_DEPOTNull() ? 'X' : ligneMessage.C_NIVEAU_DEPOT
                        );

                    if (ligneMessage.IsI_TOUR_ARRIVEENull()) { ligneMessageAncien.SetI_TOUR_ARRIVEENull(); }
                    if (ligneMessage.IsI_PHASE_ARRIVEENull()) { ligneMessageAncien.SetI_PHASE_ARRIVEENull(); }
                    if (ligneMessage.IsI_TOUR_DEPARTNull()) { ligneMessageAncien.SetI_TOUR_DEPARTNull(); }
                    if (ligneMessage.IsI_PHASE_DEPARTNull()) { ligneMessageAncien.SetI_PHASE_DEPARTNull(); }
                    if (ligneMessage.IsS_TEXTENull()) { ligneMessageAncien.SetS_TEXTENull(); }
                    if (ligneMessage.IsI_INFANTERIENull()) { ligneMessageAncien.SetI_INFANTERIENull(); }
                    if (ligneMessage.IsI_CAVALERIENull()) { ligneMessageAncien.SetI_CAVALERIENull(); }
                    if (ligneMessage.IsI_ARTILLERIENull()) { ligneMessageAncien.SetI_ARTILLERIENull(); }
                    if (ligneMessage.IsI_FATIGUENull()) { ligneMessageAncien.SetI_FATIGUENull(); }
                    if (ligneMessage.IsI_MORALNull()) { ligneMessageAncien.SetI_MORALNull(); }
                    if (ligneMessage.IsI_TOUR_SANS_RAVITAILLEMENTNull()) { ligneMessageAncien.SetI_TOUR_SANS_RAVITAILLEMENTNull(); }
                    if (ligneMessage.IsID_BATAILLENull()) { ligneMessageAncien.SetID_BATAILLENull(); }
                    if (ligneMessage.IsI_ZONE_BATAILLENull()) { ligneMessageAncien.SetI_ZONE_BATAILLENull(); }
                    if (ligneMessage.IsI_RETRAITENull()) { ligneMessageAncien.SetI_RETRAITENull(); }
                    if (ligneMessage.IsID_CASE_DEBUTNull()) { ligneMessageAncien.SetID_CASE_DEBUTNull(); }
                    if (ligneMessage.IsID_CASE_FINNull()) { ligneMessageAncien.SetID_CASE_FINNull(); }
                    if (ligneMessage.IsI_NB_PHASES_MARCHE_JOURNull()) { ligneMessageAncien.SetI_NB_PHASES_MARCHE_JOURNull(); }
                    if (ligneMessage.IsI_NB_PHASES_MARCHE_NUITNull()) { ligneMessageAncien.SetI_NB_PHASES_MARCHE_NUITNull(); }
                    if (ligneMessage.IsI_NB_HEURES_COMBATNull()) { ligneMessageAncien.SetI_NB_HEURES_COMBATNull(); }
                    if (ligneMessage.IsI_MATERIELNull()) { ligneMessageAncien.SetI_MATERIELNull(); }
                    if (ligneMessage.IsI_RAVITAILLEMENTNull()) { ligneMessageAncien.SetI_DUREE_HORS_COMBATNull(); }
                    if (ligneMessage.IsI_SOLDATS_RAVITAILLESNull()) { ligneMessageAncien.SetI_SOLDATS_RAVITAILLESNull(); }
                    if (ligneMessage.IsI_NB_HEURES_FORTIFICATIONNull()) { ligneMessageAncien.SetI_NB_HEURES_FORTIFICATIONNull(); }
                    if (ligneMessage.IsI_NIVEAU_FORTIFICATIONNull()) { ligneMessageAncien.SetI_NIVEAU_FORTIFICATIONNull(); }
                    if (ligneMessage.IsI_DUREE_HORS_COMBATNull()) { ligneMessageAncien.SetI_DUREE_HORS_COMBATNull(); }
                    if (ligneMessage.IsI_TOUR_BLESSURENull()) { ligneMessageAncien.SetI_TOUR_BLESSURENull(); }
                    if (ligneMessage.IsC_NIVEAU_DEPOTNull()) { ligneMessageAncien.SetC_NIVEAU_DEPOTNull(); }

                    ligneMessage.Delete();
                }
                else
                {
                    i++;
                }
            }
            */

            #region suppression des anciens ordres, pas facile a suivre ensuite
            /*
            i = 0;
            while (i < Donnees.m_donnees.TAB_ORDRE.Count())
            {
                bool bdetruire = false;
                Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE[i];
                if (!ligneOrdre.IsI_TOUR_FINNull() || ligneOrdre.IsID_PIONNull() || ligneOrdre.ID_PION < 0)
                {
                    bdetruire = true;
                }
                else
                {
                    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneOrdre.ID_PION);
                    if (lignePion.B_DETRUIT)
                    {
                        bdetruire = true;
                    }
                }
                if (bdetruire)
                {
                    Donnees.TAB_ORDRE_ANCIENRow ligneOrdreAncien = Donnees.m_donnees.TAB_ORDRE_ANCIEN.AddTAB_ORDRE_ANCIENRow(
                    ligneOrdre.ID_ORDRE,
                    ligneOrdre.IsID_ORDRE_TRANSMISNull() ? -1 : ligneOrdre.ID_ORDRE_TRANSMIS,
                    ligneOrdre.IsID_ORDRE_SUIVANTNull() ? -1 : ligneOrdre.ID_ORDRE_SUIVANT,
                    ligneOrdre.IsID_ORDRE_WEBNull() ? -1 : ligneOrdre.ID_ORDRE_WEB,
                    ligneOrdre.I_ORDRE_TYPE,
                    ligneOrdre.IsID_PIONNull() ? -1 : ligneOrdre.ID_PION,
                    ligneOrdre.IsID_CASE_DEPARTNull() ? -1 : ligneOrdre.ID_CASE_DEPART,
                    ligneOrdre.IsI_EFFECTIF_DEPARTNull() ? -1 : ligneOrdre.I_EFFECTIF_DEPART,
                    ligneOrdre.IsID_CASE_DESTINATIONNull() ? -1 : ligneOrdre.ID_CASE_DESTINATION,
                    ligneOrdre.IsID_NOM_DESTINATIONNull() ? -1 : ligneOrdre.ID_NOM_DESTINATION,
                    ligneOrdre.IsI_EFFECTIF_DESTINATIONNull() ? -1 : ligneOrdre.I_EFFECTIF_DESTINATION,
                    ligneOrdre.IsI_TOUR_DEBUTNull() ? -1 : ligneOrdre.I_TOUR_DEBUT,
                    ligneOrdre.IsI_PHASE_DEBUTNull() ? -1 : ligneOrdre.I_PHASE_DEBUT,
                    ligneOrdre.IsI_TOUR_FINNull() ? -1 : ligneOrdre.I_TOUR_FIN,
                    ligneOrdre.IsI_PHASE_FINNull() ? -1 : ligneOrdre.I_PHASE_FIN,
                    ligneOrdre.IsID_MESSAGENull() ? -1 : ligneOrdre.ID_MESSAGE,
                    ligneOrdre.IsID_DESTINATAIRENull() ? -1 : ligneOrdre.ID_DESTINATAIRE,
                    ligneOrdre.IsID_CIBLENull() ? -1 : ligneOrdre.ID_CIBLE,
                    ligneOrdre.IsID_DESTINATAIRE_CIBLENull() ? -1 : ligneOrdre.ID_DESTINATAIRE_CIBLE,
                    ligneOrdre.IsID_BATAILLENull() ? -1 : ligneOrdre.ID_BATAILLE,
                    ligneOrdre.IsI_ZONE_BATAILLENull() ? -1 : ligneOrdre.I_ZONE_BATAILLE,
                    ligneOrdre.IsI_HEURE_DEBUTNull() ? -1 : ligneOrdre.I_HEURE_DEBUT,
                    ligneOrdre.IsI_DUREENull() ? -1 : ligneOrdre.I_DUREE,
                    ligneOrdre.IsI_ENGAGEMENTNull() ? -1 : ligneOrdre.I_ENGAGEMENT);

                    if (ligneOrdre.IsID_ORDRE_TRANSMISNull()) { ligneOrdreAncien.SetID_ORDRE_TRANSMISNull(); }
                    if (ligneOrdre.IsID_ORDRE_SUIVANTNull()) { ligneOrdreAncien.SetID_ORDRE_SUIVANTNull(); }
                    if (ligneOrdre.IsID_ORDRE_WEBNull()) { ligneOrdreAncien.SetID_ORDRE_WEBNull(); }
                    if (ligneOrdre.IsID_PIONNull()) { ligneOrdreAncien.SetID_PIONNull(); }
                    if (ligneOrdre.IsID_CASE_DEPARTNull()) { ligneOrdreAncien.SetID_CASE_DEPARTNull(); }
                    if (ligneOrdre.IsI_EFFECTIF_DEPARTNull()) { ligneOrdreAncien.SetI_EFFECTIF_DEPARTNull(); }
                    if (ligneOrdre.IsID_CASE_DESTINATIONNull()) { ligneOrdreAncien.SetID_CASE_DESTINATIONNull(); }
                    if (ligneOrdre.IsID_NOM_DESTINATIONNull()) { ligneOrdreAncien.SetID_NOM_DESTINATIONNull(); }
                    if (ligneOrdre.IsI_EFFECTIF_DESTINATIONNull()) { ligneOrdreAncien.SetI_EFFECTIF_DESTINATIONNull(); }
                    if (ligneOrdre.IsI_TOUR_DEBUTNull()) { ligneOrdreAncien.SetI_TOUR_DEBUTNull(); }
                    if (ligneOrdre.IsI_PHASE_FINNull()) { ligneOrdreAncien.SetI_PHASE_FINNull(); }
                    if (ligneOrdre.IsI_TOUR_FINNull()) { ligneOrdreAncien.SetI_TOUR_FINNull(); }
                    if (ligneOrdre.IsI_PHASE_DEBUTNull()) { ligneOrdreAncien.SetI_PHASE_DEBUTNull(); }
                    if (ligneOrdre.IsID_MESSAGENull()) { ligneOrdreAncien.SetID_MESSAGENull(); }
                    if (ligneOrdre.IsID_DESTINATAIRENull()) { ligneOrdreAncien.SetID_DESTINATAIRENull(); }
                    if (ligneOrdre.IsID_CIBLENull()) { ligneOrdreAncien.SetID_CIBLENull(); }
                    if (ligneOrdre.IsID_DESTINATAIRE_CIBLENull()) { ligneOrdreAncien.SetID_DESTINATAIRE_CIBLENull(); }
                    if (ligneOrdre.IsID_BATAILLENull()) { ligneOrdreAncien.SetID_BATAILLENull(); }
                    if (ligneOrdre.IsI_ZONE_BATAILLENull()) { ligneOrdreAncien.SetI_ZONE_BATAILLENull(); }
                    if (ligneOrdre.IsI_HEURE_DEBUTNull()) { ligneOrdreAncien.SetI_HEURE_DEBUTNull(); }
                    if (ligneOrdre.IsI_DUREENull()) { ligneOrdreAncien.SetI_DUREENull(); }
                    if (ligneOrdre.IsI_ENGAGEMENTNull()) { ligneOrdreAncien.SetI_ENGAGEMENTNull(); }

                    ligneOrdre.Delete();
                }
                else
                {
                    i++;
                }
            }
            */
            #endregion
            /* complexe aussi sur les pions, quand on remonte sur proprietaire dans sauvegardeMesage de ClassVaocWebFichier, il faut des fois être sur un ancien des fois non et pas possible
             *  a priori de faire un cast automatique entre les deix
            i = 0;
            while (i < Donnees.m_donnees.TAB_PION.Count())
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                if (lignePion.B_DETRUIT && lignePion.estMessager)
                {
                    Donnees.TAB_PION_ANCIENRow lignePionAncien = Donnees.m_donnees.TAB_PION_ANCIEN.AddTAB_PION_ANCIENRow(
                        lignePion.ID_PION,
                        lignePion.ID_MODELE_PION,
                        lignePion.ID_PION_PROPRIETAIRE,
                        lignePion.IsID_NOUVEAU_PION_PROPRIETAIRENull() ? -1 : lignePion.ID_NOUVEAU_PION_PROPRIETAIRE,
                        lignePion.IsID_ANCIEN_PION_PROPRIETAIRENull() ? -1 : lignePion.ID_ANCIEN_PION_PROPRIETAIRE,
                        lignePion.S_NOM,
                        lignePion.IsI_INFANTERIENull() ? -1 : lignePion.I_INFANTERIE,
                        lignePion.IsI_INFANTERIE_INITIALENull() ? -1 : lignePion.I_INFANTERIE_INITIALE,
                        lignePion.IsI_CAVALERIENull() ? -1 : lignePion.I_CAVALERIE,
                        lignePion.IsI_CAVALERIE_INITIALENull() ? -1 : lignePion.I_CAVALERIE_INITIALE,
                        lignePion.IsI_ARTILLERIENull() ? -1 : lignePion.I_ARTILLERIE,
                        lignePion.IsI_ARTILLERIE_INITIALENull() ? -1 : lignePion.I_ARTILLERIE_INITIALE,
                        lignePion.IsI_FATIGUENull() ? -1 : lignePion.I_FATIGUE,
                        lignePion.IsI_MORALNull() ? -1 : lignePion.I_MORAL,
                        lignePion.I_MORAL_MAX,
                        lignePion.I_EXPERIENCE,
                        lignePion.I_TACTIQUE,
                        lignePion.I_STRATEGIQUE,
                        lignePion.C_NIVEAU_HIERARCHIQUE,
                        lignePion.I_DISTANCE_A_PARCOURIR,
                        lignePion.IsI_NB_PHASES_MARCHE_JOURNull() ? -1 : lignePion.I_NB_PHASES_MARCHE_JOUR,
                        lignePion.IsI_NB_PHASES_MARCHE_NUITNull() ? -1 : lignePion.I_NB_PHASES_MARCHE_NUIT,
                        lignePion.IsI_NB_HEURES_COMBATNull() ? -1 : lignePion.I_NB_HEURES_COMBAT,
                        lignePion.ID_CASE,
                        lignePion.IsI_TOUR_SANS_RAVITAILLEMENTNull() ? -1 : lignePion.I_TOUR_SANS_RAVITAILLEMENT,
                        lignePion.IsID_BATAILLENull() ? -1 : lignePion.ID_BATAILLE,
                        lignePion.IsI_ZONE_BATAILLENull() ? -1 : lignePion.I_ZONE_BATAILLE,
                        lignePion.IsI_TOUR_RETRAITE_RESTANTNull() ? -1 : lignePion.I_TOUR_RETRAITE_RESTANT,
                        lignePion.IsI_TOUR_FUITE_RESTANTNull() ? -1 : lignePion.I_TOUR_FUITE_RESTANT,
                        lignePion.B_DETRUIT,
                        lignePion.B_FUITE_AU_COMBAT,
                        lignePion.B_INTERCEPTION,
                        lignePion.B_REDITION_RAVITAILLEMENT,
                        lignePion.B_TELEPORTATION,
                        lignePion.B_ENNEMI_OBSERVABLE,
                        lignePion.IsI_MATERIELNull() ? -1 : lignePion.I_MATERIEL,
                        lignePion.IsI_RAVITAILLEMENTNull() ? -1 : lignePion.I_RAVITAILLEMENT,
                        lignePion.B_CAVALERIE_DE_LIGNE,
                        lignePion.B_CAVALERIE_LOURDE,
                        lignePion.B_GARDE,
                        lignePion.B_VIEILLE_GARDE,
                        lignePion.IsI_TOUR_CONVOI_CREENull() ? -1 : lignePion.I_TOUR_CONVOI_CREE,
                        lignePion.IsID_DEPOT_SOURCENull() ? -1 : lignePion.ID_DEPOT_SOURCE,
                        lignePion.IsI_SOLDATS_RAVITAILLESNull() ? -1 : lignePion.I_SOLDATS_RAVITAILLES,
                        lignePion.IsI_NB_HEURES_FORTIFICATIONNull() ? -1 : lignePion.I_NB_HEURES_FORTIFICATION,
                        lignePion.IsI_NIVEAU_FORTIFICATIONNull() ? -1 : lignePion.I_NIVEAU_FORTIFICATION,
                        lignePion.IsID_PION_REMPLACENull() ? -1 : lignePion.ID_PION_REMPLACE,
                        lignePion.IsI_DUREE_HORS_COMBATNull() ? -1 : lignePion.I_DUREE_HORS_COMBAT,
                        lignePion.IsI_TOUR_BLESSURENull() ? -1 : lignePion.I_TOUR_BLESSURE,
                        lignePion.B_BLESSES,
                        lignePion.B_PRISONNIERS,
                        lignePion.B_RENFORT,
                        lignePion.IsID_LIEU_RATTACHEMENTNull() ? -1 : lignePion.ID_LIEU_RATTACHEMENT,
                        lignePion.IsC_NIVEAU_DEPOTNull() ? 'X' : lignePion.C_NIVEAU_DEPOT,
                        lignePion.IsID_PION_ESCORTENull() ? -1 : lignePion.ID_PION_ESCORTE,
                        lignePion.IsI_INFANTERIE_ESCORTENull() ? -1 : lignePion.I_INFANTERIE_ESCORTE,
                        lignePion.IsI_CAVALERIE_ESCORTENull() ? -1 : lignePion.I_CAVALERIE_ESCORTE,
                        lignePion.IsI_MATERIEL_ESCORTENull() ? -1 : lignePion.I_MATERIEL_ESCORTE);

                    if (lignePion.IsID_NOUVEAU_PION_PROPRIETAIRENull()) { lignePionAncien.SetID_NOUVEAU_PION_PROPRIETAIRENull(); }
                    if (lignePion.IsID_ANCIEN_PION_PROPRIETAIRENull()) { lignePionAncien.SetID_ANCIEN_PION_PROPRIETAIRENull(); }
                    if (lignePion.IsI_INFANTERIENull()) { lignePionAncien.SetI_INFANTERIENull(); }
                    if (lignePion.IsI_CAVALERIENull()) { lignePionAncien.SetI_CAVALERIENull(); }
                    if (lignePion.IsI_ARTILLERIENull()) { lignePionAncien.SetI_ARTILLERIENull(); }
                    if (lignePion.IsI_INFANTERIE_INITIALENull()) { lignePionAncien.SetI_INFANTERIE_INITIALENull(); }
                    if (lignePion.IsI_CAVALERIE_INITIALENull()) { lignePionAncien.SetI_CAVALERIE_INITIALENull(); }
                    if (lignePion.IsI_ARTILLERIE_INITIALENull()) { lignePionAncien.SetI_ARTILLERIE_INITIALENull(); }
                    if (lignePion.IsI_FATIGUENull()) { lignePionAncien.SetI_FATIGUENull(); }
                    if (lignePion.IsI_MORALNull()) { lignePionAncien.SetI_MORALNull(); }
                    if (lignePion.IsI_TOUR_SANS_RAVITAILLEMENTNull()) { lignePionAncien.SetI_TOUR_SANS_RAVITAILLEMENTNull(); }
                    if (lignePion.IsID_BATAILLENull()) { lignePionAncien.SetID_BATAILLENull(); }
                    if (lignePion.IsI_ZONE_BATAILLENull()) { lignePionAncien.SetI_ZONE_BATAILLENull(); }
                    if (lignePion.IsI_TOUR_RETRAITE_RESTANTNull()) { lignePionAncien.SetI_TOUR_RETRAITE_RESTANTNull(); }
                    if (lignePion.IsI_TOUR_FUITE_RESTANTNull()) { lignePionAncien.SetI_TOUR_FUITE_RESTANTNull(); }
                    if (lignePion.IsI_NB_PHASES_MARCHE_JOURNull()) { lignePionAncien.SetI_NB_PHASES_MARCHE_JOURNull(); }
                    if (lignePion.IsI_NB_PHASES_MARCHE_NUITNull()) { lignePionAncien.SetI_NB_PHASES_MARCHE_NUITNull(); }
                    if (lignePion.IsI_NB_HEURES_COMBATNull()) { lignePionAncien.SetI_NB_HEURES_COMBATNull(); }
                    if (lignePion.IsI_MATERIELNull()) { lignePionAncien.SetI_MATERIELNull(); }
                    if (lignePion.IsI_RAVITAILLEMENTNull()) { lignePionAncien.SetI_DUREE_HORS_COMBATNull(); }
                    if (lignePion.IsI_SOLDATS_RAVITAILLESNull()) { lignePionAncien.SetI_SOLDATS_RAVITAILLESNull(); }
                    if (lignePion.IsI_TOUR_CONVOI_CREENull()) { lignePionAncien.SetI_TOUR_CONVOI_CREENull(); }
                    if (lignePion.IsID_DEPOT_SOURCENull()) { lignePionAncien.SetID_DEPOT_SOURCENull(); }
                    if (lignePion.IsI_NB_HEURES_FORTIFICATIONNull()) { lignePionAncien.SetI_NB_HEURES_FORTIFICATIONNull(); }
                    if (lignePion.IsI_NIVEAU_FORTIFICATIONNull()) { lignePionAncien.SetI_NIVEAU_FORTIFICATIONNull(); }
                    if (lignePion.IsID_PION_REMPLACENull()) { lignePionAncien.SetID_PION_REMPLACENull(); }
                    if (lignePion.IsI_DUREE_HORS_COMBATNull()) { lignePionAncien.SetI_DUREE_HORS_COMBATNull(); }
                    if (lignePion.IsI_TOUR_BLESSURENull()) { lignePionAncien.SetI_TOUR_BLESSURENull(); }
                    if (lignePion.IsID_LIEU_RATTACHEMENTNull()) { lignePionAncien.SetID_LIEU_RATTACHEMENTNull(); }
                    if (lignePion.IsC_NIVEAU_DEPOTNull()) { lignePionAncien.SetC_NIVEAU_DEPOTNull(); }
                    if (lignePion.IsID_PION_ESCORTENull()) { lignePionAncien.SetID_PION_ESCORTENull(); }
                    if (lignePion.IsI_INFANTERIE_ESCORTENull()) { lignePionAncien.SetI_INFANTERIE_ESCORTENull(); }
                    if (lignePion.IsI_CAVALERIE_ESCORTENull()) { lignePionAncien.SetI_CAVALERIE_ESCORTENull(); }
                    if (lignePion.IsI_MATERIEL_ESCORTENull()) { lignePionAncien.SetI_MATERIEL_ESCORTENull(); }
                    lignePion.Delete();
                }
                else
                {
                    i++;
                }
            }
            */
            Debug.WriteLine(string.Format("Après AmeliorationsPerformances #pions={0} #messages={1} #ordres={2}",
                Donnees.m_donnees.TAB_PION.Count(),
                Donnees.m_donnees.TAB_MESSAGE.Count(),
                Donnees.m_donnees.TAB_ORDRE.Count()));
            #endregion
        }

        /// <summary>
        /// Calcul du nombre total de pts de victoire possibles sur la partie avec les unités et les noms de lieux
        /// </summary>
        public void CalculNombreTotalPointsDeVictoire()
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

        private bool ExecuterActionHorsMouvementEnParallele()
        {
            /*i = 0;
            while (i < Donnees.m_donnees.TAB_PION.Count())
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                if (lignePion.B_DETRUIT) { i++; continue; }
                if (!ExecuterOrdreHorsMouvement(lignePion, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE))
                {
                    messageErreur = "Erreur durant le traitement ExecuterMouvement";
                    return false;
                }
                i++;
            }*/

            List<Donnees.TAB_PIONRow> liste = new List<Donnees.TAB_PIONRow>();
            for (int l = 0; l < Donnees.m_donnees.TAB_PION.Count; l++)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[l];
                if (!lignePion.B_DETRUIT)
                {
                    liste.Add(lignePion);
                }
            }
            //Parallel.ForEach(liste, item => ExecuterOrdreHorsMouvement(item, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE));             
            try
            {
                ParallelLoopResult result = Parallel.ForEach(liste, (item, loopstate) =>
                {
                    if (!ExecuterOrdreHorsMouvement(item, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE)) loopstate.Break();
                });
                return result.IsCompleted;
            }
            catch(AggregateException ex)
            {
                LogFile.Notifier("ExecuterActionHorsMouvementEnParallele Exception" + ex.Message + " trace:" + ex.StackTrace);
            }
            return true;
        }

        private bool ExecuterMouvementAvecEffectifsEnParallele()
        {
            List<Donnees.TAB_PIONRow> liste = new List<Donnees.TAB_PIONRow>();
            for (int l = 0; l < Donnees.m_donnees.TAB_PION.Count; l++)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[l];
                if (!lignePion.B_DETRUIT && lignePion.effectifTotal > 0)
                {
                    liste.Add(lignePion);
                }
            }
            try
            {
                Parallel.ForEach(liste, item => ExecuterMouvement(item, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE));
            }
            catch (AggregateException ex)
            {
                LogFile.Notifier("ExecuterMouvementAvecEffectifsEnParallele Exception" + ex.Message + " trace:" + ex.StackTrace);
            }
            return true;
        }

        private bool ExecuterMouvementSansEffectifsEnParallele()
        {
            List<Donnees.TAB_PIONRow> liste = new List<Donnees.TAB_PIONRow>();
            for (int l = 0; l < Donnees.m_donnees.TAB_PION.Count; l++)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[l];
                if (!lignePion.B_DETRUIT && 0 == lignePion.effectifTotal)
                {
                    liste.Add(lignePion);
                }
            }
            try
            {
                Parallel.ForEach(liste, item => ExecuterMouvement(item, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE));
            }
            catch (AggregateException ex)
            {
                LogFile.Notifier("ExecuterMouvementAvecEffectifsEnParallele Exception" + ex.Message + " trace:" + ex.StackTrace);
            }
            return true;
        }

        private void DistanceMinimaleEntreEnnemis(out double distanceMin, out double distanceMinEnMouvement, 
            out Donnees.TAB_PIONRow lignePionMin1, out Donnees.TAB_PIONRow lignePionMin2, 
            out Donnees.TAB_PIONRow lignePionMax1, out Donnees.TAB_PIONRow lignePionMax2)
        {
            lignePionMin1 = lignePionMin2 = lignePionMax1 = lignePionMax2 = null;
            //on recherche la distance minimale, à vol d'oiseau, entre des unités ennemis (autres que les patrouilles et messagers)
            distanceMin = double.MaxValue;
            distanceMinEnMouvement = double.MaxValue;

            //on constitue la liste des cases de chaque nation
            Dictionary<int, List<Donnees.TAB_CASERow>> lignesCasesProprietaires = new Dictionary<int, List<Donnees.TAB_CASERow>>();
            //List<Donnees.TAB_CASERow>[] lignesCasesProprietaires = new List<Donnees.TAB_CASERow>[Donnees.m_donnees.TAB_NATION.Count];
            for (int l=0; l< Donnees.m_donnees.TAB_NATION.Count; l++)
            {
                Donnees.TAB_NATIONRow ligneNation = Donnees.m_donnees.TAB_NATION[l];
                lignesCasesProprietaires.Add(ligneNation.ID_NATION, new List<Donnees.TAB_CASERow>());
            }

            //Donnees.TAB_CASERow ligneCaseD = Donnees.m_donnees.TAB_CASE.FindByXY(174, 482);
            for (int l=0; l< Donnees.m_donnees.TAB_CASE.Count; l++)
            {
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE[l];
                if (!ligneCase.IsID_PROPRIETAIRENull())
                {
                    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneCase.ID_PROPRIETAIRE);
                    if (lignePion.effectifTotal > 0)
                    {
                        lignesCasesProprietaires[lignePion.nation.ID_NATION].Add(ligneCase);
                    }
                }
            }

            int i=0;
            while (i<lignesCasesProprietaires.Count)
            {
                List<Donnees.TAB_CASERow> lignesCasesProprietairesBase = lignesCasesProprietaires.ElementAt(i).Value;
                int j=i+1;
                while (j<lignesCasesProprietaires.Count)
                {
                    List<Donnees.TAB_CASERow> lignesCasesProprietairesTest = lignesCasesProprietaires.ElementAt(j).Value;

                    for (int l=0; l< lignesCasesProprietairesBase.Count;l++)
                    {
                        Donnees.TAB_CASERow ligneCaseBase = lignesCasesProprietairesBase[l];
                        Donnees.TAB_PIONRow lignePionBase = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneCaseBase.ID_PROPRIETAIRE);
                        bool enMouvementBase= lignePionBase.enMouvement;
                        for (int ll= 0; ll < lignesCasesProprietairesTest.Count; ll++)
                        {
                            Donnees.TAB_CASERow ligneCaseTest = lignesCasesProprietairesTest[ll];
                            Donnees.TAB_PIONRow lignePionTest = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneCaseTest.ID_PROPRIETAIRE);
                            double distance = Constantes.Distance(ligneCaseBase.I_X, ligneCaseBase.I_Y, ligneCaseTest.I_X, ligneCaseTest.I_Y);
                            if (distance < distanceMin)
                            {
                                distanceMin = distance;
                                lignePionMin1 = lignePionBase;
                                lignePionMin2 = lignePionTest;
                            }
                            if (distance < distanceMinEnMouvement && enMouvementBase && lignePionTest.enMouvement)
                            {
                                distanceMinEnMouvement = distance;
                                lignePionMax1 = lignePionBase;
                                lignePionMax2 = lignePionTest;
                            }
                        }
                    }
                    j++;
                }
                i++;
            }


            //foreach (Donnees.TAB_PIONRow lignePion1 in Donnees.m_donnees.TAB_PION)
            //{
            //    if (lignePion1.B_DETRUIT || lignePion1.estMessager || lignePion1.estPatrouille) continue;
            //    foreach (Donnees.TAB_PIONRow lignePion2 in Donnees.m_donnees.TAB_PION)
            //    {
            //        if (lignePion2.B_DETRUIT || lignePion2.estMessager || lignePion2.estPatrouille) continue;
            //        if (lignePion1.estEnnemi(lignePion2))
            //        {
            //            Donnees.TAB_CASERow ligneCase1 = lignePion1.CaseCourante();
            //            Donnees.TAB_CASERow ligneCase2 = lignePion2.CaseCourante();
            //            double distance = Constantes.Distance(ligneCase1.I_X, ligneCase1.I_Y, ligneCase2.I_X, ligneCase2.I_Y);
            //            if (distance < distanceMax)
            //            {
            //                Debug.WriteLine(string.Format("DistanceMax = {0} entre {1}:{2} et {3}:{4}", distanceMax, lignePion1.ID_PION, lignePion1.S_NOM, lignePion2.ID_PION, lignePion2.S_NOM));
            //                distanceMax = distance;
            //            }
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Traitement des ordres qui ne sont pas des ordres mouvement
        /// c'est à dire les ordres de bataille et les ordres de construction
        /// </summary>
        /// <param name="lignePion">pion executant l'ordre</param>
        /// <param name="tour">tour en cours</param>
        /// <param name="phase">phase en cours</param>
        /// <returns>true si ok, false si ko</returns>
        private bool ExecuterOrdreHorsMouvement(Donnees.TAB_PIONRow lignePion, int tour, int phase)
        {
            string message;
            Donnees.TAB_BATAILLE_PIONSRow lignePionBataille;

            Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Courant(lignePion.ID_PION);
            if (null == ligneOrdre) { return true; }

            /* Maintenant, ça peut arriver puisque les ordres de combat passent par là !
            if (lignePion.estAuCombat)
            {
                //normalement ne doit pas arriver
                message = string.Format("{0}(ID={1}, en bataille:{2}, pas d'ordre autorisé)", lignePion.S_NOM, lignePion.ID_PION, lignePion.ID_BATAILLE);
                return LogFile.Notifier(message, out messageErreur);
            }
            */

            switch (ligneOrdre.I_ORDRE_TYPE)
            {
                case Constantes.ORDRES.ENDOMMAGER_PONT:
                    //cet ordre ne peut s'executer que de jour, si, si la nuit aussi c'est juste deux fois plus long
                    //if (Donnees.m_donnees.TAB_PARTIE.Nocturne()) { return true; }
                    return lignePion.ExecuterEndommagerPont(ligneOrdre, tour, phase);
                case Constantes.ORDRES.REPARER_PONT:
                    //cet ordre ne peut s'executer que de jour, si, si la nuit aussi c'est juste deux fois plus long
                    //if (Donnees.m_donnees.TAB_PARTIE.Nocturne()) { return true; }
                    return lignePion.ExecuterReparerPont(ligneOrdre, tour, phase);
                case Constantes.ORDRES.CONSTRUIRE_PONTON:
                    return lignePion.ExecuterConstruirePonton(ligneOrdre, tour, phase);
                case Constantes.ORDRES.COMBAT:
                    //s'il s'agit d'un ordre de combat
                    if (ligneOrdre.I_PHASE_DEBUT == phase)
                    {
                        //il faut supprimer tous les ordres actuellement en cours
                        lignePion.SupprimerTousLesOrdres();//normallement déjà fait lors de l'engagement en bataille, mais bon...

                        //Mise à jour l'engagement et la zone d'engagement
                        Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        lignePion.I_ZONE_BATAILLE = ligneOrdre.I_ZONE_BATAILLE;
                        lignePionBataille = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePion.ID_PION, ligneOrdre.ID_BATAILLE);
                        lignePionBataille.B_ENGAGEE = true;
                        //pour les presentations de fin de partie
                        lignePionBataille.B_ENGAGEMENT = true;
                        lignePionBataille.I_ZONE_BATAILLE_ENGAGEMENT = lignePion.I_ZONE_BATAILLE;
                        Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        //l'ordre est terminé
                        lignePion.TerminerOrdre(ligneOrdre, false, true);
                    }
                    break;
                case Constantes.ORDRES.RETRAIT:
                    //s'il s'agit d'un ordre de combat, l'unité se retire et revient en réserve
                    if (ligneOrdre.I_PHASE_DEBUT == phase)
                    {
                        //Mise à jour l'engagement et la zone d'engagement
                        Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        lignePion.SetI_ZONE_BATAILLENull();
                        Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        //il ne faut pas remettre I_ZONE_BATAILLE_ENGAGEMENT à blanc car c'est pour l'historique
                        //lignePionBataille = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePion.ID_PION, ligneOrdre.ID_BATAILLE);
                        //lignePionBataille.SetI_ZONE_BATAILLE_ENGAGEMENTNull();
                        //l'ordre est terminé
                        lignePion.TerminerOrdre(ligneOrdre, false, true);
                    }
                    break;
                case Constantes.ORDRES.RETRAITE:
                    //s'il s'agit d'un ordre de retraite, l'unité n'est plus engagée dans la bataille mais contrairement à une unité en déroute, elle n'a pas deux tours de fuite imposés
                    if (ligneOrdre.I_PHASE_DEBUT == phase)
                    {
                        //lignePion.I_TOUR_FUITE_RESTANT = 2; -> seulement pour les unités en déroute
                        Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        lignePion.I_TOUR_RETRAITE_RESTANT = 2 +1; // même chose que I_TOUR_FUITE_RESTANT mais tu peux donner des ordres, //+1 car on fait une fin de combat au tour T,phase 100, si le combat reprend à T+2,phase 0, cela ne laisse en fait qu'une heure de retraite
                        lignePionBataille = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePion.ID_PION, ligneOrdre.ID_BATAILLE);
                        lignePionBataille.B_ENGAGEE = false;
                        lignePion.SetI_ZONE_BATAILLENull();
                        Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        //l'ordre est terminé
                        lignePion.TerminerOrdre(ligneOrdre, false, true);
                    }
                    break;
                case Constantes.ORDRES.ARRET:
                    lignePion.Arret(ligneOrdre);
                    break;
                //case Constantes.ORDRES.TRANSFERER: -> Cas particulier, géré dans ExecuterMouvementSansEffectif car cet ordre n'est pas à l'initiative du joueur, on ne doit donc pas changer son ordre courant
                //    Donnees.TAB_PIONRow lignePionTransfert = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneOrdre.ID_CIBLE);
                //    Cartographie.TransfertPion(lignePionTransfert, ligneOrdre.ID_DESTINATAIRE_CIBLE);
                //    lignePion.TerminerOrdre(ligneOrdre, true, false);
                //    break;
                case Constantes.ORDRES.GENERERCONVOI:
                    //s'il s'agit d'un dépôt de niveau 'D', il se transforme en convoi
                    if ('D' == lignePion.C_NIVEAU_DEPOT)
                    {
                        Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        lignePion.S_NOM = "Convoi de ravitaillement";
                        int idModeleCONVOI = Donnees.m_donnees.TAB_MODELE_PION.RechercherModele(lignePion.modelePion.ID_NATION, "CONVOI");

                        if (idModeleCONVOI >= 0)
                        {
                            lignePion.ID_MODELE_PION = idModeleCONVOI;
                            Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        }
                        else
                        {
                            message = string.Format("ExecuterOrdreHorsMouvement : {0},ID={1}, impossible de trouver le modèle de convoi de la nation", lignePion.S_NOM, lignePion.ID_PION);
                            LogFile.Notifier(message);
                            Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                            return false;
                        }
                    }
                    else
                    { 
                        //on créer un nouveau convoi de type ravitaillement
                        Donnees.TAB_PIONRow lignePionConvoi = lignePion.CreerConvoi(lignePion.proprietaire, false /*bBlesses*/, false /*bPrisonniers*/, false /*bRenfort*/);
                        if (null == lignePionConvoi)
                        {
                            message = string.Format("ExecuterOrdreHorsMouvement : {0},ID={1}, impossible de générer un convoi", lignePion.S_NOM, lignePion.ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }
                        Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        lignePionConvoi.C_NIVEAU_DEPOT = 'D';
                        lignePionConvoi.ID_DEPOT_SOURCE = lignePion.ID_PION;
                        Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);

                        //on indique au joueur que le nouveau convoi est disponible
                        if (!ClassMessager.EnvoyerMessage(lignePionConvoi, ClassMessager.MESSAGES.MESSAGE_GENERATION_CONVOI))
                        {
                            message = string.Format("{0},ID={1}, erreur sur EnvoyerMessage avec MESSAGE_GENERATION_CONVOI dans ExecuterOrdreHorsMouvement", lignePionConvoi.S_NOM, lignePionConvoi.ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }

                        //on réduit le niveau du dépôt actuel et on note la date de création du dépot
                        Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        if (lignePion.C_NIVEAU_DEPOT != 'A')
                        {
                            lignePion.C_NIVEAU_DEPOT++;// 'A' c'est le meilleur, 'D' le pire
                        }
                        lignePion.I_TOUR_CONVOI_CREE = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                        Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    }

                    //l'ordre est terminé
                    lignePion.TerminerOrdre(ligneOrdre, false, true);
                    break;
                case Constantes.ORDRES.RENFORCER:
                    //L'unité se fusionne avec une autre pour la renforcer, il s'agit soit de renforts soit d'un ravitaillement de dépôt
                    Donnees.TAB_PIONRow lignePionARenforcer = ligneOrdre.cible;
                    if (null == lignePionARenforcer)
                    {
                        message = string.Format("{0},ID={1}, erreur sur Constantes.ORDRES.RENFORCER dans ExecuterOrdreHorsMouvement, le pion à renforcer n'est pas indiqué dans la cible", lignePion.S_NOM, lignePion.ID_PION);
                        LogFile.Notifier(message);
                        return false;
                    }
                    //dans le cas d'un convoi de ravitaillement sur un dépôt de type 'B' il faut s'assurer qu'il est sur une ville où les dépôts de type 'A' sont autorisés avant de faire l'ordre
                    if (lignePion.estConvoiDeRavitaillement && lignePionARenforcer.C_NIVEAU_DEPOT == 'B')
                    {
                        Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePion.ID_CASE);
                        double distanceCreationDepot = double.MaxValue;
                        foreach (Donnees.TAB_NOMS_CARTERow ligneNomCarte in Donnees.m_donnees.TAB_NOMS_CARTE)
                        {
                            if (ligneNomCarte.B_CREATION_DEPOT)
                            {
                                double distance = Constantes.Distance(ligneCase.I_X, ligneCase.I_Y, ligneNomCarte.I_X, ligneNomCarte.I_Y);
                                if (distance < distanceCreationDepot) distanceCreationDepot = distance;
                            }
                        }
                        if (distanceCreationDepot <= Constantes.CST_DISTANCE_CREATION_DEPOT_A * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE)
                        {
                            //tout va bien, on continue
                        }
                        else
                        {
                            //on prévient le joueur qu'il ne peut pas créer de dépôt A à cet emplacement et on ne fait rien d'autre
                            if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_CREATION_DEPOT_IMPOSSIBLE))
                            {
                                message = string.Format("{0},ID={1}, erreur sur EnvoyerMessage avec MESSAGE_CREATION_DEPOT_IMPOSSIBLE dans ExecuterOrdreHorsMouvement", lignePion.S_NOM, lignePion.ID_PION);
                                LogFile.Notifier(message);
                                return false;
                            }
                            break;//on ne fait pas la suite
                        }
                    }

                    //on indique au joueur que le renfort est arrivé (et, comme ça, il sait aussi que l'unité est détruite !)
                    lignePion.DetruirePion();
                    if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_ARRIVE_A_DESTINATION))
                    {
                        message = string.Format("{0},ID={1}, erreur sur EnvoyerMessage avec MESSAGE_ARRIVE_A_DESTINATION dans ExecuterOrdreHorsMouvement", lignePion.S_NOM, lignePion.ID_PION);
                        LogFile.Notifier(message);
                        return false;
                    }

                    if (lignePion.estConvoiDeRavitaillement)
                    {
                        //seul un convoi de niveau 'D' peut fusionner avec un dépôt
                        Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        if (lignePionARenforcer.C_NIVEAU_DEPOT != 'A')
                        {
                            lignePionARenforcer.C_NIVEAU_DEPOT--;// 'A' c'est le meilleur, 'D' le pire
                        }
                        lignePionARenforcer.I_SOLDATS_RAVITAILLES = 0;//pas bien clair dans les règles mais cela me semble logique
                        Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);

                        //on indique au joueur que le renfort a été fait
                        if (!ClassMessager.EnvoyerMessage(lignePionARenforcer, ClassMessager.MESSAGES.MESSAGE_RENFORT_DEPOT))
                        {
                            message = string.Format("{0},ID={1}, erreur sur EnvoyerMessage avec MESSAGE_RENFORT_CONVOI dans ExecuterOrdreHorsMouvement", lignePionARenforcer.S_NOM, lignePionARenforcer.ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }
                    }
                    else
                    {
                        //la fatigue, le ravitaillement, l'expérience et le matériel sont mis à jour au prorata des deux unités.
                        int effectifsARenforcer = lignePionARenforcer.effectifTotal;
                        int effectifsRenforts = lignePion.effectifTotal;
                        int effectifsFinaux = effectifsARenforcer+effectifsRenforts;
                        Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        lignePionARenforcer.I_EXPERIENCE = (lignePionARenforcer.I_EXPERIENCE * effectifsARenforcer + lignePion.I_EXPERIENCE * effectifsRenforts) / effectifsFinaux;
                        
                        int variationFatigue = lignePionARenforcer.I_FATIGUE;
                        lignePionARenforcer.I_FATIGUE = (lignePionARenforcer.I_FATIGUE * effectifsARenforcer + lignePion.I_FATIGUE * effectifsRenforts) / effectifsFinaux;
                        variationFatigue = lignePionARenforcer.I_FATIGUE - variationFatigue;

                        lignePionARenforcer.I_MATERIEL = (lignePionARenforcer.I_MATERIEL * effectifsARenforcer + lignePion.I_MATERIEL * effectifsRenforts) / effectifsFinaux;
                        lignePionARenforcer.I_RAVITAILLEMENT = (lignePionARenforcer.I_RAVITAILLEMENT * effectifsARenforcer + lignePion.I_RAVITAILLEMENT * effectifsRenforts) / effectifsFinaux;
                        //lignePionARenforcer.I_MORAL = (lignePionARenforcer.I_MORAL * effectifsARenforcer + lignePion.I_MORAL * effectifsRenforts) / effectifsFinaux; //Le moral doit être celui de la source je pense

                        //on ajoute les nouveaux effectifs
                        lignePionARenforcer.I_INFANTERIE += lignePion.I_INFANTERIE;
                        lignePionARenforcer.I_CAVALERIE += lignePion.I_CAVALERIE;
                        lignePionARenforcer.I_ARTILLERIE += lignePion.I_ARTILLERIE;
                        Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);

                        //on indique au joueur que le renfort a été fait
                        if (!ClassMessager.EnvoyerMessage(lignePionARenforcer, ClassMessager.MESSAGES.MESSAGE_RENFORT_UNITE, 0, variationFatigue))
                        {
                            message = string.Format("{0},ID={1}, erreur sur EnvoyerMessage avec MESSAGE_RENFORT_UNITE dans ExecuterOrdreHorsMouvement", lignePionARenforcer.S_NOM, lignePionARenforcer.ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }
                    }

                    break;
                case Constantes.ORDRES.SEFORTIFIER:
                    return lignePion.ExecuterSeFortifier(ligneOrdre, tour, phase);
                case Constantes.ORDRES.ETABLIRDEPOT:
                    int idModeleDEPOT = Donnees.m_donnees.TAB_MODELE_PION.RechercherModele(lignePion.modelePion.ID_NATION, "DEPOT");

                    if (idModeleDEPOT < 0)
                    {
                        message = string.Format("{0},ID={1}, erreur dans ExecuterOrdreHorsMouvement sur RechercherModele, par de modèle de DEPOT pour la nation id={2}", lignePion.S_NOM, lignePion.ID_PION, lignePion.modelePion.ID_NATION);
                        LogFile.Notifier(message);
                        return false;
                    }
                    Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    lignePion.ID_MODELE_PION = idModeleDEPOT;
                    Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);

                    //changer le nom du pion
                    string nomDepot;
                    if (!ClassMessager.NomDeDepot(lignePion.CaseCourante(), out nomDepot))
                    {
                        message = string.Format("{0},ID={1}, erreur dans ExecuterOrdreHorsMouvement sur NomDeDepot", lignePion.S_NOM, lignePion.ID_PION);
                        LogFile.Notifier(message);
                        return false;
                    }
                    Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    lignePion.S_NOM = nomDepot;

                    //supprimer tous les ordres suivants (un dépôt ne peut pas recevoir d'ordre à part création de convoi, ce qui n'aurait pas de sens ici).
                    lignePion.SupprimerTousLesOrdres();
                    lignePion.I_SOLDATS_RAVITAILLES = 0;
                    lignePion.C_NIVEAU_DEPOT = 'D';
                    Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);

                    //envoyer un message annonçant la création du dépôt
                    if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_ETABLIRDEPOT))
                    {
                        message = string.Format("{0},ID={1}, erreur sur EnvoyerMessage avec MESSAGE_ETABLIRDEPOT dans ExecuterOrdreHorsMouvement", lignePion.S_NOM, lignePion.ID_PION);
                        LogFile.Notifier(message);
                        return false;
                    }
                    break;
                case Constantes.ORDRES.LIGNE_RAVITAILLEMENT:
                    if (ligneOrdre.I_PHASE_DEBUT == phase && 
                        ((lignePion.I_TOUR_CONVOI_CREE<=0) || (Donnees.m_donnees.TAB_PARTIE[0].I_TOUR - lignePion.I_TOUR_CONVOI_CREE) >=24))
                    {
                        //on créer un nouveau convoi de type ravitaillement
                        Donnees.TAB_PIONRow lignePionConvoi = lignePion.CreerConvoi(lignePion.proprietaire, false /*bBlesses*/, false /*bPrisonniers*/, false /*bRenfort*/);
                        if (null == lignePionConvoi)
                        {
                            message = string.Format("ExecuterOrdreHorsMouvement : {0},ID={1}, impossible de générer un convoi", lignePion.S_NOM, lignePion.ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }
                        Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        lignePionConvoi.C_NIVEAU_DEPOT = 'D';
                        lignePionConvoi.ID_DEPOT_SOURCE = lignePion.ID_PION;
                        Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);

                        //on indique au joueur que le nouveau convoi est disponible
                        if (!ClassMessager.EnvoyerMessage(lignePionConvoi, ClassMessager.MESSAGES.MESSAGE_GENERATION_CONVOI))
                        {
                            message = string.Format("{0},ID={1}, erreur sur EnvoyerMessage avec MESSAGE_GENERATION_CONVOI dans ExecuterOrdreHorsMouvement", lignePionConvoi.S_NOM, lignePionConvoi.ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }

                        //on donne un ordre de mouvement au convoi
                        Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                        Donnees.TAB_ORDRERow ligneOrdreNouveau = Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                            Constantes.NULLENTIER,//ID_ORDRE_TRANSMIS
                            Constantes.NULLENTIER,//ID_ORDRE_SUIVANT global::System.Convert.DBNull,
                            Constantes.NULLENTIER,///ID_ORDRE_WEB
                            Constantes.ORDRES.MOUVEMENT,
                            lignePionConvoi.ID_PION,
                            lignePionConvoi.ID_CASE,
                            0,//effectifs depart
                            ligneOrdre.ID_CASE_DESTINATION,
                            Constantes.NULLENTIER,//id ville de destination
                            0,//I_EFFECTIF_DESTINATION
                            Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,//I_TOUR_DEBUT
                            Donnees.m_donnees.TAB_PARTIE[0].I_PHASE,//I_PHASE_DEBUT
                            Constantes.NULLENTIER,//I_TOUR_FIN
                            Constantes.NULLENTIER,//I_PHASE_FIN
                            Constantes.NULLENTIER,//ID_MESSAGE
                            Constantes.NULLENTIER,//ID_DESTINATAIRE
                            Constantes.NULLENTIER,//ID_CIBLE
                            Constantes.NULLENTIER,//ID_DESTINATAIRE_CIBLE
                            Constantes.NULLENTIER,//ID_BATAILLE
                            Constantes.NULLENTIER,//I_ZONE_BATAILLE
                            0,//I_HEURE_DEBUT
                            24,//I_DUREE
                            Constantes.NULLENTIER//I_ENGAGEMENT
                            );//ID_BATAILLE
                        Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);

                        lignePion.I_TOUR_CONVOI_CREE = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                    }
                    break;
                default:
                     //autre ordre mouvement, etc...
                    break;
            }
            return true;
        }

        /// <summary>
        /// Effectue le ravitaillement, de toutes les unités
        /// Relire, la règle
        /// </summary>
        /// <returns></returns>
        private bool Ravitaillement()
        {
            string message;
            decimal distanceRavitaillement;
            int i;
            string depotRavitaillement;
            bool bUniteRavitaillee;
            int ravitaillementInitial;
            int materielInitial;

            LogFile.Notifier("Début Ravitaillement : Cartographie.InitialiserProprietairesTrajets");
            //Cartographie.InitialiserProprietairesTrajets(); //BEA : attention, il faut le remettre dans une vrai partie même si ça prend une heure de traitement !
            LogFile.Notifier("Fin Cartographie.InitialiserProprietairesTrajets");
            i = 0;
            while (i<Donnees.m_donnees.TAB_PION.Count)//on ne peut pas faire de foreach car l'envoi de messagers change la table
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                if (!lignePion.estRavitaillable) { i++;  continue; }
                ravitaillementInitial = lignePion.I_RAVITAILLEMENT;
                materielInitial = lignePion.I_MATERIEL;
                if (!lignePion.RavitaillementUnite(out bUniteRavitaillee, out distanceRavitaillement, out depotRavitaillement)) { return false; }

                if (!bUniteRavitaillee)
                {
                    //on envoit un message si c'est la première fois que l'unité est sans ravitaillement alors qu'elle n'a pas bougé (et donc qu'elle aurait du en recevoir
                    if (lignePion.reposComplet)
                    {
                        if (0 == lignePion.I_TOUR_SANS_RAVITAILLEMENT)
                        {
                            lignePion.I_TOUR_SANS_RAVITAILLEMENT = 0;//au cas où cela aurait value null auparavant
                            //if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_SANS_RAVITAILLEMENT))
                            if (!ClassMessager.EnvoyerMessage(lignePion, distanceRavitaillement, lignePion.I_RAVITAILLEMENT - ravitaillementInitial,
                                    lignePion.I_MATERIEL - materielInitial, string.Empty, ClassMessager.MESSAGES.MESSAGE_SANS_RAVITAILLEMENT))
                            {
                                message = string.Format("{0}(ID={1}, erreur sur EnvoyerMessage avec MESSAGE_SANS_RAVITAILLEMENT dans Ravitaillement", lignePion.S_NOM, lignePion.ID_PION);
                                LogFile.Notifier(message);
                                return false;
                            }
                        }

                        lignePion.I_TOUR_SANS_RAVITAILLEMENT++;

                        if (lignePion.I_TOUR_SANS_RAVITAILLEMENT <= 0)
                        {
                            lignePion.I_TOUR_SANS_RAVITAILLEMENT = 1;//au moins un pour que l'on sache qu'il y a déjà un tour sans ravitaillement
                        }
                    }
                }
                else
                {
                    // on envoit un message si l'unité était sans ravitaillement mais qu'elle en retrouve
                    if (lignePion.I_TOUR_SANS_RAVITAILLEMENT > 0)
                    {
                        lignePion.I_TOUR_SANS_RAVITAILLEMENT = 0;
                        if (!ClassMessager.EnvoyerMessage(lignePion, distanceRavitaillement, lignePion.I_RAVITAILLEMENT - ravitaillementInitial,
                                lignePion.I_MATERIEL - materielInitial, depotRavitaillement, ClassMessager.MESSAGES.MESSAGE_NOUVEAU_RAVITAILLEMENT))
                        {
                            message = string.Format("{0}(ID={1}, erreur sur EnvoyerMessage avec MESSAGE_NOUVEAU_RAVITAILLEMENT dans Ravitaillement", lignePion.S_NOM, lignePion.ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }
                    }
                    else
                    {
                        //on envoie un message pour indiquer que l'unité est bien ravitaillée
                        if (!ClassMessager.EnvoyerMessage(lignePion, distanceRavitaillement, lignePion.I_RAVITAILLEMENT - ravitaillementInitial,
                                lignePion.I_MATERIEL - materielInitial, depotRavitaillement, ClassMessager.MESSAGES.MESSAGE_RECU_RAVITAILLEMENT))
                        {
                            message = string.Format("{0}(ID={1}, erreur sur EnvoyerMessage avec MESSAGE_RECU_RAVITAILLEMENT dans Ravitaillement", lignePion.S_NOM, lignePion.ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }
                    }
                }
                ++i;
            }
            LogFile.Notifier("Fin Ravitaillement");
            return true;
        }

        /// <summary>
        /// Vérifie si l'un des cas a gagné ou si la partie est terminée, dans ce cas, le vainqueur est déterminé
        /// </summary>
        /// <returns>true si ok, false si ko</returns>
        private bool VictoireDefaite(out bool bFinDePartie)
        {
            bFinDePartie = false;
            int[] victoire = new int[2];
            int[] defaite = new int[2];
            Donnees.TAB_MODELE_PIONRow ligneModelePion;

            try
            {
                //vient-on de jouer le dernier tour ?
                if (Donnees.m_donnees.TAB_PARTIE[0].I_TOUR >= Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_TOURS)
                {
                    //En effet c'est le cas
                    LogFile.Notifier(string.Format("VictoireDefaite : fin de la partie, on a joué le dernier tour {0}/{1}.",
                        Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_TOURS));
                    bFinDePartie = true;
                }

                if (!bFinDePartie)
                {
                    //si l'un des camps a deux fois plus de corps démoralisés que l'autre et que cela constitue au moins la moitié de son armée, renforts compris.
                    //alors la partie s'arrête
                    victoire[0] = victoire[1] = 0;
                    defaite[0] = defaite[1] = 0;
                    for (int l = 0; l < Donnees.m_donnees.TAB_PION.Count; l++)
                    {
                        Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[l];
                        if (lignePion.effectifTotal > 0 && !lignePion.B_DETRUIT && !lignePion.estPrisonniers && !lignePion.estBlesses)
                        {
                            //unité combattante
                            ligneModelePion = lignePion.modelePion;

                            if (lignePion.B_FUITE_AU_COMBAT)
                            {
                                defaite[ligneModelePion.ID_NATION]++;
                            }
                            else
                            {
                                victoire[ligneModelePion.ID_NATION]++;
                            }
                        }
                    }

                    for (int l=0; l<Donnees.m_donnees.TAB_RENFORT.Count; l++)
                    {
                        Donnees.TAB_RENFORTRow ligneRenfort = Donnees.m_donnees.TAB_RENFORT[l];
                        if (ligneRenfort.effectifTotal > 0)
                        {
                            ligneModelePion = Donnees.m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION(ligneRenfort.ID_MODELE_PION);
                            victoire[ligneModelePion.ID_NATION]++;
                        }
                    }

                    LogFile.Notifier(string.Format("VictoireDefaite : defaite(fuite au combat)[0]={0}, defaite(fuite au combat)[1]={1}, victoire[0](jamais de fuite au combat)={2}, victoire[1](jamais de fuite au combat)={3}",
                                                    defaite[0], defaite[1], victoire[0], victoire[1]));
                    if ((defaite[0] > defaite[1] * 2 && defaite[0] > victoire[0]) || (defaite[1] > defaite[0] * 2 && defaite[1] > victoire[1]))
                    {
                        LogFile.Notifier("VictoireDefaite : fin de la partie, l'un des deux camps est définitivement battu.");
                        LogFile.Notifier("VictoireDefaite : Mais on continue tant qu'un joueur ne s'avoue pas vaincu !");
                        //bFinDePartie = true;
                    }
                }
                if (bFinDePartie)
                {
                    //il faut maintenant determiner le vainqueur en ajoutant les points des zones controllées 
                    foreach (Donnees.TAB_NOMS_CARTERow ligneNomCarte in Donnees.m_donnees.TAB_NOMS_CARTE)
                    {
                        if (ligneNomCarte.I_VICTOIRE > 0 && !ligneNomCarte.IsID_NATION_CONTROLENull())
                        {
                            //pts de victoire = corps defaits + zones controlées, donc controller une zone, équivaut à ajouter les pts chez les corps défaits de l'adversaire
                            if (0 == ligneNomCarte.ID_NATION_CONTROLE)
                                defaite[1] += ligneNomCarte.I_VICTOIRE;
                            else
                                defaite[0] += ligneNomCarte.I_VICTOIRE;
                        }
                    }
                    Donnees.m_donnees.TAB_PARTIE[0].ID_VICTOIRE = Constantes.CST_VICTOIRE_EGALITE;//egalite
                    if (defaite[0] > defaite[1])
                    {
                        Donnees.m_donnees.TAB_PARTIE[0].ID_VICTOIRE = 1;
                    }
                    else
                    {
                        Donnees.m_donnees.TAB_PARTIE[0].ID_VICTOIRE = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogFile.Notifier("VictoireDefaite : Exception : "+ ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// première phase du tour
        /// </summary>
        /// <returns>OK=true, KO=false</returns>
        private bool NouvelleHeure(bool bPartieCommence, out bool bRenfort)
        {
            LogFile.Notifier("Debut nouvelle heure");
            bRenfort = false;

            #region Blessures des chefs
            int i = 0;
            Donnees.TAB_PIONRow ligneChefRemplace;
            while(i<Donnees.m_donnees.TAB_PION.Count)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                if (/*lignePion.B_DETRUIT || */lignePion.IsI_TOUR_BLESSURENull()) { i++;  continue; }//un chef blessé est marqué détruit
                if (lignePion.I_DUREE_HORS_COMBAT > 7)
                {
                    //blessure grave sur plusieurs jours, si le leader est blessés depuis plus de CST_DUREE_INDISPONIBLE_SUR_BLESSURE on affecte un remplaçant
                    if (lignePion.I_TOUR_BLESSURE + Constantes.CST_DUREE_INDISPONIBLE_SUR_BLESSURE > Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)
                    {
                        //on envoie un message un peu bidon juste pour que le joueur sache bien que le pion est "mort"
                        //il faut l'envoyer avant de changer le rôle pour que le message arrive au joueur qui tient le pion
                        if (lignePion.I_DUREE_HORS_COMBAT == int.MaxValue)
                        {
                            if (!ClassMessager.EnvoyerMessageImmediat(lignePion, ClassMessager.MESSAGES.MESSAGE_CHEF_TUE))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (!ClassMessager.EnvoyerMessageImmediat(lignePion, ClassMessager.MESSAGES.MESSAGE_CHEF_BLESSE))
                            {
                                return false;
                            }
                        }

                        /*
                        ligneChefRemplacant = lignePion.CreationRemplacantChefBlesse();
                        if (null == ligneChefRemplacant)
                        {
                            LogFile.Notifier(string.Format("Erreur dans NouvelleHeure à l'appel de CreationRemplacantChefBlesse en créant le remplaçant de {0}:{1}", lignePion.ID_PION, lignePion.S_NOM));
                            return false;
                        }

                        Donnees.TAB_PIONRow lignePrecedentChefRemplace = lignePion.pionRemplace;
                        if (null != lignePrecedentChefRemplace)
                        {
                            //un chef remplaçant blessé est simplement soustrait par son nouveau remplaçant, on ne gère pas son retour
                            ligneChefRemplacant.ID_PION_REMPLACE = lignePrecedentChefRemplace.ID_PION;
                        }
                        else
                        {
                            ligneChefRemplacant.ID_PION_REMPLACE = lignePion.ID_PION;
                        }
                        */
                        ligneChefRemplace = lignePion.CreationRemplacantChefBlesse2();
                        if (null == ligneChefRemplace)
                        {
                            LogFile.Notifier(string.Format("Erreur dans NouvelleHeure à l'appel de CreationRemplacantChefBlesse en créant le remplaçant de {0}:{1}", lignePion.ID_PION, lignePion.S_NOM));
                            return false;
                        }

                        //dans tous les cas on considère le chef précédent comme mort pour qu'il n'apparaisse plus dans aucun compte rendu
                        lignePion.DetruirePion();
                        /*
                        Donnees.TAB_ROLERow ligneRole = Donnees.m_donnees.TAB_ROLE.TrouvePion(lignePion.ID_PION);
                        if (null != ligneRole) { ligneRole.ID_PION = ligneChefRemplacant.ID_PION; }

                        //Tout ce qui était lié au chef précédent doit être lié au nouveau chef
                        if (!TransfertsDesliens(lignePion, ligneChefRemplacant))
                        {
                            LogFile.Notifier(string.Format("Erreur dans NouvelleHeure à l'appel de TransfertsDesliens de {0}:{1} vers {2}:{3} ",
                                lignePion.ID_PION, lignePion.S_NOM, ligneChefRemplacant.ID_PION, ligneChefRemplacant.S_NOM));
                            return false;
                        }

                        //on envoit un message à tous les leaders pour indiquer que le chef a changé
                        //on envoit un message avec effet immédiat pour prévenir le joueur de l'arrivée du chef
                        if (!ClassMessager.EnvoyerMessageImmediat(ligneChefRemplacant, ClassMessager.MESSAGES.MESSAGE_CHEF_REMPLACANT))
                        {
                            return false;
                        }

                        //on prévient tous les autres rôles du changement
                        if (!MessageATousLesRoles(ligneChefRemplacant, ClassMessager.MESSAGES.MESSAGE_CHEF_REMPLACANT, false))
                        {
                            return false;
                        }
                        */
                        //on envoit un message à tous les leaders pour indiquer que le chef a changé
                        //on envoit un message avec effet immédiat pour prévenir le joueur de l'arrivée du chef
                        if (!ClassMessager.EnvoyerMessageImmediat(lignePion, ClassMessager.MESSAGES.MESSAGE_CHEF_REMPLACANT))
                        {
                            return false;
                        }

                        //on prévient tous les autres rôles du changement
                        if (!MessageATousLesRoles(lignePion, ClassMessager.MESSAGES.MESSAGE_CHEF_REMPLACANT, false))
                        {
                            return false;
                        }
                    }
                }

                #region le blessé guérit lentement...
                if (lignePion.I_DUREE_HORS_COMBAT>0 && lignePion.I_DUREE_HORS_COMBAT < int.MaxValue)
                {
                    lignePion.I_DUREE_HORS_COMBAT--;
                    if (lignePion.I_DUREE_HORS_COMBAT <= 0)
                    {
                        //Le precedent leader redevient actif (quid si le remplaçant a été remplacé ? -> on remplace pas un remplaçant, voir au-dessus)

                        //on recherche qui le remplacait, s'il n'y a pas de remplaçant, c'est qu'il s'agissait d'une blessure légère
                        ligneChefRemplace = lignePion.pionRemplacant;
                        if (null != ligneChefRemplace)
                        {
                            //on remet les caractéristiques du pion d'origine et c'est tout !
                            lignePion.S_NOM = ligneChefRemplace.S_NOM;
                            lignePion.I_TACTIQUE = ligneChefRemplace.I_TACTIQUE;
                            lignePion.I_STRATEGIQUE = ligneChefRemplace.I_STRATEGIQUE;
                            lignePion.C_NIVEAU_HIERARCHIQUE = ligneChefRemplace.C_NIVEAU_HIERARCHIQUE;
                            lignePion.I_DUREE_HORS_COMBAT = 0;
                            lignePion.B_BLESSES = false;

                            /*
                            //on réétablit le rôle au chef d'origine s'il y en avait un
                            Donnees.TAB_ROLERow ligneRole = Donnees.m_donnees.TAB_ROLE.TrouvePion(ligneChefRemplacant.ID_PION);
                            if (null != ligneRole) { ligneRole.ID_PION = lignePion.ID_PION; }

                            if (!TransfertsDesliens(ligneChefRemplacant, lignePion))
                            {
                                LogFile.Notifier(string.Format("Erreur dans NouvelleHeure à l'appel de TransfertsDesliens II de {0}:{1} vers {2}:{3} ",
                                    ligneChefRemplacant.ID_PION, ligneChefRemplacant.S_NOM, lignePion.ID_PION, lignePion.S_NOM));
                                return false;
                            }*/

                            //on envoit un message avec effet immédiat pour prévenir le joueur de l'arrivée du chef
                            //if (!ClassMessager.EnvoyerMessageImmediat(lignePion, ClassMessager.MESSAGES.MESSAGE_RETOUR_CHEF))
                            //{
                            //    return false;
                            //}

                            //on informe les joueurs de la reprise de poste
                            if (!MessageATousLesRoles(lignePion, ClassMessager.MESSAGES.MESSAGE_RETOUR_CHEF, false))
                            {
                                return false;
                            }

                            //on réactive l'ancier chef
                            /*
                            lignePion.B_DETRUIT = false;
                            lignePion.ID_CASE = ligneChefRemplacant.ID_CASE;
                            lignePion.B_TELEPORTATION = true;
                            //on détruit le remplaçant
                            ligneChefRemplacant.DetruirePion();
                            */
                        }
                    }
                }
                #endregion
                i++;
            }
            #endregion

            #region Réduction des tours de fuite
            LogFile.Notifier("**** Réduction des tours de fuite ****");

            for (int l = 0; l < Donnees.m_donnees.TAB_PION.Count; l++)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[l];
                if (lignePion.B_DETRUIT) { continue; }
                Donnees.TAB_BATAILLERow ligneBataille = null;
                if (!lignePion.IsID_BATAILLENull())
                {
                    ligneBataille = Donnees.m_donnees.TAB_BATAILLE.FindByID_BATAILLE(lignePion.ID_BATAILLE);
                }

                //On ne décroit les tours de fuite et de retraite que lorsque la bataille est terminé
                //(pour éviter de faire -1 quand l'ordre de retraite est donné un tour avant, et la fuite, bien, bien avant...
                if (null == ligneBataille || !ligneBataille.IsI_TOUR_FINNull())
                {
                    if (lignePion.I_TOUR_FUITE_RESTANT > 0)
                    {
                        lignePion.I_TOUR_FUITE_RESTANT--;
                        if (0 == lignePion.I_TOUR_FUITE_RESTANT)
                        {
                            //l'unité n'est plus liée au précédent combat
                            lignePion.SetID_BATAILLENull();
                            lignePion.SetI_ZONE_BATAILLENull();
                        }
                    }

                    if (lignePion.I_TOUR_RETRAITE_RESTANT > 0)
                    {
                        lignePion.I_TOUR_RETRAITE_RESTANT--;
                        if (0 == lignePion.I_TOUR_RETRAITE_RESTANT)
                        {
                            //l'unité n'est plus liée au précédent combat
                            lignePion.SetID_BATAILLENull();
                            lignePion.SetI_ZONE_BATAILLENull();
                        }
                    }
                }
            }
            #endregion

            #region arrivée des renforts
            foreach (Donnees.TAB_RENFORTRow ligneRenfort in Donnees.m_donnees.TAB_RENFORT)
            {
                if (ligneRenfort.I_TOUR_ARRIVEE == Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)
                {
                    //on vérifie que l'on a pas déjà ajouté le pion (peut arriver si l'on a du relancer l'execution)
                    if (null==Donnees.m_donnees.TAB_PION.FindByID_PION(ligneRenfort.ID_PION))
                    {
                        Donnees.TAB_PIONRow lignePionRenfort = Donnees.m_donnees.TAB_PION.AddTAB_PIONRow(
                            ligneRenfort.ID_MODELE_PION,
                            ligneRenfort.ID_PION_PROPRIETAIRE,
                            -1,
                            -1,
                            ligneRenfort.S_NOM,
                            ligneRenfort.I_INFANTERIE,
                            ligneRenfort.I_INFANTERIE,
                            ligneRenfort.I_CAVALERIE,
                            ligneRenfort.I_CAVALERIE,
                            ligneRenfort.I_ARTILLERIE,
                            ligneRenfort.I_ARTILLERIE,
                            ligneRenfort.I_FATIGUE,
                            ligneRenfort.I_MORAL,
                            ligneRenfort.I_MORAL_MAX,
                            ligneRenfort.I_EXPERIENCE,
                            ligneRenfort.I_TACTIQUE,
                            ligneRenfort.I_STRATEGIQUE,
                            ligneRenfort.C_NIVEAU_HIERARCHIQUE,
                            0,//int I_DISTANCE_A_PARCOURIR,
                            0,//int I_NB_PHASES_MARCHE_JOUR,
                            0,//int I_NB_PHASES_MARCHE_NUIT,
                            0,//I_NB_HEURES_COMBAT
                            ligneRenfort.ID_CASE,
                            0, //int I_TOUR_SANS_RAVITAILLEMENT
                            -1,//int ID_BATAILLE, null ensuite
                            -1,//int I_ZONE_BATAILLE, null ensuite
                            0, //I_TOUR_RETRAITE_RESTANT
                            0,//int I_TOUR_FUITE_RESTANT
                            false,//bool B_DETRUIT
                            false,//B_FUITE_AU_COMBAT
                            false,//bool B_INTERCEPTION
                            false,//B_FUITE_AU_COMBAT
                            true, //B_TELEPORTATION
                            false, //B_ENNEMI_OBSERVABLE,
                            ligneRenfort.I_MATERIEL,//I_MATERIEL,
                            ligneRenfort.I_RAVITAILLEMENT,//I_RAVITAILLEMENT,
                            ligneRenfort.B_CAVALERIE_DE_LIGNE,//B_CAVALERIE_DE_LIGNE,
                            ligneRenfort.B_CAVALERIE_LOURDE,//B_CAVALERIE_LOURDE,
                            ligneRenfort.B_GARDE,//B_GARDE,
                            ligneRenfort.B_VIEILLE_GARDE,//B_VIEILLE_GARDE,
                            0,//I_TOUR_CONVOI_CREE,
                            -1,//ID_DEPOT_SOURCE
                            0,//I_SOLDATS_RAVITAILLES,
                            0,//I_NB_HEURES_FORTIFICATION,
                            0,//I_NIVEAU_FORTIFICATION,
                            -1,//ID_PION_REMPLACE,
                            0,//I_DUREE_HORS_COMBAT,
                            0,//I_TOUR_BLESSURE,
                            false,//B_BLESSES,
                            false,//B_PRISONNIERS,
                            ligneRenfort.B_RENFORT,//B_RENFORT
                            -1,//ID_LIEU_RATTACHEMENT,
                            ligneRenfort.C_NIVEAU_DEPOT,//C_NIVEAU_DEPOT
                            - 1,//ID_PION_ESCORTE, 
                            0,//I_INFANTERIE_ESCORTE, 
                            0,//I_CAVALERIE_ESCORTE
                            0,//I_MATERIEL_ESCORTE
                            0,//I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT
                            ligneRenfort.I_VICTOIRE
                            );

                        lignePionRenfort.SetID_ANCIEN_PION_PROPRIETAIRENull();
                        lignePionRenfort.SetID_NOUVEAU_PION_PROPRIETAIRENull();
                        lignePionRenfort.SetI_ZONE_BATAILLENull();
                        lignePionRenfort.SetID_BATAILLENull();
                        lignePionRenfort.SetID_PION_REMPLACENull();
                        lignePionRenfort.SetID_LIEU_RATTACHEMENTNull();
                        lignePionRenfort.SetID_PION_ESCORTENull();
                        lignePionRenfort.SetID_DEPOT_SOURCENull();

                        //il faut imposer l'id_pion du renfort
                        lignePionRenfort.ID_PION = ligneRenfort.ID_PION;

                        //Les leaders indiquent leurs positions respectives dans les deux sens
                        if (lignePionRenfort.estJoueur)
                        {
                            //on envoit un message avec effet immédiat pour prévenir le joueur de l'arrivée de la troupe
                            if (string.Empty == ligneRenfort.S_MESSAGE_ARRIVEE)
                            {
                                if (!ClassMessager.EnvoyerMessageImmediat(lignePionRenfort, ClassMessager.MESSAGES.MESSAGE_RENFORT))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (!ClassMessager.EnvoyerMessageImmediat(lignePionRenfort, ClassMessager.MESSAGES.MESSAGE_RENFORT, ligneRenfort.S_MESSAGE_ARRIVEE))
                                {
                                    return false;
                                }
                            }

                            if (!MessageATousLesRoles(lignePionRenfort, ClassMessager.MESSAGES.MESSAGE_POSITION, true))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            //on envoit un message avec effet immédiat pour prévenir le joueur de l'arrivée de la troupe
                            if (string.Empty == ligneRenfort.S_MESSAGE_ARRIVEE)
                            {
                                if (!ClassMessager.EnvoyerMessageImmediat(lignePionRenfort, ClassMessager.MESSAGES.MESSAGE_RENFORT))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (!ClassMessager.EnvoyerMessageImmediat(lignePionRenfort, ClassMessager.MESSAGES.MESSAGE_RENFORT, ligneRenfort.S_MESSAGE_ARRIVEE))
                                {
                                    return false;
                                }
                            }

                        }
                        bRenfort = true;
                    }
                }
            }
            #endregion

            if (!ControleDesVilles()) { return false; }

            LogFile.Notifier("Fin nouvelle heure");

            return true;
        }

        /// <summary>
        /// Contrôle des villes avec points de victoire ou hopital ou prison
        /// </summary>
        /// <returns>true si ok, false si ko</returns>
        private bool ControleDesVilles()
        {
            string message, messageErreur;
            string requete;
            int idNation0 = -1, idNation1 = -1;
            int[] zone = new int[2];
            List<Donnees.TAB_PIONRow> listePionControle0 = new List<Donnees.TAB_PIONRow>();
            List<Donnees.TAB_PIONRow> listePionControle1 = new List<Donnees.TAB_PIONRow>();
            LogFile.Notifier("Debut ControleDesVilles");

            foreach (Donnees.TAB_NOMS_CARTERow ligneNomCarte in Donnees.m_donnees.TAB_NOMS_CARTE)
            {
                Donnees.TAB_PIONRow lignePion;
                Donnees.TAB_MODELE_PIONRow ligneModelePion;
                listePionControle0.Clear();
                listePionControle1.Clear();
                if (ligneNomCarte.I_VICTOIRE > 0 || ligneNomCarte.B_HOPITAL || ligneNomCarte.B_PRISON)
                {
                    Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneNomCarte.ID_CASE);
                    //on regarde qui contrôle la zone à CST_RECHERCHE_ZONE_VICTOIRE kilomètres alentour
                    #region calcul du cadre de recherche
                    int xCaseHautGauche = ligneCase.I_X - Constantes.CST_RECHERCHE_ZONE_VICTOIRE * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                    int yCaseHautGauche = ligneCase.I_Y - Constantes.CST_RECHERCHE_ZONE_VICTOIRE * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                    int xCaseBasDroite = ligneCase.I_X + Constantes.CST_RECHERCHE_ZONE_VICTOIRE * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                    int yCaseBasDroite = ligneCase.I_Y + Constantes.CST_RECHERCHE_ZONE_VICTOIRE * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                    xCaseHautGauche = Math.Max(0, xCaseHautGauche);
                    yCaseHautGauche = Math.Max(0, yCaseHautGauche);
                    xCaseBasDroite = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1, xCaseBasDroite);
                    yCaseBasDroite = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1, yCaseBasDroite);
                    #endregion

                    Donnees.TAB_CASERow[] lignesCaseRecherche = Donnees.m_donnees.TAB_CASE.CasesCadre(xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite);

                    zone[0] = zone[1] = 0;
                    for (int l = 0; l < lignesCaseRecherche.Count(); l++)
                    {
                        Donnees.TAB_CASERow ligneCaseRecherche = lignesCaseRecherche[l];
                        if (!ligneCaseRecherche.EstInnocupe()) //.ID_PROPRIETAIRE. != DataSetCoutDonnees.CST_AUCUNPROPRIETAIRE)
                        {
                            lignePion = ligneCaseRecherche.TrouvePionSurCase();
                            if (null == lignePion)
                            {
                                message = string.Format("ControleDesVilles : erreur FindByID_PION introuvable sur {0} ou {1}",
                                    ligneCaseRecherche.ID_PROPRIETAIRE.ToString(), ligneCaseRecherche.ID_NOUVEAU_PROPRIETAIRE.ToString());
                                LogFile.Notifier(message, out messageErreur);
                                return false;
                            }

                            ligneModelePion = lignePion.modelePion;
                            if (null == ligneModelePion)
                            {
                                message = string.Format("ControleDesVilles : erreur FindByID_MODELE_PION introuvable sur {0}", lignePion.ID_MODELE_PION);
                                LogFile.Notifier(message, out messageErreur);
                                return false;
                            }

                            //un point de plus sur la zone pour son camp !
                            if (idNation0 == -1 || idNation0 == ligneModelePion.ID_NATION)
                            {
                                idNation0 = ligneModelePion.ID_NATION;
                                zone[0]++;
                                if (null==listePionControle0.Find(x => x==lignePion)) { listePionControle0.Add(lignePion); }
                            }
                            else
                            {
                                idNation1 = ligneModelePion.ID_NATION;
                                zone[1]++;
                                if (null == listePionControle1.Find(x => x == lignePion)) { listePionControle1.Add(lignePion); }
                            }
                        }
                    }

                    // alors, qui contrôle la zone ?
                    //il faut au moins 3 fois plus de présence pour dominer une zone
                    int nationControle = -1;
                    if (zone[0] > 3 * zone[1])
                    {
                        nationControle = idNation0;
                    }
                    else
                    {
                        if (zone[1] > 3 * zone[0])
                        {
                            nationControle = idNation1;
                        }
                    }
                    if (nationControle < 0) { continue; }

                    if (nationControle >= 0 && !ligneNomCarte.IsID_NATION_CONTROLENull() && ligneNomCarte.ID_NATION_CONTROLE != nationControle)
                    {
                        //Quand un hopital est capturé, la moitié des blessés "disparaissent", ceux qui restent ne sont soignés que s'ils sont "libérés"
                        if (ligneNomCarte.B_HOPITAL)
                        {
                            requete = string.Format("B_BLESSES=true AND ID_LIEU_RATTACHEMENT={0}", ligneNomCarte.ID_NOM);
                            Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                            Donnees.TAB_PIONRow[] lignesPionResultat = (Donnees.TAB_PIONRow[])Donnees.m_donnees.TAB_PION.Select(requete);
                            for (int l = 0; l < lignesPionResultat.Count(); l++)
                            {
                                Donnees.TAB_PIONRow lignePionBlesses = lignesPionResultat[l];
                                if (lignePionBlesses.effectifTotal / 2 < Constantes.CST_TAILLE_MINIMUM_UNITE)
                                {
                                    //Il n'y a plus assez de blessés pour constituer une unité de renforts suffisants
                                    lignePionBlesses.I_INFANTERIE = 0;
                                    lignePionBlesses.I_CAVALERIE = 0;
                                    lignePionBlesses.I_ARTILLERIE = 0;
                                }
                                else
                                {
                                    lignePionBlesses.I_INFANTERIE = lignePionBlesses.I_INFANTERIE / 2;
                                    lignePionBlesses.I_CAVALERIE = lignePionBlesses.I_CAVALERIE / 2;
                                    lignePionBlesses.I_ARTILLERIE = lignePionBlesses.I_ARTILLERIE / 2;
                                }
                            }
                            Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        }

                        //Quand une prison change de camp, les prisonniers deviennent des renforts
                        Random de = new Random();
                        if (ligneNomCarte.B_PRISON)
                        {
                            requete = string.Format("B_PRISONNIERS=true AND ID_LIEU_RATTACHEMENT={0}", ligneNomCarte.ID_NOM);
                            Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                            Donnees.TAB_PIONRow[] lignesPionResultat = (Donnees.TAB_PIONRow[])Donnees.m_donnees.TAB_PION.Select(requete);
                            for (int l = 0; l < lignesPionResultat.Count(); l++)
                            {
                                //on tire au hasard le pion qui a fait la libération
                                Donnees.TAB_PIONRow lignePionLiberateur = (nationControle == idNation0) ? listePionControle0.ElementAt(de.Next(0,listePionControle0.Count)) : listePionControle1.ElementAt(de.Next(0,listePionControle1.Count));

                                Donnees.TAB_PIONRow lignePionPrisonniers = lignesPionResultat[l];
                                lignePionPrisonniers.B_PRISONNIERS = false;
                                lignePionPrisonniers.SetID_LIEU_RATTACHEMENTNull();
                                lignePionPrisonniers.S_NOM = "Renforts de " + lignePionPrisonniers.S_NOM;
                                lignePionPrisonniers.I_MATERIEL = 0; //on le laisse pas de matériel aux prisonniers en prison :-)
                                lignePionPrisonniers.B_DETRUIT = false;
                                lignePionPrisonniers.B_RENFORT = true;
                                lignePionPrisonniers.ID_MODELE_PION = lignePionLiberateur.ID_MODELE_PION;
                                lignePionPrisonniers.ID_PION_PROPRIETAIRE = lignePionLiberateur.ID_PION_PROPRIETAIRE;

                                //envoie d'un message pour prévenir de cette libération
                                if (!ClassMessager.EnvoyerMessage(lignePionPrisonniers, ClassMessager.MESSAGES.MESSAGE_PRISONNIER_LIBERE))
                                {
                                    message = string.Format("ControleDesVilles : erreur sur l'envoi de message pour MESSAGE_PRISONNIER_LIBERE {0}:{1}", lignePionPrisonniers.S_NOM, lignePionPrisonniers.ID_PION);
                                    LogFile.Notifier(message, out messageErreur);
                                    return false;
                                }
                            }
                            Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        }
                    }
                    //si la nation change, on modifie la valeur, sinon on garde la valeur par défaut
                    if (nationControle >= 0) { ligneNomCarte.ID_NATION_CONTROLE = nationControle; }
                }
            }
            LogFile.Notifier("Fin ControleDesVilles");
            return true;
        }

        private bool TransfertsDesliens(Donnees.TAB_PIONRow ligneAncienPion, Donnees.TAB_PIONRow ligneNouveauPion)
        {
            string requete;
            Donnees.TAB_PIONRow[] resPions;
            Donnees.TAB_MESSAGERow[] resMessages;

            requete = string.Format("ID_PION_PROPRIETAIRE = {0}", ligneAncienPion.ID_PION);
            Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            resPions = (Donnees.TAB_PIONRow[])Donnees.m_donnees.TAB_PION.Select(requete);
            for (int l=0; l< resPions.Count(); l++)
            {
                Donnees.TAB_PIONRow lignePion = resPions[l];
                lignePion.ID_PION_PROPRIETAIRE = ligneNouveauPion.ID_PION;
            }

            requete = string.Format("ID_NOUVEAU_PION_PROPRIETAIRE = {0}", ligneAncienPion.ID_PION);
            resPions = (Donnees.TAB_PIONRow[])Donnees.m_donnees.TAB_PION.Select(requete);
            for (int l = 0; l < resPions.Count(); l++)
            {
                Donnees.TAB_PIONRow lignePion = resPions[l];
                lignePion.ID_NOUVEAU_PION_PROPRIETAIRE = ligneNouveauPion.ID_PION;
            }

            requete = string.Format("ID_ANCIEN_PION_PROPRIETAIRE = {0}", ligneAncienPion.ID_PION);
            resPions = (Donnees.TAB_PIONRow[])Donnees.m_donnees.TAB_PION.Select(requete);
            for (int l = 0; l < resPions.Count(); l++)
            {
                Donnees.TAB_PIONRow lignePion = resPions[l];
                lignePion.ID_ANCIEN_PION_PROPRIETAIRE = ligneNouveauPion.ID_PION;
            }
            Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);

            requete = string.Format("ID_PION_PROPRIETAIRE = {0}", ligneAncienPion.ID_PION);
            Monitor.Enter(Donnees.m_donnees.TAB_MESSAGE.Rows.SyncRoot);
            resMessages = (Donnees.TAB_MESSAGERow[])Donnees.m_donnees.TAB_MESSAGE.Select(requete);
            for (int l=0; l<resMessages.Count(); l++)
            {
                Donnees.TAB_MESSAGERow ligneMessage = resMessages[l];
                ligneMessage.ID_PION_PROPRIETAIRE = ligneNouveauPion.ID_PION;
            }
            Monitor.Exit(Donnees.m_donnees.TAB_MESSAGE.Rows.SyncRoot);

            return true;
        }

        private static bool MessageATousLesRoles(Donnees.TAB_PIONRow lignePionSourceMessage, ClassMessager.MESSAGES typeMessage, bool bMessageInverse)
        {
            foreach (Donnees.TAB_ROLERow ligneRole in Donnees.m_donnees.TAB_ROLE)
            {
                Donnees.TAB_PIONRow lignePionRole = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneRole.ID_PION);

                if (null != lignePionRole && !lignePionRole.estEnnemi(lignePionSourceMessage) && (lignePionRole.ID_PION != lignePionSourceMessage.ID_PION))
                {
                    //message immédiat à tous les autres rôles amis
                    string phrase;
                    phrase = ClassMessager.GenererPhrase(lignePionSourceMessage, typeMessage, 0, 0, 0, 0, 0, null, lignePionRole, null, null, 0, 0, 0, string.Empty);
                    if (!ClassMessager.EnvoyerMessage(lignePionSourceMessage, lignePionRole, typeMessage, phrase, true))
                    {
                        return false;
                    }

                    //message de tous les autres rôles pour prévenir de leur position à ce nouveau leader
                    if (bMessageInverse)
                    {
                        phrase = ClassMessager.GenererPhrase(lignePionRole, ClassMessager.MESSAGES.MESSAGE_POSITION, 0, 0, 0, 0, 0, null, lignePionSourceMessage, null, null, 0, 0, 0, string.Empty);
                        if (!ClassMessager.EnvoyerMessage(lignePionRole, lignePionSourceMessage, ClassMessager.MESSAGES.MESSAGE_POSITION, phrase, true))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Mise en place des nouveaux messages saisies par les joueurs en mode forum (partie non commencée ou lien direct)
        /// </summary>
        /// <returns>OK=true, KO=false</returns>
        private bool NouveauxMessages()
        {
            /* ne marche pas pour les messages forum en cours de partie
            int tourMax, phaseMax;

            //on recherche le dernier identfiant en base
            if (0 == Donnees.m_donnees.TAB_MESSAGE.Count())
            {
                //pour l'instant aucun message en base
                tourMax = phaseMax = 0;
            }
            else
            {
                System.Nullable<int> maxIdMessage =
                    (from message in Donnees.m_donnees.TAB_MESSAGE
                     orderby message.I_TOUR_DEPART descending, message.I_PHASE_DEPART descending
                     select message.ID_MESSAGE).First();
                Donnees.TAB_MESSAGERow ligneMessageMax = Donnees.m_donnees.TAB_MESSAGE.FindByID_MESSAGE((int)maxIdMessage);
                tourMax = ligneMessageMax.I_TOUR_DEPART;
                phaseMax = ligneMessageMax.I_PHASE_DEPART;
            }
            */

            List<ClassDataMessage> liste = m_iWeb.ListeMessages(Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);
            foreach (ClassDataMessage message in liste)
            {
                //if (message.ID_MESSAGE > ligneMessageMax[0].ID_MESSAGE), non fiable, même après un clear, le compteur ID ne repart pas à zéro
                //if (tourDepart>=tourMax && phaseDepart>phaseMax) ->pour le forum en cours de partie, ne marche pas, phaseMax peut être égal à 100
                Donnees.TAB_MESSAGERow ligneMessagePresent = Donnees.m_donnees.TAB_MESSAGE.FindByID_MESSAGE(message.ID_MESSAGE);
                if (null== ligneMessagePresent)
                {
                    Donnees.TAB_PIONRow lignePionEmetteur = Donnees.m_donnees.TAB_PION.FindByID_PION(message.ID_EMETTEUR);
                    if (null == lignePionEmetteur)
                    {
                        LogFile.Notifier("NouveauxMessages: Impossible de trouver un pion emetteur pour le message ID=" + message.ID_MESSAGE);
                        return false;
                    }
                    int tourDepart, phaseDepart, tourArrivee, phaseArrivee;
                    ClassMessager.TourPhase(message.DT_DEPART, out tourDepart, out phaseDepart);
                    ClassMessager.TourPhase(message.DT_ARRIVEE, out tourArrivee, out phaseArrivee);
                    int iTourSansRavitaillement = lignePionEmetteur.I_TOUR_SANS_RAVITAILLEMENT;

                    Donnees.m_donnees.TAB_MESSAGE.AddTAB_MESSAGERow(
                        message.ID_MESSAGE,
                        message.ID_EMETTEUR,
                        message.ID_PION_PROPRIETAIRE,
                        (int)ClassMessager.MESSAGES.MESSAGE_PERSONNEL,
                        tourArrivee,
                        phaseArrivee,
                        tourDepart,
                        phaseDepart,
                        message.S_MESSAGE,
                        lignePionEmetteur.I_INFANTERIE,
                        lignePionEmetteur.I_CAVALERIE,
                        lignePionEmetteur.I_ARTILLERIE,
                        lignePionEmetteur.I_FATIGUE,
                        lignePionEmetteur.Moral,
                        iTourSansRavitaillement,
                        -1,//ID_BATAILLE
                        -1,//I_ZONE_BATAILLE
                        Math.Max(lignePionEmetteur.I_TOUR_FUITE_RESTANT, lignePionEmetteur.I_TOUR_RETRAITE_RESTANT),
                        lignePionEmetteur.B_DETRUIT,
                        lignePionEmetteur.ID_CASE,
                        lignePionEmetteur.ID_CASE,
                        lignePionEmetteur.ID_CASE,
                        lignePionEmetteur.I_NB_PHASES_MARCHE_JOUR,
                        lignePionEmetteur.I_NB_PHASES_MARCHE_NUIT,
                        lignePionEmetteur.I_NB_HEURES_COMBAT,
                        lignePionEmetteur.I_MATERIEL,
                        lignePionEmetteur.I_RAVITAILLEMENT,
                        lignePionEmetteur.I_SOLDATS_RAVITAILLES,
                        lignePionEmetteur.I_NB_HEURES_FORTIFICATION,
                        lignePionEmetteur.I_NIVEAU_FORTIFICATION,
                        lignePionEmetteur.I_DUREE_HORS_COMBAT,
                        lignePionEmetteur.IsI_TOUR_BLESSURENull() ? -1 : lignePionEmetteur.I_TOUR_BLESSURE,
                        lignePionEmetteur.C_NIVEAU_DEPOT);
                }
            }
            return true;
        }

        /// <summary>
        /// Mise en place des nouveaux ordres saisies par les joueurs
        /// </summary>
        /// <param name="bPartieCommence">true si la partie à commencé, false si on est en pré-discussion</param>
        /// <returns>OK=true, KO=false</returns>
        private bool NouveauxOrdres(out int nbTourExecutes)
        {
            //string message, messageErreur;
            int tourDernierOrdre = -1;//pour s'assurer que l'on a bon fichier d'ordres !
            Donnees.TAB_ORDRERow ligneOrdreWeb;
            //int id_ordre_suivant;

            LogFile.Notifier("Debut NouveauxOrdres");
            nbTourExecutes = 0;
            if (Donnees.m_donnees.TAB_PARTIE[0].I_PHASE != 0)
            {
                //il doit s'agir d'un rechargement en cours de route
                LogFile.Notifier("NouveauxOrdres : pas de traitement, la phase n'est pas égale à 0");
                return true;
            }

            List<ClassDataOrdre> liste = m_iWeb.ListeOrdres(Donnees.m_donnees.TAB_PARTIE[0].ID_JEU,
                                                            Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);

            //pour test
            /*
            //DataSetCoutDonnees.m_donnees.TAB_ORDRE.Clear();//à virer ensuite
            //Donnees.TAB_ORDRERow ligneOrdreDebug = Donnees.m_donnees.TAB_ORDRE.FindByID_ORDRE(1067);

            ClassDataOrdre ordreTest = new ClassDataOrdre();
            ordreTest.I_TOUR = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
            ordreTest.ID_PARTIE = Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE;
            ordreTest.I_TYPE = Constantes.ORDRES.ARRET;// Constantes.ORDRES.CONSTRUIRE_PONTON;
            ordreTest.ID_ORDRE = 10000;
            ordreTest.ID_PION = 110;
            ordreTest.ID_PION_DESTINATAIRE = 111;
            liste.Add(ordreTest);

            ordreTest = new ClassDataOrdre();
            ordreTest.I_TOUR = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
            ordreTest.ID_PARTIE = Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE;
            ordreTest.I_TYPE = Constantes.ORDRES.ARRET; //Constantes.ORDRES.CONSTRUIRE_PONTON;
            ordreTest.ID_ORDRE = 10001;
            ordreTest.ID_PION = 110;
            ordreTest.ID_PION_DESTINATAIRE = 112;
            liste.Add(ordreTest);
             * */

            foreach (ClassDataOrdre ordre in liste)
            {
                //si l'ordre a bien été envoyé à ce tour et n'est pas déjà inclus dans la table, bref s'il s'agit d'un nouvel
                //ordre saisie par un joueur, on l'ajoute
                ligneOrdreWeb = Donnees.m_donnees.TAB_ORDRE.OrdreWeb(ordre.ID_ORDRE);
                tourDernierOrdre = Math.Max(tourDernierOrdre, ordre.I_TOUR);
                if ((Donnees.m_donnees.TAB_PARTIE[0].I_TOUR == ordre.I_TOUR) && null == ligneOrdreWeb)
                {
                    //si l'ordre est l'ordre suivant d'un autre, on ne fait rien, ce sera traité sur l'ordre général parent
                    //int o = 0;
                    //while (o < liste.Count())
                    //{
                    //    ClassDataOrdre ordreListe = liste[o];
                    //    if (ordreListe.ID_ORDRE_SUIVANT == ordre.ID_ORDRE)
                    //    {
                    //        break;
                    //    }
                    //    o++;
                    //}
                    //if (o == liste.Count())
                    {
                        //ce n'est pas l'ordre suivant d'un autre, on fait le traitement
                        Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ordre.ID_PION);
                        Donnees.TAB_PIONRow lignePionDestination = Donnees.m_donnees.TAB_PION.FindByID_PION(ordre.ID_PION_DESTINATAIRE);
                        EnvoyerNouvelOrdre(liste, ordre, lignePion, lignePionDestination/*, false, out id_ordre_suivant*/);
                    }
                }
            }

            if (Donnees.m_donnees.TAB_PARTIE[0].I_TOUR != tourDernierOrdre)
            {
                if (DialogResult.No == MessageBox.Show("Attention ! Pas de nouveaux ordres à ce tour, derniers ordres fait au tour:" + tourDernierOrdre + ". Est-ce normal ?", "NouveauxOrdres()", MessageBoxButtons.YesNo))
                {
                    LogFile.Notifier("NouveauxOrdres : derniers ordres reçus au tour " + tourDernierOrdre);
                    return false;
                }
                nbTourExecutes = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR - Donnees.m_donnees.TAB_PARTIE[0].I_TOUR_NOTIFICATION;
            }
            else
            {
                List<ClassDataPartie> listeParties = m_iWeb.ListeParties(Donnees.m_donnees.TAB_PARTIE[0].ID_JEU, Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);
                nbTourExecutes = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR - listeParties[0].I_TOUR;
            }

            LogFile.Notifier("Fin NouveauxOrdres");

            return true;
        }

        private bool EnvoyerNouvelOrdre(List<ClassDataOrdre> liste, ClassDataOrdre ordre, Donnees.TAB_PIONRow lignePion, Donnees.TAB_PIONRow lignePionDestination/*, bool bOrdreSuivant, out int id_ordre_suivant*/)
        {
            int id_case_destination;
            ClassMessager.COMPAS compas;
            Donnees.TAB_ORDRERow ligneOrdre;
            //Donnees.TAB_ORDRERow ligneOrdreNouveau;
            Donnees.TAB_ORDRERow ligneOrdreARemettre = null;
            Donnees.TAB_PIONRow  lignePionMessager;

            /* Pb, cela met les ordres dans l'ordre inverse de traitement (part l'insertion), quand on recherche l'ordre en cours, on récupère alors le dernier ordre de la liste au lieu du premier
            id_ordre_suivant = -1;
            if (ordre.ID_ORDRE_SUIVANT > 0)
            {
                //il faut d'abord traité l'ordre suivant pour avoir sa valeur d'id

                //on recherche l'ordre correspondant dans la liste
                ClassDataOrdre ordreListe = null;
                int i = 0;
                while (i <liste.Count())
                {
                    ordreListe = liste[i];
                    Donnees.TAB_ORDRERow ligneOrdreWeb = Donnees.m_donnees.TAB_ORDRE.OrdreWeb(ordre.ID_ORDRE);
                    if ((Donnees.m_donnees.TAB_PARTIE[0].I_TOUR == ordre.I_TOUR) && null == ligneOrdreWeb)
                    {
                        if (ordreListe.ID_ORDRE == ordre.ID_ORDRE_SUIVANT)
                        {
                            break;
                        }
                    }
                    i++;
                }
                if (i == liste.Count())
                {
                    //on ne devrait jamais se retrouver dans cette situation cela veut dire que l'ordre suivant a été traité au préalable ou n'existe pas
                    LogFile.Notifier("EnvoyerNouvelOrdre : impossible de retrouver le nouvel ordre suivant =" + ordre.ID_ORDRE_SUIVANT + " de l'ordre id=" + ordre.ID_ORDRE);
                    return false;
                }
                //on a traite l'ordre suivant pour avoir son ID
                if (!EnvoyerNouvelOrdre(liste, ordreListe, lignePion, lignePionDestination, true, out id_ordre_suivant))
                {
                    return false;
                }
            }
            */
            Donnees.TAB_ORDRERow ligneOrdrePrecedent = null;
            foreach (ClassDataOrdre ordreWeb in liste)
            {
                if (ordreWeb.ID_ORDRE_SUIVANT == ordre.ID_ORDRE)
                {
                    ligneOrdrePrecedent = Donnees.m_donnees.TAB_ORDRE.OrdreWeb(ordreWeb.ID_ORDRE);
                }
            }

            switch (ordre.I_TYPE)
            {
                case Constantes.ORDRES.MESSAGE:
                    #region envoi d'un message textuel
                    //if (bPartieCommence)
                    {
                        //il faut envoyer un message
                        compas = ClassMessager.DirectionOrdreVersCompas(ordre.I_DIRECTION);
                        if (!ClassMessager.ZoneGeographiqueVersCase(lignePion, ordre.I_DISTANCE, compas, ordre.ID_NOM_LIEU, out id_case_destination))
                        {
                            ligneOrdreARemettre = DestinationImpossible(ordre, lignePion, id_case_destination);
                        }
                        else
                        {
                            if (!ClassMessager.EnvoyerMessage(lignePion, lignePionDestination, ClassMessager.MESSAGES.MESSAGE_PERSONNEL, ordre.S_MESSAGE, false, id_case_destination))
                            {
                                return false;
                            }

                            LogFile.Notifier(string.Format("NouveauxOrdres envoie d'un message de {0}({1}) à {2}({3}) : {4}",
                                lignePion.S_NOM,
                                lignePion.ID_PION,
                                lignePionDestination.S_NOM,
                                lignePionDestination.ID_PION,
                                ordre.S_MESSAGE));
                        }
                    }
                    /*
                    else
                    {
                        //cas particulier, avant début de partie, chaque message est envoyé à tous les joueurs d'un même camp immédiatement
                        //il faut également n'envoyer que des messages qui n'ont pas déjà été envoyés !
                        //ID_ORDRE n'est pas un bon critère car d'autres ordres peuvent être crées par le programme
                        if (null == Donnees.m_donnees.TAB_ORDRE.FindByID_ORDRE(ordre.ID_ORDRE))
                        {
                            //envoie à tous les autres pion alliés ayant un rôle
                            //il faut rechercher tous les rôles dans les pions ont la même nation que le pion de l'emetteur.
                            Donnees.TAB_MODELE_PIONRow ligneModelePion = Donnees.m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION(lignePion.ID_MODELE_PION);

                            var result = from Role in Donnees.m_donnees.TAB_ROLE
                                         join Pion in Donnees.m_donnees.TAB_PION
                                         on new { Role.ID_PION } equals new { Pion.ID_PION }
                                         join Modele in Donnees.m_donnees.TAB_MODELE_PION
                                         on new { Pion.ID_MODELE_PION, ligneModelePion.ID_NATION } equals new { Modele.ID_MODELE_PION, Modele.ID_NATION }
                                         select Role.ID_PION;

                            if (result.Count() > 0)
                            {
                                //on a trouvé le modèle, il faut modifier celui du pion associé
                                for (int i = 0; i < result.Count(); i++)
                                {
                                    Donnees.TAB_PIONRow lignePionDestinationDepart = Donnees.m_donnees.TAB_PION.FindByID_PION(result.ElementAt(i));
                                    if (!ClassMessager.EnvoyerMessage(lignePion, lignePionDestinationDepart, ClassMessager.MESSAGE_PERSONNEL, ordre.S_MESSAGE, true))
                                    {
                                        return false;
                                    }
                                    LogFile.Notifier(string.Format("NouveauxOrdres envoie d'un message de {0}({1}) à {2}({3}) : {4}",
                                        lignePion.S_NOM,
                                        lignePion.ID_PION,
                                        lignePionDestinationDepart.S_NOM,
                                        lignePionDestinationDepart.ID_PION,
                                        ordre.S_MESSAGE));
                                }
                            }
                            else
                            {
                                message = string.Format("NouvelleHeure : impossible de trouver des alliés à qui envoyer des message en début de partie ID_PION={0}, ID_MODELE={1} ID_NATION:{2}",
                                    lignePion.ID_PION, lignePion.ID_MODELE_PION, ligneModelePion.ID_NATION);
                                LogFile.Notifier(message, out messageErreur);
                                return false;
                            }

                        }
                    }
                     * */
                    #endregion
                    break;

                case Constantes.ORDRES.COMBAT:
                case Constantes.ORDRES.RETRAITE:
                case Constantes.ORDRES.RETRAIT:
                    // si on recherche ligneOrdreCourant après la création de ligneOrdreARemettre, forcement, ligneOrdreCourant
                    // vaut ligneOrdreARemettre
                    Donnees.TAB_ORDRERow ligneOrdreCourantCombat = Donnees.m_donnees.TAB_ORDRE.Courant(lignePion.ID_PION);
                    #region envoi d'un ordre immédiat
                    ligneOrdreARemettre = Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                        Constantes.NULLENTIER,//id_ordre transmis
                        Constantes.NULLENTIER,//id_ordre_suivant,
                        ordre.ID_ORDRE, ///ordre web
                        ordre.I_TYPE,
                        ordre.ID_PION,//ordre.ID_PION,
                        lignePion.ID_CASE,
                        lignePion.effectifTotal,//i_effectif_depart lignePion.I_INFANTERIE + lignePion.I_CAVALERIE +lignePion.I_ARTILLERIE,
                        Constantes.NULLENTIER,//id_case_destination
                        ordre.ID_NOM_LIEU,//ville de destination
                        0,//i_effectif_destination
                        Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,
                        0,
                        Constantes.NULLENTIER,//i_tour_fin
                        Constantes.NULLENTIER,//i_phase_fin
                        Constantes.NULLENTIER,//id_message,
                        Constantes.NULLENTIER,//ID_DESTINATAIRE
                        Constantes.NULLENTIER,//ID_CIBLE
                        Constantes.NULLENTIER,//ID_DESTINATAIRE_CIBLE
                        ordre.ID_BATAILLE,//id_bataille
                        ordre.I_ZONE_BATAILLE,//I_ZONE_BATAILLE
                        ordre.I_HEURE,
                        ordre.I_DUREE,
                        ordre.I_ENGAGEMENT);

                    //cet ordre a un effet immédiat et remplace tout ordre courant.
                    if (null == ligneOrdrePrecedent)
                    {
                        ChangerOrdreCourant(lignePion, ligneOrdreCourantCombat, ligneOrdreARemettre, true);

                        LogFile.Notifier(string.Format("NouveauxOrdres envoie d'un ordre IDWeb={0}, ID={1} de type {2} à {3}({4})",
                                ordre.ID_ORDRE,
                                ligneOrdreARemettre.ID_ORDRE,
                                ordre.I_TYPE,
                                lignePion.S_NOM,
                                lignePion.ID_PION));
                    }
                    #endregion
                    break;

                case Constantes.ORDRES.ENGAGEMENT:
                    //cas très particulier puisque l'ordre n'agit pas sur une unité mais sur l'engagement d'une bataille
                    Donnees.TAB_BATAILLERow ligneBataille = Donnees.m_donnees.TAB_BATAILLE.FindByID_BATAILLE(ordre.ID_BATAILLE);
                    if (null == ligneBataille)
                    {
                        LogFile.Notifier(string.Format("EnvoyerNouvelOrdre, impossible de retrouver la bataille indiquée pour ordre.ID_ORDRE={0} ID_BATAILLE={1}", ordre.ID_ORDRE, ordre.ID_BATAILLE));
                        return false;
                    }
                    ligneBataille["I_ENGAGEMENT_" + Convert.ToString(ordre.I_ZONE_BATAILLE)] = ordre.I_ENGAGEMENT;
                    break;

                case Constantes.ORDRES.MOUVEMENT:
                case Constantes.ORDRES.PATROUILLE:
                    #region envoi d'un ordre de mouvement ou de patrouille ou d'arrêt ou de transfert devant être transmis par messager
                    int idPionOrdre = ordre.ID_PION;
                    compas = ClassMessager.DirectionOrdreVersCompas(ordre.I_DIRECTION);

                    if (!ClassMessager.ZoneGeographiqueVersCase(lignePion, ordre.I_DISTANCE, compas, ordre.ID_NOM_LIEU, out id_case_destination))
                    {
                        ligneOrdreARemettre = DestinationImpossible(ordre, lignePion, id_case_destination);
                    }
                    else
                    {
                        if (lignePion.ID_PION == lignePionDestination.ID_PION)
                        {
                            //cas d'un leader qui se "donne" un ordre
                            // il faut arrêter tout ordre en cours uniquement si cet ordre n'est pas le suivant d'un autre
                            //if (!bOrdreSuivant)
                            {
                                bool bTerminer =true;
                                Donnees.TAB_ORDRERow ligneOrdreCourant = Donnees.m_donnees.TAB_ORDRE.Courant(lignePion.ID_PION);
                                foreach (ClassDataOrdre ordreWeb in liste)
                                {
                                    if (ordreWeb.ID_ORDRE_SUIVANT == ordre.ID_ORDRE) { bTerminer = false; }
                                }
                                if (bTerminer) { lignePion.TerminerOrdre(ligneOrdreCourant, false, true); }
                            }
                            /*if (ordre.I_TYPE == Constantes.ORDRES.MOUVEMENT)
                            {
                                Donnees.TAB_ORDRERow ligneOrdreMouvement = Donnees.m_donnees.TAB_ORDRE.Mouvement(lignePion.ID_PION);
                                if (null != ligneOrdreMouvement)
                                {
                                    ligneOrdreMouvement.I_TOUR_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                                    ligneOrdreMouvement.I_PHASE_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;
                                }
                            }*/

                            ligneOrdreARemettre = Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                                Constantes.NULLENTIER,//id_ordre_transmis
                                Constantes.NULLENTIER,//id_ordre_suivant,//id_ordre_suivant
                                ordre.ID_ORDRE, ///ordre web
                                ordre.I_TYPE,
                                idPionOrdre,//ordre.ID_PION,
                                lignePion.ID_CASE,
                                0,//i_effectif_depart lignePion.I_INFANTERIE + lignePion.I_CAVALERIE +lignePion.I_ARTILLERIE,
                                id_case_destination,
                                ordre.ID_NOM_LIEU,//ville de destination
                                0,//i_effectif_destination
                                Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,
                                0,
                                Constantes.NULLENTIER,//i_tour_fin
                                Constantes.NULLENTIER,//i_phase_fin
                                Constantes.NULLENTIER,//id_message,
                                Constantes.NULLENTIER,//ID_DESTINATAIRE
                                Constantes.NULLENTIER,//ID_CIBLE
                                Constantes.NULLENTIER,//ID_DESTINATAIRE_CIBLE
                                Constantes.NULLENTIER,//id_bataille
                                Constantes.NULLENTIER,//I_ZONE_BATAILLE
                                ordre.I_HEURE,
                                ordre.I_DUREE,
                                ordre.I_ENGAGEMENT);

                            LogFile.Notifier(string.Format("NouveauxOrdres envoie d'un ordre IDWeb={0}, ID={1} de type {2} à {3}({4})",
                                ordre.ID_ORDRE,
                                ligneOrdreARemettre.ID_ORDRE,
                                ordre.I_TYPE,
                                lignePion.S_NOM,
                                lignePion.ID_PION));
                        }
                        else
                        {
                            //ordre qui doit être remis à une unité par un messager
                            ligneOrdreARemettre = Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                                Constantes.NULLENTIER,//id_ordre_transmis
                                Constantes.NULLENTIER,//id_ordre_suivant,//id_ordre_suivant
                                ordre.ID_ORDRE, ///ordre web
                                ordre.I_TYPE,
                                Constantes.NULLENTIER,//ordre.ID_PION,
                                lignePion.ID_CASE,
                                0,//i_effectif_depart lignePion.I_INFANTERIE + lignePion.I_CAVALERIE +lignePion.I_ARTILLERIE,
                                id_case_destination,
                                ordre.ID_NOM_LIEU,
                                0,//i_effectif_destination
                                Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,
                                0,
                                Constantes.NULLENTIER,//i_tour_fin
                                Constantes.NULLENTIER,//i_phase_fin
                                Constantes.NULLENTIER,//id_message,
                                ordre.ID_PION_DESTINATAIRE,
                                ordre.ID_PION_CIBLE,
                                ordre.ID_PION_DESTINATAIRE_CIBLE,
                                Constantes.NULLENTIER,//id_bataille
                                Constantes.NULLENTIER,//I_ZONE_BATAILLE
                                ordre.I_HEURE,
                                ordre.I_DUREE,
                                ordre.I_ENGAGEMENT);
                            if (ordre.ID_PION_CIBLE <0) {ligneOrdreARemettre.SetID_CIBLENull();}
                            if (ordre.ID_PION_DESTINATAIRE_CIBLE < 0) { ligneOrdreARemettre.SetID_DESTINATAIRE_CIBLENull(); };

                            if (null == ligneOrdrePrecedent)
                            {
                                lignePionMessager = ClassMessager.CreerMessager(lignePion);

                                LogFile.Notifier(string.Format("NouveauxOrdres envoie d'un ordre par messager IDWeb={0}, ID={1} de type {2} de {3}({4}) à {5}({6}) transporté par le message id={7}",
                                    ordre.ID_ORDRE,
                                    ligneOrdreARemettre.ID_ORDRE,
                                    ordre.I_TYPE,
                                    lignePion.S_NOM,
                                    lignePion.ID_PION,
                                    lignePionDestination.S_NOM,
                                    lignePionDestination.ID_PION,
                                    lignePionMessager.ID_PION));

                                //et maintenant, un ordre de mouvement
                                compas = ClassMessager.DirectionOrdreVersCompas(ordre.I_DIRECTION_DESTINATAIRE);

                                if (!ClassMessager.ZoneGeographiqueVersCase(lignePionMessager, ordre.I_DISTANCE_DESTINATAIRE, compas, ordre.ID_NOM_LIEU_DESTINATAIRE, out id_case_destination))
                                {
                                    LogFile.Notifier(string.Format("NouvelleHeure, impossible de trouver la case du destinataire pour ordre.ID_ORDRE={0} ID_NOM_LIEU={1} I_DISTANC={2}", ordre.ID_ORDRE, ordre.ID_NOM_LIEU_DESTINATAIRE, ordre.I_DISTANCE_DESTINATAIRE));
                                    return false;
                                }
                                ligneOrdre = Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                                    ligneOrdreARemettre.ID_ORDRE,//id_ordre_transmis
                                    Constantes.NULLENTIER,//id_ordre_suivant
                                    Constantes.NULLENTIER, //ordre web
                                    Constantes.ORDRES.MESSAGE,
                                    lignePionMessager.ID_PION,
                                    lignePionMessager.ID_CASE,//id_case_depart
                                    0,//I_EFFECTIF_DEPART
                                    lignePionDestination.ID_CASE,//id_case_destination, le messager sait où sont toutes les troupes ! -> s'il fallait être parfaitement logique il devrait partir vers la dernière position connue du joueur
                                    Constantes.NULLENTIER,//ville de destination
                                    0,//I_EFFECTIF_DESTINATION
                                    Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,//I_TOUR_DEBUT
                                    0,//I_PHASE_DEBUT
                                    Constantes.NULLENTIER,//I_TOUR_FIN
                                    Constantes.NULLENTIER,//I_PHASE_FIN
                                    Constantes.NULLENTIER,//ID_MESSAGE,
                                    ordre.ID_PION_DESTINATAIRE,//id_destinataire_message
                                    Constantes.NULLENTIER,//ID_CIBLE
                                    Constantes.NULLENTIER,//ID_DESTINATAIRE_CIBLE
                                    Constantes.NULLENTIER,//ID_BATAILLE
                                    Constantes.NULLENTIER,//I_ZONE_BATAILLE
                                    Constantes.NULLENTIER,//I_HEURE_DEBUT
                                    Constantes.NULLENTIER,//I_DUREE
                                    Constantes.NULLENTIER);//I_ENGAGEMENT
                            }
                        }
                    }
                    #endregion
                    break;

                case Constantes.ORDRES.LIGNE_RAVITAILLEMENT:
                    #region envoi d'un ordre de génération d'un convoir de ravitaillement tous les jours vers un endroit donné
                    compas = ClassMessager.DirectionOrdreVersCompas(ordre.I_DIRECTION);

                    if (!ClassMessager.ZoneGeographiqueVersCase(lignePion, ordre.I_DISTANCE, compas, ordre.ID_NOM_LIEU, out id_case_destination))
                    {
                        ligneOrdreARemettre = DestinationImpossible(ordre, lignePion, id_case_destination);
                    }
                    else
                    {
                        //ordre qui doit être remis à une unité par un messager
                        ligneOrdreARemettre = Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                            Constantes.NULLENTIER,//id_ordre_transmis
                            Constantes.NULLENTIER,//id_ordre_suivant,//id_ordre_suivant
                            ordre.ID_ORDRE, ///ordre web
                            ordre.I_TYPE,
                            Constantes.NULLENTIER,//ordre.ID_PION,
                            lignePion.ID_CASE,
                            0,//i_effectif_depart lignePion.I_INFANTERIE + lignePion.I_CAVALERIE +lignePion.I_ARTILLERIE,
                            id_case_destination,
                            ordre.ID_NOM_LIEU,
                            0,//i_effectif_destination
                            Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,
                            0,
                            Constantes.NULLENTIER,//i_tour_fin
                            Constantes.NULLENTIER,//i_phase_fin
                            Constantes.NULLENTIER,//id_message,
                            ordre.ID_PION_DESTINATAIRE,
                            ordre.ID_PION_CIBLE,
                            ordre.ID_PION_DESTINATAIRE_CIBLE,
                            Constantes.NULLENTIER,//id_bataille
                            Constantes.NULLENTIER,//I_ZONE_BATAILLE
                            ordre.I_HEURE,
                            ordre.I_DUREE,
                            ordre.I_ENGAGEMENT);
                        if (ordre.ID_PION_CIBLE < 0) { ligneOrdreARemettre.SetID_CIBLENull(); }
                        if (ordre.ID_PION_DESTINATAIRE_CIBLE < 0) { ligneOrdreARemettre.SetID_DESTINATAIRE_CIBLENull(); };

                        if (null == ligneOrdrePrecedent)
                        {
                            lignePionMessager = ClassMessager.CreerMessager(lignePion);

                            LogFile.Notifier(string.Format("NouveauxOrdres envoie d'un ordre par messager IDWeb={0}, ID={1} de type {2} de {3}({4}) à {5}({6}) transporté par le message id={7}",
                                ordre.ID_ORDRE,
                                ligneOrdreARemettre.ID_ORDRE,
                                ordre.I_TYPE,
                                lignePion.S_NOM,
                                lignePion.ID_PION,
                                lignePionDestination.S_NOM,
                                lignePionDestination.ID_PION,
                                lignePionMessager.ID_PION));

                            //et maintenant, un ordre de mouvement
                            compas = ClassMessager.DirectionOrdreVersCompas(ordre.I_DIRECTION_DESTINATAIRE);

                            if (!ClassMessager.ZoneGeographiqueVersCase(lignePionMessager, ordre.I_DISTANCE_DESTINATAIRE, compas, ordre.ID_NOM_LIEU_DESTINATAIRE, out id_case_destination))
                            {
                                LogFile.Notifier(string.Format("NouvelleHeure, impossible de trouver la case du destinataire pour ordre.ID_ORDRE={0} ID_NOM_LIEU={1} I_DISTANC={2}", ordre.ID_ORDRE, ordre.ID_NOM_LIEU_DESTINATAIRE, ordre.I_DISTANCE_DESTINATAIRE));
                                return false;
                            }
                            ligneOrdre = Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                                ligneOrdreARemettre.ID_ORDRE,//id_ordre_transmis
                                Constantes.NULLENTIER,//id_ordre_suivant
                                Constantes.NULLENTIER, //ordre web
                                Constantes.ORDRES.MESSAGE,
                                lignePionMessager.ID_PION,
                                lignePionMessager.ID_CASE,//id_case_depart
                                0,//I_EFFECTIF_DEPART
                                lignePionDestination.ID_CASE,//id_case_destination, le messager sait où sont toutes les troupes ! -> s'il fallait être parfaitement logique il devrait partir vers la dernière position connue du joueur
                                Constantes.NULLENTIER,//ville de destination
                                0,//I_EFFECTIF_DESTINATION
                                Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,//I_TOUR_DEBUT
                                0,//I_PHASE_DEBUT
                                Constantes.NULLENTIER,//I_TOUR_FIN
                                Constantes.NULLENTIER,//I_PHASE_FIN
                                Constantes.NULLENTIER,//ID_MESSAGE,
                                ordre.ID_PION_DESTINATAIRE,//id_destinataire_message
                                Constantes.NULLENTIER,//ID_CIBLE
                                Constantes.NULLENTIER,//ID_DESTINATAIRE_CIBLE
                                Constantes.NULLENTIER,//ID_BATAILLE
                                Constantes.NULLENTIER,//I_ZONE_BATAILLE
                                Constantes.NULLENTIER,//I_HEURE_DEBUT
                                Constantes.NULLENTIER,//I_DUREE
                                Constantes.NULLENTIER);//I_ENGAGEMENT
                        }                        
                    }
                    #endregion
                    break;

                case Constantes.ORDRES.ENDOMMAGER_PONT:
                case Constantes.ORDRES.REPARER_PONT:
                case Constantes.ORDRES.CONSTRUIRE_PONTON:
                case Constantes.ORDRES.GENERERCONVOI:
                case Constantes.ORDRES.RENFORCER:
                case Constantes.ORDRES.SEFORTIFIER:
                case Constantes.ORDRES.ETABLIRDEPOT:
                case Constantes.ORDRES.TRANSFERER:
                case Constantes.ORDRES.ARRET:
                    #region envoi d'un ordre à action immédiate sur site, devant être transmis par messager
                    //ordre qui doit être remis à une unité par un messager
                    //int idPionOrdre = (ordre.I_TYPE == Constantes.ORDRES.TRANSFERER) ? ordre.ID_PION_CIBLE : ordre.ID_PION;
                    ligneOrdreARemettre = Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                        Constantes.NULLENTIER,//id_ordre_suivant,//id_ordre_transmis
                        Constantes.NULLENTIER,//id_ordre_suivant
                        ordre.ID_ORDRE, ///ordre web
                        ordre.I_TYPE,
                        Constantes.NULLENTIER,//ordre.ID_PION,
                        lignePion.ID_CASE,
                        0,//i_effectif_depart lignePion.I_INFANTERIE + lignePion.I_CAVALERIE +lignePion.I_ARTILLERIE,
                        lignePion.ID_CASE,//ID_CASE_DESTINATION, parfois utilisé dans certains messages
                        ordre.ID_NOM_LIEU,
                        0,//i_effectif_destination
                        Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,
                        0,
                        Constantes.NULLENTIER,//i_tour_fin
                        Constantes.NULLENTIER,//i_phase_fin
                        Constantes.NULLENTIER,//id_message,
                        ordre.ID_PION_DESTINATAIRE,
                        ordre.ID_PION_CIBLE,
                        ordre.ID_PION_DESTINATAIRE_CIBLE,
                        Constantes.NULLENTIER,//id_bataille
                        Constantes.NULLENTIER,//I_ZONE_BATAILLE
                        ordre.I_HEURE,
                        ordre.I_DUREE,
                        Constantes.NULLENTIER//I_ENGAGEMENT
                        );
                    if (ordre.ID_PION_CIBLE <0) {ligneOrdreARemettre.SetID_CIBLENull();}
                    if (ordre.ID_PION_DESTINATAIRE_CIBLE<0) {ligneOrdreARemettre.SetID_DESTINATAIRE_CIBLENull();};

                    if (null == ligneOrdrePrecedent)
                    {
                        lignePionMessager = ClassMessager.CreerMessager(lignePion);

                        LogFile.Notifier(string.Format("NouveauxOrdres envoie d'un ordre par messager IDWeb={0}, ID={1} de type {2}:{8} de {3}({4}) à {5}({6}) transporté par le message id={7}",
                            ordre.ID_ORDRE,
                            ligneOrdreARemettre.ID_ORDRE,
                            ordre.I_TYPE,
                            lignePion.S_NOM,
                            lignePion.ID_PION,
                            lignePionDestination.S_NOM,
                            lignePionDestination.ID_PION,
                            lignePionMessager.ID_PION,
                            ordre.I_TYPE.ToString()));

                        //et maintenant, un ordre de mouvement
                        id_case_destination = lignePion.ID_CASE; //quand on donne un ordre à une unité, le messager sait où elle est
                        /*
                        compas = ClassMessager.DirectionOrdreVersCompas(ordre.I_DIRECTION_DESTINATAIRE);

                        if (!ClassMessager.ZoneGeographiqueVersCase(lignePionMessager, ordre.I_DISTANCE_DESTINATAIRE, compas, ordre.ID_NOM_LIEU_DESTINATAIRE, out id_case_destination))
                        {
                            LogFile.Notifier(string.Format("NouvelleHeure, impossible de trouver la case du destinataire pour ordre.ID_ORDRE={0} ID_NOM_LIEU={1} I_DISTANC={2}", ordre.ID_ORDRE, ordre.ID_NOM_LIEU_DESTINATAIRE, ordre.I_DISTANCE_DESTINATAIRE));
                            return false;
                        }*/

                        ligneOrdre = Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                            ligneOrdreARemettre.ID_ORDRE,//id_ordre_transmis
                            Constantes.NULLENTIER,//id_ordre_suivant
                            Constantes.NULLENTIER, //ordre web
                            Constantes.ORDRES.MESSAGE,
                            lignePionMessager.ID_PION,
                            lignePionMessager.ID_CASE,//id_case_depart
                            0,//I_EFFECTIF_DEPART
                            lignePionDestination.ID_CASE,//id_case_destination, le messager sait où sont toutes les troupes ! -> s'il fallait être parfaitement logique il devrait partir vers la dernière position connue du joueur
                            Constantes.NULLENTIER,//ville de destination
                            0,//I_EFFECTIF_DESTINATION
                            Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,//I_TOUR_DEBUT
                            0,//I_PHASE_DEBUT
                            Constantes.NULLENTIER,//I_TOUR_FIN
                            Constantes.NULLENTIER,//I_PHASE_FIN
                            Constantes.NULLENTIER,//ID_MESSAGE,
                            ordre.ID_PION_DESTINATAIRE,//id_destinataire_message
                            Constantes.NULLENTIER,//ID_CIBLE
                            Constantes.NULLENTIER,//ID_DESTINATAIRE_CIBLE
                            Constantes.NULLENTIER,//ID_BATAILLE
                            Constantes.NULLENTIER,//I_ZONE_BATAILLE
                            Constantes.NULLENTIER,//I_HEURE_DEBUT
                            Constantes.NULLENTIER,//I_DUREE
                            Constantes.NULLENTIER//I_ENGAGEMENT
                            );
                    }
                    #endregion
                    break;

                case Constantes.ORDRES.MESSAGE_FORUM:
                    //on ne fait rien, ordre déjà traité directement dans l'interface web -> en fait, c'est pas tout a fait vrai, il faut ajouter le message dans la table correspondante
                    //pour que le joueur recoive au moins le message au moment du courriel de fin de tour
                    //en fait, c'est inutile, déjà géré dans NouveauxMessages()
                    //Donnees.m_donnees.TAB_MESSAGE.AddTAB_MESSAGERow(...
                    LogFile.Notifier("NouveauxOrdres reception d'un message forum ID=" + ordre.ID_ORDRE);
                    break;
                default:
                    //on ne devrait jamais se trouver là
                    LogFile.Notifier("NouveauxOrdres reception d'un type d'ordre inconne ID=" + ordre.I_TYPE);
                    return false;
            }
            //if (null != ligneOrdreARemettre)
            //{
            //    id_ordre_suivant = ligneOrdreARemettre.ID_ORDRE;
            //}

            //si cet ordre est le suivant d'un ordre précédent, on le met à jour
            if (null != ligneOrdrePrecedent)
            {
                ligneOrdrePrecedent.ID_ORDRE_SUIVANT = ligneOrdreARemettre.ID_ORDRE;
            }

            return true;
        }

        private Donnees.TAB_ORDRERow DestinationImpossible(ClassDataOrdre ordre, Donnees.TAB_PIONRow lignePion, int id_case_destination)
        {
            Donnees.TAB_ORDRERow ligneOrdreARemettre;
            //Cela indique qu'il est impossible de se rendre à l'emplacement indiqué
            //il faut quand même ajouter un ordre terminé cela peut servir dans le message
            Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot); 
            ligneOrdreARemettre = Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                Constantes.NULLENTIER,//id_ordre_transmis
                Constantes.NULLENTIER,//id_ordre_suivant,//id_ordre_suivant
                ordre.ID_ORDRE, ///ordre web
                ordre.I_TYPE,
                ordre.ID_PION,
                lignePion.ID_CASE,
                0,//i_effectif_depart lignePion.I_INFANTERIE + lignePion.I_CAVALERIE +lignePion.I_ARTILLERIE,
                id_case_destination,
                ordre.ID_NOM_LIEU,//ville de destination
                0,//i_effectif_destination
                Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,
                0,
                Constantes.NULLENTIER,//i_tour_fin
                Constantes.NULLENTIER,//i_phase_fin
                Constantes.NULLENTIER,//id_message,
                Constantes.NULLENTIER,//ID_DESTINATAIRE
                ordre.ID_PION_CIBLE,//ID_CIBLE
                ordre.ID_PION_DESTINATAIRE_CIBLE,//ID_DESTINATAIRE_CIBLE
                Constantes.NULLENTIER,//id_bataille
                Constantes.NULLENTIER,//I_ZONE_BATAILLE
                ordre.I_HEURE,
                ordre.I_DUREE,
                Constantes.NULLENTIER);//I_ENGAGEMENT
            if (ordre.ID_PION_CIBLE < 0) { ligneOrdreARemettre.SetID_CIBLENull(); }
            if (ordre.ID_PION_DESTINATAIRE_CIBLE < 0) { ligneOrdreARemettre.SetID_DESTINATAIRE_CIBLENull(); }
            Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot); 

            // Il faut envoyer un message pour prévenir l'officier responsable
            ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_DESTINATION_IMPOSSIBLE);
            LogFile.Notifier(string.Format("NouvelleHeure, case de destination impossible pour ordre.ID_ORDRE={0} ID_NOM_LIEU={1} I_DISTANC={2}", ordre.ID_ORDRE, ordre.ID_NOM_LIEU, ordre.I_DISTANCE));
            return ligneOrdreARemettre;
        }

        /// <summary>
        /// Début d'un nouveau jour
        /// </summary>
        /// <returns>true si ok, false si ko</returns>
        private bool NouveauJour()
        {
            #region tirage de la météo du jour
            int totalPourcentage=0;
            foreach (Donnees.TAB_METEORow ligneMeteo in Donnees.m_donnees.TAB_METEO)
            {
                totalPourcentage += ligneMeteo.I_CHANCE;
            }
            Random de = new Random();
            int pourcentResultatMeteo = de.Next(totalPourcentage);
            totalPourcentage=0;
            int i = 0;
            while (i < Donnees.m_donnees.TAB_METEO.Count && totalPourcentage <= pourcentResultatMeteo)
            {
                totalPourcentage += Donnees.m_donnees.TAB_METEO[i++].I_CHANCE;
            }
            //Si la nouvelle météo est la même que la précédente ou que l'aggravé, on augemente le nombre de même météo successive
            if (Donnees.m_donnees.TAB_PARTIE[0].ID_METEO == Donnees.m_donnees.TAB_METEO[i - 1].ID_METEO || Donnees.m_donnees.TAB_PARTIE[0].ID_METEO == Donnees.m_donnees.TAB_METEO[i - 1].ID_METEO_AGGRAVATION)
            {
                Donnees.m_donnees.TAB_PARTIE[0].I_NB_METEO_SUCCESSIVE++;
            }
            else
            {
                Donnees.m_donnees.TAB_PARTIE[0].I_NB_METEO_SUCCESSIVE = 0;//on change de météo
            }
            //météo aggravée ?
            if (Donnees.m_donnees.TAB_METEO[i - 1].I_NB_TOURS_AGGRAVATION > 0 && Donnees.m_donnees.TAB_METEO[i - 1].I_NB_TOURS_AGGRAVATION >= Donnees.m_donnees.TAB_PARTIE[0].I_NB_METEO_SUCCESSIVE)
            {
                Donnees.m_donnees.TAB_PARTIE[0].ID_METEO = Donnees.m_donnees.TAB_METEO[i - 1].ID_METEO_AGGRAVATION;
            }
            else
            {
                Donnees.m_donnees.TAB_PARTIE[0].ID_METEO = Donnees.m_donnees.TAB_METEO[i - 1].ID_METEO;
            }
            
            LogFile.Notifier(string.Format("Nouvelle meteo id={0} ({1})", Donnees.m_donnees.TAB_METEO[i - 1].ID_METEO, Donnees.m_donnees.TAB_METEO[i - 1].S_NOM));
            #endregion

            //initialisation des unités -> déjà fait dans FinDuJour/FatigueEtRepos mais au cas où...
            for (int l = 0; l < Donnees.m_donnees.TAB_PION.Count; l++)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[l];
                lignePion.I_NB_PHASES_MARCHE_JOUR = 0;
                lignePion.I_NB_PHASES_MARCHE_NUIT = 0;
                lignePion.I_NB_HEURES_COMBAT = 0;
                lignePion.I_NB_HEURES_FORTIFICATION = 0;
            }
            return true;
        }

        /// <summary>
        /// Fin d'une journée
        /// </summary>
        /// <returns>true si ok, false si ko</returns>
        private bool FinDuJour()
        {
            //Ravitaillement de toutes les unités
            if (!Ravitaillement()) {LogFile.Notifier("Erreur grave dans FinDuJour:Ravitaillement"); return false;}

            if (!RapportQuotidienHorsRavitaillement()) { LogFile.Notifier("Erreur grave dans FinDuJour:RapportQuotidienHorsRavitaillement"); return false; }

            //On vérifie si le dêpôt n'est pas réduit suite au ravitaillement de très nombreuses unités
            if (!ReductionDesDepots()) { LogFile.Notifier("Erreur grave dans FinDuJour:ReductionDesDepots"); return false; }

            //fatigue/repos de toutes les unites
            if (!FatigueEtRepos()) {LogFile.Notifier("Erreur grave dans FinDuJour:FatigueEtRepos"); return false;}

            //Les hopitaux "libèrent" les blessés soignés toutes les semaines, en se basant sur la date d'arrivée de l'unité
            if (!SoinsAuxBlesses()) { LogFile.Notifier("Erreur grave dans FinDuJour:SoinsAuxBlesses"); return false; }

            //On envoie un rapport de situation sur les prisons
            if (!RapportDesPrisons()) { LogFile.Notifier("Erreur grave dans FinDuJour:RapportDesPrisons"); return false; }

            return true;
        }

        private bool RapportQuotidienHorsRavitaillement()
        {
            int i=0;

            while (i < Donnees.m_donnees.TAB_PION.Count)//on ne peut pas faire de foreach car l'envoi de messagers change la table
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                if (!lignePion.estRapportQuotidienHorsRavitaillement) { i++; continue; }
                //on envoit un message permettant au joueur d'avoir une indication de la position au moins une fois par jour
                if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_POSITION))
                {
                    string message = string.Format("{0}(ID={1}, erreur sur EnvoyerMessage avec MESSAGE_POSITION dans RapportQuotidienHorsRavitaillement", lignePion.S_NOM, lignePion.ID_PION);
                    LogFile.Notifier(message);
                    return false;
                }
                i++;
            }
            return true;
        }

        private bool SoinsAuxBlesses()
        {
            foreach (Donnees.TAB_NOMS_CARTERow ligneNomCarte in Donnees.m_donnees.TAB_NOMS_CARTE)
            {
                if (ligneNomCarte.B_HOPITAL  && !ligneNomCarte.IsID_NATION_CONTROLENull())
                {
                    int iBlessesInfanterie = 0;
                    int iBlessesCavalerie = 0;
                    int iBlessesArtillerie = 0;

                    //Le nombre de blessés soignés par unité est égale à (50% + La valeur de I_GUERISON dans TAB_NATION + d3 avec (1: -10%, 2: 0, 3: +10)/4 
                    int pourcentGuerison = 50 + ligneNomCarte.nation.I_GUERISON + (1 - (Constantes.JetDeDes(1) - 1) / 2) * 10;
                    LogFile.Notifier("SoinsAuxBlesses : soins aux blessés sur l'hopital " + ligneNomCarte.S_NOM + " avec un pourcentage de " + pourcentGuerison + " %");

                    //recherche de toutes les unités rattachés à ce nom
                    string requete = string.Format("B_BLESSES=true AND ID_LIEU_RATTACHEMENT={0}", ligneNomCarte.ID_NOM);
                    Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    Donnees.TAB_PIONRow[] lignesPionResultat = (Donnees.TAB_PIONRow[])Donnees.m_donnees.TAB_PION.Select(requete);
                    for (int l = 0; l < lignesPionResultat.Count(); l++)
                    {
                        Donnees.TAB_PIONRow lignePion = lignesPionResultat[l];
                        if ((ligneNomCarte.ID_NATION_CONTROLE == lignePion.nation.ID_NATION) && (lignePion.effectifTotal > 0))
                        {
                            //cela fait-il une semaine que l'unité a rejoint l'hopital
                            int nbToursHopital = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR - lignePion.I_TOUR_BLESSURE;
                            //if (true) //BEA pour test
                            if (nbToursHopital > 0 && (0 == nbToursHopital % (24 * 7)))
                            {
                                int iInfanterieRenfort = 0, iCavalerieRenfort = 0;
                                iInfanterieRenfort = lignePion.I_INFANTERIE * pourcentGuerison / 100;
                                iCavalerieRenfort = lignePion.I_CAVALERIE * pourcentGuerison / 100;
                                if (iInfanterieRenfort + iCavalerieRenfort <= Constantes.CST_TAILLE_MINIMUM_UNITE)
                                {
                                    //Toute l'unité est soignée d'un coup (pour éviter d'avoir des unités de très peu de soldats à gérer)
                                    iInfanterieRenfort = lignePion.I_INFANTERIE;
                                    iCavalerieRenfort = lignePion.I_CAVALERIE;
                                }
                                Donnees.TAB_NATIONRow ligneNation = lignePion.nation;
                                Donnees.TAB_PIONRow lignePionCommandantEnChef = ligneNation.commandantEnChef;
                                Donnees.TAB_PIONRow lignePionRenfort = lignePion.CreerConvoi(lignePionCommandantEnChef, false /*bBlesses*/, false /*bPrisonniers*/, true /*bRenfort*/);
                                if (null == lignePionRenfort)
                                {
                                    LogFile.Notifier(string.Format("SoinsAuxBlesses : impossible de créer un convoi de blessés guéris à partir de {0}:{1}", lignePion.S_NOM, lignePion.ID_PION));
                                }
                                lignePionRenfort.I_INFANTERIE = iInfanterieRenfort;
                                lignePionRenfort.I_CAVALERIE = iCavalerieRenfort;
                                lignePionRenfort.I_FATIGUE = 0;
                                lignePionRenfort.I_RAVITAILLEMENT = 50;//on est quand même pas super bien équipés quand on quitte l'hopital

                                //on retire les soignés des blessés
                                lignePion.I_INFANTERIE -= iInfanterieRenfort;
                                lignePion.I_CAVALERIE -= iCavalerieRenfort;

                                //on envoi un message pour prévenir que les renforts sont disponibles
                                if (!ClassMessager.EnvoyerMessage(lignePionRenfort, ClassMessager.MESSAGES.MESSAGE_BLESSES_SOIGNES))
                                {
                                    string message = string.Format("{0}(ID={1}, erreur sur EnvoyerMessage avec MESSAGE_BLESSES_SOIGNES dans SoinsAuxBlesses", lignePionRenfort.S_NOM, lignePionRenfort.ID_PION);
                                    LogFile.Notifier(message);
                                    return false;
                                }
                            }
                            // pour le rapport d'effectifs
                            iBlessesInfanterie += lignePion.I_INFANTERIE;
                            iBlessesCavalerie += lignePion.I_CAVALERIE;
                            iBlessesArtillerie += lignePion.I_ARTILLERIE;
                        }
                    }
                    //création d'un pion fictif ayant pour effectif, les effectifs de l'hopital pour envoyer un rapport de situation.
                    //uniquement s'il y a des blessés
                    if (iBlessesInfanterie + iBlessesCavalerie + iBlessesArtillerie > 0)
                    {
                        IEnumerable<Donnees.TAB_PIONRow> listeLeaders = Donnees.m_donnees.TAB_NATION.CommandantEnChef(ligneNomCarte.ID_NATION_CONTROLE);
                        int i = 0;
                        while (i < listeLeaders.Count())
                        {
                            Donnees.TAB_PIONRow lignePionLeader = listeLeaders.ElementAt(i);
                            //Donnees.TAB_PIONRow lignePionRapport = ClassMessager.CreerMessager(lignePionLeader); -> il ne faut pas créer un messager sinon on a un mauvais emetteur dans les messages sur le web
                            Donnees.TAB_PIONRow lignePionRapport = lignePionLeader.CreerConvoi(lignePionLeader, false, false, false);
                            lignePionRapport.S_NOM = "Hôpital de " + ligneNomCarte.S_NOM;
                            lignePionRapport.ID_CASE = ligneNomCarte.ID_CASE;
                            // pour le rapport d'effectifs
                            lignePionRapport.I_INFANTERIE = iBlessesInfanterie;
                            lignePionRapport.I_CAVALERIE = iBlessesCavalerie;
                            lignePionRapport.I_ARTILLERIE = iBlessesArtillerie;

                            //obligé de l'envoyé en immédiat, sinon le pion hopital apparait dans la liste des unités !
                            lignePionRapport.DetruirePion();
                            if (!ClassMessager.EnvoyerMessageImmediat(lignePionRapport, ClassMessager.MESSAGES.MESSAGE_RAPPORT_HOPITAL))
                            {
                                LogFile.Notifier("SoinsAuxBlesses : erreur lors de l'envoi d'un message MESSAGE_RAPPORT_HOPITAL");
                                Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                                return false;
                            }
                            i++;
                        }
                    }
                    Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                }
            }
            return true;
        }

        private bool RapportDesPrisons()
        {
            foreach (Donnees.TAB_NOMS_CARTERow ligneNomCarte in Donnees.m_donnees.TAB_NOMS_CARTE)
            {
                if (ligneNomCarte.B_PRISON && !ligneNomCarte.IsID_NATION_CONTROLENull())
                {
                    int iPrisonniersInfanterie = 0;
                    int iPrisonniersCavalerie = 0;
                    int iPrisonniersArtillerie = 0;

                    //recherche de toutes les unités rattachés à ce nom
                    Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    string requete = string.Format("B_PRISONNIERS=true AND ID_LIEU_RATTACHEMENT={0}", ligneNomCarte.ID_NOM);
                    Donnees.TAB_PIONRow[] lignesPionResultat = (Donnees.TAB_PIONRow[])Donnees.m_donnees.TAB_PION.Select(requete);
                    for (int l = 0; l < lignesPionResultat.Count(); l++)
                    {
                        Donnees.TAB_PIONRow lignePion = lignesPionResultat[l];
                        if ((ligneNomCarte.ID_NATION_CONTROLE == lignePion.nation.ID_NATION) && (lignePion.effectifTotal > 0))
                        {
                            // pour le rapport d'effectifs
                            iPrisonniersInfanterie += lignePion.I_INFANTERIE;
                            iPrisonniersCavalerie += lignePion.I_CAVALERIE;
                            iPrisonniersArtillerie += lignePion.I_ARTILLERIE;
                        }
                    }

                    //création d'un pion fictif ayant pour effectif, les effectifs de la prison pour envoyer un rapport de situation.
                    if (iPrisonniersInfanterie + iPrisonniersCavalerie + iPrisonniersArtillerie > 0)
                    {
                        IEnumerable<Donnees.TAB_PIONRow> listeLeaders = Donnees.m_donnees.TAB_NATION.CommandantEnChef(ligneNomCarte.ID_NATION_CONTROLE);
                        int i = 0;
                        while (i<listeLeaders.Count())
                        {
                            Donnees.TAB_PIONRow lignePionLeader = listeLeaders.ElementAt(i);
                            //Donnees.TAB_PIONRow lignePionRapport = ClassMessager.CreerMessager(lignePionLeader); -> il ne faut pas créer un messager sinon on a un mauvais emetteur dans les messages sur le web
                            Donnees.TAB_PIONRow lignePionRapport = lignePionLeader.CreerConvoi(lignePionLeader, false, false, false);
                            lignePionRapport.S_NOM = "Prison de " + ligneNomCarte.S_NOM;
                            lignePionRapport.ID_CASE = ligneNomCarte.ID_CASE;
                            lignePionRapport.I_INFANTERIE = iPrisonniersInfanterie;
                            lignePionRapport.I_CAVALERIE = iPrisonniersCavalerie;
                            lignePionRapport.I_ARTILLERIE = iPrisonniersArtillerie;

                            //obligé de l'envoyé en immédiat, sinon le pion prison apparait dans la liste des unités !
                            lignePionRapport.DetruirePion();
                            if (!ClassMessager.EnvoyerMessageImmediat(lignePionRapport, ClassMessager.MESSAGES.MESSAGE_RAPPORT_PRISON))
                            {
                                LogFile.Notifier("RapportDesPrisons : erreur lors de l'envoi d'un message MESSAGE_RAPPORT_PRISON");
                                Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                                return false;
                            }
                            i++;
                        }
                    }
                    Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                }
            }
            return true;
        }

        private bool ReductionDesDepots()
        {
            //string message;
            int i;
            int ligneDepotTable;

            i = 0;
            while (i < Donnees.m_donnees.TAB_PION.Count)//on ne peut pas faire de foreach car l'envoi de messagers change la table
            {
                Donnees.TAB_PIONRow ligneDepot = Donnees.m_donnees.TAB_PION[i];
                if (!ligneDepot.estDepot) { ++i; continue; } //seules les dépôts sont concernés

                ligneDepotTable = ligneDepot.C_NIVEAU_DEPOT - 'A';

                //Un depôt de niveau A n'a t-il fait aucun ravitaillement direct depuis une semaine
                if (/*'A' == ligneDepot.C_NIVEAU_DEPOT && */
                    ((ligneDepot.IsI_TOUR_DERNIER_RAVITAILLEMENT_DIRECTNull() && 24*7 < Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)
                    || (ligneDepot.I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT + 24*7 < Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)))
                {
                    // les depôt de niveau A ne sont pas réduits,mais il faut une semaine entre le dernier ravitaillement
                    // pour revenir au niveau 0
                    ligneDepot.I_SOLDATS_RAVITAILLES = 0;
                }

                //Le dépôt est-il épuisé ?
                /* -> trop sévère et incontrollable, je traite tout comme pour les dépots de type 'A'
                if (ligneDepot.I_SOLDATS_RAVITAILLES >= Constantes.tableLimiteRavitaillementDepot[ligneDepotTable])
                {
                    //le dépôt perd un niveau, s'il est de type 'D' il est détruit
                    if ('D' == ligneDepot.C_NIVEAU_DEPOT)
                    {
                        ligneDepot.DetruirePion();
                        //on envoie un message pour prévenir le joueur
                        if (!ClassMessager.EnvoyerMessage(ligneDepot, ClassMessager.MESSAGES.MESSAGE_DEPOT_REDUIT))
                        {
                            message = string.Format("{0}(ID={1}, erreur sur EnvoyerMessage avec MESSAGE_DEPOT_REDUIT dans ReductionDesDepots", ligneDepot.S_NOM, ligneDepot.ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }
                    }
                    else
                    {
                        if ('A' != ligneDepot.C_NIVEAU_DEPOT)
                        {
                            // les depôt de niveau A ne sont pas réduits,mais il faut une semaine entre le dernier ravitaillement
                            // pour revenir au niveau 0
                            ligneDepot.C_NIVEAU_DEPOT++;//on réduit mais dans la table ASCII c'est le caractère suivant !
                            ligneDepot.I_SOLDATS_RAVITAILLES = 0;

                            //on envoie un message pour prévenir le joueur
                            if (!ClassMessager.EnvoyerMessage(ligneDepot, ClassMessager.MESSAGES.MESSAGE_DEPOT_REDUIT))
                            {
                                message = string.Format("{0}(ID={1}, erreur sur EnvoyerMessage avec MESSAGE_DEPOT_REDUIT dans ReductionDesDepots", ligneDepot.S_NOM, ligneDepot.ID_PION);
                                LogFile.Notifier(message);
                                return false;
                            }
                        }
                    }
                }
                */
                ++i;
            }
            return true;
        }

        /// <summary>
        /// Calcul de la fatigue/moral en fin de journée
        /// </summary>
        /// <returns>true si ok, false si ko</returns>
        private bool FatigueEtRepos()
        {
            string messageErreur, message;
            string requete;
            int moral, diffmoral;
            int fatigue, diffatigue;
            int i;
            Donnees.TAB_METEORow ligneMeteo = Donnees.m_donnees.TAB_METEO.FindByID_METEO(Donnees.m_donnees.TAB_PARTIE[0].ID_METEO);

            i = 0;
            while (i < Donnees.m_donnees.TAB_PION.Count)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                //calcul de la fatigue et du moral
                if (!lignePion.B_DETRUIT && lignePion.effectifTotal > 0
                    && !lignePion.estDepot && !lignePion.estBlesses
                    && !lignePion.estConvoiDeRavitaillement && !lignePion.estQG && !lignePion.estMessager
                    && !lignePion.estPatrouille && !lignePion.estPontonnier && !lignePion.estPrisonniers)
                {
                    //est-ce que l'unité a fait un combat durant cette journée ?
                    requete = string.Format("ID_PION={0} AND B_ENGAGEE=True", lignePion.ID_PION);
                    Donnees.TAB_BATAILLE_PIONSRow[] resBataillePions = (Donnees.TAB_BATAILLE_PIONSRow[])Donnees.m_donnees.TAB_BATAILLE_PIONS.Select(requete);

                    //est-ce que l'unité a fait une activité ce jour ?
                    if (!lignePion.reposComplet)
                    {
                        if ((lignePion.I_NB_HEURES_COMBAT > 0) || (lignePion.I_NB_PHASES_MARCHE_JOUR > 0) || (lignePion.I_NB_PHASES_MARCHE_NUIT > 0))
                        {
                            int nbHeuresJour = (int)Math.Ceiling((decimal)lignePion.I_NB_PHASES_MARCHE_JOUR / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES);
                            int nbHeuresNuit = (int)Math.Ceiling((decimal)lignePion.I_NB_PHASES_MARCHE_NUIT / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES);

                            //bHeuresJour + nbHeuresNuit peut être supérieur à 24 à cause des arrondis, d'où le Min(24,... ci-dessous)
                            int colonneFatigue = Math.Max(0, Math.Min(24,nbHeuresJour + nbHeuresNuit) - (int)Math.Floor(lignePion.I_EXPERIENCE));
                            fatigue = (lignePion.I_CAVALERIE > 0) ? Constantes.tableFatigueCavalerie[colonneFatigue] : Constantes.tableFatigueInfanterie[colonneFatigue];
                            fatigue += nbHeuresNuit;
                            fatigue += lignePion.I_NB_HEURES_COMBAT * 2;//combat toutes les deux heures
                                                                        //fatigue += lignePion.I_NB_HEURES_FORTIFICATION; ->jamais mis à jour de toute manière
                                                                        //modification d'après la météo courante
                            fatigue = (int)((decimal)ligneMeteo.I_POURCENT_FATIGUE * fatigue / (decimal)100);
                        }
                        else
                        {
                            fatigue = 0;//cas d'une unité entrain de se fortifier, elle n'est pas en repose mais elle ne se fatigue pas
                        }

                        moral = lignePion.I_MORAL; //pas de modification permanente du moral, le moral effectif avec la fatigue doit être calculée sur l'instant, donc diffmoral vaut toujours zéro !
                        fatigue = Math.Min(lignePion.I_FATIGUE + fatigue, 100);
                        diffmoral = moral - lignePion.I_MORAL;
                        diffatigue = fatigue - lignePion.I_FATIGUE;
                        lignePion.I_MORAL = moral;
                        lignePion.I_FATIGUE = fatigue;
                        //Si un ordre est en cours il faut modifier potentiellement l'effectif de depart
                        Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(lignePion.ID_PION);
                        if (null != ligneOrdre)
                        {
                            if (ligneOrdre.I_EFFECTIF_DEPART > lignePion.effectifTotalEnMouvement)
                            {
                                ligneOrdre.I_EFFECTIF_DEPART = lignePion.effectifTotalEnMouvement;
                            }
                            if (ligneOrdre.I_EFFECTIF_DESTINATION > lignePion.effectifTotalEnMouvement)
                            {
                                ligneOrdre.I_EFFECTIF_DESTINATION = lignePion.effectifTotalEnMouvement;
                            }
                        }

                        //affection à zéro avant l'envoi des messages, sinon, cela trouble les joueurs
                        lignePion.I_NB_PHASES_MARCHE_JOUR = 0;
                        lignePion.I_NB_PHASES_MARCHE_NUIT = 0;
                        lignePion.I_NB_HEURES_COMBAT = 0;
                        lignePion.I_NB_HEURES_FORTIFICATION = 0;
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_BILAN_ACTION, diffmoral, diffatigue))
                        {
                            message = string.Format("FinDuJour : erreur lors de l'envoi d'un message MESSAGE_BILAN_ACTION");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }
                    else
                    {
                        //unité au repos
                        moral = Math.Min(lignePion.I_MORAL + Constantes.CST_GAIN_MORAL_REPOS, lignePion.I_MORAL_MAX);
                        diffmoral = moral - lignePion.I_MORAL;
                        lignePion.I_MORAL = moral;

                        //on ne fait fait la procédure de récupération de fatigue que si le moral est au moins revenu à la moitié.
                        diffatigue = 0;
                        if (moral >= lignePion.I_MORAL_MAX / 2 && lignePion.I_MORAL_MAX>0)
                        {
                            if (!RecuperationFatigue(lignePion, out diffatigue, ligneMeteo))
                            {
                                message = string.Format("FinDuJour : erreur dans RecuperationFatigue");
                                LogFile.Notifier(message, out messageErreur);
                                return false;
                            }
                        }

                        //affection à zéro avant l'envoi des messages, sinon, cela trouble les joueurs
                        lignePion.I_NB_PHASES_MARCHE_JOUR = 0;
                        lignePion.I_NB_PHASES_MARCHE_NUIT = 0;
                        lignePion.I_NB_HEURES_COMBAT = 0;
                        lignePion.I_NB_HEURES_FORTIFICATION = 0;

                        if (diffatigue > 0 && diffmoral > 0)
                        {
                            if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_BILAN_REPOS_MORAL_ET_FATIGUE, diffmoral, diffatigue))
                            {
                                message = string.Format("FinDuJour : erreur lors de l'envoi d'un message MESSAGE_BILAN_REPOS_MORAL_ET_FATIGUE");
                                LogFile.Notifier(message, out messageErreur);
                                return false;
                            }
                        }
                        else
                        {
                            if (diffmoral > 0)
                            {
                                if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_BILAN_REPOS_MORAL, diffmoral, diffatigue))
                                {
                                    message = string.Format("FinDuJour : erreur lors de l'envoi d'un message MESSAGE_BILAN_REPOS_MORAL");
                                    LogFile.Notifier(message, out messageErreur);
                                    return false;
                                }
                            }
                            if (diffatigue > 0)
                            {
                                if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_BILAN_REPOS_FATIGUE, diffmoral, diffatigue))
                                {
                                    message = string.Format("FinDuJour : erreur lors de l'envoi d'un message MESSAGE_BILAN_REPOS_FATIGUE");
                                    LogFile.Notifier(message, out messageErreur);
                                    return false;
                                }
                            }
                            if (diffmoral == 0 && diffatigue == 0)
                            {
                                if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_BILAN_REPOS))
                                {
                                    message = string.Format("FinDuJour : erreur lors de l'envoi d'un message MESSAGE_BILAN_REPOS");
                                    LogFile.Notifier(message, out messageErreur);
                                    return false;
                                }
                            }
                        }
                    }
                }
                ++i;
            }
            return true;
        }

        /// <summary>
        /// Récupération de la fatigue sur une unité
        /// 1/10 disparait à jamais
        /// 2/10 sont malades et constituent des blessés
        /// 7/10 sont récupérés au prorata du moral de l'unité
        /// </summary>
        /// <param name="lignePion">pion au repos</param>
        /// <param name="diffatigue">fatigue gagnée</param>
        /// <param name="ligneMeteo">météo courante</param>
        /// <returns>true si ok, false si ko</returns>
        private bool RecuperationFatigue(Donnees.TAB_PIONRow lignePion, out int diffatigue, Donnees.TAB_METEORow ligneMeteo)
        {
            string message;
            int nbInfanteriePerdus = 0;
            int nbCavaleriePerdus = 0;
            int nbArtilleriePerdus = 0;

            //modification d'après la météo courante
            diffatigue = -1;
            int nbTrainardsInfanterie = lignePion.I_INFANTERIE * lignePion.I_FATIGUE * ligneMeteo.I_POURCENT_RALLIEMENT / 100 / 100;
            int nbTrainardsCavalerie = lignePion.I_CAVALERIE * lignePion.I_FATIGUE * ligneMeteo.I_POURCENT_RALLIEMENT / 100 / 100;
            int nbTrainardsArtillerie = lignePion.I_ARTILLERIE * lignePion.I_FATIGUE * ligneMeteo.I_POURCENT_RALLIEMENT / 100 / 100;
            if ((nbTrainardsInfanterie + nbTrainardsCavalerie) * 2 / 10 >= Constantes.CST_TAILLE_MINIMUM_UNITE || nbTrainardsArtillerie>5)
            {
                //on constitue une unité de blessés/malades
                Donnees.TAB_PIONRow lignePionConvoiDeBlesses = lignePion.CreerConvoi(lignePion.proprietaire, true /*bBlesses*/, false /*bPrisonniers*/, false /*bRenfort*/);
                if (null == lignePionConvoiDeBlesses)
                {
                    message = string.Format("RecuperationFatigue : erreur lors de l'appel à CreerConvoi pour les blessés");
                    LogFile.Notifier(message);
                    return false;
                }
                lignePionConvoiDeBlesses.I_INFANTERIE = nbTrainardsInfanterie * 2 /10;
                lignePionConvoiDeBlesses.I_CAVALERIE = nbTrainardsCavalerie * 2 /10;
                lignePionConvoiDeBlesses.I_ARTILLERIE = nbTrainardsArtillerie * 2 / 10;
                lignePionConvoiDeBlesses.I_INFANTERIE_INITIALE = lignePionConvoiDeBlesses.I_INFANTERIE;
                lignePionConvoiDeBlesses.I_CAVALERIE_INITIALE = lignePionConvoiDeBlesses.I_CAVALERIE;
                lignePionConvoiDeBlesses.I_ARTILLERIE_INITIALE = lignePionConvoiDeBlesses.I_ARTILLERIE;
                // blessés + 1/10 perdus définitivement (soit la moitié des 2/10 des blessés)
                nbInfanteriePerdus = lignePionConvoiDeBlesses.I_INFANTERIE + lignePionConvoiDeBlesses.I_INFANTERIE/2;
                nbCavaleriePerdus = lignePionConvoiDeBlesses.I_CAVALERIE + lignePionConvoiDeBlesses.I_CAVALERIE/2;
                nbArtilleriePerdus = lignePionConvoiDeBlesses.I_ARTILLERIE + lignePionConvoiDeBlesses.I_ARTILLERIE/2;
                if (!ClassMessager.EnvoyerMessage(lignePionConvoiDeBlesses, ClassMessager.MESSAGES.MESSAGE_MALADES_RECUPERATION))
                {
                    message = string.Format("RecuperationFatigue : erreur lors de l'envoi d'un message MESSAGE_MALADES_RECUPERATION");
                    LogFile.Notifier(message);
                    return false;
                }
            }

            //s'il n'y a que de l'artillerie, l'unité ne doit jamais être fatigué donc pas de recup non plus ! -> si, fatigués comme les autres
            int recuperationFantassin = nbTrainardsInfanterie * lignePion.I_MORAL * 7 / 10 / lignePion.I_MORAL_MAX;
            int recuperationCavalerie = nbTrainardsCavalerie * lignePion.I_MORAL * 7 / 10 / lignePion.I_MORAL_MAX;
            int recuperationArtillerie = nbTrainardsArtillerie * lignePion.I_MORAL * 7 / 10 / lignePion.I_MORAL_MAX;

            // effectif reel = effectif theorique - (effectif theorique * fatigue /100)
            // donc fatigue = (effectif theorique - effectif reel) * 100 / effectif theorique
            int effectifTheorique = lignePion.I_INFANTERIE + lignePion.I_CAVALERIE + lignePion.I_ARTILLERIE;
            int effectifReel = recuperationFantassin + recuperationCavalerie + +recuperationArtillerie+ (effectifTheorique * lignePion.I_FATIGUE / 100);
            int fatigue = (effectifTheorique - effectifTheorique) * 100 / effectifTheorique;
            diffatigue = lignePion.I_FATIGUE - fatigue;
            lignePion.I_FATIGUE = fatigue;
            //on retire les blessés et perte des effectifs
            lignePion.I_INFANTERIE -= nbInfanteriePerdus;
            lignePion.I_CAVALERIE -= nbCavaleriePerdus;
            lignePion.I_ARTILLERIE -= nbArtilleriePerdus;

            return true;
        }

        private bool ExecuterMouvement(Donnees.TAB_PIONRow lignePion, int phase)
        {
            string messageErreur, message;

            //le mouvement d'une unité au combat n'est plus autorisé que si l'unité n'est pas engagé, sinon les unités en fuite ne peuvent plus fuir
            //par contre c'est vrai que cela permet à des unités de traverser des lignes ennemies, mais comme on supprime tous les ordres d'une unité ajoutée
            //et que l'on ne peut pas donner d'ordre de mouvement à une unité au combat, cela ne peut pas arriver normalement.
            /*
            if (lignePion.estAuCombat)
            {
                message = string.Format("{0}(ID={1}, en bataille:{2}, pas de mouvement autorisé)", lignePion.S_NOM, lignePion.ID_PION, lignePion.ID_BATAILLE);
                return LogFile.Notifier(message, out messageErreur);
            }
            */

            //TestCreationUnite(0, 1);
            ////il y a un ordre de mouvement pour l'unité, on prend le premier émis
            Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(lignePion.ID_PION);
            if (null != ligneOrdre)
            {
                //si l'ordre de mouvement n'est pas actif, le pion ne doit pas bouger
                if (!lignePion.OrdreActif(ligneOrdre))
                {
                    lignePion.PlacerPionEnRoute(ligneOrdre, lignePion.nation);
                    return true;
                }

                //recherche les modèles de l'unité
                Donnees.TAB_MODELE_PIONRow ligneModelePion = lignePion.modelePion;
                Donnees.TAB_MODELE_MOUVEMENTRow ligneModeleMouvement = Donnees.m_donnees.TAB_MODELE_MOUVEMENT.FindByID_MODELE_MOUVEMENT(ligneModelePion.ID_MODELE_MOUVEMENT);
                Donnees.TAB_NATIONRow ligneNation = lignePion.nation;// Donnees.m_donnees.TAB_PION.TrouveNation(lignePion);
                Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneOrdre.ID_CASE_DESTINATION);

                if (null == ligneCaseDestination)
                {
                    message = string.Format("ExecuterMouvement: {0}(ID={1}, ID_CASE:{2}, impossible de trouver la case de destination en base)", lignePion.S_NOM, lignePion.ID_PION, ligneOrdre.ID_CASE_DESTINATION);
                    return LogFile.Notifier(message, out messageErreur);
                }
                if (null == ligneNation)
                {
                    message = string.Format("ExecuterMouvement :{0}(ID={1}, Impossible de trouver la nation affectée à l'unité)", lignePion.S_NOM, lignePion.ID_PION);
                    return LogFile.Notifier(message, out messageErreur);
                }

                if (0 == ligneOrdre.I_EFFECTIF_DESTINATION)
                {
                    //si l'unité n'est pas arrivée à destination, il faut ajouter la fatigue
                    //l'unité est entrain de marcher, compte pour la fatigue en fin de journée
                    Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    if (Donnees.m_donnees.TAB_PARTIE.Nocturne())
                    {
                        lignePion.I_NB_PHASES_MARCHE_NUIT++;
                    }
                    else
                    {
                        lignePion.I_NB_PHASES_MARCHE_JOUR++;
                    }
                    Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                }

                if (lignePion.estMessager || lignePion.estPatrouille || lignePion.estQG)
                {
                    Donnees.TAB_CASERow ligneCaseDepart = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneOrdre.ID_CASE_DEPART);
                    if (null == ligneCaseDepart)
                    {
                        message = string.Format("ExecuterMouvement: {0}(ID={1}, ID_CASE_DEPART:{2}, impossible de trouver la case de départ en base)", lignePion.S_NOM, lignePion.ID_PION, ligneOrdre.ID_CASE_DEPART);
                        return LogFile.Notifier(message, out messageErreur);
                    }

                    return ExecuterMouvementSansEffectif(lignePion, ligneOrdre, ligneCaseDepart, ligneCaseDestination, ligneNation, ligneModelePion, ligneModeleMouvement);
                }
                else
                {
                    //Donnees.TAB_CASERow ligneCaseDepart = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePion.ID_CASE); -> si on fait cela, le parcours est recalculé à chaque mouvement dés qu'il n'y a plus d'effectif
                        //sur le point de départ et le chemin n'est trouvé n'est plus toujours le même, d'où un crash car il ne retrouve pas la position courante
                    Donnees.TAB_CASERow ligneCaseDepart = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneOrdre.ID_CASE_DEPART);
                    if (null == ligneCaseDepart)
                    {
                        message = string.Format("ExecuterMouvement: {0}(ID={1}, ID_CASE:{2}, impossible de trouver la case de départ en base)", lignePion.S_NOM, lignePion.ID_PION, lignePion.ID_CASE);
                        return LogFile.Notifier(message, out messageErreur);
                    }

                    return ExecuterMouvementAvecEffectif(lignePion, ligneOrdre, ligneCaseDepart, ligneCaseDestination, ligneNation, ligneModelePion, ligneModeleMouvement);
                }
            }
            else
            {
                message = string.Format("{0}(ID={1}, pas d'ordre de mouvement)", lignePion.S_NOM, lignePion.ID_PION);
                return LogFile.Notifier(message, out messageErreur);
            }
        }

        private bool ExecuterMouvementSansEffectif(Donnees.TAB_PIONRow lignePion, Donnees.TAB_ORDRERow ligneOrdre, Donnees.TAB_CASERow ligneCaseDepart, Donnees.TAB_CASERow ligneCaseDestination, Donnees.TAB_NATIONRow ligneNation, Donnees.TAB_MODELE_PIONRow ligneModelePion, Donnees.TAB_MODELE_MOUVEMENTRow ligneModeleMouvement)
        {
            string messageErreur, message;
            decimal vitesse;
            AstarTerrain[] tableCoutsMouvementsTerrain;
            List<LigneCASE> chemin;
            //Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain;
            Donnees.TAB_PIONRow lignePionDestinataire = null;
            Donnees.TAB_PIONRow lignePionNouveauDestinataire;
            double cout, coutHorsRoute;
            AStar etoile = new AStar();

            if (null == lignePion || null == ligneOrdre || null == ligneCaseDepart || null == ligneCaseDestination || null == ligneNation || null == ligneModelePion || null == ligneModeleMouvement)
            {
                message = string.Format("ExecuterMouvementSansEffectif : erreur, un des paramètres est null");
                LogFile.Notifier(message, out messageErreur);
                return false;
            }

            if (lignePion.B_DETRUIT)
            {
                message = string.Format("{0},ID={1}, ExecuterMouvementSansEffectif : unité détruite", lignePion.S_NOM, lignePion.ID_PION);
                LogFile.Notifier(message, out messageErreur);
                return true;
            }

            //l'unité avance-t-elle suffisement pour progresser d'une case de plus ?
            //lignePion.I_DISTANCE_A_PARCOURIR = 0;//pour les tests
            if (lignePion.I_DISTANCE_A_PARCOURIR > 0)
            {
                vitesse = lignePion.CalculVitesseMouvement();//vitesse en km/h
                Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                lignePion.I_DISTANCE_A_PARCOURIR -= (vitesse * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * Donnees.m_donnees.TAB_JEU[0].I_COUT_DE_BASE / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES);
                Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                message = string.Format("{0},ID={1}, en mouvement, I_DISTANCE_A_PARCOURIR={2}, vitesse={3}",
                    lignePion.S_NOM, lignePion.ID_PION, lignePion.I_DISTANCE_A_PARCOURIR, vitesse);
                LogFile.Notifier(message, out messageErreur);
            }
            if (lignePion.I_DISTANCE_A_PARCOURIR <= 0)
            {
                //faire avancer l'unité si celle-ci n'est pas arrivé à destination
                message = string.Format("{0},ID={1}, ExecuterMouvementSansEffectif :  ligneOrdre.ID_ORDRE={2} ligneOrdre.I_EFFECTIF_DEPART={3}",
                    lignePion.S_NOM, lignePion.ID_PION, ligneOrdre.ID_ORDRE, ligneOrdre.I_EFFECTIF_DEPART);
                LogFile.Notifier(message, out messageErreur);

                //on l'avance d'une case de plus
                //calcul du plus court chemin
                //il peut arriver que l'ordre de destination donne sur la même case que là où se trouve déjà l'unité.
                //par exemple si l'unité est sur la même case que son chef.
                if (lignePion.ID_CASE != ligneOrdre.ID_CASE_DESTINATION)
                {
                    if (!etoile.RechercheChemin(Constantes.TYPEPARCOURS.MOUVEMENT, lignePion, ligneCaseDepart, ligneCaseDestination, ligneOrdre, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                    {
                        message = string.Format("{0},ID={1}, erreur sur RechercheChemin (cas 2) dans ExecuterMouvementSansEffectif :{2})", lignePion.S_NOM, lignePion.ID_PION, messageErreur);
                        LogFile.Notifier(message, out messageErreur);
                        return false;
                    }
                    /*
                    CalculModeleMouvementsPion(lignePion, out tableCoutsMouvementsTerrain);
                    if (!m_etoile.SearchPath(ligneCaseDepart, ligneCaseDestination, tableCoutsMouvementsTerrain, DataSetCoutDonnees.m_donnees.TAB_JEU[0].I_ECHELLE))
                    {
                        message = string.Format("{0}(ID={1}, erreur sur SearchPath (cas 2) dans ExecuterMouvementSansEffectif)", lignePion.S_NOM, lignePion.ID_PION);
                        LogFile.Notifier(message, out messageErreur);
                        return false;
                    }
                    message = string.Format("ExecuterMouvementSansEffectif : SearchPath longueur={0}", m_etoile.PathByNodes.Length);
                     * */

                    message = string.Format("{0},ID={1}, ID_CASE={9}, ExecuterMouvementSansEffectif : SearchPath longueur={2} de {3}({4},{5}) à {6}({7},{8})",
                        lignePion.S_NOM, lignePion.ID_PION, chemin.Count, 
                        ligneCaseDepart.ID_CASE, ligneCaseDepart.I_X, ligneCaseDepart.I_Y,
                        ligneCaseDestination.ID_CASE, ligneCaseDestination.I_X, ligneCaseDestination.I_Y, lignePion.ID_CASE);
                    LogFile.Notifier(message, out messageErreur);

                    //recherche de la case sur le trajet
                    int pos = 0;
                    while (pos < chemin.Count && chemin[pos].ID_CASE != lignePion.ID_CASE) pos++;
                    if (pos==chemin.Count)
                    {
                        //ne doit jamais arrivé, bug de parallelisme quelque part
                        for(pos=0; pos<chemin.Count;pos++)
                        {
                            message = string.Format("{0},ID={1}, ExecuterMouvementSansEffectif : SearchPath pos={2} : {3}({4},{5})",
                                lignePion.S_NOM, lignePion.ID_PION, pos, chemin[pos].ID_CASE, chemin[pos].I_X, chemin[pos].I_Y);
                            LogFile.Notifier(message, out messageErreur);
                        }
                        throw new Exception("Erreur sur calcul de position dans ExecuterMouvementSansEffectif");
                    }


                    message = string.Format("{0},ID={1}, ExecuterMouvementSansEffectif : SearchPath longueur={2} position={3}",
                        lignePion.S_NOM, lignePion.ID_PION, chemin.Count, pos);
                    LogFile.Notifier(message, out messageErreur);

                    //on vérifie si l'unité ne croise pas son destinataire en visuel et qui aurait bougé
                    if (lignePion.estMessager)
                    {
                        lignePionDestinataire = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneOrdre.ID_DESTINATAIRE);
                        if (null == lignePionDestinataire)
                        {
                            message = string.Format("{0},ID={1}, ExecuterMouvementSansEffectif I pas de destinataire ID_ORDRE:{2})",
                                lignePion.S_NOM, lignePion.ID_PION, ligneOrdre.ID_ORDRE);
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }

                        // Si le destinataire n'a pas bougé, inutile de changer
                        if (lignePionDestinataire.ID_CASE != ligneOrdre.ID_CASE_DESTINATION)
                        {
                            Donnees.TAB_CASERow ligneCaseNouvelleDestination = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePionDestinataire.ID_CASE);
                            if (Constantes.Distance(chemin[pos].I_X, chemin[pos].I_Y, ligneCaseNouvelleDestination.I_X, ligneCaseNouvelleDestination.I_Y) < lignePion.vision * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE)
                            {
                                if (!etoile.RechercheChemin(Constantes.TYPEPARCOURS.MOUVEMENT, lignePion, chemin[pos], ligneCaseNouvelleDestination, null, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                                {
                                    message = string.Format("{0},ID={1}, erreur sur RechercheChemin dans ExecuterMouvementSansEffectif changement en visuel: message a un destinataire :{2})", lignePion.S_NOM, lignePion.ID_PION, messageErreur);
                                    LogFile.Notifier(message);
                                    return false;
                                }
                                pos = 0;
                                Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                                ligneOrdre.ID_CASE_DESTINATION = lignePionDestinataire.ID_CASE;
                                ligneOrdre.ID_CASE_DEPART = lignePion.ID_CASE;//sinon le trajet recalculé ne passe pas obligatoirement par la case courante
                                ligneOrdre.SetID_NOM_DESTINATIONNull();//première ville destinatrice du message, sans valeur maintenant
                                Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                            }
                        }
                    }

                    //Suite au recalcul le pion peut se retrouver sur la même case que le destinataire
                    if (lignePion.ID_CASE != ligneOrdre.ID_CASE_DESTINATION)
                    {
                        //on ajoute, le cout qu'il a fallu pour arriver jusqu'à cette case
                        int coutCase = ligneModelePion.CoutCase(chemin[pos + 1].ID_MODELE_TERRAIN);
                        Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(chemin[pos + 1].ID_MODELE_TERRAIN);

                        // Si le coût est négatif ou nul, c'est que la case est intraversable, cela peut arriver si l'unité à prévue de passer un ponton qui a été détruit.
                        // dans ce cas, il faut recalculer le trajet.
                        if (coutCase <= 0)
                        {
                            if (!EtablirCheminPratiquable(lignePion, ligneOrdre, ligneCaseDestination, ligneModelePion))
                            {
                                return false;
                            }
                        }
                        else
                        {

                            Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                            lignePion.ID_CASE = chemin[pos + 1].ID_CASE;
                            message = string.Format("{0},ID={1}, ExecuterMouvementSansEffectif nouvelle case={2}, position={3})",
                                lignePion.S_NOM, lignePion.ID_PION, lignePion.ID_CASE, pos);
                            LogFile.Notifier(message, out messageErreur);

                            if (chemin[pos + 1].I_X == chemin[pos].I_X || chemin[pos + 1].I_Y == chemin[pos].I_Y)
                            {
                                //ligne droite
                                lignePion.I_DISTANCE_A_PARCOURIR += coutCase;
                            }
                            else
                            {
                                //diagonale
                                lignePion.I_DISTANCE_A_PARCOURIR += (int)(Constantes.SQRT2 * coutCase);
                            }
                            Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                        }

                        //si l'unité emprunte un gué, on envoie un message d'alerte
                        if (!PassageSurGueOuPontDetruit(lignePion, ligneModeleTerrain)) { return false; }
                    }
                }

                //est on est arrivé à destination ?
                if (lignePion.ID_CASE==ligneOrdre.ID_CASE_DESTINATION)
                {
                    message = string.Format("{0},ID={1}, fin du mouvement, complètement arrivée)", lignePion.S_NOM, lignePion.ID_PION);
                    LogFile.Notifier(message, out messageErreur);

                    bool bOrdreTermine = true;

                    #region Messagers
                    if (lignePion.estMessager)
                    {
                        //on vérifie qu'il y a bien un destinataire valable
                        lignePionDestinataire = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneOrdre.ID_DESTINATAIRE);
                        if (null == lignePionDestinataire)
                        {
                            message = string.Format("{0}(ID={1}, ExecuterMouvementSansEffectif II pas de destinataire ID_ORDRE:{2})",
                                lignePion.S_NOM, lignePion.ID_PION, ligneOrdre.ID_ORDRE);
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }

                        // Si le destinataire est détruit, il est inutile de lui donner le message !
                        if (!lignePionDestinataire.B_DETRUIT)
                        {
                            long distanceCoutDeplacement = 0;

                            if (lignePionDestinataire.ID_CASE != ligneOrdre.ID_CASE_DESTINATION)
                            {
                                // Si le recepteur du message a bougé, aller vers la nouvelle case destination
                                // Si le destinataire et l'emetteur sont des joueur et qu'on lui porte un message, il ne faut pas qu'il soit trop éloigné pour continuer à rechercher le destinataire
                                // dans tous les autres cas, le message arrive toujours (le joueur n'étant pas à la source de l'envoi ou du problème de déplacement, on ne peut pas le pénaliser).
                                // pour ses propres unités on suppose qu'il sait toujours en gros où elles sont.
                                Donnees.TAB_PIONRow lignePionEmetteur = Donnees.m_donnees.TAB_PION.FindByID_PION(lignePion.ID_PION_PROPRIETAIRE);
                                if (lignePionDestinataire.estJoueur && lignePionEmetteur.estJoueur && (Constantes.ORDRES.MESSAGE == ligneOrdre.I_ORDRE_TYPE))
                                {
                                    //Calcul de la distance entre les deux zones indiquées
                                    Donnees.TAB_CASERow ligneCaseNouvelleDestination = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePionDestinataire.ID_CASE);

                                    if (!etoile.RechercheChemin(Constantes.TYPEPARCOURS.MOUVEMENT, lignePion, ligneCaseDestination, ligneCaseNouvelleDestination, null, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                                    {
                                        message = string.Format("{0}(ID={1}, erreur sur RechercheChemin dans ExecuterMouvementSansEffectif: message a un destinataire :{2})", lignePion.S_NOM, lignePion.ID_PION, messageErreur);
                                        LogFile.Notifier(message);
                                        return false;
                                    }

                                    // Calcul de la distance en km relative à l'effort fait pour le trajet. On divise deux fois par l'échelle, 
                                    // car le cout renvoyé est le nombre de pixel par case(echelle) * cout de la case
                                    // diviser ce cout par l'echelle et le cout de base renvoit donc la valeur en "pixels", pas en kilometre
                                    distanceCoutDeplacement = (long)cout / (Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * Donnees.m_donnees.TAB_JEU[0].I_COUT_DE_BASE);
                                }

                                if (distanceCoutDeplacement > Constantes.CST_DISTANCE_MAX_RECHERCHE_DESTINATAIRE)
                                {
                                    ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_DESTINATAIRE_INTROUVABLE);
                                    lignePion.DetruirePion();//le messager est devenu inutile
                                }
                                else
                                {
                                    //il faut suivre le destinataire !
                                    message = string.Format("{0}(ID={1}, ExecuterMouvementSansEffectif le destinataire a bougé ID_ORDRE:{2} de {3} vers {4})",
                                        lignePion.S_NOM, lignePion.ID_PION, ligneOrdre.ID_ORDRE, ligneOrdre.ID_CASE_DESTINATION, lignePionDestinataire.ID_CASE);
                                    LogFile.Notifier(message, out messageErreur);
                                    Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                                    ligneOrdre.ID_CASE_DESTINATION = lignePionDestinataire.ID_CASE;
                                    ligneOrdre.ID_CASE_DEPART = lignePion.ID_CASE;//sinon le trajet recalculé ne passe pas obligatoirement par la case courante
                                    ligneOrdre.SetID_NOM_DESTINATIONNull();//première ville destinatrice du message, sans valeur maintenant
                                    Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                                    bOrdreTermine = false; //finallement, l'ordre n'est pas terminé
                                }
                            }
                            else
                            {
                                // Un message peut-être de deux ordres, soit un message direct envoyé d'un joueur vers un autre ou d'une unité vers un joueur
                                // ou bien le message est en fait l'envoi d'un ordre d'un joueur vers l'une de ses unités
                                // dans ce deuxième cas, l'ordre transmis se trouve dans l'ordre suivant
                                Donnees.TAB_MESSAGERow ligneMessage = ligneOrdre.messageTransmis;
                                if (null != ligneMessage)
                                {
                                    //il s'agit d'un message textuel
                                    //donner le message et disparaître si le recepteur est un joueur, sinon aller vers le chef recevant le message
                                    //if (lignePionDestinataire.ID_PION_PROPRIETAIRE == lignePionDestinataire.ID_PION ||
                                    //    lignePionDestinataire.IsID_PION_PROPRIETAIRENull() || lignePionDestinataire.ID_PION_PROPRIETAIRE <= 0)
                                    if (null != Donnees.m_donnees.TAB_ROLE.TrouvePion(lignePionDestinataire.ID_PION))
                                    {
                                        message = string.Format("{0},ID={1}, ExecuterMouvementSansEffectif donne le messsage ID_ORDRE:{2} à {3}",
                                            lignePion.S_NOM, lignePion.ID_PION, ligneOrdre.ID_ORDRE, lignePionDestinataire.ID_PION);
                                        LogFile.Notifier(message, out messageErreur);

                                        Monitor.Enter(Donnees.m_donnees.TAB_MESSAGE.Rows.SyncRoot);
                                        ligneMessage.I_TOUR_ARRIVEE = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                                        ligneMessage.I_PHASE_ARRIVEE = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;
                                        ligneMessage.ID_PION_PROPRIETAIRE = lignePionDestinataire.ID_PION;
                                        Monitor.Exit(Donnees.m_donnees.TAB_MESSAGE.Rows.SyncRoot);

                                        ligneMessage.emetteur.ReceptionMessageTransfert(ligneMessage);
                                        lignePion.DetruirePion();//le messager est devenu inutile
                                    }
                                    else
                                    {
                                        //changement de destinataire
                                        lignePionNouveauDestinataire = Donnees.m_donnees.TAB_PION.FindByID_PION(lignePionDestinataire.ID_PION_PROPRIETAIRE);
                                        message = string.Format("{0},ID={1}, ExecuterMouvementSansEffectif change le destinataire du messsage ID_ORDRE:{2} de {3} ({4}) à {5} ({6})",
                                            lignePion.S_NOM, lignePion.ID_PION, ligneOrdre.ID_ORDRE,
                                            lignePionDestinataire.ID_PION,
                                            lignePionDestinataire.S_NOM,
                                            lignePionNouveauDestinataire.ID_PION,
                                            lignePionNouveauDestinataire.S_NOM);
                                        LogFile.Notifier(message, out messageErreur);

                                        Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                                        ligneOrdre.ID_DESTINATAIRE = lignePionNouveauDestinataire.ID_PION;
                                        ligneOrdre.ID_CASE_DESTINATION = lignePionNouveauDestinataire.ID_CASE;
                                        ligneOrdre.ID_CASE_DEPART = lignePion.ID_CASE;//sinon le trajet recalculé ne passe pas obligatoirement par la case courante
                                        ligneOrdre.SetID_NOM_DESTINATIONNull();//première ville destinatrice du message, sans valeur maintenant
                                        Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                                        bOrdreTermine = false;//finallement, l'ordre n'est pas terminé                                                
                                    }
                                }
                                else
                                {
                                    //le messager porte un ordre à l'unité destinatrice
                                    if (lignePionDestinataire.I_TOUR_FUITE_RESTANT > 0)
                                    {
                                        //en fuite, une unité ne peut pas recevoir de nouvel ordre, le messager doit donc attendre que son message soit lu/accepté
                                        message = string.Format("{0},ID={1}, ExecuterMouvementSansEffectif arrivée d'un message porteur d'un ordre ID_ORDRE:{2} de type {3} à {4} ID={5} alors que celui-ci est en retraite, ordre différé",
                                            lignePion.S_NOM, lignePion.ID_PION, ligneOrdre.ID_ORDRE, ligneOrdre.I_ORDRE_TYPE, lignePionDestinataire.S_NOM, lignePionDestinataire.ID_PION);
                                        LogFile.Notifier(message, out messageErreur);
                                        return true;
                                    }

                                    Donnees.TAB_ORDRERow ligneOrdreNouveau = ligneOrdre.ordreTransmis;
                                    if (null == ligneOrdreNouveau) { return true; }//pour régler un correctif d'un autre bug, à supprimer ensuite
                                    Donnees.TAB_ORDRERow ligneOrdreCourant = Donnees.m_donnees.TAB_ORDRE.Courant(lignePionDestinataire.ID_PION);
                                    Donnees.TAB_PIONRow lignePionDonneurOrdre = Donnees.m_donnees.TAB_PION.FindByID_PION(lignePion.ID_PION_PROPRIETAIRE);
                                    Donnees.TAB_PIONRow lignePionMessage = lignePionDestinataire;
                                    ClassMessager.MESSAGES tipeMessage = ClassMessager.MESSAGES.MESSAGE_AUCUN_MESSAGE;

                                    switch (ligneOrdreNouveau.I_ORDRE_TYPE)
                                    {
                                        case Constantes.ORDRES.ARRET:
                                        case Constantes.ORDRES.MOUVEMENT:
                                            //l'ordre n'est recevable que si l'unité n'est pas engagé au combat
                                            //si le pion est déjà engagé en bataille et pas en fuite, l'ordre est refusé
                                            if (!lignePionDestinataire.IsID_BATAILLENull() && 0 == lignePionDestinataire.I_TOUR_FUITE_RESTANT)
                                            {
                                                Donnees.TAB_BATAILLERow ligneBataille = Donnees.m_donnees.TAB_BATAILLE.FindByID_BATAILLE(lignePionDestinataire.ID_BATAILLE);
                                                ClassMessager.EnvoyerMessage(lignePionDestinataire, ClassMessager.MESSAGES.MESSAGE_ORDRE_MOUVEMENT_IMPOSSIBLE_AU_COMBAT, ligneBataille);
                                            }
                                            else
                                            {
                                                if (!ChangerOrdreCourant(lignePionDestinataire, ligneOrdreCourant, ligneOrdreNouveau, true)) { return false; }
                                                tipeMessage = (ligneOrdreNouveau.I_ORDRE_TYPE== Constantes.ORDRES.ARRET) ? ClassMessager.MESSAGES.MESSAGE_ORDRE_RECU : ClassMessager.MESSAGES.MESSAGE_ORDRE_MOUVEMENT_RECU;
                                            }
                                            break;

                                        case Constantes.ORDRES.ENDOMMAGER_PONT:
                                        case Constantes.ORDRES.REPARER_PONT:
                                        case Constantes.ORDRES.CONSTRUIRE_PONTON:
                                        case Constantes.ORDRES.SEFORTIFIER:
                                            //l'ordre n'est recevable que si l'unité n'est pas engagé au combat
                                            //si le pion est déjà engagé en bataille et pas en fuite, l'ordre est refusé
                                            if (!lignePionDestinataire.IsID_BATAILLENull() && 0 == lignePionDestinataire.I_TOUR_FUITE_RESTANT)
                                            {
                                                Donnees.TAB_BATAILLERow ligneBataille = Donnees.m_donnees.TAB_BATAILLE.FindByID_BATAILLE(lignePionDestinataire.ID_BATAILLE);
                                                ClassMessager.EnvoyerMessage(lignePionDestinataire, ClassMessager.MESSAGES.MESSAGE_ORDRE_MOUVEMENT_IMPOSSIBLE_AU_COMBAT, ligneBataille);
                                            }
                                            else
                                            {
                                                if (!ChangerOrdreCourant(lignePionDestinataire, ligneOrdreCourant, ligneOrdreNouveau, true)) { return false; }
                                                tipeMessage = ClassMessager.MESSAGES.MESSAGE_ORDRE_RECU;
                                            }
                                            break;

                                        case Constantes.ORDRES.GENERERCONVOI:
                                        case Constantes.ORDRES.RENFORCER:
                                        case Constantes.ORDRES.ETABLIRDEPOT:
                                        case Constantes.ORDRES.LIGNE_RAVITAILLEMENT:
                                            //l'ordre est toujours recevable, même au combat
                                            if (!ChangerOrdreCourant(lignePionDestinataire, ligneOrdreCourant, ligneOrdreNouveau, true)) { return false; }
                                            tipeMessage = ClassMessager.MESSAGES.MESSAGE_ORDRE_RECU;
                                            break;

                                        case Constantes.ORDRES.TRANSFERER:
                                            //l'ordre n'étant pas à l'initiative du joueur mais de son supérieur, il faut l'executer immédiatement sans modifier l'ordre en cours du joueur
                                            //Cartographie.TransfertPion(ligneOrdre.cible, ligneOrdre.ID_DESTINATAIRE_CIBLE);
                                            if (!ligneOrdreNouveau.cible.TransfertPion(ligneOrdreNouveau.ID_DESTINATAIRE)) { return false; }
                                            break;
                                        //case Constantes.ORDRES.RETRAITE: ->toujours un ordre direct normalement
                                        //    if (!ChangerOrdreCourant(lignePionDestinataire, ligneOrdreCourant, ligneOrdreNouveau)) { return false; }
                                        //    break;
                                        case Constantes.ORDRES.PATROUILLE:
                                            //création d'une patrouille
                                            Donnees.TAB_PIONRow lignePionPatrouille = null;

                                            //recherche du modele
                                            int idModelePATROUILLE = Donnees.m_donnees.TAB_MODELE_PION.RechercherModele(ligneModelePion.ID_NATION, "PATROUILLE");

                                            if (idModelePATROUILLE >= 0)
                                            {
                                                Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot); 
                                                lignePionPatrouille = Donnees.m_donnees.TAB_PION.AddTAB_PIONRow(
                                                    idModelePATROUILLE,//ID_MODELE_PION
                                                    lignePionDestinataire.ID_PION, //ID_PION_PROPRIETAIRE
                                                    -1,//ID_NOUVEAU_PION_PROPRIETAIRE,
                                                    -1,//ID_ANCIEN_PION_PROPRIETAIRE
                                                    lignePionDestinataire.NomDeLaPatrouille(),
                                                    0, 0,//I_INFANTERIE
                                                    0, 0,
                                                    0, 0,
                                                    0,//I_FATIGUE
                                                    100,//I_MORAL, le moral doit être à 100, sinon il va ajouter +1km/h à la vitesse pour déroute
                                                    100,//I_MORAL_MAX
                                                    0, 0, 0,
                                                    'Z',//C_NIVEAU_HIERACHIQUE
                                                    0, 0, 0, 0,
                                                    lignePionDestinataire.ID_CASE,
                                                    0, //I_TOUR_SANS_RAVITAILLEMENT
                                                    0, -1,
                                                    0,//I_TOUR_RETRAITE_RESTANT
                                                    0,
                                                    false, //B_DETRUIT
                                                    false,// B_FUITE_AU_COMBAT
                                                    false,// B_INTERCEPTION
                                                    false,//B_REDITION_RAVITAILLEMENT
                                                    false,//B_TELEPORTATION
                                                    false,//B_ENNEMI_OBSERVABLE
                                                    0,//I_MATERIEL,
                                                    0,//I_RAVITAILLEMENT,
                                                    false,//B_CAVALERIE_DE_LIGNE,
                                                    false,//B_CAVALERIE_LOURDE,
                                                    false,//B_GARDE,
                                                    false,//B_VIEILLE_GARDE,
                                                    0,//I_TOUR_CONVOI_CREE,
                                                    -1,//ID_DEPOT_SOURCE
                                                    0,//I_SOLDATS_RAVITAILLES,
                                                    0,//I_NB_HEURES_FORTIFICATION,
                                                    0,//I_NIVEAU_FORTIFICATION,
                                                    -1,//ID_PION_REMPLACE,
                                                    0,//I_DUREE_HORS_COMBAT,
                                                    0,//I_TOUR_BLESSURE,
                                                    false,//B_BLESSES,
                                                    false,//B_PRISONNIERS,
                                                    false,//B_RENFORT
                                                    -1,//ID_LIEU_RATTACHEMENT,
                                                    'Z',//C_NIVEAU_DEPOT
                                                    -1,//ID_PION_ESCORTE, 
                                                    0,//I_INFANTERIE_ESCORTE, 
                                                    0,//I_CAVALERIE_ESCORTE
                                                    0,//I_MATERIEL_ESCORTE
                                                    0,//I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT
                                                    0//I_VICTOIRE
                                                );
                                                lignePionPatrouille.SetID_ANCIEN_PION_PROPRIETAIRENull();
                                                lignePionPatrouille.SetID_NOUVEAU_PION_PROPRIETAIRENull();
                                                lignePionPatrouille.SetI_ZONE_BATAILLENull();
                                                lignePionPatrouille.SetID_BATAILLENull();
                                                lignePionPatrouille.SetID_PION_ESCORTENull();
                                                lignePionPatrouille.SetID_PION_REMPLACENull();
                                                lignePionPatrouille.SetID_DEPOT_SOURCENull();
                                                Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot); 
                                            }
                                            else
                                            {
                                                message = string.Format("ExecuterMouvementSansEffectif : Erreur grave impossible de trouver le modèle PATROUILLE ID_PION={0}, ID_NATION={1}",
                                                    lignePionPatrouille.ID_PION, ligneModelePion.ID_NATION);
                                                LogFile.Notifier(message, out messageErreur);
                                                return false;
                                            }
                                            //on lui affecte l'ordre correspondant
                                            Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                                            ligneOrdreNouveau.ID_PION = lignePionPatrouille.ID_PION;
                                            ligneOrdreNouveau.ID_CASE_DEPART = lignePionDestinataire.ID_CASE;
                                            ligneOrdreNouveau.I_TOUR_DEBUT = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                                            ligneOrdreNouveau.I_PHASE_DEBUT = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;
                                            Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                                            bOrdreTermine = false;

                                            tipeMessage = ClassMessager.MESSAGES.MESSAGE_ORDRE_PATROUILLE_RECU;
                                            lignePionMessage = lignePionPatrouille;
                                            break;

                                        default:
                                            //on ne devrait pas se retouver dans ce cas
                                            message = string.Format("{0},ID={1}, Erreur grave, ExecuterMouvementSansEffectif transmission d'un ordre ID_ORDRE:{2} de type {3} à {4} ID={5} alors que ce n'est pas possible !",
                                                lignePion.S_NOM, lignePion.ID_PION, ligneOrdreNouveau.ID_ORDRE, ligneOrdreNouveau.I_ORDRE_TYPE, lignePionDestinataire.S_NOM, lignePionDestinataire.ID_PION);
                                            LogFile.Notifier(message, out messageErreur);
                                            return false;
                                    }

                                    //envoyer un messager à l'emetteur indiquant la reception de l'ordre
                                    if (tipeMessage != ClassMessager.MESSAGES.MESSAGE_AUCUN_MESSAGE)
                                    {
                                        if (!ClassMessager.EnvoyerMessage(lignePionMessage, tipeMessage))
                                        {
                                            message = string.Format("ExecuterMouvementSansEffectif  : erreur lors de l'envoi d'un message pour indiquer qu'une unité vient de recevoir un ordre");
                                            LogFile.Notifier(message, out messageErreur);
                                            return false;
                                        }
                                    }
                                    message = string.Format("{0},ID={1}, ExecuterMouvementSansEffectif transmission d'un ordre ID_ORDRE:{2} de type {3} à {4} ID={5}",
                                        lignePion.S_NOM, lignePion.ID_PION, ligneOrdreNouveau.ID_ORDRE, ligneOrdreNouveau.I_ORDRE_TYPE, lignePionDestinataire.S_NOM, lignePionDestinataire.ID_PION);
                                    LogFile.Notifier(message, out messageErreur);

                                    lignePion.TerminerOrdre(ligneOrdre, false, false);
                                    lignePion.DetruirePion();//le messager d'origine est devenu inutile
                                }
                            }
                        }
                        else
                        {
                            lignePion.TerminerOrdre(ligneOrdre, false, false);
                            lignePion.DetruirePion();//le messager d'origine est devenu inutile
                        }
                    }
                    #endregion
                    else
                    {
                        if (lignePion.estPatrouille)
                        {
                            //analyser ce que voit la patrouille puis envoyer un messager avec le résultat
                            lignePion.RapportDePatrouille();
                            bOrdreTermine = false;//la patrouille est maintenant un message-patrouille qui revient porter son rapport
                        }
                        else
                        {
                            //envoyer un messager pour prévenir de l'arrivée complète si l'on est pas un messager, ni une patrouille
                            if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_ARRIVE_A_DESTINATION))
                            {
                                message = string.Format("ExecuterMouvementSansEffectif  : erreur lors de l'envoi d'un message pour indiquer qu'une unité arrive à destination");
                                LogFile.Notifier(message, out messageErreur);
                                return false;
                            }
                        }
                    }

                    if (bOrdreTermine)
                    {
                        //l'ordre est terminé, doit être fait à la fin, car l'ordre courant est recherché par plusieurs opérations préalable
                        lignePion.TerminerOrdre(ligneOrdre, true, false);
                    }
                }
            }
            // on place l'unité sur la case où elle est actuellement
            Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePion.ID_CASE);
            if (null == ligneCase)
            {
                message = string.Format("{0},ID={1}, ExecuterMouvementSansEffectif : impossible de trouver la case courante ID={2}", lignePion.S_NOM, lignePion.ID_PION, lignePion.ID_CASE);
                LogFile.Notifier(message, out messageErreur);
                return false;
            }
            else
            {
                int nbPlacesOccupees=0;
                lignePion.RequisitionCase(ligneCase, true, ref nbPlacesOccupees);//note : tous les cas de rencontre entre pions sont gérés dans cette méthode
                //ligneCase.ID_NOUVEAU_PROPRIETAIRE = lignePion.ID_PION;

                message = string.Format("{0},ID={1}, ExecuterMouvementSansEffectif : pion en ID={2} X={3} Y={4}",
                    lignePion.S_NOM, lignePion.ID_PION, ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y);
                LogFile.Notifier(message, out messageErreur);
            }

            return true;
        }

        /// <summary>
        /// Si le coût est négatif ou nul, c'est que la case est intraversable, cela peut arriver si l'unité à prévue de passer un ponton qui a été détruit.
        /// dans ce cas, il faut recalculer le trajet.
        /// </summary>
        /// <param name="lignePion">pion en mouvement</param>
        /// <param name="ligneOrdre">ordre de mouvement</param>
        /// <param name="ligneCaseDestination">case de destination du mouvement</param>
        /// <param name="ligneModelePion">modèle du pion</param>
        /// <param name="chemin">chemin du mouvement</param>
        /// <param name="resCout">coût sur les cases du mouvement</param>
        /// <returns>true si OK, false si KO</returns>
        private bool EtablirCheminPratiquable(Donnees.TAB_PIONRow lignePion, Donnees.TAB_ORDRERow ligneOrdre, Donnees.TAB_CASERow ligneCaseDestination, Donnees.TAB_MODELE_PIONRow ligneModelePion)
        {
            string message, messageErreur;
            AstarTerrain[] tableCoutsMouvementsTerrain;
            double cout, coutHorsRoute;
            List<LigneCASE> chemin;

            //a-t-on déjà envoyé un message pour prévenir mon supérieur recemment ?                            
            Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageEmis(lignePion.ID_PION, ClassMessager.MESSAGES.MESSAGE_CHEMIN_IMPRATICABLE);
            if (null == ligneMessage ||
                ligneMessage.I_TOUR_DEPART + ClassMessager.CST_MESSAGE_FREQUENCE_ALERTE < Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)
            {
                if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_CHEMIN_IMPRATICABLE))
                {
                    return false;
                }
            }
            Donnees.m_donnees.TAB_PARCOURS.SupprimerParcoursPion(lignePion.ID_PION);
            Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
            ligneOrdre.I_EFFECTIF_DEPART = lignePion.effectifTotalEnMouvement;
            ligneOrdre.I_EFFECTIF_DESTINATION = 0;
            ligneOrdre.ID_CASE_DEPART = lignePion.ID_CASE;
            Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
            Donnees.TAB_CASERow ligneCaseDepart = lignePion.CaseCourante();
            AStar etoile = new AStar();
            if (!etoile.RechercheChemin(Constantes.TYPEPARCOURS.MOUVEMENT, lignePion, ligneCaseDepart, ligneCaseDestination, ligneOrdre, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
            {
                message = string.Format("{0}(ID={1}, erreur sur EtablirCheminPratiquable :{2})", lignePion.S_NOM, lignePion.ID_PION, messageErreur);
                LogFile.Notifier(message);
                return false;
            }

            int coutCase = ligneModelePion.CoutCase(chemin[1].ID_MODELE_TERRAIN);
            if (coutCase <= 0)
            {
                // le nouveau chemin n'est pas plus praticable que le précédent, normalement impossible car on ne peut pas autoriser la destruction d'un ponton si une unité s'y trouve.
                // on ne peut donc pas détruire un ponton avec une unité au milieu qui se retrouverait au milieu de terrain impraticable
                message = string.Format("{0}(ID={1}, erreur, unité au milieu d'un chemin impraticable dans EtablirCheminPratiquable", lignePion.S_NOM, lignePion.ID_PION);
                LogFile.Notifier(message);
                return false;
            }
            
            return true;
        }

        private bool ChangerOrdreCourant(Donnees.TAB_PIONRow lignePionDestinataire, Donnees.TAB_ORDRERow ligneOrdreCourant, Donnees.TAB_ORDRERow ligneOrdreNouveau, bool bTerminerLesOrdresSuivant)
        {
            string message;

            //Si jamais le nouvel ordre est le même que l'ordre courant, on ne fait rien car cela peut avoir des conséquences sur l'ordre en cours
            if ((null != ligneOrdreNouveau) && null != (ligneOrdreCourant))
            {
                if (ligneOrdreCourant.I_ORDRE_TYPE == ligneOrdreNouveau.I_ORDRE_TYPE)
                {
                    bool bMemeOrdre = false;
                    switch (ligneOrdreCourant.I_ORDRE_TYPE)
                    {
                        case Constantes.ORDRES.MOUVEMENT:
                            if (ligneOrdreCourant.I_DUREE == ligneOrdreNouveau.I_DUREE
                                && ligneOrdreCourant.ID_CASE_DESTINATION == ligneOrdreNouveau.ID_CASE_DESTINATION
                                && ligneOrdreCourant.I_HEURE_DEBUT == ligneOrdreNouveau.I_HEURE_DEBUT)
                            {
                                bMemeOrdre = true;
                            }
                            break;
                        case Constantes.ORDRES.MESSAGE:
                        case Constantes.ORDRES.PATROUILLE:
                        case Constantes.ORDRES.GENERERCONVOI:
                        case Constantes.ORDRES.ETABLIRDEPOT:
                            break;
                        case Constantes.ORDRES.COMBAT:
                            if (ligneOrdreCourant.ID_BATAILLE == ligneOrdreNouveau.ID_BATAILLE
                                && ligneOrdreCourant.I_ZONE_BATAILLE == ligneOrdreNouveau.I_ZONE_BATAILLE)
                            {
                                bMemeOrdre = true;
                            }
                            break;
                        case Constantes.ORDRES.RETRAITE:
                        case Constantes.ORDRES.CONSTRUIRE_PONTON:
                        case Constantes.ORDRES.ENDOMMAGER_PONT:
                        case Constantes.ORDRES.REPARER_PONT:
                        case Constantes.ORDRES.ARRET:
                        case Constantes.ORDRES.SEFORTIFIER:
                        case Constantes.ORDRES.RETRAIT:
                            bMemeOrdre = true;
                            break;
                        case Constantes.ORDRES.TRANSFERER:
                            if (ligneOrdreCourant.ID_CIBLE == ligneOrdreNouveau.ID_CIBLE
                                && ligneOrdreCourant.ID_DESTINATAIRE_CIBLE == ligneOrdreNouveau.ID_DESTINATAIRE_CIBLE)
                            {
                                bMemeOrdre = true;
                            }
                            break;
                        case Constantes.ORDRES.RENFORCER:
                            if (ligneOrdreCourant.ID_CIBLE == ligneOrdreNouveau.ID_CIBLE)
                            {
                                bMemeOrdre = true;
                            }
                            break;
                        case Constantes.ORDRES.LIGNE_RAVITAILLEMENT:
                            if (ligneOrdreCourant.ID_CASE_DESTINATION == ligneOrdreNouveau.ID_CASE_DESTINATION)
                            {
                                bMemeOrdre = true;
                            }
                            break;
                        default:
                            LogFile.Notifier("ChangerOrdreCourant Ordre inconnu reçu");
                            return false;
                    }

                    if (bMemeOrdre)
                    {
                        message = string.Format("{0}(ID={1}, ChangerOrdreCourant reçoit le même ordre {2} que le précédent {3}, on ne le change donc pas", 
                            lignePionDestinataire.S_NOM, lignePionDestinataire.ID_PION, ligneOrdreNouveau.ID_ORDRE, ligneOrdreCourant.ID_ORDRE);
                        LogFile.Notifier(message);
                        return true;
                    }
                }
            }

            //terminer l'ordre précédent du pion, s'il existe
            if (null != ligneOrdreCourant)
            {
                //si l'unité à avancé, il faut en tenir compte, sinon on repart de la position de fin de colonne ce qui n'est pas logique
                if (lignePionDestinataire.effectifTotal > 0)
                {
                    if (!lignePionDestinataire.PlacerPionEnBivouac(ligneOrdreCourant))
                    {
                        return false;
                    }
                }
                //on terminer l'ordre sans envoyer de message, puisque c'est un changement d'ordre, il va déjà avoir un message pour le nouvel ordre
                lignePionDestinataire.TerminerOrdre(ligneOrdreCourant, false, bTerminerLesOrdresSuivant);
            }

            //activer le nouvel ordre (s'il y en a un ! Mais, normalement, il y en a toujours un)
            if (null != ligneOrdreNouveau)
            {
                Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                ligneOrdreNouveau.ID_PION = lignePionDestinataire.ID_PION;
                ligneOrdreNouveau.ID_CASE_DEPART = lignePionDestinataire.ID_CASE;
                ligneOrdreNouveau.I_TOUR_DEBUT = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                ligneOrdreNouveau.I_PHASE_DEBUT = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;
                ligneOrdreNouveau.I_EFFECTIF_DEPART = lignePionDestinataire.effectifTotalEnMouvement;
                Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);

                //si le nouvel ordre est un ordre de mouvement ou de combat, la fortification de campagne est détruite
                if (ligneOrdreNouveau.I_ORDRE_TYPE == Constantes.ORDRES.MOUVEMENT ||
                    ligneOrdreNouveau.I_ORDRE_TYPE == Constantes.ORDRES.COMBAT ||
                    ligneOrdreNouveau.I_ORDRE_TYPE == Constantes.ORDRES.RETRAITE)
                {
                    Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    lignePionDestinataire.I_NIVEAU_FORTIFICATION = 0;
                    Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                }
            }
            return true;
        }

        private bool ExecuterMouvementAvecEffectif(Donnees.TAB_PIONRow lignePion, Donnees.TAB_ORDRERow ligneOrdre, Donnees.TAB_CASERow ligneCaseDepart, Donnees.TAB_CASERow ligneCaseDestination, Donnees.TAB_NATIONRow ligneNation, Donnees.TAB_MODELE_PIONRow ligneModelePion, Donnees.TAB_MODELE_MOUVEMENTRow ligneModeleMouvement)
        {
            string messageErreur, message;
            decimal vitesse;

            if (null == lignePion || null == ligneOrdre || null == ligneCaseDepart || null == ligneCaseDestination || null == ligneNation || null == ligneModelePion || null == ligneModeleMouvement)
            {
                message = string.Format("ExecuterMouvementAvecEffectifAvecEffectif : erreur, un des paramètres est null");
                LogFile.Notifier(message, out messageErreur);
                return false;
            }

            if (lignePion.B_DETRUIT)
            {
                message = string.Format("{0},ID={1}, ExecuterMouvementAvecEffectifAvecEffectif : unité détruite", lignePion.S_NOM, lignePion.ID_PION);
                LogFile.Notifier(message, out messageErreur);
                return true;
            }

            //l'unité avance-t-elle suffisement pour progresser d'une case de plus ?
            //lignePion.I_DISTANCE_A_PARCOURIR = 0;//pour les tests
            vitesse = lignePion.CalculVitesseMouvement();//vitesse en km/h
            Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            lignePion.I_DISTANCE_A_PARCOURIR -= (vitesse * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * Donnees.m_donnees.TAB_JEU[0].I_COUT_DE_BASE / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES);
            Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            message = string.Format("{0}, ID={1}, en mouvement, I_DISTANCE_A_PARCOURIR={2}",
                lignePion.S_NOM, lignePion.ID_PION, lignePion.I_DISTANCE_A_PARCOURIR);
            LogFile.Notifier(message, out messageErreur);

            if (lignePion.I_DISTANCE_A_PARCOURIR > 0)
            {
                //il faut placer le pion dans sa position actuelle en cas de collision pour les unités qui suivent et se déplacent
                return lignePion.PlacerPionEnRoute(ligneOrdre, ligneNation);
            }
            if (lignePion.I_DISTANCE_A_PARCOURIR <= 0)
            {
                //l'unité progresse d'une case
                if (ligneOrdre.I_EFFECTIF_DESTINATION > 0)
                {
                    if (!ExecuterMouvementAvecEffectifForcesADestination(lignePion, ligneOrdre, ligneCaseDepart, ligneCaseDestination, ligneNation))
                        return false;
                }
                else
                {
                    if (ligneOrdre.I_EFFECTIF_DEPART > 0)
                    {
                        if (!ExecuterMouvementAvecEffectifForcesAuDepart(lignePion, ligneOrdre, ligneCaseDepart, ligneCaseDestination, ligneNation))
                            return false;
                    }
                    else
                    {
                        if (!ExecuterMouvementAvecEffectifForcesEnRoute(lignePion, ligneOrdre, ligneCaseDepart, ligneCaseDestination, ligneNation))
                            return false;

                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Toutes les forces de l'unité en déplacement se trouvent sur la route
        /// Aucune ne se trouvent ni au départ ni à l'arrivée
        /// </summary>
        /// <param name="lignePion"></param>
        /// <param name="ligneOrdre"></param>
        /// <param name="ligneCaseDepart"></param>
        /// <param name="ligneCaseDestination"></param>
        /// <param name="ligneNation"></param>
        /// <returns></returns>
        private bool ExecuterMouvementAvecEffectifForcesEnRoute(Donnees.TAB_PIONRow lignePion, Donnees.TAB_ORDRERow ligneOrdre, Donnees.TAB_CASERow ligneCaseDepart, Donnees.TAB_CASERow ligneCaseDestination, Donnees.TAB_NATIONRow ligneNation)
        {
            string message, messageErreur;
            decimal encombrement;
            int nbplacesOccupes;
            int iCavalerie, iInfanterie, iArtillerie;
            double cout, coutHorsRoute;
            AstarTerrain[] tableCoutsMouvementsTerrain;
            List<LigneCASE> chemin;
            int i, j, id_case_finale, id_case_source;

            //faire avancer l'unité si celle-ci n'est pas arrivé à destination
            message = string.Format("{0},ID={1}, en ExecuterMouvementAvecEffectifForcesEnRoute, aucune troupe à destination)", lignePion.S_NOM, lignePion.ID_PION);
            LogFile.Notifier(message, out messageErreur);

            //on calcule combien de cases occupe le pion
            lignePion.CalculerRepartitionEffectif(lignePion.effectifTotalEnMouvement,
                out iInfanterie, out iCavalerie, out iArtillerie);
            message = string.Format("{0},ID={1}, ExecuterMouvementAvecEffectifForcesEnRoute :effectif: i={2} c={3} a={4}", lignePion.S_NOM, lignePion.ID_PION, iInfanterie, iCavalerie, iArtillerie);
            LogFile.Notifier(message, out messageErreur);

            encombrement = lignePion.CalculerEncombrement(ligneNation, iInfanterie, iCavalerie, iArtillerie, true);
            message = string.Format("{0},ID={1}, ExecuterMouvementAvecEffectifForcesEnRoute : encombrement={2}", lignePion.S_NOM, lignePion.ID_PION, encombrement);
            LogFile.Notifier(message, out messageErreur);

            id_case_source = lignePion.ID_CASE;
            Donnees.TAB_MODELE_PIONRow ligneModelePion = lignePion.modelePion;
            //calcul du plus court chemin
            //CalculModeleMouvementsPion(lignePion, out tableCoutsMouvementsTerrain);
            //if (!m_etoile.SearchPath(ligneCaseDepart, ligneCaseDestination, tableCoutsMouvementsTerrain, DataSetCoutDonnees.m_donnees.TAB_JEU[0].I_ECHELLE))
            AStar etoile = new AStar();
            if (!etoile.RechercheChemin(Constantes.TYPEPARCOURS.MOUVEMENT, lignePion, ligneCaseDepart, ligneCaseDestination, ligneOrdre, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
            {
                message = string.Format("{0},ID={1}, erreur sur RechercheChemin (cas 2) dans ExecuterMouvementAvecEffectifForcesEnRoute: {2})", lignePion.S_NOM, lignePion.ID_PION, messageErreur);
                LogFile.Notifier(message, out messageErreur);
                return false;
            }

            message = string.Format("ExecuterMouvementAvecEffectifForcesEnRoute : SearchPath longueur={0}", chemin.Count);
            LogFile.Notifier(message, out messageErreur);

            //Le général de l'unité occupe la queue de la division, on recherche sa position actuelle
            i = 0 ;
            while (i < chemin.Count && chemin[i].ID_CASE != lignePion.ID_CASE)
            {
                i++;
            }
            if (i >= chemin.Count)
            {
                message = string.Format("{0},ID={1}, ExecuterMouvementAvecEffectifForcesEnRoute: impossible de trouver la position du pion sur le parcours !)", 
                    lignePion.S_NOM, lignePion.ID_PION);
                LogFile.Notifier(message, out messageErreur);
                return false;
            }
            //le général avance d'une case et il est sur la dernière case de la colonne
            id_case_finale = chemin[++i].ID_CASE;
            message = string.Format("{0},ID={1}, ExecuterMouvementAvecEffectifForcesEnRoute : pion en {2}({3},{4})",lignePion.S_NOM, lignePion.ID_PION, chemin[i].ID_CASE, chemin[i].I_X, chemin[i].I_Y);
            LogFile.Notifier(message, out messageErreur);

            // On place les troupes sur le chemin
            j = nbplacesOccupes = 0;
            //while (i < chemin.Count && nbplacesOccupes < encombrement)
            while (i < chemin.Count && j < encombrement)//on doit toujours considérer, libre ou non, sinon, on saute les cases occupées par l'ennemi la nuit ou on avance plus vite si c'est des amis !
            {
                if (!lignePion.RequisitionCase(chemin[i], true, ref nbplacesOccupes)) { return false; }
                i++;
                j++;
            }

            // -> on a fait i++, donc la case chemin[i] est la suivante
            //la tête de colonne avance d'une case supplémentaire elle aussi
            //si on est arrivé à destination, on affecte les troupes qui viennent d'arriver sur place
            //if (i + 1 >= chemin.Count)
            if (i >= chemin.Count)
            {
                lignePion.CalculerRepartitionEffectif(1, out iInfanterie, out iCavalerie, out iArtillerie);
                Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                ligneOrdre.I_EFFECTIF_DESTINATION = iInfanterie + iCavalerie + iArtillerie;
                Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                message = string.Format("{0},ID={1}, ExecuterMouvementAvecEffectif : premiers effectifs à destination: i={2} c={3} a={4}", lignePion.S_NOM, lignePion.ID_PION, iInfanterie, iCavalerie, iArtillerie);
                LogFile.Notifier(message, out messageErreur);

                //Est-ce que l'on voit un ennemi en arrivant sur cette nouvelle case ? Si oui, message
                if (!lignePion.MessageEnnemiObserve(ligneCaseDestination)) { return false; }
            }
            else
            {
                // -> on a fait i++, donc la case chemin[i] est la suivante
                //calcul du coût pour avancer d'une case supplémentaire & messages de blocages
                int coutCase;
                bool bBlocageParUnite;
                if (!CalculCoutCase(lignePion, ligneModelePion, ligneOrdre, ligneCaseDestination, chemin[i], out coutCase, out bBlocageParUnite))
                {
                    return false;
                }

                // Si le coût est négatif ou nul, c'est que la case est intraversable ou qu'il y a un bouchon qui empèche le mouvement, cela peut arriver si l'unité à prévue de passer un ponton qui a été détruit.
                // dans ce cas, il faut recalculer le trajet.
                if (coutCase <= 0)
                {
                    //si on recalcule le trajet, le pion perd la phase en réorganisation mais on n'augmente pas la distance à parcourir
                    //pour repartir sur le nouveau trajet à la prochaine phase
                    if (!bBlocageParUnite)
                    {
                        if (!EtablirCheminPratiquable(lignePion, ligneOrdre, ligneCaseDestination, ligneModelePion))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //List<Donnees.TAB_CASERow> chemin;
                    Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    if (0==i || chemin[i].I_X == chemin[i-1].I_X || chemin[i].I_Y == chemin[i-1].I_Y)
                    {
                        //ligne droite
                        lignePion.I_DISTANCE_A_PARCOURIR += coutCase;
                    }
                    else
                    {
                        //diagonale
                        lignePion.I_DISTANCE_A_PARCOURIR += (int)(Constantes.SQRT2 * coutCase);
                    }
                    Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    if (!lignePion.RequisitionCase(chemin[i-1], true, ref nbplacesOccupes)) { return false; }
                }

                //si l'unité a déjà modifiée sa position (à cause d'une rencontre ennemie par exemple, il ne faut pas la redeplacer)
                //dans le cas inverse, on lui affecte bien la nouvelle case lié à son déplacement
                if (id_case_source == lignePion.ID_CASE && coutCase>0)
                {
                    Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    lignePion.ID_CASE = id_case_finale;
                    Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                }
            }
            return true;
        }

        private bool CalculCoutCase(Donnees.TAB_PIONRow lignePion, Donnees.TAB_MODELE_PIONRow ligneModelePion, Donnees.TAB_ORDRERow ligneOrdre, Donnees.TAB_CASERow ligneCaseDestination, LigneCASE ligneCaseChemin, out int coutCase, out bool bBlocageParUnite)
        {
            return CalculCoutCase(lignePion, ligneModelePion, ligneOrdre, ligneCaseDestination, Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneCaseChemin.ID_CASE), out coutCase, out bBlocageParUnite);
        }

        private bool CalculCoutCase(Donnees.TAB_PIONRow lignePion, Donnees.TAB_MODELE_PIONRow ligneModelePion, Donnees.TAB_ORDRERow ligneOrdre, Donnees.TAB_CASERow ligneCaseDestination, Donnees.TAB_CASERow ligneCaseChemin, out int coutCase, out bool bBlocageParUnite)
        {
            coutCase = -1;
            bBlocageParUnite = false;

            //Est-ce que l'on voit un ennemi en arrivant sur cette nouvelle case ? Si oui, message
            if (!lignePion.MessageEnnemiObserve(ligneCaseChemin)) { return false; }

            //on ajoute, le cout qu'il a fallu pour arriver jusqu'à cette case
            Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain;
            ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCaseChemin.ID_MODELE_TERRAIN);

            //le chemin est-il occupée par une autre unité 
            int IdNouveauProprietaire = ligneCaseChemin.ID_NOUVEAU_PROPRIETAIRE;
            if ((Constantes.NULLENTIER != IdNouveauProprietaire) && (IdNouveauProprietaire != lignePion.ID_PION))
            {
                Donnees.TAB_PIONRow lignePionBlocage = Donnees.m_donnees.TAB_PION.FindByID_PION(IdNouveauProprietaire);

                if (lignePion.estEnnemi(lignePionBlocage) && lignePionBlocage.estCombattif && Donnees.m_donnees.TAB_PARTIE.Nocturne())
                {
                    //pas de combat la nuit, mais l'avancement est bloqué, il faut l'indiquer au joueur
                    //a-t-on déjà envoyé un message pour prévenir mon supérieur recemment ?                            
                    Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageEmis(lignePion.ID_PION, ClassMessager.MESSAGES.MESSAGE_CHEMIN_BLOQUE_PAR_ENNEMI_LA_NUIT);
                    if (null == ligneMessage ||
                        ligneMessage.I_TOUR_DEPART + ClassMessager.CST_MESSAGE_FREQUENCE_ALERTE < Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)
                    {
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_CHEMIN_BLOQUE_PAR_ENNEMI_LA_NUIT))
                        {
                            return false;
                        }
                    }
                    return true;
                }

                Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrainOccupe;
                ligneModeleTerrainOccupe = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(ligneCaseChemin.ID_MODELE_TERRAIN_SI_OCCUPE);
                int coutCaseOccupe = ligneModelePion.CoutCase(ligneModeleTerrainOccupe.ID_MODELE_TERRAIN);

                //la route est déjà occupée "surchargée"
                if ((ligneModeleTerrain.B_ANNULEE_SI_OCCUPEE || coutCaseOccupe <= 0)
                    && !lignePionBlocage.estStatique && lignePionBlocage.ID_PION != lignePion.ID_PION)
                {
                    //la case est occupée par une unité en mouvement et il faut attendre qu'elle se libère (cas des ponts par exemple)
                    //a-t-on déjà envoyé un message pour prévenir mon supérieur recemment ?                            
                    Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageEmis(lignePion.ID_PION, ClassMessager.MESSAGES.MESSAGE_CHEMIN_BLOQUE);
                    if (null == ligneMessage ||
                        ligneMessage.I_TOUR_DEPART + ClassMessager.CST_MESSAGE_FREQUENCE_ALERTE < Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)
                    {
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_CHEMIN_BLOQUE))
                        {
                            return false;
                        }
                    }
                    bBlocageParUnite = true;
                    return true;
                }
                else
                {
                    // On prend le terrain de substitution, sauf si c'est la même unité
                    if (lignePionBlocage.ID_PION != lignePion.ID_PION)
                    {
                        //si le terrain de substitution est intraversable, on prend le terrain par defaut
                        if (coutCaseOccupe <= 0)
                        {
                            ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(Donnees.m_donnees.TAB_JEU[0].ID_MODELE_TERRAIN_DEPLOIEMENT);
                        }
                        else
                        {
                            ligneModeleTerrain = ligneModeleTerrainOccupe;
                        }
                    }
                }
            }

            //si l'unité emprunte un gué, on envoie un message d'alerte
            if (!PassageSurGueOuPontDetruit(lignePion, ligneModeleTerrain)) { return false; }

            //calcul du cout
            coutCase = ligneModelePion.CoutCase(ligneModeleTerrain.ID_MODELE_TERRAIN);

            return true;
        }

        private bool ExecuterMouvementAvecEffectifForcesAuDepart(Donnees.TAB_PIONRow lignePion, Donnees.TAB_ORDRERow ligneOrdre, Donnees.TAB_CASERow ligneCaseDepart, Donnees.TAB_CASERow ligneCaseDestination, Donnees.TAB_NATIONRow ligneNation)
        {
            string message, messageErreur;
            decimal encombrement;
            int nbplacesOccupes;
            int iCavalerie, iInfanterie, iArtillerie;
            double cout, coutHorsRoute;
            AstarTerrain[] tableCoutsMouvementsTerrain;
            List<LigneCASE> chemin;
            int i;

            //faire avancer l'unité si celle-ci n'est pas arrivé à destination
            message = string.Format("{0}(ID={1}, ExecuterMouvementAvecEffectifForcesAuDepart en mouvement, aucune troupe à destination)", lignePion.S_NOM, lignePion.ID_PION);
            LogFile.Notifier(message, out messageErreur);
            message = string.Format("ExecuterMouvementAvecEffectifForcesAuDepart :ligneOrdre.I_EFFECTIF_DEPART={0}", ligneOrdre.I_EFFECTIF_DEPART);
            LogFile.Notifier(message, out messageErreur);

            // si le pion commence tout juste à avance, on détruit les espaces précédents
            if (lignePion.effectifTotalEnMouvement == ligneOrdre.I_EFFECTIF_DEPART)
            {
                Donnees.m_donnees.TAB_ESPACE.SupprimerEspacePion(lignePion.ID_PION);
                //AcceptChanges only updates your rows in the (in memory) dataset, that is - marks them as "not needed for actual database update".
                //Donnees.m_donnees.TAB_ESPACE.AcceptChanges();
            }

            //on calcule de combien de cases le pion a déjà avancé
            Donnees.TAB_MODELE_PIONRow ligneModelePion = lignePion.modelePion;
            lignePion.CalculerRepartitionEffectif(lignePion.effectifTotalEnMouvement - ligneOrdre.I_EFFECTIF_DEPART,
                                                    out iInfanterie, out iCavalerie, out iArtillerie);
            message = string.Format("ExecuterMouvementAvecEffectifForcesAuDepart :effectif: i={0} c={1} a={2}", iInfanterie, iCavalerie, iArtillerie);
            LogFile.Notifier(message, out messageErreur);

            encombrement = lignePion.CalculerEncombrement(ligneNation, iInfanterie, iCavalerie, iArtillerie, true);
            message = string.Format("ExecuterMouvementAvecEffectifForcesAuDepart : encombrement={0}", encombrement);
            LogFile.Notifier(message, out messageErreur);

            //calcul du plus court chemin
            AStar etoile = new AStar();
            if (!etoile.RechercheChemin(Constantes.TYPEPARCOURS.MOUVEMENT, lignePion, ligneCaseDepart, ligneCaseDestination, ligneOrdre, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
            {
                message = string.Format("{0}(ID={1}, erreur sur RechercheChemin (cas 2) dans ExecuterMouvementAvecEffectifForcesAuDepart: {2})", lignePion.S_NOM, lignePion.ID_PION, messageErreur);
                LogFile.Notifier(message, out messageErreur);
                return false;
            }
            message = string.Format("ExecuterMouvementAvecEffectifForcesAuDepart : SearchPath longueur={0}", chemin.Count);
            LogFile.Notifier(message, out messageErreur);
            i = 1;//pas 0 car la première case est la case de départ

            nbplacesOccupes = 0;
            //while (i < chemin.Count && nbplacesOccupes < encombrement) -> il ne faut pas tenir compte du fait que la case est libre ou non, sinon, on "saute" des cases occupées durant le déplacement
            //on réoccupe toutes les cases occupées durant le dernier déplacement
            while (i < chemin.Count && i < encombrement)
            {
                //la nouvelle case est-elle occupée par un ennemi ?
                if (!lignePion.RequisitionCase(chemin[i], true, ref nbplacesOccupes)) { return false; }
                i++;
            }

            //si on est arrivé à destination, on affecte les troupes qui viennent d'arriver sur place
            if (i+1 >= chemin.Count)
            {
                lignePion.CalculerRepartitionEffectif(1, out iInfanterie, out iCavalerie, out iArtillerie);
                Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                ligneOrdre.I_EFFECTIF_DESTINATION = iInfanterie + iCavalerie + iArtillerie;
                Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                message = string.Format("ExecuterMouvementAvecEffectifForcesAuDepart : premiers effectifs à destination: i={0} c={1} a={2}", iInfanterie, iCavalerie, iArtillerie);
                LogFile.Notifier(message, out messageErreur);
            }
            else
            {
                //calcul du coût pour avancer d'une case supplémentaire & messages de blocages
                int coutCase;
                bool bBlocageParUnite;
                if (!CalculCoutCase(lignePion, ligneModelePion, ligneOrdre, ligneCaseDestination, chemin[i + 1], out coutCase, out bBlocageParUnite))
                {
                    return false;
                }

                if (coutCase <= 0)
                {
                    // Si le coût est négatif ou nul, c'est que la case est intraversable ou qu'il y a un bouchon, cela peut arriver si l'unité à prévue de passer un ponton qui a été détruit.
                    // dans ce cas, il faut recalculer le trajet.
                    //si on recalcule le trajet, le pion perd la phase en réorganisation mais on n'augmente pas la distance à parcourir
                    //pour repartir sur le nouveau trajet à la prochaine phase
                    if (!bBlocageParUnite)
                    {
                        if (!EtablirCheminPratiquable(lignePion, ligneOrdre, ligneCaseDestination, ligneModelePion))
                        {
                            return false;
                        }
                    }

                }
                else
                {
                    Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    if (i < 2 || chemin[i].I_X == chemin[i - 1].I_X || chemin[i].I_Y == chemin[i - 1].I_Y)
                    {
                        //ligne droite
                        lignePion.I_DISTANCE_A_PARCOURIR += coutCase;
                    }
                    else
                    {
                        //diagonale
                        lignePion.I_DISTANCE_A_PARCOURIR += (int)(Constantes.SQRT2 * coutCase);
                    }
                    Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    //la nouvelle case est-elle occupée par un ennemi ?
                    if (!lignePion.RequisitionCase(chemin[i+1], true, ref nbplacesOccupes)) { return false; }
                }
            }

            //on calcule les effectifs qui ne sont plus au départ, avec une case de plus sur la route
            encombrement++;
            lignePion.CalculerEffectif(ligneNation, encombrement, true, out iInfanterie, out iCavalerie, out iArtillerie);
            message = string.Format("ExecuterMouvementAvecEffectifForcesAuDepart :effectif en route: i={0} c={1} a={2} encombrement={3}", iInfanterie, iCavalerie, iArtillerie, encombrement);
            LogFile.Notifier(message, out messageErreur);

            //on place les effectifs encore au départ
            Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
            ligneOrdre.I_EFFECTIF_DEPART = Math.Max(0, lignePion.effectifTotalEnMouvement - iInfanterie - iCavalerie - iArtillerie);
            Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
            lignePion.PlacementPion(ligneOrdre.ID_CASE_DEPART, ligneNation, true, ligneOrdre.I_EFFECTIF_DEPART);
            message = string.Format("ExecuterMouvementAvecEffectifForcesAuDepart : ligneOrdre.ID_ORDRE={0} ligneOrdre.I_EFFECTIF_DEPART final={1}",
                ligneOrdre.ID_ORDRE, ligneOrdre.I_EFFECTIF_DEPART);
            LogFile.Notifier(message, out messageErreur);
            return true;
        }

        /// <summary>
        /// Vérifie si un pion passe sur un Gué ou un pont détruit, si oui, envoie un message d'information
        /// </summary>
        /// <param name="lignePion">Pion avançant sur un terrain</param>
        /// <param name="ligneModeleTerrain">Modele du terrain traversé</param>
        /// <returns>true = OK, false =KO</returns>
        private bool PassageSurGueOuPontDetruit(Donnees.TAB_PIONRow lignePion, Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain)
        {
            if ((ligneModeleTerrain.B_PONTON || ligneModeleTerrain.B_PONT) && ligneModeleTerrain.B_DETRUIT
                && !lignePion.estDepot && !lignePion.estMessager && !lignePion.estPatrouille)
            {
                Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageEmis(lignePion.ID_PION, ClassMessager.MESSAGES.MESSAGE_TRAVERSE_GUE);
                if (null == ligneMessage ||
                    ligneMessage.I_TOUR_DEPART + ClassMessager.CST_MESSAGE_FREQUENCE_ALERTE < Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)
                {
                    if (!ClassMessager.EnvoyerMessage(lignePion, ligneModeleTerrain, ClassMessager.MESSAGES.MESSAGE_TRAVERSE_GUE))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// L'unité est arrivée, il faut donc "écouler" les éléments qui ne sont pas encore arrivés s'il y en a
        /// </summary>
        /// <returns></returns>
        private bool ExecuterMouvementAvecEffectifForcesADestination(Donnees.TAB_PIONRow lignePion, Donnees.TAB_ORDRERow ligneOrdre, Donnees.TAB_CASERow ligneCaseDepart, Donnees.TAB_CASERow ligneCaseDestination, Donnees.TAB_NATIONRow ligneNation)
        {
            string message, messageErreur;
            decimal encombrement, encombrementTotal;
            int nbplacesOccupes;
            int iCavalerie, iInfanterie, iArtillerie;
            int iCavalerieDestination, iInfanterieDestination, iArtillerieDestination;
            int iCavalerieRoute, iInfanterieRoute, iArtillerieRoute;
            double cout, coutHorsRoute;
            AstarTerrain[] tableCoutsMouvementsTerrain;
            List<LigneCASE> chemin;
            int i;

            if (!lignePion.MessageEnnemiObserve(ligneCaseDestination)) { return false; }

            //L'unité est arrivée, il faut donc "écouler" les éléments qui ne sont pas encore arrivés s'il y en a
            message = string.Format("\r\n{0}(ID={1}, ExecuterMouvementAvecEffectifForcesADestination : en mouvement, une partie des troupes à destination)", lignePion.S_NOM, lignePion.ID_PION);
            LogFile.Notifier(message, out messageErreur);

            iInfanterie = (lignePion.estDepot || lignePion.estConvoiDeRavitaillement || lignePion.estPontonnier) ? lignePion.effectifTotalEnMouvement : lignePion.infanterie;
            encombrementTotal = lignePion.CalculerEncombrement(ligneNation, iInfanterie, lignePion.cavalerie, lignePion.artillerie, true);
            message = string.Format("ExecuterMouvementAvecEffectifForcesADestination :ligneOrdre.I_EFFECTIF_DESTINATION={0} encombrementTotal={1}", ligneOrdre.I_EFFECTIF_DESTINATION, encombrementTotal);
            LogFile.Notifier(message, out messageErreur);
            if (ligneOrdre.I_EFFECTIF_DESTINATION <= lignePion.effectifTotalEnMouvement)
            {
                //il faut faire avancer la "queue" de troupes jusqu'à l'arrivée
                //on avance suivant le modele par défaut, sur route, dont on calcule le cout
                AStar etoile = new AStar();
                if (!etoile.RechercheChemin(Constantes.TYPEPARCOURS.MOUVEMENT, lignePion, ligneCaseDepart, ligneCaseDestination, ligneOrdre, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                {
                    message = string.Format("{0}(ID={1}, ALERTE erreur sur RechercheChemin dans ExecuterMouvementAvecEffectifForcesADestination:{2}", lignePion.S_NOM, lignePion.ID_PION, messageErreur);
                    LogFile.Notifier(message, out messageErreur);
                    return false;
                }

                Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(Donnees.m_donnees.TAB_JEU[0].ID_MODELE_TERRAIN_DEPLOIEMENT);
                Donnees.TAB_MODELE_PIONRow ligneModelePion = lignePion.modelePion;
                int coutCase = ligneModelePion.CoutCase(ligneModeleTerrain.ID_MODELE_TERRAIN);
                Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                lignePion.I_DISTANCE_A_PARCOURIR += coutCase;
                Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);

                //recherche du plus court chemin
                //CalculModeleMouvementsPion(lignePion, out tableCoutsMouvementsTerrain);
                //if (!m_etoile.SearchPath(ligneCaseDepart, ligneCaseDestination, tableCoutsMouvementsTerrain, DataSetCoutDonnees.m_donnees.TAB_JEU[0].I_ECHELLE))

                //effectifs actuellement à l'arrivée
                lignePion.CalculerRepartitionEffectif(ligneOrdre.I_EFFECTIF_DESTINATION, out iInfanterieDestination, out iCavalerieDestination, out iArtillerieDestination);
                message = string.Format("ExecuterMouvementAvecEffectifForcesADestination :effectif à destination :i={0} c={1} a={2}",
                    iInfanterieDestination, iCavalerieDestination, iArtillerieDestination);
                LogFile.Notifier(message, out messageErreur);

                //effectifs maximum sur la route
                decimal encombrementArrivee = lignePion.CalculerEncombrement(ligneNation, iInfanterieDestination, iCavalerieDestination, iArtillerieDestination, true);
                int iInfanterieLocal = (lignePion.estDepot || lignePion.estConvoiDeRavitaillement || lignePion.estPontonnier) ? lignePion.effectifTotalEnMouvement : lignePion.I_INFANTERIE;
                if (!lignePion.CalculerEffectif(ligneNation,
                        iInfanterieLocal - iInfanterieDestination,
                        lignePion.I_CAVALERIE - iCavalerieDestination,
                        lignePion.I_ARTILLERIE - iArtillerieDestination,
                        encombrementTotal - encombrementArrivee,
                        //chemin.Count, 
                        true,
                        out iInfanterieRoute, out iCavalerieRoute, out iArtillerieRoute))
                {
                    message = string.Format("ExecuterMouvementAvecEffectifForcesADestination :{0}(ID={1}, erreur CalculerEffectif sur la route renvoie false)", lignePion.S_NOM, lignePion.ID_PION);
                    LogFile.Notifier(message, out messageErreur);
                    return false;
                }
                message = string.Format("ExecuterMouvementAvecEffectifForcesADestination effectif maximum sur route iINFANTERIESource={0}, iCAVALERIESource={1}, iARTILLERIESource={2}, lg chemin={3}, iINFANTERIERoute={4}, iCAVALERIERoute={5}, iARTILLERIERoute={6}",
                    iInfanterieLocal - iInfanterieDestination,
                    lignePion.I_CAVALERIE - iCavalerieDestination,
                    lignePion.I_ARTILLERIE - iArtillerieDestination,
                    chemin.Count, iInfanterieRoute, iCavalerieRoute, iArtillerieRoute);
                LogFile.Notifier(message, out messageErreur);
                if (0 == ligneOrdre.I_EFFECTIF_DEPART)
                {
                    //la route est partiellement occupée
                    encombrement = encombrementTotal - encombrementArrivee; // Cartographie.CalculerEncombrement(lignePion, ligneNation, iInfanterieRoute, iCavalerieRoute, iArtillerieRoute, true);
                }
                else
                {
                    //la route est complètement occupée
                    encombrement = chemin.Count;
                }
                //en avançant d'une case, quels sont les nouveaux effectifs qui arrivent
                if (!lignePion.CalculerEffectif(ligneNation, iInfanterieRoute, iCavalerieRoute, iArtillerieRoute, 1, true, out iInfanterie, out iCavalerie, out iArtillerie))
                {
                    message = string.Format("ExecuterMouvementAvecEffectifForcesADestination :{0}(ID={1}, erreur CalculerEffectif renvoie false)", lignePion.S_NOM, lignePion.ID_PION);
                    LogFile.Notifier(message, out messageErreur);
                    return false;
                }
                message = string.Format("ExecuterMouvementAvecEffectifForcesADestination effectif en plus à destination iINFANTERIESource={0}, iCAVALERIESource={1}, iARTILLERIESource={2}, effectif={3}, iINFANTERIE={4}, iCAVALERIE={5}, iARTILLERIE={6}",
                    iInfanterieRoute, iCavalerieRoute, iArtillerieRoute, 1, iInfanterie, iCavalerie, iArtillerie);
                LogFile.Notifier(message, out messageErreur);

                //on modifie en conséquence les effectifs de départ et de destination
                ligneOrdre.I_EFFECTIF_DEPART = Math.Max(0, ligneOrdre.I_EFFECTIF_DEPART - iInfanterie - iCavalerie - iArtillerie);
                iInfanterieDestination += iInfanterie;
                iCavalerieDestination += iCavalerie;
                iArtillerieDestination += iArtillerie;
                Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                ligneOrdre.I_EFFECTIF_DESTINATION = iInfanterieDestination + iCavalerieDestination + iArtillerieDestination;
                Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);

                if (ligneOrdre.I_EFFECTIF_DESTINATION >= lignePion.effectifTotalEnMouvement)
                {
                    return lignePion.ArriveADestination(ligneOrdre, ligneNation);
                }

                message = string.Format("ExecuterMouvementAvecEffectifForcesADestination :encombrement sur route={0}", encombrement);
                LogFile.Notifier(message, out messageErreur);
                message = string.Format("ExecuterMouvementAvecEffectifForcesADestination :effectif destination={0}", ligneOrdre.I_EFFECTIF_DESTINATION);
                LogFile.Notifier(message, out messageErreur);

                //placer les effectifs en chemin
                i = chemin.Count - 1;
                nbplacesOccupes = 0;
                while (i >= 0 && nbplacesOccupes < encombrement)
                {
                    //la nouvelle case est-elle occupée par un ennemi ?
                    if (!lignePion.RequisitionCase(chemin[i], true, ref nbplacesOccupes)) { return false; }
                    i--;
                }
                if (nbplacesOccupes < encombrement)
                {
                    message = string.Format("ALERTE ExecuterMouvementAvecEffectifForcesADestination : impossible de placer les effectifs de fin de mouvement PION={0}({1}) nbplacesOccupes={2}<encombrement={3}",
                        lignePion.S_NOM, lignePion.ID_PION, nbplacesOccupes, encombrement);
                    LogFile.Notifier(message, out messageErreur);
                }
                if (i > 0)
                {
                    //la dernière place occupée, devient la "position" du pion, c'est à dire de son général, en queue de colonne
                    Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    lignePion.ID_CASE = chemin[i].ID_CASE;
                    Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                }
                //placement des effectifs à destination sur la carte
                lignePion.PlacementPion(ligneOrdre.ID_CASE_DESTINATION, ligneNation, false, ligneOrdre.I_EFFECTIF_DESTINATION);

                //placer les effectifs encore au point de départ
                if (ligneOrdre.I_EFFECTIF_DEPART > 0)
                {
                    message = string.Format("ExecuterMouvementAvecEffectifForcesADestination :effectif depart={0} i={1}, c={2}, a={3}",
                        ligneOrdre.I_EFFECTIF_DEPART,
                        lignePion.I_INFANTERIE - iInfanterieDestination - iInfanterieRoute,
                        lignePion.I_CAVALERIE - iCavalerieDestination - iCavalerieRoute,
                        lignePion.I_ARTILLERIE - iArtillerieDestination - iArtillerieRoute);
                    LogFile.Notifier(message, out messageErreur);
                    //out iInfanterie, out iCavalerie, out iArtillerie
                    lignePion.PlacementPion(ligneOrdre.ID_CASE_DEPART, ligneNation, true,
                        lignePion.I_INFANTERIE - iInfanterieDestination - iInfanterieRoute,
                        lignePion.I_CAVALERIE - iCavalerieDestination - iCavalerieRoute,
                        lignePion.I_ARTILLERIE - iArtillerieDestination - iArtillerieRoute);
                }
            }
            else
            {
                //Ce cas est possible si la fatigue de la nuit a réduit le nombre de forces en mouvement à moins que la force arrivée -> traiter dans fatigue et repose
                message = string.Format("{0}(ID={1}, Erreur ExecuterMouvementAvecEffectifForcesADestination fin du mouvement, complètement arrivée : cas normalement impossible)", lignePion.S_NOM, lignePion.ID_PION); 
                LogFile.Notifier(message, out messageErreur);
                return false;
            }
            return true;
        }

        private void MiseAJourProprietaires()
        {
            /* Ce traitement prend 2 minutes ! Mais il n'est pas possible de le faire en Linq, donc pas de solution à moins d'utiliser
            foreach (Donnees.TAB_CASERow ligne in Donnees.m_donnees.TAB_CASE)
            {
                if (ligne.IsID_NOUVEAU_PROPRIETAIRENull())
                {
                    ligne.SetID_PROPRIETAIRENull();
                }
                else
                {
                    ligne.ID_PROPRIETAIRE = ligne.ID_NOUVEAU_PROPRIETAIRE;
                }
                ligne.SetID_NOUVEAU_PROPRIETAIRENull();
            }
             * */
            /* autre solution en 5 secondes, mais ne marche pas car ne remet pas a blanc idProprietaire en fait
            Donnees.TAB_CASEDataTable changeDataSet = (Donnees.TAB_CASEDataTable)Donnees.m_donnees.TAB_CASE.GetChanges();
            if (0==changeDataSet.Rows.Count) { return; }
            foreach (Donnees.TAB_CASERow ligneChange in changeDataSet)
            {
                Donnees.TAB_CASERow ligne = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneChange.ID_CASE);
                if (ligne.IsID_NOUVEAU_PROPRIETAIRENull())
                {
                    ligne.SetID_PROPRIETAIRENull();
                }
                else
                {
                    ligne.ID_PROPRIETAIRE = ligne.ID_NOUVEAU_PROPRIETAIRE;
                }
                ligne.SetID_NOUVEAU_PROPRIETAIRENull();
                ligne.AcceptChanges();
            }
             */
            //Donnees.TAB_CASERow ligneCaseD = Donnees.m_donnees.TAB_CASE.FindByXY(1379, 1774);
            /*** Note : maintenant qu'il n'y a plus de valeur NULL, passer par une requete est sans intérêt
            string requete = "(ID_PROPRIETAIRE IS NOT NULL) OR (ID_NOUVEAU_PROPRIETAIRE IS NOT NULL)";
            Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
            //le resultat du select peut être modifié par un chargement case qui provoque un crash
            Donnees.TAB_CASERow[] changeRows = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete);
            for (int l=0; l<changeRows.Count(); l++)
            {
                //Donnees.TAB_CASERow ligne = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneChange.ID_CASE);
                Donnees.TAB_CASERow ligne = changeRows[l];
                int IdNouveauProprietaire = ligne.ID_NOUVEAU_PROPRIETAIRE;
                if (Constantes.NULLENTIER == IdNouveauProprietaire)
                {
                    ligne.SetID_PROPRIETAIRENull();
                }
                else
                {
                    ligne.ID_PROPRIETAIRE = IdNouveauProprietaire;
                }
                ligne.SetID_NOUVEAU_PROPRIETAIRENull();
            }
            //Donnees.TAB_CASERow ligneCaseD2 = Donnees.m_donnees.TAB_CASE.FindByXY(1379, 1774);
            Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
            */
            Monitor.Enter(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
            /**** -> prend 1m30
            foreach (Donnees.TAB_CASERow ligne in Donnees.m_donnees.TAB_CASE)
            {
                ligne.ID_PROPRIETAIRE = ligne.ID_NOUVEAU_PROPRIETAIRE;
                ligne.SetID_NOUVEAU_PROPRIETAIRENull();
            }
            **/
            string requete = string.Format("(ID_PROPRIETAIRE<>{0}) OR (ID_NOUVEAU_PROPRIETAIRE<>{0})", Constantes.NULLENTIER);
            Donnees.TAB_CASERow[] changeRows = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete);
            for (int l = 0; l < changeRows.Count(); l++)
            {
                Donnees.TAB_CASERow ligne = changeRows[l];
                ligne.ID_PROPRIETAIRE = ligne.ID_NOUVEAU_PROPRIETAIRE;
                ligne.SetID_NOUVEAU_PROPRIETAIRENull();
            }
            Monitor.Exit(Donnees.m_donnees.TAB_CASE.Rows.SyncRoot);
        }

        internal bool miseÀJourInternet(string fichierCourant, out string messageErreur)
        {
            InterfaceVaocWeb iWeb;
            LogFile.CreationLogFile(fichierCourant, "tour", Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE);
            messageErreur = string.Empty;

            Cartographie.ChargerLesFichiers();

            iWeb = ClassVaocWebFactory.CreerVaocWeb(fichierCourant, true);

            iWeb.TraitementEnCours(true, Donnees.m_donnees.TAB_JEU[0].ID_JEU, Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);

            iWeb.SauvegardeNomsCarte(Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);

            iWeb.SauvegardeMessage(Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);

            iWeb.SauvegardePion(Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);

            iWeb.SauvegardeMeteo(Donnees.m_donnees.TAB_JEU[0].ID_JEU);

            iWeb.SauvegardeModelesMouvement(Donnees.m_donnees.TAB_JEU[0].ID_JEU);

            iWeb.SauvegardeModelesPion(Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);

            iWeb.SauvegardeRole(Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);

            iWeb.SauvegardeForum(Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);

            iWeb.SauvegardeObjectifs(Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);

            iWeb.SauvegardeBataille(Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);

            iWeb.SauvegardeNation(Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);

            iWeb.SauvegardePartie(Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);

            iWeb.TraitementEnCours(false, Donnees.m_donnees.TAB_JEU[0].ID_JEU, Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);
            return true;
        }

        /// <summary>
        /// Création aléatoire d'unités et d'ordres pour faire des tests de performances sans avoir à envoyer un xml web
        /// </summary>
        internal void TestCreation()
        {
            //Ajout de phrases pour tous les cas pour éviter des crash
            TestCreationPhrases();
            //Création de cent unités dans chaque camp
            TestCreationUnite(0, 10);
            //TestCreationUnite(1, 10); -> y'a qu'un camp :-)
            //Ajout de 100 ordres
            TestCreationOrdres(); // en commentaire pour tester le placement statique
        }

        private void TestCreationPhrases()
        {
            foreach (ClassMessager.MESSAGES tipeMessage in Enum.GetValues(typeof(ClassMessager.MESSAGES)))
            {
                Donnees.m_donnees.TAB_PHRASE.AddTAB_PHRASERow((int)tipeMessage, "Phrase pour " + tipeMessage.ToString());
            }
        }

        /// <summary>
        /// Creation d'ordres fictifs pour faire des tests de performances
        /// </summary>
        private void TestCreationOrdres()
        {
            Random hasard = new Random();
            foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            {
                int heureDebut = 0; // hasard.Next(12);
                //Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[hasard.Next(Donnees.m_donnees.TAB_PION.Count())];
                Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                        Constantes.NULLENTIER,//ID_ORDRE_TRANSMIS
                        Constantes.NULLENTIER,//ID_ORDRE_SUIVANT global::System.Convert.DBNull,
                        Constantes.NULLENTIER,///ID_ORDRE_WEB
                        Constantes.ORDRES.MOUVEMENT,
                        lignePion.ID_PION,
                        lignePion.ID_CASE,
                        lignePion.I_INFANTERIE + lignePion.I_CAVALERIE + lignePion.I_ARTILLERIE,
                        Donnees.m_donnees.TAB_CASE[hasard.Next(Donnees.m_donnees.TAB_CASE.Count())].ID_CASE,//ID_CASE_DESTINATION
                        Constantes.NULLENTIER,//id ville de destination
                        0,//I_EFFECTIF_DESTINATION
                        Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,//I_TOUR_DEBUT
                        Donnees.m_donnees.TAB_PARTIE[0].I_PHASE,//I_PHASE_DEBUT
                        Constantes.NULLENTIER,//I_TOUR_FIN
                        Constantes.NULLENTIER,//I_PHASE_FIN
                        Constantes.NULLENTIER,//ID_MESSAGE
                        Constantes.NULLENTIER,//ID_DESTINATAIRE
                        Constantes.NULLENTIER,//ID_CIBLE
                        Constantes.NULLENTIER,//ID_DESTINATAIRE_CIBLE
                        Constantes.NULLENTIER,//ID_BATAILLE
                        Constantes.NULLENTIER,//I_ZONE_BATAILLE
                        heureDebut, //Donnees.m_donnees.TAB_JEU[0].I_LEVER_DU_SOLEIL,//I_HEURE_DEBUT
                        heureDebut + hasard.Next(12), //Donnees.m_donnees.TAB_JEU[0].I_COUCHER_DU_SOLEIL - Donnees.m_donnees.TAB_JEU[0].I_LEVER_DU_SOLEIL,//I_DUREE
                        Constantes.NULLENTIER//I_ENGAGEMENT
                        );//ID_BATAILLE
            }
        }

        /// <summary>
        /// Creation d'unitées fictives pour faire des tests de performances
        /// </summary>
        /// <param name="idNation">nation de l'unité</param>
        /// <param name="nbUnites">nombre d'unités</param>
        /// <summary>

        /// Creation d'unitées fictives pour faire des tests de performances

        /// </summary>

        /// <param name="idNation">nation de l'unité</param>

        /// <param name="nbUnites">nombre d'unités</param>

        private void TestCreationUnite(int idNation, int nbUnites)
        {
            Random hasard = new Random();
            for (int i = 0; i < nbUnites; i++)
            {
                string requete;
                Donnees.TAB_MODELE_PIONRow[] resModelePion;
                int iInfanterie = 0, iCavalerie = 0, iArtillerie = 0;
                char niveauDepot = 'Z';
                int idModelePion;

                switch (i % 6)
                {
                    case 0:
                        //unité d'infanterie
                        iInfanterie = 1000 + hasard.Next(10) * 1000;
                        requete = string.Format("ID_NATION={0} AND S_NOM='Division'", idNation);
                        resModelePion = (Donnees.TAB_MODELE_PIONRow[])Donnees.m_donnees.TAB_MODELE_PION.Select(requete);
                        break;
                    case 1:
                        //unité de cavalerie
                        iCavalerie = 500 + hasard.Next(10) * 500;
                        requete = string.Format("ID_NATION={0} AND S_NOM='Division'", idNation);
                        resModelePion = (Donnees.TAB_MODELE_PIONRow[])Donnees.m_donnees.TAB_MODELE_PION.Select(requete);
                        break;
                    case 2:
                        niveauDepot = 'D';
                        requete = string.Format("ID_NATION={0} AND S_NOM='Convoi'", idNation);
                        resModelePion = (Donnees.TAB_MODELE_PIONRow[])Donnees.m_donnees.TAB_MODELE_PION.Select(requete);
                        break;
                    case 3:
                        niveauDepot = 'B';
                        requete = string.Format("ID_NATION={0} AND S_NOM='Depot'", idNation);
                        resModelePion = (Donnees.TAB_MODELE_PIONRow[])Donnees.m_donnees.TAB_MODELE_PION.Select(requete);
                        break;
                    case 4:
                        //unité d'artillerie
                        iArtillerie = hasard.Next(10);
                        requete = string.Format("ID_NATION={0} AND S_NOM='Division'", idNation);
                        resModelePion = (Donnees.TAB_MODELE_PIONRow[])Donnees.m_donnees.TAB_MODELE_PION.Select(requete);
                        break;
                    default:
                        //unité mixte
                        iInfanterie = 1000 + hasard.Next(10) * 1000;
                        iCavalerie = 500 + hasard.Next(10) * 500;
                        iArtillerie = hasard.Next(10);
                        requete = string.Format("ID_NATION={0} AND S_NOM='Division'", idNation);
                        resModelePion = (Donnees.TAB_MODELE_PIONRow[])Donnees.m_donnees.TAB_MODELE_PION.Select(requete);
                        break;
                }
                idModelePion = resModelePion[0].ID_MODELE_PION;
                int idCase;
                //idCase = Donnees.m_donnees.TAB_CASE[hasard.Next(Donnees.m_donnees.TAB_CASE.Count())].ID_CASE;
                //Note : il ne faut pas qu'une unité commence sur un bord de carte ou cela fait planter l'algorithme de recherche
                idCase = Donnees.m_donnees.TAB_CASE.FindByXY((i + 1) * 30, (i + 1) * 30).ID_CASE;
                Monitor.Enter(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                Donnees.TAB_PIONRow ligneNouveauPion = Donnees.m_donnees.TAB_PION.AddTAB_PIONRow(
                    idModelePion,
                    -1,//ID_PION_PROPRIETAIRE
                    -1,
                    -1,
                    string.Format("Test {0} n°{1}", idNation, i),
                    iInfanterie, iInfanterie,
                    iCavalerie, iCavalerie,
                    iArtillerie, iArtillerie, 0, 
                    100, //I_MORAL : le moral doit être à 100, sinon il va ajouter +1km/h à la vitesse pour déroute
                    100, 0, 0, 0, 'Z', 0, 0, 0, 0,
                    idCase,
                    0, //I_TOUR_SANS_RAVITAILLEMENT
                    0, -1,
                    0, //I_TOUR_RETRAITE_RESTANT
                    0, false, false, false, false, false,
                    false,//B_ENNEMI_OBSERVABLE
                    0,//I_MATERIEL,
                    0,//I_RAVITAILLEMENT,
                    false,//B_CAVALERIE_DE_LIGNE,
                    false,//B_CAVALERIE_LOURDE,
                    false,//B_GARDE,
                    false,//B_VIEILLE_GARDE,
                    0,//I_TOUR_CONVOI_CREE,
                    -1,//ID_DEPOT_SOURCE
                    0,//I_SOLDATS_RAVITAILLES,
                    0,//I_NB_HEURES_FORTIFICATION,
                    0,//I_NIVEAU_FORTIFICATION,
                    0,//ID_PION_REMPLACE,
                    0,//I_DUREE_HORS_COMBAT,
                    0,//I_TOUR_BLESSURE,
                    false,//B_BLESSES,
                    false,//B_PRISONNIERS,
                    false,//B_RENFORT
                    -1,//ID_LIEU_RATTACHEMENT,
                    niveauDepot,//C_NIVEAU_DEPOT
                    -1,//ID_PION_ESCORTE,
                    0,//I_INFANTERIE_ESCORTE,
                    0,//I_CAVALERIE_ESCORTE
                    0,//I_MATERIEL_ESCORTE
                    0,//I_TOUR_DERNIER_RAVITAILLEMENT_DIRECT
                    1//I_VICTOIRE
                    );
                ligneNouveauPion.SetID_ANCIEN_PION_PROPRIETAIRENull();
                ligneNouveauPion.SetID_NOUVEAU_PION_PROPRIETAIRENull();
                ligneNouveauPion.SetI_ZONE_BATAILLENull();
                ligneNouveauPion.SetID_BATAILLENull();
                ligneNouveauPion.SetID_LIEU_RATTACHEMENTNull();
                ligneNouveauPion.SetID_PION_ESCORTENull();
                ligneNouveauPion.SetID_DEPOT_SOURCENull();
                ligneNouveauPion.ID_PION_PROPRIETAIRE = 0; //Le propriéaire final doit forcément appartenir à un joueur qui doit être crée comme id=0 ligneNouveauPion.ID_PION;
                ligneNouveauPion.S_NOM = string.Format("Test {0} n°{1} p:{2}", idNation, ligneNouveauPion.ID_PION, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE);
                Monitor.Exit(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
            }
        }
   }
}
