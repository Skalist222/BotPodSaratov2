﻿using Telegram.Bot.Types;
using Telegram.Bot;
using System.Drawing;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InputFiles;
using TelegramBotClean.Messages;
using TelegramBotClean.Data;
using User = TelegramBotClean.Users.User;
using TelegramBotClean.MemDir;
using TelegramBotClean.MenuDir;
using Telegram.Bot.Exceptions;
using TelegramBotClean.Commandses;
using TelegramBotClean.CommandsDir;
using TelegramBotClean.TextDir;
using TelegramBotClean.MessagesDir;
using TelegramBotClean.Bible;
using TelegramBotClean.Users;
using TelegramBotClean.UsersesDir;


namespace TelegramBotClean.Bot
{
    public class Sender
    {
        bool SaveAllMessageInDataBase = false;
        TelegramBotClient botClient { get; }
        CancellationToken token { get; }
        public Users.Users Users { get; }

        public BotDB    BotBase { get; }
        public BibleDB  BibleDb { get; }
        public Mems     Mems { get; }
        public Random   Random { get; }
        public TextWorker TextWorker { get;}
        public UnansveredsMeesages UnansweredMessage { get; }
        public BibleWorker Bible { get; }
        public Bless Bless { get; }
        public AnonMessages AnonMessages { get; }
        public Spamer Spamer { get; }



        public Sender(TelegramBotClient botClient, CancellationToken token)
        {

            this.botClient = botClient;
            this.token = token;

            this.Random = new Random();
            Logger.Info("Получение базы Бота",false, ConsoleColor.Blue);
            this.BotBase = new BotDB(Random);
            Logger.Info("Получение базы Библии", false, ConsoleColor.Blue);
            this.BibleDb = new BibleDB(Random);

            Logger.Info("Фомирование библии в программе", false, ConsoleColor.Blue);
            this.Bible = new BibleWorker(BibleDb, BotBase, Random);

            Logger.Info("Фомирование списка пользователей в программе", false, ConsoleColor.Blue);
            this.Users = new Users.Users(BotBase, Bible);

            Logger.Info("Фомирование списка мемов в программе", false, ConsoleColor.Blue);
            this.Mems = new Mems(BotBase,botClient,token);

            Logger.Info("Текстворкер", false, ConsoleColor.Blue);
            this.TextWorker = new TextWorker(BotBase,Random);

            Logger.Info("Формирование Списка Неотвеченых анонимных сообщений", false, ConsoleColor.Blue);
            this.UnansweredMessage = new UnansveredsMeesages(BotBase);

            Logger.Info("Формирование благословений", false, ConsoleColor.Blue);
            this.Bless = new Bless(Bible,Random);

            Logger.Info("Анонимные сообщение", false, ConsoleColor.Blue);
            this.AnonMessages = new AnonMessages();

            Logger.Info("Спамер", false, ConsoleColor.Blue);
            this.Spamer = new Spamer(this);

        }

        private async Task SendText(string text, long idChat)
        {
            try
            {
                await botClient.SendTextMessageAsync(chatId: idChat, text: text);
            }
            catch (ApiRequestException ex)
            {
                Console.WriteLine("Ошибка в отправке сообщения" + ex.Message);
            }
        }
        private async Task SendImage(string idFile, long idChat, string caption = "")
        {
            try
            {
                botClient.SendPhotoAsync(
               chatId: idChat,
               photo: idFile,
               caption: caption
               );
            }
            catch (Exception exx)
            {
                Logger.Error("Ошибка в отправке фотки по id");
            }
           
        }
        private async Task SendVideo(string idFile, long idChat, string caption = "")
        {
            try
            {
                botClient.SendVideoAsync(
               chatId: idChat,
               video: idFile,
               caption: caption
               );
            }
            catch (Exception exx)
            {
                Logger.Error("Ошибка в отправке фотки по id");
            }

        }
        private async Task SendImage(Bitmap image, long idChat, string caption = "")
        {
            string path = "C:/Windows/Temp/image.jpg";
            using (MemoryStream ms = new MemoryStream())
            {
                Bitmap newBM = new Bitmap(image);
                image.Dispose();
                newBM.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    await botClient.SendPhotoAsync(
                        chatId:idChat,
                        photo: new InputOnlineFile(fileStream),
                        caption: caption
                    );
                }
            }
            System.IO.File.Delete(path);
        }
       
        private async Task DeleteMessage(int idMessage, long userDeleted)
        {
            await botClient.DeleteMessageAsync(
                chatId: userDeleted,
                messageId: idMessage
                );
        }
        public async Task DeleteMesMenu(MessageI menuMes,User user)
        {
            DeleteMessage((int)menuMes.Id, user.Id);
        }
        // Что можно делать извне
        public async Task SendMessage(MessageI message, long idChat)
        {
            if (message.Type == MessageTypes.Text) SendText(message.Text,idChat);
            if (message.Type == MessageTypes.Photo || message.Type == MessageTypes.PhotoText)
            {
               SendImage(message.FileId, idChat,message.Text);
            }
        }
        public async Task SendMessage(string message, long idChat)
        {
           SendText(message, idChat);
        }
        public async Task SendAdminMessage(MessageI message)
        {
            if (message.Type.HavePhoto)
            {
                SendImage(message.FileId, 1094316046L, message.Text);
            }
            else
            {
                SendMessage(message.Text, 1094316046L);
            }
            
        }
        public async Task<Message> SendMenuMessage(MesMenuTable menu, User user, string text = "")
        {
            InlineKeyboardMarkup ikm = new InlineKeyboardMarkup(menu);
            try
            {
                return await botClient.SendTextMessageAsync(
                    chatId: user.Id,
                    text: text,
                    replyMarkup: ikm
                );
            }
            catch (ApiRequestException ex)
            {
                Logger.Error(ex.Message);
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task SendAdminMessage(string message)
        {
            SendText(message, 1094316046L);
        }
        public async Task SendMenu(long idChat,string textInfo= "Мир тебе, дорогой мой друг")
        {
            if (textInfo == "") textInfo = "Мир тебе, дорогой мой друг"; 
            User u = Users[idChat];
            BaseMenu menu = new BaseMenu();
            if (u.Type == Privileges.Teen)
            {
                if (!u.InAnon)
                {
                    menu = new TeenMenu();
                }
                else
                {
                    menu = new TeenOnAnonMenu();
                }
            }
            if (u.Type == Privileges.Teacher)
            {
                if (!u.InAnswerAnon)
                {
                    menu = new TeacherMenu();
                }
                else
                {
                    menu = new TeacherMenuAnswer();
                }
               
            }

            ReplyKeyboardMarkup mrkp = new ReplyKeyboardMarkup(keyboard: menu.ButtonTable);
            try
            {
                await botClient.SendTextMessageAsync(
                               chatId: idChat,
                               text: textInfo,
                               replyMarkup: mrkp
                );
            }
            catch (Exception e)
            {
                Console.WriteLine($"Не удалось отправить сообщение пользователю "+e.Message);
            }
        }
        public async Task SendMessageMenu(long idChat, MesMenuTable menu, string textInfo)
        {
            if (textInfo.Replace("\r\n", string.Empty).Trim() == "") textInfo = "ВОПРОС!";
            InlineKeyboardMarkup mrkp = new InlineKeyboardMarkup(menu);
            try
            {
                await botClient.SendTextMessageAsync(
                               chatId: idChat,
                               text: textInfo,
                               replyMarkup: mrkp
                );
            }
            catch (Exception e)
            {
                Logger.Error($"Не удалось отправить сообщение пользователю " + e.Message);
            }
        }

        public async void CreateAnswere(Update up)
        {
            DateTime startTimeWork = DateTime.Now;

            Telegram.Bot.Types.User telegramUser = null;
            if (up.Message is not null) telegramUser = up.Message.From;
            else telegramUser = up.CallbackQuery.From;


            User user = Users.GetOrCreate(telegramUser, BotBase);//инициализируем пользователя
            if (user is null)
            {
                Logger.Error("Не удалось инициировать пользователя");
                return;
            } // Если даже после этого этапа, юзер не определен значит есть какаято ошибка
            MessageI receivedMes = new MessageI(up, user);// инициализируем сообщение

            //Сохраняет все полученные сообщения, при вклёченном параметре(нагружает сильно!)
            //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            if (SaveAllMessageInDataBase) BotBase.CreateMessage(receivedMes, receivedMes.Commands);
            //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


            string sideMenu = user.SideMenu;
             Command selectedCommand = receivedMes.Commands.AsCommand();

            if (sideMenu == "no")// Если пользователь находится в стандартном для него меню
            {
                if (receivedMes.HaveCommand)
                {
                    selectedCommand.Execute(this,receivedMes);
                }
                else
                {
                    if (receivedMes.Type == MessageTypes.Photo)
                    {
                        Console.WriteLine("Пришло фото без допов");
                        SendAdminMessage("Пришло фото без допов");
                    }
                    SendAdminMessage("Пришло сообщение без команд");
                }
            }
            else
            {
                if (sideMenu == "anon")
                {
                    if (selectedCommand == Commands.OffAnonCommand)
                        CommandsExecutor.ExOffAnon(this, receivedMes);
                    else
                    if (selectedCommand == Commands.SayNoWay)
                    {
                        CommandsExecutor.ExSetNoWentTeacher(this,receivedMes);
                    }
                    else CommandsExecutor.ExCreateAnonimMes(this, receivedMes);
                }
                if (sideMenu == "answerAnon")
                {
                    if (selectedCommand == Commands.SelectCommands("/turnOff /answere").AsCommand())
                        CommandsExecutor.ExOffAnswereAnon(this, receivedMes);
                    else
                        CommandsExecutor.ExSendAnswerAnon(this, receivedMes);
                }
            }

            DateTime finish = DateTime.Now;
            SendAdminMessage(""+(finish-startTimeWork).TotalSeconds+" sec.");
        }
    }
}