using System.Data;
using System.Timers;
using TelegramBotClean.Bot;
using TelegramBotClean.Data;
using TelegramBotClean.Messages;
using TelegramBotClean.Userses;

namespace TelegramBotClean.MessagesDir
{
    public abstract class AnonimMessage
    {
        protected bool answered;
        protected int idMessage;
        protected string text;
        protected string uniqName;
        protected long idDesiredteacher;
        protected string desiredTeacher;
        protected long idTeen;

        public bool Answered { get { return answered; } }
        public string Text { get { return text; } }
        public string UniqName { get { return uniqName; } }
        public string TeacherName { get { return desiredTeacher; } }
        public long IdTeen { get { return idTeen; } }
        public void SetTrueAnswered() { answered = true; }
        public AnonimMessage(string text, string uniqName, long idTeen, string desiredTeacher, int idMessage,long idTeacher)
        {
            this.text = text;
            this.uniqName = uniqName;
            this.idTeen = idTeen;
            this.desiredTeacher = desiredTeacher;
            this.idMessage = idMessage;
            this.idDesiredteacher = idTeacher;
        }
    }
    public class UnansveredAnonimMessage: AnonimMessage
    {
        public UnansveredAnonimMessage(string text, string uniqName, long idTeen,  int idMessage, long idTeacher=0,string desiredTeacher="") :base(text,uniqName, idTeen, desiredTeacher, idMessage, idTeacher) { }

        public void SetAnswere(Sender sender, MessageI message)
        {
            
        }
    }
    public class UnansveredsMeesages: List<UnansveredAnonimMessage>
    {
        public bool IsEmpty { get { return this.Count == 0; } }
        public UnansveredsMeesages(BotDB botBase) : base()
        {
            DataTable t = botBase.GetAllAnon();
            Dictionary<long, string> names = botBase.GetUniqNamesWithId();
            for (int i = 0; i < t.Rows.Count; i++)
            {
                DataRow r = t.Rows[i];
                long idDesTeach = Convert.ToInt64(r["desiredTeacher"].ToString());
                if (idDesTeach == 0)
                {
                    Add(new UnansveredAnonimMessage(
                        r["text"].ToString(),
                        r["anonName"].ToString(),
                        Convert.ToInt64(r["idTeen"].ToString()),
                        Convert.ToInt32(r["idMessage"].ToString())
                        ));
                }
                else
                {
                    string desTeachName = names[idDesTeach];
                    Add(new UnansveredAnonimMessage(
                        r["text"].ToString(),
                        r["anonName"].ToString(),
                        Convert.ToInt64(r["idTeen"].ToString()),
                        Convert.ToInt32(r["idMessage"].ToString()),
                        idDesTeach,
                        desTeachName
                        ));
                }
            }
        }
  
        public UnansveredsMeesages(UnansveredAnonimMessage mes1, UnansveredAnonimMessage mes2= null, UnansveredAnonimMessage mes3 = null, UnansveredAnonimMessage mes4 = null, UnansveredAnonimMessage mes5 = null) : base()
        {
            this.Add(mes1);
            if (mes2 is not null) this.Add(mes2);
            if (mes3 is not null) this.Add(mes3);
            if (mes4 is not null) this.Add(mes4);
            if (mes5 is not null) this.Add(mes5);
        }
        public UnansveredsMeesages(List<UnansveredAnonimMessage> array):base()
        {
            for (int i = 0; i < array.Count; i++)
            {
                this.Add(array[i]);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// string - Имя ученика
        /// UnansweredsMeesages - сообщения от него
        /// </returns>
        /// 
        public GroupsUbabsvereMessage GetGroupsMessages()
        {
            int count = this.Count;
            GroupsUbabsvereMessage groups = new GroupsUbabsvereMessage();
            for (int i = 0; i < count; i++)
            {
                string uniqName = this[i].UniqName;
                if (!this[i].Answered)
                {
                    if (groups.Have(uniqName))
                    {
                        groups[uniqName].Messages.Add(this[i]);
                    }
                    else
                    {
                        //groups.Add(new GroupUnansvereMessage(this[i].IdTeen, uniqName, new UnansveredsMeesages(this[i])));
                    }
                }
            }
            return groups;
        }
        public void SetAnswered(long idTeen)
        {
            List<UnansveredAnonimMessage> mess = this.Where(el => el.IdTeen == idTeen).ToList();
            if (mess.Count != 0)
                for (int i = 0; i < mess.Count; i++)
                {
                    mess[0].SetTrueAnswered();
                }

        }
        
    }

    public class GroupUnansvereMessage 
    {
        public User User { get; }
        public long IdAnonUser { get; }
        public string UniqUserName { get; }
        public UnansveredsMeesages Messages { get; }

        public GroupUnansvereMessage(User user, string uniqUserName, UnansveredsMeesages messages)
        {
            this.User = user;
            this.UniqUserName = uniqUserName;
            this.Messages = messages;
        }
    }
    public class GroupsUbabsvereMessage : List<GroupUnansvereMessage>
    {
        public GroupsUbabsvereMessage():base()
        { 
            
        }
        public bool Have(string uniqName)
        {
            return this.Where(el => el.UniqUserName == uniqName).Count() != 0;
        }
        public GroupUnansvereMessage this[string uniqName]
        {
            get 
            {
                List<GroupUnansvereMessage> valids = this.
                    Where(el => el.UniqUserName == uniqName).
                    ToList();
                if (valids.Count == 1) return valids[0];
                else return null;
            }
        }
    }
}
