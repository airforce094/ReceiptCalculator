using Spire.Pdf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace ReceiptCalculator.MVVM
{
    public class ReceiptVM : INotifyPropertyChanged
    {
        public ObservableCollection<ReceiptModel> PDFRecords { get; set; }
        public ReceiptVM()
        {
            PDFRecords = new ObservableCollection<ReceiptModel>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        private string _receiptFolderPath = string.Empty;
        public string ReceiptFolderPath
        {
            get
            {
                return _receiptFolderPath;
            }
            set
            {
                _receiptFolderPath = value;
                NotifyPropertyChanged("ReceiptFolderPath");
            }
        }

        private double _totalCount;
        public double TotalCount
        {
            get
            {
                return _totalCount;
            }
            set
            {
                _totalCount = value;
                NotifyPropertyChanged("TotalCount");
            }
        }

        private int _paperNumbers = 0;
        public int PaperNumbers
        {
            get
            {
                return _paperNumbers;
            }
            set
            {
                _paperNumbers = value;
                NotifyPropertyChanged("PaperNumbers");
            }
        }

        private ICommand _browserCmd;
        public ICommand BrowserCmd
        {
            get { return _browserCmd ?? (_browserCmd = new OperationBrowser(this)); }
            set
            {
                _browserCmd = value;
            }
        }

        private ICommand _readCmd;
        public ICommand ReadCmd
        {
            get { return _readCmd ?? (_readCmd = new OperationRead(this)); }
            set
            {
                _readCmd = value;
            }
        }

        private ICommand _clearCmd;
        public ICommand ClearCmd
        {
            get { return _clearCmd ?? (_clearCmd = new OperationClear(this)); }
            set
            {
                _clearCmd = value;
            }
        }

        public void Core()
        {   
            var folderpath = ReceiptFolderPath;
            PDFRecords.Clear();
            if (Directory.Exists(folderpath))
            {
                var dir = new DirectoryInfo(folderpath);
                var fileinfos = dir.GetFiles("*.pdf", SearchOption.TopDirectoryOnly);
                if (fileinfos != null && fileinfos.Length > 0)
                {
                    for (var i = 0; i < fileinfos.Length; i++)
                    {
                        try
                        {
                            var file = fileinfos[i];
                            var doc = new PdfDocument();
                            doc.LoadFromFile(file.FullName);
                            foreach (PdfPageBase page in doc.Pages)
                            {
                                var pdfcontent = page.ExtractText();
                                var starttag = "小写";
                                var startindex = pdfcontent.IndexOf(starttag);
                                var str_number = pdfcontent.Substring(startindex + 4);
                                var entertag = "\r\n";
                                var endindex = str_number.IndexOf(entertag);
                                str_number = str_number.Substring(0, endindex).Trim();
                                if (str_number.StartsWith("￥"))
                                {
                                    str_number = str_number.Substring(1);
                                }
                                double result;
                                var isdouble = double.TryParse(str_number, out result);
                                if (isdouble)
                                {
                                    var model = new ReceiptModel()
                                    {
                                        Name = Path.GetFileNameWithoutExtension(file.Name),
                                        Amount = result,
                                        IsSelected = true,
                                    };
                                    model.SelectHandler += ChangeTotal;
                                    PDFRecords.Add(model);
                                }
                                break;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }
            PaperNumbers = PDFRecords.Count;
            TotalCount = PDFRecords.Sum(x => x.Amount);
        }

        private void ChangeTotal()
        {
            TotalCount = PDFRecords.Where(x => x.IsSelected).Sum(x => x.Amount);
        }
    }

    public class OperationBrowser : ICommand
    {
        private ReceiptVM vm;

        public OperationBrowser(ReceiptVM _vm)
        {
            this.vm = _vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var folderBrowserDialog = new FolderBrowserDialog
            {
                Description = "选择文件夹"
            };
            DialogResult dialogResult = folderBrowserDialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                vm.ReceiptFolderPath = folderBrowserDialog.SelectedPath;
                vm.Core();
            }
        }
    }

    public class OperationRead : ICommand
    {
        private ReceiptVM vm;

        public OperationRead(ReceiptVM _vm)
        {
            this.vm = _vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            vm.Core();
        }
    }

    public class OperationClear : ICommand
    {
        private ReceiptVM vm;

        public OperationClear(ReceiptVM _vm)
        {
            this.vm = _vm;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            vm.ReceiptFolderPath = string.Empty;
            vm.PDFRecords.Clear();
            vm.TotalCount = 0.0;
            vm.PaperNumbers = 0;
        }
    }
}
