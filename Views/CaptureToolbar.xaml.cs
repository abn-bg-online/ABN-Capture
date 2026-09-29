using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ABNCapture
{
    public partial class CaptureToolbar : Window
    {
        private string _captureMode = "area";

        public CaptureToolbar()
        {
            InitializeComponent();

            var workArea = SystemParameters.WorkArea;
            this.Loaded += (s, e) =>
            {
                this.Left = workArea.Right - this.ActualWidth - 24;
                this.Top = workArea.Top + 24;
            };
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void ModeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not ToggleButton clicked) return;

            BtnModeArea.IsChecked = false;
            BtnModeScreen.IsChecked = false;
            BtnModeWindow.IsChecked = false;
            clicked.IsChecked = true;

            _captureMode = clicked.Tag as string ?? "area";
        }

        private void Capture_Click(object sender, RoutedEventArgs e)
        {
            var main = (MainWindow)System.Windows.Application.Current.MainWindow;
            this.Hide();

            switch (_captureMode)
            {
                case "area":
                    main.StartAreaCapture();
                    break;
                case "screen":
                    main.StartFullscreenCapture();
                    break;
                case "window":
                    main.StartAreaCapture();
                    break;
            }

            this.Close();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
