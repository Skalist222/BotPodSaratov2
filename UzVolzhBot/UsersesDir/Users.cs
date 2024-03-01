using System.Data;
using TelegramBotClean.Data;
using TelegramBotClean.Bible;
using TelegramBotClean.UsersesDir;

namespace TelegramBotClean.Users
{
    public class Users : List<User>
    {
        BibleWorker bible;
        private Users(BibleWorker bW):base()
        {
            this.bible = bW;
        }
        public Users(DataTable t,BibleWorker bW):this(bW)
        {
            if (ValidationDataTableOnList(t))
            {
                for (int i = 0; i < t.Rows.Count; i++)
                {
                    this.Add(new User(t.Rows[i],bW));
                }
            }
        }
        public Users(BotDB botBase,BibleWorker bW):this(botBase.GetAllUsers(),bW){}
        public Users Teachers
        {
            get 
            {
                Users u = (Users)this.Where(e => e.Type == Privileges.Teacher).AsEnumerable().ToList();
                u.bible = this.bible;
                return u;
            }
        }

        public Users Teens
        {
            get
            {
                Users u = (Users)this.Where(e => e.Type == Privileges.Teen).AsEnumerable().ToList();
                u.bible = this.bible;
                return u;
            }
        }

        public User ByIndex(int index)
        {
            return base[index];
        }

        public string AsTextTable() 
        {
            string tabelString="";
            for (int i = 0; i < this.Count; i++)
            {
                tabelString += this[i].Id+" " + this[i].Name + Environment.NewLine;
            }
            return tabelString;
        }

        public User this[long id]
        {
            get {
                try { return this.Where(e => e.Id == id).FirstOrDefault(User.Empty); }
                catch 
                {
                    Console.WriteLine("asdadasd");
                    return User.Empty;
                }
            }
        }
        public User GetOrCreate(Telegram.Bot.Types.User telegramUser,BotDB botBase)
        {
            User u = this[telegramUser.Id];
            if (u == User.Empty)
            {
                u = new User(telegramUser, bible);
                if (botBase.CreateUser(u))
                {
                    this.Add(u);
                    return this[u.Id];
                }
                else
                {
                    Console.WriteLine("Ошибка добавления пользователя в базу данных");
                    return User.Empty;
                }
            }
            else
            {
                return u;
            }
           
        }

        public bool ValidationDataTableOnList(DataTable t)
        {
            return t.Columns.Count == 10;            
        }

        internal bool Have(long id,Privileges type = null)
        {
            if(type is null) return this.Where(e => e.Id == id).Count() == 1;
            else return this.Where(e => e.Id == id && e.Type == type).Count() == 1;
        }
    }
}
