using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotClean.UsersesDir
{
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
        static Dictionary<string, Privileges> allPrivileges = new Dictionary<string, Privileges>()
        {
             { "empty", new Privileges(-1, "empty", "учитель")},
            { "admin", new Privileges(0, "admin", "Права администратора")},
            { "user", new Privileges(1, "user", "стандартный пользователь")},
            { "teen", new Privileges(2, "teen", "ученик")},
            { "teacher", new Privileges(3, "teacher", "учитель")}
        };
        public static Privileges Admin { get { return allPrivileges["admin"]; } }
        public static Privileges DefaultUser { get { return allPrivileges["user"]; } }
        public static Privileges Teen { get { return allPrivileges["teen"]; } }
        public static Privileges Teacher { get { return allPrivileges["teacher"]; } }
        public static Privileges Empty { get { return allPrivileges["empty"]; } }
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

        public static bool operator ==(Privileges p1, Privileges p2)
        {
            return p1.Id == p2.Id;
        }
        public static bool operator !=(Privileges p1, Privileges p2)
        {
            return p1.Id != p2.Id;
        }
        public static bool operator ==(Privileges p1, string namePrivilege)
        {
            return p1.Name == namePrivilege;
        }
        public static bool operator !=(Privileges p1, string namePrivilege)
        {
            return p1.Name != namePrivilege;
        }
    }
}
