using Rajax.Avalonia.CefGlueApp.Browser;
using Rajax.Avalonia.CefGlueApp.CefEventArgs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Rajax.Avalonia.CefGlueApp
{
    public class CefWebLifeSpanHandler : CefLifeSpanHandler
    {
        private readonly AvaloniaCefBrowser _core;

        public CefWebLifeSpanHandler(AvaloniaCefBrowser core)
        {
            _core = core;
        }
        protected override void OnAfterCreated(CefBrowser browser)
        { 
            
            _core.HandleAfterCreated(browser);
           
        } 

        protected override bool OnBeforePopup(CefBrowser browser, CefFrame frame, string targetUrl, string targetFrameName, CefWindowOpenDisposition targetDisposition, bool userGesture, CefPopupFeatures popupFeatures, CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings settings, ref CefDictionaryValue extraInfo, ref bool noJavascriptAccess)
        {
            var e = new BeforePopupEventArgs(frame, targetUrl, targetFrameName, popupFeatures, windowInfo, client, settings,
                                 noJavascriptAccess);

            _core.OnBeforePopup(e);

            client = e.Client;
            noJavascriptAccess = e.NoJavascriptAccess;

            return e.Handled;

        }


    }
}

