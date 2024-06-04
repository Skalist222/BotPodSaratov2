using PodrostkiBot.DataBase.Engine;
using System.Data;
using System.Globalization;
using System.Net;
using Telegram.Bot.Types;

namespace PodrostkiBot.Bible
{
    public class GoldenVerseList : VerseList
    {
        public int count = 0;
        BotBase botBase { get; set; }
        BibleWorker worker { get; set; }
        public GoldenVerseList(BibleWorker worker, BotBase botBase)
        {
            this.botBase = botBase;
            this.worker = worker;
            UpdateInformation();
        }
        private void UpdateInformation()
        {
            DataTable t = botBase.GetAllGoldVerses();//60мс
            foreach (DataRow row in t.Rows)
            {
                string address = row["address"].ToString();
                Verse s = worker.GetVerse(address);
                if (s is not null)
                {
                    base.Add(s);
                }
            }
        }
        public Verse? GetRandomGoldVerse(Random r)
        {
            try
            {
                int id = r.Next(0, base.Count);
                Verse verse = this[id];
                return verse;
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }
        public Verse? GetGoldVerseById(int id)
        {
            DataTable verses = botBase.GetGoldVerseById(id);
            if (verses.Rows.Count > 0) return worker.GetVerse(verses.Rows[0]["address"].ToString());
            else return null;
        }
        public bool Add(Verse s)
        {
           
            int selectedRows = this.Where(vers => vers.Text == s.Text)
                .ToList().Count();
            if (selectedRows==0)
            {
                base.Add(s);
                botBase.AddGold(s);
                return true;
            }
            else 
            {
                return false;
            }

        }
        public int Count(BotBase botBase)
        {
            return botBase.GetAllGoldVerses().Rows.Count;
        }
    }
}
