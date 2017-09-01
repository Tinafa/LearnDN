using System;
using XUtliPoolLib;

namespace XMainClient
{
    public class XHeartComp : XComponent
    {
        public static new readonly uint uuID = XCommon.singleton.XHash("XHeartComp");
        public override uint ID { get { return uuID; } }

        private bool _isBeating = false;
        public bool IsBeating
        {
            get { return _isBeating; }
        }
    }
}
