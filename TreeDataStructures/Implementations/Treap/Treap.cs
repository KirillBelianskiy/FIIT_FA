using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.Treap;

public class Treap<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, TreapNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    protected override TreapNode<TKey, TValue> CreateNode(TKey key, TValue value)
    {
        return new TreapNode<TKey, TValue>(key, value);
    }

    public Treap()
    {
    }

    /// <summary>
    /// Разрезает дерево с корнем <paramref name="root"/> на два поддерева:
    /// Left: все ключи <= <paramref name="key"/>
    /// Right: все ключи > <paramref name="key"/>
    /// </summary>
    protected virtual (TreapNode<TKey, TValue>? Left, TreapNode<TKey, TValue>? Right) Split(
        TreapNode<TKey, TValue>? root, TKey key)
    {
        if (root == null) return (null, null);

        if (Comparer.Compare(root.Key, key) <= 0)
        {
            var (left, right) = Split(root.Right, key);
            root.Right = left;
            return (root, right);
        }
        else
        {
            var (left, right) = Split(root.Left, key);
            root.Left = right;
            return (left, root);
        }
    }

    /// <summary>
    /// Сливает два дерева в одно.
    /// Важное условие: все ключи в <paramref name="left"/> должны быть меньше ключей в <paramref name="right"/>.
    /// Слияние происходит на основе Priority (куча).
    /// </summary>
    protected virtual TreapNode<TKey, TValue>? Merge(TreapNode<TKey, TValue>? left, TreapNode<TKey, TValue>? right)
    {
        if (left == null) return right;
        if (right == null) return left;

        if (left.Priority > right.Priority)
        {
            left.Right = Merge(left.Right, right);
            if (left.Right != null) left.Right.Parent = left;
            return left;
        }
        else
        {
            right.Left = Merge(left, right.Left);
            if (right.Left != null) right.Left.Parent = right;
            return right;
        }
    }


    public override void Add(TKey key, TValue value)
    {
        var (left, right) = Split(Root, key);
        var newNode = CreateNode(key, value);
        Root = Merge(Merge(left, newNode), right);
        if (Root != null) Root.Parent = null;
        this.Count++;
    }

    public override bool Remove(TKey key)
    {
        if (!ContainsKey(key)) return false;

        Root = RemoveNode(Root, key);
        if (Root != null) Root.Parent = null;
        Count--;
        return true;
    }

    protected virtual TreapNode<TKey, TValue>? RemoveNode(TreapNode<TKey, TValue>? root, TKey key)
    {
        if (root == null) return null;

        int cmp = Comparer.Compare(key, root.Key);

        if (cmp < 0)
        {
            root.Left = RemoveNode(root.Left, key);
            if (root.Left != null) root.Left.Parent = root;
            return root;
        }

        if (cmp > 0)
        {
            root.Right = RemoveNode(root.Right, key);
            if (root.Right != null) root.Right.Parent = root;
            return root;
        }
        else
        {
            var merged = Merge(root.Left, root.Right);
            if (merged != null) merged.Parent = null;
            return merged;
        }
    }

    protected override void OnNodeAdded(TreapNode<TKey, TValue> newNode)
    {
    }

    protected override void OnNodeRemoved(TreapNode<TKey, TValue>? parent, TreapNode<TKey, TValue>? child)
    {
    }
}