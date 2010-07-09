using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace VMClusterManager
{

    public class TreeViewElement : INotifyPropertyChanged
    {
        private bool isSelected;

        public bool IsSelected
        {
            get { return isSelected; }
            set 
            { 
                isSelected = value;
                OnSelectedItemChanged();
                OnPropertyChanged("IsSelected");
            }
        }

        private object content;

        public object Content
        {
            get { return content; }
            set { content = value; }
        }
        public event EventHandler<EventArgs> SelectedItemChanged;

        protected void OnSelectedItemChanged()
        {
            if (SelectedItemChanged != null)
                SelectedItemChanged(this, new EventArgs());
        }

        public TreeViewElement(object _content)
        {
            this.Content = _content;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
