namespace TelegramBotClean.Commandses
{
    public abstract class CommandList : List<Command>
    {
        public string ToString()
        {
            if (this.Count == 0) return "clean";
            else if (this.Count == new Commands(false).Count) return "all";
            else
            {
                string retString = this[0].Name;
                for (int i = 1; i < this.Count; i++)
                {
                    retString +=  this[i].Name;
                }
                return retString;
            }
        }
        public bool Have(Command c)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] == c) return true;
            }
            return false;
        }
        public bool Have(Commands c)
        {
            int selectedCommand = 0;
            for (int j = 0; j < c.Count; j++)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i] == c[j])
                    {
                        // Если мы нашли какой то элемент,
                        // то его же не нужно проверять дальше
                        selectedCommand++;
                        break;
                    }
                }
            }
            return selectedCommand == c.Count;
        }
        public bool ElementEqual(Command c,int idElement) 
        {
            if (idElement < 0 || idElement > this.Count - 1) return false;
            return this[idElement] == c;
        }
        /// <summary>
        /// Проверяет, соответствует ли первый элемент набора команд, полученному
        /// </summary>
        /// <param name="c">Полученная команда</param>
        /// <returns>True-команда является первой False- Команда не является первой</returns>
        public bool FirstEqual(Command c)
        {
            return ElementEqual(c,0);
        }
        /// <summary>
        /// Проверяет, соответствует ли второй элемент набора команд, полученному
        /// </summary>
        /// <param name="c">Полученная команда</param>
        /// <returns>True-команда является второй False- Команда не является второй</returns>
        public bool SecondEqual(Command c)
        {
            return ElementEqual(c, 1);
        }
        /// <summary>
        /// Проверяет, соответствует ли третий элемент набора команд, полученному
        /// </summary>
        /// <param name="c">Полученная команда</param>
        /// <returns>True-команда является третей False- Команда не является третей</returns>
        public bool ThirdEqual(Command c)
        {
            return ElementEqual(c, 2);
        }

        public bool LastEqual(Command c)
        {
            return ElementEqual(c, this.Count-1);
        }
        public static CommandList operator +(CommandList cList, Command c2)
        {
            CommandList list = cList;
            list.Add(c2);
            return list;
        }
        public static CommandList operator +(CommandList cList1, CommandList cList2)
        {
            CommandList list = cList1;
            list.AddRange(cList2);
            return list;
        }
       

    }
}
