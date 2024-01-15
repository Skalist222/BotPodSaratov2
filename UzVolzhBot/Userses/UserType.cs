using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
