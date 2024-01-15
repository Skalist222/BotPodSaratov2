using Telegram.Bot.Types;
using Telegram.Bot;
using System.Drawing;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InputFiles;
using TelegramBotClean.Commandses;
using TelegramBotClean.Messages;
using TelegramBotClean.Userses;
using TelegramBotClean.Data;
using System.Data;

namespace TelegramBotClean.Bot
{
    public class Sender
    {
        TelegramBotClient botClient;
        CancellationToken token;
        Users users;
        public Sender(TelegramBotClient botClient, CancellationToken token)
        {
            this.botClient = botClient;
            this.token = token;
            DataTable t = new BotDB().GetAllUsers();
            users = new Users(t);
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
        private async Task SendImage(string pathImage, long idChat, string caption = "")
        {
            var bm = Bitmap.FromFile(pathImage);
            var ms = new MemoryStream();
            bm.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            ms.Position = 0;
            using (var fileStream = new FileStream(pathImage, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Telegram.Bot.Types.InputFiles.InputOnlineFile file = new Telegram.Bot.Types.InputFiles.InputOnlineFile(fileStream);
                await botClient.SendPhotoAsync(
                    chatId: idChat,
                    photo: file,
                    caption: caption
                );
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


        public async Task SendMessage(MessageI message, long idChat)
        {
            if (message.IsText) SendText(message.Text,idChat);
            if (message.IsPhoto) SendImage(message.Photo, idChat);
        }
        public async Task SendMessage(string message, long idChat)
        {
           SendText(message, idChat);
        }
        public async Task SendAdminMessage(MessageI message)
        {
            long idChat = 1094316046L;
            if (message.IsText) SendText(message.Text, idChat);
            if (message.IsPhoto) SendImage(message.Photo, idChat);
        }
        public async Task SendAdminMessage(string message)
        {
           SendText(message, 1094316046L);
        }
        public async Task SendMenu(long idChat,string textInfo= "Мир тебе, дорогой мой друг")
        {
            if (textInfo == "") textInfo = "Мир тебе, дорогой мой друг";// Если вдруг человек отправил пустую строку 
            BaseMenu menu = new BaseMenu();
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
            MessageI toSendMes = new MessageI("");// сообщение которое мы отправим в обратку
            //Во первых проверяем есть ли команда
            if (receivedMes.HaveCommand)
            {
                // В данном случае & проверяет, есть ли в командах сообщения команда МЕМ
                // если стоит &  значит проверяется первая найденая команда
                // если стоит | то проверяется есть ли среди команд введенная

                bool select = false;// определяет, найдена ли нужная команда

                // Тут получение команд
                if (!select && receivedMes.Commands.Is("мем"))
                {
                    //Типа нажата кнопка мем
                    select = true;
                }
                if (!select && receivedMes.Commands.Is("золой стих"))
                {
                    //Золотой стих
                    select = true;
                }
                if (!select && receivedMes.Commands.Is("инфо"))
                {
                    toSendMes.SetText(Config.Info.Text);
                    select = true;
                }
                if (!select && receivedMes.Commands.Is("добавить золотой стих"))
                {
                    //добавление золотого стиха
                    select = true;
                }
                if (!select && receivedMes.Commands.Is("добавить мем"))
                {
                    //информация
                    select = true;
                }
                if (!select)
                {
                    Console.WriteLine("Левая какая то команда, может быть случайные слова вообще");
                }
                SendMessage(toSendMes, receivedMes.Sender);
            }
            else
            {
                
            }
            DateTime finish = DateTime.Now;
            SendAdminMessage("Пришло сообщение ("+(finish-start).TotalSeconds+" sec.)");
        }
    }
}

