using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WaocLib
{
    public class LogFile
    {
        private static string m_fileName="";
        private static bool m_ecriture;//true si en cours d'écriture, false sinon, pour gérer le multi-processeur
        private static StringBuilder m_phrases = new StringBuilder();//phrases à écrire

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

        public static void CreationLogFile(string nomFichier, string suffixe, int tour, int phase)
        {
            string nomfichierTourPhase;
            int positionPoint = nomFichier.LastIndexOf(".");

            //recopie de la chaine avant l'extension
            nomfichierTourPhase = nomFichier.Substring(0, positionPoint);
            //ajout du tour et de la phase
            nomfichierTourPhase = string.Format("{0}_{1}Log_{002}_{003}.log", nomfichierTourPhase, suffixe, tour, phase);

            CreationLogFile(nomfichierTourPhase);
        }

        public static void CreationLogFile(string filename)
        {
            m_fileName=filename;
            //remove any old log file            
            StreamWriter  file;
            file = new StreamWriter(m_fileName, false);
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
        public static void Notifier(string message)
        {
            if (m_fileName == string.Empty) { return; }
            StreamWriter file;
            m_phrases.AppendFormat("{0}:{1}:{2} {3}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, message);
            if (!m_ecriture)
            {
                m_ecriture = true;
                file = new StreamWriter(m_fileName, true);                
                file.WriteLine(m_phrases.ToString());
                m_phrases.Clear();
                file.Close();
                m_ecriture = false;
            }
        }
    }
}
