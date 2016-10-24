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
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Make_poem_file
{
    /// <summary>
    /// Interaction logic for Poem_Maker_V2.xaml
    /// </summary>
    public partial class Poem_Maker_V2 : Window
    {
        public Poem_Maker_V2()
        {
            InitializeComponent();
        }

        private void FastReadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "serializing.dat";
            ofd.Filter = "Serialized poems|*.dat";
            if (ofd.ShowDialog() != true)
                return;
            BinaryFormatter BF = new BinaryFormatter();
            Stream st = File.OpenRead(ofd.FileName);
            try { Versifier2.VersifiersList = (List<Versifier2>)BF.Deserialize(st); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
            st.Close();
        }

        private void FirstReadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Poems.dat";
            ofd.Filter = "Arabic poems pack|*.dat";
            if (ofd.ShowDialog() != true)
                return;
            BinaryReader br = new BinaryReader(File.OpenRead(ofd.FileName));
            if (!LoadIndises(br))
            {
                br.Close();
                Versifier2.VersifiersList = null;
                MessageBox.Show("This file is not an Arabic poems pack.", "Error");
                return;
            }
            foreach (var i in Versifier2.VersifiersList)
                foreach (var j in i.Poems)
                    j.ReadPoem(true, br);
            br.Close();
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Serialized poems|*.dat";
            sfd.FileName = "serializing.dat";
            if (sfd.ShowDialog() != true)
                return;
            BinaryFormatter BF = new BinaryFormatter();
            Stream st = File.Create(sfd.FileName);
            BF.Serialize(st, Versifier2.VersifiersList);
            st.Close();
        }

        private bool LoadIndises(BinaryReader br)
        {
            try
            {
                Versifier2.VersifiersList = new List<Versifier2>();
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
                    Versifier2 tmp = new Versifier2(name, id, age, info);

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
                        tmp.Poems.Add(new Poem2(poem_name, id, tmp, "", is_vertical, start, length));
                    }
                    br.ReadByte();
                }

                return true;
            }
            catch (Exception)
            { return false; }
        }

        Dictionary<char, int> chars = new Dictionary<char, int>();
        List<string> long_words;
        List<int> long_words_v;
        List<int> long_words_p;
        List<int> long_words_l;

        private void button_Click(object sender, RoutedEventArgs e)
        {
            chars = new Dictionary<char, int>();
            long_words = new List<string>();
            long_words_v = new List<int>();
            long_words_p = new List<int>();
            long_words_l = new List<int>();
            int longest_word_size = 0;
            int longest_poem_count = 0, total_poems = 0, longest_lines_count = 0, total_lines_count = 0;
            string[] sp1 = { "\r\n" };
            string[] sp2 = { " " };
            if (Versifier2.VersifiersList == null)
                FirstReadFile_Click(null, null);
            if (Versifier2.VersifiersList == null)
                return;
            int i1 = 0;
            foreach (var i in Versifier2.VersifiersList)
            {
                int i2 = 0;
                if (i.Poems.Count > longest_poem_count)
                    longest_poem_count = i.Poems.Count;
                total_poems += i.Poems.Count;
                foreach (var j in i.Poems)
                {
                    int i3 = 0;
                    var tmp1 = j.poem.Split(sp1, StringSplitOptions.RemoveEmptyEntries);
                    if (tmp1.Length > longest_lines_count)
                        longest_lines_count = tmp1.Length;
                    total_lines_count += tmp1.Length;
                    foreach (var k in tmp1)
                    {
                        foreach (char gg in k)
                            if (!chars.ContainsKey(gg))
                                chars.Add(gg, 0);

                        var tmp2 = k.Split(sp2, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var l in tmp2)
                        {
                            if (l.Length > 20)
                            {
                                long_words.Add(l);
                                long_words_v.Add(i1);
                                long_words_p.Add(i2);
                                long_words_l.Add(i3);
                                listBox.Items.Add(k.Replace(",", ", ").Replace(":", ": "));
                            }
                            else if (longest_word_size < l.Length)
                                longest_word_size = l.Length;
                        }
                        i3++;
                    }
                    i2++;
                }
                i1++;
            }
            MessageBox.Show(long_words.Count.ToString(), long_words_p.Count.ToString());
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.SelectedIndex > -1)
                textBox.Text = textBox_Copy.Text = listBox.SelectedItem.ToString();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < listBox.Items.Count; i++)
                if (listBox.Items[i].Equals(textBox.Text))
                    listBox.Items[i] = textBox_Copy.Text;
        }

        private void button1_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (Versifier2.VersifiersList == null)
            {
                MessageBox.Show("Read the poems file first.", "Error");
                return;
            }
            for (int i = 0; i < listBox.Items.Count; i++)
            {
                int f = Versifier2.VersifiersList[long_words_v[i]].Poems[long_words_p[i]].poem.IndexOf(long_words[i]);
                if (f != -1)
                    Versifier2.VersifiersList[long_words_v[i]].Poems[long_words_p[i]].poem = Versifier2.VersifiersList[long_words_v[i]].Poems[long_words_p[i]].poem.Substring(0, f) + listBox.Items[i] + Versifier2.VersifiersList[long_words_v[i]].Poems[long_words_p[i]].poem.Substring(f + long_words[i].Length);
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Serialized poems|*.dat";
            sfd.FileName = "serializing" + new Random(DateTime.Now.Millisecond).Next() + ".dat";
            if (sfd.ShowDialog() != true)
                return;
            BinaryFormatter BF = new BinaryFormatter();
            Stream st = File.Create(sfd.FileName);
            BF.Serialize(st, Versifier2.VersifiersList);
            st.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //    foreach (var i in System.Diagnostics.Process.GetProcesses())
            //    {
            //        if(i.ProcessName.Contains("mfp"))
            //        {

            //        }
            //        ListViewItem kk = new ListViewItem();
            //        kk.Tag = i;
            //        kk.Content = i.ProcessName;
            //        kk.ToolTip = i.StartInfo.Arguments;
            //        listBox1.Items.Add(kk);
            //    }
            //}
        }

        [Serializable]
        internal class Versifier2
        {
            internal static List<Versifier2> VersifiersList;
            public string info;
            public string name;
            public int id;
            public string age;
            public int peoms_count;
            internal List<Poem2> Poems;

            public Versifier2(string n, int i, string a, string inf)
            {
                name = n;
                id = i;
                age = a;
                peoms_count = 0;
                info = inf;
                Poems = new List<Poem2>();
                VersifiersList.Add(this);
            }

            //static public int IndexOf(string st)
            //{
            //    string ver = StandartText(st);
            //    for (int i = 0; i < VersifiersList.Count; i++)
            //        if (StandartText(VersifiersList[i].name).Contains(ver) || ver.Contains(StandartText(VersifiersList[i].name)))
            //            return i;
            //    return -1;
            //}

            //static public void AddPoem(string ver, string p_n, string ag, int id, string p, bool is_shatean)
            //{
            //    int ind = IndexOf(ver);
            //    if (ind < 0)
            //    {
            //        Versifier v = new Versifier(ver, 0, ag, "");
            //        v.peoms.Add(new Poem(p_n, 0, v, p, is_shatean));
            //    }
            //    else
            //        VersifiersList[ind].peoms.Add(new Poem(p_n, 0, VersifiersList[ind], p, is_shatean));
            //}

            private static string StandartText(string ver)
            {
                return ver.Replace('ة', 'ه').Replace('أ', 'ا').Replace('إ', 'ا').Replace('ي', 'ى').Replace("َ", "").Replace("ً", "").Replace("ُ", "").Replace("ٌ", "").Replace("ِ", "").Replace("ٍ", "").Replace("ّ", "").Replace("ْ", "").Replace('ء', 'ا').Replace('آ', 'ا').Replace(":", "").Replace("\"", "");
            }

            public string GetName()
            {
                return name;
            }

            public string GetStandartName()
            {
                return StandartText(name);
            }

            public void SetID(int ID)
            {
                id = ID;
            }
        };

        [Serializable]
        internal class Poem2
        {
            public string name;
            public int id;
            public bool is_vertical;
            public Versifier2 versifier;
            public string poem;
            public long start, length;

            public Poem2(string n, int i, Versifier2 v, string sr, bool is_vertical, long start, long length)
            {
                name = n;
                id = i;
                versifier = v;
                poem = sr;
                this.start = start;
                this.length = length;
                this.is_vertical = is_vertical;
            }

            public void SetID(int ID)
            {
                id = ID;
            }

            public string GetStandartName()
            {
                return StandartText(name);
            }

            private static string StandartText(string ver)
            {
                return ver.Replace('ة', 'ه').Replace('أ', 'ا').Replace('إ', 'ا').Replace('ي', 'ى').Replace("َ", "").Replace("ً", "").Replace("ُ", "").Replace("ٌ", "").Replace("ِ", "").Replace("ٍ", "").Replace("ّ", "").Replace("ْ", "").Replace('ء', 'ا').Replace('آ', 'ا').Replace(":", "").Replace("\"", "");
            }

            internal string ReadPoem(bool set_poem, BinaryReader br)
            {
                if (poem == "")
                {
                    //Read poem from the file
                    br.BaseStream.Position = start;
                    byte[] poem_encrypted_bytes = new byte[1];
                    try
                    { poem_encrypted_bytes = br.ReadBytes(Convert.ToInt32(length)); }
                    catch (Exception)
                    { return null; }

                    //decrypte bytes
                    byte[] decryption_key = Encoding.Unicode.GetBytes("C4df!+ -Tr.y%6?k; \"p\\OX0");
                    string P = DecrptText(poem_encrypted_bytes, decryption_key);

                    if (set_poem)
                        poem = P;
                    return P;
                }
                return poem;
            }

            internal bool ReadPoem(BinaryReader br, out string error_message)
            {
                if (poem == "")
                {
                    //Read poem from the file
                    br.BaseStream.Position = start;
                    byte[] poem_encrypted_bytes = new byte[1];
                    try
                    { poem_encrypted_bytes = br.ReadBytes(Convert.ToInt32(length)); }
                    catch (Exception ex)
                    {
                        error_message = ex.Message;
                        return false;
                    }

                    //decrypte bytes
                    byte[] decryption_key = Encoding.Unicode.GetBytes("C4df!+ -Tr.y%6?k; \"p\\OX0");
                    string P = DecrptText(poem_encrypted_bytes, decryption_key);
                    poem = P;
                    error_message = "";
                    return true;
                }
                error_message = "";
                return true;
            }

            private string DecrptText(byte[] bytes, byte[] password)
            {
                var decrypt_bytes = DecryptBytes(bytes, password);
                return Encoding.Unicode.GetString(decrypt_bytes);
            }

            private byte[] DecryptBytes(byte[] word, byte[] key)
            {
                byte[] decrypted_bytes = new byte[word.Length];
                Parallel.For(0, word.Length, (i) =>
                {
                    word[i]++;
                    decrypted_bytes[i] = (byte)(word[i] ^ key[i % key.Length]);
                });
                return decrypted_bytes;
            }
        };
    }
}