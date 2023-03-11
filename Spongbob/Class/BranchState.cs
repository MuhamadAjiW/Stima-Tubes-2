using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.Class
{
    enum TileState
    {
        NotFound,
        Visited,
        BackTracked,
        BackVisited
    }
    internal class BranchState
    {
        public TileState State = TileState.NotFound;
        public string ID;

        public BranchState(string ID, TileState state)
        {
            this.ID = ID;
            this.State = state;
        }

        public BranchState(string ID)
        {
            this.ID = ID;
        }

        public bool IsIn(string ID)
        {
            return ID.StartsWith(this.ID);
        }
    }
}
