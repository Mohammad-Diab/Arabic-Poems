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
    public sealed partial class Home : Page
    {
        ObservableCollection<ListItemTemplate> list = new ObservableCollection<ListItemTemplate>();

        public Home()
        {
            this.InitializeComponent();
            last_poem_list.DataContext = list;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await LastPoem.LoadSetting();
            list.Clear();
            int counter = 0;
            foreach(var i in LastPoem.poems_list)
            {
                if (counter < 5)
                {
                    counter++;
                    int poet_index, poem_index;
                    poet_index = (int)i.PoemTag / 10000;
                    poem_index = (int)i.PoemTag % 10000;
                    list.Add(new ListItemTemplate(Versifier.VersifiersList[poet_index].Poems[poem_index].name, Versifier.VersifiersList[poet_index].name, i.Tag));
                }
            }
        }

        //  navigate legends
        //  'a'     is for  age
        //  'c'     is for  country
        //  'b'     is for  bahar
        //  'n'     is for  none (all poets)


        private void ViewAgePoets_Click(object sender, RoutedEventArgs e)
        {
            long num;
            long.TryParse((sender as Control).Tag.ToString(), out num);
            ((Window.Current.Content as Frame).Content as MainPage).rootFram.Navigate(typeof(poet), "a" + num);
        }

        private void ViewCountryPoets_Click(object sender, RoutedEventArgs e)
        {
            long num;
            long.TryParse((sender as Control).Tag.ToString(), out num);
            ((Window.Current.Content as Frame).Content as MainPage).rootFram.Navigate(typeof(poet), "c" + num);
        }

        private void ViewPoetBahar_Click(object sender, RoutedEventArgs e)
        {
            long num;
            long.TryParse((sender as Control).Tag.ToString(), out num);
            ((Window.Current.Content as Frame).Content as MainPage).rootFram.Navigate(typeof(poet), "b" + num);
        }

        private void ViewAllPoets_Click(object sender, RoutedEventArgs e)
        {
            ((Window.Current.Content as Frame).Content as MainPage).rootFram.Navigate(typeof(poet), "n100");
        }

        private void ViewPoems_Click(object sender, RoutedEventArgs e)
        {
            int num;
            int.TryParse((sender as Control).Tag.ToString(), out num);
            Bahar parameter = (Bahar)num;
            ((Window.Current.Content as Frame).Content as MainPage).rootFram.Navigate(typeof(poem), "b" + num);
        }

        private void ViewPoem_Click(object sender, RoutedEventArgs e)
        {
            ((Window.Current.Content as Frame).Content as MainPage).rootFram.Navigate(typeof(view), ((Control)sender).Tag);
        }
    }
}
