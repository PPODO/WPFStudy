using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace testc_
{
    public class Unit
    {
        private int _val;

        public int Val => _val;

        public Unit(int val)
        {
            _val = val;
        }

    }

    public class UnitCompare : IComparable<UnitCompare>
    {
        private int priority = 0;

        public UnitCompare(int priority)
        {
            this.priority = priority;
        }

        public int CompareTo(UnitCompare? obj)
        {
            if(obj == null) return -1;

            return (priority > obj.priority) ? 0 : -1;
        }
    }

    public class PriorityQueue<Element, Priority> where Priority : IComparable<Priority>
    {
        private readonly List<(Element element, Priority priority)> _heap = new List<(Element element, Priority priority)>();

        public int Count => _heap.Count;

        public bool Enqueue(Element element, Priority priority)
        {
            _heap.Add((element, priority));

            var index = _heap.Count - 1;

            while (index > 0)
            {
                var parent = (index - 1) / 2;

                if (_heap[index].priority.CompareTo(_heap[parent].priority) >= 0)
                    return true;

                (_heap[parent], _heap[index]) = (_heap[index], _heap[parent]);
                index = parent;
            }

            return false;
        }

        public bool TryDequeue(out Element element, out Priority priority)
        {
            if (_heap.Count <= 0)
            {
                element = default;
                priority = default;
                return false;
            }

            element = _heap[0].Item1;
            priority = _heap[0].Item2;

            var lastElement = _heap[^1];
            _heap[0] = lastElement;
            _heap.RemoveAt(_heap.Count - 1);

            var index = 0;
            var count = _heap.Count;

            while (true)
            {
                var left = 2 * index + 1;
                var right = 2 * index + 2;
                var current = index;

                if (left < count && _heap[left].priority.CompareTo(_heap[current].priority) < 0)
                {
                    current = left;
                }
                if (right < count && _heap[right].priority.CompareTo(_heap[current].priority) < 0)
                {
                    current = right;
                }
                if (current == index)
                {
                    return true;
                }

                (_heap[index], _heap[current]) = (_heap[current], _heap[index]);
                index = current;
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            PriorityQueue<Unit, UnitCompare> priorityQueue = new PriorityQueue<Unit, UnitCompare>();

            priorityQueue.Enqueue(new Unit(1), new UnitCompare(3));
            priorityQueue.Enqueue(new Unit(5), new UnitCompare(4));
            priorityQueue.Enqueue(new Unit(7), new UnitCompare(3));
            priorityQueue.Enqueue(new Unit(2), new UnitCompare(2));
            priorityQueue.Enqueue(new Unit(100), new UnitCompare(1));

            while (priorityQueue.Count != 0)
            {
                if (priorityQueue.TryDequeue(out Unit element, out UnitCompare unitCompare))
                    Console.WriteLine(element.Val);
            }
        }
    }
}
