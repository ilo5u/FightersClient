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
    public sealed partial class BonusPage : Page
    {
        public BonusPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string result = (string)e.Parameter;
            /* 设置比赛结果对话 */
            if (result.Equals("F"))
            {
                App.Client.Core.SendMessage(
                    new Kernel.Message
                    {
                        type = Kernel.MsgType.ADD_POKEMEN,
                        data = ""
                    }
                    );
                ResultInfo.Text = "胜利";
                ResultInfo.Foreground = new SolidColorBrush(Colors.DarkGreen);
                ResultFrame.Navigate(typeof(WinPage));
            }
            else if (result.Equals("S"))
            {
                App.Client.Core.SendMessage(
                    new Kernel.Message
                    {
                        type = Kernel.MsgType.SUB_POKEMEN,
                        data = "0"
                    }
                    );
                ResultInfo.Text = "失败";
                ResultInfo.Foreground = new SolidColorBrush(Colors.DarkRed);
                ResultFrame.Navigate(typeof(LosePage));
            }
        }
    }
}
