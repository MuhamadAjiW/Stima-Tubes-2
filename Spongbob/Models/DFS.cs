using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.Models
{
    public class DFS : Algorithm
    {
        Stack<Tuple<string, Graph>> graphsprio1 = new();
        Stack<Tuple<string, Graph>> graphsprio2 = new();
        Stack<Tuple<string, Graph>> stucks = new();
        public DFS(Map map, bool isTSP) : base(map, isTSP)
        {
        }

        public override Result JustRun()
        {
            Result res = new(map.Width, map.Height);
            string id = "";
            string backId = "";
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
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
            if (!started)
            {
                started = true;
                treasureCounts = 0;
                graphsprio1.Push(new Tuple<string, Graph>("S", map.Start));
            }

            int loc = 1;
            graphsprio1.TryPeek(out var el);
            if (el == null)
            {
                loc = 2;
                graphsprio2.TryPeek(out el);

                if(el == null){
                    loc = 3;
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
                    graphsprio1.TryPop(out el);
                    break;
                case 2:
                    graphsprio2.TryPop(out el);
                    break;
                case 3:
                    stucks.TryPop(out el);
                    break;
            }

            Graph tile = el!.Item2;

            if (IsBack)
            {
                tile.SetBackState(id, TileState.Visited);
            }
            else
            {
                tile.SetState(id, TileState.Visited);
            }

            if (!IsBack && tile.IsTreasure)
            {
                Console.WriteLine("Treasure found: " + id + "\n");
                
                for(int i = 0; i < graphsprio1.Count(); i++){
                    graphsprio2.Push(graphsprio1.ElementAt(i));
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
                        graphsprio1.Push(new Tuple<string, Graph>(id, tile));
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
                if (tile.Neighbors[i] == null) continue;
                if (IsBack && tile.Neighbors[i]?.GetBackState(id)?.State == TileState.Visited) continue;
                if (IsBack && tile.Neighbors[i] == map.Start) neighborsData.Add(new Tuple<Graph?, int>(tile.Neighbors[i], i));
                if (tile.Neighbors[i]?.GetState(id)?.State != TileState.Visited) neighborsData.Add(new Tuple<Graph?, int>(tile.Neighbors[i], i));
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
                    if (tile.Neighbors[i] != null &&
                        tile.Neighbors[i]?.GetBackState(id)?.State != TileState.Visited &&
                        tile.Neighbors[i]?.GetState(id)?.State == TileState.Visited
                    ) neighborsStuckData.Add(new Tuple<Graph?, int>(tile.Neighbors[i], i));
                }

                
                // List<Graph?> neighborsStuck = tile.Neighbors.ToList().Where(t =>
                // t != null &&
                // t.GetBackState(id)?.State != TileState.Visited &&
                // t.GetState(id)?.State == TileState.Visited).ToList();
                

                neighborsCount += neighborsStuckData.Count;
                
                AddToStack(stucks, neighborsStuckData, id, neighborsCount);
            }
            
            AddToStack(graphsprio1, neighborsData, id, neighborsCount);
            
            return id;
        }

        static void AddToStack(Stack<Tuple<string, Graph>> q, List<Tuple<Graph?, int>> neighborsData, string id, int count)
        {
            if (neighborsData.Count == 0) return;

            foreach (var n in neighborsData)
            {
                q.Push(new Tuple<string, Graph>(id + n.Item2, n.Item1!));
            }
        }

        public override Tuple<String, Graph, Graph> RunAndVisualize(string previous, Graph previousTile)
        {
            throw new NotImplementedException();
        }
    }
}
