using TelegramBotClean.Bot;
using TelegramBotClean.CommandsDir;
using TelegramBotClean.Messages;

namespace TelegramBotClean.Commandses
{
    public class Command
    {
        string name;
        List<string> texts;
        string description;
        Action<Sender, MessageI> worker;

        public string ToString()
        {
            return name;
        }
        public string Name { get { return name; } }
        public string Description { get { return description; } }
        public void SetVariants(string[] variants)
        {
            texts.AddRange(variants);
        }
        public void SetVariants(string v1, string v2 = "", string v3 = "", string v4 = "", string v5 = "", string v6 = "", string v7 = "", string v8 = "", string v9 = "", string v10 = "")
        {
            texts.Add(v1);
            if (v2 != "") texts.Add(v2);
            if (v3 != "") texts.Add(v3);
            if (v4 != "") texts.Add(v4);
            if (v5 != "") texts.Add(v5);
            if (v6 != "") texts.Add(v6);
            if (v7 != "") texts.Add(v7);
            if (v8 != "") texts.Add(v8);
            if (v9 != "") texts.Add(v9);
            if (v10 != "") texts.Add(v10);

        }

        public Command(string name, string[] texts = null, Action<Sender, MessageI> worker = null, string description = "")
        {
            this.name = name;
            this.description = description;
            if (texts is null || texts!.Length == 0) this.texts = new List<string>();
            else
            {
                this.texts = new List<string>() { name };
                this.texts.AddRange(texts!.ToList());
            }
            if(worker is not null) this.worker = worker;
        }
        public bool Check(string variantText)
        {
            for (int i = 0; i < texts.Count; i++)
            {
                if (texts[i].ToLower().IndexOf(variantText.ToLower()) != -1)
                {
                    return true;

                }
                else
                {
                    if (variantText.ToLower().IndexOf(texts[i].ToLower()) != -1)
                        return true;
                }

            }
            return false;
        }
        public void Execute(Sender sender, MessageI mes)
        {
            worker(sender, mes);
        }



        public static bool operator ==(Command c1, Command c2)
        {
            return c1.name == c2.name;
        }
        public static bool operator !=(Command c1, Command c2)
        {
            return !(c1 == c2);
        }
        public static Commands operator +(Command c1, Command c2)
        {
            Commands commands = new Commands();
            commands.Add(c1, c2);
            
            return commands;
        }
        public static Commands operator +(Command c1, Commands cList)
        {
            Commands commandList = new Commands();
            commandList.Add(c1);
            return (Commands)(commandList+cList);
        }
    }
}
