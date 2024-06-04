using PodrostkiBot.Bible;
using PodrostkiBot.DataBase.Engine;
using PodrostkiBot.Menus;
using PodrostkiBot.Messages;
using PodrostkiBot.Text;
using PodrostkiBot.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace PodrostkiBot.Configure
{
    public class Engine
    {
        private ITelegramBotClient botClient;
        public Sender Sender { get; }
        public Spam Spammer { get; }
        public BibleWorker BibleWorker { get;}
        public BotBase BotBase { get; }
        public UserList Users { get; }
        public AllCommands AllCommands { get; }
        public AllRespWords AllResponceWords { get; }
        public Selectors Selector { get; }
        public ExecutorsList Executor { get; }
        public GoldenVerseList Golds { get; }
        public BaseMenu Menus { get; }
        public Random Random { get; }

        public Engine(ITelegramBotClient botClient)
        {
            this.Menus = new BaseMenu();
            this.Random = new Random();
            this.botClient = botClient;
            this.BotBase = new BotBase();
            this.BibleWorker = new BibleWorker();
            this.AllCommands = new AllCommands(BotBase);
            this.AllResponceWords = new AllRespWords(BotBase);
            this.Users = new UserList(BibleWorker,BotBase);
            this.Selector = new Selectors(AllCommands);
            this.Golds = new GoldenVerseList(BibleWorker, BotBase);
            this.Sender = new Sender(botClient, BotBase,Users,BibleWorker);
            this.Spammer = new Spam(this);
            this.Executor = new ExecutorsList(Sender);
        }
    }
}
