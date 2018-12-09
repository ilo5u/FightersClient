using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Platform
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PVPPage : Page
    {
        /// <summary>
        /// 
        /// </summary>
        public PVPPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string winner = (string)e.Parameter;
            /* 设置比赛结果对话 */
            if (winner == "F")
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () => OnHandleWinCallBack()
                    );
            }
            else if (winner == "S")
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () => OnHandleLoseCallBack()
                    );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnHandleLoseCallBack()
        {
            ResultInfo.Text = "失败";
            ResultInfo.Foreground = new SolidColorBrush(Colors.DarkRed);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnHandleWinCallBack()
        {
            ResultInfo.Text = "胜利";
            ResultInfo.Foreground = new SolidColorBrush(Colors.DarkGreen);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackToLobby_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(WaitPage));
        }
    }
}
