using TreeDataStructures.Core;

namespace TreeDataStructures.Implementations.AVL;

public class AvlTree<TKey, TValue> : BinarySearchTreeBase<TKey, TValue, AvlNode<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    protected override AvlNode<TKey, TValue> CreateNode(TKey key, TValue value)
        => new(key, value);

    protected override void OnNodeAdded(AvlNode<TKey, TValue> newNode)
    {
        RebalanceFrom(newNode);
    }

    protected override void OnNodeRemoved(AvlNode<TKey, TValue>? parent, AvlNode<TKey, TValue>? child)
    {
        RebalanceFrom(parent ?? child);
    }

    private static int GetHeight(AvlNode<TKey, TValue>? node)
    {
        if (node == null) return 0;
        return node.Height;
    }

    private static void UpdateHeight(AvlNode<TKey, TValue> node)
    {
        node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
    }

    private static int GetBalanceFactor(AvlNode<TKey, TValue> node)
    {
        if (node.Left == null && node.Right == null) return 0;
        return GetHeight(node.Left) - GetHeight(node.Right);
    }

    private static bool IsLeftHeavy(int balance)
    {
        return balance > 1;
    }

    private static bool IsRightHeavy(int balance)
    {
        return balance < -1;
    }

    private void RebalanceFrom(AvlNode<TKey, TValue>? start)
    {
        var current = start;
        while (current != null)
        {
            UpdateHeight(current);
            var balance = GetBalanceFactor(current);

            if (IsLeftHeavy(balance) || IsRightHeavy(balance))
            {
                var newSubtreeRoot = RebalanceAt(current);
                current = newSubtreeRoot.Parent;
            }
            else
            {
                current = current.Parent;
            }
        }
    }

    private AvlNode<TKey, TValue> RebalanceAt(AvlNode<TKey, TValue> node)
    {
        var balanceFactor = GetBalanceFactor(node);

        if (IsLeftHeavy(balanceFactor))
        {
            var left = node.Left ?? throw new InvalidOperationException("AVL invariant broken: expected left child.");
            if (GetBalanceFactor(left) >= 0)
            {
                RotateLL(node);
                return left;
            }

            var newRoot = left.Right ??
                          throw new InvalidOperationException("AVL invariant broken: expected left-right child.");
            RotateLR(node);
            return newRoot;
        }

        if (IsRightHeavy(balanceFactor))
        {
            var right = node.Right ??
                        throw new InvalidOperationException("AVL invariant broken: expected right child.");
            if (GetBalanceFactor(right) <= 0)
            {
                RotateRR(node);
                return right;
            }

            var newRoot = right.Left ??
                          throw new InvalidOperationException("AVL invariant broken: expected right-left child.");
            RotateRL(node);
            return newRoot;
        }

        return node;
    }

    private void RotateLL(AvlNode<TKey, TValue> node)
    {
        var a = node;
        var b = node.Left ?? throw new InvalidOperationException("RotateLL requires left child.");
        var rightChildB = b.Right;
        
        b.Parent = a.Parent;
        if (a.Parent == null) Root = b;
        else if (a.Parent.Left == a) a.Parent.Left = b;
        else a.Parent.Right = b;
        
        a.Parent = b;
        b.Right = a;
        a.Left = rightChildB;
        if (rightChildB != null) rightChildB.Parent = a;
        
        UpdateHeight(a);
        UpdateHeight(b);
    }

    private void RotateLR(AvlNode<TKey, TValue> node)
    {
        var a = node;
        var b = node.Left ?? throw new InvalidOperationException("RotateLR requires left child.");
        var c = b.Right ?? throw new InvalidOperationException("RotateLR requires left-right child.");
        var leftChildC = c.Left;
        var rightChildC = c.Right;
        
        c.Parent = a.Parent;
        if (a.Parent == null) Root = c;
        else if (a.Parent.Left == a) a.Parent.Left = c;
        else a.Parent.Right = c;
        
        b.Parent = c;
        c.Left = b;
        c.Right = a;
        a.Parent = c;
        b.Right = leftChildC;
        
        if (leftChildC != null) leftChildC.Parent = b;
        a.Left = rightChildC;
        if (rightChildC != null) rightChildC.Parent = a;
        
        UpdateHeight(a);
        UpdateHeight(b);
        UpdateHeight(c);
    }

    private void RotateRR(AvlNode<TKey, TValue> node)
    {
        var a = node;
        var b = node.Right ?? throw new InvalidOperationException("RotateRR requires right child.");
        var leftChildB = b.Left;
        
        b.Parent = a.Parent;
        if (a.Parent == null) Root = b;
        else if (a.Parent.Left == a) a.Parent.Left = b;
        else a.Parent.Right = b;
        
        a.Parent = b;
        b.Left = a;
        a.Right = leftChildB;
        if (leftChildB != null) leftChildB.Parent = a;
        
        UpdateHeight(a);
        UpdateHeight(b);
    }

    private void RotateRL(AvlNode<TKey, TValue> node)
    {
        var a = node;
        var b = node.Right ?? throw new InvalidOperationException("RotateRL requires right child.");
        var c = b.Left ?? throw new InvalidOperationException("RotateRL requires right-left child.");
        var leftChildC = c.Left;
        var rightChildC = c.Right;
        
        c.Parent = a.Parent;
        if (a.Parent == null) Root = c;
        else if (a.Parent.Left == a) a.Parent.Left = c;
        else a.Parent.Right = c;
        
        b.Parent = c;
        c.Right = b;
        c.Left = a;
        a.Parent = c;
        b.Left = rightChildC;
        
        if (rightChildC != null) rightChildC.Parent = b;
        a.Right = leftChildC;
        if (leftChildC != null) leftChildC.Parent = a;
        
        UpdateHeight(a);
        UpdateHeight(b);
        UpdateHeight(c);
    }
}