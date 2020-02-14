using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
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
    public sealed partial class view : Page
    {
        ObservableCollection<PoemVerse> list = new ObservableCollection<PoemVerse>();
        List<ListItemTemplate> more = new List<ListItemTemplate>();
        Poem current;
        List<int> find_result;
        int find_index = -1;
        public view()
        {
            this.InitializeComponent();
            verses_list.DataContext = list;
            more_list.DataContext = more;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Versifier.VersifiersList == null || e.Parameter == null)
                return;
            var parameter = e.Parameter.ToString().Split(new char[] { '|' }, 2);
            long prt, tmp;
            int poet_inex, poem_inex;
            long.TryParse(parameter[0].Substring(1), out tmp);
            prt = (int)(tmp / 100000000);
            tmp %= 100000000;
            poet_inex = (int)(tmp / 10000);
            poem_inex = (int)(tmp % 10000);
            char section = parameter[0][0];
            if (poet_inex >= Versifier.VersifiersList.Count || poem_inex >= Versifier.VersifiersList[poet_inex].Poems.Count)
                return;
            LastPoem.AddPoem(parameter[0]);


            current = Versifier.VersifiersList[poet_inex].Poems[poem_inex];
            title.Text = current.name;
            poet_name_tb.Text = current.versifier.name;
            poet_desc_tb.Text = GetPoetDescription(current.versifier);
            UpdateFavouriteButton();
            DataTransferManager.GetForCurrentView().DataRequested += View_DataRequested;



            FillReadMoreList(current, section, prt * 100000000 + poet_inex * 10000);
            ViewPoem(current);
            poem_desc_tb.Text = GetPoemDescription(current);
            if (parameter.Length > 1)
            {
                find_box.Text = parameter[1];
                FindInPoem(find_box);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= View_DataRequested;
        }

        private void FillReadMoreList(Poem current, char section, long tag)
        {
            more.Clear();
            List<int> others = new List<int>();
            List<string> names = new List<string>();
            for (int i = 0; i < current.versifier.Poems.Count; i++)
                if (current.versifier.Poems[i].name != current.name && !names.Contains(current.versifier.Poems[i].name))
                {
                    names.Add(current.versifier.Poems[i].name);
                    others.Add(i);
                }
            Random rand = new Random(DateTime.Now.Millisecond);
            while (more.Count < 10 && others.Count > 0)
            {
                int poem_index = others[rand.Next(others.Count)];
                others.Remove(poem_index);
                more.Add(new ListItemTemplate(current.versifier.Poems[poem_index].name, "", section + (tag + poem_index).ToString()));
            }
            more_list.ItemsSource = null;
            more_list.ItemsSource = more;
        }

        private string GetPoemDescription(Poem current)
        {
            return Functions.GetBaharName(current.poem_bahar) + "\n" + current.versifier.age + "\n" + (current.is_vertical ? "عدد الأسطر: " : "عدد الأبيات: ") + list.Count;
        }

        private string GetPoetDescription(Versifier versifier)
        {
            return string.IsNullOrWhiteSpace(versifier.info) ? "لا تتوفر معلومات عن هذا الشاعر في ملف القصائد." : versifier.info;
        }

        private async void ViewPoem(Poem selected_poem)
        {
            list.Clear();
            find_result = null;
            string error_message;
            if (current.ReadPoem(((Window.Current.Content as Frame).Content as MainPage).br, out error_message))
            {
                if (current.is_vertical)
                    verses_list.ItemTemplate = (DataTemplate)Resources["OneVirse_Template"];
                else
                    verses_list.ItemTemplate = (DataTemplate)Resources["TowVirses_Template"];
                string[] splitter = { "\r\n" };
                var lines = current.poem.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                int i;
                for (i = 1; i < lines.Length; i++)
                {
                    list.Add(new PoemVerse(current.is_vertical ? i : (i + 1) / 2, lines[i], current.is_vertical ? "" : i + 1 < lines.Length ? lines[i + 1] : ""));
                    if (!current.is_vertical)
                        i++;
                }
            }
            else
                await new Windows.UI.Popups.MessageDialog(error_message, "عذرا حدث خطا ما").ShowAsync();
        }

        private void ViewPoem_Click(object sender, RoutedEventArgs e)
        {
            ((Window.Current.Content as Frame).Content as MainPage).rootFram.Navigate(typeof(view), ((HyperlinkButton)sender).Tag);
        }

        private void FindBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            find_result = null;
            find_count.Text = "";
        }

        private void FindBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            FindInPoem(sender);
        }

        private async void FindInPoem(AutoSuggestBox sender)
        {
            if (current == null)
                return;
            if (find_result == null)
            {
                string word = Functions.RemoveAccents(sender.Text.Trim());
                if (word.Length < 2)
                {
                    await new Windows.UI.Popups.MessageDialog("كلمة البحث قصيرة", "الشعر العربي").ShowAsync();
                    return;
                }
                find_result = new List<int>();
                find_index = -1;
                for (int i = 0; i < list.Count; i++)
                    if (Functions.RemoveAccents(list[i].Line1 + " " + list[i].Line2).Contains(word))
                        find_result.Add(i);
                if (find_result.Count == 0)
                {
                    verses_list.SelectedIndex = -1;
                    find_count.Text = Functions.Result(0);
                    return;
                }
            }
            if (find_result.Count > 0)
            {
                find_index = (find_index + 1) % find_result.Count;
                verses_list.SelectedIndex = find_result[find_index];
                verses_list.ScrollIntoView(verses_list.SelectedItem);
                find_count.Text = (find_index + 1) + " / " + find_result.Count;
            }
        }

        private void SelectPoem_Click(object sender, RoutedEventArgs e)
        {
            Functions.SelectPoem();
        }

        private void RandomPoem_Click(object sender, RoutedEventArgs e)
        {
            if (Versifier.VersifiersList != null)
                Functions.Navigate(typeof(view), Functions.RandomPoemTag());
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            if (current == null || current.poem == "")
                return;
            DataPackage data = new DataPackage();
            data.SetText(Functions.PoemText(current));
            Clipboard.SetContent(data);
        }

        private async void Favourite_Click(object sender, RoutedEventArgs e)
        {
            if (current == null)
                return;
            await Favourites.Toggle(current);
            UpdateFavouriteButton();
        }

        private void UpdateFavouriteButton()
        {
            bool saved = Favourites.Contains(current);
            fav_button.Label = saved ? "ازالة من المفضلة" : "اضافة للمفضلة";
            fav_icon.Glyph = saved ? "\uEB52" : "\uEB51";
        }

        private void Share_Click(object sender, RoutedEventArgs e)
        {
            if (current != null && current.poem != "")
                DataTransferManager.ShowShareUI();
        }

        private void View_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (current == null || current.poem == "")
                return;
            args.Request.Data.Properties.Title = current.name.Trim() + " - " + current.versifier.name.Trim();
            args.Request.Data.SetText(Functions.PoemText(current));
        }
    }
}

