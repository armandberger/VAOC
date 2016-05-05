using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Reflection;
using System.Diagnostics;

namespace WaocLib
{
    static public class LecteurXML
    {
        static public Stream LireFichier(string nomfichier)
        {
            /*** liste de tous les fichiers se trouvant dans l'assembly 
            System.Reflection.Assembly thisExe;
            thisExe = System.Reflection.Assembly.GetExecutingAssembly();
            string[] resources = thisExe.GetManifestResourceNames();

            // Build the string of resources.
            foreach (string resource in resources)
            {
                Debug.WriteLine(resource);
            }
            */

            Assembly assembly = Assembly.GetExecutingAssembly();
            XmlTextReader lecteur = new XmlTextReader(nomfichier);
            MemoryStream fluxSortie = new MemoryStream();
            XmlTextWriter resultat = new XmlTextWriter(fluxSortie, System.Text.Encoding.UTF8);
            
            //XmlTextWriter resultat = new XmlTextWriter("c:\\test.xml", System.Text.Encoding.UTF8);
            Type type = typeof(LecteurXML);

            Stream fichierXSLT = type.Assembly.GetManifestResourceStream("WaocLib.mysqlTransform.xslt");
            XmlTextReader transformation = new XmlTextReader(fichierXSLT);

            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(transformation);
            xslt.Transform(lecteur, resultat);

            resultat.BaseStream.Position = 0;
            return resultat.BaseStream;
        }
    }
}
