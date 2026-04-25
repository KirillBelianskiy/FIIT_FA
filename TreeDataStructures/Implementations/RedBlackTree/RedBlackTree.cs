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

        var curr = Root;
        RbNode<TKey, TValue>? parent = null;
        int cmp = 0;

        while (curr != null)
        {
            parent = curr;
            cmp = Comparer.Compare(key, curr.Key);

            if (cmp == 0)
            {
                curr.Value = value;
                return;
            }

            curr = cmp < 0 ? curr.Left : curr.Right;
        }

        var node = CreateNode(key, value);
        node.Parent = parent;

        if (cmp < 0)
            parent!.Left = node;
        else
            parent!.Right = node;

        Count++;
        FixAfterInsert(node);
        Root!.Color = RbColor.Black;
    }

    public override bool Remove(TKey key)
    {
        var z = FindNode(key);
        if (z == null) return false;

        var y = z;
        var yColor = y.Color;
        RbNode<TKey, TValue>? x, xParent;

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
            y = Min(z.Right);
            yColor = y.Color;
            x = y.Right;

            if (y.Parent == z)
            {
                xParent = y;
                if (x != null) x.Parent = y;
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

        if (yColor == RbColor.Black)
            FixAfterDelete(x, xParent);

        if (Root != null)
        {
            Root.Color = RbColor.Black;
            Root.Parent = null;
        }

        Count--;
        return true;
    }

    protected override RbNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value) { Color = RbColor.Red };

    protected override void OnNodeAdded(RbNode<TKey, TValue> newNode) { }
    protected override void OnNodeRemoved(RbNode<TKey, TValue>? parent, RbNode<TKey, TValue>? child) { }

    private void FixAfterInsert(RbNode<TKey, TValue> z)
    {
        while (IsRed(z.Parent))
        {
            var parent = z.Parent!;
            var grand = parent.Parent!;

            if (parent == grand.Left)
            {
                var uncle = grand.Right;
                if (IsRed(uncle))
                {
                    parent.Color = RbColor.Black;
                    uncle!.Color = RbColor.Black;
                    grand.Color = RbColor.Red;
                    z = grand;
                }
                else
                {
                    if (z == parent.Right)
                    {
                        z = parent;
                        RotateLeft(z);
                        parent = z.Parent!;
                        grand = parent.Parent!;
                    }

                    parent.Color = RbColor.Black;
                    grand.Color = RbColor.Red;
                    RotateRight(grand);
                }
            }
            else
            {
                var uncle = grand.Left;
                if (IsRed(uncle))
                {
                    parent.Color = RbColor.Black;
                    uncle!.Color = RbColor.Black;
                    grand.Color = RbColor.Red;
                    z = grand;
                }
                else
                {
                    if (z == parent.Left)
                    {
                        z = parent;
                        RotateRight(z);
                        parent = z.Parent!;
                        grand = parent.Parent!;
                    }

                    parent.Color = RbColor.Black;
                    grand.Color = RbColor.Red;
                    RotateLeft(grand);
                }
            }
        }
    }

    private void FixAfterDelete(RbNode<TKey, TValue>? x, RbNode<TKey, TValue>? parent)
    {
        while (x != Root && !IsRed(x))
        {
            if (parent == null) break;

            if (x == parent.Left)
            {
                var sib = parent.Right;

                if (IsRed(sib))
                {
                    sib!.Color = RbColor.Black;
                    parent.Color = RbColor.Red;
                    RotateLeft(parent);
                    sib = parent.Right;
                }

                if (!IsRed(sib?.Left) && !IsRed(sib?.Right))
                {
                    if (sib != null) sib.Color = RbColor.Red;
                    x = parent;
                    parent = x.Parent;
                }
                else
                {
                    if (!IsRed(sib?.Right))
                    {
                        if (sib?.Left != null) sib.Left.Color = RbColor.Black;
                        if (sib != null)
                        {
                            sib.Color = RbColor.Red;
                            RotateRight(sib);
                        }
                        sib = parent.Right;
                    }

                    if (sib != null) sib.Color = parent.Color;
                    parent.Color = RbColor.Black;
                    if (sib?.Right != null) sib.Right.Color = RbColor.Black;
                    RotateLeft(parent);
                    x = Root;
                    parent = null;
                }
            }
            else
            {
                var sib = parent.Left;

                if (IsRed(sib))
                {
                    sib!.Color = RbColor.Black;
                    parent.Color = RbColor.Red;
                    RotateRight(parent);
                    sib = parent.Left;
                }

                if (!IsRed(sib?.Right) && !IsRed(sib?.Left))
                {
                    if (sib != null) sib.Color = RbColor.Red;
                    x = parent;
                    parent = x.Parent;
                }
                else
                {
                    if (!IsRed(sib?.Left))
                    {
                        if (sib?.Right != null) sib.Right.Color = RbColor.Black;
                        if (sib != null)
                        {
                            sib.Color = RbColor.Red;
                            RotateLeft(sib);
                        }
                        sib = parent.Left;
                    }

                    if (sib != null) sib.Color = parent.Color;
                    parent.Color = RbColor.Black;
                    if (sib?.Left != null) sib.Left.Color = RbColor.Black;
                    RotateRight(parent);
                    x = Root;
                    parent = null;
                }
            }
        }

        if (x != null) x.Color = RbColor.Black;
    }

    private bool IsRed(RbNode<TKey, TValue>? node) => node?.Color == RbColor.Red;

    private RbNode<TKey, TValue> Min(RbNode<TKey, TValue> node)
    {
        while (node.Left != null)
            node = node.Left;
        return node;
    }
}