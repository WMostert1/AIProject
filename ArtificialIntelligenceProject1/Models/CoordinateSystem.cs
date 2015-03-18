using ArtificialIntelligenceProject1.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ArtificialIntelligenceProject1.Models
{

    /// <summary>
    ///   x0y0 x1y0 x2y0 x3y0 
    ///   x0y1 x1y1 x2y1 x3y1
    ///   x0y2 x1y2 x2y2 x3y2
    ///   x0y3 x1y3 x2y3 x3y3
    /// </summary>

    public class CoordinateSystem
    {
        public int dimension { get; set; }
        public List<List<Block>> coords  { get; set; }
        


        public CoordinateSystem(int dim)
        {
            dimension = dim;
            coords = new List<List<Block>>();
            for (int i = 0; i < dim; i++)
            {
                coords.Add(new List<Block>());
            }


            foreach(var c in coords){
                for (int i = 0; i < dim; i++)
                {
                    int j = 0;
                    c.Add(new Block(i,j));
                    j++;
                }
            }
            

        }

        public void swopBlocks(Block a, Block b)
        {
            int [] xy_a = getCoordinates(a);
            int[] xy_b = getCoordinates(b);

            coords.ElementAt(xy_a[0])[xy_a[1]] = b;
            if (coords.ElementAt(xy_a[0]).ElementAt(xy_a[1]).cell != null)
            {
                coords.ElementAt(xy_a[0]).ElementAt(xy_a[1]).cell.x = xy_a[0];
                coords.ElementAt(xy_a[0]).ElementAt(xy_a[1]).cell.y = xy_a[1];
            }
            coords.ElementAt(xy_b[0])[xy_b[1]] = a;
            if(coords.ElementAt(xy_b[0]).ElementAt(xy_b[1]).cell != null){
                 coords.ElementAt(xy_b[0]).ElementAt(xy_b[1]).cell.x = xy_b[0];
                 coords.ElementAt(xy_b[0]).ElementAt(xy_b[1]).cell.y = xy_b[1];
            }
        }

        public int[] getCoordinates(Block b)
        {
            int[] result = new int[2];
            for (int y = 0; y < dimension; y++) {
                for (int x = 0; x < dimension; x++)
                {
                    if (get(x, y) == b)
                    {
                        result[0] = x;
                        result[1] = y;
                        return result;
                    }
                }
            }
            return null;
        }





        public Block get(int x, int y)
        {
          
               if(x < 0 || x >= dimension || y < 0 || y >= dimension) return null;


                return coords.ElementAt(x).ElementAt(y);
          
        }

        public void printDebug()
        {
            for (int y = 0; y < dimension; y++)
            {
                
                for (int x = 0; x < dimension; x++)
                {
                    Block b = get(x, y);
                    if (b.cell == null)
                    {
                        
                        if(b.owner == -1)
                            Debug.Write("*");
                        else if(b.owner == 0)
                            Debug.Write("a");
                        else if (b.owner == 1)
                           Debug.Write("b");
                           
                    }
                    else if (b.cell.player == 0)
                    {
                        Debug.Write("1");
                    }
                    else if (b.cell.player == 1)
                    {
                        Debug.Write("2");
                    }
                    
                }
                Debug.WriteLine("");
            }
        }

       
    }
}