using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
                case Country.IraqAndSham:
                    return "العراق والشام";
                case Country.ArabianPeninsula:
                    return "الجزيرة العربية";
                case Country.EgyptAndSudan:
                    return "مصر والسودان";
                case Country.Maghreb:
                    return "المغرب العربي";
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
                case Bahar.Mutaqarib:
                    return "البحر المتقارب";
                case Bahar.Madid:
                    return "البحر المديد";
                case Bahar.Hazaj:
                    return "بحر الهزج";
                case Bahar.Mudari:
                    return "البحر المضارع";
                case Bahar.Muqtadab:
                    return "البحر المقتضب";
                case Bahar.Multiple:
                    return "متعدد البحور";
                case Bahar.FreeVerse:
                    return "شعر التفعيلة / الحر";
                case Bahar.Prose:
                    return "قصيدة نثرية";
                case Bahar.Muwashshah:
                    return "الموشحات";
                case Bahar.Dubait:
                    return "الدوبيت";
                case Bahar.Mawaliya:
                    return "المواليا";
                default:
                    return "البحر غير معروف";
            }
        }

        internal static readonly Bahar[] BaharOrder = { Bahar.Tawil, Bahar.Basit, Bahar.Kamil, Bahar.Wafir, Bahar.Rijz, Bahar.Raml, Bahar.Khafif, Bahar.Sarie, Bahar.Mutaqarib, Bahar.Munsarih, Bahar.Mujtath, Bahar.Mutadarik, Bahar.Madid, Bahar.Hazaj, Bahar.Mudari, Bahar.Muqtadab, Bahar.Multiple, Bahar.FreeVerse, Bahar.Prose, Bahar.Muwashshah, Bahar.Dubait, Bahar.Mawaliya };

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

        internal static string Result(int Count)
        {
            if (Count < 1)
                return "لا توجد نتائج";
            else if (Count == 1)
                return "نتيجة واحدة";
            else if (Count == 2)
                return "نتيجتان";
            else if (Count < 11)
                return Count + " نتائج";
            else
                return Count + " نتيجة";
        }

        internal static string FixAge(string age)
        {
            age = age.Trim();
            if (age.StartsWith("ديو"))
                age = age.Substring(3);
            return age;
        }

        internal static Age GetAge(string age)
        {
            if (age.Contains("الجاهلي"))
                return Age.Jahili;
            else if (age.Contains("الإسلامي") || age.Contains("الاسلامي"))
                return Age.Islam;
            else if (age.Contains("العباسي"))
                return Age.Abbasi;
            else if (age.Contains("الأندلسي") || age.Contains("الاندلسي"))
                return Age.Andalus;
            else if (age.StartsWith("شعراء"))
                return Age.Modern;
            return Age.NotSpecefic;
        }

        internal static Country GetRegion(string age)
        {
            if (age.Contains("العراق"))
                return Country.IraqAndSham;
            else if (age.Contains("الجزيرة"))
                return Country.ArabianPeninsula;
            else if (age.Contains("مصر"))
                return Country.EgyptAndSudan;
            else if (age.Contains("المغرب"))
                return Country.Maghreb;
            return Country.NotSpecefic;
        }

        internal const string Accents = "ًٌٍَُِّْٰـ";

        internal static string RemoveAccents(string str)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            foreach (char ch in str)
                if (Accents.IndexOf(ch) < 0)
                    sb.Append(OptimizeChars(ch));
            return sb.ToString();
        }

        internal static char OptimizeChars(char ch)
        {
            switch (ch)
            {
                case 'أ':
                case 'إ':
                case 'آ':
                case 'ٱ':
                    return 'ا';
                case 'ى':
                case 'ئ':
                    return 'ي';
                case 'ؤ':
                    return 'و';
                case 'ة':
                    return 'ه';
                default:
                    return ch;
            }
        }

        static Random rand = new Random(DateTime.Now.Millisecond);

        internal static string RandomPoemTag()
        {
            int v = rand.Next(Versifier.VersifiersList.Count);
            int p = rand.Next(Versifier.VersifiersList[v].Poems.Count);
            return "n" + (v * 10000 + p);
        }

        internal static void Navigate(Type page, object parameter)
        {
            var main = (Window.Current.Content as Frame).Content as MainPage;
            if (main != null && Versifier.VersifiersList != null)
                main.rootFram.Navigate(page, parameter);
        }

        internal static async void SelectPoem()
        {
            if (Versifier.VersifiersList == null)
                return;
            var dialog = new Pages.SelectPoem();
            await dialog.ShowAsync();
            if (dialog.selected_tag != null)
                Navigate(typeof(Pages.view), dialog.selected_tag);
        }

        internal static string PoemText(Poem poem)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(poem.name.Trim());
            sb.AppendLine(poem.versifier.name.Trim());
            sb.AppendLine();
            string[] splitter = { "\r\n" };
            var lines = poem.poem.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < lines.Length; i++)
            {
                if (!poem.is_vertical && i + 1 < lines.Length)
                {
                    sb.AppendLine(lines[i] + "     " + lines[i + 1]);
                    i++;
                }
                else
                    sb.AppendLine(lines[i]);
            }
            return sb.ToString();
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

        internal List<Poem> Poems;

        public Versifier(string n, int i, string a, string inf)
        {
            name = n;
            id = i;
            age = Functions.FixAge(a);
            peoms_count = 0;
            info = inf;
            Poems = new List<Poem>();
            poet_age = Functions.GetAge(age);
            poet_country = Functions.GetRegion(age);
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
        internal string search_name;

        public Poem(string n, int i, Versifier v, string sr, bool is_vertical, long start, long length)
        {
            name = n;
            id = i;
            versifier = v;
            poem = sr;
            this.start = start;
            this.length = length;
            this.is_vertical = is_vertical;
            poem_bahar = Bahar.NotSpecefic;
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
                byte[] poem_encrypted_bytes = new byte[1];
                try
                {
                    lock (br)
                    {
                        br.BaseStream.Position = start;
                        poem_encrypted_bytes = br.ReadBytes(Convert.ToInt32(length));
                    }
                }
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
                byte[] poem_encrypted_bytes = new byte[1];
                try
                {
                    lock (br)
                    {
                        br.BaseStream.Position = start;
                        poem_encrypted_bytes = br.ReadBytes(Convert.ToInt32(length));
                    }
                }
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

        public double FontSize { get => Settings.FontSize; }

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

    internal static class Settings
    {
        internal static double FontSize
        {
            get
            {
                object value = ApplicationData.Current.LocalSettings.Values["font_size"];
                return value is double ? (double)value : 28;
            }
            set { ApplicationData.Current.LocalSettings.Values["font_size"] = value; }
        }
    }

    internal static class Favourites
    {
        internal static List<Poem> Poems = new List<Poem>();

        internal static bool Contains(Poem poem)
        {
            return Poems.Contains(poem);
        }

        internal static async Task Toggle(Poem poem)
        {
            if (Poems.Contains(poem))
                Poems.Remove(poem);
            else
                Poems.Add(poem);
            await Save();
        }

        internal static async Task Load()
        {
            Poems = new List<Poem>();
            try
            {
                var file = await ApplicationData.Current.LocalFolder.TryGetItemAsync("favourites") as StorageFile;
                if (file == null)
                    return;
                foreach (var line in await FileIO.ReadLinesAsync(file))
                {
                    var parts = line.Split('\t');
                    if (parts.Length < 3)
                        continue;
                    var poem = Find(parts[0], parts[1], parts[2]);
                    if (poem != null && !Poems.Contains(poem))
                        Poems.Add(poem);
                }
            }
            catch (Exception) { }
        }

        internal static async Task Save()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("favourites", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteLinesAsync(file, Poems.Select(p => p.versifier.name + "\t" + p.name + "\t" + p.versifier.Poems.IndexOf(p)).ToList());
            }
            catch (Exception) { }
        }

        internal static string GetTag(Poem poem)
        {
            return "n" + (Versifier.VersifiersList.IndexOf(poem.versifier) * 10000 + poem.versifier.Poems.IndexOf(poem));
        }

        private static Poem Find(string versifier_name, string poem_name, string index)
        {
            if (Versifier.VersifiersList == null)
                return null;
            int poem_index;
            int.TryParse(index, out poem_index);
            foreach (var v in Versifier.VersifiersList)
                if (v.name == versifier_name)
                {
                    if (poem_index > -1 && poem_index < v.Poems.Count && v.Poems[poem_index].name == poem_name)
                        return v.Poems[poem_index];
                    foreach (var p in v.Poems)
                        if (p.name == poem_name)
                            return p;
                }
            return null;
        }
    }

    public class NameWithIndex
    {
        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        int index;
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        public NameWithIndex(string name, int index)
        {
            Name = name;
            Index = index;
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
        IraqAndSham = 21,
        ArabianPeninsula,
        EgyptAndSudan,
        Maghreb,
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
        Mutaqarib,
        Madid,
        Hazaj,
        Mudari,
        Muqtadab,
        Multiple = 20,
        FreeVerse,
        Prose,
        Muwashshah,
        Dubait,
        Mawaliya,
        None = 99,
        All = 100
    }


}