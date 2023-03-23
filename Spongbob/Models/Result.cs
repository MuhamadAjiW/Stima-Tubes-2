using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.Models
{
    public class Result
    {
        public readonly List<char> Route = new();
        public int[,] Tiles;
        public int NodesCount { get; set; } = 0;
        public int Steps { get => Route.Count; }
        public long Time { get; set; }

        public bool Found { get; set; } = true;

        public Result(int width, int height)
        {
            Tiles = new int[height, width];
        }
    }
}
