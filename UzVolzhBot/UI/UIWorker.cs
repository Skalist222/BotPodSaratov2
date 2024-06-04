using PodrostkiBot.Messages;
using PodrostkiBot.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace PodrostkiBot.UI
{
    public class UIWorker
    {
        /// <summary>
        /// "Включить анон"
        /// </summary>
        public static string TxtBtAnonTurnOn { get { return "Включить анон"; } }
        /// <summary>
        /// "Отключить анон"
        /// </summary>
        public static string TxtBtAnonTurnOff { get { return "Отключить анон"; } }

        public static string TxtBtMem { get { return "Мем"; } }
        public static string TxtBtPrevious { get { return "⏮️ "; } }
        public static string TxtBtNext { get { return "⏭️ "; } }
        public static string TxtBtGold { get { return "👑 📝 👑"; } }
        public static string TxtBtStih { get { return "🎲📝🎲"; } }
        public static string TxtBtAddGoldStih { get { return "+ 👑 📝"; } }
        public static string TxtBtInfo { get { return "Инфо"; } }
        public static string TxtBtHelp { get { return "Помощь"; } }
        public static string TxtBtBible { get { return "Библия"; } }
        public static string TxtBtRename { get { return "Сменить имя"; } }
        public static string TxtBtLessons { get { return "Уроки"; } }
        public static string TxtBtFeedbacks { get { return "Отзывы"; } }
        public static string TxtBtAnonMessages { get { return "Анонимные сообщения"; } }
        public static string TxtBtOffAnswereOnAnon { get { return "Отмена отправки ответа"; } }
        public static string TxtBtWhoNextTeacher { get { return "Кто ведет урок?"; } }
        //Стандартные кнопки
        public static KeyboardButton memBut = new KeyboardButton(TxtBtMem);
        public static KeyboardButton previousBut = new KeyboardButton(TxtBtPrevious);
        public static KeyboardButton nextBut = new KeyboardButton(TxtBtNext);
        public static KeyboardButton goldBut = new KeyboardButton(TxtBtGold);
        public static KeyboardButton stihBut = new KeyboardButton(TxtBtStih);
        public static KeyboardButton addGoldStihBut = new KeyboardButton(TxtBtAddGoldStih);
        public static KeyboardButton infoBut = new KeyboardButton(TxtBtInfo);
        public static KeyboardButton helpBut = new KeyboardButton(TxtBtHelp);
        public static KeyboardButton bibleBut = new KeyboardButton(TxtBtBible);
        public static KeyboardButton renameBut = new KeyboardButton(TxtBtRename);
      
       

        //Админские кнопки
        public static KeyboardButton adminMessageBut = new KeyboardButton("vVrAdMiN8066");
        public static KeyboardButton allusersBut = new KeyboardButton("vVrAdMiN8066 allUsers");
        public static KeyboardButton setMeAdminBut = new KeyboardButton("vVrAdMiN8066 setPrivilege 1094316046 admin");
        public static KeyboardButton setMeTeenBut = new KeyboardButton("vVrAdMiN8066 setPrivilege 1094316046 teen");
        public static KeyboardButton setMeTeacherBut = new KeyboardButton("vVrAdMiN8066 setPrivilege 1094316046 teacher");
        //Кнопки Ученика
     
        public static KeyboardButton offAnswerOnAnonBut = new KeyboardButton(TxtBtOffAnswereOnAnon);


        //Кнопки преподавателя
        public static KeyboardButton feedbacksBut = new KeyboardButton(TxtBtFeedbacks);
        public static KeyboardButton lessonsBut = new KeyboardButton(TxtBtLessons);
        public static KeyboardButton anonMessagesBut = new KeyboardButton(TxtBtAnonMessages);

        public static BoardTable standartMenu
        {
            get
            {
                return new BoardTable()    {
                    //new BoardRow(){ previousBut, bibleBut, nextBut },
                    new BoardRow(){ helpBut},
                    new BoardRow(){ stihBut, goldBut }
            };
            }
        }
        public static ReplyKeyboardMarkup TeenMenu(UserI user)
        {
            KeyboardButton b;
            BoardTable t;
            if (user.InAnonMessage)
            {
                t = new BoardTable();
                t.Add(new BoardRow() { TxtBtAnonTurnOff });
            }
            else
            {
                t = standartMenu;
              
                t.Add(new BoardRow() { TxtBtAnonTurnOn });
            }
            ReplyKeyboardMarkup mrkp = new ReplyKeyboardMarkup(t);
            mrkp.ResizeKeyboard = true;
            return mrkp;
        }
        public static ReplyKeyboardMarkup AdminMenu(UserI user)
        {
            BoardTable t = standartMenu;
            t.Add(new BoardRow() { new KeyboardButton("admin users") });
            t.Add(new BoardRow() { new KeyboardButton("admin golds") });
            ReplyKeyboardMarkup mrkp = new ReplyKeyboardMarkup(t);
            mrkp.ResizeKeyboard = true;
            return mrkp;
        }
        public static ReplyKeyboardMarkup TeacherMenu(UserI user)
        {
            BoardTable t = standartMenu;
            t.Add(new BoardRow() { feedbacksBut, lessonsBut });
            t.Add(new BoardRow() { anonMessagesBut });
            ReplyKeyboardMarkup mrkp = new ReplyKeyboardMarkup(t);
            mrkp.ResizeKeyboard = true;
            return mrkp;
        }
        public static ReplyKeyboardMarkup TeacherInAnswerAnonMenu(UserI user)
        {
            BoardTable t = standartMenu;
            t.Add(new BoardRow() { feedbacksBut, lessonsBut });
            t.Add(new BoardRow() { anonMessagesBut });
            ReplyKeyboardMarkup mrkp = new ReplyKeyboardMarkup(t);
            mrkp.ResizeKeyboard = true;
            return mrkp;
        }


    }
}
