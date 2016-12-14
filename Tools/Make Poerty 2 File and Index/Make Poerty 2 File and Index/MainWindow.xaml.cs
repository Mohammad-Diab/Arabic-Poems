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
            string[] splitter = { ">>" };
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
                    Versifier.versifiers.Clear();
                    List<string> all_text = new List<string>();
                    foreach(var i in Directory.GetFiles(tbx_source_folder.Text))
                        all_text.AddRange(File.ReadLines(i));
                    progress = 5;
                    mission = "applaying setting ...";

                    for(int i=0;i< all_text.Count;i++)
                    {
                        if (all_text[i].Contains(">>"))
                        {
                            
                            string age = "", ve = "", po = "";
                            int poem_num;
                            var tmp = all_text[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                            if (tmp.Length == 3)
                            {
                                age = tmp[0];
                                ve = tmp[1];
                                po = tmp[2];
                            }
                            Poem p = new Poem(po);
                            i+=2;
                            if (i >= all_text.Count)
                                break;
                            if(all_text[i].Contains("رقم القصيدة :"))
                            {
                                var tmp1 = all_text[i].Remove(0, 14);   // "رقم القصيدة : ".Length
                                int.TryParse(tmp1, out poem_num);

                            }
                            int lines_count = Functions.StandardText(all_text[i++]).Length;
                            bool vertical = false;
                            while (i < all_text.Count && !all_text[i].Contains(">>"))
                            {
                                if (!all_text[i].Contains("------------------") && !all_text[i].Equals(string.Empty))
                                {
                                    if (!vertical)
                                    {
                                        if (all_text[i].Contains("***") || all_text[i].Contains("..."))
                                            vertical = true;
                                        else
                                        {
                                            if (lines_count > Functions.StandardText(all_text[i]).Length + 10 && lines_count < Functions.StandardText(all_text[i]).Length - 10)
                                                vertical = true;
                                            else
                                            {
                                                lines_count = Functions.StandardText(all_text[i]).Length;
                                                if (lines_count > 35 || lines_count < 15)
                                                    vertical = true;
                                            }
                                        }
                                    }
                                    p.lines.Add(new Line(all_text[i], p));
                                }
                                i++;
                            }
                            i--;
                            p.SetVertical(vertical);
                            if (p != null)
                                Versifier.AddPoem(p, ve, age);
                        }
                    }
                }
                
                string[] splitter_space = { " " };
                Dictionary<string, List<Indices>> ind = new Dictionary<string, List<Indices>>();
                for (int i = 0; i < Versifier.versifiers.Count; i++)
                {
                    for (int j = 0; j < Versifier.versifiers[i].odes.Count; j++)
                        for (int k = 0; k < Versifier.versifiers[i].odes[j].lines.Count; k++)
                        {
                            string line = Functions.StandardText(Versifier.versifiers[i].odes[j].lines[k].Text);
                            var strs = line.Split(splitter_space, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var str in strs)
                            {
                                if (str.Length > 1)
                                {
                                    if (!ind.ContainsKey(str))
                                        ind.Add(str, new List<Indices>());
                                    ind[str].Add(new Indices(str, i, j, k));
                                }
                            }
                        }
                }
                var x = ind;
                tbx_current_mission.Text = "Done";
                textBlock3.Inlines.Add(new LineBreak());
                textBlock3.Inlines.Add(new Run(string.Format("{0} poets, {1} poems, {2} lines, {3} words in the index", Versifier.versifiers.Count, Versifier.versifiers.Sum(v => v.odes.Count), Versifier.versifiers.Sum(v => v.odes.Sum(o => o.lines.Count)), ind.Count)));
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

        Poem poem;

        public Line(string s, Poem p)
        {
            text = s;
            poem = p;
            order = poem.lines.Count;
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

        internal List<Line> lines;

        public Poem(string name)
        {
            this.name = name;
            lines = new List<Line>();
        }

        public void SetPoet(Versifier poet)
        {
            this.poet = poet;
        }

        public void SetVertical(bool is_vertical)
        {
            vertical = is_vertical;
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

        string age;
        public string Age
        {
            get { return age; }
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

        internal List<Poem> odes;

        public Versifier(string name, string age)
        {
            this.name = name;
            this.age = age;
            order = versifiers.Count;
            odes = new List<Poem>();
        }

        public static Versifier AddPoem(Poem p, string versifier, string age)
        {
            int index = -1;
            if ((index = Functions.IndexOf(Functions.StandardText(versifier))) > -1)
            {
                versifiers[index].odes.Add(p);
                p.SetPoet(versifiers[index]);
                return versifiers[index];
            }
            else
            {
                Versifier ver = new Versifier(Functions.StandardText(versifier), Functions.StandardText(age));
                ver.odes.Add(p);
                p.SetPoet(ver);
                versifiers.Add(ver);
                return ver;
            }
        }

        static internal List<Versifier> versifiers = new List<Versifier>();


    }


    class Indices
    {
        public string word;
        public int poet;
        public int poem;
        public int line;

        public Indices(string w, int t, int m, int l)
        {
            word = w;
            poem = m;
            poet = t;
            line = l;
        }
    }


    static class Functions
    {
        public static string StandardText(string ver)
        {
            return ver.Replace('ة', 'ه').Replace('أ', 'ا').Replace('إ', 'ا').Replace('ى', 'ي').Replace("َ", "").Replace("ً", "").Replace("ُ", "").Replace("ٌ", "").Replace("ِ", "").Replace("ٍ", "").Replace("ّ", "").Replace("ْ", "").Replace('ء', 'ا').Replace('آ', 'ا').Replace(":", "").Replace("\"", "");
        }

        public static int IndexOf(string standard_sting)
        {
            for (int i = 0; i < Versifier.versifiers.Count; i++)
                if (Versifier.versifiers[i].Name.Contains(standard_sting) || standard_sting.Contains(Versifier.versifiers[i].Name))
                    return i;
            return -1;
        }

    }


}
