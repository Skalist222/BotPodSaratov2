using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Drawing;
using TelegramBotClean.Data;
using System.Security.Cryptography.X509Certificates;
using Telegram.Bot.Types.Enums;
using System.Threading;

namespace TelegramBotClean.Bot
{
    public class Sender
    {
        TelegramBotClient botClient;
        CancellationToken token;
        public Sender(TelegramBotClient botClient, CancellationToken token)
        {
            this.botClient = botClient;
            this.token = token;
        }

        internal async Task SendText(string text, long idChat)
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
        internal async Task SendImage(string pathImage, long idChat, string caption = "")
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
        public async Task DeleteMessage(int idMessage, long userDeleted)
        {
            await botClient.DeleteMessageAsync(
                chatId: userDeleted,
                messageId: idMessage
                );
        }
        internal async Task SendMessage(MessageI message, long idChat)
        {
            
        }

        public async void CreateAnswere(Update up)
        {
            MessageI message = new MessageI(up,botClient,token);
            if (message.IsCommand)
            {///Нажата любая кнопка 
                // Здесь мы должны понять какая кнопка нажата и ответить в соответствии
            }
            if (message.IsText)
            {///Получен простой текст

            }
            if (message.IsSticker)
            {///Получен стикер 

            }
        }
    }

    public class MessageI
    {
        string text = "";
        string imageId = "";
        string command = "";
        string smile = "";
        Bitmap image = null;



        public string Text { get { return text; } }
        public string ImageId { get { return imageId; } }
        public string Command { get { return command; } }
        public string Smile { get { return smile; } }
        
        public bool IsText { get { return text != "" && imageId == "" && command =="" && smile == ""; } }
        public bool IsPhoto { get { return imageId != "" && text == ""; } }
        public bool IsPhotoWithText { get { return imageId != "" && text != ""; } }
        public bool IsCommand { get { return command != ""; } }
        public bool IsSticker { get { return smile != ""; } }

        public MessageI(string text, string imageId="", string command="", string smile="",Bitmap image = null)
        {
            this.text = text;
            this.imageId = imageId;
            this.command = command;
            this.smile = smile;
            this.image = image;
        }

        public MessageI(Update up,TelegramBotClient botClient,CancellationToken token)
        {
            //начинаю формирование полученого сообщения
            if (up.Message != null)
            {
                // получено обычное сообщение или нажата кнопка ОСНОВНОГО меню
                Message mes = up.Message;
                MessageType mesType = mes.Type;

                if (mesType == MessageType.Photo)
                {
                    text = mes.Caption;
                    if (text is null) text = "";
                    Telegram.Bot.Types.File fileInfo = botClient.GetFileAsync(mes.Photo[mes.Photo.Length-1].FileId).Result;
                    string filePath = fileInfo.FilePath;
                    string newNameFile = Directory.GetCurrentDirectory() + "\\"+ fileInfo.FileUniqueId[0..15] + " "+Path.GetFileName(filePath);
                    
                    using (FileStream fileStream = new FileStream(newNameFile, FileMode.Create))
                    {
                        botClient.DownloadFileAsync(
                            filePath: filePath,
                            destination: fileStream,
                            cancellationToken: token).GetAwaiter().GetResult();
                        image = (Bitmap)Bitmap.FromStream(fileStream);
                    }
                }
                if (mesType == MessageType.Text)
                {
                    text = mes.Text;
                    // проверяю не команда ли это                
                    if (text![0].ToString() == Config.InvizibleChar)
                    {
                        // Значит это команда!
                        command = text[1..];//Получаем текст команды со второго символа(чтобы обрезать невидимый)
                    }
                }
                if (mesType == MessageType.Sticker)
                {
                    text = mes.Sticker!.FileId;
                    smile = mes.Sticker!.Emoji ?? "!";
                }
                
            }
            else
            {
                if (up.CallbackQuery != null)
                {// Нажата кнопка в сообщении
                    command = up.CallbackQuery.Data;
                }
                else
                {
                    //Вот тут уже поинтереснее... я не знаю какой может быть другой вариант
                }
            }
        }
        

    }
}

