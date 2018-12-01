using Platform.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
                    () => OnHandleWinCallBack(infos[2])
                    );
                DisplayOldAndNewProps(infos[3], infos[4]);
            }
            else if (infos[0] == "S")
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                    () => OnHandleLoseCallBack(infos[2])
                    );
                DisplayOldAndNewProps(infos[3], infos[4]);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            GamePage.Current.ShowTag();
        }

        /// <summary>
        /// 设置获得经验的前后小精灵的变化
        /// </summary>
        /// <param name="olds"></param>
        /// <param name="news"></param>
        private void DisplayOldAndNewProps(string olds, string news)
        {
            string[] oldProps = olds.Split(',');
            string[] newProps = news.Split(',');

            OldPlayerIcon.Glyph = PokemenTypeConverter.ExternConvert(int.Parse(oldProps[1]));
            OldPlayerName.Text = oldProps[2];

            OldHpoints.Text = oldProps[3];
            OldAttack.Text = oldProps[4];
            OldDefense.Text = oldProps[5];
            OldAgility.Text = oldProps[6];

            OldInterval.Text = oldProps[7];
            OldCritical.Text = oldProps[8];
            OldHitratio.Text = oldProps[9];
            OldParryratio.Text = oldProps[10];

            OldPlayerExp.Value = ExpConverter.Convert(int.Parse(oldProps[11]), int.Parse(oldProps[12]));
            OldPlayerExpTextBlock.Text = (ExpConverter.Convert(int.Parse(oldProps[11]), int.Parse(oldProps[12])) / 10).ToString() + "/1000";
            OldPlayerLevel.Text = oldProps[13];

            NewPlayerIcon.Glyph = PokemenTypeConverter.ExternConvert(int.Parse(newProps[1]));
            NewPlayerName.Text = newProps[2];

            NewHpoints.Text = newProps[3];
            NewAttack.Text = newProps[4];
            NewDefense.Text = newProps[5];
            NewAgility.Text = newProps[6];

            NewInterval.Text = newProps[7];
            NewCritical.Text = newProps[8];
            NewHitratio.Text = newProps[9];
            NewParryratio.Text = newProps[10];

            NewPlayerExp.Value = ExpConverter.Convert(int.Parse(newProps[11]), int.Parse(newProps[12]));
            NewPlayerExpTextBlock.Text = (ExpConverter.Convert(int.Parse(newProps[11]), int.Parse(newProps[12])) / 10).ToString() + "/1000";
            NewPlayerLevel.Text = newProps[13];
        }

        private void OnHandleLoseCallBack(string exp)
        {
            ResultInfo.Text = "胜利";
            ResultInfo.Foreground = new SolidColorBrush(Colors.DarkGreen);
        }

        private void OnHandleWinCallBack(string exp)
        {
            ResultInfo.Text = "失败";
            ResultInfo.Foreground = new SolidColorBrush(Colors.DarkRed);
        }

        private void BackToLobby_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(WaitPage));
        }
    }
}
