using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Arabic_Poems.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class browse : Page
    {
        ObservableCollection<PoemVerse> poem_verses = new ObservableCollection<PoemVerse>();

        Rect bounds = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBounds;
        BinaryReader br;
        Poem current_poem;
        List<int> search_result;
        int current_result_index = -1;
        double font_size;

        public browse()
        {
            this.InitializeComponent();
            ////listView.ItemsSource = poem_verses;
            Read();
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
                    Random rand = new Random(DateTime.UtcNow.Millisecond);
                    int v = rand.Next(Versifier.VersifiersList.Count);
                    int p = rand.Next(Versifier.VersifiersList[v].Poems.Count);
                    ViewPoem(Versifier.VersifiersList[v].Poems[p]);
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

        private async void ViewPoem(Poem selected_poem)
        {
            poem_verses.Clear();
            string error_message;
            current_poem = selected_poem;
            if (current_poem.ReadPoem(br, out error_message))
            {
                string[] splitter = { "\r\n" };
                var lines = current_poem.poem.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                int i;
                for (i = 1; i < lines.Length; i++)
                {
                    poem_verses.Add(new PoemVerse(current_poem.is_vertical ? i : (i + 1) / 2, lines[i], current_poem.is_vertical ? "" : i + 1 < lines.Length ? lines[i + 1] : ""));
                    if (!current_poem.is_vertical)
                        i++;
                }
                verse_count_tb.Text = "عدد الأبيات: " + (current_poem.is_vertical ? i - 1 : i / 2);
                poem_name_tb.Text = current_poem.name;
                poet_name_tb.Text = current_poem.versifier.name;
            }
            else
                await new Windows.UI.Popups.MessageDialog(error_message, "عذرا حدث خطا ما").ShowAsync();
            ////if (listView.Items.Count > 0)
            ////    listView.ScrollIntoView(listView.Items[0]);


            search_result = null;
            current_result_index = -1;
            //listView.Items.Clear();


            //if (current_poem.ReadPoem(br, out error_message))
            //{
            //    //view poem
            //    string[] splitter = { "\r\n" };
            //    var lines = current_poem.poem.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            //    for (int i = 1; i < lines.Length; i++)
            //    {
            //        listView.Items.Add(MakePoemLine(current_poem.is_vertical, lines[i], current_poem.is_vertical ? "" : i + 1 < lines.Length ? lines[i + 1] : "", current_poem.is_vertical ? i : (i + 1) / 2));
            //        if (!current_poem.is_vertical) i++;
            //    }
            //    poem_name_tb.Text = current_poem.name;
            //    poet_name_tb.Text = current_poem.versifier.name;
            //}
            //else
            //    await new Windows.UI.Popups.MessageDialog(error_message, "عذرا حدث خطا ما").ShowAsync();
            //if (listView.Items.Count > 0)
            //    listView.ScrollIntoView(listView.Items[0]);
        }

        private void Page_Loading(FrameworkElement sender, object args)
        {
            PageFrame.Navigate(typeof(Home));
        }
    }



}
