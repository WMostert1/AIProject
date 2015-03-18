using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtificialIntelligenceProject1.Models
{
    public class Block
    {

        public Cell cell { get; set; }
        //-1 none
        // 0 player 1
        // 1 player 2
        public int owner { get; set; }
        public bool selected{ get; set; }
        public bool movableSpace { get; set; }
        public List<Cell> referenceCells { get; set; }
        public Block(int _x, int _y)
        {
            cell = null;
            owner = -1;
            selected = false;
            referenceCells = new List<Cell>();

        }
     
    }
}