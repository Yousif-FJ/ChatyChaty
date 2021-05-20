using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ChatyChatyClient.Blazor.ViewModel
{
    public class LoadingIndicator : INotifyPropertyChanged
    {
        private bool IsVisible;

        public bool Value
        {
            get => IsVisible;
            private set
            {
                if (value != IsVisible)
                {
                    IsVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public void Show()
        {
            Value = true;
        }

        public void Hide()
        {
            Value = false;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
