using Microsoft.Identity.Client;
using System.Data;
using System.Net.Security;
using TelegramBotClean.Bible;
using TelegramBotClean.Commandses;
using TelegramBotClean.Data;
using TelegramBotClean.Messages;
using TelegramBotClean.TextDir;
using TelegramBotClean.UsersesDir;
using Logger = TelegramBotClean.Data.Logger;


namespace TelegramBotClean.Users
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

        private string anonNameByTeen;
        private long idWentTeacher;

        private long idAnsweredTeenAnon;// ученик которому будет отправляться сообщение ответ на анонимку
        

        public MessageI MenuMessage { get; set; }
        public MessageI WentedTeacherMessage { get; set; }


        public long Id { get { return id; } }
        public string NickName { get { return nickName; } }
        public string Lastname {  get { return lastname; } }
        public string FirstName {  get { return firstName; } }
        public string UniqueName { get { return uniqName; } }
        public string TypeName { get { return privileges.Name; } }
        public string AnonName { get { return anonNameByTeen; } }

        public string Name { get 
            {
                if (UniqueName != "") return UniqueName;
                if (Lastname !="") return Lastname;
                if (FirstName !="") return FirstName;
                if (NickName != "") return NickName;
                return "NoName";
            } }
        public bool Spam { get { return spam; } }
        public bool Ban { get { return ban; } }
        public bool Deleted { get { return deleted; } }
        public bool IsAdmin { get { return Type == Privileges.Admin; } }


        public bool InAnon { get { return anonNameByTeen != ""; } }
        public bool InAnswerAnon { get { return idAnsweredTeenAnon != 0; } }
        public string SideMenu { get 
            {
                if (InAnon) return "anon";
                if (InAnswerAnon) return "answerAnon";
                return "no";
            }
        }

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
            anonNameByTeen = "";
            idAnsweredTeenAnon = 0;
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
        public User(Telegram.Bot.Types.User user,BibleWorker bW)
            :this(user.Id, user.Username, user.LastName, user.FirstName, "",true, false, false,bW.GetRandomGoldVerse(),Privileges.DefaultUser)
        {             
        }
        public static User Empty {  get { return new User(0,"","","","",true,true,true,null,Privileges.Empty); } }

        public long IdTeenAnonMessage { get { return idAnsweredTeenAnon; } }

        public long IdWentTeacher { get { return idWentTeacher; } }

        public void SetNewType(Privileges privileges)
        {
            if (privileges == this.privileges)
            {
                Logger.Info("Попытка поставить пользователю тот же тип что у него и был!");
            }
            else this.privileges = privileges;
        }
        public void SetInAnonOn(TextWorker tW,MessageI menuMes)
        {
            MenuMessage = menuMes;
            anonNameByTeen = tW.RandomAnonName();
        }
        public void SetInAnonOff(TextWorker tW)
        {
            tW.AnonNameFinished(anonNameByTeen);
            anonNameByTeen = "";
        }

        internal void SetWentedTeacher(long idTeacher, MessageI mes)
        {
            WentedTeacherMessage = mes;
            idWentTeacher = idTeacher;
        }

        internal void TurnOnAnswerOnAnon(long idTeen)
        {
            idAnsweredTeenAnon = idTeen;
        }
        internal void TurnOffAnswerOnAnon()
        {
            idAnsweredTeenAnon = 0;
        }

        public static bool operator ==(User u1, User u2)
        {
            return u1.Id == u2.Id;
        }
        public static bool operator !=(User u1, User u2)
        {
            return u1.Id != u2.Id;
        }
    }
   
}