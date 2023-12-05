
using Rajax.Avalonia.CefGlueApp.CefEventArgs;
using Xilium.CefGlue;
using Avalonia.Threading;
using System.Reactive.Concurrency;

namespace Rajax.Avalonia.CefGlueApp
{
    public class CefWebApp : CefApp
    {

        public CefWebRenderProcessHandler RenderProcessHandler { get; set; }
        public CefWebBrowserProcessHandler BrowserProcessHandler { get; set; }
        public event EventHandler WebKitInitialized;
        public event RegisterCustomSchemesHandler RegisterCustomSchemes;
        public CefWebApp()
        {
            RenderProcessHandler = new CefWebRenderProcessHandler();
            //  BrowserProcessHandler = new CefWebBrowserProcessHandler();
        }




        protected override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
        {
            if (CefRuntime.Platform == CefRuntimePlatform.Linux)
            {
                commandLine.AppendSwitch("no-zygote");
                //commandLine.AppendSwitch("disable-gpu");
                //commandLine.AppendSwitch("disable-gpu","1");

            }

            //commandLine.AppendSwitch("enable-devtools-experiments");
              commandLine.AppendSwitch("ignore-certificate-errors");
            //commandLine.AppendSwitch("enable-begin-frame-scheduling");
   
            commandLine.AppendSwitch("enable-gpu", "1");
            commandLine.AppendSwitch("enable-blink-features", "CSSPseudoHas");

            commandLine.AppendSwitch("enable-media-stream");
            commandLine.AppendSwitch("--disable-web-security");
            commandLine.AppendSwitch("--enable-media-stream", "1");
            commandLine.AppendSwitch("--enable-system-flash", "1");
            commandLine.AppendSwitch("--enable-speech-input", "1");


        }

        protected override CefBrowserProcessHandler GetBrowserProcessHandler()
        {
            if (BrowserProcessHandler == null)
            {
                BrowserProcessHandler = new CefWebBrowserProcessHandler();
            }

            return BrowserProcessHandler;
        }
        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return RenderProcessHandler;
        }
        protected override void OnRegisterCustomSchemes(CefSchemeRegistrar registrar)
        {
            RegisterCustomSchemes?.Invoke(this, new RegisterCustomSchemesEventArgs(registrar));
        }


    }
}
