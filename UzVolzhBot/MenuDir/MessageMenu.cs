
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotClean.Bot;
using TelegramBotClean.Commandses;
using TelegramBotClean.Data;
using TelegramBotClean.MessagesDir;
using TelegramBotClean.TextDir;
using But = Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton;

namespace TelegramBotClean.MenuDir
{
    public class MesMenuTable : List<MesMenuRow> 
    {
        public MesMenuTable(
            MesMenuRow r1 = null,
            MesMenuRow r2 = null,
            MesMenuRow r3 = null,
            MesMenuRow r4 = null,
            MesMenuRow r5 = null,
            MesMenuRow r6 = null,
            MesMenuRow r7 = null,
            MesMenuRow r8 = null,
            MesMenuRow r9 = null,
            MesMenuRow r10 = null
            ) : base()
        {
            if (r1 is not null) this.Add(r1);
            if (r2 is not null) this.Add(r2);
            if (r3 is not null) this.Add(r3);
            if (r4 is not null) this.Add(r4);
            if (r5 is not null) this.Add(r5);
            if (r6 is not null) this.Add(r6);
            if (r7 is not null) this.Add(r7);
            if (r8 is not null) this.Add(r8);
            if (r9 is not null) this.Add(r9);
            if (r10 is not null) this.Add(r10);
        }
        public MesMenuTable(MesMenuBut[] buttons) : base()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                Add(new MesMenuRow(buttons[i]));
            }
        }

        public static MesMenuTable CreateMenu(string selectedCommands, string[] data)
        {
            Commands commands = Commands.SelectCommands(selectedCommands);//Текст команды
          
            MesMenuBut[] buts = new MesMenuBut[data.Length];
            for (int i = 0; i < buts.Length; i++)
            {
                buts[i] = new MesMenuBut("от:" + data[i], commands.SeparatedBySpace() + " |" + data[i]);
            }
            MesMenuTable table = new MesMenuTable(buts);
            return table;
        }

    };
    public class MesMenuRow : List<MesMenuBut> 
    {
        public MesMenuRow(MesMenuBut b1 =null, MesMenuBut b2 =null, MesMenuBut b3 =null, MesMenuBut b4 = null, MesMenuBut b5 = null) : base()
        {
            if (b1 is not null) this.Add(b1);
            if (b2 is not null) this.Add(b2);
            if (b3 is not null) this.Add(b3);
            if (b4 is not null) this.Add(b4);
            if (b5 is not null) this.Add(b5);
        }
    }
    public class MesMenuBut : But
    {
        public MesMenuBut(string text) : base(text)
        {
            
        }
        public MesMenuBut(string text, string data) : base(Invizible.One+ text+ Invizible.One)
        {
            if (data.Length > 64) Logger.Error($"Команда в кнопке слишком большая!!!{data}");
            this.CallbackData = data;
        }
    }


    public class YesNoMenu: MesMenuTable
    {
        public YesNoMenu(string yesData, string noData) 
            : base(new MesMenuRow(
                new MesMenuBut("Да", yesData),
                new MesMenuBut("Нет", noData)
                ))
        {
        }
    }
}