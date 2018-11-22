﻿using System;
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

            ExpOfOpponent.Text = pokemen.exp.ToString();
            LevelOfOpponent.Text = pokemen.level.ToString();
        }

        private void PokemensView_ItemClick(object sender, ItemClickEventArgs e)
        {
            userPlayerId = ((Models.PokemenViewer)e.ClickedItem).Id;
        }

        async private void LevelUp_Click(object sender, RoutedEventArgs e)
        {
            userPlayerId = -1;
            ContentDialogResult contentDialogResult = await SelectOfPokemens.ShowAsync();
            if (contentDialogResult == ContentDialogResult.Primary)
            {
                if (userPlayerId != -1)
                {
                    App.Client.Core.SetBattlePlayersAndType(userPlayerId, AIPlayer, 0);

                    BattleChoices.Visibility = Visibility.Collapsed;
                    BattleSettings.Visibility = Visibility.Visible;
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
            ContentDialogResult contentDialogResult = await SelectOfPokemens.ShowAsync();
            if (contentDialogResult == ContentDialogResult.Primary)
            {
                if (userPlayerId != -1)
                {
                    App.Client.Core.SetBattlePlayersAndType(userPlayerId, AIPlayer, 1);
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

            AIPlayer = new Kernel.Pokemen(1, 1);
            SetAIDisplay();
        }

        private void ClubmenAI_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BattleList.IsPaneOpen = true;

            AIPlayer = new Kernel.Pokemen(1, 3);
            SetAIDisplay();
        }

        private void ProfessionalAI_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BattleList.IsPaneOpen = true;

            AIPlayer = new Kernel.Pokemen(1, 7);
            SetAIDisplay();
        }

        private void MasterAI_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BattleList.IsPaneOpen = true;

            AIPlayer = new Kernel.Pokemen(1, 12);
            SetAIDisplay();
        }
    }
}
