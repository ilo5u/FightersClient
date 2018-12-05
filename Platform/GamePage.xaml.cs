﻿using Platform.Converters;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Platform
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class GamePage : Page
    {
        /// <summary>
        /// 公共访问实例对象
        /// </summary>
        static public GamePage Current;

        public GamePage()
        {
            this.InitializeComponent();
            Current = this;
        }

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MainPage.Current.RenewTitleDisplay(true);

            App.Client.Core.SendMessage(
                new Kernel.Message { type = Kernel.MsgType.GET_ONLINE_USERS, data = "" }
                );
            App.Client.IsOnConnection = true;
            App.Client.NetDriver = new Task(NetIOTask);
            App.Client.NetDriver.Start();

            if ((int)e.Parameter == 0)
            {
                await new ContentDialog()
                {
                    Title = "提示",
                    Content = "欢迎来宠物小精灵对战平台。\n我们为您准备了三只初始精灵，您可以在'Profile'选项查看。\n\n祝您游戏愉快！",
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 36,
                    PrimaryButtonText = "我知道了",
                    FullSizeDesired = true,
                }.ShowAsync();
            }
            GameFrame.Navigate(typeof(LobbyPage));
        }

        internal void HideTag()
        {
            GameTag.Visibility = Visibility.Collapsed;
        }

        internal void ShowTag()
        {
            GameTag.Visibility = Visibility.Visible;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            MainPage.Current.RenewTitleDisplay(false);
        }

        public void RenewTitleDisplay(bool flag)
        {
            if (flag)
            {
                GameTag.Visibility = Visibility.Collapsed;
            }
            else
            {
                GameTag.Visibility = Visibility.Visible;
            }
        }

        async private void NetIOTask()
        {
            Kernel.Message message;
            while (App.Client.IsOnConnection)
            {
                message = App.Client.Core.ReadOnlineMessage();
                if (message.type != Kernel.MsgType.INVALID)
                    Debug.WriteLine("收到消息：类型=" + message.type.ToString());

                switch (message.type)
                {
                    case Kernel.MsgType.UPDATE_POKEMENS:
                        {
                            await Dispatcher.RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Normal,
                                () => OnUpdatePokemensCallBack(message.data)
                                );
                        }
                        break;

                    case Kernel.MsgType.SET_ONLINE_USERS:
                        {
                            await Dispatcher.RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Normal,
                                () => OnSetOnlineUsersCallBack(message.data)
                                );
                        }
                        break;

                    case Kernel.MsgType.UPDATE_ONLINE_USERS:
                        {
                            await Dispatcher.RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Normal,
                                () => OnUpdateOnlineUsersCallBack(message.data)
                                );
                        }
                        break;

                    case Kernel.MsgType.ADD_POKEMEN:
                        {
                            await Dispatcher.RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Normal,
                                () => OnAddPokemenCallBack(message.data)
                                );
                        }
                        break;

                    case Kernel.MsgType.SET_POKEMENS_BY_USER:
                        {
                            await Dispatcher.RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Normal,
                                () => OnSetPokemensByUserCallBack(message.data)
                                );
                        }
                        break;

                    case Kernel.MsgType.SET_POKEMENS_OVER:
                        {
                            await Dispatcher.RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Normal,
                                OnSetPokemensOverCallBack
                                );
                        }
                        break;

                    case Kernel.MsgType.RENEW_RANKLIST:
                        {
                            await Dispatcher.RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Normal,
                                () => OnRenewRanklistCallBack(message.data)
                                );
                        }
                        break;

                    case Kernel.MsgType.DISCONNECT:
                        {
                            App.Client.IsOnConnection = false;
                            if (App.Client.IsOnBattle)
                            {
                                App.Client.IsOnBattle = false;
                                App.Client.BattleDriver.Wait();
                            }

                            await Dispatcher.RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Normal,
                                OnDisconnectionCallBack
                                );
                        }
                        return;

                    default:
                        break;
                }
            }
        }

        private void OnRenewRanklistCallBack(string userInfos)
        {
            /* 更新列表数据 */

            /* 排序 */
            RankPage.Current.AllUsers.OrderBy(user => user.NumberOfPokemens);
        }

        private void OnSetPokemensOverCallBack()
        {
            RankPage.Current.ShowPokemensView();
        }

        private void OnSetPokemensByUserCallBack(string pokemenInfos)
        {
            string[] pokemenInfoArray = pokemenInfos.Split('\n');
            RankPage.Current.OtherPokemens.Add(
                    new PokemenViewer
                    {
                        Id = int.Parse(pokemenInfoArray[0]),
                        Type = int.Parse(pokemenInfoArray[1]),
                        Image = ImageConverter.Convert(int.Parse(pokemenInfoArray[1]), int.Parse(pokemenInfoArray[11])),
                        Name = pokemenInfoArray[2],
                        Hpoints = int.Parse(pokemenInfoArray[3]),
                        Attack = int.Parse(pokemenInfoArray[4]),
                        Defense = int.Parse(pokemenInfoArray[5]),
                        Agility = int.Parse(pokemenInfoArray[6]),
                        Interval = int.Parse(pokemenInfoArray[7]),
                        Critical = int.Parse(pokemenInfoArray[8]),
                        Hitratio = int.Parse(pokemenInfoArray[9]),
                        Parryratio = int.Parse(pokemenInfoArray[10]),
                        Career = int.Parse(pokemenInfoArray[11]),
                        Exp = int.Parse(pokemenInfoArray[12]),
                        Level = int.Parse(pokemenInfoArray[13])
                    }
                );
        }

        private void OnAddPokemenCallBack(string pokemenInfo)
        {
            OnUpdatePokemensCallBack(pokemenInfo);
            WinPage.Current.ShowNewPokemen();
        }

        async public void OnDisconnectionCallBack()
        {
            MessageDialog msg = new MessageDialog("与服务器断开连接。") { Title = "错误" };
            msg.Commands.Add(new UICommand("确定"));
            await msg.ShowAsync();

            await Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                MainPage.Current.OnLogoutCallBack
                );
        }

        private void OnUpdatePokemensCallBack(string pokemenInfos)
        {
            Debug.Write(pokemenInfos);
            string[] pokemenInfoArray = pokemenInfos.Split('\n');
            try
            {
                App.Client.Pokemens.First(pokemen => pokemen.Id.Equals(int.Parse(pokemenInfoArray[0]))).Renew(
                        int.Parse(pokemenInfoArray[1]),
                        int.Parse(pokemenInfoArray[3]),
                        int.Parse(pokemenInfoArray[4]),
                        int.Parse(pokemenInfoArray[5]),
                        int.Parse(pokemenInfoArray[6]),
                        int.Parse(pokemenInfoArray[7]),
                        int.Parse(pokemenInfoArray[8]),
                        int.Parse(pokemenInfoArray[9]),
                        int.Parse(pokemenInfoArray[10]),
                        int.Parse(pokemenInfoArray[11]),
                        int.Parse(pokemenInfoArray[12]),
                        int.Parse(pokemenInfoArray[13])
                    );
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                App.Client.Pokemens.Add(
                    new PokemenViewer
                    {
                        Id = int.Parse(pokemenInfoArray[0]),
                        Type = int.Parse(pokemenInfoArray[1]),
                        Image = ImageConverter.Convert(int.Parse(pokemenInfoArray[1]), int.Parse(pokemenInfoArray[11])),
                        Name = pokemenInfoArray[2],
                        Hpoints = int.Parse(pokemenInfoArray[3]),
                        Attack = int.Parse(pokemenInfoArray[4]),
                        Defense = int.Parse(pokemenInfoArray[5]),
                        Agility = int.Parse(pokemenInfoArray[6]),
                        Interval = int.Parse(pokemenInfoArray[7]),
                        Critical = int.Parse(pokemenInfoArray[8]),
                        Hitratio = int.Parse(pokemenInfoArray[9]),
                        Parryratio = int.Parse(pokemenInfoArray[10]),
                        Career = int.Parse(pokemenInfoArray[11]),
                        Exp = int.Parse(pokemenInfoArray[12]),
                        Level = int.Parse(pokemenInfoArray[13])
                    }
                );
            }
        }

        private void OnSetOnlineUsersCallBack(string userInfos)
        {
            Debug.WriteLine(userInfos);
            string[] userInfoArray = userInfos.Split('\n');
            int total = int.Parse(userInfoArray[0]);
            for (int i = 1; i <= total; ++i)
                App.Client.Users.Add(new OnlineUserViewer { Name = userInfoArray[i] });
        }

        private void OnUpdateOnlineUsersCallBack(string userInfos)
        {
            Debug.WriteLine(userInfos);
            string[] userInfoArray = userInfos.Split('\n');
            try
            {
                if (userInfoArray[1] == "OFF")
                    App.Client.Users.Remove(App.Client.Users.First(user => user.Name == userInfoArray[0]));
                else if (userInfoArray[1] == "ON")
                    App.Client.Users.Add(new OnlineUserViewer { Name = userInfoArray[0] });
            }
            catch (Exception e)
            {
                DebugPrint.Text += e.ToString();
            }
        }

        private void LobbyTag_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (GameFrame.SourcePageType == typeof(LobbyPage))
            {

            }
            else
            {
                GameFrame.Navigate(typeof(LobbyPage));

                LobbyTag.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x36, 0x64, 0x8B));
                LobbyHint.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x36, 0x64, 0x8B));

                ProfileTag.Foreground = new SolidColorBrush(Colors.Black);
                ProfileHint.Fill = new SolidColorBrush(Colors.White);

                RankTag.Foreground = new SolidColorBrush(Colors.Black);
                RankHint.Fill = new SolidColorBrush(Colors.White);
            }
        }

        private void ProfileTag_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (GameFrame.SourcePageType == typeof(ProfilePage))
            {

            }
            else
            {
                GameFrame.Navigate(typeof(ProfilePage));

                ProfileTag.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x36, 0x64, 0x8B));
                ProfileHint.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x36, 0x64, 0x8B));

                LobbyTag.Foreground = new SolidColorBrush(Colors.Black);
                LobbyHint.Fill = new SolidColorBrush(Colors.White);

                RankTag.Foreground = new SolidColorBrush(Colors.Black);
                RankHint.Fill = new SolidColorBrush(Colors.White);
            }
        }

        private void RankTag_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (GameFrame.SourcePageType == typeof(RankPage))
            {

            }
            else
            {
                GameFrame.Navigate(typeof(RankPage));

                RankTag.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x36, 0x64, 0x8B));
                RankHint.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x36, 0x64, 0x8B));

                ProfileTag.Foreground = new SolidColorBrush(Colors.Black);
                ProfileHint.Fill = new SolidColorBrush(Colors.White);

                LobbyTag.Foreground = new SolidColorBrush(Colors.Black);
                LobbyHint.Fill = new SolidColorBrush(Colors.White);
            }
        }
    }
}
