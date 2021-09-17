using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WaocLib
{
    public static class Verrou
    {
        static Dictionary<int,int> compteur = new Dictionary<int, int>();
        public static void Verrouiller(object obj)
        {
            Monitor.Enter(obj);
            int identifiant = Thread.CurrentThread.ManagedThreadId;
            if (!compteur.ContainsKey(identifiant))
            {
                compteur.Add(identifiant, 0);
            }
            compteur[identifiant]++;
            StackFrame sf = new StackTrace(true).GetFrame(1);
            //LogFile.Notifier("Verrouiller;" + identifiant + ";" + compteur[identifiant] + ";" + sf.GetFileName() + ";" + sf.GetMethod() + ";" + sf.GetFileLineNumber());            
        }

        public static void Deverrouiller(object obj)
        {
            Monitor.Exit(obj);
            int identifiant = Thread.CurrentThread.ManagedThreadId;
            compteur[identifiant]--;
            StackFrame sf = new StackTrace(true).GetFrame(1);
            //LogFile.Notifier("Deverrouiller;" + identifiant + ";"  + compteur[identifiant] + ";" + sf.GetFileName() + ";" + sf.GetMethod() + ";" + sf.GetFileLineNumber());
        }
    }
}
