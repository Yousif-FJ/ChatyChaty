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
        private bool isVisible;

        public bool IsVisible
        {
            get => isVisible;
            private set
            {
                if (value != isVisible)
                {
                    isVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public void Show()
        {
            IsVisible = true;
        }

        public void Hide()
        {
            IsVisible = false;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
