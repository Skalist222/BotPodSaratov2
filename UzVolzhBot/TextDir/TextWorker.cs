using System.Data;
using Telegram.Bot.Requests;
using TelegramBotClean.Bot;
using TelegramBotClean.Data;

namespace TelegramBotClean.TextDir
{
    public class TextWorker
    {
        Random r = new Random();
        public Dictionary<string, string[]> answers;
        public string[] anonNames;
        BotDB botBase;
        
        public TextWorker(BotDB botBase,Random r) 
        {
            this.botBase = botBase;
            this.r = r;
            UpdateAnswers();
        }
        protected void UpdateAnswers()
        {
            DataTable t = botBase.GetAllAnswersCommand();
            string[] names = t.AsEnumerable().Select(el => el["idCommand"].ToString()).Distinct().ToArray();
        }




        public string Answere(string command)
        {
            
            return "";
        }

        
      
    }
}
