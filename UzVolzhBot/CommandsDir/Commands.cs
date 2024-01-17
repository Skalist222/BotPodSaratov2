using Microsoft.Identity.Client;

namespace TelegramBotClean.Commandses
{
    public class Commands : CommandList
    {

        // Сюда писать новые команды
        private static Dictionary<string, Command> allCommands = new Dictionary<string, Command>
        {
            {"start",new Command("/start",new string[]{ "старт","start"})},
            {"mem",new Command("/mem",new string[]{ "мем" })},
            {"gold",new Command("/gold",new string[]{ "золот","gold"})},
            {"verse",new Command("/verse",new string[]{ "стих","стиш"})},
            {"add",new Command("/add",new string[]{ "добав","+","add"})},
            {"bible",new Command("/bible",new string[]{ "библ","bibl"})},
            {"info",new Command("/info",new string[]{ "инфо","info"})},
            {"on",new Command("/on",new string[]{ "включ","on"})},
            {"off",new Command("/off",new string[]{ "выключ","отключ","off"})},
            {"spam",new Command("/spam",new string[]{ "спам","spam"})},
            {"thenks",new Command("/thanks",new string[]{ "спасибо","спс","thenks"})}
        };
        public static Dictionary<string, Command> complexCommands = new Dictionary<string, Command>
        {
            {"addMem",new Command("/addMem",new string[]{ "+ мем"})},
            {"goldverse",new Command("/goldverse",new string[]{ "золотой стих"})},
            {"addGoldverse",new Command("/addGoldverse",new string[]{ "золотой стих"})}
        };

        //Для полноценной команды в программу нужно:
        //1) Создать геттер по примеру MemCommand.
        //2) В конструктор Command():base(), внутри проверки if(!clean) прописать добавление
        //   недавно созданного геттера по примеру this.Add(MemCommand)

        //Все команды
        #region Все команды (Геттеры)

        public static Command Get(string name)
        {
            return allCommands[name];
        }
        // геттеры нужны, чтобы обращаться к существующим командам
        /// <summary>
        /// Геттер команды "мем"
        /// </summary>
        public static Command MemCommand { get { return allCommands["mem"]; } }
        /// <summary>
        /// Геттер команды "золотой"
        /// </summary>
        public static Command GoldCommand { get { return allCommands["gold"]; } }
        /// <summary>
        /// Геттер команды "Стих"
        /// </summary>
        public static Command VerseCommand { get { return allCommands["verse"]; } }
        public static Command AddCommand { get { return allCommands["add"]; } }
        public static Command BibleCommand { get { return allCommands["bible"]; } }
        public static Command InfoCommand { get { return allCommands["info"]; } }
        public static Command StartCommand { get { return allCommands["start"]; } }
        #endregion
        

        #region Комплексные команды
        public static Commands GoldVerseCommands { get { return GoldCommand + VerseCommand; } }
        public static Commands AddGoldVerseCommand { get { return AddCommand + GoldVerseCommand; } }
        public static Command GoldVerseCommand { get { return allCommands["goldVerse"]; } }
        
        #endregion
      



        #region Идентификаторы (проверка что именно это за команды)
        public bool Is(string variativText)
        {
            Commands selected = Commands.SelectCommands(variativText);
            return selected == this;
        }

        // Не уверен что они пригодятся, удалить если вдруг они не пригодились
        public bool IsMem { get { return this.FirstEqual(Commands.MemCommand); } }
        public bool IsVerse { get { return this.FirstEqual(Commands.VerseCommand); } }
        public bool IsGoldVerse { get { return this.Have(Commands.GoldVerseCommand); } }
        public bool IsAddGoldVerse { get { return this.Have(Commands.AddGoldVerseCommand); } }
        public bool IsStart { get { return this.FirstEqual(Commands.StartCommand); } }
        #endregion


        #region Конструкторы
        /// <summary>
        /// Конструктор класса Commands
        /// </summary>
        /// <param name="clean">
        /// True - создасться пустой класс без команд
        /// False - создастся класс со всеми возможными командами
        /// </param>
        public Commands(bool clean = true) : base()
        {
            if (!clean)
            {
                this.AddRange(allCommands.Select(el => el.Value).ToList());
            }
        }
        #endregion


        #region Операторы
        public static bool operator ==(Commands c1, Commands c2)
        {
            if (c1 is null) { if (c2 is null) return true; else return false; }
            else 
            { 
                if (c2 is null) return false; 
                else 
                {
                    if (c1.Count != c2.Count) return false;

                    for (int i = 0; i < c2.Count; i++)
                    {
                        if (!c1.Have(c2[i])) return false;
                    }
                    return true;
                } 
            }
        }
        public static bool operator !=(Commands c1, Commands c2)
        {
            return !(c1 == c2);
        }
        #endregion


        #region Дополнительные функции
        public void Add(Command c1, Command c2= null, Command c3=null, Command c4=null, Command c5=null, Command c6=null, Command c7=null, Command c8=null, Command c9=null, Command c10=null)
        {
            base.Add(c1);
            if (c2 is not null) base.Add(c2);
            if (c3 is not null) base.Add(c3);
            if (c4 is not null) base.Add(c4);
            if (c5 is not null) base.Add(c5);
            if (c6 is not null) base.Add(c6);
            if (c7 is not null) base.Add(c7);
            if (c8 is not null) base.Add(c8);
            if (c9 is not null) base.Add(c9);
            if (c10 is not null) base.Add(c10);
        }
        public static Commands SelectCommands(string text)
        {
            Commands allCommands = new Commands(false);
            Commands selectedCommands = new Commands();
            string[] split = text.Split(' ');
            // Если в тексте больше 15 слов, то возвращаем инфу о том, что это не команда вовсе 
            if (split.Length > 15) return selectedCommands;

            // пробегаем все команды по очереди и проверяем на наличие 
            for (int i = 0; i < allCommands.Count; i++)
            {
                if (allCommands[i].Check(text))
                {
                    // если текст, который пришел в сообщении соответствует команде,
                    // то получаем команду и переходим к следующей команде
                    selectedCommands.Add(allCommands[i]);
                    continue;
                }
                // если вдруг команда не найдена по полному тексту,
                // то разбиваем полученый текст на слова и
                // проверяем каждое слово

                for (int j = 0; j < split.Length; j++)
                {
                    if (allCommands[i].Check(split[j]))
                    {
                        selectedCommands.Add(allCommands[i]);
                        break;
                    }
                }
            }
            return selectedCommands;
        }

        #endregion
    }
}
