using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace XUtliPoolLib
{
    public interface IObjectPool
    {
        int countAll { get; }
        int countActive { get; }
        int countInActive { get; }
    }

    public class ObjectPoolCache
    {
        public static readonly List<IObjectPool> s_AllPool = new List<IObjectPool>();
        public static void Clear()
        {
            s_AllPool.Clear();
        }
    }

    class ObjectPool<T> : IObjectPool
    {
        public delegate T CreateObj();
        private readonly Stack<T> m_Stack = new Stack<T>();
        private readonly UnityAction<T> m_ActionOnGet;
        private readonly UnityAction<T> m_ActionOnRelease;
        private CreateObj m_objCreator = null;

        public int countAll { get; }
        public int countActive { get; }
        public int countInActive { get; }

        public ObjectPool(CreateObj creator, UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
        {
            m_objCreator = creator;
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
            ObjectPoolCache.s_AllPool.Add(this);
        }

        public T Get()
        {
            T element;
            if (m_Stack.Count == 0)
            {
                element = m_objCreator();
                m_Stack.Push(element);
            }
            else
            {
                element = m_Stack.Pop();
            }
            if (m_ActionOnGet != null)
                m_ActionOnGet(element);
            return element;
        }

        public void Release(T element)
        {
            if(element != null)
            {
                if (m_ActionOnRelease != null)
                    m_ActionOnRelease(element);
                m_Stack.Push(element);
            }
        }
    }

    public class CommonObjectPool<T> where T : new()
    {
        private static readonly ObjectPool<System.Object> s_Pool = new ObjectPool<System.Object>(Create, null, null); 

        public static System.Object Create()
        {
            return new T();
        }

        public static T Get()
        {
            return (T)s_Pool.Get();
        }

        public static void Release(T toRelease)
        {
            s_Pool.Release(toRelease);
        }
    }

    public class ListPool<T>
    {
        // Object pool to avoid allocations.
        private static readonly ObjectPool<List<T>> s_Pool = new ObjectPool<List<T>>(Create, l => l.Clear(), l => l.Clear());

        public static List<T> Create()
        {
            return new List<T>();
        }
        public static List<T> Get()
        {
            return s_Pool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            s_Pool.Release(toRelease);
        }
    }
}
