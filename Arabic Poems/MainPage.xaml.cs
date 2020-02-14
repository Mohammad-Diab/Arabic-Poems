using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Arabic_Poems
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        internal Frame rootFram;
        internal BinaryReader br;

        public MainPage()
        {
            this.InitializeComponent();
            navview.SelectedItem = home;
            rootFram = ContentFrame;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Read(false);
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            Read(true);
        }

        private void OnNavigationViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                ContentFrame.Navigate(typeof(Pages.settings));
            }
            else
            {
                var invokedItem = navview.MenuItems.Cast<NavigationViewItem>().Single(i => i.Content.Equals(args.InvokedItem));
                switch (invokedItem.Tag)
                {
                    case "home":
                        if (Versifier.VersifiersList != null)
                            ContentFrame.Navigate(typeof(Pages.Home), invokedItem.Tag);
                        break;
                    case "fav":
                        if (Versifier.VersifiersList != null)
                            ContentFrame.Navigate(typeof(Pages.favourites));
                        break;
                    case "about":
                        ShowAbout();
                        break;
                }
            }
        }

        private async void ShowAbout()
        {
            var version = Package.Current.Id.Version;
            await new Windows.UI.Popups.MessageDialog(string.Format("الشعر العربي\nالإصدار {0}.{1}.{2}.{3}\nMohammad Diab\n2020\n\nمصدر القصائد وتراجم الشعراء: موقع أدب\nالبحور: مطابقة مع مجموعة بيانات Ashaar (arbml) - استخدام غير تجاري", version.Major, version.Minor, version.Build, version.Revision), "حول").ShowAsync();
        }

        internal async void Read(bool select_new)
        {
            StorageFile file = null;
            if (!select_new && StorageApplicationPermissions.FutureAccessList.ContainsItem("poems_file"))
            {
                try { file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync("poems_file"); }
                catch (Exception) { file = null; }
            }
            if (file == null)
            {
                FileOpenPicker fop = new FileOpenPicker();

                fop.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                fop.FileTypeFilter.Add(".dat");
                try { file = await fop.PickSingleFileAsync(); }
                catch (Exception ex) { await new Windows.UI.Popups.MessageDialog(ex.Message, "عذرا حدث خطا ما").ShowAsync(); }
                if (file == null)
                {
                    if (Versifier.VersifiersList == null)
                        empty_state.Visibility = Visibility.Visible;
                    return;
                }
            }

            BinaryReader reader = null;
            try { reader = new BinaryReader((await file.OpenReadAsync()).AsStream()); }
            catch (Exception) { reader = null; }
            if (reader != null && LoadIndises(reader))
            {
                br = reader;
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("poems_file", file);
                ApplicationData.Current.LocalSettings.Values["poems_file_name"] = file.Path;
                empty_state.Visibility = Visibility.Collapsed;
                await Favourites.Load();
                navview.SelectedItem = home;
                ContentFrame.Navigate(typeof(Pages.Home), "home");
                ContentFrame.BackStack.Clear();
            }
            else
            {
                Versifier.VersifiersList = null;
                br = null;
                if (StorageApplicationPermissions.FutureAccessList.ContainsItem("poems_file"))
                    StorageApplicationPermissions.FutureAccessList.Remove("poems_file");
                ContentFrame.Content = null;
                empty_state.Visibility = Visibility.Visible;
                await new Windows.UI.Popups.MessageDialog("عذرا حدث خطا ما أثناء قراءة الملفات.", "الشعر العربي").ShowAsync();
            }
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput || Versifier.VersifiersList == null)
                return;
            string text = sender.Text.Trim();
            string word = Functions.RemoveAccents(text);
            List<ListItemTemplate> items = new List<ListItemTemplate>();
            if (word.Length > 1)
            {
                for (int i = 0; i < Versifier.VersifiersList.Count && items.Count < 8; i++)
                    if (Functions.RemoveAccents(Versifier.VersifiersList[i].name).Contains(word))
                        items.Add(new ListItemTemplate(Versifier.VersifiersList[i].name.Trim(), "شاعر - " + Functions.Poem(Versifier.VersifiersList[i].peoms_count), "poet|n" + i));
                int poets_count = items.Count;
                for (int i = 0; i < Versifier.VersifiersList.Count && items.Count < poets_count + 12; i++)
                    for (int j = 0; j < Versifier.VersifiersList[i].Poems.Count && items.Count < poets_count + 12; j++)
                    {
                        var poem = Versifier.VersifiersList[i].Poems[j];
                        if (poem.search_name == null)
                            poem.search_name = Functions.RemoveAccents(poem.name);
                        if (poem.search_name.Contains(word))
                            items.Add(new ListItemTemplate(poem.name.Trim(), Versifier.VersifiersList[i].name.Trim(), "poem|n" + (i * 10000 + j)));
                    }
                items.Add(new ListItemTemplate("ابحث عن \"" + text + "\" في نصوص القصائد", "بحث في كل القصائد", "search|" + text));
            }
            sender.ItemsSource = items;
        }

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (Versifier.VersifiersList == null)
                return;
            var item = args.ChosenSuggestion as ListItemTemplate;
            if (item == null)
                item = new ListItemTemplate(args.QueryText, "", "search|" + args.QueryText);
            var parts = item.Tag.Split(new char[] { '|' }, 2);
            if (parts[0] == "poet")
                ContentFrame.Navigate(typeof(Pages.poem), parts[1]);
            else if (parts[0] == "poem")
                ContentFrame.Navigate(typeof(Pages.view), parts[1]);
            else if (parts[1].Trim().Length > 1)
                ContentFrame.Navigate(typeof(Pages.search), parts[1].Trim());
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
                    tmp.peoms_count = poems_count;
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

                ReadExtension(br);
                return true;
            }
            catch (Exception)
            { return false; }
        }

        private void ReadExtension(BinaryReader br)
        {
            try
            {
                if (br.BaseStream.Length < 20)
                    return;
                br.BaseStream.Position = br.BaseStream.Length - 12;
                long extension_index = br.ReadInt64();
                if (Encoding.UTF8.GetString(br.ReadBytes(4)) != "APX1" || extension_index < 0 || extension_index > br.BaseStream.Length - 20)
                    return;
                br.BaseStream.Position = extension_index;
                if (Encoding.UTF8.GetString(br.ReadBytes(4)) != "APX1")
                    return;
                int count = br.ReadInt32();
                foreach (var v in Versifier.VersifiersList)
                    foreach (var p in v.Poems)
                    {
                        if (count-- <= 0)
                            return;
                        p.poem_bahar = (Bahar)br.ReadByte();
                        br.ReadUInt16();
                    }
            }
            catch (Exception) { }
        }

        //private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        //{
        //    char page = e.Parameter.ToString()[0];
        //    var command = (CommandBar)navview.HeaderTemplate.;
        //    //page legends
        //    //  b is for     browser
        //    //  h is for     Home
        //    //  t is for     poet
        //    //  m is for     poem
        //    //  v is for     view
        //    switch(page)
        //    {
        //        case 'h':
        //            (command.PrimaryCommands[0] as Control).Visibility = Visibility.Collapsed;
        //            (command.PrimaryCommands[1] as Control).Visibility = Visibility.Visible;
        //            (command.PrimaryCommands[2] as Control).Visibility = Visibility.Collapsed;
        //            (command.PrimaryCommands[3] as Control).Visibility = Visibility.Collapsed;
        //            (command.PrimaryCommands[4] as Control).Visibility = Visibility.Collapsed;
        //            (command.PrimaryCommands[5] as Control).Visibility = Visibility.Collapsed;
        //            (command.PrimaryCommands[6] as Control).Visibility = Visibility.Collapsed;
        //            break;
        //        case 't':
        //            break;
        //        case 'm':
        //            break;
        //        case 'v':
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }



}