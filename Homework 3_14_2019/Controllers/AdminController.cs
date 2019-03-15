using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GamesdbLibrary;

namespace Homework_3_14_2019.Controllers
{
    public class AdminController : Controller
    {
        GamesdbManager mng = new GamesdbManager(Properties.Settings.Default.ConStr);
        public ActionResult Index()
        {          
            return View();
        }
        public ActionResult ViewGames()
        {
            IEnumerable<Game> games = mng.GetGames(false);
            return View(games);
        }
        [HttpPost]
        public ActionResult ViewGameDetails(int id)
        {
           IEnumerable<Player> players = mng.GetPlayers(id);
           return View(players);
        }
        [HttpPost]
        public ActionResult AddGame(Game game)
        {
            mng.AddGame(game);
            return Redirect("/admin");
        }
    }
}