using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;

namespace WaocLib
{
    public class PHPService
    {
        protected string m_http;

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
        public bool Version(out XmlDocument docXML)
        {
            docXML = new XmlDocument();
            try
            {
                WebRequest requete = WebRequest.Create(m_http + "?op=version");
                WebResponse reponse = requete.GetResponse();
                if (reponse is HttpWebResponse)
                {
                    StreamReader sr = new StreamReader(reponse.GetResponseStream(), Encoding.GetEncoding("ISO-8859-1"));
                    string strXML = sr.ReadToEnd();
                    docXML.LoadXml(strXML);
                }
            }
            catch (Exception exp)
            {
                XmlNode noeurErreur = docXML.CreateNode(XmlNodeType.Element, "ERREUR", "");
                noeurErreur.InnerText = "PHPService: Utilisateurs :" + exp.Message;
                return false;
            }
            if (null != docXML["ERREUR"])
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
        public bool Utilisateurs(ref XmlDocument docXML)
        {
            try
            {
                WebRequest requete = WebRequest.Create(m_http+"?op=utilisateurs");
                WebResponse reponse = requete.GetResponse();
                if (reponse is HttpWebResponse)
                {
                    StreamReader sr = new StreamReader(reponse.GetResponseStream(), Encoding.GetEncoding("ISO-8859-1"));
                    string strXML = sr.ReadToEnd();
                    docXML.LoadXml(strXML);
                    XmlElement elem=docXML["UTILISATEURS"];
                    foreach (XmlNode noeud in elem.ChildNodes)
                    {
                        string nom=noeud["S_NOM"].InnerText;
                    }
                }
            }
            catch (Exception exp)
            {
                docXML.InnerXml = String.Empty;
                XmlNode noeurErreur=docXML.CreateNode(XmlNodeType.Element, "ERREUR", "");
                noeurErreur.InnerText="PHPService: Utilisateurs :" + exp.Message;
                return false;
            }
            if (null != docXML["ERREUR"])
            {
                return false;
            }
            return true;

        }
    }
}
