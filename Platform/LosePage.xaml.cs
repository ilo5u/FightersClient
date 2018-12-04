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
    public sealed partial class LosePage : Page
    {
        public ObservableCollection<PokemenViewer> SelectsOfPokemens = new ObservableCollection<PokemenViewer>();
        public LosePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PokemenId = -1;
            SelectsOfPokemens.Clear();

            if (App.Client.Pokemens.Count < 4)
            {
                SelectsOfPokemens = App.Client.Pokemens;
            }
            else
            {
                List<int> selects = new List<int>();
                Random generator = new Random();
                while (selects.Count < 3)
                {
                    int id = generator.Next() % App.Client.Pokemens.Count;
                    if (selects.Contains(id))
                        continue;
                    else
                        selects.Add(id);
                }

                foreach (var id in selects)
                {
                    SelectsOfPokemens.Add(
                        App.Client.Pokemens.ElementAt(id)
                        );
                }
            }
        }

        int PokemenId;
        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            PokemenId = ((PokemenViewer)e.ClickedItem).Id;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (PokemenId == -1)
            {
                if (sender is FrameworkElement instruc)
                {
                    FlyoutBase.ShowAttachedFlyout(instruc);
                }
            }
            else
            {
                App.Client.Core.SendMessage(
                    new Kernel.Message {
                        type = Kernel.MsgType.SUB_POKEMEN,
                        data = PokemenId.ToString()
                    }
                    );
                App.Client.Pokemens.Remove(
                    App.Client.Pokemens.First(pokemen => pokemen.Id == PokemenId)
                    );
                LobbyPage.Current.BackToLobby();
            }
        }
    }
}
