using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using XUtliPoolLib;

namespace XMainClient
{
    class XLFUItem<T> : IComparable<XLFUItem<T>>
    {
        public T data;
        public uint frequent = 0;
        public int index;
        private int canPop; // -1: can pop; 1: can't pop

        public bool bCanPop
        {
            get { return canPop < 0; }
            set
            {
                canPop = value ? -1 : 1;
            }
        }

        public int CompareTo(XLFUItem<T> other)
        {
            if (canPop == other.canPop)
                return frequent.CompareTo(other.frequent);
            return canPop.CompareTo(other.canPop);
        }
    }

    class XLFU<T>
    {
        static readonly uint DURATION_COUNT = 16;
        List<XLFUItem<T>> m_Items = new List<XLFUItem<T>>();
        Dictionary<T, XLFUItem<T>> m_dicItems = new Dictionary<T, XLFUItem<T>>();

        int m_HeapSize = 0;

        int m_Size = 5;

        uint m_CurTotalCount = 0;

        public XLFU(int size)
        {
            if (size <= 0)
            {
                XDebug.singleton.AddErrorLog("size <= 0");
            }
            m_Size = size;
        }

        public T Add(T t)
        {
            _AdjustFrequent();

            XLFUItem<T> item = null;
            if (!m_dicItems.TryGetValue(t, out item))
            {
                if (m_HeapSize >= m_Items.Count)
                {
                    item = new XLFUItem<T>();
                    m_Items.Add(item);
                }
                else
                {
                    item = m_Items[m_HeapSize];
                }

                item.data = t;
                item.frequent = 0;
                m_dicItems.Add(t, item);
                item.index = m_HeapSize;
                ++m_HeapSize;

                _PercolateUp(item);
            }

            item.bCanPop = false;
            ++item.frequent;
            _PercolateDown(item);

            if (m_HeapSize > m_Size)
            {
                XLFUItem<T> top = m_Items[0];
                if (top.bCanPop)
                    return Pop();
            }

            return default(T);
        }

        public void MarkCanPop(T t, bool bCanPop)
        {
            XLFUItem<T> item = null;
            if (m_dicItems.TryGetValue(t, out item))
            {
                item.bCanPop = bCanPop;
                if (bCanPop)
                    _PercolateUp(item);
                else
                    _PercolateDown(item);
            }
        }

        public void Clear()
        {
            m_Items.Clear();
            m_dicItems.Clear();
            m_HeapSize = 0;
            m_CurTotalCount = 0;
        }

        public void Remove(T t)
        {
            XLFUItem<T> item = null;
            if (m_dicItems.TryGetValue(t, out item))
            {
                --m_HeapSize;

                int index = item.index;
                Swap(index, m_HeapSize);
                _PercolateDown(m_Items[index]);
                m_dicItems.Remove(m_Items[m_HeapSize].data);
            }
        }

        void _AdjustFrequent()
        {
            ++m_CurTotalCount;
            if (m_CurTotalCount >= DURATION_COUNT)
            {
                for (int i = 0; i < m_HeapSize; ++i)
                {
                    m_Items[i].frequent >>= 1;
                }
                m_CurTotalCount = 0;
            }
        }

        public T Pop()
        {
            if (m_HeapSize > 0)
            {
                --m_HeapSize;

                Swap(0, m_HeapSize);
                _PercolateDown(m_Items[0]);
                m_dicItems.Remove(m_Items[m_HeapSize].data);
                return m_Items[m_HeapSize].data;
            }
            return default(T);
        }

        public T Peek()
        {
            if (m_HeapSize > 0)
            {
                return m_Items[0].data;
            }
            else
                return default(T);
        }

        void Swap(int x, int y)
        {
            XLFUItem<T> t = m_Items[x];
            m_Items[x] = m_Items[y];
            m_Items[x].index = x;
            m_Items[y] = t;
            m_Items[y].index = y;
        }

        void _PercolateDown(XLFUItem<T> item)
        {
            int curIdx = item.index;
            int heapSize = m_HeapSize;

            while (curIdx < heapSize)
            {
                int left = 2 * curIdx + 1;
                int right = 2 * curIdx + 2;

                XLFUItem<T> smaller = m_Items[curIdx];
                int smallerIdx = curIdx;

                if (left < heapSize && m_Items[left].CompareTo(smaller) < 0)
                {
                    smaller = m_Items[left];
                    smallerIdx = left;
                }
                if (right < heapSize && m_Items[right].CompareTo(smaller) < 0)
                {
                    smaller = m_Items[right];
                    smallerIdx = right;
                }

                if (smallerIdx != curIdx)
                {
                    Swap(curIdx, smallerIdx);

                    curIdx = smallerIdx;
                }
                else
                    break;
            }
        }

        void _PercolateUp(XLFUItem<T> item)
        {
            int curIdx = item.index;
            while (curIdx > 0)
            {
                int parent = (curIdx - 1) / 2;

                XLFUItem<T> smaller = m_Items[parent];
                int smallerIdx = parent;

                if (parent >= 0 && m_Items[curIdx].CompareTo(smaller) < 0)
                {
                    smallerIdx = curIdx;
                }

                if (smallerIdx != parent)
                {
                    Swap(parent, smallerIdx);
                    curIdx = parent;
                }
                else
                    break;
            }
        }

    }
}
