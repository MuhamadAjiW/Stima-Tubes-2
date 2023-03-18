using Avalonia;
using Avalonia.Controls;
using DynamicData.Binding;
using ReactiveUI;
using Spongbob.Class;
using Spongbob.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.ViewModels
{
    public class ResultViewModel: ViewModelBase
    {
        public Grid? Container { get; set; }
        public List<TileViewModel> tiles = new();

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

            for (int i = 0; i < Map.Width; i++)
            {
                Container.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = GridLength.Star,
                });
            }

            Container.RowDefinitions.Clear();
            tiles.Clear();

            for (int i = 0; i < Map.Height; i++)
            {
                Container.RowDefinitions.Add(new RowDefinition()
                {
                    Height = GridLength.Star,
                });
            }

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
                    tiles.Add(tile);
                    var tileView = new TileView()
                    {
                        DataContext = tile
                    };
                    Grid.SetRow(tileView, i);
                    Grid.SetColumn(tileView, j);
                    Container.Children.Add(tileView);
                    
                }
            }


        }
    }
}
