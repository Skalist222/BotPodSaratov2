using System.Diagnostics;
using System.Drawing;

namespace TelegramBotClean.Data
{
    internal class Logger
    {
        public static bool FileLogEnabled = false;
        public static bool segmentStart = false;
        public static void Error(string msg)
        {
            string method = new StackTrace().GetFrame(1).GetMethod().Name;
            ConsoleColor defa = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!ERROR!!!!!!!!!!!!!!!!!!!!!!!!");
            if(method!="")Debug.WriteLine(method);
            Debug.WriteLine(msg);
            Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
           
            Console.WriteLine(method);
            Console.WriteLine(msg);
           
            InfoStop();
            Console.ForegroundColor = defa;
            try { File.AppendAllText("log.txt", "error|" + DateTime.Now + "|" + msg + "!!!!!!!" + Environment.NewLine); }
            catch { }

        }
        public static void Info(string msg, bool segment = true,ConsoleColor color = ConsoleColor.White)
        {
            ConsoleColor defCol = Console.ForegroundColor;
            Console.ForegroundColor = color;
            string method = new StackTrace().GetFrame(1).GetMethod().Name;
            Console.WriteLine(msg);
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
            Console.ForegroundColor = defCol;

        }
        public static void InfoStop()
        {
            segmentStart = false;
            Debug.WriteLine("___________________________________________________");
        }
        
    }
}
