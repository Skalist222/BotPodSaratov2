using PodrostkiBot.Configure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodrostkiBot.Text
{
    public class MessageI
    {
        Telegram.Bot.Types.MessageId id;
        MTypes type;
        string fileId;
        string text;
        string description;
        Commands2 commands;

        public string FileId { get { return fileId; } }
        public string Text { get { return text; } }
        public Commands2 Commands { get { return commands; } }
        public MTypes Type { get { return type; } }


        public MessageI(Telegram.Bot.Types.MessageId id,string fileId, string text, Commands2 commands, MTypes type)
        {
            this.id = id;
            this.fileId = fileId;
            this.text = text;
            this.commands = commands;
            this.type = type;
        }
        public MessageI(Telegram.Bot.Types.Update update,Engine engine)
        {
            Telegram.Bot.Types.Message mes = update.Message;
            Telegram.Bot.Types.CallbackQuery que = update.CallbackQuery;
            text = "";
            if (mes is not null) text = mes.Caption is null ? mes.Text : mes.Caption;
            else if (que is not null) text = que.Data;
            if (text is null) text = "";//КОСТЫЛЬ КОТОРЫЙ ТУТ НУЖЕН!!!

            commands = new Commands2(engine.BotBase, TextHandler.DeleteChars(text), engine.AllCommands, engine.AllResponceWords);


            if (mes is not null) 
                switch (mes.Type)
                {
                    case Telegram.Bot.Types.Enums.MessageType.Document: { type = MTypes.Doc; fileId = mes.Document.FileId; } break;
                    case Telegram.Bot.Types.Enums.MessageType.Text: { type = MTypes.Text; } break;
                    case Telegram.Bot.Types.Enums.MessageType.Photo: { type = MTypes.Photo; fileId = mes.Photo.FirstOrDefault().FileId; } break;
                    case Telegram.Bot.Types.Enums.MessageType.Video: { type = MTypes.Video; fileId = mes.Video.FileId; } break;
                    case Telegram.Bot.Types.Enums.MessageType.Audio: { type = MTypes.Audio; fileId = mes.Audio.FileId; } break;
                    
                    case Telegram.Bot.Types.Enums.MessageType.Voice: { type = MTypes.VoiceMes; fileId = mes.Voice.FileId; } break;
                    case Telegram.Bot.Types.Enums.MessageType.VideoNote: { type = MTypes.VideoMes; fileId = mes.VideoNote.FileId; } break;

                    case Telegram.Bot.Types.Enums.MessageType.Contact: { type = MTypes.Contact; } break;
                    case Telegram.Bot.Types.Enums.MessageType.Venue: { type = MTypes.Mesto; text = mes.Venue.Address; } break;
                    case Telegram.Bot.Types.Enums.MessageType.Sticker: { type = MTypes.Sticker; fileId = mes.Sticker.FileId; } break;
                    case Telegram.Bot.Types.Enums.MessageType.Poll: { type = MTypes.Sticker; fileId = mes.Poll.Id; } break;
                }
            if (que is not null) type = MTypes.Button;
        }
        
    }
    public class MTypes
    {
        public string Name { get; }
        private MTypes(string name)
        {
            Name = name;
        }
        public string ToString()
        {
            return Name;
        }
       

        public static MTypes Text       = new MTypes("text");
        public static MTypes Photo      = new MTypes("photo");
        public static MTypes Video      = new MTypes("video");
        public static MTypes Audio      = new MTypes("audio");
        public static MTypes Animation  = new MTypes("animation");
        public static MTypes VoiceMes   = new MTypes("voiceMes");
        public static MTypes VideoMes   = new MTypes("videoMes");
        public static MTypes Contact    = new MTypes("contact");
        public static MTypes Mesto      = new MTypes("mesto");
        public static MTypes Sticker = new MTypes("stik");
        public static MTypes Button = new MTypes("button");
        public static MTypes Doc = new MTypes("document");
    }


}
