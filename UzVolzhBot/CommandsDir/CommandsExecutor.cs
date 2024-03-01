using TelegramBotClean.Bot;
using TelegramBotClean.Data;
using TelegramBotClean.MemDir;
using TelegramBotClean.MenuDir;
using TelegramBotClean.Messages;
using TelegramBotClean.MessagesDir;
using TelegramBotClean.TextDir;
using TelegramBotClean.UsersesDir;
using User = TelegramBotClean.Users.User;

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
            if (mes.Sender.IsAdmin) sender.SendAdminMessage(sender.Users.AsTextTable());
            else sender.SendMessage("Ты не админ не пытайся получить данные пользователей!", mes.SenderId);

        }
        public static void ExAdminSenNewPrivilege(Sender sender, MessageI mes)
        {
            if (mes.Sender.IsAdmin)
            {
                string[] split = mes.Text.Split(' ');
                if (split.Length == 5)
                {
                    try
                    {
                        long updateUserId = Convert.ToInt64(split[3]);
                        User u = sender.Users[updateUserId];  //
                        Privileges type = Privileges.Select(split[4]);
                        if (type is null)
                        {
                            sender.SendAdminMessage($"Я не знаю такой тип, как ({split[4]})");
                        }
                        else
                        {
                            
                        }
                    }
                    catch 
                    {
                        sender.SendAdminMessage($"Айди пользователя должно тип айди не соответствует({split[3]})");
                    }
                }
                else
                {
                    sender.SendAdminMessage("Неверное количество вводимых данных, нужно 5 а у тебя "+split.Length);
                }
            }
            else sender.SendMessage("Ты не админ не пытайся получить данные пользователей!", mes.SenderId);

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
            sender.SendMenu(mes.SenderId, "Рад тебя снова видеть!");
            
        }
        public static void ExMem(Sender sender, MessageI mes)
        {
            
            Console.WriteLine("Команда мем");
            MessageI retMes = sender.Mems.GetMessageRandomMem2(sender.Random);
            if (retMes is not null)
            {
                retMes.SetText(sender.BotBase.GetRandomAnswer("mem"));
            } 
            else  retMes = new MessageI("Простите, но в базе данных нет мемов");
            sender.SendMessage(retMes, mes.SenderId);
            sender.SendAdminMessage("Получена команда мем");

        }
        public static void ExVerse(Sender sender, MessageI mes)
        {
            sender.SendAdminMessage("Получена команда Стих");
            string retText = sender.Bible.GetRandomVerse().TextWithAddress;
            sender.SendMessage(retText, mes.SenderId);
        }
        public static void ExGoldVerse(Sender sender, MessageI mes)
        {
            sender.SendAdminMessage("Получение случайного золотого стиха");
            string retText = sender.TextWorker.RandomAnswere("золотой","стих")+ TextWorker.Ln +
                sender.Bible.GetRandomGoldVerse().TextWithAddress;
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
            if (mes.Type == MessageTypes.PhotoCommand || mes.Type == MessageTypes.PhotoText)
            {
                MessageI toSendMes = new MessageI("");

                if (sender.Mems.Add(new Mem(mes))) 
                    toSendMes.SetText(sender.TextWorker.RandomAnswere("спс", "мем"));
                else 
                    toSendMes.SetText("Ой прости... что-то пошло не так, не смог добавить мем...");
                
                // отправляем ответ пользователю
                sender.SendMessage(toSendMes, mes.SenderId);

                //отправляем админу инфу о боте
                sender.SendAdminMessage($"Мем от ({sender.Users[mes.SenderId].Name} {mes.SenderId}):");
                sender.SendAdminMessage(mes);
            }
        }


        public static void ExOnAnon(Sender sender, MessageI mes)
        {
            // Создаем кнопки для выбора предпочитаемого учителя
            Users.Users teachers = sender.Users.Teachers;
            MesMenuTable table = new MesMenuTable();
            MesMenuBut but = null;
            for (int i = 0; i < teachers.Count; i++)
            {
                but = new MesMenuBut(teachers.ByIndex(i).Name, "set went |" + teachers.ByIndex(i).Id);
                table.Add(new MesMenuRow(but));
            }
            table.Add(new MesMenuRow(new MesMenuBut("Не важно", "sayNo went")));

            MessageI menuMessage = new MessageI(sender.SendMenuMessage(table, mes.Sender, "Кому нужно ответить?").Result,mes.Sender);
            User u = sender.Users[mes.SenderId];
            if (u.Type == Privileges.Teen)
            {
                u.SetInAnonOn(sender.TextWorker, menuMessage);
                sender.SendMenu(mes.SenderId, "Включена анонимная отправка сообщений");
            }
        }
        public static void ExOffAnon(Sender sender, MessageI mes)
        {
            User u = sender.Users[mes.SenderId];
            if (u.Type == Privileges.Teen)
            {
                u.SetInAnonOff(sender.TextWorker);
                sender.SendMenu(mes.SenderId, "Анонимизация отключена");
                //Если ученик 
                if(mes.Sender.IdWentTeacher != 0)
                    sender.SendMessage(
                    "Ученик хочет чтобы на анонимное сообщение ответил именно ты",
                    mes.Sender.IdWentTeacher);
                //Вот здесь еще должен быть спам преподавателям о том, что пришли анонимки
                
            }
        }
        public static void ExCreateAnonimMes(Sender sender, MessageI mes)
        {
            Console.WriteLine("Добавлена анонимка");
            sender.BotBase.CreateAnonMessage(mes,mes.Sender.AnonName,mes.Sender);
        }

        public static void ExOnAnswereAnon(Sender sender, MessageI mes)
        {
            User u = sender.Users[mes.SenderId];
            long idTeen = Convert.ToInt64(mes.Text.Split('|')[1]);
            if (u.Type == Privileges.Teacher)
            {
                u.TurnOnAnswerOnAnon(idTeen);
                sender.SendMenu(mes.SenderId, "Отправленное дальше сообщение будет направлено ученику");
            }
        }
        public static void ExOffAnswereAnon(Sender sender, MessageI mes)
        {
            User u = sender.Users[mes.SenderId];
           
            if (u.Type == Privileges.Teacher)
            {
                u.TurnOffAnswerOnAnon();
                sender.SendMenu(mes.SenderId, "Отправка ответа отменена");
            }
        }
        public static void ExSendAnswerAnon(Sender sender, MessageI mes)
        {
            User Teacher = sender.Users[mes.SenderId];
            User Teen = sender.Users[Teacher.IdTeenAnonMessage];
            if (Teacher.Type == Privileges.Teacher)
            {
                sender.UnansweredMessage.SetAnswered(Teen.Id);
                sender.SendMessage($"Ответ на анонимку от ({Teacher.UniqueName})", Teen.Id);
                sender.SendMessage(mes.Text, Teen.Id);
                Teacher.TurnOffAnswerOnAnon();
                sender.SendMenu(Teacher.Id,"Ответ отправлен!");
                
            }
            ExOffAnswereAnon(sender,mes);
        }
        public static void ExSetWentTeacher(Sender sender, MessageI mes)
        {
            if (mes.Sender.Type == Privileges.Teen)
            {
                long idTeacher = Convert.ToInt64(mes.Text.Split('|')[1]);
                mes.Sender.SetWentedTeacher(idTeacher,mes);
            }
            else
            {
                Console.WriteLine("захотели поставить конкретного учителя но не ученик");
            }
            
        }
        public static void ExSetNoWentTeacher(Sender sender, MessageI mes)
        {
            //Удаляем мену с вопросом
            sender.DeleteMesMenu(mes.Sender.WentedTeacherMessage,mes.Sender);
            //отправляем сообщение типа "ладно"
            sender.SendMessage(sender.TextWorker.RandomAnswere("ok"),mes.Sender.Id);
            
            //Console.WriteLine("Ученику не важно какой учитель ответит ему на анонимку") ;
        }

     
    }
    
}
