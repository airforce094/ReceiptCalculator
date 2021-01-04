using System;
using System.ComponentModel;

namespace ReceiptCalculator.MVVM
{
    public class ReceiptModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public string Name { get; set; }

        public double Amount { get; set; }

        private bool _isSelected = true;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
                SelectHandler?.Invoke();
            }
        }

        public Action SelectHandler { get; set; }
    }
}
