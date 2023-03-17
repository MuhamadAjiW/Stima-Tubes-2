using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.Class
{
    internal class Result
    {
        public readonly List<char> Route = new();
        public int[,] Tiles;
        public int NodesCount = 0;
        public int Steps { get => Route.Count; }
        public long Time;

        public Result(int width, int height) {
            Tiles = new int[height, width];
        }
    }
}
