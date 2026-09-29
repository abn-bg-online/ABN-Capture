using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace ABNCapture
{
    public partial class SettingsWindow : Window
    {
        private readonly AppSettings _settings;

        public SettingsWindow()
        {
            InitializeComponent();
            _settings = App.Settings;
            LoadIntoUi();
        }

        private void LoadIntoUi()
        {
            TxtSaveFolder.Text = _settings.SaveFolder;
            ChkAlwaysSave.IsChecked = _settings.AlwaysSaveToFile;
            ChkCopyClipboard.IsChecked = _settings.CopyToClipboard;
            ChkStartWithWindows.IsChecked = _settings.StartWithWindows;
            ChkDirectSelection.IsChecked = _settings.DirectSelection;
            TxtHotkeyArea.Text = _settings.HotkeyAreaCapture;
            TxtHotkeyFullscreen.Text = _settings.HotkeyFullscreenCapture;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void HotkeyBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox box)
                box.Text = "Press keys...";
        }

        private void HotkeyBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            var key = e.Key == Key.System ? e.SystemKey : e.Key;

            if (key == Key.LeftCtrl || key == Key.RightCtrl ||
                key == Key.LeftShift || key == Key.RightShift ||
                key == Key.LeftAlt || key == Key.RightAlt ||
                key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            var parts = new StringBuilder();

            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) parts.Append("Ctrl+");
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) parts.Append("Shift+");
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Alt)) parts.Append("Alt+");
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Windows)) parts.Append("Win+");

            parts.Append(key.ToString());

            if (sender is TextBox box)
            {
                box.Text = parts.ToString();
            }
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select folder for screenshots",
                InitialDirectory = _settings.SaveFolder
            };

            if (dialog.ShowDialog() == true)
            {
                TxtSaveFolder.Text = dialog.FolderName;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            _settings.SaveFolder = TxtSaveFolder.Text;
            _settings.AlwaysSaveToFile = ChkAlwaysSave.IsChecked == true;
            _settings.CopyToClipboard = ChkCopyClipboard.IsChecked == true;
            _settings.StartWithWindows = ChkStartWithWindows.IsChecked == true;
            _settings.DirectSelection = ChkDirectSelection.IsChecked == true;
            _settings.HotkeyAreaCapture = TxtHotkeyArea.Text;
            _settings.HotkeyFullscreenCapture = TxtHotkeyFullscreen.Text;

            _settings.Save();

            if (Application.Current.MainWindow is MainWindow main)
            {
                main.ReapplyHotkeys();
                main.UpdateHotkeyHintText();
            }

            this.Close();
        }
    }
}
