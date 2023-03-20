using Avalonia;
using Avalonia.Controls;
using DynamicData.Binding;
using ReactiveUI;
using Spongbob.Models;
using Spongbob.Views;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Spongbob.ViewModels
{
    public class ResultViewModel: ViewModelBase
    {
        public Grid? Container { get; set; }
        public List<TileViewModel> Tiles { get; } = new();


        public ResultViewModel() {
            this.WhenPropertyChanged(x => x.Map).Subscribe(RenderMap);
        }

        private Map? _map;

        public Map? Map
        {
            get => _map;
            set => this.RaiseAndSetIfChanged(ref _map, value);
        }

        public void RenderMap(PropertyValue<ResultViewModel, Map?> map)
        {
            if (Container == null) return;
            if (Map == null) return;


            Container.Children.Clear();

            Container.ColumnDefinitions.Clear();

            var columnDefinitions = new ColumnDefinitions();

            for (int i = 0; i < Map.Width; i++)
            {
                columnDefinitions.Add(new ColumnDefinition()
                {
                    Width = GridLength.Star,
                });
            }

            Container.RowDefinitions.Clear();
            var rowDefinitions = new RowDefinitions();

            Tiles.Clear();

            for (int i = 0; i < Map.Height; i++)
            {
                rowDefinitions.Add(new RowDefinition()
                {
                    Height = GridLength.Star,
                });
            }

            Container.ColumnDefinitions = columnDefinitions;
            Container.RowDefinitions = rowDefinitions;

            for (int i = 0; i < Map.Height; i++)
            {
                for (int j = 0; j < Map.Width; j++)
                {
                    TileType type = TileType.VOID;
                    var el = Map.Tiles[i, j];

                    if (el != null)
                    {
                        if (el == Map.Start)
                        {
                            type = TileType.START;
                        } else if (el.IsTreasure)
                        {
                            type = TileType.TREASURE;
                        } else
                        {
                            type = TileType.BLANK;
                        }
                    }

                    var tile = new TileViewModel(i, j, type);
                    Tiles.Add(tile);
                    var tileView = new Views.TileView()
                    {
                        DataContext = tile
                    };
                    Grid.SetRow(tileView, i);
                    Grid.SetColumn(tileView, j);
                    Container.Children.Add(tileView);
                    
                }
            }


        }

        public TileViewModel GetTile(int i, int j)
        {
            return Tiles[i * Map!.Width + j];
        }

        public Result RunSearch(bool bfs, bool tsp)
        {
            Algorithm algorithm;

            if (bfs)
            {
                algorithm = new BFS(Map!, tsp);
            } else
            {
                algorithm = new DFS(Map!, tsp);
            }

            return algorithm.JustRun();
        }

        public async Task RunVisualize(bool bfs, bool tsp, Func<int> getDelay, CancellationTokenSource cancellation)
        {
            Algorithm algorithm;

            if (bfs)
            {
                algorithm = new BFS(Map!, tsp);
            }
            else
            {
                algorithm = new DFS(Map!, tsp);
            }

            await algorithm.RunProper((step) =>
            {
                Graph now = step.Item2;
                Graph before = step.Item3;

                GetTile(now.Pos.Item2, now.Pos.Item1).State = TileState.CURRENT;
                if (now == before) return;
                switch (before.TileView)
                {
                    case Models.TileView.Visited:
                        GetTile(before.Pos.Item2, before.Pos.Item1).State = TileState.VISITED;
                        break;
                    case Models.TileView.BackTracked:
                        GetTile(before.Pos.Item2, before.Pos.Item1).State = TileState.BACKTRACKED;
                        break;
                }
            }, getDelay, cancellation);
        }
    }
}
