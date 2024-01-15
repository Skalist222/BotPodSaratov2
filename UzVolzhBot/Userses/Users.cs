﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramBotClean.Data;

namespace TelegramBotClean.Userses
{
    public class Users:List<User>
    {
        public string ToString()
        {
            if (this.Count == 0) return "empty";
            else
            {
                return $"({this.Count}) Ученики:{CountTeen}, Учителя:{CountTeacher}, Прихожане:{this.Count-(CountTeacher+CountTeen)}";
            }
        }
        public int CountTeacher {
            get 
            {
                if (this.Count == 0) return 0;
                else
                {
                    return this.Where(x => x.TypeUser == UserTypes.Teacher).Count();
                }
            } 
        }
        public int CountTeen
        {
            get
            {
                if (this.Count == 0) return 0;
                else
                {
                    return this.Where(x => x.TypeUser == UserTypes.Teen).Count();
                }
            }
        }
        public Users() : base() { }
        public Users(DataTable dbTable):base()
        {
            for (int i = 0; i < dbTable.Rows.Count; i++)
            {
                User u = null;
                DataRow r = dbTable.Rows[i];
                string priv = r["privileges"].ToString().Trim();
                if (priv == "teen")
                {
                    u = new Teen(r);
                }
                else
                if (priv == "teacher")
                {
                    u = new Teacher(r);
                }
                else
                if (priv == "admin")
                {
                    u = new Admin(r);
                }
                else
                {
                    u = new DefaultUser(r);
                }
                this.Add(u);
            }
        }
        public Users(BotDB botBase):base()
        {
            this.AddRange(botBase.GetAllUsers());
        }
        public User? this[long id]
        {
            get 
            {
                if (this.Count == 0) return null;
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].BaseInfo.Id == id) return this[i];
                }
                return null;
            }
        }
    }
}
