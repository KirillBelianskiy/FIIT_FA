using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.Treap;

public class Treap<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, TreapNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    /// <summary>
    /// Разрезает дерево с корнем <paramref name="root"/> на два поддерева:
    /// Left: все ключи меньше или равны <paramref name="key"/>
    /// Right: все ключи строго больше <paramref name="key"/>
    /// </summary>
    protected virtual (TreapNode<TKey, TValue>? Left, TreapNode<TKey, TValue>? Right) Split(
        TreapNode<TKey, TValue>? root, TKey key)
    {
        if (root == null)
        {
            return (null, null);
        }

        int cmp = Comparer.Compare(root.Key, key);
        if (cmp <= 0)
        {
            (TreapNode<TKey, TValue>? left, TreapNode<TKey, TValue>? right) = Split(root.Right, key);
            root.Right = left;
            if (root.Right != null)
            {
                root.Right.Parent = root;
            }

            root.Parent = null;
            if (right != null)
            {
                right.Parent = null;
            }

            return (root, right);
        }

        (TreapNode<TKey, TValue>? leftPart, TreapNode<TKey, TValue>? rightPart) = Split(root.Left, key);
        root.Left = rightPart;
        if (root.Left != null)
        {
            root.Left.Parent = root;
        }

        root.Parent = null;
        if (leftPart != null)
        {
            leftPart.Parent = null;
        }

        return (leftPart, root);
    }

    /// <summary>
    /// Сливает два дерева в одно.
    /// Важное условие: все ключи в <paramref name="left"/> должны быть меньше ключей в <paramref name="right"/>.
    /// Слияние происходит на основе Priority (куча).
    /// </summary>
    protected virtual TreapNode<TKey, TValue>? Merge(TreapNode<TKey, TValue>? left, TreapNode<TKey, TValue>? right)
    {
        if (left == null)
        {
            if (right != null)
            {
                right.Parent = null;
            }

            return right;
        }

        if (right == null)
        {
            left.Parent = null;
            return left;
        }

        if (left.Priority > right.Priority)
        {
            TreapNode<TKey, TValue>? mergedRight = Merge(left.Right, right);
            left.Right = mergedRight;
            if (left.Right != null)
            {
                left.Right.Parent = left;
            }

            left.Parent = null;
            return left;
        }

        TreapNode<TKey, TValue>? mergedLeft = Merge(left, right.Left);
        right.Left = mergedLeft;
        if (right.Left != null)
        {
            right.Left.Parent = right;
        }

        right.Parent = null;
        return right;
    }


    public override void Add(TKey key, TValue value)
    {
        TreapNode<TKey, TValue>? existing = FindNode(key);
        if (existing != null)
        {
            existing.Value = value;
            return;
        }

        TreapNode<TKey, TValue> node = CreateNode(key, value);
        (TreapNode<TKey, TValue>? left, TreapNode<TKey, TValue>? right) = Split(Root, key);
        TreapNode<TKey, TValue>? mergedLeft = Merge(left, node);
        Root = Merge(mergedLeft, right);
        if (Root != null)
        {
            Root.Parent = null;
        }

        Count++;
        OnNodeAdded(node);
    }

    public override bool Remove(TKey key)
    {
        TreapNode<TKey, TValue>? node = FindNode(key);
        if (node == null)
        {
            return false;
        }

        TreapNode<TKey, TValue>? parent = node.Parent;
        TreapNode<TKey, TValue>? merged = Merge(node.Left, node.Right);

        if (parent == null)
        {
            Root = merged;
            if (Root != null)
            {
                Root.Parent = null;
            }

            OnNodeRemoved(null, Root);
        }
        else if (node.IsLeftChild)
        {
            parent.Left = merged;
            if (merged != null)
            {
                merged.Parent = parent;
            }

            OnNodeRemoved(parent, merged);
        }
        else
        {
            parent.Right = merged;
            if (merged != null)
            {
                merged.Parent = parent;
            }

            OnNodeRemoved(parent, merged);
        }

        Count--;
        return true;
    }

    protected override TreapNode<TKey, TValue> CreateNode(TKey key, TValue value)
    {
        return new TreapNode<TKey, TValue>(key, value);
    }

    protected override void OnNodeAdded(TreapNode<TKey, TValue> newNode)
    {
    }

    protected override void OnNodeRemoved(TreapNode<TKey, TValue>? parent, TreapNode<TKey, TValue>? child)
    {
    }
}