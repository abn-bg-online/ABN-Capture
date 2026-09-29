using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace ABNCapture
{
    public enum SnipMode
    {
        Area,
        Screen
    }

    public partial class SnipOverlay : Window
    {
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int index);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int width, int height,
            IntPtr hdcSrc, int xSrc, int ySrc, int rop);

        private const int SRCCOPY = 0x00CC0020;
        private const int CAPTUREBLT = 0x40000000;

        private const int SM_CXSCREEN = 0;
        private const int SM_XVIRTUALSCREEN = 76;
        private const int SM_YVIRTUALSCREEN = 77;
        private const int SM_CXVIRTUALSCREEN = 78;
        private const int SM_CYVIRTUALSCREEN = 79;

        private enum DragKind { None, Create, Move, TL, TR, BL, BR }

        private static bool _active;

        private readonly BitmapSource _frozen;
        private readonly int _vx, _vy;
        private readonly double _scale; // physical pixels per DIP
        private readonly bool _direct;

        private SnipMode _mode;
        private DragKind _drag = DragKind.None;
        private Point _start;
        private Rect _sel = Rect.Empty;
        private Rect _selAtStart = Rect.Empty;
        private bool _hasSel;

        public static async void Start(SnipMode mode = SnipMode.Area)
        {
            if (_active)
                return;
            _active = true;

            try
            {
                // Let a closing tray menu / window disappear before the screen is frozen
                await Task.Delay(220);

                int vx = GetSystemMetrics(SM_XVIRTUALSCREEN);
                int vy = GetSystemMetrics(SM_YVIRTUALSCREEN);
                int pw = GetSystemMetrics(SM_CXVIRTUALSCREEN);
                int ph = GetSystemMetrics(SM_CYVIRTUALSCREEN);

                double scale;
                using (var g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero))
                    scale = g.DpiX / 96.0;

                BitmapSource frozen;
                using (var bmp = new System.Drawing.Bitmap(pw, ph, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    using (var g = System.Drawing.Graphics.FromImage(bmp))
                    {
                        IntPtr hdcDest = g.GetHdc();
                        IntPtr hdcSrc = GetDC(IntPtr.Zero);
                        try
                        {
                            BitBlt(hdcDest, 0, 0, pw, ph, hdcSrc, vx, vy, SRCCOPY | CAPTUREBLT);
                        }
                        finally
                        {
                            ReleaseDC(IntPtr.Zero, hdcSrc);
                            g.ReleaseHdc(hdcDest);
                        }
                    }

                    using var ms = new MemoryStream();
                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    ms.Position = 0;

                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.StreamSource = ms;
                    bi.EndInit();
                    bi.Freeze();
                    frozen = bi;
                }

                var win = new SnipOverlay(mode, frozen, vx, vy, pw, ph, scale);
                win.Closed += (s, e) => _active = false;
                win.Show();
                win.Activate();
            }
            catch (Exception ex)
            {
                _active = false;
                MessageBox.Show(ex.ToString(), "ABN Capture - capture error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private SnipOverlay(SnipMode mode, BitmapSource frozen, int vx, int vy, int pw, int ph, double scale)
        {
            InitializeComponent();

            _frozen = frozen;
            _vx = vx;
            _vy = vy;
            _scale = scale;

            // "Capture directly" setting: no toolbar, shot is taken right after selecting
            bool directSetting = App.Settings.DirectSelection;
            _direct = directSetting && mode == SnipMode.Area;

            Left = vx / scale;
            Top = vy / scale;
            Width = pw / scale;
            Height = ph / scale;

            FrozenImage.Source = frozen;

            if (directSetting)
                Bar.Visibility = Visibility.Collapsed;

            Loaded += (s, e) =>
            {
                Activate();
                Focus();
                PositionBar();

                if (directSetting && mode == SnipMode.Screen)
                    Capture();
            };

            SetMode(mode);
        }

        // ---------- Bar ----------

        private void Bar_SizeChanged(object sender, SizeChangedEventArgs e) => PositionBar();

        private void PositionBar()
        {
            if (Bar.ActualWidth <= 0)
                return;

            double primaryWidth = GetSystemMetrics(SM_CXSCREEN) / _scale;
            double primaryLeft = -(_vx / _scale);
            double workBottom = SystemParameters.WorkArea.Bottom - (_vy / _scale);

            Bar.Margin = new Thickness(
                primaryLeft + (primaryWidth - Bar.ActualWidth) / 2,
                workBottom - Bar.ActualHeight - 12,
                0, 0);
        }

        private void Mode_Click(object sender, RoutedEventArgs e)
        {
            var tag = (string)((FrameworkElement)sender).Tag;
            SetMode(tag == "screen" ? SnipMode.Screen : SnipMode.Area);
        }

        private void SetMode(SnipMode mode)
        {
            _mode = mode;
            BtnSel.IsChecked = mode == SnipMode.Area;
            BtnScr.IsChecked = mode == SnipMode.Screen;

            if (mode == SnipMode.Screen)
            {
                _sel = new Rect(0, 0, Width, Height);
                _hasSel = true;
            }
            else
            {
                _sel = Rect.Empty;
                _hasSel = false;
            }

            UpdateVisuals();
        }

        private void Capture_Click(object sender, RoutedEventArgs e) => Capture();

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsWindow { Owner = this };
            settings.ShowDialog();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
            else if (e.Key == Key.Enter)
                Capture();
        }

        // ---------- Selection ----------

        private Point Clamp(Point p) =>
            new Point(Math.Max(0, Math.Min(Width, p.X)), Math.Max(0, Math.Min(Height, p.Y)));

        private static double Dist(Point a, Point b) =>
            Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));

        private DragKind HitCorner(Point p)
        {
            const double r = 16;
            if (Dist(p, _sel.TopLeft) <= r) return DragKind.TL;
            if (Dist(p, _sel.TopRight) <= r) return DragKind.TR;
            if (Dist(p, _sel.BottomLeft) <= r) return DragKind.BL;
            if (Dist(p, _sel.BottomRight) <= r) return DragKind.BR;
            return DragKind.None;
        }

        private void Overlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_mode == SnipMode.Screen)
                return;

            var p = Clamp(e.GetPosition(Overlay));

            if (_hasSel)
            {
                var corner = HitCorner(p);
                if (corner != DragKind.None)
                {
                    _drag = corner;
                    _selAtStart = _sel;
                    Overlay.CaptureMouse();
                    return;
                }

                if (_sel.Contains(p))
                {
                    _drag = DragKind.Move;
                    _start = p;
                    _selAtStart = _sel;
                    Overlay.CaptureMouse();
                    return;
                }
            }

            _drag = DragKind.Create;
            _start = p;
            _hasSel = false;
            _sel = new Rect(p, new Size(0, 0));
            Overlay.CaptureMouse();
            UpdateVisuals();
        }

        private void Overlay_MouseMove(object sender, MouseEventArgs e)
        {
            if (_drag == DragKind.None)
                return;

            var p = Clamp(e.GetPosition(Overlay));

            switch (_drag)
            {
                case DragKind.Create:
                    _sel = new Rect(_start, p);
                    break;

                case DragKind.Move:
                    double nx = Math.Max(0, Math.Min(Width - _selAtStart.Width, _selAtStart.X + (p.X - _start.X)));
                    double ny = Math.Max(0, Math.Min(Height - _selAtStart.Height, _selAtStart.Y + (p.Y - _start.Y)));
                    _sel = new Rect(nx, ny, _selAtStart.Width, _selAtStart.Height);
                    break;

                case DragKind.TL:
                    _sel = new Rect(p, _selAtStart.BottomRight);
                    break;
                case DragKind.TR:
                    _sel = new Rect(p, _selAtStart.BottomLeft);
                    break;
                case DragKind.BL:
                    _sel = new Rect(p, _selAtStart.TopRight);
                    break;
                case DragKind.BR:
                    _sel = new Rect(p, _selAtStart.TopLeft);
                    break;
            }

            _hasSel = _sel.Width > 0 && _sel.Height > 0;
            UpdateVisuals();
        }

        private void Overlay_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_drag == DragKind.None)
                return;

            var wasCreate = _drag == DragKind.Create;
            _drag = DragKind.None;
            Overlay.ReleaseMouseCapture();

            _hasSel = _sel.Width >= 4 && _sel.Height >= 4;
            UpdateVisuals();

            if (_direct && wasCreate && _hasSel)
                Capture();
        }

        private void UpdateVisuals()
        {
            var full = new RectangleGeometry(new Rect(0, 0, Width, Height));
            bool showSel = _hasSel && _mode == SnipMode.Area;

            if (showSel)
            {
                DimPath.Data = new CombinedGeometry(GeometryCombineMode.Exclude, full, new RectangleGeometry(_sel));

                SelRect.Visibility = Visibility.Visible;
                Canvas.SetLeft(SelRect, _sel.X);
                Canvas.SetTop(SelRect, _sel.Y);
                SelRect.Width = _sel.Width;
                SelRect.Height = _sel.Height;

                Place(HandleTL, _sel.TopLeft);
                Place(HandleTR, _sel.TopRight);
                Place(HandleBL, _sel.BottomLeft);
                Place(HandleBR, _sel.BottomRight);

                SizeText.Text = $"{(int)Math.Round(_sel.Width * _scale)} × {(int)Math.Round(_sel.Height * _scale)}";
                SizeBadge.Visibility = Visibility.Visible;
                double by = _sel.Y - 34;
                if (by < 4) by = _sel.Y + 6;
                Canvas.SetLeft(SizeBadge, _sel.X);
                Canvas.SetTop(SizeBadge, by);
            }
            else
            {
                DimPath.Data = _mode == SnipMode.Screen ? null : full;
                SelRect.Visibility = Visibility.Collapsed;
                SizeBadge.Visibility = Visibility.Collapsed;
            }

            var handleVis = showSel ? Visibility.Visible : Visibility.Collapsed;
            HandleTL.Visibility = handleVis;
            HandleTR.Visibility = handleVis;
            HandleBL.Visibility = handleVis;
            HandleBR.Visibility = handleVis;
        }

        private static void Place(FrameworkElement el, Point p)
        {
            Canvas.SetLeft(el, p.X - 8);
            Canvas.SetTop(el, p.Y - 8);
        }

        // ---------- Capture ----------

        private void Capture()
        {
            Rect r;
            if (_mode == SnipMode.Screen)
                r = new Rect(0, 0, Width, Height);
            else if (_hasSel)
                r = _sel;
            else
                return;

            int x = Math.Max(0, (int)Math.Round(r.X * _scale));
            int y = Math.Max(0, (int)Math.Round(r.Y * _scale));
            int w = (int)Math.Round(r.Width * _scale);
            int h = (int)Math.Round(r.Height * _scale);

            w = Math.Min(w, _frozen.PixelWidth - x);
            h = Math.Min(h, _frozen.PixelHeight - y);
            if (w < 2 || h < 2)
                return;

            try
            {
                var crop = new CroppedBitmap(_frozen, new Int32Rect(x, y, w, h));
                crop.Freeze();

                var s = App.Settings;
                bool save = s.AlwaysSaveToFile || !s.CopyToClipboard;

                if (save)
                {
                    var dir = string.IsNullOrWhiteSpace(s.SaveFolder)
                        ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "ABN Capture")
                        : s.SaveFolder;
                    Directory.CreateDirectory(dir);

                    var file = Path.Combine(dir, $"ABN_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(crop));
                    using (var fs = File.Create(file))
                        encoder.Save(fs);
                }

                if (s.CopyToClipboard)
                {
                    try { Clipboard.SetImage(crop); } catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ABN Capture - save error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Close();
        }
    }
}
