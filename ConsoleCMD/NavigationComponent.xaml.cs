using System;
using System.Windows;
using System.Windows.Controls;

namespace ConsoleCMD
{
    /// <summary>
    /// Панель навигации на основе иерархии директорий
    /// </summary>
    public partial class NavigationComponent : UserControl
    {
        /// <summary>
        /// Корневой узел дерева навигации
        /// </summary>
        private Node[] _rootNodes = new Node[1] { null };
        public Node RootNode
        {
            get { return _rootNodes[0]; }
            set { _rootNodes[0] = value; }
        }

        /// <summary>
        /// Обработчик события, когда выбран конечный узел навигации (тот, который не содержит подузлов)
        /// </summary>
        public Action<Node> SelectionChanged;
        
        public NavigationComponent()
        {
            InitializeComponent();

            NavigationTreeView.ItemsSource = _rootNodes;
        }

        /// <summary>
        /// При выборе элемента в дереве, вызывает обработчик SelectedEndNode
        /// </summary>
        private void NavigationTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedNode = NavigationTreeView.SelectedItem as Node;

            // reset visibility
            RootNode.ForEachChildren(node => node.IsVisible = true);

            selectedNode.ForEachNeighbour(neighbour => neighbour.IsVisible = false);

            selectedNode.ForEachParent(parent => parent.ForEachNeighbour(neighbour => neighbour.IsVisible = false));

            SelectionChanged?.Invoke(selectedNode);
        }

        /// <summary>
        /// Передаёет фокус дереву дереву навигации
        /// </summary>
        public new void Focus() => NavigationTreeView.Focus();
    }
}
