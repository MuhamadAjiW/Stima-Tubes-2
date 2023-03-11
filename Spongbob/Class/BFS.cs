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
        public BFS(Map map, bool isTSP) : base(map, isTSP)
        {
        }

        public override Result JustRun()
        {
            Result res = new(map.Width, map.Height);
            string id = "0";
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            while (!IsDone)
            {
                if (!isTreasureDone && graphs.TryPeek(out var graph))
                    lastTreasure = graph.Item2;
                id = Next();
                res.NodesCount++;
                // Debug.WriteLine(res.NodesCount);
            }
            watch.Stop();
            Tuple<int, int> currentPos = map.StartPos;
            res.Tiles[currentPos.Item2, currentPos.Item2]++;

            res.Time = watch.ElapsedMilliseconds;
            Graph tile = map.Start;
            bool isBack = false;
            int treasureCount = 0;
            Debug.WriteLine(id);

            while (isTSP ? !isBack || tile != map.Start : treasureCount < map.TreasuresCount)
            {
                bool visit = false;
                foreach(Location loc in Enum.GetValues(typeof(Location)))
                {
                    Graph? neighbor = tile.GetNeighbor(loc);
                    Debug.WriteLineIf(neighbor != null, neighbor?.Pos.ToString());
                    Debug.WriteLineIf(neighbor != null, neighbor?.GetState(id).State);
                    if (isBack ? neighbor?.GetBackState(id).State == TileState.Visited : neighbor?.GetState(id).State == TileState.Visited)
                    {
                        visit = true;
                        switch(loc)
                        {
                            case Location.Left:
                                currentPos = new Tuple<int, int>(currentPos.Item1 - 1, currentPos.Item2); 
                                res.Route.Add('L');
                                break;
                            case Location.Right:
                                currentPos = new Tuple<int, int>(currentPos.Item1 + 1, currentPos.Item2);
                                res.Route.Add('R');
                                break;
                            case Location.Top:
                                currentPos = new Tuple<int, int>(currentPos.Item1, currentPos.Item2 - 1);
                                res.Route.Add('U');
                                break;
                            case Location.Bottom:
                                currentPos = new Tuple<int, int>(currentPos.Item1, currentPos.Item2 + 1);
                                res.Route.Add('D');
                                break;
                        }
                        tile.ResetState();
                        tile = neighbor;
                        res.Tiles[currentPos.Item2, currentPos.Item2]++;
                        if (neighbor.IsTreasure) treasureCount++;
                        if (!isBack)
                            isBack = neighbor == lastTreasure;
                        break;
                    }
                }
                if (!visit) break;
            }
           
            return res;
        }

        public override string Next()
        {
            if (!started)
            {
                started = true;
                graphs.Enqueue(new Tuple<string, Graph>("0", map.Start));
            }
            var el = graphs.Dequeue();
            Graph tile = el.Item2;
            string id = el.Item1;
            Debug.WriteLine(id);
            if (IsBack)
                tile.GetBackState(id).State = TileState.Visited;
            else tile.GetState(id).State = TileState.Visited;

            if (tile.IsTreasure)
            {
                treasureCounts.TryAdd(id, 0);
                treasureCounts[id]++;
                if (GetTreasureCount(id) == map.TreasuresCount)
                {
                    Debug.WriteLine("Done");
                    isTreasureDone = true;
                    if (isTSP)
                    {
                        graphs.Clear();
                        graphs.Enqueue(new Tuple<string, Graph>("0", tile));
                    }
                    return id;
                }
            }
            List<Graph?> neighbors = tile.Neighbors.ToList().Where(t => {
                if (t == null) return false;
                if (IsBack && t.GetBackState(id).State == TileState.Visited) return false;
                return t.GetState(id).State != TileState.Visited;

            }).ToList();
            if (neighbors.Count == 0 && IsBack)
            {
                neighbors = tile.Neighbors.ToList().Where(t => t != null && t.GetBackState(id).State != TileState.Visited).ToList();
            }
            int neighborsCount = neighbors.Count;

            if (neighborsCount == 1)
            {
                graphs.Enqueue(new Tuple<string, Graph>(id, neighbors[0]!));
                return id;
            }

            int i = 0;
            foreach (var n in neighbors)
            {
                graphs.Enqueue(new Tuple<string, Graph>(id + i, n!));
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
