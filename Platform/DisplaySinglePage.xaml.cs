using Kernel;
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
    public sealed partial class DisplaySinglePage : Page
    {
        public PokemenViewer DisplayOfPokemen = new PokemenViewer();
        public DisplaySinglePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayOfPokemen = (PokemenViewer)e.Parameter;

            NormalSkillNotation.Text = NotationOfSkillConverter.Convert(DisplayOfPokemen.Type);
            MainSkillNotation.Text = MainSkillConverter.Convert(DisplayOfPokemen.Type);

            CareerInfo.Text = CareerConverter.Convert(DisplayOfPokemen.Type, DisplayOfPokemen.Career);
            NotationOfCareer.Text = NotationOfCareerConverter.Convert(DisplayOfPokemen.Type, DisplayOfPokemen.Career);

            /* 设置转职选项 */
            if (DisplayOfPokemen.Career == 0 && DisplayOfPokemen.Level > 8)
                Promote.IsEnabled = true;
            else
                Promote.IsEnabled = false;
        }

        private void BackToAll_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DisplayAllPage));
        }

        private void FirstCareer_Click(object sender, RoutedEventArgs e)
        {
            App.Client.Core.SendMessage(
                new Message {
                    type = MsgType.PROMOTE_POKEMEN,
                    data = DisplayOfPokemen.Id.ToString() + "\n1\n"
                }
                );
            Promote.IsEnabled = false;
        }

        private void SecondCareer_Click(object sender, RoutedEventArgs e)
        {
            App.Client.Core.SendMessage(
                new Message
                {
                    type = MsgType.PROMOTE_POKEMEN,
                    data = DisplayOfPokemen.Id.ToString() + "\n2\n"
                }
                );
            Promote.IsEnabled = false;
        }
    }
}
