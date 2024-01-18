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
