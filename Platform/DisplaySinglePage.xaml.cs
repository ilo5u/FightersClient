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
        PokemenViewer DisplayOfPokemen;
        public DisplaySinglePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayOfPokemen = (PokemenViewer)e.Parameter;
            NameOfPokemen.Text = DisplayOfPokemen.Name;
            TypeOfPokemen.Glyph = PokemenTypeConverter.ExternConvert(DisplayOfPokemen.Type);

            Hpoints.Text = DisplayOfPokemen.Hpoints.ToString();
            Attack.Text = DisplayOfPokemen.Attack.ToString();
            Defense.Text = DisplayOfPokemen.Defense.ToString();
            Agility.Text = DisplayOfPokemen.Agility.ToString();

            Interval.Text = DisplayOfPokemen.Interval.ToString();
            Critical.Text = DisplayOfPokemen.Critical.ToString();
            Hitratio.Text = DisplayOfPokemen.Hitratio.ToString();
            Parryratio.Text = DisplayOfPokemen.Parryratio.ToString();

            ExpValue.Value = ExpConverter.Convert(DisplayOfPokemen.Exp);
            ExpInfo.Text = ExpConverter.Convert(DisplayOfPokemen.Exp).ToString() + "/100";
            LevelValue.Text = DisplayOfPokemen.Level.ToString();

            NormalSkillNotation.Text = NotationOfSkillConverter.Convert(DisplayOfPokemen.Type);
            MainSkillNotation.Text = MainSkillConverter.Convert(DisplayOfPokemen.Type);

            CareerInfo.Text = CareerConverter.Convert(DisplayOfPokemen.Type, DisplayOfPokemen.Career);
            NotationOfCareer.Text = NotationOfCareerConverter.Convert(DisplayOfPokemen.Type, DisplayOfPokemen.Career);
        }

        private void BackToAll_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DisplayAllPage));
        }
    }
}
