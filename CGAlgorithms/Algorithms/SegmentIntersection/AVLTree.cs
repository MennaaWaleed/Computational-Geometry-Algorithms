using System;
using System.Collections.Generic;

public class AVLTree<T>
{
    private class Node
    {
        public T Data;
        public Node Left, Right;
        public int Height;
        public Node(T data) { Data = data; Height = 1; }
    }

    private Node root;
    private readonly IComparer<T> comparer;

    public AVLTree(IComparer<T> comparer) => this.comparer = comparer;

    public void Insert(T data) => root = Insert(root, data);
    public void Delete(T data) => root = Delete(root, data);

    private int GetHeight(Node n) => n?.Height ?? 0;
    private int GetBalance(Node n) => n == null ? 0 : GetHeight(n.Left) - GetHeight(n.Right);

    private Node Insert(Node node, T data)
    {
        if (node == null) return new Node(data);

        int cmp = comparer.Compare(data, node.Data);
        if (cmp < 0) node.Left = Insert(node.Left, data);
        else node.Right = Insert(node.Right, data);

        return Rebalance(node);
    }

    private Node Delete(Node node, T data)
    {
        if (node == null) return null;

        //search for the node to delete
        int cmp = comparer.Compare(data, node.Data);
        if (cmp < 0) node.Left = Delete(node.Left, data);
        else if (cmp > 0) node.Right = Delete(node.Right, data);
        else // found the node to delete
        {
            if (!ReferenceEquals(node.Data, data))
            {
                node.Left = Delete(node.Left, data);
                node.Right = Delete(node.Right, data);
            }
            else
            {
                // node with only one child or no child
                if (node.Left == null || node.Right == null)
                    node = node.Left ?? node.Right;
                else // node with two children
                {
                    Node temp = GetMinValueNode(node.Right);
                    node.Data = temp.Data;
                    node.Right = Delete(node.Right, temp.Data);
                }
            }
        }

        if (node == null) return null;
        return Rebalance(node);
    }

    private Node Rebalance(Node n)
    {
        n.Height = 1 + Math.Max(GetHeight(n.Left), GetHeight(n.Right));
        int balance = GetBalance(n);

        if (balance > 1 && GetBalance(n.Left) >= 0) return RotateRight(n);
        if (balance > 1 && GetBalance(n.Left) < 0) { n.Left = RotateLeft(n.Left); return RotateRight(n); }
        if (balance < -1 && GetBalance(n.Right) <= 0) return RotateLeft(n);
        if (balance < -1 && GetBalance(n.Right) > 0) { n.Right = RotateRight(n.Right); return RotateLeft(n); }

        return n;
    }

    private Node RotateRight(Node y)
    {
        Node x = y.Left; Node T2 = x.Right;
        x.Right = y; y.Left = T2;
        y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;
        x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;
        return x;
    }

    private Node RotateLeft(Node x)
    {
        Node y = x.Right; Node T2 = y.Left;
        y.Left = x; x.Right = T2;
        x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;
        y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;
        return y;
    }

    private Node GetMinValueNode(Node node)
    {
        Node current = node;
        while (current.Left != null) current = current.Left;
        return current;
    }

    public T FindPredecessor(T data)
    {
        Node current = root, predecessor = null;
        while (current != null)
        {
            if (comparer.Compare(data, current.Data) > 0) { predecessor = current; current = current.Right; }
            else current = current.Left;
        }
        return predecessor != null ? predecessor.Data : default;
    }

    public T FindSuccessor(T data)
    {
        Node current = root, successor = null;
        while (current != null)
        {
            if (comparer.Compare(data, current.Data) < 0) { successor = current; current = current.Left; }
            else current = current.Right;
        }
        return successor != null ? successor.Data : default;
    }
}