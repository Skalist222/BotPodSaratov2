using PodrostkiBot.Configure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodrostkiBot.Text
{
    public class AnswereCreator
    {
        Engine engine;

        public AnswereCreator(Engine engine)
        {
            this.engine = engine;
        }
        public MessageI CreateAnswere(Telegram.Bot.Types.Update up)
        {
            MessageI receivedMesssage = new MessageI(up,engine);
            return receivedMesssage;
        }
        

    }
}
