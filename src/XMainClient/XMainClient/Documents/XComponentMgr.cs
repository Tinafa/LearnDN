using System;
using System.Collections.Generic;
using XUtliPoolLib;

namespace XMainClient
{
    class XComponentMgr : XSingleton<XComponentMgr>
    {
        private Dictionary<uint, int> _slots = new Dictionary<uint, int>();

        private enum XComponentSet
        {
            MainHallDocoment = 0,
            BasementDocument = 1,
            MAX = 2,
        }
        public static int ComponenCreatetCount = 0;


        public XComponentMgr()
        {
            int n = EnumInt32ToInt.Convert<XComponentSet>(XComponentSet.MAX);

            for (int i = 0; i < n; i++)
                _slots.Add(XCommon.singleton.XHash(((XComponentSet)i).ToString()), i);
        }

        int count = 0;
        public XComponent CreateComponent(XObject hoster, uint uuid)
        {
            int slot = 0;
            if (_slots.TryGetValue(uuid, out slot))
            {
                XComponent c = ComponentFactory((XComponentSet)slot);
                if (hoster != null) hoster.AttachComponent(c);
                ComponenCreatetCount++;
                count++;
                return c;
            }
            else
            {
                XDebug.singleton.AddErrorLog("CreateComponent Failed",uuid.ToString());
                return null;
            }
        }

        public void RemoveComponent(XComponent c)
        {
            count--;
        }

        private XComponent ComponentFactory(XComponentSet slot)
        {
            switch(slot)
            {
                case XComponentSet.MainHallDocoment:return new XMainHallDocument();
                case XComponentSet.BasementDocument:return new XBasementDocument();
            }
            return null;
        }
    }
}
