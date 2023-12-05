using Rajax.Avalonia.CefGlueApp.Browser;
using Xilium.CefGlue;

namespace Rajax.Avalonia.CefGlueApp
{
    public class CefWebClient : CefClient
    {
        private AvaloniaCefBrowser browser;

        private CefWebLifeSpanHandler _lifeSpanHandler;
        private CefWebRenderHandler _renderHandler;
        public CefWebClient(AvaloniaCefBrowser owner)
        {
            browser = owner;
            _lifeSpanHandler = new CefWebLifeSpanHandler(owner);
            _renderHandler = new CefWebRenderHandler(owner);
        }


        protected override CefLifeSpanHandler GetLifeSpanHandler()
        {
             return _lifeSpanHandler;
        }


        protected override CefRenderHandler GetRenderHandler()
        {
            return _renderHandler;
        }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefFrame frame, CefProcessId sourceProcess, CefProcessMessage message)
        {
            return base.OnProcessMessageReceived(browser, frame, sourceProcess, message);
        }
    
    }

    public class DevToolsWebClient : CefClient
    {

    }

}
