using System.Threading;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Windows;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;

//
// The original version of this form (from December 2009) did not use
// a ViewModel class. I have now split it into a View and a ViewModel,
// but kept them in the same file, so that I don't have to create
// another template file.
//
namespace MultiLang
{
    //=================================================================
    // Enums
    //=================================================================
    public enum enumStartupMode
    {
        UseDefaultCulture = 0,
        UseSavedCulture = 1,
        ShowDialog = 2
    }

    internal enum enumCultureMatch
    {
        None = 0,
        Language = 1,
        Neutral = 2,
        Region = 3
    }

    //=================================================================
    // ViewModel class
    //=================================================================
    public class SelectLanguageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //----------------------------------------------
        // The array of supported cultures is updated
        // automatically by Multi-Language
        //----------------------------------------------
        private static string[] SupportedCultures = { "en", "de-DE" } ; //MLHIDE

        //----------------------------------------------
        // Private members
        //----------------------------------------------
        private enumStartupMode _StartupMode = enumStartupMode.ShowDialog;

        //----------------------------------------------
        // Properties
        //----------------------------------------------
        public List<CultureInfo> CultureList { get; private set; }
        public CultureInfo SelectedCulture { get; set; }

        public enumStartupMode StartupMode
        {
            get { return _StartupMode; }
            set
            {
                _StartupMode = value;
                OnPropertyChanged("StartupMode");
                OnPropertyChanged("UseDefaultCulture");
                OnPropertyChanged("UseSavedCulture");
                OnPropertyChanged("ShowThisDialog");
            }
        }

        public bool UseDefaultCulture
        {
            get { return StartupMode == enumStartupMode.UseDefaultCulture; }
            set { if (value) StartupMode = enumStartupMode.UseDefaultCulture; }
        }

        public bool UseSavedCulture
        {
            get { return StartupMode == enumStartupMode.UseSavedCulture; }
            set { if (value) StartupMode = enumStartupMode.UseSavedCulture; }
        }

        public bool ShowThisDialog
        {
            get { return StartupMode == enumStartupMode.ShowDialog; }
            set { if (value) StartupMode = enumStartupMode.ShowDialog; }
        }

        //----------------------------------------------
        // Public methods
        //----------------------------------------------
        public void GenerateCultureList()
        {
            // Create the empty culture list
            CultureList = new List<CultureInfo>();

            // Fill the culture list in one of two ways:
            // - from the constant array SupportedCultures
            // - from the subdirectories which match a culture name
#if true
            foreach (string BaseName in SupportedCultures)
            {
#else
      var AsmLocation = Assembly.GetExecutingAssembly ().Location;
      var AsmPath     = Path.GetDirectoryName ( AsmLocation );
      var DirList     = new List<string> ();

      DirList.AddRange ( Directory.GetDirectories ( AsmPath, "??" ) );
      DirList.AddRange ( Directory.GetDirectories ( AsmPath, "??-??*" ) );

      foreach (string SubDirName in DirList)
      {
        string BaseName = Path.GetFileName ( SubDirName );
#endif
                try
                {
                    CultureInfo newCult = new CultureInfo(BaseName);
                    CultureList.Add(newCult);
                }
                catch { }
            }
        }

        public void ConstrainSelectedCulture()
        {
            // Replace the SelectedCulture with the nearest match in the CultureList
            var Match = enumCultureMatch.None;
            CultureInfo BestCulture = null;

            foreach (var cult in CultureList)
            {
                var NewMatch = enumCultureMatch.None;

                // How well does this culture match?
                if (SelectedCulture.Equals(cult))
                {
                    NewMatch = enumCultureMatch.Region;
                }
                else if (cult.TwoLetterISOLanguageName == SelectedCulture.TwoLetterISOLanguageName)
                {
                    if (cult.IsNeutralCulture)
                        NewMatch = enumCultureMatch.Neutral;
                    else
                        NewMatch = enumCultureMatch.Language;
                }

                // Is that better than the best match so far?
                if (NewMatch > Match)
                {
                    Match = NewMatch;
                    BestCulture = cult;
                }
            }

            SelectedCulture = BestCulture;
        }

        //
        // SaveSettings and LoadSettings use an XML file, saved in so called
        // Isolated Storage.
        //
        // I'm not convinced that this is really the best way or the best place
        // to store this information, but it's certainly a .NET way to do it.
        //
        public void LoadSettings()
        {
            // Set the defaults
            StartupMode = enumStartupMode.ShowDialog;
            SelectedCulture = Thread.CurrentThread.CurrentUICulture;

            // Create an IsolatedStorageFile object and get the store
            // for this application.
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForDomain();

            // Check whether the file exists
            if (isoStorage.GetFileNames("CultureSettings.xml").Length > 0) //MLHIDE
            {
                // Create isoStorage StreamReader.
                StreamReader stmReader = new StreamReader
                                             (new IsolatedStorageFileStream
                                                   ("CultureSettings.xml",
                                                    FileMode.Open,
                                                    isoStorage)); //MLHIDE

                XmlTextReader xmlReader = new XmlTextReader(stmReader);

                // Loop through the XML file until all Nodes have been read and processed.
                while (xmlReader.Read())
                {
                    switch (xmlReader.Name)
                    {
                        case "StartupMode":                                         //MLHIDE
                            StartupMode = (enumStartupMode)int.Parse(xmlReader.ReadString());
                            break;
                        case "Culture":                                             //MLHIDE
                            string CultName = xmlReader.ReadString();
                            CultureInfo CultInfo = new CultureInfo(CultName);
                            SelectedCulture = CultInfo;
                            break;
                    }
                }

                // Close the reader
                xmlReader.Close();
                stmReader.Close();

            }

            isoStorage.Close();
        }

        public void SaveSettings()
        {

            // Get an isolated store for user, domain, and assembly and put it into
            // an IsolatedStorageFile object.
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForDomain();

            // Create isoStorage StreamWriter and assign it to an XmlTextWriter variable.
            IsolatedStorageFileStream stmWriter = new IsolatedStorageFileStream("CultureSettings.xml", FileMode.Create, isoStorage); //MLHIDE
            XmlTextWriter writer = new XmlTextWriter(stmWriter, Encoding.UTF8);

            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("CultureSettings");                     //MLHIDE
            writer.WriteStartElement("StartupMode");                         //MLHIDE
            writer.WriteString(((int)StartupMode).ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("Culture");                             //MLHIDE
            writer.WriteString(SelectedCulture.Name);
            writer.WriteEndElement();
            writer.WriteEndElement();
            writer.Flush();
            writer.Close();

            stmWriter.Close();
            isoStorage.Close();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    //=================================================================
    // View class
    //=================================================================
    public partial class SelectLanguage : Window
    {
        public SelectLanguage()
        {
            InitializeComponent();
        }

        //----------------------------------------------
        // Public Methods
        //----------------------------------------------
        public void LoadSettingsAndShow()
        {
            LoadSettingsAndShow(false);
        }

        public void LoadSettingsAndShow(bool ForceShow)
        {
            var vm = new SelectLanguageViewModel();

            // Fill the list of supported cultures
            vm.GenerateCultureList();

            // Read the current settings, possibly including the selected culture
            vm.LoadSettings();

            // Constrain the culture to be one of the cultures in the list.
            vm.ConstrainSelectedCulture();

            // Either show the dialog or don't
            if (ForceShow || (vm.StartupMode == enumStartupMode.ShowDialog))
            {
                DataContext = vm;
                this.ShowDialog();
                vm.SaveSettings();
            }
            else
            {
                // If the dialog has not been shown, we still have to call close().
                // Otherwise the application may fail to exit.
                this.Close();
            }

            if (vm.StartupMode != enumStartupMode.UseDefaultCulture)
            {
                if (vm.SelectedCulture != null)
                {
                    // Actually change the culture of the current thread.
                    Thread.CurrentThread.CurrentUICulture = vm.SelectedCulture;

                    if (ForceShow)
                    {
#if false
            // The code generated by VS.NET cannot be used to change the
            // language of an active form. Show a message to this effect.
            MessageBox.Show ( "The settings have been saved.\n" +
                            "The language change will take full effect the next time you start the program.",
                            "Select language",
                            MessageBoxButton.OK );
#else
                        // MLRuntime.MLRuntime.BroadcastLanguageChanged();
#endif
                    }
                }
            }
        }

        //----------------------------------------------
        // Private Methods
        //----------------------------------------------
        private void btOK_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

    }
}
