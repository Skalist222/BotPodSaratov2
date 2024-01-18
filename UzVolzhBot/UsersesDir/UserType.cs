

namespace TelegramBotClean.Userses
{
    public class UserType
    {
        public string Name { get; }
        public string Description { get; }
        public string ToString()
        {
            return Name;
        }
        public UserType(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public static bool operator ==(UserType t1, UserType t2)
        {
            return t1.Name == t2.Name;
        }
        public static bool operator !=(UserType t1, UserType t2)
        {  
            return !(t1==t2);
        }
        override public bool Equals(object o)
        {
            if (o is UserType)
            {
                return ((UserType)o).Name == this.Name;
            }
            else return false;
        }
        
    }
}
