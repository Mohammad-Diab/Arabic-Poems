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
    public sealed partial class poet : Page
    {
        ObservableCollection<ListItemTemplate> list = new ObservableCollection<ListItemTemplate>();
        List<ListItemTemplate> all = new List<ListItemTemplate>();

        public poet()
        {
            this.InitializeComponent();
            poets_list.DataContext = list;
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            list.Clear();
            if (Versifier.VersifiersList == null || e.Parameter == null)
                return;
            long prt;
            long.TryParse(e.Parameter.ToString().Substring(1), out prt);
            char section = e.Parameter.ToString()[0];
            for (int i = 0; i < Versifier.VersifiersList.Count; i++)
            {
                if (section == 'n')
                    list.Add(new ListItemTemplate(Versifier.VersifiersList[i].name, Functions.Poem(Versifier.VersifiersList[i].peoms_count), section + "" + (10000 * prt + i)));
                else if (section == 'a')
                {
                    if (prt == (int)Versifier.VersifiersList[i].poet_age)
                        list.Add(new ListItemTemplate(Versifier.VersifiersList[i].name, Functions.Poem(Versifier.VersifiersList[i].peoms_count), section + "" + (10000 * prt + i)));
                }
                else if (section == 'c')
                {
                    if (prt == (int)Versifier.VersifiersList[i].poet_country)
                        list.Add(new ListItemTemplate(Versifier.VersifiersList[i].name, Functions.Poem(Versifier.VersifiersList[i].peoms_count), section + "" + (10000 * prt + i)));
                }
                else if (section == 'b')
                {
                    int count = Versifier.VersifiersList[i].Poems.Count(poem => prt == (int)poem.poem_bahar);
                    if (count > 0)
                        list.Add(new ListItemTemplate(Versifier.VersifiersList[i].name, Functions.Poem(count), section + "" + (10000 * prt + i)));
                }
            }
            if (section == 'a')
                subtitle.Text = Functions.GetAgeName((Age)prt);
            else if (section == 'c')
                subtitle.Text = Functions.GetCountryName((Country)prt);
            else if (section == 'b')
                subtitle.Text = Functions.GetBaharName((Bahar)prt);
            else if (section == 'n')
                subtitle.Text = "كل الشعراء";
            title.Text = (section == 'n' || section == 'b' ? "" : "شعراء ") + subtitle.Text + " (" + Functions.Poet(list.Count) + ")";
            all = list.ToList();
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
            ((Window.Current.Content as Frame).Content as MainPage).rootFram.Navigate(typeof(Home), (sender as HyperlinkButton).Tag);
        }

        private void ViewPoems_Click(object sender, RoutedEventArgs e)
        {
            ((Window.Current.Content as Frame).Content as MainPage).rootFram.Navigate(typeof(poem), (sender as HyperlinkButton).Tag);
        }
    }
}
