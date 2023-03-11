using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.Class
{
    internal class Map
    {
        private Graph?[,] tiles;
        private int width, height;
        private Vector2 start;
        private int treasureCount = 0;

        public Map(int width, int height)
        {
            tiles = new Graph?[height, width];
            this.width = width;
            this.height = height;
        }

        public void SetTile(int i, int j, bool isTreasure, bool isStart)
        {
            Graph tile = new Graph(isTreasure);
            tiles[i, j] = tile;
            if (isStart)
                start = new Vector2(j, i);
            if (isTreasure) treasureCount++;
            if (CheckValid(i-1, j))
            {
                tiles[i-1, j]!.SetNeighbor(Location.Bottom, tile);
            }
            if (CheckValid(i+1, j))
            {
                tiles[i + 1, j]!.SetNeighbor(Location.Top, tile);
            }
            if (CheckValid(i, j-1))
            {
                tiles[i, j - 1]!.SetNeighbor(Location.Right, tile);
            }
            if (CheckValid(i, j+1))
            {
                tiles[i, j + 1]!.SetNeighbor(Location.Left, tile);
            }

        }

        public void ResetState()
        {
            foreach (Graph? tile in tiles)
                if (tile != null)
                    tile.State = TileState.NotFound;
        }

        private bool CheckValid(int i, int j)
        {  
            try
            {
                return tiles[i, j] != null;
            } catch
            {
                return false;
            }
        }


    }
}
