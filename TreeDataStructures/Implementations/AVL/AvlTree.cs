using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.AVL;

public class AvlTree<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, AvlNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    protected override AvlNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);

    protected override void OnNodeAdded(AvlNode<TKey, TValue> newNode)
    {
        BalanceTreeAfterChange(newNode);
    }

    protected override void OnNodeRemoved(AvlNode<TKey, TValue>? parent, AvlNode<TKey, TValue>? child)
    {
        BalanceTreeAfterChange(parent ?? child);
    }

    private int Height(AvlNode<TKey, TValue>? node) => node?.Height ?? 0;

    private void FixHeight(AvlNode<TKey, TValue> node)
    {
        int hl = Height(node.Left);
        int hr = Height(node.Right);
        node.Height = (hl > hr ? hl : hr) + 1;
    }

    private int Balance(AvlNode<TKey, TValue> node)
    {
        return Height(node.Left) - Height(node.Right);
    }

    private void BalanceTreeAfterChange(AvlNode<TKey, TValue>? node)
    {
        while (node != null)
        {
            FixHeight(node);
            int bf = Balance(node);

            if (bf > 1)
            {
                if (Balance(node.Left!) < 0)
                    RotateDoubleRight(node);
                else
                    RotateRight(node);

                var newRoot = node.Parent;
                if (newRoot != null)
                {
                    FixHeight(node);
                    FixHeight(newRoot);
                }
                node = newRoot?.Parent;
            }
            else if (bf < -1)
            {
                if (Balance(node.Right!) > 0)
                    RotateDoubleLeft(node);
                else
                    RotateLeft(node);

                var newRoot = node.Parent;
                if (newRoot != null)
                {
                    FixHeight(node);
                    FixHeight(newRoot);
                }
                node = newRoot?.Parent;
            }
            else
            {
                node = node.Parent;
            }
        }
    }
}