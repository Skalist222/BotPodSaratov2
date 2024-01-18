
using TelegramBotClean.Bot;

BotWorker bW = new BotWorker("6331122543:AAE4Vw1s68hwT-G-_cV88xJ6VON-6U41kwo");
await bW.ListenForMessagesAsync();
Console.ReadLine();