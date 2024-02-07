using Microsoft.Identity.Client;
using System.Data;
using Telegram.Bot.Types;
using TelegramBotClean.Data;
using Logger = TelegramBotClean.Data.Logger;

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
        public bool Have(long idUser)
        {
            return this.Where(el => el.Id == idUser).Count() == 1;
        }
        public bool Have(User user)
        {
            return Have(user.Id);
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
        public User GetOrCreate(Telegram.Bot.Types.User telegramUser, BotDB botBase)
        {
            User newUser = new User(telegramUser);

            //Если список пользователей пустой
            if (this.Count == 0)
            {
                //Пытаемся добавить пользователя в базу данных
                return AttemptAddToBase(newUser, botBase);
            }
            //Если список пользователей содержит хоть одного пользователя
            else
            {
                //Если пользователь не найден
                if (!Have(telegramUser.Id))
                {
                    //Пытаемся добавить пользователя в базу данных
                    return AttemptAddToBase(newUser, botBase);
                }
                else
                {
                    // если пользователь нашелся
                    return this[newUser.Id];
                }
            }
        }
        private User AttemptAddToBase(User user,BotDB botBase)
        {
            if (botBase.CreateUser(user))
            {
                // если получилось добавить в базу данных, добавляем в список пользователей
                Add(user);
                // возвращаем имеено того пользователя, который находится в списке а не нового созданного извне
                return this[user.Id];
            }
            else
            {
                Logger.Error("При добавке пользователя произошли ошибки");
                return null;
            }
        }
    }
}
