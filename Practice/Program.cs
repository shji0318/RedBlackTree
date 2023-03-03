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
        tree.Push(new Node<int>(10));
        tree.Push(new Node<int>(20));
        tree.Push(new Node<int>(50));
        tree.Push(new Node<int>(30));
        tree.Push(new Node<int>(80));
        tree.Push(new Node<int>(40));
        tree.Push(new Node<int>(35));
        tree.Push(new Node<int>(25));
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
                ReColoring(parent,uncle,grand);
                now = grand;
                continue;
            }

            if(_dic[grand]._left == parent && _dic[parent]._left == now)//left , left
            {
                RotateRight(now,parent,grand);
                now = parent; //변경 후 parent 노드 위치에서 다시 Double Red 검사
                continue;
            }
            else if(_dic[grand]._right == parent && _dic[parent]._right == now) //right, right
            {
                RotateLeft(now, parent, grand);
                now = parent; //변경 후 parent 노드 위치에서 다시 Double Red 검사
                continue;
            }
            else if(_dic[grand]._left == parent && _dic[parent]._right == now) // left, right
            {
                RotateLeftRight(now,parent,grand);   
                // 변경 후 중앙 노드로 현재 노드가 올라오기 때문에 now 위치에서 다시 검사
            }
            else // right, left
            {
                RotateRightLeft(now,parent,grand);
                // 변경 후 중앙 노드로 현재 노드가 올라오기 때문에 now 위치에서 다시 검사
            }

        }
    }

    public void WriteTree()
    {
        Queue<Node<T>> q = new Queue<Node<T>>();
        Queue<Node<T>> next = new Queue<Node<T>>();
        q.Enqueue(_dic[_root]);

        while(q.Count > 0)
        {
            bool allNil = false;
            int qCount = q.Count;
            for (int i = 0; i < qCount; i++)
            {
                Node<T> now = q.Dequeue();
                CColor(now._color, $"{now._data}");

                if (now._left != -1 || now._right != -1)
                    allNil = true;
                next.Enqueue(_dic[now._left]);
                
                next.Enqueue(_dic[now._right]);
            }
            Console.WriteLine();
            if (allNil == false)
                break;

            int nextCount = next.Count;
            for (int i = 0; i < nextCount; i++)
            {                
                q.Enqueue(next.Dequeue());
            }

            
        }
        

    }

    public void CColor (NodeColor nc, string s)
    {
        Console.ForegroundColor = ConsoleColor.White;

        if (nc == NodeColor.Red)
            Console.ForegroundColor = ConsoleColor.Red;

        if (s == "0")
            s = "Nil";
        Console.Write("{0,-2}",s);
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

        #region 중간 값을 갖는 노드 : parent 노드를 중앙으로 이동        
        _dic[grand]._left = _dic[parent]._right; // 오른쪽 자식을 grand 노드로 변경해야하기 때문에 중앙으로 올라갈 parent 노드의 오른쪽 자식이 존재한다면
                                                 // 기존 parent의 right는 grand노드의 왼쪽 자식으로 옮긴다 
                                                 // 왼쪽 자녀 노드의 오른쪽 자녀 또한 자신보다 작은 값을 갖기 때문에 왼쪽 자식으로 붙여도 상관 없음


        //중앙 노드가 되는 parent 노드의 right는 grand 로 변경
        _dic[parent]._right = grand; 
        _dic[grand]._parent = parent;
        //중앙 노드가 되는 parent 노드의 left 는 now 로 변경
        _dic[parent]._left = now;
        _dic[now]._parent = parent;
        _dic[parent]._parent = grandParent;        

        if (_dic[grandParent]._left == grand)
            _dic[grandParent]._left = parent;
        else if (_dic[grandParent]._right == grand)
            _dic[grandParent]._right = parent;
        #endregion

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

        #region 중간 값을 갖는 노드 : parent 노드를 중앙으로 이동                     
        _dic[grand]._right = _dic[parent]._left;  // 왼쪽 자식을 grand 노드로 변경해야하기 때문에 중앙으로 올라갈 parent 노드의 왼쪽 자식이 존재한다면
                                                  // 기존 parent의 left는 grand노드의 오른쪽 자식으로 옮긴다 
                                                  // 오른쪽 자녀 노드의 왼쪽 자녀 또한 자신보다 큰 값을 갖기 때문에 오른쪽 자식으로 붙여도 상관 없음

        //중앙 노드가 되는 parent 노드의 left는 grand 로 변경
        _dic[parent]._left = grand;
        _dic[grand]._parent = parent;
        //중앙 노드가 되는 parent 노드의 right 는 now 로 변경
        _dic[parent]._right = now;
        _dic[now]._parent = parent;

        _dic[parent]._parent = grandParent;        

        if (_dic[grandParent]._left == grand)
            _dic[grandParent]._left = parent;
        else if(_dic[grandParent]._right == grand)
            _dic[grandParent]._right = parent;
        #endregion

        if (grand == _root)
            _root = parent;

        if (parent != _root)
            _dic[parent]._color = NodeColor.Red;
        else
            _dic[parent]._color = NodeColor.Black;

        _dic[grand]._color = NodeColor.Black;
        _dic[now]._color = NodeColor.Black;
    }    

    public void RotateRightLeft(int now, int parent, int grand)
    {
        _dic[parent]._left = _dic[now]._right;
        _dic[parent]._parent = now;
        _dic[now]._right = parent;
        _dic[now]._parent = parent;
        _dic[grand]._right = now;
        RotateLeft(parent, now , grand);
    }

    public void RotateLeftRight(int now, int parent, int grand)
    {
        _dic[parent]._right = _dic[now]._left;
        _dic[parent]._parent = now;
        _dic[now]._left = parent;
        _dic[now]._parent = grand;
        _dic[grand]._left = now;
        RotateRight(parent,now,grand);
    }

}
