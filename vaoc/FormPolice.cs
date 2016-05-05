using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormPolice : Form
    {
        public FormPolice()
        {
            InitializeComponent();
        }

        #region propriétés
        public Donnees.TAB_POLICEDataTable tablePolice
        {
            get
            {
                Donnees.TAB_POLICEDataTable table = new Donnees.TAB_POLICEDataTable();
                foreach (DataGridViewRow ligne in dataGridViewPolice.Rows)
                {
                    if (null != ligne.Cells["Nom"].Value)//cas pour la nouvelle ligne ajoutée automatiquement
                    {
                        Color pixelColor = ligne.Cells["Couleur"].Style.BackColor;
                        table.AddTAB_POLICERow(
                            Convert.ToString(ligne.Cells["Nom"].Value),
                            Convert.ToString(ligne.Cells["Police"].Value),
                            pixelColor.R, pixelColor.G, pixelColor.B,
                            Convert.ToInt16(ligne.Cells["Taille"].Value),
                            Convert.ToBoolean(ligne.Cells["Italique"].Value),
                            Convert.ToBoolean(ligne.Cells["Barre"].Value),
                            Convert.ToBoolean(ligne.Cells["Souligne"].Value),
                            Convert.ToBoolean(ligne.Cells["Gras"].Value)
                            );
                    }
                }
                return table;
            }
            set
            {
                dataGridViewPolice.Rows.Clear();
                foreach (Donnees.TAB_POLICERow lignePolice in value)
                {
                    Color pixelColor = Color.FromArgb(lignePolice.I_ROUGE, lignePolice.I_VERT, lignePolice.I_BLEU);
                    DataGridViewRow ligneGrid = dataGridViewPolice.Rows[dataGridViewPolice.Rows.Add()];
                    ligneGrid.Cells["Nom"].Value = lignePolice.S_NOM;
                    ligneGrid.Cells["Police"].Value = lignePolice.S_NOM_POLICE;
                    ligneGrid.Cells["Taille"].Value = lignePolice.I_TAILLE;
                    ligneGrid.Cells["Italique"].Value = lignePolice.B_ITALIQUE;
                    ligneGrid.Cells["Barre"].Value = lignePolice.B_BARRE;
                    ligneGrid.Cells["Souligne"].Value = lignePolice.B_SOULIGNE;
                    ligneGrid.Cells["Gras"].Value = lignePolice.B_GRAS;
                    ligneGrid.Cells["Couleur"].Style.BackColor = pixelColor;
                }
            }
        }

        #endregion

        private void FormPolice_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (1 == e.ColumnIndex)
            {
                if (DialogResult.OK == fontDialog.ShowDialog())
                {
                    Font police = fontDialog.Font;
                    dataGridViewPolice.Rows[e.RowIndex].Cells["Couleur"].Style.BackColor = fontDialog.Color;
                    dataGridViewPolice.Rows[e.RowIndex].Cells["Police"].Value = police.Name;
                    dataGridViewPolice.Rows[e.RowIndex].Cells["Taille"].Value = police.SizeInPoints;
                    dataGridViewPolice.Rows[e.RowIndex].Cells["Italique"].Value = police.Italic;
                    dataGridViewPolice.Rows[e.RowIndex].Cells["Barre"].Value = police.Strikeout;
                    dataGridViewPolice.Rows[e.RowIndex].Cells["Souligne"].Value = police.Underline;
                    dataGridViewPolice.Rows[e.RowIndex].Cells["Gras"].Value = police.Bold;
                }
            }
        }
    }
}