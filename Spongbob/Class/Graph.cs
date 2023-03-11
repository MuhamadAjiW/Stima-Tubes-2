using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.Class
{
    enum Location
    {
        Top = 0,
        Right = 1,
        Bottom = 2,
        Left = 3,
    }
    internal class Graph 
    {
        static int count = 0;
        public int Id;
        private bool isTreasure = false;
        public Tuple<int, int> Pos;
        private readonly List<BranchState> states = new();
        private readonly List<BranchState> backStates = new();
        private readonly Graph?[] neighbors =
        {
            null,
            null,
            null,
            null
        };

        public bool IsTreasure { get => isTreasure; }
        public Graph?[] Neighbors { get => neighbors; }

        public Graph(int x, int y, bool isTreasure = false)
        {
            this.isTreasure = isTreasure;
            Id = count;
            count++;
            Pos = new Tuple<int, int>(x, y);
        }

        public Graph(Graph? top, Graph? right, Graph? bottom, Graph? left, bool isTreasure = false)
        {
            neighbors[0] = top;
            neighbors[1] = right;
            neighbors[2] = bottom;
            neighbors[3] = left;
            this.isTreasure = isTreasure;
        }

        public Graph? GetNeighbor(Location loc)
        {
            return neighbors[(int)loc];
        }

        public void SetNeighbor(Location loc, Graph? neighbor)
        {
            neighbors[(int)loc] = neighbor;
        }
        
        public BranchState GetState(string branchID)
        {
            foreach (var state in states)
            {
                Debug.WriteLine("AAAA");
                Debug.WriteLine(state.ID);
                Debug.WriteLine(state.State);
                Debug.WriteLine("BBB");
                if (state.IsIn(branchID))
                {
                    return state;
                }
            }
            BranchState branchState = new(branchID);
            states.Add(branchState);
            return branchState;
        }

        public BranchState GetBackState(string branchID)
        {
            foreach (var state in backStates)
            {
                if (state.IsIn(branchID))
                {
                    return state;
                }
            }
            BranchState branchState = new(branchID);
            backStates.Add(branchState);
            return branchState;
        }

        public void ResetState()
        {
            states.Clear();
        }

    }
}
