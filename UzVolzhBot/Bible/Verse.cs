using System.Data;
using System.Transactions;

namespace PodrostkiBot.Bible
{
    public class Verse
    {
        AddressVerse address;
        string text;
        string description;

        public string Text { get { return text; } }
        public AddressVerse Address { get { return address; } }
        public string AddressText { get { return address.Text; } }
        public Verse(AddressVerse address, string text, string description = "")
        {
            this.address = address;
            this.text = text;
            this.description = description;
        }
        public bool IsEmpty { get { return AddressText == " 0:0"; } }
        public string ToString()
        {
            return AddressText + " \"" + Text + "\"";
        }


        public static bool operator ==(Verse v1, Verse v2)
        {
            if (v1 is null && v2 is null) return true;
            if (v1 is not null && v2 is null) return false;
            if (v1 is null && v2 is not null) return false;
           return v1.ToString() == v2.ToString();
        }
        public static bool operator !=(Verse v1, Verse v2)
        {
            return v1.ToString() != v2.ToString();
        }

        public static bool Equals(Verse v1, Verse v2)
        {
            return v1 == v2;
        }
    }
    public class VerseList : List<Verse> 
    {
        public VerseList() : base() { }
        public VerseList(DataTable t) : base()
        {

        }
        public VerseList GetDuplicates()
        {
            List<int> idsDuplicates = new List<int>();
            List<Verse> notDuplicates = this.Distinct().ToList();
            int c = this.Count;
            for (int i = 0; i < c; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    if (j != i)
                    {
                        
                        if (this[i].GetHashCode() == this[j].GetHashCode())
                        {
                            if (idsDuplicates.IndexOf(i) == -1 && idsDuplicates.IndexOf(j) == -1)
                                idsDuplicates.Add(i);
                        }
                    }
                }
            }
            VerseList list = new VerseList();
            foreach (int i in idsDuplicates)
            {
                list.Add(this[i]);
            }
            return list;
        }

        public void CleanDuplicate()
        {
            List<int> ids = new List<int>();
            for(int i =0;i<this.Count;i++)
            {
                for (int j = 0; j < this.Count; j++)
                {
                    if (j != i)
                    {
                        if (this[i].GetHashCode() == this[j].GetHashCode()) ids.Add(i);
                    }
                }
            }
            foreach (int i in ids)
            {
                RemoveAt(i);
            }
        }
    }
    public class AddressVerse
    {

        string bookName;
        int chapterId;
        int verseId;

        public string BookName { get { return bookName; } }
        public int ChapterId { get { return chapterId + 1; } }
        public int VerseId { get { return verseId + 1; } }
        public string Text { get { return bookName + " " + chapterId + ":" + verseId; } }
       

        public AddressVerse(string bookName,int c, int s)
        {
            this.bookName = bookName;
            chapterId = c;
            verseId = s;
        }
        public AddressVerse(string addresText)
        {
            string[] split = addresText.Split(' ');
            this.bookName = split[0];
            this.chapterId = Convert.ToInt32(split[1].Split(':')[0]);
            this.verseId = Convert.ToInt32(split[1].Split(':')[1]);
        }
    }
    public class EmtyVerseListException : Exception
    {
        public EmtyVerseListException() : base("Ошибка! База вернула пустой список Стихов") { }
    }
}
