using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WaocLib
{
    public class PHPService
    {
        protected string m_http;
        static readonly HttpClient m_client = new HttpClient();
        public XmlDocument m_docXML;

        /// <summary>
        /// Constructeur du service
        /// </summary>
        /// <param name="adresseHTTP">adresse web du service PHP</param>
        public PHPService(string adresseHTTP)
        {
            m_http=adresseHTTP;
        }

        /// <summary>
        /// renvoie le numéro de version de l'interface PHP
        /// </summary>
        /// <param name="docXML">données XML renvoyées par le service, VERSION ou ERREUR</param>
        /// <returns>true si OK, false si KO</returns>
        public bool Version()
        {
            try
            {
                if (null == m_docXML) { m_docXML = new XmlDocument(); }
                Task<HttpResponseMessage> response = m_client.GetAsync(m_http + "?op=version").WaitAsync(new TimeSpan(0, 0, 3, 0, 0));
                if (response.IsCompletedSuccessfully)
                {
                    Stream src = response.Result.Content.ReadAsStream();
                    StreamReader sr = new StreamReader(src, Encoding.GetEncoding("ISO-8859-1"));
                    string strXML = sr.ReadToEnd();
                    m_docXML.LoadXml(strXML);
                }
                else
                {
                    XmlNode noeurErreur = m_docXML.CreateNode(XmlNodeType.Element, "ERREUR", "");
                    noeurErreur.InnerText = "PHPService: Version timeout";
                }
            }
            catch (Exception exp)
            {
                XmlNode noeurErreur = m_docXML.CreateNode(XmlNodeType.Element, "ERREUR", "");
                noeurErreur.InnerText = "PHPService: Version :" + exp.Message;
                return false;
            }
            if (null != m_docXML["ERREUR"])
            {
                return false;
            }
            return true;

        }

        /// <summary>
        /// renvoie la liste des utilisateurs inscrits sur le site hebergeur
        /// </summary>
        /// <param name="docXML">données XML renvoyées par le service, UTILISATEURS ou ERREUR</param>
        /// <returns>true si OK, false si KO</returns>
        public bool Utilisateurs()
        {
            try
            {
                if (null == m_docXML) { m_docXML = new XmlDocument(); }
                Task<HttpResponseMessage> response = m_client.GetAsync(m_http + "?op=utilisateurs").WaitAsync(new TimeSpan(0,0,3,0,0));
                if (response.IsCompletedSuccessfully)
                {
                    Stream src = response.Result.Content.ReadAsStream();
                    StreamReader sr = new StreamReader(src, Encoding.GetEncoding("ISO-8859-1"));
                    string strXML = sr.ReadToEnd();

                    m_docXML.LoadXml(strXML);
                    XmlElement elem = m_docXML["UTILISATEURS"];
                    foreach (XmlNode noeud in elem.ChildNodes)
                    {
                        string nom = noeud["S_NOM"].InnerText;
                    }
                }
                else
                {
                    m_docXML.InnerXml = String.Empty;
                    XmlNode noeurErreur = m_docXML.CreateNode(XmlNodeType.Element, "ERREUR", "");
                    noeurErreur.InnerText = "PHPService: Utilisateurs timeout";
                }
            }
            catch (Exception exp)
            {
                m_docXML.InnerXml = String.Empty;
                XmlNode noeurErreur= m_docXML.CreateNode(XmlNodeType.Element, "ERREUR", "");
                noeurErreur.InnerText="PHPService: Utilisateurs :" + exp.Message;
                return false;
            }
            if (null != m_docXML["ERREUR"])
            {
                return false;
            }
            return true;

        }
    }
}
