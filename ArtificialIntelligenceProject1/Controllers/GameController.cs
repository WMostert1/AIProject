using ArtificialIntelligenceProject1.Models;
using ArtificialIntelligenceProject1.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArtificialIntelligenceProject1.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult newGame(string game_name, int dimensions,int no_initial_cells, string game_type)
        {
            Debug.WriteLine(game_name + no_initial_cells + dimensions + game_type);
            GameModel gm = new GameModel(dimensions, no_initial_cells, game_name, Session.SessionID);
            GameLinker.games.Add(gm);

            return RedirectToAction("Game");
        }

        public ActionResult Click(int x, int y)
        {
            GameModel gm = GameLinker.getGame(Session.SessionID);
            gm.act(x,y,Session.SessionID);

            return PartialView("_Board", gm);
        }


        public ActionResult currentGame()
        {
            GameModel gm = GameLinker.getGame(Session.SessionID);
            return PartialView("_Board", gm);
        }

        public ActionResult joinGame(int id)
        {
            foreach(var game in GameLinker.games){
                if (game.id == id)
                {
                    if (game.player_id[0] == Session.SessionID)
                    {
                         
                        return RedirectToAction("../Home/Lobby");
                    }

                    if (game.join(Session.SessionID))
                        return  RedirectToAction("Game");
                    else 
                        return RedirectToAction("../Home/Lobby");
                }
            }
            return RedirectToAction("../Home/Lobby");
        }

        public ActionResult Game()
        {
            GameModel gm = GameLinker.getGame(Session.SessionID);
            return View("Game",gm);
        }

        public JsonResult waitTurn()
        {
            GameModel gm = GameLinker.getGame(Session.SessionID);
            Boolean turn = true;
            //if true is returned there has been no change in turns    
            if (Session.SessionID == gm.turn) return Json(turn,JsonRequestBehavior.AllowGet);
 
            //if false is returned it means that the board should be refreshed
            while (Session.SessionID != gm.turn) { }
            turn = false;
            return Json(turn, JsonRequestBehavior.AllowGet);
        }




    }
}