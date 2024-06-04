using PodrostkiBot.DataBase.Engine;
using System.IO;
using Telegram.Bot.Types;

namespace MemWorkerSpace
{
    internal class MemWorker
    {
        string pathMemses;
        string[] listPhoto;
        public MemWorker(string path)
        {
            listPhoto = Directory.GetFiles(path);
            pathMemses = path;
        }
        public Mem GetRandomMem(Random r)
        {
            int i = r.Next(0, listPhoto.Count() );
            string photoPath = listPhoto[i];
            return new Mem(TypeMem.Photo, photoPath, 0);
        }
        public string GetRandomVPPathMem(Random r,string validate, bool parametrNotFullText = false)
        {
            if (!parametrNotFullText)
            {
                try { validate = validate.Split('.')[1]; }
                catch { return ""; }

            }
            VPList validVP = new VPList();
            //foreach (VP vp in listVP)
            //{
            //    foreach (string parametr in vp.Parametrs)
            //    {
            //        if (validate.ToLower().IndexOf(validate.ToLower()) != -1) 
            //            validVP.Add(vp);
            //    }
            //}
            if (validVP.Count == 0) return "";
          
            int i = r.Next(0, validVP.Count);
            return validVP[i].PathFile;
        }
        public int Length { get {
                listPhoto = Directory.GetFiles(pathMemses);
                return listPhoto.Length; 
            } }
    }

    public class VP
    {
        string[] parametrs;
        string path;

        public string PathFile { get { return path; } }
        public string[] Parametrs { get { return parametrs; } }

        public VP(string[] parametrs, string path)
        {
            this.parametrs = parametrs;
            this.path = path;
        }
        public void WriteInFile(string path)
        {
            string line = path;
            foreach (string p in parametrs)
            {
                line += "|" + p;
            }
            System.IO.File.WriteAllText(path, line + Environment.NewLine);
        }
    }
    public class VPList : List<VP>
    {
        public VPList()
        {

        }
        public VPList(string[] array) : base()
        {
            foreach (string s in array)
            {
                string[] infos = s.Split('|');
                string name = infos[0];
                string[] parametrs = infos.Skip(1).ToArray();
                Add(new VP(parametrs, name));
            }
        }
    }

    public class TypeMem
    {
        bool video;
        bool photo;

        TypeMem(bool video, bool photo)
        {
            this.video = video;
            this.photo = photo;
        }

        public bool IsPhoto { get { return photo; } }
        public bool IsVideo { get { return video; } }
        public static TypeMem Video { get { return new TypeMem(true, false); } }
        public static TypeMem Photo { get { return new TypeMem(false, true); } }
    }
    public class Mem
    {
        public readonly TypeMem Type;
        public readonly string Path;
        public readonly int RandomPar;

        public bool IsPhoto { get { return Type.IsPhoto; } }
        public bool IsVideo{ get { return Type.IsVideo; } }

        public Mem(TypeMem type, string path, int randomPar)
        {
            Type = type;
            Path = path;
            RandomPar = randomPar;
        }
    }
}
