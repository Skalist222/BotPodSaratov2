using System.Data;
using PodrostkiBot.Bible;
using PodrostkiBot.DataBase.Engine;
using Telegram.Bot.Types;

namespace PodrostkiBot.Users
{
    public class UserI
    {
        UserPrivilege privilege;
        long id;
        string name;//ник
        string firstname;//Имя
        string lastName;//Фамилия
        string uniqName;//Уникальное имя, которое есть только в боте
        bool userFirstStart;
        long idRenameUser;
        long idDesiredTeacher;// Id учителя которому адресовано анонимное письмо
        bool ban = false;

        string inBible;//"в библии"сборник адреса(постепенно собирается)
        string bookInBible;//Название книги библии (только в библии)
        string chapterInBook;//номер главы в выбранной книге (только в библии)
        string verseInChapter;// Номер стиха в выбранной главы (только в библии)
        Verse lastVerse = null;// Последний увиденный пользователем стих библии
        Verse lastAddGold = null;// Последний стих добавленный в золотые стихи

        bool spam;//Включение/отключение спама(в этом боте не работает)
        bool inAnonMessage;//Если ученик в режиме анонимного сообщения
        DateTime dateSetTeamLeson;//Дата урока, на который ставится тема(необходимо при установке темы на урок)
        long idTeenSendAnswere;//Айди подростка, которому отправляем ответ
        long idLessonOtherFeedBack;// Айди урока, на который ученик будет устанавливать свой вариант фидбэка

        int countExpletive = 0;// Количество ругательств сказанных пользователем
       

        public Message MenuMessage { get; set; }
        public Message YesNoMessage { get; set; }


        public string HashAnonimMessage { get; set; }
        public int CountAnonimMessage { get; set; }
        public long IdTeenAnonMessage { get { return idTeenSendAnswere; } }
        public long IdLessonOtherFeedBack{ get { return idLessonOtherFeedBack; } }
        public string LastAddedPhoto { get; set; }


        public bool AddedMem { get; set; }
        public long Id { get { return id; } }
        public string Name { get { return name; } }
        public string Firstname { get { return firstname; } }
        public string LastName { get { return lastName; } }
        public string UniqName { get { return uniqName; } }
        public bool Banned { get { return ban; } }

        //idDesiredTeacher
        public long IdDesiredTeacher { get { return idDesiredTeacher; } }
        public string BookInBible { get { return bookInBible; } }
        public string ChapterInBook { get { return chapterInBook; } }
        public string VerseInChapter { get { return verseInChapter; } }
        public DateTime DateSetTeamLeson { get { return dateSetTeamLeson; } }
        public UserPrivilege Privilege { get { return privilege; } }

        public bool InAnonMessage { get { return inAnonMessage; } }
        public bool InSetTeamLesson { get { return dateSetTeamLeson != new DateTime(); } }
        public bool InBible { get { return inBible != "-"; } }
        public string InBibleParametr { get { return inBible; } }
        public bool InOtherFeedBack { get { return idLessonOtherFeedBack!=0; } }
        public bool InTeacherSendMessage { get { return idTeenSendAnswere != 0; } }
        public bool InRenaming { get { return idRenameUser != 0; } }
        public long IdRenameUser { get { return idRenameUser; } }

        public string AddressInBible { get { return $"{bookInBible} {chapterInBook}:{verseInChapter}"; } }
        public bool Spam
        {
            get { return spam; }
        }
        
        public Verse LastVerse
        {
            get { return lastVerse; }
            
        }
        public string DetectName { get {
                if (UniqName != "") return UniqName;
                if (Firstname != "") return Firstname;
                if (LastName != "") return LastName;
                return Name;
            } }


        public string ToString()
        {
            return $"({id}){DetectName}";
        }
        public void SetLastVerse(Verse verse,BotBase botBase = null)
        {
                lastVerse = verse;
                if (botBase is not null)
                    botBase.RedactLastStih(Id, verse.AddressText);
            
        }
        


        public void SetPrivilege(UserPrivilege priv)
        {
            privilege = priv;
            SetInAnon(false);// Обязательно обрубаем анонимность пользователю
        }
        public void SetInAnon(bool on=true)
        {
            inAnonMessage = on;
            if(!on) CountAnonimMessage = 0;
        }
        public void SetInTeacmSetter()
        {
            dateSetTeamLeson = new DateTime();
        }
        public void SetInTeacmSetter(DateTime on)
        {
            dateSetTeamLeson = on;
        }
        public void SetInBible(string val)
        {
            if (val == "-")
            {
                bookInBible = null;
                chapterInBook = null;
                verseInChapter = null;
            }
            inBible = val;
        }
        public void SetBookInBible(string shortName)
        {
            bookInBible = shortName;
        }
        public void SetChapterInBook(string chapterNumber)
        {
            chapterInBook = chapterNumber;
        }
        public void SetVerseInChapter(string verseNumber)
        {
            verseInChapter = verseNumber;
        }
        public void SetIdLessonOtherFeedBack(long idLesson=0)
        {
            idLessonOtherFeedBack = idLesson;
        }
        public void SetIdRenameUser(long id=0)
        {
            idRenameUser = id;
        }
        public void SetDesiredTeacher(long id = 0)
        {
            idDesiredTeacher = id;
        }
        public bool SetBan(BotBase botBase,bool ban=false)
        {
            if (botBase.SetBanUser(id, ban))
            {
                this.ban = ban;
                return true;
            }
            else
            {
                return false;
            }
                
            
            
        }


        public void TeacherSendMessageTurnOn(long idTeen)
        {
            idTeenSendAnswere = idTeen;
        }
        public void TeacherSendMessageTurnOff()
        {
            idTeenSendAnswere = 0;
        }


        public UserI(long id, string name, string firstname, string lastName, BibleWorker bw, UserPrivilege privilege,string uniqName="", string addmem = "False", string lastStihAddress = "-", string inBible = "-", bool spam = true,bool ban = false)
        {
            this.id = id;
            this.name = name;
            this.firstname = firstname;
            this.lastName = lastName;
            this.spam = spam;
            this.privilege = privilege;
            this.ban = ban;
            AddedMem = addmem.ToLower() == "true";
            SetInTeacmSetter(new DateTime());
            SetIdRenameUser();
            this.uniqName = uniqName;
            if (lastStihAddress != "-")
                SetLastVerse(bw.GetVerse(lastStihAddress));
        }
        public UserI(DataRow row, BibleWorker bw)
        {
            id = long.Parse(row["id"].ToString());
            name = row["nick"].ToString();
            firstname = row["firstName"].ToString();
            lastName = row["lastName"].ToString();
            ban = row["ban"].ToString() == "true" ? true : false;
            string lastStihAddress = row["lastStih"].ToString();
            spam = row["spam"].ToString() == "True";
            uniqName = row["uniqName"].ToString();
            privilege = UserPrivilege.SelectPriv(row["privileges"].ToString());
            MenuMessage = null;
            idLessonOtherFeedBack = 0;
            userFirstStart = false;
            CountAnonimMessage = 0;
            SetDesiredTeacher();// ставим пользователю ожидаемого 
            TeacherSendMessageTurnOff();// Ставим положение, что это не ответ ученику
            if (lastStihAddress != "-")
            {
                Verse v = bw.GetVerse(lastStihAddress);
                SetLastVerse(v);
            }
        }
        /// <summary>
        /// Стартовое создание пользователя, человек автоматом идет в привилегии ученика
        /// </summary>
        /// <param name="user"></param>
        /// <param name="bw"></param>
        public UserI(User user, BibleWorker bw, BotBase botBase) : this(user.Id, user.Username, user.FirstName, user.LastName, bw,UserPrivilege.Teen)
        {
            userFirstStart = true;
        }
        public static UserI Empty { get { return new UserI(0, "", "", "", new BibleWorker(), UserPrivilege.Teen); } }


        // Нужго для счета ругательств сказанных пользователем(возможно пригодится)
        public bool Expletive()
        {
            if (countExpletive == 10) return true;
            else { countExpletive++; return false; }
        }
      
    }
    public class UserList : List<UserI>
    {
        public UserList(DataTable table, BibleWorker bw,BotBase botBase)
        {
            foreach (DataRow r in table.Rows)
            {
                base.Add(new UserI(r, bw));
            }
        }
        public UserList(BibleWorker bw, BotBase botBase) :
            this(
                botBase.GetAllUsers(), bw, botBase)
        { }
        public void Add(UserI user, BotBase botBase)
        {
            // добавление пользователя в список
            base.Add(user);
            // добавление пользователя в базу данных
            botBase.AddUser(user);
        }
        public UserI? this[long id]
        {
            get
            {
                foreach (UserI u in this)
                {
                    if (u.Id == id)
                        return u;
                }
                return null;
            }
        }
    }
    public class UserPrivilege
    {
        string name;
        string description;
        string ruName;
        public string Name { get { return name; } }
        public string Description { get { return description; } }
        public string RuName { get { return ruName; } }




        public UserPrivilege(string name, string description,string ruName)
        {
            this.name = name;
            this.description = description;
            this.ruName = ruName;
        }



        public static bool operator ==(UserPrivilege p1, UserPrivilege p2)
        {
            if (p1 is null && p2 is null) return true;
            if (p1 is null && p2 is not null) return false;
            if (p1 is not null && p2 is null) return false;
            if (p1 is not null && p2 is not null)
            {
                return p1.Name == p2.Name;
            }
            return false;
        }
        public static bool operator !=(UserPrivilege p1, UserPrivilege p2)
        {
            return !(p1 == p2);
        }

        /// <summary>
        /// Автоматическое определение привелегий
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static UserPrivilege SelectPriv(string name)
        {
            name = name.Replace(" ","");// Удаляю ненужные пробелы из имени типа
            if (name == UserPrivilege.Admin.Name) return UserPrivilege.Admin;
            if (name == UserPrivilege.Teen.Name) return UserPrivilege.Teen;
            if (name == UserPrivilege.Teacher.Name) return UserPrivilege.Teacher;
            return null;
        }
        public static UserPrivilege SelectPriv(int num)
        {
           
            if (num == 7) return UserPrivilege.Admin;
            if (num == 2) return UserPrivilege.Teen;
            if (num == 3) return UserPrivilege.Teacher;
            return null;
        }
        /// <summary>
        /// Администратор
        /// </summary>
        public static UserPrivilege Admin { get {
                return new UserPrivilege("admin",
            "Есть возможность вызывать команду администратора ",
            "Админ");
            } }
        /// <summary>
        /// Преподаватель
        /// </summary>
        public static UserPrivilege Teacher
        {
            get
            {
                return new UserPrivilege("teacher",
                "Кнопка вывода отчета\r\n" +
                "Отображение последних анонимных сообщений\r\n" +
                "Возможность написать ребенку от имени бота (анонимно)\r\n",
                "Учитель");
            }
        }
        /// <summary>
        /// Ученик
        /// </summary>
        public static UserPrivilege Teen
        {
            get
            {
                return new UserPrivilege("teen",
                "Стандартное управление пользователя",
                "Ученик");
            }
        }
    }
}