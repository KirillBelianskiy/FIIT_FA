using System.Collections;
using System.Diagnostics.CodeAnalysis;
using TreeDataStructures.Interfaces;

namespace TreeDataStructures.Core;

public abstract class BinarySearchTreeBase<TKey, TValue, TNode>(IComparer<TKey>? comparer = null)
    : ITree<TKey, TValue>
    where TNode : Node<TKey, TValue, TNode>
{
    protected TNode? Root;

    public IComparer<TKey> Comparer { get; protected set; } =
        comparer ?? Comparer<TKey>.Default; // use it to compare Keys

    public int Count { get; protected set; }

    public bool IsReadOnly => false;

    public ICollection<TKey> Keys
    {
        get
        {
            List<TKey> list = new();
            foreach (TreeEntry<TKey, TValue> item in InOrder())
            {
                list.Add(item.Key);
            }

            return list;
        }
    }

    public ICollection<TValue> Values
    {
        get
        {
            List<TValue> list = new();
            foreach (TreeEntry<TKey, TValue> item in InOrder())
            {
                list.Add(item.Value);
            }

            return list;
        }
    }


    public virtual void Add(TKey key, TValue value)
    {
        if (Root == null)
        {
            Root = CreateNode(key, value);
            Count = 1;
            OnNodeAdded(Root);
            return;
        }

        TNode? current = Root;
        TNode? parent = null;
        int cmp = 0;

        while (current != null)
        {
            parent = current;
            cmp = Comparer.Compare(key, current.Key);

            if (cmp == 0)
            {
                current.Value = value;
                return;
            }

            current = cmp < 0 ? current.Left : current.Right;
        }

        TNode newNode = CreateNode(key, value);
        newNode.Parent = parent;

        if (cmp < 0)
        {
            parent!.Left = newNode;
        }
        else
        {
            parent!.Right = newNode;
        }

        Count++;
        OnNodeAdded(newNode);
    }


    public virtual bool Remove(TKey key)
    {
        TNode? node = FindNode(key);
        if (node == null)
        {
            return false;
        }

        RemoveNode(node);
        this.Count--;
        return true;
    }


    protected void RemoveNode(TNode node)
    {
        if (node.Left == null)
        {
            TNode? parent = node.Parent;
            TNode? child = node.Right;
            Transplant(node, node.Right);
            OnNodeRemoved(parent, child);
            return;
        }

        if (node.Right == null)
        {
            TNode? parent = node.Parent;
            TNode? child = node.Left;
            Transplant(node, node.Left);
            OnNodeRemoved(parent, child);
            return;
        }

        TNode successor = GetMin(node.Right);

        if (successor.Parent != node)
        {
            TNode? parent = successor.Parent;
            TNode? child = successor.Right;

            Transplant(successor, successor.Right);
            successor.Right = node.Right;
            successor.Right!.Parent = successor;

            Transplant(node, successor);
            successor.Left = node.Left;
            successor.Left!.Parent = successor;

            OnNodeRemoved(parent, child);
            return;
        }

        Transplant(node, successor);
        successor.Left = node.Left;
        successor.Left!.Parent = successor;
        OnNodeRemoved(successor, successor.Right);
    }

    public virtual bool ContainsKey(TKey key) => FindNode(key) != null;

    public virtual bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        TNode? node = FindNode(key);
        if (node != null)
        {
            value = node.Value;
            return true;
        }

        value = default;
        return false;
    }

    public TValue this[TKey key]
    {
        get => TryGetValue(key, out TValue? val) ? val : throw new KeyNotFoundException();
        set => Add(key, value);
    }


    #region Hooks

    /// <summary>
    /// Вызывается после успешной вставки
    /// </summary>
    /// <param name="newNode">Узел, который встал на место</param>
    protected virtual void OnNodeAdded(TNode newNode)
    {
    }

    /// <summary>
    /// Вызывается после удаления. 
    /// </summary>
    /// <param name="parent">Узел, чей ребенок изменился</param>
    /// <param name="child">Узел, который встал на место удаленного</param>
    protected virtual void OnNodeRemoved(TNode? parent, TNode? child)
    {
    }

    #endregion


    #region Helpers

    protected abstract TNode CreateNode(TKey key, TValue value);


    protected TNode? FindNode(TKey key)
    {
        TNode? current = Root;
        while (current != null)
        {
            int cmp = Comparer.Compare(key, current.Key);
            if (cmp == 0)
            {
                return current;
            }

            current = cmp < 0 ? current.Left : current.Right;
        }

        return null;
    }

    protected void RotateLeft(TNode x)
    {
        throw new NotImplementedException();
    }

    protected void RotateRight(TNode y)
    {
        throw new NotImplementedException();
    }

    protected void RotateBigLeft(TNode x)
    {
        throw new NotImplementedException();
    }

    protected void RotateBigRight(TNode y)
    {
        throw new NotImplementedException();
    }

    protected void RotateDoubleLeft(TNode x)
    {
        throw new NotImplementedException();
    }

    protected void RotateDoubleRight(TNode y)
    {
        throw new NotImplementedException();
    }

    protected void Transplant(TNode u, TNode? v)
    {
        if (u.Parent == null)
        {
            Root = v;
        }
        else if (u.IsLeftChild)
        {
            u.Parent.Left = v;
        }
        else
        {
            u.Parent.Right = v;
        }

        v?.Parent = u.Parent;
    }

    #endregion

    public IEnumerable<TreeEntry<TKey, TValue>> InOrder() => new TreeIterator(Root, TraversalStrategy.InOrder);
    public IEnumerable<TreeEntry<TKey, TValue>> PreOrder() => new TreeIterator(Root, TraversalStrategy.PreOrder);
    public IEnumerable<TreeEntry<TKey, TValue>> PostOrder() => new TreeIterator(Root, TraversalStrategy.PostOrder);

    public IEnumerable<TreeEntry<TKey, TValue>> InOrderReverse() =>
        new TreeIterator(Root, TraversalStrategy.InOrderReverse);

    public IEnumerable<TreeEntry<TKey, TValue>> PreOrderReverse() =>
        new TreeIterator(Root, TraversalStrategy.PreOrderReverse);

    public IEnumerable<TreeEntry<TKey, TValue>> PostOrderReverse() =>
        new TreeIterator(Root, TraversalStrategy.PostOrderReverse);

    /// <summary>
    /// Внутренний класс-итератор. 
    /// Реализует паттерн Iterator вручную, без yield return (ban).
    /// </summary>
    private struct TreeIterator :
        IEnumerable<TreeEntry<TKey, TValue>>,
        IEnumerator<TreeEntry<TKey, TValue>>
    {
        private readonly TraversalStrategy _strategy;
        private readonly TNode? _startNode;
        private Stack<TNode>? _stack;
        private Stack<TNode>? _postOrderStack;
        private TreeEntry<TKey, TValue> _current;

        public TreeIterator(TNode? root, TraversalStrategy strategy)
        {
            _strategy = strategy;
            _startNode = root;
            _stack = null;
            _postOrderStack = null;
            _current = default;
        }

        public IEnumerator<TreeEntry<TKey, TValue>> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        public TreeEntry<TKey, TValue> Current
        {
            get { return _current; }
        }

        object IEnumerator.Current => Current;


        public bool MoveNext()
        {
            return _strategy switch
            {
                TraversalStrategy.InOrder => MoveNextInOrder(reverse: false),
                TraversalStrategy.InOrderReverse => MoveNextInOrder(reverse: true),
                TraversalStrategy.PreOrder => MoveNextPreOrder(reverse: false),
                TraversalStrategy.PostOrderReverse => MoveNextPreOrder(reverse: true),
                TraversalStrategy.PostOrder => MoveNextPostOrder(reverse: false),
                TraversalStrategy.PreOrderReverse => MoveNextPostOrder(reverse: true),
                _ => throw new ArgumentOutOfRangeException(nameof(_strategy), _strategy, null),
            };
        }

        public void Reset()
        {
            _stack = null;
            _postOrderStack = null;
            _current = default;
        }


        public void Dispose()
        {
        }

        private bool MoveNextPreOrder(bool reverse)
        {
            if (_stack == null)
            {
                _stack = new Stack<TNode>();
                if (_startNode != null)
                {
                    _stack.Push(_startNode);
                }
            }

            if (_stack.Count == 0)
            {
                return false;
            }

            TNode node = _stack.Pop();
            _current = new TreeEntry<TKey, TValue>(node.Key, node.Value, GetNodeLevel(node));

            TNode? firstChild = reverse ? node.Right : node.Left;
            TNode? secondChild = reverse ? node.Left : node.Right;

            if (secondChild != null)
            {
                _stack.Push(secondChild);
            }

            if (firstChild != null)
            {
                _stack.Push(firstChild);
            }

            return true;
        }

        private bool MoveNextInOrder(bool reverse)
        {
            if (_stack == null)
            {
                _stack = new Stack<TNode>();
                TNode? current = _startNode;
                while (current != null)
                {
                    _stack.Push(current);
                    current = reverse ? current.Right : current.Left;
                }
            }

            if (_stack.Count == 0)
            {
                return false;
            }

            TNode node = _stack.Pop();
            _current = new TreeEntry<TKey, TValue>(node.Key, node.Value, GetNodeLevel(node));

            TNode? next = reverse ? node.Left : node.Right;
            while (next != null)
            {
                _stack.Push(next);
                next = reverse ? next.Right : next.Left;
            }

            return true;
        }

        private bool MoveNextPostOrder(bool reverse)
        {
            if (_postOrderStack == null)
            {
                _stack = new Stack<TNode>();
                _postOrderStack = new Stack<TNode>();

                if (_startNode != null)
                {
                    _stack.Push(_startNode);
                }

                while (_stack.Count > 0)
                {
                    TNode node = _stack.Pop();
                    _postOrderStack.Push(node);

                    if (reverse)
                    {
                        if (node.Right != null)
                        {
                            _stack.Push(node.Right);
                        }

                        if (node.Left != null)
                        {
                            _stack.Push(node.Left);
                        }
                    }
                    else
                    {
                        if (node.Left != null)
                        {
                            _stack.Push(node.Left);
                        }

                        if (node.Right != null)
                        {
                            _stack.Push(node.Right);
                        }
                    }
                }
            }

            if (_postOrderStack.Count == 0)
            {
                return false;
            }

            TNode current = _postOrderStack.Pop();
            _current = new TreeEntry<TKey, TValue>(current.Key, current.Value, GetNodeLevel(current));
            return true;
        }

        private static int GetNodeLevel(TNode node)
        {
            int level = 0;
            TNode? current = node;
            while (current.Parent != null)
            {
                level++;
                current = current.Parent;
            }

            return level;
        }
    }


    private enum TraversalStrategy
    {
        InOrder,
        PreOrder,
        PostOrder,
        InOrderReverse,
        PreOrderReverse,
        PostOrderReverse
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        List<KeyValuePair<TKey, TValue>> pairs = new();
        foreach (TreeEntry<TKey, TValue> item in InOrder())
        {
            pairs.Add(new KeyValuePair<TKey, TValue>(item.Key, item.Value));
        }

        return pairs.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

    public void Clear()
    {
        Root = null;
        Count = 0;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item) => ContainsKey(item.Key);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        if (arrayIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }

        if (array.Length - arrayIndex < Count)
        {
            throw new ArgumentException("Destination array is too small.", nameof(array));
        }

        int currentIndex = arrayIndex;
        foreach (TreeEntry<TKey, TValue> entry in InOrder())
        {
            KeyValuePair<TKey, TValue> pair = new(entry.Key, entry.Value);
            array[currentIndex] = pair;
            currentIndex++;
        }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);

    private TNode GetMin(TNode node)
    {
        TNode current = node;
        while (current.Left != null)
        {
            current = current.Left;
        }

        return current;
    }
}