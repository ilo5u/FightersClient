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
    public sealed partial class WinPage : Page
    {
        /// <summary>
        /// 公共访问实例
        /// </summary>
        static public WinPage Current;

        /// <summary>
        /// 用户新获得的精灵
        /// </summary>
        public PokemenViewer NewPokemen = new PokemenViewer();

        /// <summary>
        /// 
        /// </summary>
        public WinPage()
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
            WaitForNewPokemen.Visibility = Visibility.Visible;
            NewPokemenDisplay.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 显示新的精灵
        /// </summary>
        internal void ShowNewPokemen()
        {
            PokemenViewer pokemen = App.Client.OnlinePokemens.Last();
            NewPokemen.Id = pokemen.Id;

            NewPokemen.Type = pokemen.Type;
            NewPokemen.Image = pokemen.Image;
            NewPokemen.Name = pokemen.Name;

            NewPokemen.Hpoints = pokemen.Hpoints;
            NewPokemen.Attack = pokemen.Attack;
            NewPokemen.Defense = pokemen.Defense;
            NewPokemen.Agility = pokemen.Agility;

            NewPokemen.Interval = pokemen.Interval;
            NewPokemen.Critical = pokemen.Critical;
            NewPokemen.Hitratio = pokemen.Hitratio;
            NewPokemen.Parryratio = pokemen.Parryratio;

            NewPokemen.Career = pokemen.Career;
            NewPokemen.Exp = pokemen.Exp;
            NewPokemen.Level = pokemen.Level;

            NewPokemen.PrimarySkill = pokemen.PrimarySkill;
            NewPokemen.SecondSkill = pokemen.SecondSkill;

            WaitForNewPokemen.Visibility = Visibility.Collapsed;
            NewPokemenDisplay.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackToLobby_Click(object sender, RoutedEventArgs e)
        {
            LobbyPage.Current.BackToLobby();
        }
    }
}
