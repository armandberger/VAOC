using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormForcesInitiales : Form
    {
        public FormForcesInitiales()
        {
            InitializeComponent();
        }

        private void FormForcesInitiales_Load(object sender, EventArgs e)
        {
            string[] libelleNiveau = new string[] { "milice", "milice","recrues","standard","supérieur","élite","élite","élite","élite" };
            string str=string.Empty;
            foreach (Donnees.TAB_NATIONRow ligneNation in Donnees.m_donnees.TAB_NATION)
            {
                str += ligneNation.S_NOM + Environment.NewLine;
                str += "Rôle".PadLeft(40)+"Divisions".PadLeft(10)+"Inf.".PadLeft(10)+ "Cav.".PadLeft(10)+"Art.".PadLeft(10)+"Qualité".PadLeft(20) + Environment.NewLine;
                foreach (Donnees.TAB_PIONRow ligneLeader in Donnees.m_donnees.TAB_PION.ToList().OrderBy(l => l.ID_PION))
                {
                    if (ligneLeader.estQG && ligneLeader.nation.ID_NATION==ligneNation.ID_NATION)
                    {
                        int nbDivisions = 0;
                        int nbFantassins = 0;
                        int nbCavaliers = 0;
                        int nbArtillerie = 0;
                        int moral = 0;
                        decimal experience = 0;
                        foreach (Donnees.TAB_PIONRow lignePion in Donnees.m_donnees.TAB_PION)
                        {
                            if (lignePion.ID_PION_PROPRIETAIRE== ligneLeader.ID_PION && lignePion.estCombattif)
                            {
                                if (!lignePion.estArtillerie)
                                {
                                    nbDivisions++;
                                    moral += lignePion.I_MORAL_MAX;
                                    experience += lignePion.I_EXPERIENCE;
                                }
                                nbFantassins += lignePion.I_INFANTERIE;
                                nbCavaliers += lignePion.I_CAVALERIE;
                                nbArtillerie += lignePion.I_ARTILLERIE;
                            }
                        }
                        if (nbDivisions > 0)
                        {
                            moral /= nbDivisions;
                            experience /= nbDivisions;
                            int niveau = moral / 10; //pour donner un chiffre entre 1 et 5 normalement
                            niveau += (int)Math.Round(experience);
                            str += ligneLeader.S_NOM.PadLeft(40) + nbDivisions.ToString().PadLeft(10) + nbFantassins.ToString().PadLeft(10) + nbCavaliers.ToString().PadLeft(10) + nbArtillerie.ToString().PadLeft(10) + libelleNiveau[niveau].PadLeft(20) + Environment.NewLine;
                        }
                    }
                }
            }
            textBoxResultat.Text = str;
        }
    }
}
