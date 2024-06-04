using static PodrostkiBot.Configure.ConstData;

using PodrostkiBot.DataBase.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PodrostkiBot.Messages
{
    internal class FileWorker
    {
        ITelegramBotClient botClient;
        CancellationToken token;
        public FileWorker(ITelegramBotClient botClient,CancellationToken token)
        {
            this.botClient = botClient;
            this.token = token;
        }

        public async Task<string> SaveImageMem(string mediaGroupId,Update up)
        {
            try
            {
                mediaGroupId = up.Message.MediaGroupId ?? "";
                string idFile = up.Message.Photo.LastOrDefault().FileId;
                Telegram.Bot.Types.File file = botClient.GetFileAsync(idFile).GetAwaiter().GetResult();
                var fileName = idFile + "." + file.FilePath.Split('.').Last();
                using (FileStream imageSaver = new FileStream(PathMems+"\\"+fileName, FileMode.Create))
                {
                    await botClient.DownloadFileAsync(file.FilePath, imageSaver,token);
                }
                return idFile;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка добавления мема!");
                Console.WriteLine(ex);
                return "";
            }
        }
       
        public async Task<string> SaveImage(Update up, string nameLastImage = null)
        {
            try
            {
                string idFile = up.Message.Photo.LastOrDefault().FileId;
                var file = botClient.GetFileAsync(idFile).GetAwaiter().GetResult();
                var fileName = idFile + "." + file.FilePath.Split('.').Last();
                using (FileStream imageSaver = new FileStream(DefaultPath + "\\" + fileName, FileMode.Create))
                {
                    await botClient.DownloadFileAsync(file.FilePath, imageSaver, token);
                }
                return DefaultPath + "\\" + fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка Сохранения фото!");
                Console.WriteLine(ex);
                return "";
            }
        }
    }
}
