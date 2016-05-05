using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;

namespace WaocLib
{
    public class CourrielService
    {
        private SmtpClient m_clientCourriel;
        private System.Net.NetworkCredential m_authentification;

        public CourrielService(string host, string utilisateur, string motDePasse)
        {
            m_clientCourriel = null;
            m_authentification = null;
            if (host != string.Empty && utilisateur != string.Empty && motDePasse != string.Empty)
            {
                m_clientCourriel = new SmtpClient();
                m_authentification = new System.Net.NetworkCredential(utilisateur, motDePasse);

                m_clientCourriel.Host = host;
                m_clientCourriel.UseDefaultCredentials = true;
                m_clientCourriel.Credentials = m_authentification;
            }
        }

        public bool EnvoyerMessage(string adresseCourriel, string titre, string texte)
        {
            if (null == m_clientCourriel) { return false; }
            MailMessage courriel = new MailMessage("vaoc@free.fr", adresseCourriel);

            courriel.Subject = titre;
            courriel.Body = texte;
            courriel.IsBodyHtml = true;

            m_clientCourriel.Send(courriel);

            return true;
        }

        static public bool EnvoyerMessage(string adresseCourriel, string titre, string texte, string host, string utilisateur, string motDePasse)
        {
            MailMessage courriel = new MailMessage(utilisateur, adresseCourriel);
            SmtpClient clientCourriel = new SmtpClient();
            System.Net.NetworkCredential authentification = new System.Net.NetworkCredential(utilisateur, motDePasse);

            courriel.Subject = titre;
            courriel.Body = texte;
            courriel.IsBodyHtml = true;

            clientCourriel.Host = host;
            clientCourriel.UseDefaultCredentials = true;
            clientCourriel.Credentials = authentification;
            clientCourriel.Send(courriel);

            return true;
        }
            
    }
}
