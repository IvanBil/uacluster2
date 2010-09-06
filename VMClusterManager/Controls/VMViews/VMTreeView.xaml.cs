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
using VMClusterManager.Controls;
using System.Reflection;

namespace VMClusterManager
{
    /// <summary>
    /// Interaction logic for CustomTreeView.xaml
    /// </summary>
    public partial class VMTreeView : UserControl, IView
    {
        //private EditableTextBlock activeTB;
        public VMTreeView()
        {
            InitializeComponent();
            //SetViewModel(new VMTreeViewModel(VMModel.GetInstance()));
        }

        #region IView Members

        public void SetViewModel(object ViewModel)
        {
            this.DataContext = ViewModel;
            //(DataContext as VMTreeViewModel).RenameActiveTreeNodeRequested += new EventHandler<EventArgs>(VMTreeView_RenameActiveTreeNodeRequested);
        }

       

        #endregion

        //public void AllowTextInput(object selectedItem)
        //{

        //}


       

       

        //public void SetSelectedItem(ref TreeView control, object item)
        //{
        //    try
        //    {
        //        DependencyObject dObject = control
        //            .ItemContainerGenerator
        //            .ContainerFromItem(item);

        //        //uncomment the following line if UI updates are unnecessary
        //        ((TreeViewItem)dObject).IsSelected = true;               

        //        MethodInfo selectMethod =
        //           typeof(TreeViewItem).GetMethod("Select",
        //           BindingFlags.NonPublic | BindingFlags.Instance);

        //        selectMethod.Invoke(dObject, new object[] { true });
        //    }
        //    catch { }
        //}

        //private bool SetSelected(ItemsControl parent, object child)
        //{
        //    if (parent == null || child == null)
        //    {
        //        return false;
        //    }

        //    TreeViewItem childNode = parent.ItemContainerGenerator.ContainerFromItem(child) as TreeViewItem;
        //    if (childNode != null)
        //    {
        //        //(parent as TreeViewItem).IsExpanded = true;
        //        childNode.Focus();
        //        //if (childNode is EditableTextBlock)
        //        //{
        //        //    activeTB = childNode.Item as EditableTextBlock;
        //        //}
        //        return childNode.IsSelected = true;
        //    }

        //    if (parent.Items.Count > 0)
        //    {
        //        foreach (object childItem in parent.Items)
        //        {
        //            ItemsControl childControl = parent.ItemContainerGenerator.ContainerFromItem(childItem) as ItemsControl;

        //            if (SetSelected(childControl, child))
        //            {
        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

        //public void NavigateTo(object node)
        //{
        //    SetSelectedItem(ref treeElements, node);
        //    //return SetSelected(treeElements, node);
        //}

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

        

        private void treeElements_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            (DataContext as VMTreeViewModel).SelectedItem = e.NewValue;
        }
    }
}
