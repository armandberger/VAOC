using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace WaocLib
{
    public class LogFile
    {
        private static string m_fileName="";
        private static StringBuilder m_phrases = new StringBuilder();//phrases à écrire
        private static readonly object m_verrouEcriture = new object();//pour gérer le multi-processeur
        //public LogFile(string nomFichier, string suffixe, int tour, int phase)
        //{
        //    string nomfichierTourPhase;
        //    int positionPoint = nomFichier.LastIndexOf(".");

        //    //recopie de la chaine avant l'extension
        //    nomfichierTourPhase = nomFichier.Substring(0, positionPoint);
        //    //ajout du tour et de la phase
        //    nomfichierTourPhase = string.Format("{0}_{1}Log_{002}_{003}.log", nomfichierTourPhase, suffixe, tour, phase);

        //    CreationLogFile(nomfichierTourPhase);
        //}

        public LogFile()
        {
        }

        public static string NomLogFile(string suffixe, int tour, int phase)
        {
            return string.Format("{0}\\logs\\Log_{1}_{002}_{003}.log", Constantes.repertoireDonnees, suffixe, tour, phase);
        }

        public static string CreationLogFile(string suffixe, int tour, int phase, bool bPardefaut = true)
        {
            string nomfichierTourPhase = NomLogFile(suffixe, tour, phase);
            CreationLogFile(nomfichierTourPhase, bPardefaut);
            return nomfichierTourPhase;
        }

        public static void CreationLogFile(string filename, bool bPardefaut = true)
        {
            if (bPardefaut) { m_fileName = filename; }
            //remove any old log file            
            StreamWriter  file;
            file = new StreamWriter(filename, false);
            file.Close();
        }

		/// <summary>
		/// write a message at the end of the log file
		/// </summary>
		/// <param name="message">message to add to the log file</param>
		/// <returns>bool if ok, false if ko</returns>
        public static bool Notifier(string message, out string errorMessage)
		{
            errorMessage = string.Empty;
            try
            {
                Notifier(message);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
            return true;
		}

        /// <summary>
        /// write a message at the end of the log file
        /// </summary>
        /// <param name="message">message to add to the log file</param>
        /// <returns>bool if ok, false if ko</returns>
        public static bool Notifier(string message)
        {
            return Notifier(m_fileName, message);
        }

        /// <summary>
        /// write a message at the end of the log file
        /// </summary>
        /// <param name="nomfichier">nom du fichier de log</param>
        /// <param name="message">message to add to the log file</param>
        /// <returns>bool if ok, false if ko</returns>
        public static bool Notifier(string nomfichier, string message)
        {
            if (nomfichier == string.Empty) { return false; }
            StreamWriter file;
            try
            {
                Monitor.Enter(m_verrouEcriture);
                m_phrases.AppendFormat("{0}{1}:{2}|{3}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, message);
                file = new StreamWriter(m_fileName, true);
                file.WriteLine(m_phrases.ToString());
                file.Close();
                m_phrases.Clear();
                Monitor.Exit(m_verrouEcriture);
                return true;
            }
            catch
            {
                Monitor.Exit(m_verrouEcriture);
                return false;
            }
        }

        public static string nomfichier { get { return m_fileName; } }
    }
}
