using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Arabic_Poems.Pages
{
    public sealed partial class favourites : Page
    {
        ObservableCollection<ListItemTemplate> list = new ObservableCollection<ListItemTemplate>();

        public favourites()
        {
            this.InitializeComponent();
            favourites_list.DataContext = list;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Fill();
        }

        private void Fill()
        {
            list.Clear();
            if (Versifier.VersifiersList == null)
                return;
            foreach (var i in Favourites.Poems)
                list.Add(new ListItemTemplate(i.name.Trim(), i.versifier.name.Trim(), Favourites.GetTag(i)));
            title.Text = "المفضلة (" + Functions.Poem(list.Count) + ")";
            empty_text.Visibility = list.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Functions.Navigate(typeof(Home), "home");
        }

        private void ViewPoem_Click(object sender, RoutedEventArgs e)
        {
            Functions.Navigate(typeof(view), ((FrameworkElement)sender).Tag);
        }

        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            string tag = ((FrameworkElement)sender).Tag.ToString();
            var poem = Favourites.Poems.FirstOrDefault(i => Favourites.GetTag(i) == tag);
            if (poem != null)
                await Favourites.Toggle(poem);
            Fill();
        }
    }
}
