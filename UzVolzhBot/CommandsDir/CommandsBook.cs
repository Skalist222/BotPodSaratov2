using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotClean.Commandses
{
    public static class CommandsBook
    {
        public static Dictionary<string, Action> CommandsWorkers = new Dictionary<string, Action>
        {
            { Commands.AddCommand.Name,Commands.AddCommand.Worker},
        };

    }
}
