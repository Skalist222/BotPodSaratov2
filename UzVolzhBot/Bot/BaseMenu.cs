using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotClean.Data;

namespace TelegramBotClean.Bot
{
    public class MenuButton : Telegram.Bot.Types.ReplyMarkups.KeyboardButton
    {
        string TextButton {get;}
        public MenuButton(string text) : base(Config.InvizibleChar + text + Config.InvizibleChar) { TextButton = text; }
        //Основные кнопки
        public static MenuButton InfoBut { get { return new MenuButton("Инфо"); } }
        public static MenuButton MemBut { get { return new MenuButton("Мем"); } }
        //Кнопки администратора
        public static MenuButton UsersBut { get { return new MenuButton("Пользователи"); } }
        public static MenuButton GoldVersesBut { get { return new MenuButton("Золотые стихи"); } }

    }
    public class MenuRow: List<MenuButton>
    {
        public MenuRow(MenuButton b1,MenuButton b2=null,MenuButton b3 = null,MenuButton b4 = null, MenuButton b5 = null, MenuButton b6 = null) : base()
        {
            this.Add(b1);
            if (b2 is not null) this.Add(b2);
            if (b3 is not null) this.Add(b3);
            if (b4 is not null) this.Add(b4);
            if (b5 is not null) this.Add(b5);
            if (b6 is not null) this.Add(b6);
        }
    }
    public class MenuTable:List<MenuRow>
    {
        public MenuTable(MenuRow r1, MenuRow r2 = null, MenuRow r3 = null, MenuRow r4 = null, MenuRow r5 = null, MenuRow r6 = null) : base()
        {
            this.Add(r1);
            if (r2 is not null) this.Add(r2);
            if (r3 is not null) this.Add(r3);
            if (r4 is not null) this.Add(r4);
            if (r5 is not null) this.Add(r5);
            if (r6 is not null) this.Add(r6);
        }
        public MenuTable(MenuButton b1, MenuButton b2 = null, MenuButton b3 = null, MenuButton b4 = null, MenuButton b5 = null, MenuButton b6 = null) : base()
        {
            Add(new MenuRow(b1, b2, b3, b4, b5, b6));
        }
        public void Add(MenuButton b1, MenuButton b2 = null, MenuButton b3 = null, MenuButton b4 = null, MenuButton b5 = null, MenuButton b6 = null)
        {
            base.Add(new MenuRow(b1, b2, b3, b4, b5, b6));
        }
    }
   
    public class BaseMenu
    {
        internal MenuTable table;
        public MenuTable ButtonTable { get { return table; } }
        public ReplyKeyboardMarkup Markup 
        { get
            {
                ReplyKeyboardMarkup MarkUp = new ReplyKeyboardMarkup(keyboard: table);
                MarkUp.ResizeKeyboard = true;
                return MarkUp;
            } 
        }
        public BaseMenu()
        {
            MenuButton b1 = MenuButton.InfoBut;
            MenuButton b2 = MenuButton.MemBut;
            table = new MenuTable(b1,b2);
        }
    }
    public class AdminMenu : BaseMenu
    {
        public AdminMenu():base() 
        {
            MenuButton b1 = new MenuButton("Пользователи");
            MenuButton b2 = new MenuButton("Золотые стихи");
            table.Add(b1,b2);
        }
    }
}
