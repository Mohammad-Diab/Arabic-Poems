using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class SelectPoem : ContentDialog
    {
        List<NameWithIndex> versifiers_backup = new List<NameWithIndex>();
        List<NameWithIndex> poems_backup = new List<NameWithIndex>();

        internal Poem selected_poem;

        public SelectPoem()
        {
            InitializeComponent();
        }

        ObservableCollection<NameWithIndex> versifiers_list_data = new ObservableCollection<NameWithIndex>();
        ObservableCollection<NameWithIndex> poems_list_data = new ObservableCollection<NameWithIndex>();

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            versifiers_list.DataContext = versifiers_list_data;
            poems_list.DataContext = poems_list_data;
            if (Versifier.VersifiersList != null)
            {
                for (int i = 0; i < Versifier.VersifiersList.Count; i++)
                {
                    NameWithIndex tmp = new NameWithIndex(Versifier.VersifiersList[i].name, i);
                    versifiers_list_data.Add(tmp);
                    versifiers_backup.Add(tmp);
                }
                versifiers_count.Text = HelperFunctions.ReturnNumbered(versifiers_list_data.Count, "شاعر واحد", "شاعران", "شاعر", "شعراء");
            }
        }

        private void VersifiersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (versifiers_list.SelectedIndex > -1)
            {
                poems_list_data.Clear();
                poems_backup.Clear();
                poem_filter.Text = "";
                for (int i = 0; i < Versifier.VersifiersList[(versifiers_list_data[versifiers_list.SelectedIndex]).Index].Poems.Count; i++)
                {
                    NameWithIndex tmp = new NameWithIndex(Versifier.VersifiersList[(versifiers_list_data[versifiers_list.SelectedIndex]).Index].Poems[i].name, i);
                    poems_list_data.Add(tmp);
                    poems_backup.Add(tmp);
                }
                poems_count.Text = HelperFunctions.ReturnNumbered(poems_list_data.Count, "قصيدة واحدة", "قصيدتان", "قصيدة", "قصائد");
            }
        }

        private async void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (versifiers_list.SelectedIndex > -1)
            {
                if (poems_list.SelectedIndex > -1)
                    selected_poem = Versifier.VersifiersList[versifiers_list_data[versifiers_list.SelectedIndex].Index].Poems[poems_list_data[poems_list.SelectedIndex].Index];
                else
                {
                    await new MessageDialog("عذرا: لم تقم بتحديد القصيدة, يرجى اختيار القصيدة أولا", "الشعر العربي").ShowAsync();
                    args.Cancel = false;
                }
            }
            else
            {
                await new MessageDialog("حدد الشاعر المطلوب ثم اختر احد قصائده", "الشعر العربي").ShowAsync();
                args.Cancel = false;
            }
        }

        private void VersifierFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            versifiers_list_data.Clear();
            poems_list_data.Clear();
            foreach (var i in versifiers_backup)
                if (HelperFunctions.RemoveAccents(i.Name.ToString()).Contains(HelperFunctions.RemoveAccents(versifier_filter.Text)))
                    versifiers_list_data.Add(i);
            poems_count.Text = HelperFunctions.ReturnNumbered(poems_list.Items.Count, "قصيدة واحدة", "قصيدتان", "قصيدة", "قصائد");
            versifiers_count.Text = HelperFunctions.ReturnNumbered(versifiers_list.Items.Count, "شاعر واحد", "شاعران", "شاعر", "شعراء");
        }

        private void PoemFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            poems_list_data.Clear();
            foreach (var i in poems_backup)
                if (HelperFunctions.RemoveAccents(i.Name.ToString()).Contains(HelperFunctions.RemoveAccents(poem_filter.Text)))
                    poems_list_data.Add(i);
            poems_count.Text = HelperFunctions.ReturnNumbered(poems_list.Items.Count, "قصيدة واحدة", "قصيدتان", "قصيدة", "قصائد");
        }
    }
}