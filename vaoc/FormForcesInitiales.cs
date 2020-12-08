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
            ForcesInitiales(false);
        }

        private void ForcesInitiales(bool bCampagne)
        {
            string[] libelleNiveau = new string[] { "milice", "milice","recrues","standard","supérieur","élite","élite","élite","élite" };
            string str=string.Empty;
            foreach (Donnees.TAB_NATIONRow ligneNation in Donnees.m_donnees.TAB_NATION)
            {
                if (bCampagne)
                {
                    str += "<table>";
                    str += "<tr><th>Rôle</th><th>Divisions</th><th>Inf.</th><th>Cav.</th><th>Art.</th><th>Qualité</th><th>Localisation</th></tr>" + Environment.NewLine;
                }
                else
                {
                    str += ligneNation.S_NOM + Environment.NewLine;
                    str += "Rôle".PadLeft(40) + "Divisions".PadLeft(10) + "Inf.".PadLeft(10) + "Cav.".PadLeft(10) + "Art.".PadLeft(10) + "Qualité".PadLeft(20) + Environment.NewLine;
                }
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
                            if (bCampagne)
                            {
                                str += "<tr><td>"+ligneLeader.S_NOM + "</td><td>"+nbDivisions + "</td><td>" + nbFantassins + "</td><td>" + nbCavaliers + "</td><td>" + nbArtillerie + "</td><td>" + libelleNiveau[niveau] + "</td><td></td></tr>" + Environment.NewLine;
                            }
                            else
                            {
                                str += ligneLeader.S_NOM.PadLeft(40) + nbDivisions.ToString().PadLeft(10) + nbFantassins.ToString().PadLeft(10) + nbCavaliers.ToString().PadLeft(10) + nbArtillerie.ToString().PadLeft(10) + libelleNiveau[niveau].PadLeft(20) + Environment.NewLine;
                            }
                        }
                    }
                }
                if (bCampagne)
                {
                    str += "</table>" + Environment.NewLine;
                }
                //La même chose avec les renforts
                str += "Renforts" + Environment.NewLine;
                foreach (Donnees.TAB_RENFORTRow ligneLeader in Donnees.m_donnees.TAB_RENFORT.ToList().OrderBy(l => l.ID_PION))
                {
                    if (ligneLeader.estQG && ligneLeader.nation.ID_NATION == ligneNation.ID_NATION)
                    {
                        int nbDivisions = 0;
                        int nbFantassins = 0;
                        int nbCavaliers = 0;
                        int nbArtillerie = 0;
                        int moral = 0;
                        decimal experience = 0;
                        foreach (Donnees.TAB_RENFORTRow lignePion in Donnees.m_donnees.TAB_RENFORT)
                        {
                            if (lignePion.ID_PION_PROPRIETAIRE == ligneLeader.ID_PION && lignePion.estCombattif)
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
                            //ajout de la date d'arrivée du renfort
                            str += "Le " + ClassMessager.DateHeure(ligneLeader.I_TOUR_ARRIVEE, 0) + Environment.NewLine;
                            str += ligneLeader.S_NOM.PadLeft(40) + nbDivisions.ToString().PadLeft(10) + nbFantassins.ToString().PadLeft(10) + nbCavaliers.ToString().PadLeft(10) + nbArtillerie.ToString().PadLeft(10) + libelleNiveau[niveau].PadLeft(20) + Environment.NewLine;
                        }
                    }
                }
            }
            textBoxResultat.Text = str;
        }

        private void checkBoxCampagne_CheckedChanged(object sender, EventArgs e)
        {
            ForcesInitiales(checkBoxCampagne.Checked);
        }
    }
}
