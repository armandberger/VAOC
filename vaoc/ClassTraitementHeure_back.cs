 using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Diagnostics;
using System.Linq;

using WaocLib;

namespace vaoc
{
    class rechercheQG
    {
        public int idPion;
        public bool bEngagee;
    }

    class ClassTraitementHeure
    {
        #region Constantes
        const int CST_PERTE_MORAL_FUITE = 10;
        const int CST_GAIN_MORAL_MAITRE_TERRAIN = 5;
        const int CST_GAIN_FATIGUE_REPOS = 20;
        const int CST_GAIN_MORAL_REPOS = 10;
        const int CST_BRUIT_DU_CANON = 30;//distance en km o� l'on entend le bruit d'une bataille
        const int CST_RECHERCHE_ZONE_VICTOIRE = 1;//rayon en km sur laquelle on recherche le contr�le autour d'un point ayant une valeur
        const int CST_PERTE_MORAL_MAX_SS_RAVITAILLEMENT = 1;//valeur maximale de moral qu'une unit� peut perdre par heure sans ravitaillement
        const int CST_DISTANCE_MAX_RAVITAILLEMENT = 200;//distance "effective" en km au dela de laquelle le ravitaillement n'est plus assur�
        const int CST_DISTANCE_MAX_RECHERCHE_DESTINATAIRE = 30; // Distance "effective" en km au dela de laquelle un messager ne d�livre pas son message si le destinataire n'est pas pr�sent
        #endregion

        protected LogFile m_log = null;
        protected AStarVille m_etoile;
        protected InterfaceVaocWeb m_iWeb;

        public ClassTraitementHeure()
        {
            m_etoile = new AStarVille();
        }

        public bool TraitementHeure(string fichierCourant, System.ComponentModel.BackgroundWorker travailleur, out string messageErreur)
        {
            string message;
            int nbPhases;
            int i;

            Debug.WriteLine("TraitementHeure");
            messageErreur = "";

            LogFile.CreationLogFile(fichierCourant, "tour", Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, -1);
            m_iWeb = ClassVaocWebFactory.CreerVaocWeb(fichierCourant, false);

            //on va rechercher les nouveaux ordres ou les nouveaux messages (quand la partie n'est pas commence)
            if (!NouveauxMessages()) { LogFile.Notifier("Erreur rencontr�e dans NouveauxMessages()"); return false; }
            if (Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE)
            {
                if (!NouveauxOrdres()) { LogFile.Notifier("Erreur rencontr�e dans NouveauxOrdres()"); return false; }
            }


            //pour la toute premi�re phase on envoit un message d'initialisation des positions pour les pions des joueurs
            if (0 == Donnees.m_donnees.TAB_MESSAGE.Count)
            //if (0 == DataSetCoutDonnees.m_donnees.TAB_PARTIE[0].I_PHASE && 0 == DataSetCoutDonnees.m_donnees.TAB_PARTIE[0].I_TOUR)
            {
                //on initialise �galement la m�t�o
                Donnees.m_donnees.TAB_PARTIE[0].ID_METEO = Donnees.m_donnees.TAB_PARTIE[0].ID_METEO_INITIALE;
                //on envoit un message avec effet imm�diat pour pr�venir le joueur de l'arriv�e de la troupe
                foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
                {
                    if (lignePion.B_DETRUIT) { continue; }//plutot curieux une unit� d�truite en d�but de partie mais...
                    lignePion.B_TELEPORTATION = true;//le pion arrive sur la carte
                }
            }

            //Teleportation, pions nouvellement arriv�s ou modification de la position par l'arbitre (pour le placement initial par exemple)
            foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            {
                if (lignePion.B_DETRUIT || !lignePion.B_TELEPORTATION) { continue; }
                if (lignePion.estMessager || lignePion.estPatrouille)
                {
                    lignePion.B_TELEPORTATION = false;//pas de message pour ce type de pion
                    continue;
                }
                if (!ClassMessager.EnvoyerMessageImmediat(lignePion, ClassMessager.MESSAGES.MESSAGE_RENFORT))
                {
                    throw new NotImplementedException("Il FAUT param�trer un message de type RENFORT de mani�re � avoir un message de position pour toutes les unit�s d�s le d�part.");
                }
                Cartographie.DetruireEspacePion(lignePion);//force, par la suite, le recalcul de tous les espaces, parcours, etc.
                lignePion.B_TELEPORTATION = false;
            }

            if (0 == Donnees.m_donnees.TAB_PARTIE.HeureCourante())
            {
                //premi�re heure de la journ�e
                if (!NouveauJour())
                {
                    return false;
                }
            }

            //pour chaque phase
            nbPhases = (Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE) ? Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES : Donnees.m_donnees.TAB_PARTIE[0].I_PHASE+1;
            //DataSetCoutDonnees.m_donnees.TAB_PARTIE[0].I_PHASE = 90;
            while (Donnees.m_donnees.TAB_PARTIE[0].I_PHASE < nbPhases)
            {
                //Initialisation de la phase
                LogFile.CreationLogFile(fichierCourant, "tour", Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE);
                message = string.Format("TraitementHeure : d�but phase={0}", Donnees.m_donnees.TAB_PARTIE[0].I_PHASE);
                if (!LogFile.Notifier(message, out messageErreur))
                {
                    return false;
                }

                //construction de la carte pour les images li�s aux messages
                Cartographie.fichierLog = m_log;
                if (!Cartographie.ChargerLesFichiers()) { return false; }

                //on reconstruit la carte graphique � chaque �tape pour voir la progression � l'�cran
                //ca ne marche pas, alors autant gagner du temps
                //Cartographie.ConstructionCarte();
                //Cartographie.AfficherUnites(Cartographie.modeleCarte.HISTORIQUE);
                //Cartographie.AfficherUnites(Cartographie.modeleCarte.ZOOM);

                //Traitement de la phase
                if (0 == Donnees.m_donnees.TAB_PARTIE[0].I_PHASE ||
                    false==Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE)
                {
                    if (!NouvelleHeure(Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE)) { return false; }
                }

                //on place toutes les unit�s statiques
                message = string.Format("**** Placement des unit�s statiques ****");
                if (!LogFile.Notifier(message, out messageErreur)) { return false;}

                if (!Cartographie.PlacerLesUnitesStatiques())
                {
                    messageErreur = "Erreur durant le traitement ExecuterMouvement";
                    return false;
                }

                //on execute les mouvements
                //toutes les unit�s avec effectifs (les unit�s de combat en fait)
                message = string.Format("**** Mouvements : toutes les unit�s AVEC effectifs ****");
                if (!LogFile.Notifier(message, out messageErreur))
                {
                    return false;
                }

                i = 0;
                while (i<Donnees.m_donnees.TAB_PION.Count())
                {
                    Donnees.TAB_PIONRow lignePion=Donnees.m_donnees.TAB_PION[i];
                    if (lignePion.B_DETRUIT) { ++i;  continue; }
                    if (lignePion.I_ARTILLERIE > 0 || lignePion.I_CAVALERIE > 0 || lignePion.I_INFANTERIE > 0)
                    {
                        if (!ExecuterMouvement(lignePion, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE))
                        {
                            messageErreur = "Erreur durant le traitement ExecuterMouvement";
                            return false;
                        }
                    }
                    ++i;
                }

                //toutes les unit�s sans effectif (QG, messagers)
                message = string.Format("**** Mouvements : toutes les unit�s SANS effectif ****");
                if (!LogFile.Notifier(message, out messageErreur))
                {
                    return false;
                }

                i = 0;
                while (i<Donnees.m_donnees.TAB_PION.Count())
                {
                    Donnees.TAB_PIONRow lignePion=Donnees.m_donnees.TAB_PION[i];
                    if (lignePion.B_DETRUIT) { i++;  continue; }
                    if (lignePion.I_ARTILLERIE == 0 && lignePion.I_CAVALERIE == 0 && lignePion.I_INFANTERIE == 0)
                    {
                        if (!ExecuterMouvement(lignePion, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE))
                        {
                            messageErreur = "Erreur durant le traitement ExecuterMouvement";
                            return false;
                        }
                    }
                    i++;
                }

                LogFile.Notifier("Mise � jour des propri�taires des cases");
                Cartographie.MiseAJourProprietaires();
                if (0 == Donnees.m_donnees.TAB_PARTIE[0].I_PHASE % 10)
                {
                    SauvegarderPartie(fichierCourant);
                }

                Donnees.m_donnees.TAB_PARTIE[0].I_PHASE++;
                travailleur.ReportProgress(0);//toujours zero durant le traitement, car affichage bas� differement d'un pourcentage
                LogFile.Notifier("Fin de la phase");
            }

            /************ A la fin de l'heure ****************/
            //on regarde toutes les unit�s qui participent � un combat
            foreach (Donnees.TAB_BATAILLERow ligneBataille in Donnees.m_donnees.TAB_BATAILLE)
            {
                if (!EffectuerBataille(ligneBataille))
                {
                    messageErreur = "Erreur durant le traitement EffectuerBataille";
                    return false;
                }
            }

            // On regarde si toutes les unit�s sont bien ravitaill�es
            //pas � faire toutes les heures, mais seulement en fin de journ�e et avec une liaison sur les d�p�ts que si l'unit� n'a pas boug�e
            //cela est donc effectu� dans FinDuJour
            //if (!Ravitaillement())
            //{
            //    messageErreur = "Erreur durant le traitement Ravitaillement";
            //    return false;
            //}

            //reconstruction de la carte suite aux batailles �ventuelles, pour l'affichage g�n�rale
            message = string.Format("**** Reconstruction carte ****");
            if (!LogFile.Notifier(message, out messageErreur)) {return false;}
            Cartographie.fichierLog = m_log;
            if (!Cartographie.ConstructionCarte())
            {
                messageErreur = "Erreur durant le traitement ConstructionCarte";
                return false;
            }

            //a la fin de la journ�e, on met � jour le moral, la fatigue et on envoit un rapport
            if (23 == Donnees.m_donnees.TAB_PARTIE.HeureCourante())
            {
                if (!FinDuJour())
                {
                    messageErreur = "Erreur durant le traitement FinDuJour";
                    return false;
                }
            }

            //On v�rifie si la partie est termin�e et, si oui, on determine le gagnant
            if (!VictoireDefaite())
            {
                messageErreur = "Erreur durant le traitement VictoireDefaite";
                return false;
            }

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
                //avant le demmarage, le temps avance d'une phase � chaque �change
                //ce qui est trait� dans la boucle while initiale
            }

            //suppression des tables d'optimisation
            //Cartographie.NettoyageBase();, bof, finalement, �a sert aussi pour g�n�rer les fichiers web ensuite alors autant les garder

            //derni�re sauvegarde pour demarrer au tour suivant
            SauvegarderPartie(fichierCourant);
            travailleur.ReportProgress(100);//c'est fini !
            return true;
        }

        /// <summary>
        /// Effectue le ravitaillement d'une unit�
        /// </summary>
        /// <param name="lignePion">Unit� � ravitailler</param>
        /// <param name="bUniteRavitaillee">true si l'unit� � �t� ravitaill�e, false sinon</param>
        /// <returns>true si OK, false si KO</returns>
        private bool RavitaillementUnite(Donnees.TAB_PIONRow lignePion, out bool bUniteRavitaillee, out int distanceRavitaillement, out string depotRavitaillement)
        {
            string message, messageErreur;
            List<Donnees.TAB_CASERow> chemin;
            AstarTerrain[] tableCoutsMouvementsTerrain;
            double cout, coutHorsRoute, meilleurcout;

            distanceRavitaillement =-1;
            depotRavitaillement = string.Empty;

            if (!lignePion.estRavitaillable) { bUniteRavitaillee = true; return true; } //seules les unit�s combattantes doivent �tre ravitaill�es

            #region Recherche le depot le plus proche de l'unite
            bUniteRavitaillee = false;
            distanceRavitaillement = int.MaxValue;
            depotRavitaillement = string.Empty;
            meilleurcout = double.MaxValue;

            foreach (Donnees.TAB_PIONRow ligneDepot in Donnees.m_donnees.TAB_PION)
            {
                if (!ligneDepot.estDepot || ligneDepot.estEnnemi(lignePion)) { continue; } //on ne ravitaille que ses copains
                Donnees.TAB_CASERow ligneCaseDepart = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePion.ID_CASE);
                Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneDepot.ID_CASE);

                if (!Cartographie.RechercheChemin(Cartographie.typeParcours.RAVITAILLEMENT, lignePion, ligneCaseDepart, ligneCaseDestination, null, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                {
                    message = string.Format("{0}(ID={1}, erreur sur RechercheChemin dans Ravitaillement :{2})", lignePion.S_NOM, lignePion.ID_PION, messageErreur);
                    LogFile.Notifier(message);
                    return false;
                }
                if (chemin.Count > 0 && cout < meilleurcout)
                {
                    distanceRavitaillement = chemin.Count / Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;//distance en km
                    meilleurcout = cout;
                    depotRavitaillement = ligneDepot.S_NOM;
                }
            }
            #endregion

            //si l'on ne trouve pas de dep�t ou si la distance de ravitaillement est sup�rieur � un �quivalent de 150km (calcul� sur la base du cout standard plaine), alors l'unit� n'est pas ravitaill�.
            if (depotRavitaillement != string.Empty)
            {
                //Calcul de la distance en km relative � l'effort fait pour le trajet. On divise deux fois par l'�chelle, car le cout renvoy� est le nomnre de pixel par case(echelle) * cout de la case
                //diviser ce cout par l'echelle et le cout de base renvoit donc la valeur en "pixels", pas en kilometre
                long distanceCoutRavitaillement = (long)(meilleurcout / (Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * Donnees.m_donnees.TAB_JEU[0].I_COUT_DE_BASE));

                if (distanceCoutRavitaillement > CST_DISTANCE_MAX_RAVITAILLEMENT)
                {
                    //c'est trop loin, pas de ravitaillement
                    message = string.Format("{0}(ID={1}, Ravitaillement trop �loign�, distance 'effective' du d�p�t = {2} km)", lignePion.S_NOM, lignePion.ID_PION, distanceCoutRavitaillement);
                    LogFile.Notifier(message);
                }
                else
                {
                    bUniteRavitaillee = true;
                    message = string.Format("{0}(ID={1}, Ravitaillement ok, distance 'effective' du d�p�t = {2} km)", lignePion.S_NOM, lignePion.ID_PION, distanceCoutRavitaillement);
                    LogFile.Notifier(message);
                }
            }

            return true;
        }

        /// <summary>
        /// Effectue le ravitaillement, de toutes les unit�s
        /// Relire, la r�gle
        /// </summary>
        /// <returns></returns>
        private bool Ravitaillement()
        {
            string message;
            int distanceRavitaillement,i;
            string depotRavitaillement;
            bool bUniteRavitaillee;

            Cartographie.InitialiserProprietairesTrajets();
            i = 0;
            while (i<Donnees.m_donnees.TAB_PION.Count)//on ne peut pas faire de foreach car l'envoi de messagers change la table
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                if (!lignePion.estRavitaillable) { ++i; continue; } //seules les unit�s combattantes doivent �tre ravitaill�es

                if (!RavitaillementUnite(lignePion, out bUniteRavitaillee, out distanceRavitaillement, out depotRavitaillement)) { return false; }

                if (!bUniteRavitaillee)
                {
                    //on envoit un message si c'est la premi�re fois que l'unit� est sans ravitaillement
                    if (lignePion.IsI_TOUR_SANS_RAVITAILLEMENTNull() || (0 == lignePion.I_TOUR_SANS_RAVITAILLEMENT))
                    {
                        lignePion.I_TOUR_SANS_RAVITAILLEMENT = 0;//au cas o� cela aurait value null auparavant
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_SANS_RAVITAILLEMENT))
                        {
                            message = string.Format("{0}(ID={1}, erreur sur EnvoyerMessage avec MESSAGE_SANS_RAVITAILLEMENT dans Ravitaillement", lignePion.S_NOM, lignePion.ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }
                    }

                    //si une unit� a un moral a z�ro et est plus de 24 tour sans ravitaillement celle-ci se rend
                    //ce cas n'est normalement pas pr�vu par les r�gles (dans les r�gles avanc�es, l'unit� s'�puise sans ravitaillement)
                    //donc r�gle fix� arbitrairement par BEA pour les r�gles de base
                    if (lignePion.I_TOUR_SANS_RAVITAILLEMENT > 0 && lignePion.I_MORAL<=0)
                    {
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_REDITION_POUR_RAVITAILLEMENT))
                        {
                            message = string.Format("{0}(ID={1}, erreur sur EnvoyerMessage avec MESSAGE_REDITION_POUR_RAVITAILLEMENT dans Ravitaillement", lignePion.S_NOM, lignePion.ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }
                        lignePion.B_REDITION_RAVITAILLEMENT = true;//compte dans les points de victoire
                        Cartographie.DetruirePion(lignePion);
                    }

                    //calcul de la perte de moral, stock�e dans I_TOUR_SANS_RAVITAILLEMENT, on ne perd du moral que le jour
                    if (!Donnees.m_donnees.TAB_PARTIE.Nocturne())
                    {
                        int pertemoral = (lignePion.I_MORAL < CST_PERTE_MORAL_MAX_SS_RAVITAILLEMENT) ? lignePion.I_MORAL : CST_PERTE_MORAL_MAX_SS_RAVITAILLEMENT;
                        lignePion.I_TOUR_SANS_RAVITAILLEMENT += pertemoral;
                        lignePion.I_MORAL -= pertemoral;
                    }
                    if (lignePion.I_TOUR_SANS_RAVITAILLEMENT<=0) 
                    {
                        lignePion.I_TOUR_SANS_RAVITAILLEMENT = 1;//au moins un pour que l'on sache qu'il y a d�j� un tour sans ravitaillement
                    }
                }
                else
                {
                    // on envoit un message si l'unit� �tait sans ravitaillement mais qu'elle en retrouve
                    if (!lignePion.IsI_TOUR_SANS_RAVITAILLEMENTNull() && lignePion.I_TOUR_SANS_RAVITAILLEMENT > 0)
                    {
                        lignePion.I_MORAL += lignePion.I_TOUR_SANS_RAVITAILLEMENT;//l'unit� regagne le moral perdu pour manque de ravitaillement
                        lignePion.I_TOUR_SANS_RAVITAILLEMENT = 0;
                        if (!ClassMessager.EnvoyerMessage(lignePion, distanceRavitaillement, depotRavitaillement, ClassMessager.MESSAGES.MESSAGE_NOUVEAU_RAVITAILLEMENT))
                        {
                            message = string.Format("{0}(ID={1}, erreur sur EnvoyerMessage avec MESSAGE_NOUVEAU_RAVITAILLEMENT dans Ravitaillement", lignePion.S_NOM, lignePion.ID_PION);
                            LogFile.Notifier(message);
                            return false;
                        }
                    }
                }
                ++i;
            }
            return true;
        }

        /// <summary>
        /// V�rifie si l'un des cas a gagn� ou si la partie est termin�e, dans ce cas, le vainqueur est d�termin�
        /// </summary>
        /// <returns>true si ok, false si ko</returns>
        private bool VictoireDefaite()
        {
            bool bFinDePartie = false;
            int[] victoire = new int[2];
            int[] defaite = new int[2];
            Donnees.TAB_MODELE_PIONRow ligneModelePion;

            try
            {
                //vient-on de jouer le dernier tour ?
                if (Donnees.m_donnees.TAB_PARTIE[0].I_TOUR >= Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_TOURS)
                {
                    //En effet c'est le cas
                    LogFile.Notifier("VictoireDefaite : fin de la partie, on a jou� le dernier tour.");
                    bFinDePartie = true;
                }

                if (!bFinDePartie)
                {
                    //si l'un des camps a deux fois plus de corps d�moralis�s que l'autre et que cela constitue au moins la moiti� de son arm�e, renforts compris.
                    //alors la partie s'arr�te
                    victoire[0] = victoire[1] = 0;
                    defaite[0] = defaite[1] = 0;
                    foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
                    {
                        if (lignePion.effectifTotal > 0 && !lignePion.B_DETRUIT)
                        {
                            //unit� combattante
                            ligneModelePion = Donnees.m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION(lignePion.ID_MODELE_PION);

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

                    foreach (Donnees.TAB_RENFORTRow ligneRenfort in Donnees.m_donnees.TAB_RENFORT)
                    {
                        if (ligneRenfort.effectifTotal > 0)
                        {
                            ligneModelePion = Donnees.m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION(ligneRenfort.ID_MODELE_PION);
                            victoire[ligneModelePion.ID_NATION]++;
                        }
                    }

                    LogFile.Notifier(string.Format("VictoireDefaite : defaite[0]={0}, defaite[1]={1}, victoire[0]={2}, victoire[1]={3}",
                                                    defaite[0], defaite[1], victoire[0], victoire[1]));
                    if ((defaite[0] > defaite[1] * 2 && defaite[0] > victoire[0]) || (defaite[1] > defaite[0] * 2 && defaite[1] > victoire[1]))
                    {
                        LogFile.Notifier("VictoireDefaite : fin de la partie, l'un des deux camps est d�finitivement battu.");
                        bFinDePartie = true;
                    }
                }
                if (bFinDePartie)
                {
                    //il faut maintenant determiner le vainqueur en ajoutant les points des zones controll�es 
                    foreach (Donnees.TAB_NOMS_CARTERow ligneNomCarte in Donnees.m_donnees.TAB_NOMS_CARTE)
                    {
                        if (ligneNomCarte.I_VICTOIRE > 0 && !ligneNomCarte.IsID_NATION_CONTROLENull())
                        {
                            //pts de victoire = corps defaits + zones control�es, donc controller une zone, �quivaut � ajouter les pts chez les corps d�faits de l'adversaire
                            if (0 == ligneNomCarte.ID_NATION_CONTROLE)
                                defaite[1] += ligneNomCarte.I_VICTOIRE;
                            else
                                defaite[0] += ligneNomCarte.I_VICTOIRE;
                        }
                    }
                    Donnees.m_donnees.TAB_PARTIE[0].ID_VICTOIRE = 3;//egalite
                    if (defaite[0] > defaite[1])
                    {
                        Donnees.m_donnees.TAB_PARTIE[0].ID_VICTOIRE = 0;
                    }
                    else
                    {
                        Donnees.m_donnees.TAB_PARTIE[0].ID_VICTOIRE = 1;
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
        /// premi�re phase du tour
        /// </summary>
        /// <returns>OK=true, KO=false</returns>
        private bool NouvelleHeure(bool bPartieCommence)
        {
            string message, messageErreur;

            LogFile.Notifier("Debut nouvelle heure");
            #region R�duction des tours de fuite
            message = string.Format("**** R�duction des tours de fuite ****");
            if (!LogFile.Notifier(message, out messageErreur))
            {
                return false;
            }

            foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            {
                if (lignePion.B_DETRUIT) { continue; }
                if (lignePion.I_TOUR_FUITE_RESTANT > 0)
                {
                    lignePion.I_TOUR_FUITE_RESTANT--;
                    if (0 == lignePion.I_TOUR_FUITE_RESTANT)
                    {
                        //l'unit� n'est plus li�e au pr�c�dent combat
                        lignePion.SetID_BATAILLENull();
                        lignePion.SetI_ZONE_BATAILLENull();
                    }
                }
            }
            #endregion

            #region arriv�e des renforts
            foreach (Donnees.TAB_RENFORTRow ligneRenfort in Donnees.m_donnees.TAB_RENFORT)
            {
                if (ligneRenfort.I_TOUR_ARRIVEE == Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)
                {
                    //on v�rifie que l'on a pas d�j� ajout� le pion (peut arriver si l'on a du relancer l'execution)
                    if (null==Donnees.m_donnees.TAB_PION.FindByID_PION(ligneRenfort.ID_PION))
                    {
                        Donnees.TAB_PIONRow lignePionRenfort = Donnees.m_donnees.TAB_PION.AddTAB_PIONRow(
                            ligneRenfort.ID_MODELE_PION,
                            ligneRenfort.ID_PION_PROPRIETAIRE,
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
                            0, //int I_TOUR_SANS_RAVITAILLEMENT, null ensuite
                            -1,//int ID_BATAILLE, null ensuite
                            -1,//int I_ZONE_BATAILLE, null ensuite
                            0,//int I_TOUR_FUITE_RESTANT
                            false,//bool B_DETRUIT
                            false,//B_FUITE_AU_COMBAT
                            false,//bool B_INTERCEPTION
                            false,//B_FUITE_AU_COMBAT
                            true //B_TELEPORTATION
                            );

                        lignePionRenfort.SetI_TOUR_SANS_RAVITAILLEMENTNull();
                        lignePionRenfort.SetI_ZONE_BATAILLENull();
                        lignePionRenfort.SetID_BATAILLENull();

                        //il faut imposer l'id_pion du renfort
                        lignePionRenfort.ID_PION = ligneRenfort.ID_PION;

                        //Les leaders indiquent leurs positions respectives dans les deux sens
                        if (lignePionRenfort.estJoueur)
                        {
                            //on envoit un message avec effet imm�diat pour pr�venir le joueur de l'arriv�e de la troupe
                            if (!ClassMessager.EnvoyerMessageImmediat(lignePionRenfort, ClassMessager.MESSAGES.MESSAGE_RENFORT))
                            {
                                return false;
                            }

                            foreach (Donnees.TAB_ROLERow ligneRole in Donnees.m_donnees.TAB_ROLE)
                            {
                                Donnees.TAB_PIONRow lignePionRole = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneRole.ID_PION);

                                if (null != lignePionRole && !lignePionRole.estEnnemi(lignePionRenfort) && (lignePionRole.ID_PION != lignePionRenfort.ID_PION))
                                {
                                    //message imm�diat � tous les autres r�les amis pour pr�venir de l'arriv�e de ce nouveau leader
                                    string phrase;
                                    phrase = ClassMessager.GenererPhrase(lignePionRenfort, ClassMessager.MESSAGES.MESSAGE_POSITION, 0, 0, 0, 0, 0, null, lignePionRole, 0, string.Empty);
                                    if (!ClassMessager.EnvoyerMessage(lignePionRenfort, lignePionRole, ClassMessager.MESSAGES.MESSAGE_POSITION, phrase,true))
                                    {
                                        return false;
                                    }

                                    //message de tous les autres r�les pour pr�venir de leur position � ce nouveau leader
                                    phrase = ClassMessager.GenererPhrase(lignePionRole, ClassMessager.MESSAGES.MESSAGE_POSITION, 0, 0, 0, 0, 0, null, lignePionRenfort, 0, string.Empty);
                                    if (!ClassMessager.EnvoyerMessage(lignePionRole, lignePionRenfort, ClassMessager.MESSAGES.MESSAGE_POSITION, phrase, true))
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            //on envoit un message avec effet imm�diat pour pr�venir le joueur de l'arriv�e de la troupe
                            if (!ClassMessager.EnvoyerMessageImmediat(lignePionRenfort, ClassMessager.MESSAGES.MESSAGE_RENFORT))
                            {
                                return false;
                            }

                        }
                    }
                }
            }
            #endregion

            #region Contr�le des villes avec points de victoire
            int idNation0 = -1, idNation1 = -1;
            int[] zone = new int[2];
            foreach (Donnees.TAB_NOMS_CARTERow ligneNomCarte in Donnees.m_donnees.TAB_NOMS_CARTE)
            {
                Donnees.TAB_PIONRow lignePion;
                Donnees.TAB_MODELE_PIONRow ligneModelePion;
                if (ligneNomCarte.I_VICTOIRE > 0)
                {
                    Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneNomCarte.ID_CASE);
                    //on regarde qui contr�le la zone � un kilom�tre alentour
                    #region calcul du cadre de recherche
                    int xCaseHautGauche = ligneCase.I_X - CST_RECHERCHE_ZONE_VICTOIRE * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                    int yCaseHautGauche = ligneCase.I_Y - CST_RECHERCHE_ZONE_VICTOIRE * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                    int xCaseBasDroite = ligneCase.I_X + CST_RECHERCHE_ZONE_VICTOIRE * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                    int yCaseBasDroite = ligneCase.I_Y + CST_RECHERCHE_ZONE_VICTOIRE * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE;
                    xCaseHautGauche = Math.Max(0, xCaseHautGauche);
                    yCaseHautGauche = Math.Max(0, yCaseHautGauche);
                    xCaseBasDroite = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_LARGEUR_CARTE - 1, xCaseBasDroite);
                    yCaseBasDroite = Math.Min(Donnees.m_donnees.TAB_JEU[0].I_HAUTEUR_CARTE - 1, yCaseBasDroite);
                    #endregion

                    string requete = string.Format("I_X>={0} AND I_Y>={1} AND I_X<{2} AND I_Y<{3}", 
                        xCaseHautGauche, yCaseHautGauche, xCaseBasDroite, yCaseBasDroite);
                    Donnees.TAB_CASERow[] lignesCaseRecherche = (Donnees.TAB_CASERow[])Donnees.m_donnees.TAB_CASE.Select(requete);

                    zone[0] = zone[1] = 0;
                    foreach (Donnees.TAB_CASERow ligneCaseRecherche in lignesCaseRecherche)
                    {
                        if (!ligneCaseRecherche.EstInnocupe()) //.ID_PROPRIETAIRE. != DataSetCoutDonnees.CST_AUCUNPROPRIETAIRE)
                        {
                            lignePion = ligneCaseRecherche.TrouvePionSurCase();
                            if (null == lignePion)
                            {
                                string proprio = ligneCaseRecherche.IsID_PROPRIETAIRENull() ? "null" : ligneCaseRecherche.ID_PROPRIETAIRE.ToString();
                                string nouveauProprio = ligneCaseRecherche.IsID_NOUVEAU_PROPRIETAIRENull() ? "null" : ligneCaseRecherche.ID_NOUVEAU_PROPRIETAIRE.ToString();
                                message = string.Format("NouvelleHeure : erreur FindByID_PION introuvable sur {0} ou {1}", proprio, nouveauProprio);
                                LogFile.Notifier(message, out messageErreur);
                                return false;
                            }

                            ligneModelePion = Donnees.m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION(lignePion.ID_MODELE_PION);
                            if (null == ligneModelePion)
                            {
                                message = string.Format("NouvelleHeure : erreur FindByID_MODELE_PION introuvable sur {0}", lignePion.ID_MODELE_PION);
                                LogFile.Notifier(message, out messageErreur);
                                return false;
                            }

                            //un point de plus sur la zone pour son camp !
                            if (idNation0 == -1 || idNation0 == ligneModelePion.ID_NATION)
                            {
                                idNation0 = ligneModelePion.ID_NATION;
                                zone[0]++;
                            }
                            else
                            {
                                idNation1 = ligneModelePion.ID_NATION;
                                zone[1]++;
                            }
                        }
                    }
                    // alors, qui contr�le la zone ?
                    if (zone[0] > zone[1])
                    {
                        ligneNomCarte.ID_NATION_CONTROLE = idNation0;
                    }
                    if (zone[1] > zone[0])
                    {
                        ligneNomCarte.ID_NATION_CONTROLE = idNation1;
                    }
                }
            }
            #endregion

            LogFile.Notifier("Fin nouvelle heure");

            return true;
        }

        /// <summary>
        /// Mise en place des nouveaux messages saisies par les joueurs en mode forum (partie non commenc�e)
        /// </summary>
        /// <returns>OK=true, KO=false</returns>
        private bool NouveauxMessages()
        {
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

            List<ClassDataMessage> liste = m_iWeb.ListeMessages(Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);
            foreach (ClassDataMessage message in liste)
            {
                Donnees.TAB_PIONRow lignePionEmetteur = Donnees.m_donnees.TAB_PION.FindByID_PION(message.ID_EMETTEUR);
                int tourDepart, phaseDepart, tourArrivee, phaseArrivee;
                if (null==lignePionEmetteur) return false;

                int iTourSansRavitaillement = lignePionEmetteur.IsI_TOUR_SANS_RAVITAILLEMENTNull() ? 0 : lignePionEmetteur.I_TOUR_SANS_RAVITAILLEMENT;
                ClassMessager.TourPhase(message.DT_DEPART, out tourDepart, out phaseDepart);
                ClassMessager.TourPhase(message.DT_ARRIVEE, out tourArrivee, out phaseArrivee);
                //if (message.ID_MESSAGE > ligneMessageMax[0].ID_MESSAGE), non fiable, m�me apr�s un clear, le compteur ID ne repart pas � z�ro
                if (tourDepart>=tourMax && phaseDepart>phaseMax)
                {
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
                        lignePionEmetteur.I_TOUR_FUITE_RESTANT,
                        lignePionEmetteur.B_DETRUIT,
                        lignePionEmetteur.ID_CASE);
                }
            }
            return true;
        }

        /// <summary>
        /// Mise en place des nouveaux ordres saisies par les joueurs
        /// </summary>
        /// <param name="bPartieCommence">true si la partie � commenc�, false si on est en pr�-discussion</param>
        /// <returns>OK=true, KO=false</returns>
        private bool NouveauxOrdres()
        {
            //string message, messageErreur;
            int id_case_destination;
            ClassMessager.COMPAS compas;
            Donnees.TAB_ORDRERow ligneOrdreWeb;

            LogFile.Notifier("Debut NouveauxOrdres");
            //DataSetCoutDonnees.m_donnees.TAB_ORDRE.Clear();//� virer ensuite

            List<ClassDataOrdre> liste = m_iWeb.ListeOrdres(Donnees.m_donnees.TAB_PARTIE[0].ID_JEU,
                                                            Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);
            foreach (ClassDataOrdre ordre in liste)
            {
                //si l'ordre a bien �t� envoy� � ce tour et n'est pas d�j� inclus dans la table, bref s'il s'agit d'un nouvel
                //ordre saisie par un joueur, on l'ajoute
                ligneOrdreWeb = Donnees.m_donnees.TAB_ORDRE.OrdreWeb(ordre.ID_ORDRE);
                if ((Donnees.m_donnees.TAB_PARTIE[0].I_TOUR == ordre.I_TOUR) && null == ligneOrdreWeb)
                {
                    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ordre.ID_PION);
                    Donnees.TAB_PIONRow lignePionDestination = Donnees.m_donnees.TAB_PION.FindByID_PION(ordre.ID_PION_DESTINATAIRE);

                    if (ordre.I_TYPE == Donnees.CST_ORDRE_MESSAGE)
                    {
                        //if (bPartieCommence)
                        {
                            //il faut envoyer un message
                            if (!ClassMessager.EnvoyerMessage(lignePion, lignePionDestination, ClassMessager.MESSAGES.MESSAGE_PERSONNEL, ordre.S_MESSAGE, false))
                            {
                                return false;
                            }
                            LogFile.Notifier(string.Format("NouveauxOrdres envoie d'un message de {0}({1}) � {2}({3}) : {4}",
                                lignePion.S_NOM,
                                lignePion.ID_PION,
                                lignePionDestination.S_NOM,
                                lignePionDestination.ID_PION,
                                ordre.S_MESSAGE));
                        }
                        /*
                        else
                        {
                            //cas particulier, avant d�but de partie, chaque message est envoy� � tous les joueurs d'un m�me camp imm�diatement
                            //il faut �galement n'envoyer que des messages qui n'ont pas d�j� �t� envoy�s !
                            //ID_ORDRE n'est pas un bon crit�re car d'autres ordres peuvent �tre cr�es par le programme
                            if (null == Donnees.m_donnees.TAB_ORDRE.FindByID_ORDRE(ordre.ID_ORDRE))
                            {
                                //envoie � tous les autres pion alli�s ayant un r�le
                                //il faut rechercher tous les r�les dans les pions ont la m�me nation que le pion de l'emetteur.
                                Donnees.TAB_MODELE_PIONRow ligneModelePion = Donnees.m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION(lignePion.ID_MODELE_PION);

                                var result = from Role in Donnees.m_donnees.TAB_ROLE
                                             join Pion in Donnees.m_donnees.TAB_PION
                                             on new { Role.ID_PION } equals new { Pion.ID_PION }
                                             join Modele in Donnees.m_donnees.TAB_MODELE_PION
                                             on new { Pion.ID_MODELE_PION, ligneModelePion.ID_NATION } equals new { Modele.ID_MODELE_PION, Modele.ID_NATION }
                                             select Role.ID_PION;

                                if (result.Count() > 0)
                                {
                                    //on a trouv� le mod�le, il faut modifier celui du pion associ�
                                    for (int i = 0; i < result.Count(); i++)
                                    {
                                        Donnees.TAB_PIONRow lignePionDestinationDepart = Donnees.m_donnees.TAB_PION.FindByID_PION(result.ElementAt(i));
                                        if (!ClassMessager.EnvoyerMessage(lignePion, lignePionDestinationDepart, ClassMessager.MESSAGE_PERSONNEL, ordre.S_MESSAGE, true))
                                        {
                                            return false;
                                        }
                                        LogFile.Notifier(string.Format("NouveauxOrdres envoie d'un message de {0}({1}) � {2}({3}) : {4}",
                                            lignePion.S_NOM,
                                            lignePion.ID_PION,
                                            lignePionDestinationDepart.S_NOM,
                                            lignePionDestinationDepart.ID_PION,
                                            ordre.S_MESSAGE));
                                    }
                                }
                                else
                                {
                                    message = string.Format("NouvelleHeure : impossible de trouver des alli�s � qui envoyer des message en d�but de partie ID_PION={0}, ID_MODELE={1} ID_NATION:{2}",
                                        lignePion.ID_PION, lignePion.ID_MODELE_PION, ligneModelePion.ID_NATION);
                                    LogFile.Notifier(message, out messageErreur);
                                    return false;
                                }

                            }
                        }
                         * */
                    }
                    else
                    {
                        compas = ClassMessager.DirectionOrdreVersCompas(ordre.I_DIRECTION);

                        if (!ClassMessager.ZoneGeographiqueVersCase(lignePion, ordre.I_DISTANCE, compas, ordre.ID_NOM_LIEU, out id_case_destination))
                        {
                            //Cela indique qu'il est impossible de se rendre � l'emplacement indiqu�
                            //il faut quand m�me ajouter un ordre termin� cela peut servir dans le message
                            Donnees.TAB_ORDRERow ligneOrdreNouveau = Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                                -1,//id_ordre_suivant
                                ordre.ID_ORDRE, ///ordre web
                                ordre.I_TYPE,
                                ordre.ID_PION,//ordre.ID_PION,
                                lignePion.ID_CASE,
                                0,//i_effectif_depart lignePion.I_INFANTERIE + lignePion.I_CAVALERIE +lignePion.I_ARTILLERIE,
                                id_case_destination,
                                ordre.ID_NOM_LIEU,//ville de destination
                                0,//i_effectif_destination
                                Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,
                                0,
                                Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,//i_tour_fin
                                0,//i_phase_fin
                                -1,//id_message,
                                -1,//ID_DESTINATAIRE_MESSAGE
                                -1,//id_bataille
                                ordre.I_HEURE,
                                ordre.I_DUREE);
                            ligneOrdreNouveau.SetID_ORDRE_SUIVANTNull();
                            ligneOrdreNouveau.SetI_TOUR_FINNull();
                            ligneOrdreNouveau.SetI_PHASE_FINNull();
                            ligneOrdreNouveau.SetID_MESSAGENull();
                            ligneOrdreNouveau.SetID_BATAILLENull();
                            ligneOrdreNouveau.SetID_DESTINATAIRE_MESSAGENull();
                            
                            // Il faut envoyer un message pour pr�venir l'officier responsable
                            ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_DESTINATION_IMPOSSIBLE);
                            LogFile.Notifier(string.Format("NouvelleHeure, case de destination impossible pour ordre.ID_ORDRE={0} ID_NOM_LIEU={1} I_DISTANC={2}", ordre.ID_ORDRE, ordre.ID_NOM_LIEU, ordre.I_DISTANCE));

                            return true;
                        }
                        else
                        {
                            if (lignePion.ID_PION == lignePionDestination.ID_PION)
                            {
                                //cas d'un leader qui se "donne" un ordre
                                Donnees.TAB_ORDRERow ligneOrdreNouveau = Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                                    -1,//id_ordre_suivant
                                    ordre.ID_ORDRE, ///ordre web
                                    ordre.I_TYPE,
                                    ordre.ID_PION,//ordre.ID_PION,
                                    lignePion.ID_CASE,
                                    0,//i_effectif_depart lignePion.I_INFANTERIE + lignePion.I_CAVALERIE +lignePion.I_ARTILLERIE,
                                    id_case_destination,
                                    ordre.ID_NOM_LIEU,//ville de destination
                                    0,//i_effectif_destination
                                    Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,
                                    0,
                                    -1,//i_tour_fin
                                    -1,//i_phase_fin
                                    -1,//id_message,
                                    -1,//ID_DESTINATAIRE_MESSAGE
                                    -1,//id_bataille
                                    ordre.I_HEURE,
                                    ordre.I_DUREE);
                                ligneOrdreNouveau.SetID_ORDRE_SUIVANTNull();
                                ligneOrdreNouveau.SetI_TOUR_FINNull();
                                ligneOrdreNouveau.SetI_PHASE_FINNull();
                                ligneOrdreNouveau.SetID_MESSAGENull();
                                ligneOrdreNouveau.SetID_BATAILLENull();
                                ligneOrdreNouveau.SetID_DESTINATAIRE_MESSAGENull();

                                LogFile.Notifier(string.Format("NouveauxOrdres envoie d'un ordre IDWeb={0}, ID={1} de type {2} � {3}({4})",
                                    ordre.ID_ORDRE,
                                    ligneOrdreNouveau.ID_ORDRE,
                                    ordre.I_TYPE,
                                    lignePion.S_NOM,
                                    lignePion.ID_PION));
                            }
                            else
                            {
                                //ordre qui doit �tre remis � une unit� par un messager
                                Donnees.TAB_ORDRERow ligneOrdreARemettre = Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                                    -1,//id_ordre_suivant
                                    ordre.ID_ORDRE, ///ordre web
                                    ordre.I_TYPE,
                                    -1,//ordre.ID_PION,
                                    lignePion.ID_CASE,
                                    0,//i_effectif_depart lignePion.I_INFANTERIE + lignePion.I_CAVALERIE +lignePion.I_ARTILLERIE,
                                    id_case_destination,
                                    ordre.ID_NOM_LIEU,
                                    0,//i_effectif_destination
                                    Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,
                                    0,
                                    -1,//i_tour_fin
                                    -1,//i_phase_fin
                                    -1,//id_message,
                                    ordre.ID_PION_DESTINATAIRE,
                                    -1,//id_bataille
                                    ordre.I_HEURE,
                                    ordre.I_DUREE);
                                ligneOrdreARemettre.SetID_ORDRE_SUIVANTNull();
                                ligneOrdreARemettre.SetI_TOUR_FINNull();
                                ligneOrdreARemettre.SetI_PHASE_FINNull();
                                ligneOrdreARemettre.SetID_MESSAGENull();
                                ligneOrdreARemettre.SetID_BATAILLENull();

                                Donnees.TAB_PIONRow lignePionMessager = ClassMessager.CreerMessager(lignePion);

                                LogFile.Notifier(string.Format("NouveauxOrdres envoie d'un ordre par messager IDWeb={0}, ID={1} de type {2} de {3}({4}) � {5}({6})",
                                    ordre.ID_ORDRE,
                                    ligneOrdreARemettre.ID_ORDRE,
                                    ordre.I_TYPE,
                                    lignePion.S_NOM,
                                    lignePion.ID_PION,
                                    lignePionDestination.S_NOM,
                                    lignePionDestination.ID_PION));

                                //et maintenant, un ordre de mouvement
                                compas = ClassMessager.DirectionOrdreVersCompas(ordre.I_DIRECTION_DESTINATAIRE);

                                if (!ClassMessager.ZoneGeographiqueVersCase(lignePionMessager, ordre.I_DISTANCE_DESTINATAIRE, compas, ordre.ID_NOM_LIEU_DESTINATAIRE, out id_case_destination))
                                {
                                    LogFile.Notifier(string.Format("NouvelleHeure, impossible de trouver la case du destinataire pour ordre.ID_ORDRE={0} ID_NOM_LIEU={1} I_DISTANC={2}", ordre.ID_ORDRE, ordre.ID_NOM_LIEU_DESTINATAIRE, ordre.I_DISTANCE_DESTINATAIRE));
                                    return false;
                                }
                                Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                                    ligneOrdreARemettre.ID_ORDRE,//id_ordre_suivant
                                    -1, //ordre web
                                    Donnees.CST_ORDRE_MESSAGE,
                                    lignePionMessager.ID_PION,
                                    lignePionMessager.ID_CASE,//id_case_depart
                                    0,//I_EFFECTIF_DEPART
                                    lignePionDestination.ID_CASE,//id_case_destination, le messager sait o� sont toutes les troupes ! -> s'il fallait �tre parfaitement logique il devrait partir vers la derni�re position connue du joueur
                                    -1,//ville de destination
                                    0,//I_EFFECTIF_DESTINATION
                                    0,//I_TOUR_DEBUT
                                    0,//I_PHASE_DEBUT
                                    0,//I_TOUR_FIN
                                    0,//I_PHASE_FIN
                                    -1,//ID_MESSAGE,
                                    ordre.ID_PION_DESTINATAIRE,//id_destinataire_message
                                    -1,
                                    0,//I_HEURE_DEBUT
                                    24);//I_DUREE
                                ligneOrdre.SetID_ORDRE_WEBNull();
                                ligneOrdre.SetID_BATAILLENull();
                                ligneOrdre.SetI_TOUR_DEBUTNull();
                                ligneOrdre.SetI_PHASE_DEBUTNull();
                                ligneOrdre.SetI_TOUR_FINNull();
                                ligneOrdre.SetI_PHASE_FINNull();
                                ligneOrdre.SetI_HEURE_DEBUTNull();
                                ligneOrdre.SetI_DUREENull();
                                ligneOrdre.SetID_MESSAGENull();
                                ligneOrdre.SetID_NOM_DESTINATIONNull();
                            }
                        }
                    }
                }
            }

            LogFile.Notifier("Fin NouveauxOrdres");

            return true;
        }

        /// <summary>
        /// D�but d'un nouveau jour
        /// </summary>
        /// <returns>true si ok, false si ko</returns>
        private bool NouveauJour()
        {
            //tirage de la m�t�o du jour
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
            Donnees.m_donnees.TAB_PARTIE[0].ID_METEO = Donnees.m_donnees.TAB_METEO[i - 1].ID_METEO;
            LogFile.Notifier(string.Format("Nouvelle meteo id={0} ({1})", Donnees.m_donnees.TAB_METEO[i - 1].ID_METEO, Donnees.m_donnees.TAB_METEO[i - 1].S_NOM));
            
            //initialisation des unit�s
            foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            {
                lignePion.I_NB_PHASES_MARCHE_JOUR = 0;
                lignePion.I_NB_PHASES_MARCHE_NUIT = 0;
            }
            return true;
        }

        /// <summary>
        /// Fin d'une journ�e
        /// </summary>
        /// <returns>true si ok, false si ko</returns>
        private bool FinDuJour()
        {
            string messageErreur, message;
            string requete;
            int moral, diffmoral;
            int fatigue, diffatigue;
            int i;

            i = 0;
            while (i<Donnees.m_donnees.TAB_PION.Count)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                //calcul de la fatigue et du moral
                if (!lignePion.B_DETRUIT && (lignePion.I_ARTILLERIE > 0 || lignePion.I_CAVALERIE > 0 || lignePion.I_INFANTERIE > 0 || lignePion.estDepot))
                {
                    //est-ce que l'unit� a fait un combat durant cette journ�e ?
                    requete = string.Format("ID_PION={0} AND B_ENGAGEE=True", lignePion.ID_PION);
                    Donnees.TAB_BATAILLE_PIONSRow[] resBataillePions=(Donnees.TAB_BATAILLE_PIONSRow[])Donnees.m_donnees.TAB_BATAILLE_PIONS.Select(requete);

                    //est-ce que l'unit� a fait une activit� ce jour ?
                    if (lignePion.I_NB_PHASES_MARCHE_JOUR > 0 || lignePion.I_NB_PHASES_MARCHE_NUIT > 0 || resBataillePions.Count() > 0)
                    {
                        int nbHeuresJour = (int)Math.Ceiling((decimal)lignePion.I_NB_PHASES_MARCHE_JOUR / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES);
                        int nbHeuresNuit = (int)Math.Ceiling((decimal)lignePion.I_NB_PHASES_MARCHE_NUIT / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES);

                        fatigue = (lignePion.I_CAVALERIE > 0) ? Constantes.tableFatigueCavalerie[nbHeuresJour + nbHeuresNuit] : Constantes.tableFatigueInfanterie[nbHeuresJour + nbHeuresNuit];
                        fatigue += nbHeuresNuit;
                        fatigue += lignePion.I_NB_HEURES_COMBAT;

                        moral = lignePion.I_MORAL; //pas de modification permanente du moral, le moral effectif avec la fatigue doit �tre calcul�e sur l'instant
                        fatigue = Math.Min(lignePion.I_FATIGUE + fatigue, 100);
                        diffmoral = moral - lignePion.I_MORAL;
                        diffatigue = fatigue - lignePion.I_FATIGUE;
                        lignePion.I_FATIGUE = fatigue;
                        lignePion.I_MORAL = moral;
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_BILAN_ACTION, diffmoral, diffatigue))
                        {
                            message = string.Format("FinDuJour : erreur lors de l'envoi d'un message MESSAGE_BILAN_ACTION");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }
                    else
                    {
                        //unit� au repos
                        //l'unit� ne se repose que si elle est ravitaill�e
                        bool bUniteRavitaillee; 
                        int distanceRavitaillement;
                        string depotRavitaillement;
                        if (!RavitaillementUnite(lignePion, out bUniteRavitaillee, out distanceRavitaillement, out depotRavitaillement)) 
                        {
                            message = string.Format("FinDuJour : erreur dans RavitaillementUnite pour ID pion = " + lignePion.ID_PION);
                            LogFile.Notifier(message, out messageErreur);
                            return false; 
                        }
                        if (bUniteRavitaillee)
                        {
                            moral = Math.Min(lignePion.I_MORAL + CST_GAIN_MORAL_REPOS, lignePion.I_MORAL_MAX);
                            fatigue = Math.Max(lignePion.I_FATIGUE - CST_GAIN_MORAL_REPOS, 0);
                            diffmoral = moral - lignePion.I_MORAL;
                            diffatigue = lignePion.I_FATIGUE - fatigue;
                            lignePion.I_FATIGUE = fatigue;
                            lignePion.I_MORAL = moral;
                            if (diffatigue > 0 && diffatigue > 0)
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
                        else
                        {
                            //unit�e non ravitaill�e, il faut envoyer un message pour pr�venir le propri�taire.
                            if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_SANS_RAVITAILLEMENT))
                            {
                                message = string.Format("{0}(ID={1}, erreur sur EnvoyerMessage avec MESSAGE_SANS_RAVITAILLEMENT dans FinDuJour", lignePion.S_NOM, lignePion.ID_PION);
                                LogFile.Notifier(message);
                                return false;
                            }
                        }
                    }
                    lignePion.I_NB_PHASES_MARCHE_JOUR = 0;
                    lignePion.I_NB_PHASES_MARCHE_NUIT = 0;
                    lignePion.I_NB_HEURES_COMBAT = 0;
                }
                ++i;
            }
            return true;
        }

        private bool ExecuterMouvement(Donnees.TAB_PIONRow lignePion, int phase)
        {
            string messageErreur, message;

            if (lignePion.estAuCombat)
            {
                message = string.Format("{0}(ID={1}, en bataille:{2}, pas de mouvement autoris�)", lignePion.S_NOM, lignePion.ID_PION, lignePion.ID_BATAILLE);
                return LogFile.Notifier(message, out messageErreur);
            }

            //if (lignePion.ID_PION == 157)
            //{
            //    Trace.Write("Pourquoi un messager de Bellegarde ne bouge pas ?");
            //}
            ////il y a un ordre de mouvement pour l'unit�, on prend le premier �mis
            Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(lignePion.ID_PION);
            if (null != ligneOrdre)
            {
                //si l'ordre de mouvement n'est pas actif, le pion ne doit pas bouger
                if (!Cartographie.OrdreActif(lignePion, ligneOrdre))
                {
                    return true;
                }
                //il y a un ordre de mouvement
                //on v�rifie si l'on est bien dans le cr�neau horaire souhait�
                //s'il n'y a pas d'heure d�finie c'est que l'unit� bouge tout le temps (messager par exemple)
                /*
                if (!ligneOrdre.IsI_HEURE_DEBUTNull() && ligneOrdre.I_DUREE>0)
                {
                    if (ligneOrdre.I_HEURE_DEBUT < (ligneOrdre.I_HEURE_DEBUT + ligneOrdre.I_DUREE) % 24)
                    {
                        if (ligneOrdre.I_HEURE_DEBUT > Donnees.m_donnees.TAB_PARTIE.HeureCourante())
                        {
                            message = string.Format("{0}(ID={1}, l'ordre ID={2} doit �tre execut� � partir de {3}, et il est {4})",
                                lignePion.S_NOM,
                                lignePion.ID_PION,
                                ligneOrdre.ID_ORDRE,
                                ligneOrdre.I_HEURE_DEBUT,
                                Donnees.m_donnees.TAB_PARTIE.HeureCourante());
                            return LogFile.Notifier(message, out messageErreur);
                        }
                        if ((ligneOrdre.I_HEURE_DEBUT + ligneOrdre.I_DUREE) % 24 < Donnees.m_donnees.TAB_PARTIE.HeureCourante())
                        {
                            message = string.Format("{0}(ID={1}, l'ordre ID={2} doit �tre execut� jusqu'� {3}({4}+{5} modulo 24={6}, et il est {7})",
                                lignePion.S_NOM,
                                lignePion.ID_PION,
                                ligneOrdre.ID_ORDRE,
                                ligneOrdre.I_HEURE_DEBUT + ligneOrdre.I_DUREE,
                                ligneOrdre.I_HEURE_DEBUT,
                                ligneOrdre.I_DUREE,
                                (ligneOrdre.I_HEURE_DEBUT + ligneOrdre.I_DUREE) % 24,
                                Donnees.m_donnees.TAB_PARTIE.HeureCourante());
                            return LogFile.Notifier(message, out messageErreur);
                        }
                    }
                    else
                    {
                        //cas d'une de marche de nuit ou d'une dur�e sur 24 heures (possible pour les QGs)
                        if ((ligneOrdre.I_HEURE_DEBUT + ligneOrdre.I_DUREE) % 24 < Donnees.m_donnees.TAB_PARTIE.HeureCourante()
                            && ligneOrdre.I_HEURE_DEBUT > Donnees.m_donnees.TAB_PARTIE.HeureCourante()) 
                        {
                            message = string.Format("{0}(ID={1}, cas 2, l'ordre ID={2} doit �tre execut� de {3} jusqu'� ({4}+{5} modulo 24={6}, et il est {7})",
                                lignePion.S_NOM,
                                lignePion.ID_PION,
                                ligneOrdre.ID_ORDRE,
                                ligneOrdre.I_HEURE_DEBUT,
                                ligneOrdre.I_HEURE_DEBUT,
                                ligneOrdre.I_DUREE,
                                (ligneOrdre.I_HEURE_DEBUT + ligneOrdre.I_DUREE) % 24,
                                Donnees.m_donnees.TAB_PARTIE.HeureCourante());
                            return LogFile.Notifier(message, out messageErreur);
                        }
                    }
                }
                 * */
                //recherche les mod�les de l'unit�
                Donnees.TAB_MODELE_PIONRow ligneModelePion = Donnees.m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION(lignePion.ID_MODELE_PION);
                Donnees.TAB_MODELE_MOUVEMENTRow ligneModeleMouvement = Donnees.m_donnees.TAB_MODELE_MOUVEMENT.FindByID_MODELE_MOUVEMENT(ligneModelePion.ID_MODELE_MOUVEMENT);
                Donnees.TAB_NATIONRow ligneNation = Donnees.m_donnees.TAB_PION.TrouveNation(lignePion);
                Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneOrdre.ID_CASE_DESTINATION);

                if (null == ligneCaseDestination)
                {
                    message = string.Format("ExecuterMouvement: {0}(ID={1}, ID_CASE:{2}, impossible de trouver la case de destination en base)", lignePion.S_NOM, lignePion.ID_PION, ligneOrdre.ID_CASE_DESTINATION);
                    return LogFile.Notifier(message, out messageErreur);
                }
                if (null == ligneNation)
                {
                    message = string.Format("ExecuterMouvement :{0}(ID={1}, Impossible de trouver la nation affect�e � l'unit�)", lignePion.S_NOM, lignePion.ID_PION);
                    return LogFile.Notifier(message, out messageErreur);
                }

                if (0 == ligneOrdre.I_EFFECTIF_DESTINATION)
                {
                    //si l'unit� n'est pas arriv�e � destination, il faut ajouter la fatigue
                    //l'unit� est entrain de marcher, compte pour la fatigue en fin de journ�e
                    if (Donnees.m_donnees.TAB_PARTIE.Nocturne())
                    {
                        lignePion.I_NB_PHASES_MARCHE_NUIT++;
                    }
                    else
                    {
                        lignePion.I_NB_PHASES_MARCHE_JOUR++;
                    }
                }

                if (lignePion.I_INFANTERIE > 0 || lignePion.I_CAVALERIE > 0 || lignePion.I_ARTILLERIE > 0)
                {
                    Donnees.TAB_CASERow ligneCaseDepart = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneOrdre.ID_CASE_DEPART);
                    if (null == ligneCaseDepart)
                    {
                        message = string.Format("ExecuterMouvement: {0}(ID={1}, ID_CASE_DEPART:{2}, impossible de trouver la case de d�part en base)", lignePion.S_NOM, lignePion.ID_PION, ligneOrdre.ID_CASE_DEPART);
                        return LogFile.Notifier(message, out messageErreur);
                    }

                    return ExecuterMouvementAvecEffectif(lignePion, ligneOrdre, ligneCaseDepart, ligneCaseDestination, ligneNation, ligneModelePion, ligneModeleMouvement);
                }
                else
                {
                    Donnees.TAB_CASERow ligneCaseDepart = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePion.ID_CASE);
                    if (null == ligneCaseDepart)
                    {
                        message = string.Format("ExecuterMouvement: {0}(ID={1}, ID_CASE:{2}, impossible de trouver la case de d�part en base)", lignePion.S_NOM, lignePion.ID_PION, lignePion.ID_CASE);
                        return LogFile.Notifier(message, out messageErreur);
                    }

                    return ExecuterMouvementSansEffectif(lignePion, ligneOrdre, ligneCaseDepart, ligneCaseDestination, ligneNation, ligneModelePion, ligneModeleMouvement);
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
            List<Donnees.TAB_CASERow> chemin;
            Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain;
            Donnees.TAB_PIONRow lignePionDestinataire;
            Donnees.TAB_PIONRow lignePionNouveauDestinataire;
            double cout, coutHorsRoute;

            if (null == lignePion || null == ligneOrdre || null == ligneCaseDepart || null == ligneCaseDestination || null == ligneNation || null == ligneModelePion || null == ligneModeleMouvement)
            {
                message = string.Format("ExecuterMouvementSansEffectif : erreur, un des param�tres est null");
                LogFile.Notifier(message, out messageErreur);
                return false;
            }

            if (lignePion.B_DETRUIT)
            {
                message = string.Format("{0},ID={1}, ExecuterMouvementSansEffectif : unit� d�truite", lignePion.S_NOM, lignePion.ID_PION);
                LogFile.Notifier(message, out messageErreur);
                return true;
            }

            //l'unit� avance-t-elle suffisement pour progresser d'une case de plus ?
            //lignePion.I_DISTANCE_A_PARCOURIR = 0;//pour les tests
            if (lignePion.I_DISTANCE_A_PARCOURIR > 0)
            {
                vitesse = Cartographie.CalculVitesseMouvementPion(lignePion);//vitesse en km/h
                lignePion.I_DISTANCE_A_PARCOURIR -= (vitesse * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * Donnees.m_donnees.TAB_JEU[0].I_COUT_DE_BASE / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES);
                message = string.Format("{0}, ID={1}, en mouvement, I_DISTANCE_A_PARCOURIR={2}, vitesse={3}",
                    lignePion.S_NOM, lignePion.ID_PION, lignePion.I_DISTANCE_A_PARCOURIR, vitesse);
                LogFile.Notifier(message, out messageErreur);
            }
            if (lignePion.I_DISTANCE_A_PARCOURIR <= 0)
            {
                //faire avancer l'unit� si celle-ci n'est pas arriv� � destination
                message = string.Format("{0}(ID={1}, en mouvement, aucune troupe � destination)", lignePion.S_NOM, lignePion.ID_PION);
                LogFile.Notifier(message, out messageErreur);
                message = string.Format("ExecuterMouvementSansEffectif :ligneOrdre.ID_ORDRE={0} ligneOrdre.I_EFFECTIF_DEPART={1}", ligneOrdre.ID_ORDRE, ligneOrdre.I_EFFECTIF_DEPART);
                LogFile.Notifier(message, out messageErreur);

                if (lignePion.ID_PION == 150)
                {
                    Debug.WriteLine("messager qui ne bouge pas");
                }
                //on l'avance d'une case de plus
                //calcul du plus court chemin
                //il peut arriver que l'ordre de destination donne sur la m�me case que l� o� se trouve d�j� l'unit�.
                //par exemple si l'unit� est sur la m�me case que son chef.
                if (lignePion.ID_CASE != ligneOrdre.ID_CASE_DESTINATION)
                {
                    if (!Cartographie.RechercheChemin(Cartographie.typeParcours.MOUVEMENT, lignePion, ligneCaseDepart, ligneCaseDestination, ligneOrdre, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                    {
                        message = string.Format("{0}(ID={1}, erreur sur RechercheChemin (cas 2) dans ExecuterMouvementSansEffectif :{2})", lignePion.S_NOM, lignePion.ID_PION, messageErreur);
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
                    message = string.Format("ExecuterMouvementSansEffectif : SearchPath longueur={0} de {1} � {2}", chemin.Count, ligneCaseDepart.ID_CASE, ligneCaseDestination.ID_CASE);
                    LogFile.Notifier(message, out messageErreur);
                    lignePion.ID_CASE = chemin[1].ID_CASE;

                    //on ajoute, le cout qu'il a fallu pour arriver jusqu'� cette case
                    ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(chemin[1].ID_MODELE_TERRAIN);
                    //lignePion.I_DISTANCE_A_PARCOURIR += tableCoutsMouvementsTerrain[ligneModeleTerrain.ID_MODELE_TERRAIN].cout;
                    string requete = string.Format("ID_MODELE_MOUVEMENT={0} AND ID_METEO={1} AND ID_MODELE_TERRAIN={2}",
                        ligneModelePion.ID_MODELE_MOUVEMENT,
                        Donnees.m_donnees.TAB_PARTIE[0].ID_METEO,
                        ligneModeleTerrain.ID_MODELE_TERRAIN);
                    Donnees.TAB_MOUVEMENT_COUTRow[] resCout = (Donnees.TAB_MOUVEMENT_COUTRow[])Donnees.m_donnees.TAB_MOUVEMENT_COUT.Select(requete);
                    lignePion.I_DISTANCE_A_PARCOURIR += resCout[0].I_COUT;
                }

                //est on est arriv� � destination ?
                if (lignePion.ID_CASE==ligneOrdre.ID_CASE_DESTINATION)
                {
                    message = string.Format("{0}(ID={1}, fin du mouvement, compl�tement arriv�e)", lignePion.S_NOM, lignePion.ID_PION);
                    LogFile.Notifier(message, out messageErreur);

                    bool bOrdreTermine = true;

                    #region Messagers
                    if (lignePion.estMessager)
                    {
                        //on v�rifie qu'il y a bien un destinataire valable
                        lignePionDestinataire= Donnees.m_donnees.TAB_PION.FindByID_PION(ligneOrdre.ID_DESTINATAIRE_MESSAGE);
                        if (null==lignePionDestinataire)
                        {
                            message = string.Format("{0}(ID={1}, ExecuterMouvementSansEffectif pas de destinataire ID_ORDRE:{2})", 
                                lignePion.S_NOM, lignePion.ID_PION, ligneOrdre.ID_ORDRE);
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }

                        // Si le destinataire est d�truit, il est inutile de lui donner le message !
                        if (!lignePionDestinataire.B_DETRUIT)
                        {
                            long distanceCoutDeplacement = 0;

                            if (lignePionDestinataire.ID_CASE != ligneOrdre.ID_CASE_DESTINATION)
                            {
                                // Si le recepteur du message a boug�, aller vers la nouvelle case destination
                                // Si le destinataire et l'emetteur sont des joueur et qu'on lui porte un message, il ne faut pas qu'il soit trop �loign� pour continuer � rechercher le destinataire
                                // dans tous les autres cas, le message arrive toujours (le joueur n'�tant pas � la source de l'envoi ou du probl�me de d�placement, on ne peut pas le p�naliser).
                                // pour ses propres unit�s on suppose qu'il sait toujours en gros o� elles sont.
                                Donnees.TAB_PIONRow lignePionEmetteur = Donnees.m_donnees.TAB_PION.FindByID_PION(lignePion.ID_PION_PROPRIETAIRE);
                                if (lignePionDestinataire.estJoueur && lignePionEmetteur.estJoueur && (Donnees.CST_ORDRE_MESSAGE == ligneOrdre.I_ORDRE_TYPE))
                                {
                                    //Calcul de la distance entre les deux zones indiqu�es
                                    Donnees.TAB_CASERow ligneCaseNouvelleDestination = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePionDestinataire.ID_CASE);

                                    if (!Cartographie.RechercheChemin(Cartographie.typeParcours.MOUVEMENT, lignePion, ligneCaseDestination, ligneCaseNouvelleDestination, null, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                                    {
                                        message = string.Format("{0}(ID={1}, erreur sur RechercheChemin dans ExecuterMouvementSansEffectif: message a un destinataire :{2})", lignePion.S_NOM, lignePion.ID_PION, messageErreur);
                                        LogFile.Notifier(message);
                                        return false;
                                    }

                                    // Calcul de la distance en km relative � l'effort fait pour le trajet. On divise deux fois par l'�chelle, 
                                    // car le cout renvoy� est le nombre de pixel par case(echelle) * cout de la case
                                    // diviser ce cout par l'echelle et le cout de base renvoit donc la valeur en "pixels", pas en kilometre
                                    distanceCoutDeplacement = (long)cout / (Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * Donnees.m_donnees.TAB_JEU[0].I_COUT_DE_BASE);

                                }

                                if (distanceCoutDeplacement > CST_DISTANCE_MAX_RECHERCHE_DESTINATAIRE)
                                {
                                    ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_DESTINATAIRE_INTROUVABLE);
                                    Cartographie.DetruirePion(lignePion);//le messager est devenu inutile
                                }
                                else
                                {
                                    //il faut suivre le destinataire !
                                    message = string.Format("{0}(ID={1}, ExecuterMouvementSansEffectif le destinataire a boug� ID_ORDRE:{2} de {3} vers {4})",
                                        lignePion.S_NOM, lignePion.ID_PION, ligneOrdre.ID_ORDRE, ligneOrdre.ID_CASE_DESTINATION, lignePionDestinataire.ID_CASE);
                                    LogFile.Notifier(message, out messageErreur);
                                    ligneOrdre.ID_CASE_DESTINATION = lignePionDestinataire.ID_CASE;
                                    ligneOrdre.SetID_NOM_DESTINATIONNull();//premi�re ville destinatrice du message, sans valeur maintenant
                                    bOrdreTermine = false; //finallement, l'ordre n'est pas termin�
                                }
                            }
                            else
                            {
                                // Un message peut-�tre de deux ordres, soit un message direct envoy� d'un joueur vers un autre ou d'une unit� vers un joueur
                                // ou bien le message est en fait l'envoi d'un ordre d'un joueur vers l'une de ses unit�s
                                // dans ce deuxi�me cas, l'ordre transmis se trouve dans l'ordre suivant
                                if (ligneOrdre.IsID_ORDRE_SUIVANTNull() || ligneOrdre.ID_ORDRE_SUIVANT<0)
                                {
                                    switch (ligneOrdre.I_ORDRE_TYPE)
                                    {
                                        case Donnees.CST_ORDRE_MESSAGE:
                                            //il s'agit d'un message textuel
                                            //donner le message et dispara�tre si le recepteur est un joueur, sinon aller vers le chef recevant le message
                                            //if (lignePionDestinataire.ID_PION_PROPRIETAIRE == lignePionDestinataire.ID_PION ||
                                            //    lignePionDestinataire.IsID_PION_PROPRIETAIRENull() || lignePionDestinataire.ID_PION_PROPRIETAIRE <= 0)
                                            if (null != Donnees.m_donnees.TAB_ROLE.TrouvePion(lignePionDestinataire.ID_PION))
                                            {
                                                message = string.Format("{0},ID={1}, ExecuterMouvementSansEffectif donne le messsage ID_ORDRE:{2} � {3}",
                                                    lignePion.S_NOM, lignePion.ID_PION, ligneOrdre.ID_ORDRE, lignePionDestinataire.ID_PION);
                                                LogFile.Notifier(message, out messageErreur);

                                                Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.FindByID_MESSAGE(ligneOrdre.ID_MESSAGE);
                                                ligneMessage.I_TOUR_ARRIVEE = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                                                ligneMessage.I_PHASE_ARRIVEE = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;
                                                ligneMessage.ID_PION_PROPRIETAIRE = lignePionDestinataire.ID_PION;

                                                Cartographie.DetruirePion(lignePion);//le messager est devenu inutile
                                            }
                                            else
                                            {
                                                //changement de destinataire
                                                lignePionNouveauDestinataire = Donnees.m_donnees.TAB_PION.FindByID_PION(lignePionDestinataire.ID_PION_PROPRIETAIRE);
                                                message = string.Format("{0},ID={1}, ExecuterMouvementSansEffectif change le destinataire du messsage ID_ORDRE:{2} de {3} ({4}) � {5} ({6})",
                                                    lignePion.S_NOM, lignePion.ID_PION, ligneOrdre.ID_ORDRE,
                                                    lignePionDestinataire.ID_PION,
                                                    lignePionDestinataire.S_NOM,
                                                    lignePionNouveauDestinataire.ID_PION,
                                                    lignePionNouveauDestinataire.S_NOM);
                                                LogFile.Notifier(message, out messageErreur);

                                                ligneOrdre.ID_DESTINATAIRE_MESSAGE = lignePionNouveauDestinataire.ID_PION;
                                                ligneOrdre.ID_CASE_DESTINATION = lignePionNouveauDestinataire.ID_CASE;
                                                ligneOrdre.SetID_NOM_DESTINATIONNull();//premi�re ville destinatrice du message, sans valeur maintenant
                                                bOrdreTermine = false;//finallement, l'ordre n'est pas termin�                                                
                                            }
                                            break;
                                        default:
                                            //on ne devrait pas se retouver dans ce cas
                                            message = string.Format("{0},ID={1}, Erreur grave, ExecuterMouvementSansEffectif arriv�e d'un message porteur d'un ordre ID_ORDRE:{2} de type {3} � {4} ID={5} alors que ce n'est pas possible !",
                                                lignePion.S_NOM, lignePion.ID_PION, ligneOrdre.ID_ORDRE, ligneOrdre.I_ORDRE_TYPE, lignePionDestinataire.S_NOM, lignePionDestinataire.ID_PION);
                                            LogFile.Notifier(message, out messageErreur);
                                        return false;
                                    }
                                }
                                else
                                {
                                    //le messager porte un ordre � l'unit� destinatrice
                                    Donnees.TAB_ORDRERow ligneOrdreNouveau = Donnees.m_donnees.TAB_ORDRE.FindByID_ORDRE(ligneOrdre.ID_ORDRE_SUIVANT);
                                    Donnees.TAB_ORDRERow ligneOrdreCourant = Donnees.m_donnees.TAB_ORDRE.Courant(lignePionDestinataire.ID_PION);
                                    Donnees.TAB_PIONRow lignePionDonneurOrdre = Donnees.m_donnees.TAB_PION.FindByID_PION(lignePion.ID_PION_PROPRIETAIRE);
                                    Donnees.TAB_PIONRow lignePionMessage = lignePionDestinataire;
                                    ClassMessager.MESSAGES tipeMessage = ClassMessager.MESSAGES.MESSAGE_ORDRE_MOUVEMENT_RECU;

                                    switch (ligneOrdreNouveau.I_ORDRE_TYPE)
                                    {
                                        case Donnees.CST_ORDRE_MOUVEMENT:
                                            //l'ordre n'est recevable que si l'unit� n'est pas engag� au combat
                                            //si le pion est d�j� engag� en bataille et pas en fuite, l'ordre est refus�
                                            if (!lignePion.IsID_BATAILLENull() && 0 == lignePion.I_TOUR_FUITE_RESTANT)
                                            {
                                                Donnees.TAB_BATAILLERow ligneBataille = Donnees.m_donnees.TAB_BATAILLE.FindByID_BATAILLE(lignePion.ID_BATAILLE);
                                                ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_ORDRE_MOUVEMENT_IMPOSSIBLE_AU_COMBAT, ligneBataille);
                                            }
                                            else
                                            {
                                                ChangerOrdreCourant(lignePionDestinataire, ligneOrdreCourant, ligneOrdreNouveau);
                                            }
                                            break;

                                        case Donnees.CST_ORDRE_RETRAITE:
                                            ChangerOrdreCourant(lignePionDestinataire, ligneOrdreCourant, ligneOrdreNouveau);
                                            break;
                                        case Donnees.CST_ORDRE_PATROUILLE:
                                            //cr�ation d'une patrouille
                                            Donnees.TAB_PIONRow lignePionPatrouille = null;

                                            //recherche du modele
                                            var result = from Modele in Donnees.m_donnees.TAB_MODELE_PION
                                                         join AptitudesPion in Donnees.m_donnees.TAB_APTITUDES_PION
                                                         on new { Modele.ID_MODELE_PION, Modele.ID_NATION } equals new { AptitudesPion.ID_MODELE_PION, ligneModelePion.ID_NATION }
                                                         join Aptitudes in Donnees.m_donnees.TAB_APTITUDES
                                                         on new { AptitudesPion.ID_APTITUDE, stipe = "PATROUILLE" } equals new { Aptitudes.ID_APTITUDE, stipe = Aptitudes.S_NOM }
                                                         select Modele.ID_MODELE_PION;

                                            if (result.Count() > 0)
                                            {
                                                lignePionPatrouille = Donnees.m_donnees.TAB_PION.AddTAB_PIONRow(
                                                    result.ElementAt(0),//ID_MODELE_PION
                                                    lignePionDestinataire.ID_PION, //ID_PION_PROPRIETAIRE
                                                    "Patrouille du " + lignePionDestinataire.S_NOM,
                                                    0, 0,//I_INFANTERIE
                                                    0, 0,
                                                    0, 0,
                                                    0,//I_FATIGUE
                                                    100,//I_MORAL
                                                    100,//I_MORAL_MAX
                                                    0, 0, 0, 
                                                    'Z',//C_NIVEAU_HIERACHIQUE
                                                    0, 0, 0, 0,
                                                    lignePionDestinataire.ID_CASE, 0, 0, -1, 0, 
                                                    false, //B_DETRUIT
                                                    false,// B_FUITE_AU_COMBAT
                                                    false,// B_INTERCEPTION
                                                    false,//B_REDITION_RAVITAILLEMENT
                                                    false//B_TELEPORTATION
                                                );
                                                lignePionDestinataire.SetI_TOUR_SANS_RAVITAILLEMENTNull();
                                                lignePionPatrouille.SetI_ZONE_BATAILLENull();
                                                lignePionPatrouille.SetID_BATAILLENull();

                                            }
                                            else
                                            {
                                                message = string.Format("ExecuterMouvementSansEffectif : Erreur grave impossible de trouver le mod�le PATROUILLE ID_PION={0}, ID_NATION={1}",
                                                    lignePionPatrouille.ID_PION, ligneModelePion.ID_NATION);
                                                LogFile.Notifier(message, out messageErreur);
                                                return false;
                                            }
                                            //on lui affecte l'ordre correspondant
                                            ligneOrdreNouveau.ID_PION = lignePionPatrouille.ID_PION;
                                            ligneOrdreNouveau.ID_CASE_DEPART = lignePionDestinataire.ID_CASE;
                                            ligneOrdreNouveau.I_TOUR_DEBUT = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                                            ligneOrdreNouveau.I_PHASE_DEBUT = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;
                                            bOrdreTermine = false;

                                            tipeMessage = ClassMessager.MESSAGES.MESSAGE_ORDRE_PATROUILLE_RECU;
                                            lignePionMessage = lignePionPatrouille;
                                            break;
                                        default:
                                            //on ne devrait pas se retouver dans ce cas
                                            message = string.Format("{0},ID={1}, Erreur grave, ExecuterMouvementSansEffectif transmission d'un ordre ID_ORDRE:{2} de type {3} � {4} ID={5} alors que ce n'est pas possible !",
                                                lignePion.S_NOM, lignePion.ID_PION, ligneOrdreNouveau.ID_ORDRE, ligneOrdreNouveau.I_ORDRE_TYPE, lignePionDestinataire.S_NOM, lignePionDestinataire.ID_PION);
                                            LogFile.Notifier(message, out messageErreur);
                                            return false;
                                    }
                                    //envoyer un messager � l'emetteur indiquant la reception de l'ordre
                                    if (!ClassMessager.EnvoyerMessage(lignePionMessage, tipeMessage))
                                    {
                                        message = string.Format("ExecuterMouvementSansEffectif  : erreur lors de l'envoi d'un message pour indiquer qu'une unit� vient de recevoir un ordre");
                                        LogFile.Notifier(message, out messageErreur);
                                        return false;
                                    }
                                    message = string.Format("{0},ID={1}, ExecuterMouvementSansEffectif transmission d'un ordre ID_ORDRE:{2} de type {3} � {4} ID={5}",
                                        lignePion.S_NOM, lignePion.ID_PION, ligneOrdreNouveau.ID_ORDRE, ligneOrdreNouveau.I_ORDRE_TYPE, lignePionDestinataire.S_NOM, lignePionDestinataire.ID_PION);
                                    LogFile.Notifier(message, out messageErreur);

                                    Cartographie.DetruirePion(lignePion);//le messager d'origine est devenu inutile
                                }
                            }
                        }
                    }
                    #endregion
                    else
                    {
                        if (lignePion.estPatrouille)
                        {
                            //analyser ce que voit la patrouille puis envoyer un messager avec le r�sultat
                            Cartographie.RapportDePatrouille(lignePion);
                            bOrdreTermine = false;//la patrouille est maintenant un message-patrouille qui revient porter son rapport
                        }
                        else
                        {
                            //envoyer un messager pour pr�venir de l'arriv�e compl�te si l'on est pas un messager, ni une patrouille
                            if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_ARRIVE_A_DESTINATION))
                            {
                                message = string.Format("ExecuterMouvementSansEffectif  : erreur lors de l'envoi d'un message pour indiquer qu'une unit� arrive � destination");
                                LogFile.Notifier(message, out messageErreur);
                                return false;
                            }
                        }
                    }

                    if (bOrdreTermine)
                    {
                        //l'ordre est termin�, doit �tre fait � la fin, car l'ordre courant est recherch� par plusieurs op�rations pr�alable
                        lignePion.I_DISTANCE_A_PARCOURIR = 0;
                        ligneOrdre.I_TOUR_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                        ligneOrdre.I_PHASE_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;
                    }
                }
            }
            // on place l'unit� sur la case o� elle est actuellement
            Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePion.ID_CASE);
            if (null == ligneCase)
            {
                message = string.Format("ExecuterMouvementSansEffectif : impossible de trouver la case courante ID={0}", lignePion.ID_CASE);
                LogFile.Notifier(message, out messageErreur);
                return false;
            }
            else
            {
                int nbPlacesOccupees=0;
                Cartographie.RequisitionCase(lignePion, ligneCase, true, ref nbPlacesOccupees);//note : tous les cas de rencontre entre pions sont g�r�s dans cette m�thode
                //ligneCase.ID_NOUVEAU_PROPRIETAIRE = lignePion.ID_PION;

                message = string.Format("ExecuterMouvementSansEffectif : pion en ID={0} X={1} Y={2}",
                    ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y);
                LogFile.Notifier(message, out messageErreur);
            }

            return true;
        }

        private void ChangerOrdreCourant(Donnees.TAB_PIONRow lignePionDestinataire, Donnees.TAB_ORDRERow ligneOrdreCourant, Donnees.TAB_ORDRERow ligneOrdreNouveau)
        {
            //terminer l'ordre pr�c�dent du pion, s'il existe
            if (null != ligneOrdreCourant)
            {
                lignePionDestinataire.I_DISTANCE_A_PARCOURIR = 0;
                //DataSetCoutDonnees.m_donnees.TAB_ORDRE.RemoveTAB_ORDRERow(ligneOrdre);
                ligneOrdreCourant.I_TOUR_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                ligneOrdreCourant.I_PHASE_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;
            }

            //activer le nouvel ordre
            ligneOrdreNouveau.ID_PION = lignePionDestinataire.ID_PION;
            ligneOrdreNouveau.ID_CASE_DEPART = lignePionDestinataire.ID_CASE;
            ligneOrdreNouveau.I_TOUR_DEBUT = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
            ligneOrdreNouveau.I_PHASE_DEBUT = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;
            ligneOrdreNouveau.I_EFFECTIF_DEPART = lignePionDestinataire.I_INFANTERIE + lignePionDestinataire.I_CAVALERIE + lignePionDestinataire.I_ARTILLERIE;
        }

        private bool ExecuterMouvementAvecEffectif(Donnees.TAB_PIONRow lignePion, Donnees.TAB_ORDRERow ligneOrdre, Donnees.TAB_CASERow ligneCaseDepart, Donnees.TAB_CASERow ligneCaseDestination, Donnees.TAB_NATIONRow ligneNation, Donnees.TAB_MODELE_PIONRow ligneModelePion, Donnees.TAB_MODELE_MOUVEMENTRow ligneModeleMouvement)
        {
            string messageErreur, message;
            decimal vitesse;

            if (null == lignePion || null == ligneOrdre || null == ligneCaseDepart || null == ligneCaseDestination || null == ligneNation || null == ligneModelePion || null == ligneModeleMouvement)
            {
                message = string.Format("ExecuterMouvementAvecEffectifAvecEffectif : erreur, un des param�tres est null");
                LogFile.Notifier(message, out messageErreur);
                return false;
            }

            if (lignePion.B_DETRUIT)
            {
                message = string.Format("{0},ID={1}, ExecuterMouvementAvecEffectifAvecEffectif : unit� d�truite", lignePion.S_NOM, lignePion.ID_PION);
                LogFile.Notifier(message, out messageErreur);
                return true;
            }

            //l'unit� avance-t-elle suffisement pour progresser d'une case de plus ?
            //lignePion.I_DISTANCE_A_PARCOURIR = 0;//pour les tests
            vitesse = Cartographie.CalculVitesseMouvementPion(lignePion);//vitesse en km/h
            lignePion.I_DISTANCE_A_PARCOURIR -= (vitesse * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE * Donnees.m_donnees.TAB_JEU[0].I_COUT_DE_BASE / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES);
            message = string.Format("{0}, ID={1}, en mouvement, I_DISTANCE_A_PARCOURIR={2}",
                lignePion.S_NOM, lignePion.ID_PION, lignePion.I_DISTANCE_A_PARCOURIR);
            LogFile.Notifier(message, out messageErreur);

            if (lignePion.I_DISTANCE_A_PARCOURIR > 0)
            {
                //il faut placer le pion dans sa position actuelle en cas de collision pour les unit�s qui suivent et se d�placent
                return Cartographie.PlacerPionEnRoute(lignePion, ligneOrdre, ligneCaseDepart, ligneCaseDestination, ligneNation);
            }
            if (lignePion.I_DISTANCE_A_PARCOURIR <= 0)
            {
                if (lignePion.ID_PION == 135)
                {
                    Trace.Write("Fresnel n'a plus l'air de bouger");
                }

                //l'unit� progresse d'une case
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
        /// Toutes les forces de l'unit� en d�placement se trouvent sur la route
        /// Aucune ne se trouvent ni au d�part ni � l'arriv�e
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
            int encombrement, nbplacesOccupes;
            int iCavalerie, iInfanterie, iArtillerie;
            double cout, coutHorsRoute;
            AstarTerrain[] tableCoutsMouvementsTerrain;
            List<Donnees.TAB_CASERow> chemin;
            int i, id_case_finale;

            //faire avancer l'unit� si celle-ci n'est pas arriv� � destination
            message = string.Format("{0}(ID={1}, en ExecuterMouvementAvecEffectifForcesEnRoute, aucune troupe � destination)", lignePion.S_NOM, lignePion.ID_PION);
            LogFile.Notifier(message, out messageErreur);

            //on calcule combien de cases occupe le pion
            Cartographie.CalculerRepartitionEffectif(lignePion, lignePion.effectifTotal,
                out iInfanterie, out iCavalerie, out iArtillerie);
            message = string.Format("ExecuterMouvementAvecEffectifForcesEnRoute :effectif: i={0} c={1} a={2}", iInfanterie, iCavalerie, iArtillerie);
            LogFile.Notifier(message, out messageErreur);

            encombrement = Cartographie.CalculerEncombrement(lignePion, ligneNation, iInfanterie, iCavalerie, iArtillerie, true);
            message = string.Format("ExecuterMouvementAvecEffectifForcesEnRoute : encombrement={0}", encombrement);
            LogFile.Notifier(message, out messageErreur);

            //calcul du plus court chemin
            //CalculModeleMouvementsPion(lignePion, out tableCoutsMouvementsTerrain);
            //if (!m_etoile.SearchPath(ligneCaseDepart, ligneCaseDestination, tableCoutsMouvementsTerrain, DataSetCoutDonnees.m_donnees.TAB_JEU[0].I_ECHELLE))
            if (!Cartographie.RechercheChemin(Cartographie.typeParcours.MOUVEMENT, lignePion, ligneCaseDepart, ligneCaseDestination, ligneOrdre, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
            {
                message = string.Format("{0}(ID={1}, erreur sur RechercheChemin (cas 2) dans ExecuterMouvementAvecEffectifForcesEnRoute: {2})", lignePion.S_NOM, lignePion.ID_PION, messageErreur);
                LogFile.Notifier(message, out messageErreur);
                return false;
            }

            message = string.Format("ExecuterMouvementAvecEffectifForcesEnRoute : SearchPath longueur={0}", chemin.Count);
            LogFile.Notifier(message, out messageErreur);

            //Le g�n�ral de l'unit� occupe la queue de la division, il avance d'une case
            i = 0 ;
            while (i < chemin.Count && chemin[i].ID_CASE != lignePion.ID_CASE)
            {
                i++;
            }
            if (i >= chemin.Count)
            {
                message = string.Format("{0}(ID={1}, ExecuterMouvementAvecEffectifForcesEnRoute: impossible de trouver la position du pion sur le parcours !)", 
                    lignePion.S_NOM, lignePion.ID_PION);
                LogFile.Notifier(message, out messageErreur);
                return false;
            }
            i++;//on avance d'une case
            id_case_finale = chemin[i].ID_CASE;
            message = string.Format("ExecuterMouvementAvecEffectifForcesEnRoute : pion en {0}({1},{2})", chemin[i].ID_CASE, chemin[i].I_X, chemin[i].I_Y);
            LogFile.Notifier(message, out messageErreur);

            // On place les troupes sur le chemin
            nbplacesOccupes = 0;
            while (i < chemin.Count && nbplacesOccupes < encombrement)
            {
                if (!Cartographie.RequisitionCase(lignePion, chemin[i], true, ref nbplacesOccupes)) { return false; }
                i++;
            }

            //si on est arriv� � destination, on affecte les troupes qui viennent d'arriver sur place
            if (i == chemin.Count)
            {
                Cartographie.CalculerRepartitionEffectif(lignePion, 1, out iInfanterie, out iCavalerie, out iArtillerie);
                ligneOrdre.I_EFFECTIF_DESTINATION = iInfanterie + iCavalerie + iArtillerie;
                message = string.Format("ExecuterMouvementAvecEffectif : premiers effectifs � destination: i={0} c={1} a={2}", iInfanterie, iCavalerie, iArtillerie);
                LogFile.Notifier(message, out messageErreur);
            }
            else
            {
                //on ajoute, le cout qu'il a fallu pour arriver jusqu'� cette case
                Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain;
                ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(chemin[i].ID_MODELE_TERRAIN);

                if (!chemin[i].IsID_NOUVEAU_PROPRIETAIRENull() && (chemin[i].ID_NOUVEAU_PROPRIETAIRE != lignePion.ID_PION))
                {
                    Donnees.TAB_PIONRow lignePionBlocage = Donnees.m_donnees.TAB_PION.FindByID_PION(chemin[i].ID_NOUVEAU_PROPRIETAIRE);

                    //la route est d�j� occup�e "surcharg�e"
                    if (ligneModeleTerrain.B_ANNULEE_SI_OCCUPEE && !lignePionBlocage.estStatique && lignePionBlocage.ID_PION != lignePion.ID_PION)
                    {
                        //la case est occup�e par une unit� en mouvement et il faut attendre qu'elle se lib�re (cas des ponts par exemple)
                        //a-t-on d�j� envoy� un message pour pr�venir mon sup�rieur recemment ?                            
                        Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageEmis(lignePion.ID_PION, ClassMessager.MESSAGES.MESSAGE_CHEMIN_BLOQUE);
                        if (null == ligneMessage ||
                            ligneMessage.I_TOUR_DEPART + ClassMessager.CST_MESSAGE_FREQUENCE_ALERTE < Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)
                        {
                            if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_CHEMIN_BLOQUE))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    else
                    {
                        //on prend le terrain de substitution
                        ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(chemin[i].ID_MODELE_TERRAIN_SI_OCCUPE);
                    }
                }

                //lignePion.I_DISTANCE_A_PARCOURIR += tableCoutsMouvementsTerrain[ligneModeleTerrain.ID_MODELE_TERRAIN].cout;//cout d'un pixel -> KO cout indiff�renci� suivant le pion et la m�t�o
                //cacul du cout
                Donnees.TAB_MODELE_PIONRow ligneModelePion = Donnees.m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION(lignePion.ID_MODELE_PION);
                string requete = string.Format("ID_MODELE_MOUVEMENT={0} AND ID_METEO={1} AND ID_MODELE_TERRAIN={2}",
                    ligneModelePion.ID_MODELE_MOUVEMENT,
                    Donnees.m_donnees.TAB_PARTIE[0].ID_METEO,
                    ligneModeleTerrain.ID_MODELE_TERRAIN);
                Donnees.TAB_MOUVEMENT_COUTRow[] resCout = (Donnees.TAB_MOUVEMENT_COUTRow[])Donnees.m_donnees.TAB_MOUVEMENT_COUT.Select(requete);
                lignePion.I_DISTANCE_A_PARCOURIR += resCout[0].I_COUT;
                lignePion.ID_CASE = id_case_finale;
            }
            return true;
        }

        private bool ExecuterMouvementAvecEffectifForcesAuDepart(Donnees.TAB_PIONRow lignePion, Donnees.TAB_ORDRERow ligneOrdre, Donnees.TAB_CASERow ligneCaseDepart, Donnees.TAB_CASERow ligneCaseDestination, Donnees.TAB_NATIONRow ligneNation)
        {
            string message, messageErreur;
            int encombrement, nbplacesOccupes;
            int iCavalerie, iInfanterie, iArtillerie;
            double cout, coutHorsRoute;
            AstarTerrain[] tableCoutsMouvementsTerrain;
            List<Donnees.TAB_CASERow> chemin;
            int i;

            //faire avancer l'unit� si celle-ci n'est pas arriv� � destination
            message = string.Format("{0}(ID={1}, ExecuterMouvementAvecEffectifForcesAuDepart en mouvement, aucune troupe � destination)", lignePion.S_NOM, lignePion.ID_PION);
            LogFile.Notifier(message, out messageErreur);
            message = string.Format("ExecuterMouvementAvecEffectifForcesAuDepart :ligneOrdre.I_EFFECTIF_DEPART={0}", ligneOrdre.I_EFFECTIF_DEPART);
            LogFile.Notifier(message, out messageErreur);

            // si le pion commence tout juste � avance, on d�truit les espaces pr�c�dents
            if (lignePion.effectifTotal == ligneOrdre.I_EFFECTIF_DEPART)
            {
                Donnees.m_donnees.TAB_ESPACE.SupprimerEspacePion(lignePion.ID_PION);
                Donnees.m_donnees.AcceptChanges();
            }

            //on calcule de combien de cases le pion a d�j� avanc�
            Cartographie.CalculerRepartitionEffectif(lignePion,
                lignePion.effectifTotal - ligneOrdre.I_EFFECTIF_DEPART,
                out iInfanterie, out iCavalerie, out iArtillerie);
            message = string.Format("ExecuterMouvementAvecEffectifForcesAuDepart :effectif: i={0} c={1} a={2}", iInfanterie, iCavalerie, iArtillerie);
            LogFile.Notifier(message, out messageErreur);

            encombrement = Cartographie.CalculerEncombrement(lignePion, ligneNation, iInfanterie, iCavalerie, iArtillerie, true);
            //on l'avance d'une case de plus
            encombrement++;
            message = string.Format("ExecuterMouvementAvecEffectifForcesAuDepart : encombrement={0}", encombrement);
            LogFile.Notifier(message, out messageErreur);

            //calcul du plus court chemin
            //CalculModeleMouvementsPion(lignePion, out tableCoutsMouvementsTerrain);
            //if (!m_etoile.SearchPath(ligneCaseDepart, ligneCaseDestination, tableCoutsMouvementsTerrain, DataSetCoutDonnees.m_donnees.TAB_JEU[0].I_ECHELLE))
            if (!Cartographie.RechercheChemin(Cartographie.typeParcours.MOUVEMENT, lignePion, ligneCaseDepart, ligneCaseDestination, ligneOrdre, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
            {
                message = string.Format("{0}(ID={1}, erreur sur RechercheChemin (cas 2) dans ExecuterMouvementAvecEffectif: {2})", lignePion.S_NOM, lignePion.ID_PION, messageErreur);
                LogFile.Notifier(message, out messageErreur);
                return false;
            }
            message = string.Format("ExecuterMouvementAvecEffectifForcesAuDepart : SearchPath longueur={0}", chemin.Count);
            LogFile.Notifier(message, out messageErreur);
            i = 0;

            nbplacesOccupes = 0;
            while (i < chemin.Count && nbplacesOccupes < encombrement)
            {
                //la nouvelle case est-elle occup�e par un ennemi ?
                if (!Cartographie.RequisitionCase(lignePion, chemin[i], true, ref nbplacesOccupes)) { return false; }
                i++;
            }
            if (nbplacesOccupes < encombrement)
            {
                message = string.Format("ALERTE ExecuterMouvementAvecEffectifForcesAuDepart : impossible de placer les effectifs en cours de mouvement PION={0}({1}) nbplacesOccupes={2}<encombrement={3}",
                    lignePion.S_NOM, lignePion.ID_PION, nbplacesOccupes, encombrement);
                LogFile.Notifier(message, out messageErreur);
            }

            //on ajoute, le cout qu'il a fallu pour arriver jusqu'� cette case
            if (lignePion.ID_PION == 124)
            {
                LogFile.Notifier("test");
            }
            Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain;
            ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(chemin[i - 1].ID_MODELE_TERRAIN);
            if (!chemin[i - 1].EstInnocupe(lignePion, true))
            {
                Donnees.TAB_PIONRow lignePionBlocage = Donnees.m_donnees.TAB_PION.FindByID_PION(chemin[i - 1].ID_NOUVEAU_PROPRIETAIRE);

                if (ligneModeleTerrain.B_ANNULEE_SI_OCCUPEE && !lignePionBlocage.estStatique && lignePionBlocage.ID_PION != lignePion.ID_PION)
                {
                    //la case est occup�e et il faut attendre qu'elle se lib�re (cas des ponts par exemple)
                    //a-t-on d�j� envoy� un message pour pr�venir mon sup�rieur recemment ?                            
                    Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageEmis(lignePion.ID_PION, ClassMessager.MESSAGES.MESSAGE_CHEMIN_BLOQUE);
                    if (null == ligneMessage ||
                        ligneMessage.I_TOUR_DEPART + ClassMessager.CST_MESSAGE_FREQUENCE_ALERTE < Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)
                    {
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_CHEMIN_BLOQUE))
                        {
                            return false;
                        }
                    }
                    return true;//on avance pas
                }
                else
                {
                    //on prend le terrain de substitution
                    ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(chemin[i - 1].ID_MODELE_TERRAIN_SI_OCCUPE);
                }
            }
            //lignePion.I_DISTANCE_A_PARCOURIR += tableCoutsMouvementsTerrain[ligneModeleTerrain.ID_MODELE_TERRAIN].cout;//cout d'un pixel
            Donnees.TAB_MODELE_PIONRow ligneModelePion = Donnees.m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION(lignePion.ID_MODELE_PION);
            string requete = string.Format("ID_MODELE_MOUVEMENT={0} AND ID_METEO={1} AND ID_MODELE_TERRAIN={2}",
                ligneModelePion.ID_MODELE_MOUVEMENT,
                Donnees.m_donnees.TAB_PARTIE[0].ID_METEO,
                ligneModeleTerrain.ID_MODELE_TERRAIN);
            Donnees.TAB_MOUVEMENT_COUTRow[] resCout = (Donnees.TAB_MOUVEMENT_COUTRow[])Donnees.m_donnees.TAB_MOUVEMENT_COUT.Select(requete);
            lignePion.I_DISTANCE_A_PARCOURIR += resCout[0].I_COUT;

            //si on est arriv� � destination, on affecte les troupes qui viennent d'arriver sur place
            if (i == chemin.Count)
            {
                Cartographie.CalculerRepartitionEffectif(lignePion, 1, out iInfanterie, out iCavalerie, out iArtillerie);
                ligneOrdre.I_EFFECTIF_DESTINATION = iInfanterie + iCavalerie + iArtillerie;
                message = string.Format("ExecuterMouvementAvecEffectifForcesAuDepart : premiers effectifs � destination: i={0} c={1} a={2}", iInfanterie, iCavalerie, iArtillerie);
                LogFile.Notifier(message, out messageErreur);
            }

            //on calcule les effectifs qui ne sont plus au d�part
            Cartographie.CalculerEffectif(ligneNation, lignePion, encombrement, true, out iInfanterie, out iCavalerie, out iArtillerie);
            message = string.Format("ExecuterMouvementAvecEffectifForcesAuDepart :effectif en route: i={0} c={1} a={2} encombrement={3}", iInfanterie, iCavalerie, iArtillerie, encombrement);
            LogFile.Notifier(message, out messageErreur);

            //on place les effectifs encore au d�part
            ligneOrdre.I_EFFECTIF_DEPART = Math.Max(0, lignePion.effectifTotal - iInfanterie - iCavalerie - iArtillerie);
            Cartographie.PlacementPion(ligneOrdre.ID_CASE_DEPART, lignePion, ligneNation, true, ligneOrdre.I_EFFECTIF_DEPART);
            message = string.Format("ExecuterMouvementAvecEffectifForcesAuDepart : ligneOrdre.ID_ORDRE={0} ligneOrdre.I_EFFECTIF_DEPART final={1}",
                ligneOrdre.ID_ORDRE, ligneOrdre.I_EFFECTIF_DEPART);
            LogFile.Notifier(message, out messageErreur);
            return true;
        }

        /// <summary>
        /// L'unit� est arriv�e, il faut donc "�couler" les �l�ments qui ne sont pas encore arriv�s s'il y en a
        /// </summary>
        /// <returns></returns>
        private bool ExecuterMouvementAvecEffectifForcesADestination(Donnees.TAB_PIONRow lignePion, Donnees.TAB_ORDRERow ligneOrdre, Donnees.TAB_CASERow ligneCaseDepart, Donnees.TAB_CASERow ligneCaseDestination, Donnees.TAB_NATIONRow ligneNation)
        {
            string message, messageErreur;
            int encombrement, encombrementTotal, nbplacesOccupes;
            int iCavalerie, iInfanterie, iArtillerie;
            int iCavalerieDestination, iInfanterieDestination, iArtillerieDestination;
            int iCavalerieRoute, iInfanterieRoute, iArtillerieRoute;
            double cout, coutHorsRoute;
            AstarTerrain[] tableCoutsMouvementsTerrain;
            List<Donnees.TAB_CASERow> chemin;
            int i;

            //L'unit� est arriv�e, il faut donc "�couler" les �l�ments qui ne sont pas encore arriv�s s'il y en a
            message = string.Format("\r\n{0}(ID={1}, ExecuterMouvementAvecEffectifForcesADestination : en mouvement, une partie des troupes � destination)", lignePion.S_NOM, lignePion.ID_PION);
            LogFile.Notifier(message, out messageErreur);

            encombrementTotal = Cartographie.CalculerEncombrement(lignePion, ligneNation, lignePion.I_INFANTERIE, lignePion.I_CAVALERIE, lignePion.I_ARTILLERIE, true);
            message = string.Format("ExecuterMouvementAvecEffectifForcesADestination :ligneOrdre.I_EFFECTIF_DESTINATION={0} encombrementTotal={1}", ligneOrdre.I_EFFECTIF_DESTINATION, encombrementTotal);
            LogFile.Notifier(message, out messageErreur);
            if (ligneOrdre.I_EFFECTIF_DESTINATION < lignePion.effectifTotal)
            {
                //il faut faire avancer la "queue" de troupes jusqu'� l'arriv�e
                //on avance suivant le modele par d�faut, sur route, dont on calcule le cout
                if (!Cartographie.RechercheChemin(Cartographie.typeParcours.MOUVEMENT, lignePion, ligneCaseDepart, ligneCaseDestination, ligneOrdre, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                {
                    message = string.Format("{0}(ID={1}, ALERTE erreur sur RechercheChemin dans ExecuterMouvementAvecEffectifForcesADestination:{2}", lignePion.S_NOM, lignePion.ID_PION, messageErreur);
                    LogFile.Notifier(message, out messageErreur);
                    return false;
                }

                Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(Donnees.m_donnees.TAB_JEU[0].ID_MODELE_TERRAIN_DEPLOIEMENT);
                //CalculModeleMouvementsPion(lignePion, out tableCoutsMouvementsTerrain);
                //lignePion.I_DISTANCE_A_PARCOURIR += tableCoutsMouvementsTerrain[ligneModeleTerrain.ID_MODELE_TERRAIN].cout;
                Donnees.TAB_MODELE_PIONRow ligneModelePion = Donnees.m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION(lignePion.ID_MODELE_PION);
                string requete = string.Format("ID_MODELE_MOUVEMENT={0} AND ID_METEO={1} AND ID_MODELE_TERRAIN={2}",
                    ligneModelePion.ID_MODELE_MOUVEMENT,
                    Donnees.m_donnees.TAB_PARTIE[0].ID_METEO,
                    ligneModeleTerrain.ID_MODELE_TERRAIN);
                Donnees.TAB_MOUVEMENT_COUTRow[] resCout = (Donnees.TAB_MOUVEMENT_COUTRow[])Donnees.m_donnees.TAB_MOUVEMENT_COUT.Select(requete);
                lignePion.I_DISTANCE_A_PARCOURIR += resCout[0].I_COUT;

                //recherche du plus court chemin
                //CalculModeleMouvementsPion(lignePion, out tableCoutsMouvementsTerrain);
                //if (!m_etoile.SearchPath(ligneCaseDepart, ligneCaseDestination, tableCoutsMouvementsTerrain, DataSetCoutDonnees.m_donnees.TAB_JEU[0].I_ECHELLE))

                //effectifs actuellement � l'arriv�e
                Cartographie.CalculerRepartitionEffectif(lignePion,
                    ligneOrdre.I_EFFECTIF_DESTINATION,
                    out iInfanterieDestination, out iCavalerieDestination, out iArtillerieDestination);
                message = string.Format("ExecuterMouvementAvecEffectifForcesADestination :effectif � destination :i={0} c={1} a={2}",
                    iInfanterieDestination, iCavalerieDestination, iArtillerieDestination);
                LogFile.Notifier(message, out messageErreur);

                //effectifs maximum sur la route
                int encombrementArrivee = Cartographie.CalculerEncombrement(lignePion, ligneNation, iInfanterieDestination, iCavalerieDestination, iArtillerieDestination, true);
                if (!Cartographie.CalculerEffectif(ligneNation, lignePion,
                    lignePion.I_INFANTERIE - iInfanterieDestination,
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
                    lignePion.I_INFANTERIE - iInfanterieDestination,
                    lignePion.I_CAVALERIE - iCavalerieDestination,
                    lignePion.I_ARTILLERIE - iArtillerieDestination,
                    chemin.Count, iInfanterieRoute, iCavalerieRoute, iArtillerieRoute);
                LogFile.Notifier(message, out messageErreur);
                if (0 == ligneOrdre.I_EFFECTIF_DEPART)
                {
                    //la route est partiellement occup�e
                    encombrement = encombrementTotal - encombrementArrivee; // Cartographie.CalculerEncombrement(lignePion, ligneNation, iInfanterieRoute, iCavalerieRoute, iArtillerieRoute, true);
                }
                else
                {
                    //la route est compl�tement occup�e
                    encombrement = chemin.Count;
                }
                //en avan�ant d'une case, quels sont les nouveaux effectifs qui arrivent
                if (!Cartographie.CalculerEffectif(ligneNation, lignePion, iInfanterieRoute, iCavalerieRoute, iArtillerieRoute, 1, true, out iInfanterie, out iCavalerie, out iArtillerie))
                {
                    message = string.Format("ExecuterMouvementAvecEffectifForcesADestination :{0}(ID={1}, erreur CalculerEffectif renvoie false)", lignePion.S_NOM, lignePion.ID_PION);
                    LogFile.Notifier(message, out messageErreur);
                    return false;
                }
                message = string.Format("ExecuterMouvementAvecEffectifForcesADestination effectif en plus � destination iINFANTERIESource={0}, iCAVALERIESource={1}, iARTILLERIESource={2}, effectif={3}, iINFANTERIE={4}, iCAVALERIE={5}, iARTILLERIE={6}",
                    iInfanterieRoute, iCavalerieRoute, iArtillerieRoute, 1, iInfanterie, iCavalerie, iArtillerie);
                LogFile.Notifier(message, out messageErreur);

                //on modifie en cons�quence les effectifs de d�part et de destination
                ligneOrdre.I_EFFECTIF_DEPART = Math.Max(0, ligneOrdre.I_EFFECTIF_DEPART - iInfanterie - iCavalerie - iArtillerie);
                iInfanterieDestination += iInfanterie;
                iCavalerieDestination += iCavalerie;
                iArtillerieDestination += iArtillerie;
                ligneOrdre.I_EFFECTIF_DESTINATION = iInfanterieDestination + iCavalerieDestination + iArtillerieDestination;

                if (ligneOrdre.I_EFFECTIF_DESTINATION >= lignePion.effectifTotal)
                {
                    return ArriveADestination(lignePion, ligneOrdre, ligneNation);
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
                    //la nouvelle case est-elle occup�e par un ennemi ?
                    if (!Cartographie.RequisitionCase(lignePion, chemin[i], true, ref nbplacesOccupes)) { return false; }
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
                    //la derni�re place occup�e, devient la "position" du pion, c'est � dire de son g�n�ral, en queue de colonne
                    lignePion.ID_CASE = chemin[i].ID_CASE;
                }
                //placement des effectifs � destination sur la carte
                Cartographie.PlacementPion(ligneOrdre.ID_CASE_DESTINATION, lignePion, ligneNation, false, ligneOrdre.I_EFFECTIF_DESTINATION);

                //placer les effectifs encore au point de d�part
                if (ligneOrdre.I_EFFECTIF_DEPART > 0)
                {
                    message = string.Format("ExecuterMouvementAvecEffectifForcesADestination :effectif depart={0} i={1}, c={2}, a={3}",
                        ligneOrdre.I_EFFECTIF_DEPART,
                        lignePion.I_INFANTERIE - iInfanterieDestination - iInfanterieRoute,
                        lignePion.I_CAVALERIE - iCavalerieDestination - iCavalerieRoute,
                        lignePion.I_ARTILLERIE - iArtillerieDestination - iArtillerieRoute);
                    LogFile.Notifier(message, out messageErreur);
                    //out iInfanterie, out iCavalerie, out iArtillerie
                    Cartographie.PlacementPion(ligneOrdre.ID_CASE_DEPART, lignePion, ligneNation, true,
                        lignePion.I_INFANTERIE - iInfanterieDestination - iInfanterieRoute,
                        lignePion.I_CAVALERIE - iCavalerieDestination - iCavalerieRoute,
                        lignePion.I_ARTILLERIE - iArtillerieDestination - iArtillerieRoute);
                }
            }
            else
            {
                message = string.Format("{0}(ID={1}, Erreur ExecuterMouvementAvecEffectifForcesADestination fin du mouvement, compl�tement arriv�e : cas normalement impossible)", lignePion.S_NOM, lignePion.ID_PION);
                LogFile.Notifier(message, out messageErreur);
                return false;
            }
            return true;
        }

        private bool ArriveADestination(Donnees.TAB_PIONRow lignePion, Donnees.TAB_ORDRERow ligneOrdre, Donnees.TAB_NATIONRow ligneNation)
        {
            //les derniers viennent d'arriver
            string message = string.Format("{0}(ID={1}, en mouvement, les derniers sont arriv�s)", lignePion.S_NOM, lignePion.ID_PION);
            LogFile.Notifier(message);
            ligneOrdre.I_EFFECTIF_DEPART = 0;
            ligneOrdre.I_EFFECTIF_DESTINATION = lignePion.effectifTotal;
            lignePion.ID_CASE = ligneOrdre.ID_CASE_DESTINATION;
            lignePion.I_DISTANCE_A_PARCOURIR = 0;
            //placer l'unit� sur la carte
            //la zone d'arriv�e devient la zone de depart pour le placement des troupes
            Donnees.m_donnees.TAB_ESPACE.DeplacerEspacePion(lignePion.ID_PION, AStar.CST_DESTINATION);

            if (null == ligneNation)
            {
                ligneNation = Donnees.m_donnees.TAB_PION.TrouveNation(lignePion);
            }
            Cartographie.PlacementPion(lignePion, ligneNation, true);
            //supprimer l'ordre
            //DataSetCoutDonnees.m_donnees.TAB_ORDRE.RemoveTAB_ORDRERow(ligneOrdre);
            ligneOrdre.I_TOUR_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
            ligneOrdre.I_PHASE_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;

            //envoyer un messager pour pr�venir de l'arriv�e compl�te
            if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_ARRIVE_A_DESTINATION))
            {
                message = string.Format("ArriveADestination  : erreur lors de l'envoi d'un message pour indiquer qu'une unit� arrive � destination");
                LogFile.Notifier(message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Calcul du modificateur strat�gique du chef de bataille
        /// </summary>
        /// <param name="idLeader">identifiant du chef</param>
        /// <param name="nbUnites">nombre d'unit�s sous son commandement</param>
        /// <returns>modificateur strat�gique</returns>
        private int CalculModificateurStrategique(int idLeader, int nbUnites)
        {
            Donnees.TAB_PIONRow lignePion;
            int modificateurStrategique;
            if (idLeader >= 0)
            {
                lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(idLeader);
                if (lignePion.I_STRATEGIQUE < nbUnites)
                {
                    modificateurStrategique = -1;
                }
                else
                {
                    modificateurStrategique = 0;
                }
            }
            else
            {
                //aucun leader sur le combat
                modificateurStrategique = -2;
            }
            return modificateurStrategique;
        }

        /// <summary>
        /// Round suppl�mentaire d'une bataille
        /// </summary>
        /// <param name="ligneBataille">bataille � effectuer</param>
        /// <returns>true si OK, false si KO</returns>
        private bool EffectuerBataille(Donnees.TAB_BATAILLERow ligneBataille)
        {
            string message, messageErreur;
            int[] des;
            int[] effectifs;
            int i,score;
            int nbUnites012 = 0, nbUnites345 = 0;//pour les bonus strat�giques
            int nbUnites012Base = 0, nbUnites345Base = 0;//pour v�rifier qu'il y a bien des unit�s pr�sentes
            int modificateurStrategique012, modificateurStrategique345;
            Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain;
            Donnees.TAB_PIONRow[] lignePionsEnBataille012;
            Donnees.TAB_PIONRow[] lignePionsEnBataille345;
            bool bUniteEnDefense;
            int idLeader;

            if (!ligneBataille.IsI_TOUR_FINNull())
            {
                return true;//la bataille est d�j� termin�e
            }
            
            if (Donnees.m_donnees.TAB_PARTIE.Nocturne())
            {
                //pas de combat la nuit
                message = string.Format("EffectuerBataille sur ID_BATAILLE={0}: Fin de la bataille � cause de l'arriv�e de la nuit.", ligneBataille.ID_BATAILLE);
                LogFile.Notifier(message, out messageErreur);
                return FinDeBataille(ligneBataille);
            }

            #region choix des leaders sur le combat
            if (!RecherchePionsEnBataille(ligneBataille, out nbUnites012, out nbUnites345, out des, out effectifs, out lignePionsEnBataille012, out lignePionsEnBataille345, null/*bengagement*/, false/*bcombattif*/))
            {
                message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille QG");
                LogFile.Notifier(message, out messageErreur);
            }

            //trouver le leader avec le plus haut niveau hierarchique
            idLeader = TrouverLeaderBataille(ligneBataille, lignePionsEnBataille012, true);
            // Note : Dans la r�gle, le leader de plus haut rang hierarchique dirige la bataille et ne peut pas s'impliquer tactiquement.
            // Dans vaoc, cela pose deux probl�mes : 1) Napol�on ne pourra jamais s'engager, son bonus tactique ne pourra jamais �tre utilis�
            //2) s'il y a deux chefs de m�me niveau, les joueurs doivent normalement d�cider entre eux qui dirige le combat et qui peut �tre impliqu� tactiquement
            //il n'est pas possible de mettre en place ce "mini" forum.
            //De fait, le premier joueur arriv� dirige le combat mais tous les leaders pr�sents peuvent �tre engag�s tactiquement.
            
            //s'agit-il du m�me leader que precedemment ? Si non, l'affecter et d�sengager le leader pr�c�dent 
            //(qui pourra ainsi intervenir au niveau tactique s'il le souhaite)
            /* devenu inutile puisque les leaders peuvent toujours s'engager
            if (!ligneBataille.IsID_LEADER_012Null() && idLeader != ligneBataille.ID_LEADER_012 && ligneBataille.ID_LEADER_012 >= 0)
            {
                Donnees.TAB_BATAILLE_PIONSRow ligneBataillePion = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(ligneBataille.ID_LEADER_012, ligneBataille.ID_BATAILLE);
                ligneBataillePion.B_ENGAGEE = false;
            }
            */

            if (ligneBataille.IsID_LEADER_012Null() || idLeader != ligneBataille.ID_LEADER_012)
            {
                if (idLeader >= 0)
                {
                    ligneBataille.ID_LEADER_012 = idLeader;
                }
                else
                {
                    ligneBataille.SetID_LEADER_012Null();
                }
                Donnees.TAB_BATAILLE_PIONSRow ligneBataillePion = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(idLeader, ligneBataille.ID_BATAILLE);
                ligneBataillePion.B_ENGAGEE = false; //tous les chefs peuvent s'engager, m�me s'ils commandent, sinon il faut mettre true;
                Donnees.TAB_PIONRow LignePionLeader = Donnees.m_donnees.TAB_PION.FindByID_PION(idLeader);
                LignePionLeader.SetI_ZONE_BATAILLENull();
            }

            idLeader = TrouverLeaderBataille(ligneBataille, lignePionsEnBataille345, false);
            //s'agit-il du m�me leader que precedemment ? Si non, l'affecter et d�sengager le leader pr�c�dent 
            //(qui pourra ainsi intervenir au niveau tactique s'il le souhaite)
            /* devenu inutile puisque les leaders peuvent toujours s'engager
            if (!ligneBataille.IsID_LEADER_345Null() && idLeader != ligneBataille.ID_LEADER_345 && ligneBataille.ID_LEADER_345 >= 0)
            {
                Donnees.TAB_BATAILLE_PIONSRow ligneBataillePion = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(ligneBataille.ID_LEADER_345, ligneBataille.ID_BATAILLE);
                ligneBataillePion.B_ENGAGEE = false;
            }
            */

            if (ligneBataille.IsID_LEADER_345Null() || idLeader != ligneBataille.ID_LEADER_345)
            {
                if (idLeader >= 0)
                {
                    ligneBataille.ID_LEADER_345 = idLeader;
                }
                else
                {
                    ligneBataille.SetID_LEADER_345Null();
                }
                Donnees.TAB_BATAILLE_PIONSRow ligneBataillePion = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(idLeader, ligneBataille.ID_BATAILLE);
                ligneBataillePion.B_ENGAGEE = false;//tous les chefs peuvent s'engager, m�me s'ils commandent, sinon il faut mettre true;
                Donnees.TAB_PIONRow LignePionLeader = Donnees.m_donnees.TAB_PION.FindByID_PION(idLeader);
                LignePionLeader.SetI_ZONE_BATAILLENull();
            }

            #endregion
            //on execute une bataille que toutes les deux heures
            if (ligneBataille.I_TOUR_DEBUT == Donnees.m_donnees.TAB_PARTIE[0].I_TOUR ||
                (Donnees.m_donnees.TAB_PARTIE[0].I_TOUR - ligneBataille.I_TOUR_DEBUT) % 2 > 0)
            {
                message = string.Format("EffectuerBataille sur ID_BATAILLE={0}: Pas de combat � ce tour-ci, au prochain tour seulement.", ligneBataille.ID_BATAILLE);
                LogFile.Notifier(message, out messageErreur);
                return true;//on ne fait le combat que toutes les deux heures
            }

            //toutes les unit�s engag�es viennent de passer une heure sur le terrain, cela joue pour la fatigue de fin de journ�e
            if (!RecherchePionsEnBataille(ligneBataille, out nbUnites012Base, out nbUnites345Base, out des, out effectifs, out lignePionsEnBataille012, out lignePionsEnBataille345, true/*bengagement*/, true/*bcombattif*/))
            {
                message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille 0");
                LogFile.Notifier(message, out messageErreur);
            }

            // on v�rifie qu'il y a bien une unit� combattive engagag�e de chaque cot�, sinon, 
            /// on en choisit une au hasard qui se met en d�fense au centre
            if (0 == nbUnites012Base)
            {
                if (!RecherchePionsEnBataille(ligneBataille, out nbUnites012, out nbUnites345, out des, out effectifs, out lignePionsEnBataille012, out lignePionsEnBataille345, false/*bengagement*/, true/*bcombattif*/))
                {
                    message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille 012");
                    LogFile.Notifier(message, out messageErreur);
                }
                if (0 == nbUnites012)
                {
                    message = string.Format("EffectuerBataille : il n'y a aucune unit� combattive dans le secteur 012 pour la bataille ID_BATAILLE={0}", ligneBataille.ID_BATAILLE);
                    LogFile.Notifier(message, out messageErreur);
                    return FinDeBataille(ligneBataille);//cas d'un ordre de retraite global donn� par un joueur
                }
                if (!AffecterUneUniteAuHasardEnBataille(ligneBataille, lignePionsEnBataille012,1))
                {
                    message = string.Format("EffectuerBataille : erreur dans AffecterUneUniteAuHasardEnBataille 012");
                    LogFile.Notifier(message, out messageErreur);
                }
            }

            if (0 == nbUnites345Base)
            {
                if (!RecherchePionsEnBataille(ligneBataille, out nbUnites012, out nbUnites345, out des, out effectifs, out lignePionsEnBataille012, out lignePionsEnBataille345, false/*bengagement*/, true/*bcombattif*/))
                {
                    message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille 345");
                    LogFile.Notifier(message, out messageErreur);
                }
                if (0 == nbUnites345)
                {
                    message = string.Format("EffectuerBataille : il n'y a aucune unit� combattive dans le secteur 345 pour la bataille ID_BATAILLE={0}", ligneBataille.ID_BATAILLE);
                    LogFile.Notifier(message, out messageErreur);
                    return FinDeBataille(ligneBataille);//cas d'un ordre de retraite global donn� par un joueur
                }
                if (!AffecterUneUniteAuHasardEnBataille(ligneBataille, lignePionsEnBataille345, 4))
                {
                    message = string.Format("EffectuerBataille : erreur dans AffecterUneUniteAuHasardEnBataille 345");
                    LogFile.Notifier(message, out messageErreur);
                }
            }

            if (!RecherchePionsEnBataille(ligneBataille, out nbUnites012, out nbUnites345, out des, out effectifs, out lignePionsEnBataille012, out lignePionsEnBataille345, true/*bengagement*/, true/*bcombattif*/))//bea, false sur bcombattif avant, mais dans ce cas, donne une fausse valeur du modificateur strat�gique
            {
                message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille I");
                LogFile.Notifier(message, out messageErreur);
            }

            message = string.Format("EffectuerBataille nombre d'unit�s en d�but de combat nbUnites012={0} nbUnites345={1}", nbUnites012, nbUnites345);
            LogFile.Notifier(message, out messageErreur);
            //combattre, �a fatigue !
            foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille012)
            {
                int iZoneBataille = lignePion.IsI_ZONE_BATAILLENull() ? -1 : lignePion.I_ZONE_BATAILLE;
                message = string.Format("EffectuerBataille unit� en 012, {1} ID={0} zone={2}", lignePion.ID_PION, lignePion.S_NOM, iZoneBataille);
                LogFile.Notifier(message, out messageErreur);
                lignePion.I_NB_HEURES_COMBAT++;
            }
            foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille345)
            {
                int iZoneBataille = lignePion.IsI_ZONE_BATAILLENull() ? -1 : lignePion.I_ZONE_BATAILLE;
                message = string.Format("EffectuerBataille unit� en 345, {1} ID={0} zone={2}", lignePion.ID_PION, lignePion.S_NOM, iZoneBataille);
                LogFile.Notifier(message, out messageErreur);
                lignePion.I_NB_HEURES_COMBAT++;
            }

            //calcul du modificateur strategique
            modificateurStrategique012 = CalculModificateurStrategique(ligneBataille.ID_LEADER_012, nbUnites012);
            modificateurStrategique345 = CalculModificateurStrategique(ligneBataille.ID_LEADER_345, nbUnites345);
            message = string.Format("EffectuerBataille modificateurStrategique012={0} modificateurStrategique345={1}", modificateurStrategique012, modificateurStrategique345);
            LogFile.Notifier(message, out messageErreur);

            #region calcul des d�s par zones
            for (i = 0; i < 3; i++)
            {
                // par d�faut il y a 3 d�s par zones (on ajoute car RecherchePionsEnBataille indique d�j� les moficateurs tactiques et d'artillerie)
                if (effectifs[i] > 0) {des[i] += 3;}
                if (effectifs[i+3] > 0) {des[i + 3] += 3; }
                if (0 == effectifs[i] || 0 == effectifs[i + 3])
                {
                    message = string.Format("EffectuerBataille attaque de flanc, avant : des={0} {1} {2} {3} {4} {5}", des[0], des[1], des[2], des[3], des[4], des[5]);
                    LogFile.Notifier(message, out messageErreur);
                    message = string.Format("EffectuerBataille attaque de flanc, avant : effectifs[{0}]={1} effectifs[{2}]={3}", i, effectifs[i], i + 3, effectifs[i + 3]);
                    LogFile.Notifier(message, out messageErreur);
                    //attaque de flanc ? 
                    //Sachant que si le centre est vide , ce qui est  possible si l'unit� a rompu a un tour pr�c�dent 
                    //et que le g�n�ral a d�cid� de ne pas faire retraite), les deux cot�s sont consid�r�s comme "pris de flanc"
                    if (effectifs[i] > 0)
                    {
                        switch (i)
                        {
                            case 0:
                            case 2:
                                if (effectifs[1] > 0) { des[1] += 2; }
                                break;
                            default:
                                if (effectifs[0] > 0) {des[0] += 2;}
                                if (effectifs[2] > 0) { des[2] += 2; }
                                break;
                        }
                    }
                    if (effectifs[i + 3] > 0)
                    {
                        switch (i + 3)
                        {
                            case 3:
                            case 5:
                                if (effectifs[4] > 0) {des[4] += 2;}
                                break;
                            default:
                                if (effectifs[3] > 0) {des[3] += 2;}
                                if (effectifs[5] > 0) {des[5] += 2;}
                                break;
                        }
                    }
                    message = string.Format("EffectuerBataille attaque de flanc, apr�s : des={0} {1} {2} {3} {4} {5}", des[0], des[1], des[2], des[3], des[4], des[5]);
                    LogFile.Notifier(message, out messageErreur);
                }
                else
                {
                    message = string.Format("EffectuerBataille avant calcul des[{0}]={1} des[{2}]={3}", i, des[i], i+3, des[i+3]);
                    LogFile.Notifier(message, out messageErreur);
                    //valeur strat�gique du chef
                    des[i] += modificateurStrategique012;
                    des[i+3] += modificateurStrategique345;
                    message = string.Format("EffectuerBataille avec valeur strat�gique des[{0}]={1} des[{2}]={3}", i, des[i], i + 3, des[i + 3]);
                    LogFile.Notifier(message, out messageErreur);

                    //rapport de force, +2 pour 2/1, +3 pour 3/1 avec un maximum de +6
                    int rapport = effectifs[i] / effectifs[i + 3];
                    if (rapport >= 2) { des[i] += Math.Min(rapport, 6); }
                    rapport = effectifs[i + 3] / effectifs[i];
                    if (rapport >= 2) { des[i+3] += Math.Min(rapport, 6); }
                    message = string.Format("EffectuerBataille avec rapport de forces des[{0}]={1} effectif={2} des[{3}]={4} effectif={5}",
                        i, des[i], effectifs[i], i + 3, des[i + 3], effectifs[i+3]);
                    LogFile.Notifier(message, out messageErreur);

                    //modificateurs de terrain, appliqu�es uniquement si l'une des unit�s de la zone est en mode d�fense
                    bUniteEnDefense = false;
                    foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille012)
                    {
                        if (!lignePion.IsI_ZONE_BATAILLENull() && lignePion.I_ZONE_BATAILLE == i && lignePion.estCombattif)
                        {
                            Donnees.TAB_BATAILLE_PIONSRow ligneBataillePion = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePion.ID_PION, ligneBataille.ID_BATAILLE);
                            if (ligneBataillePion.B_EN_DEFENSE)
                            {
                                bUniteEnDefense = true;
                            }
                        }
                    }
                    if (bUniteEnDefense)
                    {
                        ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN((int)ligneBataille["ID_TERRAIN_" + Convert.ToString(i)]);
                        des[i] += ligneModeleTerrain.I_MODIFICATEUR_DEFENSE;
                    }

                    bUniteEnDefense = false;
                    foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille345)
                    {
                        if (!lignePion.IsI_ZONE_BATAILLENull() && lignePion.I_ZONE_BATAILLE == i + 3 && lignePion.estCombattif)
                        {
                            Donnees.TAB_BATAILLE_PIONSRow ligneBataillePion = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePion.ID_PION, ligneBataille.ID_BATAILLE);
                            if (ligneBataillePion.B_EN_DEFENSE)
                            {
                                bUniteEnDefense = true;
                            }
                        }
                    }
                    if (bUniteEnDefense)
                    {
                        ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN((int)ligneBataille["ID_TERRAIN_" + Convert.ToString(i + 3)]);
                        des[i + 3] += ligneModeleTerrain.I_MODIFICATEUR_DEFENSE;
                    }
                    message = string.Format("EffectuerBataille avec modificateur de terrain des[{0}]={1} des[{2}]={3}", i, des[i], i + 3, des[i + 3]);
                    LogFile.Notifier(message, out messageErreur);

                    //on a toujours 3 au minimum
                    des[i] = Math.Max(des[i], 3);
                    des[i + 3] = Math.Max(des[i + 3], 3);
                }
            }

            message = string.Format("EffectuerBataille des={0} {1} {2} {3} {4} {5}", des[0], des[1], des[2], des[3], des[4], des[5]);
            LogFile.Notifier(message, out messageErreur);

            #endregion

            //jets de d�s et pertes de moral
            for (i = 0; i < 3; i++)
            {
                if (des[i] > 0)
                {
                    score = Constantes.JetDeDes(des[i]);
                    message = string.Format("EffectuerBataille score sur zone{0}={1} sur un lanc� de {2} d�s", i, score, des[i]);
                    LogFile.Notifier(message, out messageErreur);
                    //message indiquant le niveau de pertes inflig� � l'ennemi
                    foreach (Donnees.TAB_PIONRow lignePionEnBataille in lignePionsEnBataille012)
                    {

                        if (lignePionEnBataille.B_DETRUIT || lignePionEnBataille.I_TOUR_FUITE_RESTANT > 0) { continue; }
                        if (lignePionEnBataille.IsI_ZONE_BATAILLENull() || lignePionEnBataille.I_ZONE_BATAILLE != i) { continue; }
                        if (!ClassMessager.EnvoyerMessage(lignePionEnBataille, ClassMessager.MESSAGES.MESSAGE_TIR_SUR_ENNEMI, score, ligneBataille))
                        {
                            message = string.Format("EffectuerBataille : erreur lors de l'envoi d'un message pour tir sur l'ennemi");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }
                    foreach (Donnees.TAB_PIONRow lignePionEnBataille in lignePionsEnBataille345)
                    {
                        if (!lignePionEnBataille.estCombattif) { continue; }
                        if (lignePionEnBataille.IsI_ZONE_BATAILLENull() || lignePionEnBataille.I_ZONE_BATAILLE != i + 3) { continue; }
                        lignePionEnBataille.I_MORAL -= score;
                        if (lignePionEnBataille.Moral <= 0)
                        {
                            FuiteAuCombat(ligneBataille, lignePionEnBataille, i, lignePionsEnBataille345);
                        }
                        else
                        {
                            if (score > 0)
                            {
                                if (!ClassMessager.EnvoyerMessage(lignePionEnBataille, ClassMessager.MESSAGES.MESSAGE_PERTE_MORAL_AU_COMBAT, score, ligneBataille))
                                {
                                    message = string.Format("EffectuerBataille : erreur lors de l'envoi d'un message pour perte de moral au combat");
                                    LogFile.Notifier(message, out messageErreur);
                                    return false;
                                }
                            }
                        }
                    }
                }

                if (des[i + 3] > 0)
                {
                    score = Constantes.JetDeDes(des[i + 3]);
                    message = string.Format("EffectuerBataille score sur zone{0}={1} sur un lanc� de {2} d�s", i + 3, score, des[i + 3]);
                    LogFile.Notifier(message, out messageErreur);
                    //message indiquant le niveau de pertes inflig� � l'ennemi
                    foreach (Donnees.TAB_PIONRow lignePionEnBataille in lignePionsEnBataille345)
                    {

                        if (lignePionEnBataille.B_DETRUIT || lignePionEnBataille.I_TOUR_FUITE_RESTANT>0) { continue; }
                        if (lignePionEnBataille.IsI_ZONE_BATAILLENull() || lignePionEnBataille.I_ZONE_BATAILLE != i) { continue; }
                        if (!ClassMessager.EnvoyerMessage(lignePionEnBataille, ClassMessager.MESSAGES.MESSAGE_TIR_SUR_ENNEMI, score, ligneBataille))
                        {
                            message = string.Format("EffectuerBataille : erreur lors de l'envoi d'un message pour tir sur l'ennemi");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }
                    //pertes � l'ennemi
                    foreach (Donnees.TAB_PIONRow lignePionEnBataille in lignePionsEnBataille012)
                    {
                        if (!lignePionEnBataille.estCombattif) { continue; }
                        if (lignePionEnBataille.IsI_ZONE_BATAILLENull() || lignePionEnBataille.I_ZONE_BATAILLE != i) { continue; }
                        lignePionEnBataille.I_MORAL -= score;
                        if (lignePionEnBataille.Moral <= 0)
                        {
                            FuiteAuCombat(ligneBataille, lignePionEnBataille, i + 3, lignePionsEnBataille012);
                        }
                        else
                        {
                            if (score > 0)
                            {
                                if (!ClassMessager.EnvoyerMessage(lignePionEnBataille, ClassMessager.MESSAGES.MESSAGE_PERTE_MORAL_AU_COMBAT, score, ligneBataille))
                                {
                                    message = string.Format("EffectuerBataille : erreur lors de l'envoi d'un message pour perte de moral au combat");
                                    LogFile.Notifier(message, out messageErreur);
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            
            //Y-a-t-il encore des combattants dans chaque camp ?
            if (!RecherchePionsEnBataille(ligneBataille, out nbUnites012, out nbUnites345, out des, out effectifs, out lignePionsEnBataille012, out lignePionsEnBataille345, null/*bengagement*/, true/*bcombattif*/))
            {
                message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille II");
                LogFile.Notifier(message, out messageErreur);
            }

            message = string.Format("EffectuerBataille nombre d'unit�s en fin de combat nbUnites012={0} nbUnites345={1}", nbUnites012, nbUnites345);
            LogFile.Notifier(message, out messageErreur);
            foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille012)
            {
                message = string.Format("EffectuerBataille unit�s en 012, {1} ID={0}", lignePion.ID_PION, lignePion.S_NOM);
                LogFile.Notifier(message, out messageErreur);
            }
            foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille345)
            {
                message = string.Format("EffectuerBataille unit�s en 345, {1} ID={0}", lignePion.ID_PION, lignePion.S_NOM);
                LogFile.Notifier(message, out messageErreur);
            }

            if (0 == nbUnites012 || 0 == nbUnites345)
            {
                FinDeBataille(ligneBataille);
            }
            else
            {
                //si un leader se retrouve dans une zone sans unit� combattante, il doit �tre remis en r�serve
                RecherchePionsEnBataille(ligneBataille, out nbUnites012, out nbUnites345, out des, out effectifs, out lignePionsEnBataille012, out lignePionsEnBataille345, true/*bengagement*/, false/*bcombattif*/);
                Donnees.TAB_BATAILLE_PIONSRow lignePionBataille;
                for (i = 0; i < 3; i++)
                {
                    Donnees.TAB_PIONRow ligneLeader012 = null;
                    Donnees.TAB_PIONRow ligneLeader345 = null;
                    bool bCombattants012 = false;
                    bool bCombattants345 = false;
                    foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille012)
                    {
                        if (lignePion.IsI_ZONE_BATAILLENull() || lignePion.I_ZONE_BATAILLE != i) continue;
                        if (lignePion.estCombattif) bCombattants012 = true;
                        if (lignePion.estQG) ligneLeader012 = lignePion;
                    }
                    foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille345)
                    {
                        if (lignePion.IsI_ZONE_BATAILLENull() || lignePion.I_ZONE_BATAILLE != i + 3) continue;
                        if (lignePion.estCombattif) bCombattants345 = true;
                        if (lignePion.estQG) ligneLeader345 = lignePion;
                    }
                    if (null != ligneLeader012 && !bCombattants012)
                    {
                        message = string.Format("EffectuerBataille le QG {1} ID={0} se retrouve seul en zone {2} et est remis � disposition",
                            ligneLeader012.ID_PION, ligneLeader012.S_NOM, i);
                        LogFile.Notifier(message, out messageErreur);
                        ligneLeader012.SetI_ZONE_BATAILLENull();
                        lignePionBataille = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(ligneLeader012.ID_PION, ligneBataille.ID_BATAILLE);
                        lignePionBataille.B_ENGAGEE = false; // = dans le combat mais pas au front
                    }
                    if (null != ligneLeader345 && !bCombattants345)
                    {
                        message = string.Format("EffectuerBataille le QG {1} ID={0} se retrouve seul en zone {2} et est remis � disposition",
                            ligneLeader345.ID_PION, ligneLeader345.S_NOM, i+3);
                        LogFile.Notifier(message, out messageErreur);
                        ligneLeader345.SetI_ZONE_BATAILLENull();
                        lignePionBataille = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(ligneLeader345.ID_PION, ligneBataille.ID_BATAILLE);
                        lignePionBataille.B_ENGAGEE = false;// = dans le combat mais pas au front
                    }
                }
            }

            //Envoie d'un message pr�venant d'un bruit de canon pour toutes les unit�s non pr�sentes.
            AlerteBruitDuCanon(ligneBataille);

            //g�n�ration d'un fichier de situation pour chacun des leaders

            return true;
        }

        private int TrouverLeaderBataille(Donnees.TAB_BATAILLERow ligneBataille, Donnees.TAB_PIONRow[] lignePionsEnBataille, bool bZone012)
        {
            int retourLeader = -1;
            char cHierarchie='Z';
            Donnees.TAB_PIONRow lignePionLeader=null;

            //s'il y a d�j� un leader d'affect�, on redonne, par d�faut, le m�me.
            if (bZone012 && !ligneBataille.IsID_LEADER_012Null() && ligneBataille.ID_LEADER_012 >= 0)
            {
                retourLeader = ligneBataille.ID_LEADER_012;
                lignePionLeader = Donnees.m_donnees.TAB_PION.FindByID_PION(retourLeader);
                cHierarchie=lignePionLeader.C_NIVEAU_HIERARCHIQUE;
            }
            if (!bZone012 && !ligneBataille.IsID_LEADER_345Null() && ligneBataille.ID_LEADER_345 >=0)
            {
                retourLeader = ligneBataille.ID_LEADER_345;
                lignePionLeader = Donnees.m_donnees.TAB_PION.FindByID_PION(retourLeader);
                cHierarchie=lignePionLeader.C_NIVEAU_HIERARCHIQUE;
            }

            //on regarde s'il y a un leader de niveau hierarchique sup�rieur ou si aucun leader n'est encore choisi
            foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille)
            {
                if (lignePion.estQG)
                {
                    if (lignePion.C_NIVEAU_HIERARCHIQUE == cHierarchie && retourLeader >= 0
                        && null == lignePionLeader)//si leader de m�me niveau d�j� affect� � un tour pr�c�dent, on ne le change pas
                    {
                        //une chance sur deux de prendre le nouveau chef
                        if (Constantes.JetDeDes(1) > 3)
                        {
                            retourLeader = lignePion.ID_PION;
                        }
                    }
                    if (lignePion.C_NIVEAU_HIERARCHIQUE < cHierarchie)
                    {
                        cHierarchie = lignePion.C_NIVEAU_HIERARCHIQUE;
                        retourLeader = lignePion.ID_PION;
                    }
                }
            }
            return retourLeader;
        }

        /// <summary>
        /// Quand aucune unit� n'est engag�e au combat, il faut en choisir une au hasard
        /// </summary>
        /// <param name="ligneBataille">bataille</param>
        /// <param name="lignePionsEnBataille">unit�s combattives dans la zones</param>
        /// <param name="iZone">zone d'engagement</param>
        /// <returns></returns>
        private bool AffecterUneUniteAuHasardEnBataille(Donnees.TAB_BATAILLERow ligneBataille, Donnees.TAB_PIONRow[] lignePionsEnBataille,int iZone)
        {
            Donnees.TAB_PIONRow lignePion;

            if (lignePionsEnBataille.Count() <=0) return false;
            Random de = new Random();
            lignePion=lignePionsEnBataille[de.Next(lignePionsEnBataille.Count())];
            lignePion.I_ZONE_BATAILLE=iZone;//toujours la zone centrale donc 1 ou 4
            //engagement dans la bataille
            Donnees.TAB_BATAILLE_PIONSRow lignePionBataille =Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePion.ID_PION, ligneBataille.ID_BATAILLE);
            if (null==lignePionBataille) return false;
            lignePionBataille.B_ENGAGEE=true;
            return ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_ENGAGEMENT_AUTOMATIQUE_DANS_BATAILLE, ligneBataille);
        }

        /// <summary>
        /// Pour toutes les unit�s situ�es � CST_BRUIT_DU_CANON et non inclue en bataille, elles re�oivent un message
        /// indiquant du bruit de la bataille
        /// </summary>
        /// <param name="ligneBataille">Bataille en cours</param>
        private void AlerteBruitDuCanon(Donnees.TAB_BATAILLERow ligneBataille)
        {
            double dist;
            Donnees.TAB_CASERow ligneCasePion;
            int xBataille, yBataille, i;

            //on prend le centre de la bataille comme "case" d'o� part le bruit
            xBataille=(ligneBataille.I_X_CASE_BAS_DROITE + ligneBataille.I_X_CASE_HAUT_GAUCHE)/2;
            yBataille=(ligneBataille.I_Y_CASE_BAS_DROITE + ligneBataille.I_Y_CASE_HAUT_GAUCHE)/2;
            i = 0;
            while ( i<Donnees.m_donnees.TAB_PION.Count())
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                //Pion ne se trouvant pas d�j� dans la bataille
                var resultComplet = from BataillePion in Donnees.m_donnees.TAB_BATAILLE_PIONS
                                    where (BataillePion.ID_PION == lignePion.ID_PION)
                                    && (BataillePion.ID_BATAILLE == ligneBataille.ID_BATAILLE)
                                    select BataillePion.ID_PION;

                if (0==resultComplet.Count() && lignePion.estCombattifQG(true))
                {
                    //Pion � distance correcte
                    ligneCasePion = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePion.ID_CASE);
                    dist = Constantes.Distance(ligneCasePion.I_X, ligneCasePion.I_Y, xBataille, yBataille);
                    if (dist < CST_BRUIT_DU_CANON * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE)
                    {
                        ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_BRUIT_DU_CANON, ligneBataille);
                    }
                }
                i++;
            }
        }

        private bool RecherchePionsEnBataille(Donnees.TAB_BATAILLERow ligneBataille, out int nbUnites012, out int nbUnites345, out int[] des, out int[] effectifs, out Donnees.TAB_PIONRow[] lignePionsEnBataille012, out Donnees.TAB_PIONRow[] lignePionsEnBataille345, bool? bEngagement, bool bCombattif)
        {
            des = new int[6];
            effectifs = new int[6];

            nbUnites012 = nbUnites345 = 0;
            lignePionsEnBataille012 = lignePionsEnBataille345 = null;

            if (!RecherchePionsEnBatailleParZone(ligneBataille.ID_BATAILLE, true, out nbUnites012, ref des, ref effectifs, out lignePionsEnBataille012, bEngagement, bCombattif))
            {
                return false;
            }
            if (!RecherchePionsEnBatailleParZone(ligneBataille.ID_BATAILLE, false, out nbUnites345, ref des, ref effectifs, out lignePionsEnBataille345, bEngagement, bCombattif))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Recherche tous les poins dans une zone et calcule les effectifs et les modificateurs de
        /// d�s li�s � la tactique ou � l'artillerie
        /// </summary>
        /// <param name="idBataille">identifiant de la bataille</param>
        /// <param name="bZone012">true zone 0,1,2, false zone 3,4,5</param>
        /// <param name="nbUnites">nombre d'unit�s dans la zone</param>
        /// <param name="des">modificateur sur les d�s par zones</param>
        /// <param name="effectifs">effectifs des unit�s pr�sents</param>
        /// <param name="lignePionsEnBatailleZone">liste des pions pr�sents</param>
        /// <param name="bEngagement">true si on ne prend que les unit�s engag�es, false si on prend aussi les elligibles</param>
        /// <param name="bCombattif">true si on ne prend que les unit�s combattives</param>
        /// <returns>true si ok, false si ko</returns>
        static public bool RecherchePionsEnBatailleParZone(int idBataille, bool bZone012, out int nbUnites, ref int[] des, ref int[] effectifs, out Donnees.TAB_PIONRow[] lignePionsEnBatailleZone, bool? bEngagement, bool bCombattif)
        {
            //string requete;
            //DataSetCoutDonnees.TAB_BATAILLE_PIONSRow[] lignePionsEnBataille;
            Donnees.TAB_PIONRow lignePion;
            int nb,i;
            nbUnites = 0;
            lignePionsEnBatailleZone = null;
            int idNation;
            Donnees.TAB_BATAILLERow ligneBataille = Donnees.m_donnees.TAB_BATAILLE.FindByID_BATAILLE(idBataille);

            idNation = bZone012 ? ligneBataille.ID_NATION_012 : ligneBataille.ID_NATION_345;
            //recherche toutes les unit�s engag�es dans une bataille par une nation (donc dans une zone)
            //if (bEngagement)
            //{
            //    requete = string.Format("ID_BATAILLE={1} AND B_ENGAGEE=True", idBataille);
            //}
            //else
            //{
            //    requete = string.Format("ID_BATAILLE={1} AND B_ENGAGEE=False", idBataille);
            //}
            //lignePionsEnBataille = (DataSetCoutDonnees.TAB_BATAILLE_PIONSRow[])DataSetCoutDonnees.m_donnees.TAB_BATAILLE_PIONS.Select(requete);
            /*
            var resultPionsBataille = from BataillePion in Donnees.m_donnees.TAB_BATAILLE_PIONS
                                      from Pion in Donnees.m_donnees.TAB_PION
                                      where (BataillePion.ID_PION == Pion.ID_PION) 
                                      && (BataillePion.B_ENGAGEE == true)
                                      && (0 == Pion.I_ZONE_BATAILLE || 1 == Pion.I_ZONE_BATAILLE || 2 == Pion.I_ZONE_BATAILLE)
                                      select Pion.ID_PION;
             * */
            var resultComplet= from BataillePion in Donnees.m_donnees.TAB_BATAILLE_PIONS
                                      from Pion in Donnees.m_donnees.TAB_PION
                                      where (BataillePion.ID_PION == Pion.ID_PION)
                                      //&& (BataillePion.B_ENGAGEE == bEngagement)
                                      && (BataillePion.ID_NATION == idNation)
                                      && (BataillePion.ID_BATAILLE == idBataille)
                                      && !Pion.B_DETRUIT
                                     select new rechercheQG{idPion = Pion.ID_PION,  bEngagee=BataillePion.B_ENGAGEE};

            IEnumerable<int> resultPionsBataille;
            if (null==bEngagement) 
            {
                resultPionsBataille =from resultatPion in resultComplet
                       select resultatPion.idPion;
            } 
            else
            {
                //s'il ne s'agit pas d'un QG il faut, suivant les cas, choisir les unit�es engag�es ou non
                resultPionsBataille=from resultatPion in resultComplet
                       where (resultatPion.bEngagee == bEngagement)
                       select resultatPion.idPion;
            }

            //compte le nombre de pions concern�s
            nb = 0;
            for (i = 0; i < resultPionsBataille.Count(); i++)
            {
                lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(resultPionsBataille.ElementAt(i));
                //le pion peut avoir disparu s'il a fuit et � �t� d�truit
                if (!bCombattif || lignePion.estCombattif)
                {
                    nb++;
                }
            }
            lignePionsEnBatailleZone = new Donnees.TAB_PIONRow[nb];

            //affecte la table et les valeurs
            nb = 0;
            for (i = 0; i < resultPionsBataille.Count(); i++)
            {
                lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(resultPionsBataille.ElementAt(i));
                if (null != lignePion && (!bCombattif || lignePion.estCombattif))
                {
                    lignePionsEnBatailleZone[nb++] = lignePion;

                    if (!lignePion.IsI_ZONE_BATAILLENull() && lignePion.I_ZONE_BATAILLE>=0)//cas du leader de la zone, engag� mais sans zone
                    {
                        effectifs[lignePion.I_ZONE_BATAILLE] += lignePion.I_INFANTERIE + lignePion.I_CAVALERIE;
                        des[lignePion.I_ZONE_BATAILLE] += lignePion.I_TACTIQUE + lignePion.I_EXPERIENCE;
                        des[lignePion.I_ZONE_BATAILLE] += lignePion.I_ARTILLERIE / 1000;
                    }
                    if (!lignePion.estMessager && !lignePion.estQG)
                    {
                        if (!bCombattif || lignePion.estCombattif)
                        {
                            nbUnites++;
                        }
                    }
                }
            }

            return true;
        }

        private bool FuiteAuCombat(Donnees.TAB_BATAILLERow ligneBataille, Donnees.TAB_PIONRow lignePionFuite, int zone, Donnees.TAB_PIONRow[] lignePionsEnBataille)
        {
            string message, messageErreur;
            int iPertesInfanterie, iPertesCavalerie, iPertesArtillerie;

            //lorsque qu'une unit� fuit au combat, toutes les unit�s pr�sentes dans la m�me zone, perdent CST_PERTE_MORAL_FUITE pts de moral
            message = string.Format("FuiteAuCombat : L'unit� {0} fuit la zone {1} de la bataille {2}",
                lignePionFuite.ID_PION ,zone, lignePionFuite.ID_BATAILLE);
            LogFile.Notifier(message, out messageErreur);
            foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille)
            {
                if (lignePion.ID_PION != lignePionFuite.ID_PION &&
                    !lignePion.IsI_ZONE_BATAILLENull() && //possible si l'unit� a d�j� fuit le combat
                    lignePion.I_ZONE_BATAILLE == zone && lignePion.estCombattif)
                {
                    lignePion.I_MORAL -= CST_PERTE_MORAL_FUITE;
                    if (lignePion.Moral <= 0)
                    {
                        FuiteAuCombat(ligneBataille, lignePion, zone, lignePionsEnBataille);
                    }
                }
            }

            //L'unit� perd entre 1000 et 3000 hommes en cas de fuite par perte au moral. L'infanterie prend
            //les premi�res pertes, la cavalerie ensuite.
            int pertes = (int)Math.Round(Convert.ToDecimal(Constantes.JetDeDes(1))/2,MidpointRounding.AwayFromZero);

            RepartirPertes(lignePionFuite, pertes*1000, out iPertesInfanterie, out iPertesCavalerie, out iPertesArtillerie);

            //message pour pr�venir du d�sengagement
            if (lignePionFuite.I_INFANTERIE > 0 || lignePionFuite.I_CAVALERIE > 0 || lignePionFuite.I_ARTILLERIE > 0)
            {
                if (!ClassMessager.EnvoyerMessage(lignePionFuite, ClassMessager.MESSAGES.MESSAGE_FUITE_AU_COMBAT, iPertesInfanterie, iPertesCavalerie, iPertesArtillerie, ligneBataille))
                {
                    message = string.Format("FuiteAuCombat : erreur lors de l'envoi d'un message pour fuite au combat");
                    LogFile.Notifier(message, out messageErreur);
                    return false;
                }

                //l'unit� fuit automatiquement vers son chef
                lignePionFuite.SupprimerTousLesOrdres();
                Donnees.TAB_PIONRow lignePionChef = Donnees.m_donnees.TAB_PION.FindByID_PION(lignePionFuite.ID_PION_PROPRIETAIRE);
                Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                    0,//ID_ORDRE_SUIVANT global::System.Convert.DBNull,
                    -1,///ID_ORDRE_WEB
                    Donnees.CST_ORDRE_MOUVEMENT,
                    lignePionFuite.ID_PION,
                    lignePionFuite.ID_CASE,
                    lignePionFuite.I_INFANTERIE + lignePionFuite.I_CAVALERIE + lignePionFuite.I_ARTILLERIE,
                    lignePionChef.ID_CASE,
                    -1,//id ville de destination
                    0,//I_EFFECTIF_DESTINATION
                    Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,//I_TOUR_DEBUT
                    Donnees.m_donnees.TAB_PARTIE[0].I_PHASE,//I_PHASE_DEBUT
                    0,//I_TOUR_FIN
                    0,//I_PHASE_FIN
                    0,//ID_MESSAGE
                    0,//ID_DESTINATAIRE
                    0,//null
                    Donnees.m_donnees.TAB_JEU[0].I_LEVER_DU_SOLEIL,//I_HEURE_DEBUT
                    Donnees.m_donnees.TAB_JEU[0].I_COUCHER_DU_SOLEIL - Donnees.m_donnees.TAB_JEU[0].I_LEVER_DU_SOLEIL//I_DUREE
                    );//ID_BATAILLE
                ligneOrdre.SetID_ORDRE_SUIVANTNull();
                ligneOrdre.SetID_ORDRE_WEBNull();
                ligneOrdre.SetID_MESSAGENull();
                ligneOrdre.SetID_DESTINATAIRE_MESSAGENull();
                ligneOrdre.SetID_BATAILLENull();
                ligneOrdre.SetI_TOUR_FINNull();
                ligneOrdre.SetI_PHASE_FINNull();
                ligneOrdre.SetID_NOM_DESTINATIONNull();
            }
            else
            {
                //l'unit� est d�truite
                Cartographie.DetruirePion(lignePionFuite);//� faire avant le message, sinon, non d�truite dans le message
                if (!ClassMessager.EnvoyerMessage(lignePionFuite, ClassMessager.MESSAGES.MESSAGE_FUITE_ET_DETRUITE_AU_COMBAT))
                {
                    message = string.Format("FuiteAuCombat : erreur lors de l'envoi d'un message pour fuite au combat");
                    LogFile.Notifier(message, out messageErreur);
                    return false;
                }
                //lignePionFuite.Delete();
            }
            //mise � jour dans la table des pions au combat pour l'affichage de fin de partie
            Donnees.TAB_BATAILLE_PIONSRow ligneBataillePions = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePionFuite.ID_PION, ligneBataille.ID_BATAILLE);
            if (null == ligneBataillePions)
            {
                message = string.Format("FuiteAuCombat : impossible de retrouver le pion ID=" + lignePionFuite.ID_PION + " dans la bataille ID=" + ligneBataille.ID_BATAILLE);
                LogFile.Notifier(message, out messageErreur);
                return false;
            }
            else
            {
                ligneBataillePions.B_RETRAITE = true;
                ligneBataillePions.I_INFANTERIE_FIN = lignePionFuite.I_INFANTERIE;
                ligneBataillePions.I_CAVALERIE_FIN = lignePionFuite.I_CAVALERIE;
                ligneBataillePions.I_ARTILLERIE_FIN = lignePionFuite.I_ARTILLERIE;
                ligneBataillePions.I_MORAL_FIN = lignePionFuite.I_MORAL;
                ligneBataillePions.I_FATIGUE_FIN = lignePionFuite.I_FATIGUE;
            }

            //desengagement du combat
            //lignePionFuite.SetID_BATAILLENull(); l'unit� ne quitte d�finitivement le combat que lorsque la fuite est termin�e
            //lignePionFuite.SetI_ZONE_BATAILLENull();, � ne pas faire car la zone est encore utile pour calculer les pertes par secteur
            lignePionFuite.B_FUITE_AU_COMBAT = true;//utile uniquement pour certaines conditions de victoires o� l'on compte le nombre d'unit�s d�moralis�es durant la partie
            lignePionFuite.I_MORAL = 0;
            lignePionFuite.I_TOUR_FUITE_RESTANT = 2;
            return true;
        }

        private bool RepartirPertes(Donnees.TAB_PIONRow lignePion, int pertes, out int iPertesInfanterie, out int iPertesCavalerie, out int iPertesArtillerie)
        {
            iPertesInfanterie = iPertesCavalerie = iPertesArtillerie = 0;
            if (lignePion.I_INFANTERIE >= pertes)
            {
                iPertesInfanterie = pertes;
            }
            else
            {
                iPertesInfanterie = lignePion.I_INFANTERIE;
                if (lignePion.I_CAVALERIE >= pertes - iPertesInfanterie)
                {
                    iPertesCavalerie = pertes - iPertesInfanterie;
                }
                else
                {
                    ///d�s que l'artillerie subit une perte, elle est d�truite
                    iPertesCavalerie = lignePion.I_CAVALERIE;
                    iPertesArtillerie = lignePion.I_ARTILLERIE;
                }
            }

            lignePion.I_INFANTERIE -= iPertesInfanterie;
            lignePion.I_CAVALERIE -= iPertesCavalerie;
            lignePion.I_ARTILLERIE -= iPertesArtillerie;

            if (lignePion.effectifTotal > 0)
            {
                //si l'unit� est en cours de mouvement (du a une retraite normalement) et que, du fait des pertes, 
                // tout ce qui "reste" est d�j� arriv�e en position, il faut consid�rer l'unit� comme arriv�e � destination
                Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(lignePion.ID_PION);
                if (null != ligneOrdre)
                {
                    if (ligneOrdre.I_EFFECTIF_DESTINATION >= lignePion.effectifTotal)
                    {
                        //les derniers viennent d'arriver
                        string message = string.Format("RepartirPertes {0}(ID={1}, en mouvement, les pertes font que les derniers sont arriv�s)", lignePion.S_NOM, lignePion.ID_PION);
                        LogFile.Notifier(message);
                        if (!ArriveADestination(lignePion, ligneOrdre, null)) { return false; }
                    }
                }
            }
            return true;
        }

        private bool PertesFinDeBataille(Donnees.TAB_PIONRow[] lignePionsEnBataille, Donnees.TAB_BATAILLERow ligneBataille)
        {
            string message, messageErreur;
            int pertes;
            int iPertesInfanterie, iPertesCavalerie, iPertesArtillerie;
            foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille)
            {
                if (lignePion.B_DETRUIT || lignePion.estQG) { continue; }

                //si le moral restant � l'unit� est 10+ alors, une chance sur 2 d'avoir une perte, sinon automatique
                pertes = (lignePion.Moral < 10 || Constantes.JetDeDes(1) > 3) ? 1000 : 0;

                RepartirPertes(lignePion, pertes, out iPertesInfanterie, out iPertesCavalerie, out iPertesArtillerie);
                if (lignePion.I_INFANTERIE > 0 || lignePion.I_CAVALERIE > 0 || lignePion.I_ARTILLERIE > 0)
                {
                    //pion toujours pr�sent
                    if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_PERTES_AU_COMBAT, iPertesInfanterie, iPertesCavalerie, iPertesArtillerie, ligneBataille))
                    {
                        message = string.Format("FinDeBataille : erreur lors de l'envoi d'un message MESSAGE_PERTES_AU_COMBAT");
                        LogFile.Notifier(message, out messageErreur);
                        return false;
                    }
                }
                else
                {
                    //pion d�truit
                    Cartographie.DetruirePion(lignePion);
                    if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_DETRUITE_AU_COMBAT, ligneBataille))
                    {
                        message = string.Format("FinDeBataille : erreur lors de l'envoi d'un message MESSAGE_DETRUITE_AU_COMBAT");
                        LogFile.Notifier(message, out messageErreur);
                        return false;
                    }
                    //lignePion.SupprimerTousLesOrdres();
                }
            }
            return true;
        }

        /// <summary>
        /// Fin d'une bataille soit par manque de combattants soit � cause de l'arriv�e de la nuit
        /// </summary>
        /// <param name="ligneBataille">Bataille � terminer</param>
        /// <returns>true si OK, false si KO</returns>
        private bool FinDeBataille(Donnees.TAB_BATAILLERow ligneBataille)
        {
            string message, messageErreur;
            int[] des;
            int[] effectifs;
            int nbUnites012 = 0, nbUnites345 = 0;
            Donnees.TAB_PIONRow[] lignePionsEnBataille012;
            Donnees.TAB_PIONRow[] lignePionsEnBataille345;
            Donnees.TAB_PIONRow[] lignePionsCombattifBataille012;
            Donnees.TAB_PIONRow[] lignePionsCombattifBataille345;
            string requete;

            //recherche de tous les pions pr�sents sur le champ de bataille et qui "voient" le r�sultat
            if (!RecherchePionsEnBataille(ligneBataille, out nbUnites012, out nbUnites345, out des, out effectifs,
                out lignePionsEnBataille012, out lignePionsEnBataille345, null/*engagement*/, false/*combattif*/))
            {
                message = string.Format("FinDeBataille : erreur dans RecherchePionsEnBataille I");
                LogFile.Notifier(message, out messageErreur);
            }

            //s'il n'y a aucun tour de batailles, il n'y a pas de pertes
            if (ligneBataille.I_TOUR_DEBUT != Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)
            {
                //repartition des pertes pour toutes les unit�s pr�sentes
                //les unit�s ayant fuit au combat, prennent leur perte � ce moment l�
                message = string.Format("FinDeBataille : avant les pertes au combat il reste nbUnites012={0}  nbUnites345={1}",
                    nbUnites012, nbUnites345);
                LogFile.Notifier(message, out messageErreur);
                PertesFinDeBataille(lignePionsEnBataille012, ligneBataille);
                PertesFinDeBataille(lignePionsEnBataille345, ligneBataille);
            }

            if (!RecherchePionsEnBataille(ligneBataille, out nbUnites012, out nbUnites345, out des, out effectifs,
                out lignePionsCombattifBataille012, out lignePionsCombattifBataille345, true /*engagement*/, true/*combattif*/))
            {
                message = string.Format("FinDeBataille : erreur dans RecherchePionsEnBataille II");
                LogFile.Notifier(message, out messageErreur);
            }

            message = string.Format("FinDeBataille : apr�s les pertes au combat il reste nbUnites012={0}  nbUnites345={1}",
                nbUnites012, nbUnites345);
            LogFile.Notifier(message, out messageErreur);
            if ((0 == nbUnites012 || 0 == nbUnites345) 
                && (nbUnites012 > 0 || nbUnites345 > 0)
                && (Donnees.m_donnees.TAB_PARTIE[0].I_TOUR - ligneBataille.I_TOUR_DEBUT >= 2))
            {
                //un des deux camps a remport� le combat, il engage une poursuite sur le vaincu
                if (nbUnites012 > 0)
                {
                    Poursuite(ligneBataille, ligneBataille.ID_LEADER_012, lignePionsEnBataille012, ligneBataille.ID_LEADER_345, lignePionsEnBataille345);
                    GainMoralFinDeBataille(ligneBataille.ID_BATAILLE, lignePionsCombattifBataille012);
                    EnvoyerMessagesVictoireDefaite(lignePionsEnBataille012, lignePionsEnBataille345, ligneBataille);
                }
                else
                {
                    Poursuite(ligneBataille, ligneBataille.ID_LEADER_345, lignePionsEnBataille345, ligneBataille.ID_LEADER_012, lignePionsEnBataille012);
                    GainMoralFinDeBataille(ligneBataille.ID_BATAILLE, lignePionsCombattifBataille345);
                    EnvoyerMessagesVictoireDefaite(lignePionsEnBataille345, lignePionsEnBataille012, ligneBataille);
                }
            }
            else
            {
                //fin du combat � la nuit ou s'il n'y a plus d'unit�s pr�sentes, les deux arm�es �tant pr�sentes ou absentes, il n'y a aucun bonus de moral
                FinDeBatailleEgalite(ligneBataille, lignePionsEnBataille012);
                FinDeBatailleEgalite(ligneBataille, lignePionsEnBataille345);
            }

            //desengagement de toutes les unit�s
            requete = string.Format("ID_BATAILLE={0}", ligneBataille.ID_BATAILLE);
            Donnees.TAB_PIONRow[] resPion = (Donnees.TAB_PIONRow[])Donnees.m_donnees.TAB_PION.Select(requete);
            foreach (Donnees.TAB_PIONRow lignePion in resPion)
            {
                lignePion.SetID_BATAILLENull();
                lignePion.SetI_ZONE_BATAILLENull();
                Donnees.TAB_BATAILLE_PIONSRow ligneBataillePions = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePion.ID_PION, ligneBataille.ID_BATAILLE);
                if (null == ligneBataillePions)
                {
                    message = string.Format("FinDeBataille : impossible de retrouver le pion ID=" + lignePion.ID_PION + " dans la bataille ID=" + ligneBataille.ID_BATAILLE);
                    LogFile.Notifier(message, out messageErreur);
                    return false;
                }
                else
                {
                    ligneBataillePions.I_INFANTERIE_FIN = lignePion.I_INFANTERIE;
                    ligneBataillePions.I_CAVALERIE_FIN = lignePion.I_CAVALERIE;
                    ligneBataillePions.I_ARTILLERIE_FIN = lignePion.I_ARTILLERIE;
                    ligneBataillePions.I_MORAL_FIN = lignePion.I_MORAL;
                    ligneBataillePions.I_FATIGUE_FIN = lignePion.I_FATIGUE;
                }
            }
            //suppression des lignes dans TAB_BATAILLE_PIONS
            //ne pas le faire car on doit le garder pour savoir si, en fin de journ�e, l'unit� s'est bien repos�e
            //DataSetCoutDonnees.TAB_BATAILLE_PIONSRow[] resBataillePions=(DataSetCoutDonnees.TAB_BATAILLE_PIONSRow[])DataSetCoutDonnees.m_donnees.TAB_BATAILLE_PIONS.Select(requete);
            //foreach (DataSetCoutDonnees.TAB_BATAILLE_PIONSRow lignePionBataille in resBataillePions) { lignePionBataille.Delete(); }
            
            //fin de la bataille
            //ligneBataille.Delete(); on ne peut pas le faire l� car on fait une modification de la collection sur le foreach de toutes les batailles
            //ce n'est pas bien grave de garder la  trace de la bataille de toute mani�re
            ligneBataille.I_TOUR_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
            ligneBataille.I_PHASE_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;
            return true;
        }

        private bool EnvoyerMessagesVictoireDefaite(Donnees.TAB_PIONRow[] lignePionsVictoire,Donnees.TAB_PIONRow[] lignePionsDefaite, Donnees.TAB_BATAILLERow ligneBataille)
        {
            string message;
            //message indiquant la victoire au vainqueur
            foreach (Donnees.TAB_PIONRow lignePion in lignePionsVictoire)
            {
                if (lignePion.B_DETRUIT) {continue;}
                if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_VICTOIRE_EN_BATAILLE, ligneBataille))
                {
                    message = string.Format("EnvoyerMessagesVictoireDefaite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                    LogFile.Notifier(message);
                    return false;
                }
            }
            foreach (Donnees.TAB_PIONRow lignePion in lignePionsDefaite)
            {
                if (lignePion.B_DETRUIT) { continue; }
                if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_DEFAITE_EN_BATAILLE, ligneBataille))
                {
                    message = string.Format("EnvoyerMessagesVictoireDefaite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                    LogFile.Notifier(message);
                    return false;
                }
            }
            return true;
        }

        private bool GainMoralFinDeBataille(int id_bataille, Donnees.TAB_PIONRow[] lignePionsEnBataille)
        {
            int moral;
            string message, messageErreur;

            //gain au moral pour les unit�s qui tiennent le terrain
            foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille)
            {
                if (lignePion.B_DETRUIT) { continue; }
                moral = (lignePion.I_MORAL + CST_GAIN_MORAL_MAITRE_TERRAIN > lignePion.I_MORAL_MAX) ? lignePion.I_MORAL_MAX - lignePion.I_MORAL : CST_GAIN_MORAL_MAITRE_TERRAIN;

                if (moral > 0)
                {
                    lignePion.I_MORAL = Math.Min(lignePion.I_MORAL + CST_GAIN_MORAL_MAITRE_TERRAIN, lignePion.I_MORAL_MAX);
                    if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_GAIN_MORAL_MAITRE_TERRAIN, moral, id_bataille))
                    {
                        message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                        LogFile.Notifier(message, out messageErreur);
                        return false;
                    }
                }
            }
            return true;
        }

        private bool FinDeBatailleEgalite(Donnees.TAB_BATAILLERow ligneBataille, Donnees.TAB_PIONRow[] lignePionsEnBataille)
        {
            string message, messageErreur;

            foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille)
            {
                if (lignePion.B_DETRUIT) { continue; }
                if (Donnees.m_donnees.TAB_PARTIE.Nocturne())
                {
                    if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_FIN_DE_BATAILLE_A_LA_NUIT, ligneBataille))
                    {
                        message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                        LogFile.Notifier(message, out messageErreur);
                        return false;
                    }
                }
                else
                {
                    if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_FIN_DE_BATAILLE_FAUTE_DE_COMBATTANT, ligneBataille))
                    {
                        message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                        LogFile.Notifier(message, out messageErreur);
                        return false;
                    }
                }
            }
            return true;
        }

        private bool Poursuite(Donnees.TAB_BATAILLERow ligneBataille, int idLeaderPoursuivant, Donnees.TAB_PIONRow[] lignePionsPoursuivant, int idLeaderPoursuivi, Donnees.TAB_PIONRow[] lignePionsPoursuivi)
        {
            string message, messageErreur;
            int effectifCavaleriePoursuivant, effectifCavaleriePoursuivi;
            int moralCavaleriePoursuivant;
            int moral;
            int de;
            int modifRapport;
            decimal rapport;
            int pertes, pertesCavalerie, pertesInfanterie, pertesArtillerie;
            bool bPertes;
            SortedList listePertesInfanterie= new SortedList();
            SortedList listePertesCavalerie= new SortedList();
            SortedList listePertesArtillerie = new SortedList();
            int i;

            effectifCavaleriePoursuivant = 0;  
            moralCavaleriePoursuivant = 0;
            effectifCavaleriePoursuivi=0;
            foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivant)
            {
                if (lignePion.B_DETRUIT || lignePion.estQG) { continue; }
                if (lignePion.Moral > 0 && lignePion.I_CAVALERIE > 0)
                {
                    effectifCavaleriePoursuivant += lignePion.I_CAVALERIE;
                    moralCavaleriePoursuivant += lignePion.I_CAVALERIE * lignePion.Moral;
                }
            }
            if (0==effectifCavaleriePoursuivant)
            {
                //aucune cavalerie pour faire la poursuite, on en informe les chefs ayant des unit�s dans la bataille
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivant)
                {
                    if (lignePion.B_DETRUIT) { continue; }
                    if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_AUCUNE_POURSUITE_POSSIBLE, ligneBataille))
                    {
                        message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                        LogFile.Notifier(message, out messageErreur);
                        return false;
                    }
                }

                foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivi)
                {
                    if (lignePion.B_DETRUIT) { continue; }
                    if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_AUCUNE_POURSUITE_RECUE, ligneBataille))
                    {
                        message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_RECUE");
                        LogFile.Notifier(message, out messageErreur);
                        return false;
                    }
                }
                return true;
            }
            //calcul du moral moyen
            moralCavaleriePoursuivant = moralCavaleriePoursuivant / effectifCavaleriePoursuivant;

            //calcul des effectifs de cavalerie du poursuivi
            foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivi)
            {
                if (lignePion.B_DETRUIT || lignePion.estQG) { continue; }
                effectifCavaleriePoursuivi += lignePion.I_CAVALERIE;
            }

            rapport = (0 == effectifCavaleriePoursuivi) ? 4 : Math.Round((decimal)effectifCavaleriePoursuivant / effectifCavaleriePoursuivi);
            modifRapport = 0; 
            if (rapport >0.5m) {modifRapport=1;}
            if (rapport > 1m) { modifRapport = 2; }
            if (rapport > 2m) { modifRapport = 3; }
            if (rapport > 3m) { modifRapport = 4; }

            de = Constantes.JetDeDes(1) + modifRapport -2;
            if (de < 0) de = 0;
            if (de > 9) de = 9;

            moral=(moralCavaleriePoursuivant-1)/10;
            pertes = Constantes.tablePoursuite[de, moral];
            if (0 == pertes)
            {
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivant)
                {
                    if (lignePion.B_DETRUIT) { continue; }
                    if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_POURSUITE_SANS_EFFET, ligneBataille))
                    {
                        message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_POURSUITE_SANS_EFFET");
                        LogFile.Notifier(message, out messageErreur);
                        return false;
                    }
                }
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivi)
                {
                    if (lignePion.B_DETRUIT) { continue; }
                    if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_AUCUNE_POURSUITE_RECUE, ligneBataille))
                    {
                        message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_RECUE");
                        LogFile.Notifier(message, out messageErreur);
                        return false;
                    }
                }
                return true;
            }

            //il faut repartir les pertes sur le poursuivant suivant l'ordre suivant :
            //pion avec cavalerie, moral le moins �lev�
            var result = from pionsPoursuivi in lignePionsPoursuivi.AsEnumerable()
                         orderby pionsPoursuivi.I_CAVALERIE, pionsPoursuivi.Moral
                         select pionsPoursuivi.ID_PION;

            bPertes = true;
            while (pertes > 0 && bPertes)
            {
                bPertes = false;
                foreach (var idPion in result)
                {
                    Donnees.TAB_PIONRow lignePionPerte = Donnees.m_donnees.TAB_PION.FindByID_PION(idPion);
                    if (null == lignePionPerte || lignePionPerte.B_DETRUIT || lignePionPerte.estQG) { continue; }
                    if (lignePionPerte.I_CAVALERIE > 0)
                    {
                        if (listePertesCavalerie.ContainsKey(lignePionPerte.ID_PION))
                        {
                            listePertesCavalerie[lignePionPerte.ID_PION] = (int)listePertesCavalerie[lignePionPerte.ID_PION] + 1000;
                        }
                        else
                        {
                            listePertesCavalerie.Add(lignePionPerte.ID_PION, 1000);
                        }
                        lignePionPerte.I_CAVALERIE = Math.Max(0, lignePionPerte.I_CAVALERIE-1000);
                        pertes--;
                        bPertes = true;
                    }
                    else
                    {
                        if (lignePionPerte.I_INFANTERIE > 0)
                        {
                            if (listePertesInfanterie.ContainsKey(lignePionPerte.ID_PION))
                            {
                                listePertesInfanterie[lignePionPerte.ID_PION]=(int)listePertesInfanterie[lignePionPerte.ID_PION]+ Math.Min(lignePionPerte.I_INFANTERIE, 2000);
                            }
                            else
                            {
                                listePertesInfanterie.Add(lignePionPerte.ID_PION, Math.Min(lignePionPerte.I_INFANTERIE, 2000));
                            }
                            lignePionPerte.I_INFANTERIE=Math.Max(0,lignePionPerte.I_INFANTERIE-2000);
                            pertes--;
                            bPertes = true;
                        }
                        else
                        {
                            if (lignePionPerte.I_ARTILLERIE > 0)
                            {
                                listePertesArtillerie.Add(lignePionPerte.ID_PION, lignePionPerte.I_ARTILLERIE);
                                lignePionPerte.I_ARTILLERIE = 0;
                                pertes--;
                                bPertes = true;
                            }
                        }
                    }
                    if (lignePionPerte.I_INFANTERIE > 0 || lignePionPerte.I_CAVALERIE > 0 || lignePionPerte.I_ARTILLERIE > 0)
                    {
                        //pion toujours pr�sent
                    }
                    else
                    {
                        //pion d�truit
                        Cartographie.DetruirePion(lignePionPerte);
                        if (!ClassMessager.EnvoyerMessage(lignePionPerte, ClassMessager.MESSAGES.MESSAGE_DETRUIT_PAR_POURSUITE))
                        {
                            message = string.Format("Poursuite : erreur lors de l'envoi d'un message pour destruction");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }
                }
            }

            //calcul des pertes globales
            pertesCavalerie = pertesInfanterie = pertesArtillerie = 0;
            for (i = 0; i < listePertesInfanterie.Count; i++) { pertesInfanterie += (int)listePertesInfanterie.GetByIndex(i); }
            for (i = 0; i < listePertesCavalerie.Count; i++) { pertesCavalerie += (int)listePertesCavalerie.GetByIndex(i); }
            for (i = 0; i < listePertesArtillerie.Count; i++) { pertesArtillerie += (int)listePertesArtillerie.GetByIndex(i); }

            if (pertes > 0)
            {
                //toutes les unit�s restantes ont �t� d�truites par la poursuite
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivant)
                {
                    if (lignePion.B_DETRUIT) { continue; }
                    if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_POURSUITE_DESTRUCTION_TOTALE, pertesInfanterie, pertesCavalerie, pertesArtillerie, ligneBataille))
                    {
                        message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_POURSUITE_DESTRUCTION_TOTALE");
                        LogFile.Notifier(message, out messageErreur);
                        return false;
                    }
                }
            }
            else
            {
                //message informant globalement des pertes pour le poursuivant
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivant)
                {
                    if (lignePion.B_DETRUIT) { continue; }
                    if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_POURSUITE_PERTES_POURSUIVANT, pertesInfanterie, pertesCavalerie, pertesArtillerie, ligneBataille))
                    {
                        message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_POURSUITE_PERTES_POURSUIVANT");
                        LogFile.Notifier(message, out messageErreur);
                        return false;
                    }
                }

                //message informant individuellement des pertes pour le poursuivi
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivi)
                {
                    if (lignePion.B_DETRUIT) { continue; }
                    pertesCavalerie=listePertesCavalerie.ContainsKey(lignePion.ID_PION) ? (int)listePertesCavalerie[lignePion.ID_PION] : 0;
                    pertesInfanterie = listePertesInfanterie.ContainsKey(lignePion.ID_PION) ? (int)listePertesInfanterie[lignePion.ID_PION] : 0;
                    pertesArtillerie = listePertesArtillerie.ContainsKey(lignePion.ID_PION) ? (int)listePertesArtillerie[lignePion.ID_PION] : 0;
                    if (0 == pertesCavalerie + pertesInfanterie + pertesArtillerie)
                    {
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_AUCUNE_POURSUITE_RECUE, ligneBataille))
                        {
                            message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_RECUE");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }
                    else
                    {
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_POURSUITE_PERTES_POURSUIVI, pertesInfanterie, pertesCavalerie, pertesArtillerie, ligneBataille))
                        {
                            message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_POURSUITE_PERTES_POURSUIVI");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        internal bool mise�JourInternet(string fichierCourant, out string messageErreur)
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

            iWeb.SauvegardeObjectifs(Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);

            iWeb.SauvegardeBataille(Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);

            iWeb.SauvegardeNation(Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);

            iWeb.SauvegardePartie(Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);

            iWeb.TraitementEnCours(false, Donnees.m_donnees.TAB_JEU[0].ID_JEU, Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);
            return true;
        }

        internal bool SauvegarderPartie(string nomFichier)
        {
            //Mise � jour de la version du fichier pour de future mise � jour
            Donnees.m_donnees.TAB_JEU[0].I_VERSION = 1;
            return Dal.SauvegarderPartie(nomFichier, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE, Donnees.m_donnees);
        }
    }

}
