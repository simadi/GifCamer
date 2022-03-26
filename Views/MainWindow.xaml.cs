using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using GifCamer.Common;
using HandyControl.Controls;
using HandyControl.Tools;
using MessageBox = System.Windows.Forms.MessageBox;
using Window = System.Windows.Window;

namespace GifCamer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string gifDir;
        public MainWindow()
        {
            InitializeComponent();
            gifDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            Directory.CreateDirectory(gifDir);
        }
        #region 窗口操作
        Rect rcnormal;
        private bool IsNormal = true;
        /// <summary>
        /// 最小化
        /// </summary>
        private void Min_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// 最大化或还原
        /// </summary>
        private void Max_Click(object sender, RoutedEventArgs e)
        {
            if (IsNormal)
            {
                rcnormal = new Rect(this.Left, this.Top, this.Width, this.Height);//保存下当前位置与大小
                this.Left = 0;//设置位置
                this.Top = 0;
                Rect rc = SystemParameters.WorkArea;//获取工作区大小
                this.Width = rc.Width;
                this.Height = rc.Height;
            }
            else
            {
                this.Left = rcnormal.Left;
                this.Top = rcnormal.Top;
                this.Width = rcnormal.Width;
                this.Height = rcnormal.Height;
            }
            IsNormal = !IsNormal;
            IconElement.SetGeometry(this.btnMax, ResourceHelper.GetResource<Geometry>(IsNormal ? "WindowMaxGeometry" : "WindowRestoreGeometry"));
        }
        /// <summary>
        /// 鼠标双击 最大化或还原窗体
        /// </summary>
        private void MaxWin(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                IsNormal = this.ActualWidth != SystemParameters.WorkArea.Width;
                Max_Click(null, null);
            }
        }
        private void CloseWin_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        private bool stop = false;
        private void btnStartClick(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled=  stop = false;
            btnStop.IsEnabled = !stop;
            txtMsg.Visibility = Visibility.Hidden;
            var i = 0;
            var sb = new StringBuilder();
            var ss = new Queue<Image>();
            var gifPath = Path.Combine(gifDir, $"{DateTime.Now:yyMMddHHmmss}.gif");
            using (FileStream fs = new FileStream(gifPath, FileMode.Create))
            using (var encoder = new GifEncoder(fs))
            {
                var dt = DateTime.Now;
                while (true)
                {
                    sb.AppendLine($"{DateTime.Now:HH:mm:ss.fff}");
                    Print.printScreen();
                    if (Clipboard.ContainsImage())//印屏成功
                    {
                        i++;
                        ss.Enqueue(System.Windows.Forms.Clipboard.GetImage());
                    }
                    if (stop)
                    {
                        var total = (DateTime.Now - dt).TotalSeconds;
                        sb.AppendLine($"共耗时:{total:0.000}秒,共{i}帧,平均{(i/total):0.000}帧/秒----------------------------------------");
                        break;
                    }
                }
                foreach (Image image in ss)
                {
                    encoder.AddFrame(image);
                }
            }

            File.AppendAllText(Path.Combine(gifDir, $"log{DateTime.Now:yyyy-MM-dd}.txt"), sb.ToString());
            txtMsg.Text = "合成结束!";
            Process.Start(gifDir);
            btnStart.IsEnabled = stop;
            btnStop.IsEnabled = !stop;
        }

        private void btnStopClick(object sender, RoutedEventArgs e)
        {
               stop = true;
               btnStop.IsEnabled = false;
               txtMsg.Visibility = Visibility.Visible;
               txtMsg.Text = "合成中,请稍候.......";
        }
    }
}
