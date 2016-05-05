using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormNouveauModeleTerrain : Form
    {
        public bool enCreation
        {
            set
            {
                if (value)
                {
                    //création
                    textBoxIdentifiant.Enabled = false;
                    buttonCreer.Text = "Créer";
                }
                else
                {
                    //modification
                    textBoxIdentifiant.Enabled = true;
                    buttonCreer.Text = "Modifier";
                }
            }
        }

        public int identifiant
        {
            get
            {
                return Convert.ToInt32(textBoxIdentifiant.Text);
            }
            set
            {
                textBoxIdentifiant.Text = Convert.ToString(value);
            }
        }

        public string nomTerrain 
        { 
            get
            {
                return textBoxNom.Text;
            }
            set
            {
                textBoxNom.Text = value;
            }
        }

        public bool bPont 
        { 
            get
            {
                return checkBoxPont.Checked;
            }
            set
            {
                checkBoxPont.Checked = value;
            }
        }

        public bool bPonton
        {
            get
            {
                return checkBoxPonton.Checked;
            }
            set
            {
                checkBoxPonton.Checked = value;
            }
        }

        public bool bDetruit
        {
            get
            {
                return checkBoxDetruit.Checked;
            }
            set
            {
                checkBoxDetruit.Checked = value;
            }
        }

        public bool bAnnuleeSiOcccupee
        { 
            get
            {
                return checkBoxAnnuleeSiOccupee.Checked;
            }
            set
            {
                checkBoxAnnuleeSiOccupee.Checked = value;
            }
        }

        public bool bRoutier
        { 
            get
            {
                return checkBoxRoute.Checked;
            }
            set
            {
                checkBoxRoute.Checked = value;
            }
        }

        public bool bObstacle
        {
            get
            {
                return checkBoxObstacle.Checked;
            }
            set
            {
                checkBoxObstacle.Checked = value;
            }
        }

        public bool bAnnuleEnCombat
        {
            get
            {
                return checkBoxAnnuleEnCombat.Checked;
            }
            set
            {
                checkBoxAnnuleEnCombat.Checked = value;
            }
        }

        public int modificateurDefense
        { 
            get
            {
                return Convert.ToInt32(textBoxDefense.Text);
            }
            set
            {
                textBoxDefense.Text = Convert.ToString(value);
            }
        }

        public int CouleurR
        { 
            get
            {
                return Convert.ToInt32(textBoxR.Text);
            }
            set
            {
                textBoxR.Text = Convert.ToString(value);
            }
        }

        public int CouleurG
        {
            get
            {
                return Convert.ToInt32(textBoxG.Text);
            }
            set
            {
                textBoxG.Text = Convert.ToString(value);
            }
        }

        public int CouleurB
        {
            get
            {
                return Convert.ToInt32(textBoxB.Text);
            }
            set
            {
                textBoxB.Text = Convert.ToString(value);
            }
        }

        public FormNouveauModeleTerrain()
        {
            InitializeComponent();
        }

    }
}
