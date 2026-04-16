using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.AVL;

public class AvlTree<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, AvlNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    protected override AvlNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);
    
    protected override void OnNodeAdded(AvlNode<TKey, TValue> newNode)
    {
        AvlNode<TKey, TValue>? current = newNode;
        while (current != null)
        {
            UpdateHeight(current);
            current = Rebalance(current).Parent;
        }
    }

    protected override void OnNodeRemoved(AvlNode<TKey, TValue>? parent, AvlNode<TKey, TValue>? child)
    {
        AvlNode<TKey, TValue>? current = parent ?? child;
        while (current != null)
        {
            UpdateHeight(current);
            current = Rebalance(current).Parent;
        }
    }

    private static int Height(AvlNode<TKey, TValue>? node)
    {
        return node?.Height ?? 0;
    }

    private static void UpdateHeight(AvlNode<TKey, TValue> node)
    {
        int leftHeight = Height(node.Left);
        int rightHeight = Height(node.Right);
        node.Height = Math.Max(leftHeight, rightHeight) + 1;
    }

    private static int Balance(AvlNode<TKey, TValue> node)
    {
        return Height(node.Left) - Height(node.Right);
    }

    private AvlNode<TKey, TValue> Rebalance(AvlNode<TKey, TValue> node)
    {
        int balance = Balance(node);

        if (balance > 1)
        {
            if (node.Left != null && Balance(node.Left) < 0)
            {
                RotateLeftLocal(node.Left);
            }

            return RotateRightLocal(node);
        }

        if (balance < -1)
        {
            if (node.Right != null && Balance(node.Right) > 0)
            {
                RotateRightLocal(node.Right);
            }

            return RotateLeftLocal(node);
        }

        return node;
    }

    private AvlNode<TKey, TValue> RotateLeftLocal(AvlNode<TKey, TValue> x)
    {
        AvlNode<TKey, TValue>? y = x.Right;
        if (y == null)
        {
            return x;
        }

        AvlNode<TKey, TValue>? beta = y.Left;

        y.Parent = x.Parent;
        if (x.Parent == null)
        {
            Root = y;
        }
        else if (x.IsLeftChild)
        {
            x.Parent.Left = y;
        }
        else
        {
            x.Parent.Right = y;
        }

        y.Left = x;
        x.Parent = y;

        x.Right = beta;
        if (beta != null)
        {
            beta.Parent = x;
        }

        UpdateHeight(x);
        UpdateHeight(y);
        return y;
    }

    private AvlNode<TKey, TValue> RotateRightLocal(AvlNode<TKey, TValue> y)
    {
        AvlNode<TKey, TValue>? x = y.Left;
        if (x == null)
        {
            return y;
        }

        AvlNode<TKey, TValue>? beta = x.Right;

        x.Parent = y.Parent;
        if (y.Parent == null)
        {
            Root = x;
        }
        else if (y.IsLeftChild)
        {
            y.Parent.Left = x;
        }
        else
        {
            y.Parent.Right = x;
        }

        x.Right = y;
        y.Parent = x;

        y.Left = beta;
        if (beta != null)
        {
            beta.Parent = y;
        }

        UpdateHeight(y);
        UpdateHeight(x);
        return x;
    }
}