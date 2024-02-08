using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Timers;
using Telegram.Bot.Types;
using TelegramBotClean.Bible;
using TelegramBotClean.Commandses;
using TelegramBotClean.MemDir;
using TelegramBotClean.Messages;
using TelegramBotClean.MessagesDir;
using TelegramBotClean.Userses;
using static TelegramBotClean.Data.Logger;
using User = TelegramBotClean.Userses.User;

namespace TelegramBotClean.Data
{
    public abstract class DataBaseWorker
    {
        protected DataTable allTables;
        protected DbConnection con;
        protected DbCommand command;
        protected DbDataAdapter adapter;
        protected string constring;
        bool readyToWork;
        //key - имя таблицы, value- информация о таблице
        Dictionary<string, DataTable> infoTables;

        public bool IsReady
        {
            get
            {
                return readyToWork;
            }
        }
        public DataTable AllTables { get { return allTables; } }
        public DataTable GetInfoTable(string name)
        {

            DataTable t = new DataTable();
            var a = infoTables.Where(el => el.Key.ToLower() == name.ToLower());
            var b = infoTables.Where(el => el.Key.ToLower() == name.ToLower()).FirstOrDefault(new KeyValuePair<string, DataTable>("", new DataTable()));
            return b.Value;
        }

        private void SetNullOnElements()
        {
            command = null;
            adapter = null;
        }

        /// <summary>
        /// Конструктор объекта работы с базой
        /// </summary>
        /// <param name="baseName"></param>
        public DataBaseWorker(string baseName)
        {
            readyToWork = false;
            if (CreateConnection(baseName))
            {
                readyToWork = CheckConnection();
            }
            UpdateTables();
            UpdateInfoTables();
        }
        /// <summary>
        /// Создание шаблона подключения к базе данных, по пути к фалу шифрованного 
        /// </summary>
        /// <param name="pathbaseInfo"> Путь к файлу с зашифрованной информацией базы данных</param>
        /// <returns> возвращает true если удалось создать строку подключения</returns>
        protected abstract bool CreateConnection(string baseName);


        /// <summary>
        /// Попытка подключения к базе данных
        /// </summary>
        /// <returns></returns>
        protected bool CheckConnection()
        {
            Info("Попытка подключиться к базе данных " + con.Database);
            try
            {
                con.Open();
                con.Close();
                Info("Выполнено");
                return true;
            }
            catch (Exception e)
            {
                con.Close();
                Error(e.Message, "Connection Checking");
                return false;
            }
        }
        /// <summary>
        /// Выполняет запрос в базу данных без возвращения данных
        /// </summary>
        /// <param name="sql">Запрос формата "INSERT INTO","UPDATE","CREATE" и подобные</param>
        /// <returns>Возвращает true если была изменена хотябы одна строка в базе данных</returns>
        protected bool Execute(string sql)
        {
            if (!IsReady)
            {
                Error("База не готова к выполнению!", "Execute");
                SetNullOnElements();
                return false;
            }
            Info("Попытка выполнить запрос: " + sql, "Execute", true);
            try
            {
                con.Open();
                command = con.CreateCommand();
                command.CommandText = sql ;
                bool executed = command.ExecuteNonQuery() > 0;
               
                con.Close();
                SetNullOnElements();
                InfoStop();

                return executed;
            }
            catch (Exception e)
            {
                con.Close();
                Error(e.Message, "Execute");
                SetNullOnElements();
                return false;
            }
        }
        /// <summary>
        /// Выполняет запрос в базу данных без с возвращением данных
        /// </summary>
        /// <param name="sql">Запрос формата "SELECT"</param>
        /// <returns>Возвращает данные в формате DataTable</returns>
        protected DataTable Select(string sql)
        {
            string met = new StackTrace().GetFrame(1).GetMethod().Name;
            if (!IsReady)
            {
                Error("База не готова к выполнению!", "Execute");
                SetNullOnElements();
                return new DataTable();
            }
            Info("Попытка выполнить запрос: " + sql + " База:" + con.Database, met);
            try
            {
                List<string> jsonFormatInfo = new List<string>();
                con.Open();
                command = con.CreateCommand();
                command.CommandText = sql;
                adapter = DbProviderFactories.GetFactory(con).CreateDataAdapter();
                adapter.SelectCommand = command;
                DataTable t = new DataTable();
                adapter.Fill(t);
                con.Close();
                Info("Удалась попытка выполнить запрос: ", met);
                InfoStop();
                if (t.Rows.Count != 0)
                {
                    SetNullOnElements();
                    return t;
                }
                else
                {
                    Error("По запросу ничего не найдено");
                    SetNullOnElements();
                    return new DataTable();
                }
            }
            catch (Exception e)
            {
                con.Close();
                Error(e.Message, met);
                return new DataTable();
            }
        }





        protected DataTable Select(string[] columns, string table, string where = "")
        {
            string columnsStr = string.Join(", ", columns);
            string WhereString = where == "" ? "" : "WHERE " + where;
            string sqlCommandString = string.Join(" ", new string[] { "Select", columnsStr, "FROM", table, WhereString, ";" });
            return Select(sqlCommandString);
        }
        protected DataTable Select(string column, string table, string where = "")
        {
            string WhereString = where == "" ? "" : "WHERE " + where;
            string sqlCommandString = string.Join(" ", new string[] { "Select", column, "FROM", table, WhereString, ";" });
            return Select(sqlCommandString);
        }
        protected DataTable SelectAllIn(string table, string where = "")
        {
            string WhereString = where == "" ? "" : "WHERE " + where;
            string sqlCommandString = string.Join(" ", new string[] { "Select * FROM", table, WhereString, ";" });
            return Select(sqlCommandString);
        }

        public string SelecteStringAllIn(string table, string where = "")
        {
            string WhereString = where == "" ? "" : "WHERE " + where;
            string sqlCommandString = string.Join(" ", new string[] { "Select * FROM", table, WhereString, ";" });
            return sqlCommandString;
        }
        public string SelecteStringOneColumn(string column, string table, string where = "")
        {
            string WhereString = where == "" ? "" : "WHERE " + where;
            string sqlCommandString = string.Join(" ", new string[] { "Select column FROM", table, WhereString, ";" });
            return sqlCommandString;
        }
        protected string[] ColumnOnTableAsStringArray(string table, string column, string where = "")
        {
            DataTable t = Select(column, table, where);
            if (t.Rows.Count == 0) return new string[0];
            else
            {
                return t.AsEnumerable().Select(s => s[column].ToString()).ToArray();
            }
        }
        protected string SelectFirstString(string table, string column, string where = "")
        {
            DataTable t = SelectAllIn(table, where);
            if (t.Rows.Count == 0) return "";
            else return t.Rows[0][column].ToString();
        }
        protected long SelectFirstLong(string table, string column, string where = "")
        {
            DataTable t = SelectAllIn(table, where);
            if (t.Rows.Count == 0) return -1;
            else
            {
                string i = t.Rows[0][column].ToString();
                try { return Convert.ToInt64(i); }
                catch (Exception ex)
                {
                    Logger.Error("SelectFirstLong получил не число!!!"+Environment.NewLine
                                +ex.Message);
                    return -1;
                }
            }
        }
        protected long SelectFirstLong(string sqlQuery,bool allIn = true)
        {
            DataTable t = null;
            if (allIn) t = SelectAllIn(sqlQuery);
            else t = Select(sqlQuery);
            if (t.Rows.Count == 0) return -1;
            else
            {
                string i = t.Rows[0][0].ToString();
                try { return Convert.ToInt64(i); }
                catch (Exception ex)
                {
                    Logger.Error("SelectFirstLong получил не число!!!" + Environment.NewLine
                                + ex.Message);
                    return -1;
                }
            }
        }

        protected string SelectOneString(string sql)
        {
            DataTable t = Select(sql);
            if (t is not null && t.Rows.Count > 0)
            {
                return t.Rows[0][0].ToString();
            }
            return null;
        }
        protected float SelectOneFloat(string sql)
        {
            string valSt = SelectOneString(sql);
            if (valSt is not null)
            {
                try { return float.Parse(valSt); }
                catch (FormatException)
                {
                    Error("не удалось получить float");
                    return -1;
                }
            }
            else return -1;
        }
        protected int SelectOneInt(string sql)
        {
            string valSt = SelectOneString(sql);
            if (valSt is not null)
            {
                try { return int.Parse(valSt); }
                catch (FormatException)
                {
                    Error("не удалось получить int");
                    return -1;
                }
            }
            else return -1;
        }
        protected float SelectSum(string sql, string column)
        {
            DataTable t = Select(sql);
            float count = 0;
            if (t is not null && t.Rows.Count > 0)
            {
                if (t.Columns.Contains(column))
                {
                    string typeS = t.Columns[column].DataType.ToString();
                    if (new string[] { "float", "fouble", "int", "uInt" }.ToList().IndexOf(typeS) != -1)
                    {
                        foreach (DataRow r in t.Rows)
                        {
                            count += float.Parse(r[column].ToString());
                        }
                    }
                }
                else
                {
                    return -1;
                }
            }
            return count;
        }
        protected int SelectSumRows(string sql)
        {
            DataTable t = Select(sql);
            if (t is not null && t.Rows.Count > 0) return t.Rows.Count;
            else return 0;
        }

        protected long InsertInto(string table, string[] columns, object[] values,bool returnIndex=false)
        {
            if (columns.Length != values.Length)
            {
                Error("Получено разное количество столбцов и значений", "InsertInto");
                return -1;
            }
            else
            {
                List<string> valuesValid = new List<string>();
                DataTable tableInfo = GetInfoTable(table);
                if (tableInfo.Rows.Count != 0)
                {
                    //сравнение типов данных
                    for (int i = 0; i < columns.Length; i++)
                    {
                        
                        // проверка, есть ли вообще такая колонка
                        EnumerableRowCollection<DataRow> InfoColumn = tableInfo.AsEnumerable()
                        .Where(el => el["COLUMN_NAME"].ToString().ToLower() == columns[i].ToLower());
                        if (InfoColumn.Count() == 0)
                        {
                            Error($"Колонка {columns[i]} не найдена", "InsertInto");
                            return -1;
                        }
                        // проверка соответствия типа
                        DataRow infoColumn = InfoColumn.First();
                        string realColumnType = infoColumn["DATA_TYPE"].ToString();

                        string typecol = "";
                        // если получили нул
                        if (values[i] is null)
                        {
                            values[i] = realColumnType switch
                            {

                                "nvarchar" => "-",
                                "bigint" => 0,
                                "datetime" => DateTime.Now,
                                "bit" => false,
                                _ => "NULL"
                            };
                            typecol = realColumnType;
                        }
                        else
                        {
                            typecol = values[i].GetType().ToString().ToLower() switch
                            {

                                "system.string" => "nvarchar",
                                "system.int32" => "bigint",
                                "system.int64" => "bigint",
                                "system.datetime" => "datetime",
                                "system.bool" => "bit",
                                _ => "nvarchar"
                            };
                        }
                         
                       
                        if (realColumnType != typecol)
                        {
                            Error($"Введенное значение {values[i]} не соответствует типу колонки {infoColumn["COLUMN_NAME"]}", "InsertInto");
                            return -1;
                        }
                        string validValue = realColumnType switch
                        {
                            "nvarchar" => $"N'{values[i] ?? "-"}'",
                            "bigint" => $"{values[i] ?? "0"}",
                            "datetime" => $"'{Convert.ToDateTime(values[i] ?? DateTime.Now.ToString()).ToString("yyyy-MM-dd HH:mm")}'",
                            "bit" => $"{values[i] ?? "0"}",
                            _ => "NULL"
                        };
                        valuesValid.Add(validValue);
                    }
                    string sqlQuery = $"Insert Into {table}({string.Join(",", columns)}) values({string.Join(",", valuesValid)})";
                    long idNewRow = 0;
                    if (Execute(sqlQuery))
                    {
                        idNewRow = SelectFirstLong($"SELECT IDENT_CURRENT('{table}')",false);
                    }
                    return idNewRow;

                }
                else
                {
                    Error($"Таблица ({table}) не найдена", "InsertInto");
                    return -1;
                }

                return -1;
            }
        }



        /// <summary>
        /// Полоучить все таблицы базы данных
        /// </summary>
        /// <returns></returns>
        public void UpdateTables()
        {
            allTables = new DataTable();
            DataTable t = Select("SELECT table_name, table_schema, table_type\r\nFROM information_schema.tables\r\nORDER BY table_name ASC;");
            allTables = t;
        }
        public void UpdateInfoTables()
        {
            infoTables = new Dictionary<string, DataTable>();
            for (int i = 0; i < allTables.Rows.Count; i++)
            {
                string tableName = allTables.Rows[i][0].ToString();
                infoTables.Add(tableName, Select($"select *\r\nfrom INFORMATION_SCHEMA.COLUMNS\r\nwhere TABLE_NAME='{tableName}'"));
            }
            return;
        }


        /// <summary>
        /// Получает список всех таблиц в базе данных
        /// </summary>
        /// <returns></returns>
        protected string[] GetnamesAllTable()
        {
            if (IsReady)
            {
                DataTable t = AllTables;
                List<string> names = new List<string>();
                foreach (DataRow r in t.Rows)
                {
                    names.Add(r[0].ToString());
                }
                return names.ToArray();
            }
            else
            {
                Error("");
                return new string[0];
            }
        }
    }
    public class MSSQLDBWorker : DataBaseWorker
    {
        public MSSQLDBWorker(string basePath) : base(basePath) { }

        protected override bool CreateConnection(string basePath)
        {
            constring = @$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""{basePath}"";Integrated Security=True;Connect Timeout=30";
            try
            {
                con = new SqlConnection(constring);
                return true;
            }
            catch (Exception e)
            {
                Error(e.Message, "CreateConnection");
                return false;
            }
            return false;
        }
    }
    public class BotDB : MSSQLDBWorker
    {
        Random r;

        public BotDB(Random r) : base(Config.PathToDBBot) { this.r = r; }

        //Users
        public DataTable GetAllUsers()
        {
            DataTable dbTable = SelectAllIn("users");

            return dbTable;

        }
        public User GetUser(long id)
        {
            DataTable t = SelectAllIn("users", "id = " + id);
            if (t.Rows.Count == 1)
            {
                DataRow r = t.Rows[0];
                return new User(r);
            }
            else
            {
                return null;
            }

        }
        public bool CreateUser(User user)
        {
            if (GetUser(user.Id) is null)
            {
                return Execute($"Insert Into users(Id,nick,lastName,firstName,lastStih,spam,privileges,uniqName,ban)" +
                    $"values" +
                    $"({user.Id},N'{user.NickName}',N'{user.Lastname}',N'{user.FirstName}','-',1,N'{user.TypeUser.Name}',N'{user.UniqName}',0)");
            }
            return false;
        }
        public Dictionary<long, string> GetUniqNamesWithId()
        {
            Dictionary<long, string> retValue = new Dictionary<long, string>();
            DataTable t = Select(new string[] { "id", "uniqName" }, "Users");
            for (int i = 0; i < t.Rows.Count; i++)
            {
                long id = Convert.ToInt64(t.Rows[i]["id"].ToString());
                string uniqName = t.Rows[i]["uniqName"].ToString();
                retValue.Add(id, uniqName);
            }
            return retValue;
        }

        //Commands
        public long GetIdCommandByName(Command c)
        {
            DataTable t = Select("id", "Commands", $"textCommand ='{c.Name}'");
            if (t.Rows.Count == 0) return -1L;
            else
            {
                try
                {
                    return Convert.ToInt64(t.Rows[0][0].ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка форматирования в получении айди команды GetIdCommandByName");
                    return -1L;
                }
            }
        }
        public long GetIdCommandByName(string c)
        {
            DataTable t = Select("id", "Commands", $"textCommand ='{c}'");
            if (t.Rows.Count == 0) return -1L;
            else
            {
                try
                {
                    return Convert.ToInt64(t.Rows[0][0].ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка форматирования в получении айди команды GetIdCommandByName");
                    return -1L;
                }
            }
        }

        //Answers
        public DataTable GetAllAnswersCommand()
        {
            return SelectAllIn("Ansvere_word");
        }
        public DataTable GetAllAnswersCommandAndName()
        {
            return Select("Select textCommand as name,word from Commands,Ansvere_word Where Ansvere_word.idCommand = Commands.id ");
        }
        public Dictionary<string, List<string>> GetAlAnswersComplect()
        {
            Dictionary<string, List<string>> retValues = new Dictionary<string, List<string>>();
            List<string> commandses;
            DataTable t = GetAllAnswersCommandAndName();
            for (int i = 0; i < t.Rows.Count; i++)
            {
                string nameCommand = t.Rows[i]["name"].ToString();
                string textAnswer = t.Rows[i]["word"].ToString();

                if (retValues.Where(el => el.Key == nameCommand).Count() != 0)
                {
                    retValues[nameCommand].Add(textAnswer);
                }
                else
                {
                    retValues.Add(nameCommand, new List<string>() { textAnswer });
                }
            }
            return retValues;
        }

        public string GetRandomAnswer(Command command)
        {
            long idcommandInBase = GetIdCommandByName(command);
            if (idcommandInBase == -1) return "CommandNotFound";
            string[] answers = ColumnOnTableAsStringArray("Ansvere_word", "word", "idCommand=" + idcommandInBase);
            return answers[r.Next(0, answers.Length)];
        }
        public string GetRandomAnswer(string command)
        {
            long idcommandInBase = GetIdCommandByName("/" + command);
            if (idcommandInBase == -1) return "CommandNotFound";
            string[] answers = ColumnOnTableAsStringArray("Ansvere_word", "word", "idCommand=" + idcommandInBase);
            return answers[r.Next(0, answers.Length)];
        }
        //public string[] GetRandomAnswers(string c1, string c2="", string c3="", string c4="", string c5="")
        //{

        //    List<string> retCommands = new List<string>();
        //    List<string> commandsLs = new List<string>() { "word = '"+c1+"'" };
        //    if (c2 != "") commandsLs.Add("word =  '" + c2 + "'");
        //    if (c3 != "") commandsLs.Add("word =  '" + c3 + "'");
        //    if (c4 != "") commandsLs.Add("word =  '" + c4 + "'");
        //    if (c5 != "") commandsLs.Add("word =  '" + c5 + "'");

        //    string idcommandInBase = GetIdCommandByName("/" + command);

        //    string[] answers = ColumnOnTableAsStringArray("Ansvere_word", "word", "idCommand=" + idcommandInBase);
        //    return answers[r.Next(0, answers.Length)];
        //}


        //MemMessages
        public DataTable GetAllMems()
        {
            return SelectAllIn("Messages", "command='" + Commands.Get("+ мем").Name + "'");
        }
        public long CreateMem(Mem mem)
        {
            return CreateMessage(mem.Message);
        }

        
        


        //AnonimNames
        public string[] GetAllAnonimNames()
        {
            return SelectAllIn("AnonimUserName")
                .AsEnumerable()
                .Select(el => el[1].ToString())
                .ToArray();
        }
        public long CreateAnonName()
        {
            return InsertInto("AnonimUserName",new string[] { "name" }, 
                new object[] { "Проверка"},true);
        }
        //AnonimMessages
        public DataTable GetAllAnon()
        {
            return SelectAllIn("AnonMessages,Messages", "AnonMessage.idMessage = Messages.id");
        }
        public long CreateAnonMessage(MessageI mes, string anonName, User teen, User WentTeacher = null)
        {
            long mesId = CreateMessage(mes,Commands.SelectCommands("anon"));
            if (mesId == -1) return -1;
            long idWent = WentTeacher is not null ? WentTeacher.Id : 0;
            return InsertInto("AnonMessages",
                new string[] {"idTeen","idWentTeacher","idAnswerTeacher","idMessage"},
                new object[] {teen.Id,}
                ); ;
            // потом сделаю
        }
        //Messages
        public long CreateMessage(MessageI mes,Commands commands = null)
        {
            string commandtext = commands is not null ? commands.ToString() : mes.Commands.ToString();
            return InsertInto(
                "Messages",
                new string[] { "chatId", "fileId", "text", "type", "command" }, 
                new object[] { 
                    mes.ChatId, 
                    mes.FileId, 
                    mes.Text, 
                    mes.Type.Name,
                    commandtext });

            

        }


        //GoldVerses
        public DataTable GetGoldVerses()
        {
            return SelectAllIn("Golds");
        }


       

    }
    public class BibleDB : MSSQLDBWorker
    {
        Random r;

        public BibleDB(Random r) : base(Config.PathToDBBible) { this.r = r; }
        public string[] BooksName()
        {
            return SelectAllIn("Books")
                .AsEnumerable()
                .Select(el => el["name_book"].ToString())
                .ToArray();
        }
        public string[] ShortBooksName()
        {
            return SelectAllIn("Books")
               .AsEnumerable()
               .Select(el => el["short_name"].ToString())
               .ToArray();
        }
        public Books GetBooks()
        {
            DataTable t = SelectAllIn("Books");
            List<Book> booksList = t.AsEnumerable()
                .Select(el =>
                new Book(
                    el["name_book"].ToString(),
                    el["short_name"].ToString(),
                    Convert.ToInt64(el["id"].ToString()),
                    0)                    
                ).ToList();
            return new Books(booksList);

        }
        public Chapters GetChapters()
        {
            DataTable t = SelectAllIn("Chapters");
            List<Chapter> chapters = t.AsEnumerable()
                .Select(el =>
                new Chapter(
                    Convert.ToInt64(el["id"].ToString()),
                    Convert.ToInt64(el["book_id"].ToString()),
                    Convert.ToInt64(el["number_chapter"].ToString()),
                    Convert.ToInt64(el["count_verses"].ToString())
                    )
                ).ToList() ;
            return new Chapters(chapters);
        }
        public Verses GetVerses(Books books,Chapters chapters)
        {
            DataTable t = SelectAllIn("Verses");
            List<Verse> verses = t.AsEnumerable()
                .Select(el =>
                new Verse(
                     Convert.ToInt64(el["verse_number"].ToString()),
                     Convert.ToInt64(el["chapter_id"].ToString()),
                     Convert.ToInt64(el["id"].ToString()),
                     el["text_verse"].ToString(),
                     books,
                     chapters
                    )
                ).ToList();
            return new Verses(verses);
        }
        public int GetVerse(AddressVerse address)
        {
            return 0;
        }

    }
}
