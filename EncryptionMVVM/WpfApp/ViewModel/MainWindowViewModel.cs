using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Security;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;

namespace WpfApp.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainWindowViewModel class.
        /// </summary>
        public MainWindowViewModel()
        {
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
        }
        //cancel token ++++++
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //прогресс бар сделать - подсчитать длину строки и запустить цикл и == стринг.ленгтс


        Security.Encryption encryption = new Security.Encryption();
        private string fileName;

        private CancellationTokenSource tokenSource;
        private CancellationToken token;
        private Task uncodeTask;
        private Task loadingText;

        #region LoadFile

        private string loadTextFromFile;

        private RelayCommand LoadBtnPress;
        public ICommand LoadBtnPressCommand
        {
            get
            {
                if (LoadBtnPress == null)
                {
                    LoadBtnPress = new RelayCommand(
                        ExecuteLoadBtnPress, CanLoadBtnPress);
                }
                return LoadBtnPress;
            }
        }
        private bool CanLoadBtnPress()
        {
            if (loadingIsStart)//загрузка начата
                return false;

            return true;
        }

        private bool loadingIsStart = false;
        private void ExecuteLoadBtnPress()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
            };
            
            if (openFileDialog.ShowDialog() == true) 
            {
                fileName = openFileDialog.FileName;

                loadingText = new Task(() => LoadFile(token));
                loadingText.Start();
            }
        }

        private void LoadFile(CancellationToken cancelToken)
        {
            loadingIsStart = true;
            Status = "File loading";

            Parallel.Invoke(
                () =>
                {         
                    loadTextFromFile = File.ReadAllText(fileName);  
                });

            ShowProgress();

            if (cancelToken.IsCancellationRequested)
            {
                fileName = null;
                loadingIsStart = false;

                tokenSource = new CancellationTokenSource();
                token = tokenSource.Token;
                return;
            }


            Status = "**File loaded**";
            loadingIsStart = false;
        }
        #endregion LoadFile

        #region Cansel

        private RelayCommand CanselBtnPress;
        public ICommand CanselBtnPressCommand
        {
            get
            {
                if (CanselBtnPress == null)
                {
                    CanselBtnPress = new RelayCommand(
                        ExecuteCanselBtnPress, CanCanselBtnPress);
                }
                return CanselBtnPress;
            }
        }
        private bool CanCanselBtnPress()
        {
            return true;
        }

        private void ExecuteCanselBtnPress()
        {
            if (encryptIsStart)
            {
                Status = "Encrypting is cancel";
                tokenSource.Cancel();
            }
            else if (loadingIsStart)
            {
                Status = "Loading is cancel";
                tokenSource.Cancel();
            }
        }
        #endregion Cansel

        //шифрование
        #region Encrypt
        private RelayCommand EncryptBtnPress;
        public ICommand EncryptBtnPressCommand
        {
            get
            {
                if (EncryptBtnPress == null)
                {
                    EncryptBtnPress = new RelayCommand(
                        ExecuteEncryptBtnPress, CanEncryptBtnPress);
                }
                return EncryptBtnPress;
            }
        }

        private bool encryptIsStart = false;
        private bool CanEncryptBtnPress() 
        {
            if (encryptIsStart)  
                return false;

            if (!string.IsNullOrEmpty(KeyWordText))
            {
                if (Regex.IsMatch(KeyWordText,
                    @"([A-Za-z0-9])"))
                {                
                    return true;
                }
                else
                {
                    MessageBox.Show("Invalid key world");
                }
            }
            return false;
        }


        private void ExecuteEncryptBtnPress()
        {
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("No text in file or file does not chosen");
                return;
            }

            uncodeTask = new Task( () => EncryptAndSave(token));
            uncodeTask.Start();
        }

        private void EncryptAndSave(CancellationToken cancelToken)
        {
            encryptIsStart = true;
            Status = "Encrypting";

            StreamWriter streamWriter = new StreamWriter(fileName);

            Parallel.Invoke(() =>
            {
                //streamWriter.Write - перезаписать файл
                streamWriter.Write(
                    //шифрование
                    encryption.Uncode(KeyWordText, loadTextFromFile)
                    );

                ShowProgress();
            });


            if (cancelToken.IsCancellationRequested)
            {
                streamWriter.Write(loadTextFromFile);
                streamWriter.Close();

                encryptIsStart = false;
                fileName = null;

                tokenSource = new CancellationTokenSource();
                token = tokenSource.Token;
                return;
            }

            streamWriter.Close();

            Status = "Encrypting is done **";
            encryptIsStart = false;
            fileName = null;
        }
        #endregion Encrypt

        #region ProgressBar

        private void ShowProgress()
        {
            Percent = 0;
            for (int i = 0; i < 50; i++)
            {
                Thread.Sleep(10);
                Percent += 2;
            }
        }

        private int percent = 0;
        public int Percent
        {
            get
            {
                return percent;
            }
            set
            {
                percent = value;
                RaisePropertyChanged("Percent");
            }
        }

        #endregion ProgressBar

        #region TextBox
        private string _keyWordText = "EnterKeyWorld";
        public string KeyWordText
        {
            get
            {
                return _keyWordText;
            }
            set
            {
                _keyWordText = value;
                RaisePropertyChanged("KeyWordText");
            }
        }
        #endregion TextBox
        //status
        #region TextBlock
        private string _status = "Status";
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }
        #endregion TextBlock
    }
}