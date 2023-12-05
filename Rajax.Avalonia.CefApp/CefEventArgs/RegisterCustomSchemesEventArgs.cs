using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Rajax.Avalonia.CefGlueApp.CefEventArgs
{
    public class RegisterCustomSchemesEventArgs : EventArgs
    {
        public RegisterCustomSchemesEventArgs(CefSchemeRegistrar register)
        {
            Registrar = register;
        }

        public CefSchemeRegistrar Registrar { get; private set; }
    }

    public delegate void RegisterCustomSchemesHandler(object sender, RegisterCustomSchemesEventArgs e);
}
