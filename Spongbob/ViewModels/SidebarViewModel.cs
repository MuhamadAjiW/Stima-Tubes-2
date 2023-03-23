using DynamicData.Binding;
using ReactiveUI;
using Spongbob.Models;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Spongbob.ViewModels
{
    public class SidebarViewModel : ViewModelBase
    {
        private bool isRunning = false;

        public bool IsRunning
        {
            get => isRunning;
            set => this.RaiseAndSetIfChanged(ref isRunning, value);
        }

        private bool isRerun = false;

        public bool IsRerun
        {
            get => isRerun;
            set => this.RaiseAndSetIfChanged(ref isRerun, value);
        }

        private int speed;

        public int Speed
        {
            get => speed;
            set => this.RaiseAndSetIfChanged(ref speed, value);
        }

        private Result? result;

        public Result? Result
        {
            get => result;
            set
            {
                this.RaiseAndSetIfChanged(ref result, value);
                this.RaisePropertyChanged(nameof(CanRerun));
            }
        }

        public string? Route { get; set; }

        private string? error;

        public string? Error
        {
            get => error;
            set
            {
                this.RaiseAndSetIfChanged(ref error, value);
                this.RaisePropertyChanged(nameof(CanSearch));
            }
        }


        private string? filePath;
        public string? FilePath
        {
            get => filePath;
            set
            {
                this.RaiseAndSetIfChanged(ref filePath, value);
                this.RaisePropertyChanged(nameof(FilePath));
                this.RaisePropertyChanged(nameof(Filename));
                this.RaisePropertyChanged(nameof(CanSearch));
            }
        }
        public SidebarViewModel()
        {
            ShowFileDialog = new();
            TSP = new("TSP", "yes", "no");
            Algorithm = new("Algorithm", "BFS", "DFS");

            Search = ReactiveCommand.Create(() =>
            {

            }, this.WhenAnyValue(x => x.CanSearch));

            Visualize = ReactiveCommand.Create(() =>
            {
                return Task.CompletedTask;
            }, this.WhenAnyValue(x => x.CanSearch));

            this.WhenValueChanged(x => x.Result).Subscribe(r =>
            {
                if (r != null)
                {
                    Route = string.Join("-", r.Route);
                }
                else
                {
                    Route = null;
                }

                this.RaisePropertyChanged(nameof(Route));
            });

            Reset = ReactiveCommand.Create(() =>
            {
                Result = null;
            });

            RerunResult = ReactiveCommand.Create(() => { },
                this.WhenAnyValue(x => x.CanRerun));

        }

        public ToggleButtonViewModel TSP { get; }
        public ToggleButtonViewModel Algorithm { get; }

        public string? Filename
        {
            get => string.IsNullOrEmpty(filePath) ? "Click to select" : Path.GetFileName(filePath);
        }


        public Interaction<Unit, string?> ShowFileDialog { get; }

        public async void SelectFile()
        {
            var result = await ShowFileDialog.Handle(new Unit());
            if (result != FilePath)
            {
                Error = null;
            }
            if (result != null)
            {
                FilePath = result;
            }
        }

        public ReactiveCommand<Unit, Task> Visualize { get; set; }

        public ReactiveCommand<Unit, Unit> Search { get; set; }

        public ReactiveCommand<Unit, Unit> RerunResult { get; set; }

        public bool CanSearch
        {
            get => !string.IsNullOrEmpty(filePath) && string.IsNullOrEmpty(Error);
        }

        public bool CanRerun
        {
            get => Result != null && Result.Found;
        }

        public ReactiveCommand<Unit, Unit> Reset { get; set; }

        public int GetCurrentDelay()
        {
            return 1000 - speed;
        }
    }
}
