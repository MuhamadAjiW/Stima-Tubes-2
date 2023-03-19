using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.Models
{
    public class Map
    {
        private Graph?[,] tiles;
        public int Width, Height;
        public Tuple<int, int> StartPos = new(0, 0);
        public Graph Start { get => tiles[StartPos.Item2, StartPos.Item1]!; }
        public int TreasuresCount = 0;

        public Map(int width, int height)
        {
            tiles = new Graph?[height, width];
            this.Width = width;
            this.Height = height;
        }

        public void SetTile(int i, int j, bool isTreasure, bool isStart)
        {
            Graph tile = new(j, i, isTreasure);
            if (isTreasure)
                TreasuresCount++;
            tiles[i, j] = tile;
            if (isStart)
                StartPos = new Tuple<int, int>(j, i);
            if (CheckValid(i-1, j))
            {
                tiles[i-1, j]!.SetNeighbor(Location.Bottom, tile);
                tile.SetNeighbor(Location.Top, tiles[i - 1, j]!);
            }
            if (CheckValid(i+1, j))
            {
                tiles[i + 1, j]!.SetNeighbor(Location.Top, tile);
                tile.SetNeighbor(Location.Bottom, tiles[i + 1, j]!);
            }
            if (CheckValid(i, j-1))
            {
                tiles[i, j - 1]!.SetNeighbor(Location.Right, tile);
                tile.SetNeighbor(Location.Left, tiles[i, j - 1]!);
            }
            if (CheckValid(i, j+1))
            {
                tiles[i, j + 1]!.SetNeighbor(Location.Left, tile);
                tile.SetNeighbor(Location.Right, tiles[i, j + 1]!);
            }

        }

        public void ResetState()
        {
            foreach (Graph? tile in tiles)
                tile?.ResetState();
        }

        private bool CheckValid(int i, int j)
        {  

                return i >= 0 && i < Height && j >= 0 && j < Width && tiles[i, j] != null;
        }

        

        public void Print()
        {
            for (int i = 0; i < Height; i++)
            {
                for(int j = 0;  j < Width; j++)
                {
                    Graph? tile = tiles[i, j];
                    if (i == StartPos.Item2 && j == StartPos.Item1)
                    {
                        Debug.Write("S");
                    }
                    else if (tile == null)
                    {

                        Debug.Write("X");
                    }
                    else if (tile.IsTreasure)
                    {
                        Debug.Write("T");
                    } else
                    {
                        Debug.Write("R");
                    }
                    Debug.Write(" ");

                }
                Debug.Write("\n");
            }
        }


    }
}
