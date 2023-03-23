using System;
using System.Collections.Generic;
using System.Linq;

namespace Spongbob.Models
{
    public class BFS : Algorithm
    {
        private readonly Queue<Tuple<string, Graph>> graphsprio1 = new();
        private readonly Queue<Tuple<string, Graph>> graphsprio2 = new();
        private readonly Queue<Tuple<string, Graph>> stucks = new();
        public BFS(Map map, bool isTSP) : base(map, isTSP)
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
            graphsprio1.Enqueue(new Tuple<string, Graph>("S", map.Start));
        }

        public override Tuple<string, bool> Next(string previous)
        {
            // Check next destination from queue

            int loc = 1;
            graphsprio1.TryPeek(out var el);
            if (el == null)
            {
                // If first priority queue is empty, move second priority queue into the first one then check from the first one

                int n = graphsprio2.Count();
                for (int i = 0; i < n; i++)
                {
                    graphsprio1.Enqueue(graphsprio2.ElementAt(i));
                }
                graphsprio2.Clear();
                graphsprio1.TryPeek(out el);

                if (el == null)
                {
                    // If second queue is empty, check from stucks

                    loc = 2;
                    stucks.TryPeek(out el);
                }
            }
            if (el == null)
            {
                // If stucks is empty, no next destination

                throw new Exception("No solution");
            }

            if (IsBack ? el.Item2.BackStates == TileState.Visited : el.Item2.States == TileState.Visited)
            {
                // If next destination has been visited, remove destination from queue without doing anything

                switch (loc)
                {
                    case 1:
                        graphsprio1.TryDequeue(out el);
                        break;
                    case 2:
                        stucks.TryDequeue(out el);
                        break;
                }
                return Next(previous);
            }

            string id = el.Item1;

            // Dequeue tile if everything else is valid
            switch (loc)
            {
                case 1:
                    graphsprio1.TryDequeue(out el);
                    break;
                case 2:
                    stucks.TryDequeue(out el);
                    break;
            }

            Graph tile = el!.Item2;

            // Turn visited flags on for next tile
            if (IsBack)
                tile.BackStates = TileState.Visited;
            else tile.States = TileState.Visited;

            if (!IsBack && tile.IsTreasure)
            {
                int len1 = graphsprio1.Count();
                int len2 = graphsprio2.Count();

                // If next tile contains a treasure, refactor all routes to include backtracking steps from the treasure tile
                for (int i = 0; i < len2; i++)
                {
                    graphsprio2.TryDequeue(out var duplicate);
                    graphsprio2.Enqueue(RefactorRoute(duplicate!, id));
                }

                // Move first priority queue into the second one to prioritize routesnimize backtracking
                for (int i = 0; i < len1; i++)
                {
                    graphsprio2.Enqueue(RefactorRoute(graphsprio1.ElementAt(i), id));
                }
                graphsprio1.Clear();

                treasureCounts++;

                if (treasureCounts == map.TreasuresCount)
                {
                    // Every Treasure has been found, mark the algorithm as done
                    isTreasureDone = true;
                    if (isTSP)
                    {
                        // If TSP is required, clear every queue and restart the algorithm with start tile as the goal
                        graphsprio1.Clear();
                        graphsprio2.Clear();
                        graphsprio1.Enqueue(new Tuple<string, Graph>(id, tile));
                    }
                    return new Tuple<string, bool>(id, true);
                }


            }
            else if (IsBack && tile == map.Start)
            {
                // Start found, mark the algorithm as done
                isTSPDone = true;
                return new Tuple<string, bool>(id, true);
            }

            // Add non null neighbors to the queue
            // If TSP, skip visited tiles to prioritize routes with minimal backtracking
            List<Tuple<Graph?, int>> neighborsData = new();
            for (int i = 0; i < tile.Neighbors.Length; i++)
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
                // If TSP, load visited tiles to lower priority queue to prioritize routes with minimal backtracking
                List<Tuple<Graph?, int>> neighborsStuckData = new();
                for (int i = 0; i < tile.Neighbors.Length; i++)
                {
                    Graph? candidate = tile.Neighbors[i];
                    if (candidate != null &&
                        candidate?.BackStates != TileState.Visited &&
                        candidate?.States == TileState.Visited
                    ) neighborsStuckData.Add(new Tuple<Graph?, int>(tile.Neighbors[i], i));
                }

                neighborsCount += neighborsStuckData.Count;

                AddToQueue(stucks, neighborsStuckData, id, neighborsCount);
            }

            AddToQueue(graphsprio1, neighborsData, id, neighborsCount);

            return new Tuple<string, bool>(id, true);
        }

        static void AddToQueue(Queue<Tuple<string, Graph>> q, List<Tuple<Graph?, int>> neighborsData, string id, int count)
        {
            if (neighborsData.Count == 0) return;

            foreach (var n in neighborsData)
            {
                q.Enqueue(new Tuple<string, Graph>(id + n.Item2, n.Item1!));
            }
        }

        public override Tuple<String, Graph, Graph, bool> NextVisualize(string previous, Graph previousTile)
        {
            // Check next destination from queue

            int loc = 1;
            graphsprio1.TryPeek(out var el);
            if (el == null)
            {
                // If first priority queue is empty, move second priority queue into the first one then check from the first one

                int n = graphsprio2.Count();
                for (int i = 0; i < n; i++)
                {
                    graphsprio1.Enqueue(graphsprio2.ElementAt(i));
                }
                graphsprio2.Clear();
                graphsprio1.TryPeek(out el);

                if (el == null)
                {
                    // If second queue is empty, check from stucks

                    loc = 2;
                    stucks.TryPeek(out el);
                }
            }
            if (el == null)
            {
                // If stucks is empty, no next destination

                throw new Exception("No solution");
            }

            if (IsBack ? el.Item2.BackStates == TileState.Visited : el.Item2.States == TileState.Visited)
            {
                // If next destination has been visited, remove destination from queue without doing anything

                switch (loc)
                {
                    case 1:
                        graphsprio1.TryDequeue(out el);
                        break;
                    case 2:
                        stucks.TryDequeue(out el);
                        break;
                }
                return NextVisualize(previous, previousTile);
            }

            string id = el.Item1;
            Graph tile;

            // If next destination is not within one step, do some backtracking
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

                // Visualize backtrack only if route is not required for the whole solution
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

            // Dequeue tile if everything else is valid
            switch (loc)
            {
                case 1:
                    graphsprio1.TryDequeue(out el);
                    break;
                case 2:
                    stucks.TryDequeue(out el);
                    break;
            }

            tile = el!.Item2;

            // Turn visited flags on for next tile
            // Visualize tile as visited
            previousTile.TileView = TileView.Visited;
            tile.TileView = TileView.Visited;
            if (IsBack)
                tile.BackStates = TileState.Visited;
            else tile.States = TileState.Visited;

            if (!IsBack && tile.IsTreasure)
            {
                int len1 = graphsprio1.Count();
                int len2 = graphsprio2.Count();

                // If next tile contains a treasure
                // Set previous tiles as required for the visualizer
                SetNonTSPRoute(id);

                // Refactor all routes to include backtracking steps from the treasure tile
                for (int i = 0; i < len2; i++)
                {
                    graphsprio2.TryDequeue(out var duplicate);
                    graphsprio2.Enqueue(RefactorRoute(duplicate!, id));
                }

                // Move first priority queue into the second one to prioritize routesnimize backtracking
                for (int i = 0; i < len1; i++)
                {
                    graphsprio2.Enqueue(RefactorRoute(graphsprio1.ElementAt(i), id));
                }
                graphsprio1.Clear();

                treasureCounts++;

                if (treasureCounts == map.TreasuresCount)
                {
                    // Every Treasure has been found, mark the algorithm as done
                    isTreasureDone = true;
                    if (isTSP)
                    {
                        // If TSP is required, clear every queue and restart the algorithm with start tile as the goal
                        graphsprio1.Clear();
                        graphsprio2.Clear();
                        graphsprio1.Enqueue(new Tuple<string, Graph>(id, tile));
                    }
                    return new Tuple<string, Graph, Graph, bool>(id, tile, previousTile, true);
                }


            }
            else if (IsBack && tile == map.Start)
            {
                // Start found, mark the algorithm as done
                isTSPDone = true;
                return new Tuple<string, Graph, Graph, bool>(id, tile, previousTile, true);
            }

            // Add non null neighbors to the queue
            // If TSP, skip visited tiles to prioritize routes with minimal backtracking
            List<Tuple<Graph?, int>> neighborsData = new();
            for (int i = 0; i < tile.Neighbors.Length; i++)
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
                // If TSP, load visited tiles to lower priority queue to prioritize routes with minimal backtracking
                List<Tuple<Graph?, int>> neighborsStuckData = new();
                for (int i = 0; i < tile.Neighbors.Length; i++)
                {
                    Graph? candidate = tile.Neighbors[i];
                    if (candidate != null &&
                        candidate?.BackStates != TileState.Visited &&
                        candidate?.States == TileState.Visited
                    ) neighborsStuckData.Add(new Tuple<Graph?, int>(tile.Neighbors[i], i));
                }

                neighborsCount += neighborsStuckData.Count;

                AddToQueue(stucks, neighborsStuckData, id, neighborsCount);
            }

            AddToQueue(graphsprio1, neighborsData, id, neighborsCount);

            return new Tuple<string, Graph, Graph, bool>(id, tile, previousTile, false);
        }
    }
}
