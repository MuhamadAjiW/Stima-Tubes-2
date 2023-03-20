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

        public MainWindowViewModel()
        {
            SideBar = new();
            CancellationTokenSource? cancellation= null;
            Result = new();
            Parser parser = new();

            SideBar.Search = ReactiveCommand.Create(() =>
            {
                Result res = Result.RunSearch(
                    SideBar.Algorithm.Button1Active,
                    SideBar.TSP.Button1Active
                    );

                SideBar.Result = res;
                SideBar.IsRunning = true;

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

            SideBar.Visualize = ReactiveCommand.Create(() =>
            {
                cancellation = new CancellationTokenSource();
                SideBar.IsRunning = true;
                Result.RunVisualize(
                    SideBar.Algorithm.Button1Active,
                    SideBar.TSP.Button1Active, 
                    SideBar.GetCurrentDelay,
                    cancellation);
            }, this.WhenAnyValue(x => x.SideBar.CanSearch));

            SideBar.RaisePropertyChanged(nameof(SideBar.Search));

            SideBar.Reset = ReactiveCommand.Create(() =>
            {
                if (cancellation != null)
                {
                    cancellation.Cancel();
                }
                var res = SideBar.Result;
                if (Result.Map == null) return;
                SideBar.IsRunning = false;
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
                SideBar.Result = null;
            });

            this.WhenAnyValue(x => x.SideBar.FilePath)
                .Subscribe( file =>
                {
                    Debug.WriteLine("test");
                    if (file != null)
                    {
                        Result.Map = null;
                        try
                        {
                            Result.Map = parser.ParseFile(file);
                            Debug.WriteLine("Success");
                        } catch (Exception ex)
                        {
                            SideBar.Error = ex.Message;
                        }
                    }
                });
        }
    }
}