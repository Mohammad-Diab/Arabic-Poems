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
    public sealed partial class view : Page
    {
        ObservableCollection<PoemVerse> list = new ObservableCollection<PoemVerse>();
        List<ListItemTemplate> more = new List<ListItemTemplate>();
        Poem current;
        public view()
        {
            this.InitializeComponent();
            verses_list.DataContext = list;
            more_list.DataContext = more;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LastPoem.AddPoem(e.Parameter.ToString());
            long prt, tmp;
            int poet_inex, poem_inex;
            long.TryParse(e.Parameter.ToString().Substring(1), out tmp);
            prt = (int)(tmp / 100000000);
            tmp %= 100000000;
            poet_inex = (int)(tmp / 10000);
            poem_inex = (int)(tmp % 10000);
            char section = e.Parameter.ToString()[0];

            
            current = Versifier.VersifiersList[poet_inex].Poems[poem_inex];
            title.Text = current.name;
            poet_name_tb.Text = current.versifier.name;
            poet_desc_tb.Text = GetPoetDescription(current.versifier);
            poem_desc_tb.Text = GetPoemDescription(current);



            FillReadMoreList(current, section, prt * 100000000 + poet_inex * 10000);
            ViewPoem(current);
        }

        private void FillReadMoreList(Poem current, char section, long tag)
        {
            more.Clear();
            if (current.versifier.Poems.Count < 2)
                return;
            if (current.versifier.Poems.Count < 11)
            {
                for (int i = 0; i < current.versifier.Poems.Count; i++)
                    if (current.versifier.Poems[i].name != current.name)
                        more.Add(new ListItemTemplate(current.versifier.Poems[i].name, "", section + (tag + i).ToString()));
            }
            else
            {
                Random rand = new Random(DateTime.Now.Millisecond);
                while (more.Count < 10)
                {
                    int poem_index = rand.Next(current.versifier.Poems.Count);
                    if (!more.Exists(item => item.Name == current.versifier.Poems[poem_index].name || item.Name == current.name))
                        more.Add(new ListItemTemplate(current.versifier.Poems[poem_index].name, "", section + (tag + poem_index).ToString()));
                }
            }

        }

        private string GetPoemDescription(Poem current)
        {
            return "";
        }

        private string GetPoetDescription(Versifier versifier)
        {
            return "";
        }

        private async void ViewPoem(Poem selected_poem)
        {
            list.Clear();
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
    }
}
