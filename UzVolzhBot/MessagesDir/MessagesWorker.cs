using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramBotClean.Commandses;
using TelegramBotClean.Data;

namespace TelegramBotClean.Messages
{
    public class MessageI
    {
        User senderUser = null;
        long senderId = 0;//айди получателя, кому направлено данное сообщение
        long recipient = 0;//айди отправителя, от кого было направлено сообщение
        string text = "";//текст сообщения
        Bitmap photo = null;//картинка сообщения
        string imageId = "";//айди присланной картинки
        string command = "";//полученная команда
        string smile = "";// если получен стикер, будет определен смайлик этого стикера
        Commands commands = new Commands(true);


        public string Text { get { return text; } }
        public string ImageId { get { return imageId; } }
        public string Command { get { return command; } }
        public string Smile { get { return smile; } }
        public Bitmap Photo { get { return photo; } }
        /// <summary>
        /// Отправитель сообщения
        /// </summary>
        public long SenderId { get { return senderId; } }
        public User Sender { get { return senderUser; } }


        /// <summary>
        /// Получатель сообщения
        /// </summary>
        public long Recipient { get { return recipient; } }
        public Commands Commands { get { return commands; } }



        public void SetSender(long id) { senderId = id; }
        public void SetRecipient(long id) { recipient = id; }
        /// <summary>
        /// Записывает в сообщение новый текст
        /// </summary>
        /// <param name="newText"></param>
        public void SetText(string newText) { text = newText; }

        public bool IsText { get { return text != "" && imageId == "" && command == "" && smile == ""; } }
        public bool IsPhoto { get { return (photo is not null || imageId != "") && text == ""; } }
        //public bool IsPhotoWithText { get { return (photo is not null || imageId != "") && text != ""; } }
        public bool IsCommand { get { return command != "" && text == ""; } }
        public bool IsCommandInText { get { return command != "" && text != ""; } }
        public bool IsSticker { get { return smile != ""; } }
        public bool HaveCommand { get { return command != ""; } }
        public bool HaveText { get { return text != ""; } }
        public bool HavePhoto { get { return photo is not null || imageId != ""; } }



        public MessageI(string text, string imageId = "", string command = "", string smile = "", Bitmap image = null)
        {
            this.text = text;
            this.imageId = imageId;
            this.command = command;
            this.smile = smile;
            this.photo = image;
        }
        public MessageI(Update up, TelegramBotClient botClient, CancellationToken token)
        {
            //начинаю формирование полученого сообщения
            if (up.Message != null)
            {
                //получено обычное сообщение или нажата кнопка ОСНОВНОГО меню
                Message mes = up.Message;
                //Так делать нельзя, но мне так удобнее
                SetParamFromTelegramMessage(mes, botClient, token);
            }
            else
            {
                if (up.CallbackQuery != null)
                {// Нажата кнопка в сообщении
                    senderId = up.CallbackQuery.From.Id;
                    command = up.CallbackQuery.Data;
                }
                else
                {
                    //Вот тут уже поинтереснее... я не знаю какой может быть другой вариант
                }
            }
        }
        public MessageI(Message mes, TelegramBotClient botClient, CancellationToken token)
        {
            if (mes is null) return;
            SetParamFromTelegramMessage(mes, botClient, token);
        }
        public void SetParamFromTelegramMessage (Message mes, TelegramBotClient botClient, CancellationToken token)
        {
            senderId = mes.From.Id;
            MessageType mesType = mes.Type;

            if (mesType == MessageType.Photo)
            {
                text = mes.Caption ?? "";// Если Описание к фото было, то заполняем текст описанием
                string cleanText = text.Replace(Config.InvizibleChar, "");
                string.Join(text, text);
                SaveImage(botClient, mes, token, ref photo);
                SelectCommands(ref commands, text);
            }
            if (mesType == MessageType.Text)
            {
                text = mes.Text;
                SelectCommands(ref commands, text);

            }
            if (mesType == MessageType.Sticker)
            {
                smile = mes.Sticker!.Emoji ?? "!";
            }
        }
        
        /// <summary>
        /// Определяет есть ли в списке полученных команд введенная
        /// </summary>
        /// <param name="m">Сообщение на проверку</param>
        /// <param name="c">Введенная команда</param>
        /// <returns></returns>
        public static bool operator |(MessageI m, Command c)
        {
            return m.Commands.Have(c);
        }
        /// <summary>
        /// Определяет, является ли первой командой из введенных та, которую получил оператор
        /// </summary>
        /// <param name="m">Сообщение на проверку</param>
        /// <param name="c">Введенная команда</param>
        /// <returns></returns>
        public static bool operator &(MessageI m, Command c)
        {
            return m.Commands.FirstEqual(c);
        }
        private void SaveImage(TelegramBotClient botClient,Message mes,CancellationToken token,ref Bitmap photo)
        {
            Telegram.Bot.Types.File fileInfo = null;
            if (mes.Type == MessageType.Photo)
            {
                fileInfo = botClient.GetFileAsync(mes.Photo[mes.Photo.Length - 1].FileId).Result;
            }
            
            string filePath = fileInfo.FilePath;
            string newNameFile = Directory.GetCurrentDirectory() + "\\" + fileInfo.FileUniqueId[0..15] + " " + Path.GetFileName(filePath);
            Bitmap bm;
            using (FileStream fileStream = new FileStream(newNameFile, FileMode.Create))
            {
                botClient.DownloadFileAsync(
                    filePath: filePath,
                    destination: fileStream,
                    cancellationToken: token).GetAwaiter().GetResult();
                bm = (Bitmap)Bitmap.FromStream(fileStream);
                photo = new Bitmap(bm);
                bm.Dispose();
                bm = null;
            }
        }
        private void SelectCommands(ref Commands commands,string text)
        {
            string cleanText = text.Replace(Config.InvizibleChar, "");
            commands = Commands.SelectCommands(cleanText);//Получаем команды
                                                          // проверяю не команда ли это

            if (text![0].ToString() == Config.InvizibleChar)
            {
                // Значит это команда!
                if (commands.ToString() == "clean")
                {
                    //Команда не определена
                }
                else
                {
                    command = commands.ToString();
                }
            }

            if (commands.ToString() == "clean")
            {
                //получен просто текст
            }
            else
            {
                // Команда из сообщения
                command = commands.ToString();
            }
        }
    }
   
}
