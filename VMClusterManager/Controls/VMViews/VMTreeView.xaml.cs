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
        private EditableTextBlock activeTB;
        public VMTreeView()
        {
            InitializeComponent();
        }

        #region IView Members

        public void SetViewModel(object ViewModel)
        {
            this.DataContext = ViewModel;
        }

        #endregion

        //public void AllowTextInput(object selectedItem)
        //{

        //}

        private void treeElements_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            object selectedItem = e.NewValue;
            
            VMTreeViewModel treeViewModel = (this.DataContext as VMTreeViewModel);
            if (selectedItem is VMGroup)
            {
                treeViewModel.ActiveVMGroup = selectedItem as VMGroup;
            }
            else
            {
                ////Get Parent Group of VM
                //TreeViewItem item = e.OriginalSource as TreeViewItem;
                //if (item != null)
                //{
                //    ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(item);
                //    if (parent != null)
                //    {
                //        treeViewModel.ActiveVMGroup = parent.DataContext as VMGroup;
                //    }
                //}
                treeViewModel.ActiveVMGroup = null;
            }
            treeViewModel.SelectedItem = selectedItem;
        }

        private void treeElements_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
            {
                if (activeTB != null)
                {
                    if (activeTB.IsEditable)
                    {
                        activeTB.IsInEditMode = true;
                    }
                }
            }
            if (e.Key == Key.Enter)
            {
                if (activeTB.IsInEditMode)
                {
                    BindingExpression bindExp = activeTB.GetBindingExpression(EditableTextBlock.TextProperty);
                    bindExp.UpdateSource();
                }
            }
        }

        public void RenameActiveTreeNode()
        {
            if (activeTB != null)
            {
                if (activeTB.IsEditable)
                {
                    activeTB.IsInEditMode = true;
                }
            }
        }

        private void EditableTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            activeTB = e.Source as EditableTextBlock;
        }

        public void SetSelectedItem(ref TreeView control, object item)
        {
            try
            {
                DependencyObject dObject = control
                    .ItemContainerGenerator
                    .ContainerFromItem(item);

                //uncomment the following line if UI updates are unnecessary
                ((TreeViewItem)dObject).IsSelected = true;               

                MethodInfo selectMethod =
                   typeof(TreeViewItem).GetMethod("Select",
                   BindingFlags.NonPublic | BindingFlags.Instance);

                selectMethod.Invoke(dObject, new object[] { true });
            }
            catch { }
        }

        private bool SetSelected(ItemsControl parent, object child)
        {
            if (parent == null || child == null)
            {
                return false;
            }

            TreeViewItem childNode = parent.ItemContainerGenerator.ContainerFromItem(child) as TreeViewItem;

            if (childNode != null)
            {
                //(parent as TreeViewItem).IsExpanded = true;
                childNode.Focus();
                //if (childNode is EditableTextBlock)
                //{
                //    activeTB = childNode.Item as EditableTextBlock;
                //}
                return childNode.IsSelected = true;
            }

            if (parent.Items.Count > 0)
            {
                foreach (object childItem in parent.Items)
                {
                    ItemsControl childControl = parent.ItemContainerGenerator.ContainerFromItem(childItem) as ItemsControl;

                    if (SetSelected(childControl, child))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void NavigateTo(object node)
        {
            SetSelectedItem(ref treeElements, node);
            //return SetSelected(treeElements, node);
        }

        private void EditableTextBlock_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            object Node = activeTB.DataContext;
            //object target = e.TargetObject;
            if (Node != null)
            {
                if (Node is Group)
                {
                    (Node as Group).Save();
                }
            }
        }

        private void EditableTextBlock_GotFocus(object sender, RoutedEventArgs e)
        {
            activeTB = e.Source as EditableTextBlock;
        }
    }
}
