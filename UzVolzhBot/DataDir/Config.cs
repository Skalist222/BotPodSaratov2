
namespace TelegramBotClean.Data
{
    public class Config
    {
        public static string InvizibleChar { get { return "ㅤ"; } }
        public static string Token { get { return File.ReadAllLines(Path.Combine(Directory.GetCurrentDirectory(), "Data", "Secret.txt")).FirstOrDefault("No"); } }
        public static string PathToDBBot{ get { return Path.Combine(Directory.GetCurrentDirectory(), "Data", "BotInformation.mdf"); } }
        public static BotInformation Info { get { return new BotInformation(); } }
        public static bool ValidationConfig()
        {
            if (Token == "No") return false;
            if (!(new BotDB(new Random()).IsReady)) return false;
            return true;
        }
    }
    public class BotInformation
    {
        public static string version { get { return "v. 2.0"; } }
        public static string updates
        {
            get
            {
                return
                    "Новая версия Бота, полностью переписана, и работает стабильнее и быстрее";
            }
        }
        override public string ToString()
        {
            string nLine = Environment.NewLine;
            return string.Join(nLine, new string[]
                {
                    version,
                    updates
                });
        }
        public string Text { get { return this.ToString(); } }
    }

}
