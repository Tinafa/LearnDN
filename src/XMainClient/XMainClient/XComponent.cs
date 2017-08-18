using System;
using System.Collections.Generic;

namespace XMainClient
{
    public abstract class XComponent
    {
        public delegate bool XEventHandler(XEventArgs e);

        public static readonly uint uuID = 0;

        protected XObject _host = null;
        protected XEntity _entity = null;

        private bool _enabled = true;
        private bool _detached = false;

        private Dictionary<int, XEventHandler> _eventMap = new Dictionary<int, XEventHandler>();

        public abstract uint ID { get; }

        public bool Enabled
        {
            get { return _enabled && !_detached; }
            set { _enabled = value; }
        }

        public bool Detached
        {
            get { return _detached; }
            set { _detached = value; }
        }

        public virtual void Attached()
        { }

        public virtual void Update(float fDeltaT)
        {
            //nothing goes here
        }

        public virtual void PostUpdate(float fDeltaT)
        {
            //nothing goes here
        }

        public bool OnEvent(XEventArgs e)
        {
            int eventIndex = EnumInt32ToInt.Convert<XEventDefine>(e.ArgsDefine);
            XEventHandler func = FindEventHandler(eventIndex);
            if (func != null)
            {
                return func(e);
            }
            return false;
        }

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

        public virtual void OnEnterScene()
        {

        }

        public virtual void OnLeaveScene()
        {

        }


    }
}
