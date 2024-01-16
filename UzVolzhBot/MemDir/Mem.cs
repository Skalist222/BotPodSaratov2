using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotClean.MemDir
{
    public class Mem
    {
        string FileId { get; }
        long IdMessage { get; }
        public Mem(string fileId, long idMessage)
        {
            FileId = fileId;
            IdMessage = idMessage;
        }
    }
}
