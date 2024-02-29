using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotClean.Data;

namespace TelegramBotClean.Bible
{
    public class BibleWorker
    {
        private Random Random;
        protected Books Books { get; }
        protected Chapters Chapters { get; }
        public Verses Verses { get;}
        public Verses GoldVerses { get;}
        public BibleWorker(BibleDB bibleBase,BotDB botBase,Random r)
        {
            GoldVerses = new Verses();
            this.Random = r;
            this.Books      = bibleBase.GetBooks();
            this.Chapters   = bibleBase.GetChapters();
            this.Verses     = bibleBase.GetVerses(Books, Chapters);
            SetGoldverses(botBase);
        }
        private void SetGoldverses(BotDB botBase)
        {
            DataTable t = botBase.GetGoldVerses();
            for (int i = 0; i < t.Rows.Count; i++)
            {
                string address = t.Rows[i]["address"].ToString();
                Verse v = Verses[address];
                if (v is null)
                {
                    Logger.Error($"{address} не найден в библии!!!");
                }
                else
                {
                    this.GoldVerses.Add(v);
                }
               
            }
        }
        public Verse GetRandomVerse()
        {
            return this.Verses[Random.Next(Verses.Count)];
        }
        public Verse GetRandomGoldVerse()
        {
            return this.GoldVerses[Random.Next(GoldVerses.Count)];
        }
        public Verse GetVerseByAddress(string address)
        {
            if (address == "") return null;
            Verse v = Verses[address];
            if (v is null)
            {
                Logger.Error($"{address} не найден в библии!!!");
                return null;
            }
            else
            {
                return v;
            }
            
        }
        public Verse GetVerseByAddress(AddressVerse address)
        {
            if (address is not null) return GetVerseByAddress(address.ToString());
            else return null;
        }
    }
    
    public class AddressVerse
    {
        Book book;
        Chapter chapter;
        Verse verse;

        public string ToString()
        {
            return book.ShortName + " " + chapter.Number + ":" + verse.Number;
        }
        public AddressVerse(Book book, Chapter chapter, Verse verse)
        {
            this.book = book;
            this.chapter = chapter;
            this.verse = verse;
        }
        private AddressVerse(string textAddress, BibleWorker bibleWorker)
        {
            string[] split = textAddress.Split(' ');
            string bookShortName = split[0];
            long chapterNumber = Convert.ToInt64(split[1].Split(':')[0]);
        }
        public static AddressVerse ByTextAddress(string textAddress, BibleWorker bibleWorker)
        {
            return new AddressVerse(textAddress, bibleWorker);
           
        }
        //public string ToString()
        //{
            
        //}
      
    }
    public class Book 
    {
        string name;
        string shortName;
        long idInDB;
        long countChapters;

        public string Name { get { return name; } }
        public string ShortName { get { return shortName; } }
        public long Id { get { return idInDB; } }
        public long CountChapters { get { return countChapters; } }
  
        public Book(string name,string shortName,long idInDB,long countChapters)
        {
            this.name = name;
            this.shortName = shortName;
            this.idInDB = idInDB;
            this.countChapters = countChapters;
        }

    }
    public class Books:List<Book>
    {
        public Books() : base() { }
        public Books(List<Book> books) : base()
        {
            for (int i = 0; i < books.Count; i++)
            {
                Add(books[i]);
            }
        }
        public Book BookByChapter(Chapter chapter)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (chapter.BookId == this[i].Id) return this[i];
            }
            return null;
        }
    }
    public class Chapter 
    {
        long id;
        long bookId;
        long number;
        long countVerses;

        public long Id { get { return id; } }
        public long BookId {  get { return bookId; } }
        public long Number {  get { return number; } }
        public long CountVerses {  get { return countVerses; } }

        public Chapter(long id,long bookId, long number, long countVerses)
        {
            this.id = id;
            this.bookId = bookId;
            this.number = number;
            this.countVerses = countVerses;
        }
    }
    public class Chapters : List<Chapter> 
    {
        public Chapters() : base() { }
        public Chapters(List<Chapter> chapters) : base()
        {
            for (int i = 0; i < chapters.Count; i++)
            {
                Add(chapters[i]);
            }
        }
        public Chapter Select(long chapterId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Id == chapterId) return this[i];
            }
            return null;
        }
        
    }
    public class Verse
    {
        long verseNumber;
        long idChapter;
        long idVerse;
        string text;
        AddressVerse address;

        public long Number { get { return verseNumber; } }
        public long IdChapter { get { return idChapter; } }
        public long IdVerse { get { return  idVerse; } }
        public string Text { get { return text; } }
        public string TextWithAddress { get { return address.ToString() + $" \"{text}\""; } }
        public AddressVerse Address { get { return address; } }
        public string AddresText { get { return address.ToString(); } }

        public string ToString()
        {
            return TextWithAddress;
        }

       


        public Verse(long verseNumber,long idChapter, long idVerse,string text,Books books,Chapters chapters)
        {
            this.verseNumber = verseNumber;
            this.idChapter = idChapter;
            this.idVerse = idVerse;
            this.text = text;
            Chapter c = chapters.Select(idChapter);
            Book b = books.BookByChapter(c);

            address = new AddressVerse(b,c,this);
        }

    }
    public class Verses :List<Verse> 
    {
        public Verses() : base() { }
        public Verses(List<Verse> verses) : base()
        {
            for (int i = 0; i < verses.Count; i++)
            {
                Add(verses[i]);
            }
        }
        public Verse this[string address]
        {
            get 
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].AddresText == address)
                        return this[i];
                }
                return null;
            }
        }
    }

    
}
