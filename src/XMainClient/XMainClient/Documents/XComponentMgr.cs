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

            XAttrComp,
            XBrainComp,
            XHeartComp,
            XEnergyComp,
            XStateMachine,
            XPresentationComp,
            XAudioComp,
            XActionComp,

            MAX,
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

        /// <summary>
        /// 不用反射优化吗0.0
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        private XComponent ComponentFactory(XComponentSet slot)
        {
            switch(slot)
            {
                case XComponentSet.MainHallDocoment:return new XMainHallDocument();
                case XComponentSet.BasementDocument:return new XBasementDocument();
                case XComponentSet.XAttrComp:return new XAttrComp();
                case XComponentSet.XBrainComp:return new XBrainComp();
                case XComponentSet.XHeartComp:return new XHeartComp();
                case XComponentSet.XEnergyComp:return new XEnergyComp();
                case XComponentSet.XStateMachine:return new XStateMachine();
                case XComponentSet.XPresentationComp:return new XPresentationComp();
                case XComponentSet.XAudioComp:return new XAudioComp();
                case XComponentSet.XActionComp:return new XActionComp();
            }
            return null;
        }
    }
}
