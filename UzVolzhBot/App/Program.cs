using Telegram.Bot;
using PodrostkiBot.Configure;

var botClient = new TelegramBotClient(File.ReadAllText(ConstData.PathSecret));
await botClient.GetMeAsync();

BotWorker bW = new BotWorker(botClient);

bW.ListenForMessagesAsync().GetAwaiter().GetResult();