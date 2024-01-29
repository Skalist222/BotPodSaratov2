using TelegramBotClean.Bot;
using TelegramBotClean.Commandses;
using TelegramBotClean.Data;
using TelegramBotClean.MemDir;
using TelegramBotClean.MenuDir;
using TelegramBotClean.Messages;
using TelegramBotClean.MessagesDir;
using TelegramBotClean.TextDir;
using TelegramBotClean.Userses;
using User = TelegramBotClean.Userses.User;

namespace TelegramBotClean.CommandsDir
{
    public class CommandsExecutor
    {
        //Adminka
        public static void ExecuteAdmin(Sender sender, MessageI mes)
        {
            //Админска
        }
        public static void ExecuteAdminMems(Sender sender, MessageI mes)
        {
            //получение всех мемов
        }
        public static void ExecuteAdminUsers(Sender sender, MessageI mes)
        {
            //получение всех мемов
        }

        public static void ExecuteAllAnonimMessages(Sender sender, MessageI mes)
        {
            string txtInData = "seenAnon";//Текст команды
            Dictionary<string ,UnansweredsMeesages> groups =  sender.UnansweredMessage.GetGroupsMessages();
            string[] uniqNames = groups.Select(el => el.Key).ToArray();
            MesMenuBut[] buts = new MesMenuBut[groups.Count];
            for (int i = 0; i <buts.Length;i++)
            {
                buts[i] = new MesMenuBut("от:"+uniqNames[i],txtInData+"|"+ uniqNames[i]);
            }
            MesMenuTable table = new MesMenuTable(buts);
            sender.SendMessageMenu(mes.SenderId,table,"Анонимные сообщения:");
            //получение всех мемов
        }

        public static void ExecuteUnknow(Sender sender, MessageI mes)
        {
            Console.WriteLine("Получена неизвестная команда");
            MesMenuTable menu = new YesNoMenu("Да","Нет");
            //sender.SendMenuMessage(new YesNoMenu("hfhfh", "asdasd"), sender.Users[mes.SenderId],"Менушка да нетка");
  
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
            MessageI retMes = sender.Mems.GetMessageRandomMem2(sender.Random);
            retMes.SetText(sender.BotBase.GetRandomAnswer("mem"));
            sender.SendMessage(retMes,mes.SenderId);
        }
        public static void ExecuteVerse(Sender sender, MessageI mes)
        {
            sender.SendAdminMessage("Получена команда Стих");
        }
        public static void ExecuteInfo(Sender sender, MessageI mes)
        {// сделано
            sender.SendAdminMessage("Получена команда Инфо");
            sender.SendMessage(Config.Info.Text,mes.SenderId);
        }
        public static void ExecuteHelp(Sender sender, MessageI mes)
        {// сделано
            sender.SendAdminMessage("Получена команда помощь");
            sender.SendMessage(TextWorker.Help(sender.Users[mes.SenderId]), mes.SenderId);
        }
        public static void ExecuteHowWorkButton(Sender sender, MessageI mes)
        {// сделано
            sender.SendAdminMessage("Получена команда как работают кнопки");
            sender.SendMessage(TextWorker.Help(sender.Users[mes.SenderId]), mes.SenderId);
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
        public static void ExecuteOnAnon(Sender sender, MessageI mes)
        {
            User u = sender.Users[mes.SenderId];
            if (u.TypeUser == UserTypes.Teen)
            {
                u.TeenInfo.SetInAnon(sender.TextWorker);
                sender.SendMenu(mes.SenderId, "Включена анонимная отправка сообщений");
            }
        }
        public static void ExecuteOffAnon(Sender sender, MessageI mes)
        {
            User u = sender.Users[mes.SenderId];
            if (u.TypeUser == UserTypes.Teen)
            {
                u.TeenInfo.SetNotInAnon(sender.TextWorker);
                sender.SendMenu(mes.SenderId, "Анонимизация отключена");
                //Вот здесь еще должен быть спам преподавателям о том, что пришли анонимки
                
            }
        }
       
        
        // Запускаются без команды
        public static void CreateAnonimMes(Sender sender, MessageI mes)
        {
            sender.BotBase
                .CreateAnonMessage(
                    mes, 
                    sender.Users[mes.SenderId].TeenInfo.AnonName
                );
            sender.UnansweredMessage.Add(new UnansweredAnonimMessage(
                mes.Text,
                sender.Users[mes.SenderId].TeenInfo.AnonName,
                mes.SenderId,
                (int)mes.Id
                ));
        }

        //Внутренние проверки
        private static bool ValidAnonimus(User user)
        {
            if (user.TypeUser == UserTypes.Teen)
            {
                return user.TeenInfo.InAnonim;
            }
            else return false;
        }
        public static string ValidAll(Sender sender, MessageI mes)
        {
            if (ValidAnonimus(sender.Users[mes.SenderId])) return "anon";
            return "OK";
        }

    }
    
}
