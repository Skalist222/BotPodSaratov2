using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotClean.Data;

namespace TelegramBotClean.Bot
{
    public class MenuButton : Telegram.Bot.Types.ReplyMarkups.KeyboardButton
    {
        public MenuButton(string text) : base(text) { }
    }
    public class MenuRow: List<MenuButton>{}
    public class MenuTable:List<MenuRow>{}
   
    public class BaseMenu
    {
        MenuTable table ;
        public BaseMenu()
        {
            MenuButton b1 = new MenuButton(Config.InvizibleChar+"Инфо");
            table = new MenuTable();
        }
    }
}
