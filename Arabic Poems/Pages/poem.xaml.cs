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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Arabic_Poems.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class poem : Page
    {
        ObservableCollection<ListItemTemplate> list = new ObservableCollection<ListItemTemplate>();

        public poem()
        {
            this.InitializeComponent();
            poems_list.DataContext = list;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            list.Clear();
            long prt;
            int poet_inex;
            long.TryParse(e.Parameter.ToString().Substring(1), out prt);
            poet_inex = (int)(prt % 10000);
            prt /= 10000;
            char section = e.Parameter.ToString()[0];
            if (section == 'b')
            {
                for (int i = 0; i < Versifier.VersifiersList[poet_inex].peoms_count; i++)
                    if (prt == (int)Versifier.VersifiersList[poet_inex].Poems[i].poem_bahar)
                        list.Add(new ListItemTemplate(Versifier.VersifiersList[poet_inex].Poems[i].name, Versifier.VersifiersList[poet_inex].name, section + "" + (prt * 100000000 + poet_inex * 10000 + i)));
            }
            else
            {
                for (int i = 0; i < Versifier.VersifiersList[poet_inex].peoms_count; i++)
                    list.Add(new ListItemTemplate(Versifier.VersifiersList[poet_inex].Poems[i].name, Versifier.VersifiersList[poet_inex].name, section + "" + (prt * 100000000 + poet_inex * 10000 + i)));
            }

            subtitle2.Text = Versifier.VersifiersList[poet_inex].name;
            if (section == 'b')
                subtitle1.Content = Functions.GetBaharName((Bahar)prt);
            else if (section == 'a')
                subtitle1.Content = Functions.GetAgeName((Age)prt);
            else if (section == 'c')
                subtitle1.Content = Functions.GetCountryName((Country)prt);
            else if (section == 'n')
                subtitle1.Content = "كل الشعراء";
            ex_run.Text = Functions.Poem(list.Count);
            subtitle1.Tag = section + "" + prt;
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            ((Window.Current.Content as Frame).Content as MainPage).rootFram.Navigate(typeof(Home), ((HyperlinkButton)sender).Tag);
        }

        private void GoUp_Click(object sender, RoutedEventArgs e)
        {
            ((Window.Current.Content as Frame).Content as MainPage).rootFram.Navigate(typeof(poet), ((HyperlinkButton)sender).Tag);
        }

        private void ViewPoems_Click(object sender, RoutedEventArgs e)
        {
            ((Window.Current.Content as Frame).Content as MainPage).rootFram.Navigate(typeof(view), ((HyperlinkButton)sender).Tag);
        }
    }
}
