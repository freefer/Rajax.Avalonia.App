using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rajax.Avalonia.App.ViewModels
{
    public  class FooViewModel : ReactiveObject, IRoutableViewModel
    {
        public FooViewModel(IScreen screen) => HostScreen = screen;
        public string UrlPathSegment => "Foo";
        public IScreen HostScreen { get; }
    }
}
