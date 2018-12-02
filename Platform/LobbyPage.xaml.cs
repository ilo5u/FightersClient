using Platform.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class LobbyPage : Page
    {
        /// <summary>
        /// 公共访问实例对象
        /// </summary>
        static public LobbyPage Current;
        public Kernel.Pokemen AIPlayer;
        public int userPlayerId;

        public LobbyPage()
        {
            this.InitializeComponent();
            Current = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            BattleFrame.Navigate(typeof(WaitPage));
        }

        internal void HideBattle()
        {
            BattleList.IsPaneOpen = false;
            BattleList.Width = 0;
            BattleControl.IsChecked = false;
        }

        internal void ShowBattle()
        {
            BattleList.IsPaneOpen = false;
            BattleList.Width = 240;
        }

        private void SetAIDisplay()
        {
            Kernel.Property pokemen = AIPlayer.GetProperty();

            IconOfOppoent.Glyph = Converters.PokemenTypeConverter.ExternConvert(pokemen.type);
            NameOfOpponent.Text = pokemen.name;

            HpointsOfOpponent.Text = pokemen.hpoints.ToString();
            AttackOfOpponent.Text = pokemen.attack.ToString();
            DefenseOfOpponent.Text = pokemen.defense.ToString();
            AgilityOfOpponent.Text = pokemen.agility.ToString();

            BreakOfOpponent.Text = pokemen.interval.ToString();
            CriticalOfOpponent.Text = pokemen.critical.ToString();
            HitratioOfOpponent.Text = pokemen.hitratio.ToString();
            ParryratioOfOpponent.Text = pokemen.parryratio.ToString();

            ExpOfOpponent.Text = ExpConverter.Convert(pokemen.exp).ToString();
            LevelOfOpponent.Text = pokemen.level.ToString();
        }

        private void PokemensView_ItemClick(object sender, ItemClickEventArgs e)
        {
            userPlayerId = ((Models.PokemenViewer)e.ClickedItem).Id;
        }

        public enum BattleType
        {
            LEVELUP,
            DADORSON
        }
        public BattleType TypeOfBattle;
        async private void LevelUp_Click(object sender, RoutedEventArgs e)
        {
            userPlayerId = -1;
            /* 选择出战精灵 */
            ContentDialogResult contentDialogResult = await SelectOfPokemens.ShowAsync();
            if (contentDialogResult == ContentDialogResult.Primary)
            {
                if (userPlayerId != -1)
                {
                    TypeOfBattle = BattleType.LEVELUP;
                    App.Client.Core.SetBattlePlayersAndType(userPlayerId, AIPlayer, 0);

                    BattleChoices.Visibility = Visibility.Collapsed;
                    BattleSettings.Visibility = Visibility.Visible;
                    BattleControl.IsChecked = false;
                    BattleFrame.Navigate(typeof(BattlePage));
                }
                else
                {
                    var msgDialog = new Windows.UI.Popups.MessageDialog("请选择一个精灵！") { Title = "" };
                    msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定"));
                    await msgDialog.ShowAsync();
                }
            }
        }

        async private void DadOrSon_Click(object sender, RoutedEventArgs e)
        {
            userPlayerId = -1;
            /* 选择出战精灵 */
            ContentDialogResult contentDialogResult = await SelectOfPokemens.ShowAsync();
            if (contentDialogResult == ContentDialogResult.Primary)
            {
                if (userPlayerId != -1)
                {
                    TypeOfBattle = BattleType.DADORSON;
                    App.Client.Core.SetBattlePlayersAndType(userPlayerId, AIPlayer, 1);

                    BattleChoices.Visibility = Visibility.Collapsed;
                    BattleSettings.Visibility = Visibility.Visible;
                    BattleControl.IsChecked = false;
                    BattleFrame.Navigate(typeof(BattlePage));
                }
                else
                {
                    var msgDialog = new Windows.UI.Popups.MessageDialog("请选择一个精灵！") { Title = "" };
                    msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定"));
                    await msgDialog.ShowAsync();
                }
            }
        }

        private void GiveUp_Click(object sender, RoutedEventArgs e)
        {
            BattleList.IsPaneOpen = false;
        }

        async private void BackToLobby_Click(object sender, RoutedEventArgs e)
        {
            if (App.Client.Core.IsBattleRunning())
            {
                var msgDialog = new Windows.UI.Popups.MessageDialog("确认要退出比赛？退出比赛后无法获得奖励。") { Title = "" };
                msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("确定"));
                msgDialog.Commands.Add(new Windows.UI.Popups.UICommand("取消"));

                if ((await msgDialog.ShowAsync()).Label == "确定")
                {
                    App.Client.Core.ShutdownBattle();

                    App.Client.BattleDriver.Wait();
                    BattleList.IsPaneOpen = false;
                    BattleFrame.Navigate(typeof(WaitPage));
                }
            }
            else
            {
                App.Client.BattleDriver.Wait();
                BattleList.IsPaneOpen = false;
                BattleFrame.Navigate(typeof(WaitPage));
            }
        }

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

        private void OpenAI_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BattleList.IsPaneOpen = true;
            BattleChoices.Visibility = Visibility.Visible;
            BattleSettings.Visibility = Visibility.Collapsed;

            AIPlayer = new Kernel.Pokemen(new Random().Next(1, 4));
            SetAIDisplay();
        }

        private void ClubmenAI_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BattleList.IsPaneOpen = true;
            BattleChoices.Visibility = Visibility.Visible;
            BattleSettings.Visibility = Visibility.Collapsed;

            AIPlayer = new Kernel.Pokemen(new Random().Next(4, 8));
            SetAIDisplay();
        }

        private void ProfessionalAI_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BattleList.IsPaneOpen = true;
            BattleChoices.Visibility = Visibility.Visible;
            BattleSettings.Visibility = Visibility.Collapsed;

            AIPlayer = new Kernel.Pokemen(new Random().Next(8, 13));
            SetAIDisplay();
        }

        private void MasterAI_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BattleList.IsPaneOpen = true;
            BattleChoices.Visibility = Visibility.Visible;
            BattleSettings.Visibility = Visibility.Collapsed;

            AIPlayer = new Kernel.Pokemen(new Random().Next(13, 16));
            SetAIDisplay();
        }
    }
}
