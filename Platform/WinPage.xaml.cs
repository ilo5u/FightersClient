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
        static public WinPage Current;
        public PokemenViewer NewPokemen = new PokemenViewer();
        public WinPage()
        {
            this.InitializeComponent();
            Current = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            WaitForNewPokemen.Visibility = Visibility.Visible;
            NewPokemenDisplay.Visibility = Visibility.Collapsed;
        }

        internal void ShowNewPokemen()
        {
            NewPokemen = App.Client.Pokemens.Last();

            WaitForNewPokemen.Visibility = Visibility.Collapsed;
            NewPokemenDisplay.Visibility = Visibility.Visible;
        }

        private void BackToLobby_Click(object sender, RoutedEventArgs e)
        {
            LobbyPage.Current.BackToLobby();
        }
    }
}
