using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Rajax.Avalonia.App.ViewModels;
using ReactiveUI;

namespace Rajax.Avalonia.App.Views;

public partial class BarView : UserControl, IViewFor<BarViewModel>
{
    public BarView()
    {
        InitializeComponent();
    }

    public BarViewModel? ViewModel { get; set; }

    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (BarViewModel?)value;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}