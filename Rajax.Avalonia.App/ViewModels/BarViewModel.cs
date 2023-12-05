using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rajax.Avalonia.App.ViewModels
{
    public class BarViewModel : ReactiveObject, IRoutableViewModel

    {
        public BarViewModel(IScreen screen) => HostScreen = screen;
        public string UrlPathSegment => "Bar";
        public IScreen HostScreen { get; }
    }
}
