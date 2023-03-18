using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.ViewModels
{
    public class SidebarViewModel: ViewModelBase
    {

        private string? error;

        public string? Error
        {
            get => error;
            set => this.RaiseAndSetIfChanged(ref error, value);
        }


        private string? filePath;
        public string? FilePath { 
            get => filePath;
            set
            {
                this.RaiseAndSetIfChanged(ref filePath, value);
                this.RaisePropertyChanged(nameof(FilePath));
                this.RaisePropertyChanged(nameof(Filename));
            }
        }
        public SidebarViewModel() {
            ShowFileDialog = new();
            TSP = new("TSP", "yes", "no");
            Algorithm = new("Algorithm", "BFS", "DFS");
        }

        public ToggleButtonViewModel TSP { get; }
        public ToggleButtonViewModel Algorithm { get; }

        public string? Filename { 
            get => string.IsNullOrEmpty(filePath) ? "Click to select" : Path.GetFileName(filePath);
        }


        public Interaction<Unit, string?> ShowFileDialog { get; }

        public async void SelectFile()
        {
            var result = await ShowFileDialog.Handle(new Unit());
            if  (result != FilePath)
            {
                Error = null;
            }
            if (result != null)
            {
                FilePath = result;
            }
        }

        public void Search()
        {

        }

        [DependsOn(nameof(FilePath))]
        public bool CanSearch(object parameter)
        {
            return FilePath != null;
        }
    }
}
