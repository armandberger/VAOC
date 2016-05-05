using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormModelesMouvements : Form
    {
        public FormModelesMouvements()
        {
            InitializeComponent();
        }

        #region propriétés
        public Donnees.TAB_MODELE_MOUVEMENTDataTable tableModeleMouvement
        {
            get{
                return (Donnees.TAB_MODELE_MOUVEMENTDataTable)dataGridViewModelesMouvements.DataSource;
            }
            set
            {
                dataGridViewModelesMouvements.DataSource = value.Copy();
            }
        }
        #endregion

        private void FormModelesMouvements_Load(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void FormModelesMouvements_Resize(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void Redimensionner()
        {
            buttonValider.Left = (Width - buttonValider.Width - buttonAnnuler.Width - 20) / 2;
            buttonAnnuler.Left = buttonValider.Left + buttonValider.Width + 20;
            buttonValider.Top = Height - 3 * buttonAnnuler.Height;
            buttonAnnuler.Top = buttonValider.Top;
            labelVitesse.Top = buttonValider.Top;

            dataGridViewModelesMouvements.Width = Width - dataGridViewModelesMouvements.Left * 2;
            dataGridViewModelesMouvements.Height = Height - 4 * buttonAnnuler.Height;
        }
    }
}