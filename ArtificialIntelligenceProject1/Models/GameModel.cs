using ArtificialIntelligenceProject1.Persistence;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ArtificialIntelligenceProject1.Models
{
    public class GameModel
    {
        public  CoordinateSystem coordinates{ get; set;}
        public Block blockSelected { get; set; }
        public List<Cell> cells { get; set; }
        public string turn { get; set; }
        public string name { get; set; }
        public long id { get; set; }
        public string [] player_id { get; set; }
        

        public void toggleTurn()
        {
            if (turn.Equals(player_id[1]))
                turn = player_id[0];
            else
                turn = player_id[1];
        }

        public GameModel(int dim, int no_cells, string game_name, string session_id)
        {
            player_id = new string[2];
            player_id[0] = session_id;
            turn = player_id[0];
            player_id[1] = "";
            blockSelected = null;
            coordinates = new CoordinateSystem(dim);
            cells = new List<Cell>();

            Random rnd = new Random(DateTime.Now.Second);
          
            bool found = false;
            while (!found)
            {
                long test_id = rnd.Next();
                found = true;
                foreach (GameModel g in GameLinker.games)
                {
                    if (g.id == test_id)
                    {
                        found = false;
                    }
                }
                if (found) id = test_id;
            }

            name = game_name;
            //Create player 1 cells
            for (int i = 0; i < no_cells; i++)
            {

                int x = rnd.Next(0,dim/2-1);
                int y = rnd.Next(0,dim);
                while (coordinates.get(x, y).cell != null)
                {
                     x = rnd.Next(0,dim/2-1);
                     y = rnd.Next(0, dim);
                }
                Cell cell = new Cell(0,x,y);
                cells.Add(cell);
                coordinates.get(x, y).cell = cell;
                coordinates.get(x, y).owner = 0;
                setBlockOwnership(x, y);

            }
            //Create player 2 cells
            for (int i = 0; i < no_cells; i++)
            {

                int x = rnd.Next(dim / 2 + 1, dim);
                int y = rnd.Next(0, dim);
                while (coordinates.get(x, y).cell != null)
                {
                    x = rnd.Next(dim / 2 + 1, dim);
                    y = rnd.Next(0, dim);
                }
                Cell cell = new Cell(1,x,y);
                cells.Add(cell);
                coordinates.get(x, y).cell = cell;
                coordinates.get(x, y).owner = 1;
                setBlockOwnership(x, y);
            }
           // coordinates.printDebug();
            linkBlocksQueue(0);
            linkBlocksQueue(1);
            //linkBlocks();
           // coordinates.printDebug();
           
        }

        public bool join(string session_id)
        {
            if (player_id[1].Equals(""))
            {
                player_id[1] = session_id;
                return true;
            }
            return false;
        }

        public void act(int x,int y, string req_player)
        {

            Block b = coordinates.get(x, y);
            if (blockSelected != null)
            {
                if (b == blockSelected) //if the block clicked on is the currently selected block - deselect
                {
                    blockSelected = null;
                    b.selected = false;
                    setMovableSpace(x, y, false);

                }
                else
                {
                    move(b);
                    setBlockOwnership();
                    if (req_player.Equals(player_id[0]))
                    {
                        linkBlocksQueue(0);
                        foreach (Cell c in cells)
                        {
                            if (c.player == 0) setCellOwnership(c,0);
                        }
                        linkBlocksQueue(1);
                        foreach (Cell c in cells)
                        {
                            if (c.player == 1) setCellOwnership(c, 1);
                        }
                    }
                    else if(req_player.Equals(player_id[1]))
                    {
                        linkBlocksQueue(1);
                        foreach (Cell c in cells)
                        {
                            if (c.player == 1) setCellOwnership(c, 1);
                        }
                        linkBlocksQueue(0);
                        foreach (Cell c in cells)
                        {
                            if (c.player == 0) setCellOwnership(c, 0);
                        }
                    }
                         
                    toggleTurn();
                }

            }
            else //block has not been chose yet to act on
            {
                if (b.cell != null) {
                    if (turn.Equals(req_player))
                    {
                        b.selected = true;
                        blockSelected = b;
                        setMovableSpace(x, y, true);

                    }
                }
            }

        }

        private void setMovableSpace(int x, int y, bool movable)
        {
            //top movement area
            Block b = coordinates.get(x, y);
            bool endOfArea = false;
            for (int i = 1; endOfArea == false; i++)
            {
            
                    Block possibleBlock = coordinates.get(x, y - i);
                    
                  if ( possibleBlock != null && possibleBlock.owner == b.owner)
                    {
                        possibleBlock.movableSpace = movable;
                    }
                  else{
                      endOfArea = true;
                  }
                
            }
            //bottom movement area
            endOfArea = false;
            for (int i = 1; endOfArea == false; i++)
            {
               
                    Block possibleBlock = coordinates.get(x, y + i);
                  if ( possibleBlock != null && possibleBlock.owner == b.owner)
                    {
                        possibleBlock.movableSpace = movable;
                    }
                  else{
                      endOfArea = true;
                  }
            }
            //left movement area
            endOfArea = false;
            for (int i = 1; endOfArea == false; i++)
            {
              
                    Block possibleBlock = coordinates.get(x - i, y);
                  if ( possibleBlock != null && possibleBlock.owner == b.owner)
                    {
                        possibleBlock.movableSpace = movable;
                    }
                  else{
                      endOfArea = true;
                  }
            }
            //right movement area
            endOfArea = false;
            for (int i = 1; endOfArea == false; i++)
            {
               
                    Block possibleBlock = coordinates.get(x+i,y);
                  if ( possibleBlock != null && possibleBlock.owner == b.owner)
                    {
                        possibleBlock.movableSpace = movable;
                    }
                  else{
                      endOfArea = true;
                  }
            }
        }

        private void move(Block block){
            setMovableSpace(blockSelected.cell.x, blockSelected.cell.y, false);
            resetBlockOwnershipData();
            blockSelected.selected = false;
            coordinates.swopBlocks(block, blockSelected);
            blockSelected = null;
        }



        public void setBlockOwnership()
        {
            for (int x = 0; x < coordinates.dimension; x++)
            {
                for (int y = 0; y < coordinates.dimension; y++)
                {
                    Block b = coordinates.get(x, y);
                    if (b.cell != null)
                    {
                        b.cell.resetExtends();                     
                        b.cell.linkedCells.Clear();
                        setBlockOwnership(x, y);
                    }
                }
            }
        }


        public void setBlockOwnership(int x, int y)
        {
            //TO DO: Link rectangles
            Block b = coordinates.get(x, y);

            if (b.cell != null)
            {
              
                int x_left = x - b.cell.extends_x_l;
                int x_right = x + b.cell.extends_x_r;
                int y_top = y - b.cell.extends_y_u;
                int y_bot = y + b.cell.extends_y_d;

                for (int _x = x_left; _x <= x_right; _x++)
                {
                    for (int _y = y_top; _y <= y_bot; _y++)
                    {
                        Block _b = coordinates.get(_x, _y);
                        if ( _b != null)
                        {
                            
                            _b.owner = b.owner;
                            _b.referenceCells.Add(b.cell);
                        }

                    }
                }
            }
        }

        public void setCellOwnership(Cell c, int player)
        {
            int x_left = c.x - c.extends_x_l-1;
            int x_right = c.x + c.extends_x_r+1;
            int y_top = c.y - c.extends_y_u-1;
            int y_bot = c.y + c.extends_y_d+1;
            for (int x = x_left; x <= x_right; x++)
            {
                for (int y = y_top; y <= y_bot; y++)
                {
                    Block b = coordinates.get(x,y);
                    if (b != null && b.cell != null)
                    {
                        b.cell.player = player;
                    }
                }
            }
            
        }
        

        public void resetBlockOwnershipData()
        {
            for (int x = 0; x < coordinates.dimension; x++)
            {
                for (int y = 0; y < coordinates.dimension; y++)
                {
                    
                    Block b = coordinates.get(x,y);
                    if (b.cell == null)
                    {
                        b.owner = -1;
                        b.referenceCells.Clear();
                       
                    }
                }
            }
        }

        public void linkBlocks()
        {
            
            foreach (var c in cells)
            {
                
                //right side -- START
                for (int c_y = c.y - c.extends_y_u; c_y <= c.y + c.extends_y_d; c_y++) // for each block on the right side of the covered area of c
                {
                                          //right side        //each block on right side
                Block b = coordinates.get(c.x+c.extends_x_r+1,c_y);
                if (b != null && b.owner == c.player)
                {
                    //Debug.WriteLine("Cell :" + c.x.ToString() + " " + c.y.ToString());
                    foreach (var cell in b.referenceCells)
                    {
                        if (cell.Equals(c)) continue;

                        //get corner coordinates
                        int x_left = (c.x -c.extends_x_l <= cell.x -c.extends_x_l) ? c.x - c.extends_x_l : cell.x - cell.extends_x_l;
                            if (x_left < 0) x_left = 0;                
                        int x_right = (c.x + c.extends_x_r >= cell.x + cell.extends_x_r) ? c.x + c.extends_x_r : cell.x + cell.extends_x_r;
                             if(x_right >= coordinates.dimension) x_right = coordinates.dimension-1;
                        int y_top = (c.y - c.extends_y_u <= cell.y - cell.extends_y_u) ? c.y - cell.extends_y_u : cell.y - cell.extends_y_u;
                             if (y_top <= 0) y_top = 0;
                        int y_bot = (c.y + c.extends_y_d >= cell.y + cell.extends_y_d) ? c.y + c.extends_y_d : cell.y + cell.extends_y_d;
                             if (y_bot >= coordinates.dimension) y_bot = coordinates.dimension - 1;

                        //Set ownership of rectangle
                        for (int x = x_left; x <= x_right; x++)
                        {
                            for (int y = y_top; y <= y_bot; y++)
                            {
                                Block eval = coordinates.get(x,y);
                                if (eval.owner == -1)
                                {

                                    eval.owner = c.player;
                                    eval.referenceCells.Add(cell);
                                    eval.referenceCells.Add(c);
                                    //Debug.WriteLine("\t "+x.ToString()+"/"+y.ToString()+ " anexed by "+cell.x.ToString()+"/"+cell.y.ToString() +" and " + c.x.ToString() + "/"+c.y.ToString());
                                }
                            }
                        }

                        //update extends properties of cell and c
                        int x_l = 0;
                        for (int i = 1; coordinates.get(c.x - i, c.y) != null && coordinates.get(c.x - i, c.y).owner == c.player; i++) { x_l++; }
                        int x_r = 0;
                        for (int i = 1; coordinates.get(c.x + i, c.y) != null && coordinates.get(c.x + i, c.y).owner == c.player; i++) { x_r++; }
                        int y_u = 0;
                        for (int i = 1; coordinates.get(c.x, c.y - i) != null && coordinates.get(c.x, c.y - i).owner == c.player; i++) { y_u++; }
                        int y_d = 0;
                        for (int i = 1; coordinates.get(c.x, c.y + i) != null && coordinates.get(c.x, c.y + i).owner == c.player; i++) { y_d++; }
                        c.extends_x_l = x_l;
                        c.extends_x_r = x_r;
                        c.extends_y_d = y_d;
                        c.extends_y_u = y_u;

                        x_l = 0;
                        for (int i = 1; coordinates.get(cell.x - i, c.y) != null && coordinates.get(cell.x - i, c.y).owner == cell.player; i++) { x_l++; }
                        x_r = 0;
                        for (int i = 1; coordinates.get(cell.x + i, c.y) != null && coordinates.get(cell.x + i, c.y).owner == cell.player; i++) { x_r++; }
                        y_u = 0;
                        for (int i = 1; coordinates.get(cell.x, c.y - i) != null && coordinates.get(cell.x, c.y - i).owner == cell.player; i++) { y_u++; }
                        y_d = 0;
                        for (int i = 1; coordinates.get(cell.x, c.y + i) != null && coordinates.get(cell.x, c.y + i).owner == cell.player; i++) { y_d++; }
                        cell.extends_x_l = x_l;
                        cell.extends_x_r = x_r;
                        cell.extends_y_d = y_d;
                        cell.extends_y_u = y_u;
                        
                    }
                }
                }
                //right side -- END
                
                //bottom side -- START
              /*  for (int c_x = c.x - c.extends_x_l; c_x <= c.x + c.extends_x_r; c_x++) // for each block on the bottom side of the covered area of c
                {
                                             //each block on bottom        //bottom side
                    Block b = coordinates.get( c_x,c.y + c.extends_y_d + 1);
                    if (b != null && b.owner == c.player)
                    {
                        Debug.WriteLine("Cell :" + c.x.ToString() + " " + c.y.ToString());
                        foreach (var cell in b.referenceCells)
                        {
                            if (cell.Equals(c)) continue;

                            //get corner coordinates
                            int x_left = (c.x - c.extends_x_l <= cell.x - c.extends_x_l) ? c.x - c.extends_x_l : cell.x - cell.extends_x_l;
                            if (x_left < 0) x_left = 0;
                            int x_right = (c.x + c.extends_x_r >= cell.x + cell.extends_x_r) ? c.x + c.extends_x_r : cell.x + cell.extends_x_r;
                            if (x_right >= coordinates.dimension) x_right = coordinates.dimension - 1;
                            int y_top = (c.y - c.extends_y_u <= cell.y - cell.extends_y_u) ? c.y - cell.extends_y_u : cell.y - cell.extends_y_u;
                            if (y_top <= 0) y_top = 0;
                            int y_bot = (c.y + c.extends_y_d >= cell.y + cell.extends_y_d) ? c.y + c.extends_y_d : cell.y + cell.extends_y_d;
                            if (y_bot >= coordinates.dimension) y_bot = coordinates.dimension - 1;

                            //Set ownership of rectangle
                            for (int x = x_left; x <= x_right; x++)
                            {
                                for (int y = y_top; y <= y_bot; y++)
                                {
                                    Block eval = coordinates.get(x, y);
                                    if (eval.owner == -1)
                                    {

                                        eval.owner = c.player;
                                        eval.referenceCells.Add(cell);
                                        eval.referenceCells.Add(c);
                                        Debug.WriteLine("\t " + x.ToString() + "/" + y.ToString() + " anexed by " + cell.x.ToString() + "/" + cell.y.ToString() + " and " + c.x.ToString() + "/" + c.y.ToString());
                                    }
                                }
                            }

                        }
                    }
                }
                */
                //bottom side -- END


            }
        }

        public void linkBlocksQueue(int player)
        {
           // Debug.WriteLine("Starting Rectangle Extrapolation... for player"+player);
           // Debug.WriteLine(".......................................................");
          //  printCellPositions();
            
            Queue<Cell> queue = new Queue<Cell>();
            foreach (Cell c in cells)
            {
              if(c.player == player)  queue.Enqueue(c);
            }
            
            while (queue.Count > 0)
            {
                Cell current = queue.Dequeue();
                Cell linkCell = null;

                do
                {
                    linkCell = getNeighbourToExtendTo(current);
                    if (linkCell != null)
                    {
                        linkCell.linkedCells.Add(current);
                        current.linkedCells.Add(linkCell);
                        //queue.Enqueue(current);
                       // queue.Enqueue(linkCell);

                        //get corner coordinates
                        int x_left = (current.x - current.extends_x_l <= linkCell.x - current.extends_x_l) ? current.x - current.extends_x_l : linkCell.x - linkCell.extends_x_l;
                        if (x_left < 0) x_left = 0;
                        int x_right = (current.x + current.extends_x_r >= linkCell.x + linkCell.extends_x_r) ? current.x + current.extends_x_r : linkCell.x + linkCell.extends_x_r;
                        if (x_right >= coordinates.dimension) x_right = coordinates.dimension - 1;
                        int y_top = (current.y - current.extends_y_u <= linkCell.y - linkCell.extends_y_u) ? current.y - current.extends_y_u : linkCell.y - linkCell.extends_y_u;
                        if (y_top <= 0) y_top = 0;
                        int y_bot = (current.y + current.extends_y_d >= linkCell.y + linkCell.extends_y_d) ? current.y + current.extends_y_d : linkCell.y + linkCell.extends_y_d;
                        if (y_bot >= coordinates.dimension) y_bot = coordinates.dimension - 1;

                        //update extends values
                        linkCell.extends_x_l = linkCell.x - x_left;
                        linkCell.extends_x_r = x_right - linkCell.x;
                        linkCell.extends_y_d = y_bot - linkCell.y;
                        linkCell.extends_y_u = linkCell.y - y_top;

                        current.extends_x_l = current.x - x_left;
                        current.extends_x_r = x_right - current.x;
                        current.extends_y_d = y_bot - current.y;
                        current.extends_y_u = current.y - y_top;

                        
                    //    Debug.WriteLine("---------------------");
                     //   Debug.WriteLine("linkCell :"+linkCell.x+"|"+linkCell.y);
                    //    Debug.WriteLine("current :" + current.x + "|" + current.y);
                   //     Debug.WriteLine(x_left+" "+ x_right + " in the X direction ");
                   //     Debug.WriteLine(y_top + " " + y_bot + " in the Y direction ");
                 //       Debug.WriteLine("---------------------");
                  //      Debug.WriteLine("");

                        for (int x = x_left; x <= x_right; x++)
                        {
                            for (int y = y_top; y <= y_bot; y++)
                            {
                                Block linkBlock = coordinates.get(x, y);
                                if (!linkBlock.referenceCells.Contains(linkCell)) linkBlock.referenceCells.Add(linkCell);
                                if (!linkBlock.referenceCells.Contains(current)) linkBlock.referenceCells.Add(current);

                                linkBlock.owner = current.player;

                            }
                        }
                    }

                } while (linkCell != null);
            }

           // Debug.WriteLine("Ending Rectangle Extrapolation... for player" + player);
           // Debug.WriteLine(".......................................................");

          
        }

        public Cell getNeighbourToExtendTo(Cell c)
        {
            //checks left and right sides
            for (int side_y = (c.y - c.extends_y_u); side_y <= c.y + c.extends_y_d; side_y++)
            {
                Block left = coordinates.get(c.x - c.extends_x_l - 1,side_y);
                if (left != null && left.owner == c.player)
                {
                    foreach (Cell reff in left.referenceCells)
                    {
                        if (!reff.Equals(c) && !c.linkedCells.Contains(reff)) return reff;
                    }
                }

                Block right = coordinates.get(c.x + c.extends_x_r + 1, side_y);
                if (right != null && right.owner == c.player)
                {
                    foreach (Cell reff in right.referenceCells)
                    {
                        if (!reff.Equals(c) && !c.linkedCells.Contains(reff)) return reff;
                    }
                }
            }
            //checks top and bottom sides
            for (int side_x = (c.x - c.extends_x_l); side_x <= (c.x + c.extends_x_r); side_x++)
            {
                Block top = coordinates.get(side_x,c.y-c.extends_y_u-1);
                if (top != null && top.owner == c.player)
                {
                    foreach (Cell reff in top.referenceCells)
                    {
                        if (!reff.Equals(c) && !c.linkedCells.Contains(reff)) return reff;
                    }
                }

                Block bottom = coordinates.get(side_x, c.y + c.extends_y_d + 1);
                if (bottom != null && bottom.owner == c.player)
                {
                    foreach (Cell reff in bottom.referenceCells)
                    {
                        if (!reff.Equals(c) && !c.linkedCells.Contains(reff)) return reff;

                    }
                }

            }
            return null;
        
       }


        public void printCellPositions()
        {
            Debug.WriteLine("");
            Debug.WriteLine("Cell positions: ");
            foreach(Cell c in cells){
                Debug.WriteLine(c.x+"|"+c.y);
            }
            Debug.WriteLine("");
        }




    }
}