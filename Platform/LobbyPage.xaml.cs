using Platform.Converters;
using Platform.Models;
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
        int UserPlayerId;
        public PokemenViewer UserPlayer = new PokemenViewer();
        public int PrimarySkillType;

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
            BattleList.Visibility = Visibility.Collapsed;
        }

        internal void ShowBattle()
        {
            BattleList.Visibility = Visibility.Visible;
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

            PrimarySkillOfOpponent.Text = SkillConverter.Convert(pokemen.type, pokemen.primarySkill);
            SecondarySkillOfOpponent.Text = SkillConverter.Convert(pokemen.type, pokemen.secondSkill);
        }

        public enum BattleType
        {
            LEVELUP,
            DADORSON
        }
        public BattleType TypeOfBattle;
        async private void LevelUp_Click(object sender, RoutedEventArgs e)
        {
            UserPlayerId = -1;
            /* 选择出战精灵 */
            SkillSelect.Visibility = Visibility.Collapsed;
            ContentDialogResult contentDialogResult = await SelectOfPokemens.ShowAsync();
            if (contentDialogResult == ContentDialogResult.Primary)
            {
                if (UserPlayerId != -1)
                {
                    TypeOfBattle = BattleType.LEVELUP;
                    App.Client.Core.SetBattlePlayersAndType(UserPlayer.Id, AIPlayer, PrimarySkillType);
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
            UserPlayerId = -1;
            /* 选择出战精灵 */
            SkillSelect.Visibility = Visibility.Collapsed;
            ContentDialogResult contentDialogResult = await SelectOfPokemens.ShowAsync();
            if (contentDialogResult == ContentDialogResult.Primary)
            {
                if (UserPlayerId != -1)
                {
                    TypeOfBattle = BattleType.DADORSON;
                    App.Client.Core.SetBattlePlayersAndType(UserPlayer.Id, AIPlayer, PrimarySkillType);
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

        private void OpenAI_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BattleList.IsPaneOpen = true;
            AIPlayer = new Kernel.Pokemen(new Random().Next(1, 4));
            SetAIDisplay();
        }

        private void ClubmenAI_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BattleList.IsPaneOpen = true;
            AIPlayer = new Kernel.Pokemen(new Random().Next(4, 8));
            SetAIDisplay();
        }

        private void ProfessionalAI_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BattleList.IsPaneOpen = true;
            AIPlayer = new Kernel.Pokemen(new Random().Next(8, 13));
            SetAIDisplay();
        }

        private void MasterAI_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BattleList.IsPaneOpen = true;
            AIPlayer = new Kernel.Pokemen(new Random().Next(13, 16));
            SetAIDisplay();
        }

        private void PokemensView_ItemClick(object sender, ItemClickEventArgs e)
        {
            UserPlayer = (PokemenViewer)e.ClickedItem;
            UserPlayerId = UserPlayer.Id;
            SkillSelect.Visibility = Visibility.Visible;
            FirstSkill.IsSelected = true;
            FirstSkill.Content = SkillConverter.Convert(UserPlayer.Type, 0);
            SecondSkill.Content = SkillConverter.Convert(UserPlayer.Type, 1);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FirstSkill.IsSelected == true)
            {
                PrimarySkillType = 0;
            }
            else if (SecondSkill.IsSelected == true)
            {
                PrimarySkillType = 1;
            }
        }
    }
}
