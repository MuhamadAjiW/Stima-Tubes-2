using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Spongbob.ViewModels;
using System.Diagnostics;

namespace Spongbob.Views
{
    public partial class ResultView : ReactiveUserControl<ResultViewModel>
    {
        public ResultView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                ViewModel!.Container = this.FindControl<Grid>("container");
            });
        }
    }
}
