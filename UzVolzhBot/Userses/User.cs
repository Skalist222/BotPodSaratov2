using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotClean.Commandses;
using TelegramBotClean.Messages;

namespace TelegramBotClean.Userses
{
    public class TeenInfo
    {
        protected bool inAnonim;
        public bool InAnonim { get { return InAnonim; } }
        public void SetInAnon() { inAnonim = true; }
        public void SetNotInAnon() { inAnonim = false; }
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
    public class User
    {
        protected UserType type;
        protected TeenInfo teenInfo;
        protected TeacherInfo teacherInfo;

        public BaseUserInfo BaseInfo { get; }
        public TeenInfo TeenInfo { get { if (this is Teen) return teenInfo; else return null; } }
        public TeacherInfo TeacherInfo { get { if (this is Teacher) return teacherInfo; else return null; } }
        
        

        public string Name { get 
            {
                return new List<string>() {BaseInfo.UniqName, BaseInfo.FirstName, BaseInfo.Lastname,BaseInfo.NickName,"NoName"}.Where(s => s != "").First();
            } 
        }
        public UserType TypeUser { get { return type; } }
        public User(long id, string nickName, string firstName, string lastName, bool banned,string uniqName, MessageI lastMessage)
        {
            BaseInfo = new BaseUserInfo(id, nickName, firstName, lastName, banned, uniqName,lastMessage);
            type = UserTypes.BaseUser;
        }
        public User(DataRow r)
        {
            BaseInfo = new BaseUserInfo(r);
            type = UserTypes.BaseUser;
        }

        public string ToString()
        {
            return $"({type.Name}:{BaseInfo.Id}) {Name}";
        }


    }


    public class DefaultUser : User
    {
        public DefaultUser(long id, string nickName, string firstName, string lastName, bool banned,string uniqName, MessageI lastMessage) : base(id, nickName, firstName, lastName, banned,uniqName, lastMessage)
        {
            type = UserTypes.BaseUser;
        }
        public DefaultUser(DataRow r):base(r)
        {
           type = UserTypes.BaseUser;
        }
    }
    public class Teen : User
    {
        public Teen(long id, string nickName, string firstName, string lastName, bool banned,string uniqName, MessageI lastMessage) : base(id, nickName, firstName, lastName, banned, uniqName, lastMessage)
        {
            teenInfo = new TeenInfo();
            type = UserTypes.Teen;
        }
        public Teen(DataRow r) : base(r)
        {
            type = UserTypes.Teen;
        }
        public void SendAnonimMessage(MessageI message)
        {

        }
    }
    public class Teacher : User
    {

        public Teacher(long id, string nickName, string firstName, string lastName, bool banned,string uniqName, MessageI lastMessage) : base(id, nickName, firstName, lastName, banned, uniqName, lastMessage)
        {
            teacherInfo = new TeacherInfo();
            type = UserTypes.Teacher;
        }
        public Teacher(DataRow r) : base(r)
        {
            type = UserTypes.Teacher;
        }
    }
    public class Admin : User
    {
        public Admin(long id, string nickName, string firstName, string lastName, bool banned,string uniqName, MessageI lastMessage) : base(id, nickName, firstName, lastName, banned, uniqName, lastMessage)
        {
            teacherInfo = new TeacherInfo();
            type = UserTypes.Admin;
        }
        public Admin(DataRow r) : base(r)
        {
            type = UserTypes.Admin;
        }
    }

}
