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

    public ICollection<TKey> Keys => InOrder().Select(x => x.Key).ToList();
    public ICollection<TValue> Values => InOrder().Select(x => x.Value).ToList();


    public virtual void Add(TKey key, TValue value)
    {
        if (Root == null)
        {
            Root = CreateNode(key, value);
            this.Count++;
            OnNodeAdded(Root);
            return;
        }

        TNode curr = Root;
        while (curr != null)
        {
            int cmp = Comparer.Compare(key, curr.Key);
            if (cmp < 0)
            {
                if (curr.Left == null)
                {
                    curr.Left = CreateNode(key, value);
                    curr.Left.Parent = curr;
                    this.Count++;
                    OnNodeAdded(curr.Left);
                    return;
                }
                curr = curr.Left;
            }
            else
            {
                if (curr.Right == null)
                {
                    curr.Right = CreateNode(key, value);
                    curr.Right.Parent = curr;
                    this.Count++;
                    OnNodeAdded(curr.Right);
                    return;
                }
                curr = curr.Right;
            }
        }
    }


    public virtual bool Remove(TKey key)
    {
        TNode? node = FindNode(key);
        if (node == null)
        {
            return false;
        }

        var parentForHook = node.Parent;
        var childForHook = node.Left ?? node.Right;

        if (node.Left != null && node.Right != null)
        {
            var min = FindMin(node.Right);
            if (min != null)
            {
                childForHook = min;
                if (min.Parent != node)
                {
                    parentForHook = min.Parent;
                }
            }
        }

        RemoveNode(node);
        this.Count--;
        OnNodeRemoved(parentForHook, childForHook);
        return true;
    }


    protected virtual void RemoveNode(TNode node)
    {
        if (node.Left == null)
        {
            Transplant(node, node.Right);
        }
        else if (node.Right == null)
        {
            Transplant(node, node.Left);
        }
        else
        {
            var min = FindMin(node.Right!);
            if (min != null && min.Parent != node)
            {
                Transplant(min, min.Right);
                min.Right = node.Right;
                if (min.Right != null) min.Right.Parent = min;
            }
            Transplant(node, min);
            if (min != null)
            {
                min.Left = node.Left;
                if (min.Left != null) min.Left.Parent = min;
            }
        }
    }
    
    protected TNode? FindMin(TNode node)
    {
        while (node.Left != null)
        {
            node = node.Left;
        }
        return node;
    }

    public virtual bool ContainsKey(TKey key) => TryGetValue(key, out _);

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
        set
        {
            TNode? node = FindNode(key);
            if (node != null)
            {
                node.Value = value;
            }
            else
            {
                Add(key, value);
            }
        }
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

    public IEnumerable<TreeEntry<TKey, TValue>> InOrder(bool isReverse = false) =>
        new TreeIterator(Root, isReverse ? TraversalStrategy.InOrderReverse : TraversalStrategy.InOrder);

    public IEnumerable<TreeEntry<TKey, TValue>> InOrder() => InOrder(false);
    public IEnumerable<TreeEntry<TKey, TValue>> InOrderReverse() => InOrder(true);

    private IEnumerable<TreeEntry<TKey, TValue>> InOrderTraversal(TNode? node)
    {
        if (node == null)
        {
            yield break;
        }

        var stack = new Stack<TNode?>();
        var current = node;

        while (current != null || stack.Count > 0)
        {
            while (current != null)
            {
                stack.Push(current);
                current = current.Left;
            }

            current = stack.Pop();
            if (current != null)
            {
                yield return new TreeEntry<TKey, TValue>(current.Key, current.Value, 0);
                current = current.Right;
            }
        }
    }

    public IEnumerable<TreeEntry<TKey, TValue>> PreOrder(bool isReverse = false) =>
        new TreeIterator(Root, isReverse ? TraversalStrategy.PreOrderReverse : TraversalStrategy.PreOrder);

    public IEnumerable<TreeEntry<TKey, TValue>> PreOrder() => PreOrder(false);
    public IEnumerable<TreeEntry<TKey, TValue>> PreOrderReverse() => PreOrder(true);

    public IEnumerable<TreeEntry<TKey, TValue>> PostOrder(bool isReverse = false) =>
        new TreeIterator(Root, isReverse ? TraversalStrategy.PostOrderReverse : TraversalStrategy.PostOrder);

    public IEnumerable<TreeEntry<TKey, TValue>> PostOrder() => PostOrder(false);
    public IEnumerable<TreeEntry<TKey, TValue>> PostOrderReverse() => PostOrder(true);

    /// <summary>
    /// Внутренний класс-итератор. 
    /// Реализует паттерн Iterator вручную, без yield return (ban).
    /// </summary>
    private struct TreeIterator :
        IEnumerable<TreeEntry<TKey, TValue>>,
        IEnumerator<TreeEntry<TKey, TValue>>
    {
        private readonly TNode? _root;
        private readonly TraversalStrategy _strategy;
        private Stack<TNode?> _stack;
        private Stack<TNode?>? _stackPO;
        private List<TNode?>? _resultList;
        private int _resultIndex;
        private TNode? _current;

        public TreeIterator(TNode? root, TraversalStrategy strategy)
        {
            _root = root;
            _strategy = strategy;
            _stack = new Stack<TNode?>();
            _stackPO = new Stack<TNode?>();
            _resultList = null;
            _resultIndex = -1;
            _current = null;
            Reset();
        }

        public IEnumerator<TreeEntry<TKey, TValue>> GetEnumerator() => this;
        IEnumerator IEnumerable.GetEnumerator() => this;

        public TreeEntry<TKey, TValue> Current => new(_current!.Key, _current!.Value, 0);
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            switch (_strategy)
            {
                case TraversalStrategy.InOrder:
                    return MoveNextInOrder();
                case TraversalStrategy.PreOrder:
                    return MoveNextPreOrder();
                case TraversalStrategy.PostOrder:
                    return MoveNextPostOrder();
                case TraversalStrategy.InOrderReverse:
                    return MoveNextInOrderReverse();
                case TraversalStrategy.PreOrderReverse:
                    return MoveNextPreOrderReverse();
                case TraversalStrategy.PostOrderReverse:
                    return MoveNextPostOrderReverse();
                default:
                    return false;
            }
        }

        public void Reset()
        {
            _stack.Clear();
            _stackPO?.Clear();
            _resultList = null;
            _resultIndex = -1;
            _current = null;

            switch (_strategy)
            {
                case TraversalStrategy.InOrder:
                    PushLeft(_root);
                    break;
                case TraversalStrategy.PreOrder:
                    _stack.Push(_root);
                    break;
                case TraversalStrategy.PostOrder:
                    InitPostOrder(_root);
                    break;
                case TraversalStrategy.InOrderReverse:
                    PushRight(_root);
                    break;
                case TraversalStrategy.PreOrderReverse:
                    InitPreOrderReverse(_root);
                    break;
                case TraversalStrategy.PostOrderReverse:
                    InitPostOrderReverse(_root);
                    break;
            }
        }
        
        private void InitPreOrderReverse(TNode? root)
        {
            _resultList = new List<TNode?>();
            CollectPreOrder(root, _resultList);
            _resultList.Reverse();
            _resultIndex = 0;
        }
        
        private void CollectPreOrder(TNode? node, List<TNode?> result)
        {
            if (node == null) return;

            var stack = new Stack<TNode?>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current == null) continue;

                result.Add(current);

                if (current.Right != null) stack.Push(current.Right);
                if (current.Left != null) stack.Push(current.Left);
            }
        }
        
        private void InitPostOrder(TNode? root)
        {
            if (root == null) return;
            _stack.Clear();
            _stackPO?.Clear();
            _stackPO = new Stack<TNode?>();
            
            var tempStack = new Stack<TNode?>();
            tempStack.Push(root);
            
            while (tempStack.Count > 0)
            {
                var node = tempStack.Pop();
                _stackPO!.Push(node);
                
                if (node?.Left != null) tempStack.Push(node.Left);
                if (node?.Right != null) tempStack.Push(node.Right);
            }
        }
        
        private void InitPostOrderReverse(TNode? root)
        {
            _resultList = new List<TNode?>();
            CollectPostOrder(root, _resultList);
            _resultList.Reverse();
            _resultIndex = 0;
        }
        
        private void CollectPostOrder(TNode? node, List<TNode?> result)
        {
            if (node == null) return;

            var first = new Stack<TNode?>();
            var second = new Stack<TNode?>();

            first.Push(node);

            while (first.Count > 0)
            {
                var current = first.Pop();
                if (current == null) continue;

                second.Push(current);

                if (current.Left != null) first.Push(current.Left);
                if (current.Right != null) first.Push(current.Right);
            }

            while (second.Count > 0)
            {
                result.Add(second.Pop());
            }
        }

        public void Dispose() { }

        private bool MoveNextInOrder()
        {
            if (_stack.Count == 0) return false;
            _current = _stack.Pop();
            if (_current != null && _current.Right != null) PushLeft(_current.Right);
            return true;
        }

        private bool MoveNextPreOrder()
        {
            if (_stack.Count == 0) return false;
            _current = _stack.Pop();
            if (_current != null)
            {
                if (_current.Right != null) _stack.Push(_current.Right);
                if (_current.Left != null) _stack.Push(_current.Left);
            }
            return true;
        }

        private bool MoveNextPostOrder()
        {
            if (_stackPO == null || _stackPO.Count == 0) return false;
            _current = _stackPO.Pop();
            return _current != null;
        }

        private bool MoveNextInOrderReverse()
        {
            if (_stack.Count == 0) return false;
            _current = _stack.Pop();
            if (_current != null && _current.Left != null) PushRight(_current.Left);
            return true;
        }

        private bool MoveNextPreOrderReverse()
        {
            if (_resultList == null || _resultIndex >= _resultList.Count) return false;
            _current = _resultList[_resultIndex++];
            return _current != null;
        }

        private bool MoveNextPostOrderReverse()
        {
            if (_resultList == null || _resultIndex >= _resultList.Count) return false;
            _current = _resultList[_resultIndex++];
            return _current != null;
        }

        private void PushLeft(TNode? node)
        {
            while (node != null)
            {
                _stack.Push(node);
                node = node.Left;
            }
        }

        private void PushRight(TNode? node)
        {
            while (node != null)
            {
                _stack.Push(node);
                node = node.Right;
            }
        }

        private void PushLeftPostOrder(TNode? node)
        {
            while (node != null)
            {
                _stack.Push(node);
                if (node.Left != null)
                {
                    node = node.Left;
                }
                else
                {
                    node = node.Right;
                }
            }
        }

        private void PushRightPostOrder(TNode? node)
        {
            while (node != null)
            {
                _stack.Push(node);
                if (node.Right != null)
                {
                    node = node.Right;
                }
                else
                {
                    node = node.Left;
                }
            }
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
        return InOrder().Select(entry => new KeyValuePair<TKey, TValue>(entry.Key, entry.Value)).GetEnumerator();
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
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count) throw new ArgumentException();

        foreach (var entry in InOrder())
        {
            array[arrayIndex++] = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
        }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
}