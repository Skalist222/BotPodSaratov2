using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotClean.Data;

namespace TelegramBotClean.MemDir
{
    public class Mems:List<Mem>
    {
        public Mems() : base()
        {
            
        }
        public Mems(DataTable memMessagesTable) : base()
        {
            for (int i = 0; i < memMessagesTable.Rows.Count; i++)
            {
                DataRow r = memMessagesTable.Rows[i];
                this.Add(new Mem(r["fileId"].ToString(), Convert.ToInt64(r["idMessage"].ToString())));
            }
        }
      
    }
}
