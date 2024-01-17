using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotClean.Messages;

namespace TelegramBotClean.MemDir
{
    public class Mem
    {
        public MessageI Message { get; }
        public Mem(MessageI message)
        {
            Message = message;
        }
    }
}
