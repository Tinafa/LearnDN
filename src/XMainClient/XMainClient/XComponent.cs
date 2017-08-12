using System;
using System.Collections.Generic;

namespace XMainClient
{
    internal abstract class XComponent
    {
        public delegate bool XEventHandler(XEventArgs e);

        public static readonly uint uuID = 0;

        protected XObject _host = null;
        protected XEntity _entity = null;

        private bool _enabled = true;
        private bool _detached = false;

        private Dictionary<int, XEventHandler> _eventMap = new Dictionary<int, XEventHandler>();

        public abstract uint ID { get; }

        private XEventHandler FindEventHandler(int eventIndex)
        {
            if(_eventMap.ContainsKey(eventIndex))
            {
                return _eventMap[eventIndex];
            }
            return null;
        }

        protected void RegisterEvent(XEventDefine eventID, XEventHandler handler)
        {
            int eventIndex = EnumInt32ToInt.Convert<XEventDefine>(eventID);
            _eventMap[eventIndex] = handler;
        }

        protected virtual void EventSubscribe()
        {
        }

        protected void EventUnsubscribe()
        {
            _eventMap.Clear();
        }

        public virtual void OnAttachToHost(XObject host)
        {
            _host = host;
            _entity = _host as XEntity;
            EventSubscribe();
        }

        public virtual void OnDetachFromHost()
        {
            EventUnsubscribe();

            _host = null;
            _entity = null;
        }

    }
}
