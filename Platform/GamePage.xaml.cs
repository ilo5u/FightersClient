using Platform.Converters;
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

        /// <summary>
        /// 
        /// </summary>
        public GamePage()
        {
            this.InitializeComponent();
            Current = this;
        }

        /// <summary>
        /// 进入游戏界面
        /// 初始化
        /// 跳转至对战界面
        /// </summary>
        /// <param name="e"></param>
        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MainPage.Current.RenewTitleDisplay(true);

            App.Client.OnlineUsers    = new System.Collections.ObjectModel.ObservableCollection<OnlineUserViewer>();
            App.Client.OnlinePokemens = new System.Collections.ObjectModel.ObservableCollection<PokemenViewer>();
            App.Client.RankedUsers    = new System.Collections.ObjectModel.ObservableCollection<UserInfoViewer>();
            App.Client.RankedPokemens = new System.Collections.ObjectModel.ObservableCollection<PokemenViewer>();

            App.Client.Core.SendMessage(
                new Kernel.Message { type = Kernel.MsgType.GET_ONLINE_USERS, data = "" }
                );
            /* 打开与后台网络的通信 */
            App.Client.IsOnConnection = true;
            App.Client.NetDriver = new Task(NetIOTask);
            App.Client.NetDriver.Start();

            if ((int)e.Parameter == 0)
            {
                await new ContentDialog()
                {
                    Title = "提示",
                    Content = "欢迎来宠物小精灵对战平台。\n我们为您准备了三只初始精灵，您可以在'个人资料'选项查看。\n\n祝您游戏愉快！",
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 36,
                    PrimaryButtonText = "我知道了",
                    FullSizeDesired = true,
                }.ShowAsync();
            }

            GameFrame.Navigate(typeof(LobbyPage));
        }

        /// <summary>
        /// 隐藏标题栏（对战、个人资料、排行榜）
        /// </summary>
        internal void HideTag()
        {
            GameTag.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 显示标题栏（对战、个人资料、排行榜）
        /// </summary>
        internal void ShowTag()
        {
            GameTag.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            MainPage.Current.RenewTitleDisplay(false);
        }

        /// <summary>
        /// 接收后台的网络数据
        /// </summary>
        async private void NetIOTask()
        {
            Kernel.Message message;
            while (App.Client.IsOnConnection)
            {
                message = App.Client.Core.ReadOnlineMessage();
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
                            App.Client.IsRankedPokemensReady.Release();
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

                    case Kernel.MsgType.SET_RANKLIST:
                        {
                            await Dispatcher.RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Normal,
                                () => OnSetRanklistCallBack(message.data)
                                );
                        }
                        break;

                    case Kernel.MsgType.PVP_REQUEST:
                        {
                            if (App.Client.IsOnBattle)
                            { /* 用户正在与电脑对战 */
                                App.Client.Core.SendMessage(new Kernel.Message
                                {
                                    type = Kernel.MsgType.PVP_BUSY,
                                    data = ""
                                });
                            }
                            else
                            {
                                await Dispatcher.RunAsync(
                                    Windows.UI.Core.CoreDispatcherPriority.Normal,
                                    () => OnPVPRequestCallBack(message.data)
                                    );
                            }
                        }
                        break;

                    case Kernel.MsgType.PVP_ACCEPT:
                        {
                            await Dispatcher.RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Normal,
                                () => OnPVPAcceptCallBack(message.data)
                                );
                        }
                        break;

                    case Kernel.MsgType.PVP_CANCEL:
                        {
                            await Dispatcher.RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Normal,
                                () => OnPVPCancelCallBack(message.data)
                                );
                        }
                        break;

                    case Kernel.MsgType.PVP_BUSY:
                        {
                            await Dispatcher.RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Normal,
                                () => OnPVPBusyCallBack()
                                );
                        }
                        break;

                    case Kernel.MsgType.PVP_BATTLE:
                        {
                            await Dispatcher.RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Normal,
                                () => OnPVPBattleCallBack(message.data)
                                );
                        }
                        break;

                    case Kernel.MsgType.PVP_MESSAGE:
                        {
                            if (App.Client.IsOnBattle
                                && (BattlePage.Current != null && LobbyPage.Current.TypeOfBattle == LobbyPage.BattleType.PVP))
                            {
                                string[] infos = message.data.Split('\n');
                                if (infos[0] == "R")
                                {
                                    // 更新属性值
                                    await Dispatcher.RunAsync(
                                        Windows.UI.Core.CoreDispatcherPriority.Normal,
                                        () => BattlePage.Current.OnRenewDisplayCallBack(infos[2], infos[1])
                                        );
                                }
                                else if (infos[0] == "S")
                                {
                                    // 下方小精灵攻击
                                    await Dispatcher.RunAsync(
                                        Windows.UI.Core.CoreDispatcherPriority.Normal,
                                        () => BattlePage.Current.OnDisplayFirstPlayerCallBack(infos[1])
                                        );
                                }
                                else if (infos[0] == "F")
                                {
                                    // 上方小精灵攻击
                                    await Dispatcher.RunAsync(
                                        Windows.UI.Core.CoreDispatcherPriority.Normal,
                                        () => BattlePage.Current.OnDisplaySecondPlayerCallBack(infos[1])
                                        );
                                }
                            }
                        }
                        break;

                    case Kernel.MsgType.PVP_RESULT:
                        {
                            string[] infos = message.data.Split('\n');
                            if (App.Client.IsOnBattle
                                && (BattlePage.Current != null && LobbyPage.Current.TypeOfBattle == LobbyPage.BattleType.PVP))
                            {
                                if (infos[0].Equals("F"))
                                    infos[0] = "S";
                                else
                                    infos[0] = "F";

                                App.Client.IsOnBattle = false;
                                if (BattlePage.Current != null)
                                {
                                    await Dispatcher.RunAsync(
                                        Windows.UI.Core.CoreDispatcherPriority.Normal,
                                        () => BattlePage.Current.OnResultCallBack(infos)
                                        );
                                }
                            }
                        }
                        break;

                    case Kernel.MsgType.DISCONNECT:
                        {
                            Debug.WriteLine("断开连接！");
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

            Debug.WriteLine("网络关闭！");
        }

        private void OnPVPCancelCallBack(string canceler)
        {
            LobbyPage.Current.OnCancelCallBack(canceler);
        }

        /// <summary>
        /// 收到在线对战开始信号，接收者打开对战界面
        /// </summary>
        /// <param name="opponent"></param>
        private void OnPVPBattleCallBack(string opponent)
        {
            LobbyPage.Current.OnBattleCallBack(opponent);
        }

        /// <summary>
        /// 收到在线对战请求信号，更新在线用户列表
        /// </summary>
        /// <param name="requester"></param>
        private void OnPVPRequestCallBack(string requester)
        {
            try
            {
                App.Client.OnlineUsers.Remove(App.Client.OnlineUsers.First(user => user.Name.Equals(requester)));
                OnlineUserViewer onlineUser = new OnlineUserViewer
                {
                    Name = requester,
                    BattleType = true
                };
                App.Client.OnlineUsers.Insert(0, onlineUser);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 对方正忙，取消比赛
        /// </summary>
        private void OnPVPBusyCallBack()
        {
            LobbyPage.Current.OnBusyCallBack();
        }

        /// <summary>
        /// 对方接受比赛请求
        /// </summary>
        /// <param name="opponent"></param>
        private void OnPVPAcceptCallBack(string opponent)
        {
            LobbyPage.Current.OnAcceptCallBack(opponent);
        }

        /// <summary>
        /// 设置排行榜
        /// </summary>
        /// <param name="userInfos"></param>
        private void OnSetRanklistCallBack(string userInfos)
        {
            string[] numberAndUserInfos = userInfos.Split('\n');
            int total = int.Parse(numberAndUserInfos[0]);
            for (int pos = 1; pos <= total; ++pos)
            {
                /* 更新列表数据 */
                string[] userInfoArray = numberAndUserInfos[pos].Split(',');
                try
                {
                    App.Client.RankedUsers.First(user => user.Name.Equals(userInfoArray[0])).Renew(
                        int.Parse(userInfoArray[1]),
                        int.Parse(userInfoArray[2]),
                        int.Parse(userInfoArray[3]),
                        int.Parse(userInfoArray[4])
                        );
                }
                catch (Exception)
                {
                    App.Client.RankedUsers.Add(
                        new UserInfoViewer {
                            Name = userInfoArray[0],
                            NumberOfPokemens = int.Parse(userInfoArray[1]),
                            Rate = RateConverter.Convert(int.Parse(userInfoArray[2]), int.Parse(userInfoArray[3])),
                            Honor = HonorConverter.Convert(int.Parse(userInfoArray[1])),
                            Glory = GloryConverter.Convert(int.Parse(userInfoArray[4]))
                        }
                        );
                }
            }

            var ordered = App.Client.RankedUsers.OrderByDescending(user => user.NumberOfPokemens);
            System.Collections.ObjectModel.ObservableCollection<UserInfoViewer> backup = new System.Collections.ObjectModel.ObservableCollection<UserInfoViewer>();
            foreach (var item in ordered)
            {
                backup.Add(new UserInfoViewer
                {
                    Name = item.Name,
                    NumberOfPokemens = item.NumberOfPokemens,
                    Rate = item.Rate,
                    Honor = item.Honor,
                    Glory = item.Glory
                });
            }
            App.Client.RankedUsers = backup;
        }

        /// <summary>
        /// 更新排行榜
        /// </summary>
        /// <param name="userInfos"></param>
        private void OnRenewRanklistCallBack(string userInfos)
        {
            /* 更新列表数据 */
            string[] userInfoArray = userInfos.Split('\n');
            try
            {
                App.Client.RankedUsers.First(user => user.Name.Equals(userInfoArray[0])).Renew(
                    int.Parse(userInfoArray[1]),
                    int.Parse(userInfoArray[2]),
                    int.Parse(userInfoArray[3]),
                    int.Parse(userInfoArray[4])
                    );
            }
            catch (Exception)
            {
                App.Client.RankedUsers.Add(
                    new UserInfoViewer
                    {
                        Name = userInfoArray[0],
                        NumberOfPokemens = int.Parse(userInfoArray[1]),
                        Rate = RateConverter.Convert(int.Parse(userInfoArray[2]), int.Parse(userInfoArray[3])),
                        Honor = HonorConverter.Convert(int.Parse(userInfoArray[1])),
                        Glory = GloryConverter.Convert(int.Parse(userInfoArray[4]))
                    });
            }

            /* 排序 */
            var ordered = App.Client.RankedUsers.OrderByDescending(user => user.NumberOfPokemens);
            System.Collections.ObjectModel.ObservableCollection<UserInfoViewer> backup = new System.Collections.ObjectModel.ObservableCollection<UserInfoViewer>();
            foreach (var item in ordered)
            {
                backup.Add(new UserInfoViewer
                {
                    Name = item.Name,
                    NumberOfPokemens = item.NumberOfPokemens,
                    Rate = item.Rate,
                    Honor = item.Honor,
                    Glory = item.Glory
                });
            }
            App.Client.RankedUsers = backup;
        }

        /// <summary>
        /// 获取其他用户的小精灵
        /// </summary>
        /// <param name="pokemenInfos"></param>
        private void OnSetPokemensByUserCallBack(string pokemenInfos)
        {
            string[] pokemenInfoArray = pokemenInfos.Split('\n');
            App.Client.RankedPokemens.Add(
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

        /// <summary>
        /// 当前用户获得一个新的小精灵
        /// </summary>
        /// <param name="pokemenInfo"></param>
        private void OnAddPokemenCallBack(string pokemenInfo)
        {
            Debug.WriteLine(pokemenInfo);
            OnUpdatePokemensCallBack(pokemenInfo);
            WinPage.Current.ShowNewPokemen();
        }

        /// <summary>
        /// 与服务器异常断开连接
        /// </summary>
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

        /// <summary>
        /// 当前用户的小精灵有数据更新
        /// </summary>
        /// <param name="pokemenInfos"></param>
        private void OnUpdatePokemensCallBack(string pokemenInfos)
        {
            string[] pokemenInfoArray = pokemenInfos.Split('\n');
            try
            {
                App.Client.OnlinePokemens.First(pokemen => pokemen.Id.Equals(int.Parse(pokemenInfoArray[0]))).Renew(
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
            catch (Exception)
            {
                App.Client.OnlinePokemens.Add(
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

        /// <summary>
        /// 设置在线用户
        /// </summary>
        /// <param name="userInfos"></param>
        private void OnSetOnlineUsersCallBack(string userInfos)
        {
            string[] userInfoArray = userInfos.Split('\n');
            int total = int.Parse(userInfoArray[0]);
            for (int i = 1; i <= total; ++i)
            {
                if (!string.IsNullOrEmpty(userInfoArray[i]))
                {
                    try
                    {
                        App.Client.OnlineUsers.First(user => user.Name.Equals(userInfoArray[i]));
                    }
                    catch (Exception)
                    {
                        App.Client.OnlineUsers.Add(new OnlineUserViewer { Name = userInfoArray[i] });
                    }
                }
            }
        }

        /// <summary>
        /// 当前在线用户有更新
        /// </summary>
        /// <param name="userInfos"></param>
        private void OnUpdateOnlineUsersCallBack(string userInfos)
        {
            Debug.WriteLine(userInfos);
            string[] userInfoArray = userInfos.Split('\n');
            try
            {
                if (userInfoArray[1] == "OFF")
                    App.Client.OnlineUsers.Remove(App.Client.OnlineUsers.First(user => user.Name == userInfoArray[0]));
                else if (userInfoArray[1] == "ON")
                    App.Client.OnlineUsers.Add(new OnlineUserViewer { Name = userInfoArray[0] });
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
