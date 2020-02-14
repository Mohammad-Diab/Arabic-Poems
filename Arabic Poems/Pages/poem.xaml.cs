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
        List<ListItemTemplate> all = new List<ListItemTemplate>();

        public poem()
        {
            this.InitializeComponent();
            poems_list.DataContext = list;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            list.Clear();
            if (Versifier.VersifiersList == null || e.Parameter == null)
                return;
            long prt;
            int poet_inex;
            long.TryParse(e.Parameter.ToString().Substring(1), out prt);
            poet_inex = (int)(prt % 10000);
            prt /= 10000;
            if (poet_inex >= Versifier.VersifiersList.Count)
                return;
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
            all = list.ToList();
            poet_glance_name.Text = Versifier.VersifiersList[poet_inex].name.Trim();
            poet_glance_age.Text = Versifier.VersifiersList[poet_inex].age;
            poet_glance_info.Text = string.IsNullOrWhiteSpace(Versifier.VersifiersList[poet_inex].info) ? "لا تتوفر معلومات عن هذا الشاعر في ملف القصائد." : Versifier.VersifiersList[poet_inex].info;
        }

        private void Filter_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            string word = Functions.RemoveAccents(sender.Text.Trim());
            list.Clear();
            foreach (var i in all)
                if (word.Length == 0 || Functions.RemoveAccents(i.Name).Contains(word))
                    list.Add(i);
        }

        private void RandomPoem_Click(object sender, RoutedEventArgs e)
        {
            if (Versifier.VersifiersList != null)
                Functions.Navigate(typeof(view), Functions.RandomPoemTag());
        }

        private void SelectPoem_Click(object sender, RoutedEventArgs e)
        {
            Functions.SelectPoem();
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
