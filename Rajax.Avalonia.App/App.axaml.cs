using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Rajax.Avalonia.App.ViewModels;
using Rajax.Avalonia.App.Views;
using ReactiveUI;
using Splat;
using System;
 
namespace Rajax.Avalonia.App
{
    public partial class App : Application
    {
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
            Locator.CurrentMutable.Register(() => new FooView(), typeof(IViewFor<FooViewModel>));
            Locator.CurrentMutable.Register(() => new BarView(), typeof(IViewFor<BarViewModel>));
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                //desktop.MainWindow = new MainWindow
                //{
                //    DataContext = new MainWindowViewModel(),
                //};
                  desktop.MainWindow = new WatchWindow();

            }

            ////数据持久化
            //// Create the AutoSuspendHelper.
            //var suspension = new AutoSuspendHelper(ApplicationLifetime);
            //RxApp.SuspensionHost.CreateNewAppState = () => new MainWindowViewModel();
            // RxApp.SuspensionHost.SetupDefaultSuspendResume(new NewtonsoftJsonSuspensionDriver("appstate.json"));
            //suspension.OnFrameworkInitializationCompleted();

            //// Load the saved view model state.
            //var state = RxApp.SuspensionHost.GetAppState<MainWindowViewModel>();
            //new MainWindow { DataContext = state }.Show();
          
            base.OnFrameworkInitializationCompleted();
        }

        
    }
}