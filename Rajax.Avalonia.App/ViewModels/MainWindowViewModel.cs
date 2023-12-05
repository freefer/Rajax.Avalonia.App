using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Runtime.Serialization;

namespace Rajax.Avalonia.App.ViewModels
{
    [DataContract]
    public class MainWindowViewModel : ViewModelBase
    {
        public RoutedViewHostPageViewModel RoutedViewHost { get; } = new();
        private string greeting = "111111111111111111111111";
        [DataMember]
        public string Greeting
        {
             
            get { return greeting; }
            set { this.RaiseAndSetIfChanged(ref greeting, value); }
        }

        public void ButtonClicked() => Greeting = "Hello, Avalonia!";

        
    }

}