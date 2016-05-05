using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WaocLib;


namespace vaoc
{
    public partial class FormNomCarte : Form
    {
        private bool m_supprimer;
        private int m_ID_NOM;
        private Donnees.TAB_POLICEDataTable m_tablePolice;
        private const string CST_RENFORT_AU_PLUS_PROCHE = "Le plus proche";
        public FormNomCarte()
        {
            InitializeComponent();
            m_supprimer = false;
            m_ID_NOM = Constantes.CST_IDNULL;

            //mise à jour de la liste de selection des unités
            this.comboBoxPionPropriétaire.Items.Clear();

            this.comboBoxPionPropriétaire.Items.Add(CST_RENFORT_AU_PLUS_PROCHE);
            IEnumerable<Donnees.TAB_PIONRow> requete =
                from pion in Donnees.m_donnees.TAB_PION
                orderby pion.ID_PION
                select pion;

            foreach (Donnees.TAB_PIONRow lignePion in requete)
            {
                this.comboBoxPionPropriétaire.Items.Add(lignePion);
            }

            //Mise à jour de la liste des modeles
            comboBoxModeleRenfort.Items.Clear();
            foreach (Donnees.TAB_MODELE_PIONRow ligneModele in Donnees.m_donnees.TAB_MODELE_PION)
            {
                comboBoxModeleRenfort.Items.Add(ligneModele);
            }

            //Mise à jour de la liste des propriétaires
            foreach (Donnees.TAB_NATIONRow ligneNation in Donnees.m_donnees.TAB_NATION)
            {
                this.comboBoxProprietaire.Items.Add(ligneNation);
            }
        }

        #region propriétés
        public int X
        {
            get { return Convert.ToInt32(labelX.Text); }
            set { labelX.Text = Convert.ToString(value); }
        }

        public int Y
        {
            get { return Convert.ToInt32(labelY.Text); }
            set { labelY.Text = Convert.ToString(value); }
        }

        public bool suppression
        {
            get { return m_supprimer; }
        }

        public int id_nom
        {
            get { return m_ID_NOM; }
            set { 
                m_ID_NOM = value;
                if (Constantes.CST_IDNULL != m_ID_NOM)
                {
                    Donnees.TAB_NOMS_CARTERow ligneNomCarte;
                    ligneNomCarte = Donnees.m_donnees.TAB_NOMS_CARTE.FindByID_NOM(m_ID_NOM);
                    textBoxNom.Text = ligneNomCarte.S_NOM;
                    textBoxIDNOM.Text = ligneNomCarte.ID_NOM.ToString();
                    this.buttonSupprimer.Enabled = true;
                }
                else
                {
                    this.buttonSupprimer.Enabled = false;
                }
            }
        }

        public int id_police
        {
            set 
            {
                int i = 0;
                while (i < comboBoxPolice.Items.Count && m_tablePolice[i].ID_POLICE != value) i++;
                if (i < comboBoxPolice.Items.Count) comboBoxPolice.SelectedIndex = i;
            }
            get { return m_tablePolice[comboBoxPolice.SelectedIndex].ID_POLICE; }
        }

        public short position
        {
            set { comboBoxPosition.SelectedIndex = value; }
            get { return (short)comboBoxPosition.SelectedIndex; }
        }

        public string nom
        {
            get { return textBoxNom.Text; }
        }

        public Donnees.TAB_POLICEDataTable tablePolice
        {
            set
            {
                m_tablePolice = (Donnees.TAB_POLICEDataTable)value.Copy();
                comboBoxPolice.Items.Clear();
                for (int i=0; i<m_tablePolice.Rows.Count; i++)
                {
                    comboBoxPolice.Items.Add(m_tablePolice[i].S_NOM);
                }
            }
        }

        public int idcase
        {
            set
            {
                textBoxIDCASE.Text = value.ToString();
            }
        }

        public int victoire
        {
            set { textBoxVictoire.Text = value.ToString(); }
            get {
                try
                {
                    return Convert.ToInt32(textBoxVictoire.Text);
                }
                catch(Exception)
                {
                    return 0;
                }
            }
        }

        public Donnees.TAB_PIONRow pionProprietaireRenfort
        {
            set
            {
                if (null != value)
                {
                    int i = 0;
                    while (i < comboBoxPionPropriétaire.Items.Count && comboBoxPionPropriétaire.Items[i] != value) i++;
                    if (i < comboBoxPionPropriétaire.Items.Count) comboBoxPionPropriétaire.SelectedIndex = i;
                }
            }
            get 
            {
                //if (string.Equals(CST_RENFORT_AU_PLUS_PROCHE, (string)comboBoxPionPropriétaire.Items[comboBoxPionPropriétaire.SelectedIndex]))
                if (comboBoxPionPropriétaire.Items[comboBoxPionPropriétaire.SelectedIndex] is string)
                {
                    return null;
                }
                return (Donnees.TAB_PIONRow)comboBoxPionPropriétaire.Items[comboBoxPionPropriétaire.SelectedIndex];
            }
        }

        public Donnees.TAB_NATIONRow proprietaire
        {
            set
            {
                if (null != value)
                {
                    int i = 0;
                    while (i < comboBoxProprietaire.Items.Count && comboBoxProprietaire.Items[i] != value) i++;
                    if (i < comboBoxProprietaire.Items.Count) comboBoxProprietaire.SelectedIndex = i;
                }
            }
            get
            {
                //if (string.Equals(CST_RENFORT_AU_PLUS_PROCHE, (string)comboBoxPionPropriétaire.Items[comboBoxPionPropriétaire.SelectedIndex]))
                if (comboBoxProprietaire.SelectedIndex<0 || comboBoxProprietaire.Items[comboBoxProprietaire.SelectedIndex] is string)
                {
                    return null;
                }
                return (Donnees.TAB_NATIONRow)comboBoxProprietaire.Items[comboBoxProprietaire.SelectedIndex];
            }
        }

        public Donnees.TAB_MODELE_PIONRow modelePionRenfort
        {
            set
            {
                if (null != value)
                {
                    int i = 0;
                    while (i < comboBoxModeleRenfort.Items.Count && comboBoxModeleRenfort.Items[i] != value) i++;
                    if (i < comboBoxModeleRenfort.Items.Count) comboBoxModeleRenfort.SelectedIndex = i;
                }
            }
            get
            {
                return (Donnees.TAB_MODELE_PIONRow)comboBoxModeleRenfort.Items[comboBoxModeleRenfort.SelectedIndex];
            }
        }

        public bool prison
        {
            set { this.checkBoxPrison.Checked = value; }
            get { return this.checkBoxPrison.Checked; }
        }

        public bool hopital
        {
            set { this.checkBoxHopital.Checked = value; }
            get { return this.checkBoxHopital.Checked; }
        }

        public int chanceRenfort
        {
            set { textBoxPourcentArriveeRenfort.Text = value.ToString(); }
            get { return Convert.ToInt32(textBoxPourcentArriveeRenfort.Text); }
        }

        public int infanterie
        {
            set { textBoxRenfortFantassins.Text = value.ToString(); }
            get { return Convert.ToInt32(textBoxRenfortFantassins.Text); }
        }

        public int cavalerie
        {
            set { textBoxRenfortCavaliers.Text = value.ToString(); }
            get { return Convert.ToInt32(textBoxRenfortCavaliers.Text); }
        }

        public int artillerie
        {
            set { textBoxRenfortCanons.Text = value.ToString(); }
            get { return Convert.ToInt32(textBoxRenfortCanons.Text); }
        }

        public int materiel
        {
            set { textBoxRenfortMateriel.Text = value.ToString(); }
            get { return Convert.ToInt32(textBoxRenfortMateriel.Text); }
        }

        public int ravitaillement
        {
            set { textBoxRenfortRavitaillement.Text = value.ToString(); }
            get { return Convert.ToInt32(textBoxRenfortRavitaillement.Text); }
        }

        public int moral
        {
            set { textBoxRenfortMoral.Text = value.ToString(); }
            get { return Convert.ToInt32(textBoxRenfortMoral.Text); }
        }

        #endregion

        private void buttonSupprimer_Click(object sender, EventArgs e)
        {
            m_supprimer = true;
        }

        private void FormNomCarte_Load(object sender, EventArgs e)
        {
            if (comboBoxPosition.Items.Count > 0 && comboBoxPosition.SelectedIndex<0)
            {
                comboBoxPosition.SelectedIndex = 0;
            }
            if (comboBoxPolice.Items.Count > 0 && comboBoxPolice.SelectedIndex<0)
            {
                comboBoxPolice.SelectedIndex = 0;
            }
            if (comboBoxPionPropriétaire.Items.Count > 0 && comboBoxPionPropriétaire.SelectedIndex < 0)
            {
                comboBoxPionPropriétaire.SelectedIndex = 0;
            }
            if (comboBoxModeleRenfort.Items.Count > 0 && comboBoxModeleRenfort.SelectedIndex < 0)
            {
                comboBoxModeleRenfort.SelectedIndex = 0;
            }            
        }

        private void buttonSupprimerTout_Click(object sender, EventArgs e)
        {
            Donnees.m_donnees.TAB_NOMS_CARTE.Clear();
            MessageBox.Show("Tous les noms ont été supprimés", "Suppression de tous les noms", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.Close();
        }
    }
}