using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using viewModel;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Classes;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;
using System.Configuration;

namespace viewModel
{
    class viewModel : viewModelBase
    {
        private Visibility _visibilityHome;
        private Visibility _visibilityAddProvider;
        private Visibility _visibilityCorrespondenceSelector;
        private Visibility _visibilityContinueProvider;
        private Visibility _visibilityValidProvider;

        private ObservableCollection<Provider> _providers;

        private ICommand _closeWindow;
        private ICommand _minusWindow;
        private ICommand _openAddProvider;
        private ICommand _validProvider;
        private ICommand _continueProvider;
        private ICommand _browseCsvFile;
        private ICommand _validCorrespondence;
        private ICommand _inFinalFile;
        private ICommand _deleteProvider;
        private ICommand _exportCSV;
        private ICommand _editProvider;
        private ICommand _cancelAddProvider;
        private ICommand _importParams;
        private ICommand _exportParams;
        private ICommand _placeProviderBefore;
        private ICommand _placeProviderAfter;

        private string _errorProviderName;
        private string _errorProviderFile;
        private Visibility _visibilityErrorProviderName;
        private Visibility _visibilityErrorProviderFile;
        private string _providerName; //A retirer
        private string _providerFile; //A retirer

        private Provider _selectedProvider;

        private ObservableCollection<string> _csvColumns;

        //A retirer
        private int _selectedEanIndex;
        private int _selectedPriceIndex;
        private int _selectedDescrIndex;
        private int _selectedCategIndex;

        //A retirer
        private bool _enableProviderParams;
        private bool _checkedInFinalFile;

        private Window App;

        public viewModel(Window frm)
        {
            App = frm;

            VisibilityHome = Visible;
            VisibilityAddProvider = Hidden;
            VisibilityCorrespondenceSelector = Hidden;

            Providers = new ObservableCollection<Provider>();

            List<Provider> settingsProvider = ReadAllSettings();
            foreach (Provider p in settingsProvider)
            {
                Providers.Add(p);
            }

            //SelectedProvider = new Provider();

            ProviderName = "";
            ProviderFile = "";

            CsvColumns = new ObservableCollection<string>();

            EnableProviderParams = false;

            SelectedProvider = null;
        }

        private Visibility Visible
        {
            get
            {
                return Visibility.Visible;
            }
        }

        private Visibility Hidden
        {
            get
            {
                return Visibility.Hidden;
            }
        }

        public Visibility VisibilityHome
        {
            get
            {
                return _visibilityHome;
            }
            set
            {
                _visibilityHome = value;
                OnPropertyChanged("VisibilityHome");
            }
        }

        public Visibility VisibilityAddProvider
        {
            get
            {
                return _visibilityAddProvider;
            }
            set
            {
                _visibilityAddProvider = value;
                OnPropertyChanged("VisibilityAddProvider");
            }
        }

        public Visibility VisibilityCorrespondenceSelector
        {
            get
            {
                return _visibilityCorrespondenceSelector;
            }
            set
            {

                _visibilityCorrespondenceSelector = value;
                OnPropertyChanged("VisibilityCorrespondenceSelector");
            }
        }

        public Visibility VisibilityErrorProviderName
        {
            get
            {
                return _visibilityErrorProviderName;
            }
            set
            {
                _visibilityErrorProviderName = value;
                OnPropertyChanged("VisibilityErrorProviderName");
            }
        }

        public Visibility VisibilityErrorProviderFile
        {
            get
            {
                return _visibilityErrorProviderFile;
            }
            set
            {
                _visibilityErrorProviderFile = value;
                OnPropertyChanged("VisibilityErrorProviderFile");
            }
        }

        public Visibility VisibilityValidProvider
        {
            get
            {
                return _visibilityValidProvider;
            }
            set
            {
                _visibilityValidProvider = value;
                OnPropertyChanged("VisibilityValidProvider");
            }
        }

        public Visibility VisibilityContinueProvider
        {
            get
            {
                return _visibilityContinueProvider;
            }
            set
            {
                _visibilityContinueProvider = value;
                OnPropertyChanged("VisibilityContinueProvider");
            }
        }

        private void HideAllViews()
        {
            VisibilityHome = Hidden;
            VisibilityAddProvider = Hidden;
            VisibilityCorrespondenceSelector = Hidden;
        }

        public ObservableCollection<Provider> Providers
        {
            get
            {
                return _providers;
            }
            set
            {
                _providers = value;
                OnPropertyChanged("Providers");
            }
        }

        public ICommand OpenAddProvider
        {
            get
            {
                if (this._openAddProvider == null)
                    this._openAddProvider = new RelayCommand(() => this.openAddProvider(), () => true);

                return this._openAddProvider;
            }
        }

        private void openAddProvider()
        {
            VisibilityHome = Hidden;
            VisibilityAddProvider = Visible;
            VisibilityContinueProvider = Hidden;
            VisibilityValidProvider = Visible;

            SelectedProvider = new Provider();
        }

        public ICommand BrowseCsvFile
        {
            get
            {
                if (this._browseCsvFile == null)
                    this._browseCsvFile = new RelayCommand(() => this.browseCsvFile(), () => true);

                return this._browseCsvFile;
            }
        }

        private void browseCsvFile()
        {
            string path = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV|*.csv";
            if (openFileDialog.ShowDialog() == true)
            {
                path = openFileDialog.FileName;
                if (!IsFileLocked(path))
                {
                    ProviderFile = openFileDialog.FileName;
                }
            }
        }

        protected virtual bool IsFileLocked(string path)
        {
            try
            {
                using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                MessageBoxResult msgB = MessageBox.Show("Un fichier CSV est inaccessible car il est utilisé par une autre application !\n\nFermez le fichier en question et réessayez.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);

                return true;
            }

            //file is not locked
            return false;
        }

        public ICommand PlaceProviderBefore
        {
            get
            {
                if (this._placeProviderBefore == null)
                    this._placeProviderBefore = new RelayCommand(() => this.changeProviderPlace("before"), () => true);

                return this._placeProviderBefore;
            }
        }

        public ICommand PlaceProviderAfter
        {
            get
            {
                if (this._placeProviderAfter == null)
                    this._placeProviderAfter = new RelayCommand(() => this.changeProviderPlace("after"), () => true);

                return this._placeProviderAfter;
            }
        }

        private void changeProviderPlace(string move)
        {
            ObservableCollection<Provider> tempProviders = new ObservableCollection<Provider>();

            if (Providers.Contains(SelectedProvider))
            {
                int actualIndex = Providers.IndexOf(SelectedProvider);
                if(move == "before")
                {
                    Provider previousProvider = new Provider();
                    if (actualIndex != 0)
                    {
                        foreach (Provider p in Providers)
                        {
                            if (Providers.IndexOf(p) == actualIndex - 1)
                            {
                                previousProvider = p;
                            }
                            else if (Providers.IndexOf(p) == actualIndex)
                            {
                                tempProviders.Add(p);
                                tempProviders.Add(previousProvider);
                            }
                            else
                            {
                                tempProviders.Add(p);
                            }
                        }
                        Providers = tempProviders;
                        refreshSettings();
                    }
                }
                if(move == "after")
                {
                    Provider nextProvider = new Provider();
                    if (actualIndex != Providers.Count() - 1)
                    {
                        foreach (Provider p in Providers)
                        {
                            if (Providers.IndexOf(p) == actualIndex + 1)
                            {
                                nextProvider = p;
                            }
                            else if (Providers.IndexOf(p) == actualIndex)
                            {
                                nextProvider = Providers[Providers.IndexOf(p) + 1];
                                tempProviders.Add(nextProvider);
                                tempProviders.Add(p);
                            }
                            else
                            {
                                tempProviders.Add(p);
                            }
                        }
                        Providers = tempProviders;
                        refreshSettings();
                    }
                }
            }
        }

        public ICommand ValidProvider
        {
            get
            {
                if (this._validProvider == null)
                    this._validProvider = new RelayCommand(() => this.validProvider("valid"), () => true);

                return this._validProvider;
            }
        }

        public ICommand ContinueProvider
        {
            get
            {
                if (this._continueProvider == null)
                    this._continueProvider = new RelayCommand(() => this.validProvider("continue"), () => true);

                return this._continueProvider;
            }
        }

        private void validProvider(string state)
        {
            bool providerNotExists = true;
            Provider oldProviderParameters = new Provider();
            string odlProviderName = "";
            if (state == "continue")
            {
                odlProviderName = SelectedProvider.Name;
                oldProviderParameters = SelectedProvider;
            }
            if (ProviderName.Length > 0 && ProviderFile.Length > 0)
            {
                if (state == "valid")
                {
                    foreach (Provider p in Providers)
                    {
                        if (p.Name == ProviderName)
                        {
                            providerNotExists = false;
                        }
                    }
                }
                if (ProviderFile.Substring(ProviderFile.Length - 4, 4) == ".csv")
                {
                    if (File.Exists(ProviderFile))
                    {
                        if (providerNotExists)
                        {
                            if (state == "valid")
                            {
                                Provider theProvider = new Provider();
                                theProvider.Name = ProviderName;
                                theProvider.File = ProviderFile;
                                //Providers.Add(theProvider);
                                SelectedProvider = theProvider;
                            }
                            else
                            {
                                SelectedProvider.Name = ProviderName;
                                SelectedProvider.File = ProviderFile;
                            }

                            if (state == "valid")
                            {
                                VisibilityErrorProviderName = Hidden;
                                VisibilityErrorProviderFile = Hidden;
                                HideAllViews();

                                fillComboBox(SelectedProvider.File, ';');
                                setComboBoxSelection();

                                VisibilityCorrespondenceSelector = Visible;
                            }


                            if (state == "continue")
                            {
                                string newSettings = "";

                                newSettings = getSettingsLine(oldProviderParameters, SelectedProvider.Name, SelectedProvider.File);

                                UnSetSetting(oldProviderParameters.Name);
                                SetSetting(SelectedProvider.Name, newSettings);

                                HideAllViews();

                                fillComboBox(SelectedProvider.File, ';');
                                setComboBoxSelection();

                                VisibilityCorrespondenceSelector = Visible;
                            }
                            //resetSelectedIndex();
                        }
                        else
                        {
                            ErrorProviderName = "Ce fournisseur existe déjà !";
                            VisibilityErrorProviderName = Visible;
                        }
                    }
                    else
                    {
                        ErrorProviderFile = "Le fichier est introuvable !";
                    }
                }
                else
                {
                    ErrorProviderFile = "Uniquement des fichiers CSV !";
                }
            }
            else if (ProviderFile.Length == 0 && ProviderName.Length == 0)
            {
                ErrorProviderFile = "Veuillez remplir ce champ !";
                ErrorProviderName = "Veuillez remplir ce champ !";
                VisibilityErrorProviderName = Visible;
                VisibilityErrorProviderFile = Visible;
            }
            else if (ProviderFile.Length == 0)
            {
                ErrorProviderFile = "Veuillez remplir ce champ !";
                VisibilityErrorProviderName = Hidden;
                VisibilityErrorProviderFile = Visible;
            }
            else if (ProviderName.Length == 0)
            {
                ErrorProviderName = "Veuillez remplir ce champ !";
                VisibilityErrorProviderName = Visible;
                VisibilityErrorProviderFile = Hidden;
            }
        }

        public ICommand CancelAddProvider
        {
            get
            {
                if (this._cancelAddProvider == null)
                    this._cancelAddProvider = new RelayCommand(() => this.cancelAddProvider(), () => true);

                return this._cancelAddProvider;
            }
        }

        private void cancelAddProvider()
        {
            ProviderFile = "";
            ProviderName = "";
            SelectedProvider = null;
            VisibilityAddProvider = Hidden;
            VisibilityHome = Visible;
        }

        private void fillComboBox(string path, char csvSeparator)
        {
            string headLine;
            CsvColumns.Clear();
            StreamReader sr = new StreamReader(path);
            headLine = sr.ReadLine();
            sr.Close();
            string[] columnsName;
            columnsName = headLine.Split(csvSeparator);
            foreach (string s in columnsName)
            {
                CsvColumns.Add(s);
            }
        }

        public void setComboBoxSelection()
        {
            SelectedEanIndex = SelectedProvider.Correspondences[0];
            SelectedPriceIndex = SelectedProvider.Correspondences[1];
            SelectedDescrIndex = SelectedProvider.Correspondences[2];
            SelectedCategIndex = SelectedProvider.Correspondences[3];
        }

        public string ErrorProviderName
        {
            get
            {
                return _errorProviderName;
            }
            set
            {
                _errorProviderName = value;
                OnPropertyChanged("ErrorProviderName");
            }
        }

        public string ErrorProviderFile
        {
            get
            {
                return _errorProviderFile;
            }
            set
            {
                _errorProviderFile = value;
                OnPropertyChanged("ErrorProviderFile");
            }
        }

        public string ProviderName
        {
            get
            {
                return _providerName;
            }
            set
            {
                _providerName = value;
                OnPropertyChanged("ProviderName");
            }
        }

        public string ProviderFile
        {
            get
            {
                return _providerFile;
            }
            set
            {
                _providerFile = value;
                OnPropertyChanged("ProviderFile");
            }
        }

        public Provider SelectedProvider
        {
            get
            {
                return _selectedProvider;
            }
            set
            {
                _selectedProvider = value;
                OnPropertyChanged("SelectedProvider");
                if (SelectedProvider == null)
                {
                    CheckedInFinalFile = false;
                    EnableProviderParams = false;
                }
                else
                {
                    if (SelectedProvider.ToExport)
                    {
                        CheckedInFinalFile = true;
                    }
                    else
                    {
                        CheckedInFinalFile = false;
                    }
                    EnableProviderParams = true;
                }
            }
        }

        public ObservableCollection<string> CsvColumns
        {
            get
            {
                return _csvColumns;
            }
            set
            {
                _csvColumns = value;
                OnPropertyChanged("CsvColumns");
            }
        }

        public ICommand ValidCorrespondence
        {
            get
            {
                if (this._validCorrespondence == null)
                    this._validCorrespondence = new RelayCommand(() => this.validCorrespondence(), () => true);

                return this._validCorrespondence;
            }
        }

        private void validCorrespondence()
        {
            List<int> fieldsAssociation = new List<int>();
            if (SelectedEanIndex > -1)
            {
                fieldsAssociation.Add(SelectedEanIndex);
            }
            else
            {
                fieldsAssociation.Add(-1);
            }
            if (SelectedPriceIndex > -1)
            {
                fieldsAssociation.Add(SelectedPriceIndex);
            }
            else
            {
                fieldsAssociation.Add(-1);
            }
            if (SelectedDescrIndex > -1)
            {
                fieldsAssociation.Add(SelectedDescrIndex);
            }
            else
            {
                fieldsAssociation.Add(-1);
            }
            if (SelectedCategIndex > -1)
            {
                fieldsAssociation.Add(SelectedCategIndex);
            }
            else
            {
                fieldsAssociation.Add(-1);
            }

            SelectedProvider.Correspondences = fieldsAssociation;
            if (!Providers.Contains(SelectedProvider))
            {
                Providers.Add(SelectedProvider);
            }

            string settingsLine = SelectedProvider.Name + ';';
            for (int i = 0; i < 4; i++)
            {
                if (i < 3)
                {
                    settingsLine += SelectedProvider.Correspondences[i] + ",";
                }
                else
                {
                    settingsLine += SelectedProvider.Correspondences[i];
                }
            }
            settingsLine += ';';
            settingsLine += SelectedProvider.File;
            SetSetting(SelectedProvider.Name, settingsLine);

            if (Providers.Contains(SelectedProvider))
            {
                Provider p = SelectedProvider;
                Providers.Remove(SelectedProvider);
                Providers.Add(p);
            }

            ProviderName = "";
            ProviderFile = "";

            VisibilityCorrespondenceSelector = Hidden;
            VisibilityHome = Visible;
        }

        public int SelectedEanIndex
        {
            get
            {
                return _selectedEanIndex;
            }
            set
            {
                _selectedEanIndex = value;
                OnPropertyChanged("SelectedEanIndex");
            }
        }

        public int SelectedPriceIndex
        {
            get
            {
                return _selectedPriceIndex;
            }
            set
            {
                _selectedPriceIndex = value;
                OnPropertyChanged("SelectedPriceIndex");
            }
        }

        public int SelectedDescrIndex
        {
            get
            {
                return _selectedDescrIndex;
            }
            set
            {
                _selectedDescrIndex = value;
                OnPropertyChanged("SelectedDescrIndex");
            }
        }

        public int SelectedCategIndex
        {
            get
            {
                return _selectedCategIndex;
            }
            set
            {
                _selectedCategIndex = value;
                OnPropertyChanged("SelectedCategIndex");
            }
        }

        private void resetSelectedIndex()
        {
            SelectedEanIndex = -1;
            SelectedPriceIndex = -1;
            SelectedCategIndex = -1;
            SelectedDescrIndex = -1;
        }

        private ICommand _testButton;

        public ICommand TestButton
        {
            get
            {
                if (this._testButton == null)
                    this._testButton = new RelayCommand(() => this.testButton(), () => true);

                return this._testButton;
            }
        }

        private void testButton()
        {
            Provider f = new Provider();
            f.Name = "TEST";
            Providers.Add(f);
        }

        public bool EnableProviderParams
        {
            get
            {
                return _enableProviderParams;
            }
            set
            {
                _enableProviderParams = value;
                OnPropertyChanged("EnableProviderParams");
            }
        }

        public bool CheckedInFinalFile
        {
            get
            {
                return _checkedInFinalFile;
            }
            set
            {
                _checkedInFinalFile = value;
                OnPropertyChanged("CheckedInFinalFile");
            }
        }

        public ICommand InFinalFile
        {
            get
            {
                if (this._inFinalFile == null)
                    this._inFinalFile = new RelayCommand(() => this.inFinalFile(), () => true);

                return this._inFinalFile;
            }
        }

        private void inFinalFile()
        {
            if (SelectedProvider.ToExport)
            {
                CheckedInFinalFile = false;
                SelectedProvider.ToExport = false;
            }
            else
            {
                CheckedInFinalFile = true;
                SelectedProvider.ToExport = true;
            }
            refreshProvidersDisplay();
        }

        private void refreshProvidersDisplay()
        {
            ObservableCollection<Provider> tempProviders = new ObservableCollection<Provider>();
            foreach(Provider p in Providers)
            {
                tempProviders.Add(p);
            }
            Providers = tempProviders;
        }

        public ICommand DeleteProvider
        {
            get
            {
                if (this._deleteProvider == null)
                    this._deleteProvider = new RelayCommand(() => this.deleteProvider(), () => true);

                return this._deleteProvider;
            }
        }

        private void deleteProvider()
        {
            if (SelectedProvider != null)
            {
                UnSetSetting(SelectedProvider.Name);
                Providers.Remove(SelectedProvider);
            }
        }

        public ICommand EditProvider
        {
            get
            {
                if (this._editProvider == null)
                    this._editProvider = new RelayCommand(() => this.editProvider(), () => true);

                return this._editProvider;
            }
        }

        private void editProvider()
        {
            VisibilityHome = Hidden;
            VisibilityAddProvider = Visible;

            VisibilityContinueProvider = Visible;
            VisibilityValidProvider = Hidden;

            ProviderName = SelectedProvider.Name;
            ProviderFile = SelectedProvider.File;
        }

        public ICommand ImportParams
        {
            get
            {
                if (this._importParams == null)
                    this._importParams = new RelayCommand(() => this.importParams(), () => true);

                return this._importParams;
            }
        }

        private void importParams()
        {
            string path = "";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Fichier Texte|*.txt";
            if (openFileDialog.ShowDialog() == true)
                path = openFileDialog.FileName;

            if (path != "")
            {
                StreamReader sr = new StreamReader(path);
                while (!sr.EndOfStream)
                {
                    bool providerExists = false;
                    string[] srLine = sr.ReadLine().Split(';');
                    Provider p = new Provider();
                    p.Correspondences.Clear();
                    p.Name = srLine[0];
                    foreach (string str in srLine[1].Split(','))
                    {
                        p.Correspondences.Add(Convert.ToInt32(str));
                    }
                    p.File = srLine[2];
                    foreach (Provider prov in Providers)
                    {
                        if (p.Name == prov.Name)
                        {
                            providerExists = true;
                        }
                    }
                    if (!providerExists)
                    {
                        Providers.Add(p);
                        SetSetting(p.Name, srLine[0] + ";" + srLine[1] + ";" + srLine[2]);
                    }
                }
                sr.Close();
            }
        }

        public ICommand ExportParams
        {
            get
            {
                if (this._exportParams == null)
                    this._exportParams = new RelayCommand(() => this.exportParams(), () => true);

                return this._exportParams;
            }
        }

        private void exportParams()
        {
            var dialog = new SaveFileDialog();
            string path = "";
            dialog.InitialDirectory = ""; //textbox.Text; // Use current value for initial dir
            dialog.Title = "Sélectionner le dossier d'emplacement"; // instead of default "Save As"
            dialog.Filter = "Fichier Texte|*.txt"; // Prevents displaying files
            //dialog.FileName = "select"; // Filename will then be "select.this.directory"
            if (dialog.ShowDialog() == true)
            {
                path = dialog.FileName;
                // Remove fake filename from resulting path
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                // If user has changed the filename, create the new directory
                /*if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }*/
                // Our final value is in path
                //textbox.Text = path;
            }
            if (path != "")
            {
                createParamsFile(path);
            }
        }

        private void createParamsFile(string path)
        {
            StreamWriter sw = new StreamWriter(path, false);
            foreach (Provider p in Providers)
            {
                string swLine = "";
                swLine += p.Name + ";";
                for (int i = 0; i < p.Correspondences.Count(); i++)
                {
                    if (i < 3)
                    {
                        swLine += p.Correspondences[i] + ",";
                    }
                    else
                    {
                        swLine += p.Correspondences[i];
                    }
                }
                swLine += ";";
                swLine += p.File;
                sw.WriteLine(swLine);
            }
            sw.Close();
        }
        public ICommand ExportCSV
        {
            get
            {
                if (this._exportCSV == null)
                    this._exportCSV = new RelayCommand(() => this.exportCSV(), () => true);

                return this._exportCSV;
            }
        }

        private void exportCSV()
        {
            Dictionary<string, string> ArticleFamilys;
            bool isExportPossible = true;

            var dialog = new SaveFileDialog();
            string path = "";
            dialog.InitialDirectory = ""; //textbox.Text; // Use current value for initial dir
            dialog.Title = "Sélectionner le dossier d'emplacement final"; // instead of default "Save As"
            dialog.Filter = "Directory|*.this.directory"; // Prevents displaying files
            dialog.FileName = "select"; // Filename will then be "select.this.directory"
            if (dialog.ShowDialog() == true)
            {
                path = dialog.FileName;
                // Remove fake filename from resulting path
                path = path.Replace("\\select.this.directory", "");
                path = path.Replace(".this.directory", "");
                // If user has changed the filename, create the new directory
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                // Our final value is in path
                //textbox.Text = path;
            }
            if (path != "")
            {
                foreach(Provider p in Providers)
                {
                    if (IsFileLocked(p.File))
                    {
                        isExportPossible = false;
                    }
                }
                if(IsFileLocked(path + "\\Fournisseurs.csv"))
                {
                    isExportPossible = false;
                }
                if(IsFileLocked(path + "\\CSV_Final.csv"))
                {
                    isExportPossible = false;
                }
                if(IsFileLocked(path + "\\Familles_Articles.csv"))
                {
                    isExportPossible = false;
                }
                if (isExportPossible)
                {
                    creerFichierFournisseurs(path);
                    ArticleFamilys = createArticleFamilyFile(path);

                    creerFichierCSVFinal(path, ArticleFamilys);

                    Process.Start(@path);
                }
            }
        }

        private void creerFichierFournisseurs(string path)
        {
            int n = 1;
            foreach (Provider p in Providers)
            {
                if (p.ToExport)
                {
                    p.Id = getProviderCode(n);
                }
                n++;
            }
            StreamWriter sw = new StreamWriter(path + "\\Fournisseurs.csv", false);
            sw.WriteLine("ID_Fournisseur;Fournisseur");
            foreach (Provider p in Providers)
            {
                if (p.ToExport)
                {
                    sw.WriteLine(p.Id + ";" + p.Name);
                }
            }
            sw.Close();
        }
        private void creerFichierCSVFinal(string path, Dictionary<string, string> ArticlesFamilys)
        {
            int numArt = 1;
            StreamWriter sw = new StreamWriter(path + "\\CSV_Final.csv");
            sw.WriteLine("code_art;Fournisseur_ID;report_PA_sur_PVC;fournisseur_principal;EAN;prix_achat;description;category_name;code_familleArticle");
            foreach (Provider p in Providers)
            {
                if (p.ToExport)
                {
                    string swLine = "";
                    int eanIndex = 0, prixIndex = 1, descrIndex = 2, catIndex = 3;
                    string pathReader;
                    pathReader = p.File;
                    StreamReader sr = new StreamReader(pathReader);
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        swLine = "";
                        ulong EAN = 0;
                        string[] srLine = sr.ReadLine().Split(';');
                        List<int> list = p.Correspondences;
                        if (srLine[list[eanIndex]] != "0" && srLine[list[eanIndex]] != "")
                        {
                            ulong.TryParse(srLine[list[eanIndex]], out EAN);
                            if (EAN > 0)
                                swLine += EAN;
                            else
                                swLine += getCodeArticle(numArt);
                        }
                        else
                        {
                            swLine += getCodeArticle(numArt);
                        }
                        swLine += ";";
                        swLine += p.Id;
                        swLine += ";1;1;";
                        if (list[eanIndex] > -1)
                        {
                            if (srLine[list[eanIndex]] != "0" && srLine[list[eanIndex]] != "")
                            {
                                ulong.TryParse(srLine[list[eanIndex]], out EAN);
                                if (EAN > 0)
                                    swLine += EAN;
                            }
                        }
                        swLine += ";";
                        if (list[prixIndex] > -1)
                        {
                            string prixAchatInCSV = srLine[list[prixIndex]];
                            prixAchatInCSV = prixAchatInCSV.Trim();
                            prixAchatInCSV = prixAchatInCSV.Split(' ')[0];
                            swLine += prixAchatInCSV;
                        }
                        swLine += ";";
                        if (list[descrIndex] > -1)
                            swLine += srLine[list[descrIndex]];
                        swLine += ";";
                        if (list[catIndex] > -1)
                        {
                            string categoryName = srLine[list[catIndex]];
                            string codeArticleFamily = "";
                            swLine += srLine[list[catIndex]];
                            foreach (string dico in ArticlesFamilys.Keys)
                            {
                                if (ArticlesFamilys[dico] == categoryName)
                                {
                                    codeArticleFamily = dico;
                                }
                            }
                            swLine += ";" + codeArticleFamily;
                        }
                        else
                        {
                            swLine += ArticlesFamilys["FAR00000"] + ";" + "FAR00000";
                        }

                        sw.WriteLine(swLine);
                        numArt++;
                    }
                    sr.Close();
                }
            }
            sw.Close();
        }

        private Dictionary<string, string> createArticleFamilyFile(string path)
        {
            Dictionary<string, string> ArticleFamilys = new Dictionary<string, string>();
            List<string> familys = new List<string>();
            foreach (Provider p in Providers)
            {
                if (p.Correspondences[3] > -1)
                {
                    StreamReader sr = new StreamReader(p.File);
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        string str = sr.ReadLine();
                        str = str.Split(';')[p.Correspondences[3]];
                        if (!familys.Contains(str))
                        {
                            familys.Add(str);
                        }
                    }
                    sr.Close();
                }
            }
            familys.Sort();

            ArticleFamilys.Add("FAR00000", "Famille Inconnue");
            for (int i = 0; i < familys.Count(); i++)
            {
                ArticleFamilys.Add(getCodeArticleFamily(i + 1), familys[i]);
            }

            StreamWriter sw = new StreamWriter(path + "\\Familles_Articles.csv", false);
            sw.WriteLine("ID_FamilleArticle;Famille_Article");
            foreach (string dico in ArticleFamilys.Keys)
            {
                sw.WriteLine(dico + ";" + ArticleFamilys[dico]);
            }
            sw.Close();

            return ArticleFamilys;
        }

        private string getProviderCode(int n)
        {
            string providerId;
            if (n < 10)
                providerId = "FR0000" + n;
            else if (n < 100)
                providerId = "FR000" + n;
            else if (n < 1000)
                providerId = "FR00" + n;
            else if (n < 10000)
                providerId = "FR0" + n;
            else
                providerId = "FR" + n;

            return providerId;
        }
        private string getCodeArticleFamily(int n)
        {
            string codeArticle;
            if (n < 10)
                codeArticle = "FAR0000" + n;
            else if (n < 100)
                codeArticle = "FAR000" + n;
            else if (n < 1000)
                codeArticle = "FAR00" + n;
            else if (n < 10000)
                codeArticle = "FAR0" + n;
            else
                codeArticle = "FAR" + n;
            return codeArticle;
        }

        private string getCodeArticle(int n)
        {
            string codeArticle;
            if (n < 10)
                codeArticle = "AR0000" + n;
            else if (n < 100)
                codeArticle = "AR000" + n;
            else if (n < 1000)
                codeArticle = "AR00" + n;
            else if (n < 10000)
                codeArticle = "AR0" + n;
            else
                codeArticle = "AR" + n;
            return codeArticle;
        }

        private static string GetSetting(string key)
        {
            string result = ConfigurationManager.AppSettings[key];
            return result;
        }

        private static void SetSetting(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location);

            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);

            config.Save(ConfigurationSaveMode.Modified);
        }

        private void UnSetSetting(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location);

            config.AppSettings.Settings.Remove(key);

            config.Save(ConfigurationSaveMode.Modified);
        }

        private void refreshSettings()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location);
            string settingsValue;

            foreach (Provider p in Providers)
            {
                settingsValue = "";
                settingsValue += getSettingsLine(p);
                config.AppSettings.Settings.Remove(p.Name);
                config.AppSettings.Settings.Add(p.Name, settingsValue);
            }

            config.Save(ConfigurationSaveMode.Modified);
        }

        private string getSettingsLine(Provider p, string name = "", string file = "")
        {
            if (name == "")
            {
                name = p.Name;
            }
            if(file == "")
            {
                file = p.File;
            }
            return name + ";" + p.Correspondences[0] + "," + p.Correspondences[1] + "," + p.Correspondences[2] + "," + p.Correspondences[3] + ";" + file;
        }

        static List<Provider> ReadAllSettings()
        {
            List<Provider> providers = new List<Provider>();
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count == 0)
                {
                    //Console.WriteLine("AppSettings is empty.");
                }
                else
                {
                    foreach (var key in appSettings.AllKeys)
                    {
                        //Console.WriteLine("Key: {0} Value: {1}", key, appSettings[key]);
                        Provider p = new Provider();
                        //p.Name = key;
                        string[] settingsValue = appSettings[key].Split(';');
                        if (settingsValue.Count() == 3)
                        {
                            p.Correspondences.Clear();
                            p.Name = settingsValue[0];
                            foreach (string nb in settingsValue[1].Split(','))
                            {
                                p.Correspondences.Add(Convert.ToInt32(nb));
                            }
                            p.File = settingsValue[2];

                            providers.Add(p);
                        }
                    }
                }
            }
            catch (ConfigurationErrorsException)
            {
                //Console.WriteLine("Error reading app settings");
            }
            return providers;
        }
    }
}
