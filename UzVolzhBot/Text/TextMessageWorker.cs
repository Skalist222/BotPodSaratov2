using PodrostkiBot.Bible;
using PodrostkiBot.DataBase.Engine;
using PodrostkiBot.Messages;
using PodrostkiBot.Users;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using Telegram.Bot.Types;
using static System.Net.Mime.MediaTypeNames;


namespace PodrostkiBot.Text
{
    /// <summary>
    /// Для добавления команды нужно:
    /// 1) в WordsCommands добавить новую команду
    /// 2) в TextMessageWorker добавить поле только для ьчтения
    /// 3) в  конструкторе TextMessageWorker добавить присвоение значение этой переменной
    /// 4) в ComandWariants создать комбинатор
    /// 5) в ComandWariants в commands добавить текстовое представление команды такое же какое прописал в WordsCommands
    /// 6) в Commands добавить валидатор типа IsCommand
    /// </summary>


    public class FunctionLecal
    {
        Words lecalWords;
        Function func;
        public FunctionLecal(Words lecalWords, Function func)
        {
            this.lecalWords = lecalWords;
            this.func = func;
        }
    }
    public class Function
    {
        string command;
        string description;
        public string Command { get { return command; } }
        public string Description { get { return description; } }

        public Function(string command, string description)
        {
            this.command = command;
            this.description = description;
        }

    }
    public class Words : List<Word>
    {
        public Words(string[] args)
        {
            foreach (string a in args)
            {
                Add(new Word(a));
            }
        }
        public static Words Get(string text, char separator = ' ')
        {
            string[] split = text.Split(separator);
            Words w = new Words(split);
            return w;
        }

    }
    public class Word
    {
        string text;
        public string Text { get { return text; } }
        public Word(string t)
        {
            text = t;
        }
        public static bool operator ==(Word w1, Word w2)
        {
            return w1.text.ToLower() == w2.text.ToLower();
        }
        public static bool operator !=(Word w1, Word w2)
        {
            return !(w1 == w2);
        }

        public string ToString()
        {
            return text;
        }
    }


    public class AllCommands : List<TextCommand>
    {
        protected AllCommands() : base() { }
        public AllCommands(BotBase botBase):this()
        {
            this.AddRange(botBase.GetAllCommands().AsEnumerable().Select(el =>
            new TextCommand(int.Parse(el["id"].ToString()), el["textCommand"].ToString())
            ));
        }
        public TextCommand ById(int id)
        {
            return this.Where(el => el.Id == id).FirstOrDefault();
        }
        public TextCommand this[string s]
        {
            get 
            {
                return this.Where(el => el.Text == s || el.Text == @"/" + s).FirstOrDefault(new TextCommand(-1,"empty"));
            }
        }
    }
    public class AllRespWords : AllCommands
    {
        public AllRespWords(BotBase botBase):base()
        {
            this.AddRange(botBase.GetAllResponceWord().AsEnumerable().Select(el =>
            new TextCommand(int.Parse(el["idCommand"].ToString()), el["word"].ToString())
            ));
        }
    }


    public class Selectors : List<SelectorCommands>
    {
        public Selectors(AllCommands commands):base()
        {
            this.Add(new SelectorCommands("start", new Commands2[] { 
                new Commands2(commands["start"]),
            })) ;
            this.Add(new SelectorCommands("mem", new Commands2[] {
                new Commands2(commands["mem"]),
                new Commands2(commands["get"],commands["mem"]),
            }));
            this.Add(new SelectorCommands("info", new Commands2[] {
                new Commands2(commands["info"]),
                new Commands2(commands["get"],commands["info"]),
            }));
            this.Add(new SelectorCommands("anonMessages", new Commands2[] {
             
                new Commands2(commands["anon"],commands["messages"]),
                new Commands2(commands["messages"],commands["anon"]),
            }));
            this.Add(new SelectorCommands("report", new Commands2[] {
                new Commands2(commands["report"]),
            }));
            // /report


        }
        public SelectorCommands Unknow { get { return new SelectorCommands("unknow",new Commands2[] { }); } }
        public string GetSelectionName(Commands2 commands)
        {
            return this.Where(el => el.HaveCommandList(commands)).FirstOrDefault(Unknow).SelectorName;
        }
    }
    public class SelectorCommands
    {
        public Commands2[] VariantsCommand { get; }
        public string SelectorName { get; }
        public SelectorCommands(string selectorName, Commands2[] variantsCommand)
        {
            this.VariantsCommand = variantsCommand;
            this.SelectorName = selectorName;
        }
        public bool HaveCommandList(Commands2 c)
        {
            return VariantsCommand.Where(el => el == c).Count() > 0;
        }
    }

    public class Commands2 : List<TextCommand>
    {  
        private Dictionary<string, Commands2> allCommandsValue;
        private AllCommands allCommands;
        private AllRespWords allRespWords;
        public static Commands2 Empty { get { return new Commands2(); } }
        private Commands2() : base(){}
        public Commands2(TextCommand c1, TextCommand c2 = null, TextCommand c3 = null, TextCommand c4 = null, TextCommand c5 = null):base()
        {
            this.Add(c1);
            if (c2 is not null) this.Add(c2);
            if (c3 is not null) this.Add(c3);
            if (c4 is not null) this.Add(c4);
            if (c5 is not null) this.Add(c5);
        }
        public Commands2(BotBase botBase,string text, AllCommands allCommands, AllRespWords allRespWords):this()
        {
            this.allCommands = allCommands;
            this.allRespWords = allRespWords;

            try
            {
                string[] splitS = text.Split(' ');
                foreach (string s in splitS)
                {
                    if (allRespWords.Count > 0)
                    {
                        for (int i = 0; i < allRespWords.Count; i++)
                        {
                            if (allRespWords[i].Text.ToLower().IndexOf(s.ToLower()) != -1)
                            {
                                if (!this.IsHave(allRespWords[i].Id))
                                {
                                    this.Add(allCommands.ById(allRespWords[i].Id));
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        Logger.Error("Не удалось получить ни одной команды из базы данных");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static bool operator ==(Commands2 c1, Commands2 c2)
        {
            if (c1.Count == c2.Count)
            {
                for (int i = 0; i < c1.Count; i++)
                {
                    if(!c2.IsHave(c1[i].Text)) return false;
                }
                return true;
            }
            else return false;
        }
        public static bool operator !=(Commands2 c1, Commands2 c2)
        {
            return !(c1 == c2);
        }
        public bool IsHave(TextCommand command)
        {
            return this.Where(el => el.Text == command.Text).Count() > 0;
        }
        public bool IsHave(string textCommand)
        {
            return this.Where(el => el.Text == textCommand || el.Text == @"\" + textCommand).Count() > 0;
        }
        public bool IsHave(int idCommand)
        {
            return this.Where(el => el.Id == idCommand).Count() > 0;
        }
       
        public bool IsHaveOne(TextCommand command)
        {
            return this.Where(el => el.Text == command.Text).Count() == 1;
        }
        //Я думаю вот так использовать не стоит, потому что придется новую создавать новый объект Commands
        public bool IsHave(Commands commands)
        {
            for (int i = 0; i < commands.Count; i++)
            {
                if(this.Where(el => el.Text == commands[i].Text).Count()>0) return true;
            }
            return  false;
        }
        public bool IsHave(TextCommand[] commands)
        {
            for (int i = 0; i < commands.Length; i++)
            {
                if (this.Where(el => el.Text == commands[i].Text).Count() > 0) return true;
            }
            return false;
        }
    }
    public class Commands : List<TextCommand>
    {
        public static Commands Empty { get { return new Commands(); } }
        

        public static Commands AllComands(BotBase botBase)
        {
            DataTable t = botBase.GetAllResponceWord();
            Commands commands = new Commands();
            int c = t.Rows.Count;
            for (int i = 0; i < c; i++)
            {
                commands.Add(new TextCommand(Convert.ToInt32(t.Rows[i]["id"].ToString()), t.Rows[i]["textCommand"].ToString()));
            }
            return commands;
        }
        public bool Have(string com)
        {
            foreach (TextCommand c in this)
            {
                if (c.Text == com) return true;
            }
            return false;
        }
        public string GetAnsvere(string com, BotBase botBase)
        {
            if (Have(com)) return botBase.GetRandomAnsvere(com);
            else return "";
        }
        public string GetAllAnsvere(BotBase botBase)
        {
           
            string retStr = "";
            foreach (TextCommand c in this)
            {
                retStr += botBase.GetRandomAnsvere(c.Text) ?? "";
            }
            return retStr;
        }
        public string GetAllAnsvere(string[] commands, BotBase botBase)
        {
            string retStr = "";
            foreach (string s in commands)
            {
                retStr += GetAnsvere(s, botBase) + " ";
            }
            return retStr;
        }
        public static Commands SelectByText(string text, BotBase botBase, char split = ' ')
        {
            //if(text.Split(' ').Length>7) return  new Commands();
            try
            {
                Commands cmds = new Commands();
                string[] splitS = text.Split(' ');
                foreach (string s in splitS)
                {
                    DataTable responseWords = botBase.GetAllResponceWord();
                    if (responseWords.Rows.Count > 0)
                    {
                        TextCommand tCom;
                        bool find = false;
                        foreach (DataRow r in responseWords.Rows)
                        {
                            if (r[1].ToString().ToLower().IndexOf(s.ToLower()) !=-1 )
                            {
                                find = true;
                                tCom = botBase.GetCommand(r[0].ToString());
                                if (cmds.Where(el => el.Text == tCom.Text).Count() == 0)
                                {
                                    if (tCom is not null) cmds.Add(tCom);
                                }
                                break;
                            }
                        }
                        if (!find)
                        {
                            tCom = botBase.GetCommand(responseWords.Rows[0][0].ToString());
                            if (tCom is not null) cmds.Add(tCom);
                        }
                    }
                }
                return cmds;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public bool IsEmpty
        {
            get
            {
                return this == Empty;
            }
        }
        public bool IsAdmin { get { return Count == 1 && this[0].Text == "/admin"; } }
        public bool IsInfo { get { return Count == 1 && this[0].Text == "/info"; } }
        public bool IsStih { get { return Count == 1 && this[0].Text == "/stih"; } }
        public bool IsMem { get { return Count == 1 && this[0].Text == "/mem"; } }
        public bool IsAddMem
        {
            get
            {
                if (Count == 2)
                {
                    bool add = false;
                    bool mem = false;
                    foreach (TextCommand c in this)
                    {
                        if (c.Text == "/add") add = true;
                        if (c.Text == "/mem") mem = true;
                    }
                    return add && mem;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool IsGoldStih
        {
            get
            {
                if (Count == 2)
                {
                    bool gold = false;
                    bool stih = false;
                    foreach (TextCommand c in this)
                    {
                        if (c.Text == "/gold") gold = true;
                        if (c.Text == "/stih") stih = true;
                    }
                    return gold && stih;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool IsAddGoldStih
        {
            get
            {

                if (Count == 3)
                {
                    bool add = false;
                    bool gold = false;
                    bool mem = false;
                    foreach (TextCommand c in this)
                    {
                        if (c.Text == "/add") add = true;
                        if (c.Text == "/stih") mem = true;
                        if (c.Text == "/gold") gold = true;
                    }
                    return add && gold && mem;
                }

                if (Count == 2)
                {
                    bool add = false;
                    bool gold = false;
                    foreach (TextCommand c in this)
                    {
                        if (c.Text == "/add") add = true;
                        if (c.Text == "/gold") gold = true;
                    }
                    return add && gold;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool IsBible { get { return Count == 1 && this[0].Text == "/bible"; } }
        public bool IsStart { get { return Count == 1 && this[0].Text == "/start"; } }
        public bool IsVP { get { return Count == 1 && this[0].Text == "/vp"; } }
        public bool IsLeft { get { return Count == 1 && this[0].Text == "/left"; } }
        public bool IsRight { get { return Count == 1 && this[0].Text == "/right"; } }

        public bool IsHaveSearch
        {
            get { return Have("/search"); }
        }
        public bool IsHaveSearchVerse
        {
            get { return Have("/search") && Have("/stih"); }
        }
        public bool IsSearch { get { return Count == 1 && this[0].Text == "/search"; } }
        public bool IsSearchVerse
        {
            get
            {
                if (Count == 1) return this[0].Text == "/search";
                else
                {
                    if (Count == 2)
                    {
                        bool search = false;
                        bool stih = false;
                        foreach (TextCommand c in this)
                        {
                            if (c.Text == "/search") search = true;
                            if (c.Text == "/stih") stih = true;
                        }
                        return search && stih;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        public bool IsHaveAdmin
        {
            get 
            {
                foreach (TextCommand com in this)
                {
                    if (com.Text == "/admin") return true;
                }
                return false;
            }
        }
        public bool IsFirstAdmin { get { return this.Count >0 && this[0].Text == "/admin"; } }
        public bool IsOffAnswerOnAnon
        {
            get 
            {
                return
                    this.Count == 2 &&
                    (
                        (this[0].Text == "/answere" && this[1].Text == "/off")
                        ||
                        (this[1].Text == "/answere" && this[0].Text == "/off")
                    );
            }
        }


        public bool IsHowAreYou { get { return Count == 1 && this[0].Text == "/howAreYou"; } }
        public bool IsWhatDo { get { return Count == 1 && this[0].Text == "/whatDo"; } }
        public bool IsHaha { get { return Count == 1 && this[0].Text == "/hahaha"; } }
        public bool IsHelo { get { return Count == 1 && this[0].Text == "/helo"; } }
        public bool IsRest { get { return Count == 1 && this[0].Text == "/rest"; } }
        public bool IsHelp { get { return Count == 1 && this[0].Text == "/help"; } }
        public bool IsScreen { get { return Count == 1 && this[0].Text == "/screen"; } }
        public bool IsLessons { get { return Count == 1 && this[0].Text == "/lessons"; } }


        public bool IsAnonTeenOn
        {
            get
            {
                if (Count == 1) { return this[0].Text == "/onAnonTeen"; }
                if (Count == 2)
                {
                    return
                        this[0].Text == "/on" && this[1].Text == "/anon"
                        ||
                        this[1].Text == "/on" && this[0].Text == "/anon";
                }
                return false;
            }
        }
        public bool IsAnonTeenOff
        {
            get
            {
                if (Count == 1) { return this[0].Text == "/offAnonTeen"; }
                if (Count == 2)
                {
                    return
                        this[0].Text == "/off" && this[1].Text == "/anon"
                        ||
                        this[1].Text == "/off" && this[0].Text == "/anon"
                        ||
                        this[1].Text == "/off" && this[0].Text == "/onAnonTeen"
                        ||
                        this[1].Text == "/onAnonTeen" && this[0].Text == "/off";
                }
                return false;
            }
        }
        public bool IsAnonMessages
        {
            get
            {
                if (Count == 1) { return this[0].Text == "/anonMessages"; }
                if (Count == 2)
                {
                    return
                        this[0].Text == "/messages" && this[1].Text == "/anon"
                        ||
                        this[1].Text == "/messages" && this[0].Text == "/anon";
                }
                return false;
            }
        }
        public bool IsAnonMessages2
        {
            get
            {
                if (Count == 1) {

                    return this[0].Text == "/anonMessages2";
                }
               
                return false;
            }
        }
        public bool IsReportLesson
        {
            get 
            {
                if (this.Count == 2)
                {
                    return this[0].Text == "/lessons" && this[1].Text == "/report"
                        || this[1].Text == "/lessons" && this[2].Text == "/report";
                }
                return false;
            }
        }
        public bool IsReport
        {
            get
            {
                return this.Count == 1 && this[0].Text == "/report";
            }
        }


        public bool IsSpamOff { get { return Count == 2 && this[0].Text == "/no" && this[1].Text == "/spam"; } }
        public bool IsSpamOn { get { return Count == 2 && this[0].Text == "/yes" && this[1].Text == "/spam"; } }
        public bool IsHowWrkBut { get { return Count == 1 && this[0].Text == "/howworkbutton"; } }

        public static bool operator ==(Commands c1, Commands c2)
        {
            if (c1.Count != c2.Count) return false;
            else
            {
                for (int i = 0; i < c1.Count; i++)
                {
                    if (c1[i] != c2[i]) return false;
                }
                return true;
            }
        }
        public static bool operator !=(Commands c1, Commands c2)
        {
            return !(c1 == c2);
        }
        public static bool operator *(Commands c1, string command)
        {
            return c1.Have(command);
        }



        

        
        
        public bool IsHave(TextCommand command)
        {
            return this.Where(el => el.Text == command.Text).Count()>0;
        }
        public bool IsHaveOne(TextCommand command)
        {
            return this.Where(el => el.Text == command.Text).Count() == 0;
        }

        public string[] ToStringArray()
        {
            List<string> arr = new List<string>();
            foreach (TextCommand a in this)
            {
                arr.Add(a.Text);
            }
            return arr.ToArray();
        }
        public string ToString()
        {
            string ret = "";
            foreach (TextCommand com in this)
            {
                ret += com.Text;
            }
            return ret;
        }
    }
  
    public class ExecutorsList 
    {
        Sender sender;
        UserI currentUser;
        private Dictionary<string, Action> variants;
        private Message currentMessage;

        public ExecutorsList(Sender sender)
        {
            variants = new Dictionary<string, Action>
            {
                { "unknow", UNKNOW },
                { "start", Start },
                { "mem", GetMem },
                { "anonMessages",AnonMessages},
                { "report",Responce},
                { "info",INFO}
            };
            this.sender = sender;
        }

        public async void UNKNOW()
        {
            sender.SendAdminMessageAsync("неизвестная");
            //sender.SendMessageAsync("[]",currentUser);
        }
        public async void Start()
        {
            sender.SendAdminMessageAsync("старт");
        }
        public async void GetMem()
        {
            sender.SendAdminMessageAsync("мем");
        }
        public async void AnonMessages()
        {
            sender.SendAdminMessageAsync("анон сообщения");
        }
        public async void Responce()
        {
            sender.SendAdminMessageAsync("отзывы");
        }
        public async void INFO()
        {
            sender.SendAdminMessageAsync("информация");
        }


        public void Execute(string commandSelectorName,Message message,UserI currentUser)
        {
            currentMessage = message;
            if (variants.ContainsKey(commandSelectorName)) variants[commandSelectorName]();
            else { UNKNOW(); }
            this.currentUser = currentUser;
        }
        public void ExecuteUserState(UserI user, string commandSelectorName)
        {
            //if (user.InAnonMessage)
            //{
            //    //mesAnon = true;
            //    // если получили команду на отключение анонимности
            //    if (commandSelectorName =="anonTeenOff")
            //    {
            //        //Собственно отключаем анонимность
            //        if (user.CountAnonimMessage > 0)
            //        {
            //            sender.Spammer.SpamTeacherNewAnon(sender.Users);
            //        }
            //        users[user.Id].SetInAnon(false);
            //    }
            //    else
            //    {
            //        Console.WriteLine("Прислали анонимное сообщение");
            //        // Отправляем анонимное сообщение в базу данных
            //        botBase.AddAnonimMessage(user, text);
            //        users[user.Id].CountAnonimMessage++;
            //    }
            //}
            //if (user.InSetTeamLesson)
            //{
            //    botBase.SetTeamLesson(user.DateSetTeamLeson, text);
            //    //Answere(2041) СПАСИБО ЗА...
            //    sender.SendMessageAsync("Тема установлена на урок!" + Answere(2041) + " тему.", user);
            //    user.SetInTeacmSetter(new DateTime());
            //    sender.NextThreeSundays(user);
            //}
            //if (user.InTeacherSendMessage)
            //{
            //    if (commands.IsOffAnswerOnAnon)
            //    {
            //        // Онулирую
            //        user.TeacherSendMessageTurnOff();
            //        menus.SelectMenu(sender, "Отменена отправка сообщения ученику", user);
            //    }
            //    else
            //    {
            //        string nameAnswerer = "Ответ от (" + botBase.GetNameUser(user.Id) + ")" + Environment.NewLine;
            //        sender.SendMessageAsync(nameAnswerer + text, user, user.IdTeenAnonMessage);
            //        int lastId = botBase.SetNewAnswerAnon(text, user.Id);
            //        if (lastId == -1)
            //        {
            //            sender.SendAdminMessageAsync("неправильно сработал ответ на анонимку");

            //        }
            //        else
            //        {
            //            botBase.SetAnswerCurrentteen(user.IdTeenAnonMessage, lastId, user.IdDesiredTeacher);
            //            menus.SelectMenu(sender, "Сообщение отправлено", user);
            //            //Выключаю отправку ответа
            //            sender.CleanMenus(user);
            //            sender.FeedBackOnAnswereAnon(users[user.IdTeenAnonMessage], lastId);
            //            user.TeacherSendMessageTurnOff();
            //        }
            //    }
            //}
            //if (user.InOtherFeedBack)
            //{
            //    if (user.Privilege == UserPrivilege.Teen)
            //    {
            //        botBase.SetFeedback(user.Id, text, user.IdLessonOtherFeedBack);
            //        user.SetIdLessonOtherFeedBack();
            //        sender.SendMessageAsync(Answere("/thanks") + " отзыв!", user);
            //    }
            //    else
            //    {

            //        user.SetIdLessonOtherFeedBack();
            //        sender.SendMessageAsync(Answere("/thanks") + " отзыв! Но он не учтется, так как ты не ученик!", user);
            //    }

            //}
            //if (user.InRenaming)
            //{
            //    botBase.SetUniqName(user.IdRenameUser, text);
            //    sender.SendMessageAsync("Спасибо", user);
            //    user.SetIdRenameUser();
            //}
        }
    }

    public class TextCommand
    {
        int id;
        string textCommand;
        public string ToString()
        {
            return id+"|"+textCommand;
        }

        public int Id { get { return id; } }
        public string Text { get { return textCommand; } }

        public static bool operator ==(TextCommand c1, TextCommand c2)
        {
            return c1.Id == c2.Id;
        }
        public static bool operator !=(TextCommand c1, TextCommand c2)
        {
            return !(c1 == c2);
        }
        public TextCommand(int id, string textCommand)
        {
            this.id = id;
            this.textCommand = textCommand;
        }
    }
    public abstract class WordsCommand
    {
        internal string text = "";
        internal Words wrds;
        internal Words answers;
        internal Function func;

        public FunctionLecal Lecals { get; set; }
        public int FirstSelectedWord(string text = "")
        {
            //Обработка входящего текста, удаления ненужных символов
            if (text == "") text = this.text;
            text = TextHandler.DeleteChars(text);

            Words wInT = Words.Get(text);

            for (int i = 0; i < wInT.Count(); i++)
            {

                Word w = wInT[i];

                foreach (Word wI in wrds)
                {
                    if (wI.Text.ToLower() == text.ToLower())
                    {
                        return i;
                    }
                    if (w == wI)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        public static string operator &(WordsCommand w1, WordsCommand w2)
        {
            string text = (w1.FirstSelectedWord() != -1 ? w1.func.Command : "") + (w2.FirstSelectedWord() != -1 ? w2.func.Command : "");
            return text;
        }
        public static string operator &(string w1, WordsCommand w2)
        {

            string text = w1 + (w2.FirstSelectedWord() != -1 ? w2.func.Command : "");
            return text;
        }
        public string GetRandomAnswer(Random r)
        {
            int i = r.Next(0, answers.Count);
            return answers[i].Text;
        }
    }
    public class TextHandler
    {
        public static string DeleteChars(string text)
        {
            var charsToRemove = new string[] { "@", ",", ".", ";", "'", "/", "\\", "?" };
            foreach (var c in charsToRemove)
            {
                text = text.Replace(c, string.Empty);
            }
            return text;
        }

    }

}