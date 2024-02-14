

namespace TelegramBotClean.Userses
{
    public class UserTypes
    {
        private static Dictionary<string,UserType> all = new Dictionary<string, UserType> {
            { "user",new UserType("user", "Прихожанин, без доступа к особым функциям") },
            { "teen",new UserType("teen", "Пользователь с правами ученика")},
            { "teacher",new UserType("teacher", "Пользователь с правами учителя") },
            { "admin",new UserType("admin", "Администратор") }
         
        }; 
        public static UserType BaseUser { get { return all["user"]; } }
        public static UserType Teen { get { return all["teen"]; } }
        public static UserType Teacher { get { return all["teacher"]; } }
        public static UserType Admin { get { return all["admin"]; } }

        internal static UserType? Select(string v)
        {
            if (all.ContainsKey(v))
            {
                return all[v];
            }
            else
            {
                return null;
            }
        }
    }
}
