using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Platform
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LogonPage : Page
    {
        /// <summary>
        /// 公共访问实例对象
        /// </summary>
        static public LogonPage Current;

        public LogonPage()
        {
            this.InitializeComponent();
            Current = this;
        }

        private bool IsShiftPressed = false;
        private void Username_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Shift)
            {
                IsShiftPressed = false;
            }
        }

        private void Username_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key >= Windows.System.VirtualKey.Number0
                && e.Key <= Windows.System.VirtualKey.Number9
                && Username.Text.Length < 10
                && IsShiftPressed == false)
            {

            }
            else if (e.Key >= Windows.System.VirtualKey.A
                && e.Key <= Windows.System.VirtualKey.Z
                && Username.Text.Length < 10)
            {

            }
            else if (e.Key == Windows.System.VirtualKey.Shift)
            {
                IsShiftPressed = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Password_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key >= Windows.System.VirtualKey.Number0
                && e.Key <= Windows.System.VirtualKey.Number9
                && Password.Password.Length < 20
                && IsShiftPressed == false)
            {

            }
            else if (e.Key >= Windows.System.VirtualKey.A 
                && e.Key <= Windows.System.VirtualKey.Z
                && Password.Password.Length < 20)
            {

            }
            else if (e.Key == Windows.System.VirtualKey.Shift)
            {
                IsShiftPressed = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void Password_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Shift)
            {
                IsShiftPressed = false;
            }
        }

        bool IsOnLogon = false;
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void Logon_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Username.Text))
            {
                MessageDialog msg = new MessageDialog("用户名不能为空。") { Title = "警告" };
                msg.Commands.Add(new UICommand("确定"));
                await msg.ShowAsync();
            }
            else if (string.IsNullOrEmpty(Password.Password))
            {
                MessageDialog msg = new MessageDialog("密码不能为空。") { Title = "警告" };
                msg.Commands.Add(new UICommand("确定"));
                await msg.ShowAsync();
            }
            else
            {
                if (!IsOnLogon)
                {
                    IsOnLogon = true;
                    string username = new string(Username.Text.ToCharArray());
                    string password = new string(Password.Password.ToCharArray());
                    new Task(
                        () => LogonTask(
                            username,
                            password
                            )
                        ).Start();
                    LogonProgress.IsActive = true;
                }
            }
        }

        /// <summary>
        /// 向服务器发起注册请求
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        async private void LogonTask(string username, string password)
        {
            try
            {
                /* 检测网络连接是否成功 */
                if (!App.Client.Core.IsConnected())
                {
                    if (!App.Client.Core.StartConnection())
                    {
                        await Dispatcher.RunAsync(
                            Windows.UI.Core.CoreDispatcherPriority.Normal,
                            () => OnLogonFailedCallBack("连接服务器失败。\n请检查网络是否正常。")
                            );
                    }
                }

                /* 投递注册请求 */
                if (App.Client.Core.IsConnected())
                {
                    Kernel.Message message;

                    message.type = Kernel.MsgType.LOGON_REQUEST;
                    message.data = username + '\n' + password;
                    App.Client.Core.SendMessage(message);

                    message = App.Client.Core.ReadOnlineMessage();
                    if (message.type == Kernel.MsgType.LOGON_SUCCESS)
                    {
                        await Dispatcher.RunAsync(
                            Windows.UI.Core.CoreDispatcherPriority.Normal,
                            OnLogonSuccessCallBack
                            );
                    }
                    else
                    {
                        await Dispatcher.RunAsync(
                            Windows.UI.Core.CoreDispatcherPriority.Normal,
                            () => OnLogonFailedCallBack("该用户名已存在。")
                            );
                    }

                }
            }
            catch (Exception)
            {
            }
            finally
            {
                IsOnLogon = false;
            }
        }

        /// <summary>
        /// 注册失败
        /// </summary>
        /// <param name="msg"></param>
        async private void OnLogonFailedCallBack(string msg)
        {
            LogonProgress.IsActive = false;
            MessageDialog error =
                new MessageDialog(msg)
                { Title = "错误" };
            error.Commands.Add(new UICommand("确定"));
            await error.ShowAsync();
        }

        /// <summary>
        /// 注册成功，跳转至登陆界面
        /// </summary>
        async private void OnLogonSuccessCallBack()
        {
            LogonProgress.IsActive = false;

            MessageDialog msg =
                new MessageDialog("欢迎加入该对战平台。\n请牢记自己的帐号昵称以及密码。")
                { Title = "提示" };
            msg.Commands.Add(new UICommand("确定"));
            await msg.ShowAsync();

            Frame.Navigate(typeof(LoginPage));
        }
    }
}
