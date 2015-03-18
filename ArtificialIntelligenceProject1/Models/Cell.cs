using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtificialIntelligenceProject1.Models
{
    public class Cell
    {
        public int extends_x_l { get; set; }
        public int extends_x_r { get; set; }
        public int extends_y_u { get; set; }
        public int extends_y_d { get; set; }
        public List<Cell> linkedCells { get; set; }
        public int x { get; set; }
        public int y { get; set; }

        //0  : player 1
        //1  : player 2
        public int player { get; set; }

        public Cell(int p, int _x, int _y)
        {
            player = p;
            extends_x_l = 1;
            extends_y_u = 1;
            extends_x_r = 1;
            extends_y_d = 1;
            x = _x;
            y = _y;
            linkedCells = new List<Cell>();

        }

        public void resetExtends()
        {
            extends_x_l = 1;
            extends_y_u = 1;
            extends_x_r = 1;
            extends_y_d = 1;
        }
    }
}