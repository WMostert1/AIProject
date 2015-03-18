using ArtificialIntelligenceProject1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace ArtificialIntelligenceProject1.Persistence
{
    public static class GameLinker
    {
        public static List<GameModel> games;
        

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static GameModel joinGame(int game_id,string session_id)
        {
            return null;
        }

        public static GameModel getGame(int game_id){
            foreach(var game in games){
                if(game.id == game_id) return game;
            }
            return null;
        }

        public static GameModel getGame(string session_id){
            foreach (var game in games)
            {
                if (game.player_id[0] == session_id || game.player_id[1] == session_id) return game;
            }
            return null;
        }
    }
}