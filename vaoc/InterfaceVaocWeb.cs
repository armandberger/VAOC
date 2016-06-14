using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    interface InterfaceVaocWeb
    {
        #region mise à jour des données
        void TraitementEnCours(bool bTraitementEnCours, int idJeu, int idPartie);

        /// <summary>
        /// Sauvegarde de TAB_NOMS_CARTE
        /// </summary>
        /// <param name="idPartie">identifiant de la partie</param>
        void SauvegardeNomsCarte(int idPartie);

        void SauvegardeMessage(int idPartie);

        /// <summary>
        /// Sauvegarde TAB_METEO. Utile uniquement dans l'information de scénario des joueurs
        /// </summary>
        /// <param name="idJeu">identifiant du Jeu</param>
        void SauvegardeMeteo(int idJeu);

        /// <summary>
        /// Sauvegarde de TAB_MODELE_MOUVEMENT. Utile uniquement dans l'information de scénario des joueurs 
        /// </summary>
        /// <param name="idJeu">identifiant du Jeu</param>
        void SauvegardeModelesMouvement(int idJeu);

        /// <summary>
        /// Sauvegarde de TAB_PION
        /// </summary>
        /// <param name="idPartie">identifiant de la partie</param>
        void SauvegardePion(int idPartie);
    
        /// <summary>
        /// Sauvegarde de TAB_MODELE_PION
        /// </summary>
        /// <param name="idPartie">identifiant de la partie</param>
        void SauvegardeModelesPion(int idPartie);
    
        /// <summary>
        /// Sauvegarde de TAB_PARTIE
        /// </summary>
        /// <param name="idPartie">identifiant de la partie</param>
        void SauvegardePartie(int idPartie);

        /// <summary>
        /// Sauvegarde de TAB_NATION
        /// </summary>
        /// <param name="idPartie">identifiant de la partie</param>
        void SauvegardeNation(int idPartie);

        /// <summary>
        /// Sauvegarde de TAB_ROLE
        /// </summary>
        /// <param name="idPartie">identifiant de la partie</param>
        void SauvegardeRole(int idPartie);

        /// <summary>
        /// Sauvegarde de tab_vaoc_forum
        /// </summary>
        /// <param name="idPartie">identifiant de la partie</param>
        void SauvegardeForum(int idPartie);

        void SauvegardeBataille(int idPartie);

        /// <summary>
        /// Sauvegarde des objectifs de victoire d'une partie
        /// </summary>
        /// <param name="idPartie">identifiant de la partie</param>
        void SauvegardeObjectifs(int idPartie);
        #endregion

        #region lecture des données
        List<ClassDataRole> ListeRoles(int idPartie);
        List<ClassDataJeu> ListeJeux();
        ClassDataJeu GetJeu(int idJeu);
        List<ClassDataPartie> ListeParties();
        List<ClassDataPartie> ListeParties(int idJeu, int idPartie = -1);
        ClassDataPartie GetPartie(int idPartie);
        List<ClassDataMessage> ListeMessages(int idPartie);
        List<ClassDataOrdre> ListeOrdres(int idJeu, int idPartie);
        List<ClassDataModeles> ListeModeles(int idPartie);
        List<ClassDataUtilisateur> ListeUtilisateurs(bool bPresent);
        ClassDataUtilisateur GetUtilisateur(int id_utilisateur);
        ClassDataUtilisateur GetUtilisateur(string s_login);
        #endregion
    }
}
