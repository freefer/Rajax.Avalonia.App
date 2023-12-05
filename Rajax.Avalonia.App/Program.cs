using Avalonia;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using log4net;
using log4net.Config;
using Rajax.Avalonia.App.Views;
using Rajax.Avalonia.CefGlueApp;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Xilium.CefGlue;

namespace Rajax.Avalonia.App
{
    internal class Program
    {
        public static ILog Log = LogManager.GetLogger(typeof(Program).Name);
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {

                CefRuntime.Load();
                Console.WriteLine("1");
                var execFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                var logPath = Path.Combine(execFolder, "Config\\log4net.config");
                XmlConfigurator.Configure(new System.IO.FileInfo(logPath));
                var settings = new CefSettings
                {
                    LogSeverity = CefLogSeverity.Error,
                    LogFile = "cef.log",
                    MultiThreadedMessageLoop = true,
                    ResourcesDirPath = execFolder,
                   // RemoteDebuggingPort = 20480,
                    NoSandbox = true,
                    WindowlessRenderingEnabled = true,
                    LocalesDirPath = Path.Combine(execFolder, "locales"),
                    FrameworkDirPath = execFolder,
                    CachePath = Path.Combine(execFolder, "cach"),
                   
                };

                Console.WriteLine("2");
                var argv = args;
                if (CefRuntime.Platform != CefRuntimePlatform.Windows)
                {
                    argv = new string[args.Length + 1];
                    Array.Copy(args, 0, argv, 1, args.Length);
                    argv[0] = "-";
                }
                var mainArgs = new CefMainArgs(argv);
                var app = new CefWebApp();
                var exitCode = CefRuntime.ExecuteProcess(mainArgs, app, IntPtr.Zero);
                Console.WriteLine($"exitCode:{exitCode}");
                //  foreach (var arg in args) { if (arg.StartsWith("--type=")) { return; } }

                Log.Debug($"exitCode{exitCode}");
                if (exitCode != -1)
                {
                    return;
                }

                Console.WriteLine("3");
                CefRuntime.Initialize(mainArgs, settings, app, IntPtr.Zero);
                Console.WriteLine("4 .Initialize");

                BuildAvaloniaApp(args)
    .StartWithClassicDesktopLifetime(args);

                Console.WriteLine("5");

              
                CefRuntime.Shutdown();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {


            }

        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp(string[] args)
            => AppBuilder.Configure<App>()
            .With(new FontManagerOptions
            {
                DefaultFamilyName = "avares://Rajax.Avalonia.App/Assets/msyh.ttf#"
            })
                .UsePlatformDetect()

                //.ConfigureCefGlue(args)
                .LogToTrace()
                .UseReactiveUI();
    }
}