using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        protected Graph? lastTreasure;
        protected bool isTreasureDone = false;
        protected bool isTSPDone = false;
        protected List<Tuple<int, int>> nonTSPRoute = new();

        public delegate void Callback(Tuple<string, Graph, Graph, bool> step);

        public bool IsDone { get => isTSP ? isTSPDone : isTreasureDone; }
        public bool IsBack { get => isTSP && !isTSPDone && isTreasureDone; }

        protected Algorithm(Map map, bool isTSP = false)
        {
            this.map = map;
            this.isTSP = isTSP;
        }

        /*
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
        */

        public void GetResult(Result res, string final)
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

        public void GetNonTSPRoute(string route)
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

        public abstract Tuple<string, bool> Next(string previous);

        public abstract Result JustRun();

        public abstract Tuple<String, Graph, Graph, bool> RunAndVisualize(string previous, Graph previousTile);

        public abstract Task RunProper(Callback callback, Func<int> getDelay, CancellationTokenSource cancellation);

        public Graph getGraphStep(char idChar, Graph tile, bool reversed)
        {
            if (!reversed)
            {
                switch (idChar)
                {
                    case '0':
                        return tile.Neighbors[0]!;
                    case '1':
                        return tile.Neighbors[1]!;
                    case '2':
                        return tile.Neighbors[2]!;
                    case '3':
                        return tile.Neighbors[3]!;
                    default:
                        throw new Exception("Invalid direction");
                }
            }
            else
            {
                switch (idChar)
                {
                    case '0':
                        return tile.Neighbors[2]!;
                    case '1':
                        return tile.Neighbors[3]!;
                    case '2':
                        return tile.Neighbors[0]!;
                    case '3':
                        return tile.Neighbors[1]!;
                    default:
                        throw new Exception("Invalid direction");
                }
            }
        }
        public Tuple<string, Graph> refactorRoute(Tuple<string, Graph> route, string id)
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
