using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using PodrostkiBot.Messages;
using PodrostkiBot.DataBase.Engine;
using PodrostkiBot.Text;

namespace PodrostkiBot.Configure
{

    public class BotWorker
    {

        //static UserStihList lastUserShih = new UserStihList();
        static bool getMessage = false;
        static bool cleanerWork = true;
        static CancellationToken token;
        private readonly TelegramBotClient _botClient;

        CreatorMessage creatorMes;
        AnswereCreator anCreator;


        public BotWorker(TelegramBotClient botClient)
        {
            _botClient = botClient;
            creatorMes = new CreatorMessage(botClient);
            //anCreator = new AnswereCreator();

        }

        public async Task ListenForMessagesAsync()
        {
            using var cts = new CancellationTokenSource();
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };
            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );
            var me = await _botClient.GetMeAsync();
           
            Console.WriteLine($"Сервер запущен @{me.Username}");
            Console.ReadLine();
        }
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            token = cancellationToken;
            // Only process Message updates
            if (update.Message is not { } message && update.CallbackQuery is null)
            {
                return;
            }
            getMessage = true;
            creatorMes.CreateAnsvere(update);
            getMessage = false;
        }
        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
