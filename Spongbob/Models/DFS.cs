using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Spongbob.Models
{
    public class DFS : Algorithm
    {
        private readonly Stack<Tuple<string, Graph>> graphsprio1 = new();
        private readonly Stack<Tuple<string, Graph>> graphsprio2 = new();
        private readonly Stack<Tuple<string, Graph>> stucks = new();

        public DFS(Map map, bool isTSP) : base(map, isTSP)
        {
        }

        public override void Initialize()
        {
            graphsprio1.Clear();
            graphsprio2.Clear();
            started = true;
            treasureCounts = 0;
            nonTSPRoute.Clear();
            map.ResetState();
            graphsprio1.Push(new Tuple<string, Graph>("S", map.Start));
        }

        public override Tuple<string, bool> Next(string previous)
        {
            int loc = 1;
            graphsprio1.TryPeek(out var el);
            if (el == null)
            {
                int n = graphsprio2.Count();
                for (int i = n - 1; i >= 0; i--)
                {
                    graphsprio1.Push(graphsprio2.ElementAt(i));
                }
                graphsprio2.Clear();
                graphsprio1.TryPeek(out el);

                if (el == null)
                {
                    loc = 2;
                    stucks.TryPeek(out el);
                }
            }
            if (el == null)
            {
                throw new Exception("No solution");
            }

            if (IsBack ? el.Item2.BackStates == TileState.Visited : el.Item2.States == TileState.Visited)
            {
                switch (loc)
                {
                    case 1:
                        graphsprio1.TryPop(out el);
                        break;
                    case 2:
                        stucks.TryPop(out el);
                        break;
                }
                return Next(previous);
            }

            string id = el.Item1;

            if (!id.StartsWith(previous))
            {
                return new Tuple<string, bool>(previous.Substring(0, previous.Length - 1), false);
            }
            if (id.Length > previous.Length + 1)
            {
                return new Tuple<string, bool>(id.Substring(0, previous.Length + 1), false);
            }

            switch (loc)
            {
                case 1:
                    graphsprio1.TryPop(out el);
                    break;
                case 2:
                    stucks.TryPop(out el);
                    break;
            }

            Graph tile = el!.Item2;

            if (IsBack)
                tile.BackStates = TileState.Visited;
            else tile.States = TileState.Visited;

            if (!IsBack && tile.IsTreasure)
            {
                int len1 = graphsprio2.Count();
                int len2 = graphsprio1.Count();

                for (int i = 0; i < len1; i++)
                {
                    graphsprio2.TryPop(out var duplicate);
                    stucks.Push(RefactorRoute(duplicate!, id));
                }

                for (int i = 0; i < len1; i++)
                {
                    stucks.TryPop(out var duplicate);
                    graphsprio2.Push(duplicate!);
                }

                for (int i = len2 - 1; i >= 0; i--)
                {
                    graphsprio2.Push(RefactorRoute(graphsprio1.ElementAt(i), id));
                }
                graphsprio1.Clear();

                treasureCounts++;

                if (treasureCounts == map.TreasuresCount)
                {
                    isTreasureDone = true;
                    if (isTSP)
                    {
                        graphsprio1.Clear();
                        graphsprio2.Clear();
                        graphsprio1.Push(new Tuple<string, Graph>(id, tile));
                    }
                    return new Tuple<string, bool>(id, true);
                }

            }
            else if (IsBack && tile == map.Start)
            {
                isTSPDone = true;
                return new Tuple<string, bool>(id, true);
            }

            List<Tuple<Graph?, int>> neighborsData = new();
            for (int i = tile.Neighbors.Length - 1; i >= 0; i--)
            {
                Graph? candidate = tile.Neighbors[i];
                if (candidate == null) continue;
                if (IsBack && candidate?.BackStates == TileState.Visited) continue;
                if (IsBack && candidate == map.Start) neighborsData.Add(new Tuple<Graph?, int>(candidate, i));
                if (candidate?.States != TileState.Visited) neighborsData.Add(new Tuple<Graph?, int>(candidate, i));
            }

            int neighborsCount = neighborsData.Count;

            if (IsBack)
            {
                List<Tuple<Graph?, int>> neighborsStuckData = new();
                for (int i = tile.Neighbors.Length - 1; i >= 0; i--)
                {
                    Graph? candidate = tile.Neighbors[i];
                    if (candidate != null &&
                        candidate?.BackStates != TileState.Visited &&
                        candidate?.States == TileState.Visited
                    ) neighborsStuckData.Add(new Tuple<Graph?, int>(tile.Neighbors[i], i));
                }

                neighborsCount += neighborsStuckData.Count;

                AddToStack(stucks, neighborsStuckData, id, neighborsCount);
            }

            AddToStack(graphsprio1, neighborsData, id, neighborsCount);

            return new Tuple<string, bool>(id, true);
        }

        static void AddToStack(Stack<Tuple<string, Graph>> q, List<Tuple<Graph?, int>> neighborsData, string id, int count)
        {
            if (neighborsData.Count == 0) return;

            foreach (var n in neighborsData)
            {
                q.Push(new Tuple<string, Graph>(id + n.Item2, n.Item1!));
            }
        }

        public override Tuple<String, Graph, Graph, bool> NextVisualize(string previous, Graph previousTile)
        {
            int loc = 1;
            graphsprio1.TryPeek(out var el);
            if (el == null)
            {
                int n = graphsprio2.Count();
                for (int i = n - 1; i >= 0; i--)
                {
                    graphsprio1.Push(graphsprio2.ElementAt(i));
                }
                graphsprio2.Clear();
                graphsprio1.TryPeek(out el);


                if (el == null)
                {
                    loc = 2;
                    stucks.TryPeek(out el);
                }
            }
            if (el == null)
            {
                throw new Exception("No solution");
            }

            if (IsBack ? el.Item2.BackStates == TileState.Visited : el.Item2.States == TileState.Visited)
            {
                switch (loc)
                {
                    case 1:
                        graphsprio1.TryPop(out el);
                        break;
                    case 2:
                        stucks.TryPop(out el);
                        break;
                }
                return NextVisualize(previous, previousTile);
            }

            string id = el.Item1;
            Graph tile;

            if (!id.StartsWith(previous) && id != previous)
            {
                tile = GetGraphStep(previous[previous.Length - 1], previousTile, true);

                if (IsBack)
                {
                    tile.BackStates = TileState.Visited;
                }
                else
                {
                    tile.States = TileState.Visited;
                }

                if (!nonTSPRoute.Contains(previousTile.Pos))
                    previousTile.TileView = TileView.BackTracked;

                return new Tuple<string, Graph, Graph, bool>(previous.Substring(0, previous.Length - 1), tile, previousTile, false);
            }
            if (id.Length > previous.Length + 1)
            {
                previousTile.TileView = TileView.Visited;
                tile = GetGraphStep(id[previous.Length], previousTile, false);

                if (IsBack)
                {
                    tile.BackStates = TileState.Visited;
                }
                else
                {
                    tile.States = TileState.Visited;
                }

                return new Tuple<string, Graph, Graph, bool>(id.Substring(0, previous.Length + 1), tile, previousTile, false);
            }


            switch (loc)
            {
                case 1:
                    graphsprio1.TryPop(out el);
                    break;
                case 2:
                    stucks.TryPop(out el);
                    break;
            }

            tile = el!.Item2;


            previousTile.TileView = TileView.Visited;
            tile.TileView = TileView.Visited;
            if (IsBack)
                tile.BackStates = TileState.Visited;
            else tile.States = TileState.Visited;

            if (!IsBack && tile.IsTreasure)
            {
                SetNonTSPRoute(id);

                int len1 = graphsprio2.Count();
                int len2 = graphsprio1.Count();

                for (int i = 0; i < len1; i++)
                {
                    graphsprio2.TryPop(out var duplicate);
                    stucks.Push(RefactorRoute(duplicate!, id));
                }

                for (int i = 0; i < len1; i++)
                {
                    stucks.TryPop(out var duplicate);
                    graphsprio2.Push(duplicate!);
                }

                for (int i = len2 - 1; i >= 0; i--)
                {
                    graphsprio2.Push(RefactorRoute(graphsprio1.ElementAt(i), id));
                }
                graphsprio1.Clear();

                treasureCounts++;

                if (treasureCounts == map.TreasuresCount)
                {
                    isTreasureDone = true;
                    if (isTSP)
                    {
                        graphsprio1.Clear();
                        graphsprio2.Clear();
                        graphsprio1.Push(new Tuple<string, Graph>(id, tile));
                    }
                    return new Tuple<string, Graph, Graph, bool>(id, tile, previousTile, true);
                }

            }
            else if (IsBack && tile == map.Start)
            {
                isTSPDone = true;
                return new Tuple<string, Graph, Graph, bool>(id, tile, previousTile, true);
            }

            List<Tuple<Graph?, int>> neighborsData = new();
            for (int i = tile.Neighbors.Length - 1; i >= 0; i--)
            {
                Graph? candidate = tile.Neighbors[i];
                if (candidate == null) continue;
                if (IsBack && candidate?.BackStates == TileState.Visited) continue;
                if (IsBack && candidate == map.Start) neighborsData.Add(new Tuple<Graph?, int>(candidate, i));
                if (candidate?.States != TileState.Visited) neighborsData.Add(new Tuple<Graph?, int>(candidate, i));
            }

            int neighborsCount = neighborsData.Count;

            if (IsBack)
            {
                List<Tuple<Graph?, int>> neighborsStuckData = new();
                for (int i = tile.Neighbors.Length - 1; i >= 0; i--)
                {
                    Graph? candidate = tile.Neighbors[i];
                    if (candidate != null &&
                        candidate?.BackStates != TileState.Visited &&
                        candidate?.States == TileState.Visited
                    ) neighborsStuckData.Add(new Tuple<Graph?, int>(tile.Neighbors[i], i));
                }

                neighborsCount += neighborsStuckData.Count;

                AddToStack(stucks, neighborsStuckData, id, neighborsCount);
            }

            AddToStack(graphsprio1, neighborsData, id, neighborsCount);

            return new Tuple<string, Graph, Graph, bool>(id, tile, previousTile, true);
        }
    }
}
