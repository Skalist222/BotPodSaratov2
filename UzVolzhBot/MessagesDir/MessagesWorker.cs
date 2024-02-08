using TelegramBotClean.Commandses;

using TelegramBotClean.Userses;
using User = TelegramBotClean.Userses.User;
using Update = Telegram.Bot.Types.Update;
using MessageTypeT = Telegram.Bot.Types.Enums.MessageType;
using TelegramBotClean.TextDir;

namespace TelegramBotClean.Messages
{

    public class MessageI
    {
        // внутренние параметры
        string text;
        Commands commands;

        //геттеры Неизменимых параметров
        public long Id { get; }
        public User Sender { get; }
      
        public string FileId { get; }
        public long ChatId { get; }
        public MessageType Type { get; }
       
        


        //геттеры Изменяемых параметров
        public string Text { get { return text; } }
        public Commands Commands { get { return commands; } }
        //побочные геттеры
        public long SenderId { get { return Sender.Id; } }
        // Если команда нулева, и не содержитни одной команды, то она считается пустой
        public bool HaveCommand { get { return Commands is not null && !Commands.IsEmpty; } }


        // сеттеры
        public void SetText(string value) { text = value; }
        private void SetCommands(Commands commands)
        {
            this.commands = commands;
        }

        public MessageI(string text)
        {
            SetText(text);
            Type = MessageTypes.Text;
        }
        public MessageI(long id, long chatId, string text, User sender, string fileId,MessageType type)
        {
            this.text = text;
            Id = id;
            Sender = sender;
            FileId = fileId;
            ChatId = chatId;
            Type = type;
            SetCommands(new Commands());
        }
        public MessageI(Update up,User user)
        {           
            if (up.Message is not null)
            {
                Id = up.Message.MessageId;
                Sender = user;
                ChatId = up.Message.Chat.Id;
                SetText(up.Message.Text ?? up.Message.Caption ?? "");
                SelectCommans();
                string typeString = up.Message.Type.ToString();
                Type = MessageTypes.Identify(typeString,Text,Commands);
                if (up.Message.Photo is not null) FileId = up.Message.Photo.First().FileId;
            }
            if (up.Message is null)
            {
                if (up.CallbackQuery.Data is not null)
                {
                    SetText(up.CallbackQuery.Data.ToString().Split('|')[0]);
                    SelectCommans();
                }
            }
        }
        private void SelectCommans()
        {
            SetCommands(new Commands(false, true));

            if (Text != "")
            {
                string cleanText = Text.Replace(Invizible.One, "");
                if (cleanText == "")
                {
                   
                    return;
                }
                else
                {
                    SetCommands(Commands.SelectCommands(cleanText));
                }
                if (text[0] + "" == Invizible.One) Commands.FromButtonTrue();
                else Commands.FromButtonFalse();
            }
        }
    }

    //public class MessageI3
    //{
    //    /**/
    //    long id = 0;
    //    User senderUser = null;
    //    long senderId = 0;//айди получателя, кому направлено данное сообщение
    //    string text = "";//текст сообщения

    //    string fileId = "";//айди присланной картинки
    //    string command = "";//полученная команда
    //    string smile = "";// если получен стикер, будет определен смайлик этого стикера
    //    Commands commands;
    //    long chatId = 0;
    //    public MessageType Type { get; }


    //    public long Id { get { return id; } }
    //    public string Text { get { return text; } }
    //    public string FileId { get { return fileId; } }


    //    /// <summary>
    //    /// Отправитель сообщения
    //    /// </summary>
    //    /**/
    //    public long SenderId { get { return senderId; } }
    //    /**/
    //    public User Sender { get { return senderUser; } }
    //    /**/
    //    public long ChatId { get { return chatId; } }



    //    /**/
    //    public Commands Commands { get { return commands; } }



    //    /// <summary>
    //    /// Записывает в сообщение новый текст
    //    /// </summary>
    //    /// <param name="newText"></param>
    //    public void SetText(string newText) { text = newText; }


    //    public bool HaveCommand { get { return !Commands.IsEmpty; } }



    //    public MessageI3(string text, string fileId = "", long messageId = 0, long chatId = 0)
    //    {
    //        Type = MessageTypes.TextPhoto;
    //        this.text = text;
    //        this.fileId = fileId;
    //        this.id = messageId;
    //        this.chatId = chatId;
    //    }
 

       
        
       
    //    private void SelectCommands(ref Commands commands, string text)
    //    {
    //        string cleanText = text.Replace(Invizible.One, "");
    //        if (cleanText == "")
    //        {
    //            command = "commandNotFound";
    //            commands = new Commands(false, true);
    //            return;
    //        };
    //        commands = Commands.SelectCommands(cleanText);//Получаем команды
    //        // проверяю не команда ли это
    //        if (text![0].ToString() == Invizible.One)
    //        {

    //            if (commands.IsEmpty)
    //            {
    //                //Команда не определена
    //                command = "commandNotFound";
    //            }
    //            else
    //            {
    //                command = commands.ToString();
    //            }
    //            commands.FromButtonTrue();
    //        }
    //        else
    //        {
    //            command = commands.ToString();
    //            commands.FromButtonFalse();
    //        }
    //    }



    //}

    public class Messages : List<MessageI>
    {
        public Messages() :base(){ }
        //public Messages(DataTable fromDB) : base()
        //{
        //    for (int i = 0; i < fromDB.Rows.Count; i++)
        //    {
        //        DataRow r = fromDB.Rows[i];
        //        long messageId = Convert.ToInt64(r["id"].ToString());
        //        string fileId = r["fileId"].ToString();
        //        string text = r["text"].ToString();
        //        MessageType type = MessageTypes.Identify(r["type"].ToString());
        //        Add(new MessageI(text,fileId,messageId)) ;
        //    }
        //}
    }
    public class MessageType
    {
        public string Name { get; }
        public string Description { get; }
        public MessageType(string code,string desc) 
        {
            Description = desc;
            Name = code;
        }
        public static bool operator ==(MessageType t1, MessageType t2)
        {
            return t1.Name == t2.Name;
        }
        public static bool operator !=(MessageType t1, MessageType t2)
        {
            return !(t1 == t2);
        }
        public bool HavePhoto { get { return this == MessageTypes.Photo || this == MessageTypes.PhotoText || this == MessageTypes.PhotoCommand; } }
    }
    public class MessageTypes
    {
        private static Dictionary<string, MessageType> allTypes = new Dictionary<string, MessageType>
        {
            { "Text", new MessageType( "Text", "Только текст") },
            { "Photo", new MessageType("Photo", "Только фото") },
            { "PhotoText", new MessageType("PhotoText", "Текст с фото") },
            { "PhotoCommand", new MessageType("PhotoCommand", "Команда с фото") },
            { "Video", new MessageType("Video", "Только видео") },
            { "VideoText", new MessageType("VideoText", "Только видео") },
            { "VideoCommand", new MessageType("VideoCommand", "Только видео") },
            { "Sticker", new MessageType("Sticker", "Стикер") }
        };
        public static MessageType Text { get { return allTypes["Text"]; } }
        public static MessageType Photo { get { return allTypes["Photo"]; } }
        public static MessageType PhotoText { get { return allTypes["PhotoText"]; } }
        public static MessageType PhotoCommand { get { return allTypes["PhotoCommand"]; } }
        public static MessageType Video { get { return allTypes["Video"]; } }
        public static MessageType VideoText { get { return allTypes["VideoText"]; } }
        public static MessageType VideoCommand { get { return allTypes["VideoCommand"]; } }
        public static MessageType Sticker { get { return allTypes["Sticker"]; } }
        

        public static MessageType Emprty { get { return new MessageType("Empty", "empty"); } }
        /// <summary>
        /// Определяет тип по названию типа
        /// </summary>
        /// <param name="telegramType"></param>
        /// <returns></returns>
        public static MessageType Identify(string telegramType, string text,Commands commands)
        {
            if (text != "") text = commands.IsEmpty ? "Command" : "Text";
            if (allTypes.ContainsKey(telegramType + text))
            {
                return allTypes[telegramType + text];
            }
            else
            {
                return Emprty;
            }
        }

    }
}
