using Microsoft.Identity.Client;
using System.Data;
using System.Net.Security;
using TelegramBotClean.Commandses;
using TelegramBotClean.Messages;
using TelegramBotClean.TextDir;
using Logger = TelegramBotClean.Data.Logger;

namespace TelegramBotClean.Userses
{
    public class TeenInfo
    {
        protected bool inAnonim;
        protected string anonName;

        public bool InAnonim { get { return inAnonim; } }
        public string AnonName { get { return anonName; } }

        public void SetInAnon(TextWorker tw) {
            if (!inAnonim)
            {
                inAnonim = true;
                anonName = tw.RandomAnonName();
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

      
        public TeenInfo TeenInfo { get { if (this is Teen) return teenInfo; else return null; } }
        public TeacherInfo TeacherInfo { get { if (this is Teacher) return teacherInfo; else return null; } }

        public string Name { get 
            {
                return new List<string>() {UniqName, FirstName, Lastname, NickName,"NoName"}.Where(s => s != "").First();
            } 
        }
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
            return $"({type.Name}:{Id}) {Name}";
        }
        public string SideMenu()
        {

            if (type == UserTypes.Teen && teenInfo.InAnonim) return "anon";
            if (type == UserTypes.Teacher && teacherInfo.InAnswerAnon) return "answerAnon";
            return "no";
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
