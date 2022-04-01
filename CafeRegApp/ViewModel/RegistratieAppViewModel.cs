using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Printing;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CafeRegApp
{
    public class RegistratieAppViewModel : BaseViewModel
    {
        //private fields
        #region private stuff
        private string _directory;
        private int _hoeveelDagenBewaren;
        private string _wachtwoord;
        private string _inputwachtwoord;
        private string _naam;
        private string _telnr;
        private string _zoekenopnaam;
        private string _zoekResultaat;
        private string _encryptedWachtwoord;
        private Visibility _visibilityInvoer ;
        private Visibility _visibilityMain ;
        private Visibility _visibilityZoeken;
        private Visibility _visibilityInstellingen;
        private bool _zelfcorona;
        private bool _gezincorona;
        private bool _contactmetanderecorona;
        private bool _zelfandereziekte;
        private bool _gezinandereziekte;
        private ICommand _displayInvoerCommand;
        private ICommand _displayMainCommand;
        private ICommand _displayZoekenCommand;
        private ICommand _displayInstellingenCommand;
        private ICommand _saveDataCommand;
        private ICommand _saveIniCommand;
        private ICommand _zoekNaaminFiles;
        private ICommand _openDataMap;
        private bool canExecute = true;
        private const String strPermutation = "ouiveyxaqtd";
        private const Int32 bytePermutation1 = 0x19;
        private const Int32 bytePermutation2 = 0x59;
        private const Int32 bytePermutation3 = 0x17;
        private const Int32 bytePermutation4 = 0x41;
        #endregion

        //public properties 
        #region properties
        public bool ZelfCorona { get { return _zelfcorona; } set { _zelfcorona = value; OnPropertyChanged("ZelfCorona"); } }
        public bool GezinCorona { get { return _gezincorona; } set { _gezincorona = value; OnPropertyChanged("GezinCorona"); } }
        public bool ContactmetAndereCorona { get { return _contactmetanderecorona; } set { _contactmetanderecorona = value; OnPropertyChanged("ContactmetAndereCorona"); } }
        public bool ZelfAndereZiekte { get { return _zelfandereziekte; } set { _zelfandereziekte = value; OnPropertyChanged("ZelfAndereZiekte"); } }
        public bool GezinAndereZiekte { get { return _gezinandereziekte; } set { _gezinandereziekte = value; OnPropertyChanged("GezinAndereZiekte"); } }
        public SecureString SecurePassword { private get; set; }
        public string ZoekResultaat
        {
            get { return _zoekResultaat; }
            set
            {
                _zoekResultaat = value;
                OnPropertyChanged("ZoekResultaat");
            }
        }

        public string Wachtwoord
        {
            get { return _wachtwoord; }
            set
            {
                _wachtwoord = value;
                OnPropertyChanged("Wachtwoord");
            }
        }
        public string InputWachtwoord
        {
            get { return _inputwachtwoord; }
            set
            {
                _inputwachtwoord = value;
                OnPropertyChanged("InputWachtwoord");
            }
        }
        public string Naam
        {
            get { return _naam; }
            set { _naam = value;
                OnPropertyChanged("Naam");
            }
        }
        public int  HoeveelDagenBewaren
        {
            get { return _hoeveelDagenBewaren;}
            set { _hoeveelDagenBewaren = value;
                OnPropertyChanged("HoeveelDagenBewaren");
            }
        }
        public string ZoekenOpNaam
        {
            get { return _zoekenopnaam; }
            set { _zoekenopnaam = value;
                OnPropertyChanged("ZoekenOpNaam");
            }
        }
        public string TelefoonNummer
        {
            get { return _telnr;}
            set {_telnr = value;
                OnPropertyChanged("TelefoonNummer");
            }
        }
        public bool CanExecute { 
            get {  return this.canExecute;
            }  
            set {  if (this.canExecute == value)
                {
                    return;
                }
                this.canExecute = value;
            }
        }
        public Visibility SpVisibilityInvoer
        {
            get { return _visibilityInvoer; }
            set {
                if (!string.Equals(_visibilityInvoer, value))
                {
                    _visibilityInvoer = value;
                    OnPropertyChanged("SpVisibilityInvoer");
                }
            }
        }
        public Visibility SpVisibilityMain
        {
            get { return _visibilityMain; }
            set {
                if (!string.Equals(_visibilityMain, value))
                {
                    _visibilityMain = value;
                    OnPropertyChanged("SpVisibilityMain");
                }
            }
        }
        public Visibility SpVisibilityZoeken
        {
            get { return _visibilityZoeken; }
            set {
                if (!string.Equals(_visibilityZoeken, value))
                {
                    _visibilityZoeken = value;
                    OnPropertyChanged("SpVisibilityZoeken");
                }
            }
        }
        public Visibility SpVisibilityInstellingen
        {
            get { return _visibilityInstellingen; }
            set
            {
                if (!string.Equals(_visibilityInstellingen, value))
                {
                    _visibilityInstellingen = value;
                    OnPropertyChanged("SpVisibilityInstellingen");
                }
            }
        }
        #endregion
        // commands
        #region command interfaces
        public ICommand SaveDataCommand { get { return _saveDataCommand; } set { _saveDataCommand = value; } }
        public ICommand DisplayInvoerCommand { get { return _displayInvoerCommand; } set { _displayInvoerCommand = value; } }
        public ICommand DisplayZoekenCommand { get { return _displayZoekenCommand; } set { _displayZoekenCommand = value; } }
        public ICommand DisplayInstellingenCommand { get { return _displayInstellingenCommand; } set { _displayInstellingenCommand = value; } }
        public ICommand DisplayMainCommand { get { return _displayMainCommand; } set { _displayMainCommand = value; } }
        public ICommand SaveIniCommand { get { return _saveIniCommand; } set { _saveIniCommand = value; } }
        public ICommand ZoekNaamInFilesCommand { get { return _zoekNaaminFiles; }  set { _zoekNaaminFiles = value; } }
        public ICommand OpenDataMapCommand { get { return _openDataMap; } set { _openDataMap = value; } }

        #endregion
        // constructor
        #region constructor
        public RegistratieAppViewModel(Window window)
        {
            /* window events */
            window.Loaded += (sender, e) =>
            {
                CreateIni();
                LoadIni();
                DeleteOldFiles();
            };

            window.Closed += (sender, e) =>
            {
            };
            _visibilityInstellingen = Visibility.Collapsed;
            _visibilityInvoer = Visibility.Collapsed;
            _visibilityZoeken = Visibility.Collapsed;
            _directory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\CorRegA"); 
            SaveDataCommand = new RelayCommand(SaveData, param => this.canExecute);
            DisplayInvoerCommand = new RelayCommand(DisplayInvoer, param =>this.canExecute);
            DisplayZoekenCommand = new RelayCommand(DisplayZoeken, param => this.CanExecute);
            DisplayInstellingenCommand = new RelayCommand(DisplayInstellingen, param => this.CanExecute);
            DisplayMainCommand = new RelayCommand(DisplayMain, param => this.canExecute);
            SaveIniCommand = new RelayCommand(SaveIni, param => this.canExecute);
            ZoekNaamInFilesCommand = new RelayCommand(ZoekNaamInFiles, param => this.CanExecute);
            OpenDataMapCommand = new RelayCommand(OpenDataMap, param => this.canExecute);
        }
        #endregion
        //private methods
        #region private methods
        private  string Encrypt(string strData)
        {

            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(strData)));
            // reference https://msdn.microsoft.com/en-us/library/ds4kkd55(v=vs.110).aspx

        }


        // decoding
        private  string Decrypt(string strData)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(strData)));
            // reference https://msdn.microsoft.com/en-us/library/system.convert.frombase64string(v=vs.110).aspx

        }

        // encrypt
        private  byte[] Encrypt(byte[] strData)
        {
            PasswordDeriveBytes passbytes =
            new PasswordDeriveBytes(strPermutation,
            new byte[] { bytePermutation1,
                         bytePermutation2,
                         bytePermutation3,
                         bytePermutation4
            });

            MemoryStream memstream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passbytes.GetBytes(aes.KeySize / 8);
            aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

            CryptoStream cryptostream = new CryptoStream(memstream,
            aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptostream.Write(strData, 0, strData.Length);
            cryptostream.Close();
            return memstream.ToArray();
        }

        // decrypt
        private  byte[] Decrypt(byte[] strData)
        {
            PasswordDeriveBytes passbytes =
            new PasswordDeriveBytes(strPermutation,
            new byte[] { bytePermutation1,
                         bytePermutation2,
                         bytePermutation3,
                         bytePermutation4
            });

            MemoryStream memstream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passbytes.GetBytes(aes.KeySize / 8);
            aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

            CryptoStream cryptostream = new CryptoStream(memstream,
            aes.CreateDecryptor(), CryptoStreamMode.Write);
            cryptostream.Write(strData, 0, strData.Length);
            cryptostream.Close();
            return memstream.ToArray();
        }

        private void OpenDataMap(object obj)
        {
            //string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\CorRegA\data");
            string path = _directory + @"\data";
            var psi = new System.Diagnostics.ProcessStartInfo() { FileName = path, UseShellExecute = true };
            try
            {
                System.Diagnostics.Process.Start(psi);
            }
            catch
            {
                MessageBox.Show("Nog geen data ingevoerd.");
            }
        }

        private void ZoekNaamInFiles(object obj)
        {
            string path = _directory + @"\data";
            string[] allfiles = Directory.GetFiles(path, "", SearchOption.AllDirectories);
            StreamReader filereader;
            StringBuilder searchresult = new StringBuilder(); string line;
            foreach (string file in allfiles) 
            {
                filereader = new StreamReader(file);
                while ((line = filereader.ReadLine()) != null)
                {
                    if (line.Contains(ZoekenOpNaam, StringComparison.CurrentCultureIgnoreCase))
                    {
                        searchresult.Append(Path.GetFileName(file) + " " + line + "\n");
                    }
                }
                ZoekResultaat = searchresult.ToString();
            }
        }

        private void LoadIni()
        {
            string path = _directory + @"\CorRegA.ini";
            string tline;
            StreamReader sr = new StreamReader(path);
            while ((tline = sr.ReadLine()) != null)
                {
                var firstSpaceIndex = tline.IndexOf(" ");
                if (firstSpaceIndex > 0)
                {
                    if (tline.Substring(0, firstSpaceIndex) == "DAGENBEWAREN")
                    {
                        HoeveelDagenBewaren = Int32.Parse(tline.Substring(firstSpaceIndex + 1));
                    }
                    if (tline.Substring(0, firstSpaceIndex) == "WACHTWOORD")
                    {
                        Wachtwoord = Decrypt(tline.Substring(firstSpaceIndex + 1));
                    }
                }
            }
        }

        private void SaveIni(object obj)
        {
            string path = _directory + @"\CorRegA.ini";
            FileInfo fileusername = new FileInfo(path);
            StreamWriter sr = fileusername.CreateText(); 
            sr.WriteLine("ALGEMEEN");
            sr.WriteLine("DAGENBEWAREN " + HoeveelDagenBewaren);
            sr.WriteLine("WACHTWOORD " + Encrypt(Wachtwoord));
            sr.Close();
            
        }

        private void DeleteOldFiles()
        {
            string path = _directory + @"\data";
            string[] allfiles = Directory.GetFiles(path, "", SearchOption.AllDirectories);
            foreach (string file in allfiles) 
            {
                DateTime creation = File.GetCreationTime(file);
                if ((DateTime.Now - creation).TotalDays > HoeveelDagenBewaren)
                {
                    File.Delete(file);
                }
            }
        }
        private void CreateIni()
        {
            string path = _directory;
            if (!Directory.Exists(path + @"\data"))
            {
                // This path is a directory
                Directory.CreateDirectory(path + @"\data");
            }
            string filename = @"\CorRegA.ini";
            if (!File.Exists(path + filename))
            {
                FileInfo fi = new FileInfo(path + filename);
                using FileStream fs = fi.Create();
                fs.Close();
                FileInfo fileusername = new FileInfo(path + filename);
                StreamWriter sr = fileusername.CreateText();
                sr.WriteLine("ALGEMEEN");
                sr.WriteLine("DAGENBEWAREN 21" );
                sr.WriteLine("WACHTWOORD " + Encrypt("12345"));
                sr.Close();
            }
        }
        private void SaveData(object obj)
        {
            if (Naam == null || Naam == "")
            {
                return;
            }
            string path = _directory + @"\data";
            string date = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString();
            using (StreamWriter sw = File.AppendText(path + @"\" + date + ".csv"))
            {
                
                sw.WriteLine(DateTime.Now.ToString("HH:mm") +  ";" + Naam + ";" + TelefoonNummer + ";" + ZelfCorona.ToString() + ";" + GezinCorona.ToString() + ";" + ContactmetAndereCorona.ToString() + ";" + ZelfAndereZiekte.ToString() + ";" + GezinAndereZiekte.ToString());
            }

            Naam = "";  TelefoonNummer = ""; ZelfCorona = false; GezinCorona = false; GezinAndereZiekte = false; ZelfAndereZiekte = false; ContactmetAndereCorona = false;
        }
        private void DisplayInvoer(object obj)
        {
            string path = _directory + @"\data";
            if (!Directory.Exists(path))
            {
                // This path is a directory
                Directory.CreateDirectory(path);
            }
            string date = DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString();
            if (!File.Exists(path + @"\" + date + ".csv"))
            {
                FileInfo fi = new FileInfo(path + @"\" + date + ".csv");
                using FileStream fs = fi.Create();
                Byte[] columnnames = new UTF8Encoding(true).GetBytes("tijd; naam; telefoonnummer; zelfcorona; gezincorona; contactmetanderecorona; zelfandereziekte; gezinandereziekte\n");
                fs.Write(columnnames, 0, columnnames.Length);
            }
            SpVisibilityInstellingen = Visibility.Collapsed;
            SpVisibilityMain = Visibility.Collapsed;
            SpVisibilityZoeken = Visibility.Collapsed;
            SpVisibilityInvoer = Visibility.Visible;
            InputWachtwoord = "";
        }
        private void DisplayMain(object obj)
        {
            SpVisibilityInstellingen = Visibility.Collapsed;
            SpVisibilityMain = Visibility.Visible;
            SpVisibilityInvoer = Visibility.Collapsed;
            SpVisibilityZoeken = Visibility.Collapsed;

        }
        private void DisplayZoeken(object obj)
        {
            if (Wachtwoord == (InputWachtwoord))
            {
                SpVisibilityInstellingen = Visibility.Collapsed;
                SpVisibilityMain = Visibility.Collapsed;
                SpVisibilityInvoer = Visibility.Collapsed;
                SpVisibilityZoeken = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Verkeerd wachtwoord");
            }
            InputWachtwoord = "";
        }
        private void DisplayInstellingen(object obj)
        {
            if (Wachtwoord == (InputWachtwoord))
            {
                SpVisibilityInstellingen = Visibility.Visible;
                SpVisibilityMain = Visibility.Collapsed;
                SpVisibilityInvoer = Visibility.Collapsed;
                SpVisibilityZoeken = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Verkeerd wachtwoord");
            }
            InputWachtwoord = "";
        }
        #endregion
    }  
}
