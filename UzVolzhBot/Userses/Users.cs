using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotClean.Userses
{
    internal class Users:List<User>
    {
        public Users(DataTable dbTable)
        {
            for (int i = 0; i < dbTable.Rows.Count; i++)
            {
                User u = null;
                DataRow r = dbTable.Rows[i];
                string priv = r["privileges"].ToString().Trim();
                if (priv == "teen")
                {
                    u = new Teen(r);
                }
                else
                if (priv == "teacher")
                {
                    u = new Teacher(r);
                }
                else
                if (priv == "admin")
                {
                    u = new Admin(r);
                }
                else
                {
                    u = new DefaultUser(r);
                }
                this.Add(u);
            }
        }
        public User? this[long id]
        {
            get 
            {
                if (this.Count == 0) return null;
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].BaseInfo.Id == id) return this[i];
                }
                return null;
            }
        }
    }
}
