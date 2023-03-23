using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.Models
{
    public class Parser
    {
        public Parser()
        {

        }

        public Map ParseFile(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            int height = lines.Length;
            List<string[]> tiles = lines.ToList().ConvertAll(line => line.Split());
            int width = tiles[0].Length;
            for (int i = 1; i < tiles.Count; i++)
            {
                if (tiles[i].Length != width)
                {
                    throw new Exception(String.Format("Different width on line {0}", i + 1));
                }
            }


            Map map = new(width, height);
            bool hasStart = false;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    string tile = tiles[y][x];
                    switch (tile)
                    {
                        case "K":
                            if (hasStart)
                                throw new Exception("More than one start tile");
                            map.SetTile(y, x, false, true);
                            hasStart = true;
                            break;
                        case "R":
                            map.SetTile(y, x, false, false);
                            break;
                        case "T":
                            map.SetTile(y, x, true, false); 
                            break;
                        case "X":
                            continue;
                        default:
                            throw new Exception(String.Format("Invalid tile code on line {0} column {1}", y, x));
                    }

                }
            }
            if (map.TreasuresCount == 0)
            {
                throw new Exception("There is no treasure on the map");
            } 

            return map;
        }
    }
}
