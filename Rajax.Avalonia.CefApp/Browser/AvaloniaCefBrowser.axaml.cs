using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Rajax.Avalonia.CefGlueApp.CefEventArgs;
using System.Runtime.InteropServices;
using Xilium.CefGlue;
using Avalonia.Layout;

using Avalonia.Controls.Shapes;
using System.Diagnostics;
using System.Reflection.Metadata;
using MouseButton = Avalonia.Input.MouseButton;
 
namespace Rajax.Avalonia.CefGlueApp.Browser
{
     
    public class AvaloniaCefBrowser : TemplatedControl
    {
        private static readonly Key[] HandledKeys =
     {
            Key.Tab, Key.Home, Key.End, Key.Left, Key.Right, Key.Up, Key.Down
        };
        private bool _disposed;
        private bool _created;
        private SynchronizationContext synchronizationContext;
        private Image _browserPageImage;

        private WriteableBitmap _browserPageBitmap;

        private int _browserWidth;
        private int _browserHeight;
        private bool _browserSizeChanged;

        private CefBrowser _browser;
        private CefBrowserHost _browserHost;
        private CefWebClient _cefClient;

        private ToolTip _tooltip;
        private DispatcherTimer _tooltipTimer;

        private Popup _popup;
        private Image _popupImage;
        private WriteableBitmap _popupImageBitmap;

        private TaskCompletionSource<string> _messageReceiveCompletionSource;

        public string StartUrl { get; set; }
        public bool AllowsTransparency { get; set; }


        public CefBrowser Browser => _browser;

        public AvaloniaCefBrowser()
        {
            _popup = CreatePopup();

            synchronizationContext = SynchronizationContext.Current;

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

            RenderOptions.SetBitmapInterpolationMode(temp, BitmapInterpolationMode.None);

            temp.Stretch = Stretch.None;
            temp.HorizontalAlignment = HorizontalAlignment.Left;
            temp.VerticalAlignment = VerticalAlignment.Top;
            temp.Source = _popupImageBitmap;

            return temp;
        }


        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _browserPageImage = e.NameScope.Find<Image>("PART_Image");
        }

       
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = base.ArrangeOverride(arrangeBounds);
            try
            {

                if (_browserPageImage != null)
                {
                    var newWidth = (int)size.Width;
                    var newHeight = (int)size.Height;

                    if (newWidth > 0 && newHeight > 0)
                    {
                        if (!_created)
                        {
                             AttachEventHandlers(this); // TODO: ?

                            // Create the bitmap that holds the rendered website bitmap
                            _browserWidth = newWidth;
                            _browserHeight = newHeight;
                            _browserSizeChanged = true;

                            // Find the window that's hosting us                        
                            var parentWnd = TopLevel.GetTopLevel(this).TryGetPlatformHandle().Handle;
                            if (parentWnd != null)
                            {
                              
                                IntPtr hParentWnd = parentWnd;

                                var windowInfo = CefWindowInfo.Create();
                                windowInfo.SetAsWindowless(hParentWnd, AllowsTransparency);
                                windowInfo.SharedTextureEnabled = true;
                              
                                //windows dot not use Windowless
                               //windowInfo.SetAsChild(hParentWnd, new CefRectangle(0, 0, _browserWidth, _browserHeight));

                                var settings = new CefBrowserSettings();
                                settings.WindowlessFrameRate = 60;
                    
                                
                                _cefClient = new CefWebClient(this);
                                Console.WriteLine($"SetAsChild, hParentWnd is null {hParentWnd == null} {hParentWnd}");
                                // This is the first time the window is being rendered, so create it.
                                CefBrowserHost.CreateBrowser(windowInfo, _cefClient, settings, !string.IsNullOrEmpty(StartUrl) ? StartUrl : "about:blank");

                                _created = true;
                            }
                        }
                        else
                        {
                            // Only update the bitmap if the size has changed
                            if (_browserPageBitmap == null || (_browserPageBitmap.PixelSize.Width != newWidth || _browserPageBitmap.PixelSize.Height != newHeight))
                            {
                                _browserWidth = newWidth;
                                _browserHeight = newHeight;
                                _browserSizeChanged = true;

                                // If the window has already been created, just resize it
                                if (_browserHost != null)
                                {
                                    //windows api
                                  //  NativeMethods.SetWindowPos(_browserHost.GetWindowHandle(), IntPtr.Zero,0, 0, _browserWidth, _browserHeight, SetWindowPosFlags.NoMove | SetWindowPosFlags.NoZOrder);
                                   _browserHost.WasResized();
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return size;

        }
        

         

        private bool isShiftKeyPressed = false;
        private bool isAltKeyPressed = false;
        private bool isControlKeyPressed = false;
        private static CefEventFlags GetKeyboardModifiers(KeyModifiers kbModifiers)
        {
            CefEventFlags modifiers = new CefEventFlags();

            if (kbModifiers == KeyModifiers.Alt)
                modifiers |= CefEventFlags.AltDown;

            if (kbModifiers == KeyModifiers.Control)
                modifiers |= CefEventFlags.ControlDown;

            if (kbModifiers == KeyModifiers.Shift)
                modifiers |= CefEventFlags.ShiftDown;

            return modifiers;
        }

        private static CefEventFlags GetMouseModifiers(PointerPoint pointer)
        {
            CefEventFlags modifiers = new CefEventFlags();

            if (pointer.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
                modifiers |= CefEventFlags.LeftMouseButton;

            if (pointer.Properties.PointerUpdateKind == PointerUpdateKind.MiddleButtonPressed)
                modifiers |= CefEventFlags.MiddleMouseButton;

            if (pointer.Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed)
                modifiers |= CefEventFlags.RightMouseButton;
            if (pointer.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
                modifiers |= CefEventFlags.LeftMouseButton;
            if (pointer.Properties.PointerUpdateKind == PointerUpdateKind.RightButtonReleased)
                modifiers |= CefEventFlags.RightMouseButton;
            return modifiers;
        }

        private CefEventFlags GetKeyboardModifiers()
        {
            CefEventFlags modifiers = new CefEventFlags();
            if (this.isAltKeyPressed)
            {
                modifiers |= CefEventFlags.AltDown;
            }
            else if (this.isControlKeyPressed)
            {
                modifiers |= CefEventFlags.ControlDown;
            }
            else if (this.isShiftKeyPressed)
            {
                modifiers |= CefEventFlags.ShiftDown;
            }
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
                        Point cursorPos = arg.GetPosition(this);
                        CefMouseEvent mouseEvent = new CefMouseEvent()
                        {
                            X = (int)cursorPos.X,
                            Y = (int)cursorPos.Y,
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
                        var mouseButton = arg.GetCurrentPoint(this);
                        mouseEvent.Modifiers = GetMouseModifiers(mouseButton);
                        if (mouseButton.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
                            _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Left, false, arg.ClickCount);
                        else if (mouseButton.Properties.PointerUpdateKind == PointerUpdateKind.MiddleButtonPressed)
                            _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Middle, false, arg.ClickCount);
                        else if (mouseButton.Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed)
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

                        if (pointer.Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
                            _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Left, true, 1);
                        else if (pointer.Properties.PointerUpdateKind == PointerUpdateKind.MiddleButtonReleased)
                            _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Middle, true, 1);
                        else if (pointer.Properties.PointerUpdateKind == PointerUpdateKind.RightButtonReleased)
                            _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Right, true, 1);

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

                        const int WHEEL_DELTA = 120;
                        var pointer = arg.GetCurrentPoint(this);
                        mouseEvent.Modifiers = GetMouseModifiers(pointer);
                        var x = (int)arg.Delta.X * WHEEL_DELTA;
                        var y = (int)arg.Delta.Y * WHEEL_DELTA;
                        _browserHost.SendMouseWheelEvent(mouseEvent, x, y);
                    }

                    arg.Handled = true;
                }
                catch (Exception ex)
                {
                    //_logger.ErrorException("WpfCefBrowser: Caught exception in MouseWheel()", ex);
                }
            };

            //// TODO: require more intelligent processing
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
                            Character = c,
                        };

                        keyEvent.Modifiers = GetKeyboardModifiers();

                        _browserHost.SendKeyEvent(keyEvent);
                    }
                }

                arg.Handled = true;
            };

            //// TODO: require more intelligent processing
            //browser.KeyDown += (sender, arg) =>
            //{
            //    try
            //    {
            //        if (_browserHost != null)
            //        {
            //            //_logger.Debug(string.Format("KeyDown: system key {0}, key {1}", arg.SystemKey, arg.Key));
            //            CefKeyEvent keyEvent = new CefKeyEvent()
            //            {
            //                EventType = CefKeyEventType.RawKeyDown,
            //                WindowsKeyCode = (int)arg.Key,
            //                NativeKeyCode = CefRuntime.Platform == CefRuntimePlatform.Linux ? KeyInterop.VirtualKeyFromKey(arg.Key) : (int)arg.Key,
            //                IsSystemKey = arg.Key == Key.System,
            //            };


            //            keyEvent.Modifiers = GetKeyboardModifiers(arg.KeyModifiers);
            //            if (arg.KeyModifiers == KeyModifiers.Alt)
            //            {
            //                this.isAltKeyPressed = true;
            //            }
            //            else if (arg.KeyModifiers == KeyModifiers.Shift)
            //            {
            //                this.isShiftKeyPressed = true;
            //            }
            //            else if (arg.KeyModifiers == KeyModifiers.Control)
            //            {
            //                this.isControlKeyPressed = true;
            //            }

            //            _browserHost.SendKeyEvent(keyEvent);
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //    }

            //    arg.Handled = HandledKeys.Contains(arg.Key);
            //};



            //browser.KeyUp += (sender, arg) =>
            //    {
            //        try
            //        {
            //            if (_browserHost != null)
            //            {
            //                //_logger.Debug(string.Format("KeyUp: system key {0}, key {1}", arg.SystemKey, arg.Key));
            //                CefKeyEvent keyEvent = new CefKeyEvent()
            //                {
            //                    EventType = CefKeyEventType.KeyUp,
            //                    WindowsKeyCode = (int)arg.Key,
            //                    NativeKeyCode = CefRuntime.Platform == CefRuntimePlatform.Linux ? KeyInterop.VirtualKeyFromKey(arg.Key) : (int)arg.Key,
            //                    IsSystemKey = arg.Key == Key.System,
            //                };

            //                keyEvent.Modifiers = GetKeyboardModifiers(arg.KeyModifiers);
            //                if (arg.KeyModifiers == KeyModifiers.Alt)
            //                {
            //                    this.isAltKeyPressed = false;
            //                }
            //                else if (arg.KeyModifiers == KeyModifiers.Shift)
            //                {
            //                    this.isShiftKeyPressed = false;
            //                }
            //                else if (arg.KeyModifiers == KeyModifiers.Control)
            //                {
            //                    this.isControlKeyPressed = false;
            //                }
            //                _browserHost.SendKeyEvent(keyEvent);
            //            }
            //        }
            //        catch (Exception ex)
            //        {

            //        }

            //        arg.Handled = true;
            //    };

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


            //browser._popup.PointerPressed += (sender, arg) =>
            //{
            //    try
            //    {
            //        if (_browserHost != null)
            //        {
            //            Point cursorPos = arg.GetPosition(this);

            //            CefMouseEvent mouseEvent = new CefMouseEvent()
            //            {
            //                X = (int)cursorPos.X,
            //                Y = (int)cursorPos.Y
            //            };

            //            var pointer = arg.GetCurrentPoint(this);
            //            mouseEvent.Modifiers = GetMouseModifiers(pointer);

            //            _browserHost.SendMouseClickEvent(mouseEvent, CefMouseButtonType.Left, true, 1);

            //            //_logger.Debug(string.Format("Popup_MouseDown: ({0},{1})", cursorPos.X, cursorPos.Y));
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //};
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
                        const int WHEEL_DELTA = 120;
                        var pointer = arg.GetCurrentPoint(this);
                        mouseEvent.Modifiers = GetMouseModifiers(pointer);
                        var x = (int)arg.Delta.X * WHEEL_DELTA;
                        var y = (int)arg.Delta.Y * WHEEL_DELTA;
                        _browserHost.SendMouseWheelEvent(mouseEvent,x  ,y );

                        //_logger.Debug(string.Format("MouseWheel: ({0},{1})", cursorPos.X, cursorPos.Y));
                    }
                }
                catch (Exception ex)
                {

                }
            };

        }

        internal bool OnTooltip(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                // _tooltipTimer.Stop();
                UpdateTooltip(null);
            }
            else
            {
                /*   _tooltipTimer.Tick += (sender, args) => UpdateTooltip(text);
                   _tooltipTimer.Start();*/
            }

            return true;
        }

        //public event LoadStartEventHandler LoadStart;
        //public event LoadEndEventHandler LoadEnd;
        //public event LoadingStateChangeEventHandler LoadingStateChange;
        //public event LoadErrorEventHandler LoadError;
        //public event BrowserCreatedHandler BrowserCreated;

        public static event EventHandler WebKitInitialized;
        public static void OnWebKitInitialized(object sender, EventArgs e)
        {
            WebKitInitialized?.Invoke(sender, e);
        }


        public async Task<string> ExecuteScriptAsync(string code, string scriptUrl = null)
        {
            var message = CefProcessMessage.Create("executeJs");
            message.Arguments.SetString(0, code);
            message.Arguments.SetString(1, scriptUrl);

            _messageReceiveCompletionSource = new TaskCompletionSource<string>();

            _browser.GetMainFrame().SendProcessMessage(CefProcessId.Renderer, message);

            await _messageReceiveCompletionSource.Task;

            var messageReceived = _messageReceiveCompletionSource.Task.Result;

            return messageReceived;
        }

        private void UpdateTooltip(string text)
        {
            Dispatcher.UIThread.InvokeAsync(
                    () =>
                    {
                        if (string.IsNullOrEmpty(text))
                        {
                            //_tooltip.IsOpen = false;
                        }
                        else
                        {
                            //_tooltip.Placement = PlacementMode.Mouse;
                            _tooltip.Content = text;
                            //_tooltip.IsOpen = true;
                            //_tooltip.Visibility = Visibility.Visible;
                        }
                    });

            //_tooltipTimer.Stop();
        }

        public void HandleAfterCreated(CefBrowser browser)
        {
            int width = 0, height = 0;

            bool hasAlreadyBeenInitialized = false;

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (_browser != null)
                {
                    hasAlreadyBeenInitialized = true;
                }
                else
                {
                    _browser = browser;
                    _browserHost = _browser.GetHost();
                    var wi = CefWindowInfo.Create();
                    wi.SetAsPopup(IntPtr.Zero, "DevTools");
                    _browserHost.ShowDevTools(wi, new DevToolsWebClient(), new CefBrowserSettings(), new CefPoint(0, 0));
                    // _browserHost.SetFocus(IsFocused);

                    width = (int)_browserWidth;
                    height = (int)_browserHeight;
                }
                if (hasAlreadyBeenInitialized)
                    return;

                if (width > 0 && height > 0)
                    _browserHost.WasResized();
            });

            // Make sure we don't initialize ourselves more than once. That seems to break things.


            // 			mainUiDispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            // 			{
            // 				if (!string.IsNullOrEmpty(this.initialUrl))
            // 				{
            // 					NavigateTo(this.initialUrl);
            // 					this.initialUrl = string.Empty;
            // 				}
            // 			}));

            //this.BrowserCreated?.Invoke(this, new BrowserCreatedEventArgs(browser));
        }

        public void OnBeforeClose()
        {

        }
        public void OnBeforePopup(BeforePopupEventArgs e)
        {
        }
        internal bool GetViewRect(ref CefRectangle rect)
        {
            bool rectProvided = false;
            CefRectangle browserRect = new CefRectangle();

            // TODO: simplify this
            //_mainUiDispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            //{
            try
            {
                // The simulated screen and view rectangle are the same. This is necessary
                // for popup menus to be located and sized inside the view.
                browserRect.X = browserRect.Y = 0;
                browserRect.Width = (int)_browserWidth;
                browserRect.Height = (int)_browserHeight;

                rectProvided = true;
            }
            catch (Exception ex)
            {
                rectProvided = false;
            }
            //}));

            if (rectProvided)
            {
                rect = browserRect;
            }

            return rectProvided;
        }

        internal void GetScreenPoint(int viewX, int viewY, ref int screenX, ref int screenY)
        {
            PixelPoint ptScreen = new PixelPoint();

            synchronizationContext.Send(_ =>
            {
                try
                {
                    Point ptView = new Point(viewX, viewY);
                    ptScreen = this.PointToScreen(ptView);

                }
                catch (Exception ex)
                {

                }
            }, null);

            screenX = ptScreen.X;
            screenY = ptScreen.Y;
            Debug.WriteLine($"x:{screenX} y:{screenY}");
        }



        internal void HandleViewPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
        {
            // When browser size changed - we just skip frame updating.
            // This is dirty precheck to do not do Invoke whenever is possible.
            if (_browserSizeChanged && (width != _browserWidth || height != _browserHeight)) return;


            // Actual browser size changed check.
            if (_browserSizeChanged && (width != _browserWidth || height != _browserHeight)) return;

            try
            {
                if (_browserSizeChanged)
                {
                    _browserPageBitmap = new WriteableBitmap(new PixelSize((int)_browserWidth, (int)_browserHeight), new Vector(96, 96), PixelFormat.Bgra8888);//new WriteableBitmap((int)_browserWidth, (int)_browserHeight, 96, 96, AllowsTransparency ? PixelFormats.Bgra32 : PixelFormats.Bgr32, null);


                    _browserSizeChanged = false;
                }

                if (_browserPageBitmap != null)
                {
                    DoRenderBrowser(_browserPageBitmap, width, height, dirtyRects, buffer);
                    Dispatcher.UIThread.Post(() =>
                    {
                        _browserPageImage.Source = _browserPageBitmap;
                        _browserPageImage.InvalidateVisual();
                    });


                }

            }
            catch (Exception ex)
            {
            }


        }

        internal void HandlePopupPaint(int width, int height, CefRectangle[] dirtyRects, IntPtr sourceBuffer)
        {
            if (width == 0 || height == 0)
            {
                return;
            }

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                int stride = width * 4;
                int sourceBufferSize = stride * height;

                foreach (CefRectangle dirtyRect in dirtyRects)
                {
                    if (dirtyRect.Width == 0 || dirtyRect.Height == 0)
                    {
                        continue;
                    }

                    int adjustedWidth = dirtyRect.Width;

                    int adjustedHeight = dirtyRect.Height;

                    Rect sourceRect = new Rect(dirtyRect.X, dirtyRect.Y, adjustedWidth, adjustedHeight);

                    // _popupImageBitmap.WritePixels(sourceRect, sourceBuffer, sourceBufferSize, stride, dirtyRect.X, dirtyRect.Y);
                }
            });
        }

        private void DoRenderBrowser(WriteableBitmap bitmap, int browserWidth, int browserHeight, CefRectangle[] dirtyRects, IntPtr sourceBuffer)
        {
            int stride = browserWidth * 4;
            int sourceBufferSize = stride * browserHeight;

            if (browserWidth == 0 || browserHeight == 0)
            {
                return;
            }

            foreach (CefRectangle dirtyRect in dirtyRects)
            {
                if (dirtyRect.Width == 0 || dirtyRect.Height == 0)
                {
                    continue;
                }


                int adjustedWidth = (int)dirtyRect.Width;


                int adjustedHeight = (int)dirtyRect.Height;

            }

            using (var l = bitmap.Lock())
            {
                byte[] managedArray = new byte[sourceBufferSize];

                Marshal.Copy(sourceBuffer, managedArray, 0, sourceBufferSize);

                Marshal.Copy(managedArray, 0, l.Address, sourceBufferSize);
            }
        }



        internal void OnPopupShow(bool show)
        {
            if (_popup == null)
            {
                return;
            }

            Dispatcher.UIThread.InvokeAsync(() => _popup.IsOpen = show);
        }



    }
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);
    }

    [Flags]
    internal enum SetWindowPosFlags : uint
    {
        /// <summary>
        /// If the calling thread and the thread that owns the window are attached to different input queues, 
        /// the system posts the request to the thread that owns the window. This prevents the calling thread from 
        /// blocking its execution while other threads process the request.
        /// </summary>
        /// <remarks>SWP_ASYNCWINDOWPOS</remarks>
        AsyncWindowPosition = 0x4000,

        /// <summary>
        /// Prevents generation of the WM_SYNCPAINT message.
        /// </summary>
        /// <remarks>SWP_DEFERERASE</remarks>
        DeferErase = 0x2000,

        /// <summary>
        /// Draws a frame (defined in the window's class description) around the window.
        /// </summary>
        /// <remarks>SWP_DRAWFRAME</remarks>
        DrawFrame = 0x0020,

        /// <summary>
        /// Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to 
        /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE 
        /// is sent only when the window's size is being changed.
        /// </summary>
        /// <remarks>SWP_FRAMECHANGED</remarks>
        FrameChanged = 0x0020,

        /// <summary>
        /// Hides the window.
        /// </summary>
        /// <remarks>SWP_HIDEWINDOW</remarks>
        HideWindow = 0x0080,

        /// <summary>
        /// Does not activate the window. If this flag is not set, the window is activated and moved to the 
        /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
        /// </summary>
        /// <remarks>SWP_NOACTIVATE</remarks>
        NoActivate = 0x0010,

        /// <summary>
        /// Discards the entire contents of the client area. If this flag is not specified, the valid contents 
        /// of the client area are saved and copied back into the client area after the window is sized or repositioned.
        /// </summary>
        /// <remarks>SWP_NOCOPYBITS</remarks>
        NoCopyBits = 0x0100,

        /// <summary>
        /// Retains the current position (ignores X and Y parameters).
        /// </summary>
        /// <remarks>SWP_NOMOVE</remarks>
        NoMove = 0x0002,

        /// <summary>
        /// Does not change the owner window's position in the Z order.
        /// </summary>
        /// <remarks>SWP_NOOWNERZORDER</remarks>
        NoOwnerZOrder = 0x0200,

        /// <summary>
        /// Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to 
        /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent 
        /// window uncovered as a result of the window being moved. When this flag is set, the application must 
        /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
        /// </summary>
        /// <remarks>SWP_NOREDRAW</remarks>
        NoRedraw = 0x0008,

        /// <summary>
        /// Same as the SWP_NOOWNERZORDER flag.
        /// </summary>
        /// <remarks>SWP_NOREPOSITION</remarks>
        NoReposition = 0x0200,

        /// <summary>
        /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
        /// </summary>
        /// <remarks>SWP_NOSENDCHANGING</remarks>
        NoSendChanging = 0x0400,

        /// <summary>
        /// Retains the current size (ignores the cx and cy parameters).
        /// </summary>
        /// <remarks>SWP_NOSIZE</remarks>
        NoSize = 0x0001,

        /// <summary>
        /// Retains the current Z order (ignores the hWndInsertAfter parameter).
        /// </summary>
        /// <remarks>SWP_NOZORDER</remarks>
        NoZOrder = 0x0004,

        /// <summary>
        /// Displays the window.
        /// </summary>
        /// <remarks>SWP_SHOWWINDOW</remarks>
        ShowWindow = 0x0040,
    }




}

