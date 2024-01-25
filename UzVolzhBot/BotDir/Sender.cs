using Telegram.Bot.Types;
using Telegram.Bot;
using System.Drawing;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InputFiles;
using TelegramBotClean.Messages;
using TelegramBotClean.Userses;
using TelegramBotClean.Data;
using User = TelegramBotClean.Userses.User;
using TelegramBotClean.MemDir;
using TelegramBotClean.MenuDir;
using System;
using Telegram.Bot.Exceptions;
using TelegramBotClean.Commandses;
using TelegramBotClean.CommandsDir;
using TelegramBotClean.TextDir;

namespace TelegramBotClean.Bot
{
    public class Sender
    {


        TelegramBotClient botClient { get; }
        CancellationToken token { get; }
        public Users Users { get; }
        public BotDB BotBase { get; }
        public Mems Mems { get; }
        public Random Random { get; }
       public TextWorker textWorker { get; }


        public Sender(TelegramBotClient botClient, CancellationToken token)
        {
            this.botClient = botClient;
            this.token = token;

            Random = new Random();
            BotBase = new BotDB(Random);
            Users = new Users(BotBase);          
            Mems = new Mems(BotBase,botClient,token);
        }

        private async Task SendText(string text, long idChat)
        {
            try
            {
                await botClient.SendTextMessageAsync(chatId: idChat, text: text);
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)
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

        // Что можно делать извне
        public async Task SendMessage(MessageI message, long idChat)
        {
            if (message.IsText) SendText(message.Text,idChat);
            if (message.HavePhoto)
            {
                if (message.Photo is not null) SendImage(message.Photo, idChat, message.Text);
                else SendImage(message.ImageId, idChat,message.Text);
            }
            else
            {
                
            } 
        }
        public async Task SendMessage(string message, long idChat)
        {
           SendText(message, idChat);
        }
        public async Task SendAdminMessage(MessageI message)
        {
            SendMessage(message.Text, 1094316046L);
        }
        public async Task SendAdminMessage(Bitmap bitMap)
        {
            SendImage(bitMap, 1094316046L);
        }
        public async Task SendMenuMessage(MesMenuTable menu, User user, string text = "")
        {
            InlineKeyboardMarkup ikm = new InlineKeyboardMarkup(menu);
            try
            {
                await botClient.SendTextMessageAsync(
                    chatId: user.Id,
                    text: text,
                    replyMarkup: ikm
                );
            }
            catch (ApiRequestException ex)
            {
                Logger.Error(ex.Message);
                Console.WriteLine(ex.Message);
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
            if (u.TypeUser == UserTypes.Teen)
            {
                if (!u.TeenInfo.InAnonim)
                {
                    menu = new TeenMenu();
                }
                else
                {
                    menu = new TeenOnAnonMenu();
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
        


        public async void CreateAnswere(Update up)
        {
            DateTime start = DateTime.Now;
            MessageI receivedMes = new MessageI(up,botClient,token);// Полученное сообщение
        
            //Во первых проверяем есть ли команда
            if (receivedMes.HaveCommand)
            {
                Command selectedCommand = receivedMes.Commands.AsCommand();
                bool inAnon = Users[receivedMes.SenderId].TeenInfo!.InAnonim;
                if (!inAnon)
                {
                    selectedCommand.Execute(this, receivedMes);
                }
                else
                {
                    if (selectedCommand == Commands.OffAnonCommand)
                    {
                        selectedCommand.Execute(this, receivedMes);
                    }
                    else
                    {
                        CommandsExecutor.CreateAnonimMes(this, receivedMes);
                    }
                }
            }
            else
            {
                if (receivedMes.HavePhoto)
                {
                    Console.WriteLine("Пришло фото");
                }
                SendAdminMessage("Пришло сообщение без команд");
            }
            DateTime finish = DateTime.Now;
            SendAdminMessage(""+(finish-start).TotalSeconds+" sec.");
        }
    }
}

