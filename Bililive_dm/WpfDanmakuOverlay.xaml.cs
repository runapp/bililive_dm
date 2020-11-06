﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.ComponentModel;
using System.Windows.Forms;

namespace Bililive_dm
{
    using static WINAPI.USER32;

    /// <summary>
    /// WpfDanmakuOverlay.xaml 的交互逻辑
    /// </summary>
    public partial class WpfDanmakuOverlay : Window, IDanmakuWindow
    {
        public WpfDanmakuOverlay()
        {
            this.InitializeComponent();

            this.Deactivated += Overlay_Deactivated;
            this.Background = Brushes.Transparent;
            this.SourceInitialized += delegate
            {
                IntPtr hWnd = new WindowInteropHelper(this).Handle;
                var exStyles = GetExtendedWindowStyles(hWnd);
                SetExtendedWindowStyles(hWnd, exStyles | ExtendedWindowStyles.Transparent | ExtendedWindowStyles.ToolWindow);
                SetMonitor(Store.FullScreenMonitor);
            };
            this.ShowInTaskbar = false;
            this.Topmost = true;
            this.Top = SystemParameters.WorkArea.Top;
            this.Left = SystemParameters.WorkArea.Left;
            this.Width = SystemParameters.WorkArea.Width;
            this.Height = 550;
        }

        void IDisposable.Dispose()
        {
            // do nothing
        }

        void IDanmakuWindow.Show()
        {
            (this as Window).Show();
        }

        void IDanmakuWindow.Close()
        {
            (this as Window).Close();
        }

        void IDanmakuWindow.ForceTopmost()
        {
            this.Topmost = false;
            this.Topmost = true;
        }

        void IDanmakuWindow.AddDanmaku(DanmakuType type, string comment, uint color)
        {
            if ((this as Window).CheckAccess())
            {
                //<Storyboard x:Key="Storyboard1">
                //			<ThicknessAnimationUsingKeyFrames Storyboard.TargetProperty="(FrameworkElement.Margin)" Storyboard.TargetName="fullScreenDanmaku">
                //				<EasingThicknessKeyFrame KeyTime="0" Value="3,0,0,0"/>
                //				<EasingThicknessKeyFrame KeyTime="0:0:1.9" Value="220,0,0,0"/>
                //			</ThicknessAnimationUsingKeyFrames>
                //		</Storyboard>
                lock (LayoutRoot.Children)
                {
                    var v = new FullScreenDanmaku();
                    v.Text.Text = comment;
                    v.ChangeHeight();
                    var wd = v.Text.DesiredSize.Width;

                    Dictionary<double, bool> dd = new Dictionary<double, bool>();
                    dd.Add(0, true);
                    foreach (var child in LayoutRoot.Children)
                    {
                        if (child is FullScreenDanmaku)
                        {
                            var c = child as FullScreenDanmaku;
                            if (!dd.ContainsKey(Convert.ToInt32(c.Margin.Top)))
                            {
                                dd.Add(Convert.ToInt32(c.Margin.Top), true);
                            }
                            if (c.Margin.Left > (SystemParameters.PrimaryScreenWidth - wd - 50))
                            {
                                dd[Convert.ToInt32(c.Margin.Top)] = false;
                            }
                        }
                    }
                    double top;
                    if (dd.All(p => p.Value == false))
                    {
                        top = dd.Max(p => p.Key) + v.Text.DesiredSize.Height;
                    }
                    else
                    {
                        top = dd.Where(p => p.Value).Min(p => p.Key);
                    }
                    // v.Height = v.Text.DesiredSize.Height;
                    // v.Width = v.Text.DesiredSize.Width;
                    Storyboard s = new Storyboard();
                    Duration duration =
                        new Duration(
                            TimeSpan.FromTicks(Convert.ToInt64((SystemParameters.PrimaryScreenWidth + wd) /
                                                               Store.FullOverlayEffect1 * TimeSpan.TicksPerSecond)));
                    ThicknessAnimation f =
                        new ThicknessAnimation(new Thickness(SystemParameters.PrimaryScreenWidth, top, 0, 0),
                            new Thickness(-wd, top, 0, 0), duration);
                    s.Children.Add(f);
                    s.Duration = duration;
                    Storyboard.SetTarget(f, v);
                    Storyboard.SetTargetProperty(f, new PropertyPath("(FrameworkElement.Margin)"));
                    LayoutRoot.Children.Add(v);
                    s.Completed += s_Completed;
                    s.Begin();
                }
            }
            else
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(
                    () => (this as IDanmakuWindow).AddDanmaku(type, comment, color))
                );
            }
        }

        public void SetMonitor(string deviceName)
        {
            Screen s = Screen.AllScreens.FirstOrDefault(p => p.DeviceName == deviceName) ?? Screen.PrimaryScreen;
            System.Drawing.Rectangle r = s.WorkingArea;
            this.Top = r.Top;
            this.Left = r.Left;
            this.Width = r.Width;
            
        }

        private void s_Completed(object sender, EventArgs e)
        {
            var s = sender as ClockGroup;
            if (s == null) return;
            var c = Storyboard.GetTarget(s.Children[0].Timeline) as FullScreenDanmaku;
            if (c != null)
            {
                LayoutRoot.Children.Remove(c);
            }
        }

        private void Overlay_Deactivated(object sender, EventArgs e)
        {
            if (sender is WpfDanmakuOverlay)
            {
                (sender as WpfDanmakuOverlay).Topmost = true;
            }
        }

        void IDanmakuWindow.OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);
            SetWindowDisplayAffinity(wndHelper.Handle, Store.DisplayAffinity ? WindowDisplayAffinity.ExcludeFromCapture : 0);


            if (e.PropertyName == nameof(Store.FullScreenMonitor))
            {
                SetMonitor(Store.FullScreenMonitor);
            }
            // ignore
        }
    }
}