using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
using Microsoft.Win32;

namespace Peoms_Serializer
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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Poems text files|*.txt";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() != true)
                return;
            var h = ofd.FileNames.OrderBy(x => x).ToArray();
            string[] splitter = { ">>" };
            int i = 0;
            StreamReader m = new StreamReader(File.Open(h[i], FileMode.Open));
            string c = m.ReadLine();
            if (c == null || !c.Contains(">>"))
            {
                m.Close();
                MessageBox.Show("The first line of the first file must be a poem title (age >> poet >> poem).", "Error");
                return;
            }
            Versifier.versifiers.Clear();
            do
            {
                bool stop = false;
                if (c.Contains(">>"))
                {
                    string age = "", ve = "", po = "";
                    var b = c.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                    if (b.Length == 3)
                    {
                        age = b[0];
                        ve = b[1];
                        po = b[2];
                    }
                    StringBuilder sb = new StringBuilder();
                    while (!(c = m.ReadLine()).Contains(">>"))
                    {
                        if (m.EndOfStream)
                        {
                            m.Close();
                            i++;
                            if (i > h.Length - 1)
                            {
                                stop = true;
                                break;
                            }
                            m = new StreamReader(File.Open(h[i], FileMode.Open));
                        }
                        if (!c.Contains("------------------") && !c.Equals(string.Empty))
                            sb.AppendLine(c);
                    }
                    if (stop)
                        break;
                    Versifier.AddPoem(ve, po, age, 0, sb.ToString());
                }
                if (!stop && m.EndOfStream)
                {
                    m.Close();
                    i++;
                    if (i > h.Length - 1)
                        break;
                    m = new StreamReader(File.Open(h[i], FileMode.Open));
                }
            } while (true);
            BinaryFormatter j = new BinaryFormatter();

            //j.Serialize(File.OpenWrite(@"C:\Users\pal\Desktop\دواوين الشعر العربي على مر العصور\11.dat"), Versifier.versifiers);
            List<string> kk = new List<string>();
            foreach (var n in Versifier.versifiers)
                kk.Add(n.GetName());
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Data files|*.dat";
            sfd.FileName = "12.dat";
            if (sfd.ShowDialog() != true)
                return;
            Stream s = File.Create(sfd.FileName);
            j.Serialize(s, kk);
            s.Close();
        }

        [Serializable]
        class Versifier
        {
            public static List<Versifier> versifiers = new List<Versifier>();
            string info;
            string name;
            int id;
            string age;
            int peoms_count;
            List<Poem> peoms;

            public Versifier(string n, int i, string a,string inf)
            {
                name = n;
                id = i;
                age = a;
                peoms_count = 0;
                info = inf;
                peoms = new List<Poem>();
                versifiers.Add(this);
            }

            static public int IndexOf(string st)
            {
                string ver = StandartText(st);
                for (int i = 0; i < versifiers.Count; i++)
                    if (StandartText(versifiers[i].name).Contains(ver) || ver.Contains(StandartText(versifiers[i].name)))
                        return i;
                return -1;
            }

            static public void AddPoem(string ver, string p_n, string ag, int id, string p)
            {
                int ind = IndexOf(ver);
                if (ind < 0)
                {
                    Versifier v = new Versifier(ver, 0, ag, "");
                    v.peoms.Add(new Poem(p_n, 0, v, p));
                }
                else
                    versifiers[ind].peoms.Add(new Poem(p_n, 0, versifiers[ind], p));
            }

            private static string StandartText(string ver)
            {
                return ver.Replace('ة', 'ه').Replace('أ', 'ا').Replace('إ', 'ا').Replace('ي', 'ى').Replace("َ", "").Replace("ً", "").Replace("ُ", "").Replace("ٌ", "").Replace("ِ", "").Replace("ٍ", "").Replace("ّ", "").Replace("ْ", "").Replace('ء', 'ا').Replace('آ', 'ا').Replace(":","").Replace("\"", "");
            }

            public string GetName()
            {
                return name;
            }
        };

        [Serializable]
        class Poem
        {
            string name;
            int id;
            Versifier versifier;
            string poem;

            public Poem(string n, int i, Versifier v, string sr)
            {
                name = n;
                id = i;
                versifier = v;
                poem = sr;
            }

        };
    }
}
