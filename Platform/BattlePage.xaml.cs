using Kernel;
using Platform.Converters;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Platform
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BattlePage : Page
    {
        /// <summary>
        /// 公共访问实例对象
        /// </summary>
        static public BattlePage Current;

        /// <summary>
        /// 玩家精灵的显示信息
        /// </summary>
        public ObservableCollection<StateViewer> PlayerStates = new ObservableCollection<StateViewer>();
        public PokemenViewer PlayerDisplay = new PokemenViewer();

        /// <summary>
        /// 电脑精灵的显示信息
        /// </summary>
        public ObservableCollection<StateViewer> AIStates = new ObservableCollection<StateViewer>();
        public PokemenViewer AIDisplay = new PokemenViewer();

        /// <summary>
        /// 
        /// </summary>
        public BattlePage()
        {
            this.InitializeComponent();
            Current = this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (LobbyPage.Current.TypeOfBattle == LobbyPage.BattleType.DADORSON
                || LobbyPage.Current.TypeOfBattle == LobbyPage.BattleType.PVP)
                BackToLobby.Visibility = Visibility.Collapsed; // 隐藏“返回大厅”按钮
            else
                BackToLobby.Visibility = Visibility.Visible;

            PlayerStates  = new ObservableCollection<StateViewer>();
            AIStates      = new ObservableCollection<StateViewer>();
            PlayerDisplay = (PokemenViewer)App.Client.Core.GetPropertyAt(LobbyPage.Current.UserPlayer.Id);
            AIDisplay     = (PokemenViewer)LobbyPage.Current.AIPlayer.GetProperty();

            if (LobbyPage.Current.SenderOrReciver
                || LobbyPage.Current.TypeOfBattle != LobbyPage.BattleType.PVP)
            {
                // 启动对战信息实时接收线程
                App.Client.IsOnBattle = true;
                (App.Client.BattleDriver = new Task(BattleTask)).Start();
                App.Client.Core.StartBattle();

                BattleControl.IsChecked = false;
                BattleControl.Content = "▶";
            }
            else
            {
                BattleControl.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        async private void BattleTask()
        {
            Message message;
            while (App.Client.IsOnBattle)
            {
                message = App.Client.Core.ReadOfflineMessage();
                string[] infos = message.data.Split('\n');
                switch (message.type)
                {
                    case MsgType.PVE_MESSAGE:
                    case MsgType.PVP_MESSAGE:
                        {
                            if (infos[0] == "R")
                            {
                                // 更新属性值
                                await Dispatcher.RunAsync(
                                    Windows.UI.Core.CoreDispatcherPriority.Normal,
                                    () => OnRenewDisplayCallBack(infos[1], infos[2])
                                    );
                            }
                            else if (infos[0] == "F")
                            {
                                // 上方小精灵攻击
                                await Dispatcher.RunAsync(
                                    Windows.UI.Core.CoreDispatcherPriority.Normal,
                                    () => OnDisplayFirstPlayerCallBack(infos[1])
                                    );
                            }
                            else if (infos[0] == "S")
                            {
                                // 下方小精灵攻击
                                await Dispatcher.RunAsync(
                                    Windows.UI.Core.CoreDispatcherPriority.Normal,
                                    () => OnDisplaySecondPlayerCallBack(infos[1])
                                    );
                            }
                        }
                        break;

                    case MsgType.PVE_RESULT:
                    case MsgType.PVP_RESULT:
                        {
                            App.Client.IsOnBattle = false;
                            await Dispatcher.RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Normal,
                                () => OnResultCallBack(infos)
                                );
                        }
                        return;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 比赛结束
        /// </summary>
        /// <param name="infos"></param>
        internal void OnResultCallBack(string[] infos)
        {
            switch (LobbyPage.Current.TypeOfBattle)
            {
                case LobbyPage.BattleType.LEVELUP:
                    Frame.Navigate(typeof(ResultPage), infos);
                    break;

                case LobbyPage.BattleType.DADORSON:
                    Frame.Navigate(typeof(BonusPage), infos[0]);
                    break;

                case LobbyPage.BattleType.PVP:
                    Frame.Navigate(typeof(PVPPage), infos[0]);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 更新精灵UI信息
        /// </summary>
        /// <param name="firstPlayer"></param>
        /// <param name="secondPlayer"></param>
        internal void OnRenewDisplayCallBack(string firstPlayer, string secondPlayer)
        {
            string[] firstProperties = firstPlayer.Split(',');

            FirstHpoints.Text = firstProperties[0];
            FirstAttack.Text = firstProperties[1];
            FirstDefense.Text = firstProperties[2];
            FirstAgility.Text = firstProperties[3];

            FirstInterval.Text = firstProperties[4];
            FirstCritical.Text = firstProperties[5];
            FirstHitratio.Text = firstProperties[6];
            FirstParryratio.Text = firstProperties[7];

            FirstPlayerAnger.Value = int.Parse(firstProperties[8]);

            RenewStates(PlayerStates, int.Parse(firstProperties[9]));

            string[] secondProperties = secondPlayer.Split(',');

            SecondHpoints.Text = secondProperties[0];
            SecondAttack.Text = secondProperties[1];
            SecondDefense.Text = secondProperties[2];
            SecondAgility.Text = secondProperties[3];

            SecondInterval.Text = secondProperties[4];
            SecondCritical.Text = secondProperties[5];
            SecondHitratio.Text = secondProperties[6];
            SecondParryratio.Text = secondProperties[7];

            SecondPlayerAnger.Value = int.Parse(secondProperties[8]);

            RenewStates(AIStates, int.Parse(secondProperties[9]));
        }

        /// <summary>
        /// 更新精灵状态UI
        /// </summary>
        /// <param name="stateViewers"></param>
        /// <param name="states"></param>
        private void RenewStates(ObservableCollection<StateViewer> stateViewers, int states)
        {
            if ((states & (int)StateConverter.StateType.ANGRIED) > 0)
            {
                if (!stateViewers.Contains(new StateViewer { Type = StateConverter.StateType.ANGRIED }))
                    stateViewers.Add(new StateViewer { Type = StateConverter.StateType.ANGRIED });
            }
            else
            {
                stateViewers.Remove(new StateViewer { Type = StateConverter.StateType.ANGRIED });
            }

            if ((states & (int)StateConverter.StateType.ARMOR) > 0)
            {
                if (!stateViewers.Contains(new StateViewer { Type = StateConverter.StateType.ARMOR }))
                    stateViewers.Add(new StateViewer { Type = StateConverter.StateType.ARMOR });
            }
            else
            {
                stateViewers.Remove(new StateViewer { Type = StateConverter.StateType.ARMOR });
            }

            if ((states & (int)StateConverter.StateType.AVATAR) > 0)
            {
                if (!stateViewers.Contains(new StateViewer { Type = StateConverter.StateType.AVATAR }))
                    stateViewers.Add(new StateViewer { Type = StateConverter.StateType.AVATAR });
            }
            else
            {
                stateViewers.Remove(new StateViewer { Type = StateConverter.StateType.AVATAR });
            }

            if ((states & (int)StateConverter.StateType.BLEED) > 0)
            {
                if (!stateViewers.Contains(new StateViewer { Type = StateConverter.StateType.BLEED }))
                    stateViewers.Add(new StateViewer { Type = StateConverter.StateType.BLEED });
            }
            else
            {
                stateViewers.Remove(new StateViewer { Type = StateConverter.StateType.BLEED });
            }

            if ((states & (int)StateConverter.StateType.DIZZYING) > 0)
            {
                if (!stateViewers.Contains(new StateViewer { Type = StateConverter.StateType.DIZZYING }))
                    stateViewers.Add(new StateViewer { Type = StateConverter.StateType.DIZZYING });
            }
            else
            {
                stateViewers.Remove(new StateViewer { Type = StateConverter.StateType.DIZZYING });
            }

            if ((states & (int)StateConverter.StateType.INSPIRED) > 0)
            {
                if (!stateViewers.Contains(new StateViewer { Type = StateConverter.StateType.INSPIRED }))
                    stateViewers.Add(new StateViewer { Type = StateConverter.StateType.INSPIRED });
            }
            else
            {
                stateViewers.Remove(new StateViewer { Type = StateConverter.StateType.INSPIRED });
            }

            if ((states & (int)StateConverter.StateType.RAGED) > 0)
            {
                if (!stateViewers.Contains(new StateViewer { Type = StateConverter.StateType.RAGED }))
                    stateViewers.Add(new StateViewer { Type = StateConverter.StateType.RAGED });
            }
            else
            {
                stateViewers.Remove(new StateViewer { Type = StateConverter.StateType.RAGED });
            }

            if ((states & (int)StateConverter.StateType.REBOUND) > 0)
            {
                if (!stateViewers.Contains(new StateViewer { Type = StateConverter.StateType.REBOUND }))
                    stateViewers.Add(new StateViewer { Type = StateConverter.StateType.REBOUND });
            }
            else
            {
                stateViewers.Remove(new StateViewer { Type = StateConverter.StateType.REBOUND });
            }

            if ((states & (int)StateConverter.StateType.SILENT) > 0)
            {
                if (!stateViewers.Contains(new StateViewer { Type = StateConverter.StateType.SILENT }))
                    stateViewers.Add(new StateViewer { Type = StateConverter.StateType.SILENT });
            }
            else
            {
                stateViewers.Remove(new StateViewer { Type = StateConverter.StateType.SILENT });
            }

            if ((states & (int)StateConverter.StateType.SLOWED) > 0)
            {
                if (!stateViewers.Contains(new StateViewer { Type = StateConverter.StateType.SLOWED }))
                    stateViewers.Add(new StateViewer { Type = StateConverter.StateType.SLOWED });
            }
            else
            {
                stateViewers.Remove(new StateViewer { Type = StateConverter.StateType.SLOWED });
            }

            if ((states & (int)StateConverter.StateType.SUNDERED) > 0)
            {
                if (!stateViewers.Contains(new StateViewer { Type = StateConverter.StateType.SUNDERED }))
                    stateViewers.Add(new StateViewer { Type = StateConverter.StateType.SUNDERED });
            }
            else
            {
                stateViewers.Remove(new StateViewer { Type = StateConverter.StateType.SUNDERED });
            }

            if ((states & (int)StateConverter.StateType.WEAKEN) > 0)
            {
                if (!stateViewers.Contains(new StateViewer { Type = StateConverter.StateType.WEAKEN }))
                    stateViewers.Add(new StateViewer { Type = StateConverter.StateType.WEAKEN });
            }
            else
            {
                stateViewers.Remove(new StateViewer { Type = StateConverter.StateType.WEAKEN });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        internal void OnDisplayFirstPlayerCallBack(string message)
        {
            BattleMessageOfFirstPlayer.Text += message + '\n';
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        internal void OnDisplaySecondPlayerCallBack(string message)
        {
            BattleMessageOfSecondPlayer.Text += message + '\n';
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void BackToLobby_Click(object sender, RoutedEventArgs e)
        {
            if (App.Client.Core.IsBattleRunning())
            {
                var msgDialog = new MessageDialog("确认要退出比赛？退出比赛后无法获得奖励。") { Title = "" };
                msgDialog.Commands.Add(new UICommand("确定"));
                msgDialog.Commands.Add(new UICommand("取消"));

                if ((await msgDialog.ShowAsync()).Label == "确定")
                {
                    App.Client.Core.ShutdownBattle();

                    App.Client.IsOnBattle = false;
                    App.Client.BattleDriver.Wait();
                    Frame.Navigate(typeof(WaitPage));
                }
            }
            else
            {
                App.Client.BattleDriver.Wait();
                Frame.Navigate(typeof(WaitPage));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BattleControl_Click(object sender, RoutedEventArgs e)
        {
            if (BattleControl.IsChecked == true)
            {
                BattleControl.Content = "⏸";
                App.Client.Core.SetBattleOn();
            }
            else
            {
                BattleControl.Content = "▶";
                App.Client.Core.SetBattlePasue();
            }
        }
    }
}
