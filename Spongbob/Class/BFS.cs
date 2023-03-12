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
            res.Time = watch.ElapsedMilliseconds;
            GetResult(res, id, backId, lastTreasure!);
           
            return res;
        }

        public override string Next()
        {
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
            List<Graph?> neighbors = tile.Neighbors.ToList().Where(t => {
                if (t == null) return false;
                if (IsBack && t.GetBackState(id)?.State == TileState.Visited) return false;
                if (IsBack && t == map.Start) return true;
                return t.GetState(id)?.State != TileState.Visited;
            }).ToList();
            int neighborsCount = neighbors.Count;
            int startI = 0;
            if (IsBack)
            {
                List<Graph?> neighborsStuck = tile.Neighbors.ToList().Where(t =>
                t != null && t.GetBackState(id)?.State != TileState.Visited &&
                t.GetState(id)?.State == TileState.Visited).ToList();
                neighborsCount += neighborsStuck.Count;
                AddToQueue(stucks, neighborsStuck, id, neighborsCount, startI);
                startI += neighborsStuck.Count;
            }
            AddToQueue(graphs, neighbors, id, neighborsCount, startI);
            return id;
        }

        static void AddToQueue(Queue<Tuple<string, Graph>> q, List<Graph?> neighbors, string id, int count, int startI)
        {
            if (neighbors.Count == 0) return;

            if (count == 1)
            {
                q.Enqueue(new Tuple<string, Graph>(id, neighbors[0]!));
                return;
            }
            foreach (var n in neighbors)
            {
                q.Enqueue(new Tuple<string, Graph>(id + startI, n!));
                ++startI;
            }
        }

        public override void RunAndVisualize()
        {
            throw new NotImplementedException();
        }
    }
}
