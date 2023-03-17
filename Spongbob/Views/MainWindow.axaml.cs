using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Spongbob.ViewModels;
using System.Reactive;
using System.Threading.Tasks;

namespace Spongbob.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(d => d(ViewModel!.SideBar.ShowFileDialog.RegisterHandler(DoShowFileDialogAsync)));
        }

        private async Task DoShowFileDialogAsync(InteractionContext<Unit, string?> interaction)
        {
            OpenFileDialog dialog = new()
            {
                AllowMultiple = false
            };

            var result =  await dialog.ShowAsync(this);

            if (result != null && result.Length > 0)
            {
                interaction.SetOutput(result[0]);
            } else
            {
                interaction.SetOutput(null);
            }


        }
    }
}