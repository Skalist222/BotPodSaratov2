using System.Data;
using TelegramBotClean.Data;

namespace TelegramBotClean.Userses
{
    public class Users:List<User>
    {
        public string ToString()
        {
            if (this.Count == 0) return "empty";
            else
            {
                return $"({this.Count}) Ученики:{CountTeen}, Учителя:{CountTeacher}, Прихожане:{this.Count-(CountTeacher+CountTeen)}";
            }
        }
        public int CountTeacher {
            get 
            {
                if (this.Count == 0) return 0;
                else
                {
                    return this.Where(x => x.TypeUser == UserTypes.Teacher).Count();
                }
            } 
        }
        public int CountTeen
        {
            get
            {
                if (this.Count == 0) return 0;
                else
                {
                    return this.Where(x => x.TypeUser == UserTypes.Teen).Count();
                }
            }
        }
        public Users() : base() { }
        public Users(DataTable dbTable):base()
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
        public Users(BotDB botBase):base()
        {
            
            DataTable t = botBase.GetAllUsers();
            for (int i = 0; i < t.Rows.Count; i++)
            {
                User u = null;
                DataRow r = t.Rows[i];
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
                try
                {
                    var grp = this.Where(u => u.Id == id);
                    if (grp.Count() == 0) return null;
                    else return grp.First();
                }
                catch (InvalidOperationException ex)
                { return null; }               
            }
        }
        public void Add(User user)
        {
            base.Add(user);
        }
    }
}
