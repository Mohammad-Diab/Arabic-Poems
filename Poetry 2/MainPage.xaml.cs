using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Poetry_2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        BinaryReader br;
        Poem current_poem;
        List<int> search_result;
        int current_result_index = -1;
        double font_size;
        Windows.Foundation.Rect bounds = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBounds;

        WindowState view;
        WindowState ViewMode
        {
            get { return view; }
            set
            {
                switch (view)
                {
                    case WindowState.MobileP:
                    case WindowState.MobileLandscape:
                        switch (value)
                        {
                            case WindowState.FHDP:
                            case WindowState.HDP:
                            case WindowState.HDLandscape:
                            case WindowState.FHDLandscape:
                                SetViewToLandscape();
                                break;
                        }
                        break;
                    case WindowState.FHDP:
                    case WindowState.HDP:
                    case WindowState.FHDLandscape:
                    case WindowState.HDLandscape:
                        switch (value)
                        {
                            case WindowState.MobileP:
                            case WindowState.MobileLandscape:
                                SetViewToMobile();
                                break;
                        }
                        break;
                }
                view = value;
            }
        }

        void SetViewToMobile()
        {
            search_textbox.FontSize = 16;
            search_textbox.HorizontalAlignment = HorizontalAlignment.Stretch;
            search_textbox.Margin = new Thickness(5, 10, 53, 0);
            search_textbox.Width = double.NaN;

            search_button.HorizontalAlignment = HorizontalAlignment.Right;
            search_button.Height = 33;
            search_button.Margin = new Thickness(0, 10, 10, 0);

            ignore_difference.FontSize = 14;
            ignore_difference.Margin = new Thickness(5, 50, 0, 0);

            checkBox.Margin = new Thickness(5, 50, 10, 0);
            checkBox.Height = checkBox.Width = 0;
            checkBox.Content = "";

            searchResult_list.Margin = new Thickness(5, 135, 10, 84);
            searchResult_list.FontSize = 16;
            //if (ActualHeight < 360)
            //    searchResult_list.MaxHeight = ActualHeight;
            //else
            //    searchResult_list.MaxHeight = ActualHeight - 200;

            saparated.Margin = new Thickness(5, 122, 10, 10);
            saparated.VerticalAlignment = VerticalAlignment.Top;
            saparated.HorizontalAlignment = HorizontalAlignment.Stretch;
            saparated.Height = 3;
            saparated.Width = double.NaN;

            comboBox.Visibility = Visibility.Visible;
            comboBox.SelectedIndex = versifier_list.SelectedIndex + 1;

            versifier_list.Visibility = Visibility.Collapsed;

            info_grid.VerticalAlignment = VerticalAlignment.Bottom;
            info_grid.Margin = new Thickness(5, 0, 10, 30);


            versifierName_textblock.FontSize = 14;
            versifierName_textblock.Margin = new Thickness(0, 5, 0, 0);

            versifierName_textblock.FontSize = 14;
            poemName_textblock.Margin = new Thickness(0, 30, 0, 0);

            select_stack.Visibility = Visibility.Collapsed;

            searchInfo_stack.Margin = new Thickness(5, 0, 10, 5);

            textBlock_Copy.Visibility = Visibility.Collapsed;
            search_scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        void SetViewToLandscape()
        {
            search_textbox.FontSize = 22;
            search_textbox.HorizontalAlignment = HorizontalAlignment.Left;
            search_textbox.Width = 348;
            search_textbox.Margin = new Thickness(10, 10, 0, 0);

            search_button.HorizontalAlignment = HorizontalAlignment.Left;
            search_button.Height = 42;
            search_button.Margin = new Thickness(358, 10, 0, 0);

            ignore_difference.FontSize = 16;
            ignore_difference.Margin = new Thickness(10, 57, 0, 0);

            checkBox.Margin = new Thickness(0, 5, 10, 0);
            checkBox.Height = 75;
            checkBox.Width = 144;
            checkBox.Content = "save poems in memory (this will speed up search but required more amount of RAM)";

            searchResult_list.Margin = new Thickness(415, 85, 5, 30);
            searchResult_list.FontSize = 20;
            //searchResult_list.MaxHeight = Double.NaN;

            saparated.Margin = new Thickness(405, 40, 0, 40);
            saparated.VerticalAlignment = VerticalAlignment.Stretch;
            saparated.HorizontalAlignment = HorizontalAlignment.Left;
            saparated.Height = double.NaN;
            saparated.Width = 3;

            comboBox.Visibility = Visibility.Collapsed;
            //comboBox.SelectedIndex = versifier_list.SelectedIndex + 1;

            versifier_list.Visibility = Visibility.Visible;
            SelectAllButton_Click(null, null);

            info_grid.VerticalAlignment = VerticalAlignment.Top;
            info_grid.Margin = new Thickness(415, 20, 5, 0);


            versifierName_textblock.FontSize = 16;
            versifierName_textblock.Margin = new Thickness(5, 5, 0, 0);

            versifierName_textblock.FontSize = 16;
            poemName_textblock.Margin = new Thickness(5, 35, 0, 0);

            select_stack.Visibility = Visibility.Visible;

            searchInfo_stack.Margin = new Thickness(405, 0, 5, 5);

            textBlock_Copy.Visibility = Visibility.Visible;
            search_scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
        }


        public MainPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Page_SizeChanged(null, null);
            view = WindowState.HDLandscape;
            searchResult_list.DataContext = searchResult_data;
            await SelectFile();
        }

        private async Task SelectFile()
        {
            FileOpenPicker fop = new FileOpenPicker();

            fop.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            fop.FileTypeFilter.Add(".dat");
            StorageFile file = null;
            try { file = await fop.PickSingleFileAsync(); }
            catch (Exception ex) { await new MessageDialog(ex.Message, "عذرا حدث خطا ما").ShowAsync(); }
            if (file != null)
            {
                if (search_cancel != null)
                    search_cancel.Cancel();
                versifier_list.Items.Clear();
                comboBox.Items.Clear();
                searchResult_data.Clear();
                br = new BinaryReader((await file.OpenReadAsync()).AsStream());
                if (LoadIndises(br))
                {
                    empty_state.Visibility = Visibility.Collapsed;
                    await Favourites.Load();
                    Random rand = new Random(DateTime.UtcNow.Millisecond);
                    int v = rand.Next(Versifier.VersifiersList.Count);
                    int p = rand.Next(Versifier.VersifiersList[v].Poems.Count);
                    ViewPoem(Versifier.VersifiersList[v].Poems[p]);
                }
                else
                {
                    Versifier.VersifiersList = null;
                    current_poem = null;
                    listView.Items.Clear();
                    poem_name.Text = "القصيدة";
                    versifier_name.Text = "الشاعر";
                    empty_state.Visibility = Visibility.Visible;
                    await new MessageDialog("عذرا حدث خطا ما أثناء قراءة الملفات.", "الشعر العربي").ShowAsync();
                }
            }
        }

        private bool LoadIndises(BinaryReader br)
        {
            try
            {
                Versifier.VersifiersList = new List<Versifier>();
                var file_header = br.ReadBytes(8);
                byte[] orginal_header = { 0xAC, 0x4D, 0x0F, 0x14, 0xF2, 0x82, 0x7C, 0xBB };
                //check header
                if (!file_header.SequenceEqual(orginal_header))
                    return false;

                //string text = "Arabic poems encrypted pack, version 1.0, by Mohammad Diab.";
                byte[] orginal_file_info = { 0x41, 0x72, 0x61, 0x62, 0x69, 0x63, 0x20, 0x70, 0x6F, 0x65, 0x6D, 0x73, 0x20, 0x65, 0x6E, 0x63, 0x72, 0x79,
                0x70, 0x74, 0x65, 0x64, 0x20, 0x70, 0x61, 0x63, 0x6B, 0x2C, 0x20, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x20, 0x31, 0x2E,
                0x30, 0x2C, 0x20, 0x62, 0x79, 0x20, 0x4D, 0x6F, 0x68, 0x61, 0x6D, 0x6D, 0x61, 0x64, 0x20, 0x44, 0x69, 0x61, 0x62, 0x2E,0x00,
                0x00, 0x00, 0x00, 0x00 };
                var file_info = br.ReadBytes(64);

                //check header

                var version = br.ReadSingle();

                var first_index = br.ReadInt64();
                var versifiers_count = br.ReadInt32();
                //br.BaseStream.Position = first_index;
                for (int v = 0; v < versifiers_count; v++)
                {
                    string name, age, info;
                    int id;
                    int poems_count;
                    short name_length, age_length;
                    int info_length;

                    id = br.ReadInt32();

                    name_length = br.ReadInt16();
                    age_length = br.ReadInt16();
                    info_length = br.ReadInt32();
                    name = Encoding.Unicode.GetString(br.ReadBytes(name_length * 2));
                    age = Encoding.Unicode.GetString(br.ReadBytes(age_length * 2));
                    info = Encoding.Unicode.GetString(br.ReadBytes(info_length * 2));
                    poems_count = br.ReadInt32();
                    Versifier tmp = new Versifier(name, id, age, info);

                    for (int p = 0; p < poems_count; p++)
                    {
                        string poem_name;
                        int poem_id, versifier_id;
                        short poem_name_length;
                        long start, length;
                        bool is_vertical;

                        poem_id = br.ReadInt32();
                        versifier_id = br.ReadInt32();
                        poem_name_length = br.ReadInt16();
                        start = br.ReadInt64();
                        length = br.ReadInt64();
                        var poem_info = br.ReadByte();

                        //for other information
                        switch (poem_info)
                        {
                            case 0x00:
                                is_vertical = false;
                                break;
                            case 0x01:
                                is_vertical = true;
                                break;
                            default:
                                is_vertical = false;
                                break;
                        }

                        poem_name = Encoding.Unicode.GetString(br.ReadBytes(poem_name_length * 2));
                        tmp.Poems.Add(new Poem(poem_name, id, tmp, "", is_vertical, start, length));
                    }
                    br.ReadByte();
                }

                Fill();
                return true;
            }
            catch (Exception)
            { return false; }
        }

        private void Fill()
        {
            if (Versifier.VersifiersList != null)
            {
                if (versifier_list.Items.Count == 0)
                {
                    comboBox.Items.Add("كل الشعراء");
                    for (int i = 0; i < Versifier.VersifiersList.Count; i++)
                    {
                        versifier_list.Items.Add(MakeVersifierListItem(Versifier.VersifiersList[i].name, i));
                        comboBox.Items.Add(Versifier.VersifiersList[i].name);
                    }
                }
                else if (bounds.Width > 640 || ActualWidth > 640)
                    SelectAllButton_Click(selectAll_button, new RoutedEventArgs());
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ActualWidth < 361 || bounds.Width < 361)
                ViewMode = WindowState.MobileP;
            else if (ActualWidth < 641 || bounds.Width < 641)
                ViewMode = WindowState.MobileLandscape;
            else if (ActualWidth < 769 || bounds.Width < 769)
                ViewMode = WindowState.HDP;
            else if ((ActualWidth < 1367 && ActualHeight < 769) || (bounds.Width < 1367 && bounds.Width < 769))
                ViewMode = WindowState.HDLandscape;
            else if (ActualWidth < ActualHeight || bounds.Width < bounds.Height)
                ViewMode = WindowState.FHDP;
            else if (ActualWidth < ActualHeight || bounds.Width > bounds.Height)
                ViewMode = WindowState.FHDLandscape;
            else
                ViewMode = WindowState.HDLandscape;
        }

        private async void FontSettingButton_Click(object sender, RoutedEventArgs e)
        {
            await new FontSetting().ShowAsync();
        }

        private async void FavouriteButton_Click(object sender, RoutedEventArgs e)
        {
            FavouriteList FL = new FavouriteList();
            FL.current_poem = current_poem;
            await FL.ShowAsync();
            if (FL.selected_poem != null)
                ViewPoem(FL.selected_poem);
        }

        private async void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            var version = Windows.ApplicationModel.Package.Current.Id.Version;
            await new MessageDialog(string.Format("الشعر العربي\nالإصدار {0}.{1}.{2}.{3}\nMohammad Diab\n2016", version.Major, version.Minor, version.Build, version.Revision), Windows.ApplicationModel.Package.Current.DisplayName).ShowAsync();
        }



        #region Explore Poems

        //View Poem

        private void PivotExplore_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            BottomAppBar.Visibility = Visibility.Visible;
        }

        private async void ViewPoem_Click(object sender, RoutedEventArgs e)
        {
            SelectPoem SP = new SelectPoem();
            await SP.ShowAsync();
            var selected_poem = SP.selected_poem;
            if (selected_poem != null)
                ViewPoem(selected_poem);
        }

        private async void ViewPoem(Poem selected_poem)
        {
            current_poem = selected_poem;
            search_result = null;
            current_result_index = -1;
            listView.Items.Clear();
            string error_message;

            if (current_poem.ReadPoem(br, out error_message))
            {
                //view poem
                string[] splitter = { "\r\n" };
                var lines = current_poem.poem.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 1; i < lines.Length; i++)
                {
                    listView.Items.Add(MakePoemLine(current_poem.is_vertical, lines[i], current_poem.is_vertical ? "" : i + 1 < lines.Length ? lines[i + 1] : "", current_poem.is_vertical ? i : (i + 1) / 2));
                    if (!current_poem.is_vertical) i++;
                }
                poem_name.Text = current_poem.name;
                versifier_name.Text = current_poem.versifier.name;
            }
            else
                await new MessageDialog(error_message, "عذرا حدث خطا ما").ShowAsync();
            if (listView.Items.Count > 0)
                listView.ScrollIntoView(listView.Items[0]);
        }

        private Grid MakePoemLine(bool is_vertical, string first, string second, int line_number)
        {
            TextBlock t1 = new TextBlock();
            t1.Margin = new Thickness(40, 5, 0, 0);
            t1.Tag = t1.Text = first;
            t1.VerticalAlignment = VerticalAlignment.Top;
            t1.HorizontalAlignment = HorizontalAlignment.Left;
            t1.FontSize = bounds.Width > 1919 ? 48 : bounds.Width < 641 ? 24 : 32;
            t1.TextWrapping = TextWrapping.NoWrap;

            TextBlock number = new TextBlock();
            number.Text = line_number.ToString(); ;
            number.VerticalAlignment = VerticalAlignment.Top;
            number.HorizontalAlignment = HorizontalAlignment.Left;
            number.Width = 30;
            number.TextAlignment = TextAlignment.Center;
            number.Foreground = new SolidColorBrush(new Color() { A = 0xFF, R = 0x40, G = 0x40, B = 0x40 });
            number.Margin = new Thickness(0, (is_vertical ? t1.FontSize / 2 + 4 : t1.FontSize + 10), 0, 0);
            number.FontFamily = new FontFamily("Segoe UI");

            Rectangle rec = new Rectangle();
            rec.HorizontalAlignment = HorizontalAlignment.Left;
            rec.Fill = new SolidColorBrush(new Color() { A = 0xFF, R = 0xF4, G = 0xF4, B = 0xF5 });
            rec.StrokeThickness = 0;
            rec.Margin = new Thickness(0, 5, 0, 5);
            rec.Width = 30;

            Grid grid = new Grid();
            //grid.Height = is_vertical ? 60 : 110;

            grid.Children.Add(rec);
            grid.Children.Add(number);
            grid.Children.Add(t1);



            if (!is_vertical)
            {
                TextBlock t2 = new TextBlock();
                t2.Margin = new Thickness(bounds.Width > 1919 ? 380 : bounds.Width < 641 ? 120 : 250, bounds.Width > 1919 ? 75 : bounds.Width < 641 ? 40 : 55, 0, 0);
                t2.Text = second;
                t2.VerticalAlignment = VerticalAlignment.Top;
                t2.HorizontalAlignment = HorizontalAlignment.Left;
                t2.FontSize = bounds.Width > 1919 ? 48 : bounds.Width < 641 ? 24 : 32;
                t2.Tag = second;
                t2.TextWrapping = TextWrapping.NoWrap;
                grid.Children.Add(t2);
                //grid.Height = t2.Margin.Top + t2.ActualHeight + 10;
            }



            //else
            //    grid.Height = t1.Margin.Top + t1.ActualHeight + 10;


            //ListBoxItem item = new ListBoxItem();
            //item.Content = grid;
            return grid;
        }



        //Find in Page

        private void ShowFindInPageDialogButton_Click(object sender, RoutedEventArgs e)
        {
            if (search_in_page.Visibility == Visibility.Collapsed)
            {
                search_in_page.Visibility = Visibility.Visible;
                //BegainAnimaton(search_in_page, new Thickness(0, 0, 10, 0), new Thickness(0, 0, 10, 20), TimeSpan.FromMilliseconds(100), null);
                search_box.Focus(FocusState.Keyboard);
            }
        }

        private async void FindInPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (current_poem == null || current_poem.poem == null)
                return;
            if (search_result == null)
            {
                RestSearch();
                search_result = new List<int>();
                string word = HelperFunctions.RemoveAccents(search_box.Text.Trim());
                if (word.Length > 1)
                {
                    string[] sp = { "\r\n" };
                    var split = current_poem.poem.Split(sp, StringSplitOptions.RemoveEmptyEntries);
                    if (current_poem.is_vertical)
                    {
                        for (int i = 1; i < split.Length && i - 1 < listView.Items.Count; i++)
                        {
                            if (HelperFunctions.RemoveAccents(split[i]).Contains(word))
                            {
                                SelectWord(i - 1, word, current_poem.is_vertical);
                                search_result.Add(i - 1);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 1; i < split.Length && (i - 1) / 2 < listView.Items.Count; i += 2)
                        {
                            if ((HelperFunctions.RemoveAccents(i + 1 < split.Length ? split[i] + " " + split[i + 1] : split[i])).Contains(word))
                            {
                                SelectWord((i - 1) / 2, word, current_poem.is_vertical);
                                search_result.Add((i - 1) / 2);
                            }
                        }
                    }
                    if (search_result.Count == 0)
                    {
                        if (listView.Items.Count > 0)
                            listView.ScrollIntoView(listView.Items[0]);
                        await new MessageDialog("لا توجد نتائج", "الشعر العربي").ShowAsync();
                    }
                }
                else
                    await new MessageDialog("كلمة البحث قصيرة", "الشعر العربي").ShowAsync();
            }
            if (search_result.Count > 0)
            {
                if (current_result_index == search_result.Count - 1)
                    current_result_index = -1;
                listView.ScrollIntoView(listView.Items[search_result[++current_result_index]]);
            }
        }

        private void SearchBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                FindInPageButton_Click(sender, null);
        }

        private void SelectWord(int line_number, string text, bool is_vertical)
        {
            Grid gr = listView.Items[line_number] as Grid;
            (gr.Children[0] as Rectangle).Fill = new SolidColorBrush(Colors.SkyBlue);
            TextBlock t1 = gr.Children[2] as TextBlock;
            List<Run> tmp = new List<Run>();

            var indises = FindIndicess(t1.Text, text);
            for (int i = 1; i < indises.Count; i++)
            {
                Run r1 = new Run();
                r1.Text = t1.Text.Substring(indises[i - 1].X + indises[i - 1].Y, indises[i].X - (indises[i - 1].X + indises[i - 1].Y));
                Run r2 = new Run();
                r2.Text = t1.Text.Substring(indises[i].X, indises[i].Y);
                r2.Foreground = new SolidColorBrush(Colors.Tomato);
                tmp.Add(r1);
                tmp.Add(r2);
            }
            if ((indises[indises.Count - 1].X + indises[indises.Count - 1].Y) < t1.Text.Length)
            {
                Run r3 = new Run();
                r3.Text = t1.Text.Substring((indises[indises.Count - 1].X + indises[indises.Count - 1].Y));
                tmp.Add(r3);
            }

            t1.Text = "";
            foreach (var i in tmp)
                t1.Inlines.Add(i);

            tmp.Clear();

            if (!is_vertical)
            {
                TextBlock t2 = gr.Children[3] as TextBlock;
                indises = FindIndicess(t2.Text, text);
                for (int i = 1; i < indises.Count; i++)
                {
                    Run r1 = new Run();
                    r1.Text = t2.Text.Substring(indises[i - 1].X + indises[i - 1].Y, indises[i].X - (indises[i - 1].X + indises[i - 1].Y));
                    Run r2 = new Run();
                    r2.Text = t2.Text.Substring(indises[i].X, indises[i].Y);
                    r2.Foreground = new SolidColorBrush(Colors.Tomato);
                    tmp.Add(r1);
                    tmp.Add(r2);
                }
                if ((indises[indises.Count - 1].X + indises[indises.Count - 1].Y) < t2.Text.Length)
                {
                    Run r3 = new Run();
                    r3.Text = t2.Text.Substring((indises[indises.Count - 1].X + indises[indises.Count - 1].Y));
                    tmp.Add(r3);
                }
                t2.Text = "";
                foreach (var i in tmp)
                    t2.Inlines.Add(i);

                tmp.Clear();
            }
        }

        private List<Point> FindIndicess(string Text, string search_word)
        {
            List<Point> indexes = new List<Point>();
            indexes.Add(new Point(0, 0));
            string Accent = HelperFunctions.Accents;

            for (int i = 0; i < Text.Length; i++)
            {
                int ind = -1, length = 0;
                if (Accent.Contains(Text[i].ToString()))
                    continue;
                if (HelperFunctions.OptimizeChars(Text[i]) == search_word[0])
                {
                    length++;
                    int k = 1;
                    ind = i;
                    for (int j = 1; k < search_word.Length; j++)
                    {
                        if (i + j >= Text.Length)
                        {
                            ind = -1;
                            break;
                        }
                        if (Accent.Contains(Text[i + j].ToString()))
                        {
                            length++;
                            continue;
                        }
                        if (HelperFunctions.OptimizeChars(Text[i + j]) == search_word[k])
                        {
                            k++;
                            length++;
                        }
                        else
                        {
                            ind = -1;
                            break;
                        }
                    }
                    if (ind > -1)
                    {
                        i += length - 1;
                        indexes.Add(new Point(ind, length));
                    }
                }
            }
            return indexes;
        }

        private void RestSearch()
        {
            SolidColorBrush rec_brush = new SolidColorBrush(new Color() { A = 0xFF, R = 0xF4, G = 0xF4, B = 0xF5 });
            foreach (Grid i in listView.Items)
            {
                (i.Children[0] as Rectangle).Fill = rec_brush;
                TextBlock t1 = i.Children[2] as TextBlock;
                t1.Inlines.Clear();
                t1.Text = t1.Tag.ToString();
                if (i.Children.Count == 4)
                {
                    t1 = i.Children[3] as TextBlock;
                    t1.Inlines.Clear();
                    t1.Text = t1.Tag.ToString();
                }
            }
            search_result = null;
            current_result_index = -1;
        }

        private void FindInPageButton_TextChanged(object sender, TextChangedEventArgs e)
        {
            search_result = null;
            current_result_index = -1;
        }

        private void HideFindInPageButton_Click(object sender, RoutedEventArgs e)
        {
            search_box.Text = "";
            RestSearch();
            //BegainAnimaton(search_in_page, new Thickness(0, 0, 10, 20), new Thickness(0, 0, 10, 0), TimeSpan.FromMilliseconds(100), (object sender2, RunWorkerCompletedEventArgs e2) => { search_in_page.Visibility = Visibility.Collapsed; });
            search_in_page.Visibility = Visibility.Collapsed;
        }



        //Other Functions

        private void BegainAnimaton(FrameworkElement element, Thickness start, Thickness end, TimeSpan time, RunWorkerCompletedEventHandler do_after_finish)
        {
            element.Margin = start;
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (object sender, DoWorkEventArgs e) =>
            {
                for (int i = 0; i < 20; i++)
                {
                    Task.Delay((int)(time.TotalMilliseconds / 20)).Wait();
                    bw.ReportProgress(i);
                }
            };
            bw.ProgressChanged += (object sender, ProgressChangedEventArgs e) =>
            {
                double left, right, top, bottom;

                if (start.Left == end.Left) left = start.Left;
                else if (start.Left < end.Left) left = start.Left + (end.Left * e.ProgressPercentage) / 20;
                else left = start.Left - (end.Left * e.ProgressPercentage) / 20;

                if (start.Right == end.Right) right = start.Right;
                else if (start.Right < end.Right) right = start.Right + (end.Right * e.ProgressPercentage) / 20;
                else right = start.Right - (end.Right * e.ProgressPercentage) / 20;

                if (start.Top == end.Top) top = start.Top;
                else if (start.Top < end.Top) top = start.Top + (end.Top * e.ProgressPercentage) / 20;
                else top = start.Top - (end.Top * e.ProgressPercentage) / 20;

                if (start.Bottom == end.Bottom) bottom = start.Bottom;
                else if (start.Bottom < end.Bottom) bottom = start.Bottom + (end.Bottom * e.ProgressPercentage) / 20;
                else bottom = start.Bottom - (end.Bottom * e.ProgressPercentage) / 20;

                element.Margin = new Thickness(left, top, right, bottom);
            };
            bw.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) => { element.Margin = end; };
            if (do_after_finish != null)
                bw.RunWorkerCompleted += do_after_finish;
            bw.WorkerReportsProgress = true;
            bw.RunWorkerAsync();
        }


        #endregion



        #region Search

        //ObservableCollection<NameWithIndex> versifier_data = new ObservableCollection<NameWithIndex>();
        ObservableCollection<SearchResult> searchResult_data = new ObservableCollection<SearchResult>();
        CancellationTokenSource search_cancel;
        const int max_results = 2000;

        private void PivotSearch_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //if (bounds.Width < 641||ActualWidth<641)
            //{
            //    search_textbox.FontSize = 16;
            //    search_textbox.HorizontalAlignment = HorizontalAlignment.Stretch;
            //    search_textbox.Margin = new Thickness(5, 10, 49, 0);
            //    search_textbox.Width = double.NaN;

            //    search_button.HorizontalAlignment = HorizontalAlignment.Right;
            //    search_button.Height = 33;
            //    search_button.Margin = new Thickness(0, 10, 5, 0);

            //    ignore_difference.FontSize = 14;
            //    ignore_difference.Margin = new Thickness(5, 50, 0, 0);

            //    checkBox.Margin = new Thickness(5, 50, 0, 0);
            //    checkBox.Height = checkBox.Width = 0;
            //    checkBox.Content = "";

            //    searchResult_list.Margin = new Thickness(5, 135, 5, 84);
            //    searchResult_list.FontSize = 16;

            //    saparated.Margin = new Thickness(5, 122, 5, 10);
            //    saparated.VerticalAlignment = VerticalAlignment.Top;
            //    saparated.HorizontalAlignment = HorizontalAlignment.Stretch;
            //    saparated.Height = 3;
            //    saparated.Width = double.NaN;

            //    comboBox.Visibility = Visibility.Visible;
            //    comboBox.SelectedIndex = versifier_list.SelectedIndex + 1;

            //    versifier_list.Visibility = Visibility.Collapsed;

            //    info_grid.VerticalAlignment = VerticalAlignment.Bottom;
            //    info_grid.Margin = new Thickness(5, 0, 5, 30);


            //    versifierName_textblock.FontSize = 14;
            //    versifierName_textblock.Margin = new Thickness(0, 5, 0, 0);

            //    versifierName_textblock.FontSize = 14;
            //    poemName_textblock.Margin = new Thickness(0, 30, 0, 0);

            //    select_stack.Visibility = Visibility.Collapsed;

            //    searchInfo_stack.Margin = new Thickness(5, 0, 5, 5);

            //    textBlock_Copy.Visibility = Visibility.Collapsed;
            //}

            BottomAppBar.Visibility = Visibility.Collapsed;
            if (Versifier.VersifiersList != null)
            {
                if (versifier_list.Items.Count == 0)
                {
                    for (int i = 0; i < Versifier.VersifiersList.Count; i++)
                        versifier_list.Items.Add(MakeVersifierListItem(Versifier.VersifiersList[i].name, i));
                }
                else
                    SelectAllButton_Click(selectAll_button, new RoutedEventArgs());
            }
        }

        private CheckBox MakeVersifierListItem(string name, int index)
        {
            CheckBox it = new CheckBox();
            it.Content = name;
            it.IsChecked = true;
            it.Tag = index;
            return it;
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
            foreach (CheckBox i in versifier_list.Items)
                i.IsChecked = true;
        }

        private void SelectNoneButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox i in versifier_list.Items)
                i.IsChecked = false;
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
            string search_word = Ignore_def ? HelperFunctions.RemoveAccents(search_textbox.Text.Trim()) : search_textbox.Text.Trim();
            bool save_in_memory = checkBox.IsChecked.Value;
            Progress_bar.Value = 0;
            if (search_word.Length > 1)
            {
                long start_time = DateTime.Now.Ticks;
                List<int> selected_versifiers = new List<int>();
                Progress_bar.Maximum = 0;
                if (versifier_list.Visibility == Visibility.Visible)
                    foreach (CheckBox i in versifier_list.Items)
                    {
                        if (i.IsChecked.Value)
                        {
                            Progress_bar.Maximum += Versifier.VersifiersList[Convert.ToInt32(i.Tag)].Poems.Count;
                            selected_versifiers.Add(Convert.ToInt32(i.Tag));
                        }
                    }
                else
                {
                    if (comboBox.SelectedIndex == 0)
                        for (int i = 0; i < Versifier.VersifiersList.Count; i++)
                        {
                            Progress_bar.Maximum += Versifier.VersifiersList[i].Poems.Count;
                            selected_versifiers.Add(i);
                        }
                    else
                    {
                        if (comboBox.SelectedIndex > 0)
                        {
                            Progress_bar.Maximum += Versifier.VersifiersList[comboBox.SelectedIndex - 1].Poems.Count;
                            selected_versifiers.Add(comboBox.SelectedIndex - 1);
                        }
                    }
                }
                if (selected_versifiers.Count == 0)
                {
                    await new MessageDialog("حدد شاعرا واحدا على الأقل", "الشعر العربي").ShowAsync();
                    return;
                }
                searchResult_data.Clear();
                Progress_bar.Visibility = Visibility.Visible;
                versifier_list.IsEnabled = comboBox.IsEnabled = selectNone_button.IsEnabled = selectAll_button.IsEnabled = false;
                search_icon.Symbol = Symbol.Cancel;
                search_cancel = new CancellationTokenSource();
                var token = search_cancel.Token;
                var reader = br;
                var versifiers = Versifier.VersifiersList;
                bool more = false;
                List<Poem> p_list = new List<Poem>();
                List<string> l_list = new List<string>();
                IProgress<int> progress = new Progress<int>((count) => { Progress_bar.Value += count; });

                await Task.Run(() =>
                {
                    string[] splitter = { "\r\n" };
                    foreach (int i in selected_versifiers)
                    {
                        foreach (var j in versifiers[i].Poems)
                        {
                            if (token.IsCancellationRequested || more)
                                break;
                            string Tmp = j.ReadPoem(save_in_memory, reader);
                            if (Tmp == null)
                                continue;
                            string search_in = Ignore_def ? HelperFunctions.RemoveAccents(Tmp) : Tmp;
                            if (search_in.Contains(search_word))
                            {
                                var lines = Tmp.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                                for (int k = 1; k < lines.Length; k++)
                                {
                                    if ((Ignore_def ? HelperFunctions.RemoveAccents(lines[k]) : lines[k]).Contains(search_word))
                                    {
                                        if (p_list.Count == max_results)
                                        {
                                            more = true;
                                            break;
                                        }
                                        p_list.Add(j);
                                        if (j.is_vertical)
                                            l_list.Add(lines[k]);
                                        else if (k % 2 == 0)
                                            l_list.Add(lines[k - 1] + " " + lines[k]);
                                        else if (k + 1 < lines.Length)
                                        {
                                            l_list.Add(lines[k] + " " + lines[k + 1]);
                                            k++;
                                        }
                                        else
                                            l_list.Add(lines[k]);
                                    }
                                }
                            }
                        }
                        progress.Report(versifiers[i].Poems.Count);
                        if (token.IsCancellationRequested || more)
                            break;
                    }
                });

                for (int i = 0; i < l_list.Count && i < p_list.Count; i++)
                    searchResult_data.Add(new SearchResult(l_list[i], p_list[i]));
                search_cancel = null;
                search_icon.Symbol = Symbol.Find;
                versifier_list.IsEnabled = comboBox.IsEnabled = selectNone_button.IsEnabled = selectAll_button.IsEnabled = true;
                Progress_bar.Visibility = Visibility.Collapsed;
                resultCount_textblock.Text = (more ? "أول " : "") + HelperFunctions.ReturnNumbered(searchResult_data.Count, "نتيجة واحدة", "نتيجتان", "نتيجة", "نتائج") + (token.IsCancellationRequested ? " (تم ايقاف البحث)" : "");
                searchTime_textblock.Text = "زمن البحث: " + Math.Round((DateTime.Now.Ticks - start_time) / 10000000.0, 3) + " ثانية";
                if (searchResult_data.Count == 0 && !token.IsCancellationRequested)
                    await new MessageDialog("لا توجد نتائج", "الشعر العربي").ShowAsync();
            }
            else
                await new MessageDialog("كلمة البحث قصيرة", "الشعر العربي").ShowAsync();
        }

        private void SearchTextbox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
                SearchButton_Click(sender, null);
        }

        private void SearchResultList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var result = e.ClickedItem as SearchResult;
            if (result != null)
            {
                pivot.SelectedIndex = 0;
                ViewPoem(result.PoemPointer);
                search_box.Text = search_textbox.Text.Trim();
                search_in_page.Visibility = Visibility.Visible;
            }
        }

        private void SearchResultList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchResult_list.SelectedIndex > -1)
            {
                versifierName_textblock.Text = "الشاعر: " + searchResult_data[searchResult_list.SelectedIndex].PoemPointer.versifier.name;
                poemName_textblock.Text = "القصيدة: " + searchResult_data[searchResult_list.SelectedIndex].PoemPointer.name;
            }
        }

        #endregion

        private async void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            await SelectFile();
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((Pivot)sender).SelectedIndex == 0)
                BottomAppBar.Visibility = Visibility.Visible;
            else if (((Pivot)sender).SelectedIndex == 1)
                BottomAppBar.Visibility = Visibility.Collapsed;
            //if (ActualWidth < 361 || bounds.Width < 361)
            //    ViewMode = WindowState.MobileP;
            //else if (ActualWidth < 641 || bounds.Width < 641)
            //    ViewMode = WindowState.MobileLandscape;
            //else if (ActualWidth < 768 || bounds.Width < 768)
            //    ViewMode = WindowState.HDP;
            //else if ((ActualWidth < 1366 && ActualHeight < 768) || (bounds.Width < 1366 && bounds.Width < 768))
            //    ViewMode = WindowState.HDLandscape;
            //else if (ActualWidth < ActualHeight || bounds.Width < bounds.Height)
            //    ViewMode = WindowState.FHDP;
            //else if (ActualWidth < ActualHeight || bounds.Width > bounds.Height)
            //    ViewMode = WindowState.FHDLandscape;
            //else
            //    ViewMode = WindowState.HDLandscape;
        }
    }

    class Versifier
    {
        internal static List<Versifier> VersifiersList;
        public string info;
        public string name;
        public int id;
        public string age;
        public int peoms_count;
        internal List<Poem> Poems;

        public Versifier(string n, int i, string a, string inf)
        {
            name = n;
            id = i;
            age = a;
            peoms_count = 0;
            info = inf;
            Poems = new List<Poem>();
            VersifiersList.Add(this);
        }

        //static public int IndexOf(string st)
        //{
        //    string ver = StandartText(st);
        //    for (int i = 0; i < VersifiersList.Count; i++)
        //        if (StandartText(VersifiersList[i].name).Contains(ver) || ver.Contains(StandartText(VersifiersList[i].name)))
        //            return i;
        //    return -1;
        //}

        //static public void AddPoem(string ver, string p_n, string ag, int id, string p, bool is_shatean)
        //{
        //    int ind = IndexOf(ver);
        //    if (ind < 0)
        //    {
        //        Versifier v = new Versifier(ver, 0, ag, "");
        //        v.peoms.Add(new Poem(p_n, 0, v, p, is_shatean));
        //    }
        //    else
        //        VersifiersList[ind].peoms.Add(new Poem(p_n, 0, VersifiersList[ind], p, is_shatean));
        //}

        private static string StandartText(string ver)
        {
            return ver.Replace('ة', 'ه').Replace('أ', 'ا').Replace('إ', 'ا').Replace('ي', 'ى').Replace("َ", "").Replace("ً", "").Replace("ُ", "").Replace("ٌ", "").Replace("ِ", "").Replace("ٍ", "").Replace("ّ", "").Replace("ْ", "").Replace('ء', 'ا').Replace('آ', 'ا').Replace(":", "").Replace("\"", "");
        }

        public string GetName()
        {
            return name;
        }

        public string GetStandartName()
        {
            return StandartText(name);
        }

        public void SetID(int ID)
        {
            id = ID;
        }
    };

    internal class Poem
    {
        public string name;
        public int id;
        public bool is_vertical;
        public Versifier versifier;
        public string poem;
        public long start, length;

        public Poem(string n, int i, Versifier v, string sr, bool is_vertical, long start, long length)
        {
            name = n;
            id = i;
            versifier = v;
            poem = sr;
            this.start = start;
            this.length = length;
            this.is_vertical = is_vertical;
        }

        public void SetID(int ID)
        {
            id = ID;
        }

        public string GetStandartName()
        {
            return StandartText(name);
        }

        private static string StandartText(string ver)
        {
            return ver.Replace('ة', 'ه').Replace('أ', 'ا').Replace('إ', 'ا').Replace('ي', 'ى').Replace("َ", "").Replace("ً", "").Replace("ُ", "").Replace("ٌ", "").Replace("ِ", "").Replace("ٍ", "").Replace("ّ", "").Replace("ْ", "").Replace('ء', 'ا').Replace('آ', 'ا').Replace(":", "").Replace("\"", "");
        }

        internal string ReadPoem(bool set_poem, BinaryReader br)
        {
            if (poem == "")
            {
                //Read poem from the file
                byte[] poem_encrypted_bytes = new byte[1];
                try
                {
                    lock (br)
                    {
                        br.BaseStream.Position = start;
                        poem_encrypted_bytes = br.ReadBytes(Convert.ToInt32(length));
                    }
                }
                catch (Exception)
                { return null; }

                //decrypte bytes
                byte[] decryption_key = Encoding.Unicode.GetBytes("C4df!+ -Tr.y%6?k; \"p\\OX0");
                string P = DecrptText(poem_encrypted_bytes, decryption_key);

                if (set_poem)
                    poem = P;
                return P;
            }
            return poem;
        }

        internal bool ReadPoem(BinaryReader br, out string error_message)
        {
            if (poem == "")
            {
                //Read poem from the file
                byte[] poem_encrypted_bytes = new byte[1];
                try
                {
                    lock (br)
                    {
                        br.BaseStream.Position = start;
                        poem_encrypted_bytes = br.ReadBytes(Convert.ToInt32(length));
                    }
                }
                catch (Exception ex)
                {
                    error_message = ex.Message;
                    return false;
                }

                //decrypte bytes
                byte[] decryption_key = Encoding.Unicode.GetBytes("C4df!+ -Tr.y%6?k; \"p\\OX0");
                string P = DecrptText(poem_encrypted_bytes, decryption_key);
                poem = P;
                error_message = "";
                return true;
            }
            error_message = "";
            return true;
        }

        private string DecrptText(byte[] bytes, byte[] password)
        {
            var decrypt_bytes = DecryptBytes(bytes, password);
            return Encoding.Unicode.GetString(decrypt_bytes);
        }

        private byte[] DecryptBytes(byte[] word, byte[] key)
        {
            byte[] decrypted_bytes = new byte[word.Length];
            Parallel.For(0, word.Length, (i) =>
            {
                word[i]++;
                decrypted_bytes[i] = (byte)(word[i] ^ key[i % key.Length]);
            });
            return decrypted_bytes;
        }
    };

    internal static class HelperFunctions
    {
        internal const string Accents = "ًٌٍَُِّْٰـ";

        internal static string RemoveAccents(string str)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            foreach (char ch in str)
                if (Accents.IndexOf(ch) < 0)
                    sb.Append(OptimizeChars(ch));
            return sb.ToString();
        }

        internal static string DecrptText(byte[] bytes, byte[] password)
        {
            var decrypt_bytes = DecryptBytes(bytes, password);
            return Encoding.Unicode.GetString(decrypt_bytes);
        }

        private static byte[] DecryptBytes(byte[] word, byte[] key)
        {
            byte[] decrypted_bytes = new byte[word.Length];
            Parallel.For(0, word.Length, (i) =>
            {
                word[i]++;
                decrypted_bytes[i] = (byte)(word[i] ^ key[i % key.Length]);
            });
            return decrypted_bytes;
        }

        internal static char OptimizeChars(char ch)
        {
            switch (ch)
            {
                case 'أ':
                case 'إ':
                case 'آ':
                case 'ٱ':
                    return 'ا';
                case 'ى':
                case 'ئ':
                    return 'ي';
                case 'ؤ':
                    return 'و';
                case 'ة':
                    return 'ه';
                default:
                    return ch;
            }
        }

        internal static string ReturnNumbered(int count, string one, string two, string single, string multi)
        {
            if (count == 1)
                return one;
            else if (count == 2)
                return two;
            else if (count % 100 < 3)
                return count + " " + single;
            else if (count % 100 < 11)
                return count + " " + multi;
            else
                return count + " " + single;
        }
    }

    struct Point
    {
        public int X;
        public int Y;

        public Point(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }

    public class NameWithIndex
    {
        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        int index;
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        bool is_selected;
        public bool IsSelected
        {
            get { return is_selected; }
            set { is_selected = value; }
        }

        public NameWithIndex(string name, int index)
        {
            Name = name;
            Index = index;
            IsSelected = false;
        }

    }

    class SearchResult
    {
        string line;
        public string Line
        {
            get { return line; }
            set { line = value; }
        }


        Poem poem;
        public Poem PoemPointer
        {
            get { return poem; }
            set { poem = value; }
        }

        public SearchResult(string line, Poem poem)
        {
            PoemPointer = poem;
            Line = line;
        }

    }

    enum WindowState
    {
        MobileP = 1,
        MobileLandscape = 2,
        HDP = 3,
        HDLandscape,
        FHDP,
        FHDLandscape
    }
}
