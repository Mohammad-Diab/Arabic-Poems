using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Poem_Serializer
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

        private void Read_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    class Line
    {
        uint ID;
        uint number;
        string Line1;
        string Line2;
        bool is_dual;

        Line() { }
    }

    class Poem
    {
        uint ID;
        uint number;
        string name;
        Poet poet;
        List<Line> poem_lines;

        Poem() { }
    }

    class Poet
    {
        uint ID;
        uint number;
        string name;
        List<Poem> poems;
        int peoms_count;
        string age;

        Poet() { }
    }
}
