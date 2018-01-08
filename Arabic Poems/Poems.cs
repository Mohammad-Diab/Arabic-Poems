using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Arabic_Poems
{
    class Poems
    {
    }

    internal static class Functions
    {
        internal static string GetAgeName(Age id)
        {
            switch (id)
            {
                case Age.Abbasi:
                    return "العصر العباسي";
                case Age.Andalus:
                    return "عصر الأندلس";
                case Age.Ayoubi:
                    return "العصر الأيوبي";
                case Age.EarlyIslam:
                    return "عصر المخضرمين";
                case Age.Islam:
                    return "العصر الاسلامي";
                case Age.Jahili:
                    return "العصر الجاهلي";
                case Age.Mamluki:
                    return "عصر المماليك";
                case Age.Modern:
                    return "العصر الحديث";
                case Age.Ottoman:
                    return "العصر العثماني";
                case Age.Umayyad:
                    return "العصر الأموي";
                case Age.All:
                    return "كل الشعراء";
                default:
                    return "عصر غير معروف";
            }
        }

        internal static string GetCountryName(Country id)
        {
            switch (id)
            {
                case Country.Algeria:
                    return "الجزائر";
                case Country.Bahrain:
                    return "البحرين";
                case Country.Egypt:
                    return "مصر";
                case Country.Iraq:
                    return "العراق";
                case Country.Jordan:
                    return "الأردن";
                case Country.Kuwait:
                    return "الكويت";
                case Country.Lebanon:
                    return "لبنان";
                case Country.Libya:
                    return "ليبيا";
                case Country.Mauritania:
                    return "الجزائر";
                case Country.Morocco:
                    return "المغرب";
                case Country.Oman:
                    return "عمان";
                case Country.Palestine:
                    return "فلسطين";
                case Country.Qatar:
                    return "قطر";
                case Country.Saudi:
                    return "السعودية";
                case Country.Sudan:
                    return "السودان";
                case Country.Syria:
                    return "الجزائر";
                case Country.Tunisia:
                    return "تونس";
                case Country.UAE:
                    return "الامارات";
                case Country.Yemen:
                    return "اليمن";
                case Country.All:
                    return "كل الشعراء";
                default:
                    return "دولة غير محددة";
            }
        }

        internal static string GetBaharName(Bahar id)
        {
            switch (id)
            {
                case Bahar.Mutadarik:
                    return "بحر المتدارك";
                case Bahar.Tawil:
                    return "البحر الطويل";
                case Bahar.Wafir:
                    return "البحر الوافر";
                case Bahar.Basit:
                    return "البحر البسيط";
                case Bahar.Kamil:
                    return "البحر الكامل";
                case Bahar.Rijz:
                    return "بحر الرجز";
                case Bahar.Raml:
                    return "بحر الرمل";
                case Bahar.Sarie:
                    return "البحر السريع";
                case Bahar.Munsarih:
                    return "البحر المنسرح";
                case Bahar.Khafif:
                    return "البحر الخفيف";
                case Bahar.Mujtath:
                    return "بحر المجتث";
                case Bahar.Khabab:
                    return "بحر الخبب";
                default:
                    return "دول غير محددة";
            }
        }

        internal static string Poet(int Count)
        {
            if (Count < 1)
                return "لا يوجد شعراء";
            else if (Count == 1)
                return "شاعر واحد";
            else if (Count == 2)
                return "شاعران";
            else if (Count < 11)
                return Count + " شعراء";
            else
                return Count + " شاعراً";
        }

        internal static string Poem(int Count)
        {
            if (Count < 1)
                return "لا يوجد قصائد";
            else if (Count == 1)
                return "قصيدة واحدة";
            else if (Count == 2)
                return "قصيدتان";
            else if (Count < 11)
                return Count + " قصائد";
            else
                return Count + " " + "قصيدة";
        }
    }

    internal class Versifier
    {
        internal static List<Versifier> VersifiersList;
        public string info;
        public string name;
        public int id;
        public string age;
        public int peoms_count;
        public Age poet_age;
        public Country poet_country;
        static Random rand = new Random(DateTime.Now.Millisecond);

        internal List<Poem> Poems;

        public Versifier(string n, int i, string a, string inf)
        {
            name = n;
            id = i;
            age = a;
            peoms_count = 0;
            info = inf;
            Poems = new List<Poem>();
            poet_age = (Age)rand.Next(1, 11);
            poet_country = (Country)rand.Next(1, 20);
            VersifiersList.Add(this);
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

    internal class Poem
    {
        public string name;
        public int id;
        public bool is_vertical;
        public Versifier versifier;
        public string poem;
        public Bahar poem_bahar;
        public long start, length;
        static Random rand = new Random(DateTime.Now.Millisecond);

        public Poem(string n, int i, Versifier v, string sr, bool is_vertical, long start, long length)
        {
            name = n;
            id = i;
            versifier = v;
            poem = sr;
            this.start = start;
            this.length = length;
            this.is_vertical = is_vertical;
            poem_bahar = (Bahar)rand.Next(1, 13);
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

    internal class ListItemTemplate
    {
        string name;
        public string Name { get => name; set => name = value; }

        string description;
        public string Description { get => description; set => description = value; }

        string tag;
        public string Tag { get => tag; set => tag = value; }

        public ListItemTemplate(string name, string description, string tag)
        {
            Name = name;
            Description = description;
            Tag = tag;
        }
    }

    internal class PoemVerse : INotifyPropertyChanged
    {
        int id;

        string verse_id;
        public string VerseID { get => verse_id; set => verse_id = value; }

        string lint1;
        public string Line1 { get => lint1; set => lint1 = value; }

        string lint2;
        public string Line2 { get => lint2; set => lint2 = value; }

        Visibility line2_visibility;
        Visibility Line2Visibility
        {
            get => line2_visibility;
            set
            {
                line2_visibility = value;
                OnPropertyChanged("Line2Visibility");
            }
        }

        public PoemVerse(int ID, string FirstLine, string SecontLine = "")
        {
            id = ID;
            VerseID = id.ToString();
            Line1 = FirstLine;
            Line2 = SecontLine;
            Line2Visibility = string.IsNullOrWhiteSpace(SecontLine) ? Visibility.Collapsed : Visibility.Visible;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Serializable]
    internal class LastPoem : IComparable
    {
        public static List<LastPoem> poems_list = new List<LastPoem>(50);
        DateTime time;
        string tag;
        long poem_tag;
        long ticks;

        public DateTime Time { get => time; set => time = value; }
        public string Tag { get => tag; set => tag = value; }
        public long PoemTag { get => poem_tag; set => poem_tag = value; }
        public long Ticks { get => ticks; set => ticks = value; }

        private LastPoem(string tag)
        {
            Tag = tag;
            Time = DateTime.Now;
            Ticks = Time.Ticks;

            long tmp;
            long.TryParse(Tag.Substring(1), out tmp);
            PoemTag = tmp % 100000000;
        }

        
        public static void AddPoem(string tag)
        {
            var item = new LastPoem(tag);
            var index = poems_list.IndexOf(item);
            if (index < 0)
            {
                if (poems_list.Count == poems_list.Capacity)
                    poems_list.RemoveAt(0);
                poems_list.Add(item);
            }
            else
                poems_list[index].UpdateTime();
            poems_list.Sort();
            SaveSetting();
        }

        private void UpdateTime()
        {
            Time = DateTime.Now;
            Ticks = Time.Ticks;
        }

        public int CompareTo(object obj)
        {
            return -Ticks.CompareTo((obj as LastPoem).Ticks);
        }

        public override bool Equals(object obj)
        {
            var poem = obj as LastPoem;
            return poem != null && PoemTag == poem.PoemTag;
        }

        public override int GetHashCode()
        {
            return 924669270 + PoemTag.GetHashCode();
        }

        public async static Task<bool> LoadSetting()
        {
            var dcs = new System.Runtime.Serialization.DataContractSerializer(typeof(List<LastPoem>));
            StorageFolder sf = ApplicationData.Current.LocalFolder;
            if (File.Exists(Path.Combine(sf.Path, "last_poem")))
            {
                try
                {
                    StorageFile file = await StorageFile.GetFileFromPathAsync(Path.Combine(sf.Path, "last_poem"));
                    Stream st = await file.OpenStreamForWriteAsync();
                    poems_list = (List<LastPoem>)dcs.ReadObject(st);
                    st.Dispose();
                    poems_list.Capacity = 50;
                }
                catch (Exception)
                {
                    poems_list = new List<LastPoem>(50);
                }
            }
            else
                poems_list = new List<LastPoem>(50);
            return true;
        }

        private async static void SaveSetting()
        {
            var dcs = new System.Runtime.Serialization.DataContractSerializer(typeof(List<LastPoem>));
            try
            {
                StorageFolder sf = ApplicationData.Current.LocalFolder;
                var file = await sf.CreateFileAsync("last_poem", CreationCollisionOption.ReplaceExisting);
                Stream st = await file.OpenStreamForWriteAsync();
                dcs.WriteObject(st, poems_list);
                st.Dispose();
            }
            catch (Exception) { }
        }
    }

    internal enum Country
    {
        NotSpecefic = 0,
        Jordan,
        UAE,
        Bahrain,
        Tunisia,
        Algeria,
        Saudi,
        Sudan,
        Syria,
        Iraq,
        Oman,
        Palestine,
        Qatar,
        Kuwait,
        Lebanon,
        Libya,
        Egypt,
        Morocco,
        Mauritania,
        Yemen,
        None = 99,
        All = 100
    }

    internal enum Age
    {
        NotSpecefic = 0,
        Jahili,
        EarlyIslam,
        Islam,
        Umayyad,
        Andalus,
        Abbasi,
        Ayoubi,
        Mamluki,
        Ottoman,
        Modern,
        None = 99,
        All = 100
    }

    internal enum Bahar
    {
        NotSpecefic = 0,
        Mutadarik,
        Tawil,
        Wafir,
        Basit,
        Kamil,
        Rijz,
        Raml,
        Sarie,
        Munsarih,
        Khafif,
        Mujtath,
        Khabab,
        None = 99,
        All = 100
    }


}
