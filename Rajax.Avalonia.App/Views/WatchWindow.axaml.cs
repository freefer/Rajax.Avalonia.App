using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Rajax.Avalonia.App.Views
{
    public partial class WatchWindow : Window
    {

        public string Url = "";
        public WatchWindow()
        {
            InitializeComponent();
             
        }

        private void InitializeComponent()
        {
             
            AvaloniaXamlLoader.Load(this);
          
        }
    }
}
