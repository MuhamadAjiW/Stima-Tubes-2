using DynamicData.Binding;
using System.Diagnostics;
using System;
using ReactiveUI;
using Spongbob.Models;
using Avalonia.Controls;
using System.Threading;

namespace Spongbob.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public SidebarViewModel SideBar { get; }
        public ResultViewModel Result { get; }

        private CancellationTokenSource? cancellation;

        public MainWindowViewModel()
        {
            SideBar = new();
            Result = new();
            Parser parser = new();

            SideBar.Search = ReactiveCommand.Create(() =>
            {
                Result.Found = true;
                Result res = Result.RunSearch(
                    SideBar.Algorithm.Button1Active,
                    SideBar.TSP.Button1Active
                    );
                Result.Found = res.Found;
                SideBar.Result = res;
                SideBar.IsRunning = true;

                if (!res.Found) return;

                for (int i = 0; i < res.Tiles.GetLength(0); i++)
                {
                    for (int j = 0; j < res.Tiles.GetLength(1); j++)
                    {
                        var tile = Result.Tiles[i * res.Tiles.GetLength(1) + j];
                        if (res.Tiles[i, j] == 0)
                        {
                            tile.State = TileState.BLANK;
                        } else
                        {
                            tile.State = TileState.VISITED;
                        }
                    }
                }
            }, this.WhenAnyValue(x => x.SideBar.CanSearch));

            SideBar.Visualize = ReactiveCommand.Create(async () =>
            {
                cancellation = new CancellationTokenSource();
                SideBar.IsRunning = true;
                await Result.RunVisualize(
                    SideBar.Algorithm.Button1Active,
                    SideBar.TSP.Button1Active, 
                    SideBar.GetCurrentDelay,
                    cancellation);

                if (cancellation.IsCancellationRequested) return;
                Result res = Result.RunSearch(
                    SideBar.Algorithm.Button1Active,
                    SideBar.TSP.Button1Active
                    );

                SideBar.Result = res;
                Result.Found = true;
            }, this.WhenAnyValue(x => x.SideBar.CanSearch));

            SideBar.RaisePropertyChanged(nameof(SideBar.Search));

            SideBar.Reset = ReactiveCommand.Create(() =>
            {
                Result.Found = true;
                if (cancellation != null)
                {
                    cancellation.Cancel();
                }
                var res = SideBar.Result;
                if (Result.Map == null) return;
                SideBar.IsRunning = false;
                resetMap();
                SideBar.Result = null;
            });

            this.WhenAnyValue(x => x.SideBar.FilePath)
                .Subscribe( file =>
                {
                    Result.Map = null;
                    if (file != null)
                    {
                        try
                        {
                            Result.Map = parser.ParseFile(file);
                        } catch (Exception ex)
                        {
                            SideBar.Error = ex.Message;
                        }
                    }
                });

            SideBar.RerunResult = ReactiveCommand.Create(RerunResult, this.WhenAnyValue(x => x.SideBar.CanRerun));
        }

        public void RerunResult()
        {
            if (Result.Map == null || SideBar.Result == null || !SideBar.Result.Found) return;
            cancellation?.Cancel();
            resetMap();
            cancellation = new();
            Algorithm.RerunResult(Result.Map, SideBar.Result, (Graph prev, Graph now) =>
            {
                Result.GetTile(now.Pos.Item2, now.Pos.Item1).State = TileState.CURRENT;

                if (prev != now)
                {
                    Result.GetTile(prev.Pos.Item2, prev.Pos.Item1).State = TileState.VISITED;
                }
            }, SideBar.GetCurrentDelay, cancellation);
            
        }

        void resetMap()
        {
            if (Result.Map == null) return;
            for (int i = 0; i < Result.Map.Height; i++)
            {
                for (int j = 0; j < Result.Map.Width; j++)
                {
                    var tile = Result.GetTile(i, j);
                    tile.State = TileState.BLANK;

                    var start = Result.Map!.StartPos;

                    if (start != null && start.Item1 == j && start.Item2 == i)
                    {
                        tile.State = TileState.CURRENT;
                    }
                }
            }
        }
    }
}