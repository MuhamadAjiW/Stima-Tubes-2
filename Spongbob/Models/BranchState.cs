using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.Models
{
    public enum TileState
    {
        NotFound,
        Visited,
    }

    public enum TileView
    {
        NotVisited,
        Visited,
        BackTracked
    }
}
