using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.Class
{
    internal class BFS : Algorithm
    {
        Queue<Tuple<string, Graph>> graphs = new();
        Queue<Tuple<string, Graph>> stucks = new();
        public BFS(Map map, bool isTSP) : base(map, isTSP)
        {
        }

        public override Result JustRun()
        {
            Result res = new(map.Width, map.Height);
            string id = "0";
            string backId = "";
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            while (!IsDone)
            {
                if (!isTreasureDone && graphs.TryPeek(out var graph))
                    lastTreasure = graph.Item2;
                if (IsBack)
                {
                    backId = Next();
                }
                else id = Next();
                res.NodesCount++;
            }
            watch.Stop();
            res.Tiles[map.StartPos.Item2, map.StartPos.Item1]++;

            res.Time = watch.ElapsedMilliseconds;
            Graph tile = map.Start;
            bool isBack = false;
            Debug.WriteLine(backId);

            while (isTSP ? !isBack || tile != map.Start : tile != lastTreasure)
            {
                foreach(Location loc in Enum.GetValues(typeof(Location)))
                {
                    Graph? neighbor = tile.GetNeighbor(loc);
                    if (isBack ? neighbor?.GetBackState(backId)?.State == TileState.Visited : neighbor?.GetState(id)?.State == TileState.Visited)
                    {
                        switch(loc)
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
           
            return res;
        }

        public override string Next()
        0.{
            if (!started)
            {
                started = true;
                graphs.Enqueue(new Tuple<string, Graph>("0", map.Start));
            }
            graphs.TryDequeue(out var el);
            if (el == null)
            {
                stucks.TryDequeue(out el);
            }
            if (el == null)
            {
                throw new Exception("No solution");
            }
            Graph tile = el.Item2;
            string id = el.Item1;
            if (IsBack)
                tile.SetBackState(id, TileState.Visited);
            else tile.SetState(id, TileState.Visited);

            if (!IsBack && tile.IsTreasure)
            {
                treasureCounts.TryAdd(id, 0);
                treasureCounts[id]++;
                if (GetTreasureCount(id) == map.TreasuresCount)
                {
                    isTreasureDone = true;
                    if (isTSP)
                    {
                        graphs.Clear();
                        graphs.Enqueue(new Tuple<string, Graph>(id, tile));
                    }
                    return id;
                }
            } else if (IsBack && tile == map.Start)
            {
                isTSPDone = true;
                return id;
            }
            bool stuck = false;
            List<Graph?> neighbors = tile.Neighbors.ToList().Where(t => {
                if (t == null) return false;
                if (IsBack && t.GetBackState(id)?.State == TileState.Visited) return false;
                if (IsBack && t == map.Start) return true;
                return t.GetState(id)?.State != TileState.Visited;
            }).ToList();
            if (neighbors.Count == 0 && IsBack)
            {
                stuck = true;
                neighbors = tile.Neighbors.ToList().Where(t => t != null && t.GetBackState(id)?.State != TileState.Visited).ToList();
            }
            Queue<Tuple<string, Graph>> q = stuck ? stucks : graphs;
            int neighborsCount = neighbors.Count;
            //if (IsBack)
            //{
            //    Debug.Write("id: ");
            //    Debug.Write(id);
            //    Debug.WriteLine($" {neighborsCount} neighbors");
            //}

            if (neighborsCount == 1)
            {
                q.Enqueue(new Tuple<string, Graph>(id, neighbors[0]!));
                return id;
            }
            int i = 0;
            foreach (var n in neighbors)
            {
                q.Enqueue(new Tuple<string, Graph>(id + i, n!));
                ++i;
            }

            return id;
        }

        public override void RunAndVisualize()
        {
            throw new NotImplementedException();
        }
    }
}
