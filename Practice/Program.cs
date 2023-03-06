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
        tree.Insert(10);
        tree.Insert(20);
        tree.Insert(50);
        tree.Insert(30);
        tree.Insert(80);
        tree.Insert(40);
        tree.Insert(35);
        tree.Insert(25);
        tree.Insert(33);
        tree.Insert(5);
        tree.Insert(15);
        tree.Insert(37);
        tree.Insert(45);
        tree.Insert(2);
        tree.Insert(27);

        tree.Remove(15);
        tree.Remove(33);
        tree.Remove(37);
        tree.Remove(35);
        tree.Remove(40);
        tree.Remove(50);
        tree.Remove(80);
        tree.Remove(27);
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

    public void Insert(T data)
    {
        _dic[-1]._parent = -1;
        _dic[-1]._color = NodeColor.Black;
        Node<T> node = new Node<T>(data);
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
                RightRotate(now,parent,grand);
                _dic[parent]._color = NodeColor.Black;
                _dic[grand]._color = NodeColor.Red;
                _dic[now]._color = NodeColor.Red;
                now = parent; //변경 후 parent 노드 위치에서 다시 Double Red 검사
                continue;
            }
            else if(_dic[grand]._right == parent && _dic[parent]._right == now) //right, right
            {
                LeftRotate(now, parent, grand);
                _dic[parent]._color = NodeColor.Black;
                _dic[grand]._color = NodeColor.Red;
                _dic[now]._color = NodeColor.Red;
                now = parent; //변경 후 parent 노드 위치에서 다시 Double Red 검사
                continue;
            }
            else if(_dic[grand]._left == parent && _dic[parent]._right == now) // left, right
            {
                LeftRightRotate(now,parent,grand);
                _dic[now]._color = NodeColor.Black;
                _dic[grand]._color = NodeColor.Red;
                _dic[parent]._color = NodeColor.Red;
                // 변경 후 중앙 노드로 현재 노드가 올라오기 때문에 now 위치에서 다시 검사
            }
            else if (_dic[grand]._right == parent && _dic[parent]._left == now)// right, left
            {
                RightLeftRotate(now,parent,grand);
                _dic[now]._color = NodeColor.Black;
                _dic[grand]._color = NodeColor.Red;
                _dic[parent]._color = NodeColor.Red;
                // 변경 후 중앙 노드로 현재 노드가 올라오기 때문에 now 위치에서 다시 검사
            }

        }
    }

    public void Remove(T data)
    {
        _dic[-1]._parent = -1;
        _dic[-1]._color = NodeColor.Black;
        Node<T> node = _dic[_root];

        #region 해당 노드 찾기
        while (true)
        {
            if (node._data.CompareTo(data) < 0)
            {
                if (node._right == -1)
                    break;
                node = _dic[node._right];
            }
            else if (node._data.CompareTo(data) > 0)
            {
                if (node._left == -1)
                    break;

                node = _dic[node._left];
            }
            else
                break;
        }

        if (node._data.CompareTo(data) != 0)
        {
            Console.WriteLine("해당 값은 현재 Tree에 존재하지 않습니다.");
            return;
        }
        #endregion

        // 자식이 둘 다 존재할 경우 => 제거되는 노드의 색은 Successor의 노드 색을 따라감
        if (node._left != -1 && node._right != -1)
        {
            //Successor 찾기 => 오른쪽 자식의 왼쪽 끝 leaf 노드
            Node<T> suc = FindSuccessor(node);

            node._data = suc._data;

            _dic[suc._parent]._left = suc._right;
            _dic[suc._right]._parent = suc._parent;
            
            _dic.Remove(suc._num);

            //삭제되는 노드의 색이 Red 일 경우 기존 RB Tree의 규칙을 위반하지 않음
            if (suc._color == NodeColor.Red)
                return;            

            //위에 조건문에 해당하지 않았다면 삭제 알고리즘의 해당하는 Case를 찾기 위해 자신, 부모와 형제 노드를 비교
            Node<T> parent = _dic[suc._parent];
            
            FixedRemove(_dic[suc._right], parent);
        }
        else // 자식이 1개 or 0개
        {
            int child = -1;

            child = node._left != -1 ? node._left:node._right;

            if (_dic[node._parent]._left == node._num)
                _dic[node._parent]._left = child;
            else
                _dic[node._parent]._right = child;

            if(child != -1)
                _dic[child]._parent = node._parent;

            _dic.Remove(node._num);            

            if (node._color == NodeColor.Red)
                return;

            Node<T> parent = _dic[node._parent];        

            FixedRemove(_dic[child],parent);
            
        }
        

        
    }

    public void WriteTree()
    {
        _dic[-1]._parent = -1;
        _dic[-1]._color = NodeColor.Black;
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

    public void RightRotate(int now, int parent, int grand)
    {
        int grandParent = _dic[grand]._parent;

        #region 중간 값을 갖는 노드 : parent 노드를 중앙으로 이동        
        _dic[grand]._left = _dic[parent]._right;    // 오른쪽 자식을 grand 노드로 변경해야하기 때문에 중앙으로 올라갈 parent 노드의 오른쪽 자식이 존재한다면
        _dic[_dic[parent]._right]._parent = grand;  // 기존 parent의 right는 grand노드의 왼쪽 자식으로 옮긴다 
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
        {
            _root = parent;
            _dic[parent]._color = NodeColor.Black;
        }

    }

    public void LeftRotate(int now, int parent, int grand)
    {
        int grandParent = _dic[grand]._parent;

        #region 중간 값을 갖는 노드 : parent 노드를 중앙으로 이동                     
        _dic[grand]._right = _dic[parent]._left;   // 왼쪽 자식을 grand 노드로 변경해야하기 때문에 중앙으로 올라갈 parent 노드의 왼쪽 자식이 존재한다면
        _dic[_dic[parent]._left]._parent = grand;  // 기존 parent의 left는 grand노드의 오른쪽 자식으로 옮긴다 
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
        {
            _root = parent;
            _dic[parent]._color = NodeColor.Black;
        }
            
        
        
    }
    public void LeftRightRotate(int now, int parent, int grand)
    {
        //기존 Rotate 전 현재 노드와 부모 노드를 왼쪽으로 먼저 Rotate 해서 위에 상황과 동일하게 만든 후, 현재, 부모, 조부모를 기준으로 오른쪽 Rotate
        _dic[parent]._right = _dic[now]._left;
        _dic[_dic[now]._left]._parent = parent;
        _dic[parent]._parent = now;
        _dic[now]._left = parent;
        _dic[now]._parent = grand;
        _dic[grand]._left = now;
        RightRotate(parent,now,grand);
    }
    public void RightLeftRotate(int now, int parent, int grand)
    {
        //기존 Rotate 전 현재 노드와 부모 노드를 오른쪽으로 먼저 Rotate 해서 위에 상황과 동일하게 만든 후, 현재, 부모, 조부모를 기준으로 왼쪽 Rotate
        _dic[parent]._left = _dic[now]._right;
        _dic[_dic[now]._right]._parent = parent;
        _dic[parent]._parent = now;
        _dic[now]._right = parent;
        _dic[now]._parent = grand;
        _dic[grand]._right = now;
        LeftRotate(parent, now, grand);
    }

    public Node<T> FindSuccessor(Node<T> node)
    {
        Node<T> now = _dic[node._right];
        while(now._left != -1)
        {
            now = _dic[now._left];
        }

        return now;
    }

    public void FixedRemove(Node<T> node , Node<T> parent)
    {
        _dic[-1]._parent = -1;
        _dic[-1]._color = NodeColor.Black;
        // 대체된 노드가 Red라면 Red And Black => Black 으로 변경 후 종료
        if (node._color == NodeColor.Red)
        {
            node._color = NodeColor.Black;
            return;
        }

        if(_root == node._num)
        {
            node._color = NodeColor.Black;
            return;
        }

        Node<T> brother;

        if (parent._left == node._num || parent._left == -1)
            brother = _dic[parent._right];
        else
            brother = _dic[parent._left];

        switch (brother._color)
        {
            case NodeColor.Black:
                {
                    if(parent._left == brother._num)
                    {
                        if(_dic[brother._left]._color == NodeColor.Red)
                        {
                            Node<T> child = _dic[brother._left];
                            RemoveRightRotate(brother, parent, child);
                        }
                        else if (_dic[brother._left]._color == NodeColor.Black && _dic[brother._right]._color == NodeColor.Red)
                        {
                            Node<T> child = _dic[brother._right];
                            RemoveLeftRightRotate(brother, parent, child);
                        }
                        else
                        {
                            brother._color = NodeColor.Red;

                            if (parent._color == NodeColor.Red)
                                parent._color = NodeColor.Black;
                            else
                                FixedRemove(parent, _dic[parent._parent]);
                        }
                    }
                    else
                    {
                        if (_dic[brother._right]._color == NodeColor.Red)
                        {
                            Node<T> child = _dic[brother._right];
                            RemoveLeftRotate(brother, parent, child);
                        }
                        else if (_dic[brother._right]._color == NodeColor.Black && _dic[brother._left]._color == NodeColor.Red)
                        {
                            Node<T> child = _dic[brother._left];
                            RemoveRightLeftRotate(brother, parent, child);
                        }
                        else
                        {
                            brother._color = NodeColor.Red;

                            if (parent._color == NodeColor.Red)
                                parent._color = NodeColor.Black;
                            else
                                FixedRemove(parent, _dic[parent._parent]);
                        }
                    }
                }
                break;
            case NodeColor.Red:
                {
                    parent._color = brother._color;
                    brother._color = NodeColor.Black;

                    if(parent._left == brother._num)
                    {
                        RightRotate(brother._left, brother._num, parent._num);
                        FixedRemove(node, parent);
                    }
                    else
                    {
                        LeftRotate(brother._right,brother._num,parent._num);
                        FixedRemove(node, parent);
                    }
                }
                break;
        }
    }

    public void RemoveRightRotate(Node<T> brother, Node<T> parent, Node<T> child)
    {
        brother._color = parent._color;
        parent._color = NodeColor.Black;
        child._color = NodeColor.Black;
        RightRotate(child._num, brother._num, parent._num);
    }

    public void RemoveLeftRotate(Node<T> brother, Node<T> parent, Node<T> child)
    {
        brother._color = parent._color;
        parent._color = NodeColor.Black;
        child._color = NodeColor.Black;
        LeftRotate(child._num, brother._num, parent._num);
    }

    public void RemoveLeftRightRotate(Node<T> brother, Node<T> parent, Node<T> child)
    {
        brother._color = child._color;
        child._color = NodeColor.Black;

        child._color = parent._color;
        parent._color = NodeColor.Black;
        brother._color = NodeColor.Black;
        LeftRightRotate(child._num, brother._num, parent._num);
    }

    public void RemoveRightLeftRotate(Node<T> brother, Node<T> parent, Node<T> child)
    {
        brother._color = child._color;
        child._color = NodeColor.Black;

        child._color = parent._color;
        parent._color = NodeColor.Black;
        brother._color = NodeColor.Black;
        RightLeftRotate(child._num, brother._num, parent._num);
    }

}
