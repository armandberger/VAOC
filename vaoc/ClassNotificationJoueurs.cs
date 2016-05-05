using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WaocLib;

namespace vaoc
{
    public class ClassNotificationJoueurs
    {
        private string m_fichierCourant;

        public ClassNotificationJoueurs(string fichierCourant)
        {
            LogFile.CreationLogFile(fichierCourant, "courriel", Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE);
            m_fichierCourant = fichierCourant;
        }

        public bool NotificationRole(Donnees.TAB_ROLERow ligneRole, int iDernierNotification, out string titre, out StringBuilder texte)
        {
            Donnees.TAB_MESSAGERow[] ligneMessageResultat;
            string nomZoneGeographique;
            string requete;
            string message, messageErreur;
            texte = new StringBuilder();

            titre = string.Format("VAOC : {0} {1} - Messages pour {2} le {3}",
                Donnees.m_donnees.TAB_JEU[0].S_NOM,
                Donnees.m_donnees.TAB_PARTIE[0].S_NOM,
                ligneRole.S_NOM,
                ClassMessager.DateHeure(false)
                );

            # region en tête et style
            texte.Remove(0, texte.Length);
            texte.AppendLine("<html>");
            texte.AppendLine("<style type=\"text/css\">");
            texte.AppendLine(".nombre td {text-align: right;}");
            texte.AppendLine("</style>");
            #endregion

            //indique la position actuelle
            Donnees.TAB_PIONRow lignePionRole = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneRole.ID_PION);
            if (null == lignePionRole)
            {
                //il s'agit probablement d'un renfort
                Donnees.TAB_RENFORTRow ligneRenfortRole = Donnees.m_donnees.TAB_RENFORT.FindByID_PION(ligneRole.ID_PION);
                if (null == ligneRenfortRole)
                {
                    //ça c'est bizarre !
                    message = string.Format("NotificationJoueurs : on ne trouve ni le pion, ni le renfort pour ID_PION={0}", ligneRole.ID_PION);
                    LogFile.Notifier(message, out messageErreur);
                    return false;
                }
                texte.AppendLine("Vous n'êtes pas encore sur le théatre d'opérations<br/>");
                ClassMessager.CaseVersZoneGeographique(ligneRenfortRole.ID_CASE, out nomZoneGeographique);
                texte.AppendLine(string.Format("Votre arrivée est prévue le {0} à {1}.<br/>",
                    ClassMessager.DateHeure(ligneRenfortRole.I_TOUR_ARRIVEE, 2, false), nomZoneGeographique));
            }
            else
            {
                ClassMessager.CaseVersZoneGeographique(lignePionRole.ID_CASE, out nomZoneGeographique);
                string sOrdreCourant = DescriptifOrdreEnCours(lignePionRole, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR, Donnees.m_donnees.TAB_PARTIE[0].I_PHASE);
                texte.AppendLine(string.Format("Vous êtes actuellement à {0} avec l'ordre suivant : {1}<br/><br/>", nomZoneGeographique, sOrdreCourant));
            }

            //Envoie de tous les messages reçus durant la dernière heure
            if (Donnees.m_donnees.TAB_PARTIE[0].FL_DEMARRAGE)
            {
                requete = string.Format("ID_PION_PROPRIETAIRE={0} AND I_TOUR_ARRIVEE<={1}  AND I_TOUR_ARRIVEE>={2}",
                    ligneRole.ID_PION, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR - 1, iDernierNotification);
            }
            else
            {
                requete = string.Format("ID_PION_PROPRIETAIRE={0} AND I_TOUR_ARRIVEE={1} AND ID_PION_PROPRIETAIRE<>ID_PION_EMETTEUR",
                    ligneRole.ID_PION, Donnees.m_donnees.TAB_PARTIE[0].I_TOUR - 1);
            }
            ligneMessageResultat = (Donnees.TAB_MESSAGERow[])Donnees.m_donnees.TAB_MESSAGE.Select(requete, "I_PHASE_ARRIVEE");
            if (0 == ligneMessageResultat.Length)
            {
                texte.AppendLine(Donnees.m_donnees.TAB_PHRASE.DonneUnePhrase(ClassMessager.MESSAGES.MESSAGE_AUCUN_MESSAGE));
            }
            else
            {
                foreach (Donnees.TAB_MESSAGERow ligneMessage in ligneMessageResultat)
                {
                    ClassMessager.CaseVersZoneGeographique(ligneMessage.ID_CASE_FIN, out nomZoneGeographique);
                    Donnees.TAB_PIONRow lignePionEmetteur = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneMessage.ID_PION_EMETTEUR);
                    texte.AppendLine(string.Format("<div>Message reçu par le {0}, parti de {1} le {2}:<br/>",
                        ClassMessager.DateHeure(ligneMessage.I_TOUR_ARRIVEE, ligneMessage.I_PHASE_ARRIVEE, false),
                        nomZoneGeographique,
                        ClassMessager.DateHeure(ligneMessage.I_TOUR_DEPART, ligneMessage.I_PHASE_DEPART, false)));
                    texte.AppendLine("\"" + ligneMessage.S_TEXTE + "\"<br/>" + lignePionEmetteur.S_NOM + "</div><br/>");
                }
            }

            //Envoie d'un état des lieux de toutes les unités
            if (null != lignePionRole)
            {
                texte.AppendLine("<div><b>Unités sous votre commandement :</b></div>");
                texte.AppendLine(string.Format("<table border=\"0\" cellspacing=\"0\" cellpadding=\"5\"><tr bgcolor=\"DarkGrey\"><th>{0}</th><th>{1}</th><th>{2}</th><th>{3}</th><th>{4}</th><th>{5}</th><th>{6}</th><th>{7}</th><th>{8}</th><th>{9}</th><th>{10}</th><th>{11}</th><th>{12}</th></tr>",
                    "Nom", "Inf", "Cav", "Art", "Fatigue", "Moral", "Matériel", "Ravitaillement", "# Marche Jour/Nuit", "# Combat", "Position", "Ordre", "Date de situation"));
                requete = string.Format("ID_PION_PROPRIETAIRE={0}", ligneRole.ID_PION);
                Donnees.TAB_PIONRow[] lignePionResultat = (Donnees.TAB_PIONRow[])Donnees.m_donnees.TAB_PION.Select(requete);
                int numLigne = 0;
                foreach (Donnees.TAB_PIONRow lignePion in lignePionResultat)
                {
                    //on ne donne pas la liste des messagers et des patrouilles
                    if (lignePion.estMessager || lignePion.estPatrouille) continue;

                    string nom, format;
                    int iInfanterie, iCavalerie, iArtillerie, iInfanterieMax, iCavalerieMax, iArtillerieMax, iFatigue, iMoral, iMoralMax, iMateriel, iRavitaillement;
                    string sNbPhasesMarcheJour, sNbPhasesMarcheNuit, sNbHeuresCombat, sDateDernierMessage, sOrdreCourant;

                    numLigne++;
                    nom = lignePion.S_NOM;
                    iMoral = lignePion.Moral;
                    iMoralMax = lignePion.I_MORAL_MAX;
                    iMateriel = lignePion.I_MATERIEL;
                    iRavitaillement = lignePion.I_RAVITAILLEMENT;
                    iInfanterieMax = lignePion.I_INFANTERIE_INITIALE;
                    iCavalerieMax = lignePion.I_CAVALERIE_INITIALE;
                    iArtillerieMax = lignePion.I_ARTILLERIE_INITIALE;
                    sNbPhasesMarcheJour = "N/A";
                    sNbPhasesMarcheNuit = "N/A";
                    sNbHeuresCombat = "N/A";
                    
                    //on recherche le dernier message reçu et émis concernant cette unité, s'il n'y
                    //en a pas, on donne la position réelle
                    //requete = string.Format("ID_PION_PROPRIETAIRE={0} AND ID_PION_EMETTEUR={1}", ligneRole.ID_PION, lignePion.ID_PION);
                    //ligneMessageResultat = (Donnees.TAB_MESSAGERow[])Donnees.m_donnees.TAB_MESSAGE.Select(requete, "I_TOUR_ARRIVEE, I_PHASE_ARRIVEE");
                    Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageRecu(lignePion.ID_PION, ligneRole.ID_PION);
                    if (null == ligneMessage)
                    {
                        /* Normalement ne doit jamais arrivé sauf pour une unités de blessés/prisonniers alors que l'on est pas sur le terrain et, auquel cas, l'unité ne doit pas être visible 
                        ClassMessager.CaseVersZoneGeographique(lignePion.ID_CASE, out nomZoneGeographique);
                        iInfanterie = lignePion.I_INFANTERIE;
                        iCavalerie = lignePion.I_CAVALERIE;
                        iArtillerie = lignePion.I_ARTILLERIE;
                        iFatigue = lignePion.I_FATIGUE;
                        if (!lignePion.IsI_NB_PHASES_MARCHE_JOURNull())
                        { 
                            sNbPhasesMarcheJour = Math.Ceiling((decimal)lignePion.I_NB_PHASES_MARCHE_JOUR / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES).ToString(); 
                        }
                        if (!lignePion.IsI_NB_PHASES_MARCHE_NUITNull())
                        {
                            sNbPhasesMarcheNuit = Math.Ceiling((decimal)lignePion.I_NB_PHASES_MARCHE_NUIT / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES).ToString();
                        }
                        if (!lignePion.IsI_NB_HEURES_COMBATNull())
                        {
                            sNbHeuresCombat = lignePion.I_NB_HEURES_COMBAT.ToString();
                        }
                        sDateDernierMessage = "N/A";
                        sOrdreCourant = "N/A";
                         * */
                        continue;
                    }
                    else
                    {
                        //le dernier message est en fait le plus récent
                        ClassMessager.CaseVersZoneGeographique(ligneMessage.ID_CASE_FIN, out nomZoneGeographique);
                        iInfanterie = ligneMessage.I_INFANTERIE;
                        iCavalerie = ligneMessage.I_CAVALERIE;
                        iArtillerie = ligneMessage.I_ARTILLERIE;
                        iFatigue = ligneMessage.I_FATIGUE;
                        if (!ligneMessage.IsI_NB_PHASES_MARCHE_JOURNull())
                        {
                            sNbPhasesMarcheJour = Math.Ceiling((decimal)ligneMessage.I_NB_PHASES_MARCHE_JOUR / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES).ToString();
                        }
                        if (!ligneMessage.IsI_NB_PHASES_MARCHE_NUITNull())
                        {
                            sNbPhasesMarcheNuit = Math.Ceiling((decimal)ligneMessage.I_NB_PHASES_MARCHE_NUIT / Donnees.m_donnees.TAB_JEU[0].I_NOMBRE_PHASES).ToString();
                        }
                        if (!ligneMessage.IsI_NB_HEURES_COMBATNull())
                        {
                            sNbHeuresCombat = ligneMessage.I_NB_HEURES_COMBAT.ToString();
                        }
                        sDateDernierMessage = ClassMessager.DateHeure(ligneMessage.I_TOUR_DEPART, ligneMessage.I_PHASE_DEPART, false);

                        sOrdreCourant = DescriptifOrdreEnCours(lignePion, ligneMessage.I_TOUR_DEPART, ligneMessage.I_PHASE_DEPART);
                    }
                    string libelleFatigue, libelleMoral, /*libelleMoralMax, */LibelleMateriel, LibelleRavitaillement;
                    if (lignePion.estQG || lignePion.estConvoi)
                    {
                        libelleFatigue = "N/A";
                        libelleMoral = "N/A ";
                        LibelleMateriel = "N/A";
                        LibelleRavitaillement = "N/A";
                    }
                    else
                    {
                        libelleFatigue = iFatigue.ToString();
                        if (lignePion.estUniteArtillerie)
                        {
                            //Les unités d'artillerie n'ont pas de matériel ou de ravitaillement
                            libelleMoral = "N/A ";
                            LibelleMateriel = "N/A";
                            LibelleRavitaillement = "N/A";
                        }
                        else
                        {
                            //libelleFatigue = Constantes.LibelleFatigue(iFatigue);
                            //libelleMoral = Constantes.LibelleMoral(iMoral);
                            //libelleMoralMax = Constantes.LibelleMoral(iMoralMax);
                            //LibelleMateriel = Constantes.LibelleMaterielRavitaillement(iMateriel);
                            //LibelleRavitaillement = Constantes.LibelleMaterielRavitaillement(iRavitaillement);
                            libelleMoral = iMoral.ToString() + "/" + iMoralMax.ToString();
                            LibelleMateriel = iMateriel.ToString();
                            LibelleRavitaillement = iRavitaillement.ToString();
                        }
                    }

                    //string test1 = string.Format("{0,50}",
                    //texte.AppendLine(string.Format("<tr><td>{0,50}</td><td style=\"nombre\">{1,5:D}</td><td style=\"nombre\">{2,5:D}</td><td style=\"nombre\">{3,5:D}</td><td style=\"nombre\">{4,3:D}</td><td style=\"nombre\">{5,7:D}</td><td style=\"nombre\">{6,8:D}</td><td>{7}</td></tr>",
                    format = (0 == numLigne % 3) ? " bgcolor=\"LightGrey\"" : "";
                    texte.AppendLine(string.Format("<tr {8}><td>{0}</td><td align=\"right\" style=\"nombre\">{1:D}/{9:D}</td><td align=\"right\" style=\"nombre\">{2:D}/{10:D}</td>"+
                                                    "<td align=\"right\" style=\"nombre\">{3:D}/{11:D}</td>"+
                                                    "<td align=\"right\">{4}</td><td align=\"right\" >{5}</td>"+
                                                    "<td align=\"right\">{17}</td><td align=\"right\">{18}</td>" +
                                                    "<td align=\"right\" style=\"nombre\">{12:D}/{13:D}</td><td align=\"right\" style=\"nombre\">{14}</td><td>{7}</td><td>{15}</td><td>{16}</td></tr>",
                        nom,
                        iInfanterie,
                        iCavalerie,
                        iArtillerie,
                        libelleFatigue,
                        libelleMoral,//5
                        "",//libelleMoralMax,->juste pour pas tout renuméroter, glandeur va !
                        nomZoneGeographique,
                        format,
                        iInfanterieMax,
                        iCavalerieMax,//10
                        iArtillerieMax,
                        sNbPhasesMarcheJour,
                        sNbPhasesMarcheNuit,
                        sNbHeuresCombat,
                        sOrdreCourant,//15
                        sDateDernierMessage,
                        LibelleMateriel,
                        LibelleRavitaillement
                        ));
                }
                texte.AppendLine("</table><br/>");
            }

            //Envoie d'un éventuel message de l'arbitre
            if (Donnees.m_donnees.TAB_PARTIE[0].S_MESSAGE_ARBITRE.Length > 0)
            {
                texte.AppendLine("<div><b>Message de l'arbitre :</b>");
                texte.AppendLine(Donnees.m_donnees.TAB_PARTIE[0].S_MESSAGE_ARBITRE);
                texte.AppendLine("</div><br/>");
            }

            //Rappel de l'adresse du jeu
            texte.AppendLine("<div><b>" + ligneRole.S_NOM + "</b> vos troupes attendent vos ordres au <a href=\"http://vaoc.free.fr/\">camp de rassemblement.</a></div>");

            //Si le joueur ne souhaite donner aucun ordre supplémentaire
            texte.AppendLine("<div><b>" + ligneRole.S_NOM + "</b> prevenez vos aides de camp que vous n'avez <a href=\"http://vaoc.free.fr/vaocpasdenouveauxordres.php?id_partie=" + Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE + "&id_role=" + ligneRole.ID_ROLE + "\">aucun nouvel ordre à donner.</a></div>");

            texte.AppendLine("</html>");
            return true;
        }

        private static string DescriptifOrdreEnCours(Donnees.TAB_PIONRow lignePion, int tour, int phase)
        {
            string sOrdreCourant, requete;
            //recherche de l'ordre courant de l'unité au moment du dernier message
            //requete = string.Format("ID_PION={0} AND I_TOUR_DEBUT<={1} AND I_PHASE_DEBUT<={2} AND ((I_TOUR_FIN IS NULL AND I_PHASE_FIN IS NULL) OR (I_TOUR_FIN>{1} AND I_PHASE_FIN>{2}))",
            //    lignePion.ID_PION, ligneMessage.I_TOUR_DEPART, ligneMessage.I_PHASE_DEPART);
            //string requete = string.Format("ID_PION={0} AND I_TOUR_DEBUT<={1} AND ((I_TOUR_FIN IS NULL) OR (I_TOUR_FIN>{1}))", lignePion.ID_PION, tour);
            requete = string.Format("ID_PION={0} AND I_TOUR_DEBUT<={1} AND ((I_TOUR_FIN IS NULL) OR (I_TOUR_FIN>{1}) OR (I_TOUR_FIN={1} AND I_PHASE_FIN>{2}))",
                                         lignePion.ID_PION, tour, phase);
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
        /// Génération d'un courriel de notification pour tous les joueurs
        /// </summary>
        /// <returns>true si OK, false si KO</returns>
        public bool NotificationJoueurs()
        {
            StringBuilder texte;
            string titre, adresseCourriel;
            string requete;
            string message, messageErreur;
            Donnees.TAB_ROLERow[] ligneRoleResultat;
            CourrielService serviceCourriel;

            serviceCourriel = new CourrielService(Donnees.m_donnees.TAB_PARTIE[0].S_HOST_COURRIEL,
                Donnees.m_donnees.TAB_PARTIE[0].S_HOST_UTILISATEUR,
                Donnees.m_donnees.TAB_PARTIE[0].S_HOST_MOTDEPASSE);

            InterfaceVaocWeb iWeb = ClassVaocWebFactory.CreerVaocWeb(m_fichierCourant, false);
            foreach (ClassDataUtilisateur utilisateur in iWeb.ListeUtilisateurs(true))
            {
                requete = string.Format("ID_UTILISATEUR={0}", utilisateur.ID_UTILISATEUR);
                ligneRoleResultat = (Donnees.TAB_ROLERow[])Donnees.m_donnees.TAB_ROLE.Select(requete);
                if (0 == ligneRoleResultat.Length)
                {
                    message = string.Format("NotificationJoueurs : ligneRoleResultat est vide pour ID_UTILISATEUR={0}", utilisateur.ID_UTILISATEUR);
                    LogFile.Notifier(message, out messageErreur);
                    continue;//c'est juste un utilisateur qui ne joue pas cette partie
                }
                adresseCourriel = utilisateur.S_COURRIEL;

                foreach (Donnees.TAB_ROLERow ligneRole in ligneRoleResultat)
                {
                    if (!NotificationRole(ligneRole, (Donnees.m_donnees.TAB_PARTIE[0].IsI_TOUR_NOTIFICATIONNull()) ? 0 : Donnees.m_donnees.TAB_PARTIE[0].I_TOUR_NOTIFICATION, out titre, out texte))
                    {
                        return false;
                    }
                    message = string.Format("NotificationJoueurs : envoie d'un message à {0} titre:{1} message={2}",
                        adresseCourriel, titre, texte.ToString());
                    LogFile.Notifier(message, out messageErreur);

                    serviceCourriel.EnvoyerMessage(adresseCourriel, titre, texte.ToString());
                }
            }
            return true;
        }

    }
}
