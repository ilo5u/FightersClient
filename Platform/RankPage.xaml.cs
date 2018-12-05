using Kernel;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class RankPage : Page
    {
        static public RankPage Current;
        public ObservableCollection<UserInfoViewer> AllUsers = new ObservableCollection<UserInfoViewer>();
        public ObservableCollection<PokemenViewer> OtherPokemens = new ObservableCollection<PokemenViewer>(); 
        public RankPage()
        {
            this.InitializeComponent();
            Current = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            WaitForPokemens.Visibility = Visibility.Collapsed;
            PokemensGrid.Visibility = Visibility.Collapsed;
        }

        private void UsersGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            WaitForPokemens.Visibility = Visibility.Visible;
            PokemensGrid.Visibility = Visibility.Collapsed;

            App.Client.Core.SendMessage(new Message
            {
                type = MsgType.GET_POKEMENS_BY_USER,
                data = ((UserInfoViewer)e.ClickedItem).Name
            });
        }

        internal void ShowPokemensView()
        {
            WaitForPokemens.Visibility = Visibility.Collapsed;
            PokemensGrid.Visibility = Visibility.Visible;
        }
    }
}
