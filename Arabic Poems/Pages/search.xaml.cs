using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Arabic_Poems.Pages
{
    public sealed partial class search : Page
    {
        ObservableCollection<ListItemTemplate> results = new ObservableCollection<ListItemTemplate>();
        CancellationTokenSource search_cancel;
        List<Bahar> bahar_list = new List<Bahar>();
        const int max_results = 2000;

        public search()
        {
            this.InitializeComponent();
            results_list.DataContext = results;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Versifier.VersifiersList == null)
                return;
            poets_combo.Items.Add("كل الشعراء");
            foreach (var i in Versifier.VersifiersList)
                poets_combo.Items.Add(i.name.Trim());
            poets_combo.SelectedIndex = 0;
            bahar_list.Add(Bahar.All);
            bahar_combo.Items.Add("كل البحور");
            foreach (var b in Functions.BaharOrder)
                if (Versifier.VersifiersList.Exists(v => v.Poems.Exists(p => p.poem_bahar == b)))
                {
                    bahar_list.Add(b);
                    bahar_combo.Items.Add(Functions.GetBaharName(b));
                }
            bahar_combo.SelectedIndex = 0;
            bahar_combo.Visibility = bahar_list.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
            if (e.Parameter is string && ((string)e.Parameter).Length > 1)
            {
                search_textbox.Text = (string)e.Parameter;
                SearchButton_Click(search_button, null);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (search_cancel != null)
                search_cancel.Cancel();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Functions.Navigate(typeof(Home), "home");
        }

        private void SearchTextbox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                SearchButton_Click(sender, null);
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (search_cancel != null)
            {
                search_cancel.Cancel();
                return;
            }
            if (Versifier.VersifiersList == null)
                return;
            bool Ignore_def = ignore_difference.IsChecked.Value;
            string search_word = Ignore_def ? Functions.RemoveAccents(search_textbox.Text.Trim()) : search_textbox.Text.Trim();
            if (search_word.Length < 2)
            {
                await new Windows.UI.Popups.MessageDialog("كلمة البحث قصيرة", "الشعر العربي").ShowAsync();
                return;
            }
            List<int> selected_versifiers = new List<int>();
            if (poets_combo.SelectedIndex > 0)
                selected_versifiers.Add(poets_combo.SelectedIndex - 1);
            else
                for (int i = 0; i < Versifier.VersifiersList.Count; i++)
                    selected_versifiers.Add(i);

            long start_time = DateTime.Now.Ticks;
            results.Clear();
            Progress_bar.Value = 0;
            Progress_bar.Maximum = selected_versifiers.Sum(i => Versifier.VersifiersList[i].Poems.Count);
            Progress_bar.Visibility = Visibility.Visible;
            poets_combo.IsEnabled = bahar_combo.IsEnabled = ignore_difference.IsEnabled = false;
            search_icon.Symbol = Symbol.Cancel;
            resultCount_textblock.Text = "جاري البحث ...";
            searchTime_textblock.Text = "";
            search_cancel = new CancellationTokenSource();
            var token = search_cancel.Token;
            var reader = ((Window.Current.Content as Frame).Content as MainPage).br;
            var versifiers = Versifier.VersifiersList;
            string query = search_textbox.Text.Trim();
            Bahar bahar = bahar_combo.SelectedIndex > 0 ? bahar_list[bahar_combo.SelectedIndex] : Bahar.All;
            bool more = false;
            List<ListItemTemplate> found = new List<ListItemTemplate>();
            IProgress<int> progress = new Progress<int>((count) => { Progress_bar.Value += count; });

            await Task.Run(() =>
            {
                string[] splitter = { "\r\n" };
                foreach (int i in selected_versifiers)
                {
                    for (int j = 0; j < versifiers[i].Poems.Count; j++)
                    {
                        if (token.IsCancellationRequested || more)
                            break;
                        var poem = versifiers[i].Poems[j];
                        if (bahar != Bahar.All && poem.poem_bahar != bahar)
                            continue;
                        string text = poem.ReadPoem(false, reader);
                        if (text == null)
                            continue;
                        string search_in = Ignore_def ? Functions.RemoveAccents(text) : text;
                        if (!search_in.Contains(search_word))
                            continue;
                        var lines = text.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        for (int k = 1; k < lines.Length; k++)
                        {
                            if (!(Ignore_def ? Functions.RemoveAccents(lines[k]) : lines[k]).Contains(search_word))
                                continue;
                            if (found.Count == max_results)
                            {
                                more = true;
                                break;
                            }
                            string line;
                            if (poem.is_vertical)
                                line = lines[k];
                            else if (k % 2 == 0)
                                line = lines[k - 1] + "     " + lines[k];
                            else if (k + 1 < lines.Length)
                            {
                                line = lines[k] + "     " + lines[k + 1];
                                k++;
                            }
                            else
                                line = lines[k];
                            found.Add(new ListItemTemplate(line, poem.name.Trim() + " - " + versifiers[i].name.Trim(), "n" + (i * 10000 + j) + "|" + query));
                        }
                    }
                    progress.Report(versifiers[i].Poems.Count);
                    if (token.IsCancellationRequested || more)
                        break;
                }
            });

            foreach (var i in found)
                results.Add(i);
            search_cancel = null;
            search_icon.Symbol = Symbol.Find;
            poets_combo.IsEnabled = bahar_combo.IsEnabled = ignore_difference.IsEnabled = true;
            Progress_bar.Visibility = Visibility.Collapsed;
            resultCount_textblock.Text = (more ? "أول " : "") + Functions.Result(results.Count) + (token.IsCancellationRequested ? " (تم ايقاف البحث)" : "");
            searchTime_textblock.Text = "زمن البحث: " + Math.Round((DateTime.Now.Ticks - start_time) / 10000000.0, 3) + " ثانية";
        }

        private void ResultsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ListItemTemplate;
            if (item != null)
                Functions.Navigate(typeof(view), item.Tag);
        }
    }
}
