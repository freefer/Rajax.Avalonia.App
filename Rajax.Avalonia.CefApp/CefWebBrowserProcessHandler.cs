using Xilium.CefGlue;
using System.Reactive.Linq;
using Avalonia.Threading;
using System.Reactive.Concurrency;
using System.Reflection;

namespace Rajax.Avalonia.CefGlueApp
{
    public class CefWebBrowserProcessHandler : CefBrowserProcessHandler
    {
       
        protected override void OnBeforeChildProcessLaunch(CefCommandLine commandLine)
        {
            commandLine.AppendSwitch("default-encoding", "utf-8");
            commandLine.AppendSwitch("allow-file-access-from-files");
            commandLine.AppendSwitch("allow-universal-access-from-files");
            commandLine.AppendSwitch("disable-web-security");
            commandLine.AppendSwitch("ignore-certificate-errors");
             
 
        }
        private IDisposable _current;
        private readonly object _scheduleLock = new object();

        protected override void OnScheduleMessagePumpWork(long delayMs)
        {
            lock (_scheduleLock)
            {
                if (_current != null)
                {
                    _current.Dispose();
                }

                if (delayMs <= 0)
                {
                    delayMs = 1;
                }

                _current = Observable.Interval(TimeSpan.FromMilliseconds(delayMs))
                    .Subscribe((i) =>
                    {
                        Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            CefRuntime.DoMessageLoopWork();
                        });
                    });
            }
        }

    }
}
