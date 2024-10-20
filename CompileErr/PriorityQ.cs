using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompileErr
{
    public class PriorityQueue<T1, T2>
    {
        class MyVector
        {
            public T1 target;
            public int value;
        }
        private List<MyVector> list = new List<MyVector>();
        public int Count { get { return list.Count; } }
        public T1 Dequeue()
        {
            MyVector result = null;
            foreach (MyVector myVector in list)
            {
                if (result == null || result.value > myVector.value)
                {
                    result = myVector;
                }
            }
            list.Remove(result);
            return result.target;
        }
        public void Enqueue(T1 input, int val)
        {
            MyVector newVector = new MyVector();
            newVector.target = input;
            newVector.value = val;
            list.Add(newVector);
        }
    }
}
