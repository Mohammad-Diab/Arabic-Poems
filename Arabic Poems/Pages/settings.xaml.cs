using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

namespace Arabic_Poems.Pages
{
    public sealed partial class settings : Page
    {
        bool loading = true;

        public settings()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            loading = true;
            font_slider.Value = Settings.FontSize;
            sample_text.FontSize = Settings.FontSize;
            object name = ApplicationData.Current.LocalSettings.Values["poems_file_name"];
            file_name.Text = Versifier.VersifiersList != null && name != null ? name.ToString() : "لم يتم تحديد ملف";
            loading = false;
        }

        private void FontSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (sample_text == null)
                return;
            sample_text.FontSize = e.NewValue;
            if (!loading)
                Settings.FontSize = e.NewValue;
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            var main = (Window.Current.Content as Frame).Content as MainPage;
            if (main != null)
                main.Read(true);
        }
    }
}
