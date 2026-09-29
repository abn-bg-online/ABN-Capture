using System;
using System.Drawing;
using System.IO;
using System.Windows;
using H.NotifyIcon;

namespace ABNCapture
{
    public partial class App : System.Windows.Application
    {
        private TaskbarIcon? _trayIcon;
        public static AppSettings Settings { get; private set; } = AppSettings.Load();

        protected override void OnStartup(StartupEventArgs e)
        {
            // Tray app: stay alive even when no window is visible
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
                LogError(args.ExceptionObject as Exception, "AppDomain.UnhandledException");

            DispatcherUnhandledException += (s, args) =>
            {
                LogError(args.Exception, "DispatcherUnhandledException");
                args.Handled = true;
            };

            try
            {
                base.OnStartup(e);

                var mainWindow = new MainWindow();
                MainWindow = mainWindow;

                // Create the window handle (hotkeys need it) but keep it invisible
                mainWindow.Opacity = 0;
                mainWindow.ShowInTaskbar = false;
                mainWindow.Show();
                mainWindow.Hide();
                mainWindow.Opacity = 1;
                mainWindow.ShowInTaskbar = true;
            }
            catch (Exception ex)
            {
                ShowFatal(ex, "MainWindow");
                Shutdown(1);
                return;
            }

            try
            {
                _trayIcon = (TaskbarIcon)FindResource("TrayIcon");
                _trayIcon.Icon = LoadTrayIcon();
                _trayIcon.ForceCreate(enablesEfficiencyMode: false);
            }
            catch (Exception ex)
            {
                LogError(ex, "TrayIcon");
            }
        }

        private static Icon LoadTrayIcon()
        {
            try
            {
                var exePath = Environment.ProcessPath;
                if (!string.IsNullOrEmpty(exePath))
                {
                    var icon = Icon.ExtractAssociatedIcon(exePath);
                    if (icon != null)
                        return icon;
                }
            }
            catch
            {
            }

            return SystemIcons.Application;
        }

        private static void LogError(Exception? ex, string source)
        {
            try
            {
                var path = Path.Combine(Path.GetTempPath(), "abncapture_startup_error.txt");
                File.AppendAllText(path, $"[{source}] {DateTime.Now}\n{ex}\n\n");
            }
            catch
            {
            }
        }

        private static void ShowFatal(Exception ex, string source)
        {
            LogError(ex, source);
            MessageBox.Show(ex.ToString(), "ABN Capture - startup error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // Tray actions open the frozen-screen overlay with the floating bar
        private void TrayArea_Click(object sender, RoutedEventArgs e)
        {
            SnipOverlay.Start(SnipMode.Area);
        }

        private void TrayFullscreen_Click(object sender, RoutedEventArgs e)
        {
            SnipOverlay.Start(SnipMode.Screen);
        }

        private void TraySettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void TrayExit_Click(object sender, RoutedEventArgs e)
        {
            Shutdown();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ((MainWindow)MainWindow)?.DisposeHotkeys();
            _trayIcon?.Dispose();
            base.OnExit(e);
        }
    }
}
