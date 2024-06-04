using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Drawing;
using static PodrostkiBot.Configure.ConstData;
using Telegram.Bot.Types.ReplyMarkups;
using PodrostkiBot.Bible;
using System.IO;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using PodrostkiBot.Users;
using PodrostkiBot.DataBase.Engine;
using MemWorkerSpace;
using static System.Net.WebRequestMethods;
using Telegram.Bot.Exceptions;
using PodrostkiBot.Text;
using PodrostkiBot.App;
using static System.Runtime.InteropServices.JavaScript.JSType;
using PodrostkiBot.UI;
using System.Reflection;
using System;
using PodrostkiBot.Menus;
using System.Diagnostics.PerformanceData;

namespace PodrostkiBot.Messages
{
    public class Sender
    {
        private string textBible = "🕮Книги Библии___________________________";
        public ITelegramBotClient BotClient { get; }
      
        public BotBase BotBase { get; }
        public BibleWorker BibleWorker{ get;}
      
        UserList users;
        public Sender(ITelegramBotClient botClient, BotBase botBase,UserList users, BibleWorker bible)
        {
            this.BotClient = botClient;
            this.BotBase = botBase;
            this.users = users;
            this.BibleWorker = bible;
        }

        public async Task SendMenu(string answer,ReplyKeyboardMarkup menu, UserI user)
        {
            try
            {
                   BotClient.SendTextMessageAsync(
                           chatId: user.Id,
                           text: answer,
                           replyMarkup: menu
                            );
            }
            catch (ApiRequestException ex)
            {
                Logger.Error("Не получилось отправить меню");
                  SendMessageAsync(  "Вызвана ошибка! Не удалось отправить менюшку для " + user.ToString(), 1094316046L);
                if (ex.Message == "Bad Request: message must be non-empty")
                {
                    return;
                } 
            }
        }
        public async Task SendMessageMenu(string answer, InlineKeyboardMarkup menu, UserI user)
        {
            try
            {
                user.MenuMessage = await BotClient.SendTextMessageAsync(
                  chatId: user.Id,
                  text: answer,
                  replyMarkup: menu);
            }
         
            catch (ApiRequestException ex)
            {
               
                Logger.Error("Не получилось отредактировать менюшку отправил новую");
                Logger.Error(ex.Message);
                SendAdminMessageAsync("Вызвана ошибка! Не удалось отправить менюшку для " + user.ToString());
                if (ex.Message == "Bad Request: message must be non-empty")
                {
                    return;
                }
            }

        }
        public async Task SendMessageAsync(string info,UserI user)
        {
            try
            {
                Message sendArtwork = await  BotClient.SendTextMessageAsync(
                chatId: user.Id,
                text: info,
                parseMode: ParseMode.Html);
                Logger.Info($"Пользователю {user.Id} было отправлено сообщение:", true, ConsoleColor.Blue);
                Logger.Info(info, true, ConsoleColor.Blue);
                Logger.InfoStop();
            }
            catch (ApiRequestException ap)
            {
                if (ap.Message != "Bad Request: chat not found")
                {
                    Logger.Error("Ошибка при отправке сообщения");
                    Logger.Error(ap.Message);
                }
                else
                {
                    Logger.Error("Не найден чат " + user.Id);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Logger.Error("Не удалось отправить сообщение пользователю ...(" + user.Id + "): " + e.Message);
              
            }
           
        }
        public async Task SendMessageAsync(string info, long idUser)
        {
            try
            {
                Message sendArtwork = await BotClient.SendTextMessageAsync(
                chatId: idUser,
                text: info,
                parseMode: ParseMode.Html);
                Logger.Info($"Пользователю {idUser} было отправлено сообщение:", true, ConsoleColor.Blue);
                Logger.Info(info, true, ConsoleColor.Blue);
                Logger.InfoStop();
            }
            catch (ApiRequestException ap)
            {
                if (ap.Message != "Bad Request: chat not found")
                {
                    Logger.Error("Ошибка при отправке сообщения");
                    Logger.Error(ap.Message);
                }
                else
                {
                    Logger.Error("Не найден чат " + idUser);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine();
                Logger.Error("Не удалось отправить сообщение пользователю ...(" + idUser + "): " + e.Message);

            }

        }
        public async Task SendAdminMessageAsync(string text)
        {
            long id = 1094316046;
            SendMessageAsync(text, users[id]);
           
        }
        public async Task SendImage(string fileId, UserI user, string caption = "")
        {
            
            //long id = idChat == -1 ? message.Chat.Id : idChat;
            //var cap = caption == "" ? message.Caption ?? "" : caption;
            //var bm = Bitmap.FromFile(pathImage);
            //var ms = new MemoryStream();
            //bm.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            //ms.Position = 0;
            //using (var fileStream = new FileStream(pathImage, FileMode.Open, FileAccess.Read, FileShare.Read))
            //{
            //    InputFile file = InputFile.FromStream(fileStream);
            //      await BotClient.SendPhotoAsync(
            //        chatId: id,
            //        photo: file,
            //        caption: cap
            //    );
            //}
            SendMessageAsync("Прости, но пока отправка картинок не работает", user);
        }
       
        public async Task DeleteMessage(Message message,long userDeleted)
        {
            DeleteMessage(message.MessageId, userDeleted);
        }
        public async Task DeleteMessage(CallbackQuery callBack, long userDeleted)
        {
            DeleteMessage(callBack.Message.MessageId, userDeleted);
        }
        public async Task DeleteMessage(int messageId, long userDeleted)
        {
            try
            {
                BotClient.DeleteMessageAsync(
                 chatId: userDeleted,
                 messageId: messageId
                 );
            }
            catch (Exception e) 
            {
                Logger.Error(
                    "Не удалось удалить сообщение"+Environment.NewLine+
                    "Тип ошибки:"+e.GetType().ToString()+Environment.NewLine+
                    e.Message);
            }
                   
                
              
        }

        /// <summary>
        /// Установить меню в соответствии с его привелегиями
        /// </summary>
        /// <param name="answer">Текст выводимый в обратном сообщении</param>
        /// <param name="inform">текст для дополнительной информации</param>
        /// <param name="users">Список пользователей</param>
        /// <param name="bW">класс работы с библией</param>
        /// <param name="id"> id пользователя которому устанавливаем меню</param>
        public async void SendOK(long id)
        {
             SendMessageAsync( BotBase.GetRandomAnsvere("/ok")??"ок",id);
        }
        public async void TeacherInAnswerAnonMenu(string answer, long id, string inform = "")
        {
            answer += inform + Environment.NewLine;
            UserI user;
            user = users[id];
            BoardTable board = new BoardTable();
            board.Add(new BoardRow() { UIWorker.offAnswerOnAnonBut});

            ReplyKeyboardMarkup mrkp = new ReplyKeyboardMarkup(keyboard: board);
            mrkp.ResizeKeyboard = true;
            try
            {
                  BotClient.SendTextMessageAsync(
                               chatId: id,
                               text: answer,
                               replyMarkup: mrkp
                );
            }
            catch (ApiRequestException ex)
            {
                if (ex.Message == "Bad Request: message must be non-empty") return;
                  SendAdminMessageAsync(ex.Message); 
            }
            catch (Exception e)
            {
                Console.WriteLine($"Не удалось установить меню ({id}) {user.Firstname} {user.LastName}");
                SendAdminMessageAsync($"Не удалось установить меню ({id}) {user.Firstname} {user.LastName}");
            }
            if (inform != "")   SendMessageAsync(inform, user);
        }

        
        public async void Bible(Update up, BibleWorker bW,UserI user)
        {
            if (user.InBibleParametr == "b") return;
            MenuRow row = new MenuRow();
            MenuTable table = new MenuTable();
            List<MenuTable> tables = new List<MenuTable>();
            InlineKeyboardButton but;
            string[] shortNames = bW.GetBooksShortNames();
           
            int column = 0;
            int rows = 0;
            for (int i = 0; i < shortNames.Length; i++)
            {
                row = new MenuRow();
                for (int j = 0; j < 6; j++)
                {
                    if (i >= shortNames.Length)
                    {
                        
                        break;
                    }
                    InlineKeyboardButton btn = new InlineKeyboardButton(shortNames[i]+ "");
                    btn.CallbackData = "book|" + shortNames[i];
                    btn.CallbackData = "book|" + shortNames[i];
                    row.Add(btn);
                    i++;
                }
                table.Add(row);
            }
            InlineKeyboardMarkup ikm = new InlineKeyboardMarkup(table);
            user.SetInBible("b");
            SendMessageMenu(textBible, ikm, user);

        }
        public async void Chapters(Update up, BibleWorker bW,string shortNameBook,UserI user)
        {
            MenuRow row = new MenuRow();
            MenuTable table = new MenuTable();
            List<MenuTable> tables = new List<MenuTable>();
            InlineKeyboardButton but;
            int countChapters = bW.ChapterCount(shortNameBook);
            string bookName = bW.GetBookNameByShortName(shortNameBook);
            int column = 0;
            int rows = 0;
            bool breakAll = false;
            bool bigBook = false;
            for (int i = 0; i < countChapters || !breakAll; i++)
            {
                InlineKeyboardButton btn;
                row = new MenuRow();
                for (int j = 0; j < 6; j++)
                {
                    if (i == 99)
                    {
                        btn = new InlineKeyboardButton("➡️");
                        btn.CallbackData = "nextChapters|" + (countChapters - 99) + "|" + user.Id;
                        row.Add(btn);
                        breakAll = true;
                        bigBook = true;
                        break;

                    }
                    if (i >= countChapters)
                    {
                        break;
                    }
                    btn = new InlineKeyboardButton((i + 1) + "");
                    btn.CallbackData = "chapter|" + (i + 1);
                    row.Add(btn);
                    i++;
                }
                if (i < countChapters)
                {
                    btn = new InlineKeyboardButton((i + 1) + "");
                    btn.CallbackData = "chapter|" + (i + 1);
                    row.Add(btn);
                }

                table.Add(row);
            }



            bool breacAll = false;
            for (int i = 0; i < countChapters; i++)
            {
                InlineKeyboardButton btn;
                row = new MenuRow();
                for (int j = 0; j < 6; j++)
                {
                    if (i == 99)
                    {
                        btn = new InlineKeyboardButton("➡️");
                        btn.CallbackData = "nextChapters|" + (countChapters-99);
                        row.Add(btn);
                        breacAll = true;
                        break;
                    }
                    if (i >= countChapters)
                    {
                        break;
                    }
                    btn = new InlineKeyboardButton((i + 1) + "");
                    btn.CallbackData = "chapter|" + (i+1);
                    row.Add(btn);
                    i++;
                }
                if (breacAll)
                {
                    table.Add(row);
                    break;
                } 
                if (i < countChapters)
                {
                    btn = new InlineKeyboardButton((i + 1) + "");
                    btn.CallbackData = "chapter|" + (i + 1);
                    row.Add(btn);
                }
                table.Add(row);
            }
           

            int minSize = textBible.Length;
            InlineKeyboardMarkup ikm = new InlineKeyboardMarkup(table);
            string text = $"🕮{bookName} Выберите главу";

            if (text.Length < minSize)
                for (int i = 0; i < minSize - text.Length; i++)
                    text += "_";

            SendMessageMenu(text, ikm, user);
            //🕮Книги Библии__________________________
            
          
        }
        public async void Verses(BibleWorker bW, UserI user)
        {
            if (user.InBibleParametr == "v") return;
            MenuRow row = new MenuRow();
            MenuTable table = new MenuTable();
            List<MenuTable> tables = new List<MenuTable>();
            InlineKeyboardButton but;
            int countVerses = bW.VersesCount(user.BookInBible, user.ChapterInBook);
            string bookName = bW.GetBookNameByShortName(user.BookInBible);
            int column = 0;
            int rows = 0;
            bool breakAll=false;
            for (int i = 0; i < countVerses || !breakAll; i++)
            {
                InlineKeyboardButton btn;
                row = new MenuRow();
                for (int j = 0; j < 6; j++)
                {
                    if (i == 99)
                    {
                        btn = new InlineKeyboardButton("➡️");
                        btn.CallbackData = "nextVerses|" + (countVerses - 99)+"|"+user.Id;
                        row.Add(btn);
                        breakAll = true;
                        break;

                    }
                    if (i >= countVerses)
                    {
                        break;
                    }
                    btn = new InlineKeyboardButton((i + 1) + "");
                    btn.CallbackData = "verse|" + (i + 1);
                    row.Add(btn);
                    i++;
                }
                if (i < countVerses)
                {
                    btn = new InlineKeyboardButton((i + 1) + "");
                    btn.CallbackData = "verse|" + (i + 1);
                    row.Add(btn);
                }

                table.Add(row);
            }
            InlineKeyboardMarkup menu = new InlineKeyboardMarkup(table);
            string text = $"🕮{bookName} {user.ChapterInBook}:Выберите стих";
            int minSize = textBible.Length;
            if (text.Length < minSize)
                for (int i = 0; i < minSize - text.Length; i++)
                    text += "_";
            SendMessageMenu(text, menu, user);
        }
        public async void NextVerses(BibleWorker bW, UserI user,int countNextVerses)
        {
            MenuRow row = new MenuRow();
            MenuTable table = new MenuTable();
    
            InlineKeyboardButton but;
            int countVerses = countNextVerses;
            string bookName = bW.GetBookNameByShortName(user.BookInBible);


            for (int i = 0; i < countVerses; i++)
            {
                InlineKeyboardButton btn;
                row = new MenuRow();
                for (int j = 0; j < 6; j++)
                {
                    if (i >= countVerses)
                    {
                        break;
                    }
                    btn = new InlineKeyboardButton((99+(i + 1)) + "");
                    btn.CallbackData = "verse|" + (99 + (i + 1));
                    row.Add(btn);
                    i++;
                }
                if (i < countVerses)
                {
                    btn = new InlineKeyboardButton((99 + (i + 1)) + "");
                    btn.CallbackData = "verse|" + (99+(i + 1));
                    row.Add(btn);
                }

                table.Add(row);
            }

            //🕮Книги Библии__________________________
            int minSize = textBible.Length;
            InlineKeyboardMarkup ikm = new InlineKeyboardMarkup(table);
            string text = $"🕮{bookName} {user.ChapterInBook}: Выберите стих";
            SendMessageMenu(text, ikm, user);

        }
        public async void NextChapters(BibleWorker bW, UserI user,int countNextChapters)
        {
            MenuRow row = new MenuRow();
            MenuTable table = new MenuTable();
            List<MenuTable> tables = new List<MenuTable>();
            InlineKeyboardButton but;
            int countChapters = countNextChapters;
            string bookName = bW.GetBookNameByShortName(user.BookInBible);
            for (int i = 0; i < countChapters; i++)
            {
                InlineKeyboardButton btn;
                row = new MenuRow();
                for (int j = 0; j < 6; j++)
                {
                    if (i >= countChapters)
                    {
                        break;
                    }
                    btn = new InlineKeyboardButton((99 + (i + 1)) + "");
                    btn.CallbackData = "chapter|" + (99+(i + 1));
                    row.Add(btn);
                    i++;
                }
                if (i < countChapters)
                {
                    btn = new InlineKeyboardButton((99 + (i + 1)) + "");
                    btn.CallbackData = "chapter|" + (99 + (i + 1));
                    row.Add(btn);
                }

                table.Add(row);
            }
            tables.Add(table);

            InlineKeyboardMarkup menu = new InlineKeyboardMarkup(table);
            string text = $"🕮{bookName} Выберите главу";
            //🕮Книги Библии__________________________
            int minSize = textBible.Length;
            if (text.Length < minSize)
                for (int i = 0; i < minSize - text.Length; i++)
                    text += "_";
            SendMessageMenu(text,menu,user);

        }
        
        
        public async void SundaySpamMessage(UserI user)
        {
            if (user is null) return;
            MenuTable table = new MenuTable();
            MenuRow row = new MenuRow();
            // если айди не пришел, то айди определяется автоматический
            DataTable variantsTable = BotBase.GetAllWariantsFeedbacks();
            if (variantsTable == null)
            {
                Console.WriteLine("Нет вариантов ответов на фидбэк");
                return;
            }
            InlineKeyboardButton but;
            int idLesson = BotBase.GetIdLastLesson();
            if (idLesson == -1)
            {
                Console.WriteLine("Предполагаемый курок для рассылки не найден");
                return;
            }

            InlineKeyboardButton btn;
            for (int i = 0; i < variantsTable.Rows.Count; i++)
            {
                row = new MenuRow();

                btn = new InlineKeyboardButton(variantsTable.Rows[i]["text"] + "");
                string buttonText = "aL|" + variantsTable.Rows[i]["id"] + "|" + idLesson;
                // Максимум текста в кнопке, должно быть 64
                if (buttonText.Length >= 64) buttonText = buttonText.Substring(0,63);
                btn.CallbackData = buttonText;
                row.Add(btn);
                if (i + 1 < variantsTable.Rows.Count)
                {
                    btn = new InlineKeyboardButton(variantsTable.Rows[i + 1]["text"] + "");
                    buttonText = "aL|" + variantsTable.Rows[i + 1]["id"] + "|" + idLesson;
                    // Максимум текста в кнопке, должно быть 64
                    if (buttonText.Length >= 64) buttonText = buttonText.Substring(0, 63);
                    btn.CallbackData = buttonText;
                    i++;
                    row.Add(btn);
                }
                table.Add(row);
            }
            row = new MenuRow();
            btn = new InlineKeyboardButton("!Другой вариант!");
            btn.CallbackData = "aLOther|"+ idLesson;
            row.Add(btn);
            table.Add(row);
            // Предыдущий урок
            string afterLesson = BotBase.GetRandomAnsvere("/hello") + ", " + BotBase.GetNameUser(user.Id) + ", " + BotBase.GetRandomAnsvere("/afterLesson")+Environment.NewLine+"Можешь выбрать несколько вариантов";

            InlineKeyboardMarkup ikm = new InlineKeyboardMarkup(table);
            SendMessageMenu(afterLesson, ikm, user);
            
        }
        public async void OnAnonMessages(Sender sender, UserI user, BaseMenu menus)
        {
            string messageOff = "Анонимность отключена";
            user.SetInAnon(true);
            menus.SelectMenu(sender, "Анонимность включена", user);

            new Thread(() => 
            {
                Message startMessage = BotClient.SendTextMessageAsync(
                  chatId: user.Id,
                  text: "Все сообщения которые ты отправишь во время работы таймера, будут анонимны =)",
                  parseMode: ParseMode.Html).Result;

                // Здесь ставится таймер (часы, минуты, секунды)
                TimeSpan time = new TimeSpan(0,5,0);
                bool startedAnonMessages = true;
                string info = time.Minutes + ":" + time.Seconds;
                Message sendArtwork = BotClient.SendTextMessageAsync(
                   chatId: user.Id,
                   text: info,
                   parseMode: ParseMode.Html).Result;
                while (startedAnonMessages)
                {
                    // Если подросток отключит анонимные сообщения
                    if (!user.InAnonMessage) 
                    {
                        BotClient.EditMessageTextAsync(user.Id, sendArtwork.MessageId, messageOff);
                        startedAnonMessages = false;
                        break; 
                    }
                    info = time.Minutes + ":" + time.Seconds;
                    BotClient.EditMessageTextAsync(user.Id, sendArtwork.MessageId, info);
                    Thread.Sleep(1000);
                    time -= new TimeSpan(0, 0, 1);
                    if (time == new TimeSpan(0, 0, 0))
                    {
                       
                        BotClient.EditMessageTextAsync(user.Id, sendArtwork.MessageId, messageOff);
                        startedAnonMessages = false;
                    }
                }

                user.SetInAnon(false);
                menus.SelectMenu(sender, messageOff, user);
               
            }).Start();
        }

        public async void NextThreeSundays( UserI teacher)
        {
            long idTeacher = teacher.Id;
            // если айди не пришел, то айди определяется автоматический
            DateTime[] nextThreeSun =  DateWorker.GetNextThreeSundays();
            List<string> textButtons = new List<string>();
            List<string> callbackMessages = new List<string>();
            foreach (DateTime date in nextThreeSun)
            {
                long idSelectedTeacher = BotBase.GetIdTeacherLesson(date);
                string name = BotBase.GetNameTeacherLesson(date);
                string team = BotBase.GetTeamLessonByDate(date);
                //
                team = team != "" ? $"(Тема: {team})" : "";

                if (idSelectedTeacher == 0) 
                {
                    textButtons.Add("Хочу вести:" + date.ToShortDateString()+team);
                    callbackMessages.Add("setLes|Хочу вести:" + date.ToShortDateString());
                }
                else
                if (idSelectedTeacher == idTeacher) 
                { 
                    textButtons.Add("Я веду:" + date.ToShortDateString() + team);
                    callbackMessages.Add("setLes|Я веду:" + date.ToShortDateString());
                }
                else 
                { 
                    textButtons.Add("Ведет:" + name + ":" + date.ToShortDateString() + team);
                    callbackMessages.Add("setLes|Ведет:" + date.ToShortDateString());
                }
            }

            DateTime now = DateTime.Now;
            MenuRow row = new MenuRow();
            MenuTable table = new MenuTable();
            List<MenuTable> tables = new List<MenuTable>();
            InlineKeyboardButton but;
            int rows = 0;
            string text = "";
            for (int i = 0; i < textButtons.Count; i++)
            {
                InlineKeyboardButton btn;
                btn = new InlineKeyboardButton(textButtons[i] + "");
                btn.CallbackData = callbackMessages[i];
                row = new MenuRow();
                row.Add(btn);
                table.Add(row);
            }
            tables.Add(table);
            // Предыдущий урок
            string afterLesson = "Грядущие уроки"+DateTime.Now.ToShortTimeString();

            foreach (MenuTable t in tables)
            {
                InlineKeyboardMarkup ikm = new InlineKeyboardMarkup(t);
                try
                {
                    
                        teacher.MenuMessage = await BotClient.SendTextMessageAsync(
                            chatId: idTeacher,
                            text: afterLesson,
                            replyMarkup: ikm
                            );
                    
                }
                catch (ApiRequestException) { Logger.Error("Вызвана ошибка, перезапускаю приложение"); }

            }
        }
        /// <summary>
        /// Хотите провести этот урок?(Когда урок прикреплен за другого преподавателя)
        /// </summary>
        /// <param name="idteacher"></param>
        /// <param name="date"></param>
        public async void QuestionRefreshTeacher(DateTime date, UserI teacher)
        {
            string afterLesson = "Хотите провести этот урок?";

            DateTime now = DateTime.Now;
            MenuRow row = new MenuRow();
            MenuTable table = new MenuTable();
            List<MenuTable> tables = new List<MenuTable>();
            InlineKeyboardButton but;
            int rows = 0;
            string text = "";
            row = new MenuRow();
            but = new InlineKeyboardButton("Да");
            but.CallbackData = "refTeachLess|" + date.ToShortDateString();
            row.Add(but);
            but = new InlineKeyboardButton("Нет");
            but.CallbackData = "NO|" + date.ToShortDateString();
            row.Add(but);

            table.Add(row);

            tables.Add(table);
            // Предыдущий урок


            foreach (MenuTable t in tables)
            {
                InlineKeyboardMarkup ikm = new InlineKeyboardMarkup(t);
                try
                {
                    if (teacher.MenuMessage == null)
                    {
                        teacher.MenuMessage = teacher.YesNoMessage = await BotClient.SendTextMessageAsync(
                            chatId: teacher.Id,
                            text: afterLesson,
                            replyMarkup: ikm
                            );
                    }
                    else
                    {
                        teacher.MenuMessage = teacher.YesNoMessage = await BotClient.EditMessageTextAsync(
                            messageId: teacher.MenuMessage.MessageId,
                            chatId: teacher.Id,
                            text: afterLesson,
                            replyMarkup: ikm
                            );
                    }
                }
                catch (ApiRequestException ApiEx) 
                {
                    Logger.Error("Вызвана ошибка, перезапускаю приложение");
                }

            }
        }
        /// <summary>
        /// Хотите убрать себя с этого урока?(Урок прикреплен за этим пользователем)
        /// </summary>
        /// <param name="idteacher"></param>
        /// <param name="date"></param>
        public async void VariantsUsageLesson(UserI teacher,DateTime date)
        {
            string team = BotBase.GetTeamLessonByDate(date);
            string afterLesson = "Урок " + date.ToShortDateString() + Environment.NewLine
                + "Ведет " + teacher.UniqName + Environment.NewLine
                + ((team!="") ?
                    "Тема:" + team + Environment.NewLine :
                    "Тема не установлена" + Environment.NewLine);
                
            DateTime now = DateTime.Now;
            MenuRow row = new MenuRow();
            MenuTable table = new MenuTable();
            List<MenuTable> tables = new List<MenuTable>();
            InlineKeyboardButton but;
            int rows = 0;
            string text = "";
            row = new MenuRow();
            but = new InlineKeyboardButton("Удалить из моих уроков");
            but.CallbackData = "dTeachLess|" + date.ToShortDateString();
            row.Add(but);
            but = new InlineKeyboardButton("Установить тему урока");
            but.CallbackData = "setTeamLesson|" + date.ToShortDateString();
            row.Add(but);
            but = new InlineKeyboardButton("Ничего");
            but.CallbackData = "NO";
            row.Add(but);

            table.Add(row);
            InlineKeyboardMarkup ikm = new InlineKeyboardMarkup(table);
            SendMessageMenu("Что нужно сделать?", ikm, teacher);
        }
        public async void QuestionDisplaceTeacher( DateTime date, UserI user)
        {
            string afterLesson = "Точно удалить?";

            DateTime now = DateTime.Now;
            MenuRow row = new MenuRow();
            MenuTable table = new MenuTable();
            List<MenuTable> tables = new List<MenuTable>();
            InlineKeyboardButton but;
            int rows = 0;
            string text = "";
            row = new MenuRow();
            but = new InlineKeyboardButton("Да");
            but.CallbackData = "displaceTeachLess|" + date.ToShortDateString();
            row.Add(but);
            but = new InlineKeyboardButton("Нет");
            but.CallbackData = "NO|" + date.ToShortDateString();
            row.Add(but);

            table.Add(row);

            InlineKeyboardMarkup ikm = new InlineKeyboardMarkup(table);
            Thread.Sleep(100);
            SendMessageMenu(afterLesson, ikm, user);
            
        }
        //Старая версия анонимок
        //public async void AnonimMessages(UserI user)
        //{
        //    MenuTable table = new MenuTable();
        //    List<MenuTable> tables = new List<MenuTable>();
        //    InlineKeyboardButton but;
        //    MenuRow row;
        //    // если айди не пришел, то айди определяется автоматический
         
        //    long[] idAnonimUsers = BotBase.GetIdUsersMessagesAnon();
        //    List<string> usingAnonimNames = new List<string>();
        //    string text = idAnonimUsers.Length>0? "Есть анонимные сообщения":"Анонимных сообщений нет";
        //    string[] anonNames = BotBase.GetAllAnonimNames();
        //    int countRestartListNames = 1;
        //    for (int i = 0; i < idAnonimUsers.Length; i++)
        //    {
        //        string name = anonNames[i];
        //        long id = idAnonimUsers[i];
                
        //        long idDesTeacher = BotBase.GetIdDesiredteacher(id);
        //        string desString = idDesTeacher != 0?" для "+BotBase.GetNameUser(idDesTeacher) :"";
        //        InlineKeyboardButton btn;
        //        btn = new InlineKeyboardButton("От " + name+ desString);
        //        btn.CallbackData = "anonMessages|" +id;
        //        row = new MenuRow();
        //        row.Add(btn);
        //        table.Add(row);
        //    }
           
        //    // Предыдущий урок
        //    InlineKeyboardMarkup ikm = new InlineKeyboardMarkup(table);
        //    SendMessageMenu(text,ikm,user);
        //}
        public async void AnonimMessages2(UserI user)
        {
           
            MenuTable table = new MenuTable();
            List<MenuTable> tables = new List<MenuTable>();
            InlineKeyboardButton but;
            MenuRow row;
            // если айди не пришел, то айди определяется автоматический
            DataTable allAnonims = BotBase.GetAllAnonimNotAnsweredMessage();
            Dictionary<(long,long),string> texts = new Dictionary<(long, long), string>();//Тексты сообщений
            for (int i = 0; i < allAnonims.Rows.Count; i++)
            {
                DataRow r = allAnonims.Rows[i];

                long idDesTeacher = Convert.ToInt64(r["desiredTeacher"].ToString());
                long idTeen = Convert.ToInt64(r["idTeen"].ToString());
                string textMessage = r["text"].ToString();
                if (texts.ContainsKey((idTeen,idDesTeacher)))
                {
                    texts[(idTeen, idDesTeacher)] += textMessage+Environment.NewLine;
                }
                else
                {
                    texts.Add((idTeen, idDesTeacher), textMessage);
                }   
            }
           
            string[] anonNames = BotBase.GetAllAnonimNames();
            string text = texts.Count > 0 ? "Есть анонимные сообщения" : "Анонимных сообщений нет";
            if (text == "Анонимных сообщений нет")
            {
                SendMessageAsync(  text, user);
                return;
            }
            for (int i = 0; i < texts.Count; i++)
            {
                (long,long) ids = texts.Keys.ToArray()[i];
                string name = anonNames[i];
                long id = ids.Item1;
                long idDesTeacher = ids.Item2;
                string desString = idDesTeacher != 0 ? " для " + BotBase.GetNameUser(idDesTeacher) : "";
                InlineKeyboardButton btn;
                btn = new InlineKeyboardButton("От " + name + desString);
                btn.CallbackData = "anonMessages|" + id+"|"+idDesTeacher;
                row = new MenuRow();
                row.Add(btn);
                table.Add(row);
            }
            InlineKeyboardMarkup ikm = new InlineKeyboardMarkup(table);
            SendMessageMenu(text, ikm, user);
            


            //long[] idAnonimUsers = BotBase.GetIdUsersMessagesAnon();
            //List<string> usingAnonimNames = new List<string>();
            //string text = idAnonimUsers.Length > 0 ? "Есть анонимные сообщения" : "Анонимных сообщений нет";
            //string[] anonNames = BotBase.GetAllAnonimNames();
            //int countRestartListNames = 1;
            //for (int i = 0; i < idAnonimUsers.Length; i++)
            //{
            //    string name = anonNames[i];
            //    long id = idAnonimUsers[i];

            //    long idDesTeacher = BotBase.GetIdDesiredteacher(id);
            //    string desString = idDesTeacher != 0 ? " для " + BotBase.GetNameUser(idDesTeacher) : "";
            //    InlineKeyboardButton btn;
            //    btn = new InlineKeyboardButton("От " + name + desString);
            //    btn.CallbackData = "anonMessages|" + id;
            //    row = new MenuRow();
            //    row.Add(btn);
            //    table.Add(row);
            //}

            //// Предыдущий урок
            //InlineKeyboardMarkup ikm = new InlineKeyboardMarkup(table);
            //SendMessageMenu(text, ikm, user);
        }
        public async void CurrentAnonimMessages(UserI teen, UserI teacherSeen,UserI desTeacher)
        {

            MenuTable table = new MenuTable();
            InlineKeyboardButton btn;
            MenuRow row = new MenuRow();
            string text = "Анонимное сообщение";
            long desTeacherId = 0;
            if (desTeacher is not null) desTeacherId = desTeacher.Id;
            string textReturnMessage = BotBase.GetAnonMessagesCurrentUser(teen.Id, desTeacherId);
            SendMessageAsync(  
                text+Environment.NewLine + Environment.NewLine+ textReturnMessage,
                teacherSeen
                );
            
            btn = new InlineKeyboardButton("Да");
            btn.CallbackData = "answerAnonMes|" + teen.Id+"|"+ desTeacherId;
            row.Add(btn);
            btn = new InlineKeyboardButton("Нет");
            btn.CallbackData = "NO|anonMessages|" + teen.Id + "|" + desTeacherId;
            row.Add(btn);
            table.Add(row);

            InlineKeyboardMarkup ikm = new InlineKeyboardMarkup(table);
            Thread.Sleep(100);
            teacherSeen.YesNoMessage = await BotClient.SendTextMessageAsync(
                            chatId: teacherSeen.Id,
                            text: "Ответить?",
                            replyMarkup: ikm
                            );
        }
        
        /// <summary>
        /// Отзыв о полученом ответе на анонимки
        /// </summary>
        /// <param name="teen"></param>
        /// <param name="teacher"></param>
        public async void FeedBackOnAnswereAnon(UserI teen,long idAnswere)
        {
            MenuTable table = new MenuTable();
            InlineKeyboardButton btn;
            MenuRow row = new MenuRow();
            string text = "Оцени ответ:";
            string[] vars = new string[] { "💖", "Я во всем разобрался", "Остались вопросы","Ничего не понял", "😾" };
           
            for (int i = 0; i < vars.Length; i++)
            {
                btn = new InlineKeyboardButton(vars[i]);
                btn.CallbackData = "feedAnswAnon|" + idAnswere+ "|" + vars[i];
                row = new MenuRow();
                row.Add(btn);
                table.Add(row);
            }
            //table.Add(row);

            InlineKeyboardMarkup ikm = new InlineKeyboardMarkup(table);
            Thread.Sleep(100);
            teen.MenuMessage = await BotClient.SendTextMessageAsync(
                            chatId: teen.Id,
                            text: text,
                            replyMarkup: ikm
                            );
        }

        public async void GetLessonsForReports(UserI teacher)
        {
            MenuRow row = new MenuRow();
            MenuTable table = new MenuTable();
            List<MenuTable> tables = new List<MenuTable>();
            InlineKeyboardButton but;
            string text = "Выберите Урок";

            DataTable t = BotBase.GetAllLastLessons(true);
            foreach (DataRow r in t.Rows)
            {
                DateTime date = Convert.ToDateTime(r["Date"].ToString());
                string sId = r["idTeacher"].ToString();
                long idTeacher = 0;
                string teachername = "";

                if (sId != "")
                {
                    idTeacher = Convert.ToInt64(sId);
                    teachername = BotBase.GetNameUser(idTeacher);
                }
                string teamLesson = r["Team"].ToString();
                but = new InlineKeyboardButton("Урок от "+date.ToShortDateString() + " " + teachername+" "+ teamLesson);
                but.CallbackData = $"reportLesson|{date.ToShortDateString()}";
                row = new MenuRow();
                row.Add(but);
                table.Add(row);
            }

            InlineKeyboardMarkup ikm = new InlineKeyboardMarkup(table);
            try
            {
                if (teacher.MenuMessage == null)
                {
                    teacher.MenuMessage = await BotClient.SendTextMessageAsync(
                        chatId: teacher.Id,
                        text: text,
                        replyMarkup: ikm
                        );
                }
                else
                {
                    if (teacher.MenuMessage.Text != text)
                    {
                        teacher.MenuMessage = await BotClient.EditMessageTextAsync(
                       messageId: teacher.MenuMessage.MessageId,
                       chatId: teacher.Id,
                       text: text,
                       replyMarkup: ikm
                       );
                    }
                }
            }

            catch (ApiRequestException ex) 
            {
                SendAdminMessageAsync("GetLessonsForReports-Ищи в классе Sender ~1160"+ex.Message);
                
            }
        }
        public async void GetReport(UserI teacher, DateTime dateLesson)
        {
            CleanMenus(teacher);
            bool haveUnic = false;
            DataTable feeds = BotBase.GetFeedbacksOnDate(dateLesson);
            if (feeds is null || feeds.Rows.Count == 0)
            {
                  SendMessageAsync(  "На этот урок небыло отзывов ", teacher);
                return;
            }

            string returntext = "";
            string[] feedBacksVariants = BotBase.GetAllWariantsFeedbacks().AsEnumerable().Select(x=> x["text"].ToString()).ToList().ToArray();
            int[] feedCounters = new int[feedBacksVariants.Length];
            for (int i = 0; i < feedCounters.Length; i++) feedCounters[i] = 0;

            foreach (DataRow r in feeds.Rows)
            {
                bool selected = false;
                for (int i = 0; i < feedCounters.Length; i++)
                {
                    string textFeed = r["text"].ToString();
                    string variantFeed = feedBacksVariants[i];
                    if (textFeed == variantFeed)
                    {
                        feedCounters[i]++;
                        selected = true;
                        break;
                    }
                }
                if (!selected)
                {
                    returntext += "⭐" + r["text"].ToString()+ Environment.NewLine;
                    haveUnic = true;
                }
            }

            for (int i = 0; i < feedCounters.Length; i++)
            {
                if (feedCounters[i]!=0) returntext += feedBacksVariants[i] + ":" + feedCounters[i] + Environment.NewLine ;
            }

            string textInMes = "";
            if (haveUnic) textInMes = "📈Отзывы со звездочкой дети написали сами📉";
            else textInMes = "📈Итоги📉";
            string text = textInMes + Environment.NewLine+returntext;
              SendMessageAsync( text,teacher);
           
        }
        public async void AdminUsersRedact(UserI user)
        {
            // Текстом будут юзеры
            string text = string.Join(Environment.NewLine,users.Select(u=>u.ToString()));
            CleanMenus(user);
            SendMessageAsync(text, user);

        }
        /// <summary>
        ///  менюшка выбора учителя которому будут направлены анононимки
        /// </summary>
        /// <param name="teenId">i ученика, который отправляет анонимки</param>
        public async void CoiceTeacherAnonMessage(long teenId)
        {
            // Текстом будут юзеры
            string text = "Кому предпочтительно прочитать твои сообщения?";
            UserI user = users[teenId];
            CleanMenus(user);

            MenuRow row = new MenuRow();
            InlineKeyboardButton btn;
            MenuTable table = new MenuTable();

            DataTable t = BotBase.GetAllTeachers();
            long[] ids = t.AsEnumerable().Select(el => long.Parse(el["id"].ToString())).ToArray();
            for (int i = 0; i < ids.Length; i++)
            {
                row = new MenuRow();
                long idTeacher = ids[i];
                string nameTeacher = BotBase.GetNameUser(idTeacher);
                btn = new InlineKeyboardButton(nameTeacher);
                btn.CallbackData = "setDesired|" + idTeacher;
                row.Add(btn);
                if (i + 1 < ids.Length)
                {
                    idTeacher = ids[i+1];
                    nameTeacher = BotBase.GetNameUser(idTeacher);
                    btn = new InlineKeyboardButton(nameTeacher);
                    btn.CallbackData = "setDesired|" + idTeacher;
                    row.Add(btn);
                    i++;
                }
                table.Add(row);
            }
            row = new MenuRow();
            btn = new InlineKeyboardButton("Не важно");
            btn.CallbackData = "setDesired|0";
            row.Add(btn);
            table.Add(row);

            InlineKeyboardMarkup ikm = new InlineKeyboardMarkup(table);
            try
            {

                user.MenuMessage = await BotClient.SendTextMessageAsync(
                    chatId: user.Id,
                    text: text,
                    replyMarkup: ikm
                    );
            }
            catch (ApiRequestException ex)
            {
                SendAdminMessageAsync("CoiceTeacherAnonMessage-Ищи в классе Sender"+ex.Message);
            }
        }


        public void CleanMenus(UserI user, int idMessage)
        {
            if (user.MenuMessage is not null)
            {
                if (user.MenuMessage.MessageId == idMessage)
                {
                    user.MenuMessage = null;
                }
            }
            if (user.YesNoMessage is not null)
            {
                if (user.YesNoMessage.MessageId == idMessage)
                {
                    user.YesNoMessage = null;
                }
            }
            DeleteMessage(idMessage, user.Id);
        }
        public void CleanMenus(UserI user)
        {
            if (user.MenuMessage is not null)
            {
                CleanMenus(user,user.MenuMessage.MessageId);
            }
            if (user.YesNoMessage is not null)
            {
                CleanMenus(user, user.YesNoMessage.MessageId);
            }
        }

        ///// <summary> Старая функция добавления мема LastImageIsMem
        ///// 
        ///// </summary>
        ///// <param name="up"></param>
        ///// <param name="bW"></param>
        ///// <param name="text"></param>
        //public async void LastImageIsMem(Update up, BibleWorker bW, string text)
        //{
        //    InlineKeyboardButton yes = new InlineKeyboardButton("Да");
        //    yes.CallbackData = "lastImageIsMem";
        //    InlineKeyboardButton no = new InlineKeyboardButton("Нет");
        //    no.CallbackData = "lastImageIsNotMem";

        //    InlineKeyboardMarkup mark = new InlineKeyboardMarkup(
        //        new MenuTable
        //        {
        //            new MenuRow{
        //                yes,
        //                no
        //            }
        //        }
        //    );
        //    try
        //    {
        //          botClient.SendTextMessageAsync(
        //               chatId: up.Message.Chat.Id,
        //               text: text,
        //               replyMarkup: mark
        //           );
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e + Environment.NewLine + e.Message);
        //    }
        //}

    }
}
