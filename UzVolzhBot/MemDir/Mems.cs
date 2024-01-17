using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
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
            DataTable memMessagesTable = botBase.GetAllMemMessages();
            for (int i = 0; i < memMessagesTable.Rows.Count; i++)
            {
                DataRow r = memMessagesTable.Rows[i];
                this.Add(r["fileId"].ToString(), Convert.ToInt64(r["idMessage"].ToString()), Convert.ToInt64(r["idChat"].ToString()));
            }
        }
        public bool Add(Mem mem)
        {
            if (botBase.CreateMemMessage(mem))
            {
                base.Add(mem);
                return true;
            }
            else
            {
                Error("Не удалось отправить мем в базу данных!!", "AddMem(Mem mem, BotBD botBase)");
                return false;
            }
        }
        public bool Add(string fileId,long idMessage,long idChat)
        {
            return this.Add(new Mem(new MessageI("",fileId,idMessage,idChat)));
        }
        protected Bitmap GetRandomMemImage(Random r)
        {
            int randomIndexMem = r.Next(0, this.Count);
            MessageI m = this[randomIndexMem].Message;
            string idMessage = m.ImageId;
            Telegram.Bot.Types.File fileInfo = null;
            fileInfo = botClient.GetFileAsync(m.ImageId).Result;
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
            string textMessage = botBase.GetRandomAnswer("catch")+ botBase.GetRandomAnswer("mem")+")";
            Bitmap bm = GetRandomMemImage(r);
            return new MessageI(textMessage,bm);
        }
        

    }
}
