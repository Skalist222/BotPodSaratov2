using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Constantdata;

namespace TelegramBotClean.Bot
{
    public class BotWorker
    {
        static CancellationToken token;
        private readonly TelegramBotClient botClient;
        internal Sender sender;

        public BotWorker(string tokenString)
        {
            botClient = new TelegramBotClient(tokenString);
            sender = new Sender(botClient,token);
        }
        public async Task ListenForMessagesAsync()
        {
            botClient.StartReceiving(
                updateHandler: GetUpdates,
                pollingErrorHandler: GetErrors,
                receiverOptions: new ReceiverOptions{AllowedUpdates = Array.Empty<UpdateType>()},
                cancellationToken: token
            );
            Console.WriteLine($"Бот запущен в: @{botClient.GetMeAsync().Result.Username}");
            Console.ReadLine();
        }
        private async Task GetUpdates(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            token = cancellationToken;
            // Only process Message updates
            if (update.Message is not { } message)
            {
                return;
            }
            sender.CreateAnswere(update);
        }
        private Task GetErrors(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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


        private static async Task SendMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken, string info, long idChat = -1)
        {

            long id = idChat == -1 ? message.Chat.Id : idChat;
            try
            {
                Message sendArtwork = await botClient.SendTextMessageAsync(
                chatId: id,
                text: info,
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка в отправке текстового сообщения: " + e.Message);
            }
        }
     
    }
}
