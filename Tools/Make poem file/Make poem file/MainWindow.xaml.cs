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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace Make_poem_file
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
        //D:\Working Folder\Temp\Old Projects Temp\دواوين الشعر العربي على مر العصور\Saare
        private void button_Click(object sender, RoutedEventArgs e)
        {
            var files_array = Directory.GetFiles(@"D:\Working Folder\Temp\Old Projects Temp\دواوين الشعر العربي على مر العصور\Saare");
            string[] splitter = { ">>" };
            int i = 0;
            StreamReader SR = new StreamReader(File.Open(files_array[i], FileMode.Open));
            string line = SR.ReadLine();
            do
            {
                bool stop = false;
                if (line.Contains(">>"))
                {
                    string age = "", ve = "", po = "";
                    bool is_shatian = true;
                    var b = line.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                    if (b.Length == 3)
                    {
                        age = b[0];
                        ve = b[1];
                        po = b[2];
                    }
                    StringBuilder sb = new StringBuilder();
                    while (!(line = SR.ReadLine()).Contains(">>"))
                    {
                        if (SR.EndOfStream)
                        {
                            SR.Close();
                            i++;
                            if (i > 9)
                            {
                                stop = true;
                                break;
                            }
                            SR = new StreamReader(File.Open(files_array[i], FileMode.Open));
                        }
                        if (!line.Contains("------------------") && !line.Equals(string.Empty))
                        {
                            if (line.Contains("***") || line.Contains("..."))
                                is_shatian = false;
                            if (!line.Contains("رقم القصيدة : "))
                                sb.AppendLine(line);
                        }
                    }
                    if (stop)
                        break;
                    Versifier.AddPoem(ve, po, age, 0, sb.ToString(), is_shatian);
                }
                if (!stop && SR.EndOfStream)
                {
                    SR.Close();
                    i++;
                    if (i > 9)
                        break;
                    SR = new StreamReader(File.Open(files_array[i], FileMode.Open));
                }
            } while (true);

            var ii = Versifier.versifiers;

            ii.Sort((Versifier x, Versifier y) => { return x.GetStandartName().CompareTo(y.GetStandartName()); });
            int counter_p = 0;
            int counter_v = 0;

            foreach (var j in ii)
            {
                j.SetID(counter_v++);
                j.peoms.Sort((Poem x, Poem y) => { return x.GetStandartName().CompareTo(y.GetStandartName()); });
                foreach (var k in j.peoms)
                    k.SetID(counter_p++);
            }

            ////BinaryWriter BW = new BinaryWriter(File.Open(@"D:\Working Folder\Temp\Old Projects Temp\دواوين الشعر العربي على مر العصور\Saare.kkk", FileMode.OpenOrCreate));
            ////BW.BaseStream.Position = 100000;
            ////BW.Write(10);
            ////BW.Close();


            ////foreach (var j in ii)
            ////{
            ////    j.SetID(counter_v++);
            ////    j.peoms.Sort((Poem x, Poem y) => { return x.GetStandartName().CompareTo(y.GetStandartName()); });
            ////    foreach (var k in j.peoms)
            ////        k.SetID(counter_p++);
            ////}

            ////print result


            //BinaryFormatter BF = new BinaryFormatter();
            ////List<string> kk = new List<string>();
            ////foreach (var n in Versifier.versifiers)
            ////    kk.Add(n.GetName());
            //BF.Serialize(File.Open(@"C:\Users\Mohammad\Desktop\Project Temp\Temp\kkk.tmp", FileMode.OpenOrCreate), ii);


            BinaryFormatter BF = new BinaryFormatter();
            ////List<string> kk = new List<string>();
            ////foreach (var n in Versifier.versifiers)
            ////    kk.Add(n.GetName());
            Stream sr;
            var ghh = (List<Versifier>)BF.Deserialize(sr = File.Open(@"D:\Working Folder\Temp\Old Projects Temp\دواوين الشعر العربي على مر العصور\Poems - Copy.dat", FileMode.Open));
            sr.Close();

            ////BinaryWriter WriteBigFile = new BinaryWriter(File.Open(@"C:\Users\Mohammad\Desktop\Project Temp\Temp\Poems\File.Big", FileMode.OpenOrCreate));
            ////BinaryWriter WriteIndex = new BinaryWriter(File.Open(@"C:\Users\Mohammad\Desktop\Project Temp\Temp\Poems\index1.dat", FileMode.OpenOrCreate));
            ////BinaryWriter WriteIndexIndex = new BinaryWriter(File.Open(@"C:\Users\Mohammad\Desktop\Project Temp\Temp\Poems\index2.dat", FileMode.OpenOrCreate));
            //////BinaryWriter PasswordBW = new BinaryWriter(File.Open(@"C:\Users\Mohammad\Desktop\Project Temp\Temp\Poems\password.bin", FileMode.OpenOrCreate));

            //////Encryptor enc = new Encryptor();
            //////TripleDES trdes = new TripleDESCryptoServiceProvider();
            //////string pass = "C4df!+ -Tr.y%6?k; \"p\\OX0";
            //////byte[] key_arr = Encoding.UTF8.GetBytes(pass);
            //////trdes.Mode = CipherMode.CBC;
            //////trdes.Key = key_arr;
            //////PasswordBW.Write(pass);
            //////PasswordBW.Write(key_arr);
            //////PasswordBW.Close();

            ////foreach (var sheer in ghh)
            ////{
            ////    WriteIndexIndex.Write(sheer.id);
            ////    WriteIndexIndex.Write(byte.MaxValue);
            ////    WriteIndexIndex.Write(sheer.GetStandartName());
            ////    WriteIndexIndex.Write(byte.MaxValue);
            ////    WriteIndexIndex.Write(sheer.name);
            ////    WriteIndexIndex.Write(byte.MaxValue);
            ////    WriteIndexIndex.Write(sheer.age);
            ////    WriteIndexIndex.Write(byte.MaxValue);
            ////    WriteIndexIndex.Write(sheer.info);
            ////    WriteIndexIndex.Write(byte.MaxValue);
            ////    WriteIndexIndex.Write(sheer.peoms_count);
            ////    WriteIndexIndex.Write(byte.MaxValue);
            ////    WriteIndexIndex.Write(WriteBigFile.BaseStream.Position);
            ////    WriteIndexIndex.Write(byte.MaxValue);

            ////    foreach (var kaseda in sheer.peoms)
            ////    {
            ////        WriteIndex.Write(kaseda.id);
            ////        WriteIndex.Write(byte.MaxValue);
            ////        WriteIndex.Write(kaseda.GetStandartName());
            ////        WriteIndex.Write(byte.MaxValue);
            ////        WriteIndex.Write(kaseda.name);
            ////        WriteIndex.Write(byte.MaxValue);
            ////        WriteIndex.Write(kaseda.versifier.id);
            ////        WriteIndex.Write(byte.MaxValue);
            ////        WriteIndex.Write(kaseda.is_vertical);
            ////        WriteIndex.Write(byte.MaxValue);
            ////        WriteIndex.Write(WriteBigFile.BaseStream.Position);

            ////        WriteBigFile.Write(/*enc.Encrypt(trdes, */kaseda.poem)/*)*/;
            ////        WriteBigFile.Write(byte.MaxValue);
            ////    }
            ////}
            ////WriteBigFile.Close();
            ////WriteIndex.Close();
            ////WriteIndexIndex.Close();
            ////////////StreamWriter WritePoemsFile = new StreamWriter(File.OpenWrite(@"C:\Users\Mohammad\Desktop\Project Temp\Temp\poems.big"));
            ////////////StreamWriter WriteJson0 = new StreamWriter(File.OpenWrite(@"C:\Users\Mohammad\Desktop\Project Temp\Temp\index-0.json"));
            ////////////StreamWriter WriteJson1 = new StreamWriter(File.OpenWrite(@"C:\Users\Mohammad\Desktop\Project Temp\Temp\index-1.json"));
            ////////////StreamWriter WriteJson2 = new StreamWriter(File.OpenWrite(@"C:\Users\Mohammad\Desktop\Project Temp\Temp\index-2.json"));
            ////////////StreamWriter WriteJson3 = new StreamWriter(File.OpenWrite(@"C:\Users\Mohammad\Desktop\Project Temp\Temp\index-3.json"));
            ////////////StreamWriter WriteJson = WriteJson0;

            ////////////foreach (var sheer in ghh)
            ////////////{
            ////////////    if (sheer.id == 0)
            ////////////        WriteJson.WriteLine("{\"Versifiers\": [");
            ////////////   else if (sheer.id == 100)
            ////////////    {
            ////////////        WriteJson = WriteJson1;
            ////////////        WriteJson.WriteLine("{\"Versifiers\": [");
            ////////////    }
            ////////////    else if (sheer.id == 200)
            ////////////    {
            ////////////        WriteJson = WriteJson2;
            ////////////        WriteJson.WriteLine("{\"Versifiers\": [");
            ////////////    }
            ////////////    else if (sheer.id == 300)
            ////////////    {
            ////////////        WriteJson = WriteJson3;
            ////////////        WriteJson.WriteLine("{\"Versifiers\": [");
            ////////////    }
            ////////////    WriteJson.WriteLine("  {");
            ////////////    WriteJson.WriteLine(string.Format("    \"Age\": \"{0}\",", sheer.age.Replace("\"", "'")));
            ////////////    WriteJson.WriteLine(string.Format("    \"ContentCount\": \"{0}\",", sheer.peoms_count));
            ////////////    WriteJson.WriteLine(string.Format("    \"Id\": \"{0}\",", sheer.id));
            ////////////    WriteJson.WriteLine(string.Format("    \"Info\": \"{0}\",", sheer.info.Equals(string.Empty) ? "No info!" : sheer.info));
            ////////////    WriteJson.WriteLine(string.Format("    \"Name\": \"{0}\",", sheer.name.Replace("\"","'")));
            ////////////    WriteJson.WriteLine("    \"Poems\": ");
            ////////////    WriteJson.WriteLine("    [");
            ////////////    foreach (var kaseda in sheer.peoms)
            ////////////    {
            ////////////        WriteJson.WriteLine("      {");
            ////////////        WriteJson.WriteLine(string.Format("        \"From\": \"{0}\",", WritePoemsFile.BaseStream.Position));
            ////////////        WriteJson.WriteLine(string.Format("        \"Id\": \"{0}\",", kaseda.id));
            ////////////        WriteJson.WriteLine(string.Format("        \"IsVertical\": \"{0}\",", kaseda.is_vertical ? "true" : "false"));
            ////////////        WriteJson.WriteLine(string.Format("        \"Name\": \"{0}\",", kaseda.name.Replace("\"", "'")));
            ////////////        WritePoemsFile.Write(kaseda.poem);
            ////////////        WriteJson.WriteLine(string.Format("        \"To\": \"{0}\",", WritePoemsFile.BaseStream.Position));
            ////////////        WriteJson.WriteLine(string.Format("        \"VersifierId\": \"{0}\"", kaseda.versifier.id));
            ////////////        if (kaseda != kaseda.versifier.peoms[kaseda.versifier.peoms.Count - 1])
            ////////////            WriteJson.WriteLine("      },");
            ////////////        else
            ////////////            WriteJson.WriteLine("      }");
            ////////////    }
            ////////////    WriteJson.WriteLine("    ]");
            ////////////    if (sheer.id == 99 || sheer.id == 199 || sheer.id == 299 || sheer.id == 351)
            ////////////    {
            ////////////        WriteJson.WriteLine("  }");
            ////////////        WriteJson.WriteLine("]");
            ////////////        WriteJson.WriteLine("}");
            ////////////    }
            ////////////    else
            ////////////        WriteJson.WriteLine("  },");
            ////////////}

            ////////////WriteJson0.Close();
            ////////////WriteJson1.Close();
            ////////////WriteJson2.Close();
            ////////////WriteJson3.Close();
            ////////////WritePoemsFile.Close();


            //kaseda.poem = kaseda.poem.Replace("\r\n", "\n");
            //var poemay = kaseda.poem.Split('\n');
            //StringBuilder sb = new StringBuilder();
            //for (int i = 0; i < poemay.Length - 1; i++)
            //    sb.Append(poemay[i].Replace('\"', '\'') + "\\r\\n");
            //if (poemay.Length > 1)
            //    sb.Append(poemay[poemay.Length - 1].Replace('\"', ' '));

            //WriteJson.WriteLine(string.Format("        \"Poem\": \"{0}\"", sb.ToString()));
        }

        private void Read_Click(object sender, RoutedEventArgs e)
        {
            BinaryReader readbig = new BinaryReader(File.OpenRead(@"C:\Users\Mohammad\Desktop\Project Temp\Temp\Poems\File.Big"));
            string kas = "";
            long gg = 0;
            BinaryReader readindex= new BinaryReader(File.OpenRead(@"C:\Users\Mohammad\Desktop\Project Temp\Temp\Poems\index1.dat"));
            for (int i = 0; i < 80000; i++)
            {
                readindex.ReadString();
                readindex.ReadInt64();
                if (i == 15280)
                {
                    kas = readindex.ReadString();
                    gg = readindex.ReadInt64();
                    break;
                }
            }


            Decryptor dec = new Decryptor();
            TripleDES trdes = new TripleDESCryptoServiceProvider();
            string pass = "C4df!+ -Tr.y%6?k; \"p\\OX0";
            byte[] key_arr = Encoding.UTF8.GetBytes(pass);
            trdes.Mode = CipherMode.CBC;
            trdes.Key = key_arr;

            readbig.BaseStream.Position = gg;
            while (readbig.BaseStream.Position < readbig.BaseStream.Length)
            {
                string poim = readbig.ReadString();
                string poem = dec.Decrypt(trdes, poim);
                bool x = readbig.ReadBoolean();
            }
        }




        private void WriteEncryptedPoems(List<Versifier> obj, string file_path)
        {
            long start_index = CalcHeaderSize(obj);
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(file_path));
            bw.BaseStream.Position = start_index;

            string pass = "C4df!+ -Tr.y%6?k; \"p\\OX0";
            byte[] key = Encoding.Unicode.GetBytes(pass);

            for (int v = 0; v < obj.Count; v++)
            {
                for (int p = 0; p < obj[v].peoms.Count; p++)
                {
                    obj[v].peoms[p].start = bw.BaseStream.Position;
                    byte[] tmp;
                    bw.Write(tmp = EncrptText(obj[v].peoms[p].poem, key));
                    obj[v].peoms[p].length = tmp.Length;
                    bw.Write(0);
                }
            }
            WriteHeader(bw, obj, file_path, start_index);
            bw.Close();
        }

        private long CalcHeaderSize(List<Versifier> obj)
        {
            //header + file info + version + versifier count + first poem index;
            long size = 8 + 64 + sizeof(float) + 4 + 8;
            foreach (var i in obj)
            {
                //id + name size + age size + info size + name + age + info + poems cont;
                size += (4 + 2 + 2 + 4 + ((i.name.Length + i.age.Length + i.info.Length) * 2) + 4);
                foreach (var j in i.peoms)
                {
                    //id  + versifier id + name length + start + length + info (is _vertical) + name;
                    size += (4 + 4 + 2 + 8 + 8 + 1 + j.name.Length * 2);
                }
                size++;
            }
            size += 1024;
            return size;
        }

        private void WriteHeader(BinaryWriter bw, List<Versifier> obj, string file_path, long first_index)
        {
            bw.BaseStream.Position = 0x00;
            byte[] header = { 0xAC, 0x4D, 0x0F, 0x14, 0xF2, 0x82, 0x7C, 0xBB };
            bw.Write(header);
            //string text = "Arabic poems encrypted pack, version 1.0, by Mohammad Diab.";
            byte[] file_info = { 0x41, 0x72, 0x61, 0x62, 0x69, 0x63, 0x20, 0x70, 0x6F, 0x65, 0x6D, 0x73, 0x20, 0x65, 0x6E, 0x63, 0x72, 0x79,
                0x70, 0x74, 0x65, 0x64, 0x20, 0x70, 0x61, 0x63, 0x6B, 0x2C, 0x20, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x20, 0x31, 0x2E,
                0x30, 0x2C, 0x20, 0x62, 0x79, 0x20, 0x4D, 0x6F, 0x68, 0x61, 0x6D, 0x6D, 0x61, 0x64, 0x20, 0x44, 0x69, 0x61, 0x62, 0x2E,0x00,
                0x00, 0x00, 0x00, 0x00 };
            bw.Write(file_info);
            bw.Write((float)1.0);
            bw.Write(first_index);
            bw.Write(obj.Count);
            foreach (var v in obj)
            {
                bw.Write(v.id);
                bw.Write((short)v.name.Length);
                bw.Write((short)v.age.Length);
                bw.Write(v.info.Length);
                bw.Write(Encoding.Unicode.GetBytes(v.name));
                bw.Write(Encoding.Unicode.GetBytes(v.age));
                bw.Write(Encoding.Unicode.GetBytes(v.info));
                bw.Write(v.peoms.Count);

                foreach (var p in v.peoms)
                {
                    bw.Write(p.id);
                    bw.Write(obj.IndexOf(v));
                    bw.Write((short)p.name.Length);
                    bw.Write(p.start);
                    bw.Write(p.length);
                    bw.Write(p.is_vertical ? (byte)0x00 : (byte)0x01);
                    bw.Write(Encoding.Unicode.GetBytes(p.name));
                }

                bw.Write(byte.MinValue);
            }
        }



        private byte[] EncrptText(string text, byte[] password)
        {
            byte[] bytes = new byte[text.Length * sizeof(char)];
            Buffer.BlockCopy(text.ToCharArray(), 0, bytes, 0, text.Length * sizeof(char));

            return EncryptBytes(bytes, password);
        }

        private string DecrptText(byte[] bytes, byte[] password)
        {
            var decrypt_bytes = DecryptBytes(bytes, password);
            return Encoding.Unicode.GetString(decrypt_bytes);
        }


        private byte[] EncryptBytes(byte[] word, byte[] key)
        {
            byte[] encrypted_bytes = new byte[word.Length];
            Parallel.For(0, word.Length, (i) =>
            {
                encrypted_bytes[i] = (byte)(word[i] ^ key[i % key.Length]);
                encrypted_bytes[i]--;
            });
            return encrypted_bytes;
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


        private void WriteEncFile_Click(object sender, RoutedEventArgs e)
        {

            #region Read Files

            var files_array = Directory.GetFiles(@"D:\Working Folder\Temp\Old Projects Temp\دواوين الشعر العربي على مر العصور\Saare");
            string[] splitter = { ">>" };
            int i = 0;
            StreamReader SR = new StreamReader(File.Open(files_array[i], FileMode.Open));
            string line = SR.ReadLine();
            do
            {
                bool stop = false;
                if (line.Contains(">>"))
                {
                    string age = "", ve = "", po = "";
                    bool is_shatian = true;
                    var b = line.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                    if (b.Length == 3)
                    {
                        age = b[0];
                        ve = b[1];
                        po = b[2];
                    }
                    StringBuilder sb = new StringBuilder();
                    while (!(line = SR.ReadLine()).Contains(">>"))
                    {
                        if (SR.EndOfStream)
                        {
                            SR.Close();
                            i++;
                            if (i > 9)
                            {
                                stop = true;
                                break;
                            }
                            SR = new StreamReader(File.Open(files_array[i], FileMode.Open));
                        }
                        if (!line.Contains("------------------") && !line.Equals(string.Empty))
                        {
                            if (line.Contains("***") || line.Contains("..."))
                                is_shatian = false;
                            if (!line.Contains("رقم القصيدة : "))
                                sb.AppendLine(line);
                        }
                    }
                    if (stop)
                        break;
                    Versifier.AddPoem(ve, po, age, 0, sb.ToString(), !is_shatian);
                }
                if (!stop && SR.EndOfStream)
                {
                    SR.Close();
                    i++;
                    if (i > 9)
                        break;
                    SR = new StreamReader(File.Open(files_array[i], FileMode.Open));
                }
            } while (true);

            var ii = Versifier.versifiers;

            ii.Sort((Versifier x, Versifier y) => { return x.GetStandartName().CompareTo(y.GetStandartName()); });
            int counter_p = 0;
            int counter_v = 0;

            foreach (var j in ii)
            {
                j.SetID(counter_v++);
                j.peoms.Sort((Poem x, Poem y) => { return x.GetStandartName().CompareTo(y.GetStandartName()); });
                foreach (var k in j.peoms)
                    k.SetID(counter_p++);
                j.peoms_count = j.peoms.Count;
            }
            #endregion

            #region Encrypt And Save File

            WriteEncryptedPoems(ii, file_paths.Text);

            #endregion
        }


        private void Test_Click(object sender, RoutedEventArgs e)
        {
            string pass = "C4df!+ -Tr.y%6?k; \"p\\OX0";
            byte[] key = Encoding.Unicode.GetBytes(pass);
            var enc = EncrptText(text.Text, key);
            textBlock.Text = DecrptText(enc, key);
            textBlock1.Text = Encoding.Unicode.GetString(enc);
        }
    }
    [Serializable]
    class Versifier
    {
        public static List<Versifier> versifiers = new List<Versifier>();
        public string info;
        public string name;
        public int id;
        public string age;
        public int peoms_count;
        internal List<Poem> peoms;

        public Versifier(string n, int i, string a, string inf)
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

        static public void AddPoem(string ver, string p_n, string ag, int id, string p,bool is_shatean)
        {
            int ind = IndexOf(ver);
            if (ind < 0)
            {
                Versifier v = new Versifier(ver, 0, ag, "");
                v.peoms.Add(new Poem(p_n, 0, v, p, is_shatean));
            }
            else
                versifiers[ind].peoms.Add(new Poem(p_n, 0, versifiers[ind], p, is_shatean));
        }

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
    class Poem
    {
        public string name;
        public int id;
        public bool is_vertical;
        public Versifier versifier;
        public string poem;
        public long start, length;

        public Poem(string n, int i, Versifier v, string sr, bool is_shatrean)
        {
            name = n;
            id = i;
            versifier = v;
            poem = sr;
            is_vertical = !is_shatrean;
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

    };

    public class Encryptor
    {
        public Encryptor() { }

        public string Encrypt(TripleDES des, string MainString)
        {
            MemoryStream memory = new MemoryStream();
            CryptoStream stream = new CryptoStream(memory, des.CreateEncryptor(), CryptoStreamMode.Write);
            StreamWriter streamwriter = new StreamWriter(stream);
            streamwriter.WriteLine(MainString);
            streamwriter.Close();
            stream.Close();
            byte[] buffer = memory.ToArray();
            memory.Close();
            return Convert.ToBase64String(buffer);
        }
    }

    public class Decryptor
    {
        public Decryptor() { }

        public string Decrypt(TripleDES des, string MainString)
        {
            byte[] buffer = Convert.FromBase64String(MainString.Trim());
            MemoryStream ms = new MemoryStream(buffer);
            CryptoStream encStream = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(encStream);
            string val = sr.ReadToEnd();
            sr.Close();
            encStream.Close();
            ms.Close();
            return val;
        }
    }
}
