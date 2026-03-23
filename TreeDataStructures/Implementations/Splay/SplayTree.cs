using System.Diagnostics.CodeAnalysis;
using TreeDataStructures.Implementations.BST;

namespace TreeDataStructures.Implementations.Splay;

public class SplayTree<TKey, TValue> : BinarySearchTree<TKey, TValue>
    where TKey : IComparable<TKey>
{
    protected override BstNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);

    public SplayTree()
    {
    }

    private new void RotateLeft(BstNode<TKey, TValue> node)
    {
        if (node.Right == null) return;

        var right = node.Right;
        var leftOfRight = right.Left;
        var parent = node.Parent;

        node.Right = leftOfRight;
        if (leftOfRight != null) leftOfRight.Parent = node;

        right.Left = node;
        node.Parent = right;

        right.Parent = parent;
        if (parent != null)
        {
            if (parent.Left == node)
                parent.Left = right;
            else
                parent.Right = right;
        }
        else
        {
            Root = right;
        }
    }

    private new void RotateRight(BstNode<TKey, TValue> node)
    {
        if (node.Left == null) return;

        var left = node.Left;
        var rightOfLeft = left.Right;
        var parent = node.Parent;

        node.Left = rightOfLeft;
        if (rightOfLeft != null) rightOfLeft.Parent = node;

        left.Right = node;
        node.Parent = left;

        left.Parent = parent;
        if (parent != null)
        {
            if (parent.Left == node)
                parent.Left = left;
            else
                parent.Right = left;
        }
        else
        {
            Root = left;
        }
    }

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
                    RotateRight(grandParent);
                    RotateRight(parent);
                }
                else if (!nodeIsLeft && !parentIsLeft)
                {
                    //Zig-Zig (right-right)
                    RotateLeft(grandParent);
                    RotateLeft(parent);
                }
                else
                {
                    // Zig-Zag
                    if (nodeIsLeft)
                    {
                        RotateRight(parent);
                        RotateLeft(grandParent);
                    }
                    else
                    {
                        RotateLeft(parent);
                        RotateRight(grandParent);
                    }
                }
            }
        }
        Root = node;
    }


    protected override void OnNodeAdded(BstNode<TKey, TValue> newNode)
    {
        if (Root == null || Count < 3) return;
        if (Comparer.Compare(newNode.Key, Root.Key) < 0)
        {
            Splay(newNode);
        }
    }

    protected override void OnNodeRemoved(BstNode<TKey, TValue>? parent, BstNode<TKey, TValue>? child)
    {
        Splay(parent ?? child);
    }

    private BstNode<TKey, TValue>? FindNode(TKey key, out BstNode<TKey, TValue>? lastVisited)
    {
        lastVisited = null;
        var current = Root;
        while (current != null)
        {
            lastVisited = current;
            int cmp = Comparer.Compare(key, current.Key);
            if (cmp == 0)
                return current;

            current = cmp < 0 ? current.Left : current.Right;
        }

        return null;
    }


    public override bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        var node = FindNode(key, out var lastVisited);
        if (node != null)
        {
            value = node.Value;
            Splay(node);
            return true;
        }
        else
        {
            Splay(lastVisited);
            value = default;
            return false;
        }
    }
}