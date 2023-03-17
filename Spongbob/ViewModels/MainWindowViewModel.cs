namespace Spongbob.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public SidebarViewModel SideBar { get; }
        public ResultViewModel Result { get; }

        public MainWindowViewModel()
        {
            SideBar = new();
            Result = new();
        }
    }
}