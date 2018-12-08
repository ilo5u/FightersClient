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
using Windows.UI.Xaml.Media.Imaging;
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

        static public ObservableCollection<PokemenViewer> DadOrSonPokemens = new ObservableCollection<PokemenViewer>();

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

        internal void BackToLobby()
        {
            BattleFrame.Navigate(typeof(WaitPage));
        }

        private void SetAIDisplay()
        {
            Kernel.Property pokemen = AIPlayer.GetProperty();

            IconOfOppoent.Glyph = PokemenTypeConverter.ExternConvert(pokemen.type);
            BitmapImage image = new BitmapImage {
                UriSource = new Uri("ms-appx:" + ImageConverter.Convert(pokemen.type, pokemen.career), UriKind.RelativeOrAbsolute)
            };
            ImageOfOpponent.Source = image; 
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
            SkillSelectOfLevelUpPokemens.Visibility = Visibility.Collapsed;
            ContentDialogResult contentDialogResult = await SelectOfLeveLUpPkemens.ShowAsync();
            if (contentDialogResult == ContentDialogResult.Primary)
            {
                if (UserPlayerId != -1)
                {
                    TypeOfBattle = BattleType.LEVELUP;
                    App.Client.Core.SetBattlePlayersAndType(UserPlayer.Id, AIPlayer, PrimarySkillType, false);
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

        bool IsOnSelect = false;
        private void DadOrSon_Click(object sender, RoutedEventArgs e)
        {
            if (!IsOnSelect)
            {
                UserPlayerId = -1;
                /* 选择出战精灵 */

                IsOnSelect = true;
                new Task(SelectDadOrSonPokemens).Start();
            }
        }

        async private void SelectDadOrSonPokemens()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, OnSelectBeginCallBack);
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, OnSelectOverCallBack);
            IsOnSelect = false;
        }

        private void OnSelectBeginCallBack()
        {
            DadOrSonPokemens.Clear();
            switch (LevelOfBattle)
            {
                case BattleLevel.OPEN:
                    foreach (var pokemen in App.Client.OnlinePokemens)
                    {
                        if (pokemen.Level < 4)
                           DadOrSonPokemens.Add(pokemen);
                    }
                    break;

                case BattleLevel.CLUBMEN:
                    foreach (var pokemen in App.Client.OnlinePokemens)
                    {
                        if (pokemen.Level > 3 && pokemen.Level < 8)
                            DadOrSonPokemens.Add(pokemen);
                    }
                    break;

                case BattleLevel.PROFESSIONAL:
                    foreach (var pokemen in App.Client.OnlinePokemens)
                    {
                        if (pokemen.Level > 7 && pokemen.Level < 13)
                            DadOrSonPokemens.Add(pokemen);
                    }
                    break;

                case BattleLevel.MASTER:
                    foreach (var pokemen in App.Client.OnlinePokemens)
                    {
                        if (pokemen.Level > 12 && pokemen.Level < 16)
                            DadOrSonPokemens.Add(pokemen);
                    }
                    break;
            }
        }

        async private void OnSelectOverCallBack()
        {
            SkillSelectOfDadOrSonPokemens.Visibility = Visibility.Collapsed;
            ContentDialogResult contentDialogResult = await SelectOfDadOrSonPokemens.ShowAsync();
            if (contentDialogResult == ContentDialogResult.Primary)
            {
                if (UserPlayerId != -1)
                {
                    TypeOfBattle = BattleType.DADORSON;
                    App.Client.Core.SetBattlePlayersAndType(UserPlayer.Id, AIPlayer, PrimarySkillType, false);
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

        private enum BattleLevel
        {
            OPEN,
            CLUBMEN,
            PROFESSIONAL,
            MASTER
        }
        BattleLevel LevelOfBattle;
        private void OpenAI_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BattleList.IsPaneOpen = true;
            AIPlayer = new Kernel.Pokemen(new Random().Next(1, 4));
            SetAIDisplay();
            LevelOfBattle = BattleLevel.OPEN;
        }

        private void ClubmenAI_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BattleList.IsPaneOpen = true;
            AIPlayer = new Kernel.Pokemen(new Random().Next(4, 8));
            SetAIDisplay();
            LevelOfBattle = BattleLevel.CLUBMEN;
        }

        private void ProfessionalAI_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BattleList.IsPaneOpen = true;
            AIPlayer = new Kernel.Pokemen(new Random().Next(8, 13));
            SetAIDisplay();
            LevelOfBattle = BattleLevel.PROFESSIONAL;
        }

        private void MasterAI_Tapped(object sender, TappedRoutedEventArgs e)
        {
            BattleList.IsPaneOpen = true;
            AIPlayer = new Kernel.Pokemen(new Random().Next(13, 16));
            SetAIDisplay();
            LevelOfBattle = BattleLevel.MASTER;
        }

        private void PokemensViewOfDadOrSonPokemens_ItemClick(object sender, ItemClickEventArgs e)
        {
            UserPlayer = (PokemenViewer)e.ClickedItem;
            UserPlayerId = UserPlayer.Id;
            SkillSelectOfDadOrSonPokemens.Visibility = Visibility.Visible;
            FirstSkillOfDadOrSonPokemens.IsSelected = true;
            FirstSkillOfDadOrSonPokemens.Content = SkillConverter.Convert(UserPlayer.Type, 0);
            SecondSkillOfDadOrSonPokemens.Content = SkillConverter.Convert(UserPlayer.Type, 1);
        }

        bool IsOnWaitForPlayer = false;
        async internal void OnAcceptCallBack(string opponent)
        {
            if (!IsOnWaitForPlayer)
                return;
            WaitForPlayer.Hide();

            string[] pokemenInfoArrays = opponent.Split('\n');
            AIPlayer = new Kernel.Pokemen(int.Parse(pokemenInfoArrays[1]), int.Parse(pokemenInfoArrays[13]));
            AIPlayer.SetProperty(
                int.Parse(pokemenInfoArrays[0]),
                pokemenInfoArrays[2],
                int.Parse(pokemenInfoArrays[3]),
                int.Parse(pokemenInfoArrays[4]),
                int.Parse(pokemenInfoArrays[5]),
                int.Parse(pokemenInfoArrays[6]),
                int.Parse(pokemenInfoArrays[7]),
                int.Parse(pokemenInfoArrays[8]),
                int.Parse(pokemenInfoArrays[9]),
                int.Parse(pokemenInfoArrays[10]),
                int.Parse(pokemenInfoArrays[11])
                );

            UserPlayerId = -1;
            do
            {
                /* 选择出战精灵 */
                SkillSelectOfLevelUpPokemens.Visibility = Visibility.Collapsed;
                await SelectOfLeveLUpPkemens.ShowAsync();
                if (UserPlayerId == -1)
                {
                    var msgDialog = new MessageDialog("请选择一个精灵！") { Title = "" };
                    msgDialog.Commands.Add(new UICommand("确定"));
                    await msgDialog.ShowAsync();
                }
            } while (UserPlayerId == -1);

            TypeOfBattle = BattleType.LEVELUP;
            App.Client.Core.SendMessage(new Kernel.Message
            {
                type = Kernel.MsgType.PVP_BATTLE,
                data = UserPlayerId.ToString()
            });

            App.Client.Core.SetBattlePlayersAndType(UserPlayer.Id, AIPlayer, PrimarySkillType, true);
            BattleFrame.Navigate(typeof(BattlePage));
        }

        /// <summary>
        /// 收到发起方的开始信号
        /// </summary>
        /// <param name="opponent"></param>
        internal void OnBattleCallBack(string opponent)
        {
            if (!IsOnWaitForPlayer)
                return;
            WaitForPlayer.Hide();

            string[] pokemenInfoArrays = opponent.Split('\n');
            AIPlayer = new Kernel.Pokemen(int.Parse(pokemenInfoArrays[1]), int.Parse(pokemenInfoArrays[13]));
            AIPlayer.SetProperty(
                int.Parse(pokemenInfoArrays[0]),
                pokemenInfoArrays[2],
                int.Parse(pokemenInfoArrays[3]),
                int.Parse(pokemenInfoArrays[4]),
                int.Parse(pokemenInfoArrays[5]),
                int.Parse(pokemenInfoArrays[6]),
                int.Parse(pokemenInfoArrays[7]),
                int.Parse(pokemenInfoArrays[8]),
                int.Parse(pokemenInfoArrays[9]),
                int.Parse(pokemenInfoArrays[10]),
                int.Parse(pokemenInfoArrays[11])
                );
            App.Client.Core.SetBattlePlayersAndType(UserPlayerId, AIPlayer, PrimarySkillType, true);
            BattleFrame.Navigate(typeof(BattlePage));
        }

        /// <summary>
        /// 发送正忙信号
        /// </summary>
        async internal void OnBusyCallBack()
        {
            WaitForPlayer.Hide();

            var msgDialog = new MessageDialog("对方正忙！") { Title = "" };
            msgDialog.Commands.Add(new UICommand("确定"));
            await msgDialog.ShowAsync();
        }

        /// <summary>
        /// 发送拒绝信号
        /// </summary>
        async internal void OnRefuseCallBack()
        {
            WaitForPlayer.Hide();

            var msgDialog = new MessageDialog("对方拒绝了您的请求！") { Title = "" };
            msgDialog.Commands.Add(new UICommand("确定"));
            await msgDialog.ShowAsync();
        }

        private void SkillSelectOfDadOrSonPokemens_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FirstSkillOfDadOrSonPokemens.IsSelected == true)
            {
                PrimarySkillType = 0;
            }
            else if (SecondSkillOfDadOrSonPokemens.IsSelected == true)
            {
                PrimarySkillType = 1;
            }
        }

        private void PokemensViewOfLevelUpPokemens_ItemClick(object sender, ItemClickEventArgs e)
        {
            UserPlayer = (PokemenViewer)e.ClickedItem;
            UserPlayerId = UserPlayer.Id;
            SkillSelectOfLevelUpPokemens.Visibility = Visibility.Visible;
            FirstSkillOfLevelUpPokemens.IsSelected = true;
            FirstSkillOfLevelUpPokemens.Content = SkillConverter.Convert(UserPlayer.Type, 0);
            SecondSkillOfLevelUpPokemens.Content = SkillConverter.Convert(UserPlayer.Type, 1);
        }

        private void SkillSelectOfLevelUpPokemens_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FirstSkillOfLevelUpPokemens.IsSelected == true)
            {
                PrimarySkillType = 0;
            }
            else if (SecondSkillOfLevelUpPokemens.IsSelected == true)
            {
                PrimarySkillType = 1;
            }
        }

        async private void OnlineBattle_Click(object sender, RoutedEventArgs e)
        {
            TextBlock onlineuser = ((Button)sender).DataContext as TextBlock;
            if (App.Client.OnlineUsers.First(user => user.Name.Equals(onlineuser.Text)).BattleType)
            { /* 接受对战请求 */
                UserPlayerId = -1;
                do
                {
                    /* 选择出战精灵 */
                    SkillSelectOfLevelUpPokemens.Visibility = Visibility.Collapsed;
                    await SelectOfLeveLUpPkemens.ShowAsync();
                    if (UserPlayerId == -1)
                    {
                        var msgDialog = new MessageDialog("请选择一个精灵！") { Title = "" };
                        msgDialog.Commands.Add(new UICommand("确定"));
                        await msgDialog.ShowAsync();
                    }
                } while (UserPlayerId == -1);

                App.Client.Core.SendMessage(new Kernel.Message
                {
                    type = Kernel.MsgType.PVP_ACCEPT,
                    data = onlineuser.Text
                });
                App.Client.OnlineUsers.First(user => user.Name.Equals(onlineuser.Text)).BattleType = false;

                IsOnWaitForPlayer = true;
                await WaitForPlayer.ShowAsync();
                IsOnWaitForPlayer = false;
            }
            else
            { /* 发起对战请求 */
                App.Client.Core.SendMessage(new Kernel.Message
                {
                    type = Kernel.MsgType.PVP_REQUEST,
                    data = onlineuser.Text
                });

                IsOnWaitForPlayer = true;
                ContentDialogResult result = await WaitForPlayer.ShowAsync();
                IsOnWaitForPlayer = false;
            }
        }

        private void WaitForPlayer_CloseButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            App.Client.Core.SendMessage(new Kernel.Message
            {
                type = Kernel.MsgType.PVP_CANCEL,
                data = ""
            });
        }


    }
}
