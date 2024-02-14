using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotClean.Messages;

namespace TelegramBotClean.Bible
{
    public class Bless
    {
        public BibleWorker Bible { get; }
        private Random random;
        public Bless(BibleWorker bible,Random random) 
        {
            this.Bible = bible;
            this.random = random;
        }
        public MessageI GetMessageAnyBless()
        {
            Verse randomVerse = Bible.GetRandomGoldVerse();
            //тут еще будут цитаты, и просто добрые пожелания, пока не неализовано
            MessageI messageBless = new MessageI(randomVerse.ToString());
            return messageBless;
        }
    }
}
