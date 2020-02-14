using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Arabic_Poems.Pages
{
    public sealed partial class SelectPoem : ContentDialog
    {
        List<NameWithIndex> versifiers_backup = new List<NameWithIndex>();
        List<NameWithIndex> poems_backup = new List<NameWithIndex>();
        int versifier_index = -1;

        internal string selected_tag;

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
                    NameWithIndex tmp = new NameWithIndex(Versifier.VersifiersList[i].name.Trim(), i);
                    versifiers_list_data.Add(tmp);
                    versifiers_backup.Add(tmp);
                }
                versifiers_count.Text = Functions.Poet(versifiers_list_data.Count);
            }
        }

        private void VersifiersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (versifiers_list.SelectedIndex > -1)
            {
                versifier_index = versifiers_list_data[versifiers_list.SelectedIndex].Index;
                poems_list_data.Clear();
                poems_backup.Clear();
                poem_filter.Text = "";
                for (int i = 0; i < Versifier.VersifiersList[versifier_index].Poems.Count; i++)
                {
                    NameWithIndex tmp = new NameWithIndex(Versifier.VersifiersList[versifier_index].Poems[i].name.Trim(), i);
                    poems_list_data.Add(tmp);
                    poems_backup.Add(tmp);
                }
                poems_count.Text = Functions.Poem(poems_list_data.Count);
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (versifier_index > -1 && poems_list.SelectedIndex > -1)
                selected_tag = "n" + (versifier_index * 10000 + poems_list_data[poems_list.SelectedIndex].Index);
            else
            {
                Title = versifier_index > -1 ? "عذرا: لم تقم بتحديد القصيدة, يرجى اختيار القصيدة أولا" : "حدد الشاعر المطلوب ثم اختر احد قصائده";
                args.Cancel = true;
            }
        }

        private void PoemsList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (versifier_index > -1 && poems_list.SelectedIndex > -1)
            {
                selected_tag = "n" + (versifier_index * 10000 + poems_list_data[poems_list.SelectedIndex].Index);
                Hide();
            }
        }

        private void VersifierFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            versifiers_list_data.Clear();
            poems_list_data.Clear();
            versifier_index = -1;
            foreach (var i in versifiers_backup)
                if (Functions.RemoveAccents(i.Name).Contains(Functions.RemoveAccents(versifier_filter.Text)))
                    versifiers_list_data.Add(i);
            poems_count.Text = "";
            versifiers_count.Text = Functions.Poet(versifiers_list_data.Count);
        }

        private void PoemFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            poems_list_data.Clear();
            foreach (var i in poems_backup)
                if (Functions.RemoveAccents(i.Name).Contains(Functions.RemoveAccents(poem_filter.Text)))
                    poems_list_data.Add(i);
            poems_count.Text = Functions.Poem(poems_list_data.Count);
        }
    }
}
