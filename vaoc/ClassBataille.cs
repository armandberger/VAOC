using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WaocLib;

namespace vaoc
{
    partial class Donnees
    {
        partial class TAB_BATAILLEDataTable
        {
            public int ProchainID_BATAILLE
            {
                get
                {
                    if (this.Count > 0)
                    {
                        System.Nullable<int> maxId =
                            (from element in this
                             select element.ID_BATAILLE)
                            .Max();
                        return (int)maxId+1;
                    }
                    return 0;
                }
            }
        }

        partial class TAB_BATAILLERow
        {
            public bool AjouterPionDansLaBataille(Donnees.TAB_PIONRow lignePion, Donnees.TAB_CASERow ligneCaseCombat)
            {
                return AjouterPionDansLaBataille(lignePion, ligneCaseCombat, false);
            }

            /// <summary>
            /// Ajout d'un pion dans la bataille s'il est elligible
            /// </summary>
            /// <param name="lignePion">Pion à ajouter</param>
            /// <param name="ligneCaseCombat">Case sur laquelle l'unité est devenue elligible</param>
            /// <param name="uniteCreantLaBataille">true si l'unité est celle à l'origine de la bataille, false sinon</param>
            /// <returns>false en cas d'erreur de traitement, true sinon</returns>
            public bool AjouterPionDansLaBataille(Donnees.TAB_PIONRow lignePion, Donnees.TAB_CASERow ligneCaseCombat, bool uniteCreantLaBataille)
            {
                string message, messageErreur;
                bool bEnDefense;

                //si l'unité est déjà dans un combat ou est en fuite ou n'a plus de moral, il ne faut pas l'ajouter
                if (!uniteCreantLaBataille && (lignePion.estAuCombat || !lignePion.estCombattifQG(true, false)))// && 0==lignePion.effectifTotal) si on met la condition précédente, une unité en retraite devient elligible sur un combat
                {
                    return true;
                }

                //si l'unité n'est pas déjà elligible dans la bataille, il faut l'ajouter
                Donnees.TAB_BATAILLE_PIONSRow ligneBataillePions = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePion.ID_PION, ID_BATAILLE);
                if (null == ligneBataillePions)
                {
                    Donnees.TAB_MODELE_PIONRow ligneModele = lignePion.modelePion;
                    //si le pion n'est pas en mouvement, il est considéré en défense
                    Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(lignePion.ID_PION);
                    //bEnDefense = (null == ligneOrdre && !lignePion.estQG) ? true : false; -> ne tenait pas du compte du fait si l'unité était en mouvement à l'heure de la rencontre
                    if (null == ligneOrdre)
                    {
                        bEnDefense = true;
                    }
                    else
                    {
                        bEnDefense = !lignePion.OrdreActif(ligneOrdre);
                    }

                    if (null == Donnees.m_donnees.TAB_BATAILLE_PIONS.AddTAB_BATAILLE_PIONSRow(
                        ID_BATAILLE, lignePion.ID_PION, ligneModele.ID_NATION, false, bEnDefense,
                        lignePion.I_INFANTERIE,
                        -1,
                        lignePion.I_CAVALERIE,
                        -1,
                        lignePion.I_ARTILLERIE,
                        -1,
                        lignePion.I_MORAL,
                        -1,
                        lignePion.I_FATIGUE,
                        -1,
                        false,
                        false,
                        -1
                    ))
                    {
                        message = string.Format("AjouterPionDansLaBataille : impossible d'ajouter le pion {0} à la bataille {1}", lignePion.ID_PION, ID_BATAILLE);
                        LogFile.Notifier(message, out messageErreur);
                        return false;//problème à l'ajout
                    }
                    lignePion.ID_BATAILLE = ID_BATAILLE;

                    //il faut terminer tout ordre en cours d'execution par l'unité et placer l'unité dans la case courante du combat
                    lignePion.ID_CASE = ligneCaseCombat.ID_CASE;
                    lignePion.SupprimerTousLesOrdres();
                    lignePion.PlacerStatique();

                    //il faut envoyer un message au propriétaire de l'unité pour indiquer cet ajout
                    ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_ARRIVEE_DANS_BATAILLE, this);

                    message = string.Format("AjouterPionDansLaBataille : ajout du pion {0} à la bataille {1}, defensif={2} sur idcase={3}",
                        lignePion.ID_PION, ID_BATAILLE, bEnDefense, lignePion.ID_CASE);
                    LogFile.Notifier(message, out messageErreur);
                }
                return true;
            }

            public bool FinDeBataille(out bool bFinDeBataille)
            {
                return FinDeBataille(false, false, out bFinDeBataille);
            }

            /// <summary>
            /// Fin d'une bataille soit par manque de combattants soit à cause de l'arrivée de la nuit
            /// </summary>
            /// <param name="ligneBataille">Bataille à terminer</param>
            /// <returns>true si OK, false si KO</returns>
            public bool FinDeBataille(bool bRetraite012, bool bRetraite345, out bool bFinDeBataille)
            {
                string message, messageErreur;
                int[] des;
                int[] effectifs;
                int[] canons;
                int nbUnites012 = 0, nbUnites345 = 0;
                Donnees.TAB_PIONRow[] lignePionsEnBataille012;
                Donnees.TAB_PIONRow[] lignePionsEnBataille345;
                Donnees.TAB_PIONRow[] lignePionsCombattifBataille012;
                Donnees.TAB_PIONRow[] lignePionsCombattifBataille345;
                string requete;
                bool bVictoire012 = true, bVictoire345 = true;

                bFinDeBataille = false;
                //fin de la bataille, toutes les unités engagées gagnent de l'expérience
                I_TOUR_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                I_PHASE_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;

                //Donnees.TAB_PIONRow lignePionTest = Donnees.m_donnees.TAB_PION.FindByID_PION(20);
                //recherche de tous les pions présents sur le champ de bataille et qui "voient" le résultat et peuvent subir une poursuite
                if (!RecherchePionsEnBataille(out nbUnites012, out nbUnites345, out des, out effectifs, out canons,
                    out lignePionsEnBataille012, out lignePionsEnBataille345,
                    true,//engagement
                    false//combattif
                    ))
                {
                    message = string.Format("FinDeBataille : erreur dans RecherchePionsEnBataille I");
                    LogFile.Notifier(message, out messageErreur);
                }

                /* Cela n'a plus d'intérêt en règles avancées, les pertes sont calculées directement durant le combat
                //s'il n'y a aucun tour de batailles, il n'y a pas de pertes
                if (ligneBataille.I_TOUR_DEBUT != Donnees.m_donnees.TAB_PARTIE[0].I_TOUR)
                {
                    //repartition des pertes pour toutes les unités présentes
                    //les unités ayant fuit au combat, prennent leur perte à ce moment là
                    message = string.Format("FinDeBataille : avant les pertes au combat il reste nbUnites012={0}  nbUnites345={1}",
                        nbUnites012, nbUnites345);
                    LogFile.Notifier(message, out messageErreur);
                    // Une unité ne subit des pertes que s'il se trouve, en face, une unité qui a été engagée
                    var unitesEngagees = from BataillePion in Donnees.m_donnees.TAB_BATAILLE_PIONS
                                        from Pion in Donnees.m_donnees.TAB_PION
                                        where (BataillePion.ID_PION == Pion.ID_PION)
                                            && (BataillePion.B_ENGAGEE)
                                             //&& (BataillePion.I_MORAL_DEBUT > 0)
                                        && (BataillePion.ID_NATION == ligneBataille.ID_NATION_345)
                                        && (BataillePion.ID_BATAILLE == ligneBataille.ID_BATAILLE)
                                        select new rechercheQG { idPion = Pion.ID_PION, bEngagee = BataillePion.B_ENGAGEE };

                    if (unitesEngagees.Count() > 0)
                    {
                        PertesFinDeBataille(lignePionsEnBataille012, ligneBataille);
                    }

                    unitesEngagees = from BataillePion in Donnees.m_donnees.TAB_BATAILLE_PIONS
                                         from Pion in Donnees.m_donnees.TAB_PION
                                         where (BataillePion.ID_PION == Pion.ID_PION)
                                            && (BataillePion.B_ENGAGEE)
                                         //&& (BataillePion.I_MORAL_DEBUT > 0)
                                         && (BataillePion.ID_NATION == ligneBataille.ID_NATION_012)
                                         && (BataillePion.ID_BATAILLE == ligneBataille.ID_BATAILLE)
                                         select new rechercheQG { idPion = Pion.ID_PION, bEngagee = BataillePion.B_ENGAGEE };
                    if (unitesEngagees.Count() > 0)
                    {
                        PertesFinDeBataille(lignePionsEnBataille345, ligneBataille);
                    }
                }
                */

                if (!RecherchePionsEnBataille(out nbUnites012, out nbUnites345, out des, out effectifs, out canons,
                    out lignePionsCombattifBataille012, out lignePionsCombattifBataille345, true /*engagement*/, true/*combattif*/))
                {
                    message = string.Format("FinDeBataille : erreur dans RecherchePionsEnBataille II");
                    LogFile.Notifier(message, out messageErreur);
                }

                message = string.Format("FinDeBataille : après les pertes au combat il reste nbUnites012={0}  nbUnites345={1}",
                    nbUnites012, nbUnites345);
                LogFile.Notifier(message, out messageErreur);
                if ((0 == nbUnites012 || 0 == nbUnites345)
                    && (nbUnites012 > 0 || nbUnites345 > 0)
                    && (Donnees.m_donnees.TAB_PARTIE[0].I_TOUR - I_TOUR_DEBUT >= 2))
                {
                    //un des deux camps a remporté le combat, il engage une poursuite sur le vaincu
                    if (nbUnites012 > 0)
                    {
                        Poursuite(ID_LEADER_012, lignePionsEnBataille012, ID_LEADER_345, lignePionsEnBataille345);
                        SortieDuChampDeBataille(lignePionsEnBataille345);
                        GainMoralFinDeBataille(lignePionsCombattifBataille012);
                        EnvoyerMessagesVictoireDefaite(lignePionsEnBataille012, lignePionsEnBataille345);
                        //victoireCombat = VICTOIRECOMBAT.VICTOIRE012;
                        bVictoire345 = false;
                    }
                    else
                    {
                        Poursuite( ID_LEADER_345, lignePionsEnBataille345, ID_LEADER_012, lignePionsEnBataille012);
                        SortieDuChampDeBataille(lignePionsEnBataille012);
                        GainMoralFinDeBataille(lignePionsCombattifBataille345);
                        EnvoyerMessagesVictoireDefaite(lignePionsEnBataille345, lignePionsEnBataille012);
                        //victoireCombat = VICTOIRECOMBAT.VICTOIRE345;
                        bVictoire012 = false;
                    }
                }
                else
                {
                    //victoireCombat = VICTOIRECOMBAT.EGALITE;
                    //fin du combat à la nuit ou s'il n'y a plus d'unités présentes, les deux armées étant présentes ou absentes, il n'y a aucun bonus de moral
                    FinDeBatailleEgalite(lignePionsEnBataille012);
                    FinDeBatailleEgalite(lignePionsEnBataille345);
                }

                //desengagement de toutes les unités
                requete = string.Format("ID_BATAILLE={0}", ID_BATAILLE);
                Donnees.TAB_PIONRow[] resPion = (Donnees.TAB_PIONRow[])Donnees.m_donnees.TAB_PION.Select(requete);
                foreach (Donnees.TAB_PIONRow lignePion in resPion)
                {
                    lignePion.SetID_BATAILLENull();
                    lignePion.SetI_ZONE_BATAILLENull();
                    Donnees.TAB_BATAILLE_PIONSRow ligneBataillePions = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePion.ID_PION, ID_BATAILLE);
                    if (null == ligneBataillePions)
                    {
                        message = string.Format("FinDeBataille : impossible de retrouver le pion ID=" + lignePion.ID_PION + " dans la bataille ID=" + ID_BATAILLE);
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

                        if (lignePion.estQG || lignePion.estConvoi)
                        {
                            //remet les zones du dernier message reçu à vide, un peu débile mais sinon l'unité est toujours marquée comme engagée !
                            //Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageRecu(lignePion.ID_PION);
                            //ligneMessage.SetID_BATAILLENull();
                            //ligneMessage.SetI_ZONE_BATAILLENull();

                            //A la place, j'envoie un message de position
                            if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_POSITION))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (I_TOUR_FIN - I_TOUR_DEBUT >= 4)
                            {
                                lignePion.GainExperienceFinDeBataille();
                            }

                            if (!PertesFinDeBataille(lignePion, ligneBataillePions, 
                                (ligneBataillePions.I_ZONE_BATAILLE_ENGAGEMENT > 2) ? bVictoire345 : bVictoire012,
                                (ligneBataillePions.I_ZONE_BATAILLE_ENGAGEMENT > 2) ? lignePionsCombattifBataille012 : lignePionsCombattifBataille345))
                            {
                                message = string.Format("FinDeBataille : erreur dans FinDeBataille !QG");
                                LogFile.Notifier(message, out messageErreur);
                                return false;
                            }
                        }
                    }
                }

                //suppression des lignes dans TAB_BATAILLE_PIONS
                //ne pas le faire car on doit le garder pour savoir si, en fin de journée, l'unité s'est bien reposée
                //DataSetCoutDonnees.TAB_BATAILLE_PIONSRow[] resBataillePions=(DataSetCoutDonnees.TAB_BATAILLE_PIONSRow[])DataSetCoutDonnees.m_donnees.TAB_BATAILLE_PIONS.Select(requete);
                //foreach (DataSetCoutDonnees.TAB_BATAILLE_PIONSRow lignePionBataille in resBataillePions) { lignePionBataille.Delete(); }
                bFinDeBataille = true;
                return true;
            }

            /// <summary>
            /// Toutes les unités du camp perdant la bataille sont "éjéctées" de la zone de combat
            /// Ceci afin d'éviter qu'un nouveau contact entre troupes n'aient lieu dés la fin d'un combat
            /// Mais aussi pour laisser une chance au perdant de pouvoir s'enfuir. les deux heures ne sont pas suffisantes
            /// les troupes ne pouvant se déplacer qu'en "ligne" sur la route, c'est assez long
            /// </summary>
            /// <param name="lignePionsEnBataille">liste des pions dans la bataille et faisant partie des vaincus</param>
            private bool SortieDuChampDeBataille(Donnees.TAB_PIONRow[] lignePionsEnBataille)
            {
                double cout, coutHorsRoute;
                AstarTerrain[] tableCoutsMouvementsTerrain;
                List<Donnees.TAB_CASERow> chemin;
                string message, messageErreur;
                int i;

                //Deux cas possibles, soit l'unité est en mouvement (possible pour les unités non engagées) et on "avance" l'unité jusqu'à ce qu'elle
                //sorte du champ de bataille
                //soit elle est fixe et on la repositionne sur le bord le plus proche. Chose que l'on fait également si le mouvement
                //ne conduit pas hors du champ de bataille

                //recherche de toutes les cases du champ de bataille
                var listeCasesBataille = from Case in Donnees.m_donnees.TAB_CASE
                                         where (I_X_CASE_BAS_DROITE >= Case.I_X && I_Y_CASE_BAS_DROITE >= Case.I_Y
                                                && I_X_CASE_HAUT_GAUCHE <= Case.I_X && I_Y_CASE_HAUT_GAUCHE <= Case.I_Y)
                                         select Case;

                foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille)
                {
                    if (lignePion.B_DETRUIT) { continue; }
                    //si l'unité n'a aucune position dans le champ de bataille, cela ne sert à rien de la bouger !
                    var listeCasesOccupees = from Case in listeCasesBataille
                                             where (!Case.IsID_PROPRIETAIRENull() && Case.ID_PROPRIETAIRE == lignePion.ID_PION)
                                             || (!Case.IsID_NOUVEAU_PROPRIETAIRENull() && Case.ID_NOUVEAU_PROPRIETAIRE == lignePion.ID_PION)
                                             select Case;
                    if (0 == listeCasesOccupees.Count()) { continue; }

                    bool bRepositionSurBord = false;
                    Donnees.TAB_NATIONRow ligneNation = lignePion.nation; // Donnees.m_donnees.TAB_PION.TrouveNation(lignePion);
                    Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(lignePion.ID_PION);
                    if (null != ligneOrdre)
                    {
                        //On recherche le point du parcours où l'on va sortir de la zone de combat
                        Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneOrdre.ID_CASE_DESTINATION);
                        Donnees.TAB_CASERow ligneCaseDepart = (lignePion.effectifTotal > 0) ? Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneOrdre.ID_CASE_DEPART) : Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePion.ID_CASE);

                        if (!Cartographie.RechercheChemin(Constantes.TYPEPARCOURS.MOUVEMENT, lignePion, ligneCaseDepart, ligneCaseDestination, ligneOrdre, out chemin, out cout, out coutHorsRoute, out tableCoutsMouvementsTerrain, out messageErreur))
                        {
                            message = string.Format("{0}(ID={1}, erreur sur RechercheChemin dans SortieDuChampDeBataille: {2})", lignePion.S_NOM, lignePion.ID_PION, messageErreur);
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }

                        i = 0;
                        while (i < chemin.Count && (I_X_CASE_BAS_DROITE != chemin[i].I_X && I_Y_CASE_BAS_DROITE != chemin[i].I_Y
                                    && I_X_CASE_HAUT_GAUCHE != chemin[i].I_X && I_Y_CASE_HAUT_GAUCHE != chemin[i].I_Y))
                        {
                            i++;
                        }
                        if (i == chemin.Count)
                        {
                            //l'ordre de mouvement de l'unité ne l'a fait pas sortir du champ de bataille, il faut donc la remettre sur un bord
                            bRepositionSurBord = true;
                        }
                        else
                        {
                            //s'il n'y a pas d'effectifs, c'est facile ;-)
                            if (lignePion.effectifTotal > 0)
                            {
                                //on reporte en effectifs de départ, les hommes qui se trouvent sur le chemin entre la case de départ et la case de sortie
                                //de la zone de bataille
                                //on calcule les effectifs qui ne sont plus au départ
                                int iInfanterie, iCavalerie, iArtillerie;
                                lignePion.CalculerEffectif(ligneNation, i, true, out iInfanterie, out iCavalerie, out iArtillerie);
                                message = string.Format("SortieDuChampDeBataille :effectif en route: i={0} c={1} a={2} encombrement={3}", iInfanterie, iCavalerie, iArtillerie, i);
                                LogFile.Notifier(message, out messageErreur);

                                ligneOrdre.I_EFFECTIF_DEPART = Math.Min(lignePion.effectifTotalEnMouvement, ligneOrdre.I_EFFECTIF_DEPART + iInfanterie + iCavalerie + iArtillerie);
                            }
                            lignePion.ID_CASE = chemin[i].ID_CASE;
                            ligneOrdre.ID_CASE_DEPART = chemin[i].ID_CASE;
                            lignePion.DetruireEspacePion();//force, par la suite, le recalcul de tous les espaces et le parcours.
                            lignePion.PlacementPion(ligneNation, true);
                        }
                    }
                    else
                    {
                        bRepositionSurBord = true;
                    }
                    if (bRepositionSurBord)
                    {
                        //on place l'unité sur le bord de la zone de combat le plus proche et qui ne soit pas occupée par l'ennemi
                        //on prend le coté qui a le moins de cases occupées par l'ennemi, et, en cas d'égalité, le plus de cases occupées par
                        //des amis
                        Donnees.TAB_CASERow ligneCasePion = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePion.ID_CASE);
                        Donnees.TAB_CASERow ligneCaseSortie = null;
                        int nbCasesOccupeesParEnnemi = int.MaxValue, nbCasesOccupeesParAmis = 0;

                        //coté haut
                        RechercheCaseDeSortie(false,
                            I_X_CASE_HAUT_GAUCHE, I_X_CASE_BAS_DROITE, I_Y_CASE_HAUT_GAUCHE,
                            I_X_CASE_HAUT_GAUCHE, I_Y_CASE_HAUT_GAUCHE,
                            I_X_CASE_BAS_DROITE, I_Y_CASE_HAUT_GAUCHE + (I_Y_CASE_BAS_DROITE - I_Y_CASE_HAUT_GAUCHE) / 2,
                            lignePion, ligneCasePion, ref ligneCaseSortie, ref nbCasesOccupeesParEnnemi, ref nbCasesOccupeesParAmis);
                        //coté bas
                        RechercheCaseDeSortie(false,
                            I_X_CASE_HAUT_GAUCHE, I_X_CASE_BAS_DROITE, I_Y_CASE_BAS_DROITE,
                            I_X_CASE_HAUT_GAUCHE, I_Y_CASE_HAUT_GAUCHE + (I_Y_CASE_BAS_DROITE - I_Y_CASE_HAUT_GAUCHE) / 2,
                            I_X_CASE_BAS_DROITE, I_Y_CASE_BAS_DROITE,
                            lignePion, ligneCasePion, ref ligneCaseSortie, ref nbCasesOccupeesParEnnemi, ref nbCasesOccupeesParAmis);
                        //coté gauche
                        RechercheCaseDeSortie(true,
                            I_Y_CASE_HAUT_GAUCHE, I_Y_CASE_BAS_DROITE, I_X_CASE_HAUT_GAUCHE,
                            I_X_CASE_HAUT_GAUCHE, I_Y_CASE_HAUT_GAUCHE,
                            I_X_CASE_HAUT_GAUCHE + (I_X_CASE_BAS_DROITE - I_X_CASE_HAUT_GAUCHE) / 2, I_Y_CASE_BAS_DROITE,
                            lignePion, ligneCasePion, ref ligneCaseSortie, ref nbCasesOccupeesParEnnemi, ref nbCasesOccupeesParAmis);
                        //coté droit
                        RechercheCaseDeSortie(true,
                            I_Y_CASE_HAUT_GAUCHE, I_Y_CASE_BAS_DROITE, I_X_CASE_BAS_DROITE,
                            I_X_CASE_HAUT_GAUCHE + (I_X_CASE_BAS_DROITE - I_X_CASE_HAUT_GAUCHE) / 2, I_Y_CASE_HAUT_GAUCHE,
                            I_X_CASE_BAS_DROITE, I_Y_CASE_BAS_DROITE,
                            lignePion, ligneCasePion, ref ligneCaseSortie, ref nbCasesOccupeesParEnnemi, ref nbCasesOccupeesParAmis);
                        if (null == ligneCaseSortie)
                        {
                            message = string.Format("SortieDuChampDeBataille : impossible de trouver une case de sortie pour le pion ID=" + lignePion.ID_PION + " dans la bataille ID=" + ID_BATAILLE);
                            LogFile.Notifier(message);
                            return false;
                        }
                        // On teleporte le pion à sa nouvelle position
                        lignePion.ID_CASE = ligneCaseSortie.ID_CASE;
                        lignePion.SupprimerTousLesOrdres();
                        lignePion.DetruireEspacePion();//force, par la suite, le recalcul de tous les espaces, parcours, etc.
                        lignePion.PlacementPion(ligneNation, true);
                    }
                }
                return true;
            }

            private void RechercheCaseDeSortie(bool bVertical, int minimum, int maximum, int autreCoordonnee,
                int xCaseHautGauche, int yCaseHautGauche, int xCaseBasDroite, int yCaseBasDroite,
                Donnees.TAB_PIONRow lignePion, Donnees.TAB_CASERow ligneCasePion,
                ref Donnees.TAB_CASERow ligneCaseSortie, ref int nbCasesOccupeesParEnnemis, ref int nbCasesOccupeesParAmis)
            {
                Donnees.TAB_CASERow ligneCase;
                double distanceCaseSortie = double.MaxValue;

                var listeCasesOccupeesParEnnemis = from Case in Donnees.m_donnees.TAB_CASE
                                                   where (xCaseBasDroite >= Case.I_X && yCaseBasDroite >= Case.I_Y
                                                 && xCaseHautGauche <= Case.I_X && yCaseHautGauche <= Case.I_Y
                                                 && Case.EstOccupeeParEnnemi(lignePion))
                                                   select Case;

                int nblisteCasesOccupeesParEnnemis = listeCasesOccupeesParEnnemis.Count();
                var listeT = from Case in Donnees.m_donnees.TAB_CASE
                             where (xCaseBasDroite >= Case.I_X && yCaseBasDroite >= Case.I_Y
                           && xCaseHautGauche <= Case.I_X && yCaseHautGauche <= Case.I_Y
                           )
                             select Case;
                int nbT = listeT.Count();

                if (nblisteCasesOccupeesParEnnemis > nbCasesOccupeesParEnnemis)
                {
                    return; //la solution précedente est meilleure
                }
                nbT = nbT + 1;
                //deuxième critère, le nombre de cases occupées par les amis
                var listeCasesOccupeesParAmis = from Case in Donnees.m_donnees.TAB_CASE
                                                where (xCaseBasDroite >= Case.I_X && yCaseBasDroite >= Case.I_Y
                                            && xCaseHautGauche <= Case.I_X && yCaseHautGauche <= Case.I_Y
                                            && Case.EstOccupeeParAmi(lignePion))
                                                select Case;
                int nblisteCasesOccupeesParAmis = listeCasesOccupeesParAmis.Count();
                if (listeCasesOccupeesParAmis.Count() == nblisteCasesOccupeesParAmis)
                {
                    if (listeCasesOccupeesParAmis.Count() < nbCasesOccupeesParAmis)
                    {
                        return; //la solution précedente est meilleure
                    }
                }
                nbCasesOccupeesParEnnemis = nblisteCasesOccupeesParEnnemis;
                nbCasesOccupeesParAmis = nblisteCasesOccupeesParAmis;

                for (int i = minimum; i < maximum; i++)
                {
                    ligneCase = bVertical ? Donnees.m_donnees.TAB_CASE.FindByXY(autreCoordonnee, i) : Donnees.m_donnees.TAB_CASE.FindByXY(i, autreCoordonnee);
                    if (!ligneCase.EstOccupeeOuBloqueParEnnemi(lignePion, false) && ligneCase.EstMouvementPossible())
                    {
                        double distance = Constantes.Distance(ligneCase.I_X, ligneCase.I_Y, ligneCasePion.I_X, ligneCasePion.I_Y);
                        if (distance < distanceCaseSortie)
                        {
                            distanceCaseSortie = distance;
                            ligneCaseSortie = ligneCase;
                        }
                    }
                }
            }

            private bool EnvoyerMessagesVictoireDefaite(Donnees.TAB_PIONRow[] lignePionsVictoire, Donnees.TAB_PIONRow[] lignePionsDefaite)
            {
                string message;
                //message indiquant la victoire au vainqueur
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsVictoire)
                {
                    if (lignePion.B_DETRUIT) { continue; }
                    if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_VICTOIRE_EN_BATAILLE, this))
                    {
                        message = string.Format("EnvoyerMessagesVictoireDefaite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                        LogFile.Notifier(message);
                        return false;
                    }
                }
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsDefaite)
                {
                    if (lignePion.B_DETRUIT) { continue; }
                    if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_DEFAITE_EN_BATAILLE, this))
                    {
                        message = string.Format("EnvoyerMessagesVictoireDefaite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                        LogFile.Notifier(message);
                        return false;
                    }
                }
                return true;
            }

            private bool GainMoralFinDeBataille(Donnees.TAB_PIONRow[] lignePionsEnBataille)
            {
                int moral;
                string message, messageErreur;

                //gain au moral pour les unités qui tiennent le terrain
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille)
                {
                    if (lignePion.B_DETRUIT) { continue; }
                    moral = (lignePion.I_MORAL + Constantes.CST_GAIN_MORAL_MAITRE_TERRAIN > lignePion.I_MORAL_MAX) ? lignePion.I_MORAL_MAX - lignePion.I_MORAL : Constantes.CST_GAIN_MORAL_MAITRE_TERRAIN;

                    if (moral > 0)
                    {
                        lignePion.I_MORAL = Math.Min(lignePion.I_MORAL + Constantes.CST_GAIN_MORAL_MAITRE_TERRAIN, lignePion.I_MORAL_MAX);
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_GAIN_MORAL_MAITRE_TERRAIN, moral, this))
                        {
                            message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }
                }
                return true;
            }

            private bool FinDeBatailleEgalite(Donnees.TAB_PIONRow[] lignePionsEnBataille)
            {
                string message, messageErreur;

                foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille)
                {
                    if (lignePion.B_DETRUIT) { continue; }
                    if (Donnees.m_donnees.TAB_PARTIE.Nocturne())
                    {
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_FIN_DE_BATAILLE_A_LA_NUIT, this))
                        {
                            message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }
                    else
                    {
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_FIN_DE_BATAILLE_FAUTE_DE_COMBATTANT, this))
                        {
                            message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }
                }
                return true;
            }

            private bool Poursuite(int idLeaderPoursuivant, Donnees.TAB_PIONRow[] lignePionsPoursuivant, int idLeaderPoursuivi, Donnees.TAB_PIONRow[] lignePionsPoursuivi)
            {
                string message, messageErreur;
                int effectifCavaleriePoursuivant, effectifCavaleriePoursuivi;
                int moralCavaleriePoursuivant;
                int moral;
                int de;
                int modifRapport;
                decimal rapport;
                int pertes, pertesCavalerieTotal, pertesInfanterieTotal, pertesArtillerieTotal, pertesRavitaillementTotal, pertesMaterielTotal;
                int pertesMoral, pertesRavitaillement, pertesMateriel, pourcentagePertesArtillerie;
                bool bPertes, bPremierPassage;
                SortedList listePertesInfanterie = new SortedList();
                SortedList listePertesCavalerie = new SortedList();
                SortedList listePertesArtillerie = new SortedList();
                SortedList listePertesMoral = new SortedList();
                SortedList listePertesMateriel = new SortedList();
                SortedList listePertesRavitaillement = new SortedList();
                int effectifPoursuivantTotal, artilleriePoursuivantTotal;
                int i;

                //aucune poursuite si la bataille se termine à la tombée de la nuit
                if (Donnees.m_donnees.TAB_PARTIE.Nocturne())
                {
                    message = string.Format("Poursuite : {0} aucune poursuite à cause de la nuit", S_NOM);
                    LogFile.Notifier(message);
                    return true;
                }

                effectifCavaleriePoursuivant = 0;
                moralCavaleriePoursuivant = 0;
                effectifCavaleriePoursuivi = 0;
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivant)
                {
                    if (lignePion.B_DETRUIT || lignePion.estQG) { continue; }
                    if (lignePion.Moral >= Constantes.CST_LIMITE_MORAL_CAVALERIE_POURSUITE && lignePion.cavalerie > 0)
                    {
                        //Dans la règle, toute unité de cavalierie ayant moins de CST_LIMITE_MORAL_CAVALERIE_POURSUITE (20) ne peut pas poursuivre
                        //Toute unité de cavalerie qui n'a pas été engagée comme pour sa totalité, sinon pour la moitié
                        // J'ai remplacé non engagé par moral au maximum car dans VAOC une unité peut ne pas être engagée uniquement parce qu'elle souhaite poursuivre son mouvement
                        // -> C'est un écart à indiquer dans le fichier d'aide
                        int nombreCavaliersPoursuivants = (lignePion.Moral == lignePion.I_MORAL_MAX) ? lignePion.cavalerie : lignePion.cavalerie / 2;
                        effectifCavaleriePoursuivant += nombreCavaliersPoursuivants;
                        moralCavaleriePoursuivant += nombreCavaliersPoursuivants * lignePion.Moral;
                        message = string.Format("Poursuite : ID={0} avec {1} cavaliers et {2} de moral", lignePion.ID_PION, effectifCavaleriePoursuivant, lignePion.Moral);
                        LogFile.Notifier(message);
                    }
                }

                if (0 == effectifCavaleriePoursuivant)
                {
                    #region aucune cavalerie pour faire la poursuite, on en informe les chefs ayant des unités dans la bataille
                    foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivant)
                    {
                        if (lignePion.B_DETRUIT) { continue; }
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_AUCUNE_POURSUITE_POSSIBLE, this))
                        {
                            message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }

                    foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivi)
                    {
                        if (lignePion.B_DETRUIT) { continue; }
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_AUCUNE_POURSUITE_RECUE, this))
                        {
                            message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_RECUE");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }
                    return true;
                    #endregion
                }

                //calcul du moral moyen
                moralCavaleriePoursuivant = moralCavaleriePoursuivant / effectifCavaleriePoursuivant;
                LogFile.Notifier("Poursuite : moralCavaleriePoursuivant = " + moralCavaleriePoursuivant);

                //calcul des effectifs de cavalerie du poursuivi
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivi)
                {
                    if (lignePion.B_DETRUIT || lignePion.estQG) { continue; }
                    effectifCavaleriePoursuivi += lignePion.I_CAVALERIE;
                }

                rapport = (0 == effectifCavaleriePoursuivi) ? 4 : Math.Round((decimal)effectifCavaleriePoursuivant / effectifCavaleriePoursuivi);
                modifRapport = 0;
                if (rapport > 0.5m) { modifRapport = 1; }
                if (rapport > 1m) { modifRapport = 2; }
                if (rapport > 2m) { modifRapport = 3; }
                if (rapport > 3m) { modifRapport = 4; }

                message = string.Format("Poursuite : rapport={0} modifRapport={1}", rapport, modifRapport);
                LogFile.Notifier(message);

                de = Constantes.JetDeDes(1) + modifRapport - 2;
                if (de < 0) de = 0;
                if (de > 9) de = 9;

                moral = (moralCavaleriePoursuivant - 1) / 10;
                pertes = ((Constantes.tablePoursuite[de, moral] * effectifCavaleriePoursuivant / 100) / Constantes.CST_PAS_DE_PERTES) * Constantes.CST_PAS_DE_PERTES;//on travaille par multiples de CST_PAS_DE_PERTES
                pertesMoral = Constantes.tablePoursuite[de, moral]; //pertes en points de moral des unités, également égal aux poucentage de pertes en ravitaillement *2, d'équipement *3, de canons *2
                pertesRavitaillement = pertesMoral * 2;
                pertesMateriel = pertesMoral * 3;
                pourcentagePertesArtillerie = pertesMoral * 2;

                /* -> on fait le calcul par unité, pas au global
                effectifArtilleriePoursuivi = 0;
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivi)
                {
                    effectifArtilleriePoursuivi += lignePion.artillerie;
                }
                pertesArtillerie = 2 * Constantes.tablePoursuite[de, moral] * effectifArtilleriePoursuivi / 100;
                 * */

                message = string.Format("Poursuite : de={0}, ligne moral={1}, rapport pertes={2}%, effectifCavaleriePoursuivant={3}, pertes={4}, pertes en Moral={5}, pertes en artillerie={6}%, pertes en equipement={7}%, pertes en ravitaillement={8}%",
                    de, moral, Constantes.tablePoursuite[de, moral], effectifCavaleriePoursuivant, pertes, pertesMoral, pourcentagePertesArtillerie, pertesMateriel, pertesRavitaillement);
                LogFile.Notifier(message);

                //repartition des pertes physiques, cavaliers et fantassins
                if (0 == pertes)
                {
                    #region On informe les unités que la poursuite est sans effet
                    foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivant)
                    {
                        if (lignePion.B_DETRUIT) { continue; }
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_POURSUITE_SANS_EFFET, this))
                        {
                            message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_POURSUITE_SANS_EFFET");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }
                    foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivi)
                    {
                        if (lignePion.B_DETRUIT) { continue; }
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_AUCUNE_POURSUITE_RECUE, this))
                        {
                            message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_RECUE");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }
                    return true;
                    #endregion
                }

                //il faut repartir les pertes sur le poursuivant suivant l'ordre suivant :
                //pion avec cavalerie, moral le moins élevé
                var result = from pionsPoursuivi in lignePionsPoursuivi.AsEnumerable()
                             orderby pionsPoursuivi.I_CAVALERIE, pionsPoursuivi.Moral
                             select pionsPoursuivi.ID_PION;

                //les unités affectées par la poursuite perdent également leur équipement en proportion du pourcentage de pertes en poursuite
                //cet équipement/ravitaillement/canons récupéré est ensuite donner aux unités faisant la poursuite
                bPertes = true;
                bPremierPassage = true;
                pertesArtillerieTotal = pertesRavitaillementTotal = pertesMaterielTotal = 0;
                while (pertes > 0 && bPertes)
                {
                    bPertes = false;
                    foreach (var idPion in result)
                    {
                        Donnees.TAB_PIONRow lignePionPerte = Donnees.m_donnees.TAB_PION.FindByID_PION(idPion);
                        if (null == lignePionPerte || lignePionPerte.B_DETRUIT || lignePionPerte.estQG || lignePionPerte.effectifTotal <= 0) { continue; }
                        if (lignePionPerte.I_CAVALERIE > 0)
                        {
                            int perteLocale = Math.Min(lignePionPerte.I_CAVALERIE, (lignePionPerte.Moral <= 0) ? Math.Max(0, lignePionPerte.I_CAVALERIE - 2 * Constantes.CST_PAS_DE_PERTES) : Math.Max(0, lignePionPerte.I_CAVALERIE - Constantes.CST_PAS_DE_PERTES));
                            if (listePertesCavalerie.ContainsKey(lignePionPerte.ID_PION))
                            {
                                listePertesCavalerie[lignePionPerte.ID_PION] = (int)listePertesCavalerie[lignePionPerte.ID_PION] + perteLocale;
                            }
                            else
                            {
                                listePertesCavalerie.Add(lignePionPerte.ID_PION, perteLocale);
                            }
                            lignePionPerte.I_CAVALERIE -= perteLocale;
                            message = string.Format("Poursuite : pertes de {3} cavaliers reste {0} pour {1}:{2}", lignePionPerte.I_CAVALERIE, lignePionPerte.ID_PION, lignePionPerte.S_NOM, perteLocale);
                            LogFile.Notifier(message);

                            pertes -= Constantes.CST_PAS_DE_PERTES;
                            bPertes = true;
                        }
                        else
                        {
                            //une perte en infanterie coute le double d'une perte en cavalerie
                            if (lignePionPerte.I_INFANTERIE > 0)
                            {
                                int perteLocale = Math.Min(lignePionPerte.I_INFANTERIE, (lignePionPerte.Moral <= 0) ? Math.Max(0, lignePionPerte.I_INFANTERIE - 4 * Constantes.CST_PAS_DE_PERTES) : Math.Max(0, lignePionPerte.I_INFANTERIE - 2 * Constantes.CST_PAS_DE_PERTES));
                                if (listePertesInfanterie.ContainsKey(lignePionPerte.ID_PION))
                                {
                                    listePertesInfanterie[lignePionPerte.ID_PION] = (int)listePertesInfanterie[lignePionPerte.ID_PION] + perteLocale;
                                }
                                else
                                {
                                    listePertesInfanterie.Add(lignePionPerte.ID_PION, perteLocale);
                                }
                                lignePionPerte.I_INFANTERIE -= perteLocale;
                                message = string.Format("Poursuite : pertes de {3} fantassins reste {0} pour {1}:{2}",
                                    lignePionPerte.I_INFANTERIE, lignePionPerte.ID_PION, lignePionPerte.S_NOM, perteLocale);
                                LogFile.Notifier(message);

                                pertes -= Constantes.CST_PAS_DE_PERTES;
                                bPertes = true;
                            }
                            else
                            {
                                /* NON ! les pertes en artillerie sont traitées differement, voir ci-dessous
                                if (lignePionPerte.I_ARTILLERIE > 0)
                                {
                                    listePertesArtillerie.Add(lignePionPerte.ID_PION, lignePionPerte.I_ARTILLERIE);
                                    lignePionPerte.I_ARTILLERIE = 0;
                                    pertes -= CST_PAS_DE_PERTES;
                                    bPertes = true;
                                }
                                 * */
                            }
                        }
                        if (bPremierPassage && bPertes)
                        {
                            //une unité qui a des pertes en poursuite, perd aussi du moral, de l'artillerie, du matériel, du ravitaillement
                            //Pertes en moral
                            if (lignePionPerte.Moral > 0)
                            {
                                int perteAuMoral = Math.Min(lignePionPerte.Moral, pertesMoral);
                                lignePionPerte.I_MORAL -= perteAuMoral;

                                if (listePertesMateriel.ContainsKey(lignePionPerte.ID_PION))
                                {
                                    listePertesMateriel[lignePionPerte.ID_PION] = (int)listePertesMateriel[lignePionPerte.ID_PION] + pertesMateriel;
                                }
                                else
                                {
                                    listePertesMateriel.Add(lignePionPerte.ID_PION, pertesMateriel);
                                }
                                message = string.Format("Poursuite : pertes de {3} en moral reste {0} pour {1}:{2}",
                                    lignePionPerte.Moral, lignePionPerte.ID_PION, lignePionPerte.S_NOM, perteAuMoral);
                                LogFile.Notifier(message);
                            }

                            //Pertes en artillerie
                            if (lignePionPerte.artillerie > 0)
                            {
                                int perteEnArtillerie = Math.Max(1, lignePionPerte.artillerie * pourcentagePertesArtillerie / 100);//on perd au moins un canon
                                pertesArtillerieTotal += perteEnArtillerie;
                                lignePionPerte.I_ARTILLERIE -= perteEnArtillerie;

                                if (listePertesArtillerie.ContainsKey(lignePionPerte.ID_PION))
                                {
                                    listePertesArtillerie[lignePionPerte.ID_PION] = (int)listePertesArtillerie[lignePionPerte.ID_PION] + perteEnArtillerie;
                                }
                                else
                                {
                                    listePertesArtillerie.Add(lignePionPerte.ID_PION, perteEnArtillerie);
                                }
                                message = string.Format("Poursuite : pertes de {3} canons en artillerie reste {0} pour {1}:{2}",
                                    lignePionPerte.artillerie, lignePionPerte.ID_PION, lignePionPerte.S_NOM, perteEnArtillerie);
                                LogFile.Notifier(message);
                            }

                            //Pertes en materiel
                            if (lignePionPerte.I_MATERIEL > 0)
                            {
                                int perteDuMateriel = Math.Min(lignePionPerte.I_MATERIEL, pertesMateriel);
                                pertesMaterielTotal += lignePionPerte.effectifTotal * perteDuMateriel / 100; //la on compte en nb d'hommes perdant du matériel, pas en pourcentage
                                lignePionPerte.I_MATERIEL -= perteDuMateriel;

                                if (listePertesMateriel.ContainsKey(lignePionPerte.ID_PION))
                                {
                                    listePertesMateriel[lignePionPerte.ID_PION] = (int)listePertesMateriel[lignePionPerte.ID_PION] + perteDuMateriel;
                                }
                                else
                                {
                                    listePertesMateriel.Add(lignePionPerte.ID_PION, perteDuMateriel);
                                }
                                message = string.Format("Poursuite : pertes de {3}% en matériel reste {0} pour {1}:{2}",
                                    lignePionPerte.I_MATERIEL, lignePionPerte.ID_PION, lignePionPerte.S_NOM, perteDuMateriel);
                                LogFile.Notifier(message);
                            }

                            //Pertes en ravitaillement
                            if (lignePionPerte.I_RAVITAILLEMENT > 0)
                            {
                                int perteDuRavitaillement = Math.Min(lignePionPerte.I_RAVITAILLEMENT, pertesRavitaillement);
                                pertesRavitaillementTotal += lignePionPerte.effectifTotal * perteDuRavitaillement / 100; //la on compte en nb d'hommes perdant du ravitaillement, pas en pourcentage
                                lignePionPerte.I_RAVITAILLEMENT -= perteDuRavitaillement;

                                if (listePertesRavitaillement.ContainsKey(lignePionPerte.ID_PION))
                                {
                                    listePertesRavitaillement[lignePionPerte.ID_PION] = (int)listePertesRavitaillement[lignePionPerte.ID_PION] + perteDuRavitaillement;
                                }
                                else
                                {
                                    listePertesRavitaillement.Add(lignePionPerte.ID_PION, perteDuRavitaillement);
                                }
                                message = string.Format("Poursuite : pertes de {3}% en ravitaillement reste {0} pour {1}:{2}",
                                    lignePionPerte.I_RAVITAILLEMENT, lignePionPerte.ID_PION, lignePionPerte.S_NOM, perteDuRavitaillement);
                                LogFile.Notifier(message);
                            }
                        }
                    }
                    bPremierPassage = false;
                }

                //Poursuivre, ça fatigue (si on est une cavalerie)
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivant)
                {
                    if (lignePion.estCombattif && lignePion.cavalerie>0) 
                    { 
                        lignePion.I_NB_HEURES_COMBAT++;//compte double dans le calcul de la fatigue
                    }
                }
                //Etre poursuivi ça fatigue aussi !
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivi)
                {
                    if (lignePion.B_DETRUIT) { continue; }
                    lignePion.I_NB_HEURES_COMBAT++;//compte double dans le calcul de la fatigue
                }

                //repartition des pertes en artillerie, equipement et matériel sur les unités poursuivies
                /*
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivi)
                {

                    effectifArtilleriePoursuivi += lignePion.artillerie;
                }
                bPertes = true;
                while (pertesArtillerie > 0 && bPertes)
                {
                    bPertes = false;
                    foreach (var idPion in result)
                    {
                        Donnees.TAB_PIONRow lignePionPerte = Donnees.m_donnees.TAB_PION.FindByID_PION(idPion);
                        if (null == lignePionPerte || lignePionPerte.estQG) { continue; }
                        if (lignePionPerte.artillerie > 0)
                        {
                            if (listePertesArtillerie.ContainsKey(lignePionPerte.ID_PION))
                            {
                                listePertesArtillerie[lignePionPerte.ID_PION] = (int)listePertesArtillerie[lignePionPerte.ID_PION] + 1;
                            }
                            else
                            {
                                listePertesArtillerie.Add(lignePionPerte.ID_PION, 1);
                            }
                            message = string.Format("Poursuite : pertes d'un point de moral, reste {0}  pour {1}:{2}", lignePionPerte.I_MORAL - 1, lignePionPerte.ID_PION, lignePionPerte.S_NOM);
                            LogFile.Notifier(message);

                            lignePionPerte.I_ARTILLERIE--;
                            pertesMoral--;
                            bPertes = true;
                        }
                    }
                }

                //repartition des pertes en moral sur les unités
                bPertes = true;
                while (pertesMoral > 0 && bPertes)
                {
                    bPertes = false;
                    foreach (var idPion in result)
                    {
                        Donnees.TAB_PIONRow lignePionPerte = Donnees.m_donnees.TAB_PION.FindByID_PION(idPion);
                        if (null == lignePionPerte || lignePionPerte.B_DETRUIT || lignePionPerte.estQG) { continue; }
                        if (lignePionPerte.I_MORAL > 0)
                        {
                            if (listePertesMoral.ContainsKey(lignePionPerte.ID_PION))
                            {
                                listePertesMoral[lignePionPerte.ID_PION] = (int)listePertesMoral[lignePionPerte.ID_PION] + 1;
                            }
                            else
                            {
                                listePertesMoral.Add(lignePionPerte.ID_PION, 1);
                            }
                            message = string.Format("Poursuite : pertes d'un point de moral, reste {0}  pour {1}:{2}", lignePionPerte.I_MORAL - 1, lignePionPerte.ID_PION, lignePionPerte.S_NOM);
                            LogFile.Notifier(message);

                            lignePionPerte.I_MORAL--;
                            pertesMoral--;
                            bPertes = true;
                        }
                    }
                }*/

                #region calcul des pertes globales
                pertesCavalerieTotal = pertesInfanterieTotal = pertesArtillerieTotal = 0;
                for (i = 0; i < listePertesInfanterie.Count; i++) { pertesInfanterieTotal += (int)listePertesInfanterie.GetByIndex(i); }
                for (i = 0; i < listePertesCavalerie.Count; i++) { pertesCavalerieTotal += (int)listePertesCavalerie.GetByIndex(i); }
                for (i = 0; i < listePertesArtillerie.Count; i++) { pertesArtillerieTotal += (int)listePertesArtillerie.GetByIndex(i); }

                if (pertes > 0)
                {
                    //toutes les unités restantes ont été détruites par la poursuite
                    //on informe le poursuivant
                    foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivant)
                    {
                        if (lignePion.B_DETRUIT) { continue; }
                        //note : on ne donne pas le moral perdu par l'ennemi, comment pourrait-il l'estimer ?
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_POURSUITE_DESTRUCTION_TOTALE, pertesInfanterieTotal, pertesCavalerieTotal, pertesArtillerieTotal, 0, 0, 0, this))
                        {
                            message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_POURSUITE_DESTRUCTION_TOTALE");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }
                    //on détruit les unités poursuivies
                    foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivi)
                    {
                        if (lignePion.B_DETRUIT) { continue; }
                        lignePion.DetruirePion();
                    }
                }
                else
                {
                    //message informant globalement des pertes pour le poursuivant
                    foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivant)
                    {
                        if (lignePion.B_DETRUIT) { continue; }
                        //note : on ne donne pas le moral perdu par l'ennemi, comment pourrait-il l'estimer ?
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_POURSUITE_PERTES_POURSUIVANT, pertesInfanterieTotal, pertesCavalerieTotal, pertesArtillerieTotal, 0, 0, 0, this))
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
                        int pertesCavaleriePion = listePertesCavalerie.ContainsKey(lignePion.ID_PION) ? (int)listePertesCavalerie[lignePion.ID_PION] : 0;
                        int pertesInfanteriePion = listePertesInfanterie.ContainsKey(lignePion.ID_PION) ? (int)listePertesInfanterie[lignePion.ID_PION] : 0;
                        int pertesArtilleriePion = listePertesArtillerie.ContainsKey(lignePion.ID_PION) ? (int)listePertesArtillerie[lignePion.ID_PION] : 0;
                        int pertesMoralPion = listePertesMoral.ContainsKey(lignePion.ID_PION) ? (int)listePertesMoral[lignePion.ID_PION] : 0;
                        int pertesMaterielPion = listePertesMateriel.ContainsKey(lignePion.ID_PION) ? (int)listePertesRavitaillement[lignePion.ID_PION] : 0;
                        int pertesRavitaillementPion = listePertesRavitaillement.ContainsKey(lignePion.ID_PION) ? (int)listePertesArtillerie[lignePion.ID_PION] : 0;
                        if (0 == pertesCavalerieTotal + pertesInfanterieTotal + pertesArtillerieTotal)
                        {
                            if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_AUCUNE_POURSUITE_RECUE, this))
                            {
                                message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_RECUE");
                                LogFile.Notifier(message, out messageErreur);
                                return false;
                            }
                        }
                        else
                        {
                            if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_POURSUITE_PERTES_POURSUIVI,
                                pertesInfanteriePion, pertesCavaleriePion, pertesArtilleriePion, pertesMoralPion, pertesMaterielPion, pertesRavitaillementPion, this))
                            {
                                message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_POURSUITE_PERTES_POURSUIVI");
                                LogFile.Notifier(message, out messageErreur);
                                return false;
                            }
                        }
                        //if (lignePion.effectifTotal <= 0 && lignePion.estCombattif)
                        if (!lignePion.estQG && lignePion.effectifTotal <= 0 && !lignePion.B_DETRUIT)
                        {
                            //pion détruit par la poursuite
                            if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_DETRUIT_PAR_POURSUITE))
                            {
                                message = string.Format("Poursuite : erreur lors de l'envoi d'un message pour destruction");
                                LogFile.Notifier(message, out messageErreur);
                                return false;
                            }
                            lignePion.DetruirePion();
                        }
                    }
                }
                #endregion

                //Repartition du butin
                //pertesMaterielTotal et pertesRavitaillementTotal sont exprimés en nombre d'hommes pouvant reprendre du matériel ou du ravitaillement, il faut ensuite le reconvertir en pourcentage
                //la repartition est faite suivant le nombre de canons déjà possédés pour les canons et suivant l'effectif global pour le reste
                pertesArtillerieTotal /= 2;//un canon sur deux "perdu" lors d'une poursuite n'est pas récupérable par le poursuivant
                message = string.Format("Poursuite : butin à répartir de {0} hommes/matériel, {1} hommes/ravitaillement et {2} canons",
                    pertesMaterielTotal, pertesRavitaillementTotal, pertesArtillerieTotal);
                LogFile.Notifier(message);

                if (pertesMaterielTotal > 0 || pertesRavitaillementTotal > 0 || pertesArtillerieTotal > 0)
                {
                    effectifPoursuivantTotal = artilleriePoursuivantTotal = 0;
                    //somme des effectifs pour déterminer la clef de répartition
                    foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivant)
                    {
                        if (lignePion.B_DETRUIT || lignePion.effectifTotal <= 0 || lignePion.Moral <= 0) { continue; }
                        effectifPoursuivantTotal += lignePion.effectifTotal;
                        artilleriePoursuivantTotal += lignePion.artillerie;
                    }

                    //Repartition effective
                    foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivant)
                    {
                        if (lignePion.B_DETRUIT || lignePion.effectifTotal <= 0 || lignePion.Moral <= 0) { continue; }
                        int canonsButin = pertesArtillerieTotal * lignePion.artillerie / artilleriePoursuivantTotal;
                        int materielButin = Math.Min(100 - lignePion.I_MATERIEL,
                            pertesMaterielTotal * 100 / effectifPoursuivantTotal);//il faut passer d'un nombre d'hommes a un pourcentage
                        int ravitaillementButin = Math.Min(100 - lignePion.I_RAVITAILLEMENT,
                            pertesRavitaillementTotal * 100 / effectifPoursuivantTotal);//il faut passer d'un nombre d'hommes a un pourcentage

                        lignePion.I_ARTILLERIE += canonsButin;
                        lignePion.I_MATERIEL = materielButin;
                        lignePion.I_RAVITAILLEMENT = ravitaillementButin;
                        message = string.Format("Poursuite : Butin pour {0}:{1} de {2}% en matériel (total {3}%), {4}% en ravitaillement (total {5}%) et {6} canons (total {7} canons)",
                            lignePion.ID_PION, lignePion.S_NOM, materielButin, lignePion.I_MATERIEL, ravitaillementButin, lignePion.I_RAVITAILLEMENT, canonsButin, lignePion.I_ARTILLERIE);

                        //envoi d'un message pour prévenir de la prise de butin
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_POURSUITE_BUTIN, 0, 0, canonsButin, 0, ravitaillementButin, materielButin, this))
                        {
                            message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_POURSUITE_PERTES_POURSUIVANT");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }
                }
                return true;
            }

            /// <summary>
            /// Calcul du modificateur stratégique du chef de bataille
            /// </summary>
            /// <param name="idLeader">identifiant du chef</param>
            /// <param name="nbUnites">liste des unités sous son commandement</param>
            /// <returns>modificateur stratégique</returns>
            private int CalculModificateurStrategique(int idLeader, Donnees.TAB_PIONRow[] lignePionsEnBataille)
            {
                int nbUnites = 0;
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille)
                {
                    if (lignePion.estCombattif) nbUnites++;
                }
                return CalculModificateurStrategique(idLeader, nbUnites);
            }

            /// <summary>
            /// Calcul du modificateur stratégique du chef de bataille
            /// </summary>
            /// <param name="idLeader">identifiant du chef</param>
            /// <param name="nbUnites">nombre d'unités sous son commandement</param>
            /// <returns>modificateur stratégique</returns>
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
            /// Round supplémentaire d'une bataille
            /// </summary>
            /// <param name="ligneBataille">bataille à effectuer</param>
            /// <returns>true si OK, false si KO</returns>
            public bool EffectuerBataille(out bool bFinDeBataille)
            {
                string message, messageErreur;
                int[] des;
                int[] effectifs;
                int[] canons;
                //int[] score = new int[6];
                //int[] pertesMoral = new int[6];
                int[] relance = new int[6];                //nombre de dés autorises à être relancés en cas de superiorité d'artillerie
                int i;
                int nbUnites012 = 0, nbUnites345 = 0;//pour les bonus stratégiques
                int nbUnites012Base = 0, nbUnites345Base = 0;//pour vérifier qu'il y a bien des unités présentes
                int modificateurStrategique012, modificateurStrategique345;
                Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain;
                Donnees.TAB_PIONRow[] lignePionsEnBataille012;
                Donnees.TAB_PIONRow[] lignePionsEnBataille345;
                bool bUniteEnDefense;
                int idLeader;

                bFinDeBataille = false;
                Donnees.TAB_PIONRow lignePionTest = Donnees.m_donnees.TAB_PION.FindByID_PION(20);
                if (!IsI_TOUR_FINNull())
                {
                    return true;//la bataille est déjà terminée
                }

                message = string.Format("*************** EffectuerBataille sur {0} (ID={1}) ************************",
                    S_NOM, ID_BATAILLE);
                LogFile.Notifier(message, out messageErreur);

                //if (Donnees.m_donnees.TAB_PARTIE.Nocturne())
                //{
                //    //pas de combat la nuit
                //    message = string.Format("EffectuerBataille sur ID_BATAILLE={0}: Fin de la bataille à cause de l'arrivée de la nuit.", ligneBataille.ID_BATAILLE);
                //    LogFile.Notifier(message, out messageErreur);
                //    return FinDeBataille(ligneBataille);
                //}

                #region choix des leaders sur le combat
                if (!RecherchePionsEnBataille(out nbUnites012, out nbUnites345, out des, out effectifs, out canons, out lignePionsEnBataille012, out lignePionsEnBataille345, null/*bengagement*/, false/*bcombattif*/))
                {
                    message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille QG");
                    LogFile.Notifier(message, out messageErreur);
                }

                //trouver le leader avec le plus haut niveau hierarchique
                idLeader = TrouverLeaderBataille(lignePionsEnBataille012, true);
                // Note : Dans la règle, le leader de plus haut rang hierarchique dirige la bataille et ne peut pas s'impliquer tactiquement.
                // Dans vaoc, cela pose deux problèmes : 1) Napoléon ne pourra jamais s'engager, son bonus tactique ne pourra jamais être utilisé
                //2) s'il y a deux chefs de même niveau, les joueurs doivent normalement décider entre eux qui dirige le combat et qui peut être impliqué tactiquement
                //il n'est pas possible de mettre en place ce "mini" forum.
                //De fait, le premier joueur arrivé dirige le combat mais tous les leaders présents peuvent être engagés tactiquement.

                //s'agit-il du même leader que precedemment ? Si non, l'affecter et désengager le leader précédent 
                //(qui pourra ainsi intervenir au niveau tactique s'il le souhaite)
                /* devenu inutile puisque les leaders peuvent toujours s'engager
                if (!ligneBataille.IsID_LEADER_012Null() && idLeader != ligneBataille.ID_LEADER_012 && ligneBataille.ID_LEADER_012 >= 0)
                {
                    Donnees.TAB_BATAILLE_PIONSRow ligneBataillePion = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(ligneBataille.ID_LEADER_012, ligneBataille.ID_BATAILLE);
                    ligneBataillePion.B_ENGAGEE = false;
                }
                */

                if (IsID_LEADER_012Null() || idLeader != ID_LEADER_012)
                {
                    if (idLeader >= 0)
                    {
                        ID_LEADER_012 = idLeader;
                    }
                    else
                    {
                        SetID_LEADER_012Null();
                    }
                    //Pourquoi changer cette valeur ? Un leader promu peut reste engager non ?
                    /*
                    Donnees.TAB_BATAILLE_PIONSRow ligneBataillePion = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(idLeader, ligneBataille.ID_BATAILLE);
                    ligneBataillePion.B_ENGAGEE = false; //tous les chefs peuvent s'engager, même s'ils commandent, sinon il faut mettre true; 
                    Donnees.TAB_PIONRow LignePionLeader = Donnees.m_donnees.TAB_PION.FindByID_PION(idLeader);
                    LignePionLeader.SetI_ZONE_BATAILLENull();
                     * */
                }

                idLeader = TrouverLeaderBataille(lignePionsEnBataille345, false);
                //s'agit-il du même leader que precedemment ? Si non, l'affecter et désengager le leader précédent 
                //(qui pourra ainsi intervenir au niveau tactique s'il le souhaite)
                /* devenu inutile puisque les leaders peuvent toujours s'engager
                if (!ligneBataille.IsID_LEADER_345Null() && idLeader != ligneBataille.ID_LEADER_345 && ligneBataille.ID_LEADER_345 >= 0)
                {
                    Donnees.TAB_BATAILLE_PIONSRow ligneBataillePion = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(ligneBataille.ID_LEADER_345, ligneBataille.ID_BATAILLE);
                    ligneBataillePion.B_ENGAGEE = false;
                }
                */

                if (IsID_LEADER_345Null() || idLeader != ID_LEADER_345)
                {
                    if (idLeader >= 0)
                    {
                        ID_LEADER_345 = idLeader;
                    }
                    else
                    {
                        SetID_LEADER_345Null();
                    }
                    //Pourquoi changer cette valeur ? Un leader promu peut reste engager non ?
                    /*
                    Donnees.TAB_BATAILLE_PIONSRow ligneBataillePion = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(idLeader, ligneBataille.ID_BATAILLE);
                    ligneBataillePion.B_ENGAGEE = false;//tous les chefs peuvent s'engager, même s'ils commandent, sinon il faut mettre true;
                    Donnees.TAB_PIONRow LignePionLeader = Donnees.m_donnees.TAB_PION.FindByID_PION(idLeader);
                    LignePionLeader.SetI_ZONE_BATAILLENull();
                     */
                }

                #endregion
                //on execute une bataille que toutes les deux heures
                if (I_TOUR_DEBUT == Donnees.m_donnees.TAB_PARTIE[0].I_TOUR ||
                    (Donnees.m_donnees.TAB_PARTIE[0].I_TOUR - I_TOUR_DEBUT) % 2 > 0)
                {
                    message = string.Format("EffectuerBataille sur {0} (ID_BATAILLE={1}): Pas de combat à ce tour-ci, au prochain tour seulement.",
                        S_NOM, ID_BATAILLE);
                    LogFile.Notifier(message, out messageErreur);

                    return true;//on ne fait le combat que toutes les deux heures
                }

                if (Donnees.m_donnees.TAB_PARTIE.Nocturne())
                {
                    //pas de combat la nuit
                    message = string.Format("EffectuerBataille sur ID_BATAILLE={0}: Fin de la bataille à cause de l'arrivée de la nuit.", ID_BATAILLE);
                    LogFile.Notifier(message, out messageErreur);
                    return FinDeBataille(out bFinDeBataille);
                }

                #region Fin de bataille sur ordre de retraite
                //Un ordre de retraite a-t-il été donné à ce tour ? ou à un autre ? En fait, il ne peut y avoir qu'une retraite sur une bataille !
                string requete = string.Format("ID_BATAILLE={0} AND  I_ORDRE_TYPE={1}",
                    ID_BATAILLE, Constantes.ORDRES.RETRAITE);
                Donnees.TAB_ORDRERow[] resOrdreRetraite = (Donnees.TAB_ORDRERow[])Donnees.m_donnees.TAB_ORDRE.Select(requete);
                if (resOrdreRetraite.Length > 0)
                {
                    message = string.Format("EffectuerBataille sur {0} (ID_BATAILLE={1}): Un ordre de retraite a été donné sur cette bataille.",
                        S_NOM, ID_BATAILLE);
                    LogFile.Notifier(message, out messageErreur);
                    bool bRetraite012 = false;
                    bool bRetraite345 = false;
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
                    return FinDeBataille(bRetraite012, bRetraite345, out bFinDeBataille);
                }
                #endregion

                //toutes les unités engagées viennent de passer une heure sur le terrain, cela joue pour la fatigue de fin de journée
                if (!RecherchePionsEnBataille(out nbUnites012Base, out nbUnites345Base, out des, out effectifs, out canons, out lignePionsEnBataille012, out lignePionsEnBataille345, true/*bengagement*/, true/*bcombattif*/))
                {
                    message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille 0");
                    LogFile.Notifier(message, out messageErreur);
                }

                // on vérifie qu'il y a bien une unité combattive engagagée de chaque coté, sinon, 
                /// on en choisit une au hasard qui se met en défense au centre
                if (0 == nbUnites012Base)
                {
                    if (!RecherchePionsEnBataille(out nbUnites012, out nbUnites345, out des, out effectifs, out canons, out lignePionsEnBataille012, out lignePionsEnBataille345, false/*bengagement*/, true/*bcombattif*/))
                    {
                        message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille 012");
                        LogFile.Notifier(message, out messageErreur);
                    }
                    if (0 == nbUnites012)
                    {
                        message = string.Format("EffectuerBataille : il n'y a aucune unité combattive dans le secteur 012 pour la bataille ID_BATAILLE={0}", ID_BATAILLE);
                        LogFile.Notifier(message, out messageErreur);
                        return FinDeBataille(out bFinDeBataille);//cas d'un ordre de retraite global donné par un joueur
                    }
                    if (!AffecterUneUniteAuHasardEnBataille(lignePionsEnBataille012, 1))
                    {
                        message = string.Format("EffectuerBataille : erreur dans AffecterUneUniteAuHasardEnBataille 012");
                        LogFile.Notifier(message, out messageErreur);
                    }
                }

                if (0 == nbUnites345Base)
                {
                    if (!RecherchePionsEnBataille(out nbUnites012, out nbUnites345, out des, out effectifs, out canons, out lignePionsEnBataille012, out lignePionsEnBataille345, false/*bengagement*/, true/*bcombattif*/))
                    {
                        message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille 345");
                        LogFile.Notifier(message, out messageErreur);
                    }
                    if (0 == nbUnites345)
                    {
                        message = string.Format("EffectuerBataille : il n'y a aucune unité combattive dans le secteur 345 pour la bataille ID_BATAILLE={0}", ID_BATAILLE);
                        LogFile.Notifier(message, out messageErreur);
                        return FinDeBataille(out bFinDeBataille);//cas d'un ordre de retraite global donné par un joueur
                    }
                    if (!AffecterUneUniteAuHasardEnBataille(lignePionsEnBataille345, 4))
                    {
                        message = string.Format("EffectuerBataille : erreur dans AffecterUneUniteAuHasardEnBataille 345");
                        LogFile.Notifier(message, out messageErreur);
                    }
                }

                if (!RecherchePionsEnBataille(out nbUnites012, out nbUnites345, out des, out effectifs, out canons, out lignePionsEnBataille012, out lignePionsEnBataille345, true/*bengagement*/, false/*bcombattif*/))//false sur bcombattif avant, mais dans ce cas, donne une fausse valeur du modificateur stratégique, remis a false car sinon on ne joue plus la blessure des chefs
                {
                    message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille I");
                    LogFile.Notifier(message, out messageErreur);
                }

                message = string.Format("EffectuerBataille sur {2} nombre d'unités en début de combat nbUnites012={0} nbUnites345={1}",
                    nbUnites012, nbUnites345, S_NOM);
                LogFile.Notifier(message, out messageErreur);
                //combattre, ça fatigue !
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille012)
                {
                    int iZoneBataille = lignePion.IsI_ZONE_BATAILLENull() ? -1 : lignePion.I_ZONE_BATAILLE;
                    message = string.Format("EffectuerBataille unité en 012, {1} ID={0} zone={2}", lignePion.ID_PION, lignePion.S_NOM, iZoneBataille);
                    LogFile.Notifier(message, out messageErreur);
                    lignePion.I_NB_HEURES_COMBAT++;
                }
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille345)
                {
                    int iZoneBataille = lignePion.IsI_ZONE_BATAILLENull() ? -1 : lignePion.I_ZONE_BATAILLE;
                    message = string.Format("EffectuerBataille unité en 345, {1} ID={0} zone={2}", lignePion.ID_PION, lignePion.S_NOM, iZoneBataille);
                    LogFile.Notifier(message, out messageErreur);
                    lignePion.I_NB_HEURES_COMBAT++;
                }

                //calcul du modificateur strategique
                modificateurStrategique012 = CalculModificateurStrategique(ID_LEADER_012, lignePionsEnBataille012);
                modificateurStrategique345 = CalculModificateurStrategique(ID_LEADER_345, lignePionsEnBataille345);
                message = string.Format("EffectuerBataille modificateurStrategique012={0} modificateurStrategique345={1}", modificateurStrategique012, modificateurStrategique345);
                LogFile.Notifier(message, out messageErreur);

                #region calcul des dés par zones
                for (i = 0; i < 3; i++)
                {
                    // par défaut il y a un nombre de dés par zones égal au maximum du niveau d'engagement (on ajoute car RecherchePionsEnBataille indique déjà les modificateurs tactiques)
                    int engagementZoneI = (int)this["I_ENGAGEMENT_" + Convert.ToString(i)];
                    int engagementZoneI3 = (int)this["I_ENGAGEMENT_" + Convert.ToString(i + 3)];
                    int desEngagement = Math.Max(engagementZoneI, engagementZoneI3);
                    if (effectifs[i] > 0) { des[i] += desEngagement; }
                    if (effectifs[i + 3] > 0) { des[i + 3] += desEngagement; }

                    if (0 == effectifs[i] || 0 == effectifs[i + 3])
                    {
                        message = string.Format("EffectuerBataille attaque de flanc, avant : des={0} {1} {2} {3} {4} {5}", des[0], des[1], des[2], des[3], des[4], des[5]);
                        LogFile.Notifier(message, out messageErreur);
                        message = string.Format("EffectuerBataille attaque de flanc, avant : effectifs[{0}]={1} effectifs[{2}]={3}", i, effectifs[i], i + 3, effectifs[i + 3]);
                        LogFile.Notifier(message, out messageErreur);
                        //attaque de flanc ? 
                        //Sachant que si le centre est vide , ce qui est  possible si l'unité a rompu a un tour précédent 
                        //et que le général a décidé de ne pas faire retraite), les deux cotés sont considérés comme "pris de flanc"
                        if (effectifs[i] > 0)
                        {
                            switch (i)
                            {
                                case 0:
                                case 2:
                                    if (effectifs[1] > 0) { des[1] += 2; }
                                    break;
                                default:
                                    if (effectifs[0] > 0) { des[0] += 2; }
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
                                    if (effectifs[4] > 0) { des[4] += 2; }
                                    break;
                                default:
                                    if (effectifs[3] > 0) { des[3] += 2; }
                                    if (effectifs[5] > 0) { des[5] += 2; }
                                    break;
                            }
                        }
                        message = string.Format("EffectuerBataille attaque de flanc, après : des={0} {1} {2} {3} {4} {5}", des[0], des[1], des[2], des[3], des[4], des[5]);
                        LogFile.Notifier(message, out messageErreur);
                    }
                    else
                    {
                        message = string.Format("EffectuerBataille avant calcul des[{0}]={1} des[{2}]={3}", i, des[i], i + 3, des[i + 3]);
                        LogFile.Notifier(message, out messageErreur);
                        //valeur stratégique du chef
                        des[i] += modificateurStrategique012;
                        des[i + 3] += modificateurStrategique345;
                        message = string.Format("EffectuerBataille avec valeur stratégique des[{0}]={1} des[{2}]={3}", i, des[i], i + 3, des[i + 3]);
                        LogFile.Notifier(message, out messageErreur);

                        //rapport de force, +2 pour 2/1, +3 pour 3/1 avec un maximum de +6
                        int rapport = effectifs[i] / effectifs[i + 3];
                        if (rapport >= 2) { des[i] += Math.Min(rapport, 6); }
                        rapport = effectifs[i + 3] / effectifs[i];
                        if (rapport >= 2) { des[i + 3] += Math.Min(rapport, 6); }
                        message = string.Format("EffectuerBataille avec rapport de forces des[{0}]={1} effectif={2} des[{3}]={4} effectif={5}",
                            i, des[i], effectifs[i], i + 3, des[i + 3], effectifs[i + 3]);
                        LogFile.Notifier(message, out messageErreur);

                        int rapportArtillerie = (0 == canons[i + 3]) ? canons[i] > 0 ? 2 : 0 : canons[i] / canons[i + 3];
                        if (rapportArtillerie >= 1) { relance[i] = Math.Min(rapportArtillerie, 2); } // 2 dés de relance au maximum
                        rapportArtillerie = (0 == canons[i]) ? canons[i + 3] > 0 ? 2 : 0 : canons[i + 3] / canons[i];
                        if (rapportArtillerie >= 1) { relance[i + 3] = Math.Min(rapportArtillerie, 2); }// 2 dés de relance au maximum
                        message = string.Format("EffectuerBataille relance[{0}]={1} relance[{2}]={3}", i, relance[i], i + 3, relance[i + 3]);

                        //modificateurs de terrain, appliquées uniquement si l'une des unités de la zone est en mode défense et si son niveau d'engagement est inférieur ou égal à l'attaquant
                        int valeurDesFortifications = 0;
                        bUniteEnDefense = false;
                        foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille012)
                        {
                            if (!lignePion.IsI_ZONE_BATAILLENull() && lignePion.I_ZONE_BATAILLE == i && lignePion.estCombattif)
                            {
                                Donnees.TAB_BATAILLE_PIONSRow ligneBataillePion = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePion.ID_PION, ID_BATAILLE);
                                if (ligneBataillePion.B_EN_DEFENSE)
                                {
                                    bUniteEnDefense = true;
                                    valeurDesFortifications = Math.Max(valeurDesFortifications, lignePion.I_NIVEAU_FORTIFICATION);
                                }
                            }
                        }
                        //on ne peut bénéficier d'un bonus de défense que si son niveau est inférieur ou égal à celui de l'adversaire
                        if (bUniteEnDefense && (engagementZoneI <= engagementZoneI3))
                        {
                            ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN((int)this["ID_TERRAIN_" + Convert.ToString(i)]);
                            //les modificateurs de terrain sont en réduction des dés de l'adversaire
                            des[i + 3] -= ligneModeleTerrain.I_MODIFICATEUR_DEFENSE + valeurDesFortifications;
                        }

                        valeurDesFortifications = 0;
                        bUniteEnDefense = false;
                        foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille345)
                        {
                            if (!lignePion.IsI_ZONE_BATAILLENull() && lignePion.I_ZONE_BATAILLE == i + 3 && lignePion.estCombattif)
                            {
                                Donnees.TAB_BATAILLE_PIONSRow ligneBataillePion = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePion.ID_PION, ID_BATAILLE);
                                if (ligneBataillePion.B_EN_DEFENSE)
                                {
                                    bUniteEnDefense = true;
                                    valeurDesFortifications = Math.Max(valeurDesFortifications, lignePion.I_NIVEAU_FORTIFICATION);
                                }
                            }
                        }
                        if (bUniteEnDefense && (engagementZoneI3 <= engagementZoneI))
                        {
                            ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN((int)this["ID_TERRAIN_" + Convert.ToString(i + 3)]);
                            //les modificateurs de terrain sont en réduction des dés de l'adversaire
                            des[i] -= ligneModeleTerrain.I_MODIFICATEUR_DEFENSE + valeurDesFortifications;
                            //des[i + 3] += ligneModeleTerrain.I_MODIFICATEUR_DEFENSE + valeurDesFortifications;
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

                if (!CalculDesPertesAuCombat(des, relance, lignePionsEnBataille012, lignePionsEnBataille345))
                {
                    message = string.Format("EffectuerBataille : erreur dans CalculDesPertesAuCombat II");
                    LogFile.Notifier(message, out messageErreur);
                    return false;
                }

                //Y-a-t-il encore des combattants dans chaque camp ?
                if (!RecherchePionsEnBataille(out nbUnites012, out nbUnites345, out des, out effectifs, out canons, out lignePionsEnBataille012, out lignePionsEnBataille345, null/*bengagement*/, true/*bcombattif*/))
                {
                    message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille II");
                    LogFile.Notifier(message, out messageErreur);
                    return false;
                }

                message = string.Format("EffectuerBataille nombre d'unités en fin de combat nbUnites012={0} nbUnites345={1}", nbUnites012, nbUnites345);
                LogFile.Notifier(message, out messageErreur);
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille012)
                {
                    message = string.Format("EffectuerBataille unités en 012, {1} ID={0}", lignePion.ID_PION, lignePion.S_NOM);
                    LogFile.Notifier(message, out messageErreur);
                }
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille345)
                {
                    message = string.Format("EffectuerBataille unités en 345, {1} ID={0}", lignePion.ID_PION, lignePion.S_NOM);
                    LogFile.Notifier(message, out messageErreur);
                }

                if (0 == nbUnites012 || 0 == nbUnites345)
                {
                    FinDeBataille(out bFinDeBataille);
                }
                else
                {
                    //La bataille continue seulement s'il ne fait pas nuit
                    if (Donnees.m_donnees.TAB_PARTIE.Nocturne())
                    {
                        //pas de combat la nuit
                        message = string.Format("EffectuerBataille sur {0} (ID_BATAILLE={1}): Fin de la bataille à cause de l'arrivée de la nuit.",
                            S_NOM, ID_BATAILLE);
                        LogFile.Notifier(message, out messageErreur);
                        return FinDeBataille(out bFinDeBataille);
                    }

                    //si un leader se retrouve dans une zone sans unité combattante, il doit être remis en réserve
                    RemiseLeaderEnReserve();
                }

                //Envoie d'un message prévenant d'un bruit de canon pour toutes les unités non présentes.
                AlerteBruitDuCanon();

                return true;
            }

            private int EffectifTotalSurZone(int zone, Donnees.TAB_PIONRow[] lignePionsEnBataille)
            {
                int effectifTotal = 0;
                foreach (Donnees.TAB_PIONRow lignePionBataille in lignePionsEnBataille)
                {
                    if (lignePionBataille.estCombattif && lignePionBataille.I_ZONE_BATAILLE == zone)
                    {
                        effectifTotal += lignePionBataille.effectifTotal;
                    }
                }
                return effectifTotal;
            }

            /// <summary>
            /// jets de dés et calcul des pertes
            /// </summary>
            /// <param name="ligneBataille"></param>
            /// <param name="des"></param>
            /// <param name="relance"></param>
            /// <param name="lignePionsEnBataille012"></param>
            /// <param name="lignePionsEnBataille345"></param>
            /// <returns>true si OK, false si KO</returns>
            private bool CalculDesPertesAuCombat( int[] des, int[] relance, Donnees.TAB_PIONRow[] lignePionsEnBataille012, Donnees.TAB_PIONRow[] lignePionsEnBataille345)
            {
                string message, messageErreur;
                int[] score = new int[6];
                int[] pertesMoral = new int[6];
                int[] pertesEffectifs = new int[6];
                bool[] blessureChef = new bool[6];
                int i;
                int nb6;//nombre de '6' lancés sur un secteur

                #region on lance les des et on calcule les pertes au moral et en effectifs infligés par chaque camp
                for (i = 0; i < 6; i++)
                {
                    nb6 = 0;
                    score[i] = (des[i] > 0) ? Constantes.JetDeDes(des[i], relance[i], out nb6) : 0;
                    blessureChef[i] = (nb6 >= 4) ? true : false;
                    this["S_COMBAT_" + Convert.ToString(i)]=string.Format("{0} dés, {1} relances = {2}", des[i], relance[i], score[i]);
                }

                for (i = 0; i < 3; i++)
                {
                    pertesMoral[i] = score[i + 3] / 2;
                    //les pertes en effectifs sont égales au quart du score * effectifs engagés / 100
                    pertesEffectifs[i] = EffectifTotalSurZone(i, lignePionsEnBataille012) * score[i + 3] / 400;
                }
                for (i = 3; i < 6; i++)
                {
                    pertesMoral[i] = score[i - 3] / 2;
                    //les pertes en effectifs sont égales au quart du score * effectifs engagés / 100
                    pertesEffectifs[i] = EffectifTotalSurZone(i, lignePionsEnBataille345) * score[i - 3] / 400;
                }
                #endregion

                message = string.Format("EffectuerBataille pertesMoral: {0} {1} {2} {3} {4} {5}",
                    pertesMoral[0], pertesMoral[1], pertesMoral[2], pertesMoral[3], pertesMoral[4], pertesMoral[5]);
                LogFile.Notifier(message, out messageErreur);
                message = string.Format("EffectuerBataille pertesEffectifs: {0} {1} {2} {3} {4} {5}",
                    pertesEffectifs[0], pertesEffectifs[1], pertesEffectifs[2], pertesEffectifs[3], pertesEffectifs[4], pertesEffectifs[5]);
                LogFile.Notifier(message, out messageErreur);

                #region on vérifie que chaque camp peut bien prendre toutes les pertes en moral et effectif, si ce n'est pas le cas, les pertes sont nuls pour l'autre camp
                for (i = 0; i < 3; i++)
                {
                    bool PremierCampKO = true;
                    bool DeuxiemeCampKO = true;

                    if (des[i] > 0 && des[i + 3] > 0)
                    {
                        //int effectifTotal = 0; -> Je ne vois pas comment une troupe ne pourrait plus avoir de soldats sans avoir, auparavant, perdu tout son moral
                        //mais si un camp ne peut pas supporter toutes les pertes, il est quand même KO !
                        if (pertesEffectifs[i] < EffectifTotalSurZone(i, lignePionsEnBataille012))
                        {
                            foreach (Donnees.TAB_PIONRow lignePionEnBataille in lignePionsEnBataille012)
                            {
                                //if (lignePionEnBataille.B_DETRUIT || lignePionEnBataille.I_TOUR_FUITE_RESTANT > 0 || lignePionEnBataille.I_TOUR_RETRAITE_RESTANT > 0) { continue; }
                                if (!lignePionEnBataille.estCombattif) { continue; }
                                if (lignePionEnBataille.IsI_ZONE_BATAILLENull() || lignePionEnBataille.I_ZONE_BATAILLE != i) { continue; }
                                //effectifTotal += lignePionEnBataille.effectifTotal;
                                if (lignePionEnBataille.I_MORAL > pertesMoral[i]) { PremierCampKO = false; }
                            }
                        }

                        if (pertesEffectifs[i + 3] < EffectifTotalSurZone(i + 3, lignePionsEnBataille345))
                        {
                            foreach (Donnees.TAB_PIONRow lignePionEnBataille in lignePionsEnBataille345)
                            {
                                if (!lignePionEnBataille.estCombattif) { continue; }
                                if (lignePionEnBataille.IsI_ZONE_BATAILLENull() || lignePionEnBataille.I_ZONE_BATAILLE != i + 3) { continue; }
                                if (lignePionEnBataille.I_MORAL > pertesMoral[i + 3]) { DeuxiemeCampKO = false; }
                            }
                        }

                        //un des bords va fuir complètement et pas l'autre ?
                        if (!PremierCampKO || !DeuxiemeCampKO)
                        {
                            if (PremierCampKO)
                            {
                                pertesMoral[i + 3] = 0;
                                pertesEffectifs[i + 3] = 0;
                                LogFile.Notifier("i=" + i + " le premier Camp est mis en déroute et n'inflige aucune perte à l'adversaire", out messageErreur);
                            }
                            if (DeuxiemeCampKO)
                            {
                                pertesMoral[i] = 0;
                                pertesEffectifs[i] = 0;
                                LogFile.Notifier("i=" + (i + 3) + " le deuxième Camp est mis en déroute et n'inflige aucune perte à l'adversaire", out messageErreur);
                            }
                        }
                    }
                }
                #endregion

                #region Un des chefs présents est-il blessé ?
                for (int kkk = 0; kkk < blessureChef.Count(); kkk++) blessureChef[kkk] = true;//test sur la blessure des chefs
                for (i = 0; i < 3; i++)
                {
                    //Un des chefs présents est-il blessé ?
                    if (pertesMoral[i] > 0 && blessureChef[i])
                    {
                        if (!BlessureChef(i, lignePionsEnBataille012))
                        {
                            LogFile.Notifier("i=" + i + " erreur dans BlessureChef", out messageErreur);
                        }
                    }
                    if (pertesMoral[i + 3] > 0 && blessureChef[i + 3])
                    {
                        if (!BlessureChef(i + 3, lignePionsEnBataille345))
                        {
                            LogFile.Notifier("i=" + i + " erreur dans BlessureChef", out messageErreur);
                        }
                    }
                }
                #endregion

                for (i = 0; i < 3; i++)
                {
                    if (des[i] > 0)
                    {
                        message = string.Format("EffectuerBataille score sur zone{0}={1} sur un lancé de {2} dés, pertes moral={3} effectifs={4}%", i, score[i], des[i], pertesMoral[i], pertesEffectifs[i]);
                        LogFile.Notifier(message, out messageErreur);

                        if (!CalculDesPertesAuCombatParSecteur(i + 3, lignePionsEnBataille345, i, lignePionsEnBataille012, pertesMoral[i], pertesEffectifs[i]))
                        {
                            message = string.Format("CalculDesPertesAuCombat erreur dans CalculDesPertesAuCombatParSecteur zone{0}", i);
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }

                    if (des[i + 3] > 0)
                    {
                        message = string.Format("EffectuerBataille score sur zone{0}={1} sur un lancé de {2} dés, pertes moral={3} effectifs={4}%", i, score[i + 3], des[i + 3], pertesMoral[i + 3], pertesEffectifs[i + 3]);
                        LogFile.Notifier(message, out messageErreur);

                        if (!CalculDesPertesAuCombatParSecteur(i, lignePionsEnBataille012, i + 3, lignePionsEnBataille345, pertesMoral[i + 3], pertesEffectifs[i + 3]))
                        {
                            message = string.Format("CalculDesPertesAuCombat erreur dans CalculDesPertesAuCombatParSecteur zone{0}", i);
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }
                }
                return true;
            }

            /// <summary>
            /// Un chef d'un secteur est blesse dans une bataille
            /// </summary>
            /// <param name="zone">secteur dans lequel un chef doit être blessé (s'il y en a un)</param>
            /// <param name="lignePionsEnBataille">Pions dans la bataille zone 012 ou 345</param>
            /// <returns>true si OK, false si KO</returns>
            private bool BlessureChef(int zone, Donnees.TAB_PIONRow[] lignePionsEnBataille)
            {
                List<Donnees.TAB_PIONRow> listeChefsEnBataille = new List<Donnees.TAB_PIONRow>();

                //Recherche de tous les chefs présents dans le secteur
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille)
                {
                    if (lignePion.estQG && (!lignePion.IsI_ZONE_BATAILLENull() && lignePion.I_ZONE_BATAILLE == zone))
                    {
                        listeChefsEnBataille.Add(lignePion);
                    }
                }

                if (listeChefsEnBataille.Count > 0)
                {
                    Random de = new Random();
                    Donnees.TAB_PIONRow ligneChefBlesse = listeChefsEnBataille[de.Next(0, listeChefsEnBataille.Count)];
                    string message = string.Format("BlessureChef {0}:{1} est blessé", ligneChefBlesse.ID_PION, ligneChefBlesse.S_NOM);
                    LogFile.Notifier(message);

                    ligneChefBlesse.I_TOUR_BLESSURE = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                    //On determine la gravité de la blessure sur 2d6
                    int graviteBlessure = Constantes.JetDeDes(2);
                    //graviteBlessure = 8; //BEA, test sur blessure
                    if (graviteBlessure <= 7)
                    {
                        //blessure légère, chef indisponible pour un nombre d'heures égales au jet de dés, dans ce cas, le chef n'est pas remplacé
                        ligneChefBlesse.I_DUREE_HORS_COMBAT = graviteBlessure;
                    }
                    else
                    {
                        //il faut au minimum deux heures pour trouver un remplaçant, temps durant lequel l'unité ne peut pas être commandé
                        if (graviteBlessure <= 11)
                        {
                            //blessure grave, chef indisponible pour un nombre de jours égaux au jet de dés
                            ligneChefBlesse.I_DUREE_HORS_COMBAT = graviteBlessure * 24;
                        }
                        else
                        {
                            //sur un 12, le leader est mort
                            ligneChefBlesse.I_DUREE_HORS_COMBAT = int.MaxValue;//on ne revient jamais de la mort d'habitude
                        }
                        // -> Le chef remplaçant n'est crée qu'après une période d'inactivité (voir NouvelleHeure)
                    }
                }
                return true;
            }

            private bool CalculDesPertesAuCombatParSecteur(int zoneAttaquant, Donnees.TAB_PIONRow[] lignePionsEnBatailleAttaquant, int zoneDefenseur, Donnees.TAB_PIONRow[] lignePionsEnBatailleDefenseur, int pertesMoral, int pertesEffectif)
            {
                string message, messageErreur;
                int pertesInfanterieTotal, pertesCavalerieTotal, pertesArtillerieTotal;
                int pertesInfanterie, pertesCavalerie, pertesArtillerie;

                pertesInfanterieTotal = pertesCavalerieTotal = pertesArtillerieTotal = 0;

                //pertes infligés à l'ennemi, en % du global
                int effectifTotalDefenseur = EffectifTotalSurZone(zoneDefenseur, lignePionsEnBatailleDefenseur);
                foreach (Donnees.TAB_PIONRow lignePionEnBataille in lignePionsEnBatailleDefenseur)
                {
                    if (!lignePionEnBataille.estCombattif) { continue; }
                    if (lignePionEnBataille.IsI_ZONE_BATAILLENull() || lignePionEnBataille.I_ZONE_BATAILLE != zoneDefenseur) { continue; }
                    lignePionEnBataille.I_MORAL -= pertesMoral;

                    decimal rapporDePerteUnite = (decimal)lignePionEnBataille.effectifTotal / effectifTotalDefenseur;
                    //en-dessous d'une certaine taille, une unité est détruite pour ne pas avoir à gérer de trop petites unités, sauf si ce n'est que de l'artillerie
                    if (lignePionEnBataille.effectifTotal - (pertesEffectif * rapporDePerteUnite) < Constantes.CST_TAILLE_MINIMUM_UNITE)
                    {
                        //Message indiquant une unité détruite
                        pertesInfanterieTotal += lignePionEnBataille.I_INFANTERIE;
                        pertesCavalerieTotal += lignePionEnBataille.I_CAVALERIE;
                        pertesArtillerieTotal += lignePionEnBataille.I_ARTILLERIE;

                        //on detruit le pion et on envoie un message
                        lignePionEnBataille.DetruirePion();
                        if (!ClassMessager.EnvoyerMessage(lignePionEnBataille, ClassMessager.MESSAGES.MESSAGE_DETRUITE_AU_COMBAT, this))
                        {
                            message = string.Format("CalculDesPertesAuCombat : erreur lors de l'envoi d'un message MESSAGE_DETRUITE_AU_COMBAT");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                        continue;
                    }

                    //Pertes sur l'unité, repartie en pourcentage relatif du total
                    pertesInfanterie = (int)(lignePionEnBataille.I_INFANTERIE * pertesEffectif * rapporDePerteUnite / lignePionEnBataille.effectifTotal);
                    pertesCavalerie = (int)(lignePionEnBataille.I_CAVALERIE * pertesEffectif * rapporDePerteUnite / lignePionEnBataille.effectifTotal);
                    pertesArtillerie = (int)(lignePionEnBataille.I_ARTILLERIE * pertesEffectif * rapporDePerteUnite / lignePionEnBataille.effectifTotal);
                    pertesInfanterieTotal += pertesInfanterie;
                    pertesCavalerieTotal += pertesCavalerie;
                    pertesArtillerieTotal += pertesArtillerie;
                    lignePionEnBataille.I_INFANTERIE -= pertesInfanterie;
                    lignePionEnBataille.I_CAVALERIE -= pertesCavalerie;
                    lignePionEnBataille.I_ARTILLERIE -= pertesArtillerie;
                    if (lignePionEnBataille.Moral <= 0 && !lignePionEnBataille.estUniteArtillerie)
                    {
                        FuiteAuCombat(lignePionEnBataille, zoneDefenseur, lignePionsEnBatailleDefenseur);
                    }
                    else
                    {
                        if (pertesMoral > 0)
                        {
                            if (!ClassMessager.EnvoyerMessage(lignePionEnBataille, ClassMessager.MESSAGES.MESSAGE_PERTES_AU_COMBAT, pertesInfanterie, pertesCavalerie, pertesArtillerie, pertesMoral, 0, 0, this))
                            {
                                message = string.Format("CalculDesPertesAuCombat : erreur lors de l'envoi d'un message pour perte de moral au combat");
                                LogFile.Notifier(message, out messageErreur);
                                return false;
                            }
                        }
                    }
                }

                //message indiquant le niveau de pertes infligé à l'ennemi
                foreach (Donnees.TAB_PIONRow lignePionEnBataille in lignePionsEnBatailleAttaquant)
                {
                    if (lignePionEnBataille.B_DETRUIT || lignePionEnBataille.I_TOUR_FUITE_RESTANT > 0 || lignePionEnBataille.I_TOUR_RETRAITE_RESTANT > 0) { continue; }
                    if (lignePionEnBataille.IsI_ZONE_BATAILLENull() || lignePionEnBataille.I_ZONE_BATAILLE != zoneAttaquant) { continue; }

                    if (!ClassMessager.EnvoyerMessage(lignePionEnBataille, ClassMessager.MESSAGES.MESSAGE_TIR_SUR_ENNEMI, pertesInfanterieTotal, pertesCavalerieTotal, pertesArtillerieTotal, pertesMoral, 0, 0, this))
                    {
                        message = string.Format("CalculDesPertesAuCombat : erreur lors de l'envoi d'un message pour tir sur l'ennemi");
                        LogFile.Notifier(message, out messageErreur);
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// Si un leader se retrouve dans une zone sans unité combattante, il doit être remis en réserve
            /// </summary>
            /// <param name="ligneBataille">Bataille pour laquelle on recherche si un leader se retrouve sans unité au combat</param>
            private void RemiseLeaderEnReserve()
            {
                string message, messageErreur;
                int[] des;
                int[] effectifs;
                int[] canons;
                int i;
                int nbUnites012 = 0, nbUnites345 = 0;//pour les bonus stratégiques
                Donnees.TAB_PIONRow[] lignePionsEnBataille012;
                Donnees.TAB_PIONRow[] lignePionsEnBataille345;

                RecherchePionsEnBataille(out nbUnites012, out nbUnites345, out des, out effectifs, out canons, out lignePionsEnBataille012, out lignePionsEnBataille345, true/*bengagement*/, false/*bcombattif*/);
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
                        message = string.Format("EffectuerBataille le QG {1} ID={0} se retrouve seul en zone {2} et est remis à disposition",
                            ligneLeader012.ID_PION, ligneLeader012.S_NOM, i);
                        LogFile.Notifier(message, out messageErreur);
                        ligneLeader012.SetI_ZONE_BATAILLENull();
                        lignePionBataille = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(ligneLeader012.ID_PION, ID_BATAILLE);
                        lignePionBataille.B_ENGAGEE = false; // = dans le combat mais pas au front
                    }
                    if (null != ligneLeader345 && !bCombattants345)
                    {
                        message = string.Format("EffectuerBataille le QG {1} ID={0} se retrouve seul en zone {2} et est remis à disposition",
                            ligneLeader345.ID_PION, ligneLeader345.S_NOM, i + 3);
                        LogFile.Notifier(message, out messageErreur);
                        ligneLeader345.SetI_ZONE_BATAILLENull();
                        lignePionBataille = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(ligneLeader345.ID_PION, ID_BATAILLE);
                        lignePionBataille.B_ENGAGEE = false;// = dans le combat mais pas au front
                    }
                }
            }

            public int TrouverLeaderBataille(Donnees.TAB_PIONRow[] lignePionsEnBataille, bool bZone012)
            {
                int retourLeader = -1;
                char cHierarchie = 'Z';
                Donnees.TAB_PIONRow lignePionLeader = null;

                //s'il y a déjà un leader d'affecté, on redonne, par défaut, le même.
                if (bZone012 && !IsID_LEADER_012Null() && ID_LEADER_012 >= 0)
                {
                    retourLeader = ID_LEADER_012;
                    lignePionLeader = Donnees.m_donnees.TAB_PION.FindByID_PION(retourLeader);
                    cHierarchie = lignePionLeader.C_NIVEAU_HIERARCHIQUE;
                }
                if (!bZone012 && !IsID_LEADER_345Null() && ID_LEADER_345 >= 0)
                {
                    retourLeader = ID_LEADER_345;
                    lignePionLeader = Donnees.m_donnees.TAB_PION.FindByID_PION(retourLeader);
                    cHierarchie = lignePionLeader.C_NIVEAU_HIERARCHIQUE;
                }

                //on regarde s'il y a un leader de niveau hierarchique supérieur ou si aucun leader n'est encore choisi
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille)
                {
                    if (lignePion.B_DETRUIT || lignePion.B_BLESSES) { continue; }
                    if (lignePion.estQG && lignePion.I_STRATEGIQUE > 0)// test sur strategique ajouté à cause du cas "Bessières", général pouvant uniquement servir en appui tactique mais pas en commandement
                    {
                        if (lignePion.C_NIVEAU_HIERARCHIQUE == cHierarchie && retourLeader >= 0
                            && null == lignePionLeader)//si leader de même niveau déjà affecté à un tour précédent, on ne le change pas
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
            /// Quand aucune unité n'est engagée au combat, il faut en choisir une au hasard
            /// </summary>
            /// <param name="ligneBataille">bataille</param>
            /// <param name="lignePionsEnBataille">unités combattives dans la zones</param>
            /// <param name="iZone">zone d'engagement</param>
            /// <returns></returns>
            private bool AffecterUneUniteAuHasardEnBataille(Donnees.TAB_PIONRow[] lignePionsEnBataille, int iZone)
            {
                Donnees.TAB_PIONRow lignePion;

                if (lignePionsEnBataille.Count() <= 0) return false;
                Random de = new Random();
                lignePion = lignePionsEnBataille[de.Next(lignePionsEnBataille.Count())];
                lignePion.I_ZONE_BATAILLE = iZone;//toujours la zone centrale donc 1 ou 4
                //engagement dans la bataille
                Donnees.TAB_BATAILLE_PIONSRow lignePionBataille = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePion.ID_PION, ID_BATAILLE);
                if (null == lignePionBataille) return false;
                lignePionBataille.B_ENGAGEE = true;
                //pour les presentations de fin de partie
                lignePionBataille.B_ENGAGEMENT = true;
                lignePionBataille.I_ZONE_BATAILLE_ENGAGEMENT = iZone;
                return ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_ENGAGEMENT_AUTOMATIQUE_DANS_BATAILLE, this);
            }

            /// <summary>
            /// Pour toutes les unités situées à CST_BRUIT_DU_CANON et non inclue en bataille, elles reçoivent un message
            /// indiquant du bruit de la bataille
            /// </summary>
            private void AlerteBruitDuCanon()
            {
                double dist;
                Donnees.TAB_CASERow ligneCasePion;
                int xBataille, yBataille, i;

                //on prend le centre de la bataille comme "case" d'où part le bruit
                xBataille = (I_X_CASE_BAS_DROITE + I_X_CASE_HAUT_GAUCHE) / 2;
                yBataille = (I_Y_CASE_BAS_DROITE + I_Y_CASE_HAUT_GAUCHE) / 2;
                i = 0;
                while (i < Donnees.m_donnees.TAB_PION.Count())
                {
                    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                    //Pion ne se trouvant pas déjà dans la bataille
                    var resultComplet = from BataillePion in Donnees.m_donnees.TAB_BATAILLE_PIONS
                                        where (BataillePion.ID_PION == lignePion.ID_PION)
                                        && (BataillePion.ID_BATAILLE == ID_BATAILLE)
                                        select BataillePion.ID_PION;

                    if (0 == resultComplet.Count() && lignePion.estCombattifQG(true, true))
                    {
                        //Pion à distance correcte
                        ligneCasePion = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePion.ID_CASE);
                        dist = Constantes.Distance(ligneCasePion.I_X, ligneCasePion.I_Y, xBataille, yBataille);
                        if (dist < Constantes.CST_BRUIT_DU_CANON * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE)
                        {
                            ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_BRUIT_DU_CANON, this);
                        }
                    }
                    i++;
                }
            }

            public bool RecherchePionsEnBataille(out int nbUnites012, out int nbUnites345, out int[] des, out int[] effectifs, out int[] canons, out Donnees.TAB_PIONRow[] lignePionsEnBataille012, out Donnees.TAB_PIONRow[] lignePionsEnBataille345, bool? bEngagement, bool bCombattif)
            {
                des = new int[6];
                effectifs = new int[6];
                canons = new int[6];

                nbUnites012 = nbUnites345 = 0;
                lignePionsEnBataille012 = lignePionsEnBataille345 = null;

                if (!RecherchePionsEnBatailleParZone(ID_BATAILLE, true, out nbUnites012, ref des, ref effectifs, ref canons, out lignePionsEnBataille012, bEngagement, bCombattif))
                {
                    return false;
                }
                if (!RecherchePionsEnBatailleParZone(ID_BATAILLE, false, out nbUnites345, ref des, ref effectifs, ref canons, out lignePionsEnBataille345, bEngagement, bCombattif))
                {
                    return false;
                }
                return true;
            }

            /// <summary>
            /// Recherche tous les poins dans une zone et calcule les effectifs et les modificateurs de
            /// dés liés à la tactique ou à l'artillerie
            /// </summary>
            /// <param name="idBataille">identifiant de la bataille</param>
            /// <param name="bZone012">true zone 0,1,2, false zone 3,4,5</param>
            /// <param name="nbUnites">nombre d'unités dans la zone</param>
            /// <param name="des">modificateur sur les dés par zones</param>
            /// <param name="effectifs">effectifs des unités présentes</param>
            /// <param name="canons">nombre de canons des unités présentes</param>
            /// <param name="lignePionsEnBatailleZone">liste des pions présents</param>
            /// <param name="bEngagement">true si on ne prend que les unités engagées, false si on prend aussi les elligibles</param>
            /// <param name="bCombattif">true si on ne prend que les unités combattives</param>
            /// <returns>true si ok, false si ko</returns>
            public bool RecherchePionsEnBatailleParZone(int idBataille, bool bZone012, out int nbUnites, ref int[] des, ref int[] effectifs, ref int[] canons, out Donnees.TAB_PIONRow[] lignePionsEnBatailleZone, bool? bEngagement, bool bCombattif)
            {
                //string requete;
                //DataSetCoutDonnees.TAB_BATAILLE_PIONSRow[] lignePionsEnBataille;
                Donnees.TAB_PIONRow lignePion;
                int nb, i;
                nbUnites = 0;
                lignePionsEnBatailleZone = null;
                int idNation;
                Donnees.TAB_BATAILLERow ligneBataille = Donnees.m_donnees.TAB_BATAILLE.FindByID_BATAILLE(idBataille);
                int[] valeurTactique = new int[6];
                bool[] presenceCavalerieDeLigne = new bool[] { false, false, false, false, false, false };
                bool[] presenceCavalerieLourde = new bool[] { false, false, false, false, false, false };
                bool[] presenceGarde = new bool[] { false, false, false, false, false, false };
                bool[] presenceVieilleGarde = new bool[] { false, false, false, false, false, false };

                idNation = bZone012 ? ligneBataille.ID_NATION_012 : ligneBataille.ID_NATION_345;
                //recherche toutes les unités engagées dans une bataille par une nation (donc dans une zone)
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
                var resultComplet = from BataillePion in Donnees.m_donnees.TAB_BATAILLE_PIONS
                                    from Pion in Donnees.m_donnees.TAB_PION
                                    where (BataillePion.ID_PION == Pion.ID_PION)
                                        //&& (BataillePion.B_ENGAGEE == bEngagement)
                                    && (BataillePion.ID_NATION == idNation)
                                    && (BataillePion.ID_BATAILLE == idBataille)
                                    && !Pion.B_DETRUIT
                                    select new rechercheQG { idPion = Pion.ID_PION, bEngagee = BataillePion.B_ENGAGEE, izoneEngagement = Pion.IsI_ZONE_BATAILLENull() ? -1 : Pion.I_ZONE_BATAILLE };

                IEnumerable<int> resultPionsBataille;
                if (null == bEngagement)
                {
                    resultPionsBataille = from resultatPion in resultComplet
                                          select resultatPion.idPion;
                }
                else
                {
                    //s'il ne s'agit pas d'un QG il faut, suivant les cas, choisir les unitées engagées ou non
                    if (bEngagement == true)
                    {
                        resultPionsBataille = from resultatPion in resultComplet
                                              where (resultatPion.bEngagee == bEngagement) && resultatPion.izoneEngagement >= 0
                                              select resultatPion.idPion;
                    }
                    else
                    {
                        resultPionsBataille = from resultatPion in resultComplet
                                              where (resultatPion.bEngagee == bEngagement)
                                              select resultatPion.idPion;
                    }
                }

                //compte le nombre de pions concernés
                nb = 0;
                for (i = 0; i < resultPionsBataille.Count(); i++)
                {
                    lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(resultPionsBataille.ElementAt(i));
                    //le pion peut avoir disparu s'il a fuit et à été détruit
                    if (!bCombattif || lignePion.estCombattif)
                    {
                        nb++;
                    }
                }
                lignePionsEnBatailleZone = new Donnees.TAB_PIONRow[nb];

                //affecte la table et les valeurs
                for (i = 0; i < 6; i++) { valeurTactique[i] = -1; }

                nb = 0;
                for (i = 0; i < resultPionsBataille.Count(); i++)
                {
                    lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(resultPionsBataille.ElementAt(i));
                    if (null != lignePion && (!bCombattif || lignePion.estCombattif))
                    {
                        lignePionsEnBatailleZone[nb++] = lignePion;

                        if (!lignePion.IsI_ZONE_BATAILLENull() && lignePion.I_ZONE_BATAILLE >= 0)//cas du leader de la zone, engagé mais sans zone
                        {
                            effectifs[lignePion.I_ZONE_BATAILLE] += lignePion.I_INFANTERIE + lignePion.I_CAVALERIE;
                            des[lignePion.I_ZONE_BATAILLE] += (int)Math.Floor(lignePion.I_EXPERIENCE);
                            canons[lignePion.I_ZONE_BATAILLE] += lignePion.I_ARTILLERIE;

                            //modification liée à la valeur de matériel et de ravitaillement
                            des[lignePion.I_ZONE_BATAILLE] += Constantes.CalculerEfficaciteAuCombat(lignePion.I_MATERIEL, lignePion.I_RAVITAILLEMENT);

                            if (valeurTactique[lignePion.I_ZONE_BATAILLE] < lignePion.I_TACTIQUE)
                            {
                                valeurTactique[lignePion.I_ZONE_BATAILLE] = lignePion.I_TACTIQUE;
                            }
                            if (lignePion.B_CAVALERIE_DE_LIGNE) { presenceCavalerieDeLigne[lignePion.I_ZONE_BATAILLE] = true; }
                            if (lignePion.B_CAVALERIE_LOURDE) { presenceCavalerieLourde[lignePion.I_ZONE_BATAILLE] = true; }
                            if (lignePion.B_GARDE) { presenceGarde[lignePion.I_ZONE_BATAILLE] = true; }
                            if (lignePion.B_VIEILLE_GARDE) { presenceVieilleGarde[lignePion.I_ZONE_BATAILLE] = true; }
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

                for (i = 0; i < 6; i++)
                {
                    if (valeurTactique[i] >= 0)
                    {
                        //si un chef est au moins présent, valeurTactique est passé au minimum à 0 (au lieu de -1)
                        //si un chef est présent, on ajoute au minimum 1 dé supplémentaire.
                        des[i] += Math.Max(1, valeurTactique[i]);
                        LogFile.Notifier(string.Format("RecherchePionsEnBatailleParZone valeurTactique sur zone={0} = {1}", i, valeurTactique[i]));
                    }
                    if (presenceCavalerieDeLigne[i]) { des[i] += 1; }
                    if (presenceCavalerieLourde[i]) { des[i] += 2; }
                    if (presenceGarde[i]) { des[i] += 2; }
                    if (presenceVieilleGarde[i]) { des[i] += 3; }
                }

                return true;
            }

            private bool FuiteAuCombat(Donnees.TAB_PIONRow lignePionFuite, int zone, Donnees.TAB_PIONRow[] lignePionsEnBataille)
            {
                string message, messageErreur;
                bool[] unitesCombattiveHorsArtillerie= new bool[6];
                //int iPertesInfanterie, iPertesCavalerie, iPertesArtillerie;

                Donnees.TAB_BATAILLE_PIONSRow ligneBataillePions = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePionFuite.ID_PION, ID_BATAILLE);
                if (null == ligneBataillePions)
                {
                    message = string.Format("FuiteAuCombat : impossible de retrouver le pion ID=" + lignePionFuite.ID_PION + " dans la bataille ID=" + ID_BATAILLE);
                    LogFile.Notifier(message, out messageErreur);
                    return false;
                }

                //lorsque qu'une unité fuit au combat, toutes les unités présentes dans la même zone, perdent CST_PERTE_MORAL_FUITE pts de moral
                message = string.Format("FuiteAuCombat : L'unité {0} fuit la zone {1} de la bataille {2}",
                    lignePionFuite.ID_PION, zone, lignePionFuite.ID_BATAILLE);
                LogFile.Notifier(message, out messageErreur);
                for (int i=0; i<6; i++) {unitesCombattiveHorsArtillerie[i]=false;}
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille)
                {
                    if (lignePion.ID_PION != lignePionFuite.ID_PION &&
                        !lignePion.IsI_ZONE_BATAILLENull() && //possible si l'unité a déjà fuit le combat
                        lignePion.I_ZONE_BATAILLE == zone && lignePion.estCombattif
                        && !lignePion.estUniteArtillerie)
                    {
                        lignePion.I_MORAL -= Constantes.CST_PERTE_MORAL_FUITE;
                        if (lignePion.Moral <= 0)
                        {
                            FuiteAuCombat(lignePion, zone, lignePionsEnBataille);
                        }
                        else
                        {
                            unitesCombattiveHorsArtillerie[zone]=true;
                        }
                    }
                }

                if (!unitesCombattiveHorsArtillerie[zone])
                {
                    //S'il n'y a plus que des unités d'artillerie présentes, elles doivent fuir aussi
                    foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille)
                    {
                        if (lignePion.ID_PION != lignePionFuite.ID_PION &&
                            !lignePion.IsI_ZONE_BATAILLENull() && //possible si l'unité a déjà fuit le combat
                            lignePion.I_ZONE_BATAILLE == zone && lignePion.estCombattif)
                        {
                            FuiteAuCombat(lignePion, zone, lignePionsEnBataille);
                        }
                    }
                }

                /* Cela n'est plus vrai avec les règles avancées
                //L'unité perd entre 1000 et 3000 hommes en cas de fuite par perte au moral. L'infanterie prend
                //les premières pertes, la cavalerie ensuite.            
                int pertes = (int)Math.Round(Convert.ToDecimal(Constantes.JetDeDes(1))/2,MidpointRounding.AwayFromZero);

                RepartirPertes(lignePionFuite, pertes*1000, out iPertesInfanterie, out iPertesCavalerie, out iPertesArtillerie);
                */

                //message pour prévenir du désengagement
                if (lignePionFuite.effectifTotal > 0)
                {
                    if (!ClassMessager.EnvoyerMessage(lignePionFuite, ClassMessager.MESSAGES.MESSAGE_FUITE_AU_COMBAT,
                            ligneBataillePions.I_INFANTERIE_DEBUT - ligneBataillePions.I_INFANTERIE_FIN,
                            ligneBataillePions.I_CAVALERIE_DEBUT - ligneBataillePions.I_ARTILLERIE_FIN,
                            ligneBataillePions.I_ARTILLERIE_DEBUT - ligneBataillePions.I_ARTILLERIE_FIN, Constantes.CST_PERTE_MORAL_FUITE, 0, 0, this))
                    {
                        message = string.Format("FuiteAuCombat : erreur lors de l'envoi d'un message pour fuite au combat");
                        LogFile.Notifier(message, out messageErreur);
                        return false;
                    }

                    //l'unité fuit automatiquement vers son chef
                    lignePionFuite.SupprimerTousLesOrdres();
                    Donnees.TAB_PIONRow lignePionChef = lignePionFuite.proprietaire;
                    Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                        -1,//ID_ORDRE_TRANSMIS
                        -1,//ID_ORDRE_SUIVANT global::System.Convert.DBNull,
                        -1,///ID_ORDRE_WEB
                        Constantes.ORDRES.MOUVEMENT,
                        lignePionFuite.ID_PION,
                        lignePionFuite.ID_CASE,
                        lignePionFuite.I_INFANTERIE + lignePionFuite.I_CAVALERIE + lignePionFuite.I_ARTILLERIE,
                        lignePionChef.ID_CASE,
                        -1,//id ville de destination
                        0,//I_EFFECTIF_DESTINATION
                        Donnees.m_donnees.TAB_PARTIE[0].I_TOUR,//I_TOUR_DEBUT
                        Donnees.m_donnees.TAB_PARTIE[0].I_PHASE,//I_PHASE_DEBUT
                        -1,//I_TOUR_FIN
                        -1,//I_PHASE_FIN
                        -1,//ID_MESSAGE
                        -1,//ID_DESTINATAIRE
                        -1,//ID_CIBLE
                        -1,//ID_DESTINATAIRE_CIBLE
                        -1,//null
                        -1,//I_ZONE_BATAILLE
                        Donnees.m_donnees.TAB_JEU[0].I_LEVER_DU_SOLEIL,//I_HEURE_DEBUT
                        Donnees.m_donnees.TAB_JEU[0].I_COUCHER_DU_SOLEIL - Donnees.m_donnees.TAB_JEU[0].I_LEVER_DU_SOLEIL,//I_DUREE
                        -1//I_ENGAGEMENT
                        );//ID_BATAILLE
                    ligneOrdre.SetID_ORDRE_TRANSMISNull();
                    ligneOrdre.SetID_ORDRE_SUIVANTNull();
                    ligneOrdre.SetID_ORDRE_WEBNull();
                    ligneOrdre.SetID_MESSAGENull();
                    ligneOrdre.SetID_DESTINATAIRENull();
                    ligneOrdre.SetID_BATAILLENull();
                    ligneOrdre.SetI_TOUR_FINNull();
                    ligneOrdre.SetI_PHASE_FINNull();
                    ligneOrdre.SetID_NOM_DESTINATIONNull();
                    ligneOrdre.SetID_CIBLENull();
                    ligneOrdre.SetID_DESTINATAIRE_CIBLENull();
                    ligneOrdre.SetI_ENGAGEMENTNull();
                }
                else
                {
                    //l'unité est détruite
                    lignePionFuite.DetruirePion();//à faire avant le message, sinon, non détruite dans le message
                    if (!ClassMessager.EnvoyerMessage(lignePionFuite, ClassMessager.MESSAGES.MESSAGE_FUITE_ET_DETRUITE_AU_COMBAT, this))
                    {
                        message = string.Format("FuiteAuCombat : erreur lors de l'envoi d'un message pour fuite au combat");
                        LogFile.Notifier(message, out messageErreur);
                        return false;
                    }
                    //lignePionFuite.Delete();
                }
                //mise à jour dans la table des pions au combat pour l'affichage de fin de partie
                ligneBataillePions.B_RETRAITE = true;
                ligneBataillePions.I_INFANTERIE_FIN = lignePionFuite.I_INFANTERIE;
                ligneBataillePions.I_CAVALERIE_FIN = lignePionFuite.I_CAVALERIE;
                ligneBataillePions.I_ARTILLERIE_FIN = lignePionFuite.I_ARTILLERIE;
                ligneBataillePions.I_MORAL_FIN = lignePionFuite.I_MORAL;
                ligneBataillePions.I_FATIGUE_FIN = lignePionFuite.I_FATIGUE;

                //desengagement du combat
                //lignePionFuite.SetID_BATAILLENull(); l'unité ne quitte définitivement le combat que lorsque la fuite est terminée
                //lignePionFuite.SetI_ZONE_BATAILLENull();, à ne pas faire car la zone est encore utile pour calculer les pertes par secteur
                lignePionFuite.B_FUITE_AU_COMBAT = true;//utile uniquement pour certaines conditions de victoires où l'on compte le nombre d'unités démoralisées durant la partie
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
                        ///dés que l'artillerie subit une perte, elle est détruite
                        iPertesCavalerie = lignePion.I_CAVALERIE;
                        iPertesArtillerie = lignePion.I_ARTILLERIE;
                    }
                }

                lignePion.I_INFANTERIE -= iPertesInfanterie;
                lignePion.I_CAVALERIE -= iPertesCavalerie;
                lignePion.I_ARTILLERIE -= iPertesArtillerie;

                if (lignePion.effectifTotal > 0)
                {
                    //si l'unité est en cours de mouvement (du a une retraite normalement) et que, du fait des pertes, 
                    // tout ce qui "reste" est déjà arrivée en position, il faut considérer l'unité comme arrivée à destination
                    Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(lignePion.ID_PION);
                    if (null != ligneOrdre)
                    {
                        if (ligneOrdre.I_EFFECTIF_DESTINATION >= lignePion.effectifTotal)
                        {
                            //les derniers viennent d'arriver
                            string message = string.Format("RepartirPertes {0}(ID={1}, en mouvement, les pertes font que les derniers sont arrivés)", lignePion.S_NOM, lignePion.ID_PION);
                            LogFile.Notifier(message);
                            if (!lignePion.ArriveADestination(ligneOrdre, null)) { return false; }
                        }
                    }
                }
                return true;
            }

            /// <summary>
            /// Calcul des pertes réels en fin de bataille, répartition entre prisonniers et blessés
            /// </summary>
            /// <param name="lignePionsEnBataille">liste des pions dont on calcule les pertes</param>
            /// <param name="bVictoire">true si les unités sont victorieuse dans la bataille, false sinon</param>
            /// <param name="lignePionsEnnemis">Liste des pions ennemis susceptibles de prendre en charge les prisonniers</param>
            /// <returns>true si ok, false si ko</returns>
            private bool PertesFinDeBataille(Donnees.TAB_PIONRow lignePion, Donnees.TAB_BATAILLE_PIONSRow ligneBataillePion, bool bVictoire, Donnees.TAB_PIONRow[] lignePionsEnnemis)
            {
                string message;
                int infanterieBlesse, cavalerieBlesse;

                //if (lignePion.B_DETRUIT) { return true; } -> si detruit, n'a plus de moral, donc tout va arriver en blessé grave

                if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.MESSAGE_PERTES_TOTALE_AU_COMBAT,
                    ligneBataillePion.I_INFANTERIE_DEBUT - ligneBataillePion.I_INFANTERIE_FIN,
                    ligneBataillePion.I_CAVALERIE_DEBUT - ligneBataillePion.I_CAVALERIE_FIN,
                    ligneBataillePion.I_ARTILLERIE_DEBUT - ligneBataillePion.I_ARTILLERIE_FIN,
                    ligneBataillePion.I_MORAL_DEBUT - ligneBataillePion.I_MORAL_FIN, 0, 0, this))
                {
                    message = string.Format("PertesFinDeBataille : erreur lors de l'envoi d'un message MESSAGE_PERTES_AU_COMBAT");
                    LogFile.Notifier(message);
                    return false;
                }

                infanterieBlesse = (ligneBataillePion.I_INFANTERIE_DEBUT - ligneBataillePion.I_INFANTERIE_FIN) * 4 / 10;
                cavalerieBlesse = (ligneBataillePion.I_CAVALERIE_DEBUT - ligneBataillePion.I_CAVALERIE_FIN) * 4 / 10;
                if (bVictoire)
                {
                    //les blessés legers reviennent au prorata du moral résiduel, les autres sont des blessés graves
                    int infanterieRetour = (ligneBataillePion.I_INFANTERIE_DEBUT - ligneBataillePion.I_INFANTERIE_FIN) * 3 * ligneBataillePion.I_MORAL_FIN / 10 / lignePion.I_MORAL_MAX;
                    int cavalerieRetour = (ligneBataillePion.I_CAVALERIE_DEBUT - ligneBataillePion.I_CAVALERIE_FIN) * 3 * ligneBataillePion.I_MORAL_FIN / 10 / lignePion.I_MORAL_MAX;
                    if (infanterieRetour > 0 || cavalerieRetour > 0)
                    {
                        lignePion.I_INFANTERIE += infanterieRetour;
                        lignePion.I_CAVALERIE += cavalerieRetour;
                        if (!ClassMessager.EnvoyerMessage(lignePion, ClassMessager.MESSAGES.SOINS_APRES_BATAILLE, infanterieRetour, cavalerieRetour, 0, 0, 0, 0, this))
                        {
                            message = string.Format("PertesFinDeBataille : erreur lors de l'envoi d'un message SOINS_APRES_BATAILLE");
                            LogFile.Notifier(message);
                            return false;
                        }
                    }

                    infanterieBlesse += ((ligneBataillePion.I_INFANTERIE_DEBUT - ligneBataillePion.I_INFANTERIE_FIN) * 3 / 10) - infanterieRetour;
                    cavalerieBlesse += ((ligneBataillePion.I_CAVALERIE_DEBUT - ligneBataillePion.I_CAVALERIE_FIN) * 3 / 10) - cavalerieRetour;
                }
                else
                {
                    int infanteriePrisonnier = (ligneBataillePion.I_INFANTERIE_DEBUT - ligneBataillePion.I_INFANTERIE_FIN) * 3 / 10;
                    int cavaleriePrisonnier = (ligneBataillePion.I_CAVALERIE_DEBUT - ligneBataillePion.I_CAVALERIE_FIN) * 3 / 10;
                    if (infanteriePrisonnier + cavaleriePrisonnier >= Constantes.CST_TAILLE_MINIMUM_UNITE)
                    {
                        //on déterminer au hasard l'unité qui a fait la capture
                        Random de = new Random();
                        Donnees.TAB_PIONRow lignePionQuiCapture = lignePionsEnnemis[de.Next(0, lignePionsEnnemis.Length - 1)];

                        Donnees.TAB_PIONRow lignePionConvoiDePrisonniers = lignePion.CreerConvoiDePrisonniers(lignePionQuiCapture, infanteriePrisonnier, cavaleriePrisonnier, 0);
                        if (null == lignePionConvoiDePrisonniers)
                        {
                            message = string.Format("PertesFinDeBataille : erreur lors de l'appel à CreerConvoiDePrisonniers");
                            LogFile.Notifier(message);
                            return false;
                        }
                        if (!ClassMessager.EnvoyerMessage(lignePionConvoiDePrisonniers, ClassMessager.MESSAGES.MESSAGE_PRISONNIERS_APRES_BATAILLE, this))
                        {
                            message = string.Format("PertesFinDeBataille : erreur lors de l'envoi d'un message MESSAGE_PRISONNIERS_APRES_BATAILLE");
                            LogFile.Notifier(message);
                            return false;
                        }
                    }
                }

                //on crée le convoi de blessés
                if (infanterieBlesse + cavalerieBlesse >= Constantes.CST_TAILLE_MINIMUM_UNITE)
                {
                    Donnees.TAB_PIONRow lignePionConvoiDeBlesses = lignePion.CreerConvoi(lignePion.proprietaire, true /*bBlesses*/, false /*bPrisonniers*/, false /*bRenfort*/);
                    if (null == lignePionConvoiDeBlesses)
                    {
                        message = string.Format("PertesFinDeBataille : erreur lors de l'appel à CreerConvoi pour les blessés");
                        LogFile.Notifier(message);
                        return false;
                    }
                    lignePionConvoiDeBlesses.I_INFANTERIE = infanterieBlesse;
                    lignePionConvoiDeBlesses.I_CAVALERIE = cavalerieBlesse;
                    if (!ClassMessager.EnvoyerMessage(lignePionConvoiDeBlesses, ClassMessager.MESSAGES.MESSAGE_BLESSES_APRES_BATAILLE, this))
                    {
                        message = string.Format("PertesFinDeBataille : erreur lors de l'envoi d'un message MESSAGE_BLESSES_APRES_BATAILLE");
                        LogFile.Notifier(message);
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// Gain d'expérience pour toutes les unités ayant participées à une bataille de 4 heures ou plus
            /// </summary>
            /// <returns>true si ok, false si ko</returns>
            private bool GainExperienceFinDeBataille(Donnees.TAB_PIONRow[] lignePionsEnBataille)
            {
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille)
                {
                    if (!lignePion.GainExperienceFinDeBataille()) { return false; }
                }
                return true;
            }

        }
    }
}
