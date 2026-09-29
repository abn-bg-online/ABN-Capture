using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ABNCapture
{
    public class HotkeyManager : IDisposable
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int WM_HOTKEY = 0x0312;

        public const uint MOD_ALT = 0x0001;
        public const uint MOD_CONTROL = 0x0002;
        public const uint MOD_SHIFT = 0x0004;
        public const uint MOD_WIN = 0x0008;

        private readonly IntPtr _handle;
        private int _currentId = 9000;

        public event Action<int>? HotkeyPressed;

        public HotkeyManager(Window window)
        {
            _handle = new WindowInteropHelper(window).Handle;

            if (_handle == IntPtr.Zero)
                throw new InvalidOperationException("Window handle is not ready yet. Call this after the window is shown (e.g. in Loaded event).");

            HwndSource.FromHwnd(_handle)?.AddHook(HwndHook);
        }

        public (int id, bool success) Register(uint modifiers, uint key)
        {
            int id = _currentId++;
            bool success = RegisterHotKey(_handle, id, modifiers, key);
            return (id, success);
        }

        public void Unregister(int id)
        {
            UnregisterHotKey(_handle, id);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                HotkeyPressed?.Invoke(id);
                handled = true;
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            for (int id = 9000; id < _currentId; id++)
                UnregisterHotKey(_handle, id);
        }
    }
}
