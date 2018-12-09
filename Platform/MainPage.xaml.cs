using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Platform
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// 公共访问实例对象
        /// </summary>
        static public MainPage Current;
        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
        }

        /// <summary>
        /// 当用户处于登陆、注册界面和游戏界面时，标题栏不同
        /// </summary>
        /// <param name="flag"></param>
        public void RenewTitleDisplay(bool flag)
        {
            if (flag)
            {
                HelpListBoxItem.Visibility = Visibility.Collapsed;
                Login.Visibility = Visibility.Collapsed;
                Register.Visibility = Visibility.Collapsed;
                Logout.Visibility = Visibility.Visible;

                Title.Text = "游戏中";
            }
            else
            {
                HelpListBoxItem.Visibility = Visibility.Visible;
                Login.Visibility = Visibility.Visible;
                Register.Visibility = Visibility.Visible;
                Logout.Visibility = Visibility.Collapsed;

                Title.Text = "主页";
            }
        }

        /// <summary>
        /// 返回主页，加载登陆界面
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            RenewTitleDisplay(false);
            SubFrame.Navigate(typeof(LoginPage));
        }

        private void Hamburger_Click(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            HomeListBoxItem.IsSelected = true;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (SubFrame.SourcePageType == typeof(LoginPage))
            {

            }
            else
            {
                SubFrame.Navigate(typeof(LoginPage));
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (SubFrame.SourcePageType == typeof(LogonPage))
            {

            }
            else
            {
                SubFrame.Navigate(typeof(LogonPage));
            }
        }

        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HomeListBoxItem.IsSelected)
            {
                Title.Text = "主页";
                Back.Visibility = Visibility.Collapsed;
                SubFrame.Navigate(typeof(LoginPage));
            }
            else if (HelpListBoxItem.IsSelected)
            {
                Title.Text = "帮助";
                Back.Visibility = Visibility.Visible;
                SubFrame.Navigate(typeof(HelpPage));
            }
        }

        bool IsOnLogout = false;
        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void Logout_Click(object sender, RoutedEventArgs e)
        {
            if (!IsOnLogout)
            {
                IsOnLogout = true;
                bool exit = true;
                if (App.Client.IsOnBattle)
                {
                    /* 处于对战模式 则需要提醒用户 */
                    MessageDialog msgDialog = new MessageDialog("确定要退出？此次比赛将不予保存。") { Title = "" };
                    msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("退出"));
                    msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("取消"));
                    if ((await msgDialog.ShowAsync()).Label == "取消")
                    {
                        exit = false;
                    }
                }
                if (exit)
                {
                    Title.Text = "保存中...";
                    Saving.Visibility = Visibility.Visible;
                    new Task(LogoutTask).Start();
                }
            }
        }

        /// <summary>
        /// 后台线程，释放IO和Battle资源
        /// </summary>
        async public void LogoutTask()
        {
            if (App.Client.IsOnBattle)
            {
                App.Client.IsOnBattle = false;
                if (App.Client.BattleDriver != null)
                {
                    App.Client.BattleDriver.Wait();
                }
            }

            if (App.Client.IsOnConnection)
            {
                App.Client.IsOnConnection = false;
                if (App.Client.NetDriver != null)
                {
                    App.Client.NetDriver.Wait();
                }
            }

            if (LobbyPage.Current != null
                && LobbyPage.Current.IsOnWaitForPlayer)
            {
                LobbyPage.Current.HideWait();
            }

            /* 注销请求 */
            App.Client.Core.SendMessage(
                new Kernel.Message { type = Kernel.MsgType.LOGOUT, data = "" }
                );

            IsOnLogout = false;
            await Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                OnLogoutCallBack
                );
        }

        /// <summary>
        /// 返回登陆界面
        /// </summary>
        public void OnLogoutCallBack()
        {
            Saving.Visibility = Visibility.Collapsed;
            SubFrame.Navigate(typeof(LoginPage));
        }
    }
}
