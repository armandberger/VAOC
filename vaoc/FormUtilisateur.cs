using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormUtilisateur : Form
    {
        public FormUtilisateur()
        {
            InitializeComponent();
        }

        #region propriétés
        public string fichierCourant
        {
            set
            {
                InterfaceVaocWeb iWeb = ClassVaocWebFactory.CreerVaocWeb(value, false);
                List<ClassDataUtilisateur> liste = iWeb.ListeUtilisateurs(false);
                dataGridViewUtilisateur.DataSource = liste;
            }
        }
        #endregion

        private void FormUtilisateur_Resize(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void FormUtilisateur_Load(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void Redimensionner()
        {
            #region positionnement des boutons annuler
            buttonAnnuler.Left = (Width - buttonAnnuler.Width) / 2;
            buttonAnnuler.Top = Height - 3 * buttonAnnuler.Height;
            #endregion

            #region positionnement des grilles
            dataGridViewUtilisateur.Width = Width - dataGridViewUtilisateur.Left * 2;
            dataGridViewUtilisateur.Height = Height / 3;
            dataGridViewUtilisateur.Height = Height - 4 * buttonAnnuler.Height ;
            #endregion
        }
    }
}