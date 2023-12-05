using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Rajax.Avalonia.CefGlueApp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;
using Avalonia.Layout;
using Avalonia.VisualTree;
using Avalonia.Threading;
using Xilium.CefGlue.Platform.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace Rajax.Avalonia.CefGlueApp.Browser
{
    public class AvaloniaCefBrowser : Control
    {
        private static readonly Key[] HandledKeys =
     {
            Key.Tab, Key.Home, Key.End, Key.Left, Key.Right, Key.Up, Key.Down
        };
        private bool _disposed;
        private CefBrowser _browser;
        private CefBrowserHost _browserHost;
        private CefWebClient _cefClient;
        private bool _created = false;
        public bool AllowsTransparency { get; set; }

        private int _browserWidth;
        private int _browserHeight;
        private bool _browserSizeChanged;
        public string StartUrl { get; set; }

        private Popup _popup;
        private Image _popupImage;
        private WriteableBitmap _popupImageBitmap;

        Dispatcher _mainUiDispatcher;
        public AvaloniaCefBrowser()
        {
            _popup = CreatePopup();

            _mainUiDispatcher = Dispatcher.UIThread;


        }

        ~AvaloniaCefBrowser()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

                if (_browserHost != null)
                {
                    _browserHost.CloseBrowser();
                    _browserHost = null;
                }

                if (_browser != null)
                {
                    _browser.Dispose();
                    _browser = null;
                }
            }

            _disposed = true;
        }

        private Popup CreatePopup()
        {
            var popup = new Popup
            {
                Child = this._popupImage = CreatePopupImage(),
                PlacementTarget = this,
                Placement = PlacementMode.Center
            };

            return popup;
        }
        private Image CreatePopupImage()
        {
            var temp = new Image();

            RenderOptions.SetBitmapInterpolationMode(temp, BitmapInterpolationMode.Default);

            temp.Stretch = Stretch.None;
            temp.HorizontalAlignment = HorizontalAlignment.Left;
            temp.VerticalAlignment = VerticalAlignment.Top;
            temp.Source = _popupImageBitmap;

            return temp;
        }

        public void OnBrowserAfterCreated(CefBrowser browser)
        {
            int width = 0, height = 0;

            bool hasAlreadyBeenInitialized = false;

            _mainUiDispatcher.Post(new Action(delegate
            {
                if (_browser != null)
                {
                    hasAlreadyBeenInitialized = true;
                }
                else
                {
                    _browser = browser;
                    _browserHost = _browser.GetHost();


                    width = (int)_browserWidth;
                    height = (int)_browserHeight;
                }
            }), DispatcherPriority.Normal);

            // Make sure we don't initialize ourselves more than once. That seems to break things.
            if (hasAlreadyBeenInitialized)
                return;

            if (width > 0 && height > 0)
                _browserHost.WasResized();

        }

        public void OnBeforeClose()
        {

        }

        public void OnBeforePopup(BeforePopupEventArgs e)
        {
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.ArrangeOverride(finalSize);

            if (!_created)
            {
                var newWidth = (int)size.Width;
                var newHeight = (int)size.Height;

                AttachEventHandlers(this);

                _browserWidth = newWidth;
                _browserHeight = newHeight;
                _browserSizeChanged = true;
                var parentWnd = this.FindAncestorOfType<Window>().PlatformImpl.Handle;
                if (parentWnd != null)
                {

                    var windowInfo = CefWindowInfo.Create();
                    // windowInfo.SetAsWindowless(parentWnd.Handle, AllowsTransparency);

                    windowInfo.StyleEx |= WindowStyleEx.WS_EX_NOACTIVATE; // disable window activation (prevent stealing focus)
                    windowInfo.SetAsChild(parentWnd.Handle, new CefRectangle(0, 0, _browserWidth, _browserHeight));
                    var settings = new CefBrowserSettings();
                    _cefClient = new CefWebClient(this);

                    // This is the first time the window is being rendered, so create it.
                    CefBrowserHost.CreateBrowser(windowInfo, _cefClient, settings, !string.IsNullOrEmpty(StartUrl) ? StartUrl : "about:blank");

                    _created = true;
                }
                _created = true;
            }


            return size;
        }


        private static CefEventFlags GetMouseModifiers(PointerPoint pointer)
        {
            CefEventFlags modifiers = new CefEventFlags();

            if (pointer.Properties.IsLeftButtonPressed)
                modifiers |= CefEventFlags.LeftMouseButton;

            if (pointer.Properties.IsMiddleButtonPressed)
                modifiers |= CefEventFlags.MiddleMouseButton;

            if (pointer.Properties.IsRightButtonPressed)
                modifiers |= CefEventFlags.RightMouseButton;

            return modifiers;
        }
        private void AttachEventHandlers(AvaloniaCefBrowser browser)
        {
            browser.GotFocus += (sender, arg) =>
            {
                try
                {

                    if (_browserHost != null)
                    {
                        _browserHost.SendFocusEvent(true);
                    }
                }
                catch (Exception ex)
                {

                }
            };

            browser.LostFocus += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        _browserHost.SendFocusEvent(false);
                    }
                }
                catch (Exception ex)
                {
                }
            };

            browser.PointerExited += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        CefMouseEvent mouseEvent = new CefMouseEvent()
                        {
                            X = 0,
                            Y = 0
                        };
                        var pointer = arg.GetCurrentPoint(this);
                        mouseEvent.Modifiers = GetMouseModifiers(pointer);

                        _browserHost.SendMouseMoveEvent(mouseEvent, true);
                        //_logger.Debug("Browser_MouseLeave");
                    }
                }
                catch (Exception ex)
                {

                }
            };

            browser.PointerMoved += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        Point cursorPos = arg.GetPosition(this);

                        CefMouseEvent mouseEvent = new CefMouseEvent()
                        {
                            X = (int)cursorPos.X,
                            Y = (int)cursorPos.Y
                        };
                        var pointer = arg.GetCurrentPoint(this);
                        mouseEvent.Modifiers = GetMouseModifiers(pointer);

                        _browserHost.SendMouseMoveEvent(mouseEvent, false);

                        //_logger.Debug(string.Format("Browser_MouseMove: ({0},{1})", cursorPos.X, cursorPos.Y));
                    }
                }
                catch (Exception ex)
                {

                }
            };

            browser.PointerPressed += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        Focus();

                        Point cursorPos = arg.GetPosition(this);

                        CefMouseEvent mouseEvent = new CefMouseEvent()
                        {
                            X = (int)cursorPos.X,
                            Y = (int)cursorPos.Y,
                        };
                        var pointer = arg.GetCurrentPoint(this);
                        mouseEvent.Modifiers = GetMouseModifiers(pointer);

                        if (pointer.Properties.IsLeftButtonPressed)
                            _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Left, false, arg.ClickCount);
                        else if (pointer.Properties.IsMiddleButtonPressed)
                            _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Middle, false, arg.ClickCount);
                        else if (pointer.Properties.IsRightButtonPressed)
                            _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Right, false, arg.ClickCount);

                        //_logger.Debug(string.Format("Browser_MouseDown: ({0},{1})", cursorPos.X, cursorPos.Y));
                    }
                }
                catch (Exception ex)
                {

                }
            };

            browser.PointerReleased += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        Point cursorPos = arg.GetPosition(this);

                        CefMouseEvent mouseEvent = new CefMouseEvent()
                        {
                            X = (int)cursorPos.X,
                            Y = (int)cursorPos.Y,
                        };
                        var pointer = arg.GetCurrentPoint(this);
                        mouseEvent.Modifiers = GetMouseModifiers(pointer);

                        if (pointer.Properties.IsLeftButtonPressed)
                            _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Left, false, 1);
                        else if (pointer.Properties.IsMiddleButtonPressed)
                            _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Middle, false, 1);
                        else if (pointer.Properties.IsRightButtonPressed)
                            _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Right, false, 1);

                        //_logger.Debug(string.Format("Browser_MouseUp: ({0},{1})", cursorPos.X, cursorPos.Y));
                    }
                }
                catch (Exception ex)
                {

                }
            };

            browser.PointerWheelChanged += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        Point cursorPos = arg.GetPosition(this);

                        CefMouseEvent mouseEvent = new CefMouseEvent()
                        {
                            X = (int)cursorPos.X,
                            Y = (int)cursorPos.Y,
                        };

                        _browserHost.SendMouseWheelEvent(mouseEvent, 0, (int)arg.Delta.Y);
                    }
                }
                catch (Exception ex)
                {

                }
            };

            // TODO: require more intelligent processing
            browser.TextInput += (sender, arg) =>
            {
                if (_browserHost != null)
                {


                    foreach (var c in arg.Text)
                    {
                        CefKeyEvent keyEvent = new CefKeyEvent()
                        {
                            EventType = CefKeyEventType.Char,
                            WindowsKeyCode = (int)c,
                            // Character = c,
                        };

                        // keyEvent.Modifiers = GetKeyboardModifiers();

                        _browserHost.SendKeyEvent(keyEvent);
                    }
                }

                arg.Handled = true;
            };

            // TODO: require more intelligent processing
            browser.KeyDown += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        //_logger.Debug(string.Format("KeyDown: system key {0}, key {1}", arg.SystemKey, arg.Key));
                        CefKeyEvent keyEvent = new CefKeyEvent()
                        {
                            EventType = CefKeyEventType.RawKeyDown,
                            WindowsKeyCode = (int)arg.Key,
                            NativeKeyCode = 0,
                            IsSystemKey = arg.Key == Key.System,
                        };

                        keyEvent.Modifiers = GetKeyboardModifiers(arg.KeyModifiers);

                        _browserHost.SendKeyEvent(keyEvent);
                    }
                }
                catch (Exception ex)
                {

                }

                arg.Handled = HandledKeys.Contains(arg.Key);
            };

            // TODO: require more intelligent processing
            browser.KeyUp += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        //_logger.Debug(string.Format("KeyUp: system key {0}, key {1}", arg.SystemKey, arg.Key));
                        CefKeyEvent keyEvent = new CefKeyEvent()
                        {
                            EventType = CefKeyEventType.KeyUp,
                            WindowsKeyCode = (int)arg.Key,
                            NativeKeyCode = 0,
                            IsSystemKey = arg.Key == Key.System,
                        };

                        keyEvent.Modifiers = GetKeyboardModifiers(arg.KeyModifiers);

                        _browserHost.SendKeyEvent(keyEvent);
                    }
                }
                catch (Exception ex)
                {

                }

                arg.Handled = true;
            };
            browser._popup.PointerMoved += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        Point cursorPos = arg.GetPosition(this);

                        CefMouseEvent mouseEvent = new CefMouseEvent()
                        {
                            X = (int)cursorPos.X,
                            Y = (int)cursorPos.Y
                        };

                        var pointer = arg.GetCurrentPoint(this);
                        mouseEvent.Modifiers = GetMouseModifiers(pointer);

                        _browserHost.SendMouseMoveEvent(mouseEvent, false);

                        //_logger.Debug(string.Format("Popup_MouseMove: ({0},{1})", cursorPos.X, cursorPos.Y));
                    }
                }
                catch (Exception ex)
                {

                }
            };
            browser._popup.PointerPressed += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        Point cursorPos = arg.GetPosition(this);

                        CefMouseEvent mouseEvent = new CefMouseEvent()
                        {
                            X = (int)cursorPos.X,
                            Y = (int)cursorPos.Y
                        };

                        var pointer = arg.GetCurrentPoint(this);
                        mouseEvent.Modifiers = GetMouseModifiers(pointer);

                        _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Left, true, 1);

                        //_logger.Debug(string.Format("Popup_MouseDown: ({0},{1})", cursorPos.X, cursorPos.Y));
                    }
                }
                catch (Exception ex)
                {

                }
            };
            browser._popup.PointerWheelChanged += (sender, arg) =>
            {
                try
                {
                    if (_browserHost != null)
                    {
                        Point cursorPos = arg.GetPosition(this);

                        CefMouseEvent mouseEvent = new CefMouseEvent()
                        {
                            X = (int)cursorPos.X,
                            Y = (int)cursorPos.Y
                        };

                        var pointer = arg.GetCurrentPoint(this);
                        mouseEvent.Modifiers = GetMouseModifiers(pointer);
                        _browserHost.SendMouseWheelEvent(mouseEvent, 0, (int)arg.Delta.Y);

                        //_logger.Debug(string.Format("MouseWheel: ({0},{1})", cursorPos.X, cursorPos.Y));
                    }
                }
                catch (Exception ex)
                {

                }
            };
        }

        private static CefEventFlags GetKeyboardModifiers(KeyModifiers key)
        {
            CefEventFlags modifiers = new CefEventFlags();

            if (key == KeyModifiers.Alt)
                modifiers |= CefEventFlags.AltDown;

            if (key == KeyModifiers.Control)
                modifiers |= CefEventFlags.ControlDown;

            if (key == KeyModifiers.Shift)
                modifiers |= CefEventFlags.ShiftDown;

            return modifiers;
        }
    }
}
