using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Program
{
    
    public static void Main()
    {
        RedBlackTree<int> tree = new RedBlackTree<int>();
        tree.Push(new Node<int>(8));
        tree.Push(new Node<int>(4));
        tree.Push(new Node<int>(2));

        tree.WriteTree();

    }

    


}
public enum NodeColor
{
    Red,
    Black,
}
public class Node<T> where T : IComparable
{
    public Node (T num)
    {
        _data = num;
    }

    public int _num;
    public T _data;
    public NodeColor _color = NodeColor.Black;
    public int _left = -1; // -1 Nil 노드로 가정
    public int _right = -1;
    public int _parent = -1;
    
}

public class RedBlackTree<T> where T : IComparable
{
    Dictionary<int, Node<T>> _dic = new Dictionary<int, Node<T>>();
    int _count = 0;
    int _root;

    public RedBlackTree () // 생성자를 통해 nil 노드 생성 
    {
        _dic.Add(-1, new Node<T>(default(T)));
    }

    public void Push(Node<T> node)
    {
        if(_count == 0)
        {
            _root = _count;
            _dic.Add(_count, node);
            node._color = NodeColor.Black;
            _count++;
            return;
        }
        node._num = _count++;
        node._color = NodeColor.Red;
        _dic.Add(node._num, node);

        int now = _root;
        while(true)
        {
            if(_dic[now]._data.CompareTo(node._data) < 0)
            {
                if (_dic[now]._right == -1)
                {
                    _dic[now]._right = node._num;
                    node._parent = _dic[now]._num;
                    now = _dic[now]._right;
                    break;
                }
                now = _dic[now]._right;
            }
            else
            {
                if (_dic[now]._left == -1)
                {
                    _dic[now]._left = node._num;
                    node._parent = _dic[now]._num;
                    now = _dic[now]._left;
                    break;
                }
                now = _dic[now]._left;
            }
        }

        while (now != _root && _dic[now]._parent != -1)
        {
            if (_dic[now]._color != NodeColor.Red || _dic[_dic[now]._parent]._color != NodeColor.Red)
                break;

            int parent = _dic[now]._parent;
            int grand = _dic[parent]._parent;
            int uncle = _dic[grand]._left == parent ? _dic[grand]._right : _dic[grand]._left;


            if (_dic[uncle]._color == NodeColor.Red)
            {
                ReColoring(parent,grand,uncle);
                now = grand;
                continue;
            }

            if(_dic[grand]._left == parent && _dic[parent]._left == now)//left , left
            {
                RotateRight(now,parent,grand);
            }
            else if(_dic[grand]._right == parent && _dic[parent]._right == now) //right, right
            {
                RotateLeft(now, parent, grand);
            }
            else if(_dic[grand]._left == parent && _dic[parent]._right == now) // left, right
            {
                RotateLeftRight();
            }
            else // right, left
            {
                RotateRightLeft();
            }

        }
    }

    public void WriteTree()
    {
        Queue<Node<T>> q = new Queue<Node<T>>();
        q.Enqueue(_dic[_root]);

        while(q.Count>0)
        {
            Node<T> now = q.Dequeue();
            Console.Write(now._data);

            if (now._left != -1)
                q.Enqueue(_dic[now._left]);

            if (now._right != -1)
                q.Enqueue(_dic[now._right]);
        }


    }

    public void ReColoring(int parent, int uncle, int grand)
    {   
        _dic[uncle]._color = NodeColor.Black;
        _dic[parent]._color = NodeColor.Black;

        if (grand != _root)
            _dic[grand]._color = NodeColor.Red;
        else
            _dic[grand]._color = NodeColor.Black;
    }

    public void RotateRight(int now, int parent, int grand)
    {
        int grandParent = _dic[grand]._parent;

        // 중간 값을 갖는 노드 : parent 노드를 중앙으로 이동
        _dic[parent]._right = grand; 
        _dic[grand]._parent = parent;
        _dic[parent]._left = now;
        _dic[now]._parent = parent;
        _dic[parent]._parent = grandParent;
        _dic[grand]._left = -1;

        if (_dic[grandParent]._left == grand)
            _dic[grandParent]._left = parent;
        else
            _dic[grandParent]._right = parent;

        if (grand == _root)
            _root = parent;

        if (parent != _root)
            _dic[parent]._color = NodeColor.Red;
        else
            _dic[parent]._color = NodeColor.Black;

        _dic[grand]._color = NodeColor.Black;
        _dic[now]._color = NodeColor.Black;
    }

    public void RotateLeft(int now, int parent, int grand)
    {
        int grandParent = _dic[grand]._parent;

        // 중간 값을 갖는 노드 : parent 노드를 중앙으로 이동
        _dic[parent]._left = grand;
        _dic[grand]._parent = parent;
        _dic[parent]._right = now;
        _dic[now]._parent = parent;
        _dic[parent]._parent = grandParent;
        _dic[grand]._right = -1;

        if (_dic[grandParent]._left == grand)
            _dic[grandParent]._left = parent;
        else
            _dic[grandParent]._right = parent;

        if (grand == _root)
            _root = parent;

        if (parent != _root)
            _dic[parent]._color = NodeColor.Red;
        else
            _dic[parent]._color = NodeColor.Black;

        _dic[grand]._color = NodeColor.Black;
        _dic[now]._color = NodeColor.Black;
    }    

    public void RotateRightLeft()
    {
        //RotateLeft();
    }

    public void RotateLeftRight()
    {
        //RotateRight();
    }

}
