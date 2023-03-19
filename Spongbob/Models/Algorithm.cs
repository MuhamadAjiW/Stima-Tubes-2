using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        public abstract string Next(string previous);

        public abstract Result JustRun();

        public abstract Tuple<String, Graph, Graph> RunAndVisualize(string previous, Graph previousTile);


    }
}
