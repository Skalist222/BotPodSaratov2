using PodrostkiBot.Bible;
using PodrostkiBot.DataBase.Engine;
using PodrostkiBot.Users;
using MemWorkerSpace;
using Telegram.Bot.Types;

using static PodrostkiBot.Configure.ConstData;
using PodrostkiBot.Menus;
using PodrostkiBot.Text;
using static System.Net.Mime.MediaTypeNames;
using PodrostkiBot.Configure;

namespace PodrostkiBot.Messages
{
    public class Spam
    {
        Engine Engine;
        public Spam(Engine engine)
        {
            Engine = engine;
        }
        public async void SpamText(string text)
        {
            foreach (UserI u in Engine.Users)
            {
                await Engine.Sender.SendMessageAsync(text, u);
            }
        }
        public void SenderWorker()
        {
            while (true)
            {
                // Нужно для тестирования
                string toDay = DateTime.Now.DayOfWeek.ToString();
                DateTime now = DateTime.Now;
                //Console.WriteLine(now.Hour)
                if (toDay == "Sunday")//"Sunday")
                {
                    if (now.Hour == 12 && now.Minute == 02)
                    {
                        SendSpamSunday();
                        Thread.Sleep(60000);
                    }
                }

                if (now.Hour == 07 && now.Minute == 10)

                {
                    SpamBless("Доброе утро!");
                    Thread.Sleep(60000);
                }
                
                Thread.Sleep(3000); 
            }
        }
        public void SendSpamSunday()
        {
            //Поправлен 20.03.24
            
            foreach (UserI user in Engine.Users)
            {
                if (user.Spam)
                {
                    if (user.Privilege == UserPrivilege.Teen)
                    {
                        Engine.Sender.SundaySpamMessage(user);
                    }
                }
            }

        }
        public async void SpamBless(string startText)
        {
            Logger.Info("Утренняя рассылка Золотых стихов" + Environment.NewLine, false, ConsoleColor.Yellow);
            string text = startText + Environment.NewLine;
            text += Engine.Golds.GetRandomGoldVerse(Engine.Random).ToString();
            foreach (UserI u in Engine.Users)
            {
                if (u.Spam)
                {
                    string komu = "Для " + u.Name + Environment.NewLine;
                    Logger.Info(komu + text + Environment.NewLine, false, ConsoleColor.Yellow);
                    Engine.Sender.SendMessageAsync(text, u);
                }
            }
        }

        public async void SpamInfoBot()
        {
            foreach (UserI u in Engine.Users)
            {
                string info = "Бот обновился 🥰 Вот информация об обновлениях" + Environment.NewLine + BotInfo(u.Privilege);
                info += Environment.NewLine;
                await Engine.Sender.SendMessageAsync(info, u);
            }
        }
        public async void SpamRestartMenu(BaseMenu menus)
        {
            foreach (UserI u in Engine.Users)
            {
                if (u.Spam)
                {
                    menus.SelectMenu(Engine.Sender, "Я обновился и перезагрузил ваше меню 🥰", u);
                }
            }
        }
   
        public async void SpamTeacherNewAnon()
        {
            foreach (UserI u in Engine.Users)
            {
                if (u.Privilege == UserPrivilege.Teacher)
                {
                    await Engine.Sender.SendMessageAsync( "Пришло новое анонимное сообщение",u);
                }
            }
        }
        public async void SpamTeacherAnonAnswerSended(UserList users,string anonName)
        {
            foreach (UserI u in users)
            {
                if (u.Privilege == UserPrivilege.Teacher)
                {
                    await Engine.Sender.SendMessageAsync(  "На вопрос от "+ anonName+" был отпрален ответ", u);
                }
            }
        }
        /// <summary>
        /// Отправляет всем обновление панелей
        /// </summary>
        /// <param name="users">список пользователей</param>
        /// <param name="text">Текст отправляемый пользователю</param>


    }
}
