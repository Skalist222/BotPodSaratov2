using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotClean.Bot;
using TelegramBotClean.Commandses;
using TelegramBotClean.Data;
using TelegramBotClean.MemDir;
using TelegramBotClean.Messages;
using User = TelegramBotClean.Userses.User;

namespace TelegramBotClean.CommandsDir
{
    public class CommandsExecutor
    {
        public static void ExecuteUnknow(Sender sender, MessageI mes)
        {
            Console.WriteLine("Получена неизвестная команда");
        }
        public static void ExecuteStart(Sender sender, MessageI mes)
        {
            sender.SendAdminMessage("Получена команда старт");
            User user = sender.Users[mes.SenderId];
            // При начале работы бота или при нажатии на кнопку старт
            if (user == null)
            {
                if (sender.BotBase.CreateUser(new User(mes.Sender)))
                {
                    sender.Users.Add(sender.BotBase.GetUser(mes.SenderId));
                    sender.SendAdminMessage("Создан новый пользователь " + sender.Users[mes.SenderId].ToString());
                    Console.WriteLine("Добавлен новый пользователь");
                    sender.SendMenu(mes.SenderId, "Привет, дорогой друг. Этот бот предназначен для учеников воскресной школы(подростков). Нажми /help чтобы разобраться, как работает бот.");
                }
                else
                {
                    Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    Console.WriteLine("!!!Не удалось добавить пользователя!!!");
                    Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                }
            }
            else
            {
                sender.SendMenu(mes.SenderId, "Рад тебя снова видеть!");
            }
        }
        public static void ExecuteMem(Sender sender, MessageI mes)
        {
            sender.SendAdminMessage("Получена команда мем");
            Console.WriteLine("Команда мем");
            MessageI retMes = sender.Mems.GetMessageRandomMem(sender.Random);
            sender.SendMessage(retMes,mes.SenderId);
        }
        public static void ExecuteVerse(Sender sender, MessageI mes)
        {
            sender.SendAdminMessage("Получена Стих");
        }
        public static void ExecuteInfo(Sender sender, MessageI mes)
        {
            sender.SendAdminMessage("Получена команда Инфо");
            sender.SendMessage(Config.Info.Text,mes.SenderId);
        }


        public static void ExecuteGoldVerse(Sender sender, MessageI mes)
        {
            sender.SendAdminMessage("Получена команда Золотой стих");
        }
        public static void ExecuteAddGoldVerse(Sender sender, MessageI mes)
        {
            sender.SendAdminMessage("Получена команда добавления золотого стих");
        }
        public static void ExecuteAddMem(Sender sender, MessageI mes)
        {
            MessageI toSendMes = new MessageI("");
            if (mes.HavePhoto)
            {
                if (sender.Mems.Add(new Mem(mes)))
                {
                    string thenks = sender.BotBase.GetRandomAnswer(Commands.Get("спс"));
                    string mem = sender.BotBase.GetRandomAnswer(Commands.Get("мем"));
                    toSendMes.SetText(thenks + " " + mem);
                }
                else
                {
                    toSendMes.SetText("Ой прости.. что-то пошло не так, не смог добавить мем...");
                }
            }
            else
            {
                toSendMes.SetText("Ну слушай... я конечно добавлю мем, ты главное мне отправь его вместе с просьбой отправить");
            }
            sender.SendMessage(toSendMes,mes.SenderId);

            // после всех обработак отправляем админу инфу о меме
            sender.SendAdminMessage("Получен этот мем от " + sender.Users[mes.SenderId].Name + $"({mes.SenderId}):");
            sender.SendAdminMessage("file:" + mes.ImageId + " message:" + mes.Id);
            sender.SendAdminMessage(mes.Photo);
        }

    }
    
}
