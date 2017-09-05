using System;
using System.Collections.Generic;
using XUtliPoolLib;

namespace XMainClient
{
    public class XEventMgr : XSingleton<XEventMgr>
    {
        private List<EventPoolClear> m_eventPoolClearCb = new List<EventPoolClear>();

        public bool FireEvent(XEventArgs args)
        {
            bool bHandled = false;

            if (args.Firer != null)
                bHandled = args.Firer.DispatchEvent(args);

            if (!args.ManualRecycle)
                args.Recycle();

            return bHandled;
        }

        public void RegisterEventPool(EventPoolClear epc)
        {
            m_eventPoolClearCb.Add(epc);
        }

        public void Clear()
        {
            for (int i = 0; i < m_eventPoolClearCb.Count; ++i)
            {
                EventPoolClear epc = m_eventPoolClearCb[i];
                if (epc != null)
                {
                    epc();
                }
            }
            m_eventPoolClearCb.Clear();
        }
    }

    public delegate void EventPoolClear();
    public class XEventPool<T> where T : XEventArgs, new()
    {
        private static Queue<T> _pool = null;

        public static void Clear()
        {
            if(_pool != null)
            {
                _pool.Clear();
                _pool = null;
            }
        }

        public static T GetEvent()
        {
            if (_pool == null)
            {
                _pool = new Queue<T>();
                XEventMgr.singleton.RegisterEventPool(Clear);
            }
            
            if(_pool.Count>0){
                T t = _pool.Dequeue();
                t.Token = XCommon.singleton.UniqueToken;
                return t;
            }
            else
            {
                return new T();
            }
        }

        public static void Recycle(T args)
        {
            _pool.Enqueue(args);
        }
    }
}
