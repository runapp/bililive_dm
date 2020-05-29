using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Bililive_dm
{
    using static WINAPI.USER32;

    /// <summary>
    /// MainOverlay.xaml 的交互逻辑
    /// </summary>
    public partial class MainOverlay : Window
    {
        public MainOverlay()
        {
            this.InitializeComponent();
            this.Topmost = true;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // http://stackoverflow.com/a/551847
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);
            var exStyles = GetExtendedWindowStyles(wndHelper.Handle);
            exStyles |= ExtendedWindowStyles.ToolWindow;
            SetExtendedWindowStyles(wndHelper.Handle, exStyles);
            SetWindowAffinity();
        }

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetWindowAffinity();
        }
        private void SetWindowAffinity()
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);
            SetWindowDisplayAffinity(wndHelper.Handle, Store.DisplayAffinity ? WindowDisplayAffinity.ExcludeFromCapture : 0);
        }
    }
}