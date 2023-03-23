using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Spongbob.Models
{
    public abstract class Algorithm
    {
        protected Map map;
        protected bool started = false;
        protected bool isTSP;
        protected int treasureCounts = 0;
        protected bool isTreasureDone = false;
        protected bool isTSPDone = false;
        protected List<Tuple<int, int>> nonTSPRoute = new();

        public delegate void StepCallback(Tuple<string, Graph, Graph, bool> step);
        public delegate void RerunResultCallback(Graph prev, Graph now);

        public bool IsDone { get => isTSP ? isTSPDone : isTreasureDone; }
        public bool IsBack { get => isTSP && !isTSPDone && isTreasureDone; }

        protected Algorithm(Map map, bool isTSP = false)
        {
            this.map = map;
            this.isTSP = isTSP;
        }


        /// <summary>
        /// Initialize all stacks/queue before running the algorithm
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Run next step of the algorithm
        /// </summary>
        /// <param name="previous">Previous id</param>
        /// <returns>Tuple of id and boolean valid, if not valid means only backtrack</returns>
        public abstract Tuple<string, bool> Next(string previous);

        /// <summary>
        /// Just like Next, but for visualize
        /// </summary>
        /// <param name="previous"></param>
        /// <param name="previousTile"></param>
        /// <returns>Tuple of id, prev graph, current graph, boolean valid</returns>
        public abstract Tuple<string, Graph, Graph, bool> NextVisualize(string previous, Graph previousTile);

        /// <summary>
        /// Get result after the algorithm finished
        /// </summary>
        /// <param name="res"></param>
        /// <param name="final"></param>
        public void GenerateResult(Result res, string final)
        {
            int limit = final.Length;
            int y = map.StartPos.Item2;
            int x = map.StartPos.Item1;
            res.Tiles[y, x]++;
            for(int i = 1; i < limit; i++){
                if(final[i] == '0'){
                    y--;
                    res.Route.Add('U');
                }
                else if(final[i] == '1'){
                    x++;
                    res.Route.Add('R');
                }
                else if(final[i] == '2'){
                    y++;
                    res.Route.Add('D');
                }
                else if(final[i] == '3'){
                    x--;
                    res.Route.Add('L');
                }
                res.Tiles[y, x]++;
            }
        }

        /// <summary>
        /// Update nonTSPRoute property
        /// </summary>
        /// <param name="route"></param>
        public void SetNonTSPRoute(string route)
        {
            int limit = route.Length;
            int y = map.StartPos.Item2;
            int x = map.StartPos.Item1;
            nonTSPRoute.Add(map.StartPos);
            for (int i = 1; i < limit; i++)
            {
                if (route[i] == '0')
                {
                    y--;
                }
                else if (route[i] == '1')
                {
                    x++;
                }
                else if (route[i] == '2')
                {
                    y++;
                }
                else if (route[i] == '3')
                {
                    x--;
                }

                nonTSPRoute.Add(new Tuple<int, int>(x, y));
            }
        }

        /// <summary>
        /// Run without visualizing
        /// </summary>
        /// <returns>Result of the run</returns>
        public Result JustRun()
        {
            Result res = new(map.Width, map.Height);
            string id = "";
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            Initialize();
            Tuple<string, bool> step = Next(id);
            id = step.Item1;

            while (!IsDone)
            {
                try
                {
                    step = Next(id);
                }
                catch
                {
                    watch.Stop();
                    res.Time = watch.ElapsedMilliseconds;
                    res.Found = false;
                    return res;
                }

                id = step.Item1;

                if (step.Item2)
                    res.NodesCount++;
            }
            watch.Stop();
            res.Time = watch.ElapsedMilliseconds;

            GenerateResult(res, id);

            return res;
        }

        /// <summary>
        /// Run the algorithm and visualize at every step (will call callback)
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="getDelay"></param>
        /// <param name="cancellation"></param>
        public async void RunAndVisualize(StepCallback callback, Func<int> getDelay, CancellationTokenSource cancellation)
        {
            string id = "";

            Initialize();
            Graph position = map.Start;
            Tuple<string, Graph, Graph, bool> step = NextVisualize(id, position);
            id = step.Item1;

            while (!IsDone)
            {
                if (cancellation.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    step = NextVisualize(id, position);
                }
                catch
                {
                    break;
                }

                id = step.Item1;
                position = step.Item2;
            
                callback(step);
                await Task.Delay(getDelay());
            }
        }

        /// <summary>
        /// VIsualize but only for the result routes
        /// </summary>
        /// <param name="map"></param>
        /// <param name="res"></param>
        /// <param name="callback"></param>
        /// <param name="getDelay"></param>
        /// <param name="cancellation"></param>
        public static async void RerunResult(Map map, Result res, RerunResultCallback callback, Func<int> getDelay, CancellationTokenSource cancellation)
        {
            if (!res.Found) return;
            Graph prev = map.Start;
            Graph now = map.Start;
            callback(prev, now);
            await Task.Delay(getDelay());

            foreach (var x in res.Route)
            {
                if (cancellation.IsCancellationRequested) return;
                prev = now;
                now = x switch
                {
                    'U' => now.GetNeighbor(Location.Top)!,
                    'R' => now.GetNeighbor(Location.Right)!,
                    'D' => now.GetNeighbor(Location.Bottom)!,
                    _ => now.GetNeighbor(Location.Left)!,
                };
                callback(prev, now);
                await Task.Delay(getDelay());
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idChar"></param>
        /// <param name="tile"></param>
        /// <param name="reversed"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Graph GetGraphStep(char idChar, Graph tile, bool reversed)
        {
            if (!reversed)
            {
                return idChar switch
                {
                    '0' => tile.Neighbors[0]!,
                    '1' => tile.Neighbors[1]!,
                    '2' => tile.Neighbors[2]!,
                    '3' => tile.Neighbors[3]!,
                    _ => throw new Exception("Invalid direction"),
                };
            }
            else
            {
                return idChar switch
                {
                    '0' => tile.Neighbors[2]!,
                    '1' => tile.Neighbors[3]!,
                    '2' => tile.Neighbors[0]!,
                    '3' => tile.Neighbors[1]!,
                    _ => throw new Exception("Invalid direction"),
                };
            }
        }
        public Tuple<string, Graph> RefactorRoute(Tuple<string, Graph> route, string id)
        {
            string oldRoute = route.Item1;

            if (oldRoute != id)
            {
                int prefixLen = 0;
                while (oldRoute[prefixLen] == id[prefixLen])
                {
                    prefixLen++;
                }

                string newRoute = id;
                for (int j = id.Length - 1; j >= prefixLen; j--)
                {
                    switch (id[j])
                    {
                        case '0':
                            newRoute += '2';
                            break;

                        case '1':
                            newRoute += '3';
                            break;

                        case '2':
                            newRoute += '0';
                            break;
                        case '3':
                            newRoute += '1';
                            break;
                    }
                }
                newRoute += oldRoute.Substring(prefixLen, oldRoute.Length - prefixLen);

                return new Tuple<string, Graph>(newRoute, route.Item2);
            }
            else
            {
                return route;
            }
        }


    }
}
