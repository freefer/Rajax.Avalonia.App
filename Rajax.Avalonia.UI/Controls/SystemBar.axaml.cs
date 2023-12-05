using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Material.Icons;

namespace Rajax.Avalonia.UI.Controls;

public partial class SystemBar : UserControl
{
    public SystemBar()
    {
        InitializeComponent();
     
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public static readonly StyledProperty<IBrush> LogoColorProperty = AvaloniaProperty.Register<SystemBar, IBrush>(nameof(LogoColor), defaultValue: Brushes.DarkSlateBlue);

    public IBrush LogoColor
    {
        get { return GetValue(LogoColorProperty); }
        set { SetValue(LogoColorProperty, value); }
    }

    public static readonly StyledProperty<MaterialIconKind> LogoKindProperty = AvaloniaProperty.Register<SystemBar, MaterialIconKind>(nameof(LogoKind), defaultValue: MaterialIconKind.DotNet);

    public MaterialIconKind LogoKind
    {
        get { return GetValue(LogoKindProperty); }
        set { SetValue(LogoKindProperty, value); }
    }

    public static readonly StyledProperty<IImage>   LogoImageProperty = AvaloniaProperty.Register<SystemBar, IImage>(nameof(LogoImage),null);

    public IImage LogoImage
    {
        get { return GetValue(LogoImageProperty); }
        set { SetValue(LogoImageProperty, value); }
    }

    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<SystemBar, string>(nameof(Title), defaultValue: "You Application Title");

    public string Title
    {
        get { return GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }


    public static readonly StyledProperty<bool> CustomLogoVisibilityProperty = AvaloniaProperty.Register<SystemBar, bool>(nameof(CustomLogoVisibility), defaultValue: false);

    public bool CustomLogoVisibility
    {
        get { return GetValue(CustomLogoVisibilityProperty); }
        set { SetValue(CustomLogoVisibilityProperty, value); }
    }

}