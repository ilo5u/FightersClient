using Kernel;
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
    public sealed partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            this.InitializeComponent();
        }

        int pokemenId;
        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            PokemenViewer pokemen = (PokemenViewer)e.ClickedItem;
            Display.IsPaneOpen = true;

            pokemenId = pokemen.Id;
            if (pokemen.Career > 0 || pokemen.Level < 9)
                Promote.IsEnabled = false;
            else
                Promote.IsEnabled = true;
        }

        private void FirstCareer_Click(object sender, RoutedEventArgs e)
        {
            Message msg = new Message { type = MsgType.UPGRADE_POKEMEN, data = pokemenId.ToString() + "\n1\n" };
            App.Client.Core.SendMessage(msg);
        }

        private void SecondCareer_Click(object sender, RoutedEventArgs e)
        {
            Message msg = new Message { type = MsgType.UPGRADE_POKEMEN, data = pokemenId.ToString() + "\n2\n" };
            App.Client.Core.SendMessage(msg);
        }
    }
}
