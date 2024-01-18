
using TelegramBotClean.Bot;
using TelegramBotClean.Data;

if (Config.ValidationConfig())
{
    BotWorker bW = new BotWorker(Config.Token);
    await bW.ListenForMessagesAsync();
    Console.ReadLine();
}
else
{
    Logger.Error("Не удалось запустить бот, конфигурация не прошла валидацию!!!","Старт программы");
}
