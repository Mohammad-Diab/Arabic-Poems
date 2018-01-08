using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
            Read();
        }

        private void OnNavigationViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                //if (navview.SelectedItem != navview.SettingsItem)
                //ContentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                var invokedItem = navview.MenuItems.Cast<NavigationViewItem>().Single(i => i.Content.Equals(args.InvokedItem));
                switch(invokedItem.Tag)
                {
                    case "home":
                        ContentFrame.Navigate(typeof(Pages.Home), invokedItem.Tag);
                        break;
                    case "fav":
                        break;
                    case "about":
                        break;
                }
            }
        }

        private async void Read()
        {
            FileOpenPicker fop = new FileOpenPicker();

            fop.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            fop.FileTypeFilter.Add(".dat");
            StorageFile file = null;
            try { file = await fop.PickSingleFileAsync(); }
            catch (Exception ex) { await new Windows.UI.Popups.MessageDialog(ex.Message, "عذرا حدث خطا ما").ShowAsync(); }
            //if (file == null)
            //    file = await StorageFile.GetFileFromPathAsync(@"d:\Poems.dat");
            //file = await Package.Current.InstalledLocation.GetFileAsync(@"Poems.dat");
            if (file != null)
            {
                br = new BinaryReader((await file.OpenReadAsync()).AsStream());
                if (LoadIndises(br))
                {
                }
                else
                {
                    await new Windows.UI.Popups.MessageDialog("عذرا حدث خطا ما أثناء قراءة الملفات.", "الشعر العربي").ShowAsync();
                    //Application.Current.Exit();
                }
            }

            //else
            //    Application.Current.Exit();
        }

        private bool LoadIndises(BinaryReader br)
        {
            try
            {
                Versifier.VersifiersList = new List<Versifier>();
                var file_header = br.ReadBytes(8);
                byte[] orginal_header = { 0xAC, 0x4D, 0x0F, 0x14, 0xF2, 0x82, 0x7C, 0xBB };
                //check header

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

                return true;
            }
            catch (Exception)
            { return false; }
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
