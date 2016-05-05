using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormRole : Form
    {
        private InterfaceVaocWeb iWeb;

        public FormRole()
        {
            InitializeComponent();
        }
    
        #region propriétés
        public string fichierCourant
        {
            set
            {
                iWeb = ClassVaocWebFactory.CreerVaocWeb(value, false);
            }
        }

        public Donnees.TAB_ROLEDataTable tableRole
        {
            get
            {
                Donnees.TAB_ROLEDataTable table = new Donnees.TAB_ROLEDataTable();
                foreach (DataGridViewRow ligne in this.dataGridViewRole.Rows)
                {
                    if (null != ligne.Cells["S_NOM"].Value)//cas pour la nouvelle ligne ajoutée automatiquement
                    {
                        //recherche de l'id utilisateur couplé au nom séléctionné
                        int iUtilisateur = ligne.Cells["S_UTILISATEUR"].Value.ToString().IndexOf("(");
                        string login = ligne.Cells["S_UTILISATEUR"].Value.ToString().Substring(iUtilisateur+1, ligne.Cells["S_UTILISATEUR"].Value.ToString().Length - iUtilisateur-2);

                        ClassDataUtilisateur utilisateur = iWeb.GetUtilisateur(login);

                        table.AddTAB_ROLERow(
                            Convert.ToInt32(ligne.Cells["ID_ROLE"].Value),
                            utilisateur.ID_UTILISATEUR,
                            Convert.ToString(ligne.Cells["S_NOM"].Value),
                            Convert.ToInt32(ligne.Cells["ID_PION"].Value)
                            );
                    }
                }
                return table;
            }
            set
            {
                dataGridViewRole.Rows.Clear();

                //mise à jour de la liste déroulante des utilisateurs disponibles
                foreach (ClassDataUtilisateur utilisateur in iWeb.ListeUtilisateurs(false))
                {
                    this.S_UTILISATEUR.Items.Add(string.Format("{0} {1}({2})", utilisateur.S_PRENOM, utilisateur.S_NOM, utilisateur.S_LOGIN));
                }

                //mise à jour de la valeur séléctionnée dans la liste déroulante
                foreach (Donnees.TAB_ROLERow ligneRole in value)
                {
                    //recherche du nom utilisateur couplé à l'id séléctionné
                    ClassDataUtilisateur utilisateur = iWeb.GetUtilisateur(ligneRole.ID_UTILISATEUR);

                    DataGridViewRow ligneGrid = dataGridViewRole.Rows[dataGridViewRole.Rows.Add()];
                    ligneGrid.Cells["S_NOM"].Value = ligneRole.S_NOM;
                    ligneGrid.Cells["ID_ROLE"].Value = ligneRole.ID_ROLE;
                    //ligneGrid.Cells["ID_PARTIE"].Value = ligneRole.ID_PARTIE;
                    ligneGrid.Cells["S_UTILISATEUR"].Value = string.Format("{0} {1}({2})", utilisateur.S_PRENOM, utilisateur.S_NOM, utilisateur.S_LOGIN);
                    ligneGrid.Cells["ID_PION"].Value = ligneRole.ID_PION;
                }
            }
        }

        #endregion

        private void Redimensionner()
        {
            #region positionnement des boutons annuler et valider
            buttonValider.Left = (Width - buttonValider.Width - buttonAnnuler.Width) / 3;
            buttonAnnuler.Left = buttonValider.Width + 2 * (Width - buttonValider.Width - buttonAnnuler.Width) / 3;
            buttonValider.Top = buttonAnnuler.Top = Height - 3 * buttonValider.Height;
            #endregion

            #region positionnement de la grille
            dataGridViewRole.Width = Width;
            dataGridViewRole.Height = Height - 4 * buttonValider.Height;
            #endregion
        }

        #region evenements
        private void FormRole_Resize(object sender, EventArgs e)
        {
            Redimensionner();
        }

        private void FormRole_Load(object sender, EventArgs e)
        {
            Redimensionner();
        }
        #endregion

    }
}