using PodrostkiBot.Users;
using System.Diagnostics;
using System.Drawing;


namespace PodrostkiBot.Text
{
    internal class Logger
    {
        public static bool FileLogEnabled = false;
        public static bool SegmentStart = false;
        private static string CurrentPath = Directory.GetCurrentDirectory();
        public static void Error(string msg)
        {
            string method = new StackTrace().GetFrame(2).GetMethod().Name;
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
            try { File.AppendAllText(CurrentPath+"\\log.txt", "error|" + DateTime.Now + "|" + msg + "!!!!!!!" + Environment.NewLine); }
            catch (Exception e){ Console.WriteLine("Не смог сохранить в файл log.txt___"+e.Message); }

        }
        public static void Info(string msg, bool segment = true,ConsoleColor color = ConsoleColor.White)
        {
            ConsoleColor defCol = Console.ForegroundColor;
            Console.ForegroundColor = color;
            string method = new StackTrace().GetFrame(1).GetMethod().Name;
            Console.WriteLine(msg);
            if (!segment)
            {
                
                if (!SegmentStart) Debug.WriteLine("````````````````````````info```````````````````````");
                Debug.WriteLine(msg);
                if (method != "") Debug.WriteLine("In method " + method);
                Debug.WriteLine("___________________________________________________");
                SegmentStart = false;
            }
            else
            {
                if (!SegmentStart)
                {
                    SegmentStart = true;
                    Debug.WriteLine("````````````````````````info```````````````````````");
                    if (method != "") Debug.WriteLine("In method " + method);
                }
                Debug.WriteLine(msg);
            }
            try
            {
                File.AppendAllText(CurrentPath+"\\log.txt", "info|" + DateTime.Now + "|" + msg + "" + Environment.NewLine);
            }
            catch { }
            Console.ForegroundColor = defCol;

        }
        public static void InfoStop()
        {
            SegmentStart = false;
            Debug.WriteLine("___________________________________________________");
        }
        public static void SaveMessage(UserI user,MessageI mes)
        {
            string nl = Environment.NewLine;//Решил что проще сделать так
            try
            {
                string saveMessage="";
                saveMessage += DateTime.Now.ToString() + nl;
                if (user is not null)
                {
                    saveMessage += "Отправитель: " + user.Name + $"({user.Id})" + nl;
                }
                else
                {
                    saveMessage += "Отправитель: Неизвестный" + nl;
                }
                
                saveMessage += "Тип сообщения: " + mes.Type.Name + nl;
                if (mes.Type != MTypes.Text && mes.Type != MTypes.Button) saveMessage += "FileId:" + mes.FileId + nl;
                saveMessage += "Текст сообщения: "+ mes.Text + nl;               
                saveMessage += "___________________________________________________________" + nl;
                File.AppendAllText(CurrentPath + "\\Message.txt", saveMessage);
            }
            catch(Exception ex)
            {
                Error(ex.Message);
            }
        }
        
    }
}
