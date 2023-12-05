using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rajax.Avalonia.App.ViewModels
{
     public class RoutedViewHostPageViewModel : ReactiveObject, IScreen
    {
        public RoutedViewHostPageViewModel()
        {
            Foo = new(this);
            Bar = new(this);
            Router.Navigate.Execute(Foo);
        }

        public RoutingState Router { get; } = new();
        public FooViewModel Foo { get; }
        public BarViewModel Bar { get; }

        public void ShowFoo() => Router.Navigate.Execute(Foo);
        public void ShowBar() => Router.Navigate.Execute(Bar);
    }
}
