using PodrostkiBot.DataBase.Engine;
using System.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PodrostkiBot.Bible
{
    public class BibleWorker
    {
        public BibleBase DB;
        public string pathBible;


        public BibleWorker()
        {
            DB = new BibleBase();
        }
        public int ChapterCount(string shortName)
        {
            string idBook = DB.GetIdBook(shortName);
            if (idBook is not null) return DB.ChapterCount(idBook);
            return 0;
        }
        public int VersesCount(string shortNameBook, string chapterNum)
        {
            string idChapter = DB.GetIdChapter(shortNameBook, chapterNum);
            if (idChapter is not null) return DB.VersesCount(idChapter);
            return 0;
        }
        public string[] GetBooksNames()
        {
            List<string> names = new List<string>();
            DataTable t = DB.GetAllBooks();
            if (t is not null && t.Rows.Count != 0)
            {
                foreach (DataRow r in t.Rows)
                {
                    names.Add(r["name_book"].ToString());
                }
                return names.ToArray();
            }
            else return new string[] { };
        }
        public string[] GetBooksShortNames()
        {
            List<string> shortNames = new List<string>();
            DataTable t = DB.GetAllBooks();
            if (t is not null && t.Rows.Count != 0)
            {
                foreach (DataRow r in t.Rows)
                {
                    shortNames.Add(r["short_name"].ToString());
                }
                return shortNames.ToArray();
            }
            else return new string[] { };
        }
        public string GetBookNameByShortName(string shortName)
        {
            string nameBook = DB.GetBookName(shortName);
            if (nameBook is not null) return nameBook;
            return "";
        }
        public Verse GetRandomVerse()
        {
            DataTable t = DB.GetRandomVerse();
            if (t is null || t.Rows is null || t.Rows.Count == 0) throw new EmtyVerseListException();
            else
            {
                Verse v = new Verse(new AddressVerse(t.Rows[0][4].ToString()), t.Rows[0][3].ToString());
                return v;
            }
        }
        public Verse? GetVerse(string address)
        {
            try
            {
                return DB.GetVerse(address);

            }
            catch (EmtyVerseListException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public Verse GetNextVerse(Verse s)
        {
            //Verse retVerse = GetVerse("Откр 22:21");
            Verse defaultVerse = GetVerse("Быт 1:1");
            if (s.AddressText == "Откр 22:21") return defaultVerse;
            else
            {

                return DB.GetNextVerse(s.AddressText);
            }
        }
        public Verse GetPreVerse(Verse s)
        {
            Verse defaultVerse = GetVerse("Откр 22:21");
            if (s.AddressText == "Быт 1:1") return defaultVerse;
            else
            {
                return DB.GetPreVerse(s.AddressText);
            }
            return null;
        }
        public Verse? SearchVerse(string text)
        {

            string[] split = text.Split(' ');
            string shortNameBook = "";
            int chapterNumber = 0;
            int verseNumber = 0;
            int bookNum = 0;

            for (int i = 0; i < split.Length; i++)
            {
                string s = split[i];
                string[] wDot = s.Split(':');

                if (wDot.Length == 2)
                {
                    try
                    {
                        chapterNumber = int.Parse(wDot[0]);
                        verseNumber = int.Parse(wDot[1]);
                        continue;
                    }
                    catch
                    {
                        continue;
                    }

                }
                if (bookNum == 0)
                {
                    bookNum = s == "1" ? 1 : s == "2" ? 2 : s == "3" ? 3 : s == "4" ? 4 : 0;
                    if (bookNum != 0) continue;
                }
                if (shortNameBook == "")
                {
                    //филипп
                    string currentShortName = DB.GetFirstBookShortName(s);
                    if (currentShortName is not null)
                    {
                        shortNameBook = currentShortName;
                    }
                }
            }
            if (shortNameBook == "" || chapterNumber == 0 || verseNumber == 0)
            {
                text = text.Substring(split[0].Length + 1, text.Length - (split[0].Length + 1));
                return SearchVerseByText(text);
            }
            else
            {
                if (bookNum != 0)
                {
                    //найди 1 цар 2:8
                    string sub = shortNameBook.Substring(1, shortNameBook.Length - 1);
                    shortNameBook = bookNum + "" + sub;
                }
                string address = shortNameBook + " " + chapterNumber + ":" + verseNumber;

                return DB.GetVerse(address);

            }
            return null;
        }
        public async void SearchAndSendVerse(string text, ITelegramBotClient botClient, CancellationToken token)
        {

        }
        public Verse? SearchVerseByText(string text)
        {

            VerseList verses = new VerseList();
            string[] words = text.Split(' ');
            //Защита от дурака
            if (words.Length > 15) return null;
            Verse[] versesInBase = DB.GetAllVerses();

            DateTime startTime = DateTime.Now;

            int finishedIteration = 0;

            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (word != "и" && word != "а" && word != "о" && word != "с")
                {
                    new Thread(() =>
                    {
                        for (int j = 0; j < versesInBase.Length; j++)
                        {
                            if (versesInBase[j].Text.ToLower().IndexOf(word.ToLower()) != -1) verses.Add(versesInBase[j]);
                        }
                        finishedIteration++;
                    }).Start();
                }
                else finishedIteration++;
            }
            //127 милисекунд
            while (finishedIteration < words.Length) { }
            // Если ничего не нашел реально
            if (verses.Count == 0) return null;
            int[] verseMaximuses;
            VerseList list = verses.GetDuplicates();
            DateTime finishTime = DateTime.Now;

            //Console.WriteLine((finishTime - startTime).Milliseconds);
            int a = 0;
            if (list.Count != 0)
            {
                verseMaximuses = new int[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    foreach (string word in words)
                    {
                        if (list[i].Text.ToLower().IndexOf(word.ToLower()) != -1) verseMaximuses[i]++;
                    }
                }
                int max = verseMaximuses.Max();
                List<int> verseIndexes = new List<int>();
                for (int i = 0; i < verseMaximuses.Length; i++)
                {
                    if (verseMaximuses[i] == max) verseIndexes.Add(i);
                }
                return list[verseIndexes[0]];
            }
            else
            {
                verseMaximuses = new int[verses.Count];
                foreach (Verse v in verses)
                {
                    for (int i = 0; i < words.Length; i++)
                    {
                        string word = words[i];
                        if (v.Text.ToLower().IndexOf(word.ToLower()) != -1)
                            verseMaximuses[i]++;
                    }
                }
                int index = verseMaximuses.ToList().IndexOf(verseMaximuses.Max());
                return verses[index];
            }

        }
        public Verse? SearchVerseByTextAsync(string text)
        {
            VerseList verses = new VerseList();
            string[] words = text.Split(' ');
            //Защита от дурака
            if (words.Length > 15) return null;
            Verse[] versesInBase = DB.GetAllVerses();


            DateTime startTime = DateTime.Now;

            int finishedIteration = 0;

            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (word != "и" && word != "а" && word != "о" && word != "с")
                {
                    new Thread(() =>
                    {
                        for (int j = 0; j < versesInBase.Length; j++)
                        {
                            if (versesInBase[j].Text.ToLower().IndexOf(word.ToLower()) != -1) verses.Add(versesInBase[j]);
                        }
                        finishedIteration++;
                    }).Start();
                }
            }
            //127 милисекунд
            while (finishedIteration < words.Length) { }

            // Если ничего не нашел реально
            if (verses.Count == 0) return null;
            int[] verseMaximuses;
            VerseList list = verses.GetDuplicates();
            DateTime finishTime = DateTime.Now;

            //Console.WriteLine((finishTime - startTime).Milliseconds);
            int a = 0;
            if (list.Count != 0)
            {
                verseMaximuses = new int[list.Count];
                for (int i = 0; i < list.Count; i++)
                {
                    foreach (string word in words)
                    {
                        if (list[i].Text.ToLower().IndexOf(word.ToLower()) != -1) verseMaximuses[i]++;
                    }
                }
                int max = verseMaximuses.Max();
                List<int> verseIndexes = new List<int>();
                for (int i = 0; i < verseMaximuses.Length; i++)
                {
                    if (verseMaximuses[i] == max) verseIndexes.Add(i);
                }
                return list[verseIndexes[0]];
            }
            else
            {
                verseMaximuses = new int[verses.Count];
                foreach (Verse v in verses)
                {
                    for (int i = 0; i < words.Length; i++)
                    {
                        string word = words[i];
                        if (v.Text.ToLower().IndexOf(word.ToLower()) != -1)
                            verseMaximuses[i]++;
                    }
                }
                int index = verseMaximuses.ToList().IndexOf(verseMaximuses.Max());
                return verses[index];
            }

        }
    }
}
