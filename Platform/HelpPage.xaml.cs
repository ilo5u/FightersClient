using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public sealed partial class HelpPage : Page
    {
        public class IPWatcher : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private string ip;
            public string IP
            {
                get
                {
                    return ip;
                }
                set
                {
                    ip = value;
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("IP"));
                    }
                }
            }
        }

        public IPWatcher IPOfServer = new IPWatcher();
        public HelpPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IPOfServer.IP = App.Client.Core.GetIP();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string newIP = IPGetter.Text;
            string[] subSeg = newIP.Split('.');
            if (subSeg.Length == 4
                && ((!string.IsNullOrEmpty(subSeg[0]) && 0 <= int.Parse(subSeg[0]) && int.Parse(subSeg[0]) <= 255)
                    && (!string.IsNullOrEmpty(subSeg[1]) && 0 <= int.Parse(subSeg[1]) && int.Parse(subSeg[1]) <= 255)
                    && (!string.IsNullOrEmpty(subSeg[2]) && 0 <= int.Parse(subSeg[2]) && int.Parse(subSeg[2]) <= 255)
                    && (!string.IsNullOrEmpty(subSeg[3]) && 0 <= int.Parse(subSeg[3]) && int.Parse(subSeg[3]) <= 255)))
            {
                    IPOfServer.IP = newIP;
                    App.Client.Core.SetIP(IPOfServer.IP);
            }
            else
            {
                if (sender is FrameworkElement error)
                {
                    FlyoutBase.ShowAttachedFlyout(error);
                }
            }
        }
    }
}
