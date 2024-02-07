using TelegramBotClean.CommandsDir;
using TelegramBotClean.Data;
using TelegramBotClean.TextDir;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static TelegramBotClean.CommandsDir.CommandsExecutor;

namespace TelegramBotClean.Commandses
{
    public class Commands : CommandList
    {
        private bool fromButton;
        public bool IsButtonCommand { get { return fromButton; } }
        public void FromButtonTrue() { fromButton = true; }
        public void FromButtonFalse() { fromButton = false; }
        public string SeparatedBySpace()
        {
            return string.Join(" ", this.Select(el => el.Name));
        }

        // Сюда писать новые команды
        private static Dictionary<string, Command> simpleCommands = new Dictionary<string, Command>
        {
            {"/admin",new Command("/admin",new string[]{ "admin","админ"},ExAdmin)},
            {"/start",new Command("/start",new string[]{ "старт","start"},ExStart)},
            {"/add",new Command("/add",new string[]{ "добав","+","add"},ExUnknow)},
            {"/turnOn",new Command("/turnOn",new string[]{ "включ","turnOn"}, ExUnknow)},
            {"/turnOff",new Command("/turnOff",new string[]{ "выключ","отключ", "отмен", "turnoff"},ExUnknow)},
            {"/mem",new Command("/mem",new string[]{ "мем","mem" },ExMem)},
            {"/gold",new Command("/gold",new string[]{ "золот","gold"},ExUnknow)},
            {"/verse",new Command("/verse",new string[]{ "стих","стиш"},ExUnknow)},
            {"/bible",new Command("/bible",new string[]{ "библ","bibl"})},
            {"/info",new Command("/info",new string[]{ "инфо","info"},ExInfo)},
            {"/spam",new Command("/spam",new string[]{ "спам","spam"}, ExUnknow)},
            {"/thanks",new Command("/thanks",new string[]{ "спасиб","спс","thanks"}, ExUnknow)},
            {"/anon",new Command("/anon",new string[]{ "анон","anon"}, ExUnknow)},
            {"/messages",new Command("/messages",new string[]{ "сообщения" },ExUnknow)},
            // Сочетание второго и третьего невидимого символа, будут означать "все"
            {"/all",new Command("/all",new string[]{ "все",InvizibleEquals.All},ExUnknow)},
            {"/help",new Command("/help",new string[]{ "помощь"},ExHelp)},
            {"/howWorkButton",new Command("/howWorkButton",new string[]{ "Как работают кнопки"},ExHowWorkButton)},
            {"/read",new Command("/read",new string[]{ "прочитать","прочесть","читать","read"},ExUnknow)},
            {"/answere",new Command("/answere",new string[]{"ответ"},ExUnknow)},
            {"/random",new Command("/random",new string[]{"случай","рандом"},ExUnknow)},

        };
        public static Dictionary<string, Command> complexCommands = new Dictionary<string, Command>
        {
            {"/add/mem",new Command("/add/mem",new string[]{ "+ мем"},ExAddMem)},
            {"/add/gold/verse",new Command("/add/gold/verse",new string[]{ "добавить золотой стих"},ExAddGoldVerse)},
            {"/turnOn/anon",new Command("/turnOn/anon",new string[]{"включить анон" },ExOnAnon)},
            {"/turnOff/anon",new Command("/turnOff/anon",new string[]{"отключить анон" },ExOffAnon)},
            {"/anon/read",new Command("/anon/read",new string[]{"прочесть анонимку" },ExReadAnon)},
            {"/admin/users",new Command("/admin/users",new string[]{"Все пользователи админу" },ExAdminUsers)},
            {"/admin/mem/all",new Command("/admin/mem/all",new string[]{"Все мемы админу" },ExAdminMems)},
            {"/anon/messages/all",new Command("/anon/messages/all",new string[]{"все неотвеченые анонимки" },ExAllAnonimMessages)},
            {"/anon/messages/answere",new Command("/anon/messages/answere",new string[]{"ответ на анонимку" },ExecuteOnAnswereAnon)},
            {"/turnOff/answere",new Command("/turnOff/answere",new string[]{"отмена ответа на анонимку" },ExecuteOffAnswereAnon)},
            
            {"/verse/random",new Command("/verse/random",new string[]{ "Случайный стих"},ExVerse)},
            {"/gold/verse/random",new Command("/gold/verse/random",new string[]{ "Случайный золотой стих"},ExGoldVerse)},
        };

        //Для полноценной команды в программу нужно:


        //Все команды
        #region Все команды (Геттеры)

        public static Command Get(string name)
        {
            Commands commands = Commands.SelectCommands(name);
            return commands.AsCommand();
        }
        // геттеры нужны, чтобы обращаться к существующим командам
        /// <summary>
        /// Геттер команды "мем"
        /// </summary>
        /// 
        public static Command Unknow { get {
                return new Command("clean",new string[] { }, CommandsExecutor.ExUnknow);
            } }
        public static Command MemCommand { get { return simpleCommands["/mem"]; } }
        /// <summary>
        /// Геттер команды "золотой"
        /// </summary>
        public static Command GoldCommand { get { return simpleCommands["/gold"]; } }
        /// <summary>
        /// Геттер команды "Стих"
        /// </summary>
        public static Command VerseCommand { get { return simpleCommands["/verse"]; } }
        public static Command AddCommand { get { return simpleCommands["/add"]; } }
        public static Command BibleCommand { get { return simpleCommands["/bible"]; } }
        public static Command InfoCommand { get { return simpleCommands["/info"]; } }
        public static Command StartCommand { get { return simpleCommands["/start"]; } }

        #endregion
        

        #region Комплексные команды
        public static Commands GoldVerseCommands { get { return GoldCommand + VerseCommand; } }
        public static Commands AddGoldVerseCommand { get { return AddCommand + GoldVerseCommand; } }
        public static Command GoldVerseCommand { get { return complexCommands["/gold/verse"]; } }
        public static Command OffAnonCommand { get { return complexCommands["/turnOff/anon"]; } }
        #endregion


        #region Идентификаторы (проверка что именно это за команды)
        public bool Is(string variativText)
        {
            Commands selected = Commands.SelectCommands(variativText);
            return selected == this;
        }
        public Command AsCommand()
        {
            string construct = this.ToString();
            
            Command simple = simpleCommands.Where(x => x.Key == construct).FirstOrDefault()!.Value;
            Command complex= complexCommands.Where(x => x.Key == construct).FirstOrDefault()!.Value;
            if (simple is not null) return simple;
            if (complex is not null) return complex;
            //Если и один и второй нул, значит неизвестная
            return Unknow;
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
        public Commands(bool fromButton = false, bool clean = true) : base()
        {
            this.fromButton = fromButton;
            if (!clean)
            {
                this.AddRange(simpleCommands.Select(el => el.Value).ToList());
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
            Commands allCommands = new Commands(false,false);
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
    public class ZeroEquals
    {
        
    }
}
