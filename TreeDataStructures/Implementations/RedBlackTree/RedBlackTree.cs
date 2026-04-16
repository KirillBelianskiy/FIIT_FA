using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.RedBlackTree;

public class RedBlackTree<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, RbNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    public override void Add(TKey key, TValue value)
    {
        if (Root == null)
        {
            Root = CreateNode(key, value);
            Root.Color = RbColor.Black;
            Count = 1;
            return;
        }

        RbNode<TKey, TValue>? current = Root;
        RbNode<TKey, TValue>? parent = null;
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

        RbNode<TKey, TValue> newNode = CreateNode(key, value);
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
        InsertFixup(newNode);
        Root!.Color = RbColor.Black;
    }

    public override bool Remove(TKey key)
    {
        RbNode<TKey, TValue>? z = FindNode(key);
        if (z == null)
        {
            return false;
        }

        RbNode<TKey, TValue> y = z;
        RbColor yOriginalColor = y.Color;
        RbNode<TKey, TValue>? x;
        RbNode<TKey, TValue>? xParent;

        if (z.Left == null)
        {
            x = z.Right;
            xParent = z.Parent;
            Transplant(z, z.Right);
        }
        else if (z.Right == null)
        {
            x = z.Left;
            xParent = z.Parent;
            Transplant(z, z.Left);
        }
        else
        {
            y = Minimum(z.Right);
            yOriginalColor = y.Color;
            x = y.Right;

            if (y.Parent == z)
            {
                xParent = y;
                if (x != null)
                {
                    x.Parent = y;
                }
            }
            else
            {
                xParent = y.Parent;
                Transplant(y, y.Right);
                y.Right = z.Right;
                y.Right!.Parent = y;
            }

            Transplant(z, y);
            y.Left = z.Left;
            y.Left!.Parent = y;
            y.Color = z.Color;
        }

        if (yOriginalColor == RbColor.Black)
        {
            DeleteFixup(x, xParent);
        }

        if (Root != null)
        {
            Root.Color = RbColor.Black;
            Root.Parent = null;
        }

        Count--;
        return true;
    }

    protected override RbNode<TKey, TValue> CreateNode(TKey key, TValue value)
    {
        return new RbNode<TKey, TValue>(key, value) { Color = RbColor.Red };
    }
    
    protected override void OnNodeAdded(RbNode<TKey, TValue> newNode)
    {
        // Insert balancing is handled in Add/InsertFixup.
    }

    protected override void OnNodeRemoved(RbNode<TKey, TValue>? parent, RbNode<TKey, TValue>? child)
    {
        // Delete balancing is handled in Remove/DeleteFixup.
    }

    private void InsertFixup(RbNode<TKey, TValue> node)
    {
        RbNode<TKey, TValue> z = node;
        while (ColorOf(z.Parent) == RbColor.Red)
        {
            RbNode<TKey, TValue> parent = z.Parent!;
            RbNode<TKey, TValue> grandParent = parent.Parent!;

            if (parent == grandParent.Left)
            {
                RbNode<TKey, TValue>? uncle = grandParent.Right;
                if (ColorOf(uncle) == RbColor.Red)
                {
                    parent.Color = RbColor.Black;
                    uncle!.Color = RbColor.Black;
                    grandParent.Color = RbColor.Red;
                    z = grandParent;
                }
                else
                {
                    if (z == parent.Right)
                    {
                        z = parent;
                        RotateLeftLocal(z);
                        parent = z.Parent!;
                        grandParent = parent.Parent!;
                    }

                    parent.Color = RbColor.Black;
                    grandParent.Color = RbColor.Red;
                    RotateRightLocal(grandParent);
                }
            }
            else
            {
                RbNode<TKey, TValue>? uncle = grandParent.Left;
                if (ColorOf(uncle) == RbColor.Red)
                {
                    parent.Color = RbColor.Black;
                    uncle!.Color = RbColor.Black;
                    grandParent.Color = RbColor.Red;
                    z = grandParent;
                }
                else
                {
                    if (z == parent.Left)
                    {
                        z = parent;
                        RotateRightLocal(z);
                        parent = z.Parent!;
                        grandParent = parent.Parent!;
                    }

                    parent.Color = RbColor.Black;
                    grandParent.Color = RbColor.Red;
                    RotateLeftLocal(grandParent);
                }
            }
        }
    }

    private void DeleteFixup(RbNode<TKey, TValue>? x, RbNode<TKey, TValue>? parent)
    {
        RbNode<TKey, TValue>? current = x;
        RbNode<TKey, TValue>? currentParent = parent;

        while (current != Root && ColorOf(current) == RbColor.Black)
        {
            if (currentParent == null)
            {
                break;
            }

            if (current == currentParent.Left)
            {
                RbNode<TKey, TValue>? sibling = currentParent.Right;

                if (ColorOf(sibling) == RbColor.Red)
                {
                    sibling!.Color = RbColor.Black;
                    currentParent.Color = RbColor.Red;
                    RotateLeftLocal(currentParent);
                    sibling = currentParent.Right;
                }

                if (ColorOf(sibling?.Left) == RbColor.Black && ColorOf(sibling?.Right) == RbColor.Black)
                {
                    if (sibling != null)
                    {
                        sibling.Color = RbColor.Red;
                    }

                    current = currentParent;
                    currentParent = current.Parent;
                }
                else
                {
                    if (ColorOf(sibling?.Right) == RbColor.Black)
                    {
                        if (sibling?.Left != null)
                        {
                            sibling.Left.Color = RbColor.Black;
                        }

                        if (sibling != null)
                        {
                            sibling.Color = RbColor.Red;
                            RotateRightLocal(sibling);
                        }

                        sibling = currentParent.Right;
                    }

                    if (sibling != null)
                    {
                        sibling.Color = currentParent.Color;
                    }

                    currentParent.Color = RbColor.Black;
                    if (sibling?.Right != null)
                    {
                        sibling.Right.Color = RbColor.Black;
                    }

                    RotateLeftLocal(currentParent);
                    current = Root;
                    currentParent = null;
                }
            }
            else
            {
                RbNode<TKey, TValue>? sibling = currentParent.Left;

                if (ColorOf(sibling) == RbColor.Red)
                {
                    sibling!.Color = RbColor.Black;
                    currentParent.Color = RbColor.Red;
                    RotateRightLocal(currentParent);
                    sibling = currentParent.Left;
                }

                if (ColorOf(sibling?.Right) == RbColor.Black && ColorOf(sibling?.Left) == RbColor.Black)
                {
                    if (sibling != null)
                    {
                        sibling.Color = RbColor.Red;
                    }

                    current = currentParent;
                    currentParent = current.Parent;
                }
                else
                {
                    if (ColorOf(sibling?.Left) == RbColor.Black)
                    {
                        if (sibling?.Right != null)
                        {
                            sibling.Right.Color = RbColor.Black;
                        }

                        if (sibling != null)
                        {
                            sibling.Color = RbColor.Red;
                            RotateLeftLocal(sibling);
                        }

                        sibling = currentParent.Left;
                    }

                    if (sibling != null)
                    {
                        sibling.Color = currentParent.Color;
                    }

                    currentParent.Color = RbColor.Black;
                    if (sibling?.Left != null)
                    {
                        sibling.Left.Color = RbColor.Black;
                    }

                    RotateRightLocal(currentParent);
                    current = Root;
                    currentParent = null;
                }
            }
        }

        if (current != null)
        {
            current.Color = RbColor.Black;
        }
    }

    private void RotateLeftLocal(RbNode<TKey, TValue> x)
    {
        RbNode<TKey, TValue>? y = x.Right;
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

    private void RotateRightLocal(RbNode<TKey, TValue> y)
    {
        RbNode<TKey, TValue>? x = y.Left;
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

    private static RbColor ColorOf(RbNode<TKey, TValue>? node)
    {
        return node?.Color ?? RbColor.Black;
    }

    private static RbNode<TKey, TValue> Minimum(RbNode<TKey, TValue> node)
    {
        RbNode<TKey, TValue> current = node;
        while (current.Left != null)
        {
            current = current.Left;
        }

        return current;
    }
}