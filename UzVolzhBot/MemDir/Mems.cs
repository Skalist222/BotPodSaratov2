using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotClean.Data;
using static TelegramBotClean.Data.Logger;

namespace TelegramBotClean.MemDir
{
    public class Mems:List<Mem>
    {
        BotDB botBase;
        public Mems(BotDB botBase) : base()
        {
            this.botBase = botBase;
            this.AddRange(botBase.GetAllMemMessages());
        }
        public Mems(DataTable memMessagesTable) : base()
        {
            for (int i = 0; i < memMessagesTable.Rows.Count; i++)
            {
                DataRow r = memMessagesTable.Rows[i];
                this.Add(new Mem(r["fileId"].ToString(), Convert.ToInt64(r["idMessage"].ToString())));
            }
        }
        public void Add(Mem mem, BotDB botBase)
        {
            if (botBase.CreateMemMessage(mem)) this.Add(mem);
            else
            {
                Error("Не удалось отправить мем в базу данных!!","AddMem(Mem mem, BotBD botBase)");
            }
        }
      
    }
}
