
using Telegram.Bot.Types.ReplyMarkups;
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
    };
    public class MesMenuRow : List<But> 
    {
        public MesMenuRow(But b1=null, But b2=null, But b3=null, But b4=null, But b5=null) : base()
        {
            if (b1 is not null) this.Add(b1);
            if (b2 is not null) this.Add(b2);
            if (b3 is not null) this.Add(b3);
            if (b4 is not null) this.Add(b4);
            if (b5 is not null) this.Add(b5);
        }
    }

    public class YesNoMenu: MesMenuTable
    {
        public YesNoMenu(But yes, But no)
            :base(new MesMenuRow(yes,no))
        { }
        public YesNoMenu(string yes, string no) 
            : base(new MesMenuRow(new But("Да"),new But("Нет")))
        {
            this[0][0].CallbackData = yes;
            this[0][1].CallbackData = no;
        }
    }
}