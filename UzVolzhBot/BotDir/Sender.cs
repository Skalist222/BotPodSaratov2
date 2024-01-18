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
using User = TelegramBotClean.Userses.User;
using TelegramBotClean.MemDir;

namespace TelegramBotClean.Bot
{
    public class Sender
    {
        Random Random { get; }
        TelegramBotClient botClient { get; }
        CancellationToken Token { get; }
        Users Users { get; }
        BotDB BotBase;
        Mems Mems;

      
        public Sender(TelegramBotClient botClient, CancellationToken token)
        {
            Random = new Random();
            BotBase = new BotDB(Random);
            this.botClient = botClient;
            this.Token = token;
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

        // Что можно делать извне
        public async Task SendMessage(MessageI message, long idChat)
        {
            if (message.IsText) SendText(message.Text,idChat);
            if (message.HavePhoto) SendImage(message.Photo, idChat,message.Text);
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
            MessageI receivedMes = new MessageI(up,botClient,Token);// Полученное сообщение
            MessageI toSendMes = new MessageI("");// сообщение которое мы отправим в обратку

            Telegram.Bot.Types.User userTelegram = up.Message is not null ? up.Message.From : up.CallbackQuery.From;
            //Если пользователя нет в базе данных
            User user = BotBase.GetUser(receivedMes.SenderId);


            //Во первых проверяем есть ли команда
            if (receivedMes.HaveCommand)
            {
                // В данном случае & проверяет, есть ли в командах сообщения команда МЕМ
                // если стоит &  значит проверяется первая найденая команда
                // если стоит | то проверяется есть ли среди команд введенная
                string comString = receivedMes.Commands.ToString();
                
                bool select = false;// определяет, найдена ли нужная команда
                // Тут получение команд
                if (!select && receivedMes.Commands.Is("старт"))
                {
                    SendAdminMessage("Получена команда старт");
                    // При начале работы бота или при нажатии на кнопку старт
                    if (user == null)
                    {
                        if (BotBase.CreateUser(new User(userTelegram)))
                        {
                            Users.Add(BotBase.GetUser(userTelegram.Id));
                            SendAdminMessage("Создан новый пользователь " + Users[userTelegram.Id].ToString());
                            Console.WriteLine("Добавлен новый пользователь");
                            SendMenu(userTelegram.Id, "Привет, дорогой друг. Этот бот предназначен для учеников воскресной школы(подростков). Нажми /help чтобы разобраться, как работает бот.");
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
                        SendMenu(userTelegram.Id,"Рад тебя снова видеть!");
                    }
                    select = true;
                }
                if (!select && receivedMes.Commands.Is("мем"))
                {
                    SendAdminMessage("Получена команда мем");
                    Console.WriteLine("Команда мем");
                    toSendMes = Mems.GetMessageRandomMem(Random);
                    select = true;
                }
                if (!select && receivedMes.Commands.Is("золой стих"))
                {
                    SendAdminMessage("Получена команда Золотой стих");
                    //Золотой стих
                    select = true;
                }
                if (!select && receivedMes.Commands.Is("инфо"))
                {
                    SendAdminMessage("Получена команда Инфо");
                    toSendMes.SetText(Config.Info.Text);
                    select = true;
                }
                if (!select && receivedMes.Commands.Is("добыавить золотой стих"))
                {
                    SendAdminMessage("Получена команда Добавление золотого стиха");
                    //добавление золотого стиха
                    select = true;
                }
                if (!select && receivedMes.Commands.Is("добавить мем"))
                {
                    SendAdminMessage("Получена команда добавления мема");
                    if (receivedMes.HavePhoto)
                    {
                        if (Mems.Add(new Mem(receivedMes)))
                        {
                            string thenks = BotBase.GetRandomAnswer(Commands.Get("thenks"));
                            string mem = BotBase.GetRandomAnswer(Commands.Get("mem"));
                            toSendMes.SetText(thenks +" "+ mem);
                        }
                        else
                        {
                            toSendMes.SetText("Ой прости.. что-то пошло не так, не смог добавить мем...");
                        }
                    }
                    //информация
                    select = true;
                }
                if (!select)
                {
                    SendAdminMessage("Пришла неизвестная команда");
                    Console.WriteLine("Левая какая то команда, может быть случайные слова вообще");
                }
                else
                {
                    if(toSendMes.Text !="") SendMessage(toSendMes, receivedMes.SenderId);
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

