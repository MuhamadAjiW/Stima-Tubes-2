using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.OpenGL;

namespace Spongbob.Models
{
    public class BFS : Algorithm
    {
        Queue<Tuple<string, Graph>> graphsprio1 = new();
        Queue<Tuple<string, Graph>> graphsprio2 = new();
        private Queue<Tuple<string, Graph>> stucks = new();
        public BFS(Map map, bool isTSP) : base(map, isTSP)
        {
        }

        public void initialize()
        {
            graphsprio1.Clear();
            graphsprio2.Clear();
            started = true;
            treasureCounts = 0;
            nonTSPRoute.Clear();
            map.ResetState();
            graphsprio1.Enqueue(new Tuple<string, Graph>("S", map.Start));
        }

        public override Result JustRun()
        {
            Result res = new(map.Width, map.Height);
            string id = "";
            string backId = "";
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            initialize();

            while (!IsDone)
            {
                if(!IsBack)
                    Console.WriteLine("Current id: " + id);
                else Console.WriteLine("Current id: " + backId);
                
                Console.Write("graphsprio1: ");
                for(int i = 0; i < graphsprio1.Count(); i++){
                    Console.Write(graphsprio1.ElementAt(i).Item1 + " ");
                }
                Console.Write("\n");

                Console.Write("graphsprio2: ");
                for(int i = 0; i < graphsprio2.Count(); i++){
                    Console.Write(graphsprio2.ElementAt(i).Item1 + " ");
                }
                Console.Write("\n");
                
                Console.Write("stucks: ");
                for(int i = 0; i < stucks.Count(); i++){
                    Console.Write(stucks.ElementAt(i).Item1 + " ");
                }
                Console.Write("\n");


                if (!isTreasureDone && graphsprio1.TryPeek(out var graph))
                    lastTreasure = graph.Item2;
                
                if (!isTreasureDone && graphsprio2.TryPeek(out var graph2))
                    lastTreasure = graph2.Item2;

                if (IsBack)
                {
                    backId = Next(backId);
                }
                else id = Next(id);
                res.NodesCount++;
            }
            watch.Stop();
            res.Time = watch.ElapsedMilliseconds;

            if (isTSP) 
                GetResult(res, backId);
            else
                GetResult(res, id);
           
            //print result
            Console.WriteLine("Result: ");
            Console.WriteLine("Time: " + res.Time + "ms");
            Console.WriteLine("Nodes: " + res.NodesCount);
            for(int i = 0; i < res.Route.Count(); i++){
                Console.Write(res.Route.ElementAt(i) + " ");
            }
            Console.Write("\n");

            return res;
        }

        public override string Next(string previous)
        {
            int loc = 1;
            graphsprio1.TryPeek(out var el);
            if (el == null)
            {
                graphsprio2.TryPeek(out el);
                for (int i = 0; i < graphsprio2.Count(); i++)
                {
                    graphsprio1.Enqueue(graphsprio2.ElementAt(i));
                }
                graphsprio2.Clear();

                graphsprio1.TryPeek(out el);

                if (el == null){
                    loc = 2;
                    stucks.TryPeek(out el);
                }
            }
            if (el == null)
            {
                throw new Exception("No solution");
            }

            string id = el.Item1;

            Console.WriteLine("next: " + id + "\n");
            Console.Write("\n");

            
            if(!id.StartsWith(previous)){
                return previous.Substring(0, previous.Length - 1);
            }
            if(id.Length > previous.Length + 1){
                return id.Substring(0, previous.Length + 1);
            }
            
            
            
            switch (loc){    
                case 1:
                    graphsprio1.TryDequeue(out el);
                    break;
                case 2:
                    stucks.TryDequeue(out el);
                    break;
            }

            Graph tile = el!.Item2;

            if (IsBack)
                tile.backStates = TileState.Visited;
            else tile.states = TileState.Visited;

            if (!IsBack && tile.IsTreasure)
            {
                Console.WriteLine("Treasure found: " + id + "\n");
                

                
                for(int i = 0; i < graphsprio1.Count(); i++)
                {
                    graphsprio2.Enqueue(refactorRoute(graphsprio1.ElementAt(i), id));
                }
                graphsprio1.Clear();

                treasureCounts++;

                if (treasureCounts == map.TreasuresCount)
                {
                    isTreasureDone = true;
                    Console.WriteLine("Done!");
                    if (isTSP)
                    {
                        Console.WriteLine("Starting TSP\n");
                        graphsprio1.Clear();
                        graphsprio2.Clear();   
                        graphsprio1.Enqueue(new Tuple<string, Graph>(id, tile));
                    }
                    return id;
                }


            }
            else if (IsBack && tile == map.Start)
            {
                isTSPDone = true;
                return id;
            }

            List<Tuple<Graph?, int>> neighborsData = new();
            for(int i = 0; i < tile.Neighbors.Length; i++){
                Graph? candidate = tile.Neighbors[i];
                if (candidate == null) continue;
                if (IsBack && candidate?.backStates == TileState.Visited) continue;
                if (graphsprio1.Any(s => s.Item2 == candidate)) continue;
                if (IsBack && candidate == map.Start) neighborsData.Add(new Tuple<Graph?, int>(candidate, i));
                if (candidate?.states != TileState.Visited) neighborsData.Add(new Tuple<Graph?, int>(candidate, i));
            }
            /*
            List<Graph?> neighbors = tile.Neighbors.ToList().Where(t => {
                if (t == null) return false;
                if (IsBack && t.GetBackState(id)?.State == TileState.Visited) return false;
                if (IsBack && t == map.Start) return true;

                return t.GetState(id)?.State != TileState.Visited;

            }).ToList();
            */

            int neighborsCount = neighborsData.Count;
            
            if (IsBack)
            {
                List<Tuple<Graph?, int>> neighborsStuckData = new();
                for(int i = 0; i < tile.Neighbors.Length; i++){
                    Graph? candidate = tile.Neighbors[i];
                    if (candidate != null &&
                        candidate?.backStates != TileState.Visited &&
                        candidate?.states == TileState.Visited
                    ) neighborsStuckData.Add(new Tuple<Graph?, int>(tile.Neighbors[i], i));
                }

                /*
                List<Graph?> neighborsStuck = tile.Neighbors.ToList().Where(t =>
                t != null &&
                t.GetBackState(id)?.State != TileState.Visited &&
                t.GetState(id)?.State == TileState.Visited).ToList();
                */

                neighborsCount += neighborsStuckData.Count;
                
                AddToQueue(stucks, neighborsStuckData, id, neighborsCount);
            }
            
            AddToQueue(graphsprio1, neighborsData, id, neighborsCount);
            
            return id;
        }

        static void AddToQueue(Queue<Tuple<string, Graph>> q, List<Tuple<Graph?, int>> neighborsData, string id, int count)
        {
            if (neighborsData.Count == 0) return;

            foreach (var n in neighborsData)
            {
                q.Enqueue(new Tuple<string, Graph>(id + n.Item2, n.Item1!));
            }
        }

        public override Tuple<String, Graph, Graph> RunAndVisualize(string previous, Graph previousTile)
        {
            int loc = 1;
            graphsprio1.TryPeek(out var el);
            if (el == null)
            {
                graphsprio2.TryPeek(out el);
                for (int i = 0; i < graphsprio2.Count(); i++)
                {
                    graphsprio1.Enqueue(graphsprio2.ElementAt(i));
                }
                graphsprio2.Clear();

                graphsprio1.TryPeek(out el);

                if (el == null){
                    loc = 2;
                    stucks.TryPeek(out el);
                }
            }
            if (el == null)
            {
                throw new Exception("No solution");
            }

            string id = el.Item1;
            Graph tile;

            Console.WriteLine("next: " + id + "\n");
            Console.Write("\n");

            
            if(!id.StartsWith(previous) && id != previous){
                tile = getGraphStep(previous[previous.Length - 1], previousTile, true);

                if (IsBack)
                {
                    tile.backStates = TileState.Visited;
                }
                else
                {
                    tile.states = TileState.Visited;
                }

                if (!nonTSPRoute.Contains(previousTile.Pos))
                    previousTile.TileView = TileView.BackTracked;

                return new Tuple<string, Graph, Graph>(previous.Substring(0, previous.Length - 1), tile, previousTile);
            }
            if(id.Length > previous.Length + 1){
                previousTile.TileView = TileView.Visited;
                tile = getGraphStep(id[previous.Length], previousTile, false);

                if (IsBack)
                {
                    tile.backStates = TileState.Visited;
                }
                else
                {
                    tile.states = TileState.Visited;
                }

                return new Tuple<string, Graph, Graph>(id.Substring(0, previous.Length + 1), tile, previousTile);
            }
            
            switch (loc){    
                case 1:
                    graphsprio1.TryDequeue(out el);
                    break;
                case 2:
                    stucks.TryDequeue(out el);
                    break;
            }

            tile = el!.Item2;

            previousTile.TileView = TileView.Visited;
            tile.TileView = TileView.Visited;
            if (IsBack)
                tile.backStates = TileState.Visited;
            else tile.states = TileState.Visited;

            if (!IsBack && tile.IsTreasure)
            {
                GetNonTSPRoute(id);
                Console.WriteLine("Treasure found: " + id + "\n");
                
                for(int i = 0; i < graphsprio1.Count(); i++){
                    graphsprio2.Enqueue(refactorRoute(graphsprio1.ElementAt(i), id));
                }
                graphsprio1.Clear();

                treasureCounts++;

                if (treasureCounts == map.TreasuresCount)
                {
                    isTreasureDone = true;
                    Console.WriteLine("Done!");
                    if (isTSP)
                    {
                        Console.WriteLine("Starting TSP\n");
                        graphsprio1.Clear();
                        graphsprio2.Clear();   
                        graphsprio1.Enqueue(new Tuple<string, Graph>(id, tile));
                    }
                    return new Tuple<string, Graph, Graph>(id, tile, previousTile);
                }


            }
            else if (IsBack && tile == map.Start)
            {
                isTSPDone = true;
                return new Tuple<string, Graph, Graph>(id, tile, previousTile);
            }

            List<Tuple<Graph?, int>> neighborsData = new();
            for(int i = 0; i < tile.Neighbors.Length; i++)
            {
                Graph? candidate = tile.Neighbors[i];
                if (candidate == null) continue;
                if (IsBack && candidate?.backStates == TileState.Visited) continue;
                if (graphsprio1.Any(s => s.Item2 == candidate)) continue;
                if (IsBack && candidate == map.Start) neighborsData.Add(new Tuple<Graph?, int>(candidate, i));
                if (candidate?.states != TileState.Visited) neighborsData.Add(new Tuple<Graph?, int>(candidate, i));
            }

            int neighborsCount = neighborsData.Count;
            
            if (IsBack)
            {
                List<Tuple<Graph?, int>> neighborsStuckData = new();
                for(int i = 0; i < tile.Neighbors.Length; i++){
                    Graph? candidate = tile.Neighbors[i];
                    if (candidate != null &&
                        candidate?.backStates != TileState.Visited &&
                        candidate?.states == TileState.Visited
                    ) neighborsStuckData.Add(new Tuple<Graph?, int>(tile.Neighbors[i], i));
                }

                neighborsCount += neighborsStuckData.Count;
                
                AddToQueue(stucks, neighborsStuckData, id, neighborsCount);
            }
            
            AddToQueue(graphsprio1, neighborsData, id, neighborsCount);
            
            return new Tuple<string, Graph, Graph>(id, tile, previousTile);
        }

        public override async Task RunProper(Callback callback, Func<int> getDelay,
            CancellationTokenSource cancellation)
        {

            Result res = new(map.Width, map.Height);
            string id = "";
            string backId = "";
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            initialize();
            Graph position = map.Start;

            while (!IsDone)
            {
                if (cancellation.IsCancellationRequested)
                {
                    return;
                }

                if (!IsBack)
                    Console.WriteLine("Current id: " + id);
                else Console.WriteLine("Current id: " + backId);

                Console.Write("graphsprio1: ");
                for (int i = 0; i < graphsprio1.Count(); i++)
                {
                    Console.Write(graphsprio1.ElementAt(i).Item1 + " ");
                }

                Console.Write("\n");

                Console.Write("graphsprio2: ");
                for (int i = 0; i < graphsprio2.Count(); i++)
                {
                    Console.Write(graphsprio2.ElementAt(i).Item1 + " ");
                }

                Console.Write("\n");

                Console.Write("stucks: ");
                for (int i = 0; i < stucks.Count(); i++)
                {
                    Console.Write(stucks.ElementAt(i).Item1 + " ");
                }

                Console.Write("\n");


                if (!isTreasureDone && graphsprio1.TryPeek(out var graph))
                    lastTreasure = graph.Item2;

                if (!isTreasureDone && graphsprio2.TryPeek(out var graph2))
                    lastTreasure = graph2.Item2;


                Tuple<string, Graph, Graph> step;
                if (IsBack)
                {
                    step = RunAndVisualize(backId, position);
                    backId = step.Item1;
                }
                else
                {
                    step = RunAndVisualize(id, position);
                    id = step.Item1;

                    if (!isTreasureDone) backId = id;
                }

                position = step.Item2;
                callback(step);
                await Task.Delay(getDelay());
            }

            watch.Stop();
            res.Time = watch.ElapsedMilliseconds;

            if (isTSP)
                GetResult(res, backId);
            else
                GetResult(res, id);

            //print result
            Console.WriteLine("Result: ");
            Console.WriteLine("Time: " + res.Time + "ms");
            Console.WriteLine("Nodes: " + res.NodesCount);
            for (int i = 0; i < res.Route.Count(); i++)
            {
                Console.Write(res.Route.ElementAt(i) + " ");
            }

            Console.Write("\n");
        }
    }
}
