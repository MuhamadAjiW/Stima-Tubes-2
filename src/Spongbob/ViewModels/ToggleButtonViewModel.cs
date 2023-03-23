using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spongbob.ViewModels
{
    public class ToggleButtonViewModel : ViewModelBase
    {
        public string Title { get; }
        public string Button1Title { get; }

        private int _selected = 1;
        public int Selected
        {
            get => _selected;
            set
            {
                this.RaiseAndSetIfChanged(ref _selected, value);
                this.RaisePropertyChanged(nameof(Button2Active));
                this.RaisePropertyChanged(nameof(Button1Active));
            }
        }

        public bool Button1Active
        {
            get => _selected == 1;
        }

        public bool Button2Active
        {
            get => _selected == 2;
        }

        public string Button2Title { get; }

        public ToggleButtonViewModel(string title, string button1Title, string button2Title)
        {
            Title = title;
            Button1Title = button1Title;
            Button2Title = button2Title;
            Select = ReactiveCommand.Create<string>((value) =>
            {
                Debug.WriteLine(value);
                if (value == "1")
                {
                    Selected = 1;
                }
                else
                {
                    Selected = 2;
                }
            });
        }

        public ReactiveCommand<string, Unit> Select { get; }
    }
}
