﻿using System.Data;
using Telegram.Bot.Requests;
using TelegramBotClean.Bot;
using TelegramBotClean.Commandses;
using TelegramBotClean.Data;
using TelegramBotClean.Userses;

namespace TelegramBotClean.TextDir
{
    public class TextWorker
    {
        Random r = new Random();
        public Dictionary<string, List<string>> answers;
        public List<string> anonNames;
        BotDB botBase;
        
        public TextWorker(BotDB botBase,Random r) 
        {
            this.botBase = botBase;
            this.r = r;
          
            UpdateAnswers();
            UpdateAnonimNames();
        }
        /// <summary>
        /// Эквивалентно Environment.NewLine
        /// </summary>
        public static string Ln { get { return Environment.NewLine; } }

        protected void UpdateAnswers()
        {
            answers = botBase.GetAlAnswersComplect();
        }
        protected void UpdateAnonimNames()
        {
            anonNames = botBase.GetAllAnonimNames().ToList();
        }




        public string RandomAnswere(string text)
        {
            Commands c = Commands.SelectCommands(text);
            string[] words = answers[c.AsCommand().Name].ToArray();
            return words[r.Next(0, words.Length)];
        }
        public string RandomAnswere(Command command)
        {
            string[] words = answers[command.Name].ToArray();
            return words[r.Next(0, words.Length)];
        }

        public string RandomAnonName()
        {
            if (anonNames.Count == 0)
            {
                return "Кто-то еще";
            }
            else
            {
                string name = anonNames[r.Next(0, anonNames.Count)];
                anonNames.Remove(name);
                return name;
            }
        }
        public void AnonNameFinished(string name)
        {
            if (anonNames.IndexOf(name) == -1)
            {
                anonNames.Add(name);            
            }
        }
        public static string Help(User user) 
        {
            string retInformation = BotInformation.version + Ln;
            retInformation += "Узнать как работают кнопки нажми: /howWorkButton"+ Ln;
            retInformation += "";
            return retInformation;
        }
        public static string HowWorkButton(User user)
        {

            string retInformation = "";
            retInformation += "Кнопка (Помощь): при нажатии на кнопку срабатывает окошко с помощью" + Ln;
            if (user.TypeUser == UserTypes.Teen)
            {
                retInformation += "Кнопка (Включить Анон): При нажатии, включается режим анонимности, и отключаются все возможности бота. Все сообщения, которые  ты отправишь, будут анонимно направлены учителям. Они смогут тебе ответить на любые вопросы.";
                retInformation += "Кнопка (Отключить Анон): Появляется после включения анона. После нажатия, включаются все функции бота и отключается режим анонимности.";
            }
            
            return retInformation;
        }
    }
    public class Invizible
    {
        public static string One { get { return " "; } }
        public static string Two { get { return " "; } }
        public static string Three { get { return "ㅤ"; } }
    }
    public class InvizibleEquals
    {
        public static string All
        {
            get { return Invizible.Three + Invizible.Three; }
        }
    }
}
