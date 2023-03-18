using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.ViewModels
{
    public enum TileType
    {
        VOID,
        BLANK,
        START,
        TREASURE,
    }

    public enum TileState
    {
        BLANK,
        CURRENT,
        VISITED,
        BACKTRACKED,
    }
    public class TileViewModel: ViewModelBase
    {
        public int Row { get; }
        public int Col { get; }


        private TileType type;
        public TileType Type { 
            get => type;
            set => this.RaiseAndSetIfChanged(ref type, value);
        }

        private TileState state;
        public TileState State
        {
            get => state;
            set => this.RaiseAndSetIfChanged(ref state, value);
        }

        public TileViewModel(int row, int col, TileType type)
        {
            Row = row;
            Col = col;
            Type = type;
            if (type == TileType.START)
                State = TileState.CURRENT;
            else
                State = TileState.BLANK;
            
        }

        public void Click()
        {
            Type = TileType.VOID;
        }
    }
}
