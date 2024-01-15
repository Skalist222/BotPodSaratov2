using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotClean.Data
{
    internal class Logger
    {
        public static bool FileLogEnabled = false;
        public static bool segmentStart = false;
        public static void Error(string msg, string method = "")
        {

            Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!ERROR!!!!!!!!!!!!!!!!!!!!!!!!");
            Debug.WriteLine(msg);
            Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            InfoStop();

            try { File.AppendAllText("log.txt", "error|" + DateTime.Now + "|" + msg + "!!!!!!!" + Environment.NewLine); }
            catch { }

        }
        public static void Info(string msg, string method = "", bool segment = true)
        {
            if (!segment)
            {
                if (!segmentStart) Debug.WriteLine("````````````````````````info```````````````````````");
                Debug.WriteLine(msg);
                if (method != "") Debug.WriteLine("In method " + method);
                Debug.WriteLine("___________________________________________________");
                segmentStart = false;
            }
            else
            {
                if (!segmentStart)
                {
                    segmentStart = true;
                    Debug.WriteLine("````````````````````````info```````````````````````");
                    if (method != "") Debug.WriteLine("In method " + method);
                }
                Debug.WriteLine(msg);
            }
            try
            {
                File.AppendAllText("log.txt", "info|" + DateTime.Now + "|" + msg + "" + Environment.NewLine);
            }
            catch { }

        }
        public static void InfoStop()
        {
            segmentStart = false;
            Debug.WriteLine("___________________________________________________");
        }
        
    }
}
