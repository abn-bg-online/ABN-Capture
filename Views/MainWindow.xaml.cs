using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Drawing;
using System.Drawing.Imaging;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace ABNCapture
{
    public partial class MainWindow : Window
    {
        private HotkeyManager? _hotkeyManager;
        private int _hotkeyAreaId = -1;
        private int _hotkeyFullscreenId = -1;

        public MainWindow()
        {
            InitializeComponent();
            Directory.CreateDirectory(App.Settings.SaveFolder);
            UpdateHotkeyHintText();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeHotkeys();
        }

        public void InitializeHotkeys()
        {
            if (_hotkeyManager != null) return;

            _hotkeyManager = new HotkeyManager(this);
            _hotkeyManager.HotkeyPressed += OnHotkeyPressed;
            ReapplyHotkeys();
        }

        public void ReapplyHotkeys()
        {
            if (_hotkeyManager == null) return;

            if (_hotkeyAreaId != -1) { _hotkeyManager.Unregister(_hotkeyAreaId); _hotkeyAreaId = -1; }
            if (_hotkeyFullscreenId != -1) { _hotkeyManager.Unregister(_hotkeyFullscreenId); _hotkeyFullscreenId = -1; }

            var (areaMod, areaKey) = HotkeyParser.Parse(App.Settings.HotkeyAreaCapture);
            var (fsMod, fsKey) = HotkeyParser.Parse(App.Settings.HotkeyFullscreenCapture);

            var failedHotkeys = new System.Collections.Generic.List<string>();

            if (areaKey != 0)
            {
                var (id, success) = _hotkeyManager.Register(areaMod, areaKey);
                if (success) _hotkeyAreaId = id;
                else failedHotkeys.Add(App.Settings.HotkeyAreaCapture);
            }

            if (fsKey != 0)
            {
                var (id, success) = _hotkeyManager.Register(fsMod, fsKey);
                if (success) _hotkeyFullscreenId = id;
                else failedHotkeys.Add(App.Settings.HotkeyFullscreenCapture);
            }

            if (failedHotkeys.Count > 0)
            {
                MessageBox.Show(
                    "Тези комбинации вече се ползват от друго приложение и не могат да се регистрират:\n" + string.Join(", ", failedHotkeys) + "\n\nИзбери друга комбинация.",
                    "Hotkey conflict", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void UpdateHotkeyHintText()
        {
            TxtHotkeyHint.Text = App.Settings.HotkeyAreaCapture + " = Select Area   |   " + App.Settings.HotkeyFullscreenCapture + " = Full Screen";
        }

        public void DisposeHotkeys()
        {
            _hotkeyManager?.Dispose();
        }

        private void OnHotkeyPressed(int id)
        {
            if (id == _hotkeyAreaId)
            {
                StartAreaCapture();
            }
            else if (id == _hotkeyFullscreenId)
            {
                StartFullscreenCapture();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void BtnFullscreen_Click(object sender, RoutedEventArgs e)
        {
            StartFullscreenCapture();
        }

        private void BtnArea_Click(object sender, RoutedEventArgs e)
        {
            StartAreaCapture();
        }

        private void BtnRecord_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Recording ще свържем с ScreenRecorderLib в следваща стъпка.");
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        public void ShowToolbar()
        {
            this.Hide();
            var toolbar = new CaptureToolbar();
            toolbar.Show();
        }

        public void StartFullscreenCapture()
        {
            int screenLeft = (int)SystemParameters.VirtualScreenLeft;
            int screenTop = (int)SystemParameters.VirtualScreenTop;
            int screenWidth = (int)SystemParameters.VirtualScreenWidth;
            int screenHeight = (int)SystemParameters.VirtualScreenHeight;

            CaptureRegion(screenLeft, screenTop, screenWidth, screenHeight);
        }

        public void StartAreaCapture()
        {
            this.Hide();

            var overlay = new AreaSelectWindow();
            bool? result = overlay.ShowDialog();

            this.Show();

            if (result == true && overlay.SelectedRegion.HasValue)
            {
                var region = overlay.SelectedRegion.Value;
                CaptureRegion(region.X, region.Y, region.Width, region.Height);
            }
        }

        private void CaptureRegion(int x, int y, int width, int height)
        {
            if (width <= 0 || height <= 0)
                return;

            using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(new Point(x, y), Point.Empty, new Size(width, height));

            var settings = App.Settings;

            if (settings.AlwaysSaveToFile)
            {
                Directory.CreateDirectory(settings.SaveFolder);
                string fileName = "ABN_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
                string fullPath = Path.Combine(settings.SaveFolder, fileName);
                bitmap.Save(fullPath, ImageFormat.Png);
            }

            if (settings.CopyToClipboard)
            {
                System.Windows.Clipboard.SetImage(
                    System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        bitmap.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions()));
            }
        }
    }
}
