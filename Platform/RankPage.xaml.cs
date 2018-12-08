using Kernel;
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
        public static RankPage Current;

        public RankPage()
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
            WaitForPokemens.Visibility = Visibility.Collapsed;
            PokemensGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsersGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            WaitForPokemens.Visibility = Visibility.Visible;
            PokemensGrid.Visibility = Visibility.Collapsed;

            App.Client.RankedPokemens.Clear();

            string username = ((UserInfoViewer)e.ClickedItem).Name;
            new Task(() => WaitForPokemensReady(username)).Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        async private void WaitForPokemensReady(string username)
        {
            App.Client.IsRankedPokemensReady = new System.Threading.Semaphore(0, 1);
            App.Client.Core.SendMessage(new Message
            {
                type = MsgType.GET_POKEMENS_BY_USER,
                data = username
            });

            App.Client.IsRankedPokemensReady.WaitOne();
            await Dispatcher.RunAsync(
                Windows.UI.Core.CoreDispatcherPriority.Normal,
                () => {
                    WaitForPokemens.Visibility = Visibility.Collapsed;
                    PokemensGrid.Visibility = Visibility.Visible;
                }
                );
        }
    }
}
