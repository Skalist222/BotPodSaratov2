﻿using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotClean.Data;
using TelegramBotClean.TextDir;

namespace TelegramBotClean.MenuDir
{
    public class MenuButton : KeyboardButton
    {
        string TextButton { get; }
        public MenuButton(string text) : base(Invizible.One + text + Invizible.One) { TextButton = text; }
        //Основные кнопки
        public static MenuButton InfoBut { get { return new MenuButton("Инфо"); } }
        public static MenuButton HelpBut { get { return new MenuButton("Помощь"); } }
        public static MenuButton MemBut { get { return new MenuButton("Мем"); } }
        //Кнопки администратора
        public static MenuButton UsersBut { get { return new MenuButton("Пользователи"); } }
        public static MenuButton GoldVersesBut { get { return new MenuButton("Золотые стихи"); } }

        public static MenuButton OnAnonBut { get { return new MenuButton("Включить анон"); } }
        public static MenuButton OffAnonBut { get { return new MenuButton("Отключить анон"); } }
        public static MenuButton OffAnsvereAnonBut { get { return new MenuButton("Отмена отправки ответа"); } }
        public static MenuButton AllAnonMessagesBut { get { return new MenuButton("Анонимные сообщения "+ InvizibleEquals.All); } }
        public static MenuButton RandomVerseBut { get { return new MenuButton("Случайный стих"); } }
        public static MenuButton RandomGoldVerseBut { get { return new MenuButton("Случайный золотой стих"); } }
    }
    public class MenuRow : List<MenuButton>
    {
        public MenuRow(MenuButton b1, MenuButton b2 = null, MenuButton b3 = null, MenuButton b4 = null, MenuButton b5 = null, MenuButton b6 = null) : base()
        {
            Add(b1);
            if (b2 is not null) Add(b2);
            if (b3 is not null) Add(b3);
            if (b4 is not null) Add(b4);
            if (b5 is not null) Add(b5);
            if (b6 is not null) Add(b6);
        }
    }
    public class MenuTable : List<MenuRow>
    {
        public MenuTable(MenuRow r1, MenuRow r2 = null, MenuRow r3 = null, MenuRow r4 = null, MenuRow r5 = null, MenuRow r6 = null) : base()
        {
            Add(r1);
            if (r2 is not null) Add(r2);
            if (r3 is not null) Add(r3);
            if (r4 is not null) Add(r4);
            if (r5 is not null) Add(r5);
            if (r6 is not null) Add(r6);
        }
        public MenuTable(MenuButton b1, MenuButton b2 = null, MenuButton b3 = null, MenuButton b4 = null, MenuButton b5 = null, MenuButton b6 = null) : base()
        {
            Add(new MenuRow(b1, b2, b3, b4, b5, b6));
        }
        public void Add(MenuButton b1, MenuButton b2 = null, MenuButton b3 = null, MenuButton b4 = null, MenuButton b5 = null, MenuButton b6 = null)
        {
            Add(new MenuRow(b1, b2, b3, b4, b5, b6));
        }
    }

    public class BaseMenu
    {
        internal MenuTable table;
        public MenuTable ButtonTable { get { return table; } }
        public ReplyKeyboardMarkup Markup
        {
            get
            {
                ReplyKeyboardMarkup MarkUp = new ReplyKeyboardMarkup(keyboard: table);
                MarkUp.ResizeKeyboard = true;
                return MarkUp;
            }
        }
        public BaseMenu()
        {
            MenuButton b1 = MenuButton.HelpBut;
            MenuButton b2 = MenuButton.MemBut;
            table = new MenuTable(b1,b2);
        }
    }

    //TEacher menus
    public class TeenMenu : BaseMenu
    {
        public TeenMenu() : base()
        {
            MenuButton b1 = MenuButton.OnAnonBut;
            table.Add(b1);
        }
    }
    public class TeenOnAnonMenu : BaseMenu
    {
        public TeenOnAnonMenu() : base()
        {
            MenuButton b1 = MenuButton.OffAnonBut;
            table = new MenuTable(b1);
        }
    }

    public class TeacherMenu : BaseMenu
    {
        public TeacherMenu() : base()
        {
            MenuButton b1 = MenuButton.AllAnonMessagesBut;
            MenuButton b2 = MenuButton.RandomVerseBut;
            MenuButton b3 = MenuButton.RandomGoldVerseBut;
            table.Add(b1,b2,b3);
        }
    }
    public class TeacherMenuAnswer : BaseMenu
    {
        public TeacherMenuAnswer() : base()
        {
            MenuButton b1 = MenuButton.OffAnsvereAnonBut;
            table = new MenuTable(b1);
        }
    }


    public class AdminMenu : BaseMenu
    {
        public AdminMenu() : base()
        {
            MenuButton b1 = new MenuButton("Пользователи");
            MenuButton b2 = new MenuButton("Золотые стихи");
            table.Add(b1, b2);
        }
    }
}
