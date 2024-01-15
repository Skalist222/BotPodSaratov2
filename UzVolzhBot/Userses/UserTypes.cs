using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotClean.Userses
{
    public class UserTypes
    {
        public static UserType BaseUser { get { return new UserType("user", "Прихожанин, без доступа к особым функциям"); } }
        public static UserType Teen { get { return new UserType("teen", "Пользователь с правами ученика"); } }
        public static UserType Teacher { get { return new UserType("teacher", "Пользователь с правами учителя"); } }
        public static UserType Admin { get { return new UserType("admin", "Администратор"); } }
    }
}
