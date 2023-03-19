using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.Models
{
    public enum Location
    {
        Top = 0,
        Right = 1,
        Bottom = 2,
        Left = 3,
    }
    public class Graph 
    {
        static int count = 0;
        public int Id;
        private bool isTreasure = false;
        public Tuple<int, int> Pos;
        public TileView View = TileView.NotVisited;
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

        public TileView GetTileView { 
            get => View;
        }
        public void SetTileView(TileView newView) {
            View = newView;
        }

        public Graph? GetNeighbor(Location loc)
        {
            return neighbors[(int)loc];
        }

        public void SetNeighbor(Location loc, Graph? neighbor)
        {
            neighbors[(int)loc] = neighbor;
        }
        
        public BranchState? GetState(string branchID, bool exact = false)
        {
            foreach (var state in states)
            {
                if (exact)
                {
                    if (state.ID == branchID)
                        return state;
                }
                else if (state.IsIn(branchID))
                {
                    return state;
                }
            }
            return null;
        }

        public void SetState(string branchID, TileState state)
        {
            BranchState? bState = GetState(branchID, true);
            if (bState != null)
            {
                bState.State = state;
            } else
            {
                bState = new BranchState(branchID);
                bState.State = state;
                states.Add(bState);
            }
        }

        public BranchState? GetBackState(string branchID, bool exact = false)
        {
            foreach (var state in backStates)
            {
                if (exact)
                {
                    if (state.ID == branchID)
                        return state;
                }
                else if (state.IsIn(branchID))
                {
                    return state;
                }
            }
            return null;
        }

        public void SetBackState(string branchID, TileState state)
        {
            BranchState? bState = GetBackState(branchID, true);
            if (bState != null)
            {
                bState.State = state;
            }
            else
            {
                bState = new BranchState(branchID);
                bState.State = state;
                backStates.Add(bState);
            }
        }

        public void ResetState()
        {
            states.Clear();
        }

        public void ResetBackState()
        {
            backStates.Clear();
        }

    }
}
