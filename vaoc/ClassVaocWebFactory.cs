using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace vaoc
{
    class ClassVaocWebFactory
    {
        private static InterfaceVaocWeb m_iWeb;

        public static InterfaceVaocWeb CreerVaocWeb(string fichierJeu, bool nouveauFichier)
        {
            //if (null == m_iWeb)//, ne permet pas de changer le nom pour le nouveau tour
            //{
               try
               {
                   //m_iWeb = new ClassVaocWebHTTP(DataSetCoutDonnees.m_donnees.TAB_PARTIE[0].S_PHPSERVICE);
                   m_iWeb = new ClassVaocWebFichier(fichierJeu, nouveauFichier);
               }
               catch (Exception ex1)
               {
                   //si l'interface HTTP ne fonctionne pas, on tente le mode dégradé avec un fichier
                   try
                   {
                       m_iWeb = new ClassVaocWebFichier(fichierJeu, nouveauFichier);
                   }
                   catch (Exception ex)
                   {
                       MessageBox.Show("Impossible de créer l'interface HTTP:" + ex1.Message + " ni l'interface fichier : " + ex.Message,
                           "CreerVaocWeb : InterfaceVaocWeb", MessageBoxButtons.OK, MessageBoxIcon.Error);
                       return null;
                   }
               }
            //}
            return m_iWeb;
        }
    }
}
