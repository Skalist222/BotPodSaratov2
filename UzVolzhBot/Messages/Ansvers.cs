using PodrostkiBot.Bible;

using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using PodrostkiBot.Users;
using static PodrostkiBot.Configure.ConstData;
using PodrostkiBot.Text;
using PodrostkiBot.App;
using Telegram.Bot.Exceptions;
using PodrostkiBot.UI;
using System.Data;
using PodrostkiBot.Configure;

namespace PodrostkiBot.Messages
{

    public class BoardTable : List<BoardRow> { }
    public class BoardRow : List<Telegram.Bot.Types.ReplyMarkups.KeyboardButton> { }

    public class MenuTable : List<MenuRow> { };
    public class MenuRow : List<InlineKeyboardButton> { }
    public class CreatorMessage
    {
        public Engine Engine { get; }
        DateTime StartCreateAnswer;
        int CountGoldVerses;
        bool adminLog;



        public CreatorMessage(ITelegramBotClient botClient)
        {
            Engine = new Engine(botClient);
            adminLog = false;
            Thread spam = new Thread(Engine.Spammer.SenderWorker);
            spam.Start();
            Engine.Sender.SendAdminMessageAsync("Был выполнен запуск бота");
        }
        public async void CreateAnsvere(Update up)
        {
            MessageI m = new MessageI(up,Engine);
          
            bool mesAnon = false;
            StartCreateAnswer = DateTime.Now;
            Message mes = up.Message;
            if (mes is not null)
            {
                //Тип сообщения
                MessageType type = up.Message.Type;
                User user = up.Message.From;
               
                //Получаю из списка пользователя по id
                UserI userMe = Engine.Users[user.Id];
                Logger.SaveMessage(userMe, m);
                if (userMe is null)
                {
                    Engine.Users.Add(new UserI(user, Engine.BibleWorker, Engine.BotBase));
                    userMe = Engine.Users[user.Id];
                    Engine.BotBase.AddUser(userMe);
                    Engine.Sender.SendMessageAsync("Новый пользователь " + userMe.ToString(), Engine.Users[1094316046L]);
                    Engine.Sender.SendMessageAsync("Чтобы не потерять инфу: " + Environment.NewLine +
                         "id|" + userMe.Id + Environment.NewLine +
                         "nick|" + userMe.Name + Environment.NewLine +
                         "lastName|" + userMe.LastName + Environment.NewLine +
                         "firstName|" + userMe.Firstname + Environment.NewLine +
                         "uniqName|" + userMe.UniqName + Environment.NewLine
                         , Engine.Users[1094316046L]);
                }
                if (userMe.Banned)
                {
                    Engine.Sender.SendMessageAsync("У вас Бан, я не могу ничем вам помочь. Обратитесь к разработчику бота @valvor222",userMe);
                    return;
                }
                // Если пользователя нет в списке, добавляю его в список и в базу данных
                

                Engine.Sender.CleanMenus(userMe);
                string text = "";
                    text = (up.Message.Text ?? up.Message.Caption) ?? "";
          
                Commands commands = Commands.SelectByText(TextHandler.DeleteChars(text), Engine.BotBase);
                
                //Commands2 commands2 = new Commands2(Engine.BotBase,TextHandler.DeleteChars(text),Engine.AllCommands,Engine.AllResponceWords);

              
                string responceText = "";
                //string selectorNames = Engine.Selector.GetSelectionName(commands2);
                //Engine.Executor.Execute(selectorNames,mes,userMe);

                if (adminLog)
                {
                    Engine.Sender.SendAdminMessageAsync( "Сообщение от " + user.Id + " " + user.Username + Environment.NewLine + text + Environment.NewLine +DateTime.Now.ToString());
                }
                
                // Если получаем фото, то проверяем это мем или просто дичь какая то
             
               
                    //Если полчаем текст
               if (text == UIWorker.TxtBtAnonTurnOff && !userMe.InAnonMessage)
                    {
                        Engine.Menus.SelectMenu(Engine.Sender, "Мир тебе",userMe);
                    }
                //Если подросток в анонимномном режиме
                if (userMe.InAnonMessage || userMe.InTeacherSendMessage || userMe.InSetTeamLesson || userMe.InOtherFeedBack || userMe.InRenaming)
                {
                    if (userMe.InAnonMessage)
                    {
                        mesAnon = true;
                        // если получили команду на отключение анонимности
                        if (commands.IsAnonTeenOff)
                        {
                            //Собственно отключаем анонимность
                            if (Engine.Users[user.Id].CountAnonimMessage > 0)
                            {
                                Engine.Spammer.SpamTeacherNewAnon();
                            }
                            Engine.Users[user.Id].SetInAnon(false);
                        }
                        else
                        {
                            Console.WriteLine("Прислали анонимное сообщение");
                            // Отправляем анонимное сообщение в базу данных
                            Engine.BotBase.AddAnonimMessage(userMe, text);
                            Engine.Users[user.Id].CountAnonimMessage++;
                        }
                    }
                    if (userMe.InSetTeamLesson)
                    {
                        Engine.BotBase.SetTeamLesson(userMe.DateSetTeamLeson, text);
                        //Answere(2041) СПАСИБО ЗА...
                        Engine.Sender.SendMessageAsync("Тема установлена на урок!" + Answere(2041) + " тему.", userMe);
                        userMe.SetInTeacmSetter(new DateTime());
                        Engine.Sender.NextThreeSundays(userMe);
                    }
                    if (userMe.InTeacherSendMessage)
                    {
                        if (commands.IsOffAnswerOnAnon)
                        {
                            // Онулирую
                            userMe.TeacherSendMessageTurnOff();
                            Engine.Menus.SelectMenu(Engine.Sender, "Отменена отправка сообщения ученику", userMe);
                        }
                        else
                        {
                            string nameAnswerer = "Ответ от (" + Engine.BotBase.GetNameUser(userMe.Id) + ")" + Environment.NewLine;
                            Engine.Sender.SendMessageAsync(nameAnswerer + text, userMe.IdTeenAnonMessage);
                            int lastId = Engine.BotBase.SetNewAnswerAnon(text, userMe.Id);
                            if (lastId == -1)
                            {
                                Engine.Sender.SendAdminMessageAsync("неправильно сработал ответ на анонимку");

                            }
                            else
                            {
                                Engine.BotBase.SetAnswerCurrentteen(userMe.IdTeenAnonMessage, lastId, userMe.IdDesiredTeacher);
                                Engine.Menus.SelectMenu(Engine.Sender, "Сообщение отправлено", userMe);
                                //Выключаю отправку ответа
                                Engine.Sender.CleanMenus(userMe);
                                Engine.Sender.FeedBackOnAnswereAnon(Engine.Users[userMe.IdTeenAnonMessage], lastId);
                                userMe.TeacherSendMessageTurnOff();
                            }
                        }
                    }
                    if (userMe.InOtherFeedBack)
                    {
                        if (userMe.Privilege == UserPrivilege.Teen)
                        {
                            Engine.BotBase.SetFeedback(userMe.Id, text, userMe.IdLessonOtherFeedBack);
                            userMe.SetIdLessonOtherFeedBack();
                            Engine.Sender.SendMessageAsync(Answere("/thanks") + " отзыв!", userMe);
                        }
                        else
                        {

                            userMe.SetIdLessonOtherFeedBack();
                            Engine.Sender.SendMessageAsync(Answere("/thanks") + " отзыв! Но он не учтется, так как ты не ученик!", userMe);
                        }

                    }
                    if (userMe.InRenaming)
                    {
                        Engine.BotBase.SetUniqName(userMe.IdRenameUser, text);
                        Engine.Sender.SendMessageAsync("Спасибо", userMe);
                        userMe.SetIdRenameUser();
                    }
                }
                else
                {
                    //Если команда не Библия, то обнуляется библия
                    if (!commands.IsBible)
                    {
                        if (userMe is not null) userMe.SetInBible("-");
                    }
                    //ADMIN
                    if (commands.IsFirstAdmin)
                    {
                        // Команды администратора будут работать только если ты админ (или Валентин аахахха)
                        if (Engine.Users[user.Id].Privilege == UserPrivilege.Admin || user.Id == 1094316046L)
                        {
                            string[] split = text.Split(' ');
                            string s1 = split[0] ?? "";s1 = s1.ToLower();
                            string s2 = split[1] ?? "";
                            if (split.Length == 1)
                            {
                                Engine.BotBase.GetAllLastLessons(true);//
                                return;
                            }
                            else
                            {
                                //Команда смены привелегий пользователю
                                if (split[1] == "setType")
                                {
                                    string priv = split[2];
                                    long idUser = 0;
                                    try
                                    {
                                        idUser = Convert.ToInt64(split[3]);
                                        AdminWorker.SetNewPrivelegeUser(Engine, Engine.Users[idUser], priv);
                                    }
                                    catch
                                    {
                                        Engine.Sender.SendAdminMessageAsync($"Неправильный id ({split[3]})");
                                    }
                                }
                                if (split[1] == "users")
                                {
                                    if (split.Length == 4)
                                    {
                                        if (split[2] == "ban")
                                        {
                                            try
                                            {
                                                long idUser = long.Parse(split[3].ToString());
                                                if(Engine.Users[idUser].SetBan(Engine.BotBase,true))
                                                {
                                                    Engine.Sender.SendMessageAsync("Вы получили БАН обратитесь к администратору бота",idUser);
                                                }
                                                
                                            }
                                            catch { }
                                        }
                                        if (split[2] == "deban")
                                        {
                                            try
                                            {
                                                int idUser = int.Parse(split[3].ToString());
                                                if (Engine.Users[idUser].SetBan(Engine.BotBase, false))
                                                {
                                                    Engine.Sender.SendMessageAsync("Вас вытащили из бана, поздравляю =)", idUser);
                                                }
                                            }
                                            catch { }
                                        }
                                    }
                                    else
                                    {
                                        Engine.Sender.AdminUsersRedact(userMe);
                                    }
                                    
                                }
                                if (split[1] == "golds")
                                {
                                    if (split.Length >= 3)
                                    {
                                        if (split[2] == "del")
                                        {
                                            List<int> idS = new List<int>();
                                            for (int i = 3; i < split.Length; i++)
                                            {
                                                if (split[i] != "")
                                                {
                                                    idS.Add(int.Parse(split[i]));
                                                }
                                            }
                                            Engine.BotBase.DeleteGoldVerses(idS.ToArray());
                                        }
                                        if (split[2] == "seen")
                                        {
                                            try
                                            {
                                                int idGold = int.Parse(split[3].ToString());
                                                Verse verse= Engine.Golds.GetGoldVerseById(idGold);
                                                Engine.Sender.SendMessageAsync(verse.ToString(), userMe);
                                            }
                                            catch
                                            {
                                                Engine.Sender.SendMessageAsync("Ошибка ввода айди",userMe);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        
                                        Engine.Sender.SendMessageAsync(AdminWorker.GetAllGolds(Engine.BotBase), userMe);
                                    }
                                }
                                if (split[1] == "spam")
                                {
                                    if (split.Length == 3 && split[2] == "restart")
                                    {
                                        // Нужно когда изменилась одна из менюшек, типа менюшки управления преподавателя
                                        Engine.Spammer.SpamRestartMenu(Engine.Menus);
                                    }
                                    else
                                    {
                                        string textMess = text.Replace(split[0], "").Replace(split[1], "");
                                        Engine.Spammer.SpamText(textMess);
                                    }
                                }
                                if (split[1] == "message" || split[1] == "mes")
                                {
                                    long idUser = 0;
                                    UserI u = null;
                                    try
                                    {
                                        idUser = Convert.ToInt64(split[2]);
                                        u = Engine.Users[idUser];
                                    }
                                    catch
                                    {
                                        Engine.Sender.SendAdminMessageAsync("Пользователь с айди {idUser} не найден");
                                        return;
                                    }

                                    string PS = Environment.NewLine + "P.S. письмо от создателя бота ❤️";
                                    int countSpace = split[0].Length + split[1].Length + split[2].Length + 2;
                                    await Engine.Sender.SendMessageAsync(text.Substring(countSpace) + PS, u);
                                    await Engine.Sender.SendAdminMessageAsync("Сообщение отправлено");
                                }
                                if (split[1] == "sendFeed")
                                {
                                    long idUser = 0;
                                    try
                                    {
                                        idUser = long.Parse(split[2]);
                                        Engine.Sender.SundaySpamMessage(Engine.Users[idUser]);
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                        }
                        else
                        {
                            Engine.Sender.SendMessageAsync("По жопе получишь!У тебя нет прав админа, не лезь сюда", userMe);
                        }
                    }
                    else
                    if (commands.IsEmpty)
                    {
                        responceText = Answere("/empty");
                        await Engine.Sender.SendMessageAsync(responceText, userMe);
                    }
                    else
                    // Команда старт (создает меню пользователя)
                    if (commands.IsStart)
                    {

                        responceText = Answere("/start") + Environment.NewLine;
                        responceText += Answere("/hello") + " " + Engine.BotBase.GetNameUser(userMe.Id);
                        Engine.Menus.SelectMenu(Engine.Sender, responceText, userMe);

                    }
                    else
                    // Команда библии (дает возможность открыть библию в три клика Книга, глава, стих)
                    if (commands.IsBible)
                    {

                        Engine.Sender.Bible(up, Engine.BibleWorker, Engine.Users[user.Id]);
                    }
                    else
                    // Получить инфу о боте (присылает текст из Info.txt)
                    if (commands.IsInfo)
                    {
                        responceText = GetInfo(userMe);
                        Engine.Sender.SendMessageAsync(responceText, userMe);
                    }
                    else
                    // Запрос мема, отправляет случайный мем
                    if (commands.IsMem)
                    {
                        //responceText = Answere("/mem");
                        await Engine.Sender.SendMessageAsync("Прости, дорогой друг, но функция мемов, больше не работает", userMe);
                        //Mem m = GetRandomMem();
                        //if (m.IsPhoto)
                        //{
                        //    await Engine.Sender.SendImage(mes, m.Path);
                        //    //if (user.Id != 1094316046L) Engine.Sender.SendImage(mes, m.Path, 1094316046L);
                        //}
                        //if (m.IsVideo)
                        //{
                        //    await Engine.Sender.SendVideo(mes, m.Path);
                        //    if (user.Id == 1094316046L) Engine.Sender.SendVideo(mes, m.Path, 1094316046L);
                        //}
                    }
                    else
                    // Запрос на получение рандомного стиха из библии
                    if (commands.IsStih)
                    {
                        responceText = GetRandomVerse(Answere("/stih"), user.Id);
                        await Engine.Sender.SendMessageAsync(responceText, userMe);
                    }
                    else
                    // Запрос золотого стиха, отправляет случайный золотой стих
                    if (commands.IsGoldStih)
                    {
                        responceText = GetRandomGoldVerse(Answere("/gold"));
                        await Engine.Sender.SendMessageAsync(responceText, userMe);
                    }
                    else
                    // Запрос на добавление золотого стиха, добавляет в БД последний прочитаный стих, как золотой
                    if (commands.IsAddGoldStih)
                    {
                        responceText =
                            Answere("/add") +
                            ". Спасибо за " + Answere("/gold");
                        await Engine.Sender.SendMessageAsync(AddGoldenStih(responceText, user.Id), userMe);
                    }
                    else
                    // Переход на предыдущий стих
                    if (commands.IsLeft)
                    {
                        responceText = PreVerse(user.Id);
                        await Engine.Sender.SendMessageAsync(responceText, userMe);
                    }
                    else
                    // Переход на следующий стих
                    if (commands.IsRight)
                    {
                        responceText = NextVerse(user.Id);
                        await Engine.Sender.SendMessageAsync(responceText, userMe);
                    }
                    else
                    // Запрос на вывод помощи, отправляет содержание файла Help.txt
                    if (commands.IsHelp)
                    {
                        responceText = GetHelp();
                        await Engine.Sender.SendMessageAsync(responceText, userMe);
                    }
                    else
                    // То же что и IsHelp только файл другой
                    if (commands.IsHowWrkBut)
                    {
                        responceText = GetHowWorkButton(userMe);
                        await Engine.Sender.SendMessageAsync(responceText, userMe);
                    }
                    else
                    if (commands.IsAnonTeenOn)
                    {
                        Engine.Sender.CoiceTeacherAnonMessage(userMe.Id);
                        Engine.Sender.OnAnonMessages(Engine.Sender, userMe, Engine.Menus);
                    }
                    else
                    if (commands.IsAnonMessages)
                    {
                        Engine.Sender.AnonimMessages2(userMe);
                    }
                    else
                    // Поиск стиха
                    if (commands.IsHaveSearchVerse || commands.IsHaveSearch)
                    {
                        responceText = Answere("/search") + ", " + Answere("/stih").ToLower();
                        await Engine.Sender.SendMessageAsync("Ищу, это может занять немного больше времени чем ты предполагаешь...", userMe);
                        //SendMessageAsync(botClient, mes, token, responceText);

                        new Thread(() =>
                        {
                            Verse selectedS = Engine.BibleWorker.SearchVerse(text);
                            DateTime start = DateTime.Now;
                            if (selectedS is null)
                            {
                                Engine.Sender.SendMessageAsync(Answere("/dontSelectedStih"), userMe);
                            }
                            else
                            {
                                userMe.SetLastVerse(selectedS, Engine.BotBase);
                                responceText += Environment.NewLine + selectedS.ToString();
                                DateTime finish = DateTime.Now;
                                int seconds = (finish - start).Seconds;
                                if (seconds > 1)
                                    responceText = $"Прости что так долго, вся работа поиска заняла:{seconds} сек." + Environment.NewLine + responceText;
                                Engine.Sender.SendMessageAsync(responceText, userMe);
                            }
                        }).Start();
                    }
                    else
                    // нажата кнопка уроки
                    if (commands.IsLessons)
                    {
                        Engine.Sender.NextThreeSundays(userMe);
                    }
                    // Нажата кнопка Отменить отправку ответа
                    else
                    if (commands.IsReportLesson || commands.IsReport)
                    {
                        if (userMe.Privilege == UserPrivilege.Teacher)
                        {
                            Engine.Sender.GetLessonsForReports(userMe);
                        }
                        else
                        {
                            Engine.Sender.SendOK(userMe.Id);
                        }
                    }
                    // Если вдруг ни одна команда не найдена, значит человек отправил простое сообщение
                    else
                    {
                        responceText = "Я не буду болтать, хозяин сказал мне не говорить глупостей";
                        await Engine.Sender.SendMessageAsync(responceText, userMe);
                    }

                    if (!mesAnon) Logger.Info("Сообщение от" + userMe.ToString() + ":" + Environment.NewLine + text + Environment.NewLine + DateTime.Now.ToString(), false, ConsoleColor.Green);
                    //await Engine.Sender.SendAdminMessageAsync(mes, type, "От " + user.FirstName + "(" + user.Id + ")");
                    //await Engine.Sender.SendAdminMessageAsync(mes, type, text);
                    //SaveMessage(text, up, responceText);
                }

                
            }
            //КОМАНДЫ ПРИНЯТЫЕ С КНОПОК ИЗ СООБЩЕНИЙ WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW
            else
            {
               
                string responceText = "";
                if (up.CallbackQuery is not null)
                {
                    CallbackQuery call = up.CallbackQuery;
                    long userId = up.CallbackQuery.From.Id;
                    
                    UserI userMe
                        = Engine.Users[up.CallbackQuery.From.Id];
                    string[] split = up.CallbackQuery.Data.Split('|');

                    //Работа с библией
                    if (split[0] == "book")
                    {
                        userMe.SetBookInBible(split[1]);
                        Engine.Sender.Chapters(up, Engine.BibleWorker, split[1], Engine.Users[userId]);
                        Engine.Sender.CleanMenus(userMe,call.Message.MessageId);
                    }
                    if (split[0] == "chapter")
                    {
                        userMe.SetChapterInBook(split[1]);
                        Engine.Sender.Verses(Engine.BibleWorker, userMe);
                        Engine.Sender.CleanMenus(userMe, call.Message.MessageId);
                    }
                    if (split[0] == "verse")
                    {
                        userMe.SetVerseInChapter(split[1]);
                        if (userMe.VerseInChapter is not null && userMe.BookInBible is not null && userMe.ChapterInBook is not null)
                        {
                            Verse verse = Engine.BibleWorker.GetVerse(userMe.AddressInBible);
                            userMe.SetLastVerse(verse,Engine.BotBase);
                            Engine.Sender.SendMessageAsync(verse.ToString(), userMe);
                            Engine.Users[userMe.Id].SetInBible("-");
                            Engine.Sender.DeleteMessage(userMe.MenuMessage,userMe.Id);
                            userMe.MenuMessage = null;
                        }
                        Engine.Sender.CleanMenus(userMe, call.Message.MessageId);
                    }
                    if (split[0] == "nextVerses")
                    {
                        Engine.Sender.NextVerses(Engine.BibleWorker, userMe, Convert.ToInt32(split[1].ToString()));
                        Engine.Sender.CleanMenus(userMe, call.Message.MessageId);
                    }
                    if (split[0] == "nextChapters")
                    {
                        Engine.Sender.NextChapters(Engine.BibleWorker, userMe, Convert.ToInt32(split[1].ToString()));
                        Engine.Sender.CleanMenus(userMe,call.Message.MessageId);
                    }

                        // нажата кнопка из списка уроков которая выводится при нажатии "уроки"
                    if (split[0] == "setLes")
                    {
                        string[] split2 = split[1].Split(':');
                        DateTime date = new DateTime();
                        date = Convert.ToDateTime(split2[1].ToString());
                        if (split2[0] == "Ведет")
                        {
                            Engine.Sender.QuestionRefreshTeacher(date, userMe);
                        }
                        if (split2[0] == "Хочу вести")
                        {
                            //Уроку не прикреплен преподаватель открепляем, и говорим об этом преподавателю
                            Engine.BotBase.SetTeacherOnLesson(date,userId);
                            Engine.Sender.SendMessageAsync("Поставил вас на "+date.ToShortDateString(), userMe  );
                            Engine.Sender.NextThreeSundays(Engine.Users[userId]);
                            Engine.Sender.CleanMenus(userMe, call.Message.MessageId);

                        }
                        if (split2[0] == "Я веду")
                        {
                            Engine.Sender.VariantsUsageLesson(Engine.Users[userId],date);
                            //К уроку прикреплен этот преподаватель,
                            //надо предложить сместить себя
                            Engine.Sender.CleanMenus(userMe, call.Message.MessageId);

                        }
                        // Если урок ни на кого не установлен
                    }
                    // Смена учителя на уроке
                    if (split[0] == "refTeachLess")
                    {
                        DateTime date = Convert.ToDateTime(split[1].ToString());
                        // Нажата кнопка "Да", при вопросе о смене преподавателя на уроке
                        long idLastTeacher = Engine.BotBase.GetIdTeacherLesson(date);
                        string nameNewTeacher = Engine.BotBase.GetNameUser(userMe.Id);
                        string nameLastTeacher = Engine.BotBase.GetNameUser(idLastTeacher);
                        Engine.BotBase.SetTeacherOnLesson(date, userId);
                        Engine.Sender.SendMessageAsync( "Поставил вас на " + date.ToShortDateString(), userMe);
                        Engine.Sender.NextThreeSundays( Engine.Users[userId]);
                        Engine.Sender.SendMessageAsync( nameNewTeacher + " забрал ваш урок " + date.ToShortDateString(), userMe);
                        Engine.Sender.CleanMenus(userMe,call.Message.MessageId);
                    }
                    //убрать с уроков текущего учителя(если учитель и был установлен на этом уроке)
                    if (split[0] == "displaceTeachLess")
                    {
                        DateTime date = Convert.ToDateTime(split[1].ToString());
                        Engine.BotBase.SetTeacherOnLesson(date);
                        Engine.Sender.SendMessageAsync( "Убрал из ваших уроков " + date.ToShortDateString(), userMe);
                        Engine.Sender.NextThreeSundays( Engine.Users[userId]);
                        Engine.Sender.CleanMenus(userMe,call.Message.MessageId);
                    }
                    //АНОНИМНЫЕ СООБЩЕНИЯ
                    //Выбран один из анонимных учеников в списке анонимных сообщений
                    if (split[0] == "anonMessages")
                    {
                        try
                        {
                            long idTeen = Convert.ToInt64(split[1].ToString());
                            long idDesiredTeacher = Convert.ToInt64(split[2].ToString());
                            UserI teen = Engine.Users[idTeen];
                            UserI teacher = Engine.Users[userId];
                            UserI desTeacher = Engine.Users[idDesiredTeacher];
                            Engine.Sender.CurrentAnonimMessages(teen, teacher,desTeacher);
                        }
                        catch (ApiRequestException)
                        {
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.GetType().ToString());
                        }
                        Engine.Sender.CleanMenus(userMe,call.Message.MessageId);
                    }
                    //нажато "Да", при вопросе "Хотите ответить на анонимное сообщение"
                    if (split[0] == "answerAnonMes")
                    {
                        long idTeen = Convert.ToInt64(split[1].ToString());
                        long idDesTeacher = Convert.ToInt64(split[2].ToString());
                        userMe.TeacherSendMessageTurnOn(idTeen);
                        userMe.SetDesiredTeacher(idDesTeacher);
                        Engine.Sender.TeacherInAnswerAnonMenu("Дорогой учитель! То сообщение, которое ты сейчас отправишь, будет отправлено ученику, будь внимателен!", userMe.Id);
                        Engine.Sender.CleanMenus(userMe,call.Message.MessageId);
                    }
                    //Пришел Фидбэк от ученика
                    if (split[0] == "aL")
                    {
                        string textFeedBack = Engine.BotBase.GetTextWariantFeedbackById(Convert.ToInt32(split[1].ToString()));
                        int idLesson = Convert.ToInt32(split[2].ToString());
                        Engine.Sender.SendAdminMessageAsync("Пришел фидбэк (" + textFeedBack + ") от " + userMe.Name + "");
                        if (!Engine.BotBase.HaveFeedBack(userMe.Id, textFeedBack, idLesson))
                        {
                            
                            Engine.BotBase.SetFeedback(userMe.Id, textFeedBack, idLesson);
                            Engine.Sender.SendMessageAsync(  $"({textFeedBack}) " + Answere("/ok"), userMe);
                        }
                        else
                        {
                            Engine.Sender.SendMessageAsync(  $"({textFeedBack}) Этот вариант ты уже выбирал", userMe);
                        }
                        
                    }
                    // При выборе своего урока преподаватель выбрал пункт удалить себя с урока
                    if (split[0] == "dTeachLess")
                    {
                        DateTime date = Convert.ToDateTime(split[1]);
                        Engine.Sender.QuestionDisplaceTeacher(date, Engine.Users[userId]);
                        Engine.Sender.CleanMenus(userMe,call.Message.MessageId);
                    }
                    if (split[0] == "setTeamLesson")
                    {
                        userMe.SetInTeacmSetter(Convert.ToDateTime(split[1].ToString()));// Включаем ожидание темы урока
                        Engine.Sender.SendMessageAsync( "Какую тему поставить?",userMe);
                        Console.WriteLine("попытка установить тему урока");
                        Engine.Sender.CleanMenus(userMe,call.Message.MessageId);
                    }
                    if (split[0] == "reportLesson")
                    {
                        DateTime date = Convert.ToDateTime(split[1].ToString());
                        Engine.Sender.GetReport(userMe, date);
                        Engine.Sender.CleanMenus(userMe,call.Message.MessageId);
                    }
                    if (split[0] == "aLOther")
                    {
                        userMe.SetIdLessonOtherFeedBack(Convert.ToInt64(split[1]));
                        Engine.Sender.SendMessageAsync( "Напиши своё впечатление одним сообщением",userMe);
                        Engine.Sender.CleanMenus(userMe,call.Message.MessageId);
                    }
                    if (split[0] == "setPriv")
                    {
                        Engine.Sender.CleanMenus(userMe,call.Message.MessageId);
                    }
                    if (split[0] == "setName")
                    {
                         await Engine.Sender.SendMessageAsync(  "Какое имя поставить человеку?", userMe);
                        long idUser = Convert.ToInt64(split[1].ToString());
                        userMe.SetIdRenameUser(idUser);
                        Engine.Sender.CleanMenus(userMe,call.Message.MessageId);
                    }
                    if (split[0] == "setDesired")
                    {
                       
                        if (split[1] != "0")
                        {
                            userMe.SetDesiredTeacher(long.Parse(split[1].ToString()));
                        }
                        Engine.Sender.SendMessageAsync( Answere("/ok"), userMe);
                        Engine.Sender.CleanMenus(userMe,call.Message.MessageId);
                    }
                    if (split[0] == "feedAnswAnon")
                    {
                        DataTable asw = Engine.BotBase.GetOneAnswerOnAnonim(long.Parse(split[1]));
                        long idTeacher = long.Parse(asw.Rows[0]["idTeacher"].ToString());
                        string textteacherMess = asw.Rows[0]["text"].ToString();
                        string textfeedBack = split[2];
                        if (textteacherMess.Length > 70) textteacherMess = textteacherMess.Substring(0, 70)+"...";

                        string text = $"На твой ответ ({textteacherMess}) получен отзыв \"{textfeedBack}\"";
                        Engine.Sender.SendMessageAsync( text, Engine.Users[idTeacher]).GetAwaiter().GetResult();
                        Engine.Sender.CleanMenus(userMe,call.Message.MessageId);
                    }
                    // Любой ответ НЕТ приведет сюда, бот просто скажет что то типа(ладно)
                    if (split[0] == "NO")
                    {
                        Engine.Sender.SendMessageAsync( Answere("/ok"), userMe);
                        Engine.Sender.CleanMenus(userMe,call.Message.MessageId);
                    }
                }
                else
                {
                    Console.WriteLine("Получено пустое сообщение");
                }
            }
            DateTime FinisCreationAnswere = DateTime.Now;
            string mesTime = $"Время обработки запроса: {(FinisCreationAnswere - StartCreateAnswer).TotalSeconds} сек.";
            if(adminLog)Engine.Sender.SendAdminMessageAsync(mesTime);
            Logger.Info(mesTime, false, ConsoleColor.Cyan);

        }
        // Getters
        public string GetInfo(UserI user)
        {
            string info = BotInfo(user.Privilege);
            info += Environment.NewLine;
            info += "Золотых стихов:" + CountGoldVerses + Environment.NewLine;
            return info;
        }
        public string GetHelp()
        {
            string info = System.IO.File.ReadAllText(PathHelp);
            info += Environment.NewLine;
            return info;
        }
        public string GetHowWorkButton(UserI user)
        {
            return HowWorkButton(user.Privilege);
        }
        public string GetRandomVerse(string preview, long idUser)
        {
            UserI selectuser = Engine.Users[idUser];
            Verse newVerse = Engine.BibleWorker.GetRandomVerse();
            if (selectuser is not null)
            {
                Engine.Users[idUser].SetLastVerse(newVerse,Engine.BotBase);
            }
            string t = preview + Environment.NewLine + Environment.NewLine;
            //lastUserShih.Add(new USL(idUser, stih));
            return t + newVerse.ToString();
        }
        public string GetRandomGoldVerse( string preview)
        {
            string t = preview + Environment.NewLine;
            Verse stih = Engine.Golds.GetRandomGoldVerse(Engine.Random);
            string stihS = stih.ToString();
            return t + stihS;
        }
       
        public string AddGoldenStih(string preview, long idUser)
        {
            Verse newGoldStih = Engine.Users[idUser] is not null ? Engine.Users[idUser].LastVerse : null;
            if (newGoldStih is null)
            {
                return GetRandomVerse("Ты еще не получал случайный стих и не можешь добавить золотой стих =(" + Environment.NewLine + "Но не печалься, вот тебе случайный стих, можешь его добавить если понравится 🥰", idUser);
            }
            try
            {
                if (Engine.Golds.Add(newGoldStih))
                {
                    CountGoldVerses = Engine.Golds.Count(Engine.BotBase);
                    return preview;
                }
                else
                {
                    return "Ой, а этот "+Answere("/gold")+" уже есть в базе данных, но все равно спасибо =)";
                }
               
            }
            catch (ApiRequestException)
            {
                AppWorker.Restart();
                return null;
            }
            catch (Exception e)
            {
                return "кажется какая то кракозябра... не работает добавление, скажи об этом владельцу бота";
            }
           
        }
       
      
        public string PreVerse(long idUser)
        {
            Verse l = Engine.Users[idUser] is not null ? Engine.Users[idUser].LastVerse : null;
            if (l is not null)
            {
                Verse preStih = Engine.BibleWorker.GetPreVerse(l);
                Engine.Users[idUser].SetLastVerse(preStih,Engine.BotBase);
                return preStih.ToString();
            }
            else
            {
                return GetRandomVerse("Ты еще не запрашивал случайный стих, поэтому вот тебе он", idUser);
            }
        }
        public string NextVerse(long idUser)
        {
            Verse l = Engine.Users[idUser] is not null ? Engine.Users[idUser].LastVerse : null;
            if (l is not null)
            {
                Verse nextSt = Engine.BibleWorker.GetNextVerse(l);
                Engine.Users[idUser].SetLastVerse(nextSt,Engine.BotBase);
                return nextSt.ToString();
            }
            else
            {
                return GetRandomVerse("Ты еще не запрашивал случайный стих, поэтому вот тебе он", idUser);
            }
        }

        //Admin
        public void SaveMessage(string message, Update up, string responce, UserI userIn = null)
        {
            UserI u = userIn ?? Engine.Users[up.Message.From.Id] ?? new UserI(-1,"","","",Engine.BibleWorker,UserPrivilege.Teen);
            string textInFile = "----------------Start Message---------------" + Environment.NewLine;
            textInFile += $"({u.Name} {u.LastName} {u.Id}) ";
            textInFile += DateTime.Now.ToLongDateString() + Environment.NewLine;
            textInFile += "mesText: " + message + Environment.NewLine;
            textInFile += "responce: " + responce + Environment.NewLine;
            textInFile += "____________________________________________" + Environment.NewLine;
            string pathFile = Path.Combine(DefaultPath, "Messages.txt");
            Console.WriteLine(textInFile);
            System.IO.File.AppendAllText(pathFile, textInFile);
        }
       
        public string Answere(string command)
        {
            return Engine.BotBase.GetRandomAnsvere(command);
        }
        public string Answere(int numCommand)
        {
            return Engine.BotBase.GetRandomAnsvere(numCommand);
        }
        


    }
}