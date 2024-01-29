using System.Data;
using TelegramBotClean.Bot;
using TelegramBotClean.Data;
using TelegramBotClean.Messages;

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
    public class UnansweredAnonimMessage: AnonimMessage
    {
        public UnansweredAnonimMessage(string text, string uniqName, long idTeen,  int idMessage, long idTeacher=0,string desiredTeacher="") :base(text,uniqName, idTeen, desiredTeacher, idMessage, idTeacher) { }

        public void SetAnswere(Sender sender, MessageI message)
        {
            
        }
    }
    public class UnansweredsMeesages: List<UnansweredAnonimMessage>
    {
        public bool Empty { get { return this.Count ==0; } }
        public UnansweredsMeesages(BotDB botBase) : base()
        {
            DataTable t = botBase.GetAllAnonMessageWithTeacherName();
            Dictionary<long, string> names = botBase.GetUniqNamesWithId();
            for (int i = 0; i < t.Rows.Count; i++)
            {
                DataRow r = t.Rows[i];
                long idDesTeach = Convert.ToInt64(r["desiredTeacher"].ToString());
                if (idDesTeach == 0)
                {
                    Add(new UnansweredAnonimMessage(
                        r["text"].ToString(),
                        r["anonName"].ToString(),
                        Convert.ToInt64(r["idTeen"].ToString()),
                        Convert.ToInt32(r["idMessage"].ToString())
                        ));
                }
                else
                {
                    string desTeachName = names[idDesTeach];
                    Add(new UnansweredAnonimMessage(
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
  
        public UnansweredsMeesages(UnansweredAnonimMessage mes1, UnansweredAnonimMessage mes2= null, UnansweredAnonimMessage mes3 = null, UnansweredAnonimMessage mes4 = null, UnansweredAnonimMessage mes5 = null) : base()
        {
            this.Add(mes1);
            if (mes2 is not null) this.Add(mes2);
            if (mes3 is not null) this.Add(mes3);
            if (mes4 is not null) this.Add(mes4);
            if (mes5 is not null) this.Add(mes5);
        }
        public UnansweredsMeesages(List<UnansweredAnonimMessage> array):base()
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
        public Dictionary<string, UnansweredsMeesages> GetGroupsMessages()
        {
            return this.GroupBy(el => el.UniqName)
                .ToDictionary(
                g => g.Key, 
                g => new UnansweredsMeesages(g.ToList()
                ));
                
        }
        
    }

}
