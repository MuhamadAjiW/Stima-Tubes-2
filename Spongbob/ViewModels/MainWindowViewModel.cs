using DynamicData.Binding;
using System.Diagnostics;
using System;
using ReactiveUI;
using Spongbob.Class;
using Avalonia.Controls;

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
            Parser parser = new();

            this.WhenAnyValue(x => x.SideBar.FilePath)
                .Subscribe( file =>
                {
                    Debug.WriteLine("test");
                    if (file != null)
                    {
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