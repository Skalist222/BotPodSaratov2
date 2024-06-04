using PodrostkiBot.App;
using PodrostkiBot.Messages;
using PodrostkiBot.UI;
using PodrostkiBot.Users;

using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace PodrostkiBot.Menus
{
    public class BaseMenu
    {
        public async void SelectMenu(Sender sender,string answer, UserI user)
        {
            answer += Environment.NewLine;
            user.MenuMessage = null;
            if (user.Privilege == UserPrivilege.Admin)
            {
                AdminMenu(sender,answer, user);
                return;
            }
            if (user.Privilege == UserPrivilege.Teacher)
            {
                TeacherMenu(sender, answer, user);
                return;
            }
            if (user.Privilege == UserPrivilege.Teen)
            {
                TeenMenu(sender,answer,user);
                return;
            }
            // Если программа дошла до сюда, значит возникла какаято проблема с определением привелегий пользователя
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            Console.WriteLine("У пользователя " + user.ToString() + " не установлена привилегия!!!");
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
        public async void TeenMenu(Sender sender, string answer, UserI user)
        {
            answer += Environment.NewLine;
            sender.SendMenu(answer, UIWorker.TeenMenu(user), user);
        }
        public async void TeacherMenu(Sender sender, string answer, UserI user)
        {
            answer +=  Environment.NewLine;
            sender.SendMenu(answer, UIWorker.TeacherMenu(user), user);

        }
        public async void AdminMenu(Sender sender, string answer, UserI user)
        {
            answer += Environment.NewLine;
   
            BoardTable board = new BoardTable();
            board.Add(new BoardRow() { UIWorker.feedbacksBut, UIWorker.adminMessageBut });
            board.Add(new BoardRow() { UIWorker.allusersBut });
            board.Add(new BoardRow() { UIWorker.setMeAdminBut });
            board.Add(new BoardRow() { UIWorker.setMeTeenBut });
            board.Add(new BoardRow() { UIWorker.setMeTeacherBut });


            ReplyKeyboardMarkup mrkp = new ReplyKeyboardMarkup(keyboard: board);
            mrkp.ResizeKeyboard = true;
            sender.SendMenu(answer, mrkp, user);
        }
    }
}
