using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.Class
{
    abstract class Algorithm
    {
        protected Map map;
        protected bool started = false;
        protected bool isTSP;
        protected readonly Dictionary<string, int> treasureCounts = new();
        protected Graph? lastTreasure;
        protected bool isTreasureDone = false;
        protected bool isTSPDone = false;

        public bool IsDone { get => isTSP ? isTSPDone : isTreasureDone; }
        public bool IsBack { get => isTSP && !isTSPDone && isTreasureDone; }

        protected Algorithm(Map map, bool isTSP = false)
        {
            this.map = map;
            this.isTSP = isTSP;
        }

        public int GetTreasureCount(string id)
        {
            int count = 0;
            for (int i = 1; i <= id.Length; i++)
            {
                if (treasureCounts.TryGetValue(id.Substring(0, i), out int n))
                    count += n;
            }
            return count;
        }

        public void GetResult(Result res, string id, string backId, Graph lastTreasure)
        {
            res.Tiles[map.StartPos.Item2, map.StartPos.Item1]++;
            Graph tile = map.Start;
            bool isBack = false;

            while (isTSP ? !isBack || tile != map.Start : tile != lastTreasure)
            {
                foreach (Location loc in Enum.GetValues(typeof(Location)))
                {
                    Graph? neighbor = tile.GetNeighbor(loc);
                    if (isBack ? neighbor?.GetBackState(backId)?.State == TileState.Visited : neighbor?.GetState(id)?.State == TileState.Visited)
                    {
                        switch (loc)
                        {
                            case Location.Left:
                                res.Route.Add('L');
                                break;
                            case Location.Right:
                                res.Route.Add('R');
                                break;
                            case Location.Top:
                                res.Route.Add('U');
                                break;
                            case Location.Bottom:
                                res.Route.Add('D');
                                break;
                        }
                        if (isBack)
                            tile.ResetBackState();
                        else
                            tile.ResetState();
                        tile = neighbor;
                        res.Tiles[neighbor.Pos.Item2, neighbor.Pos.Item1]++;
                        if (!isBack)
                            isBack = neighbor == lastTreasure;
                        break;
                    }
                }
            }
        }

        public abstract string Next();

        public abstract Result JustRun();

        public abstract void RunAndVisualize();


    }
}
