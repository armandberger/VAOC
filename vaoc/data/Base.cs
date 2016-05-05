using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    [Serializable]
    class BD
    {
        public static BD Base = new BD();

        private TableCASE tableCase;
        private TablePCC_COUTS tablePccCouts;
        private TableJEU tableJeu;
        private TablePION tablePion;
        private TableMODELE_MOUVEMENT tableModeleMouvement;
        private TableMOUVEMENT_COUT tableMouvementCout;
        private TableMODELE_TERRAIN tableModeleTerrain;
        private TablePARTIE tablePartie;
        private TableORDRE tableOrdre;
        private TableMESSAGE tableMessage;
        private TableBATAILLE tableBataille;

        public TableBATAILLE Bataille { get { return tableBataille; } }
        public TableMESSAGE Message { get { return tableMessage; } }
        public TableORDRE Ordre { get { return tableOrdre; } }
        public TablePARTIE Partie { get { return tablePartie; } }
        public TableCASE Case { get { return tableCase; } }
        public TablePCC_COUTS PccCouts { get { return tablePccCouts; }}
        public TableJEU Jeu { get { return tableJeu; }}
        public TablePION Pion { get { return tablePion; }}
        public TableMODELE_MOUVEMENT ModeleMouvement { get { return tableModeleMouvement; } }
        public TableMOUVEMENT_COUT MouvementCout { get { return tableMouvementCout; } }
        public TableMODELE_TERRAIN ModeleTerrain { get { return tableModeleTerrain; } }

        public void Initialisation(int largeur, int hauteur)
        {
            tableCase = new TableCASE(largeur, hauteur);
            tablePccCouts = new TablePCC_COUTS();
            tableJeu = new TableJEU();
            tablePion = new TablePION();
            tableModeleMouvement = new TableMODELE_MOUVEMENT();
            tableMouvementCout = new TableMOUVEMENT_COUT();
            tableModeleTerrain = new TableMODELE_TERRAIN();
            tablePartie = new TablePARTIE();
            tableOrdre = new TableORDRE();
            tableMessage = new TableMESSAGE();
            tableBataille = new TableBATAILLE();
        }

        public bool Importer(Donnees donnees)
        {
            if (!tableCase.Importer(donnees.TAB_CASE)) { return false; }
            if (!tablePion.Importer(donnees.TAB_PION)) { return false; }
            if (!tablePccCouts.Importer(donnees.TAB_PCC_COUTS)) { return false; }
            if (!tableJeu.Importer(donnees.TAB_JEU)) { return false; }
            if (!tableModeleMouvement.Importer(donnees.TAB_MODELE_MOUVEMENT)) { return false; }
            if (!tableMouvementCout.Importer(donnees.TAB_MOUVEMENT_COUT)) { return false; }
            if (!tableModeleTerrain.Importer(donnees.TAB_MODELE_TERRAIN)) { return false; }
            if (!tablePartie.Importer(donnees.TAB_PARTIE)) { return false; }
            if (!tableOrdre.Importer(donnees.TAB_ORDRE)) { return false; }
            if (!tableMessage.Importer(donnees.TAB_MESSAGE)) { return false; }
            if (!tableBataille.Importer(donnees.TAB_BATAILLE)) { return false; }
            return true;
        }

        public bool Exporter(Donnees donnees)
        {
            if (!tableCase.Exporter(donnees.TAB_CASE)) { return false; }
            if (!tablePion.Exporter(donnees.TAB_PION)) { return false; }
            if (!tablePccCouts.Exporter(donnees.TAB_PCC_COUTS)) { return false; }
            if (!tableJeu.Exporter(donnees.TAB_JEU)) { return false; }
            if (!tableModeleMouvement.Exporter(donnees.TAB_MODELE_MOUVEMENT)) { return false; }
            if (!tableMouvementCout.Exporter(donnees.TAB_MOUVEMENT_COUT)) { return false; }
            if (!tableModeleTerrain.Exporter(donnees.TAB_MODELE_TERRAIN)) { return false; }
            if (!tablePartie.Exporter(donnees.TAB_PARTIE)) { return false; }
            if (!tableOrdre.Exporter(donnees.TAB_ORDRE)) { return false; }
            if (!tableMessage.Exporter(donnees.TAB_MESSAGE)) { return false; }
            if (!tableBataille.Exporter(donnees.TAB_BATAILLE)) { return false; }
            return true;
        }
    }
}
