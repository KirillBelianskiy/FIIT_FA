using System.Diagnostics.CodeAnalysis;
using TreeDataStructures.Implementations.BST;

namespace TreeDataStructures.Implementations.Splay;

public class SplayTree<TKey, TValue> : BinarySearchTree<TKey, TValue>
    where TKey : IComparable<TKey>
{
    protected override BstNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);

    private void Splay(BstNode<TKey, TValue>? node)
    {
        if (node == null) return;

        while (node.Parent != null)
        {
            var parent = node.Parent;
            var grandParent = parent.Parent;

            if (grandParent == null)
            {
                // Zig
                if (node == parent.Left)
                    RotateRight(parent);
                else
                    RotateLeft(parent);
            }
            else
            {
                bool nodeIsLeft = (node == parent.Left);
                bool parentIsLeft = (parent == grandParent.Left);

                if (nodeIsLeft && parentIsLeft)
                {
                    // Zig-Zig (left-left)
                    RotateDoubleRight(grandParent);
                }
                else if (!nodeIsLeft && !parentIsLeft)
                {
                    // Zig-Zig (right-right)
                    RotateDoubleLeft(grandParent);
                }
                else if (nodeIsLeft && !parentIsLeft)
                {
                    // Zig-Zag (left-right)
                    RotateBigLeft(grandParent);
                }
                else
                {
                    // Zig-Zag (right-left)
                    RotateBigRight(grandParent);
                }
            }
        }

        Root = node;
    }


    protected override void OnNodeAdded(BstNode<TKey, TValue> newNode)
    {
        Splay(newNode);
    }

    protected override void OnNodeRemoved(BstNode<TKey, TValue>? parent, BstNode<TKey, TValue>? child)
    {
        if (parent != null)
        {
            Splay(parent);
        }
        else if (child != null)
        {
            Splay(child);
        }
    }

    public override bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        var node = FindNode(key);
        if (node != null)
        {
            value = node.Value;
            Splay(node);
            return true;
        }
        else
        {
            var current = Root;
            BstNode<TKey, TValue>? lastVisited = null;
            while (current != null)
            {
                lastVisited = current;
                int cmp = Comparer.Compare(key, current.Key);
                if (cmp == 0)
                    break;
                current = cmp < 0 ? current.Left : current.Right;
            }
            Splay(lastVisited);
            value = default;
            return false;
        }
    }
}