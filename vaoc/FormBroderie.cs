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
    public partial class FormBroderie : Form
    {
        public FormBroderie()
        {
            InitializeComponent();
        }

        private void buttonImage_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == this.openFileDialog.ShowDialog(this))
            {
                textBoxFichierImage.Text = openFileDialog.FileName;
            }

        }

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (DialogResult.OK ==  colorDialog.ShowDialog(this) )
            {
                textBoxCouleur.BackColor = colorDialog.Color;
            }
        }

        private void buttonTraitement_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap imageCarte = (Bitmap)Image.FromFile(textBoxFichierImage.Text);
                string nomFichier = textBoxFichierImage.Text.Substring(0, textBoxFichierImage.Text.LastIndexOf('.'));
                Graphics graph = Graphics.FromImage(imageCarte);
                Pen styloFin = new Pen(textBoxCouleur.BackColor, 1);
                Pen styloGras = new Pen(textBoxCouleur.BackColor, 3);
                decimal pasHorizontal = (decimal)imageCarte.Width / Convert.ToInt32(textBoxHorizontal.Text);
                decimal pasVertical = (decimal)imageCarte.Height / Convert.ToInt32(textBoxVertical.Text);
                if (checkBoxMajorite.Checked)
                {
                    //on dessine dans le rectangle la couleur majoritaire
                    Dictionary<Color, int> couleurs = new Dictionary<Color, int>();
                    for (decimal h = 0; h < imageCarte.Width; h += pasHorizontal)
                    {
                        for (decimal v = 0; v < imageCarte.Height; v += pasVertical)
                        {
                            couleurs.Clear();
                            for (decimal x = h; x < h + pasHorizontal; x++)
                            {
                                for (decimal y = v; y < v + pasVertical; y++)
                                {
                                    Color pixelColor = imageCarte.GetPixel(Math.Min((int)x, imageCarte.Width-1), 
                                                                            Math.Min((int)y, imageCarte.Height-1));
                                    if (couleurs.ContainsKey(pixelColor))
                                    {
                                        couleurs[pixelColor]++;
                                    }
                                    else
                                    {
                                        couleurs.Add(pixelColor, 1);
                                    }
                                }
                            }
                            //couleur majoritaire dans le rectangle
                            IOrderedEnumerable<KeyValuePair<Color, int>> couleurMajoritaire = from coul in couleurs orderby coul.Value descending select coul;
                            SolidBrush brosse = new SolidBrush(couleurMajoritaire.First().Key);
                            graph.FillRectangle(brosse, (int)h, (int)v, (int)pasHorizontal+1, (int)pasVertical+1);
                        }
                    }
                }

                //on affiche les traits des carrés de broderie
                for (decimal h = 0; h < imageCarte.Width; h += pasHorizontal)
                {
                    Pen stylo = (0 == Math.Round(h/pasHorizontal) % 5) ? styloGras : styloFin;
                    graph.DrawLine(stylo, (int)h, 0, (int)h, imageCarte.Height);
                }
                graph.DrawLine(styloGras, imageCarte.Width-1, 0, imageCarte.Width-1, imageCarte.Height);

                for (decimal v = 0; v < imageCarte.Height; v += pasVertical)
                {
                    Pen stylo = (0 == Math.Round(v/pasVertical) % 5) ? styloGras : styloFin;
                    graph.DrawLine(stylo, 0, (int)v, imageCarte.Width, (int)v);
                }
                graph.DrawLine(styloGras, 0, imageCarte.Height-1, imageCarte.Width, imageCarte.Height-1);
                graph.Dispose();

                string nomFichierDestination = nomFichier + "_broderie.png";
                imageCarte.Save(nomFichierDestination, System.Drawing.Imaging.ImageFormat.Png);
                MessageBox.Show(nomFichierDestination + " sauvegardé", "Broderie", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message+ "\\r\\f" + ex.StackTrace, "Broderie", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
