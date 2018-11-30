using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class ResultPage : Page
    {
        public static ResultPage Current;
        public ResultPage()
        {
            this.InitializeComponent();
            Current = this;
        }

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string[] infos = (string[])e.Parameter;
            /* 设置比赛结果对话 */
            if (infos[0] == "F")
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () => OnHandleWinCallBack(infos[1], infos[2])
                    );
            }
            else if (infos[0] == "S")
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () => OnHandleLoseCallBack(infos[1], infos[2])
                    );
            }
        }

        private void OnHandleLoseCallBack(string loser, string extra)
        {

        }

        private void OnHandleWinCallBack(string winner, string extra)
        {

        }

        private void BackToLobby_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
