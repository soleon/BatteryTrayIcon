using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace Wpf
{
    public class MetroWindow : Window
    {
        static MetroWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroWindow),
                new FrameworkPropertyMetadata(typeof(MetroWindow)));
        }

        public MetroWindow()
        {
            Loaded += (_, __) =>
            {
                if (Template.FindName("CloseButton", this) is Button button)
                {
                    button.Click += (___, ____) => Close();
                }

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
                if (e.ChangedButton == MouseButton.Left)
                {
                    DragMove();
                }
            };
            KeyDown += (_, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    Close();
                }
            };
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        private readonly struct WindowCompositionAttributeData
        {
            internal WindowCompositionAttributeData(int attribute, int sizeOfData, IntPtr data)
            {
                _attribute = attribute;
                _sizeOfData = sizeOfData;
                _data = data;
            }

            private readonly int _attribute;
            private readonly IntPtr _data;
            private readonly int _sizeOfData;
        }

        private readonly struct AccentPolicy
        {
            internal AccentPolicy(int accentState)
            {
                _accentState = accentState;
                _accentFlags = _gradientColor = _animationId = default;
            }

            private readonly int _accentState;
            private readonly int _accentFlags;
            private readonly int _gradientColor;
            private readonly int _animationId;
        }
    }
}