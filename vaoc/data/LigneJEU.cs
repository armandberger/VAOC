using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    [Serializable]
    class LigneJEU
    {
        public int ID_JEU { get; set; }
        public string S_NOM { get; set; }
        public int I_NOMBRE_TOURS { get; set; }
        public int I_NOMBRE_PHASES { get; set; }
        public DateTime DT_INITIALE { get; set; }
        //public int ID_CARTE_COMPTE_RENDU { get; set; }
        public short I_LEVER_DU_SOLEIL { get; set; }
        public short I_COUCHER_DU_SOLEIL { get; set; }
        public int I_ECHELLE { get; set; }
        public int ID_MODELE_TERRAIN_DEPLOIEMENT { get; set; }
        public int I_COUT_DE_BASE { get; set; }
        public short I_RAYON_BATAILLE { get; set; }
        public short I_HEURE_INITIALE { get; set; }
        public int I_HAUTEUR_CARTE { get; set; }
        public int I_LARGEUR_CARTE { get; set; }
        public string S_NOM_CARTE_TOPOGRAPHIQUE { get; set; }
        public string S_NOM_CARTE_ZOOM { get; set; }
        public string S_NOM_CARTE_GRIS { get; set; }
        public string S_NOM_CARTE_HISTORIQUE { get; set; }
        public int I_OBJECTIF { get; set; }
        public int I_TAILLEBLOC_PCC { get; set; }
        public int I_ZONEVILLE_PCC { get; set; }
        public int I_DISTANCEVILLEMAX_PCC { get; set; }
        public ushort I_VERSION { get; set; }

        public LigneJEU(
            int idJeu,
            string nom,
            int nombreTours,
            int nombrePhases,
            DateTime dateInitiale,
            //int idCarteCompteRendu,
            short leverDuSioleil,
            short coucherDuSoleil,
            int echelle,
            int idModeleTerrainDeploiement,
            int coutDeBase,
            short rayonBataille,
            short heureInitiale,
            int hauteurCarte,
            int largeurCarte,
            string nomCarteTopographique,
            string nomCarteZoom,
            string nomCarteGris,
            string nomCarteHistorique,
            int objectif,
            int tailleBlocPCC,
            //int zoneVillePCC,
            //int distanceVilleMaxPCC,
            ushort version
        )
        {
            ID_JEU = idJeu;
            S_NOM = nom;
            I_NOMBRE_TOURS= nombreTours;
            I_NOMBRE_PHASES= nombrePhases;
            DT_INITIALE= dateInitiale;
            //ID_CARTE_COMPTE_RENDU= idCarteCompteRendu;
            I_LEVER_DU_SOLEIL= leverDuSioleil;
            I_COUCHER_DU_SOLEIL= coucherDuSoleil;
            I_ECHELLE= echelle ;
            ID_MODELE_TERRAIN_DEPLOIEMENT= idModeleTerrainDeploiement;
            I_COUT_DE_BASE= coutDeBase;
            I_RAYON_BATAILLE= rayonBataille;
            I_HEURE_INITIALE= heureInitiale;
            I_HAUTEUR_CARTE= hauteurCarte;
            I_LARGEUR_CARTE= largeurCarte;
            S_NOM_CARTE_TOPOGRAPHIQUE= nomCarteTopographique;
            S_NOM_CARTE_ZOOM= nomCarteZoom;
            S_NOM_CARTE_GRIS= nomCarteGris;
            S_NOM_CARTE_HISTORIQUE= nomCarteHistorique;
            I_OBJECTIF= objectif;
            I_TAILLEBLOC_PCC= tailleBlocPCC;
            //I_ZONEVILLE_PCC= zoneVillePCC;
            //I_DISTANCEVILLEMAX_PCC= distanceVilleMaxPCC;
            I_VERSION= version;
        }

        public LigneJEU(Donnees.TAB_JEURow ligneXML)
        {
            ID_JEU = ligneXML.ID_JEU;
            S_NOM = ligneXML.S_NOM;
            I_NOMBRE_TOURS = ligneXML.I_NOMBRE_TOURS;
            I_NOMBRE_PHASES = ligneXML.I_NOMBRE_PHASES;
            DT_INITIALE = ligneXML.DT_INITIALE;
            //ID_CARTE_COMPTE_RENDU= idCarteCompteRendu;
            I_LEVER_DU_SOLEIL = ligneXML.I_LEVER_DU_SOLEIL;
            I_COUCHER_DU_SOLEIL = ligneXML.I_COUCHER_DU_SOLEIL;
            I_ECHELLE = ligneXML.I_ECHELLE;
            ID_MODELE_TERRAIN_DEPLOIEMENT = ligneXML.ID_MODELE_TERRAIN_DEPLOIEMENT;
            I_COUT_DE_BASE = ligneXML.I_COUT_DE_BASE;
            I_RAYON_BATAILLE = ligneXML.I_RAYON_BATAILLE;
            I_HEURE_INITIALE = ligneXML.I_HEURE_INITIALE;
            I_HAUTEUR_CARTE = ligneXML.I_HAUTEUR_CARTE;
            I_LARGEUR_CARTE = ligneXML.I_LARGEUR_CARTE;
            S_NOM_CARTE_TOPOGRAPHIQUE = ligneXML.S_NOM_CARTE_TOPOGRAPHIQUE;
            S_NOM_CARTE_ZOOM = ligneXML.S_NOM_CARTE_ZOOM;
            S_NOM_CARTE_GRIS = ligneXML.S_NOM_CARTE_GRIS;
            S_NOM_CARTE_HISTORIQUE = ligneXML.S_NOM_CARTE_HISTORIQUE;
            I_OBJECTIF = ligneXML.I_OBJECTIF;
            I_TAILLEBLOC_PCC = ligneXML.I_TAILLEBLOC_PCC;
            //I_ZONEVILLE_PCC= zoneVillePCC;
            //I_DISTANCEVILLEMAX_PCC= distanceVilleMaxPCC;
            I_VERSION = ligneXML.I_VERSION;
        }
    }
}
