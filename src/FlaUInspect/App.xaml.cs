using System;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;

namespace FlaUInspect
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            // register low level exception handler
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            // register high level exception filter
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            System.Windows.Forms.Application.ThreadException += WinFormApplication_ThreadException;
        }

        void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs args)
        {
        }

        // An unhandled exception was thrown in the current dispatcher thread.
        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs args)
        {
            lock (messages)
            {
                Exception exception = args.Exception;

                if (!messages.Contains(exception.Message))
                {
                    string caption = "FlaUInspect - Dispatcher exception";
                    string message = "Do you want to mark the exception as handled?";

                    message += "\nException: " + exception.Message;

                    if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        args.Handled = true;

                        messages.Add(exception.Message);
                    }
                }
            }
        }

        // Unhandled exception was thrown in the current application domain
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            lock (messages)
            {
                string caption = "FlaUInspect - Unhandled exception";
                string message = "Common language runtime is terminating: " + args.IsTerminating;

                if (args.ExceptionObject is Exception exception)
                {
                    message += "\nException: " + exception.Message;

                    Notepad.Show(exception.ToText());
                }

                MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Unobserved task exception was thrown in the executing thread.
        void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs args)
        {
            lock (messages)
            {
                Exception exception = args.Exception;

                if (!messages.Contains(exception.Message))
                {
                    string caption = "FlaUInspect - Unobserved exception";
                    string message = "Do you want to mark the exception as observed?";

                    message += "\nException: " + exception.Message;

                    if (MessageBox.Show(message, caption, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        args.SetObserved();

                        messages.Add(exception.Message);
                    }
                }
            }
        }

        // Unhandled exception was thrown in the current running thread
        void WinFormApplication_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs args)
        {
            lock (messages)
            {
                string caption = "FlaUInspect - Thread exception";
                string message = "Unhandled exception was thrown in the current running thread.";

                message += "\nException: " + args.Exception.Message;

                MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        List<string> messages = new List<string>();
    }

    public static class Extensions
    {
        public static string ToText(this Exception exception)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("Exception   : {0}\n", exception.Message);

            if (exception.StackTrace != null)
            {
                sb.AppendFormat("Stack Trace : {0}\n", exception.StackTrace);
            }

            while (exception.InnerException != null)
            {
                exception = exception.InnerException;

                sb.AppendFormat("  Inner Exception : {0}\n", exception.Message);

                if (exception.StackTrace != null)
                {
                    sb.AppendFormat("  Stack Trace     : {0}\n", exception.StackTrace);
                }
            }

            return sb.ToString();
        }
    }

    public static class Notepad
    {
        [DllImport("user32.dll")] static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")] static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        private const int WM_SETTEXT = 0x0c;

        public static void Show(string text)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo("notepad.exe");

                if (process.Start())
                {
                    while (process.MainWindowHandle == IntPtr.Zero)
                    {
                        Thread.Sleep(100);
                    }

                    IntPtr child = FindWindowEx(process.MainWindowHandle, new IntPtr(0), "Edit", null);

                    SendMessage(child, WM_SETTEXT, 0, text);
                }
            }
        }
    }
}
