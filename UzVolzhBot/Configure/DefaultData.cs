using PodrostkiBot.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodrostkiBot.Configure
{
    public class ConstData
    {
        //public static string ConstantFilePath = Directory.GetCurrentDirectory() + "\\Constant.ini";
        public static string DefaultPath { get { return Directory.GetCurrentDirectory() + "\\Data"; } }
        public static string PathResends = DefaultPath + "\\ResendPhotos\\";
        public static string PathMems = DefaultPath + "\\Memases\\";
        public static string PathGoldStih = DefaultPath + "\\GoldenStihArray.txt";
        public static string PathUsers = DefaultPath + "\\Users.xml";

        public static string BotDB = DefaultPath + "\\BotInformation.mdf";
        public static string BibleDB = DefaultPath + "\\Bible.mdf";
        public static string GameDB = DefaultPath + "\\Game.accdb";
        public static string PathSecret = DefaultPath + "\\Secret.txt";
        public static string PathScreen = DefaultPath + "\\ScreenShot.png";

        public static string PathInfo = DefaultPath + "\\BotInfo.txt";
        public static string PathHelp = DefaultPath + "\\Help.txt";
        public static string PathHowWorkButton = DefaultPath + "\\HowWorkButton.txt";
        public static string PathHowWorkButtonTeacher = DefaultPath + "\\HowWorkButtonTeacher.txt";
        public static string PathHowWorkButtonTeen = DefaultPath + "\\HowWorkButtonTeen.txt";

        private static string BotVersion = "v. 0.4";
        public static string InfoRedactBot(UserPrivilege priv)
        {
            if (priv == UserPrivilege.Teen)
            {
                //Что я изменил у учеников
                return @$"Изменил я вот что:
1) Если ты хочешь чтобы тебе ответил определенный учитель на твои анонимные сообщения, ты можешь выбрать его!)
2) Теперь ты можешь давать оценку ответам, которые тебе приходят? Попробуй, убедишься!)
";
            }
            if (priv == UserPrivilege.Teacher)
            {
                //Что я изменил у учителей
                return @$"Изменил я вот что:
1) Появилась возможность отменять отправку ответа (если нажал 'да' на 'ответить?')
2) У учеников появилась возможность делать оценку твоим ответам (Я во всем разобрался,Остались вопросы,Ничего не понял)
";
            }
            return "";
        }
        public static string BotInfo(UserPrivilege priv)
        {
                return @$"{BotVersion}
Если нужна помощь нажми: /help
Если не понимаешь как работают кнопки: /howworkbutton

{InfoRedactBot(priv)}
Видишь какую-то проблему? Пиши Валентину, не бойся, он не кусается!)
";
            
        }

        public static string HowWorkButtonStandart 
        {
            get 
            {
                return @"
1) Кнопка ""Библия"" присылает вам меню для выбора книги, главы и стиха
2) Кнопки ""Вперед"" и ""Назад"" присылают тебе предыдущий и следующий стихи библии соответственно
3) Кнопка ""Инфо"" присылает актуальную информацию о боте
4) Кнопка ""Мем"" присылает случайный христианский мем
5) Кнопка с кубиками  листочком (🎲📝🎲) присылает абсолютно случайный стих из библии
6) Кнопка с двумя коронами и листочком (👑 📝 👑) присылает  случайный золотой стих
7) Кнопка с плюсиком, короной и листочком (+ 👑 📝) отправляет последний, полученый от бота стих в золотые стихи
";
            }
        }
        public static string HowWorkButtonTeacher
        {
            get
            {
                return @"8) Кнопка ""Отчет"" присылает отчет по отзывам от учеников по последнему уроку
9) Кнопка ""Уроки"" присылает три ближайшие урока в виде кнопок
9.1) Если на кнопке написано (хочу вести) значит можно на нее нажать и тебя поставят на эту дату
9.2) Если на кнопке написано (Ведет) и стоит чье-то имя, то при нажатии на кнопку, бот предложит поставить тебя. Если ты согласишься, Бот отправит преподавателю, которого ты сместил сообщение об этом, а тебя поставит на его место.
9.3) Если написано (Я веду), то при нажатии на кнопку, бот предложит сместить тебя с урока, и если ты соглашаешься, бот убирает тебя с этого урока.
10) Кнопка ""Анонимные сообщения"" присылает список анонимных детей, которые прислали сообщения. 
10.1) Нажав на любую из пришедших кнопок вам придут те сообщения, которые отправил анонимный ученик, и предложится ответить на данное сообщение
10.2) Когда вы нажали ""Да"", на дредложение ответить, одно следующее ваше сообщение будет отправленно адресату(Бот знает кому писать).";
            }
        }
        public static string HowWorkButtonTeen
        {
            get
            {
                return @"8) Кнопка ""Включить анон"" включает возможность отправлять анонимные сообщения. В течении таймера, можно отправлять сообщения, и они придут учителям, но они не узнают от кого эти сообщения(хехе)";            }
        }
        public static string HowWorkButton(UserPrivilege priv)
        {
            if (priv == UserPrivilege.Teen)
            {
                return HowWorkButtonStandart + HowWorkButtonTeen;
            }
            if (priv == UserPrivilege.Teacher)
            {
                return HowWorkButtonStandart + HowWorkButtonTeacher;
            }
            return HowWorkButtonStandart;
        }


    }

}


