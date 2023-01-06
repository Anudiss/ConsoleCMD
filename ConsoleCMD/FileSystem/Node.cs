using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ConsoleCMD
{
    /// <summary>
    /// Представляет узел панели навигации
    /// </summary>
    public class Node : INotifyPropertyChanged
    {
        private string _title;
        public string Title
        {
            get => _title;
            set => _title = value;
        }

        private ImageSource _iconSource;
        public ImageSource IconSource
        {
            get => _iconSource;
            set => _iconSource = value;
        }

        private Node[] _children;
        public Node[] Children
        {
            get => _children;
            set {
                _children = value;
                foreach (var child in _children)
                    child._parent = this;
            }
        }

        private Node _parent = null;
        public Node Parent
        {
            get => _parent;
            set => _parent = value;
        }

        private bool _isExpanded = false;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Создаёт новый узел
        /// </summary>
        /// <param name="title">Название</param>
        /// <param name="iconSource">Изображение</param>
        /// <param name="children">Массив подузлов</param>
        public Node(string title = "", ImageSource iconSource = null, Node[] children = null)
        {
            _title = title;
            _iconSource = iconSource ?? new BitmapImage(new Uri("/Resources/Icons/directory.png", UriKind.Relative));
            _children = children ?? new Node[0];
        }

        /// <summary>
        /// Выполняет действие для каждого ребёнка
        /// </summary>
        /// <param name="action">Действие</param>
        public void ForEachChildren(Action<Node> action)
        {
            action(this);
            foreach (var childNode in _children)
                childNode.ForEachChildren(action);
        }

        /// <summary>
        /// Выполняет действие для каждого родителя
        /// </summary>
        /// <param name="action">Действие</param>
        public void ForEachParent(Action<Node> action)
        {
            if (Parent == null)
                return;
            action(Parent);
            Parent.ForEachParent(action);
        }

        /// <summary>
        /// Выполняет действие для каждого соседа
        /// </summary>
        /// <param name="action"></param>
        public void ForEachNeighbour(Action<Node> action)
        {
            if (Parent == null)
                return;
            foreach (var neighbourNode in Parent._children)
                if (neighbourNode != this)
                    action(neighbourNode);
        }
    }
}
