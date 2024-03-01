using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotClean.Data;
using TelegramBotClean.Messages;
using TelegramBotClean.Users;

namespace TelegramBotClean.MessagesDir
{
    public class AnonMessage
    {//reply_to_message

        bool answered;
        public bool Answered { get { return answered; } }
        public void SetAnswered()
           
        {
            answered = true;
        }
        public User Teen { get; }
        public User AnswereTeacher { get; }
        public User WantDectinary { get; }
        public MessageI Message { get; }
        public BotDB BotBase { get; }
       
        public AnonMessage(User teen, User answereTeacher, User wantDectinary, MessageI message,BotDB botBase)
        {
            // сразу надо добавлять в базу
            answered = false;
            Teen = teen;
            AnswereTeacher = answereTeacher;
            WantDectinary = wantDectinary;
            Message = message;
            BotBase = botBase;
        }
        
        
    }
    public class AnonMessages : List<AnonimMessage>
    {
        public AnonMessages():base() 
        { 

        }

        public AnonMessages(BotDB botBase, Users.Users users) : base()
        {
            DataTable fromDataBase = botBase.GetAllAnon();
            for (int i = 0; i < fromDataBase.Rows.Count; i++)
            {
                DataRow r = fromDataBase.Rows[i];
                long idTeen = Convert.ToInt64(r["idTeen"].ToString());
                long idWentTeacher = Convert.ToInt64(r["idWentTeacher"].ToString());
                long idAnswerTeacher = Convert.ToInt64(r["idAnswerTeacher"].ToString());
                long idMessage = Convert.ToInt64(r["idMessage"].ToString());
                User Teen = null;
                User AnswereTeacher = null;
                User WentTeacher = null;
                if (users.Have(idTeen)) Teen = users[idTeen];
                if (users.Have(idWentTeacher)) AnswereTeacher = users[idWentTeacher];
                if (users.Have(idWentTeacher)) WentTeacher = users[idWentTeacher];

            }
        }
    }
}
