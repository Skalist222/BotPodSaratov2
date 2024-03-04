using Microsoft.Identity.Client;
using System.Data;
using System.Drawing;
using Telegram.Bot;
using TelegramBotClean.Data;
using TelegramBotClean.Messages;
using static TelegramBotClean.Data.Logger;

namespace TelegramBotClean.MemDir
{
    public class Mems:List<Mem>
    {
        BotDB botBase;
        TelegramBotClient botClient;
        CancellationToken token;
        public Mems(BotDB botBase, TelegramBotClient botClient, CancellationToken token) 
        {
            this.botBase = botBase;
            this.botClient = botClient;
            this.token = token;
            DataTable memMessagesTable = botBase.GetAllMems();
            if(memMessagesTable.Rows.Count>0)
            for (int i = 0; i < memMessagesTable.Rows.Count; i++)
            {
                DataRow r = memMessagesTable.Rows[i];
                this.Add(r["fileId"].ToString(), Convert.ToInt64(r["id"].ToString()), Convert.ToInt64(r["chatId"].ToString()));
            }
        }
        public bool Add(Mem mem)
        {
            long idNewMem = botBase.CreateMem(mem);
            if (idNewMem != -1)
            {
                base.Add(mem);
                return true;
            }
            else
            {
                Error("Не удалось отправить мем в базу данных!!");
                return false;
            }
        }
        public bool Add(string fileId,long idMessage,long idChat)
        {
            try
            {
                base.Add(new Mem(new MessageI("",fileId,MessageTypes.Photo.Name)));
            }
            catch
            {
                return false;
            }
            return true;
        }
        protected Bitmap GetRandomMemImage(Random r)
        {
            int randomIndexMem = r.Next(0, this.Count);
            MessageI m = this[randomIndexMem].Message;
            string idMessage = m.FileId;
            Telegram.Bot.Types.File fileInfo = null;
            fileInfo = botClient.GetFileAsync(m.FileId).Result;
            string filePath = fileInfo.FilePath;
            string newNameFile = Directory.GetCurrentDirectory() + "\\" + fileInfo.FileUniqueId[0..15] + " " + Path.GetFileName(filePath);
            Bitmap bm;
            Bitmap returnBm;
            using (FileStream fileStream = new FileStream(newNameFile, FileMode.Create))
            {
                botClient.DownloadFileAsync(
                    filePath: filePath,
                    destination: fileStream,
                    cancellationToken: token).GetAwaiter().GetResult();
                bm = (Bitmap)Bitmap.FromStream(fileStream);
                returnBm = new Bitmap(bm);
                bm.Dispose();
                bm = null;
            }
            System.IO.File.Delete(newNameFile);
            return returnBm;
        }
        public MessageI GetMessageRandomMem(Random r)
        {
            string textMessage = botBase.GetRandomAnswer("catch") + " "+ botBase.GetRandomAnswer("mem") + ")";
            Bitmap bm = GetRandomMemImage(r);
            return new MessageI(textMessage);
        }
        public MessageI? GetMessageRandomMem2(Random r)
        {
            if (this.Count == 0) return null;
            int randomIndexMem = r.Next(0, this.Count);
            MessageI m = this[randomIndexMem].Message;
            return m;
        }
    }
}
