using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.Class
{
    enum TileState
    {
        NotFound,
        Visited,
        BackTracked,
    }

    enum Location
    {
        Top = 0,
        Right = 1,
        Bottom = 2,
        Left = 3,
    }
    internal class Graph 
    {
        private bool isTreasure = false;
        public TileState State = TileState.NotFound;
        private readonly Graph?[] neighbors =
        {
            null,
            null,
            null,
            null
        };

        public bool IsTreasure { get => isTreasure; }
        public Graph?[] Neighbors { get => neighbors; }

        public Graph(bool isTreasure = false)
        {
            this.isTreasure = isTreasure;
        }

        public Graph(Graph? top, Graph? right, Graph? bottom, Graph? left, bool isTreasure = false)
        {
            neighbors[0] = top;
            neighbors[1] = right;
            neighbors[2] = bottom;
            neighbors[3] = left;
            this.isTreasure = isTreasure;
        }

        public Graph? GetNeighbor(Location loc)
        {
            return neighbors[(int)loc];
        }

        public void SetNeighbor(Location loc, Graph? neighbor)
        {
            neighbors[(int)loc] = neighbor;
        }
        
    }
}
