using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
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
                string message;
                bool bEnDefense;

                if (lignePion.estMessager || lignePion.estPatrouille || lignePion.estDepot || lignePion.estBlesses || lignePion.estConvoi || lignePion.estPrisonniers || lignePion.estCapturable)
                {
                    return true;
                }
                //si l'unité est déjà dans un combat ou est en fuite ou n'a plus de moral, il ne faut pas l'ajouter
                //if (!uniteCreantLaBataille && (lignePion.estAuCombat || !lignePion.estCombattifQG(true, false))), si on ajoute estCombattifQG, une unité sans matériel ou moral passe au travers les lignes sans problèmes !// && 0==lignePion.effectifTotal) si on met la condition précédente, une unité en retraite devient elligible sur un combat
                if (!uniteCreantLaBataille && lignePion.estAuCombat)
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

                    LogFile.Notifier(string.Format("AjouterPionDansLaBataille:AddTAB_BATAILLE_PIONSRow {0},{1}",
                        lignePion.S_NOM, lignePion.ID_PION));
                    Monitor.Enter(Donnees.m_donnees.TAB_BATAILLE_PIONS.Rows.SyncRoot);
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
                        Monitor.Exit(Donnees.m_donnees.TAB_BATAILLE_PIONS.Rows.SyncRoot);
                        message = string.Format("AjouterPionDansLaBataille : impossible d'ajouter le pion {0} à la bataille {1}", lignePion.ID_PION, ID_BATAILLE);
                        LogFile.Notifier(message);
                        return false;//problème à l'ajout
                    }
                    Monitor.Exit(Donnees.m_donnees.TAB_BATAILLE_PIONS.Rows.SyncRoot);
                    Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                    lignePion.ID_BATAILLE = ID_BATAILLE;

                    //il faut terminer tout ordre en cours d'execution par l'unité et placer l'unité dans la case courante du combat
                    lignePion.ID_CASE = ligneCaseCombat.ID_CASE;
                    lignePion.SupprimerTousLesOrdres();
                    lignePion.PlacerStatique();
                    Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);

                    //il faut envoyer un message au propriétaire de l'unité pour indiquer cet ajout
                    EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_ARRIVEE_DANS_BATAILLE);

                    message = string.Format("AjouterPionDansLaBataille : ajout du pion {0} à la bataille {1}, defensif={2} sur idcase={3}",
                        lignePion.ID_PION, ID_BATAILLE, bEnDefense, lignePion.ID_CASE);
                    LogFile.Notifier(message);

                    //si un leader suivait l'unité, il doit être inclus immédiatement
                    foreach (Donnees.TAB_ROLERow ligneRole in Donnees.m_donnees.TAB_ROLE)
                    {
                        Donnees.TAB_PIONRow lignePionRole = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneRole.ID_PION);
                        Donnees.TAB_ORDRERow ligneOrdreRole = Donnees.m_donnees.TAB_ORDRE.Mouvement(lignePionRole.ID_PION);
                        if ((null != ligneOrdreRole) && (ligneOrdreRole.I_ORDRE_TYPE == Constantes.ORDRES.SUIVRE_UNITE) && (ligneOrdreRole.ID_CIBLE == lignePion.ID_PION))
                        {
                            AjouterPionDansLaBataille(lignePionRole, ligneCaseCombat);
                            //on s'assure que l'ajout de ce chef ne modifie pas le responsable de la bataille -> inutile fait dans effectuerbataille à la fin de chaque tour même si la bataille n'est pas terminée
                            //si, il faut le faire, sinon, à la création initiale, le chef n'est pas là
                            int idLeader;
                            int[] des, modificateurs, effectifs, canons;
                            Donnees.TAB_PIONRow[] tablePionsPresents012;
                            Donnees.TAB_PIONRow[] tablePionsPresents345;
                            if (!RecherchePionsEnBataille(out int nbUnites012, out int nbUnites345, out des, out modificateurs, out effectifs, out canons, out tablePionsPresents012, out tablePionsPresents345, null/*bengagement*/, false/*bcombattif*/, true/*QG*/, true /*bArtillerie*/))
                            {
                                message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille QG");
                                LogFile.Notifier(message);
                            }

                            //trouver le leader avec le plus haut niveau hierarchique
                            if (lignePionRole.nation.ID_NATION == this.ID_NATION_012)
                            {
                                idLeader = TrouverLeaderBataille(tablePionsPresents012, true);
                                this.ID_LEADER_012 = idLeader;
                            }
                            else
                            {
                                idLeader = TrouverLeaderBataille(tablePionsPresents345, false);
                                this.ID_LEADER_345=idLeader;
                            }
                        }

                    }
                }
                return true;
            }

            public bool FinDeBataille(ref bool bFinDeBataille)
            {
                return FinDeBataille(false, false, ref bFinDeBataille);
            }

            /// <summary>
            /// Fin d'une bataille soit par manque de combattants soit à cause de l'arrivée de la nuit
            /// </summary>
            /// <param name="ligneBataille">Bataille à terminer</param>
            /// <returns>true si OK, false si KO</returns>
            public bool FinDeBataille(bool bRetraite012, bool bRetraite345, ref bool bFinDeBataille)
            {
                string message;
                Donnees.TAB_PIONRow[] lignePionsEnBataille012;
                Donnees.TAB_PIONRow[] lignePionsEnBataille345;
                Donnees.TAB_PIONRow[] lignePionsCombattifBataille012;
                Donnees.TAB_PIONRow[] lignePionsCombattifBataille345;
                Donnees.TAB_PIONRow[] lignePionsEnBatailleRetraite012;
                Donnees.TAB_PIONRow[] lignePionsEnBatailleRetraite345;
                string requete;
                bool bVictoire012 = true, bVictoire345 = true;

                //fin de la bataille, toutes les unités engagées gagnent de l'expérience
                I_TOUR_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                I_PHASE_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;

                //initialiation des données pour la video
                for (int z = 0; z < 6; z++)
                {
                    this["S_COMBAT_" + Convert.ToString(z)] = string.Empty;
                    this["I_PERTES_" + Convert.ToString(z)] = 0;
                }

                //Donnees.TAB_PIONRow lignePionTest = Donnees.m_donnees.TAB_PION.FindByID_PION(20);
                //recherche de tous les pions présents sur le champ de bataille et qui "voient" le résultat et peuvent subir une poursuite
                if (!RecherchePionsEnBataille(out _, out _, out _, out _, out _, out _,
                    out lignePionsEnBataille012, out lignePionsEnBataille345,
                    true,//engagement
                    false,//combattif
                    true,//QG
                    true//bArtillerie
                    ))
                {
                    message = string.Format("FinDeBataille : erreur dans RecherchePionsEnBataille I");
                    LogFile.Notifier(message);
                }

                if (!RecherchePionsEnBataille(out int nbUnites012, out int nbUnites345, out _, out _, out _, out _,
                    out lignePionsCombattifBataille012, out lignePionsCombattifBataille345, null /*engagement*/, true/*combattif*/, false/*QG*/, false /*bArtillerie*/))
                {
                    message = string.Format("FinDeBataille : erreur dans RecherchePionsEnBataille II");
                    LogFile.Notifier(message);
                }

                message = string.Format("FinDeBataille : après les pertes au combat il reste nbUnites012={0}  nbUnites345={1}",
                    nbUnites012, nbUnites345);
                LogFile.Notifier(message);
                if (bRetraite012 || bRetraite345 ||
                    (!Donnees.m_donnees.TAB_PARTIE.Nocturne(m_donnees.TAB_PARTIE.HeureCourante())//il ne faut pas mettre +1, même une phase avant la nuit, on peut encore faire un résultat de bataille
                        && (0 == nbUnites012 || 0 == nbUnites345)
                        && (nbUnites012 > 0 || nbUnites345 > 0)
                    )
                    && (Donnees.m_donnees.TAB_PARTIE[0].I_TOUR - I_TOUR_DEBUT >= 2))
                {
                    //un des deux camps a remporté le combat, il engage une poursuite sur le vaincu si on est pas la nuit
                    if (!RecherchePionsEnBataille(out _, out _, out _, out _, out _, out _,
                        out lignePionsEnBatailleRetraite012, out lignePionsEnBatailleRetraite345, null /*engagement*/, false/*combattif*/, true/*QG*/, true /*bArtillerie*/))
                    {
                        message = string.Format("FinDeBataille : erreur dans RecherchePionsEnBataille III");
                        LogFile.Notifier(message);
                    }

                    if (nbUnites012 > 0 || bRetraite345)
                    {
                        Poursuite(ID_LEADER_012, lignePionsEnBataille012, ID_LEADER_345, lignePionsEnBataille345);
                        SortieDuChampDeBataille(lignePionsEnBatailleRetraite345);
                        if (I_TOUR_FIN - I_TOUR_DEBUT >= 4)
                        {
                            GainMoralFinDeBataille(lignePionsCombattifBataille012);
                        }
                        EnvoyerMessageBataillesVictoireDefaite(lignePionsEnBataille012, lignePionsEnBataille345);
                        DesengagementDuVaincu(false);
                        //victoireCombat = VICTOIRECOMBAT.VICTOIRE012;
                        bVictoire345 = false;
                    }
                    else
                    {
                        Poursuite( ID_LEADER_345, lignePionsEnBataille345, ID_LEADER_012, lignePionsEnBataille012);
                        SortieDuChampDeBataille(lignePionsEnBatailleRetraite012);
                        if (I_TOUR_FIN - I_TOUR_DEBUT >= 4)
                        {
                            GainMoralFinDeBataille(lignePionsCombattifBataille345);
                        }
                        EnvoyerMessageBataillesVictoireDefaite(lignePionsEnBataille345, lignePionsEnBataille012);
                        //victoireCombat = VICTOIRECOMBAT.VICTOIRE345;
                        bVictoire012 = false;
                        DesengagementDuVaincu(true);
                    }
                    if (bVictoire012) { S_FIN = "VICTOIRE012"; }
                    if (bVictoire345) { S_FIN = "VICTOIRE345"; }
                    if (bRetraite012) { S_FIN = "RETRAITE012"; }
                    if (bRetraite345) { S_FIN = "RETRAITE345"; }
                }
                else
                {
                    //victoireCombat = VICTOIRECOMBAT.EGALITE;
                    //fin du combat à la nuit ou s'il n'y a plus d'unités présentes, les deux armées étant présentes ou absentes, il n'y a aucun bonus de moral
                    FinDeBatailleEgalite(lignePionsEnBataille012);
                    FinDeBatailleEgalite(lignePionsEnBataille345);
                    S_FIN = "NUIT";
                }

                //les données de poursuite sont mis en données surnuméraires par rapport au déroulement standard d'où le +1
                AjouterDonneesVideo(Donnees.m_donnees.TAB_PARTIE[0].I_TOUR+1, Donnees.m_donnees.TAB_BATAILLE_VIDEO, Donnees.m_donnees.TAB_BATAILLE_PIONS_VIDEO);

                //desengagement de toutes les unités
                requete = string.Format("ID_BATAILLE={0}", ID_BATAILLE);
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                Donnees.TAB_PIONRow[] resPion = (Donnees.TAB_PIONRow[])Donnees.m_donnees.TAB_PION.Select(requete);
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                for (int l=0; l<resPion.Length; l++)
                {
                    Donnees.TAB_PIONRow lignePion = resPion[l];
                    lignePion.SetID_BATAILLENull();
                    lignePion.SetI_ZONE_BATAILLENull();
                    Donnees.TAB_BATAILLE_PIONSRow ligneBataillePions = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePion.ID_PION, ID_BATAILLE);
                    if (null == ligneBataillePions)
                    {
                        message = string.Format("FinDeBataille : impossible de retrouver le pion ID=" + lignePion.ID_PION + " dans la bataille ID=" + ID_BATAILLE);
                        LogFile.Notifier(message);
                        return false;
                    }
                    else
                    {
                        ligneBataillePions.I_INFANTERIE_FIN = lignePion.I_INFANTERIE;
                        ligneBataillePions.I_CAVALERIE_FIN = lignePion.I_CAVALERIE;
                        ligneBataillePions.I_ARTILLERIE_FIN = lignePion.I_ARTILLERIE;
                        ligneBataillePions.I_MORAL_FIN = lignePion.I_MORAL;
                        ligneBataillePions.I_FATIGUE_FIN = lignePion.I_FATIGUE;

                        if (lignePion.estQG || lignePion.estConvoi || lignePion.estRenfort)
                        {
                            //remet les zones du dernier message reçu à vide, un peu débile mais sinon l'unité est toujours marquée comme engagée !
                            //Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageRecu(lignePion.ID_PION);
                            //ligneMessage.SetID_BATAILLENull();
                            //ligneMessage.SetI_ZONE_BATAILLENull();

                            //A la place, j'envoie un message de position
                            if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_POSITION))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (I_TOUR_FIN - I_TOUR_DEBUT >= 4 && !lignePion.estArtillerie)
                            {
                                lignePion.GainExperienceFinDeBataille();
                            }

                            if (!PertesFinDeBataille(lignePion, ligneBataillePions, 
                                (ligneBataillePions.I_ZONE_BATAILLE_ENGAGEMENT > 2) ? bVictoire345 : bVictoire012,
                                (ligneBataillePions.I_ZONE_BATAILLE_ENGAGEMENT > 2) ? lignePionsCombattifBataille012 : lignePionsCombattifBataille345))
                            {
                                message = string.Format("FinDeBataille : erreur dans FinDeBataille !QG");
                                LogFile.Notifier(message);
                                return false;
                            }
                        }
                    }
                }

                //suppression des lignes dans TAB_BATAILLE_PIONS
                //ne pas le faire car on doit le garder pour savoir si, en fin de journée, l'unité s'est bien reposée
                //DataSetCoutDonnees.TAB_BATAILLE_PIONSRow[] resBataillePions=(DataSetCoutDonnees.TAB_BATAILLE_PIONSRow[])DataSetCoutDonnees.m_donnees.TAB_BATAILLE_PIONS.Select(requete);
                //foreach (DataSetCoutDonnees.TAB_BATAILLE_PIONSRow lignePionBataille in resBataillePions) { lignePionBataille.Delete(); }
                //if (!Donnees.m_donnees.TAB_PARTIE.Nocturne(m_donnees.TAB_PARTIE.HeureCourante() + 1)  && (this.ID_LEADER_012>=0 || this.ID_LEADER_345>=0))
                if (this.ID_LEADER_012 >= 0 || this.ID_LEADER_345 >= 0)
                {
                    //c'est une fin de bataille necessitant un arrêt du tour en cours que si un leader un présent et que l'on ne va pas s'arrêter de toute manière à cause de la nuit
                    bFinDeBataille = true;
                }
                return true;
            }

            /// <summary>
            ///tous les pions dans la zone de bataille des battus mais non engagées, on un temps de retraite pour éviter de redéclencher immédiatement un combat sur d'autres unités
            ///// correction -> prendre même les non engagées sinon les engagées se retrouvent "coincées" dans la zone et ne peuvent pas sortir (sauf si c'est un dépôt qui ne bouge donc pas)
            ///il faut prendre toutes les zones dans la zone, pas seulement celles dans la bataille car les unités avec 0 de moral ne sont pas incluses et cela reprovoque une fin de
            ///bataille juste après la fin de l'autre si plusieurs unités à 0 de moral sont dans la zone.
            /// </summary>
            /// <param name="bZone012">true prendre la nation de la zone 012, 345 sinon</param>
            private void DesengagementDuVaincu(bool bZone012)
            {
                int idNation = bZone012 ? ID_NATION_012 : ID_NATION_345;

                //Donnees.TAB_CASERow[] lignesCaseBataille = Donnees.m_donnees.TAB_CASE.CasesCadre(this.I_X_CASE_HAUT_GAUCHE, this.I_Y_CASE_HAUT_GAUCHE, this.I_X_CASE_BAS_DROITE, this.I_Y_CASE_BAS_DROITE);
                int i = 0;
                int nbPions = Donnees.m_donnees.TAB_PION.Count;
                while (i < nbPions)
                {
                    Donnees.TAB_PIONRow lignePionEnBataille = Donnees.m_donnees.TAB_PION[i++];
                    if (lignePionEnBataille.B_DETRUIT || lignePionEnBataille.nation.ID_NATION!= idNation 
                        //|| lignePionEnBataille.I_TOUR_RETRAITE_RESTANT>0 || lignePionEnBataille.I_TOUR_FUITE_RESTANT>0 ->reprendre le compte à zéro, peu importe qu'elle soit déjà en retraite
                        || lignePionEnBataille.estMessager || lignePionEnBataille.estQG || lignePionEnBataille.estConvoi || lignePionEnBataille.estDepot || lignePionEnBataille.estCapturable) { continue; }

                    Donnees.TAB_CASERow ligneCasePionBataille = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePionEnBataille.ID_CASE);
                    if (ligneCasePionBataille.I_X >= I_X_CASE_HAUT_GAUCHE && ligneCasePionBataille.I_Y >= I_Y_CASE_HAUT_GAUCHE 
                        && ligneCasePionBataille.I_X <= I_X_CASE_BAS_DROITE && ligneCasePionBataille.I_Y <= I_Y_CASE_BAS_DROITE)
                    {
                        lignePionEnBataille.I_TOUR_RETRAITE_RESTANT = 2 +1;//+1 car on fait une fin de combat au tour T,phase 100, si le combat reprend à T+2,phase 0, cela ne laisse en fait qu'une heure de retraite
                    }
                }
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
                AstarTerrain[] tableCoutsMouvementsTerrain;
                List<LigneCASE> chemin;
                string message;
                int i;
                AStar etoile = new();
                List<Donnees.TAB_PIONRow> listePionsEnBataille = new();

                if (0==lignePionsEnBataille.Length) { return true; }//possible si toutes les unités adverses ont été détruites au combat
                foreach (Donnees.TAB_PIONRow l in lignePionsEnBataille) listePionsEnBataille.Add(l);
                int nation = listePionsEnBataille[0].idNation;

                //Deux cas possibles, soit l'unité est en mouvement (possible pour les unités non engagées) et on "avance" l'unité jusqu'à ce qu'elle
                //sorte du champ de bataille
                //soit elle est fixe et on la repositionne sur le bord le plus proche. Chose que l'on fait également si le mouvement
                //ne conduit pas hors du champ de bataille

                // accès aux cases du champ de bataille pour forcer le chargerCase si besoin, sinon le résultat de la requête est fausse et,
                // dans le suite du traitement, on a crash car le résultat a été modifié
                for (int x= I_X_CASE_HAUT_GAUCHE; x< I_X_CASE_BAS_DROITE; x+=10) 
                { 
                    for (int y = I_Y_CASE_HAUT_GAUCHE; y < I_Y_CASE_BAS_DROITE; y += 10)
                    {
                        Donnees.m_donnees.TAB_CASE.FindParXY(x, y);
                    }
                }

                //recherche de toutes les cases du champ de bataille
                List<TAB_CASERow> listeCasesBataille = (from Case in Donnees.m_donnees.TAB_CASE
                                         where (I_X_CASE_BAS_DROITE >= Case.I_X && I_Y_CASE_BAS_DROITE >= Case.I_Y
                                                && I_X_CASE_HAUT_GAUCHE <= Case.I_X && I_Y_CASE_HAUT_GAUCHE <= Case.I_Y)
                                         select Case).ToList();
                //si des unités sont présentes mais n'ont pas été engagées dans le combat, elles doivent quand même sortir du champ de bataille 
                //sinon des unités se retrouvent encore en plein milieu du champ occupé par l'adversaire
                foreach(TAB_CASERow ligneCaseBataille in listeCasesBataille)
                {
                    bool btrouveProprietaire = false;
                    bool btrouveNouveauProprietaire = false;
                    if (!ligneCaseBataille.IsID_PROPRIETAIRENull() || !ligneCaseBataille.IsID_NOUVEAU_PROPRIETAIRENull())
                    {
                        foreach (TAB_PIONRow lignePionBataille in listePionsEnBataille)
                        {
                            if (lignePionBataille.ID_PION == ligneCaseBataille.ID_PROPRIETAIRE) { btrouveProprietaire = true; }
                            if (lignePionBataille.ID_PION == ligneCaseBataille.ID_NOUVEAU_PROPRIETAIRE) { btrouveNouveauProprietaire = true; }
                        }
                        if (!ligneCaseBataille.IsID_PROPRIETAIRENull() && !btrouveProprietaire)
                        {
                            TAB_PIONRow lignePionBatailleAjout = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneCaseBataille.ID_PROPRIETAIRE);
                            if (!lignePionBatailleAjout.estMessager && !lignePionBatailleAjout.estPatrouille && !lignePionBatailleAjout.estDepot && nation ==lignePionBatailleAjout.nation.ID_NATION) 
                            { 
                                listePionsEnBataille.Add(lignePionBatailleAjout); 
                            }
                        }
                        if (!ligneCaseBataille.IsID_NOUVEAU_PROPRIETAIRENull() && !btrouveNouveauProprietaire)
                        {
                            TAB_PIONRow lignePionBatailleAjout = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneCaseBataille.ID_NOUVEAU_PROPRIETAIRE);
                            if (!lignePionBatailleAjout.estMessager && !lignePionBatailleAjout.estPatrouille && !lignePionBatailleAjout.estDepot && nation == lignePionBatailleAjout.nation.ID_NATION) 
                            { 
                                listePionsEnBataille.Add(lignePionBatailleAjout); 
                            }
                        }
                    }
                }
                //même chose mais avec des unités qui pourraient être masquées par d'autres
                foreach (TAB_PIONRow lignePionTerrain in Donnees.m_donnees.TAB_PION)
                {
                    //if (!lignePionTerrain.estMessager && lignePionTerrain.ID_PION>8418)
                    //{
                    //    int ttt = 0;// pour mettre un point d'arrêt
                    //}
                    if (lignePionTerrain.B_DETRUIT || nation != lignePionTerrain.nation.ID_NATION) { continue; }
                    TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePionTerrain.ID_CASE);
                    if (I_X_CASE_BAS_DROITE >= ligneCase.I_X && I_Y_CASE_BAS_DROITE >= ligneCase.I_Y
                           && I_X_CASE_HAUT_GAUCHE <= ligneCase.I_X && I_Y_CASE_HAUT_GAUCHE <= ligneCase.I_Y)
                    {
                        bool btrouvePion = false;
                        foreach (TAB_PIONRow lignePionBataille in listePionsEnBataille)
                        {
                            if (lignePionBataille.ID_PION == lignePionTerrain.ID_PION) { btrouvePion = true; }
                        }
                        if (!btrouvePion)
                        {
                            if (!lignePionTerrain.estMessager && !lignePionTerrain.estPatrouille && !lignePionTerrain.estDepot)
                            {
                                listePionsEnBataille.Add(lignePionTerrain);
                            }
                        }
                    }
                }


                for (int l = 0; l < listePionsEnBataille.Count; l++)
                {
                    Donnees.TAB_PIONRow lignePion = listePionsEnBataille[l];
                    if (lignePion.B_DETRUIT) { continue; }
                    //si l'unité n'a aucune position dans le champ de bataille, cela ne sert à rien de la bouger !
                    // BEA 18/09/2021, l'unité est bien présente en bataille, ce n'est pas parce qu'elle n'est pas présente qu'elle ne doit pas être sortie
                    //var listeCasesOccupees = from Case in listeCasesBataille
                    //                         where (Constantes.NULLENTIER != Case.ID_PROPRIETAIRE && Case.ID_PROPRIETAIRE == lignePion.ID_PION)
                    //                         || (Constantes.NULLENTIER != Case.ID_NOUVEAU_PROPRIETAIRE && Case.ID_NOUVEAU_PROPRIETAIRE == lignePion.ID_PION)
                    //                         select Case;
                    //if (0 == listeCasesOccupees.Count()) { continue; }

                    bool bRepositionSurBord = false;
                    Donnees.TAB_NATIONRow ligneNation = lignePion.nation; // Donnees.m_donnees.TAB_PION.TrouveNation(lignePion);
                    Donnees.TAB_ORDRERow ligneOrdre = Donnees.m_donnees.TAB_ORDRE.Mouvement(lignePion.ID_PION);
                    if (null != ligneOrdre)
                    {
                        //On recherche le point du parcours où l'on va sortir de la zone de combat
                        Donnees.TAB_CASERow ligneCaseDestination = Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DESTINATION);
                        Donnees.TAB_CASERow ligneCaseDepart = (lignePion.effectifTotal > 0) ? Donnees.m_donnees.TAB_CASE.FindParID_CASE(ligneOrdre.ID_CASE_DEPART) : Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePion.ID_CASE);

                        if (!etoile.RechercheChemin(Constantes.TYPEPARCOURS.MOUVEMENT, lignePion, ligneCaseDepart, ligneCaseDestination, out chemin, out double cout, out double coutHorsRoute, out tableCoutsMouvementsTerrain, out string messageErreur))
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
                                lignePion.CalculerEffectif(i, true, out iInfanterie, out iCavalerie, out iArtillerie);
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
                        Donnees.TAB_CASERow ligneCasePion = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePion.ID_CASE);
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
                    }
                }
                for (int l = 0; l < listePionsEnBataille.Count; l++)
                {
                    Donnees.TAB_PIONRow lignePion = listePionsEnBataille[l];
                    lignePion.PlacementPion(lignePion.nation, true);
                }
                return true;
            }

            private static void RechercheCaseDeSortie(bool bVertical, int minimum, int maximum, int autreCoordonnee,
                int xCaseHautGauche, int yCaseHautGauche, int xCaseBasDroite, int yCaseBasDroite,
                Donnees.TAB_PIONRow lignePion, Donnees.TAB_CASERow ligneCasePion,
                ref Donnees.TAB_CASERow ligneCaseSortie, ref int nbCasesOccupeesParEnnemis, ref int nbCasesOccupeesParAmis)
            {
                LogFile.Notifier("Début RechercheCaseDeSortie");
                Donnees.TAB_CASERow ligneCase;
                double distanceCaseSortie = double.MaxValue;

                var listeCasesOccupeesParEnnemis = from Case in Donnees.m_donnees.TAB_CASE
                                                   where (xCaseBasDroite >= Case.I_X && yCaseBasDroite >= Case.I_Y
                                                 && xCaseHautGauche <= Case.I_X && yCaseHautGauche <= Case.I_Y
                                                 && Case.EstOccupeeParEnnemi(lignePion))
                                                   select Case;

                int nblisteCasesOccupeesParEnnemis = listeCasesOccupeesParEnnemis.Count();
                //var listeT = from Case in Donnees.m_donnees.TAB_CASE
                //             where (xCaseBasDroite >= Case.I_X && yCaseBasDroite >= Case.I_Y
                //           && xCaseHautGauche <= Case.I_X && yCaseHautGauche <= Case.I_Y
                //           )
                //             select Case;
                //int nbT = listeT.Count();

                if (nblisteCasesOccupeesParEnnemis > nbCasesOccupeesParEnnemis)
                {
                    //LogFile.Notifier("RechercheCaseDeSortie : nblisteCasesOccupeesParEnnemis > nbCasesOccupeesParEnnemis");
                    return; //la solution précedente est meilleure
                }
                //nbT = nbT + 1;
                //deuxième critère, le nombre de cases occupées par les amis
                var listeCasesOccupeesParAmis = from Case in Donnees.m_donnees.TAB_CASE
                                                where (xCaseBasDroite >= Case.I_X && yCaseBasDroite >= Case.I_Y
                                            && xCaseHautGauche <= Case.I_X && yCaseHautGauche <= Case.I_Y
                                            && Case.EstOccupeeParAmi(lignePion))
                                                select Case;
                int nblisteCasesOccupeesParAmis = listeCasesOccupeesParAmis.Count();
                if (nblisteCasesOccupeesParAmis < nbCasesOccupeesParAmis)
                {
                    //LogFile.Notifier("RechercheCaseDeSortie : listeCasesOccupeesParAmis.Count() < nbCasesOccupeesParAmis");
                    return; //la solution précedente est meilleure
                }
                nbCasesOccupeesParEnnemis = nblisteCasesOccupeesParEnnemis;
                nbCasesOccupeesParAmis = nblisteCasesOccupeesParAmis;

                for (int i = minimum; i < maximum; i++)
                {
                    ligneCase = bVertical ? Donnees.m_donnees.TAB_CASE.FindParXY(autreCoordonnee, i) : Donnees.m_donnees.TAB_CASE.FindParXY(i, autreCoordonnee);
                    //LogFile.Notifier(string.Format("RechercheCaseDeSortie : case={0} : {1},{2}", ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y));
                    if (!ligneCase.EstOccupeeOuBloqueParEnnemi(lignePion, false) && ligneCase.EstMouvementPossible())
                    {
                        double distance = Constantes.Distance(ligneCase.I_X, ligneCase.I_Y, ligneCasePion.I_X, ligneCasePion.I_Y);
                        if (distance < distanceCaseSortie)
                        {
                            distanceCaseSortie = distance;
                            ligneCaseSortie = ligneCase;
                            //LogFile.Notifier(string.Format("RechercheCaseDeSortie : case sortie={0} : {1},{2}", ligneCase.ID_CASE, ligneCase.I_X, ligneCase.I_Y));
                        }
                    }
                }
                LogFile.Notifier("Fin RechercheCaseDeSortie");
            }

            private static bool ChefPresent(Donnees.TAB_PIONRow lignePion, Donnees.TAB_PIONRow[] listePions)
            {
                foreach (TAB_PIONRow ligneChef in listePions)
                {
                    if (lignePion.ID_PION_PROPRIETAIRE == ligneChef.ID_PION) { return true; }
                }
                return false;
            }

            private bool EnvoyerMessageBataillesVictoireDefaite(Donnees.TAB_PIONRow[] lignePionsVictoire, Donnees.TAB_PIONRow[] lignePionsDefaite)
            {
                string message;
                //message indiquant la victoire au vainqueur
                for (int l = 0; l < lignePionsVictoire.Length; l++)
                {
                    Donnees.TAB_PIONRow lignePion = lignePionsVictoire[l];
                    if (lignePion.B_DETRUIT) { continue; }
                    
                    if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_VICTOIRE_EN_BATAILLE))
                    {
                        message = string.Format("EnvoyerMessageBataillesVictoireDefaite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                        LogFile.Notifier(message);
                        return false;
                    }
                }
                for (int l = 0; l < lignePionsDefaite.Length; l++)
                {
                    Donnees.TAB_PIONRow lignePion = lignePionsDefaite[l];
                    if (lignePion.B_DETRUIT) { continue; }
                    if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_DEFAITE_EN_BATAILLE))
                    {
                        message = string.Format("EnvoyerMessageBataillesVictoireDefaite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                        LogFile.Notifier(message);
                        return false;
                    }
                }
                return true;
            }

            private bool GainMoralFinDeBataille(Donnees.TAB_PIONRow[] lignePionsEnBataille)
            {
                int moral;
                string message;

                //gain au moral pour les unités qui tiennent le terrain
                for (int l = 0; l < lignePionsEnBataille.Length; l++)
                {
                    Donnees.TAB_PIONRow lignePion = lignePionsEnBataille[l];
                    if (lignePion.B_DETRUIT) { continue; }
                    moral = (lignePion.I_MORAL + Constantes.CST_GAIN_MORAL_MAITRE_TERRAIN > lignePion.I_MORAL_MAX) ? lignePion.I_MORAL_MAX - lignePion.I_MORAL : Constantes.CST_GAIN_MORAL_MAITRE_TERRAIN;

                    if (moral > 0)
                    {
                        lignePion.I_MORAL = Math.Min(lignePion.I_MORAL + Constantes.CST_GAIN_MORAL_MAITRE_TERRAIN, lignePion.I_MORAL_MAX);
                        if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_GAIN_MORAL_MAITRE_TERRAIN, moral))
                        {
                            message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                            LogFile.Notifier(message);
                            return false;
                        }
                    }
                }
                return true;
            }

            private bool FinDeBatailleEgalite(Donnees.TAB_PIONRow[] lignePionsEnBataille)
            {
                string message;

                for (int l = 0; l < lignePionsEnBataille.Length; l++)
                {
                    Donnees.TAB_PIONRow lignePion = lignePionsEnBataille[l];
                    if (lignePion.B_DETRUIT) { continue; }
                    if (Donnees.m_donnees.TAB_PARTIE.Nocturne(m_donnees.TAB_PARTIE.HeureCourante()+1))
                    {
                        if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_FIN_DE_BATAILLE_A_LA_NUIT))
                        {
                            message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                            LogFile.Notifier(message);
                            return false;
                        }
                    }
                    else
                    {
                        if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_FIN_DE_BATAILLE_FAUTE_DE_COMBATTANT))
                        {
                            message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                            LogFile.Notifier(message);
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
                SortedList listePertesInfanterie = new();
                SortedList listePertesCavalerie = new();
                SortedList listePertesArtillerie = new();
                SortedList listePertesMoral = new();
                SortedList listePertesMateriel = new();
                SortedList listePertesRavitaillement = new();
                string ratio;
                int effectifPoursuivantTotal;
                int i;

                //aucune poursuite si la bataille se termine à la tombée de la nuit -> testé auparavant et le test n'est pas bon
                //if (Donnees.m_donnees.TAB_PARTIE.Nocturne((m_donnees.TAB_PARTIE.HeureCourante() + 1) % 24))
                //{
                //    message = string.Format("Poursuite : {0} aucune poursuite à cause de la nuit", S_NOM);
                //    LogFile.Notifier(message);
                //    return true;
                //}

                effectifCavaleriePoursuivant = 0;
                moralCavaleriePoursuivant = 0;
                effectifCavaleriePoursuivi = 0;
                for (int l = 0; l < lignePionsPoursuivant.Length; l++)
                {
                    Donnees.TAB_PIONRow lignePion = lignePionsPoursuivant[l];
                    if (lignePion.B_DETRUIT || lignePion.estQG) { continue; }
                    if (lignePion.Moral >= Constantes.CST_LIMITE_MORAL_CAVALERIE_POURSUITE && lignePion.cavalerie > 0)
                    {
                        //Dans la règle, toute unité de cavalierie ayant moins de CST_LIMITE_MORAL_CAVALERIE_POURSUITE (20) ne peut pas poursuivre
                        //Toute unité de cavalerie qui n'a pas été engagée comme pour sa totalité, sinon pour la moitié
                        // J'ai remplacé non engagé par moral au maximum car dans VAOC une unité peut ne pas être engagée uniquement parce qu'elle souhaite poursuivre son mouvement
                        // -> C'est un écart à indiquer dans le fichier d'aide
                        int nombreCavaliersPoursuivants = (lignePion.Moral >= lignePion.I_MORAL_MAX*0.9) ? lignePion.cavalerie : lignePion.cavalerie / 2;//BEA 16/09/2021 ajout du *0,9 sinon trop restrictif
                        effectifCavaleriePoursuivant += nombreCavaliersPoursuivants;
                        moralCavaleriePoursuivant += nombreCavaliersPoursuivants * lignePion.Moral;
                        message = string.Format("Poursuite : ID={0} avec {1} cavaliers et {2} de moral", lignePion.ID_PION, effectifCavaleriePoursuivant, lignePion.Moral);
                        LogFile.Notifier(message);
                    }
                }

                if (0 == effectifCavaleriePoursuivant)
                {
                    #region aucune cavalerie pour faire la poursuite, on en informe les chefs ayant des unités dans la bataille
                    for (int l = 0; l < lignePionsPoursuivant.Length; l++)
                    {
                        Donnees.TAB_PIONRow lignePion = lignePionsPoursuivant[l];
                        if (lignePion.B_DETRUIT) { continue; }
                        if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_AUCUNE_POURSUITE_POSSIBLE))
                        {
                            message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_POSSIBLE");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }

                    for (int l = 0; l < lignePionsPoursuivi.Length; l++)
                    {
                        Donnees.TAB_PIONRow lignePion = lignePionsPoursuivi[l];
                        if (lignePion.B_DETRUIT) { continue; }
                        if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_AUCUNE_POURSUITE_RECUE))
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
                moralCavaleriePoursuivant /= effectifCavaleriePoursuivant;
                LogFile.Notifier("Poursuite : moralCavaleriePoursuivant = " + moralCavaleriePoursuivant);

                //calcul des effectifs de cavalerie du poursuivi
                for (int l = 0; l < lignePionsPoursuivi.Length; l++)
                {
                    Donnees.TAB_PIONRow lignePion = lignePionsPoursuivi[l];
                    if (lignePion.B_DETRUIT || lignePion.estQG) { continue; }
                    effectifCavaleriePoursuivi += lignePion.cavalerie;// lignePion.I_CAVALERIE; BEA 16/09, pourquoi devrait-on prendre l"effectif total et pas seulement les présents ?
                }

                rapport = (0 == effectifCavaleriePoursuivi) ? 4 : (decimal)effectifCavaleriePoursuivant / (decimal)effectifCavaleriePoursuivi;
                modifRapport = 0;
                ratio = "< 1/2";
                if (rapport > 0.5m) { modifRapport = 1; ratio = "1/2"; }
                if (rapport > 1m) { modifRapport = 2; ratio = "1/1"; }
                if (rapport > 2m) { modifRapport = 3; ratio = "2/1"; }
                if (rapport > 3m) { modifRapport = 4; ratio = "3/1"; }
                if (rapport > 4m) { modifRapport = 5; ratio = "4/1"; }//avant 27/7/2022 cette ligne n'existait pas

                message = string.Format("Poursuite : rapport={0} modifRapport={1}", rapport, modifRapport);
                LogFile.Notifier(message);

                de = Constantes.JetDeDes(1) + modifRapport -1;
                if (de < 0) de = 0;
                if (de > 10) de = 10;//avant 27/7/2022 c'était 9

                moral = Math.Min(3,(moralCavaleriePoursuivant - 1) / 10);
                //pertes = ((Constantes.tablePoursuite[de, moral] * effectifCavaleriePoursuivant / 100) / Constantes.CST_PAS_DE_PERTES) * Constantes.CST_PAS_DE_PERTES;//on travaille par multiples de CST_PAS_DE_PERTES
                pertes = Constantes.tablePoursuite[de, moral] * effectifCavaleriePoursuivant / 100 ;//BEA 16/09/2021 pourquoi travailler par multiples de CST_PAS_DE_PERTES ?
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
                    for (int l = 0; l < lignePionsPoursuivant.Length; l++)
                    {
                        Donnees.TAB_PIONRow lignePion = lignePionsPoursuivant[l];
                        if (lignePion.B_DETRUIT) { continue; }
                        if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_POURSUITE_SANS_EFFET))
                        {
                            message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_POURSUITE_SANS_EFFET");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                    }
                    for (int l = 0; l < lignePionsPoursuivi.Length; l++)
                    {
                        Donnees.TAB_PIONRow lignePion = lignePionsPoursuivi[l];
                        if (lignePion.B_DETRUIT) { continue; }
                        if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_AUCUNE_POURSUITE_RECUE))
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
                             orderby pionsPoursuivi.I_CAVALERIE descending, pionsPoursuivi.Moral descending
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
                            int perteLocale = Math.Min(lignePionPerte.I_CAVALERIE, (lignePionPerte.Moral <= 0) ? Math.Min(2 * Constantes.CST_PAS_DE_PERTES, lignePionPerte.I_CAVALERIE) : Math.Min(Constantes.CST_PAS_DE_PERTES, lignePionPerte.I_CAVALERIE));
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
                                int perteLocale = Math.Min(lignePionPerte.I_INFANTERIE, (lignePionPerte.Moral <= 0) ? Math.Min(4 * Constantes.CST_PAS_DE_PERTES, lignePionPerte.I_INFANTERIE) : Math.Min(2 * Constantes.CST_PAS_DE_PERTES, lignePionPerte.I_INFANTERIE));
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
                                if (lignePionPerte.I_MORAL <= 0 ) { lignePionPerte.B_FUITE_AU_COMBAT = true; }

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

                for (int z = 0; z < 6; z++)
                {
                    //on met le même chiffre global partout, le générateur de vidéo saura pendre la bonne zone
                    this["S_COMBAT_" + Convert.ToString(z)] = string.Format("{0} dés, ratio = {1}", de, ratio);
                    this["I_PERTES_" + Convert.ToString(z)] = pertesCavalerieTotal + pertesInfanterieTotal + pertesArtillerieTotal;
                }

                if (pertes > 0)
                {
                    //toutes les unités restantes ont été détruites par la poursuite
                    //on informe le poursuivant
                    foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivant)
                    {
                        if (lignePion.B_DETRUIT) { continue; }
                        //note : on ne donne pas le moral perdu par l'ennemi, comment pourrait-il l'estimer ?
                        if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_POURSUITE_DESTRUCTION_TOTALE, 
                            pertesInfanterieTotal, pertesCavalerieTotal, pertesArtillerieTotal,0,0,0,null))
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
                        if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_POURSUITE_PERTES_POURSUIVANT, pertesInfanterieTotal, pertesCavalerieTotal, pertesArtillerieTotal, 0, 0, 0, null))
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
                        int pertesInfanteriePion = listePertesInfanterie.ContainsKey(lignePion.ID_PION) ? (int)listePertesInfanterie[lignePion.ID_PION] : 0;
                        int pertesCavaleriePion = listePertesCavalerie.ContainsKey(lignePion.ID_PION) ? (int)listePertesCavalerie[lignePion.ID_PION] : 0;
                        int pertesArtilleriePion = listePertesArtillerie.ContainsKey(lignePion.ID_PION) ? (int)listePertesArtillerie[lignePion.ID_PION] : 0;
                        int pertesMoralPion = listePertesMoral.ContainsKey(lignePion.ID_PION) ? (int)listePertesMoral[lignePion.ID_PION] : 0;
                        int pertesMaterielPion = listePertesMateriel.ContainsKey(lignePion.ID_PION) ? (int)listePertesMateriel[lignePion.ID_PION] : 0;
                        int pertesRavitaillementPion = listePertesRavitaillement.ContainsKey(lignePion.ID_PION) ? (int)listePertesRavitaillement[lignePion.ID_PION] : 0;
                        if (0 == pertesCavalerieTotal + pertesInfanterieTotal + pertesArtillerieTotal)
                        {
                            if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_AUCUNE_POURSUITE_RECUE))
                            {
                                message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_AUCUNE_POURSUITE_RECUE");
                                LogFile.Notifier(message, out messageErreur);
                                return false;
                            }
                        }
                        else
                        {
                            if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_POURSUITE_PERTES_POURSUIVI,
                                pertesInfanteriePion, pertesCavaleriePion, pertesArtilleriePion, pertesMoralPion, pertesMaterielPion, pertesRavitaillementPion, null))
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
                            if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_DETRUIT_PAR_POURSUITE))
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
                    effectifPoursuivantTotal = 0;
                    //somme des effectifs pour déterminer la clef de répartition
                    foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivant)
                    {
                        if (lignePion.B_DETRUIT || lignePion.effectifTotal <= 0 || lignePion.Moral <= 0) { continue; }
                        effectifPoursuivantTotal += lignePion.effectifTotal;
                    }

                    //Repartition effective
                    if (effectifPoursuivantTotal > 0)
                    {
                        foreach (Donnees.TAB_PIONRow lignePion in lignePionsPoursuivant)
                        {
                            if (lignePion.B_DETRUIT || lignePion.effectifTotal <= 0 || lignePion.Moral <= 0) { continue; }
                            int canonsButin = pertesArtillerieTotal * lignePion.effectifTotal / effectifPoursuivantTotal;
                            int materielButin = Math.Min(100 - lignePion.I_MATERIEL,
                                pertesMaterielTotal * 100 / effectifPoursuivantTotal);//il faut passer d'un nombre d'hommes a un pourcentage
                            int ravitaillementButin = Math.Min(100 - lignePion.I_RAVITAILLEMENT,
                                pertesRavitaillementTotal * 100 / effectifPoursuivantTotal);//il faut passer d'un nombre d'hommes a un pourcentage

                            lignePion.I_ARTILLERIE += canonsButin;
                            lignePion.I_MATERIEL += materielButin;
                            lignePion.I_RAVITAILLEMENT += ravitaillementButin;
                            message = string.Format("Poursuite : Butin pour {0}:{1} de {2}% en matériel (total {3}%), {4}% en ravitaillement (total {5}%) et {6} canons (total {7} canons)",
                                lignePion.ID_PION, lignePion.S_NOM, materielButin, lignePion.I_MATERIEL, ravitaillementButin, lignePion.I_RAVITAILLEMENT, canonsButin, lignePion.I_ARTILLERIE);

                            //envoi d'un message pour prévenir de la prise de butin
                            if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_POURSUITE_BUTIN, 0, 0, canonsButin, 0, ravitaillementButin, materielButin, null))
                            {
                                message = string.Format("Poursuite : erreur lors de l'envoi d'un message MESSAGE_POURSUITE_PERTES_POURSUIVANT");
                                LogFile.Notifier(message, out messageErreur);
                                return false;
                            }
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
            private static int CalculModificateurStrategique(int idLeader, List<Donnees.TAB_PIONRow> lignePionsEnBataille)
            {
                int nbUnites = 0;
                for (int l = 0; l < lignePionsEnBataille.Count; l++)
                {
                    Donnees.TAB_PIONRow lignePion = lignePionsEnBataille[l];
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
            private static int CalculModificateurStrategique(int idLeader, int nbUnites)
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
            public bool EffectuerBataille(string fichierCourant, ref bool bFinDeBataille)
            {
                string message;
                int[] des;
                int[] effectifs;
                int[] canons;
                int[] modificateurs;
                int[] relance = new int[6];                //nombre de dés autorises à être relancés en cas de superiorité d'artillerie
                int i;
                int nbUnites012, nbUnites345;//pour les bonus stratégiques
                int nbUnites012Base, nbUnites345Base;//pour vérifier qu'il y a bien des unités présentes
                int modificateurStrategique012, modificateurStrategique345;
                Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain;
                bool bUniteEnDefense;
                bool bRetraite012 = false;
                bool bRetraite345 = false;

                //Donnees.TAB_PIONRow lignePionTest = Donnees.m_donnees.TAB_PION.FindByID_PION(62);
                if (!IsI_TOUR_FINNull())
                {
                    return true;//la bataille est déjà terminée
                }
                
                message = string.Format("*************** EffectuerBataille sur {0} (ID={1}) ************************",
                    S_NOM, ID_BATAILLE);
                LogFile.Notifier(message);

                //if (Donnees.m_donnees.TAB_PARTIE.Nocturne())
                //{
                //    //pas de combat la nuit
                //    message = string.Format("EffectuerBataille sur ID_BATAILLE={0}: Fin de la bataille à cause de l'arrivée de la nuit.", ligneBataille.ID_BATAILLE);
                //    LogFile.Notifier(message, out messageErreur);
                //    return FinDeBataille(ligneBataille);
                //}

                ChoixDesLeadersDeLaBataille();

                //Normalement pas possible mais si la bataille a commencé au tour précédent, il faut arrêter tout de suite.
                //il faut qu'une bataille commencé à 2 heure+1 minute avant la nuit soit executée, d'où le -1
                if (Donnees.m_donnees.TAB_PARTIE.Nocturne(m_donnees.TAB_PARTIE.HeureCourante() % 24) &&
                    I_TOUR_DEBUT == Donnees.m_donnees.TAB_PARTIE[0].I_TOUR-1)
                {
                    //pas de combat la nuit
                    message = string.Format("EffectuerBataille sur ID_BATAILLE={0}: Fin de la bataille à cause de l'arrivée de la nuit.", ID_BATAILLE);
                    LogFile.Notifier(message);
                    return FinDeBataille(ref bFinDeBataille);
                }

                //on execute une bataille que toutes les deux heures
                if (I_TOUR_DEBUT == Donnees.m_donnees.TAB_PARTIE[0].I_TOUR ||
                    (Donnees.m_donnees.TAB_PARTIE[0].I_TOUR - I_TOUR_DEBUT) % 2 > 0)
                {
                    message = string.Format("EffectuerBataille sur {0} (ID_BATAILLE={1}): Pas de combat à ce tour-ci, au prochain tour seulement.",
                        S_NOM, ID_BATAILLE);
                    LogFile.Notifier(message);

                    return true;//on ne fait le combat que toutes les deux heures
                }

                #region Fin de bataille sur ordre de retraite
                //Un ordre de retraite a-t-il été donné à ce tour ? ou à un autre ? En fait, il ne peut y avoir qu'une retraite sur une bataille !
                string requete = string.Format("ID_BATAILLE={0} AND  I_ORDRE_TYPE={1}",
                    ID_BATAILLE, Constantes.ORDRES.RETRAITE);
                Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                Donnees.TAB_ORDRERow[] resOrdreRetraite = (Donnees.TAB_ORDRERow[])Donnees.m_donnees.TAB_ORDRE.Select(requete);
                Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                if (resOrdreRetraite.Length > 0)
                {
                    message = string.Format("EffectuerBataille sur {0} (ID_BATAILLE={1}): Un ordre de retraite a été donné sur cette bataille.",
                        S_NOM, ID_BATAILLE);
                    LogFile.Notifier(message);
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
                    //même pour un ordre de retrait il doit au moins y avoir un tour de combat
                    if (Donnees.m_donnees.TAB_PARTIE[0].I_TOUR - I_TOUR_DEBUT > 2)
                    {
                        return FinDeBataille(bRetraite012, bRetraite345, ref bFinDeBataille);
                    }
                }
                #endregion

                //on retire des zones de combats toute unité demoralisée qui serait encore présente
                //il faut laisser l'unité au round précédent pour montrer qu'elle a été démoralisée mais il faut la retirer ensuite pour qu'elle ne soit pas comptabilisée comme unité d'attaque
                if (!RecherchePionsEnBataille(out _, out _, out _, out _, out _, out _, 
                                                out Donnees.TAB_PIONRow[] tablePionsEngagesAvantCombat012, out Donnees.TAB_PIONRow[] tablePionsEngagesAvantCombat345, true/*bengagement*/, false/*bcombattif*/, false/*QG*/, false /*bArtillerie*/))
                {
                    message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille tablePionsEngagesAvantCombat");
                    LogFile.Notifier(message);
                }
                foreach (Donnees.TAB_PIONRow lignePion in tablePionsEngagesAvantCombat012)
                {
                    int iZoneBataille = lignePion.IsI_ZONE_BATAILLENull() ? -1 : lignePion.I_ZONE_BATAILLE;
                    if (!lignePion.estCombattif && iZoneBataille >= 0)
                    {
                        message = string.Format("EffectuerBataille unité non combattive en 012 retirée de la bataille, {1} ID={0} zone={2}", lignePion.ID_PION, lignePion.S_NOM, iZoneBataille);
                        LogFile.Notifier(message);
                        lignePion.SetI_ZONE_BATAILLENull();
                    }
                }
                foreach (Donnees.TAB_PIONRow lignePion in tablePionsEngagesAvantCombat345)
                {
                    int iZoneBataille = lignePion.IsI_ZONE_BATAILLENull() ? -1 : lignePion.I_ZONE_BATAILLE;
                    if (!lignePion.estCombattif && iZoneBataille >= 0)
                    {
                        message = string.Format("EffectuerBataille unité non combattive en 345 retirée de la bataille, {1} ID={0} zone={2}", lignePion.ID_PION, lignePion.S_NOM, iZoneBataille);
                        LogFile.Notifier(message);
                        lignePion.SetI_ZONE_BATAILLENull();
                    }
                }


                // on vérifie qu'il y a bien une unité combattive engagagée de chaque coté, sinon, 
                /// on en choisit une au hasard qui se met en défense au centre
                Donnees.TAB_PIONRow[] tablePionsEngagesCombattifs012;
                Donnees.TAB_PIONRow[] tablePionsEngagesCombattifs345;
                if (!RecherchePionsEnBataille(out nbUnites012Base, out nbUnites345Base, out des, out modificateurs, out effectifs, out canons, out tablePionsEngagesCombattifs012, out tablePionsEngagesCombattifs345, true/*bengagement*/, true/*bcombattif*/, false/*QG*/, false /*bArtillerie*/))
                {
                    message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille 0");
                    LogFile.Notifier(message);
                }

                Donnees.TAB_PIONRow lignePionEngage012 = null;
                Donnees.TAB_PIONRow lignePionEngage345 = null;
                if (0 == nbUnites012Base)
                {
                    Donnees.TAB_PIONRow[] tablePionsCombattifsNonEngagesSansQG012;
                    Donnees.TAB_PIONRow[] tablePionsCombattifsNonEngagesSansQG345;
                    if (!RecherchePionsEnBataille(out nbUnites012, out nbUnites345, out des, out modificateurs, out effectifs, out canons, out tablePionsCombattifsNonEngagesSansQG012, out tablePionsCombattifsNonEngagesSansQG345, false/*bengagement*/, true/*bcombattif*/, false/*QG*/, false /*bArtillerie*/))
                    {
                        message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille 012");
                        LogFile.Notifier(message);
                    }
                    //il faut au moins un round de combat, même si toutes les unités sont démoralisées
                    if (0 == nbUnites012 && Donnees.m_donnees.TAB_PARTIE[0].I_TOUR- I_TOUR_DEBUT>2)
                    {
                        message = string.Format("EffectuerBataille : il n'y a aucune unité combattive dans le secteur 012 pour la bataille ID_BATAILLE={0}", ID_BATAILLE);
                        LogFile.Notifier(message);
                        return FinDeBataille(ref bFinDeBataille);//cas d'un ordre de retraite global donné par un joueur
                    }
                    if (!RecherchePionsEnBataille(out _, out _, out _, out _, out _, out _, 
                                                out Donnees.TAB_PIONRow[] tablePionsNonEngagesSansQG012, out _, false/*bengagement*/, false/*bcombattif*/, false/*QG*/, false /*bArtillerie*/))
                    {
                        message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille 012-II");
                        LogFile.Notifier(message);
                    }

                    lignePionEngage012 = AffecterUneUniteAuHasardEnBataille(tablePionsCombattifsNonEngagesSansQG012, tablePionsNonEngagesSansQG012, 1);
                    if (null == lignePionEngage012)
                    {
                        message = string.Format("EffectuerBataille : erreur dans AffecterUneUniteAuHasardEnBataille 012");
                        LogFile.Notifier(message);
                    }
                }

                if (0 == nbUnites345Base)
                {
                    Donnees.TAB_PIONRow[] tablePionsCombattifsNonEngagesSansQG012;
                    Donnees.TAB_PIONRow[] tablePionsCombattifsNonEngagesSansQG345;
                    if (!RecherchePionsEnBataille(out nbUnites012, out nbUnites345, out des, out modificateurs, out effectifs, out canons, out tablePionsCombattifsNonEngagesSansQG012, out tablePionsCombattifsNonEngagesSansQG345, false/*bengagement*/, true/*bcombattif*/, false/*QG*/, false /*bArtillerie*/))
                    {
                        message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille 345");
                        LogFile.Notifier(message);
                    }
                    //il faut au moins un round de combat, même si toutes les unités sont démoralisées
                    if (0 == nbUnites345 && Donnees.m_donnees.TAB_PARTIE[0].I_TOUR - I_TOUR_DEBUT > 2)
                    {
                        message = string.Format("EffectuerBataille : il n'y a aucune unité combattive dans le secteur 345 pour la bataille ID_BATAILLE={0}", ID_BATAILLE);
                        LogFile.Notifier(message);
                        return FinDeBataille(ref bFinDeBataille);//cas d'un ordre de retraite global donné par un joueur
                    }
                    if (!RecherchePionsEnBataille(out _, out _, out _, out _, out _, out _, 
                                                    out _, out Donnees.TAB_PIONRow[] tablePionsNonEngagesSansQG345, false/*bengagement*/, false/*bcombattif*/, false/*QG*/, false /*bArtillerie*/))
                    {
                        message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille 345-II");
                        LogFile.Notifier(message);
                    }
                    lignePionEngage345 = AffecterUneUniteAuHasardEnBataille(tablePionsCombattifsNonEngagesSansQG345, tablePionsNonEngagesSansQG345, 4);
                    if (null == lignePionEngage345)
                    {
                        message = string.Format("EffectuerBataille : erreur dans AffecterUneUniteAuHasardEnBataille 345");
                        LogFile.Notifier(message);
                    }
                }

                //false sur bcombattif avant, mais dans ce cas, donne une fausse valeur du modificateur stratégique, remis a false car sinon on ne joue plus la blessure des chefs,
                //remis à true sinon, les unités en déroute sont comptabilisées,, remis à false car s'il n'y a que des unités démoralisées, il faut bien faire le combat ! par contre, retrait automatique
                //des unités démoralisées des zones avant le combat pour ne pas avoir d'unités démoralisée restante d'un combat précédent
                if (!RecherchePionsEnBataille(out nbUnites012, out nbUnites345, out des, out modificateurs, out effectifs, out canons, 
                                                out TAB_PIONRow[] tablePionsEngages012, out TAB_PIONRow[] tablePionsEngages345, true/*bengagement*/, false/*bcombattif*/, true/*QG*/, true /*bArtillerie*/))
                {
                    message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille I");
                    LogFile.Notifier(message);
                }

                List<Donnees.TAB_PIONRow> listePionsEngages012= tablePionsEngages012.ToList<Donnees.TAB_PIONRow>();
                List<Donnees.TAB_PIONRow> listePionsEngages345 = tablePionsEngages345.ToList<Donnees.TAB_PIONRow>();
                //if (null != lignePionEngage012) { listePionsEngages012.Add(lignePionEngage012); } -> inutile maintenant, sinon on ajoute deux fois une unités engagées de force
                //if (null != lignePionEngage345) { listePionsEngages345.Add(lignePionEngage345); }

                message = string.Format("EffectuerBataille sur {2} nombre d'unités en début de combat nbUnites012={0} nbUnites345={1}",
                    nbUnites012, nbUnites345, S_NOM);
                LogFile.Notifier(message);
                //combattre, ça fatigue !
                foreach (Donnees.TAB_PIONRow lignePion in listePionsEngages012)
                {
                    int iZoneBataille = lignePion.IsI_ZONE_BATAILLENull() ? -1 : lignePion.I_ZONE_BATAILLE;
                    //if (lignePion.estCombattif && iZoneBataille>=0)
                    if (iZoneBataille >= 0)//même les unités démoralisées engagées doivent être comptées sinon elles vont pouvoir se reposer alors qu'elles ont combattues
                    {
                        message = string.Format("EffectuerBataille unité en 012, {1} ID={0} zone={2}", lignePion.ID_PION, lignePion.S_NOM, iZoneBataille);
                        LogFile.Notifier(message);
                        lignePion.I_NB_HEURES_COMBAT++;
                    }
                }
                foreach (Donnees.TAB_PIONRow lignePion in listePionsEngages345)
                {
                    int iZoneBataille = lignePion.IsI_ZONE_BATAILLENull() ? -1 : lignePion.I_ZONE_BATAILLE;
                    //if (lignePion.estCombattif && iZoneBataille >= 0)
                    if (iZoneBataille >= 0)//même les unités démoralisées engagées doivent être comptées sinon elles vont pouvoir se reposer alors qu'elles ont combattues
                    {
                        message = string.Format("EffectuerBataille unité en 345, {1} ID={0} zone={2}", lignePion.ID_PION, lignePion.S_NOM, iZoneBataille);
                        LogFile.Notifier(message);
                        lignePion.I_NB_HEURES_COMBAT++;
                    }
                }

                //calcul du modificateur strategique
                modificateurStrategique012 = CalculModificateurStrategique(ID_LEADER_012, listePionsEngages012);
                modificateurStrategique345 = CalculModificateurStrategique(ID_LEADER_345, listePionsEngages345);
                message = string.Format("EffectuerBataille modificateurStrategique012={0} modificateurStrategique345={1}", modificateurStrategique012, modificateurStrategique345);
                LogFile.Notifier(message);

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
                        LogFile.Notifier(message);
                        message = string.Format("EffectuerBataille attaque de flanc, avant : effectifs[{0}]={1} effectifs[{2}]={3}", i, effectifs[i], i + 3, effectifs[i + 3]);
                        LogFile.Notifier(message);
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
                        LogFile.Notifier(message);
                    }
                    else
                    {
                        message = string.Format("EffectuerBataille avant calcul des[{0}]={1} des[{2}]={3}", i, des[i], i + 3, des[i + 3]);
                        LogFile.Notifier(message);
                        //valeur stratégique du chef
                        des[i] += modificateurStrategique012;
                        des[i + 3] += modificateurStrategique345;
                        message = string.Format("EffectuerBataille avec valeur stratégique des[{0}]={1} des[{2}]={3}", i, des[i], i + 3, des[i + 3]);
                        LogFile.Notifier(message);

                        //rapport de force, +2 pour 2/1, +3 pour 3/1 avec un maximum de +6, +2 sur une zone de combat unique (= forteresse)
                        ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN((int)this["ID_TERRAIN_" + Convert.ToString(i)]);
                        int rapportmaximum = (ligneModeleTerrain.B_BATAILLE_ZONE_UNIQUE) ? 2 : 6;
                        int rapport = effectifs[i] / effectifs[i + 3];
                        if (rapport >= 2) { des[i] += Math.Min(rapport, rapportmaximum); }
                        rapport = effectifs[i + 3] / effectifs[i];
                        if (rapport >= 2) { des[i + 3] += Math.Min(rapport, rapportmaximum); }
                        message = string.Format("EffectuerBataille avec rapport de forces des[{0}]={1} effectif={2} des[{3}]={4} effectif={5}",
                            i, des[i], effectifs[i], i + 3, des[i + 3], effectifs[i + 3]);
                        LogFile.Notifier(message);

                        int rapportArtillerie = (0 == canons[i + 3]) ? canons[i] > 0 ? 2 : 0 : canons[i] / canons[i + 3];
                        if (rapportArtillerie >= 1) { relance[i] = Math.Min(rapportArtillerie, 2); } // 2 dés de relance au maximum
                        rapportArtillerie = (0 == canons[i]) ? canons[i + 3] > 0 ? 2 : 0 : canons[i + 3] / canons[i];
                        if (rapportArtillerie >= 1) { relance[i + 3] = Math.Min(rapportArtillerie, 2); }// 2 dés de relance au maximum
                        message = string.Format("EffectuerBataille relance[{0}]={1} relance[{2}]={3}", i, relance[i], i + 3, relance[i + 3]);
                        LogFile.Notifier(message);

                        //modificateurs de terrain, appliquées uniquement si l'une des unités de la zone est en mode défense et si son niveau d'engagement est inférieur ou égal à l'attaquant
                        int valeurDesFortifications = 0;
                        bUniteEnDefense = false;
                        foreach (Donnees.TAB_PIONRow lignePion in listePionsEngages012)
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
                            message = string.Format("EffectuerBataille fortification zone:{0}, terrain defense={1}, fortification defense={4}, des[{2}]={3}", 
                                i, relance[i], i + 3, des[i + 3], valeurDesFortifications);
                            LogFile.Notifier(message);
                        }

                        valeurDesFortifications = 0;
                        bUniteEnDefense = false;
                        foreach (Donnees.TAB_PIONRow lignePion in listePionsEngages345)
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
                        LogFile.Notifier(message);

                        //on a toujours au minimum, le nombre de dés d'egagement
                        des[i] = Math.Max(des[i], desEngagement);
                        des[i + 3] = Math.Max(des[i + 3], desEngagement);
                    }
                }

                message = string.Format("EffectuerBataille des={0} {1} {2} {3} {4} {5}", des[0], des[1], des[2], des[3], des[4], des[5]);
                LogFile.Notifier(message);
                message = string.Format("EffectuerBataille modificateurs={0} {1} {2} {3} {4} {5}", modificateurs[0], modificateurs[1], modificateurs[2], modificateurs[3], modificateurs[4], modificateurs[5]);
                LogFile.Notifier(message);

                #endregion

                if (!CalculDesPertesAuCombat(des, modificateurs, relance, listePionsEngages012, listePionsEngages345, bRetraite012, bRetraite345))
                {
                    message = string.Format("EffectuerBataille : erreur dans CalculDesPertesAuCombat II");
                    LogFile.Notifier(message);
                    return false;
                }

                //Y-a-t-il encore des combattants dans chaque camp ?
                Donnees.TAB_PIONRow[] tablePionsCombattifs012;
                Donnees.TAB_PIONRow[] tablePionsCombattifs345;
                if (!RecherchePionsEnBataille(out nbUnites012, out nbUnites345, out des, out modificateurs, out effectifs, out canons, out tablePionsCombattifs012, out tablePionsCombattifs345, 
                                                null/*bengagement*/, true/*bcombattif*/, false/*QG*/, false /*bArtillerie*/))
                {
                    message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille II");
                    LogFile.Notifier(message);
                    return false;
                }

                message = string.Format("EffectuerBataille nombre d'unités en fin de combat nbUnites012={0} nbUnites345={1}", nbUnites012, nbUnites345);
                LogFile.Notifier(message);
                foreach (Donnees.TAB_PIONRow lignePion in tablePionsCombattifs012)
                {
                    message = string.Format("EffectuerBataille unités en 012, {1} ID={0}", lignePion.ID_PION, lignePion.S_NOM);
                    LogFile.Notifier(message);
                }
                foreach (Donnees.TAB_PIONRow lignePion in tablePionsCombattifs345)
                {
                    message = string.Format("EffectuerBataille unités en 345, {1} ID={0}", lignePion.ID_PION, lignePion.S_NOM);
                    LogFile.Notifier(message);
                }

                //Envoie d'un message prévenant d'un bruit de canon pour toutes les unités non présentes.
                AlerteBruitDuCanon();

                //Ajout des logs pour le compte-rendu video
                AjouterDonneesVideo(Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_BATAILLE_VIDEO, Donnees.m_donnees.TAB_BATAILLE_PIONS_VIDEO);

                //on regarde si la bataille ne se termine pas
                if (0 == nbUnites012 || 0 == nbUnites345)
                {
                    return FinDeBataille(ref bFinDeBataille);
                }
                else
                {
                    //La bataille continue seulement s'il ne fait pas nuit
                    if (Donnees.m_donnees.TAB_PARTIE.Nocturne(m_donnees.TAB_PARTIE.HeureCourante() + 1))
                    {
                        //pas de combat la nuit
                        message = string.Format("EffectuerBataille sur {0} (ID_BATAILLE={1}): Fin de la bataille à cause de l'arrivée de la nuit.",
                            S_NOM, ID_BATAILLE);
                        LogFile.Notifier(message);
                        return FinDeBataille(ref bFinDeBataille);
                    }

                    //si un leader ou une unite d'artillerie se retrouve dans une zone sans unité combattante, il doit être remis en réserve
                    RemiseLeaderEnReserve();
                }

                //Si un ordre de retraite a été donné et que c'était au premier jour, il est effectué en fin de combat
                if (bRetraite012 || bRetraite345)
                {
                    return FinDeBataille(bRetraite012, bRetraite345, ref bFinDeBataille);
                }

                return true;
            }

            private void ChoixDesLeadersDeLaBataille()
            {
                string message;
                int idLeader;
                if (!RecherchePionsEnBataille(out _, out _, out _, out _, out _, out _, out Donnees.TAB_PIONRow[] tablePionsPresents012, out Donnees.TAB_PIONRow[] tablePionsPresents345, null/*bengagement*/, false/*bcombattif*/, true/*QG*/, true /*bArtillerie*/))
                {
                    message = string.Format("EffectuerBataille : erreur dans RecherchePionsEnBataille QG");
                    LogFile.Notifier(message);
                }

                //trouver le leader avec le plus haut niveau hierarchique
                idLeader = TrouverLeaderBataille(tablePionsPresents012, true);
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

                idLeader = TrouverLeaderBataille(tablePionsPresents345, false);
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
            }

            public static int EffectifTotalSurZone(int zone, List<Donnees.TAB_PIONRow> lignePionsEnBataille, bool bCombatif)
            {
                int effectifTotal = 0;
                for (int l = 0; l < lignePionsEnBataille.Count; l++)
                {
                    Donnees.TAB_PIONRow lignePionBataille = lignePionsEnBataille[l];
                    if ((!bCombatif || lignePionBataille.estCombattif) && lignePionBataille.I_ZONE_BATAILLE == zone)
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
            private bool CalculDesPertesAuCombat( int[] des, int[] modificateurs, int[] relance, List<Donnees.TAB_PIONRow> lignePionsEnBataille012, List<Donnees.TAB_PIONRow> lignePionsEnBataille345, bool bRetraite012, bool bRetraite345)
            {
                string message;
                int[] score = new int[6];
                int[] pertesMoral = new int[6];
                int[] pertesEffectifs = new int[6];
                bool[] blessureChef = new bool[6];
                int nb6;//nombre de '6' lancés sur un secteur

                #region on lance les des et on calcule les pertes au moral et en effectifs infligés par chaque camp
                for (int i = 0; i < 6; i++)
                {
                    nb6 = 0;
                    score[i] = (des[i] > 0) ? Constantes.JetDeDes(des[i], modificateurs[i], relance[i], out nb6) : 0;
                    if (i < 3) { blessureChef[i+3] = (nb6 >= 4); } else { blessureChef[i-3] = (nb6 >= 4); }
                    this["S_COMBAT_" + Convert.ToString(i)]=string.Format("{0} dés + {1}, {2} relances = {3}", des[i], modificateurs[i], relance[i], score[i]);
                }

                for (int i = 0; i < 3; i++)
                {
                    pertesMoral[i] = score[i + 3] / 2;
                    //les pertes en effectifs sont égales au quart du score * effectifs engagés / 100
                    int effectifsSurZone = EffectifTotalSurZone(i + 3, lignePionsEnBataille345, false /*bCombatif*/);
                    pertesEffectifs[i] = effectifsSurZone  * score[i + 3] / 400;
                    message = string.Format("EffectuerBataille zone: {0} score={1} effectifs={2} pertes={3}",
                        i, score[i + 3], effectifsSurZone, pertesEffectifs[i]);
                    LogFile.Notifier(message);
                }
                for (int i = 3; i < 6; i++)
                {
                    pertesMoral[i] = score[i - 3] / 2;
                    //les pertes en effectifs sont égales au quart du score * effectifs engagés / 100
                    int effectifsSurZone = EffectifTotalSurZone(i - 3, lignePionsEnBataille012, false /*bCombatif*/);
                    pertesEffectifs[i] = effectifsSurZone * score[i - 3] / 400;

                    message = string.Format("EffectuerBataille zone: {0} score={1} effectifs={2} pertes={3}",
                        i, score[i - 3], effectifsSurZone, pertesEffectifs[i]);
                    LogFile.Notifier(message);
                }
                #endregion

                message = string.Format("EffectuerBataille pertesMoral: {0} {1} {2} {3} {4} {5}",
                    pertesMoral[0], pertesMoral[1], pertesMoral[2], pertesMoral[3], pertesMoral[4], pertesMoral[5]);
                LogFile.Notifier(message);
                message = string.Format("EffectuerBataille pertesEffectifs: {0} {1} {2} {3} {4} {5}",
                    pertesEffectifs[0], pertesEffectifs[1], pertesEffectifs[2], pertesEffectifs[3], pertesEffectifs[4], pertesEffectifs[5]);
                LogFile.Notifier(message);

                #region on vérifie que chaque camp peut bien prendre toutes les pertes en moral et effectif, si ce n'est pas le cas, les pertes sont nuls pour l'autre camp
                for (int i = 0; i < 3; i++)
                {
                    bool bCamp012KO = true;
                    bool bCamp345KO = true;

                    if (des[i] > 0 && des[i + 3] > 0)
                    {
                        //int effectifTotal = 0; -> Je ne vois pas comment une troupe ne pourrait plus avoir de soldats sans avoir, auparavant, perdu tout son moral -> si juste par des pertes si elle a un moral élevé et des effectifs faibles
                        // => il faut donc aussi prendre en compte le moral
                        //mais si un camp ne peut pas supporter toutes les pertes, il est quand même KO !
                        if (pertesEffectifs[i] < EffectifTotalSurZone(i, lignePionsEnBataille012, false /*bCombatif*/))
                        {
                            foreach (Donnees.TAB_PIONRow lignePionEnBataille in lignePionsEnBataille012)
                            {
                                //if (lignePionEnBataille.B_DETRUIT || lignePionEnBataille.I_TOUR_FUITE_RESTANT > 0 || lignePionEnBataille.I_TOUR_RETRAITE_RESTANT > 0) { continue; }
                                if (!lignePionEnBataille.estCombattif) { continue; }
                                if (lignePionEnBataille.IsI_ZONE_BATAILLENull() || lignePionEnBataille.I_ZONE_BATAILLE != i) { continue; }
                                //effectifTotal += lignePionEnBataille.effectifTotal;
                                if (!lignePionEnBataille.estArtillerie && lignePionEnBataille.I_MORAL > pertesMoral[i]) 
                                {
                                    message = string.Format("EffectuerBataille le premier camp n'est pas KO grace à l'unité {0}:{1} moral {2}>{3}",
                                        lignePionEnBataille.ID_PION, lignePionEnBataille.S_NOM, lignePionEnBataille.I_MORAL, pertesMoral[i]);
                                    LogFile.Notifier(message);
                                    bCamp012KO = false; 
                                }
                            }
                        }

                        if (pertesEffectifs[i + 3] < EffectifTotalSurZone(i + 3, lignePionsEnBataille345, false /*bCombatif*/))
                        {
                            foreach (Donnees.TAB_PIONRow lignePionEnBataille in lignePionsEnBataille345)
                            {
                                if (!lignePionEnBataille.estCombattif) { continue; }
                                if (lignePionEnBataille.IsI_ZONE_BATAILLENull() || lignePionEnBataille.I_ZONE_BATAILLE != i + 3) { continue; }
                                if (!lignePionEnBataille.estArtillerie && lignePionEnBataille.I_MORAL > pertesMoral[i + 3]) 
                                {
                                    message = string.Format("EffectuerBataille le deuxième camp n'est pas KO grace à l'unité {0}:{1} moral {2}>{3}",
                                        lignePionEnBataille.ID_PION, lignePionEnBataille.S_NOM, lignePionEnBataille.I_MORAL, pertesMoral[i+3]);
                                    LogFile.Notifier(message);
                                    bCamp345KO = false; 
                                }
                            }
                        }

                        //un des bords va fuir complètement et pas l'autre ?
                        if (!bCamp012KO || !bCamp345KO)
                        {
                            if (bCamp012KO || bRetraite012)
                            {
                                pertesMoral[i + 3] = 0;
                                pertesEffectifs[i + 3] = 0;
                                LogFile.Notifier("i=" + i + " le premier Camp est mis en déroute ou a donner l'ordre de retraite et n'inflige aucune perte à l'adversaire");
                            }
                            if (bCamp345KO || bRetraite345)
                            {
                                pertesMoral[i] = 0;
                                pertesEffectifs[i] = 0;
                                LogFile.Notifier("i=" + (i + 3) + " le deuxième Camp est mis en déroute ou a donner l'ordre de retraite et n'inflige aucune perte à l'adversaire");
                            }
                        }
                    }
                }
                #endregion

                #region Un des chefs présents est-il blessé ?
                for (int i  = 0; i < 3; i++)
                {
                    //Un des chefs présents est-il blessé ?
                    if (pertesMoral[i] > 0 && blessureChef[i])
                    {
                        if (!BlessureChef(i))
                        {
                            LogFile.Notifier("i=" + i + " erreur dans BlessureChef");
                        }
                    }
                    if (pertesMoral[i + 3] > 0 && blessureChef[i + 3])
                    {
                        if (!BlessureChef(i + 3))
                        {
                            LogFile.Notifier("i=" + i + " erreur dans BlessureChef");
                        }
                    }
                }
                #endregion

                int pertesReelles;
                for (int i = 0; i < 3; i++)
                {
                    if (des[i + 3] > 0)
                    {
                        message = string.Format("EffectuerBataille score sur zone{0}={1} sur un lancé de {2} dés, pertes moral={3} effectifs={4}", i, score[i], des[i], pertesMoral[i], pertesEffectifs[i]);
                        LogFile.Notifier(message);

                        if (!CalculDesPertesAuCombatParSecteur(i + 3, lignePionsEnBataille345, i, lignePionsEnBataille012, pertesMoral[i], pertesEffectifs[i], out pertesReelles))
                        {
                            message = string.Format("CalculDesPertesAuCombat erreur dans CalculDesPertesAuCombatParSecteur zone{0}", i);
                            LogFile.Notifier(message);
                            return false;
                        }
                        pertesEffectifs[i] = pertesReelles;
                    }

                    if (des[i] > 0)
                    {
                        message = string.Format("EffectuerBataille score sur zone{0}={1} sur un lancé de {2} dés, pertes moral={3} effectifs={4}", i, score[i + 3], des[i + 3], pertesMoral[i + 3], pertesEffectifs[i + 3]);
                        LogFile.Notifier(message);

                        if (!CalculDesPertesAuCombatParSecteur(i, lignePionsEnBataille012, i + 3, lignePionsEnBataille345, pertesMoral[i + 3], pertesEffectifs[i + 3], out pertesReelles))
                        {
                            message = string.Format("CalculDesPertesAuCombat erreur dans CalculDesPertesAuCombatParSecteur zone{0}", i);
                            LogFile.Notifier(message);
                            return false;
                        }
                        pertesEffectifs[i+3] = pertesReelles;
                    }
                }
                for (int p = 0; p < 6; p++)
                {
                    //pour les rapports video de batailles
                    this["I_PERTES_" + Convert.ToString(p)] = pertesEffectifs[p];
                }
                return true;
            }

            /*
            /// <summary>
            /// Vérifie s'il n'y a pas un effet boule de neige parce que des unités démoralisées démoralisent d'autres unités
            /// </summary>
            /// <param name="pertesMoral">perte en moral sur la zone</param>
            /// <param name="nombreUnitesCassant">nombre d'unités cassant au moral et modifiant le calcul initial</param>
            /// <param name="lignePionsEnBataille">liste des pions sur la zone</param>
            private void DemoralisationSurZone(int pertesMoral, ref int nombreUnitesCassant, List<TAB_PIONRow> lignePionsEnBataille)
            {
                foreach(TAB_PIONRow lignePion in lignePionsEnBataille)
                {
                    if (lignePion.estCombattif)
                    {
                        if (lignePion.I_MORAL <= pertesMoral + nombreUnitesCassant * Constantes.CST_PERTE_MORAL_FUITE
                            && lignePion.I_MORAL > pertesMoral + (nombreUnitesCassant+1) * Constantes.CST_PERTE_MORAL_FUITE)
                        {
                            //l'unité casse maintenant mais ne cassait pas avant
                            nombreUnitesCassant++;
                            DemoralisationSurZone(pertesMoral, ref nombreUnitesCassant, lignePionsEnBataille);
                        }
                    }
                }
            }
            */

            /// <summary>
            /// Un chef d'un secteur est blesse dans une bataille
            /// </summary>
            /// <param name="zone">secteur dans lequel un chef doit être blessé (s'il y en a un)</param>
            /// <returns>true si OK, false si KO</returns>
            private bool BlessureChef(int zone)
            {
                //return true;//le mode de calcul ne convient, grosse bataille, certains d'être blessé, petite bataille aucune chance...
                Monitor.Enter(Donnees.m_donnees.TAB_BATAILLE_PIONS.Rows.SyncRoot);
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                //Recherche de tous les chefs présents dans le secteur
                var resultComplet = from BataillePion in Donnees.m_donnees.TAB_BATAILLE_PIONS
                                    from Pion in Donnees.m_donnees.TAB_PION
                                    where (BataillePion.ID_PION == Pion.ID_PION)
                                    && (BataillePion.B_ENGAGEE == true)
                                    && (BataillePion.ID_BATAILLE == ID_BATAILLE)
                                    && (BataillePion.I_ZONE_BATAILLE_ENGAGEMENT == zone)
                                    && !Pion.B_DETRUIT
                                    select new rechercheQG { idPion = Pion.ID_PION, bEngagee = BataillePion.B_ENGAGEMENT, izoneEngagement = Pion.IsI_ZONE_BATAILLENull() ? -1 : Pion.I_ZONE_BATAILLE };

                IEnumerable<int> resultPionsBataille;
                resultPionsBataille = from resultatPion in resultComplet
                                      select resultatPion.idPion;

                List<Donnees.TAB_PIONRow> listeChefsEnBataille = new();
                foreach (int id in resultPionsBataille)
                {
                    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(id);
                    if (lignePion.estQG)
                    {
                        listeChefsEnBataille.Add(lignePion);
                    }
                }

                if (listeChefsEnBataille.Count > 0)
                {
                    Random de = new();
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
                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                Monitor.Exit(Donnees.m_donnees.TAB_BATAILLE_PIONS.Rows.SyncRoot);
                return true;
            }

            private bool CalculDesPertesAuCombatParSecteur(int zoneAttaquant, List<Donnees.TAB_PIONRow> lignePionsEnBatailleAttaquant, int zoneDefenseur, List<Donnees.TAB_PIONRow> lignePionsEnBatailleDefenseur, int pertesMoral, int pertesEffectif, out int pertesEffectifReels)
            {
                string message, messageErreur;
                int pertesInfanterieTotal, pertesCavalerieTotal, pertesArtillerieTotal;
                int pertesInfanterie, pertesCavalerie, pertesArtillerie;
                List<Donnees.TAB_PIONRow> listePionsEnBatailleAttaquant = new();
                List<Donnees.TAB_PIONRow> listePionsEnBatailleDefenseur = new();

                pertesInfanterieTotal = pertesCavalerieTotal = pertesArtillerieTotal = 0;
                pertesEffectifReels = 0;

                //on reprend les unités qui ont vraiment participées au combat
                foreach (Donnees.TAB_PIONRow lignePionEnBataille in lignePionsEnBatailleDefenseur)
                {
                    if (lignePionEnBataille.estQG) { continue; }
                    if (lignePionEnBataille.IsI_ZONE_BATAILLENull() || lignePionEnBataille.I_ZONE_BATAILLE != zoneDefenseur) { continue; }
                    //if (lignePionEnBataille.I_MORAL <= 0) { continue; } //l'unité n'est déjà plus vraiment sur la zone... -> possible si toutes les unités présentes sont démoralisées

                    listePionsEnBatailleDefenseur.Add(lignePionEnBataille);
                }

                foreach (Donnees.TAB_PIONRow lignePionEnBataille in lignePionsEnBatailleAttaquant)
                {
                    if (lignePionEnBataille.B_DETRUIT || lignePionEnBataille.I_TOUR_FUITE_RESTANT > 0 || lignePionEnBataille.I_TOUR_RETRAITE_RESTANT > 0) { continue; }
                    if (lignePionEnBataille.IsI_ZONE_BATAILLENull() || lignePionEnBataille.I_ZONE_BATAILLE != zoneAttaquant) { continue; }

                    listePionsEnBatailleAttaquant.Add(lignePionEnBataille);
                }

                //pertes infligés à l'ennemi, en % du global
                int effectifTotalDefenseur = EffectifTotalSurZone(zoneDefenseur, lignePionsEnBatailleDefenseur, false /*bCombatif*/);
                foreach (Donnees.TAB_PIONRow lignePionEnBataille in listePionsEnBatailleDefenseur)
                {
                    lignePionEnBataille.I_MORAL -= pertesMoral;
                    //effectifTotalDefenseur peut valoir 0 si la fatigue est à 100%
                    decimal rapporDePerteUnite = (0 == effectifTotalDefenseur) ? 1 : (decimal)lignePionEnBataille.effectifTotal / effectifTotalDefenseur;
                    //en-dessous d'une certaine taille, une unité est détruite pour ne pas avoir à gérer de trop petites unités, sauf si ce n'est que de l'artillerie
                    if ((pertesEffectif * rapporDePerteUnite)>=1 &&
                        (
                        ((lignePionEnBataille.infanterie>0 || lignePionEnBataille.I_CAVALERIE>0) &&
                        (lignePionEnBataille.effectifTotal - (pertesEffectif * rapporDePerteUnite) < Constantes.CST_TAILLE_MINIMUM_UNITE))
                        ||
                        ((lignePionEnBataille.infanterie == 0 && lignePionEnBataille.I_CAVALERIE == 0 && lignePionEnBataille.I_ARTILLERIE>0) &&
                        (lignePionEnBataille.effectifTotal - (pertesEffectif * rapporDePerteUnite) < 1))
                        ))
                    {
                        //Message indiquant une unité détruite
                        pertesInfanterieTotal += lignePionEnBataille.infanterie;
                        pertesCavalerieTotal += lignePionEnBataille.cavalerie;
                        pertesArtillerieTotal += lignePionEnBataille.artillerie;

                        //on detruit le pion et on envoie un message
                        lignePionEnBataille.DetruirePion();
                        if (!EnvoyerMessageBataille(lignePionEnBataille, ClassMessager.MESSAGES.MESSAGE_DETRUITE_AU_COMBAT))
                        {
                            message = string.Format("CalculDesPertesAuCombat : erreur lors de l'envoi d'un message MESSAGE_DETRUITE_AU_COMBAT");
                            LogFile.Notifier(message, out messageErreur);
                            return false;
                        }
                        continue;
                    }

                    //Pertes sur l'unité, repartie en pourcentage relatif du total
                    pertesInfanterie = (0 == lignePionEnBataille.effectifTotal) ? 0 : (int)(lignePionEnBataille.infanterie * pertesEffectif * rapporDePerteUnite / lignePionEnBataille.effectifTotal);
                    pertesCavalerie = (0 == lignePionEnBataille.effectifTotal) ? 0 : (int)(lignePionEnBataille.cavalerie * pertesEffectif * rapporDePerteUnite / lignePionEnBataille.effectifTotal);
                    pertesArtillerie = (0 == lignePionEnBataille.effectifTotal) ? 0 : (int)(lignePionEnBataille.artillerie * pertesEffectif * rapporDePerteUnite / lignePionEnBataille.effectifTotal);
                    pertesInfanterieTotal += pertesInfanterie;
                    pertesCavalerieTotal += pertesCavalerie;
                    pertesArtillerieTotal += pertesArtillerie;
                    lignePionEnBataille.I_INFANTERIE -= pertesInfanterie;
                    lignePionEnBataille.I_CAVALERIE -= pertesCavalerie;
                    lignePionEnBataille.I_ARTILLERIE -= pertesArtillerie;
                    if (lignePionEnBataille.Moral <= 0 && !lignePionEnBataille.estArtillerie)
                    {
                        FuiteAuCombat(lignePionEnBataille, zoneDefenseur, lignePionsEnBatailleDefenseur);
                    }
                    else
                    {
                        if (pertesMoral > 0)
                        {
                            if (!EnvoyerMessageBataille(lignePionEnBataille, ClassMessager.MESSAGES.MESSAGE_PERTES_AU_COMBAT, 
                                                            pertesInfanterie, pertesCavalerie, pertesArtillerie, pertesMoral, 0, 0, listePionsEnBatailleAttaquant))
                            {
                                message = string.Format("CalculDesPertesAuCombat : erreur lors de l'envoi d'un message pour perte de moral au combat");
                                LogFile.Notifier(message, out messageErreur);
                                return false;
                            }
                        }
                    }
                }

                //message indiquant le niveau de pertes infligé à l'ennemi
                foreach (Donnees.TAB_PIONRow lignePionEnBataille in listePionsEnBatailleAttaquant)
                {
                    if (!EnvoyerMessageBataille(lignePionEnBataille, ClassMessager.MESSAGES.MESSAGE_TIR_SUR_ENNEMI, 
                                                        pertesInfanterieTotal, pertesCavalerieTotal, pertesArtillerieTotal, 
                                                        pertesMoral, 0, 0, listePionsEnBatailleDefenseur))
                    {
                        message = string.Format("CalculDesPertesAuCombat : erreur lors de l'envoi d'un message pour tir sur l'ennemi");
                        LogFile.Notifier(message, out messageErreur);
                        return false;
                    }
                }
                pertesEffectifReels = pertesInfanterieTotal + pertesCavalerieTotal + pertesArtillerieTotal;
                return true;
            }

            /// <summary>
            /// Si un leader ou une unité d'artillerie se retrouve dans une zone sans unité combattante, il doit être remis en réserve
            /// </summary>
            /// <param name="ligneBataille">Bataille pour laquelle on recherche si un leader ou une unité d'artillerie se retrouve sans unité au combat</param>
            private void RemiseLeaderEnReserve()
            {
                string message;
                int i;

                RecherchePionsEnBataille(out _, out _, out _, out _, out _, out _, 
                                        out Donnees.TAB_PIONRow[] lignePionsEnBataille012, out Donnees.TAB_PIONRow[] lignePionsEnBataille345, true/*bengagement*/, false/*bcombattif*/, true/*QG*/, true /*bArtillerie*/);
                Donnees.TAB_BATAILLE_PIONSRow lignePionBataille;
                for (i = 0; i < 3; i++)
                {
                    List<Donnees.TAB_PIONRow> ligneLeaderArtillerie012 = new();
                    List<Donnees.TAB_PIONRow> ligneLeaderArtillerie345 = new();
                    bool bCombattants012 = false;
                    bool bCombattants345 = false;
                    foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille012)
                    {
                        if (lignePion.IsI_ZONE_BATAILLENull() || lignePion.I_ZONE_BATAILLE != i) continue;
                        if (lignePion.estCombattif && !lignePion.estArtillerie) bCombattants012 = true;
                        if (lignePion.estQG || lignePion.estArtillerie) ligneLeaderArtillerie012.Add(lignePion);
                    }
                    foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille345)
                    {
                        if (lignePion.IsI_ZONE_BATAILLENull() || lignePion.I_ZONE_BATAILLE != i + 3) continue;
                        if (lignePion.estCombattif && !lignePion.estArtillerie) bCombattants345 = true;
                        if (lignePion.estQG || lignePion.estArtillerie) ligneLeaderArtillerie345.Add(lignePion);
                    }
                    if (ligneLeaderArtillerie012.Count>0 && !bCombattants012)
                    {
                        foreach (Donnees.TAB_PIONRow lignePion in ligneLeaderArtillerie012)
                        {
                            message = string.Format("EffectuerBataille {1} ID={0} se retrouve seul en zone {2} et est remis en réserve",
                                                    lignePion.ID_PION, lignePion.S_NOM, i);
                            LogFile.Notifier(message);
                            lignePion.SetI_ZONE_BATAILLENull();
                            lignePionBataille = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePion.ID_PION, ID_BATAILLE);
                            lignePionBataille.B_ENGAGEE = false; // = dans le combat mais pas au front
                        }
                    }
                    if (ligneLeaderArtillerie345.Count>0 && !bCombattants345)
                    {
                        foreach (Donnees.TAB_PIONRow lignePion in ligneLeaderArtillerie345)
                        {
                            message = string.Format("EffectuerBataille {1} ID={0} se retrouve seul en zone {2} et est remis à disposition",
                                                    lignePion.ID_PION, lignePion.S_NOM, i + 3);
                            LogFile.Notifier(message);
                            lignePion.SetI_ZONE_BATAILLENull();
                            lignePionBataille = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePion.ID_PION, ID_BATAILLE);
                            lignePionBataille.B_ENGAGEE = false;// = dans le combat mais pas au front
                        }
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
                for (int l = 0; l < lignePionsEnBataille.Length; l++)
                {
                    Donnees.TAB_PIONRow lignePion = lignePionsEnBataille[l];
                    if (lignePion.B_DETRUIT || lignePion.B_BLESSES) { continue; }
                    if (lignePion.estQG && lignePion.estJoueur)// test sur estJoueur ajouté à cause du cas "Bessières", général pouvant uniquement servir en appui tactique mais pas en commandement
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
            /// <param name="listePionsCombattifsSansQG">unités combattives dans la zones</param>
            /// <param name="listePionsNonEngagesSansQG">unités non engagées dans la zones</param>
            /// <param name="iZone">zone d'engagement</param>
            /// <returns>unité engagée</returns>
            private Donnees.TAB_PIONRow AffecterUneUniteAuHasardEnBataille(Donnees.TAB_PIONRow[] listePionsCombattifsSansQG, Donnees.TAB_PIONRow[] listePionsNonEngagesSansQG, int iZone)
            {
                Donnees.TAB_PIONRow lignePion;

                if (listePionsNonEngagesSansQG.Length <= 0) return null;
                Random de = new();
                if (listePionsCombattifsSansQG.Length>0)
                {
                    lignePion = listePionsCombattifsSansQG[de.Next(listePionsCombattifsSansQG.Length)];
                }
                else
                {
                    lignePion = listePionsNonEngagesSansQG[de.Next(listePionsNonEngagesSansQG.Length)];
                }
                //if (this.ID_BATAILLE==19) lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(321);
                //lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(171);//debug


                lignePion.I_ZONE_BATAILLE = iZone;//toujours la zone centrale donc 1 ou 4
                //engagement dans la bataille
                Donnees.TAB_BATAILLE_PIONSRow lignePionBataille = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePion.ID_PION, ID_BATAILLE);
                if (null == lignePionBataille) return null;
                lignePionBataille.B_ENGAGEE = true;
                //pour les presentations de fin de partie
                lignePionBataille.B_ENGAGEMENT = true;
                lignePionBataille.I_ZONE_BATAILLE_ENGAGEMENT = iZone;
                if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_ENGAGEMENT_AUTOMATIQUE_DANS_BATAILLE))
                {
                    return null;
                }
                return lignePion;
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
                int nbPions = Donnees.m_donnees.TAB_PION.Count;
                while (i < nbPions)
                {
                    Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION[i];
                    //Pion à distance correcte
                    ligneCasePion = Donnees.m_donnees.TAB_CASE.FindParID_CASE(lignePion.ID_CASE);
                    dist = Constantes.Distance(ligneCasePion.I_X, ligneCasePion.I_Y, xBataille, yBataille);
                    if (dist < Constantes.CST_BRUIT_DU_CANON * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE)
                    {
                        //Pion ne se trouvant pas déjà dans la bataille
                        var resultComplet = from BataillePion in Donnees.m_donnees.TAB_BATAILLE_PIONS
                                            where (BataillePion.ID_PION == lignePion.ID_PION)
                                            && (BataillePion.ID_BATAILLE == ID_BATAILLE)
                                            select BataillePion.ID_PION;

                        if (!resultComplet.Any() && lignePion.estCombattifQG(true, true))
                        {
                            EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_BRUIT_DU_CANON);
                        }
                    }
                    i++;
                }
            }

            public bool RecherchePionsEnBataille(out int nbUnites012, out int nbUnites345, out int[] des, out int[] modificateurs, out int[] effectifs, out int[] canons, out Donnees.TAB_PIONRow[] lignePionsEnBataille012, out Donnees.TAB_PIONRow[] lignePionsEnBataille345, bool? bEngagement, bool bCombattif, bool bQG, bool bArtillerie)
            {
                des = new int[6];
                effectifs = new int[6];
                canons = new int[6];
                modificateurs = new int[6];

                nbUnites345 = 0;
                lignePionsEnBataille345 = null;

                if (!RecherchePionsEnBatailleParZone(ID_BATAILLE, true, out nbUnites012, ref des, ref modificateurs, ref effectifs, ref canons, out lignePionsEnBataille012, bEngagement, bCombattif, bQG, bArtillerie))
                {
                    return false;
                }
                if (!RecherchePionsEnBatailleParZone(ID_BATAILLE, false, out nbUnites345, ref des, ref modificateurs, ref effectifs, ref canons, out lignePionsEnBataille345, bEngagement, bCombattif, bQG, bArtillerie))
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
            /// <param name="bQG">true si on  prend les unités QG, false sinon</param>
            /// <param name="bArtillerie">true si on prend les unités d'artillerie pure, false sinon</param>
            /// <returns>true si ok, false si ko</returns>
            public bool RecherchePionsEnBatailleParZone(int idBataille, bool bZone012, out int nbUnites, ref int[] des, ref int[] modificateurs, ref int[] effectifs, ref int[] canons, out Donnees.TAB_PIONRow[] lignePionsEnBatailleZone, bool? bEngagement, bool bCombattif, bool bQG, bool bArtillerie)
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
                bool[] presenceArtillerie = new bool[] { false, false, false, false, false, false };

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
                Monitor.Enter(Donnees.m_donnees.TAB_BATAILLE_PIONS.Rows.SyncRoot);
                Verrou.Verrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                var resultComplet = from BataillePion in Donnees.m_donnees.TAB_BATAILLE_PIONS
                                    from Pion in Donnees.m_donnees.TAB_PION
                                    where (BataillePion.ID_PION == Pion.ID_PION)
                                        //&& (BataillePion.B_ENGAGEE == bEngagement)
                                    && (BataillePion.ID_NATION == idNation)
                                    && (BataillePion.ID_BATAILLE == idBataille)
                                    && !Pion.B_DETRUIT
                                    select new rechercheQG { idPion = Pion.ID_PION, bEngagee = BataillePion.B_ENGAGEMENT, izoneEngagement = Pion.IsI_ZONE_BATAILLENull() ? -1 : Pion.I_ZONE_BATAILLE };

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
                                              where (resultatPion.bEngagee == bEngagement) // && resultatPion.izoneEngagement >= 0 -> si on garde ce test, on ne trouve aucune unité engagée au moment de la retaire car la zone d'engagement est remise à null
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
                        if (bQG || (!lignePion.estQG && (!lignePion.estArtillerie || bArtillerie)))
                        {
                            nb++;
                        }
                    }
                }
                lignePionsEnBatailleZone = new Donnees.TAB_PIONRow[nb];

                //affecte la table et les valeurs
                for (i = 0; i < 6; i++) { valeurTactique[i] = -1; }

                nb = 0;
                for (i = 0; i < resultPionsBataille.Count(); i++)
                {
                    lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(resultPionsBataille.ElementAt(i));
                    if (null != lignePion && (!bCombattif || lignePion.estCombattif || bArtillerie || bQG) && (bQG || (!lignePion.estQG && (!lignePion.estArtillerie || bArtillerie))))
                    {

                        if (!bCombattif || lignePion.estCombattif)
                        {
                            if (bQG || (!lignePion.estQG && (!lignePion.estArtillerie || bArtillerie)))
                            {
                                lignePionsEnBatailleZone[nb++] = lignePion;
                            }
                        }

                        if (!lignePion.IsI_ZONE_BATAILLENull() && lignePion.I_ZONE_BATAILLE >= 0)//cas du leader de la zone, engagé mais sans zone
                        {
                            if (lignePion.estQG)
                            {
                                if (valeurTactique[lignePion.I_ZONE_BATAILLE] < lignePion.I_TACTIQUE)
                                {
                                    valeurTactique[lignePion.I_ZONE_BATAILLE] = lignePion.I_TACTIQUE;
                                }
                            }
                            else
                            {
                                if (!lignePion.estArtillerie)
                                {
                                    //modification liée à la valeur de matériel et de ravitaillement
                                    modificateurs[lignePion.I_ZONE_BATAILLE] += Constantes.CalculerEfficaciteAuCombat(lignePion.I_MATERIEL, lignePion.I_RAVITAILLEMENT);
                                }
                                else
                                {
                                    presenceArtillerie[lignePion.I_ZONE_BATAILLE] = true;
                                }
                                canons[lignePion.I_ZONE_BATAILLE] += lignePion.I_ARTILLERIE;
                                effectifs[lignePion.I_ZONE_BATAILLE] += lignePion.I_INFANTERIE + lignePion.I_CAVALERIE;
                                modificateurs[lignePion.I_ZONE_BATAILLE] += (int)Math.Floor(lignePion.I_EXPERIENCE);

                                if (lignePion.B_CAVALERIE_DE_LIGNE) { presenceCavalerieDeLigne[lignePion.I_ZONE_BATAILLE] = true; }
                                if (lignePion.B_CAVALERIE_LOURDE) { presenceCavalerieLourde[lignePion.I_ZONE_BATAILLE] = true; }
                                if (lignePion.B_GARDE) { presenceGarde[lignePion.I_ZONE_BATAILLE] = true; }
                                if (lignePion.B_VIEILLE_GARDE) { presenceVieilleGarde[lignePion.I_ZONE_BATAILLE] = true; }
                            }
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

                Verrou.Deverrouiller(Donnees.m_donnees.TAB_PION.Rows.SyncRoot);
                Monitor.Exit(Donnees.m_donnees.TAB_BATAILLE_PIONS.Rows.SyncRoot);
                for (i = 0; i < 6; i++)
                {
                    if (valeurTactique[i] >= 0)
                    {
                        //si un chef est au moins présent, valeurTactique est passé au minimum à 0 (au lieu de -1)
                        //si un chef est présent, on ajoute au minimum 1 dé supplémentaire.
                        des[i] += Math.Max(1, valeurTactique[i]);
                        LogFile.Notifier(string.Format("RecherchePionsEnBatailleParZone valeurTactique sur zone={0} = {1}", i, valeurTactique[i]));
                    }
                    Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN((int)this["ID_TERRAIN_" + Convert.ToString(i)]);
                    if (!ligneModeleTerrain.B_SANS_BONUS_CAVALERIE)
                    {
                        if (presenceCavalerieDeLigne[i] && !presenceCavalerieLourde[i]) { des[i] += 1; }
                        if (presenceCavalerieLourde[i]) { des[i] += 2; }
                    }
                    if (presenceGarde[i] && !presenceVieilleGarde[i]) { des[i] += 2; }
                    if (presenceVieilleGarde[i]) { des[i] += 3; }
                    if (presenceArtillerie[i]) { des[i] += 1; }
                }

                return true;
            }

            private bool FuiteAuCombat(Donnees.TAB_PIONRow lignePionFuite, int zone, List<Donnees.TAB_PIONRow> lignePionsEnBataille)
            {
                string message;
                bool[] unitesCombattiveHorsArtillerie= new bool[6];
                //int iPertesInfanterie, iPertesCavalerie, iPertesArtillerie;

                message = string.Format("Début FuiteAuCombat pour le pion ID={0}:{1} moral={2}",lignePionFuite.ID_PION, lignePionFuite.S_NOM, lignePionFuite.I_MORAL);
                LogFile.Notifier(message);
                if (lignePionFuite.I_MORAL<=0)
                {
                    //l'unité a déjà fuit, elle ne va pas fuir deux fois !
                    lignePionFuite.I_MORAL = 0;
                    lignePionFuite.B_FUITE_AU_COMBAT = true;
                    return true;
                }
                Donnees.TAB_BATAILLE_PIONSRow ligneBataillePions = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePionFuite.ID_PION, ID_BATAILLE);
                if (null == ligneBataillePions)
                {
                    message = string.Format("FuiteAuCombat : impossible de retrouver le pion ID=" + lignePionFuite.ID_PION + " dans la bataille ID=" + ID_BATAILLE);
                    LogFile.Notifier(message);
                    return false;
                }

                //lorsque qu'une unité fuit au combat, toutes les unités présentes dans la même zone, perdent CST_PERTE_MORAL_FUITE pts de moral
                message = string.Format("FuiteAuCombat : L'unité {0} fuit la zone {1} de la bataille {2}",
                    lignePionFuite.ID_PION, zone, lignePionFuite.ID_BATAILLE);
                LogFile.Notifier(message);
                for (int i=0; i<6; i++) {unitesCombattiveHorsArtillerie[i]=false;}
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille)
                {
                    if (lignePion.ID_PION != lignePionFuite.ID_PION &&
                        !lignePion.IsI_ZONE_BATAILLENull() && //possible si l'unité a déjà fuit le combat
                        lignePion.I_ZONE_BATAILLE == zone && lignePion.estCombattif
                        && !lignePion.estArtillerie && lignePion.I_MORAL>0)
                    {
                        message = string.Format("FuiteAuCombat effet 'boule de neige' pour le pion ID={0}:{1} moral={2}", lignePion.ID_PION, lignePion.S_NOM, lignePionFuite.I_MORAL);
                        LogFile.Notifier(message, out string messageErreur);
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
                    //mise à jour dans la table des pions au combat pour l'affichage de fin de partie et les messages
                    ligneBataillePions.B_RETRAITE = true;
                    ligneBataillePions.I_INFANTERIE_FIN = lignePionFuite.I_INFANTERIE;
                    ligneBataillePions.I_CAVALERIE_FIN = lignePionFuite.I_CAVALERIE;
                    ligneBataillePions.I_ARTILLERIE_FIN = lignePionFuite.I_ARTILLERIE;
                    ligneBataillePions.I_MORAL_FIN = lignePionFuite.I_MORAL;
                    ligneBataillePions.I_FATIGUE_FIN = lignePionFuite.I_FATIGUE;

                    if (!EnvoyerMessageBataille(lignePionFuite, ClassMessager.MESSAGES.MESSAGE_FUITE_AU_COMBAT,
                            ligneBataillePions.I_INFANTERIE_DEBUT - ligneBataillePions.I_INFANTERIE_FIN,
                            ligneBataillePions.I_CAVALERIE_DEBUT - ligneBataillePions.I_ARTILLERIE_FIN,
                            ligneBataillePions.I_ARTILLERIE_DEBUT - ligneBataillePions.I_ARTILLERIE_FIN, 
                            Constantes.CST_PERTE_MORAL_FUITE, 0, 0, null))
                    {
                        message = string.Format("FuiteAuCombat : erreur lors de l'envoi d'un message pour fuite au combat");
                        LogFile.Notifier(message);
                        return false;
                    }

                    //l'unité fuit automatiquement vers son chef
                    lignePionFuite.SupprimerTousLesOrdres();
                    Donnees.TAB_PIONRow lignePionChef = lignePionFuite.proprietaire;
                    Monitor.Enter(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                    Donnees.m_donnees.TAB_ORDRE.AddTAB_ORDRERow(
                        Constantes.NULLENTIER,//ID_ORDRE_TRANSMIS
                        Constantes.NULLENTIER,//ID_ORDRE_SUIVANT global::System.Convert.DBNull,
                        Constantes.NULLENTIER,///ID_ORDRE_WEB
                        Constantes.ORDRES.MOUVEMENT,
                        lignePionFuite.ID_PION,
                        lignePionFuite.ID_CASE,
                        lignePionFuite.I_INFANTERIE + lignePionFuite.I_CAVALERIE + lignePionFuite.I_ARTILLERIE,
                        lignePionChef.ID_CASE,
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
                        Donnees.m_donnees.TAB_JEU[0].I_LEVER_DU_SOLEIL,//I_HEURE_DEBUT
                        Donnees.m_donnees.TAB_JEU[0].I_COUCHER_DU_SOLEIL - Donnees.m_donnees.TAB_JEU[0].I_LEVER_DU_SOLEIL,//I_DUREE
                        Constantes.NULLENTIER//I_ENGAGEMENT
                        );//ID_BATAILLE
                    Monitor.Exit(Donnees.m_donnees.TAB_ORDRE.Rows.SyncRoot);
                }
                else
                {
                    //l'unité est détruite
                    lignePionFuite.DetruirePion();//à faire avant le message, sinon, non détruite dans le message
                    if (!EnvoyerMessageBataille(lignePionFuite, ClassMessager.MESSAGES.MESSAGE_FUITE_ET_DETRUITE_AU_COMBAT))
                    {
                        message = string.Format("FuiteAuCombat : erreur lors de l'envoi d'un message pour fuite au combat");
                        LogFile.Notifier(message);
                        return false;
                    }
                    //lignePionFuite.Delete();
                }
                //desengagement du combat
                //lignePionFuite.SetID_BATAILLENull(); l'unité ne quitte définitivement le combat que lorsque la fuite est terminée
                //lignePionFuite.SetI_ZONE_BATAILLENull();, à ne pas faire car la zone est encore utile pour calculer les pertes par secteur
                lignePionFuite.B_FUITE_AU_COMBAT = true;//utile uniquement pour certaines conditions de victoires où l'on compte le nombre d'unités démoralisées durant la partie
                lignePionFuite.I_MORAL = 0;
                lignePionFuite.I_TOUR_FUITE_RESTANT = 2;
                return true;
            }

            private static bool RepartirPertes(Donnees.TAB_PIONRow lignePion, int pertes, out int iPertesInfanterie, out int iPertesCavalerie, out int iPertesArtillerie)
            {
                iPertesCavalerie = iPertesArtillerie = 0;
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

                LogFile.Notifier(string.Format("RepartirPertes sur le pion ID={0}:{1} inf={2} cav={3} art={4}", lignePion.ID_PION, lignePion.S_NOM, iPertesInfanterie, iPertesCavalerie, iPertesArtillerie));

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

                if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_PERTES_TOTALE_AU_COMBAT,
                    ligneBataillePion.I_INFANTERIE_DEBUT - ligneBataillePion.I_INFANTERIE_FIN,
                    ligneBataillePion.I_CAVALERIE_DEBUT - ligneBataillePion.I_CAVALERIE_FIN,
                    ligneBataillePion.I_ARTILLERIE_DEBUT - ligneBataillePion.I_ARTILLERIE_FIN,
                    ligneBataillePion.I_MORAL_DEBUT - ligneBataillePion.I_MORAL_FIN, 0, 0, null))
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
                    if (lignePion.I_MORAL_MAX>0)
                    {
                        int infanterieRetour = (ligneBataillePion.I_INFANTERIE_DEBUT - ligneBataillePion.I_INFANTERIE_FIN) * 3 * ligneBataillePion.I_MORAL_FIN / 10 / lignePion.I_MORAL_MAX;
                        int cavalerieRetour = (ligneBataillePion.I_CAVALERIE_DEBUT - ligneBataillePion.I_CAVALERIE_FIN) * 3 * ligneBataillePion.I_MORAL_FIN / 10 / lignePion.I_MORAL_MAX;
                        if (infanterieRetour > 0 || cavalerieRetour > 0)
                        {
                            lignePion.I_INFANTERIE += infanterieRetour;
                            lignePion.I_CAVALERIE += cavalerieRetour;
                            if (!EnvoyerMessageBataille(lignePion, ClassMessager.MESSAGES.MESSAGE_SOINS_APRES_BATAILLE, 
                                                              infanterieRetour, cavalerieRetour, 0, 0, 0, 0, null))
                            {
                                message = string.Format("PertesFinDeBataille : erreur lors de l'envoi d'un message SOINS_APRES_BATAILLE");
                                LogFile.Notifier(message);
                                return false;
                            }
                        }

                        infanterieBlesse += ((ligneBataillePion.I_INFANTERIE_DEBUT - ligneBataillePion.I_INFANTERIE_FIN) * 3 / 10) - infanterieRetour;
                        cavalerieBlesse += ((ligneBataillePion.I_CAVALERIE_DEBUT - ligneBataillePion.I_CAVALERIE_FIN) * 3 / 10) - cavalerieRetour;
                    }
                }
                else
                {
                    int infanteriePrisonnier = (ligneBataillePion.I_INFANTERIE_DEBUT - ligneBataillePion.I_INFANTERIE_FIN) * 3 / 10;
                    int cavaleriePrisonnier = (ligneBataillePion.I_CAVALERIE_DEBUT - ligneBataillePion.I_CAVALERIE_FIN) * 3 / 10;
                    if (infanteriePrisonnier + cavaleriePrisonnier >= Constantes.CST_TAILLE_MINIMUM_UNITE)
                    {
                        //on déterminer au hasard l'unité qui a fait la capture
                        Random de = new();
                        Donnees.TAB_PIONRow lignePionQuiCapture = lignePionsEnnemis[de.Next(0, lignePionsEnnemis.Length - 1)];

                        Donnees.TAB_PIONRow lignePionConvoiDePrisonniers = lignePion.CreerConvoiDePrisonniers(lignePionQuiCapture, infanteriePrisonnier, cavaleriePrisonnier, 0);
                        if (null == lignePionConvoiDePrisonniers)
                        {
                            message = string.Format("PertesFinDeBataille : erreur lors de l'appel à CreerConvoiDePrisonniers");
                            LogFile.Notifier(message);
                            return false;
                        }
                        if (!EnvoyerMessageBataille(lignePionConvoiDePrisonniers, ClassMessager.MESSAGES.MESSAGE_PRISONNIERS_APRES_BATAILLE))
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
                    lignePionConvoiDeBlesses.I_INFANTERIE_INITIALE = lignePionConvoiDeBlesses.I_INFANTERIE;
                    lignePionConvoiDeBlesses.I_CAVALERIE = cavalerieBlesse;
                    lignePionConvoiDeBlesses.I_CAVALERIE_INITIALE = lignePionConvoiDeBlesses.I_CAVALERIE;
                    if (!EnvoyerMessageBataille(lignePionConvoiDeBlesses, ClassMessager.MESSAGES.MESSAGE_BLESSES_APRES_BATAILLE))
                    {
                        message = string.Format("PertesFinDeBataille : erreur lors de l'envoi d'un message MESSAGE_BLESSES_APRES_BATAILLE");
                        LogFile.Notifier(message);
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// si le leader du pion est dans la zone de bataille, le message est immédiat dans tous les cas
            /// Trop de problèmes avec les messages arrivant trop tardivement alors que le chef est présent
            /// </summary>
            /// <param name="lignePion">pion envoyant le message</param>
            /// <param name="typeMessage">type du message</param>
            private bool EnvoyerMessageBataille(TAB_PIONRow lignePion, ClassMessager.MESSAGES typeMessage)
            {
                TAB_CASERow ligneCaseProprietaire = Donnees.m_donnees.tableTAB_CASE.FindParID_CASE(lignePion.proprietaire.ID_CASE);
                if (ligneCaseProprietaire.I_X >= I_X_CASE_HAUT_GAUCHE && ligneCaseProprietaire.I_Y >= I_Y_CASE_HAUT_GAUCHE
                                        && ligneCaseProprietaire.I_X <= I_X_CASE_BAS_DROITE && ligneCaseProprietaire.I_Y <= I_Y_CASE_BAS_DROITE)
                {
                    return ClassMessager.EnvoyerMessageImmediat(lignePion, typeMessage, this);
                }
                return ClassMessager.EnvoyerMessage(lignePion, typeMessage, this);
            }

            /// <summary>
            /// si le leader du pion est dans la zone de bataille, le message est immédiat dans tous les cas
            /// Trop de problèmes avec les messages arrivant trop tardivement alors que le chef est présent
            /// </summary>
            /// <param name="lignePion">pion envoyant le message</param>
            /// <param name="typeMessage">type du message</param>
            private bool EnvoyerMessageBataille(TAB_PIONRow lignePion, ClassMessager.MESSAGES typeMessage, int moral)
            {
                TAB_CASERow ligneCaseProprietaire = Donnees.m_donnees.tableTAB_CASE.FindParID_CASE(lignePion.proprietaire.ID_CASE);
                if (ligneCaseProprietaire.I_X >= I_X_CASE_HAUT_GAUCHE && ligneCaseProprietaire.I_Y >= I_Y_CASE_HAUT_GAUCHE
                                        && ligneCaseProprietaire.I_X <= I_X_CASE_BAS_DROITE && ligneCaseProprietaire.I_Y <= I_Y_CASE_BAS_DROITE)
                {
                    return ClassMessager.EnvoyerMessageImmediat(lignePion, typeMessage, moral, this);
                }
                return ClassMessager.EnvoyerMessage(lignePion, typeMessage, moral, this);
            }


            private bool EnvoyerMessageBataille(TAB_PIONRow lignePion, ClassMessager.MESSAGES typeMessage, int pertesInfanterieTotal, int pertesCavalerieTotal, int pertesArtillerieTotal, 
                int pertesMoral, int pertesMaterielPion, int pertesRavitaillementPion, List<TAB_PIONRow> listePionsEnBataille)
            {
                bool bImmediat = false;
                TAB_CASERow ligneCaseProprietaire = Donnees.m_donnees.tableTAB_CASE.FindParID_CASE(lignePion.proprietaire.ID_CASE);
                if (ligneCaseProprietaire.I_X >= I_X_CASE_HAUT_GAUCHE && ligneCaseProprietaire.I_Y >= I_Y_CASE_HAUT_GAUCHE
                                        && ligneCaseProprietaire.I_X <= I_X_CASE_BAS_DROITE && ligneCaseProprietaire.I_Y <= I_Y_CASE_BAS_DROITE)
                {
                    bImmediat = true;
                }
                return ClassMessager.EnvoyerMessage(lignePion, typeMessage, pertesInfanterieTotal, pertesCavalerieTotal, pertesArtillerieTotal, pertesMoral, 0,
                    this, null, null, null, 0, pertesRavitaillementPion, pertesMaterielPion, string.Empty, bImmediat, string.Empty, listePionsEnBataille);
            }

            /// <summary>
            /// Gain d'expérience pour toutes les unités ayant participées à une bataille de 4 heures ou plus
            /// </summary>
            /// <returns>true si ok, false si ko</returns>
            private static bool GainExperienceFinDeBataille(Donnees.TAB_PIONRow[] lignePionsEnBataille)
            {
                foreach (Donnees.TAB_PIONRow lignePion in lignePionsEnBataille)
                {
                    if (!lignePion.GainExperienceFinDeBataille()) { return false; }
                }
                return true;
            }

            /// <summary>
            /// Generation du film résulmant la bataille
            /// </summary>
            /// <param name="nomFichier"></param>
            /// <returns></returns>
            public string GenererFilm(string nomFichierPartie, string nomFichier, string repertoireVideo, ref int positionFilm, int hauteurFilm, int largeurFilm, bool genererVideo)
            {
                List<ZoneBataille> zonesBataille = new();
                List<UniteBataille> unitesBataille = new();
                List<RoleBataille> rolesBataille=new();
                TIPETERRAINBATAILLE[] terrains= new TIPETERRAINBATAILLE[6];
                TIPETERRAINBATAILLE[] obstacles = new TIPETERRAINBATAILLE[3];
                TIPEORIENTATIONBATAILLE orientation = (this.C_ORIENTATION =='V') ? TIPEORIENTATIONBATAILLE.VERTICAL : TIPEORIENTATIONBATAILLE.HORIZONTAL;
                TIPEFINBATAILLE fin = TIPEFINBATAILLE.RETRAITE012;
                int tourpoursuite = Donnees.m_donnees.TAB_PARTIE.Nocturne(Donnees.m_donnees.TAB_PARTIE.HeureBase(this.I_TOUR_FIN)) ? 0 :1 ;
                int iNation012=-1, iNation345 = -1;
                /*
                switch(this.S_FIN)
                {
                    case "RETRAITE012":
                        fin = TIPEFINBATAILLE.RETRAITE012;
                        tourpoursuite = 1;
                        break;
                    case "RETRAITE345":
                        fin = TIPEFINBATAILLE.RETRAITE345;
                        tourpoursuite = 1;
                        break;
                    case "VICTOIRE012":
                        fin = TIPEFINBATAILLE.VICTOIRE012;
                        tourpoursuite = 1;
                        break;
                    case "VICTOIRE345":
                        fin = TIPEFINBATAILLE.VICTOIRE345;
                        tourpoursuite = 1;
                        break;
                    default:
                        fin = TIPEFINBATAILLE.NUIT;
                        break;
                }
                */
                for (int i=0;i<6;i++)
                {
                    terrains[i] = FilmTerrain((int)this["ID_TERRAIN_" + Convert.ToString(i)]);
                }
                obstacles[0] = FilmTerrain(this.ID_OBSTACLE_03);
                obstacles[1] = FilmTerrain(this.ID_OBSTACLE_14);
                obstacles[2] = FilmTerrain(this.ID_OBSTACLE_25);

                for (int tour = this.I_TOUR_DEBUT; tour < this.I_TOUR_FIN + tourpoursuite + 1; tour++)
                {
                    TAB_BATAILLE_VIDEORow ligneBatailleVideo = Donnees.m_donnees.TAB_BATAILLE_VIDEO.FindByID_BATAILLEI_TOUR(this.ID_BATAILLE, tour);
                    if (null != ligneBatailleVideo)
                    {
                        ZoneBataille zb;
                        zb = new()
                        {
                            iTour = tour,
                            sCombat = new string[6]
                        };
                        for (int i = 0; i < 6; i++)
                        {
                            zb.sCombat[i] = (string)ligneBatailleVideo["S_COMBAT_" + Convert.ToString(i)];
                        }
                        zb.iPertes = new int[6];
                        for (int i = 0; i < 6; i++)
                        {
                            zb.iPertes[i] = (int)ligneBatailleVideo["I_PERTES_" + Convert.ToString(i)];
                        }
                        zonesBataille.Add(zb);

                        if (!ligneBatailleVideo.IsID_LEADER_012Null() || !ligneBatailleVideo.IsID_LEADER_345Null())
                        {
                            if (!ligneBatailleVideo.IsID_LEADER_012Null() && ligneBatailleVideo.ID_LEADER_012>=0)
                            {
                                RoleBataille roleBataille = new() { iTour = tour };
                                TAB_PIONRow lignePion = m_donnees.TAB_PION.FindByID_PION(ligneBatailleVideo.ID_LEADER_012);
                                roleBataille.iNation012 = lignePion.idNation;
                                roleBataille.nomLeader012 = lignePion.S_NOM;
                                roleBataille.nomLeader345 = string.Empty;
                                rolesBataille.Add(roleBataille);
                            }
                            if (!ligneBatailleVideo.IsID_LEADER_345Null() && ligneBatailleVideo.ID_LEADER_345>=0)
                            {
                                RoleBataille roleBataille = new() { iTour = tour };
                                TAB_PIONRow lignePion = m_donnees.TAB_PION.FindByID_PION(ligneBatailleVideo.ID_LEADER_345);
                                roleBataille.iNation345 = lignePion.idNation;
                                roleBataille.nomLeader012 = string.Empty;
                                roleBataille.nomLeader345 = lignePion.S_NOM;
                                rolesBataille.Add(roleBataille);
                            }
                        }
                    }
                    //TAB_BATAILLE_VIDEORow ligneBatailleVideoFin = Donnees.m_donnees.TAB_BATAILLE_VIDEO.FindByID_BATAILLEI_TOUR(this.ID_BATAILLE, this.I_TOUR_FIN + tourpoursuite);
                    switch (this.S_FIN)
                    {
                        case "RETRAITE012":
                            fin = TIPEFINBATAILLE.RETRAITE012;
                            tourpoursuite = 1;
                            break;
                        case "RETRAITE345":
                            fin = TIPEFINBATAILLE.RETRAITE345;
                            tourpoursuite = 1;
                            break;
                        case "VICTOIRE012":
                            fin = TIPEFINBATAILLE.VICTOIRE012;
                            tourpoursuite = 1;
                            break;
                        case "VICTOIRE345":
                            fin = TIPEFINBATAILLE.VICTOIRE345;
                            tourpoursuite = 1;
                            break;
                        default:
                            fin = TIPEFINBATAILLE.NUIT;
                            break;
                    }

                    TAB_BATAILLE_PIONS_VIDEORow[] listeBataillePionsVideo = 
                        (TAB_BATAILLE_PIONS_VIDEORow[]) Donnees.m_donnees.TAB_BATAILLE_PIONS_VIDEO.Select(string.Format("ID_BATAILLE={0} AND I_TOUR={1}",
                            this.ID_BATAILLE, tour));
                    foreach(TAB_BATAILLE_PIONS_VIDEORow ligneBataillePionsVideo in listeBataillePionsVideo)
                    {
                        TIPEUNITEBATAILLE tipe = DefinitionTipeUniteBataille(ligneBataillePionsVideo);
                        unitesBataille.Add(new UniteBataille() { iTour = tour, 
                            iZone = ligneBataillePionsVideo.I_ZONE_BATAILLE_ENGAGEMENT, 
                            tipe = tipe, 
                            effectifInfanterie = ligneBataillePionsVideo.I_INFANTERIE, 
                            effectifCavalerie = ligneBataillePionsVideo.I_CAVALERIE, 
                            effectifArtillerie = ligneBataillePionsVideo.I_ARTILLERIE, 
                            moral = ligneBataillePionsVideo.I_MORAL,
                            ID = ligneBataillePionsVideo.ID_PION, 
                            iNation = ligneBataillePionsVideo.ID_NATION, 
                            nom = ligneBataillePionsVideo.S_NOM
                        });

                        //recherche de la nation
                        TAB_PIONRow lignePion = m_donnees.TAB_PION.FindByID_PION(ligneBataillePionsVideo.ID_PION);
                        if (ligneBataillePionsVideo.B_ENGAGEE && ligneBataillePionsVideo.I_ZONE_BATAILLE_ENGAGEMENT>=0)
                        {
                            if (ligneBataillePionsVideo.I_ZONE_BATAILLE_ENGAGEMENT < 3)
                            {
                                iNation012 = lignePion.idNation;
                            }
                            else
                            {
                                iNation345 = lignePion.idNation;
                            }
                        }
                    }
                }
                // -> pas logique mais sur l'historique il manque parfois des données, il faut compléter
                if (iNation012 < 0) { iNation012 = (0==iNation345) ? 1 : 0; }
                if (iNation345 < 0) { iNation345 = (0 == iNation012) ? 1 : 0; }


                #region TEST
                /* 
                ZoneBataille zb;
                zb = new ZoneBataille() { iTour = 50 };
                zb.sCombat = new string[6];
                zb.sCombat[1] = "1 dés + 1, 0 relances = 5";
                zb.sCombat[4] = "4 dés + 0, 2 relances = 21";
                zb.iPertes = new int[6];
                zb.iPertes[1] = 1323;
                zb.iPertes[4] = 32;
                zonesBataille.Add(zb);

                zb = new ZoneBataille() { iTour = 52 };
                zb.sCombat = new string[6];
                zb.sCombat[1] = "1 dés + 1";
                zb.sCombat[4] = "4 dés + 0";
                zb.iPertes = new int[6];
                zb.iPertes[1] = 888;
                zb.iPertes[4] = 1076;
                zonesBataille.Add(zb);

                unitesBataille.Add(new UniteBataille() { iTour = 50, iZone = -1, tipe = TIPEUNITEBATAILLE.QG, effectifInfanterie = 0, effectifCavalerie = 0, effectifArtillerie = 0, ID = 123, iNation = 0, nom = "Napoléon" });
                unitesBataille.Add(new UniteBataille() { iTour = 50, iZone = -1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 700, effectifArtillerie = 12, ID = 123, iNation = 0, nom = "5 Lourd: Cuvellier" });
                unitesBataille.Add(new UniteBataille() { iTour = 50, iZone = -1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 700, effectifArtillerie = 12, ID = 123, iNation = 0, nom = "5 Lourd: Cuvellier" });
                unitesBataille.Add(new UniteBataille() { iTour = 50, iZone = -1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 700, effectifArtillerie = 12, ID = 123, iNation = 0, nom = "5 Lourd: Cuvellier" });
                unitesBataille.Add(new UniteBataille() { iTour = 50, iZone = -1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 700, effectifArtillerie = 12, ID = 123, iNation = 0, nom = "5 Lourd: Cuvellier" });
                unitesBataille.Add(new UniteBataille() { iTour = 50, iZone = -1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 700, effectifArtillerie = 12, ID = 123, iNation = 0, nom = "5 Lourd: Cuvellier" });
                unitesBataille.Add(new UniteBataille() { iTour = 50, iZone = -1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 7000, effectifArtillerie = 12, ID = 123, iNation = 0, nom = "5 Lourd: Cuvellier" });
                unitesBataille.Add(new UniteBataille() { iTour = 50, iZone = -1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 700, effectifArtillerie = 12, ID = 123, iNation = 0, nom = "5 Lourd: Cuvellier" });
                unitesBataille.Add(new UniteBataille() { iTour = 50, iZone = -1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 17000, effectifArtillerie = 12, ID = 123, iNation = 0, nom = "5 Lourd: Cuvellier" });
                unitesBataille.Add(new UniteBataille() { iTour = 50, iZone = -1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 700, effectifArtillerie = 12, ID = 123, iNation = 0, nom = "5 Lourd: Cuvellier" });
                unitesBataille.Add(new UniteBataille() { iTour = 50, iZone = 1, tipe= TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie=0, effectifCavalerie=1200, effectifArtillerie=8, ID=123, iNation=0, nom= "5 Leg: Lorge" });
                unitesBataille.Add(new UniteBataille() { iTour = 50, iZone = 1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 1200, effectifArtillerie = 8, ID = 123, iNation = 0, nom = "5 Leg: Lorge" });
                unitesBataille.Add(new UniteBataille() { iTour = 50, iZone = 1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 1200, effectifArtillerie = 8, ID = 123, iNation = 0, nom = "5 Leg: Lorge" });
                unitesBataille.Add(new UniteBataille() { iTour = 50, iZone = 1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 1200, effectifArtillerie = 8, ID = 123, iNation = 0, nom = "5 Leg: Lorge" });
                unitesBataille.Add(new UniteBataille() { iTour = 50, iZone = 1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 1200, effectifArtillerie = 8, ID = 123, iNation = 0, nom = "5 Leg: Lorge" });
                unitesBataille.Add(new UniteBataille() { iTour = 50, iZone = 1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 1200, effectifArtillerie = 8, ID = 123, iNation = 0, nom = "5 Leg: Lorge" });
                unitesBataille.Add(new UniteBataille() { iTour = 50, iZone = 1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 1200, effectifArtillerie = 8, ID = 123, iNation = 0, nom = "5 Leg: Lorge" });
                unitesBataille.Add(new UniteBataille() { iTour = 50, iZone = 4, tipe = TIPEUNITEBATAILLE.INFANTERIE, effectifInfanterie = 11800, effectifCavalerie = 200, effectifArtillerie = 14, ID = 123, iNation = 1, nom = "7B: Horn" });

                unitesBataille.Add(new UniteBataille() { iTour = 51, iZone = 1, tipe = TIPEUNITEBATAILLE.QG, effectifInfanterie = 0, effectifCavalerie = 0, effectifArtillerie = 0, ID = 123, iNation = 0, nom = "Napoléon" });
                unitesBataille.Add(new UniteBataille() { iTour = 51, iZone = 1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 700, effectifArtillerie = 12, ID = 123, iNation = 0, nom = "5 Lourd: Cuvellier" });
                unitesBataille.Add(new UniteBataille() { iTour = 51, iZone = 1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 980, effectifArtillerie = 7, ID = 123, iNation = 0, nom = "5 Leg: Lorge" });
                unitesBataille.Add(new UniteBataille() { iTour = 51, iZone = 4, tipe = TIPEUNITEBATAILLE.INFANTERIE, effectifInfanterie = 11500, effectifCavalerie = 200, effectifArtillerie = 14, ID = 123, iNation = 1, nom = "7B: Horn" });
                //pour la retraite
                unitesBataille.Add(new UniteBataille() { iTour = 52, iZone = -1, tipe = TIPEUNITEBATAILLE.QG, effectifInfanterie = 0, effectifCavalerie = 0, effectifArtillerie = 0, ID = 123, iNation = 0, nom = "Napoléon" });
                unitesBataille.Add(new UniteBataille() { iTour = 52, iZone = -1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 700, effectifArtillerie = 12, ID = 123, iNation = 0, nom = "5 Lourd: Cuvellier" });
                unitesBataille.Add(new UniteBataille() { iTour = 52, iZone = -1, tipe = TIPEUNITEBATAILLE.CAVALERIE, effectifInfanterie = 0, effectifCavalerie = 980, effectifArtillerie = 7, ID = 123, iNation = 0, nom = "5 Leg: Lorge" });
                unitesBataille.Add(new UniteBataille() { iTour = 52, iZone = 4, tipe = TIPEUNITEBATAILLE.INFANTERIE, effectifInfanterie = 11500, effectifCavalerie = 200, effectifArtillerie = 14, ID = 123, iNation = 1, nom = "7B: Horn" });

                rolesBataille.Add(new RoleBataille() { nomLeader012 = "Napoléon", nomLeader345=string.Empty, iNation012=0, iNation345=1, iTour=50});
                rolesBataille.Add(new RoleBataille() { nomLeader012 = "Napoléon", nomLeader345 = string.Empty, iNation012 = 0, iNation345 = 1, iTour = 51 });
                rolesBataille.Add(new RoleBataille() { nomLeader012 = "Napoléon", nomLeader345 = string.Empty, iNation012 = 0, iNation345 = 1, iTour = 52 });
                terrains[0] = TIPETERRAINBATAILLE.PLAINE;
                terrains[1] = TIPETERRAINBATAILLE.COLLINE;
                terrains[2] = TIPETERRAINBATAILLE.FORET;
                terrains[3] = TIPETERRAINBATAILLE.FORTERESSE;
                terrains[4] = TIPETERRAINBATAILLE.VILLE;
                terrains[5] = TIPETERRAINBATAILLE.VILLE;
                obstacles[0] = TIPETERRAINBATAILLE.AUCUN;
                obstacles[1] = TIPETERRAINBATAILLE.RIVIERE;
                obstacles[2] = TIPETERRAINBATAILLE.FLEUVE;

                FabricantDeFilmDeBataille film = new FabricantDeFilmDeBataille();
                //tout est fait dans initialisation
                string erreur = film.Initialisation(this.S_NOM,
                    "C:\\Users\\Armand.BERGER\\Downloads",
                    new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular | FontStyle.Bold),
                    1600,1200,
                    unitesBataille,rolesBataille,zonesBataille,terrains,obstacles,
                    orientation, fin,
                    2,//nbetapes
                    50 //debut
                    );                */
                #endregion
                int positionPoint = nomFichierPartie.LastIndexOf("\\");
                string repertoire = nomFichierPartie[..positionPoint];

                string nomFichierFinal = (string.Empty == nomFichier) ? Donnees.m_donnees.TAB_PARTIE[0].S_NOM.Replace(" ", "") + "_Bataille" + this.ID_BATAILLE : nomFichier;
                string nomRepertoireFinal = (string.Empty == nomFichier) ? repertoire + "\\batailles" : repertoireVideo;
                FabricantDeFilmDeBataille film = new();
                //tout est fait dans initialisation
                string erreur = film.Initialisation(
                    Donnees.m_donnees.TAB_JEU[0].S_NOM.Replace(" ", ""),
                    nomFichierFinal,
                    nomRepertoireFinal,
                    this.S_NOM,
                    new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular | FontStyle.Bold),
                    new Font(FontFamily.GenericSansSerif, 50, FontStyle.Regular | FontStyle.Bold),//policeTitre
                    new Font(FontFamily.GenericSansSerif, 20, FontStyle.Regular | FontStyle.Bold),//policeTitreEffectifs
                    largeurFilm, hauteurFilm, ref positionFilm,
                    iNation012, iNation345,
                    unitesBataille,
                    rolesBataille,
                    zonesBataille,
                    terrains,
                    obstacles,
                    orientation, 
                    fin,
                    this.I_TOUR_FIN - this.I_TOUR_DEBUT,//nbetapes
                    this.I_TOUR_DEBUT, //debut
                    this.I_PHASE_DEBUT,
                    genererVideo
                    );
                if (erreur !=string.Empty)
                {
                    LogFile.Notifier("Erreur dans Bataille-GenererFilm :" + this.ID_BATAILLE + " : " + erreur);
                    return erreur;
                }
                return string.Empty;
            }

            private static TIPEUNITEBATAILLE DefinitionTipeUniteBataille(TAB_BATAILLE_PIONS_VIDEORow ligneBataillePionsVideo)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneBataillePionsVideo.ID_PION);
                if (lignePion.estQG)
                {
                    return TIPEUNITEBATAILLE.QG;
                }
                TIPEUNITEBATAILLE retour; //= TIPEUNITEBATAILLE.AUTRE;
                if (ligneBataillePionsVideo.I_INFANTERIE > 0)
                {
                    retour = TIPEUNITEBATAILLE.INFANTERIE;
                }
                else
                {
                    if (ligneBataillePionsVideo.I_CAVALERIE > 0)
                    {
                        retour = TIPEUNITEBATAILLE.CAVALERIE;
                    }
                    else
                    {
                        if (ligneBataillePionsVideo.I_ARTILLERIE > 0)
                        {
                            retour = TIPEUNITEBATAILLE.ARTILLERIE;
                        }
                        else
                        {
                            //unite entièrement détruite, on se base sur les effectifs initiaux
                            if (lignePion.I_INFANTERIE_INITIALE > 0)
                            {
                                retour = TIPEUNITEBATAILLE.INFANTERIE;
                            }
                            else
                            {
                                if (lignePion.I_CAVALERIE_INITIALE > 0)
                                {
                                    retour = TIPEUNITEBATAILLE.CAVALERIE;
                                }
                                else
                                {
                                    if (lignePion.I_ARTILLERIE_INITIALE > 0)
                                    {
                                        retour = TIPEUNITEBATAILLE.ARTILLERIE;
                                    }
                                    else
                                    {
                                        //on ne devrait pas arriver jusque là
                                        retour = TIPEUNITEBATAILLE.QG;
                                    }
                                }
                            }
                        }
                    }
                }
                return retour;
            }
            private static TIPETERRAINBATAILLE FilmTerrain(int iD_TERRAIN)
            {
                TAB_MODELE_TERRAINRow ligneTerrain = m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(iD_TERRAIN);
                if (null != ligneTerrain)
                {
                    string nomterrain = Constantes.MinusculeSansAccents(ligneTerrain.S_NOM);
                    if (nomterrain.Contains("plaine", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return TIPETERRAINBATAILLE.PLAINE;
                    }
                    if (nomterrain.Contains("colline", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return TIPETERRAINBATAILLE.COLLINE;
                    }
                    if (nomterrain.Contains("foret", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return TIPETERRAINBATAILLE.FORET;
                    }
                    if (nomterrain.Contains("forteresse", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return TIPETERRAINBATAILLE.FORTERESSE;
                    }
                    if (nomterrain.Contains("ville", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return TIPETERRAINBATAILLE.VILLE;
                    }
                    if (nomterrain.Contains("riviere", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return TIPETERRAINBATAILLE.RIVIERE;
                    }
                    if (nomterrain.Contains("fleuve", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return TIPETERRAINBATAILLE.FLEUVE;
                    }
                }
                return TIPETERRAINBATAILLE.AUCUN;
            }

            public bool AjouterDonneesVideo(int tour, Donnees.TAB_BATAILLE_VIDEODataTable tableBatailleVideo, Donnees.TAB_BATAILLE_PIONS_VIDEODataTable tableBataillePionsVideo)
            {
                TAB_BATAILLE_VIDEORow ligneBatailleVideo = tableBatailleVideo.AddTAB_BATAILLE_VIDEORow(
                    this.ID_BATAILLE,
                    tour,
                    this.ID_LEADER_012, this.ID_LEADER_345,
                    this.I_ENGAGEMENT_0, this.I_ENGAGEMENT_1, this.I_ENGAGEMENT_2, this.I_ENGAGEMENT_3, this.I_ENGAGEMENT_4, this.I_ENGAGEMENT_5,
                    this.S_COMBAT_0, this.S_COMBAT_1, this.S_COMBAT_2, this.S_COMBAT_3, this.S_COMBAT_4, this.S_COMBAT_5,
                    this.I_PERTES_0, this.I_PERTES_1, this.I_PERTES_2, this.I_PERTES_3, this.I_PERTES_4, this.I_PERTES_5, this.S_FIN);
                if (null== ligneBatailleVideo)
                {
                    LogFile.Notifier("Erreur dans Bataille-AjouterDonneesVideo :" + this.ID_BATAILLE + " : AddTAB_BATAILLE_VIDEORow renvoie NULL");
                    return false;
                }
                TAB_BATAILLE_PIONSRow[] listeBataillePions =
                    (TAB_BATAILLE_PIONSRow[])Donnees.m_donnees.TAB_BATAILLE_PIONS.Select(string.Format("ID_BATAILLE={0}", this.ID_BATAILLE));

                foreach (TAB_BATAILLE_PIONSRow ligneBataillePion in listeBataillePions)
                {
                    TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneBataillePion.ID_PION);
                    if (null == lignePion)
                    {
                        LogFile.Notifier("Erreur dans Bataille-AjouterDonneesVideo :" + this.ID_BATAILLE + " : impossible de trouver le pion ID_PION="+ ligneBataillePion.ID_PION);
                        return false;
                    }
                    tableBataillePionsVideo.AddTAB_BATAILLE_PIONS_VIDEORow(
                        this.ID_BATAILLE,
                        tour,
                        ligneBataillePion.ID_PION,
                        lignePion.ID_PION_PROPRIETAIRE,
                        lignePion.S_NOM,
                        lignePion.idNation,
                        ligneBataillePion.B_ENGAGEE,
                        ligneBataillePion.B_EN_DEFENSE,
                        lignePion.infanterie,
                        lignePion.cavalerie,
                        lignePion.artillerie,
                        lignePion.I_MORAL,
                        lignePion.I_FATIGUE,
                        ligneBataillePion.B_RETRAITE,
                        ligneBataillePion.B_ENGAGEMENT,
                        ligneBataillePion.I_ZONE_BATAILLE_ENGAGEMENT);
                    if (null == lignePion)
                    {
                        LogFile.Notifier("Erreur dans Bataille-AjouterDonneesVideo :" + this.ID_BATAILLE + " : AddTAB_BATAILLE_PIONS_VIDEORow renvoie NULL sur ID_PION=" + ligneBataillePion.ID_PION);
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// pour l'affichage dans la liste déroulante des vidéos de bataille
            /// </summary>
            /// <returns>chaine à afficher dans la liste déroulante</returns>
            public override string ToString()
            {
                string iTourFin = IsI_TOUR_FINNull() ? "en cours" : I_TOUR_FIN.ToString();
                return ID_BATAILLE + " - " + S_NOM + " Fin :" + iTourFin;
            }
        }
    }
}
