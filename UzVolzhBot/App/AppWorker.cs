using PodrostkiBot.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodrostkiBot.App
{
    public class AppWorker
    {
        public static  void Restart()
        {
            System.Diagnostics.Process.Start(Directory.GetCurrentDirectory()+ "\\ChristMemBot.exe");
            Environment.Exit(0);
        }
    }
}
