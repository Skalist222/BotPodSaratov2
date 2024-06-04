using System.Data;
using System.Data.OleDb;


using static PodrostkiBot.Configure.ConstData;
using System.Diagnostics;

using PodrostkiBot.Bible;
using System.Net;
using System.Data.SqlClient;
using PodrostkiBot.Users;
using System.ComponentModel.Design;
using PodrostkiBot.Text;
using PodrostkiBot.App;
using Telegram.Bot.Exceptions;
using PodrostkiBot.Messages;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace PodrostkiBot.DataBase.Engine
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
            Logger.Info("Попытка подключиться к базе данных " + con.Database);
            try
            {
                con.Open();
                con.Close();
                Logger.Info("Выполнено");
                return true;
            }
            catch (Exception e)
            {
                con.Close();
                Logger.Error(e.Message);
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
                Logger.Error("База не готова к выполнению!");
                SetNullOnElements();
                return false;
            }
            Logger.Info("Попытка выполнить запрос: " + sql, true);
            try
            {
                con.Open();
                command = con.CreateCommand();
                command.CommandText = sql;
                bool executed = command.ExecuteNonQuery() > 0;

                con.Close();
                SetNullOnElements();
                Logger.InfoStop();

                return executed;
            }
            catch (Exception e)
            {
                con.Close();
                Logger.Error(e.Message);
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
                Logger.Error("База не готова к выполнению!");
                SetNullOnElements();
                return new DataTable();
            }
            Logger.Info("Попытка выполнить запрос: " + sql + " База:" + con.Database);
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
                Logger.Info("Удалась попытка выполнить запрос: ");
                Logger.InfoStop();
                if (t.Rows.Count != 0)
                {
                    SetNullOnElements();
                    return t;
                }
                else
                {
                    Logger.Error(sql + Environment.NewLine + "По запросу ничего не найдено");
                    SetNullOnElements();
                    return new DataTable();
                }
            }
            catch (Exception e)
            {
                con.Close();
                Logger.Error(e.Message);
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
                    Logger.Error("SelectFirstLong получил не число!!!" + Environment.NewLine
                                + ex.Message);
                    return -1;
                }
            }
        }
        protected long SelectFirstLong(string sqlQuery, bool allIn = true)
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
        protected bool SelectExist(string table, string where)
        {
            return SelectAllIn(table, where).Rows.Count != 0;

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
                    Logger.Error("не удалось получить float");
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
                    Logger.Error("не удалось получить int");
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

        protected long InsertInto(string table, string[] columns, object[] values, bool returnIndex = false)
        {
            if (columns.Length != values.Length)
            {
                Logger.Error("Получено разное количество столбцов и значений");
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
                            Logger.Error($"Колонка {columns[i]} не найдена");
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
                            Logger.Error($"Введенное значение {values[i]} не соответствует типу колонки {infoColumn["COLUMN_NAME"]}");
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
                        idNewRow = SelectFirstLong($"SELECT IDENT_CURRENT('{table}')", false);
                    }
                    return idNewRow;

                }
                else
                {
                    Logger.Error($"Таблица ({table}) не найдена");
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
                Logger.Error("");
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
                Logger.Info($"{con.Database}Подключение проверено!", false, ConsoleColor.Cyan);
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return false;
            }
            return false;
        }
    }

    internal interface IBase
    {
        DataTable? Select(string sql);
        bool Execute(string sql);
    }
    internal interface IBotBase : IBase
    {
        public string? GetRandomIdMem();
        public bool CreateNewMem(string idMem);
        public DataTable? SelectAllUsers();
        public bool HaveUser(long id);
        public bool CreateUser(UserI user);
        public bool RedactLastStih(long idUser, string newStih);
        public bool RedactLastGold(long idUser, string idLastGold);
        public DataTable GetAllGoldVerses();
        public bool CreateGold(Verse s);
        public TextCommand GetCommand(string idCommand);
        public string GetStringsResponce(string command, char split = ' ');
        public string GetStringsAnsveres(string command, char split = ' ');
        public string? GetRandomAnswere(string command);
      
    }
    internal interface IBibleBase
    {
        public int CountChaptersInBook(int idBook);

        public string GetTextStihByAddress(string address);
    }

  
   
    public abstract class SQL : IBase
    {
        protected string conStr;
        protected SqlConnection con;
        public readonly bool BaseReady;
        public SQL(string basePath)
        {
            //BaseReady
            //= "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + PathDB
            conStr = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={basePath};Integrated Security=True;Connect Timeout=30";
            con = new SqlConnection(conStr);
            try
            {
                con.Open();
                con.Close();
                BaseReady = true;
            }
            catch (ApiRequestException)
            {
                AppWorker.Restart();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                try { con.Close(); }
                catch (Exception e2)
                {
                    Console.WriteLine(e.Message + " ВТОРОЙ!");
                }
                BaseReady = false;
            }
        }

        public DataTable? Select(string sql)
        {
            try
            {
                con.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(sql, con);
                DataTable table = new DataTable();
                adapter.Fill(table);
                con.Close();
                return table;
            }
            catch (ApiRequestException)
            {
                return null;
                AppWorker.Restart();
            }
            catch (Exception e)
            {
                Debug.WriteLine("SELECT ERROR!!!!!! DATABASE");
                Debug.WriteLine(e.Message);
                try { con.Close(); } catch { }
                return null;
            }

        }
        public bool Execute(string sql)
        {
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand(sql, con);
                com.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (ApiRequestException)
            {
                return false;
                AppWorker.Restart();
            }
            catch (Exception e)
            {
                Debug.WriteLine("EXECUTE ERROR!!!!!! DATABASE");
                Debug.WriteLine("Запрос:"+sql);
                Debug.WriteLine("");
                Debug.WriteLine(e.Message);
                try { con.Close(); } catch { }
                return false;
            }
        }
        public bool Execute(string[] sqls)
        {
            try
            {
                con.Open();
                SqlCommand com = new SqlCommand("", con);
                foreach (string sql in sqls)
                {
                    com.CommandText = sql;
                    com.ExecuteNonQuery();
                }
                con.Close();
                return true;
            }
            catch (ApiRequestException)
            {
                return false;
                AppWorker.Restart();
            }
            catch (Exception e)
            {
                Debug.WriteLine("MultiEXECUTE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                Debug.WriteLine(e.Message);
                try { con.Close(); } catch { }
                return false;
            }
        }
    }
    public class BotBase : MSSQLDBWorker
    {
        // Правила нейминга в этом документе:
        // GET- Если нужно что-то получить
        // ADD- Если что то добавляем в таблицу
        // SET- Если редактируем какую-то строку
        // HAVE- Определяем наличие в базе
        public BotBase() : base(BotDB) { }

        //Memases
        public string? GetRandomIdMem(Random r)
        {
            DataTable t = Select($"Select IdPhoto From Memases");
            if (t.Rows.Count > 0)
            {
                int selectedId = r.Next(0, t.Rows.Count);
                return t.Rows[selectedId][0].ToString();
            }
            else return null;
        }
        public bool AddNewMem(string idMem)
        {
            return Execute($"Insert Into Memases(idPhoto) values(N'{idMem}')");
        }
        
        //Users
        public bool SetSpam(long userId, bool spam)
        {
            string spamBool = spam ? "-1" : "0";
            return Execute($"UPDATE Users SET spam = {spamBool}  WHERE Id = '{userId}'");
        }
        public DataTable? GetAllUsers()
        {
            DataTable t = Select("Select * From users");
            return t;
        }
        public DataTable? GetAllTeachers()
        {
            string textQuery = $"Select * From users Where privileges = '{UserPrivilege.Teacher.Name}'";
            DataTable t = Select(textQuery);
            return t;
        }
        public bool HaveUser(long id)
        {
            try
            {
                DataTable t = Select("Select * from users where id='" + id + "'");
                if (t is null) return false;
                return t.Rows.Count == 1;
            }
            catch (ApiRequestException)
            {
                return false;
                AppWorker.Restart();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public string GetNameUser(long idUser)
        {
            DataTable t = Select($"Select * From Users Where id = {idUser}");
            if (t is not null && t.Rows.Count > 0)
            {
                if (t.Rows[0]["uniqName"] != "") return t.Rows[0]["uniqName"].ToString();
                else
                if (t.Rows[0]["lastName"] != "") return t.Rows[0]["lastName"].ToString();
                else
                if (t.Rows[0]["firstName"] != "") return t.Rows[0]["firstName"].ToString();
                else
                if (t.Rows[0]["nick"] != "") return t.Rows[0]["nick"].ToString();
            }
            return "noName";
        }

        public bool AddUser(UserI user)
        {
            try
            {
                string lastStih = user.LastVerse is not null ? user.LastVerse.AddressText : "-";
                string add = user.AddedMem ? 1 + "" : 0 + "";
                string commandS = $"Insert Into Users(" +
                    $"Id," +
                    $"nick," +
                    $"lastName," +
                    $"firstName," +
                    $"lastStih," +
                    $"spam," +
                    $"privileges," +
                    $"uniqName," +
                    $"ban," +
                    $"deleted) " +
                    $"values(" +

                /**/$"N'{user.Id}'," +
                /**/$"N'{user.Name}'," +
                /**/$"N'{user.LastName}'," +
                /**/$"N'{user.Firstname}'," +
                /**/$"N'{lastStih}'," +
            /*spam*/$"1," +
                /**/$"'{UserPrivilege.Teen.Name}'," +
        /*uniqName*/$"''," +
             /*ban*/$"0," +
         /*deleted*/$"0)";
                return Execute(commandS);
            }
            catch (ApiRequestException)
            {
                AppWorker.Restart();
                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine("___!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                Debug.WriteLine(e.Message);
                Debug.WriteLine("___!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                return false;
            }
        }
        public bool RedactLastStih(long idUser, string newStih)
        {
            try
            {
                string commandS = "Update users Set lastStih = N'" + newStih + "' where id ='" + idUser + "'";
                return Execute(commandS);
            }
            catch (ApiRequestException)
            {
                AppWorker.Restart();
                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public bool RedactLastGold(long idUser, string idLastGold)
        {
            try
            {
                string commandS = "Update users Set lastAddGold = " + idLastGold + " where id ='" + idUser + "'";
                return Execute(commandS);
            }
            catch (ApiRequestException)
            {
                AppWorker.Restart();
                return false;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public bool SetPrivileges(long iduser, UserPrivilege priv)
        {
            if (!HaveUser(iduser)) return false;
            if (priv is not null)
                return Execute($"Update Users Set privileges = N'{priv.Name}' WHERE id = {iduser}");
            else return false;
             
        }
        public bool SetUniqName(long iduser, string uniqName)
        {
            if (!HaveUser(iduser)) return false;
            return Execute($"Update Users Set uniqName = N'{uniqName}' WHERE id = {iduser}");
        }
        public bool SetBanUser(long idUser,bool ban=true)
        {
            string banStr = ban ? "1" : "0";
            string query = $"UPDATE Users SET ban = {banStr} WHERE id ={idUser}";
            return Execute(query);
        }
        //Goldverses
        public DataTable GetAllGoldVerses()
        {
            DataTable t = Select("Select * From Golds");
            return t;
        }
        public DataTable GetGoldVerseById(int id)
        {
            DataTable t = Select($"Select * From Golds Where id ={id}");
            return t;
        }
        public string AddGold(Verse s)
        {
            try
            {
                DataTable t = GetAllGoldVerses();
                for (int i = 0; i < t.Rows.Count; i++)
                {
                    if (t.Rows[i]["textI"] == s.Text)
                    {
                        return "SEL";
                    }
                }
                string commandS = $"Insert into Golds (address,textI) values(N'{s.AddressText}',N'{s.Text}');";
                return Execute(commandS)?"OK":"ERR";
            }
            catch (ApiRequestException)
            {
                AppWorker.Restart();
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return "ERR";
            }
        }
        public bool DeleteGoldverse(int idGoldVerse)
        {
            return Execute($"Delete From Golds Where id = {idGoldVerse}");
        }
        public bool DeleteGoldVerses(int[] idGoldverses)
        {
            if (idGoldverses.Length == 0) return false;
            else
            {
                if (idGoldverses.Length == 1)
                    return Execute("DELETE FROM Golds WHERE id = " + idGoldverses[0]);
                else
                {
                    string textQuery = "DELETE FROM Golds WHERE id = "+ idGoldverses[0]+" ";
                    for (int i = 1; i < idGoldverses.Length; i++)
                    {
                        textQuery += "OR id = " + idGoldverses[i]+" ";
                    }
                    return Execute(textQuery);
                }
            }
           
        }


        //Commands
        public TextCommand GetCommand(string idCommand)
        {
            DataTable t = Select($"Select * From Commands Where id ={idCommand}");
            if (t is not null && t.Rows.Count > 0)
            {
                TextCommand tCommand = new TextCommand(Convert.ToInt32(idCommand), t.Rows[0][1].ToString());
                return tCommand;
            }
            else return null;
        }
        public string GetStringsResponce(string command, char split = ' ')
        {
            DataTable t = Select($"Select id From Commands where textCommand = '{command}'");
            try
            {
                string idCommand = t.Rows[0][0].ToString();
                t = Select($"Select word From Responce_word where idCommand = {idCommand}");
                string splittedString = "";
                foreach (DataRow r in t.Rows)
                {
                    splittedString += r[0].ToString() + split;
                }
                splittedString = splittedString.Substring(0, splittedString.Length - 1);
                return splittedString;
            }
            catch (ApiRequestException)
            {
                AppWorker.Restart();
                return null;
            }
            catch
            {
                return null;
            }
        }
        public string GetStringsAnsveres(string command, char split = ' ')
        {
            DataTable t = Select($"Select id From Commands where textCommand = '{command}'");
            try
            {
                string idCommand = t.Rows[0][0].ToString();
                t = Select($"Select word From Ansvere_word where idCommand = {idCommand}");
                string splittedString = "";
                foreach (DataRow r in t.Rows)
                {
                    splittedString += r[0].ToString() + split;
                }
                splittedString = splittedString.Substring(0, splittedString.Length - 1);
                return splittedString;
            }
            catch (ApiRequestException)
            {
                AppWorker.Restart();
                return null;
            }
            catch
            {
                return null;
            }
        }
        public string? GetRandomAnsvere(string command)
        {
            try
            {
                DataTable t = Select($"Select word From Ansvere_word where idCommand = (Select id From Commands where textCommand = '{command}')");
                Random r = new Random();
                int num = r.Next(0, t.Rows.Count);
                return t.Rows[num][0].ToString();
            }
            catch (ApiRequestException)
            {
                AppWorker.Restart();
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public string? GetRandomAnsvere(int commandId)
        {
            try
            {
                DataTable t = Select($"Select word From Ansvere_word where idCommand = (Select id From Commands where id = '{commandId}')");
                Random r = new Random();
                int num = r.Next(0, t.Rows.Count);
                return t.Rows[num][0].ToString();
            }
            catch (ApiRequestException)
            {
                AppWorker.Restart();
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public DataTable GetAllCommands()
        {
            return Select("id,textCommand","Commands");
        }
        public DataTable GetAllResponceWord()
        {
            return Select("idCommand,word", "responce_word");
        }
        public DataTable GetAllResponceWordLike(string like)
        {
            return Select("idCommand,word", "responce_word", $"word LIKE '{like}'");
        }
        //Feedbacks
        public DataTable GetFeedbacksOnDate(DateTime date)
        {
            int idLesson = GetIdLessonByDate(date);
            return Select($"Select * From Feedbacks Where Lesson = {idLesson}");
        }
        public bool HaveFeedBack(long teenId, string text, int idLastLesson)
        {
            DataTable t = Select($"Select * From Feedbacks Where [IdTeen] = {teenId} AND [Text] = N'{text}' AND [Lesson] = {idLastLesson}");
            return t is not null && t.Rows.Count > 0;
        }
        public bool SetFeedback(long teenId, string text, long idLastLesson)
        {
            if (HaveUser(teenId))
            {
                return Execute($"Insert Into Feedbacks(IdTeen,Text,Lesson) values({teenId},N'{text}',{idLastLesson})");
            }
            else 
            {
                Console.WriteLine("Пользователь не найден");
            }
            return false;
        }
        public DataTable  GetAllWariantsFeedbacks()
        {
            return Select("Select * From Feedbacks_wariants");
        }
        /// <summary>
        /// Получить айди варианта фидюека по тексту
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public int GetIdWariantFeedbackByText(string text)
        {
            DataTable t = Select($"Select id From Feedbacks_wariants WHERE text = '{text}'");
            if (t is not null && t.Rows.Count > 0) return Convert.ToInt32(t.Rows[0][0].ToString());
            else return -1;
        }
        /// <summary>
        ///  Получить текст фидбека по айди
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetTextWariantFeedbackById(int id)
        {
            DataTable t = Select($"Select text From Feedbacks_wariants WHERE id = {id}");
            if (t is not null && t.Rows.Count > 0) return t.Rows[0][0].ToString();
            else return null;
        }


        //Lessons
        /// <summary>
        /// id,Date,idteacher,Team,Description
        /// </summary>
        /// <returns>Возвращает все Уроки прошедшие и непрошедшие </returns>
        public DataTable GetAllLessons()
        {
            return Select("Select * From Lessons");
        }
        public DataTable GetAllLastLessons(bool haveFeedbacks = false)
        {
            DateTime now = DateTime.Now;
            if (haveFeedbacks)
            {
                return Select(@$"
SELECT 
Lessons.id,
Lessons.Date,
Lessons.idTeacher,
Lessons.Team,
Lessons.Description
FROM Lessons Join Feedbacks ON Lessons.id = Feedbacks.Lesson WHERE Lessons.Date < '{now.ToString("yyyy-MM-dd")}'
GROUP BY 
Lessons.id,
Lessons.Date,
Lessons.idTeacher,
Lessons.Team,
Lessons.Description
");
            }
            else
            {
                return Select($"Select * From Lessons Where Date < '{now.ToString("yyyy-MM-dd")}'");
            }
           
        }
      

        public DataTable GetNextLesson()
        {
            DateTime now = DateTime.Now;
            return Select($"Select * From Lessons Where Date < '{now.ToString("yyyy-MM-dd")}'");

        }
        public bool Addlesson(DateTime date,long idTeacher = 0)
        {
            if (idTeacher == 0)
            {
                return Execute($"Insert Into Lessons(Date) values('{date.ToString("yyyy-MM-dd")}')");
            }
            else
            {
                return Execute($"Insert Into Lessons(Date,idTeacher) values('{date.ToString("yyyy-MM-dd")}',{idTeacher})");
            }
        }
        public int GetIdLastLesson()
        {
            DateTime lastSunday = DateWorker.GetPreviousSunday();
            DataTable t = Select($"Select * from Lessons Where Date = '{lastSunday.ToString("yyyy-MM-dd")}'");
            if (t is not null && t.Rows.Count != 0)
            {
                return Convert.ToInt32(t.Rows[0]["id"].ToString());
            }
            else
            {
                return -1;
            }
        }
        

        public string GetNameTeacherLesson(DateTime date)
        {
            DataTable t = Select($"Select idTeacher From Lessons Where Date = '{date.ToString("yyyy.MM.dd")}'");
            if (t is null || t.Rows.Count == 0)
            {
                Addlesson(date);
                return null;
            }
            else 
            {
                string idTeacher = t.Rows[0]["idTeacher"].ToString();
                if (idTeacher == "") return null;
                else
                {
                    
                    return GetNameUser(Convert.ToInt64(idTeacher));
                }
            }
            return null;
        }
        public long GetIdTeacherLesson(DateTime date)
        {
            DataTable t = Select($"Select idTeacher From Lessons Where Date = '{date.ToString("yyyy.MM.dd")}'");
            if (t is null || t.Rows.Count == 0)
            {
                Addlesson(date);
                return 0;
            }
            else
            {
                string idString = t.Rows[0]["idTeacher"].ToString();
                if (idString != "")
                {
                    long idTeacher = Convert.ToInt64(idString);
                    return idTeacher;
                }
                else { return 0; }
            }
            return 0;
        }
        public int GetIdLessonByDate(DateTime date)
        {
            DataTable t = Select($"Select id From Lessons Where Date = '{date.ToString("yyyy-MM-dd")}'");
            if (t is not null && t.Rows.Count > 0) return Convert.ToInt32(t.Rows[0][0].ToString());
            else return -1;
        }
        public bool SetTeamLesson(DateTime date, string team)
        {
            if(GetIdLessonByDate(date) != -1) return Execute($"Update Lessons Set Team = N'{team}' Where Date ='{date.ToString("yyyy-MM-dd")}'");
            else return false;
        }
        public bool SetTeacherOnLesson(DateTime date, long idTeacher=0)
        {
            if (idTeacher != 0)
            {
                return Execute($"Update Lessons Set idTeacher = '{idTeacher}' WHERE Date = '{date.ToString("yyyy.MM.dd")}'");
            }
            else
            {
                return Execute($"Update Lessons Set idTeacher = NULL WHERE Date = '{date.ToString("yyyy.MM.dd")}'");
            }
        }
        public string GetTeamLessonByDate(DateTime date)
        {
            DataTable t = Select($"Select team From Lessons Where Date='{date.ToString("yyyy-MM-dd")}'");
            if(t is not null && t.Rows.Count>0)
            {
                return t.Rows[0][0].ToString();
            }
            return null;
        }

        //AnonimMessages
        public DataTable GetAllAnonimMessage()
        {
            return Select("Select * From Anonim_messages");
        }
        public DataTable GetAllAnonimNotAnsweredMessage()
        {
            return SelectAllIn("Anonim_messages",$"idAnswer_message = 0");
        }
        public bool AddAnonimMessage(UserI teen,string text)
        {
           
            DateTime now = DateTime.Now;
            // Получается в хеш будет записываться вариант с датой вплоть до часа
            
            bool ret = Execute("Insert Into " +
            "Anonim_messages(text,idTeen,dateTime,desiredTeacher)" +
                $"values(N'{text}',{teen.Id},'{now.ToString("yyyy-MM-dd HH:mm")}',{teen.IdDesiredTeacher})");
            return ret;
        }
        public bool SetResponceMessage(int idAnonimMessage,int idResponce)
        {
            return Execute($"Update Anonim_messages Set idResponce_message = {idResponce} WHERE id ={idAnonimMessage}");
        }
        public DataTable GetNewAnonimMessages()
        {
            return Select("Select * From Anonim_messages Where idAnswer_message = 0");
            
        }
        public long[] GetIdUsersMessagesAnon()
        {
            DataTable t = Select("Select * From Anonim_messages Where idAnswer_message = 0");
            List<long> idS = new List<long>();
            if (t is not null && t.Rows.Count > 0)
            {
                foreach (DataRow r in t.Rows)
                {
                    //string hash = r["seriesMessHash"].ToString();
                    long idTeen = Convert.ToInt64(r["idTeen"].ToString());
                    if (idS.IndexOf(idTeen) == -1)
                    {
                        idS.Add(idTeen);
                    }
                }
            }
            return idS.ToArray();
        }
        public long GetIdDesiredteacher(long idTeen)
        {
            DataTable t = Select(
                "Select desiredTeacher " +
                "FROM Anonim_messages " +
                "Where idAnswer_message = 0 AND " +
                "desiredTeacher <> 0 AND " +
                "idTeen = "+idTeen);
            if (t is not null && t.Rows.Count > 0)
            {
                return long.Parse(t.Rows[0][0].ToString());
            }
            else
            {
                return 0;
            }
        }
        public string[] GetAnonMessagesInCompact(string seriesHash)
        {
            DataTable t = Select("Select * From Anonim_messages ORDER BY seriesMessHash");
            string[] messages;
            if (t is not null && t.Rows.Count > 0)
            {
                foreach (DataRow r in t.Rows)
                {
                    string hash = r["seriesMessHash"].ToString();
                    long idTeen = Convert.ToInt64(r["idTeen"].ToString());
                    string text = r["text"].ToString();

                }
            }
            return new string[] { };
        }
        public string GetAnonMessagesCurrentUser(long idTeen,long desiredTeacher)
        {
            DataTable dt = Select($"Select * From Anonim_messages Where idTeen = {idTeen} AND idAnswer_message = 0 AND desiredTeacher = {desiredTeacher}");
            string retText = "";
            foreach (DataRow row in dt.Rows)
            {
                retText += row["text"] + Environment.NewLine;
            }
            return retText;
        }
       
        public string[] GetAllAnonimNames()
        {
            List<string> names = new List<string>();
            DataTable t = Select("Select Name FROM Anonim_nicks");
            foreach (DataRow r in t.Rows)
            {
                names.Add(r[0].ToString());
            }
            return names.ToArray();
        }
       

        // Answer_on_anon
        public int SetNewAnswerAnon(string text,long idTeacher)
        {
            DataTable t = Select("Select Count(*) From Answer_on_anon");
            int countRowsAnswers = Convert.ToInt32(t.Rows[0][0].ToString());

            if (Execute($"Insert Into Answer_on_anon(id,idTeacher,text) values({countRowsAnswers + 1},{idTeacher},N'{text}')"))
            {
                return countRowsAnswers + 1;
            }
            else return -1;
        }
        public bool SetAnswerCurrentteen(long idTeen, long idMessageAnswere,long idDesTeacher)
        {
            return Execute($"Update Anonim_messages Set idAnswer_message = {idMessageAnswere} Where idTeen = {idTeen} AND idAnswer_message = 0 AND desiredTeacher ={idDesTeacher}");
        }
        public DataTable GetOneAnswerOnAnonim(long idAnswere)
        {
            return Select("Select * From Answer_on_anon Where id = " + idAnswere);
        }
        
    }
    public class BibleBase : SQL
    {
        public BibleBase() : base(BibleDB) { }

        public int ChapterCount(string bookId)
        {
            DataTable t = Select($"Select Count(id) From Chapters where book_id={bookId}");
            if (t is not null && t.Rows.Count != 0) return Convert.ToInt32(t.Rows[0][0].ToString());
            return 0;
        }
        public int VersesCount(string chapterId)
        {
            DataTable t = Select($"Select Count(id) From Verses where chapter_id={chapterId}");
            if (t is not null && t.Rows.Count != 0) return Convert.ToInt32(t.Rows[0][0].ToString());
            return 0;
        }

        public int SelectBookIdByChapterId(int chapterId)
        {
            DataTable t = Select($"Select book_id From Chapters where id = {chapterId}");
            if (t is null || t.Rows is null || t.Rows.Count == 0) return -1;
            else return Convert.ToInt32(t.Rows[0][0].ToString());
        }
        public DataTable GetAllBooks()
        {
            return Select("Select * From Books");
        }
        public DataTable GetAllChapters(int bookId)
        {
            return Select($"Select * From Chapters Where book_id={bookId}");
        }
        public DataTable GetAllVersesInChapter(int idChapter)
        {
            string selectedStrig = $"Select * From Verses Where chapter_id = N'{idChapter}'";
            return Select(selectedStrig);
        }
        public DataTable GetAllVersesDT()
        {
            return Select("Select * From Verses");
        }


        public string? GetIdBook(string shortName)
        {
            DataTable t = Select($"Select id From Books Where short_name = N'{shortName}'");
            if (t is not null && t.Rows.Count != 0) return t.Rows[0][0].ToString();
            return null;
        }
        public string? GetIdChapter(string shortName,string chapter_number)
        {
            string idBook = GetIdBook(shortName);
            if (idBook is not null)
            {
                DataTable t = Select($"Select id From Chapters Where book_id = {idBook} AND number_chapter = {chapter_number}");
                if (t is not null && t.Rows.Count != 0) return t.Rows[0][0].ToString();
                return null;
            }
            return null;
           
        }
        public string? GetBookName(string shortName)
        {
            DataTable t = Select($"Select name_book From Books Where short_name = N'{shortName}'");
            if (t is not null && t.Rows.Count != 0) return t.Rows[0][0].ToString();
            return null;
        }

        public DataTable GetRandomVerse()
        {
            //айдишники в базе начинаются с 1708 а заканчиваются на 32809
            Random r = new Random();
            int rSt = r.Next(1707, 32809);
            return Select($"Select name_book,number_chapter,verse_number,text_verse,address From Verses,Chapters,Books where Verses.id ={rSt} AND chapter_id = Chapters.id AND book_id = Books.id");
        }
        public Verse GetVerse(string address)
        {
            string s = $"Select name_book,number_chapter,verse_number,text_verse,address from Verses,Chapters,Books Where address = N'{address}' AND chapter_id = Chapters.id AND book_id = Books.id";
            DataTable t = Select(s);
            if (t is null || t.Rows is null || t.Rows.Count == 0) return null;
            else return new Verse(new AddressVerse(t.Rows[0]["address"].ToString()), t.Rows[0]["text_verse"].ToString());
        }
        public Verse GetNextVerse(string address)
        {
            DataTable t = Select($"Select id from Verses Where address=N'{address}'");
            t = Select($"Select * From Verses Where id = {Convert.ToInt32(t.Rows[0][0]) + 1}");
            if (t is null || t.Rows is null || t.Rows.Count == 0) return null;
            else return new Verse(new AddressVerse(t.Rows[0]["address"].ToString()), t.Rows[0]["text_verse"].ToString());
        }
        public Verse GetPreVerse(string address)
        {
            DataTable t = Select($"Select id from Verses Where address=N'{address}'");
            t = Select($"Select * From Verses Where id = {Convert.ToInt32(t.Rows[0][0]) - 1}");
            if (t is null || t.Rows is null || t.Rows.Count == 0) return null;
            else return new Verse(new AddressVerse(t.Rows[0]["address"].ToString()), t.Rows[0]["text_verse"].ToString());
        }
        public string? GetFirstBookShortName(string pattern)
        {
            //Флп
            DataTable allBook = GetAllBooks();
            foreach (DataRow r in allBook.Rows)
            {
                string lowNameBook = r["name_book"].ToString().ToLower();
                lowNameBook=lowNameBook.Replace(" ","");
                if (lowNameBook.IndexOf(pattern.ToLower()) != -1)
                {
                    return r["short_name"].ToString();
                }
            }
            return null;
        }
        public Verse SearchVerseByText(string text)
        {
            string s = $"Select * from Verses Where text_verse Like '{text}'";
            DataTable t = Select(s);
            if (t is null || t.Rows is null || t.Rows.Count == 0) return null;
            else return new Verse(new AddressVerse(t.Rows[0]["address"].ToString()), t.Rows[0]["text_verse"].ToString());
        }
        public Verse[] SearchAllVersesByText(string text)
        {
            string s = $"Select * from Verses Where text_verse Like N'%{text}%';";
            DataTable t = Select(s);
            if (t is null || t.Rows is null || t.Rows.Count == 0) return null;
            else
            {
                List<Verse> verses = new List<Verse>();
                foreach (DataRow r in t.Rows)
                {
                    Verse v =  new Verse(new AddressVerse(t.Rows[0]["address"].ToString()), t.Rows[0]["text_verse"].ToString());
                    verses.Add(v);
                }
                return verses.ToArray();
            }
        }
        public Verse[]? GetAllVerses()
        {
            List<Verse> verses = new List<Verse>();
            DataTable t = Select("Select * From Verses");
            if (t is null || t.Rows.Count == 0) return null;
            foreach (DataRow r in t.Rows)
            {
                verses.Add(new Verse(new AddressVerse(r["address"].ToString()), r["text_verse"].ToString()));
            }
            return verses.ToArray();
        }
    }
}