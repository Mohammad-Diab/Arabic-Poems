using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Make_Poerty_2_File_and_Index
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string mission = "";
        double progress = 0;

        private void SelectSourceButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if(fbd.ShowDialog()== System.Windows.Forms.DialogResult.OK)
            {
                tbx_source_folder.Text = fbd.SelectedPath;
            }
        }

        private void SelectDestinationButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                tbx_destination_folder.Text = fbd.SelectedPath;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(tbx_source_folder.Text))
            {
                if (!Directory.Exists(tbx_destination_folder.Text))
                    try { Directory.CreateDirectory(tbx_destination_folder.Text); }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message, "Error");
                        return;
                    }
                mission = "Reading files ...";
                progress = 1;
                {
                    List<string> all_text = new List<string>();
                    foreach(var i in Directory.GetFiles(tbx_source_folder.Text))
                        all_text.AddRange(File.ReadLines(i));
                    progress = 5;
                    mission = "applaying sittion ...";

                    for(int i=0;i< all_text.Count;i++)
                    {
                        if(all_text[i].Contains(">>"))
                        {
                            Poem p = new Poem();
                        }
                    }
                }

            }
            else
                System.Windows.MessageBox.Show("Directory not found");
        }
    }

    class Line
    {
        int id;

        public int ID
        {
            get { return id; }
        }

        string text;
        public string Text
        {
            get { return text; }
        }

        int order;
        public int Order
        {
            get { return order; }
        }

        public Line()
        {

        }

    }


    class Poem
    {
        int id;

        public int ID
        {
            get { return id; }
        }

        string name;
        public string Name
        {
            get { return name; }
        }

        int order;
        public int Order
        {
            get { return order; }
        }

        bool vertical;
        public bool Vertical
        {
            get { return vertical; }
        }

        Versifier poet;
        public Versifier Poet
        {
            get { return poet; }
        }

        string info;
        public string Info
        {
            get { return info; }
        }

        List<Line> lines = new List<Line>();

        public Poem()
        {

        }

    }


    class Versifier
    {
        int id;

        public int ID
        {
            get { return id; }
        }

        string name;
        public string Name
        {
            get { return name; }
        }

        string info;
        public string Info
        {
            get { return info; }
        }

        int order;
        public int Order
        {
            get { return order; }
        }

        List<Poem> odes = new List<Poem>();

        public Versifier()
        {

        }

    }


}
