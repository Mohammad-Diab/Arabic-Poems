using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Poetry_2
{
    public sealed partial class FavouriteList : ContentDialog
    {
        ObservableCollection<NameWithIndex> list_data = new ObservableCollection<NameWithIndex>();

        internal Poem current_poem;
        internal Poem selected_poem;

        public FavouriteList()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            listView.DataContext = list_data;
            Fill();
        }

        private void Fill()
        {
            list_data.Clear();
            for (int i = 0; i < Favourites.Poems.Count; i++)
                list_data.Add(new NameWithIndex(Favourites.Poems[i].name.Trim() + " - " + Favourites.Poems[i].versifier.name.Trim(), i));
            empty_text.Visibility = list_data.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            button_Copy2.IsEnabled = current_poem != null;
            button_Copy2.Content = current_poem != null && Favourites.Poems.Contains(current_poem) ? "ازالة القصيدة" : "اضافة القصيدة";
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex > -1)
            {
                selected_poem = Favourites.Poems[list_data[listView.SelectedIndex].Index];
                Hide();
            }
        }

        private void ListView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ViewButton_Click(sender, null);
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (current_poem == null)
                return;
            if (Favourites.Poems.Contains(current_poem))
                Favourites.Poems.Remove(current_poem);
            else
                Favourites.Poems.Add(current_poem);
            Fill();
            await Favourites.Save();
        }

        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedIndex > -1)
            {
                Favourites.Poems.RemoveAt(list_data[listView.SelectedIndex].Index);
                Fill();
                await Favourites.Save();
            }
        }

        private async void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Favourites.Poems.Clear();
            Fill();
            await Favourites.Save();
        }
    }

    internal static class Favourites
    {
        internal static List<Poem> Poems = new List<Poem>();

        internal static async Task Load()
        {
            Poems = new List<Poem>();
            try
            {
                var file = await ApplicationData.Current.LocalFolder.TryGetItemAsync("favourites") as StorageFile;
                if (file == null)
                    return;
                foreach (var line in await FileIO.ReadLinesAsync(file))
                {
                    var parts = line.Split('\t');
                    if (parts.Length < 3)
                        continue;
                    var poem = Find(parts[0], parts[1], parts[2]);
                    if (poem != null && !Poems.Contains(poem))
                        Poems.Add(poem);
                }
            }
            catch (Exception) { }
        }

        internal static async Task Save()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("favourites", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteLinesAsync(file, Poems.Select(p => p.versifier.name + "\t" + p.name + "\t" + p.versifier.Poems.IndexOf(p)).ToList());
            }
            catch (Exception) { }
        }

        private static Poem Find(string versifier_name, string poem_name, string index)
        {
            if (Versifier.VersifiersList == null)
                return null;
            int poem_index;
            int.TryParse(index, out poem_index);
            foreach (var v in Versifier.VersifiersList)
                if (v.name == versifier_name)
                {
                    if (poem_index > -1 && poem_index < v.Poems.Count && v.Poems[poem_index].name == poem_name)
                        return v.Poems[poem_index];
                    foreach (var p in v.Poems)
                        if (p.name == poem_name)
                            return p;
                }
            return null;
        }
    }
}
