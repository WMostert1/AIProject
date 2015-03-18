using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArtificialIntelligenceProject1.Persistence;

namespace ArtificialIntelligenceProject1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Lobby(string alias)
        {

            Session["alias"] = alias;
            ViewBag.alias = alias;
            return View("Lobby",GameLinker.games);
        }

        [HttpGet]
        public ActionResult Lobby()
        {
            
                ViewBag.status = "Your request could not be completed. You are either trying to play against yourself or someone else took the game before you!";

              ViewBag.alias = Session["alias"];
            return View("Lobby", GameLinker.games);
        }

    }
}