using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vaoc
{
    class ClassDataTest
    {
        static public List<ClassDataTest> liste = new List<ClassDataTest>();

        public int ID_MODELE_TERRAIN {get; set;}
        public int I_X  {get; set;}
        public int I_Y {get; set;}
        public int ID_PROPRIETAIRE {get; set;}
        public int ID_NOUVEAU_PROPRIETAIRE {get; set;}
        public int ID_MODELE_TERRAIN_SI_OCCUPE {get; set;}
        public Double I_COUT {get; set;}
        public int ID_NOM { get; set;}

        public ClassDataTest(int pID_MODELE_TERRAIN,
        int pI_X ,
        int pI_Y,
        int pID_PROPRIETAIRE,
        int pID_NOUVEAU_PROPRIETAIRE,
        int pID_MODELE_TERRAIN_SI_OCCUPE,
        Double pI_COUT,
        int pID_NOM)
        {
            ID_MODELE_TERRAIN= pID_MODELE_TERRAIN;
            I_X = pI_X;
            I_Y= pI_Y;
            ID_PROPRIETAIRE= pID_PROPRIETAIRE;
            ID_NOUVEAU_PROPRIETAIRE= pID_NOUVEAU_PROPRIETAIRE;
            ID_MODELE_TERRAIN_SI_OCCUPE= pID_MODELE_TERRAIN_SI_OCCUPE;
            I_COUT= pI_COUT;
            ID_NOM = pID_NOM;
        }
    }
}
