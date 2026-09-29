using System.Windows.Input;

namespace ABNCapture
{
    public static class HotkeyParser
    {
        public static (uint modifiers, uint virtualKey) Parse(string hotkeyText)
        {
            uint modifiers = 0;
            uint vk = 0;

            var parts = hotkeyText.Split('+', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries);

            foreach (var part in parts)
            {
                switch (part.ToLowerInvariant())
                {
                    case "ctrl":
                        modifiers |= HotkeyManager.MOD_CONTROL;
                        break;
                    case "shift":
                        modifiers |= HotkeyManager.MOD_SHIFT;
                        break;
                    case "alt":
                        modifiers |= HotkeyManager.MOD_ALT;
                        break;
                    case "win":
                        modifiers |= HotkeyManager.MOD_WIN;
                        break;
                    default:
                        if (System.Enum.TryParse<Key>(part, true, out var key))
                        {
                            vk = (uint)KeyInterop.VirtualKeyFromKey(key);
                        }
                        break;
                }
            }

            return (modifiers, vk);
        }
    }
}
