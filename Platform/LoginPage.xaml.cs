using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class LoginPage : Page
    {
        /// <summary>
        /// 网络IO操作
        /// </summary>
        private bool IsOnLogin = false;

        public LoginPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Username.Text))
            {
                MessageDialog msg 
                    = new MessageDialog("用户名不能为空。") { Title = "警告" };
                msg.Commands.Add(new UICommand("确定"));
                await msg.ShowAsync();
            }
            else if (string.IsNullOrEmpty(Password.Password))
            {
                MessageDialog msg 
                    = new MessageDialog("密码不能为空。") { Title = "警告" };
                msg.Commands.Add(new UICommand("确定"));
                await msg.ShowAsync();
            }
            else if (!IsOnLogin)
            {
                IsOnLogin = true; // 避免重复请求
                string username = new string(Username.Text.ToCharArray());
                string password = new string(Password.Password.ToCharArray());
                new Task(
                    () => LoginTask(
                        username,
                        password
                        )
                    ).Start();
                LoginProgress.IsActive = true;
            }
        }

        /// <summary>
        /// 工作线程，向服务端发出登陆请求
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        async private void LoginTask(string username, string password)
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
                            () => OnLoginFailedCallBack("连接服务器失败。\n请检查网络是否正常。")
                            );
                    }
                }

                /* 投递登陆请求 */
                if (App.Client.Core.IsConnected())
                {
                    Kernel.Message message;
                    int reqCnt = 0;
                    while (reqCnt < 3)
                    { /* 请求三次 */
                        message.type = Kernel.MsgType.LOGIN_REQUEST;
                        message.data = username + '\n' + password;
                        App.Client.Core.SendMessage(message);

                        message = App.Client.Core.ReadOnlineMessage();
                        if (message.type == Kernel.MsgType.LOGIN_SUCCESS)
                        {
                            await Dispatcher.RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Normal,
                                () => OnLoginSuccessCallBack(int.Parse(message.data))
                                );
                            break;
                        }
                        else if (message.type == Kernel.MsgType.LOGIN_FAILED || reqCnt == 2)
                        {
                            await Dispatcher.RunAsync(
                                Windows.UI.Core.CoreDispatcherPriority.Normal,
                                () => OnLoginFailedCallBack("用户名或密码错误。")
                                );
                            break;
                        }
                        ++reqCnt;
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                IsOnLogin = false;
            }
        }

        /// <summary>
        /// 登陆失败
        /// </summary>
        /// <param name="msg"></param>
        async private void OnLoginFailedCallBack(string msg)
        {
            LoginProgress.IsActive = false;
            MessageDialog error =
                new MessageDialog(msg)
                { Title = "错误" };
            error.Commands.Add(new UICommand("确定"));
            await error.ShowAsync();
        }

        /// <summary>
        /// 登陆成功
        /// </summary>
        /// <param name="numberOfPokemens"></param>
        private void OnLoginSuccessCallBack(int numberOfPokemens)
        {
            LoginProgress.IsActive = false;
            /* 处理本地密码记录 */
            int index = App.Client.LocalRecords.FindIndex(
                s => s.Name.Equals(Username.Text)
                );
            if (SavePassword.IsChecked == true)
            { /* 保存密码 */
                if (index == -1)
                {
                    App.Client.LocalRecords.Add(
                        new App.Record { Name = Username.Text, Password = Password.Password }
                        );
                    new Task(WriteBackPasswords).Start();
                }
            }
            else
            { /* 不保存密码 */
                if (index != -1)
                { 
                    App.Client.LocalRecords.RemoveAt(index);
                    new Task(WriteBackPasswords).Start();
                }
            }
            Frame.Navigate(typeof(GamePage), numberOfPokemens);
        }

        /// <summary>
        /// 写入密码到本地记录
        /// </summary>
        async private void WriteBackPasswords()
        {
            StorageFile file 
                = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                    App.FilenameOfLocalRecords,
                    CreationCollisionOption.ReplaceExisting
                    );
            foreach (var perRec in App.Client.LocalRecords)
            {
                await FileIO.AppendTextAsync(file, perRec.Name + "," + perRec.Password + " ");
            }
        }

        private bool IsShiftPressed = false;
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

        private void Username_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Shift)
            {
                IsShiftPressed = false;
            }
        }

        private void Username_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Username.Text)
                && App.Client.LocalRecords.Count != 0)
            {
                int index = App.Client.LocalRecords.FindIndex(s => s.Name.Equals(Username.Text));
                if (index == -1)
                {
                    Password.Password = "";
                    SavePassword.IsChecked = false;
                }
                else
                {
                    Password.Password = App.Client.LocalRecords[index].Password;
                    SavePassword.IsChecked = true;
                }
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
    }
}
