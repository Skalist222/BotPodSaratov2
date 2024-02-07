using Telegram.Bot.Types;
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
        public static void ExAdmin(Sender sender, MessageI mes)
        {
            //Админска
        }
        public static void ExAdminMems(Sender sender, MessageI mes)
        {
            //for (int i = 0; i < sender.Mems.Count; i++)
            //{

            //}
            long idMes = sender.Mems[0].Message.Id;
           


            //получение всех мемов
        }
        public static void ExAdminUsers(Sender sender, MessageI mes)
        {
            //получение всех мемов
        }

        public static void ExAllAnonimMessages(Sender sender, MessageI mes)
        {
            GroupsUbabsvereMessage groups = sender.UnansweredMessage.GetGroupsMessages();
            string[] uniqNames = groups.Select(el => el.UniqUserName).ToArray();
            //read anon - текст команды, который отправ
            MesMenuTable table = MesMenuTable.CreateMenu("read anon", uniqNames);
            sender.SendMessageMenu(mes.SenderId,table,"Анонимные сообщения:");
            //получение всех мемов
        }
        public static void ExReadAnon(Sender sender, MessageI mes)
        {
            string uniqNameAnon = mes.Text.Split('|')[1];
            GroupsUbabsvereMessage groups = sender.UnansweredMessage.GetGroupsMessages();
            UnansveredsMeesages messages = groups[uniqNameAnon].Messages;
            long idTeen = groups[uniqNameAnon].IdAnonUser;
            string textMessage = "Сообщение от " + uniqNameAnon + ":" + TextWorker.Ln + TextWorker.Ln;
            for (int i = 0; i < messages.Count; i++)
            {
                textMessage += messages[i].Text + TextWorker.Ln;
            }
            sender.SendMessage(textMessage, mes.SenderId);
            MesMenuTable table = new YesNoMenu("answere anon messages |"+ idTeen, "turnOff answere");
            sender.SendMessageMenu(mes.SenderId, table, "Ответить на анонимку?");
        }

        public static void ExUnknow(Sender sender, MessageI mes)
        {
            Console.WriteLine("Получена неизвестная команда");
            
            //sender.SendMenuMessage(new YesNoMenu("hfhfh", "asdasd"), sender.Users[mes.SenderId],"Менушка да нетка");
  
        }
        public static void ExStart(Sender sender, MessageI mes)
        {
            sender.SendAdminMessage("Получена команда старт");
            User user = sender.Users[mes.SenderId];
            // При начале работы бота или при нажатии на кнопку старт
            if (user == null)
            {
                if (sender.BotBase.CreateUser(new User(new Telegram.Bot.Types.User() )))
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
        public static void ExMem(Sender sender, MessageI mes)
        {
            sender.SendAdminMessage("Получена команда мем");
            Console.WriteLine("Команда мем");
            MessageI retMes = sender.Mems.GetMessageRandomMem2(sender.Random);
            if (retMes is not null)
            {
                retMes.SetText(sender.BotBase.GetRandomAnswer("mem"));
            } 
            else  retMes = new MessageI("Простите, но в базе данных нет мемов");
            sender.SendMessage(retMes, mes.SenderId);

        }
        public static void ExVerse(Sender sender, MessageI mes)
        {
            sender.SendAdminMessage("Получена команда Стих");
            string retText = sender.BibleWorker.GetRandomVerse().TextWithAddress;
            sender.SendMessage(retText, mes.SenderId);
        }
        public static void ExGoldVerse(Sender sender, MessageI mes)
        {
            sender.SendAdminMessage("Получение случайного золотого стиха");
            string retText = sender.TextWorker.RandomAnswere("золотой") + " " + sender.TextWorker.RandomAnswere("стих") + TextWorker.Ln +
                sender.BibleWorker.GetRandomGoldVerse().TextWithAddress;
            sender.SendMessage(retText, mes.SenderId);
        }


        public static void ExInfo(Sender sender, MessageI mes)
        {// сделано
            sender.SendAdminMessage("Получена команда Инфо");
            sender.SendMessage(Config.Info.Text,mes.SenderId);
        }
        public static void ExHelp(Sender sender, MessageI mes)
        {// сделано
            sender.SendAdminMessage("Получена команда помощь");
            sender.SendMessage(TextWorker.Help(sender.Users[mes.SenderId]), mes.SenderId);
        }
        public static void ExHowWorkButton(Sender sender, MessageI mes)
        {// сделано
            sender.SendAdminMessage("Получена команда как работают кнопки");
            sender.SendMessage(TextWorker.HowWorkButton(sender.Users[mes.SenderId]), mes.SenderId);
        }
        
        public static void ExAddGoldVerse(Sender sender, MessageI mes)
        {
            sender.SendAdminMessage("Получена команда добавления золотого стих");
        }
        public static void ExAddMem(Sender sender, MessageI mes)
        {
            MessageI toSendMes = new MessageI("");
            if (mes.Type == MessageTypes.Photo)
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
            sender.SendAdminMessage("file:" + mes.FileId + " message:" + mes.Id);
            sender.SendAdminMessage(mes.FileId);
        }


        public static void ExOnAnon(Sender sender, MessageI mes)
        {
            User u = sender.Users[mes.SenderId];
            if (u.TypeUser == UserTypes.Teen)
            {
                u.TeenInfo.SetInAnon(sender.TextWorker);
                sender.SendMenu(mes.SenderId, "Включена анонимная отправка сообщений");
            }
        }
        public static void ExOffAnon(Sender sender, MessageI mes)
        {
            User u = sender.Users[mes.SenderId];
            if (u.TypeUser == UserTypes.Teen)
            {
                u.TeenInfo.SetNotInAnon(sender.TextWorker);
                sender.SendMenu(mes.SenderId, "Анонимизация отключена");
                //Вот здесь еще должен быть спам преподавателям о том, что пришли анонимки
                
            }
        }
        public static void ExCreateAnonimMes(Sender sender, MessageI mes)
        {
            Console.WriteLine("Обработка анонимных сообщений");
        }

        public static void ExecuteOnAnswereAnon(Sender sender, MessageI mes)
        {
            User u = sender.Users[mes.SenderId];
            long idTeen = Convert.ToInt64(mes.Text.Split('|')[1]);
            if (u.TypeUser == UserTypes.Teacher)
            {
                u.TeacherInfo.TurnOnAnswerOnAnon(idTeen);
                sender.SendMenu(mes.SenderId, "Отправленное дальше сообщение будет направлено ученику");
            }
        }
        public static void ExecuteOffAnswereAnon(Sender sender, MessageI mes)
        {
            User u = sender.Users[mes.SenderId];
           
            if (u.TypeUser == UserTypes.Teacher)
            {
                u.TeacherInfo.TurnOffAnswerOnAnon();
                sender.SendMenu(mes.SenderId, "Отправка ответа отменена");
            }
        }
        public static void ExecuteSendAnswerAnon(Sender sender, MessageI mes)
        {
            User Teacher = sender.Users[mes.SenderId];
            User Teen = sender.Users[Teacher.TeacherInfo.IdTeenAnonMessage];
            if (Teacher.TypeUser == UserTypes.Teacher)
            {
                sender.UnansweredMessage.SetAnswered(Teen.Id);
                sender.SendMessage($"Ответ на анонимку от ({Teacher.UniqName})", Teen.Id);
                sender.SendMessage(mes.Text, Teen.Id);
                Teacher.TeacherInfo.TurnOffAnswerOnAnon();
                sender.SendMenu(Teacher.Id,"Ответ отправлен!");
                
            }
            ExecuteOffAnswereAnon(sender,mes);
        }


        // Запускаются без команды
       

        //Внутренние проверки
        private static bool ValidAnonimus(User user)
        {
            if (user.TypeUser == UserTypes.Teen)
            {
                return user.TeenInfo.InAnonim;
            }
            else return false;
        }
        private static bool ValidAnswerOnAnonim(User user)
        {
            if (user.TypeUser == UserTypes.Teacher)
            {
                return user.TeacherInfo.InAnswerAnon;
            }
            else return false;
        }

        public static string ValidAll(Sender sender, MessageI mes)
        {
            if (ValidAnonimus(sender.Users[mes.SenderId])) return "anon";
            if (ValidAnswerOnAnonim(sender.Users[mes.SenderId])) return "answerAnon";
            return "OK";
        }
      

    }
    
}
