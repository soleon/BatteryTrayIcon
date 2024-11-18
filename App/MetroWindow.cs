using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Button = System.Windows.Controls.Button;

namespace Percentage.App;

public class MetroWindow : Window
{
    static MetroWindow()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroWindow),
            new FrameworkPropertyMetadata(typeof(MetroWindow)));
    }

    public MetroWindow()
    {
        Loaded += (_, _) =>
        {
            if (Template.FindName("CloseButton", this) is Button button) button.Click += (_, _) => Close();

            var accent = new AccentPolicy(3);
            var accentStructSize = Marshal.SizeOf(accent);
            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);
            var data = new WindowCompositionAttributeData(19, accentStructSize, accentPtr);
            SetWindowCompositionAttribute(new WindowInteropHelper(this).Handle, ref data);
            Marshal.FreeHGlobal(accentPtr);
        };
        MouseDown += (_, e) =>
        {
            if (e.ChangedButton == MouseButton.Left) DragMove();
        };
        KeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape) Close();
        };
    }

    [DllImport("user32.dll")]
    private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

    private readonly struct AccentPolicy
    {
        internal AccentPolicy(int accentState)
        {
        }
    }

    private readonly struct WindowCompositionAttributeData
    {
        internal WindowCompositionAttributeData(int attribute, int sizeOfData, IntPtr data)
        {
        }
    }
}