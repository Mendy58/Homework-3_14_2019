using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GamesdbLibrary;

namespace Homework_3_14_2019.Controllers
{
    public class HomeController : Controller
    {
        GamesdbManager mng = new GamesdbManager(Properties.Settings.Default.ConStr);
        public ActionResult Index()
        {
            bool? submitted = null;
            if (TempData["message"] != null)
            {
                submitted = bool.Parse((string)TempData["Message"]);
            }
            return View(submitted);
        }

        public ActionResult JoinNextGame()
        {
            Game game = mng.GetNextGame();
            return View(game);
        }
        public ActionResult AddPlayer(Player p)
        {
            mng.AddPlayer(p);
            TempData["message"] = $"true";
            return Redirect("~/Home");
        }

        public ActionResult NotificationSignUp()
        {
            return View();
        }
        public ActionResult SubmitSub(Esubscribtion sub)
        {
            mng.AddEsubscribtion(sub);
            return Redirect("~/Home");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}