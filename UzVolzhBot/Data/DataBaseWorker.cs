using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using Log = TelegramBotClean.Data.Logger;


namespace TelegramBotClean.Data
{
    public abstract class DataBaseWorker
    {
        
        protected DbConnection con;
        protected DbCommand command;
        protected DbDataAdapter adapter;
        protected string constring;
        bool readyToWork;

        public bool IsReady
        {
            get
            {
                return readyToWork;
            }
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
            Log.Info("Попытка подключиться к базе данных " + con.Database);
            try
            {
                con.Open();
                con.Close();
                Log.Info("Выполнено");
                return true;
            }
            catch (Exception e)
            {
                con.Close();
                Log.Error(e.Message, "Connection Checking");
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
                Log.Error("База не готова к выполнению!", "Execute");
                SetNullOnElements();
                return false;
            }
            Log.Info("Попытка выполнить запрос: " + sql, "Execute", true);
            try
            {
                con.Open();
                Log.Info("Открыл подключение: " + sql, "Execute", true);
                command = con.CreateCommand();
                command.CommandText = sql;
                int count = command.ExecuteNonQuery();
                Log.Info("Выполнил запрос: " + sql, "Execute", true);
                con.Close();

                Log.Info("Удалось выполнить запрос: " + sql, "Execute", true);
                SetNullOnElements();
                Log.Info("Закрыл подключение/очистил кэш: " + sql, "Execute", true);
                return count > 0;
            }
            catch (Exception e)
            {
                con.Close();
                Log.Error(e.Message, "Execute");
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
                Log.Error("База не готова к выполнению!", "Execute");
                SetNullOnElements();
                return null;
            }
            Log.Info("Попытка выполнить запрос: " + sql + " База:" + con.Database, met);
            try
            {
                List<string> jsonFormatInfo = new List<string>();
                con.Open();
                Log.Info("Открыл подключение ", met);
                command = con.CreateCommand();
                command.CommandText = sql;
                adapter = DbProviderFactories.GetFactory(con).CreateDataAdapter();
                Log.Info("Сформировал запрос: ", met);
                adapter.SelectCommand = command;
                DataTable t = new DataTable();
                adapter.Fill(t);
                Log.Info("Выполнил запрос: ", met);
                con.Close();
                Log.Info("Закрыл подключение: ", met);
                Log.Info("Удалась попытка выполнить запрос: ", met);
                Log.InfoStop();
                if (t.Rows.Count != 0)
                {
                    SetNullOnElements();
                    return t;
                }
                else
                {
                    Log.Error("По запросу " + sql + " ничего не найдено");
                    SetNullOnElements();
                    return new DataTable();
                }
            }
            catch (Exception e)
            {
                con.Close();
                Log.Error(e.Message, met);
                return new DataTable();
            }
        }
        
        protected DataTable Select(string[] columns, string table, string where = "")
        {
            string columnsStr = string.Join(", ",columns);
            string WhereString = where == "" ? "" : "WHERE " + where;
            string sqlCommandString = string.Join(" ", new string[] { "Select", columnsStr,"FROM", table, WhereString, ";" });
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
            string sqlCommandString = string.Join(" ", new string[] { "Select * FROM", table,  where, ";" });
            return Select(sqlCommandString);
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
                    Log.Error("не удалось получить float");
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
                    Log.Error("не удалось получить int");
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

        /// <summary>
        /// Полоучить все таблицы базы данных
        /// </summary>
        /// <returns></returns>
        protected DataTable GetTables()
        {
            DataTable t = new DataTable();
            t = Select("SHOW TABLES");
            return t;
        }
        /// <summary>
        /// Получает список всех таблиц в базе данных
        /// </summary>
        /// <returns></returns>
        protected string[] GetnamesAllTable()
        {
            if (IsReady)
            {
                DataTable t = GetTables();
                List<string> names = new List<string>();
                foreach (DataRow r in t.Rows)
                {
                    names.Add(r[0].ToString());
                }
                return names.ToArray();
            }
            else
            {
                Log.Error("");
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
                Log.Error(e.Message, "CreateConnection");
                return false;
            }
            return false;
        }
    }
    public class BotDB : MSSQLDBWorker
    {
        public BotDB() : base(Config.PathToDBBot) { }
        public DataTable GetAllUsers()
        {
            return SelectAllIn("users");
        }
    }
    internal class Logger
    {
        public static bool FileLogEnabled = false;
        public static bool segmentStart = false;
        public static void Error(string msg, string method = "")
        {

            Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!ERROR!!!!!!!!!!!!!!!!!!!!!!!!");
            Debug.WriteLine(msg);
            Debug.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            InfoStop();

            try { File.AppendAllText("log.txt", "error|" + DateTime.Now + "|" + msg + "!!!!!!!" + Environment.NewLine); }
            catch { }

        }
        public static void Info(string msg, string method = "", bool segment = true)
        {
            if (!segment)
            {
                if (!segmentStart) Debug.WriteLine("````````````````````````info```````````````````````");
                Debug.WriteLine(msg);
                if (method != "") Debug.WriteLine("In method " + method);
                Debug.WriteLine("___________________________________________________");
                segmentStart = false;
            }
            else
            {
                if (!segmentStart)
                {
                    segmentStart = true;
                    Debug.WriteLine("````````````````````````info```````````````````````");
                    if (method != "") Debug.WriteLine("In method " + method);
                }
                Debug.WriteLine(msg);
            }
            try
            {
                File.AppendAllText("log.txt", "info|" + DateTime.Now + "|" + msg + "" + Environment.NewLine);
            }
            catch { }

        }
        public static void InfoStop()
        {
            segmentStart = false;
            Debug.WriteLine("___________________________________________________");
        }
    }
}
