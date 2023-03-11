using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.Class
{
    internal class Parser
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
            tiles.ForEach(tile =>
            {
                if (tile.Length != width)
                {
                    throw new Exception("Width is not uniform");
                }
            });

            Map map = new(width, height);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    string tile = tiles[y][x];
                    switch (tile)
                    {
                        case "K":
                            map.SetTile(y, x, false, true);
                            break;
                        case "R":
                            map.SetTile(y, x, false, false);
                            break;
                        case "T":
                            map.SetTile(y, x, true, false); break;
                        case "X":
                            continue;
                        default:
                            throw new Exception("Invalid tile code");
                    }

                }
            }

            return map;
        }
    }
}
