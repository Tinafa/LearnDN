using System;
using System.Collections.Generic;
using UnityEngine;

namespace XMainClient
{
    public abstract class XBaseObject : MonoBehaviour
    {
        public uint UID { get; set; }

        public delegate bool XEventHandler(XEventArgs e);
        private Dictionary<int, XEventHandler> _eventMap = new Dictionary<int, XEventHandler>();

        protected void RegisterEvent(XEventDefine eventID, XEventHandler handler)
        {
            int eventIndex = EnumInt32ToInt.Convert<XEventDefine>(eventID);
            _eventMap[eventIndex] = handler;
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
            if (_eventMap.ContainsKey(eventIndex))
            {
                return _eventMap[eventIndex];
            }
            return null;
        }
    }
}
