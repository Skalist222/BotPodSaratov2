using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotClean.Bot;
using TelegramBotClean.Commandses;
using TelegramBotClean.Data;
using TelegramBotClean.Messages;
using TelegramBotClean.Userses;
using User = TelegramBotClean.Userses.User;

namespace TelegramBotClean.CommandsDir
{
    public class CommandsExecutor
    {
        Sender sender;
        MessageI mes;
        Users users;
        BotDB botBase;
        Update up;
        public CommandsExecutor(Sender sender, MessageI mes,Users users,BotDB botBase,Update up)
        { 
            this.sender = sender;
            this.mes = mes;
            this.users = users;
            this.botBase = botBase;
            this.up = up;
        }
    }
    
}
