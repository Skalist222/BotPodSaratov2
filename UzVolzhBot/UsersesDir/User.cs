using Microsoft.Identity.Client;
using System.Data;
using System.Net.Security;
using TelegramBotClean.Bible;
using TelegramBotClean.Commandses;
using TelegramBotClean.Data;
using TelegramBotClean.Messages;
using TelegramBotClean.TextDir;
using Logger = TelegramBotClean.Data.Logger;

namespace TelegramBotClean.Userses
{
    public class TeenInfo
    {
        protected bool inAnonim;
        protected string anonName;
        protected long idWentTeacher;
        public MessageI wentedTeacherMessage;

        public bool InAnonim { get { return inAnonim; } }
        public string AnonName { get { return anonName; } }
        public long IdWentTeacher { get { return idWentTeacher; } }
        public MessageI WentedTeacherMessage { get { return wentedTeacherMessage; } }

        public void SetInAnon(TextWorker tw,MessageI message) {
            if (!inAnonim)
            {
                inAnonim = true;
                anonName = tw.RandomAnonName();
                wentedTeacherMessage = message;
            }
            else
            {
                Logger.Error("Попытка включить анон когда он и так включен");
            }
        }
        public void SetNotInAnon(TextWorker tw) {
            if (inAnonim)
            {
                inAnonim = false;
                tw.AnonNameFinished(anonName);
            }
            else 
            {
                Logger.Error("Попытка отключить анон когда он и так отключен");
            }
        }
        public void SetWentedTeacher(User teacher)
        {
            idWentTeacher = teacher.Id;
        }
        public void SetWentedTeacher(long idTeacher)
        {
            idWentTeacher = idTeacher;
        }
        

        public TeenInfo()
        {
            inAnonim = false;
        }
    }
    public class TeacherInfo
    {
        #region параметры анонимности
        protected long idTeenAnonMessage;
        protected bool inAnswerOnAnon;
        public long IdTeenAnonMessage { get { return idTeenAnonMessage; } }
        public bool InAnswerAnon { get { return inAnswerOnAnon; } }
        public void TurnOnAnswerOnAnon(long idTeen)
        {
            idTeenAnonMessage = idTeen;
            inAnswerOnAnon = true;
        }
        public void TurnOffAnswerOnAnon()
        {
            idTeenAnonMessage = 0;
            inAnswerOnAnon = false;
        }
        #endregion

        public TeacherInfo()
        {
            TurnOffAnswerOnAnon();
        }
    }
    public class BaseUserInfo
    {
        protected long id;
        protected string nickName;
        protected string firstName;
        protected string lastName;
        protected string uniqName;
        protected bool banned;



        protected MessageI lastMessage;
        

        public long Id { get { return id; } }
        public string NickName { get { return nickName; } }
        public string FirstName { get { return firstName; } }
        public string Lastname { get { return lastName; } }
        public bool Ban { get { return banned; } }

  
        public string UniqName { get { return uniqName; } }
        public string LastMessageText { get { return lastMessage!.Text; } }
        public Commands LastMessageCommands { get { return lastMessage!.Commands; } }
        public void SetLastMessage(MessageI mes)
        {
            lastMessage = mes;
        }
      
        public void CleanLastMessage()
        {
            lastMessage = null;
        }


        public BaseUserInfo(long id, string nickName, string firstName, string lastName, bool banned, string uniqName, MessageI lastMessage)
        {
            this.id = id;
            this.nickName = nickName;
            this.firstName = firstName;
            this.lastName = lastName;
            this.banned = banned;
            this.lastMessage = lastMessage;
        }
        public BaseUserInfo(DataRow r)
        {
            id = Convert.ToInt64(r["Id"].ToString());
            nickName = r["nick"].ToString();
            firstName = r["firstName"].ToString();
            lastName = r["lastName"].ToString();
            banned = r["ban"].ToString() == "True";
            uniqName = r["uniqName"].ToString();
            lastMessage = null;
        }
    }
    public class User:BaseUserInfo
    {
        protected UserType type;
        protected TeenInfo teenInfo;
        protected TeacherInfo teacherInfo;


        public bool IsAdmin { get 
            {
                if (Config.AdminsId.AsEnumerable().Contains(id)) return true;
                else return type == UserTypes.Admin;
            }
        }
      
        public TeenInfo TeenInfo { get { if (this is Teen) return teenInfo; else return null; } }
        public TeacherInfo TeacherInfo { get { if (this is Teacher) return teacherInfo; else return null; } }

        public string Name { get 
            {
                return new List<string>() {UniqName, FirstName, Lastname, NickName,"NoName"}.Where(s => s != "").First();
            } 
        }
        public string AllNames { get 
            {
          
                return string.Join(",","nick:"+nickName, "uniq" + uniqName,"имя" +firstName,"фамилия"+lastName);
            } }
        public UserType TypeUser { get { return type; } }

        public User(DataRow r):base(r)
        {
            type = UserTypes.BaseUser;
        }
        public User(long id, string nickName, string firstName, string lastName, bool banned, string uniqName, MessageI lastMessage) 
            :base(id,  nickName,  firstName,  lastName,  banned,  uniqName,  lastMessage)
        {
            type = UserTypes.BaseUser;
        }
        public User(Telegram.Bot.Types.User user) : this(user.Id, user.Username, user.FirstName, user.LastName, false, "", null) 
        {
            type = UserTypes.BaseUser;
        }

        public string ToString()
        {
            return $"({type.Name}:{Id}) {Name} ";//[{AllNames}]
        }
        public string SideMenu()
        {
            return "";
        }
    }


    public class DefaultUser : User
    {
       
        public DefaultUser(DataRow r):base(r)
        {
           type = UserTypes.BaseUser;
        }
    }
    public class Teen : User
    {
        public Teen(DataRow r) : base(r)
        {
            type = UserTypes.Teen;
            teenInfo = new TeenInfo();
        }
        public void SendAnonimMessage(MessageI message)
        {

        }
    }
    public class Teacher : User
    {
        public Teacher(DataRow r) : base(r)
        {
            type = UserTypes.Teacher;
            teacherInfo = new TeacherInfo();
        }
    }
    public class Admin : User
    {
        public Admin(DataRow r) : base(r)
        {
            type = UserTypes.Admin;
        }
    }

}
namespace TelegramBotClean.Users2
{
    public class User
    {
        private long id;
        private string nickName;
        private string lastname;
        private string firstName;
        private string uniqName;
        private bool spam;
        private bool ban;
        private bool deleted;

        private Verse lastverse;
        private Privileges privileges;
        
        

        public long Id { get { return id; } }
        public string NickName { get { return nickName; } }
        public string Lastname {  get { return lastname; } }
        public string FirstName {  get { return firstName; } }
        public string UniqueName { get { return uniqName; } }
        public string TypeName { get { return privileges.Name; } }
        public bool Spam { get { return spam; } }
        public bool Ban { get { return ban; } }
        public bool Deleted { get { return deleted; } }

        public Verse Lastverse {  get { return lastverse; } }
        public Privileges Type { get { return privileges; } }

        public User(long id, string nickName, string lastname, string firstName, string uniqName, bool spam, bool ban, bool deleted, Verse lastverse, Privileges privileges)
        {
            this.id = id;
            this.nickName = nickName;
            this.lastname = lastname;
            this.firstName = firstName;
            this.uniqName = uniqName;
            this.spam = spam;
            this.ban = ban;
            this.deleted = deleted;
            this.lastverse = lastverse;
            this.privileges = privileges;
        }
        public User(DataRow r,BibleWorker bW)
        {
            this.id = Convert.ToInt64(r["id"].ToString());
            this.nickName = r["nick"].ToString() ;
            this.lastname = r["lastname"].ToString();
            this.firstName = r["firstName"].ToString();
            this.uniqName = r["uniqName"].ToString();
            this.spam = r["firstName"].ToString() == "True";
            this.ban = r["ban"].ToString() == "True";
            this.deleted = r["firstName"].ToString() == "True";
            this.lastverse = bW.GetVerseByAddress(r["lastStih"].ToString());
            this.privileges = Privileges.Select(r["privileges"].ToString());
        }
    }
    public class Privileges
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        private Privileges(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
        public static Dictionary<string, Privileges> allPrivileges = new Dictionary<string, Privileges>()
        {
            { "admin", new Privileges(0, "admin", "Права администратора")},
            { "user", new Privileges(1, "user", "стандартный пользователь")},
            { "teen", new Privileges(2, "teen", "ученик")},
            { "teacher", new Privileges(3, "teacher", "учитель")}
        };
        public Privileges Admin { get { return allPrivileges["admin"]; } }
        public Privileges DefaultUser { get { return allPrivileges["user"]; } }
        public Privileges Teen { get { return allPrivileges["teen"]; } }
        public Privileges Teacher { get { return allPrivileges["teacher"]; } }
        public static Privileges Select(string name)
        {
            if (allPrivileges.ContainsKey(name))
            {
                return allPrivileges[name];
            }
            else
            {
                return allPrivileges["user"];
            }
        }
    }
}