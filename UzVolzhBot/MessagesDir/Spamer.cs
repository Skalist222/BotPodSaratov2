
using Microsoft.Identity.Client;
using System.Globalization;
using Telegram.Bot.Types;
using TelegramBotClean.Bot;
using TelegramBotClean.Messages;
using TelegramBotClean.Userses;

namespace TelegramBotClean.MessagesDir
{
    public class Spamer
    {
        
        private Sender sender;
        public Spamer(Sender sender) 
        {
            this.sender = sender;
            SpamOnTime();
        }

        private async void SpamOnTime()
        {
            new Thread(() => {
                while (true)
                {
                    DateTime timeNow = DateTime.Now;
                    string day = CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat.GetDayName(timeNow.DayOfWeek);
                    string textTime = string.Join(":", new int[] { timeNow.Hour, timeNow.Minute });
                    if (day == "воскресенье")
                    {
                        if (textTime == "12:5")
                        {
                            Console.WriteLine("Запущена отправка вопроса");
                            //Пока не придумал как сделать
                            // Рассылка вопроса фитбека
                        }
                    }

                    if (textTime == "5:50")
                    {
                        Console.WriteLine("Заря! Начало дня для сов! ");
                        //SendAll(sender.Bless.GetMessageAnyBless());
                        // отправка благословения
                    }
                    if (textTime == "9:28")
                    {
                        
                        //SendAll(sender.Bless.GetMessageAnyBless());
                        // отправка благословения
                    }
                    if (textTime == "18:20")
                    {
                        //SendAll(sender.Bless.GetMessageAnyBless());
                        // отправка благословения
                    }
                    Thread.Sleep(1000);
                }
            }).Start();
        }

        public async Task SendAdmins(MessageI message)
        {
            
        }
        //Текстовые отправки
        public async Task SendTextAll(MessageI message)
        {
            // Всем пользователям отправляем сообщение message
            for (int i = 0; i < sender.Users.Count; i++)
            {
                await sender.SendMessage(message,sender.Users.ByIndex(i).Id);
            }
        }
        public async Task SendTextTeachers(MessageI message)
        {
            Users teachers = sender.Users.Teachers;
            for (int i = 0; i < teachers.Count; i++)
            {
                await sender.SendMessage(message, teachers.ByIndex(i).Id);
            }
        }
        public async Task SendTextTeens(MessageI message)
        {
            Users teens = sender.Users.Teens;
            for (int i = 0; i < teens.Count; i++)
            {
                await sender.SendMessage(message, teens.ByIndex(i).Id);
            }
        }

        // Отправка обновления основного меню
        public async Task SendUpdateMenu(string text = "")
        {
            for (int i = 0; i < sender.Users.Count; i++)
            {
                if (text == "") await sender.SendMenu(sender.Users.ByIndex(i).Id);
                else await sender.SendMenu(sender.Users.ByIndex(i).Id, text);
            }
            
        }

        // Отправка кнопочек в сообщении
        public async Task SendMessageMenuAll()
        {
           ///
        }
    }
}
