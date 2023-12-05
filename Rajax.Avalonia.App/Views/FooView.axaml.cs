using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Rajax.Avalonia.App.ViewModels;
using ReactiveUI;

namespace Rajax.Avalonia.App.Views;

public partial class FooView : UserControl, IViewFor<FooViewModel>
{
    public FooView()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
         
    }

    public FooViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (FooViewModel?)value;
    }


}