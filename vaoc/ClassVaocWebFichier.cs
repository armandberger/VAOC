using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using WaocLib;
using System.Diagnostics;
using System.Globalization;

namespace vaoc
{
    class ClassVaocWebFichier : InterfaceVaocWeb
    {
        private string m_fileNameSQL;
        private string m_fileNameXML;

        #region InterfaceVaocWeb Members
        public ClassVaocWebFichier(string connexion, bool nouveauFichier)
        {
            int positionPoint = connexion.LastIndexOf(".");
            //recopie de la chaine avant l'extension
            string nomFichier = connexion.Substring(0, positionPoint);

            int tourfichier = -1;//indication du tour courant dans le nom du fichier
            int i = nomFichier.Length - 1;
            while (char.IsDigit(nomFichier[i])) i--;
            //string test = nomfichierTourPhase.Substring(i + 1, nomfichierTourPhase.Length - i - 1);
            if (i != nomFichier.Length - 1) tourfichier = Convert.ToInt32(nomFichier.Substring(i + 1, nomFichier.Length - i - 1));
            if (tourfichier > 0 && Donnees.m_donnees.TAB_PARTIE[0].I_TOUR > tourfichier)
            {
                m_fileNameSQL = string.Format("{0}{1}.sql",
                        nomFichier.Substring(0, i + 1),
                        Donnees.m_donnees.TAB_PARTIE[0].I_TOUR);
            }
            else
            {
                m_fileNameSQL = nomFichier + ".sql";
            }

            m_fileNameXML = nomFichier + ".xml";

            if (nouveauFichier && File.Exists(m_fileNameSQL))
            {
                File.Delete(m_fileNameSQL);
                File.Create(m_fileNameSQL).Close();
            }
        }


        #region lecture des données
        public List<ClassDataRole> ListeRoles(int idPartie)
        {
            string xpath;
            List<ClassDataRole> listeRoles = new List<ClassDataRole>();
            XmlDocument xDoc = new XmlDocument();
            //xDoc.Load(m_fileNameXML);
            xDoc.Load(LecteurXML.LireFichier(m_fileNameXML));

            if (idPartie >= 0)
            {
                xpath = string.Format("/vaoc/tab_vaoc_role[ID_PARTIE={0}]", idPartie);
            }
            else
            {
                xpath = "/vaoc/tab_vaoc_role";
            }

            foreach (XmlNode noeud in xDoc.SelectNodes(xpath))
            {
                ClassDataRole role = new ClassDataRole();
                role.ID_ROLE = Convert.ToInt32(noeud["ID_ROLE"].InnerText);
                role.ID_UTILISATEUR = Convert.ToInt32(noeud["ID_UTILISATEUR"].InnerText);
                role.ID_PARTIE = Convert.ToInt32(noeud["ID_PARTIE"].InnerText);
                role.S_NOM = noeud["S_NOM"].InnerText;
                listeRoles.Add(role);
                role.ID_PION = Convert.ToInt32(noeud["ID_PION"].InnerText);
                role.ID_NATION = Convert.ToInt32(noeud["ID_NATION"].InnerText);
            }
            return listeRoles;
        }

        /// <summary>
        /// Renvoie la liste des utilisateurs disponibles dans la base de données
        /// </summary>
        /// <param name="bPresent">true si l'on ne souhaite que les utilisateurs qui sont présents dans cette partie, false si on les veut tous</param>
        /// <returns>La liste des utilisateurs</returns>
        public List<ClassDataUtilisateur> ListeUtilisateurs(bool bPresent)
        {
            string xpath;
            List<ClassDataUtilisateur> listeUtilisateurs = new List<ClassDataUtilisateur>();
            XmlDocument xDoc = new XmlDocument();
            //xDoc.Load(m_fileNameXML);
            xDoc.Load(LecteurXML.LireFichier(m_fileNameXML));
            xpath = "/vaoc/tab_utilisateurs";

            foreach (XmlNode noeud in xDoc.SelectNodes(xpath))
            {
                ClassDataUtilisateur utilisateur = AffecterUtilisateur(noeud);
                if (!bPresent || (bPresent && utilisateur.I_ONR >= 0))
                {
                    listeUtilisateurs.Add(utilisateur);
                }
            }
            return listeUtilisateurs;
        }

        private ClassDataUtilisateur AffecterUtilisateur(XmlNode noeud)
        {
            ClassDataUtilisateur utilisateur = new ClassDataUtilisateur();
            utilisateur.ID_UTILISATEUR = Convert.ToInt32(noeud["ID_UTILISATEUR"].InnerText);
            utilisateur.DT_CREATION = Convert.ToDateTime(noeud["DT_CREATION"].InnerText);
            utilisateur.DT_DERNIERECONNEXION = Convert.ToDateTime(noeud["DT_DERNIERECONNEXION"].InnerText);
            utilisateur.S_LOGIN = noeud["S_LOGIN"].InnerText;
            utilisateur.S_COURRIEL = noeud["S_COURRIEL"].InnerText;
            utilisateur.S_NOM = noeud["S_NOM"].InnerText;
            utilisateur.S_PRENOM = noeud["S_PRENOM"].InnerText;
            utilisateur.I_ONR = CalculNombreDeToursSansOrdre(utilisateur.ID_UTILISATEUR);
            return utilisateur;
        }

        private int CalculNombreDeToursSansOrdre(int id_utilisateur)
        {
            Donnees.TAB_ROLERow[] lignesRole = (Donnees.TAB_ROLERow[])Donnees.m_donnees.TAB_ROLE.Select("ID_UTILISATEUR = " + id_utilisateur.ToString());
            if (0 == lignesRole.Count())
            {
                //le joueur n'a pas de pion sur le terrain
                return -1;
            }
            //recherche de tous les pions controlés par le joueur, sachant qu'il peut avoir plusieurs rôles !
            int i_onr = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;//le joueur n'a jamais donné d'ordres, valeur par defaut

            List<ClassDataOrdre> listeOrdresXML = ListeOrdres(Donnees.m_donnees.TAB_PARTIE[0].ID_JEU,
                                                            Donnees.m_donnees.TAB_PARTIE[0].ID_PARTIE);

            foreach (Donnees.TAB_ROLERow ligneRole in lignesRole)
            {
                Donnees.TAB_PIONRow[] lignesPion = (Donnees.TAB_PIONRow[])Donnees.m_donnees.TAB_PION.Select(string.Format("ID_PION = {0} OR ID_PION_PROPRIETAIRE = {0}", ligneRole.ID_PION));
                foreach (Donnees.TAB_PIONRow lignePion in lignesPion)
                {
                    if (lignePion.estMessager || lignePion.estPatrouille || 
                        (lignePion.estJoueur && lignePion.ID_PION!=ligneRole.ID_PION)) { continue; }

                    //on recherche le dernier ordre
                    foreach (ClassDataOrdre xmlOrdre in listeOrdresXML)
                    {
                        if (xmlOrdre.ID_PION== lignePion.ID_PION && (Donnees.m_donnees.TAB_PARTIE[0].I_TOUR - xmlOrdre.I_TOUR < i_onr))
                        {
                            i_onr = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR - xmlOrdre.I_TOUR;
                        }
                    }
                }
            }
            return i_onr;
            /*
                        string requete = string.Empty;

                        foreach (Donnees.TAB_ROLERow ligneRole in lignesRole)
                        {
                            Donnees.TAB_PIONRow[] lignesPion = (Donnees.TAB_PIONRow[])Donnees.m_donnees.TAB_PION.Select(string.Format("ID_PION = {0} OR ID_PION_PROPRIETAIRE = {0}", ligneRole.ID_PION));
                            foreach (Donnees.TAB_PIONRow lignePion in lignesPion)
                            {
                                if (lignePion.estMessager || lignePion.estPatrouille) { continue; }

                                if (requete.Length > 0) requete += " OR ";
                                requete += " (ID_DESTINATAIRE = " + lignePion.ID_PION + " ) ";
                            }
                        }
                        Donnees.TAB_ORDRERow[] ligneOrdre = (Donnees.TAB_ORDRERow[])Donnees.m_donnees.TAB_ORDRE.Select(requete, "I_TOUR_DEBUT DESC");

                        if (0 == ligneOrdre.Count())
                        {
                            return Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;//le joueur n'a jamais donné d'ordres
                        }
                        return Donnees.m_donnees.TAB_PARTIE[0].I_TOUR - ligneOrdre[0].I_TOUR_DEBUT;
                        */
        }

        public ClassDataUtilisateur GetUtilisateur(int id_utilisateur)
        {
            string xpath;
            XmlDocument xDoc = new XmlDocument();
            //xDoc.Load(m_fileNameXML);
            xDoc.Load(LecteurXML.LireFichier(m_fileNameXML));
            xpath = string.Format("/vaoc/tab_utilisateurs[ID_UTILISATEUR={0}]", id_utilisateur);

            XmlNode noeud = xDoc.SelectSingleNode(xpath);
            if (null == noeud)
            {
                return null;
            }
            ClassDataUtilisateur utilisateur = AffecterUtilisateur(noeud);
            return utilisateur;
        }

        public ClassDataUtilisateur GetUtilisateur(string s_login)
        {
            string xpath;
            XmlDocument xDoc = new XmlDocument();
            //xDoc.Load(m_fileNameXML);
            xDoc.Load(LecteurXML.LireFichier(m_fileNameXML));
            xpath = string.Format("/vaoc/tab_utilisateurs[S_LOGIN='{0}']", s_login);

            XmlNode noeud = xDoc.SelectSingleNode(xpath);
            if (null == noeud)
            {
                return null;
            }
            ClassDataUtilisateur utilisateur = AffecterUtilisateur(noeud);
            return utilisateur;
        }

        public List<ClassDataJeu> ListeJeux()
        {
            string xpath;
            List<ClassDataJeu> listeJeux = new List<ClassDataJeu>();
            XmlDocument xDoc = new XmlDocument();
            //xDoc.Load(m_fileNameXML);
            xDoc.Load(LecteurXML.LireFichier(m_fileNameXML));
            xpath = "/vaoc/tab_vaoc_jeu";

            foreach (XmlNode noeud in xDoc.SelectNodes(xpath))
            {
                ClassDataJeu jeu = new ClassDataJeu();
                jeu.DT_INITIALE = Convert.ToDateTime(noeud["DT_INITIALE"].InnerText);
                jeu.ID_JEU = Convert.ToInt32(noeud["ID_JEU"].InnerText);
                jeu.I_NOMBRE_PHASES = Convert.ToInt32(noeud["I_NOMBRE_PHASES"].InnerText);
                jeu.I_NOMBRE_TOURS = Convert.ToInt32(noeud["I_NOMBRE_TOURS"].InnerText);
                jeu.I_HEURE_INITIALE = Convert.ToInt32(noeud["I_HEURE_INITIALE"].InnerText);                
                jeu.I_LEVER_DU_SOLEIL = Convert.ToInt32(noeud["I_LEVER_DU_SOLEIL"].InnerText);                
                jeu.I_COUCHER_DU_SOLEIL = Convert.ToInt32(noeud["I_COUCHER_DU_SOLEIL"].InnerText);                
                        
                jeu.S_NOM = noeud["S_NOM"].InnerText;
                listeJeux.Add(jeu);
            }
            return listeJeux;
        }

        public ClassDataJeu GetJeu(int idJeu)
        {
            string xpath;
            XmlDocument xDoc = new XmlDocument();
            //xDoc.Load(m_fileNameXML);
            xDoc.Load(LecteurXML.LireFichier(m_fileNameXML));
            xpath = string.Format("/vaoc/tab_vaoc_jeu[ID_JEU={0}]", idJeu);

            XmlNode noeud = xDoc.SelectSingleNode(xpath);
            if (null == noeud)
            {
                return null;
            }
            ClassDataJeu jeu = new ClassDataJeu();
            jeu.DT_INITIALE = Convert.ToDateTime(noeud["DT_INITIALE"].InnerText);
            jeu.ID_JEU = Convert.ToInt32(noeud["ID_JEU"].InnerText);
            jeu.I_NOMBRE_PHASES = Convert.ToInt32(noeud["NOMBRE_PHASES"].InnerText);
            jeu.I_NOMBRE_TOURS = Convert.ToInt32(noeud["NOMBRE_TOURS"].InnerText);
            jeu.I_HEURE_INITIALE = Convert.ToInt32(noeud["I_HEURE_INITIALE"].InnerText);
            jeu.I_LEVER_DU_SOLEIL = Convert.ToInt32(noeud["I_LEVER_DU_SOLEIL"].InnerText);
            jeu.I_COUCHER_DU_SOLEIL = Convert.ToInt32(noeud["I_COUCHER_DU_SOLEIL"].InnerText);
            jeu.S_NOM = noeud["S_NOM"].InnerText;

            return jeu;
        }

        public List<ClassDataPartie> ListeParties()
        {
            return ListeParties(-1);
        }

        private ClassDataPartie ConversionPartie(XmlNode noeud)
        {
            ClassDataPartie partie = new ClassDataPartie();
            partie.ID_PARTIE = Convert.ToInt32(noeud["ID_PARTIE"].InnerText);
            partie.ID_JEU = Convert.ToInt32(noeud["ID_JEU"].InnerText);
            partie.S_NOM = noeud["S_NOM"].InnerText;
            partie.I_TOUR = Convert.ToInt32(noeud["I_TOUR"].InnerText);
            try
            {
                partie.DT_TOUR = Convert.ToDateTime(noeud["DT_TOUR"].InnerText);
            }
            catch (System.FormatException)
            {
                partie.DT_TOUR = DateTime.Now;
            }
            partie.I_PHASE = Convert.ToInt32(noeud["I_PHASE"].InnerText);
            try
            {
                partie.DT_CREATION = Convert.ToDateTime(noeud["DT_CREATION"].InnerText);
            }
            catch (System.FormatException)
            {
                partie.DT_CREATION = DateTime.Now;
            }
            try
            {
                partie.DT_MISEAJOUR = Convert.ToDateTime(noeud["DT_MISEAJOUR"].InnerText);
            }
            catch (System.FormatException)
            {
                partie.DT_MISEAJOUR = DateTime.Now;
            }
            partie.H_JOUR = Convert.ToInt32(noeud["H_JOUR"].InnerText);
            partie.H_NUIT = Convert.ToInt32(noeud["H_NUIT"].InnerText);
            partie.S_REPERTOIRE = noeud["S_REPERTOIRE"].InnerText;
            partie.FL_MISEAJOUR = ("1"==noeud["FL_MISEAJOUR"].InnerText) ? true : false;
            partie.FL_DEMARRAGE = ("1"==noeud["FL_DEMARRAGE"].InnerText) ? true : false;
            partie.I_NB_CARTE_X = Convert.ToInt32(noeud["I_NB_CARTE_X"].InnerText);
            partie.I_NB_CARTE_Y = Convert.ToInt32(noeud["I_NB_CARTE_Y"].InnerText);
            partie.I_NB_CARTE_ZOOM_X = Convert.ToInt32(noeud["I_NB_CARTE_ZOOM_X"].InnerText);
            partie.I_NB_CARTE_ZOOM_Y = Convert.ToInt32(noeud["I_NB_CARTE_ZOOM_Y"].InnerText);
            partie.D_MULT_ZOOM_X = Convert.ToDecimal(noeud["D_MULT_ZOOM_X"].InnerText);
            partie.D_MULT_ZOOM_Y = Convert.ToDecimal(noeud["D_MULT_ZOOM_Y"].InnerText);
            partie.I_LARGEUR_CARTE_ZOOM = Convert.ToInt32(noeud["I_LARGEUR_CARTE_ZOOM"].InnerText);
            partie.I_HAUTEUR_CARTE_ZOOM = Convert.ToInt32(noeud["I_HAUTEUR_CARTE_ZOOM"].InnerText);
            partie.I_ECHELLE = Convert.ToInt32(noeud["I_ECHELLE"].InnerText);
            return partie;
        } 

        public List<ClassDataPartie> ListeParties(int idJeu, int idPartie = -1)
        {
            string xpath;
            List<ClassDataPartie> listeParties = new List<ClassDataPartie>();
            XmlDocument xDoc = new XmlDocument();
            //xDoc.Load(m_fileNameXML);
            xDoc.Load(LecteurXML.LireFichier(m_fileNameXML));
            if (idJeu >= 0)
            {
                if (idPartie >= 0)
                {
                    xpath = string.Format("/vaoc/tab_vaoc_partie[ID_JEU={0} and ID_PARTIE={1}]", idJeu, idPartie);
                }
                else
                {
                    xpath = string.Format("/vaoc/tab_vaoc_partie[ID_JEU={0}]", idJeu);
                }
            }
            else
            {
                xpath = string.Format("/vaoc/tab_vaoc_partie", idJeu);
            }

            foreach (XmlNode noeud in xDoc.SelectNodes(xpath))
            {
                ClassDataPartie partie = ConversionPartie(noeud);
                listeParties.Add(partie);
            }
            return listeParties;
        }

        public ClassDataPartie GetPartie(int idPartie)
        {
            string xpath;
            XmlDocument xDoc = new XmlDocument();
            //xDoc.Load(m_fileNameXML);
            xDoc.Load(LecteurXML.LireFichier(m_fileNameXML));
            //xpath = string.Format("/vaoc/tab_vaoc_partie[ID_JEU={0} and ID_PARTIE={1}]", idJeu, idPartie);//and ou or en minuscules absolument
            xpath = string.Format("/vaoc/tab_vaoc_partie[ID_PARTIE={0}]", idPartie);//and ou or en minuscules absolument

            XmlNode noeud = xDoc.SelectSingleNode(xpath);
            if (null == noeud)
            {
                return null;
            }
            ClassDataPartie partie = ConversionPartie(noeud);

            return partie;
        }

        public List<ClassDataMessage> ListeMessages(int idPartie)
        {
            string xpath;
            List<ClassDataMessage> listeMessages = new List<ClassDataMessage>();
            XmlDocument xDoc = new XmlDocument();
            //xDoc.Load(m_fileNameXML);
            xDoc.Load(LecteurXML.LireFichier(m_fileNameXML));
            if (idPartie >= 0)
            {
                xpath = string.Format("/vaoc/tab_vaoc_message[ID_PARTIE={0}]", idPartie);
            }
            else
            {
                xpath = "/vaoc/tab_vaoc_ordre";
            }

            foreach (XmlNode noeud in xDoc.SelectNodes(xpath))
            {
                ClassDataMessage message = new ClassDataMessage();
                message.ID_MESSAGE = Convert.ToInt32(noeud["ID_MESSAGE"].InnerText);
                message.ID_PARTIE = Convert.ToInt32(noeud["ID_PARTIE"].InnerText);
                message.ID_EMETTEUR = Convert.ToInt32(noeud["ID_EMETTEUR"].InnerText);
                message.ID_PION_PROPRIETAIRE = Convert.ToInt32(noeud["ID_PION_PROPRIETAIRE"].InnerText);
                message.DT_DEPART = Convert.ToDateTime(noeud["DT_DEPART"].InnerText);
                message.DT_ARRIVEE = Convert.ToDateTime(noeud["DT_ARRIVEE"].InnerText);
                message.S_MESSAGE = Convert.ToString(noeud["S_MESSAGE"].InnerText).Replace("\\'", "'");//remplacement des \' par de simples apostrophes 
;
                listeMessages.Add(message);
            }

            return listeMessages;
        }

        public List<ClassDataOrdre> ListeOrdres(int idJeu, int idPartie)
        {
            string xpath;
            List<ClassDataOrdre> listeOrdres = new List<ClassDataOrdre>();
            XmlDocument xDoc = new XmlDocument();
            //xDoc.Load(m_fileNameXML);
            xDoc.Load(LecteurXML.LireFichier(m_fileNameXML));
            if (idJeu >= 0 && idPartie >= 0)
            {
                xpath = string.Format("/vaoc/tab_vaoc_ordre[ID_PARTIE={0}]", idPartie);
            }
            else
            {
                xpath = "/vaoc/tab_vaoc_ordre";
            }

            //Donnees.TAB_ORDRERow ligneOrdreDebug = Donnees.m_donnees.TAB_ORDRE.FindByID_ORDRE(1067);
            foreach (XmlNode noeud in xDoc.SelectNodes(xpath))
            {
                ClassDataOrdre ordre = new ClassDataOrdre();
                ordre.ID_ORDRE = Convert.ToInt32(noeud["ID_ORDRE"].InnerText);
                ordre.ID_ORDRE_SUIVANT = null != noeud["ID_ORDRE_SUIVANT"] ? Convert.ToInt32(noeud["ID_ORDRE_SUIVANT"].InnerText) : -1;
                ordre.ID_PION = Convert.ToInt32(noeud["ID_PION"].InnerText);
                ordre.ID_PARTIE = Convert.ToInt32(noeud["ID_PARTIE"].InnerText);
                ordre.I_TOUR = Convert.ToInt32(noeud["I_TOUR"].InnerText);
                ordre.I_TYPE = Convert.ToInt32(noeud["I_TYPE"].InnerText);
                ordre.S_MESSAGE = Convert.ToString(noeud["S_MESSAGE"].InnerText).Replace("\\'", "'");//remplacement des \' par de simples apostrophes 
                ordre.I_DISTANCE = Convert.ToInt32(noeud["I_DISTANCE"].InnerText);
                ordre.I_DIRECTION = Convert.ToInt32(noeud["I_DIRECTION"].InnerText);
                ordre.ID_NOM_LIEU = Convert.ToInt32(noeud["ID_NOM_LIEU"].InnerText);
                ordre.I_HEURE = Convert.ToInt32(noeud["I_HEURE"].InnerText);
                ordre.I_DUREE = Convert.ToInt32(noeud["I_DUREE"].InnerText);
                ordre.I_DISTANCE_DESTINATAIRE = Convert.ToInt32(noeud["I_DISTANCE"].InnerText);
                ordre.I_DIRECTION_DESTINATAIRE = Convert.ToInt32(noeud["I_DIRECTION"].InnerText);
                ordre.ID_NOM_LIEU_DESTINATAIRE = Convert.ToInt32(noeud["ID_NOM_LIEU"].InnerText);
                ordre.I_ZONE_BATAILLE = Convert.ToInt32(noeud["I_ZONE_BATAILLE"].InnerText);
                ordre.ID_BATAILLE = Convert.ToInt32(noeud["ID_BATAILLE"].InnerText);
                ordre.ID_PION_CIBLE = null != noeud["ID_PION_CIBLE"] ? Convert.ToInt32(noeud["ID_PION_CIBLE"].InnerText) : -1;
                ordre.ID_PION_DESTINATAIRE_CIBLE = null != noeud["ID_PION_DESTINATAIRE_CIBLE"] ? Convert.ToInt32(noeud["ID_PION_DESTINATAIRE_CIBLE"].InnerText) : -1;
                ordre.I_ENGAGEMENT = null != noeud["I_ENGAGEMENT"] ? Convert.ToInt32(noeud["I_ENGAGEMENT"].InnerText) : -1;
                
                //l'ordre est toujours donné du responsable du pion vers le pion, sauf si c'est un QG
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ordre.ID_PION);
                if (null == lignePion) continue;//possible si j'ai remis un fichier d'ordres ultérieurs au tour, les unités peuvent ne pas avoir été crées

                switch (ordre.I_TYPE)
                {
                    case Constantes.ORDRES.MOUVEMENT:
                    case Constantes.ORDRES.MESSAGE:
                    case Constantes.ORDRES.PATROUILLE:
                    case Constantes.ORDRES.CONSTRUIRE_PONTON:
                    case Constantes.ORDRES.ENDOMMAGER_PONT:
                    case Constantes.ORDRES.REPARER_PONT:
                    case Constantes.ORDRES.ARRET:
                    case Constantes.ORDRES.TRANSFERER:
                    case Constantes.ORDRES.GENERERCONVOI:
                    case Constantes.ORDRES.RENFORCER:
                    case Constantes.ORDRES.SEFORTIFIER:
                    case Constantes.ORDRES.ETABLIRDEPOT:
                        if (lignePion.estQG)
                        {
                            if (null == noeud["ID_PION_DESTINATION"])
                            {
                                ordre.ID_PION_DESTINATAIRE = ordre.ID_PION;//possible dans le cas d'un ordre de mouvement par exemple
                            }
                            else
                            {
                                ordre.ID_PION_DESTINATAIRE = Convert.ToInt32(noeud["ID_PION_DESTINATION"].InnerText);
                            }
                        }
                        else
                        {
                            ordre.ID_PION = lignePion.ID_PION_PROPRIETAIRE;
                            ordre.ID_PION_DESTINATAIRE = lignePion.ID_PION;
                        }
                        break;
                    case Constantes.ORDRES.COMBAT:
                    case Constantes.ORDRES.RETRAITE:
                    case Constantes.ORDRES.RETRAIT:
                    case Constantes.ORDRES.ENGAGEMENT:
                        //ordre.ID_PION ne change pas
                        ordre.ID_PION_DESTINATAIRE = lignePion.ID_PION;
                        break;
                    default:
                        LogFile.Notifier("ListeOrdres Ordre inconnu reçu avec le code =" + ordre.I_TYPE);
                        return null;
                }
                /*
                if (lignePion.estQG)
                {
                    // si le QG reçoit un ordre de mouvement et a déjà un ordre de mouvement, il faut l'arrêter
                    if (ordre.I_TYPE == Constantes.ORDRES.MOUVEMENT)
                    {
                        Donnees.TAB_ORDRERow ligneOrdreMouvement = Donnees.m_donnees.TAB_ORDRE.Mouvement(lignePion.ID_PION);
                        if (null != ligneOrdreMouvement)
                        {
                            ligneOrdreMouvement.I_TOUR_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_TOUR;
                            ligneOrdreMouvement.I_PHASE_FIN = Donnees.m_donnees.TAB_PARTIE[0].I_PHASE;
                        }
                    }

                    if (null == noeud["ID_PION_DESTINATION"])
                    {
                        ordre.ID_PION_DESTINATAIRE = ordre.ID_PION;//possible dans le cas d'un ordre de mouvement par exemple
                    }
                    else
                    {
                        ordre.ID_PION_DESTINATAIRE = Convert.ToInt32(noeud["ID_PION_DESTINATION"].InnerText);
                    }
                }
                else
                {
                    ordre.ID_PION = lignePion.ID_PION_PROPRIETAIRE;
                    ordre.ID_PION_DESTINATAIRE = lignePion.ID_PION;
                }
                */
                /* pas une bonne idée de mettre le traitement en dehors de l'heure car cela n'a rien à voir avec un traitement de fichier
                tous ces traitements doivent se trouver dans ExecuterOrdreHorsMouvement 
                switch (ordre.I_TYPE)
                {
                    case Constantes.ORDRES.COMBAT:
                        //s'il s'agit d'un ordre de combat
                        //il faut supprimer tous les ordres actuellement en cours
                        lignePion.SupprimerTousLesOrdres();//normallement déjà fait lors de l'engagement en bataille, mais bon...
                        
                        //Mise à jour l'engagement et la zone d'engagement
                        lignePion.I_ZONE_BATAILLE = Convert.ToInt32(noeud["I_ZONE_BATAILLE"].InnerText);
                        lignePionBataille = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePion.ID_PION, Convert.ToInt32(noeud["ID_BATAILLE"].InnerText));
                        lignePionBataille.B_ENGAGEE = true;
                        //pour les presentations de fin de partie
                        lignePionBataille.B_ENGAGEMENT = true;
                        lignePionBataille.I_ZONE_BATAILLE_ENGAGEMENT = lignePion.I_ZONE_BATAILLE;
                        break;
                    case Constantes.ORDRES.RETRAITE:
                        //s'il s'agit d'un ordre de retraite, l'unité fuit durant deux tours et elle n'est plus engagée dans la bataille
                        lignePion.I_TOUR_FUITE_RESTANT = 2;
                        lignePionBataille = Donnees.m_donnees.TAB_BATAILLE_PIONS.FindByID_PIONID_BATAILLE(lignePion.ID_PION, Convert.ToInt32(noeud["ID_BATAILLE"].InnerText));
                        lignePionBataille.B_ENGAGEE = false;
                        //lignePion.I_ZONE_BATAILLE = WaocLib.Constantes.CST_IDNULL;
                        lignePion.SetI_ZONE_BATAILLENull();
                        break;
                    default:
                        listeConstantes.ORDRES.Add(ordre);
                        break;
                }
                 * */
                listeOrdres.Add(ordre);
            }
            // tri par numero d'ordre
            return listeOrdres.OrderBy(o => o.ID_ORDRE).ToList(); ;
        }

        public List<ClassDataModeles> ListeModeles(int idPartie)
        {
            string xpath;
            List<ClassDataModeles> listeModeles = new List<ClassDataModeles>();
            XmlDocument xDoc = new XmlDocument();
            //xDoc.Load(m_fileNameXML);
            xDoc.Load(LecteurXML.LireFichier(m_fileNameXML));
            if (idPartie >= 0)
            {
                xpath = string.Format("/vaoc/tab_vaoc_modele_pion[ID_PARTIE={0}]", idPartie);
            }
            else
            {
                xpath = "/vaoc/tab_vaoc_modele_pion";
            }

            foreach (XmlNode noeud in xDoc.SelectNodes(xpath))
            {
                ClassDataModeles modele = new ClassDataModeles();
                modele.ID_MODELE_PION  = Convert.ToInt32(noeud["ID_MODELE_PION"].InnerText);
                modele.ID_PARTIE = Convert.ToInt32(noeud["ID_PARTIE"].InnerText);
                modele.S_NOM = null!=noeud["S_NOM"] ? noeud["S_NOM"].InnerText : "inconnu";
                modele.S_IMAGE = null!=noeud["S_IMAGE"] ? noeud["S_IMAGE"].InnerText : "inconnu.jpg";
                modele.I_VISION_JOUR = null!=noeud["I_VISION_JOUR"] ? Convert.ToInt32(noeud["I_VISION_JOUR"].InnerText) : 0;
                modele.I_VISION_NUIT = null!=noeud["I_VISION_NUIT"] ? Convert.ToInt32(noeud["I_VISION_NUIT"].InnerText) : 0;
                listeModeles.Add(modele);
            }
            return listeModeles;
        }

        #endregion

        #region mise à jour des données

        /// <summary>
        /// Sauvegarde de tab_vaoc_forum
        /// </summary>
        /// <param name="idPartie">identifiant de la partie</param>
        public void SauvegardeForum(int idPartie)
        {
            string requete;

            //On remet la table à vide pour la partie
            requete = string.Format("DELETE FROM `tab_vaoc_forum` WHERE ID_PARTIE={0};",
                                    idPartie);
            AjouterLigne(requete);
            foreach (Donnees.TAB_ROLERow ligneRole in Donnees.m_donnees.TAB_ROLE)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneRole.ID_PION);
                if (null == lignePion)
                {
                    //il s'agit probablement d'un renfort,et, dans ce cas, on ne le met pas dans les rôles disponibles
                    continue;
                }
                else
                {
                    Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePion.ID_CASE);
                    foreach (Donnees.TAB_ROLERow ligneRole2 in Donnees.m_donnees.TAB_ROLE)
                    {
                        Donnees.TAB_PIONRow lignePion2 = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneRole2.ID_PION);
                        if (null == lignePion2)
                        {
                            //il s'agit probablement d'un renfort,et, dans ce cas, on ne le met pas dans les rôles disponibles
                            continue;
                        }
                        if (lignePion.ID_PION == lignePion2.ID_PION) { continue; }
                        if (lignePion.nation.ID_NATION != lignePion2.nation.ID_NATION) { continue; }

                        //si les deux pions sont séparés de moins d'un kilomètre, on ajoute les lignes dans la table
                        Donnees.TAB_CASERow ligneCase2 = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePion2.ID_CASE);
                        double dist = Constantes.Distance(ligneCase.I_X, ligneCase.I_Y, ligneCase2.I_X, ligneCase2.I_Y);
                        if (dist <= 1)
                        {
                            requete = string.Format("INSERT INTO `tab_vaoc_forum` (`ID_PARTIE`, `ID_PION1`, `ID_PION2`) VALUES ({0}, {1}, {2});",
                                                    idPartie,
                                                    lignePion.ID_PION,
                                                    lignePion2.ID_PION
                                                    );
                            AjouterLigne(requete);
                        }
                    }
                }
            }
        }

        public void TraitementEnCours(bool bTraitementEnCours, int idJeu, int idPartie)
        {
            int iTraitementEnCours = (bTraitementEnCours) ? 1 : 0;
            string requete = string.Format("update tab_vaoc_partie SET FL_MISEAJOUR={0} WHERE ID_JEU={1} AND ID_PARTIE={2};",
                iTraitementEnCours, idJeu, idPartie);
            AjouterLigne(requete);
        }

        public void SauvegardeNomsCarte(int idPartie)
        {
            string requete;

            //on reconstitue systématiquement tous les noms
            requete = string.Format("delete from tab_vaoc_noms_carte WHERE ID_PARTIE={0};",
                                    idPartie);
            AjouterLigne(requete);

            foreach (Donnees.TAB_NOMS_CARTERow ligneNom in Donnees.m_donnees.TAB_NOMS_CARTE)
            {
                int bPont = (ligneNom.B_PONT) ? 1 : 0;
                string nomCarte = (ligneNom.IsS_NOM_INDEXNull() || ligneNom.S_NOM_INDEX == string.Empty) ? ligneNom.S_NOM : ligneNom.S_NOM_INDEX;
                Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(ligneNom.ID_CASE);
                requete = string.Format("INSERT INTO `tab_vaoc_noms_carte` (`ID_NOM`, `ID_PARTIE`, `S_NOM`, `I_X`, `I_Y`, `B_PONT`) VALUES ({0}, {1}, '{2}', {3}, {4}, {5});",
                                        ligneNom.ID_NOM, idPartie, 
                                        Constantes.ChaineSQL(ligneNom.S_NOM), 
                                        ligneCase.I_X, ligneCase.I_Y, bPont);
                AjouterLigne(requete);
            }

        }

        public void SauvegardeNation(int idPartie)
        {
            /* ne plus le faire, car cela detruit les informations d'images
            string requete;

            //on reconstitue systématiquement toutes les nations
            requete = string.Format("delete from tab_vaoc_nation WHERE ID_PARTIE={0};",
                                    idPartie);
            AjouterLigne(requete);

            foreach (Donnees.TAB_NATIONRow ligneNation in Donnees.m_donnees.TAB_NATION)
            {
                requete = string.Format("INSERT INTO `tab_vaoc_nation` (`ID_NATION`, `ID_PARTIE`, `S_NOM`) VALUES ({0}, {1}, '{2}');",
                                        ligneNation.ID_NATION, idPartie, ClassMessager.ChaineSQL(ligneNation.S_NOM));
                AjouterLigne(requete);
            }
            */
        }

        public void SauvegardeMessage(int idPartie)
        {
            string requete, nomZoneGeographique;

            //INSERT INTO `tab_vaoc_message` (`ID_MESSAGE`, `ID_PARTIE`, `ID_EMETTEUR`, `ID_PION_PROPRIETAIRE`, `DT_DEPART`, `DT_ARRIVEE`, `S_MESSAGE`) VALUES (1, 1, 2, 1, '1805-06-15 02:04:18', '1805-06-15 22:40:15', 'La division a reçu un ordre : aller à Grenoble en partant à 8h00 durant 6 heures/jour'), (2, 1, 3, 1, '1805-06-02 22:52:27', '1805-06-03 12:52:27', 'La division vient d''arriver à Lyon et attend vos Constantes.ORDRES.');

            //on reconstitue systématiquement tous les messages
            requete = string.Format("delete from tab_vaoc_message WHERE ID_PARTIE={0};",
                                    idPartie);
            AjouterLigne(requete);

            foreach (Donnees.TAB_MESSAGERow ligneMessage in Donnees.m_donnees.TAB_MESSAGE)
            {
                if (!ligneMessage.IsI_TOUR_ARRIVEENull() && !ligneMessage.IsI_PHASE_ARRIVEENull())
                {
                    //Il faut indiquer en pion emetteur, le véritable emetteur et non le messager
                    //d'une part parce que l'on se moque d'avoir le nom du messager mais, aussi parce que les messagers
                    //n'étant pas exportés comme pion en base, on ne voit pas les messagers si on laisse l'id d'un messager
                    int idEmetteur = ligneMessage.ID_PION_EMETTEUR;
                    Donnees.TAB_PIONRow lignePionEmetteur = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneMessage.ID_PION_EMETTEUR);
                    //BEA 21/10/2012 test d'envoi du proprietaire de la patrouille
                    //à voir ce que cela donne quand c'est la patrouille qui renvoie un message, sinon on retire cette ligne
                    //on voit "div X: patrouille" comme envoyeur c'est pas nickel au moment de la reception de l'ordre d'envoi
                    //exception, si le messager est celui d'une division de
                    while (lignePionEmetteur.estMessager || lignePionEmetteur.estPatrouille)
                    {
                        lignePionEmetteur = lignePionEmetteur.proprietaire;
                    }
                    idEmetteur = lignePionEmetteur.ID_PION;

                    ClassMessager.CaseVersZoneGeographique(ligneMessage.ID_CASE_FIN, out nomZoneGeographique);
                    requete = string.Format("INSERT INTO `tab_vaoc_message` (`ID_MESSAGE`, `ID_PARTIE`, `ID_EMETTEUR`, `ID_PION_PROPRIETAIRE`, `DT_DEPART`, `DT_ARRIVEE`, `S_ORIGINE`, `S_MESSAGE`) "+
                                            "VALUES ({0}, {1}, {2}, {3}, '{4}', '{5}', '{6}', '{7}');",
                                            ligneMessage.ID_MESSAGE,
                                            idPartie,
                                            idEmetteur,
                                            ligneMessage.ID_PION_PROPRIETAIRE,
                                            ClassMessager.DateHeureSQL(ligneMessage.I_TOUR_DEPART, ligneMessage.I_PHASE_DEPART),
                                            ClassMessager.DateHeureSQL(ligneMessage.I_TOUR_ARRIVEE, ligneMessage.I_PHASE_ARRIVEE),
                                            Constantes.ChaineSQL(nomZoneGeographique),
                                            Constantes.ChaineSQL(ligneMessage.S_TEXTE));
                    AjouterLigne(requete);
                }
            }
        }

        //public void SauvegardeNation(int idPartie)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Sauvegarde de TAB_PION
        /// </summary>
        /// <param name="idJeu">identifiant du jeu</param>
        /// <param name="idPartie">identifiant de la partie</param>
        public void SauvegardePion(int idPartie)
        {
            string requete;

            //on reconstitue systématiquement tous les noms
            requete = string.Format("delete from tab_vaoc_pion WHERE ID_PARTIE={0};",
                                    idPartie);
            AjouterLigne(requete);

            foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
            {
                //on recherche la dernière position de l'unité connue par le joueur, pas la position réelle (sauf pour le joueur lui même)
                //il y a forcement un message, toutes les unités s'annoncent au tour 0, sauf pour les messagers et les patrouilles !
                //mais eux, il n'est pas necessaire de les mettre en base
                // !!!! Si,si, il faut les mettre en base sinon, on ne sait pas toujours qui a émis le message et donc, le joueur ne les
                //voit pas :-))
                //le problème c'est que si on met les lignes qui suivent entre parenthèses on voit les messagers
                //pas les patrouilles je pense car elles ne sont jamais directement rattachées à un joueur
                //il faut donc que les messages (voir SauvegarderMessages reference toujours le propriétaire du porteur du message et non le porteur lui-même)
                //if (lignePion.estMessager || lignePion.estPatrouille)
                if (lignePion.estMessager)
                {
                    continue;
                }

                requete = string.Empty;
                if (lignePion.IsID_NOUVEAU_PION_PROPRIETAIRENull())
                {
                    //Tant qu'id nouveau proprietaire est renseigné, le nouveau propriétaire ne doit pas voir l'unité dans son bilan
                    //la valeur est remise à vide quand l'ancien proprietaire reçoit le message/ordre du transfert
                    Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageRecu(lignePion.ID_PION, -1);
                    requete = GenereLignePion(lignePion, idPartie, lignePion.ID_PION_PROPRIETAIRE, ligneMessage);
                }
                else
                {

                    if (!lignePion.IsID_ANCIEN_PION_PROPRIETAIRENull())
                    {
                        //Tant qu'id ancien proprietaire est renseigné, l'ancien propriétaire doit continuer à voir l'unité dans son bilan
                        //la valeur est remise à vide quand l'ancien proprietaire reçoit le message/ordre du transfert
                        Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageRecu(lignePion.ID_PION, lignePion.ID_ANCIEN_PION_PROPRIETAIRE);
                        requete = GenereLignePion(lignePion, idPartie, lignePion.ID_ANCIEN_PION_PROPRIETAIRE, ligneMessage);
                    }
                    else
                    {
                        Donnees.TAB_MESSAGERow ligneMessage = Donnees.m_donnees.TAB_MESSAGE.DernierMessageRecu(lignePion.ID_PION, lignePion.ID_PION_PROPRIETAIRE);
                        requete = GenereLignePion(lignePion, idPartie, lignePion.ID_PION_PROPRIETAIRE, ligneMessage);
                    }
                }
                AjouterLigne(requete);
            }
        }

        private static string GenereLignePion(Donnees.TAB_PIONRow lignePion, int idPartie, int id_pion_proprietaire, Donnees.TAB_MESSAGERow ligneMessage)
        {
            int bFuiteAuCombat;
            int bReditionRavitaillement;
            int bDepot;
            int bPontonnier;
            int iPatrouillesDisponibles;
            int iPatrouillesMax;
            string nomZoneGeographique;
            string sPosition;
            int bDetruit;
            decimal iVitesse;
            string sVitesse;
            int iZoneBataille;
            int idBataille;
            string requete;
            int bConvoi;
            int bRenfort;
            int bQG;

            bFuiteAuCombat = (lignePion.B_FUITE_AU_COMBAT) ? 1 : 0;
            bReditionRavitaillement = (lignePion.B_REDITION_RAVITAILLEMENT) ? 1 : 0;
            bDepot = (lignePion.estDepot) ? 1 : 0;
            bPontonnier = (lignePion.estPontonnier) ? 1 : 0;
            bConvoi = (lignePion.estConvoi) ? 1 : 0;
            bRenfort = (lignePion.B_RENFORT) ? 1 : 0;
            bQG = (lignePion.estQG) ? 1 : 0;

            if (lignePion.estQG || null == ligneMessage)
            {
                iZoneBataille = (lignePion.IsI_ZONE_BATAILLENull()) ? -1 : lignePion.I_ZONE_BATAILLE;//on voit toujours la position réelle en bataille
                idBataille = (lignePion.IsID_BATAILLENull()) ? -1 : lignePion.ID_BATAILLE;
            }
            else
            {
                //pour la zone de bataille on met toujours la vraie valeur, sinon, quand il y a retrait et que le proprietaire n'est pas dans le combat, on ne le voit pas
                //la bataille doit rester un message justement pour un chef distant du combat (qui, de toute façon ne voit pas la zone)
                //iZoneBataille = (ligneMessage.IsI_ZONE_BATAILLENull()) ? -1 : ligneMessage.I_ZONE_BATAILLE;//on voit toujours la position réelle en bataille
                iZoneBataille = (lignePion.IsI_ZONE_BATAILLENull()) ? -1 : lignePion.I_ZONE_BATAILLE;//on voit toujours la position réelle en bataille
                idBataille = (ligneMessage.IsID_BATAILLENull()) ? -1 : ligneMessage.ID_BATAILLE;
            }
            if (null == ligneMessage)
            {
                if (Donnees.m_donnees.TAB_PARTIE[0].IsID_VICTOIRENull() || Donnees.m_donnees.TAB_PARTIE[0].ID_VICTOIRE < 0)
                {
                    bDetruit = 0;
                }
                else
                {
                    bDetruit = (ligneMessage.B_DETRUIT) ? 1 : 0;//cas improbable où la partie serait terminée sans que le pion n'est reçu un seul message
                }

                if (lignePion.I_CAVALERIE > 0)
                {
                    iPatrouillesMax = Math.Max(1, lignePion.I_CAVALERIE / 1000);
                    //il faut retrancher les patrouilles déjà en cours de mission
                    int iPatrouillesEnCours = lignePion.nombrePatrouillesEnCours();
                    iPatrouillesDisponibles = (iPatrouillesMax <= iPatrouillesEnCours) ? 0 : iPatrouillesMax - iPatrouillesEnCours;
                }
                else
                {
                    iPatrouillesDisponibles = 0;
                    iPatrouillesMax = 0;
                }
            }
            else
            {
                bDetruit = (ligneMessage.B_DETRUIT) ? 1 : 0;
                if (null != ligneMessage && ligneMessage.I_CAVALERIE > 0)
                {
                    iPatrouillesMax = Math.Max(1, ligneMessage.I_CAVALERIE / 1000);
                    //il faut retrancher les patrouilles déjà en cours de mission
                    int iPatrouillesEnCours = lignePion.nombrePatrouillesEnCours();
                    iPatrouillesDisponibles = (iPatrouillesMax <= iPatrouillesEnCours) ? 0 : iPatrouillesMax - iPatrouillesEnCours;
                }
                else
                {
                    iPatrouillesDisponibles = 0;
                    iPatrouillesMax = 0;
                }
            }

            if (null != Donnees.m_donnees.TAB_ROLE.TrouvePion(lignePion.ID_PION) || null == ligneMessage)
            {
                //s'il y a un role, c'est un joueur, il faut trouver sa position reelle
                ClassMessager.CaseVersZoneGeographique(lignePion.ID_CASE, out sPosition);
            }
            else
            {
                if (ligneMessage.ID_CASE_DEBUT != ligneMessage.ID_CASE_FIN)
                {
                    ClassMessager.CaseVersZoneGeographique(ligneMessage.ID_CASE_DEBUT, out nomZoneGeographique);
                    sPosition = "Entre " + nomZoneGeographique;
                    ClassMessager.CaseVersZoneGeographique(ligneMessage.ID_CASE_FIN, out nomZoneGeographique);
                    sPosition += " et " + nomZoneGeographique;
                }
                else
                {
                    ClassMessager.CaseVersZoneGeographique(ligneMessage.ID_CASE_FIN, out sPosition);
                }
            }
            sPosition = sPosition.Replace("'", "''");//remplacement des apostrophes pour l'insertion en base

            iVitesse = decimal.MaxValue;
            Donnees.TAB_MODELE_PIONRow ligneModelePion = lignePion.modelePion;
            Donnees.TAB_MODELE_MOUVEMENTRow ligneModeleMouvement = Donnees.m_donnees.TAB_MODELE_MOUVEMENT.FindByID_MODELE_MOUVEMENT(ligneModelePion.ID_MODELE_MOUVEMENT);
            Donnees.TAB_CASERow ligneCase = Donnees.m_donnees.TAB_CASE.FindByID_CASE(lignePion.ID_CASE);

            //cas d'une unité sans effectif, on prend la vitesse de la cavalerie
            if (lignePion.I_INFANTERIE == 0 && lignePion.I_CAVALERIE == 0 && lignePion.I_ARTILLERIE == 0)
            {
                iVitesse = ligneModeleMouvement.I_VITESSE_CAVALERIE;
            }
            else
            {
                if (lignePion.I_INFANTERIE > 0 && ligneModeleMouvement.I_VITESSE_INFANTERIE < iVitesse)
                {
                    iVitesse = ligneModeleMouvement.I_VITESSE_INFANTERIE;
                }
                if (lignePion.I_CAVALERIE > 0 && ligneModeleMouvement.I_VITESSE_CAVALERIE < iVitesse)
                {
                    iVitesse = ligneModeleMouvement.I_VITESSE_CAVALERIE;
                }
                if (lignePion.I_ARTILLERIE > 0 && ligneModeleMouvement.I_VITESSE_ARTILLERIE < iVitesse)
                {
                    iVitesse = ligneModeleMouvement.I_VITESSE_ARTILLERIE;
                }
            }
            sVitesse = iVitesse.ToString().Replace(",", ".");
            int iInfanterie = (null == ligneMessage) ? lignePion.I_INFANTERIE : ligneMessage.I_INFANTERIE;
            int iCavalerie = (null == ligneMessage) ? lignePion.I_CAVALERIE : ligneMessage.I_CAVALERIE;
            int iArtillerie = (null == ligneMessage) ? lignePion.I_ARTILLERIE : ligneMessage.I_ARTILLERIE;
            int iFatigue = (null == ligneMessage) ? lignePion.I_FATIGUE : ligneMessage.I_FATIGUE;
            int iMoral = (null == ligneMessage) ? lignePion.I_MORAL : ligneMessage.I_MORAL;
            int iRetraite = (null == ligneMessage) ? lignePion.I_TOUR_FUITE_RESTANT : ligneMessage.I_RETRAITE;

            requete = string.Format(
                                    "INSERT INTO `tab_vaoc_pion` (`ID_PION`, `ID_PARTIE`, `ID_PION_PROPRIETAIRE`, `ID_MODELE_PION`, `S_NOM`, `I_INFANTERIE`, " +
                                    "`I_CAVALERIE`, `I_ARTILLERIE`, `I_FATIGUE`, `I_MORAL`, `I_INFANTERIE_REEL`, `I_CAVALERIE_REEL`, `I_ARTILLERIE_REEL`, " +
                                    "`I_FATIGUE_REEL`, `I_MORAL_REEL`, `I_MORAL_MAX`, `I_EXPERIENCE`, `I_TACTIQUE`, `I_STRATEGIQUE`, `C_NIVEAU_HIERARCHIQUE`, " +
                                    "`I_RETRAITE`, `ID_BATAILLE`, `I_ZONE_BATAILLE`, `S_POSITION`, `B_DETRUIT`, `I_PATROUILLES_DISPONIBLES`, `I_PATROUILLES_MAX`, " +
                                    "`I_VITESSE`, `I_X` ,`I_Y`, `I_INFANTERIE_INITIALE`, `I_CAVALERIE_INITIALE`, `I_ARTILLERIE_INITIALE`, " +
                                    "`B_FUITE_AU_COMBAT`, `B_REDITION_RAVITAILLEMENT`, `B_DEPOT`, `B_PONTONNIER`," +
                                    "`I_MATERIEL`, `I_RAVITAILLEMENT`, `I_NIVEAU_FORTIFICATION`, " +
                                    "`I_TOUR_CONVOI_CREE`, `ID_DEPOT_SOURCE`, `B_CAVALERIE_DE_LIGNE`, `B_CAVALERIE_LOURDE`, `B_GARDE`, `B_VIEILLE_GARDE`," +
                                    "`B_BLESSES`, `B_PRISONNIERS`, `C_NIVEAU_DEPOT`, `B_CONVOI`, `B_RENFORT`, `B_QG`, `I_SOLDATS_RAVITAILLES` " +
                                    ") VALUES " +
                                    "({0}, {1}, {2}, {3}, '{4}', {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, '{19}', {20}, " +
                                    "{21}, {22}, '{23}', {24}, {25}, {26}, {27}, {28}, {29}, {30}, {31}, {32}, {33}, {34}, {35}, {36}, {37}, {38}, {39}, {40}, "+
                                    "{41}, {42}, {43}, {44}, {45}, {46}, {47}, '{48}', {49}, {50}, {51}, {52});",
                                    lignePion.ID_PION,//0
                                    idPartie,
                                    (null == ligneMessage) ? -1 : id_pion_proprietaire,//si on a pas reçu de message, on ne doit pas voir l'unité, cas de convois de blessés dans un combat où l'on était pas
                                    lignePion.ID_MODELE_PION,
                                    Constantes.ChaineSQL(lignePion.S_NOM),
                                    iInfanterie,//5
                                    iCavalerie,
                                    iArtillerie,
                                    iFatigue,
                                    iMoral,
                                    lignePion.I_INFANTERIE,//10
                                    lignePion.I_CAVALERIE,
                                    lignePion.I_ARTILLERIE,
                                    lignePion.I_FATIGUE,
                                    lignePion.Moral,
                                    lignePion.I_MORAL_MAX,
                                    lignePion.I_EXPERIENCE.ToString().Replace(",", "."),
                                    lignePion.I_TACTIQUE,
                                    lignePion.I_STRATEGIQUE,
                                    lignePion.IsC_NIVEAU_HIERARCHIQUENull() || lignePion.C_NIVEAU_HIERARCHIQUE==0 ? 'Z' : lignePion.C_NIVEAU_HIERARCHIQUE,
                                    iRetraite,//20
                                    idBataille,
                                    iZoneBataille,//22
                                    sPosition,//`S_POSITION`
                                    bDetruit,
                                    iPatrouillesDisponibles,//25
                                    iPatrouillesMax,
                                    sVitesse,
                                    ligneCase.I_X,
                                    ligneCase.I_Y,
                                    lignePion.I_INFANTERIE_INITIALE,//30
                                    lignePion.I_CAVALERIE_INITIALE,
                                    lignePion.I_ARTILLERIE_INITIALE,
                                    bFuiteAuCombat,
                                    bReditionRavitaillement,
                                    bDepot,//35
                                    bPontonnier,
                                    lignePion.I_MATERIEL,
                                    lignePion.I_RAVITAILLEMENT,
                                    lignePion.I_NIVEAU_FORTIFICATION,
                                    lignePion.IsI_TOUR_CONVOI_CREENull() ? -1 : lignePion.I_TOUR_CONVOI_CREE,//40
                                    lignePion.IsID_DEPOT_SOURCENull() ? -1 : lignePion.ID_DEPOT_SOURCE,
                                    lignePion.B_CAVALERIE_DE_LIGNE ? 1 : 0,
                                    lignePion.B_CAVALERIE_LOURDE ? 1 : 0,
                                    lignePion.B_GARDE ? 1 : 0,
                                    lignePion.B_VIEILLE_GARDE ? 1 : 0,//45
                                    lignePion.B_BLESSES ? 1 : 0,
                                    lignePion.B_PRISONNIERS ? 1 : 0,
                                    lignePion.C_NIVEAU_DEPOT,
                                    bConvoi,
                                    bRenfort,//50
                                    bQG,
                                    lignePion.IsI_SOLDATS_RAVITAILLESNull() ? 0 : lignePion.I_SOLDATS_RAVITAILLES
                                    );
            return requete;
        }

        /// <summary>
        /// Sauvegarde TAB_METEO. Utile uniquement dans l'information de scénario des joueurs
        /// </summary>
        /// <param name="idJeu">identifiant du Jeu</param>
        public void SauvegardeMeteo(int idJeu)
        {
            string requete;

            //Supression des valeurs précédentes
            requete = string.Format("delete from tab_vaoc_meteo WHERE ID_JEU={0};",
                                    idJeu);
            AjouterLigne(requete);

            foreach (Donnees.TAB_METEORow ligneMeteo in Donnees.m_donnees.TAB_METEO)
            {
                requete = string.Format("INSERT INTO `tab_vaoc_meteo` (`ID_JEU`, `ID_METEO`, `S_NOM`, `I_CHANCE`) " +
                                        "VALUES ({0}, {1}, '{2}', {3});",
                                        idJeu,
                                        ligneMeteo.ID_METEO,
                                        ligneMeteo.S_NOM,                                        
                                        ligneMeteo.I_CHANCE
                                        );

                AjouterLigne(requete);
            }
        }

        /// <summary>
        /// Sauvegarde de TAB_MODELE_MOUVEMENT. Utile uniquement dans l'information de scénario des joueurs 
        /// </summary>
        /// <param name="idJeu">identifiant du Jeu</param>
        public void SauvegardeModelesMouvement(int idJeu)
        {
            string requete;
                                        
            //Supression des valeurs précédentes
            requete = string.Format("delete from tab_vaoc_modele_mouvement WHERE ID_JEU={0};",
                                    idJeu);
            AjouterLigne(requete);
            
            foreach (Donnees.TAB_MODELE_PIONRow ligneModelePion in Donnees.m_donnees.TAB_MODELE_PION)
            {
                Donnees.TAB_MODELE_MOUVEMENTRow ligneModeleMouvement = Donnees.m_donnees.TAB_MODELE_MOUVEMENT.FindByID_MODELE_MOUVEMENT(ligneModelePion.ID_MODELE_MOUVEMENT);
                foreach (Donnees.TAB_METEORow ligneMeteo in Donnees.m_donnees.TAB_METEO)
                {
                    foreach (Donnees.TAB_MODELE_TERRAINRow ligneModeleTerrain in Donnees.m_donnees.TAB_MODELE_TERRAIN)
                    {
                        int cout = Donnees.m_donnees.TAB_MOUVEMENT_COUT.CalculCout(ligneMeteo.ID_METEO, ligneModeleTerrain.ID_MODELE_TERRAIN, ligneModeleMouvement.ID_MODELE_MOUVEMENT);
                        Donnees.TAB_NATIONRow ligneNation = Donnees.m_donnees.TAB_NATION.FindByID_NATION(ligneModelePion.ID_NATION);

                        requete = string.Format("INSERT INTO `tab_vaoc_modele_mouvement` (`ID_JEU`, `ID_MODELE_MOUVEMENT`, `S_NATION`, `S_METEO`, `S_MODELE`, `S_TERRAIN`, `I_VITESSE_INFANTERIE`, `I_VITESSE_CAVALERIE`, `I_VITESSE_ARTILLERIE`) " +
                                        "VALUES ({0}, {1}, '{2}', '{3}', '{4}', '{5}', {6}, {7}, {8});",
                                        idJeu,
                                        ligneModelePion.ID_MODELE_PION,
                                        ligneNation.S_NOM,
                                        ligneMeteo.S_NOM,
                                        ligneModelePion.S_NOM,
                                        ligneModeleTerrain.S_NOM,
                                        Constantes.ChaineSQL((0 == cout) ? 0 : (decimal)(ligneModeleMouvement.I_VITESSE_INFANTERIE * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE) / cout),
                                        Constantes.ChaineSQL((0 == cout) ? 0 : (decimal)(ligneModeleMouvement.I_VITESSE_CAVALERIE * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE) / cout),
                                        Constantes.ChaineSQL((0 == cout) ? 0 : (decimal)(ligneModeleMouvement.I_VITESSE_ARTILLERIE * Donnees.m_donnees.TAB_JEU[0].I_ECHELLE) / cout)
                                        );

                        AjouterLigne(requete);
                    }
                }
            }
        }

        /// <summary>
        /// Sauvegarde de TAB_MODELE_PION
        /// </summary>
        /// <param name="idPartie">identifiant de la partie</param>
        public void SauvegardeModelesPion(int idPartie)
        {
            string requete;
            bool bExiste;
            int i;
            List<ClassDataModeles> listeModeles = ListeModeles(idPartie);

            //INSERT INTO `tab_vaoc_modele_pion` (`ID_MODELE_PION`, `ID_PARTIE`, `S_NOM`,`S_IMAGE`) VALUES (1, 1, 'Napoleon', 'napoleon_tete.jpeg')
            //on reconstitue systématiquement tous les noms
            //requete = string.Format("delete from tab_vaoc_modele_pion WHERE ID_PARTIE={0};",
            //                        idPartie);
            //AjouterLigne(requete);

            foreach (Donnees.TAB_MODELE_PIONRow ligneModelePion in Donnees.m_donnees.TAB_MODELE_PION)
            {
                Donnees.TAB_MODELE_MOUVEMENTRow ligneModeleMouvement = Donnees.m_donnees.TAB_MODELE_MOUVEMENT.FindByID_MODELE_MOUVEMENT(ligneModelePion.ID_MODELE_MOUVEMENT);

                //si le modele existe déjà, on fait un update, sinon un insert
                bExiste = false;
                i = 0;
                while (!bExiste && i<listeModeles.Count)
                {
                    if (listeModeles[i].ID_MODELE_PION == ligneModelePion.ID_MODELE_PION)
                    {
                        bExiste = true;
                    }
                    i++;
                }

                if (bExiste)
                {
                    requete = string.Format("UPDATE `tab_vaoc_modele_pion` SET `S_NOM`='{2}',`I_VISION_JOUR`={3},`I_VISION_NUIT`={4}, `ID_NATION`={5} WHERE (ID_MODELE_PION={0} AND ID_PARTIE={1});",
                                            ligneModelePion.ID_MODELE_PION,
                                            idPartie,
                                            ligneModelePion.S_NOM,
                                            //"inconnu.jpg", -> à ne pas mettre à jour surtout
                                            ligneModelePion.I_VISION_JOUR,
                                            ligneModelePion.I_VISION_NUIT,
                                            ligneModelePion.ID_NATION);
                }
                else
                {
                    //requete = string.Format("INSERT INTO `tab_vaoc_modele_pion` (`ID_MODELE_PION`, `ID_PARTIE`, `S_NOM`,`S_IMAGE`,`I_VISION_JOUR`,`I_VISION_NUIT`, `ID_NATION`) VALUES ({0}, {1}, '{2}', '{3}', {4}, {5}, {6});",
                    requete = string.Format("INSERT INTO `tab_vaoc_modele_pion` (`ID_MODELE_PION`, `ID_PARTIE`, `S_NOM`,`I_VISION_JOUR`,`I_VISION_NUIT`, `ID_NATION`) VALUES ({0}, {1}, '{2}', {3}, {4}, {5});",
                                            ligneModelePion.ID_MODELE_PION,
                                            idPartie,
                                            ligneModelePion.S_NOM,
                                            //"inconnu.jpg",
                                            ligneModelePion.I_VISION_JOUR,
                                            ligneModelePion.I_VISION_NUIT,
                                            ligneModelePion.ID_NATION);
                }

                AjouterLigne(requete);
            }
        }

        /// <summary>
        /// Mise à jour de TAB_PARTIE
        /// </summary>
        /// <param name="idJeu">identifiant du jeu</param>
        /// <param name="idPartie">identifiant de la partie</param>
        public void SauvegardePartie(int idPartie)
        {
            string requete;
            ClassDataPartie partie = GetPartie(idPartie);
            string repertoireTour;

//INSERT INTO `tab_vaoc_partie` (`ID_PARTIE`, `ID_JEU`, `S_NOM`, `I_TOUR`, `DT_TOUR`, `I_PHASE`, `DT_CREATION`, `DT_MISEAJOUR`, `H_JOUR`, `H_NUIT`, `S_REPERTOIRE`, `FL_MISEAJOUR`, `FL_DEMARRAGE`, `I_NB_CARTE_X`, `I_NB_CARTE_Y`, `I_NB_CARTE_ZOOM_X`, `I_NB_CARTE_ZOOM_Y`, `D_MULT_ZOOM_X`, `D_MULT_ZOOM_Y`, `I_LARGEUR_CARTE_ZOOM`, `I_HAUTEUR_CARTE_ZOOM`, `I_ECHELLE`) VALUES
//(2, 2, 'Bataille de Russie 1801', 0, '1809-04-16 09:00:00', 0, '2007-07-29 21:51:41', '2010-02-23 18:21:40', 8, 18, 'BatailledeRussie1801_Thewar_0', '0', '0', 3, 2, 10, 9, 5, 5, 800, 600, 10);
            
            Donnees.TAB_PARTIERow lignePartie = Donnees.m_donnees.TAB_PARTIE[0];
            Donnees.TAB_JEURow ligneJeu = Donnees.m_donnees.TAB_JEU[0];

            repertoireTour = string.Format("{0}_{1}",
                                ligneJeu.S_NOM.Replace(" ", ""),
                                lignePartie.S_NOM.Replace(" ", ""));

            int flDemmarage = (lignePartie.FL_DEMARRAGE) ? 1 : 0;
            int largeur = Cartographie.GetImageLargeur(Constantes.MODELESCARTE.HISTORIQUE);
            int hauteur = Cartographie.GetImageHauteur(Constantes.MODELESCARTE.HISTORIQUE);
            int largeurZoom = Cartographie.GetImageLargeur(Constantes.MODELESCARTE.ZOOM);
            int hauteurZoom = Cartographie.GetImageHauteur(Constantes.MODELESCARTE.ZOOM);
            string sMultZoomX = ((decimal)largeurZoom/largeur).ToString().Replace(",", ".");
            string sMultZoomY = ((decimal)hauteurZoom / hauteur).ToString().Replace(",", ".");
            int idVictoire = lignePartie.IsID_VICTOIRENull() ? -1 : lignePartie.ID_VICTOIRE;
            Donnees.TAB_METEORow ligneMeteo = Donnees.m_donnees.TAB_METEO.FindByID_METEO(lignePartie.ID_METEO);
            string sMeteo = (null == ligneMeteo) ? "imprévisible" : ligneMeteo.S_NOM;
            //int maxIDOrdre = Donnees.m_donnees.TAB_ORDRE.MaxID_ORDRE;

            if (null != partie)
            {
                requete = string.Format("UPDATE `tab_vaoc_partie` SET `ID_JEU`={1}, `S_NOM`='{2}', `I_TOUR`={3},`DT_TOUR`='{4}',`I_PHASE`={5},`DT_MISEAJOUR`=NOW(), `H_JOUR`={7}, " +
                                        "`H_NUIT`={8}, `S_REPERTOIRE`='{9}', `FL_DEMARRAGE`={10}, `I_NB_CARTE_X`={11}, `I_NB_CARTE_Y`={12}, "+
                                        "`I_NB_CARTE_ZOOM_X`={13}, `I_NB_CARTE_ZOOM_Y`={14}, `D_MULT_ZOOM_X`={15}, `D_MULT_ZOOM_Y`={16}, `I_LARGEUR_CARTE_ZOOM`={17}, `I_HAUTEUR_CARTE_ZOOM`={18}, "+
                                        "`I_ECHELLE`={19}, `ID_VICTOIRE`={20}, `S_METEO`='{21}' WHERE (ID_PARTIE={0});",
                                        idPartie,
                                        lignePartie.ID_JEU,
                                        lignePartie.S_NOM,
                                        lignePartie.I_TOUR,
                                        ClassMessager.DateHeureSQL(lignePartie.I_TOUR, lignePartie.I_PHASE), //DT_TOUR,
                                        lignePartie.I_PHASE,//5
                                        //ClassMessager.DateHeureSQL(DateTime.Now), //DT_CREATION,
                                        Constantes.DateHeureSQL(DateTime.Now),//DT_MISEAJOUR -> remplacé dans le code par le fonction NOW
                                        ligneJeu.I_LEVER_DU_SOLEIL,//H_JOUR
                                        ligneJeu.I_COUCHER_DU_SOLEIL,//H_NUIT
                                        repertoireTour, //9, S_REPERTOIRE
                                        //1,//FL_MISEAJOUR
                                        flDemmarage, //10, FL_DEMARRAGE
                                        (int)Math.Ceiling((double)largeur/lignePartie.I_LARGEUR_CARTE_ZOOM_WEB),//`I_NB_CARTE_X`
                                        (int)Math.Ceiling((double)hauteur/lignePartie.I_HAUTEUR_CARTE_ZOOM_WEB),//`I_NB_CARTE_Y`
                                        (int)Math.Ceiling((double)largeurZoom/lignePartie.I_LARGEUR_CARTE_ZOOM_WEB),// `I_NB_CARTE_ZOOM_X`
                                        (int)Math.Ceiling((double)hauteurZoom/lignePartie.I_HAUTEUR_CARTE_ZOOM_WEB),//`I_NB_CARTE_ZOOM_Y`
                                        sMultZoomX,//15, `D_MULT_ZOOM_X`
                                        sMultZoomY,//`D_MULT_ZOOM_Y`
                                        lignePartie.I_LARGEUR_CARTE_ZOOM_WEB,//`I_LARGEUR_CARTE_ZOOM`
                                        lignePartie.I_HAUTEUR_CARTE_ZOOM_WEB,//`I_HAUTEUR_CARTE_ZOOM`
                                        ligneJeu.I_ECHELLE,//19, `I_ECHELLE`     
                                        idVictoire,
                                        sMeteo
                                        );                                        
            }
            else
            {
                requete = string.Format("INSERT INTO `tab_vaoc_partie` (`ID_PARTIE`, `ID_JEU`, `S_NOM`, `I_TOUR`, `DT_TOUR`, `I_PHASE`, `DT_CREATION`, `DT_MISEAJOUR`, `H_JOUR`, "+
                                        "`H_NUIT`, `S_REPERTOIRE`, `FL_MISEAJOUR`, `FL_DEMARRAGE`, `I_NB_CARTE_X`, `I_NB_CARTE_Y`, `I_NB_CARTE_ZOOM_X`, `I_NB_CARTE_ZOOM_Y`, `D_MULT_ZOOM_X`, `D_MULT_ZOOM_Y`, "+
                                        "`I_LARGEUR_CARTE_ZOOM`, `I_HAUTEUR_CARTE_ZOOM`, `I_ECHELLE`, `ID_VICTOIRE`,`S_METEO`, `MAX_ID_ORDRE`) " +
                                        "VALUES ({0}, {1}, '{2}', {3}, '{4}', {5}, '{6}', '{7}', {8}, {9}, '{10}', {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, '{23}', {24});",
                                        idPartie,//`ID_PARTIE`
                                        lignePartie.ID_JEU,//`ID_JEU`
                                        lignePartie.S_NOM,
                                        lignePartie.I_TOUR,
                                        ClassMessager.DateHeureSQL(lignePartie.I_TOUR, lignePartie.I_PHASE), //DT_TOUR,
                                        lignePartie.I_PHASE,//5
                                        Constantes.DateHeureSQL(DateTime.Now), //DT_CREATION,
                                        Constantes.DateHeureSQL(DateTime.Now),//DT_MISEAJOUR,
                                        ligneJeu.I_LEVER_DU_SOLEIL,//H_JOUR
                                        ligneJeu.I_COUCHER_DU_SOLEIL,//H_NUIT
                                        repertoireTour, //10, S_REPERTOIRE
                                        1,//FL_MISEAJOUR
                                        flDemmarage, //FL_DEMARRAGE
                                        (int)Math.Ceiling((double)largeur/lignePartie.I_LARGEUR_CARTE_ZOOM_WEB),//`I_NB_CARTE_X`
                                        (int)Math.Ceiling((double)hauteur/lignePartie.I_HAUTEUR_CARTE_ZOOM_WEB),//`I_NB_CARTE_Y`
                                        (int)Math.Ceiling((double)largeurZoom/lignePartie.I_LARGEUR_CARTE_ZOOM_WEB),//15, `I_NB_CARTE_ZOOM_X`
                                        (int)Math.Ceiling((double)hauteurZoom/lignePartie.I_HAUTEUR_CARTE_ZOOM_WEB),//`I_NB_CARTE_ZOOM_Y`
                                        sMultZoomX,//`D_MULT_ZOOM_X`
                                        sMultZoomY,//`D_MULT_ZOOM_Y`
                                        lignePartie.I_LARGEUR_CARTE_ZOOM_WEB,//`I_LARGEUR_CARTE_ZOOM`
                                        lignePartie.I_HAUTEUR_CARTE_ZOOM_WEB,//20,`I_HAUTEUR_CARTE_ZOOM`
                                        ligneJeu.I_ECHELLE,//`I_ECHELLE`
                                        idVictoire,
                                        sMeteo,//23
                                        1
                                        );
            }
            AjouterLigne(requete);
        }

        /// <summary>
        /// Sauvegarde de TAB_ROLE
        /// </summary>
        /// <param name="idPartie">identifiant de la partie</param>
        public void SauvegardeRole(int idPartie)
        {
            string requete;
            bool bExiste, bEnDanger;
            int i, iAucontact;
            List<ClassDataRole> listeRoles = ListeRoles(idPartie);
            Donnees.TAB_MODELE_PIONRow ligneModele;
            string sUnitesVisibles;

            //INSERT INTO `tab_vaoc_modele_pion` (`ID_MODELE_PION`, `ID_PARTIE`, `S_NOM`,`S_IMAGE`) VALUES (1, 1, 'Napoleon', 'napoleon_tete.jpeg')
            //on reconstitue systématiquement tous les noms
            //requete = string.Format("delete from tab_vaoc_modele_pion WHERE ID_PARTIE={0};",
            //                        idPartie);
            //AjouterLigne(requete);

            foreach (Donnees.TAB_ROLERow ligneRole in Donnees.m_donnees.TAB_ROLE)
            {
                Donnees.TAB_PIONRow lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneRole.ID_PION);
                if (null == lignePion)
                {
                    //il s'agit probablement d'un renfort,et, dans ce cas, on ne le met pas dans les rôles disponibles
                    continue;
                    //DataSetCoutDonnees.TAB_RENFORTRow ligneRenfort = DataSetCoutDonnees.m_donnees.TAB_RENFORT.FindByID_PION(ligneRole.ID_PION);
                    //if (null == ligneRenfort)
                    //{
                    //    throw new NotImplementedException(string.Format("SauvegardeRole : Le role ID_PION={0} n'a pas de pion, ni de renfort",ligneRole.ID_ROLE));
                    //}
                    //ligneModele = DataSetCoutDonnees.m_donnees.TAB_MODELE_PION.FindByID_MODELE_PION(ligneRenfort.ID_MODELE_PION);
                }
                else
                {
                    ligneModele = lignePion.modelePion;
                }
                //si le role existe déjà, on fait un update, sinon un insert
                bExiste = false;
                i = 0;
                while (!bExiste && i<listeRoles.Count)
                {
                    if (listeRoles[i].ID_ROLE == ligneRole.ID_ROLE && listeRoles[i].ID_PARTIE == idPartie)
                    {
                        bExiste = true;
                    }
                    i++;
                }

                if (!ClassMessager.PionsEnvironnants(lignePion, ClassMessager.MESSAGES.MESSAGE_AUCUN_MESSAGE, null, out sUnitesVisibles, out bEnDanger))
                {
                    throw new NotImplementedException(string.Format("SauvegardeRole : PionsEnvironnants renvoie une erreur pour le role ID_ROLE={0}, ID_PION={0}", ligneRole.ID_ROLE, lignePion.ID_PION));
                }

                //regarde si le QG est "au contact" de l'ennemi sans protection
                //si le rôle est au contact, il ne peut donner aucun ordre à d'autres unités
                iAucontact = bEnDanger ? 1 : 0;
                if (bExiste)
                {
                    requete = string.Format("UPDATE `tab_vaoc_role` SET `ID_UTILISATEUR`={2}, `S_NOM`='{3}',`ID_PION`={4},`ID_NATION`={5}, `S_UNITES_VISIBLES`='{6}', `B_AU_CONTACT`={7} WHERE (ID_ROLE={0} AND ID_PARTIE={1});",
                                            ligneRole.ID_ROLE,
                                            idPartie,
                                            ligneRole.ID_UTILISATEUR,
                                            ligneRole.S_NOM,
                                            ligneRole.ID_PION,
                                            ligneModele.ID_NATION,
                                            Constantes.ChaineSQL(sUnitesVisibles),
                                            iAucontact);
                                            //"black", à ne pas mettre à jour
                                            //"white" à ne pas mettre à jour
                                            
                }
                else
                {
                    requete = string.Format("INSERT INTO `tab_vaoc_role` (`ID_ROLE`, `ID_PARTIE`, `ID_UTILISATEUR`, `S_NOM`,`ID_PION`,`ID_NATION`,`S_COULEUR_FOND`,`S_COULEUR_TEXTE`, `S_UNITES_VISIBLES`, `B_AU_CONTACT`) VALUES ({0}, {1}, {2}, '{3}', {4}, {5}, '{6}', '{7}', '{8}', {9});",
                                            ligneRole.ID_ROLE,
                                            idPartie,
                                            ligneRole.ID_UTILISATEUR,
                                            ligneRole.S_NOM,
                                            ligneRole.ID_PION,
                                            ligneModele.ID_NATION,
                                            "black",
                                            "white",
                                            Constantes.ChaineSQL(sUnitesVisibles),
                                            iAucontact);
                }

                AjouterLigne(requete);
            }
            //On remet le flag des ordres terminés à 0
            requete = string.Format("UPDATE `tab_vaoc_role` SET `B_ORDRES_TERMINES`=0 WHERE ID_PARTIE={0};",
                                    idPartie);
            AjouterLigne(requete);
        }

        /// <summary>
        /// Sauvegarde de TAB_BATAILLE
        /// </summary>
        /// <param name="idPartie">identifiant de la partie</param>
        public void SauvegardeBataille(int idPartie)
        {
            string requete;
            List<ClassDataRole> listeRoles = ListeRoles(idPartie);
            Donnees.TAB_MODELE_PIONRow ligneModele;
            Donnees.TAB_PIONRow lignePion;

            int bEngagee, bEnDefense, bRetraite, bEngagement;
            string s_terrain0, s_terrain1, s_terrain2, s_terrain3, s_terrain4, s_terrain5;
            string s_couleurTerrain0,s_couleurTerrain1,s_couleurTerrain2,s_couleurTerrain3,s_couleurTerrain4,s_couleurTerrain5;
            string s_obstacle0,s_obstacle1,s_obstacle2,s_couleurObstacle0,s_couleurObstacle1,s_couleurObstacle2;
            string dateFin;
            
            //Batailles
            requete = string.Format("delete from tab_vaoc_bataille WHERE ID_PARTIE={0};",
                                    idPartie);
            AjouterLigne(requete);

            foreach (Donnees.TAB_BATAILLERow ligneBataille in Donnees.m_donnees.TAB_BATAILLE)
            {
                FormatTerrainBataille(ligneBataille.ID_TERRAIN_0, out s_terrain0, out s_couleurTerrain0);
                FormatTerrainBataille(ligneBataille.ID_TERRAIN_1, out s_terrain1, out s_couleurTerrain1);
                FormatTerrainBataille(ligneBataille.ID_TERRAIN_2, out s_terrain2, out s_couleurTerrain2);
                FormatTerrainBataille(ligneBataille.ID_TERRAIN_3, out s_terrain3, out s_couleurTerrain3);
                FormatTerrainBataille(ligneBataille.ID_TERRAIN_4, out s_terrain4, out s_couleurTerrain4);
                FormatTerrainBataille(ligneBataille.ID_TERRAIN_5, out s_terrain5, out s_couleurTerrain5);

                FormatTerrainBataille(ligneBataille.ID_OBSTACLE_03, out s_obstacle0, out s_couleurObstacle0);
                FormatTerrainBataille(ligneBataille.ID_OBSTACLE_14, out s_obstacle1, out s_couleurObstacle1);
                FormatTerrainBataille(ligneBataille.ID_OBSTACLE_25, out s_obstacle2, out s_couleurObstacle2);

                dateFin = ligneBataille.IsI_PHASE_FINNull() ? string.Empty : ClassMessager.DateHeureSQL(ligneBataille.I_TOUR_FIN, ligneBataille.I_PHASE_FIN);

                requete = string.Format("INSERT INTO `tab_vaoc_bataille` (`ID_PARTIE`, `ID_BATAILLE`, `S_NOM`, `DT_BATAILLE_DEBUT`, `C_ORIENTATION`, `S_TERRAIN0`, `S_TERRAIN1`, `S_TERRAIN2`, `S_TERRAIN3`, `S_TERRAIN4`, `S_TERRAIN5`, `S_COULEURTERRAIN0`, `S_COULEURTERRAIN1`, `S_COULEURTERRAIN2`, `S_COULEURTERRAIN3`, `S_COULEURTERRAIN4`, `S_COULEURTERRAIN5`, `S_OBSTACLE0`, `S_OBSTACLE1`, `S_OBSTACLE2`, `S_COULEUROBSTACLE0`, `S_COULEUROBSTACLE1`, `S_COULEUROBSTACLE2`, `ID_NATION_012`, `ID_NATION_345`, `ID_LEADER_012`, `ID_LEADER_345`, `DT_BATAILLE_FIN`, `I_TOUR_DEBUT`, `S_COMBAT_0`, `S_COMBAT_1`, `S_COMBAT_2`, `S_COMBAT_3`, `S_COMBAT_4`, `S_COMBAT_5`) " +
                                        "VALUES ({0}, {1}, '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', {23}, {24}, {25}, {26}, '{27}', {28}, '{29}', '{30}', '{31}', '{32}', '{33}', '{34}');",
                                        idPartie,
                                        ligneBataille.ID_BATAILLE,
                                        Constantes.ChaineSQL(ligneBataille.S_NOM),
                                        ClassMessager.DateHeureSQL(ligneBataille.I_TOUR_DEBUT, ligneBataille.I_PHASE_DEBUT),
                                        ligneBataille.C_ORIENTATION,
                                        s_terrain0,//5
                                        s_terrain1,
                                        s_terrain2,
                                        s_terrain3,
                                        s_terrain4,
                                        s_terrain5,//10
                                        s_couleurTerrain0,
                                        s_couleurTerrain1,
                                        s_couleurTerrain2,
                                        s_couleurTerrain3,
                                        s_couleurTerrain4,//15
                                        s_couleurTerrain5,
                                        s_obstacle0,
                                        s_obstacle1,
                                        s_obstacle2,
                                        s_couleurObstacle0,//20
                                        s_couleurObstacle1,
                                        s_couleurObstacle2,
                                        ligneBataille.ID_NATION_012,
                                        ligneBataille.ID_NATION_345,
                                        ligneBataille.ID_LEADER_012,//25
                                        ligneBataille.ID_LEADER_345,
                                        dateFin,
                                        ligneBataille.I_TOUR_DEBUT,//28
                                        ligneBataille.S_COMBAT_0,
                                        ligneBataille.S_COMBAT_1,//30
                                        ligneBataille.S_COMBAT_2,
                                        ligneBataille.S_COMBAT_3,
                                        ligneBataille.S_COMBAT_4,
                                        ligneBataille.S_COMBAT_5
                                        );

                AjouterLigne(requete);
            }

            //Pions en bataille
            requete = string.Format("delete from tab_vaoc_bataille_pions WHERE ID_PARTIE={0};",
                                    idPartie);
            AjouterLigne(requete);
            foreach (Donnees.TAB_BATAILLE_PIONSRow ligneBataillePion in Donnees.m_donnees.TAB_BATAILLE_PIONS)
            {
                lignePion = Donnees.m_donnees.TAB_PION.FindByID_PION(ligneBataillePion.ID_PION);
                ligneModele = lignePion.modelePion;

                bEnDefense = (ligneBataillePion.IsB_EN_DEFENSENull() || !ligneBataillePion.B_EN_DEFENSE) ? 0 : 1;
                bEngagee = (ligneBataillePion.IsB_ENGAGEENull() || !ligneBataillePion.B_ENGAGEE) ? 0 : 1;
                bRetraite = (ligneBataillePion.IsB_RETRAITENull() || !ligneBataillePion.B_RETRAITE) ? 0 : 1;//l'unité à fait retraite dans le combat
                bEngagement = (ligneBataillePion.IsB_ENGAGEMENTNull() || !ligneBataillePion.B_ENGAGEMENT) ? 0 : 1;//l'unite a été engagée dans le combat

                requete = string.Format("INSERT INTO `tab_vaoc_bataille_pions` (`ID_PARTIE`, `ID_BATAILLE`,`ID_PION`,`ID_NATION`,`B_ENGAGEE`,`B_EN_DEFENSE`" +
                                        ",`I_INFANTERIE_DEBUT`,`I_INFANTERIE_FIN`,`I_CAVALERIE_DEBUT`,`I_CAVALERIE_FIN`,`I_ARTILLERIE_DEBUT`,`I_ARTILLERIE_FIN`,`I_MORAL_DEBUT`,`I_MORAL_FIN`" +
                                        ",`I_FATIGUE_DEBUT`,`I_FATIGUE_FIN`,`B_RETRAITE`,`B_ENGAGEMENT`,`I_ZONE_BATAILLE_ENGAGEMENT`"+
                                        ") VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18});",
                                        idPartie,
                                        ligneBataillePion.ID_BATAILLE,
                                        ligneBataillePion.ID_PION,
                                        ligneModele.ID_NATION,
                                        bEngagee,//4
                                        bEnDefense,
                                        ligneBataillePion.IsI_INFANTERIE_DEBUTNull() ? 0 : ligneBataillePion.I_INFANTERIE_DEBUT,
                                        ligneBataillePion.IsI_INFANTERIE_FINNull() ? 0 : ligneBataillePion.I_INFANTERIE_FIN,
                                        ligneBataillePion.IsI_CAVALERIE_DEBUTNull() ? 0 : ligneBataillePion.I_CAVALERIE_DEBUT,
                                        ligneBataillePion.IsI_CAVALERIE_FINNull() ? 0 : ligneBataillePion.I_CAVALERIE_FIN,
                                        ligneBataillePion.IsI_ARTILLERIE_DEBUTNull() ? 0 : ligneBataillePion.I_ARTILLERIE_DEBUT,//10
                                        ligneBataillePion.IsI_ARTILLERIE_FINNull() ? 0 : ligneBataillePion.I_ARTILLERIE_FIN,
                                        ligneBataillePion.IsI_MORAL_DEBUTNull() ? 0 : ligneBataillePion.I_MORAL_DEBUT,
                                        ligneBataillePion.IsI_MORAL_FINNull() ? 0 : ligneBataillePion.I_MORAL_FIN,
                                        ligneBataillePion.IsI_FATIGUE_DEBUTNull() ? 0 : ligneBataillePion.I_FATIGUE_DEBUT,
                                        ligneBataillePion.IsI_FATIGUE_FINNull() ? 0 : ligneBataillePion.I_FATIGUE_FIN,//15
                                        bRetraite,
                                        bEngagement,
                                        ligneBataillePion.IsI_ZONE_BATAILLE_ENGAGEMENTNull() ? -1 : ligneBataillePion.I_ZONE_BATAILLE_ENGAGEMENT//18
                                        );

                AjouterLigne(requete);
            }
        }

        /// <summary>
        /// Sauvegarde des objectifs de victoire d'une partie
        /// </summary>
        /// <param name="idPartie">identifiant de la partie</param>
        public void SauvegardeObjectifs(int idPartie)
        {
            string requete;
            int id=0;

            //Supression des valeurs précédentes
            requete = string.Format("delete from tab_vaoc_objectifs WHERE ID_PARTIE={0};",idPartie);
            AjouterLigne(requete);

            foreach (Donnees.TAB_NOMS_CARTERow ligneNomCarte in Donnees.m_donnees.TAB_NOMS_CARTE)
            {
                if (ligneNomCarte.I_VICTOIRE>0)
                {
                    requete = string.Format("INSERT INTO `tab_vaoc_objectifs` (`ID_PARTIE`, `ID_OBJECTIF`, `S_NOM`, `I_VICTOIRE`, `ID_NATION`) " +
                                    "VALUES ({0}, {1}, '{2}', {3}, {4});",
                                    idPartie,
                                    id++,
                                    ligneNomCarte.S_NOM,
                                    ligneNomCarte.I_VICTOIRE,
                                    (ligneNomCarte.IsID_NATION_CONTROLENull() || ligneNomCarte.ID_NATION_CONTROLE<0) ? -1 : ligneNomCarte.ID_NATION_CONTROLE
                                    );

                    AjouterLigne(requete);
                }
            }
        }
        #endregion
        #endregion

        #region Méthodes internes à la classe
        /// <summary>
        /// write a sql line at the end of the lile
        /// </summary>
        /// <param name="line">sql line to add</param>
        private void AjouterLigne(string line)
        {
            StreamWriter file;
            file = new StreamWriter(m_fileNameSQL, true);
            file.WriteLine(line);
            file.Close();
        }

        private void FormatTerrainBataille(int idTerrain, out string sTerrain, out string sCouleur)
        {
            Donnees.TAB_MODELE_TERRAINRow ligneTerrain;
            Donnees.TAB_GRAPHISMERow ligneGraphique;
            Donnees.TAB_POINTRow lignePoint;

            if (idTerrain < 0)
            {
                //s'il n'y a pas d'obstacle déclaré, on prend le terrain "par défaut"
                idTerrain = Donnees.m_donnees.TAB_JEU[0].ID_MODELE_TERRAIN_DEPLOIEMENT;
            }
            ligneTerrain = Donnees.m_donnees.TAB_MODELE_TERRAIN.FindByID_MODELE_TERRAIN(idTerrain);
            ligneGraphique = Donnees.m_donnees.TAB_GRAPHISME.FindByID_GRAPHIQUE(ligneTerrain.ID_GRAPHIQUE);
            lignePoint = Donnees.m_donnees.TAB_POINT.FindByID_POINT(ligneGraphique.ID_POINT);
            // en fait, les modificateurs de défense vont en moins, pas en plus
            sTerrain = (0 == ligneTerrain.I_MODIFICATEUR_DEFENSE) ? ligneTerrain.S_NOM : string.Format("{0}(-{1})", ligneTerrain.S_NOM, ligneTerrain.I_MODIFICATEUR_DEFENSE);
            //sTerrain = (0 == ligneTerrain.I_MODIFICATEUR_DEFENSE) ? ligneTerrain.S_NOM : string.Format("{0}({1:+0;-0})", ligneTerrain.S_NOM, ligneTerrain.I_MODIFICATEUR_DEFENSE);
            sCouleur = "#" + lignePoint.I_ROUGE.ToString("X2") + lignePoint.I_VERT.ToString("X2") + lignePoint.I_BLEU.ToString("X2");
        }
        #endregion

    }
}
