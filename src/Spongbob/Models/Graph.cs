using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.Models
{
    public enum TileState
    {
        NotFound,
        Visited,
    }

    public enum TileView
    {
        NotVisited,
        Visited,
        BackTracked
    }
    public enum Location
    {
        Top = 0,
        Right = 1,
        Bottom = 2,
        Left = 3,
    }
    public class Graph
    {
        static int count = 0;
        public int Id;
        private readonly bool isTreasure = false;
        public Tuple<int, int> Pos;
        public TileView TileView { get; set; } = TileView.NotVisited;
        public TileState States { get; set; } = TileState.NotFound;
        public TileState BackStates { get; set; } = TileState.NotFound;
        private readonly Graph?[] neighbors =
        {
            null,
            null,
            null,
            null
        };

        public bool IsTreasure { get => isTreasure; }
        public Graph?[] Neighbors { get => neighbors; }

        public Graph(int x, int y, bool isTreasure = false)
        {
            this.isTreasure = isTreasure;
            Id = count;
            count++;
            Pos = new Tuple<int, int>(x, y);
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
