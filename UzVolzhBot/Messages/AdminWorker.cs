using PodrostkiBot.DataBase.Engine;
using PodrostkiBot.Users;
using System.Data;
using PodrostkiBot.Configure;

namespace PodrostkiBot.Messages
{
    public class AdminWorker
    {
        public static string GetAllUserNamesAndId(BotBase botBase)
        {
            string retString ="";
            DataTable t = botBase.GetAllUsers();
            int i = 1;
            foreach (DataRow r in t.Rows)
            {
                long id = Convert.ToInt64(r["id"].ToString());
                string name = botBase.GetNameUser(id);
                string priv = UserPrivilege.SelectPriv(r["privileges"].ToString()).RuName;
                retString+=$"({i}) {id} {name} {priv}"+Environment.NewLine;
                i++;
            }
            if (retString == "") return "Пусто";
            return retString;
        }
        public static string GetAllGolds(BotBase botBase)
        {
            string retString = "";
            DataTable t = botBase.GetAllGoldVerses();
            
            foreach (DataRow r in t.Rows)
            {
                string id = r["id"].ToString();
                string text = r["address"].ToString()+" "+r["textI"].ToString();
                if (text.Length > 30) text = text.Substring(0,30)+ Environment.NewLine;
                retString += id +") "+ text;
            }
            if (retString == "") return "Пусто";
            return retString;
        }

        public static bool SetNewPrivelegeUser(Engine engine,UserI user,string priv)
        {
            
                UserPrivilege privilege = UserPrivilege.SelectPriv(priv);
                if (engine.BotBase.SetPrivileges(user.Id, privilege))
                {
                    user.SetPrivilege(privilege);
                    engine.Menus.SelectMenu(engine.Sender, "Тебе даны другие права управления ботом:", user);
                    return true;
                }
            return false;
            
        }


    }
}
