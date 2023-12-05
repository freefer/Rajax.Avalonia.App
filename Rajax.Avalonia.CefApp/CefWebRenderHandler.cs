using Rajax.Avalonia.CefGlueApp.Browser;
using Xilium.CefGlue;

namespace Rajax.Avalonia.CefGlueApp
{
    internal sealed class CefWebRenderHandler : CefRenderHandler
    {
        private readonly AvaloniaCefBrowser _owner;


        public CefWebRenderHandler(AvaloniaCefBrowser owner)
        {

            _owner = owner;
        }

        protected override bool GetRootScreenRect(CefBrowser browser, ref CefRectangle rect)
        {
            return _owner.GetViewRect(ref rect);
        }

        protected override void GetViewRect(CefBrowser browser, out CefRectangle rect)
        {
            rect = new CefRectangle();
            _owner.GetViewRect(ref rect);
        }
        protected override bool GetScreenPoint(CefBrowser browser, int viewX, int viewY, ref int screenX, ref int screenY)
        {
            _owner.GetScreenPoint(viewX, viewY, ref screenX, ref screenY);
            return true;
        }
        protected override bool GetScreenInfo(CefBrowser browser, CefScreenInfo screenInfo)
        {
            return false;
        }
        protected override void OnPopupShow(CefBrowser browser, bool show)
        {
            _owner.OnPopupShow(show);
        }
        
        protected override CefAccessibilityHandler GetAccessibilityHandler()
        {
            return null;
        }
        protected override void OnPopupSize(CefBrowser browser, CefRectangle rect)
        {
            //_owner.OnPopupSize(rect);
        }
        protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
        {
            if (type == CefPaintElementType.View)
            {
                _owner.HandleViewPaint(browser, type, dirtyRects, buffer, width, height);
            }
            else if (type == CefPaintElementType.Popup)
            {
                _owner.HandlePopupPaint(width, height, dirtyRects, buffer);
            }
        }

        
        protected override void OnAcceleratedPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr sharedHandle)
        {
            
        }
       
        protected override void OnScrollOffsetChanged(CefBrowser browser, double x, double y)
        {
           
        }
        protected override void OnImeCompositionRangeChanged(CefBrowser browser, CefRange selectedRange, CefRectangle[] characterBounds)
        {

        }

    }

}

