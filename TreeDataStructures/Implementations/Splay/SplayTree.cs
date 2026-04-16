using System.Diagnostics.CodeAnalysis;
using TreeDataStructures.Implementations.BST;

namespace TreeDataStructures.Implementations.Splay;

public class SplayTree<TKey, TValue> : BinarySearchTree<TKey, TValue>
    where TKey : IComparable<TKey>
{
    protected override BstNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);
    
    protected override void OnNodeAdded(BstNode<TKey, TValue> newNode)
    {
        if (Root != null && Count >= 3 && Comparer.Compare(newNode.Key, Root.Key) < 0)
        {
            Splay(newNode);
        }
    }
    
    protected override void OnNodeRemoved(BstNode<TKey, TValue>? parent, BstNode<TKey, TValue>? child)
    {
    }

    public override bool ContainsKey(TKey key)
    {
        BstNode<TKey, TValue>? node = FindNode(key);
        if (node == null)
        {
            return false;
        }

        Splay(node);
        return true;
    }
    
    public override bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        BstNode<TKey, TValue>? node = FindNode(key);
        if (node != null)
        {
            Splay(node);
            value = node.Value;
            return true;
        }

        value = default;
        return false;
    }

    private void Splay(BstNode<TKey, TValue> node)
    {
        while (node.Parent != null)
        {
            BstNode<TKey, TValue> parent = node.Parent;
            BstNode<TKey, TValue>? grandParent = parent.Parent;

            if (grandParent == null)
            {
                if (node.IsLeftChild)
                {
                    RotateR(parent);
                }
                else
                {
                    RotateL(parent);
                }

                continue;
            }

            if (node.IsLeftChild && parent.IsLeftChild)
            {
                RotateR(grandParent);
                RotateR(parent);
            }
            else if (node.IsRightChild && parent.IsRightChild)
            {
                RotateL(grandParent);
                RotateL(parent);
            }
            else if (node.IsRightChild && parent.IsLeftChild)
            {
                RotateL(parent);
                RotateR(grandParent);
            }
            else
            {
                RotateR(parent);
                RotateL(grandParent);
            }
        }

        Root = node;
        Root.Parent = null;
    }

    private void RotateL(BstNode<TKey, TValue> x)
    {
        BstNode<TKey, TValue>? y = x.Right;
        if (y == null)
        {
            return;
        }

        x.Right = y.Left;
        if (y.Left != null)
        {
            y.Left.Parent = x;
        }

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
    }

    private void RotateR(BstNode<TKey, TValue> y)
    {
        BstNode<TKey, TValue>? x = y.Left;
        if (x == null)
        {
            return;
        }

        y.Left = x.Right;
        if (x.Right != null)
        {
            x.Right.Parent = y;
        }

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
    }
    
}
