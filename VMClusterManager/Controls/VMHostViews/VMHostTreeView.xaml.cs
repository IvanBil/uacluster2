using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VMClusterManager.ViewModels.VMHostModels;

namespace VMClusterManager.Controls.VMHostViews
{
    /// <summary>
    /// Interaction logic for VMHostTreeView.xaml
    /// </summary>
    public partial class VMHostTreeView : UserControl, IView
    {
        private EditableTextBlock activeTB;
        public VMHostTreeView()
        {
            InitializeComponent();
            //this.SetViewModel(new VMHostTreeViewModel(VMModel.GetInstance()));
        }

        #region IView Members

        public void SetViewModel(object ViewModel)
        {
            this.DataContext = ViewModel;
            //(DataContext as VMHostTreeViewModel).RenameActiveTreeNodeRequested += new EventHandler<EventArgs>(VMHostTreeView_RenameActiveTreeNodeRequested);
        }

       
        #endregion

        private void treeElements_LayoutUpdated(object sender, EventArgs e)
        {
            //(sender as TreeView).Height = (sender as TreeView).ActualHeight; 
        }

        private void treeElements_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            (DataContext as VMHostTreeViewModel).SelectedItem = e.NewValue;
        }

        private void EditableTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //activeTB = e.Source as EditableTextBlock;
        }

        private void EditableTextBlock_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void EditableTextBlock_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            try
            {
                EditableTextBlock activeTB = sender as EditableTextBlock;
                object Node = activeTB.DataContext;
                if (Node != null)
                {
                    if (Node is Group)
                    {
                        (Node as Group).Save();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "EditableTextBlock_SourceUpdated", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
    }
}
