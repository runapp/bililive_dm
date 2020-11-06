﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bililive_dm
{
    /// <summary>
    /// LanguageSelector.xaml 的交互逻辑
    /// </summary>
    public partial class LanguageSelector : Window
    {
        public LanguageSelector()
        {
            InitializeComponent();

            switch (Properties.Settings.Default.lang)
            {
                case "zh":
                default:
                    this.cn.IsChecked = true;
                    break;

            }



        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.cn.IsChecked == true)
            {
                Properties.Settings.Default.lang = "zh";
            }
            Properties.Settings.Default.Save();
            MessageBox.Show(this, "语言设定将在重启弹幕姬后生效.");
            this.Close();

        }
    }
}
