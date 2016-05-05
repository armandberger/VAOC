using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    public partial class FormMeteo : Form
    {
        public FormMeteo()
        {
            InitializeComponent();
        }

        #region propriétés
        public Donnees.TAB_METEODataTable tableMeteo
        {
            get{
                /*
                DataSetCoutDonnees.TAB_METEODataTable table = new DataSetCoutDonnees.TAB_METEODataTable();
                foreach (DataGridViewRow ligne in dataGridViewMeteo.Rows)
                {
                    if (null != ligne.Cells[0].Value)
                    {
                        //test sur le null car sinon renvoit aussi la prochaine ligne a saisir
                        table.AddTAB_METEORow(Convert.ToInt32(ligne.Cells[0].Value),
                            Convert.ToString(ligne.Cells[1].Value),
                            Convert.ToInt16(ligne.Cells[2].Value));
                    }
                }                
                return table;
                 */
                return (Donnees.TAB_METEODataTable)dataGridViewMeteo.DataSource;
            }
            set
            {
                dataGridViewMeteo.DataSource = value.Copy();
                //dataGridViewMeteo.Rows.Clear();
                //foreach (DataSetCoutDonnees.TAB_METEORow ligne in value)
                //{
                //    DataGridViewRow ligneGrid = dataGridViewMeteo.Rows[dataGridViewMeteo.Rows.Add()];
                //    ligneGrid.Cells["ID_METEO"].Value = ligne.ID_METEO;
                //    ligneGrid.Cells["S_NOM"].Value = ligne.S_NOM;
                //    ligneGrid.Cells["I_CHANCE"].Value = ligne.I_CHANCE;
                //}
            }
        }
        #endregion
    }
}