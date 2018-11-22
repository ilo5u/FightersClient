using Kernel;
using Platform.Converters;
using Platform.Models;
using System;
using System.Collections.Generic;
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
    public sealed partial class BattlePage : Page
    {
        /// <summary>
        /// 公共访问实例对象
        /// </summary>
        static public BattlePage Current;

        public BattlePage()
        {
            this.InitializeComponent();
            Current = this;
        }

        private PokemenViewer FirstPlayer;
        private PokemenViewer SecondPlayer;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // 启动对战信息实时接收线程
            App.Client.IsOnBattle = true;
            App.Client.BattleDriver = new Task(BattleTask);
            App.Client.BattleDriver.Start();

            FirstPlayer = (PokemenViewer)App.Client.Core.GetPropertyAt(LobbyPage.Current.userPlayerId);
            SecondPlayer = (PokemenViewer)LobbyPage.Current.AIPlayer.GetProperty();

            // 设置左侧小精灵的初始信息
            FirstPlayerName.Text = FirstPlayer.Name;
            FirstPlayerIcon.Glyph = PokemenTypeConverter.ExternConvert(FirstPlayer.Type);

            FirstHpoints.Text = FirstPlayer.Hpoints.ToString();
            FirstAttack.Text = FirstPlayer.Attack.ToString();
            FirstDefense.Text = FirstPlayer.Defense.ToString();
            FirstAgility.Text = FirstPlayer.Agility.ToString();

            FirstInterval.Text = FirstPlayer.Interval.ToString();
            FirstCritical.Text = FirstPlayer.Critical.ToString();
            FirstHitratio.Text = FirstPlayer.Hitratio.ToString();
            FirstParryratio.Text = FirstPlayer.Parryratio.ToString();

            FirstPlayerExp.Value = FirstPlayer.Exp;
            FirstPlayerLevel.Text = FirstPlayer.Level.ToString();

            SecondPlayerName.Text = SecondPlayer.Name;
            SecondPlayerIcon.Glyph = PokemenTypeConverter.ExternConvert(SecondPlayer.Type);

            SecondHpoints.Text = SecondPlayer.Hpoints.ToString();
            SecondAttack.Text = SecondPlayer.Attack.ToString();
            SecondDefense.Text = SecondPlayer.Defense.ToString();
            SecondAgility.Text = SecondPlayer.Agility.ToString();

            SecondInterval.Text = SecondPlayer.Interval.ToString();
            SecondCritical.Text = SecondPlayer.Critical.ToString();
            SecondHitratio.Text = SecondPlayer.Hitratio.ToString();
            SecondParryratio.Text = SecondPlayer.Parryratio.ToString();

            SecondPlayerExp.Value = SecondPlayer.Exp;
            SecondPlayerLevel.Text = SecondPlayer.Level.ToString();
        }

        async private void BattleTask()
        {
            Message message;
            while (App.Client.IsOnBattle)
            {
                message = App.Client.Core.ReadOfflineMessage();
                Debug.WriteLine(message.data);

                switch (message.type)
                {
                    case MsgType.PVE_MESSAGE:
                        {
                            if (message.data.StartsWith("RENEW="))
                            {
                                // 更新属性值
                                message.data = message.data.Remove(0, 6);
                                string[] players = message.data.Split('\n');

                                Debug.WriteLine(players[0]);
                                Debug.WriteLine(players[1]);

                                players[0] = players[0].Remove(0, 6);
                                players[1] = players[1].Remove(0, 7);

                                await Dispatcher.RunAsync(
                                    Windows.UI.Core.CoreDispatcherPriority.Normal, 
                                    () => OnRenewDisplayCallBack(players[0], players[1])
                                    );
                            }
                            else if (message.data.StartsWith("FIRST="))
                            {
                                // 上方小精灵攻击
                                message.data = message.data.Remove(0, 6);
                                string[] messages = message.data.Split('\n');
                                await Dispatcher.RunAsync(
                                    Windows.UI.Core.CoreDispatcherPriority.Normal,
                                    () => OnDisplayFirstPlayerCallBack(messages[0])
                                    );
                            }
                            else if (message.data.StartsWith("SECOND="))
                            {
                                // 下方小精灵攻击
                                message.data = message.data.Remove(0, 7);
                                string[] messages = message.data.Split('\n');
                                await Dispatcher.RunAsync(
                                    Windows.UI.Core.CoreDispatcherPriority.Normal, 
                                    () => OnDisplaySecondPlayerCallBack(messages[0])
                                    );
                            }
                        }
                        break;

                    case MsgType.PVE_RESULT:
                        {
                            if (message.data.StartsWith("WINNER"))
                            {

                            }
                            App.Client.IsOnBattle = false;
                        }
                        return;

                    default:
                        break;
                }
            }
        }

        private void OnRenewDisplayCallBack(string firstPlayer, string secondPlayer)
        {
            string[] firstProperties = firstPlayer.Split(',');

            FirstHpoints.Text = firstProperties[0];
            FirstAttack.Text = firstProperties[1];
            FirstDefense.Text = firstProperties[2];
            FirstAgility.Text = firstProperties[3];

            FirstInterval.Text = firstProperties[4];
            FirstCritical.Text = firstProperties[5];
            FirstHitratio.Text = firstProperties[6];
            FirstParryratio.Text = firstProperties[7];

            FirstPlayerAnger.Value = int.Parse(firstProperties[8]);

            string[] secondProperties = secondPlayer.Split(',');

            SecondHpoints.Text = secondProperties[0];
            SecondAttack.Text = secondProperties[1];
            SecondDefense.Text = secondProperties[2];
            SecondAgility.Text = secondProperties[3];

            SecondInterval.Text = secondProperties[4];
            SecondCritical.Text = secondProperties[5];
            SecondHitratio.Text = secondProperties[6];
            SecondParryratio.Text = secondProperties[7];

            SecondPlayerAnger.Value = int.Parse(secondProperties[8]);
        }

        private void OnDisplayFirstPlayerCallBack(string message)
        {
            BattleMessageOfFirstPlayer.Text += message + '\n';
        }

        private void OnDisplaySecondPlayerCallBack(string message)
        {
            BattleMessageOfSecondPlayer.Text += message + '\n';
        }
    }
}
